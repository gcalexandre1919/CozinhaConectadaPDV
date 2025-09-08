using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;
using SistemaPDV.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace SistemaPDV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly PDVDbContext _context;
        private readonly ILogger<SeedController> _logger;
        private readonly IPasswordService _passwordService;

        public SeedController(PDVDbContext context, ILogger<SeedController> logger, IPasswordService passwordService)
        {
            _context = context;
            _logger = logger;
            _passwordService = passwordService;
        }

        [HttpPost("criar-dados-iniciais")]
        public async Task<ActionResult> CriarDadosIniciais()
        {
            try
            {
                // Verificar se já existe um restaurante
                var restauranteExistente = await _context.Restaurantes.AnyAsync();
                if (restauranteExistente)
                {
                    return Ok(new { mensagem = "Dados já existem no sistema" });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Criar restaurante padrão
                    var restaurante = new Restaurante
                    {
                        Nome = "Restaurante Demo",
                        CNPJ = "12.345.678/0001-90",
                        Telefone = "(11) 99999-9999",
                        Email = "contato@restaurante.com",
                        Endereco = "Rua Principal",
                        Numero = "123",
                        Bairro = "Centro",
                        Cidade = "São Paulo",
                        UF = "SP",
                        CEP = "01000-000",
                        DataCadastro = DateTime.UtcNow,
                        Ativo = true
                    };

                    _context.Restaurantes.Add(restaurante);
                    await _context.SaveChangesAsync();

                    // Criar usuário administrador
                    var passwordHash = _passwordService.HashPassword("123456");
                    var user = new ApplicationUser
                    {
                        Email = "admin@sistema.com",
                        PasswordHash = passwordHash,
                        Nome = "Administrador",
                        RestauranteId = restaurante.Id,
                        CriadoEm = DateTime.UtcNow,
                        IsActive = true
                    };

                    _context.ApplicationUsers.Add(user);

                    // Criar algumas categorias
                    var categorias = new List<Categoria>
                    {
                        new() { Nome = "Pratos Principais", Descricao = "Pratos principais do restaurante" },
                        new() { Nome = "Bebidas", Descricao = "Bebidas e sucos" },
                        new() { Nome = "Sobremesas", Descricao = "Doces e sobremesas" }
                    };

                    _context.Categorias.AddRange(categorias);
                    await _context.SaveChangesAsync();

                    // Criar alguns produtos
                    var produtos = new List<Produto>
                    {
                        new() 
                        { 
                            Nome = "Hambúrguer Especial", 
                            Descricao = "Hambúrguer com carne, queijo e salada",
                            Preco = 25.90m,
                            CategoriaId = categorias[0].Id,
                            Ativo = true
                        },
                        new() 
                        { 
                            Nome = "Refrigerante", 
                            Descricao = "Refrigerante 350ml",
                            Preco = 4.50m,
                            CategoriaId = categorias[1].Id,
                            Ativo = true
                        },
                        new() 
                        { 
                            Nome = "Pudim", 
                            Descricao = "Pudim de leite condensado",
                            Preco = 8.90m,
                            CategoriaId = categorias[2].Id,
                            Ativo = true
                        }
                    };

                    _context.Produtos.AddRange(produtos);

                    // Criar um cliente exemplo
                    var cliente = new Cliente
                    {
                        Nome = "Cliente Exemplo",
                        Email = "cliente@email.com",
                        Telefone = "(11) 88888-8888",
                        RestauranteId = restaurante.Id,
                        DataCadastro = DateTime.UtcNow,
                        Ativo = true
                    };

                    _context.Clientes.Add(cliente);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    _logger.LogInformation("Dados iniciais criados com sucesso");

                    return Ok(new 
                    { 
                        mensagem = "Dados iniciais criados com sucesso!",
                        login = new 
                        {
                            email = "admin@sistema.com",
                            senha = "123456"
                        }
                    });
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar dados iniciais");
                return StatusCode(500, new { erro = "Erro interno do servidor" });
            }
        }

        [HttpGet("verificar-usuario")]
        public async Task<ActionResult> VerificarUsuario(string email)
        {
            try
            {
                var usuario = await _context.ApplicationUsers
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (usuario == null)
                {
                    return Ok(new { existe = false, mensagem = "Usuário não encontrado" });
                }

                return Ok(new 
                { 
                    existe = true, 
                    nome = usuario.Nome, 
                    email = usuario.Email,
                    hashLength = usuario.PasswordHash.Length,
                    restauranteId = usuario.RestauranteId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar usuário");
                return StatusCode(500, new { erro = "Erro interno" });
            }
        }
    }
}