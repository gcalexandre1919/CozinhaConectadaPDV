using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;

namespace SistemaPDV.Core.Interfaces
{
    public interface IPedidoService
    {
        Task<IEnumerable<PedidoDto>> ObterTodosAsync();
        Task<PedidoDto?> ObterPorIdAsync(int id);
        Task<PedidoDto> CriarAsync(CriarPedidoDto pedidoDto);
        Task<PedidoDto> AdicionarItemAsync(int pedidoId, AdicionarItemDto itemDto);
        Task<PedidoDto> RemoverItemAsync(int pedidoId, int itemId);
        Task<PedidoDto> AtualizarItemAsync(int pedidoId, int itemId, AdicionarItemDto itemDto);
        Task<PedidoDto> FecharContaAsync(FecharContaDto fecharContaDto);
        Task<PedidoDto> CancelarAsync(int id);
        Task<bool> ImprimirAsync(int id);
        
        // Relatórios
        Task<IEnumerable<PedidoDto>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<PedidoDto>> ObterPorClienteAsync(int clienteId);
        Task<IEnumerable<PedidoDto>> ObterPorTipoAsync(TipoPedido tipo);
        Task<IEnumerable<PedidoDto>> ObterEmAbertoAsync();
    }
}
