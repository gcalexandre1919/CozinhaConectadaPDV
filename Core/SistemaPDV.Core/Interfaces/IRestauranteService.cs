using SistemaPDV.Core.DTOs;

namespace SistemaPDV.Core.Interfaces
{
    public interface IRestauranteService
    {
        #region Modern Methods (English)
        Task<IEnumerable<RestauranteDto>> GetAllAsync();
        Task<IEnumerable<RestauranteDto>> GetActiveAsync();
        Task<RestauranteDto?> GetByIdAsync(int id);
        Task<RestauranteDto> CreateAsync(RestauranteCriacaoDto restauranteDto);
        Task<RestauranteDto?> UpdateAsync(int id, RestauranteAtualizacaoDto restauranteDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ActivateAsync(int id);
        Task<bool> DeactivateAsync(int id);
        Task<RestauranteEstatisticasDto?> GetStatisticsAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> NameExistsAsync(string nome, int? excludeId = null);
        Task<bool> CnpjExistsAsync(string cnpj, int? excludeId = null);
        #endregion

        #region Legacy Methods (Portuguese - Backward Compatibility)
        Task<IEnumerable<RestauranteDto>> ObterTodosAsync();
        Task<IEnumerable<RestauranteDto>> ObterAtivosAsync();
        Task<RestauranteDto?> ObterPorIdAsync(int id);
        Task<RestauranteDto> CriarAsync(RestauranteCriacaoDto restauranteDto);
        Task<RestauranteDto?> AtualizarAsync(int id, RestauranteAtualizacaoDto restauranteDto);
        Task<bool> DeletarAsync(int id);
        Task<bool> AtivarAsync(int id);
        Task<bool> DesativarAsync(int id);
        Task<RestauranteEstatisticasDto?> ObterEstatisticasAsync(int id);
        Task<bool> ExisteAsync(int id);
        Task<bool> ExisteNomeAsync(string nome, int? idExcluir = null);
        Task<bool> ExisteCNPJAsync(string cnpj, int? idExcluir = null);
        #endregion
    }
}
