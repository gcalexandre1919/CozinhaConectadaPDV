using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;
using SistemaPDV.Infrastructure.Data;

namespace SistemaPDV.Infrastructure.Services;

/// <summary>
/// Serviço para operações de produtos com segurança multi-tenant
/// </summary>
public class ProdutoService : IProdutoService
{
    private readonly PDVDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProdutoService> _logger;

    public ProdutoService(
        PDVDbContext context,
        ICurrentUserService currentUserService,
        ILogger<ProdutoService> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<IEnumerable<ProdutoDto>> GetAllAsync()
    {
        try
        {
            var restauranteId = await _currentUserService.GetRestauranteIdAsync();
            
            var produtos = await _context.Produtos
                .AsNoTracking()
                .Where(p => p.RestauranteId == restauranteId)
                .Include(p => p.Categoria)
                .Include(p => p.Restaurante)
                .OrderBy(p => p.Nome)
                .Select(p => new ProdutoDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    Preco = p.Preco,
                    CategoriaId = p.CategoriaId,
                    CategoriaNome = p.Categoria != null ? p.Categoria.Nome : string.Empty,
                    Ativo = p.Ativo,
                    EstoqueMinimo = p.EstoqueMinimo,
                    EstoqueAtual = p.EstoqueAtual,
                    Codigo = p.Codigo,
                    RestauranteId = p.RestauranteId
                })
                .ToListAsync();

            _logger.LogInformation("Produtos recuperados: {Count} para restaurante {RestauranteId}", 
                produtos.Count(), restauranteId);

            return produtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao recuperar produtos");
            throw;
        }
    }

    public async Task<ProdutoDto?> GetByIdAsync(int id)
    {
        try
        {
            var restauranteId = await _currentUserService.GetRestauranteIdAsync();
            
            var produto = await _context.Produtos
                .AsNoTracking()
                .Where(p => p.Id == id && p.RestauranteId == restauranteId)
                .Include(p => p.Categoria)
                .Include(p => p.Restaurante)
                .Select(p => new ProdutoDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    Preco = p.Preco,
                    CategoriaId = p.CategoriaId,
                    CategoriaNome = p.Categoria != null ? p.Categoria.Nome : string.Empty,
                    Ativo = p.Ativo,
                    EstoqueMinimo = p.EstoqueMinimo,
                    EstoqueAtual = p.EstoqueAtual,
                    Codigo = p.Codigo,
                    RestauranteId = p.RestauranteId
                })
                .FirstOrDefaultAsync();

            if (produto != null)
            {
                _logger.LogInformation("Produto encontrado: {ProdutoId} - {Nome}", id, produto.Nome);
            }
            else
            {
                _logger.LogWarning("Produto não encontrado: {ProdutoId} para restaurante {RestauranteId}", 
                    id, restauranteId);
            }

            return produto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao recuperar produto {ProdutoId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<ProdutoDto>> GetByCategoriaIdAsync(int categoriaId)
    {
        try
        {
            var restauranteId = await _currentUserService.GetRestauranteIdAsync();
            
            var produtos = await _context.Produtos
                .AsNoTracking()
                .Where(p => p.CategoriaId == categoriaId && p.RestauranteId == restauranteId)
                .Include(p => p.Categoria)
                .OrderBy(p => p.Nome)
                .Select(p => new ProdutoDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    Preco = p.Preco,
                    CategoriaId = p.CategoriaId,
                    CategoriaNome = p.Categoria != null ? p.Categoria.Nome : string.Empty,
                    Ativo = p.Ativo,
                    EstoqueMinimo = p.EstoqueMinimo,
                    EstoqueAtual = p.EstoqueAtual,
                    Codigo = p.Codigo,
                    RestauranteId = p.RestauranteId
                })
                .ToListAsync();

            _logger.LogInformation("Produtos por categoria {CategoriaId}: {Count} para restaurante {RestauranteId}", 
                categoriaId, produtos.Count(), restauranteId);

            return produtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao recuperar produtos da categoria {CategoriaId}", categoriaId);
            throw;
        }
    }

    public async Task<IEnumerable<ProdutoDto>> GetAtivosAsync()
    {
        try
        {
            var restauranteId = await _currentUserService.GetRestauranteIdAsync();
            
            var produtos = await _context.Produtos
                .AsNoTracking()
                .Where(p => p.Ativo && p.RestauranteId == restauranteId)
                .Include(p => p.Categoria)
                .OrderBy(p => p.Nome)
                .Select(p => new ProdutoDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    Preco = p.Preco,
                    CategoriaId = p.CategoriaId,
                    CategoriaNome = p.Categoria != null ? p.Categoria.Nome : string.Empty,
                    Ativo = p.Ativo,
                    EstoqueMinimo = p.EstoqueMinimo,
                    EstoqueAtual = p.EstoqueAtual,
                    Codigo = p.Codigo,
                    RestauranteId = p.RestauranteId
                })
                .ToListAsync();

            return produtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao recuperar produtos ativos");
            throw;
        }
    }

