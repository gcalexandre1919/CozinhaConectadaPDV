using SistemaPDV.Core.DTOs;

namespace SistemaPDV.Core.Interfaces
{
    public interface ICategoriaService
    {
        Task<(IEnumerable<CategoriaDto> Categorias, int Total)> GetCategoriasAsync(int pagina = 1, int tamanhoPagina = 10);
        Task<CategoriaDto?> GetCategoriaByIdAsync(int id);
        Task<CategoriaDto> CreateCategoriaAsync(CategoriaCriacaoDto categoriaDto);
        Task<CategoriaDto?> UpdateCategoriaAsync(int id, CategoriaAtualizacaoDto categoriaDto);
        Task<bool> DeleteCategoriaAsync(int id);
        Task<(IEnumerable<CategoriaDto> Categorias, int Total)> BuscarCategoriasAsync(string? termo, int pagina = 1, int tamanhoPagina = 10);
        Task<IEnumerable<CategoriaDto>> GetCategoriasAtivasAsync();
        Task<bool> ExisteCategoriaPorNomeAsync(string nome, int? idExcluir = null);
    }
}
