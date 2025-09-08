using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;
using SistemaPDV.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SistemaPDV.Infrastructure.Services
{
    /// <summary>
    /// Serviço para gerenciamento avançado de tokens JWT e refresh tokens
    /// Implementa funcionalidades modernas de segurança e renovação de tokens
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;
        private readonly PDVDbContext _context;

        // Configurações de token
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _accessTokenExpirationMinutes = 60; // 1 hora
        private readonly int _refreshTokenExpirationDays = 30; // 30 dias

        public TokenService(
            IConfiguration configuration,
            ILogger<TokenService> logger,
            PDVDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;

            _jwtKey = _configuration["Jwt:Key"] ?? "ChaveSecretaMuitoSeguraParaJWT123456789ChaveSecretaMuitoSeguraParaJWT123456789";
            _jwtIssuer = _configuration["Jwt:Issuer"] ?? "SistemaPDV";
            _jwtAudience = _configuration["Jwt:Audience"] ?? "SistemaPDV-Client";

            // Configurar timeouts do configuration se disponível
            if (int.TryParse(_configuration["Jwt:AccessTokenExpirationMinutes"], out var accessMinutes))
                _accessTokenExpirationMinutes = accessMinutes;

            if (int.TryParse(_configuration["Jwt:RefreshTokenExpirationDays"], out var refreshDays))
                _refreshTokenExpirationDays = refreshDays;
        }

        /// <summary>
        /// Gera token JWT para o usuário autenticado
        /// </summary>
        public string GenerateAccessToken(ApplicationUser user)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(_jwtKey);
                var expiresAt = DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes);

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Name, user.Nome),
                    new("RestauranteId", user.RestauranteId.ToString()),
                    new("jti", Guid.NewGuid().ToString()), // JWT ID para rastreamento
                    new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                };

                // Adicionar roles/permissions se necessário
                if (user.IsAdmin)
                {
                    claims.Add(new(ClaimTypes.Role, "Admin"));
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = expiresAt,
                    Issuer = _jwtIssuer,
                    Audience = _jwtAudience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                _logger.LogInformation("Token JWT gerado para usuário {UserId}, expira em {ExpiresAt}",
                    user.Id, expiresAt);

                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar token JWT para usuário {UserId}", user.Id);
                throw new InvalidOperationException("Erro interno ao gerar token de acesso", ex);
            }
        }

        /// <summary>
        /// Gera token de refresh para renovação
        /// </summary>
        public async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            try
            {
                // Gerar token criptograficamente seguro
                var randomBytes = new byte[64];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomBytes);
                var refreshToken = Convert.ToBase64String(randomBytes);

                var expiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

                // Salvar no banco de dados
                var tokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = userId,
                    ExpiresAt = expiresAt,
                    CreatedAt = DateTime.UtcNow
                };

                _context.RefreshTokens.Add(tokenEntity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Refresh token gerado para usuário {UserId}, expira em {ExpiresAt}",
                    userId, expiresAt);

                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar refresh token para usuário {UserId}", userId);
                throw new InvalidOperationException("Erro interno ao gerar refresh token", ex);
            }
        }

        /// <summary>
        /// Valida token JWT e retorna claims
        /// </summary>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                _logger.LogDebug("Token JWT validado com sucesso");

                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogDebug("Token JWT expirado");
                return null;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Token JWT inválido");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar token JWT");
                return null;
            }
        }

        /// <summary>
        /// Valida refresh token
        /// </summary>
        public async Task<int?> ValidateRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var tokenEntity = await _context.RefreshTokens
                    .AsNoTracking()
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

                if (tokenEntity == null)
                {
                    _logger.LogWarning("Refresh token não encontrado");
                    return null;
                }

                if (!tokenEntity.IsActive)
                {
                    _logger.LogWarning("Refresh token inativo (expirado ou revogado) para usuário {UserId}",
                        tokenEntity.UserId);
                    return null;
                }

                _logger.LogDebug("Refresh token validado para usuário {UserId}", tokenEntity.UserId);

                return tokenEntity.UserId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar refresh token");
                return null;
            }
        }

        /// <summary>
        /// Revoga refresh token específico
        /// </summary>
        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var tokenEntity = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

                if (tokenEntity == null)
                {
                    _logger.LogWarning("Tentativa de revogar refresh token inexistente");
                    return false;
                }

                tokenEntity.RevokedAt = DateTime.UtcNow;
                tokenEntity.RevokedReason = "Revogado manualmente";

                await _context.SaveChangesAsync();

                _logger.LogInformation("Refresh token revogado para usuário {UserId}", tokenEntity.UserId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao revogar refresh token");
                return false;
            }
        }

        /// <summary>
        /// Revoga todos os refresh tokens do usuário
        /// </summary>
        public async Task<bool> RevokeAllRefreshTokensAsync(int userId)
        {
            try
            {
                var activeTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                    .ToListAsync();

                foreach (var token in activeTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                    token.RevokedReason = "Logout completo";
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Todos os refresh tokens revogados para usuário {UserId}. Total: {Count}",
                    userId, activeTokens.Count);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao revogar todos os refresh tokens do usuário {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Obtém tempo restante do token
        /// </summary>
        public TimeSpan? GetRemainingTokenTime(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var expirationTime = jwtToken.ValidTo;
                var remainingTime = expirationTime - DateTime.UtcNow;

                return remainingTime > TimeSpan.Zero ? remainingTime : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tempo restante do token");
                return null;
            }
        }

        /// <summary>
        /// Extrai ID do usuário do token
        /// </summary>
        public int? GetUserIdFromToken(string token)
        {
            try
            {
                var principal = ValidateToken(token);
                var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(userIdClaim, out var userId))
                {
                    return userId;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair ID do usuário do token");
                return null;
            }
        }
    }
}
