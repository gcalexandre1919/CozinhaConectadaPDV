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
        private readonly ICurrentUserService _currentUserService;

        public ImpressaoService(PDVDbContext context, ILogger<ImpressaoService> logger, ICurrentUserService currentUserService)
        {
            _context = context;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        #region Gestão de Impressoras - Padrão de Excelência

        /// <summary>
        /// Obter todas as impressoras ativas do restaurante com performance otimizada
        /// </summary>
        public async Task<IEnumerable<Impressora>> GetAllAsync()
        {
            try
            {
                var impressoras = await _context.Impressoras
                    .AsNoTracking()
                    .Where(i => i.Ativa)
                    .OrderBy(i => i.Nome)
                    .ToListAsync();

                _logger.LogInformation("Obtidas {Count} impressoras ativas", impressoras.Count());

                return impressoras;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter impressoras ativas");
                throw;
            }
        }

        /// <summary>
        /// Obter impressora por ID com validação
        /// </summary>
        public async Task<Impressora?> GetByIdAsync(int id)
        {
            try
            {
                var impressora = await _context.Impressoras
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (impressora != null)
                {
                    _logger.LogInformation("Impressora {Id} obtida com sucesso", id);
                }
                else
                {
                    _logger.LogWarning("Impressora {Id} não encontrada", id);
                }

                return impressora;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter impressora {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Criar nova impressora
        /// </summary>
        public async Task<Impressora> CreateAsync(Impressora impressora)
        {
            try
            {
                impressora.DataCadastro = DateTime.Now;
                impressora.Ativa = true;

                _context.Impressoras.Add(impressora);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Impressora {Nome} criada com ID {Id}", impressora.Nome, impressora.Id);

                return impressora;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar impressora {Nome}", impressora?.Nome);
                throw;
            }
        }

        /// <summary>
        /// Atualizar impressora existente
        /// </summary>
        public async Task<Impressora> UpdateAsync(Impressora impressora)
        {
            try
            {
                var impressoraExistente = await _context.Impressoras
                    .FirstOrDefaultAsync(i => i.Id == impressora.Id);
                
                if (impressoraExistente == null)
                    throw new ArgumentException("Impressora não encontrada");

                impressoraExistente.Nome = impressora.Nome;
                impressoraExistente.Caminho = impressora.Caminho;
                impressoraExistente.Tipo = impressora.Tipo;
                impressoraExistente.Ativa = impressora.Ativa;
                impressoraExistente.LarguraPapel = impressora.LarguraPapel;
                impressoraExistente.CortarPapel = impressora.CortarPapel;
                impressoraExistente.AbrirGaveta = impressora.AbrirGaveta;
                impressoraExistente.Observacoes = impressora.Observacoes;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Impressora {Id} atualizada com sucesso", impressora.Id);

                return impressoraExistente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar impressora {Id}", impressora?.Id);
                throw;
            }
        }

        /// <summary>
        /// Remover impressora (soft delete)
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            try
            {
                var impressora = await _context.Impressoras
                    .FirstOrDefaultAsync(i => i.Id == id);
                
                if (impressora != null)
                {
                    // Soft delete
                    impressora.Ativa = false;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Impressora {Id} removida (soft delete)", id);
                }
                else
                {
                    _logger.LogWarning("Tentativa de remoção de impressora {Id} não encontrada", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover impressora {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Testar funcionamento da impressora
        /// </summary>
        public async Task<bool> TestAsync(int id)
        {
            try
            {
                var impressora = await GetByIdAsync(id);
                if (impressora == null) 
                {
                    _logger.LogWarning("Tentativa de teste de impressora {Id} não encontrada", id);
                    return false;
                }

                var conteudoTeste = GerarConteudoTestePagina();
                var resultado = await ImprimirConteudoAsync(conteudoTeste, impressora);

                _logger.LogInformation("Teste de impressora {Id} - Resultado: {Resultado}", id, resultado);

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao testar impressora {Id}", id);
                return false;
            }
        }

        /// <summary>
        /// Verificar se impressora existe
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _context.Impressoras
                    .AnyAsync(i => i.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência da impressora {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Buscar impressoras por termo (nome, caminho, etc.)
        /// </summary>
        public async Task<IEnumerable<Impressora>> SearchAsync(string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                    return new List<Impressora>();
                
                var impressoras = await _context.Impressoras
                    .AsNoTracking()
                    .Where(i => i.Ativa &&
                               (i.Nome.ToLower().Contains(term.ToLower()) ||
                                i.Caminho.ToLower().Contains(term.ToLower()) ||
                                (i.Observacoes != null && i.Observacoes.ToLower().Contains(term.ToLower()))))
                    .OrderBy(i => i.Nome)
                    .ToListAsync();

                _logger.LogInformation("Busca por '{Termo}' retornou {Count} impressoras", 
                    term, impressoras.Count());

                return impressoras;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar impressoras por termo: {Termo}", term);
                throw;
            }
        }

        /// <summary>
        /// Obter apenas impressoras ativas
        /// </summary>
        public async Task<IEnumerable<Impressora>> GetActiveAsync()
        {
            return await GetAllAsync(); // Já filtra por ativas
        }

        /// <summary>
        /// Obter impressoras por tipo
        /// </summary>
        public async Task<IEnumerable<Impressora>> GetByTypeAsync(TipoImpressora tipo)
        {
            try
            {
                var impressoras = await _context.Impressoras
                    .AsNoTracking()
                    .Where(i => i.Ativa && i.Tipo == tipo)
                    .OrderBy(i => i.Nome)
                    .ToListAsync();

                _logger.LogInformation("Obtidas {Count} impressoras do tipo {Tipo}", 
                    impressoras.Count(), tipo);

                return impressoras;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter impressoras do tipo {Tipo}", tipo);
                throw;
            }
        }

        #endregion

        #region Métodos Legados - Compatibilidade

        [Obsolete("Use GetAllAsync() instead", false)]
        public async Task<IEnumerable<Impressora>> ObterImpressorasAsync()
        {
            return await GetAllAsync();
        }

        [Obsolete("Use GetByIdAsync() instead", false)]
        public async Task<Impressora?> ObterImpressoraPorIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        [Obsolete("Use CreateAsync() instead", false)]
        public async Task<Impressora> CadastrarImpressoraAsync(Impressora impressora)
        {
            return await CreateAsync(impressora);
        }

        [Obsolete("Use UpdateAsync() instead", false)]
        public async Task<Impressora> AtualizarImpressoraAsync(Impressora impressora)
        {
            return await UpdateAsync(impressora);
        }

        [Obsolete("Use DeleteAsync() instead", false)]
        public async Task RemoverImpressoraAsync(int id)
        {
            await DeleteAsync(id);
        }

        [Obsolete("Use TestAsync() instead", false)]
        public async Task<bool> TestarImpressoraAsync(int id)
        {
            return await TestAsync(id);
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

        #region Impressão Multi-Área

        public async Task<bool> ImprimirItemPorAreaAsync(int pedidoId, int itemId, int impressoraId)
        {
            try
            {
                // Buscar o item específico
                var item = await _context.PedidoItens
                    .Include(i => i.Produto)
                    .Include(i => i.Pedido)
                        .ThenInclude(p => p.Cliente)
                    .FirstOrDefaultAsync(i => i.Id == itemId && i.PedidoId == pedidoId);

                if (item == null)
                    return false;

                // Buscar a impressora específica
                var impressora = await ObterImpressoraPorIdAsync(impressoraId);
                if (impressora == null)
                    return false;

                // Gerar conteúdo específico do item para a área
                var conteudo = GerarConteudoItemPorArea(item);

                // Imprimir diretamente na impressora da área
                return await ImprimirConteudoAsync(conteudo, impressora);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao imprimir item {ItemId} do pedido {PedidoId} na impressora {ImpressoraId}", 
                    itemId, pedidoId, impressoraId);
                return false;
            }
        }

        public async Task<bool> ImprimirItemAsync(int pedidoId, int itemId)
        {
            try
            {
                // Usar impressora padrão
                var impressora = await ObterImpressoraParaUso(null);
                if (impressora == null)
                    return false;

                return await ImprimirItemPorAreaAsync(pedidoId, itemId, impressora.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao imprimir item {ItemId} do pedido {PedidoId}", itemId, pedidoId);
                return false;
            }
        }

        private string GerarConteudoItemPorArea(PedidoItem item)
        {
            var sb = new StringBuilder();
            sb.AppendLine("================================");
            sb.AppendLine($"     PEDIDO #{item.PedidoId} - ITEM");
            sb.AppendLine("================================");
            sb.AppendLine($"Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Cliente: {item.Pedido?.Cliente?.Nome ?? "N/A"}");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"PRODUTO: {item.Produto?.Nome}");
            sb.AppendLine($"Qtd: {item.Quantidade}");
            
            if (!string.IsNullOrEmpty(item.Observacoes))
            {
                sb.AppendLine($"OBS: {item.Observacoes}");
            }
            
            sb.AppendLine("================================");
            return sb.ToString();
        }

        #endregion
    }
}