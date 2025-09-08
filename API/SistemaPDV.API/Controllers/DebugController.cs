using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPDV.Infrastructure.Data;

namespace SistemaPDV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly PDVDbContext _context;

        public DebugController(PDVDbContext context)
        {
            _context = context;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.ApplicationUsers
                .Select(u => new { u.Id, u.Email, u.Nome, u.RestauranteId, u.IsActive })
                .ToListAsync();
            
            return Ok(new { 
                TotalUsers = users.Count,
                Users = users
            });
        }

        [HttpGet("restaurants")]
        public async Task<IActionResult> GetRestaurants()
        {
            var restaurants = await _context.Restaurantes
                .Select(r => new { r.Id, r.Nome, r.Ativo })
                .ToListAsync();
            
            return Ok(new { 
                TotalRestaurants = restaurants.Count,
                Restaurants = restaurants
            });
        }

        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin()
        {
            try
            {
                // Verificar se admin já existe
                var existingAdmin = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == "admin@sistema.com");
                if (existingAdmin != null)
                {
                    return Ok(new { message = "Admin já existe", user = new { existingAdmin.Email, existingAdmin.Nome } });
                }

                // Criar restaurante se não existir
                var restaurant = await _context.Restaurantes.FirstOrDefaultAsync();
                if (restaurant == null)
                {
                    restaurant = new SistemaPDV.Core.Entities.Restaurante
                    {
                        Nome = "Restaurante Padrão",
                        IsActive = true,
                        CriadoEm = DateTime.UtcNow
                    };
                    _context.Restaurantes.Add(restaurant);
                    await _context.SaveChangesAsync();
                }

                // Criar usuário admin
                var passwordHash = HashPassword("123456");
                var adminUser = new SistemaPDV.Core.Entities.ApplicationUser
                {
                    Email = "admin@sistema.com",
                    PasswordHash = passwordHash,
                    Nome = "Administrador",
                    RestauranteId = restaurant.Id,
                    CriadoEm = DateTime.UtcNow,
                    IsActive = true
                };

                _context.ApplicationUsers.Add(adminUser);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Admin criado com sucesso!", user = new { adminUser.Email, adminUser.Nome } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + "SistemaPDV_Salt"));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
