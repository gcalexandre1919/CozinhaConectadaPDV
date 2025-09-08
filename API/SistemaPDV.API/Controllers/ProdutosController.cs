using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Interfaces;

namespace SistemaPDV.API.Controllers;

/// <summary>
/// Controller para operações de produtos com segurança multi-tenant
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(
        IProdutoService produtoService,
        ILogger<ProdutosController> logger)
    {
        _produtoService = produtoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todos os produtos do restaurante atual
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetAll()
    {
        try
        {
            var produtos = await _produtoService.GetAllAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao recuperar produtos");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém um produto específico por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProdutoDto>> GetById(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { message = "ID inválido" });

            var produto = await _produtoService.GetByIdAsync(id);
            
            if (produto == null)
                return NotFound(new { message = "Produto não encontrado" });

            return Ok(produto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao recuperar produto {ProdutoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém produtos por categoria
    /// </summary>
    [HttpGet("categoria/{categoriaId}")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetByCategoria(int categoriaId)
    {
        try
        {
            if (categoriaId <= 0)
                return BadRequest(new { message = "ID da categoria inválido" });

            var produtos = await _produtoService.GetByCategoriaIdAsync(categoriaId);
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao recuperar produtos da categoria {CategoriaId}", categoriaId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém apenas produtos ativos
    /// </summary>
    [HttpGet("ativos")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetAtivos()
    {
        try
        {
            var produtos = await _produtoService.GetAtivosAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao recuperar produtos ativos");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Busca produtos por nome
    /// </summary>
    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> Search([FromQuery] string nome)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nome))
                return BadRequest(new { message = "Nome é obrigatório para busca" });

            var produtos = await _produtoService.SearchByNameAsync(nome);
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos por nome: {Nome}", nome);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém produtos com baixo estoque
    /// </summary>
    [HttpGet("baixo-estoque")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetBaixoEstoque()
    {
        try
        {
            var produtos = await _produtoService.GetBaixoEstoqueAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao recuperar produtos com baixo estoque");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProdutoDto>> Create([FromBody] ProdutoDto produtoDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var produto = await _produtoService.CreateAsync(produtoDto);
            
            return CreatedAtAction(
                nameof(GetById),
                new { id = produto.Id },
                produto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProdutoDto>> Update(int id, [FromBody] ProdutoDto produtoDto)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { message = "ID inválido" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var produto = await _produtoService.UpdateAsync(id, produtoDto);
            return Ok(produto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto {ProdutoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Ativa/desativa um produto
    /// </summary>
    [HttpPatch("{id}/toggle-ativo")]
    public async Task<ActionResult> ToggleAtivo(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { message = "ID inválido" });

            var success = await _produtoService.ToggleAtivoAsync(id);
            
            if (!success)
                return NotFound(new { message = "Produto não encontrado" });

            return Ok(new { message = "Status do produto alterado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status do produto {ProdutoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Atualiza estoque de um produto
    /// </summary>
    [HttpPatch("{id}/estoque")]
    public async Task<ActionResult> UpdateEstoque(int id, [FromBody] decimal novoEstoque)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { message = "ID inválido" });

            if (novoEstoque < 0)
                return BadRequest(new { message = "Estoque não pode ser negativo" });

            var success = await _produtoService.UpdateEstoqueAsync(id, novoEstoque);
            
            if (!success)
                return NotFound(new { message = "Produto não encontrado" });

            return Ok(new { message = "Estoque atualizado com sucesso" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar estoque do produto {ProdutoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Exclui um produto
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { message = "ID inválido" });

            var success = await _produtoService.DeleteAsync(id);
            
            if (!success)
                return NotFound(new { message = "Produto não encontrado" });

            return Ok(new { message = "Produto excluído com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir produto {ProdutoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Verifica se um produto existe
    /// </summary>
    [HttpHead("{id}")]
    public async Task<ActionResult> Exists(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest();

            var exists = await _produtoService.ExistsAsync(id);
            
            return exists ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do produto {ProdutoId}", id);
            return StatusCode(500);
        }
    }
}
