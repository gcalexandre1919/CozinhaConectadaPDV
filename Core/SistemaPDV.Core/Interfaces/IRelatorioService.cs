using SistemaPDV.Core.DTOs;

namespace SistemaPDV.Core.Interfaces
{
    public interface IRelatorioService
    {
        // Modern Basic Reports Methods
        Task<IEnumerable<RelatorioVendaDto>> GetSalesReportAsync(DateTime startDate, DateTime endDate, int? restaurantId = null);
        Task<IEnumerable<RelatorioProdutoDto>> GetProductsSoldReportAsync(DateTime startDate, DateTime endDate, int? restaurantId = null);
        Task<IEnumerable<RelatorioClienteDto>> GetClientsReportAsync(int? restaurantId = null);
        
        // Modern PDF Generation Methods
        Task<byte[]> GenerateSalesReportPdfAsync(DateTime startDate, DateTime endDate, int? restaurantId = null);
        Task<byte[]> GenerateProductsReportPdfAsync(DateTime startDate, DateTime endDate, int? restaurantId = null);
        Task<byte[]> GenerateClientsReportPdfAsync(int? restaurantId = null);
        Task<byte[]> GenerateOrderReportPdfAsync(int orderId);
        
        // Modern Statistics Methods
        Task<RelatorioEstatisticasDto> GetStatisticsAsync(DateTime startDate, DateTime endDate, int? restaurantId = null);
        
        // Advanced Filter Methods
        Task<IEnumerable<RelatorioVendaDto>> GetSalesReportWithFiltersAsync(DateTime startDate, DateTime endDate, int? restaurantId = null, string? orderType = null);
        Task<IEnumerable<RelatorioProdutoDto>> GetTopProductsAsync(DateTime startDate, DateTime endDate, int? restaurantId = null, int take = 10);
        Task<IEnumerable<RelatorioClienteDto>> GetTopClientsAsync(int? restaurantId = null, int take = 10);
        Task<bool> OrderExistsAsync(int orderId);
        
        // Legacy Methods (Obsolete - Backward Compatibility)
        [Obsolete("Use GetSalesReportAsync instead. This method will be removed in a future version.")]
        Task<IEnumerable<RelatorioVendaDto>> GerarRelatorioVendasAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null);
        
        [Obsolete("Use GetProductsSoldReportAsync instead. This method will be removed in a future version.")]
        Task<IEnumerable<RelatorioProdutoDto>> GerarRelatorioProdutosVendidosAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null);
        
        [Obsolete("Use GetClientsReportAsync instead. This method will be removed in a future version.")]
        Task<IEnumerable<RelatorioClienteDto>> GerarRelatorioClientesAsync(int? restauranteId = null);
        
        [Obsolete("Use GenerateSalesReportPdfAsync instead. This method will be removed in a future version.")]
        Task<byte[]> GerarRelatorioVendasPdfAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null);
        
        [Obsolete("Use GenerateProductsReportPdfAsync instead. This method will be removed in a future version.")]
        Task<byte[]> GerarRelatorioProdutosPdfAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null);
        
        [Obsolete("Use GenerateClientsReportPdfAsync instead. This method will be removed in a future version.")]
        Task<byte[]> GerarRelatorioClientesPdfAsync(int? restauranteId = null);
        
        [Obsolete("Use GenerateOrderReportPdfAsync instead. This method will be removed in a future version.")]
        Task<byte[]> GerarRelatorioPedidoPdfAsync(int pedidoId);
        
        [Obsolete("Use GetStatisticsAsync instead. This method will be removed in a future version.")]
        Task<RelatorioEstatisticasDto> GerarEstatisticasAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null);
    }
}
