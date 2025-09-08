using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;

namespace SistemaPDV.Core.Interfaces
{
    /// <summary>
    /// Interface para operações de pedidos com segurança multi-tenant
    /// </summary>
    public interface IPedidoService
    {
        /// <summary>
        /// Obtém todos os pedidos do restaurante (sem itens para performance)
        /// </summary>
        Task<IEnumerable<PedidoDto>> GetAllAsync();
        
        /// <summary>
        /// Obtém todos os pedidos com itens completos
        /// </summary>
        Task<IEnumerable<PedidoDto>> GetAllWithItemsAsync();
        
        /// <summary>
        /// Obtém um pedido específico por ID
        /// </summary>
        Task<PedidoDto?> GetByIdAsync(int id);
        
        /// <summary>
        /// Cria um novo pedido
        /// </summary>
        Task<PedidoDto> CreateAsync(CriarPedidoDto pedidoDto);
        
        /// <summary>
        /// Adiciona item ao pedido
        /// </summary>
        Task<PedidoDto> AddItemAsync(int pedidoId, AdicionarItemDto itemDto);
        
        /// <summary>
        /// Remove item do pedido
        /// </summary>
        Task<PedidoDto> RemoveItemAsync(int pedidoId, int itemId);
        
        /// <summary>
        /// Atualiza item do pedido
        /// </summary>
        Task<PedidoDto> UpdateItemAsync(int pedidoId, int itemId, AdicionarItemDto itemDto);
        
        /// <summary>
        /// Fecha conta do pedido (apenas para refeição no local)
        /// </summary>
        Task<PedidoDto> CloseAccountAsync(FecharContaDto fecharContaDto);
        
        /// <summary>
        /// Cancela pedido
        /// </summary>
        Task<PedidoDto> CancelAsync(int id);
        
        /// <summary>
        /// Altera status do pedido
        /// </summary>
        Task<PedidoDto> ChangeStatusAsync(int id, StatusPedido novoStatus);
        
        /// <summary>
        /// Imprime pedido
        /// </summary>
        Task<bool> PrintAsync(int id);
        
        // Métodos de consulta e relatórios
        /// <summary>
        /// Obtém pedidos por período
        /// </summary>
        Task<IEnumerable<PedidoDto>> GetByPeriodAsync(DateTime dataInicio, DateTime dataFim);
        
        /// <summary>
        /// Obtém pedidos por cliente
        /// </summary>
        Task<IEnumerable<PedidoDto>> GetByClientAsync(int clienteId);
        
        /// <summary>
        /// Obtém pedidos por tipo
        /// </summary>
        Task<IEnumerable<PedidoDto>> GetByTypeAsync(TipoPedido tipo);
        
        /// <summary>
        /// Obtém pedidos em aberto
        /// </summary>
        Task<IEnumerable<PedidoDto>> GetOpenOrdersAsync();
        
        /// <summary>
        /// Verifica se pedido existe
        /// </summary>
        Task<bool> ExistsAsync(int id);
        
        /// <summary>
        /// Busca pedidos por termo
        /// </summary>
        Task<IEnumerable<PedidoDto>> SearchAsync(string termo);
    }
}
