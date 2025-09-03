using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;
using SistemaPDV.Infrastructure.Services;
using SistemaPDV.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace SistemaPDV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly PDVDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AuthService authService,
            PDVDbContext context,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _context = context;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            try
            {
                _logger.LogInformation("Tentativa de registro para email: {Email}", dto.Email);

                // Verificar se o email já existe
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (existingUser != null)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Email já está em uso"
                    });
                }

                // Verificar se o restaurante existe
                var restaurante = await _context.Restaurantes.FindAsync(dto.RestauranteId);
                if (restaurante == null)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Restaurante não encontrado"
                    });
                }

                // Criar hash da senha
                var passwordHash = HashPassword(dto.Password);

                // Criar usuário
                var user = new ApplicationUser
                {
                    Email = dto.Email,
                    PasswordHash = passwordHash,
                    Nome = dto.Nome,
                    RestauranteId = dto.RestauranteId,
                    DataCriacao = DateTime.UtcNow,
                    Ativo = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuário criado com sucesso: {Email}", dto.Email);

                return Ok(new AuthResponseDto
                {
                    Sucesso = true,
                    Erro = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no registro");
                return StatusCode(500, new AuthResponseDto
                {
                    Sucesso = false,
                    Erro = "Erro interno do servidor"
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            try
            {
                _logger.LogInformation("Tentativa de login para email: {Email}", dto.Email);

                var user = await _context.Users
                    .Include(u => u.Restaurante)
                    .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Ativo);
                
                if (user == null)
                {
                    return Unauthorized(new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Email ou senha inválidos"
                    });
                }

                // Verificar senha
                if (!VerifyPassword(dto.Password, user.PasswordHash))
                {
                    return Unauthorized(new AuthResponseDto
                    {
                        Sucesso = false,
                        Erro = "Email ou senha inválidos"
                    });
                }

                // Gerar token JWT
                var token = _authService.GenerateJwtToken(user);

                _logger.LogInformation("Login realizado com sucesso para: {Email}", dto.Email);

                return Ok(new AuthResponseDto
                {
                    Sucesso = true,
                    Token = token,
                    Nome = user.Nome ?? user.Email,
                    RestauranteId = user.RestauranteId,
                    ExpiraEm = DateTime.UtcNow.AddHours(24)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no login");
                return StatusCode(500, new AuthResponseDto
                {
                    Sucesso = false,
                    Erro = "Erro interno do servidor"
                });
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "SistemaPDV_Salt"));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var computedHash = HashPassword(password);
            return computedHash == hash;
        }
    }
}
