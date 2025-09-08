using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;
using SistemaPDV.Infrastructure.Data;
using System.Net;

namespace SistemaPDV.Infrastructure.Services
{
    /// <summary>
    /// Serviço principal de autenticação e autorização
    /// Implementa funcionalidades modernas de segurança, auditoria e gestão de usuários
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly PDVDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;
        private readonly ICurrentUserService _currentUserService;

        // Configurações de segurança
        private const int MaxLoginAttempts = 5;
        private const int LockoutMinutes = 30;
        private const int LoginHistoryDays = 90;

        public AuthService(
            PDVDbContext context,
            IPasswordService passwordService,
            ITokenService tokenService,
            ILogger<AuthService> logger,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Realiza login do usuário com validações de segurança
        /// </summary>
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var userAgent = _currentUserService.GetUserAgent();
            var ipAddress = _currentUserService.GetUserIpAddress();

            try
            {
                _logger.LogInformation("Tentativa de login para email {Email} do IP {IpAddress}", 
                    loginDto.Email, ipAddress);

                // Verificar se a conta está bloqueada
                if (await IsAccountLockedAsync(loginDto.Email))
                {
                    await RecordLoginAttemptAsync(loginDto.Email, null, false, 
                        "Conta bloqueada por múltiplas tentativas", ipAddress, userAgent);

                    return new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Conta temporariamente bloqueada devido a múltiplas tentativas de login. Tente novamente em 30 minutos."
                    };
                }

                // Buscar usuário
                var user = await _context.ApplicationUsers
                    .AsNoTracking()
                    .Include(u => u.Restaurante)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

                if (user == null)
                {
                    await RecordLoginAttemptAsync(loginDto.Email, null, false, 
                        "Usuário não encontrado", ipAddress, userAgent);

                    _logger.LogWarning("Tentativa de login com email inexistente: {Email}", loginDto.Email);
                    
                    return new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Email ou senha inválidos"
                    };
                }