    public async Task<ProdutoDto> CreateAsync(ProdutoDto produtoDto)
    {
        try
        {
            var restauranteId = await _currentUserService.GetRestauranteIdAsync();

            // Validações
            if (string.IsNullOrWhiteSpace(produtoDto.Nome))
                throw new ArgumentException("Nome do produto é obrigatório");

            if (produtoDto.Preco <= 0)
                throw new ArgumentException("Preço deve ser maior que zero");

            if (produtoDto.CategoriaId <= 0)
                throw new ArgumentException("Categoria é obrigatória");

            // Verificar se a categoria existe e pertence ao restaurante
            var categoriaExists = await _context.Categorias
                .AnyAsync(c => c.Id == produtoDto.CategoriaId && c.RestauranteId == restauranteId);

            if (!categoriaExists)
                throw new ArgumentException("Categoria não encontrada ou não pertence ao restaurante");

            // Verificar se já existe produto com mesmo nome
            var produtoExistente = await _context.Produtos
                .AnyAsync(p => p.Nome.ToLower() == produtoDto.Nome.ToLower() && 
                              p.RestauranteId == restauranteId);

            if (produtoExistente)
                throw new ArgumentException("Já existe um produto com este nome");

            var produto = new Produto
            {
                Nome = produtoDto.Nome.Trim(),
                Descricao = produtoDto.Descricao?.Trim(),
                Preco = produtoDto.Preco,
                CategoriaId = produtoDto.CategoriaId,
                Ativo = produtoDto.Ativo,
                EstoqueMinimo = produtoDto.EstoqueMinimo,
                EstoqueAtual = produtoDto.EstoqueAtual,
                Codigo = produtoDto.Codigo?.Trim(),
                RestauranteId = restauranteId
            };

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            // Recarregar com dados da categoria
            await _context.Entry(produto)
                .Reference(p => p.Categoria)
                .LoadAsync();

            var result = new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                CategoriaId = produto.CategoriaId,
                CategoriaNome = produto.Categoria?.Nome ?? string.Empty,
                Ativo = produto.Ativo,
                EstoqueMinimo = produto.EstoqueMinimo,
                EstoqueAtual = produto.EstoqueAtual,
                Codigo = produto.Codigo,
                RestauranteId = produto.RestauranteId
            };

            _logger.LogInformation("Produto criado: {ProdutoId} - {Nome} para restaurante {RestauranteId}", 
                produto.Id, produto.Nome, restauranteId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto {Nome}", produtoDto.Nome);
            throw;
        }
    }

