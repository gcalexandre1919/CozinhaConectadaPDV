using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;
using SistemaPDV.Infrastructure.Data;
using System.Text;

namespace SistemaPDV.Infrastructure.Services
{
    public class ImpressaoService : IImpressaoService
    {
        private readonly PDVDbContext _context;
        private readonly ILogger<ImpressaoService> _logger;

        public ImpressaoService(PDVDbContext context, ILogger<ImpressaoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Gestão de Impressoras

        public async Task<IEnumerable<Impressora>> ObterImpressorasAsync()
        {
            return await _context.Impressoras
                .Where(i => i.Ativa)
                .OrderBy(i => i.Nome)
                .ToListAsync();
        }

        public async Task<Impressora?> ObterImpressoraPorIdAsync(int id)
        {
            return await _context.Impressoras.FindAsync(id);
        }

        public async Task<Impressora> CadastrarImpressoraAsync(Impressora impressora)
        {
            impressora.DataCadastro = DateTime.Now;
            impressora.Ativa = true;

            _context.Impressoras.Add(impressora);
            await _context.SaveChangesAsync();

            return impressora;
        }

        public async Task<Impressora> AtualizarImpressoraAsync(Impressora impressora)
        {
            var impressoraExistente = await _context.Impressoras.FindAsync(impressora.Id);
            if (impressoraExistente == null)
                throw new ArgumentException("Impressora não encontrada");

            impressoraExistente.Nome = impressora.Nome;
            impressoraExistente.Caminho = impressora.Caminho;
            impressoraExistente.Tipo = impressora.Tipo;
            impressoraExistente.Ativa = impressora.Ativa;

            await _context.SaveChangesAsync();
            return impressoraExistente;
        }

        public async Task RemoverImpressoraAsync(int id)
        {
            var impressora = await _context.Impressoras.FindAsync(id);
            if (impressora != null)
            {
                // Soft delete
                impressora.Ativa = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> TestarImpressoraAsync(int id)
        {
            try
            {
                var impressora = await ObterImpressoraPorIdAsync(id);
                if (impressora == null) return false;

                var conteudoTeste = GerarConteudoTestePagina();
                return await ImprimirConteudoAsync(conteudoTeste, impressora);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao testar impressora {Id}", id);
                return false;
            }
        }

        #endregion

        #region Impressão de Documentos

        public async Task<bool> ImprimirPedidoAsync(int pedidoId, int? impressoraId = null)
        {
            try
            {
                var pedido = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.Itens)
                        .ThenInclude(i => i.Produto)
                    .FirstOrDefaultAsync(p => p.Id == pedidoId);

                if (pedido == null) return false;

                var conteudo = GerarConteudoPedido(pedido);
                var impressora = await ObterImpressoraParaUso(impressoraId);

                if (impressora == null)
                {
                    // Adicionar à fila se não há impressora disponível
                    await AdicionarAFilaAsync(pedidoId, conteudo);
                    return true;
                }

                return await ImprimirConteudoAsync(conteudo, impressora);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao imprimir pedido {PedidoId}", pedidoId);
                return false;
            }
        }

        public async Task<bool> ReimprimirPedidoAsync(int pedidoId, int? impressoraId = null)
        {
            // Mesmo processo da impressão normal
            return await ImprimirPedidoAsync(pedidoId, impressoraId);
        }

        public async Task<bool> ImprimirRelatorioAsync(string conteudoRelatorio, int? impressoraId = null)
        {
            try
            {
                var impressora = await ObterImpressoraParaUso(impressoraId);
                if (impressora == null) return false;

                return await ImprimirConteudoAsync(conteudoRelatorio, impressora);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao imprimir relatório");
                return false;
            }
        }

        #endregion

        #region Fila de Impressão

        public async Task<IEnumerable<FilaImpressao>> ObterFilaImpressaoAsync()
        {
            return await _context.FilaImpressao
                .Where(f => f.Status == StatusImpressao.Pendente)
                .OrderBy(f => f.DataCriacao)
                .ToListAsync();
        }

        public async Task<bool> ProcessarFilaImpressaoAsync()
        {
            try
            {
                var itensNaFila = await ObterFilaImpressaoAsync();
                var impressora = await ObterImpressoraParaUso(null);

                if (impressora == null) return false;

                foreach (var item in itensNaFila)
                {
                    try
                    {
                        var sucesso = await ImprimirConteudoAsync(item.Conteudo, impressora);
                        
                        item.Status = sucesso ? StatusImpressao.Impresso : StatusImpressao.Erro;
                        item.DataProcessamento = DateTime.Now;
                        
                        if (!sucesso)
                        {
                            item.TentativasImpressao++;
                            if (item.TentativasImpressao >= 3)
                            {
                                item.Status = StatusImpressao.Cancelado;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar item da fila {ItemId}", item.Id);
                        item.Status = StatusImpressao.Erro;
                        item.TentativasImpressao++;
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar fila de impressão");
                return false;
            }
        }

        public async Task<bool> CancelarItemFilaAsync(int filaId)
        {
            var item = await _context.FilaImpressao.FindAsync(filaId);
            if (item != null)
            {
                item.Status = StatusImpressao.Cancelado;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        #endregion

        #region Configurações

        public async Task<ConfiguracaoImpressao?> ObterConfiguracaoAsync()
        {
            return await _context.ConfiguracoesImpressao.FirstOrDefaultAsync();
        }

        public async Task<ConfiguracaoImpressao> SalvarConfiguracaoAsync(ConfiguracaoImpressao configuracao)
        {
            var configExistente = await ObterConfiguracaoAsync();
            if (configExistente != null)
            {
                configExistente.ImpressoraPadraoId = configuracao.ImpressoraPadraoId;
                configExistente.ImprimirAutomaticamente = configuracao.ImprimirAutomaticamente;
                configExistente.NumeroVias = configuracao.NumeroVias;
                configExistente.TamanhoFonte = configuracao.TamanhoFonte;
                configExistente.MargemEsquerda = configuracao.MargemEsquerda;
                configExistente.MargemSuperior = configuracao.MargemSuperior;
            }
            else
            {
                _context.ConfiguracoesImpressao.Add(configuracao);
                configExistente = configuracao;
            }

            await _context.SaveChangesAsync();
            return configExistente;
        }

        #endregion

        #region Métodos Auxiliares

        private async Task<Impressora?> ObterImpressoraParaUso(int? impressoraId)
        {
            if (impressoraId.HasValue)
            {
                return await ObterImpressoraPorIdAsync(impressoraId.Value);
            }

            // Buscar impressora padrão na configuração
            var config = await ObterConfiguracaoAsync();
            if (config?.ImpressoraPadraoId != null)
            {
                return await ObterImpressoraPorIdAsync(config.ImpressoraPadraoId.Value);
            }

            // Buscar primeira impressora ativa
            var impressoras = await ObterImpressorasAsync();
            return impressoras.FirstOrDefault();
        }

        private async Task AdicionarAFilaAsync(int pedidoId, string conteudo)
        {
            var item = new FilaImpressao
            {
                PedidoId = pedidoId,
                Conteudo = conteudo,
                Status = StatusImpressao.Pendente,
                DataCriacao = DateTime.Now,
                TentativasImpressao = 0
            };

            _context.FilaImpressao.Add(item);
            await _context.SaveChangesAsync();
        }

        private Task<bool> ImprimirConteudoAsync(string conteudo, Impressora impressora)
        {
            try
            {
                // Para impressoras Windows (locais)
                if (impressora.Tipo == TipoImpressora.Termica || impressora.Tipo == TipoImpressora.Matricial)
                {
                    return Task.FromResult(ImprimirEmImpressoraWindows(conteudo, impressora.Caminho));
                }
                
                // Para impressoras de rede ou outras
                _logger.LogWarning("Tipo de impressora não suportado: {Tipo}", impressora.Tipo);
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao imprimir conteúdo na impressora {Impressora}", impressora.Nome);
                return Task.FromResult(false);
            }
        }

        private bool ImprimirEmImpressoraWindows(string conteudo, string nomeImpressora)
        {
            try
            {
                // Implementação simplificada - por enquanto apenas log
                // TODO: Implementar impressão real usando biblioteca específica
                _logger.LogInformation("Simulando impressão em {Impressora}:\n{Conteudo}", nomeImpressora, conteudo);
                
                // Simular sucesso da impressão
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao imprimir documento");
                return false;
            }
        }

        private string GerarConteudoPedido(Pedido pedido)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("================================");
            sb.AppendLine("           PEDIDO #" + pedido.Id);
            sb.AppendLine("================================");
            sb.AppendLine($"Data: {pedido.DataCriacao:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Cliente: {pedido.Cliente?.Nome}");
            sb.AppendLine($"Telefone: {pedido.Cliente?.Telefone}");
            sb.AppendLine($"Tipo: {pedido.Tipo}");
            sb.AppendLine("--------------------------------");
            sb.AppendLine("ITENS:");
            
            foreach (var item in pedido.Itens)
            {
                sb.AppendLine($"{item.Quantidade}x {item.Produto?.Nome}");
                sb.AppendLine($"   R$ {item.PrecoUnitario:F2} = R$ {item.SubTotal:F2}");
                if (!string.IsNullOrEmpty(item.Observacoes))
                    sb.AppendLine($"   OBS: {item.Observacoes}");
            }
            
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"Subtotal: R$ {pedido.SubTotal:F2}");
            
            if (pedido.ValorGarcom > 0)
                sb.AppendLine($"Garçom ({pedido.PercentualGarcom}%): R$ {pedido.ValorGarcom:F2}");
            
            if (pedido.TaxaEntrega > 0)
                sb.AppendLine($"Taxa Entrega: R$ {pedido.TaxaEntrega:F2}");
            
            sb.AppendLine($"TOTAL: R$ {pedido.ValorTotal:F2}");
            sb.AppendLine("================================");
            
            if (!string.IsNullOrEmpty(pedido.Observacoes))
            {
                sb.AppendLine("OBSERVAÇÕES:");
                sb.AppendLine(pedido.Observacoes);
                sb.AppendLine("================================");
            }
            
            return sb.ToString();
        }

        private string GerarConteudoTestePagina()
        {
            var sb = new StringBuilder();
            sb.AppendLine("================================");
            sb.AppendLine("       TESTE DE IMPRESSORA");
            sb.AppendLine("================================");
            sb.AppendLine($"Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine("Impressora funcionando corretamente!");
            sb.AppendLine("================================");
            return sb.ToString();
        }

        #endregion
    }
}