using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPDV.Core.Entities;
using SistemaPDV.Infrastructure.Data;

namespace SistemaPDV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantesController : ControllerBase
    {
        private readonly PDVDbContext _context;
        private readonly ILogger<RestaurantesController> _logger;

        public RestaurantesController(PDVDbContext context, ILogger<RestaurantesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obter todos os restaurantes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Restaurante>>> ObterTodos()
        {
            try
            {
                var restaurantes = await _context.Restaurantes
                    .Where(r => r.Ativo)
                    .OrderBy(r => r.Nome)
                    .ToListAsync();

                return Ok(restaurantes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter restaurantes");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter restaurante por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurante>> ObterPorId(int id)
        {
            try
            {
                var restaurante = await _context.Restaurantes.FindAsync(id);
                if (restaurante == null)
                    return NotFound($"Restaurante com ID {id} não encontrado");

                return Ok(restaurante);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter restaurante {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Criar novo restaurante
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Restaurante>> Criar([FromBody] Restaurante restaurante)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Verificar se já existe restaurante com o mesmo nome
                var existeNome = await _context.Restaurantes
                    .AnyAsync(r => r.Nome.ToLower() == restaurante.Nome.ToLower() && r.Ativo);
                
                if (existeNome)
                    return BadRequest("Já existe um restaurante com este nome");

                restaurante.DataCadastro = DateTime.Now;
                restaurante.Ativo = true;

                _context.Restaurantes.Add(restaurante);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(ObterPorId), new { id = restaurante.Id }, restaurante);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar restaurante");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Atualizar restaurante
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Restaurante>> Atualizar(int id, [FromBody] Restaurante restaurante)
        {
            try
            {
                if (id != restaurante.Id)
                    return BadRequest("ID do restaurante não confere");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var restauranteExistente = await _context.Restaurantes.FindAsync(id);
                if (restauranteExistente == null)
                    return NotFound($"Restaurante com ID {id} não encontrado");

                // Verificar se já existe outro restaurante com o mesmo nome
                var existeOutroNome = await _context.Restaurantes
                    .AnyAsync(r => r.Nome.ToLower() == restaurante.Nome.ToLower() && r.Id != id && r.Ativo);
                
                if (existeOutroNome)
                    return BadRequest("Já existe outro restaurante com este nome");

                restauranteExistente.Nome = restaurante.Nome;
                restauranteExistente.CNPJ = restaurante.CNPJ;
                restauranteExistente.Telefone = restaurante.Telefone;
                restauranteExistente.Email = restaurante.Email;
                restauranteExistente.Endereco = restaurante.Endereco;
                restauranteExistente.Numero = restaurante.Numero;
                restauranteExistente.Complemento = restaurante.Complemento;
                restauranteExistente.Bairro = restaurante.Bairro;
                restauranteExistente.Cidade = restaurante.Cidade;
                restauranteExistente.UF = restaurante.UF;
                restauranteExistente.CEP = restaurante.CEP;

                await _context.SaveChangesAsync();

                return Ok(restauranteExistente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar restaurante {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Desativar restaurante (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Desativar(int id)
        {
            try
            {
                var restaurante = await _context.Restaurantes.FindAsync(id);
                if (restaurante == null)
                    return NotFound($"Restaurante com ID {id} não encontrado");

                // Verificar se existem pedidos vinculados
                var temPedidos = await _context.Pedidos.AnyAsync(p => p.RestauranteId == id);
                if (temPedidos)
                {
                    // Soft delete
                    restaurante.Ativo = false;
                    await _context.SaveChangesAsync();
                    return Ok(new { mensagem = "Restaurante desativado com sucesso" });
                }
                else
                {
                    // Hard delete se não tem pedidos
                    _context.Restaurantes.Remove(restaurante);
                    await _context.SaveChangesAsync();
                    return Ok(new { mensagem = "Restaurante removido com sucesso" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar restaurante {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Ativar restaurante
        /// </summary>
        [HttpPost("{id}/ativar")]
        public async Task<ActionResult> Ativar(int id)
        {
            try
            {
                var restaurante = await _context.Restaurantes.FindAsync(id);
                if (restaurante == null)
                    return NotFound($"Restaurante com ID {id} não encontrado");

                restaurante.Ativo = true;
                await _context.SaveChangesAsync();

                return Ok(new { mensagem = "Restaurante ativado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao ativar restaurante {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter estatísticas do restaurante
        /// </summary>
        [HttpGet("{id}/estatisticas")]
        public async Task<ActionResult> ObterEstatisticas(int id)
        {
            try
            {
                var restaurante = await _context.Restaurantes.FindAsync(id);
                if (restaurante == null)
                    return NotFound($"Restaurante com ID {id} não encontrado");

                var hoje = DateTime.Today;
                var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

                var estatisticas = new
                {
                    pedidosHoje = await _context.Pedidos.CountAsync(p => p.RestauranteId == id && p.DataCriacao.Date == hoje),
                    pedidosMes = await _context.Pedidos.CountAsync(p => p.RestauranteId == id && p.DataCriacao >= inicioMes),
                    vendasHoje = await _context.Pedidos
                        .Where(p => p.RestauranteId == id && p.DataCriacao.Date == hoje && p.Status != StatusPedido.Cancelado)
                        .SumAsync(p => p.ValorTotal),
                    vendasMes = await _context.Pedidos
                        .Where(p => p.RestauranteId == id && p.DataCriacao >= inicioMes && p.Status != StatusPedido.Cancelado)
                        .SumAsync(p => p.ValorTotal),
                    clientesAtivos = await _context.Clientes
                        .Where(c => c.Pedidos.Any(p => p.RestauranteId == id) && c.Ativo)
                        .CountAsync()
                };

                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas do restaurante {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
