using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;

namespace SistemaPDV.Core.Interfaces
{
    public interface IClienteService
    {
        #region Modern Methods (English)
        Task<IEnumerable<ClienteDto>> GetAllAsync();
        Task<ClienteDto?> GetByIdAsync(int id);
        Task<ClienteDto> CreateAsync(ClienteCriacaoDto clienteDto);
        Task<ClienteDto?> UpdateAsync(int id, ClienteAtualizacaoDto clienteDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ClienteDto>> SearchAsync(string termo);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task<bool> CpfExistsAsync(string cpf, int? excludeId = null);
        Task<bool> CnpjExistsAsync(string cnpj, int? excludeId = null);
        Task<IEnumerable<ClienteDto>> GetActiveClientsAsync();
        Task<IEnumerable<ClienteDto>> GetRecentClientsAsync(int days = 30);
        Task<bool> ClientExistsAsync(int id);
        #endregion

        #region Legacy Methods (Portuguese - Backward Compatibility)
        Task<IEnumerable<ClienteDto>> ObterTodosAsync();
        Task<ClienteDto?> ObterPorIdAsync(int id);
        Task<ClienteDto> CriarAsync(ClienteCriacaoDto clienteDto);
        Task<ClienteDto?> AtualizarAsync(int id, ClienteAtualizacaoDto clienteDto);
        Task<bool> DeletarAsync(int id);
        Task<IEnumerable<ClienteDto>> BuscarAsync(string termo);
        Task<bool> ExisteEmailAsync(string email, int? idExcluir = null);
        Task<bool> ExisteCPFAsync(string cpf, int? idExcluir = null);
        Task<bool> ExisteCNPJAsync(string cnpj, int? idExcluir = null);
        #endregion
    }
}
