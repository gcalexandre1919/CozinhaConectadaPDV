using Microsoft.AspNetCore.Mvc;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SistemaPDV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly PDVDbContext _context;
        private readonly ILogger<ProdutosController> _logger;

        public ProdutosController(PDVDbContext context, ILogger<ProdutosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetProdutos()
        {
            try
            {
                var produtos = await _context.Produtos
                    .Include(p => p.Categoria)
                    .Select(p => new ProdutoDto
                    {
                        Id = p.Id,
                        Nome = p.Nome,
                        Descricao = p.Descricao,
                        Preco = p.Preco,
                        CategoriaId = p.CategoriaId,
                        NomeCategoria = p.Categoria != null ? p.Categoria.Nome : "Sem categoria",
                        CategoriaNome = p.Categoria != null ? p.Categoria.Nome : "Sem categoria",
                        Codigo = $"PROD{p.Id:000}",
                        QuantidadeEstoque = 50, // Valor padrão por enquanto
                        Ativo = p.Ativo,
                        DataCriacao = p.DataCriacao
                    })
                    .ToListAsync();

                return Ok(produtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produtos");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoDto>> GetProduto(int id)
        {
            try
            {
                var produto = await _context.Produtos
                    .Include(p => p.Categoria)
                    .Where(p => p.Id == id)
                    .Select(p => new ProdutoDto
                    {
                        Id = p.Id,
                        Nome = p.Nome,
                        Descricao = p.Descricao,
                        Preco = p.Preco,
                        CategoriaId = p.CategoriaId,
                        NomeCategoria = p.Categoria != null ? p.Categoria.Nome : "Sem categoria",
                        CategoriaNome = p.Categoria != null ? p.Categoria.Nome : "Sem categoria",
                        Codigo = $"PROD{p.Id:000}",
                        QuantidadeEstoque = 50,
                        Ativo = p.Ativo,
                        DataCriacao = p.DataCriacao
                    })
                    .FirstOrDefaultAsync();

                if (produto == null)
                    return NotFound();

                return Ok(produto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produto {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoDto>> CreateProduto(ProdutoCriacaoDto produtoDto)
        {
            try
            {
                var produto = new SistemaPDV.Core.Entities.Produto
                {
                    Nome = produtoDto.Nome,
                    Descricao = produtoDto.Descricao,
                    Preco = produtoDto.Preco,
                    CategoriaId = produtoDto.CategoriaId,
                    Ativo = true,
                    DataCriacao = DateTime.Now
                };

                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync();

                var produtoRetorno = new ProdutoDto
                {
                    Id = produto.Id,
                    Nome = produto.Nome,
                    Descricao = produto.Descricao,
                    Preco = produto.Preco,
                    CategoriaId = produto.CategoriaId,
                    Codigo = $"PROD{produto.Id:000}",
                    QuantidadeEstoque = 50,
                    Ativo = produto.Ativo,
                    DataCriacao = produto.DataCriacao
                };

                return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produtoRetorno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar produto");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProdutoDto>> UpdateProduto(int id, ProdutoDto produtoDto)
        {
            try
            {
                var produto = await _context.Produtos.FindAsync(id);
                if (produto == null)
                    return NotFound();

                produto.Nome = produtoDto.Nome;
                produto.Descricao = produtoDto.Descricao;
                produto.Preco = produtoDto.Preco;
                produto.CategoriaId = produtoDto.CategoriaId;
                produto.Ativo = produtoDto.Ativo;

                await _context.SaveChangesAsync();

                var produtoRetorno = new ProdutoDto
                {
                    Id = produto.Id,
                    Nome = produto.Nome,
                    Descricao = produto.Descricao,
                    Preco = produto.Preco,
                    CategoriaId = produto.CategoriaId,
                    Codigo = $"PROD{produto.Id:000}",
                    QuantidadeEstoque = 50,
                    Ativo = produto.Ativo,
                    DataCriacao = produto.DataCriacao
                };

                return Ok(produtoRetorno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar produto {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var produto = await _context.Produtos.FindAsync(id);
                if (produto == null)
                    return NotFound();

                produto.Ativo = !produto.Ativo;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do produto {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            try
            {
                var produto = await _context.Produtos.FindAsync(id);
                if (produto == null)
                    return NotFound();

                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir produto {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> BuscarProdutos([FromQuery] string termo)
        {
            try
            {
                var query = _context.Produtos.Include(p => p.Categoria).AsQueryable();

                if (!string.IsNullOrWhiteSpace(termo))
                {
                    query = query.Where(p => 
                        p.Nome.Contains(termo) || 
                        (p.Descricao != null && p.Descricao.Contains(termo)));
                }

                var produtos = await query
                    .Select(p => new ProdutoDto
                    {
                        Id = p.Id,
                        Nome = p.Nome,
                        Descricao = p.Descricao,
                        Preco = p.Preco,
                        CategoriaId = p.CategoriaId,
                        NomeCategoria = p.Categoria != null ? p.Categoria.Nome : "Sem categoria",
                        CategoriaNome = p.Categoria != null ? p.Categoria.Nome : "Sem categoria",
                        Codigo = $"PROD{p.Id:000}",
                        QuantidadeEstoque = 50,
                        Ativo = p.Ativo,
                        DataCriacao = p.DataCriacao
                    })
                    .ToListAsync();

                return Ok(produtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produtos com termo: {Termo}", termo);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
