using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Interfaces;

namespace SistemaPDV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(ICategoriaService categoriaService, ILogger<CategoriasController> logger)
        {
            _categoriaService = categoriaService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetCategorias(
            [FromQuery] int pagina = 1, 
            [FromQuery] int tamanhoPagina = 10)
        {
            try
            {
                if (pagina <= 0) pagina = 1;
                if (tamanhoPagina <= 0 || tamanhoPagina > 100) tamanhoPagina = 10;

                var (categorias, total) = await _categoriaService.GetCategoriasAsync(pagina, tamanhoPagina);

                var response = new
                {
                    Categorias = categorias,
                    Paginacao = new
                    {
                        PaginaAtual = pagina,
                        TamanhoPagina = tamanhoPagina,
                        TotalItens = total,
                        TotalPaginas = (int)Math.Ceiling((double)total / tamanhoPagina)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias - Página: {Pagina}, Tamanho: {Tamanho}", pagina, tamanhoPagina);
                return StatusCode(500, new { Erro = "Erro interno do servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Erro = "ID inválido" });
                }

                var categoria = await _categoriaService.GetCategoriaByIdAsync(id);
                
                if (categoria == null)
                {
                    return NotFound(new { Erro = "Categoria não encontrada" });
                }

                return Ok(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categoria {Id}", id);
                return StatusCode(500, new { Erro = "Erro interno do servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDto>> CreateCategoria(CategoriaCriacaoDto categoriaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var categoria = await _categoriaService.CreateCategoriaAsync(categoriaDto);
                return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoria);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Erro de negócio ao criar categoria: {Erro}", ex.Message);
                return BadRequest(new { Erro = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar categoria");
                return StatusCode(500, new { Erro = "Erro interno do servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoriaDto>> UpdateCategoria(int id, CategoriaAtualizacaoDto categoriaDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Erro = "ID inválido" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var categoria = await _categoriaService.UpdateCategoriaAsync(id, categoriaDto);
                
                if (categoria == null)
                {
                    return NotFound(new { Erro = "Categoria não encontrada" });
                }

                return Ok(categoria);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Erro de negócio ao atualizar categoria {Id}: {Erro}", id, ex.Message);
                return BadRequest(new { Erro = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar categoria {Id}", id);
                return StatusCode(500, new { Erro = "Erro interno do servidor" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Erro = "ID inválido" });
                }

                var sucesso = await _categoriaService.DeleteCategoriaAsync(id);
                
                if (!sucesso)
                {
                    return NotFound(new { Erro = "Categoria não encontrada" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Erro de negócio ao excluir categoria {Id}: {Erro}", id, ex.Message);
                return BadRequest(new { Erro = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir categoria {Id}", id);
                return StatusCode(500, new { Erro = "Erro interno do servidor" });
            }
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<object>> BuscarCategorias(
            [FromQuery] string? termo,
            [FromQuery] int pagina = 1, 
            [FromQuery] int tamanhoPagina = 10)
        {
            try
            {
                if (pagina <= 0) pagina = 1;
                if (tamanhoPagina <= 0 || tamanhoPagina > 100) tamanhoPagina = 10;

                var (categorias, total) = await _categoriaService.BuscarCategoriasAsync(termo, pagina, tamanhoPagina);

                var response = new
                {
                    Categorias = categorias,
                    TermoBusca = termo,
                    Paginacao = new
                    {
                        PaginaAtual = pagina,
                        TamanhoPagina = tamanhoPagina,
                        TotalItens = total,
                        TotalPaginas = (int)Math.Ceiling((double)total / tamanhoPagina)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias com termo: {Termo}", termo);
                return StatusCode(500, new { Erro = "Erro interno do servidor" });
            }
        }

        [HttpGet("ativas")]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategoriasAtivas()
        {
            try
            {
                var categorias = await _categoriaService.GetCategoriasAtivasAsync();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias ativas");
                return StatusCode(500, new { Erro = "Erro interno do servidor" });
            }
        }

        [HttpGet("verificar-nome")]
        public async Task<ActionResult<object>> VerificarNome([FromQuery] string nome, [FromQuery] int? idExcluir = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nome))
                {
                    return BadRequest(new { Erro = "Nome é obrigatório" });
                }

                var existe = await _categoriaService.ExisteCategoriaPorNomeAsync(nome, idExcluir);
                return Ok(new { Existe = existe });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar nome da categoria: {Nome}", nome);
                return StatusCode(500, new { Erro = "Erro interno do servidor" });
            }
        }
    }
}
