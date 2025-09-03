using Microsoft.AspNetCore.Mvc;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SistemaPDV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly PDVDbContext _context;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(PDVDbContext context, ILogger<CategoriasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
        {
            try
            {
                var categorias = await _context.Categorias
                    .Include(c => c.Impressora)
                    .Select(c => new CategoriaDto
                    {
                        Id = c.Id,
                        Nome = c.Nome,
                        Descricao = c.Descricao,
                        Ativo = c.Ativo,
                        DataCriacao = c.DataCriacao,
                        ImpressoraId = c.ImpressoraId,
                        ImpressoraNome = c.Impressora != null ? c.Impressora.Nome : null
                    })
                    .ToListAsync();

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias
                    .Where(c => c.Id == id)
                    .Select(c => new CategoriaDto
                    {
                        Id = c.Id,
                        Nome = c.Nome,
                        Descricao = c.Descricao,
                        Ativo = c.Ativo,
                        DataCriacao = c.DataCriacao
                    })
                    .FirstOrDefaultAsync();

                if (categoria == null)
                    return NotFound();

                return Ok(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categoria {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDto>> CreateCategoria(CategoriaCriacaoDto categoriaDto)
        {
            try
            {
                var categoria = new SistemaPDV.Core.Entities.Categoria
                {
                    Nome = categoriaDto.Nome,
                    Descricao = categoriaDto.Descricao,
                    Ativo = true,
                    DataCriacao = DateTime.Now
                };

                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();

                var categoriaRetorno = new CategoriaDto
                {
                    Id = categoria.Id,
                    Nome = categoria.Nome,
                    Descricao = categoria.Descricao,
                    Ativo = categoria.Ativo,
                    DataCriacao = categoria.DataCriacao
                };

                return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoriaRetorno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar categoria");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoriaDto>> UpdateCategoria(int id, CategoriaDto categoriaDto)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria == null)
                    return NotFound();

                categoria.Nome = categoriaDto.Nome;
                categoria.Descricao = categoriaDto.Descricao;
                categoria.Ativo = categoriaDto.Ativo;

                await _context.SaveChangesAsync();

                var categoriaRetorno = new CategoriaDto
                {
                    Id = categoria.Id,
                    Nome = categoria.Nome,
                    Descricao = categoria.Descricao,
                    Ativo = categoria.Ativo,
                    DataCriacao = categoria.DataCriacao
                };

                return Ok(categoriaRetorno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar categoria {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria == null)
                    return NotFound();

                // Verificar se há produtos usando esta categoria
                var produtosComCategoria = await _context.Produtos
                    .Where(p => p.CategoriaId == id)
                    .CountAsync();

                if (produtosComCategoria > 0)
                {
                    return BadRequest($"Não é possível excluir a categoria pois há {produtosComCategoria} produto(s) associado(s).");
                }

                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir categoria {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> BuscarCategorias([FromQuery] string termo)
        {
            try
            {
                var query = _context.Categorias.AsQueryable();

                if (!string.IsNullOrWhiteSpace(termo))
                {
                    query = query.Where(c => 
                        c.Nome.Contains(termo) || 
                        (c.Descricao != null && c.Descricao.Contains(termo)));
                }

                var categorias = await query
                    .Select(c => new CategoriaDto
                    {
                        Id = c.Id,
                        Nome = c.Nome,
                        Descricao = c.Descricao,
                        Ativo = c.Ativo,
                        DataCriacao = c.DataCriacao
                    })
                    .ToListAsync();

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias com termo: {Termo}", termo);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
