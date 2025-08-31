using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;

namespace SistemaPDV.Core.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDto>> ObterTodosAsync();
        Task<ClienteDto?> ObterPorIdAsync(int id);
        Task<ClienteDto> CriarAsync(ClienteCriacaoDto clienteDto);
        Task<ClienteDto?> AtualizarAsync(int id, ClienteAtualizacaoDto clienteDto);
        Task<bool> DeletarAsync(int id);
        Task<IEnumerable<ClienteDto>> BuscarAsync(string termo);
        Task<bool> ExisteEmailAsync(string email, int? idExcluir = null);
        Task<bool> ExisteCPFAsync(string cpf, int? idExcluir = null);
        Task<bool> ExisteCNPJAsync(string cnpj, int? idExcluir = null);
    }
}