                // Verificar senha
                if (!_passwordService.VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    await RecordLoginAttemptAsync(loginDto.Email, user.Id, false, 
                        "Senha incorreta", ipAddress, userAgent);

                    _logger.LogWarning("Senha incorreta para usuário {UserId}", user.Id);
                    
                    return new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Email ou senha inválidos"
                    };
                }

                // Verificar se o usuário está ativo
                if (!user.IsActive)
                {
                    await RecordLoginAttemptAsync(loginDto.Email, user.Id, false, 
                        "Usuário inativo", ipAddress, userAgent);

                    return new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Usuário inativo. Contate o administrador."
                    };
                }

                // Verificar se o restaurante está ativo
                if (user.Restaurante != null && !user.Restaurante.IsActive)
                {
                    await RecordLoginAttemptAsync(loginDto.Email, user.Id, false, 
                        "Restaurante inativo", ipAddress, userAgent);

                    return new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Restaurante inativo. Contate o suporte."
                    };
                }

                // Gerar tokens
                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

                // Atualizar último login
                user.UltimoLogin = DateTime.UtcNow;
                _context.ApplicationUsers.Update(user);

                // Registrar tentativa bem-sucedida
                await RecordLoginAttemptAsync(loginDto.Email, user.Id, true, 
                    "Login bem-sucedido", ipAddress, userAgent);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Login bem-sucedido para usuário {UserId}", user.Id);

                return new AuthResponseDto
                {
                    Sucesso = true,
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Nome = user.Nome,
                    UserId = user.Id,
                    RestauranteId = user.RestauranteId,
                    ExpiraEm = DateTime.UtcNow.AddHours(1),
                    RefreshExpiraEm = DateTime.UtcNow.AddDays(30),
                    Permissions = GetUserPermissions(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante login para email {Email}", loginDto.Email);
                
                await RecordLoginAttemptAsync(loginDto.Email, null, false, 
                    "Erro interno", ipAddress, userAgent);

                return new AuthResponseDto
                {
                    Sucesso = false,
                    Erro = "Erro interno do servidor. Tente novamente."
                };
            }
        }

        /// <summary>
        /// Registra novo usuário para restaurante existente
        /// </summary>
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                _logger.LogInformation("Tentativa de registro de usuário para restaurante {RestauranteId}", 
                    registerDto.RestauranteId);

                // Validar força da senha
                var passwordValidation = _passwordService.ValidatePasswordStrength(registerDto.Password);
                if (!passwordValidation.IsValid)
                {
                    return new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = $"Senha não atende aos critérios de segurança: {string.Join(", ", passwordValidation.Errors)}"
                    };
                }

                // Verificar se email já existe
                var existingUser = await _context.ApplicationUsers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == registerDto.Email.ToLower());

                if (existingUser != null)
                {
                    return new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Email já está em uso"
                    };
                }

                // Verificar se restaurante existe
                var restaurante = await _context.Restaurantes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == registerDto.RestauranteId);

                if (restaurante == null)
                {
                    return new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Restaurante não encontrado"
                    };
                }

                // Verificar se senha foi comprometida
                if (await _passwordService.IsPasswordComprommisedAsync(registerDto.Password))
                {
                    return new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Senha foi comprometida em vazamentos de dados. Escolha uma senha diferente."
                    };
                }

                // Criar usuário
                var passwordHash = _passwordService.HashPassword(registerDto.Password);
                
                var user = new ApplicationUser
                {
                    Email = registerDto.Email.ToLower(),
                    Nome = registerDto.Nome,
                    PasswordHash = passwordHash,
                    RestauranteId = registerDto.RestauranteId,
                    IsActive = true,
                    IsAdmin = false,
                    CriadoEm = DateTime.UtcNow
                };

                _context.ApplicationUsers.Add(user);
                await _context.SaveChangesAsync();

                // Gerar tokens
                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

                await transaction.CommitAsync();

                _logger.LogInformation("Usuário {UserId} registrado com sucesso para restaurante {RestauranteId}", 
                    user.Id, registerDto.RestauranteId);

                return new AuthResponseDto
                {
                    Sucesso = true,
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Nome = user.Nome,
                    UserId = user.Id,
                    RestauranteId = user.RestauranteId,
                    ExpiraEm = DateTime.UtcNow.AddHours(1),
                    RefreshExpiraEm = DateTime.UtcNow.AddDays(30),
                    Permissions = GetUserPermissions(user)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro durante registro de usuário");
                
                return new AuthResponseDto
                {
                    Sucesso = false,
                    Erro = "Erro interno do servidor. Tente novamente."
                };
            }
        }

        /// <summary>
        /// Registro completo: restaurante + usuário administrador
        /// </summary>
        public async Task<AuthResponseDto> RegisterCompleteAsync(RegistroCompletoDto registroCompletoDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                _logger.LogInformation("Tentativa de registro completo para restaurante {Nome}", 
                    registroCompletoDto.Restaurante.Nome);

                // Validar força da senha
                var passwordValidation = _passwordService.ValidatePasswordStrength(registroCompletoDto.Usuario.Password);
                if (!passwordValidation.IsValid)
                {
                    return new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = $"Senha não atende aos critérios de segurança: {string.Join(", ", passwordValidation.Errors)}"
                    };
                }

                // Verificar se email já existe
                var existingUser = await _context.ApplicationUsers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == registroCompletoDto.Usuario.Email.ToLower());

                if (existingUser != null)
                {
                    return new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Email já está em uso"
                    };
                }

                // Verificar se CNPJ já existe (se fornecido)
                if (!string.IsNullOrWhiteSpace(registroCompletoDto.Restaurante.CNPJ))
                {
                    var existingRestaurante = await _context.Restaurantes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(r => r.CNPJ == registroCompletoDto.Restaurante.CNPJ);

                    if (existingRestaurante != null)
                    {
                        return new AuthResponseDto
                        {
                            Sucesso = false,
                            Erro = "CNPJ já está em uso"
                        };
                    }
                }

                // Verificar se senha foi comprometida
                if (await _passwordService.IsPasswordComprommisedAsync(registroCompletoDto.Usuario.Password))
                {
                    return new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Senha foi comprometida em vazamentos de dados. Escolha uma senha diferente."
                    };
                }

                // Criar restaurante
                var restaurante = new Restaurante
                {
                    Nome = registroCompletoDto.Restaurante.Nome,
                    CNPJ = registroCompletoDto.Restaurante.CNPJ,
                    Telefone = registroCompletoDto.Restaurante.Telefone,
                    Email = registroCompletoDto.Restaurante.Email,
                    Endereco = registroCompletoDto.Restaurante.Endereco,
                    Numero = registroCompletoDto.Restaurante.Numero,
                    Complemento = registroCompletoDto.Restaurante.Complemento,
                    Bairro = registroCompletoDto.Restaurante.Bairro,
                    Cidade = registroCompletoDto.Restaurante.Cidade,
                    UF = registroCompletoDto.Restaurante.UF,
                    CEP = registroCompletoDto.Restaurante.CEP,
                    IsActive = true,
                    CriadoEm = DateTime.UtcNow
                };

                _context.Restaurantes.Add(restaurante);
                await _context.SaveChangesAsync();

                // Criar usuário administrador
                var passwordHash = _passwordService.HashPassword(registroCompletoDto.Usuario.Password);
                
                var user = new ApplicationUser
                {
                    Email = registroCompletoDto.Usuario.Email.ToLower(),
                    Nome = registroCompletoDto.Usuario.Nome,
                    PasswordHash = passwordHash,
                    RestauranteId = restaurante.Id,
                    IsActive = true,
                    IsAdmin = true, // Primeiro usuário é admin
                    CriadoEm = DateTime.UtcNow
                };

                _context.ApplicationUsers.Add(user);
                await _context.SaveChangesAsync();

                // Gerar tokens
                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

                await transaction.CommitAsync();

                _logger.LogInformation("Registro completo realizado: Restaurante {RestauranteId}, Usuário Admin {UserId}", 
                    restaurante.Id, user.Id);

                return new AuthResponseDto
                {
                    Sucesso = true,
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Nome = user.Nome,
                    UserId = user.Id,
                    RestauranteId = user.RestauranteId,
                    ExpiraEm = DateTime.UtcNow.AddHours(1),
                    RefreshExpiraEm = DateTime.UtcNow.AddDays(30),
                    Permissions = GetUserPermissions(user)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro durante registro completo");
                
                return new AuthResponseDto
                {
                    Sucesso = false,
                    Erro = "Erro interno do servidor. Tente novamente."
                };
            }
        }

        /// <summary>
        /// Altera senha do usuário atual
        /// </summary>
        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = _currentUserService.GetUserId();
                if (!userId.HasValue)
                {
                    _logger.LogWarning("Tentativa de alteração de senha sem usuário autenticado");
                    return false;
                }

                var user = await _context.ApplicationUsers
                    .FirstOrDefaultAsync(u => u.Id == userId.Value);

                if (user == null)
                {
                    _logger.LogWarning("Usuário {UserId} não encontrado para alteração de senha", userId);
                    return false;
                }

                // Verificar senha atual
                if (!_passwordService.VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
                {
                    _logger.LogWarning("Senha atual incorreta para usuário {UserId}", userId);
                    return false;
                }

                // Validar nova senha
                var passwordValidation = _passwordService.ValidatePasswordStrength(changePasswordDto.NewPassword);
                if (!passwordValidation.IsValid)
                {
                    _logger.LogWarning("Nova senha não atende critérios para usuário {UserId}: {Errors}", 
                        userId, string.Join(", ", passwordValidation.Errors));
                    return false;
                }

                // Verificar se a nova senha é diferente da atual
                if (_passwordService.VerifyPassword(changePasswordDto.NewPassword, user.PasswordHash))
                {
                    _logger.LogWarning("Nova senha é igual à atual para usuário {UserId}", userId);
                    return false;
                }

                // Verificar se senha foi comprometida
                if (await _passwordService.IsPasswordComprommisedAsync(changePasswordDto.NewPassword))
                {
                    _logger.LogWarning("Nova senha comprometida para usuário {UserId}", userId);
                    return false;
                }

                // Atualizar senha
                user.PasswordHash = _passwordService.HashPassword(changePasswordDto.NewPassword);
                user.ModificadoEm = DateTime.UtcNow;

                // Revogar todos os refresh tokens por segurança
                await _tokenService.RevokeAllRefreshTokensAsync(userId.Value);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Senha alterada com sucesso para usuário {UserId}", userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar senha");
                return false;
            }
        }

        /// <summary>
        /// Valida se o token JWT é válido
        /// </summary>
        public async Task<ApplicationUser?> ValidateTokenAsync(string token)
        {
            try
            {
                var principal = _tokenService.ValidateToken(token);
                if (principal == null)
                    return null;

                var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return null;

                var user = await _context.ApplicationUsers
                    .AsNoTracking()
                    .Include(u => u.Restaurante)
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar token");
                return null;
            }
        }

        /// <summary>
        /// Gera novo token de acesso usando refresh token
        /// </summary>
        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                _logger.LogDebug("Tentativa de refresh token");

                var userId = await _tokenService.ValidateRefreshTokenAsync(refreshToken);
                if (!userId.HasValue)
                {
                    return new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Refresh token inválido ou expirado"
                    };
                }

                var user = await _context.ApplicationUsers
                    .AsNoTracking()
                    .Include(u => u.Restaurante)
                    .FirstOrDefaultAsync(u => u.Id == userId.Value && u.IsActive);

                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Usuário não encontrado ou inativo"
                    };
                }

                // Revogar o refresh token usado
                await _tokenService.RevokeRefreshTokenAsync(refreshToken);

                // Gerar novos tokens
                var newAccessToken = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

                _logger.LogInformation("Tokens renovados para usuário {UserId}", user.Id);

                return new AuthResponseDto
                {
                    Sucesso = true,
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    Nome = user.Nome,
                    UserId = user.Id,
                    RestauranteId = user.RestauranteId,
                    ExpiraEm = DateTime.UtcNow.AddHours(1),
                    RefreshExpiraEm = DateTime.UtcNow.AddDays(30),
                    Permissions = GetUserPermissions(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante refresh token");
                
                return new AuthResponseDto
                {
                    Sucesso = false,
                    Erro = "Erro interno do servidor"
                };
            }
        }

        /// <summary>
        /// Revoga todos os tokens do usuário (logout completo)
        /// </summary>
        public async Task<bool> RevokeAllTokensAsync(int userId)
        {
            try
            {
                var success = await _tokenService.RevokeAllRefreshTokensAsync(userId);
                
                _logger.LogInformation("Logout completo realizado para usuário {UserId}", userId);
                
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar logout completo para usuário {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Verifica se a conta está bloqueada por tentativas de login
        /// </summary>
        public async Task<bool> IsAccountLockedAsync(string email)
        {
            try
            {
                var cutoffTime = DateTime.UtcNow.AddMinutes(-LockoutMinutes);
                
                var recentFailedAttempts = await _context.LoginAttempts
                    .AsNoTracking()
                    .Where(la => la.Email.ToLower() == email.ToLower() 
                              && !la.Success 
                              && la.AttemptedAt > cutoffTime)
                    .CountAsync();

                var isLocked = recentFailedAttempts >= MaxLoginAttempts;

                if (isLocked)
                {
                    _logger.LogWarning("Conta bloqueada para email {Email}. Tentativas falhadas: {Attempts}", 
                        email, recentFailedAttempts);
                }

                return isLocked;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar bloqueio da conta para email {Email}", email);
                return false;
            }
        }

        /// <summary>
        /// Obtém histórico de tentativas de login
        /// </summary>
        public async Task<List<LoginAttemptDto>> GetLoginHistoryAsync(int userId, int take = 10)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-LoginHistoryDays);

                var attempts = await _context.LoginAttempts
                    .AsNoTracking()
                    .Where(la => la.UserId == userId && la.AttemptedAt > cutoffDate)
                    .OrderByDescending(la => la.AttemptedAt)
                    .Take(take)
                    .Select(la => new LoginAttemptDto
                    {
                        Id = la.Id,
                        UserId = la.UserId ?? 0,
                        Email = la.Email,
                        Success = la.Success,
                        FailureReason = la.FailureReason,
                        IpAddress = la.IpAddress,
                        UserAgent = la.UserAgent ?? "",
                        AttemptedAt = la.AttemptedAt,
                        Location = la.Location
                    })
                    .ToListAsync();

                return attempts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter histórico de login para usuário {UserId}", userId);
                return new List<LoginAttemptDto>();
            }
        }

        #region Métodos Privados

        /// <summary>
        /// Registra tentativa de login para auditoria
        /// </summary>
        private async Task RecordLoginAttemptAsync(string email, int? userId, bool success, 
            string? failureReason, string ipAddress, string? userAgent)
        {
            try
            {
                var loginAttempt = new LoginAttempt
                {
                    Email = email,
                    UserId = userId,
                    Success = success,
                    FailureReason = failureReason,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    AttemptedAt = DateTime.UtcNow
                };

                _context.LoginAttempts.Add(loginAttempt);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar tentativa de login");
            }
        }

        /// <summary>
        /// Obtém permissões do usuário
        /// </summary>
        private static List<string> GetUserPermissions(ApplicationUser user)
        {
            var permissions = new List<string> { "user" };

            if (user.IsAdmin)
            {
                permissions.AddRange(new[] { "admin", "users:manage", "reports:view" });
            }

            return permissions;
        }

        #endregion
    }
}
