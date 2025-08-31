using SistemaPDV.Core.Entities;

namespace SistemaPDV.Core.Interfaces
{
    public interface IClienteEnderecoService
    {
        Task<IEnumerable<ClienteEndereco>> ObterPorClienteAsync(int clienteId);
        Task<ClienteEndereco?> ObterPorIdAsync(int id);
        Task<ClienteEndereco> CriarAsync(ClienteEndereco endereco);
        Task<ClienteEndereco> AtualizarAsync(ClienteEndereco endereco);
        Task<bool> ExcluirAsync(int id);
        Task<bool> DefinirComoPrincipalAsync(int id, int clienteId);
    }
}
