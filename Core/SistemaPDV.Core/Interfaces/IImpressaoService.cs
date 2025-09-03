using SistemaPDV.Core.Entities;

namespace SistemaPDV.Core.Interfaces
{
    public interface IImpressaoService
    {
        // Gestão de Impressoras
        Task<IEnumerable<Impressora>> ObterImpressorasAsync();
        Task<Impressora?> ObterImpressoraPorIdAsync(int id);
        Task<Impressora> CadastrarImpressoraAsync(Impressora impressora);
        Task<Impressora> AtualizarImpressoraAsync(Impressora impressora);
        Task RemoverImpressoraAsync(int id);
        Task<bool> TestarImpressoraAsync(int id);

        // Impressão de Documentos
        Task<bool> ImprimirPedidoAsync(int pedidoId, int? impressoraId = null);
        Task<bool> ReimprimirPedidoAsync(int pedidoId, int? impressoraId = null);
        Task<bool> ImprimirRelatorioAsync(string conteudoRelatorio, int? impressoraId = null);
        
        // Impressão Multi-Área
        Task<bool> ImprimirItemPorAreaAsync(int pedidoId, int itemId, int impressoraId);
        Task<bool> ImprimirItemAsync(int pedidoId, int itemId);
        
        // Fila de Impressão
        Task<IEnumerable<FilaImpressao>> ObterFilaImpressaoAsync();
        Task<bool> ProcessarFilaImpressaoAsync();
        Task<bool> CancelarItemFilaAsync(int filaId);
        
        // Configurações
        Task<ConfiguracaoImpressao?> ObterConfiguracaoAsync();
        Task<ConfiguracaoImpressao> SalvarConfiguracaoAsync(ConfiguracaoImpressao configuracao);
    }
}