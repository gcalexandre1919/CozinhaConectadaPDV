using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;

namespace SistemaPDV.Core.Interfaces;

/// <summary>
/// Interface para operações de produtos com segurança multi-tenant
/// </summary>
public interface IProdutoService
{
    /// <summary>
    /// Obtém todos os produtos do restaurante atual
    /// </summary>
    Task<IEnumerable<ProdutoDto>> GetAllAsync();

    /// <summary>
    /// Obtém um produto específico por ID (com validação de restaurante)
    /// </summary>
    Task<ProdutoDto?> GetByIdAsync(int id);

    /// <summary>
    /// Obtém produtos por categoria
    /// </summary>
    Task<IEnumerable<ProdutoDto>> GetByCategoriaIdAsync(int categoriaId);

    /// <summary>
    /// Obtém produtos ativos do restaurante
    /// </summary>
    Task<IEnumerable<ProdutoDto>> GetAtivosAsync();

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    Task<ProdutoDto> CreateAsync(ProdutoDto produtoDto);

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    Task<ProdutoDto> UpdateAsync(int id, ProdutoDto produtoDto);

    /// <summary>
    /// Ativa/desativa um produto
    /// </summary>
    Task<bool> ToggleAtivoAsync(int id);

    /// <summary>
    /// Exclui um produto (soft delete)
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Verifica se um produto existe no restaurante atual
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Busca produtos por nome
    /// </summary>
    Task<IEnumerable<ProdutoDto>> SearchByNameAsync(string nome);

    /// <summary>
    /// Obtém produtos com baixo estoque
    /// </summary>
    Task<IEnumerable<ProdutoDto>> GetBaixoEstoqueAsync();

    /// <summary>
    /// Atualiza estoque de um produto
    /// </summary>
    Task<bool> UpdateEstoqueAsync(int id, decimal novoEstoque);
}
