using SistemaPDV.Core.DTOs;

namespace SistemaPDV.Core.Interfaces
{
    public interface IRelatorioService
    {
        // Relatórios básicos
        Task<IEnumerable<RelatorioVendaDto>> GerarRelatorioVendasAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null);
        Task<IEnumerable<RelatorioProdutoDto>> GerarRelatorioProdutosVendidosAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null);
        Task<IEnumerable<RelatorioClienteDto>> GerarRelatorioClientesAsync(int? restauranteId = null);
        
        // Geração de PDF
        Task<byte[]> GerarRelatorioVendasPdfAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null);
        Task<byte[]> GerarRelatorioProdutosPdfAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null);
        Task<byte[]> GerarRelatorioClientesPdfAsync(int? restauranteId = null);
        Task<byte[]> GerarRelatorioPedidoPdfAsync(int pedidoId);
        
        // Relatórios estatísticos
        Task<RelatorioEstatisticasDto> GerarEstatisticasAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null);
    }
}