    public async Task<ProdutoDto> UpdateAsync(int id, ProdutoDto produtoDto)
    {
        try
        {
            var restauranteId = await _currentUserService.GetRestauranteIdAsync();

            var produto = await _context.Produtos
                .Where(p => p.Id == id && p.RestauranteId == restauranteId)
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync();

            if (produto == null)
                throw new ArgumentException("Produto não encontrado");

            // Validações
            if (string.IsNullOrWhiteSpace(produtoDto.Nome))
                throw new ArgumentException("Nome do produto é obrigatório");

            if (produtoDto.Preco <= 0)
                throw new ArgumentException("Preço deve ser maior que zero");

            if (produtoDto.CategoriaId <= 0)
                throw new ArgumentException("Categoria é obrigatória");

            // Verificar se a categoria existe e pertence ao restaurante
            var categoriaExists = await _context.Categorias
                .AnyAsync(c => c.Id == produtoDto.CategoriaId && c.RestauranteId == restauranteId);

            if (!categoriaExists)
                throw new ArgumentException("Categoria não encontrada ou não pertence ao restaurante");

            // Verificar se já existe outro produto com mesmo nome
            var produtoExistente = await _context.Produtos
                .AnyAsync(p => p.Nome.ToLower() == produtoDto.Nome.ToLower() && 
                              p.RestauranteId == restauranteId && 
                              p.Id != id);

            if (produtoExistente)
                throw new ArgumentException("Já existe outro produto com este nome");

            // Atualizar dados
            produto.Nome = produtoDto.Nome.Trim();
            produto.Descricao = produtoDto.Descricao?.Trim();
            produto.Preco = produtoDto.Preco;
            produto.CategoriaId = produtoDto.CategoriaId;
            produto.Ativo = produtoDto.Ativo;
            produto.EstoqueMinimo = produtoDto.EstoqueMinimo;
            produto.EstoqueAtual = produtoDto.EstoqueAtual;
            produto.Codigo = produtoDto.Codigo?.Trim();

            await _context.SaveChangesAsync();

            // Recarregar categoria se mudou
            if (produto.Categoria?.Id != produtoDto.CategoriaId)
            {
                await _context.Entry(produto)
                    .Reference(p => p.Categoria)
                    .LoadAsync();
            }

            var result = new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                CategoriaId = produto.CategoriaId,
                CategoriaNome = produto.Categoria?.Nome ?? string.Empty,
                Ativo = produto.Ativo,
                EstoqueMinimo = produto.EstoqueMinimo,
                EstoqueAtual = produto.EstoqueAtual,
                Codigo = produto.Codigo,
                RestauranteId = produto.RestauranteId
            };

            _logger.LogInformation("Produto atualizado: {ProdutoId} - {Nome}", id, produto.Nome);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto {ProdutoId}", id);
            throw;
        }
    }

    public async Task<bool> ToggleAtivoAsync(int id)
    {
        try
        {
            var restauranteId = await _currentUserService.GetRestauranteIdAsync();

            var produto = await _context.Produtos
                .Where(p => p.Id == id && p.RestauranteId == restauranteId)
                .FirstOrDefaultAsync();

            if (produto == null)
                return false;

            produto.Ativo = !produto.Ativo;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Produto {ProdutoId} - {Nome} {Status}", 
                id, produto.Nome, produto.Ativo ? "ativado" : "desativado");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status do produto {ProdutoId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var restauranteId = await _currentUserService.GetRestauranteIdAsync();

            var produto = await _context.Produtos
                .Where(p => p.Id == id && p.RestauranteId == restauranteId)
                .FirstOrDefaultAsync();

            if (produto == null)
                return false;

            // Verificar se o produto está sendo usado em pedidos
            var produtoEmUso = await _context.Produtos
                .Where(p => p.Id == id)
                .AnyAsync();

            if (produtoEmUso)
            {
                // Soft delete - apenas desativar
                produto.Ativo = false;
                _logger.LogInformation("Produto {ProdutoId} - {Nome} desativado (em uso)", id, produto.Nome);
            }
            else
            {
                // Hard delete - remover completamente
                _context.Produtos.Remove(produto);
                _logger.LogInformation("Produto {ProdutoId} - {Nome} removido", id, produto.Nome);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir produto {ProdutoId}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            var restauranteId = await _currentUserService.GetRestauranteIdAsync();
            
            return await _context.Produtos
                .AnyAsync(p => p.Id == id && p.RestauranteId == restauranteId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do produto {ProdutoId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<ProdutoDto>> SearchByNameAsync(string nome)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nome))
                return new List<ProdutoDto>();

            var restauranteId = await _currentUserService.GetRestauranteIdAsync();
            
            var produtos = await _context.Produtos
                .AsNoTracking()
                .Where(p => p.Nome.ToLower().Contains(nome.ToLower()) && 
                           p.RestauranteId == restauranteId)
                .Include(p => p.Categoria)
                .OrderBy(p => p.Nome)
                .Select(p => new ProdutoDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    Preco = p.Preco,
                    CategoriaId = p.CategoriaId,
                    CategoriaNome = p.Categoria != null ? p.Categoria.Nome : string.Empty,
                    Ativo = p.Ativo,
                    EstoqueMinimo = p.EstoqueMinimo,
                    EstoqueAtual = p.EstoqueAtual,
                    Codigo = p.Codigo,
                    RestauranteId = p.RestauranteId
                })
                .ToListAsync();

            return produtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos por nome: {Nome}", nome);
            throw;
        }
    }

    public async Task<IEnumerable<ProdutoDto>> GetBaixoEstoqueAsync()
    {
        try
        {
            var restauranteId = await _currentUserService.GetRestauranteIdAsync();
            
            var produtos = await _context.Produtos
                .AsNoTracking()
                .Where(p => p.EstoqueAtual <= p.EstoqueMinimo && 
                           p.RestauranteId == restauranteId && 
                           p.Ativo)
                .Include(p => p.Categoria)
                .OrderBy(p => p.EstoqueAtual)
                .ThenBy(p => p.Nome)
                .Select(p => new ProdutoDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    Preco = p.Preco,
                    CategoriaId = p.CategoriaId,
                    CategoriaNome = p.Categoria != null ? p.Categoria.Nome : string.Empty,
                    Ativo = p.Ativo,
                    EstoqueMinimo = p.EstoqueMinimo,
                    EstoqueAtual = p.EstoqueAtual,
                    Codigo = p.Codigo,
                    RestauranteId = p.RestauranteId
                })
                .ToListAsync();

            return produtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao recuperar produtos com baixo estoque");
            throw;
        }
    }

    public async Task<bool> UpdateEstoqueAsync(int id, decimal novoEstoque)
    {
        try
        {
            var restauranteId = await _currentUserService.GetRestauranteIdAsync();

            var produto = await _context.Produtos
                .Where(p => p.Id == id && p.RestauranteId == restauranteId)
                .FirstOrDefaultAsync();

            if (produto == null)
                return false;

            if (novoEstoque < 0)
                throw new ArgumentException("Estoque não pode ser negativo");

            var estoqueAnterior = produto.EstoqueAtual;
            produto.EstoqueAtual = novoEstoque;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Estoque atualizado produto {ProdutoId} - {Nome}: {EstoqueAnterior} → {NovoEstoque}", 
                id, produto.Nome, estoqueAnterior, novoEstoque);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar estoque do produto {ProdutoId}", id);
            throw;
        }
    }
}
