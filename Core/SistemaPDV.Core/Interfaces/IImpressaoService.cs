using SistemaPDV.Core.Entities;

namespace SistemaPDV.Core.Interfaces
{
    /// <summary>
    /// Interface para operações de impressão com multi-tenancy e padrão de excelência
    /// </summary>
    public interface IImpressaoService
    {
        // Gestão de Impressoras - Padrão de Excelência
        /// <summary>Obter todas as impressoras ativas do restaurante</summary>
        Task<IEnumerable<Impressora>> GetAllAsync();
        
        /// <summary>Obter impressora por ID com validação multi-tenant</summary>
        Task<Impressora?> GetByIdAsync(int id);
        
        /// <summary>Criar nova impressora para o restaurante</summary>
        Task<Impressora> CreateAsync(Impressora impressora);
        
        /// <summary>Atualizar impressora existente com validação de propriedade</summary>
        Task<Impressora> UpdateAsync(Impressora impressora);
        
        /// <summary>Remover impressora (soft delete) com validação de propriedade</summary>
        Task DeleteAsync(int id);
        
        /// <summary>Testar funcionamento da impressora</summary>
        Task<bool> TestAsync(int id);
        
        /// <summary>Verificar se impressora existe e pertence ao restaurante</summary>
        Task<bool> ExistsAsync(int id);
        
        /// <summary>Buscar impressoras por termo (nome, caminho, etc.)</summary>
        Task<IEnumerable<Impressora>> SearchAsync(string term);
        
        /// <summary>Obter apenas impressoras ativas</summary>
        Task<IEnumerable<Impressora>> GetActiveAsync();
        
        /// <summary>Obter impressoras por tipo</summary>
        Task<IEnumerable<Impressora>> GetByTypeAsync(TipoImpressora tipo);

        // Métodos legados para compatibilidade - serão removidos em versão futura
        [Obsolete("Use GetAllAsync() instead", false)]
        Task<IEnumerable<Impressora>> ObterImpressorasAsync();
        
        [Obsolete("Use GetByIdAsync() instead", false)]
        Task<Impressora?> ObterImpressoraPorIdAsync(int id);
        
        [Obsolete("Use CreateAsync() instead", false)]
        Task<Impressora> CadastrarImpressoraAsync(Impressora impressora);
        
        [Obsolete("Use UpdateAsync() instead", false)]
        Task<Impressora> AtualizarImpressoraAsync(Impressora impressora);
        
        [Obsolete("Use DeleteAsync() instead", false)]
        Task RemoverImpressoraAsync(int id);
        
        [Obsolete("Use TestAsync() instead", false)]
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