using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;
using SistemaPDV.Infrastructure.Data;

namespace SistemaPDV.Infrastructure.Services
{
    /// <summary>
    /// Serviço para operações de pedidos com segurança multi-tenant
    /// </summary>
    public class PedidoService : IPedidoService
    {
        private readonly PDVDbContext _context;
        private readonly IImpressaoService _impressaoService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<PedidoService> _logger;

        public PedidoService(
            PDVDbContext context, 
            IImpressaoService impressaoService, 
            ICurrentUserService currentUserService, 
            ILogger<PedidoService> logger)
        {
            _context = context;
            _impressaoService = impressaoService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<IEnumerable<PedidoDto>> GetAllAsync()
        {
            try
            {
                var restauranteId = await _currentUserService.GetRestauranteIdAsync();
                
                var pedidos = await _context.Pedidos
                    .AsNoTracking()
                    .Include(p => p.Cliente)
                    .Where(p => p.RestauranteId == restauranteId)
                    .OrderByDescending(p => p.DataCriacao)
                    .Select(p => new PedidoDto
                    {
                        Id = p.Id,
                        ClienteId = p.ClienteId,
                        NomeCliente = p.Cliente != null ? p.Cliente.Nome : string.Empty,
                        Tipo = p.Tipo,
                        Status = p.Status,
                        DataCriacao = p.DataCriacao,
                        DataFinalizacao = p.DataFinalizacao,
                        SubTotal = p.SubTotal,
                        PercentualGarcom = p.PercentualGarcom,
                        ValorGarcom = p.ValorGarcom,
                        TaxaEntrega = p.TaxaEntrega,
                        ValorTotal = p.ValorTotal,
                        Observacoes = p.Observacoes,
                        Itens = new List<PedidoItemDto>() // Lista vazia para performance
                    })
                    .ToListAsync();

                _logger.LogInformation("Pedidos recuperados: {Count} para restaurante {RestauranteId}", 
                    pedidos.Count(), restauranteId);

                return pedidos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar pedidos");
                throw;
            }
        }

        public async Task<IEnumerable<PedidoDto>> GetAllWithItemsAsync()
        {
            try
            {
                var restauranteId = await _currentUserService.GetRestauranteIdAsync();
                
                var pedidos = await _context.Pedidos
                    .AsNoTracking()
                    .Include(p => p.Cliente)
                    .Include(p => p.Itens)
                        .ThenInclude(i => i.Produto)
                    .Where(p => p.RestauranteId == restauranteId)
                    .OrderByDescending(p => p.DataCriacao)
                    .ToListAsync();

                var result = pedidos.Select(ConvertToDto);
                
                _logger.LogInformation("Pedidos com itens recuperados: {Count} para restaurante {RestauranteId}", 
                    pedidos.Count(), restauranteId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar pedidos com itens");
                throw;
            }
        }

        public async Task<PedidoDto?> GetByIdAsync(int id)
        {
            try
            {
                var restauranteId = await _currentUserService.GetRestauranteIdAsync();
                
                var pedido = await _context.Pedidos
                    .AsNoTracking()
                    .Include(p => p.Cliente)
                    .Include(p => p.Itens)
                        .ThenInclude(i => i.Produto)
                    .Where(p => p.Id == id && p.RestauranteId == restauranteId)
                    .FirstOrDefaultAsync();

                if (pedido != null)
                {
                    _logger.LogInformation("Pedido encontrado: {PedidoId} para restaurante {RestauranteId}", 
                        id, restauranteId);
                }
                else
                {
                    _logger.LogWarning("Pedido não encontrado: {PedidoId} para restaurante {RestauranteId}", 
                        id, restauranteId);
                }

                return pedido != null ? ConvertToDto(pedido) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar pedido {PedidoId}", id);
                throw;
            }
        }

        public async Task<PedidoDto> CreateAsync(CriarPedidoDto pedidoDto)
        {
            try
            {
                var restauranteId = await _currentUserService.GetRestauranteIdAsync();
                var userName = _currentUserService.GetUserName();

                // Validações
                if (pedidoDto.ClienteId <= 0)
                    throw new ArgumentException("Cliente é obrigatório");

                // Verificar se cliente existe e pertence ao restaurante
                var clienteExists = await _context.Clientes
                    .AnyAsync(c => c.Id == pedidoDto.ClienteId && c.RestauranteId == restauranteId);

                if (!clienteExists)
                    throw new ArgumentException("Cliente não encontrado ou não pertence ao restaurante");
                
                var pedido = new Pedido
                {
                    ClienteId = pedidoDto.ClienteId,
                    Tipo = pedidoDto.Tipo,
                    Status = StatusPedido.Aberto,
                    Observacoes = pedidoDto.Observacoes?.Trim(),
                    RestauranteId = restauranteId
                };

                // Configurar valores específicos por tipo
                if (pedidoDto.Tipo == TipoPedido.RefeicaoNoLocal)
                {
                    pedido.PercentualGarcom = pedidoDto.PercentualGarcom ?? 10;
                }
                else if (pedidoDto.Tipo == TipoPedido.Entrega)
                {
                    pedido.TaxaEntrega = pedidoDto.TaxaEntrega ?? 0;
                }

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Pedido criado: {PedidoId} - Cliente: {ClienteId}, Tipo: {Tipo}, Usuário: {Usuario}, Restaurante: {RestauranteId}", 
                    pedido.Id, pedido.ClienteId, pedido.Tipo, userName, restauranteId);

                // Adicionar itens se fornecidos
                foreach (var itemDto in pedidoDto.Itens)
                {
                    await AddItemInternalAsync(pedido.Id, itemDto);
                }

                return await GetByIdAsync(pedido.Id) ?? throw new InvalidOperationException("Erro ao criar pedido");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido");
                throw;
            }
        }

        public async Task<PedidoDto> AddItemAsync(int pedidoId, AdicionarItemDto itemDto)
        {
            try
            {
                await AddItemInternalAsync(pedidoId, itemDto);
                return await GetByIdAsync(pedidoId) ?? throw new InvalidOperationException("Pedido não encontrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar item ao pedido {PedidoId}", pedidoId);
                throw;
            }
        }

        private async Task AddItemInternalAsync(int pedidoId, AdicionarItemDto itemDto)
        {
            var restauranteId = await _currentUserService.GetRestauranteIdAsync();
            
            // Validações
            if (itemDto.ProdutoId <= 0)
                throw new ArgumentException("Produto é obrigatório");
                
            if (itemDto.Quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero");

            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                    .ThenInclude(c => c!.Impressora)
                .FirstOrDefaultAsync(p => p.Id == itemDto.ProdutoId && 
                                         p.RestauranteId == restauranteId);
            
            if (produto == null)
                throw new ArgumentException($"Produto {itemDto.ProdutoId} não encontrado ou não pertence ao restaurante");

            if (!produto.Ativo)
                throw new ArgumentException($"Produto {produto.Nome} não está ativo");

            var item = new PedidoItem
            {
                PedidoId = pedidoId,
                ProdutoId = itemDto.ProdutoId,
                Quantidade = itemDto.Quantidade,
                PrecoUnitario = produto.Preco,
                Observacoes = itemDto.Observacoes?.Trim()
            };

            _context.PedidoItens.Add(item);
            await _context.SaveChangesAsync();

            // Recalcular totais do pedido
            await RecalcularTotaisAsync(pedidoId);

            _logger.LogInformation("Item adicionado ao pedido: {PedidoId} - Produto: {ProdutoId}, Quantidade: {Quantidade}", 
                pedidoId, itemDto.ProdutoId, itemDto.Quantidade);

            // Impressão multi-área automática
            await ImprimirItemPorAreaAsync(pedidoId, item, produto);
        }

        /// <summary>
        /// Imprime automaticamente o item na impressora específica da categoria (multi-área)
        /// </summary>
        private async Task ImprimirItemPorAreaAsync(int pedidoId, PedidoItem item, Produto produto)
        {
            try
            {
                // Se a categoria tem impressora específica, imprimir o item nela
                if (produto.Categoria?.ImpressoraId.HasValue == true)
                {
                    await _impressaoService.ImprimirItemPorAreaAsync(pedidoId, item.Id, produto.Categoria.ImpressoraId.Value);
                }
                else
                {
                    // Senão, usar impressora padrão (se configurada)
                    await _impressaoService.ImprimirItemAsync(pedidoId, item.Id);
                }
            }
            catch (Exception ex)
            {
                // Log do erro, mas não interrompe o fluxo do pedido
                Console.WriteLine($"Erro na impressão automática do item {item.Id}: {ex.Message}");
            }
        }

        public async Task<PedidoDto> RemoverItemAsync(int pedidoId, int itemId)
        {
            var item = await _context.PedidoItens
                .Include(i => i.Pedido)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.PedidoId == pedidoId && 
                                         i.Pedido!.RestauranteId == _currentUserService.GetRestauranteId());

            if (item != null)
            {
                _context.PedidoItens.Remove(item);
                await _context.SaveChangesAsync();
                await RecalcularTotaisAsync(pedidoId);
            }

            return await GetByIdAsync(pedidoId) ?? throw new InvalidOperationException("Pedido não encontrado");
        }

        public async Task<PedidoDto> AtualizarItemAsync(int pedidoId, int itemId, AdicionarItemDto itemDto)
        {
            var item = await _context.PedidoItens
                .Include(i => i.Pedido)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.PedidoId == pedidoId && 
                                         i.Pedido!.RestauranteId == _currentUserService.GetRestauranteId());

            if (item != null)
            {
                item.Quantidade = itemDto.Quantidade;
                item.Observacoes = itemDto.Observacoes;
                await _context.SaveChangesAsync();
                await RecalcularTotaisAsync(pedidoId);
            }

            return await GetByIdAsync(pedidoId) ?? throw new InvalidOperationException("Pedido não encontrado");
        }

        public async Task<PedidoDto> FecharContaAsync(FecharContaDto fecharContaDto)
        {
            var pedido = await _context.Pedidos
                .Where(p => p.Id == fecharContaDto.PedidoId && p.RestauranteId == _currentUserService.GetRestauranteId())
                .FirstOrDefaultAsync();
            if (pedido == null)
                throw new ArgumentException("Pedido não encontrado");

            if (pedido.Tipo == TipoPedido.RefeicaoNoLocal)
            {
                var percentualAnterior = pedido.PercentualGarcom;
                if (fecharContaDto.PercentualGarcomFinal.HasValue)
                {
                    pedido.PercentualGarcom = fecharContaDto.PercentualGarcomFinal.Value;
                }
                
                pedido.FecharConta();
                await RecalcularTotaisAsync(pedido.Id);
                await _context.SaveChangesAsync();

                // Log de auditoria
                _logger.LogInformation("Conta fechada - ID: {PedidoId}, Percentual garçom anterior: {PercentualAnterior}%, Final: {PercentualFinal}%, Usuário: {Usuario}, Restaurante: {RestauranteId}", 
                    pedido.Id, percentualAnterior, pedido.PercentualGarcom, _currentUserService.GetUserName(), _currentUserService.GetRestauranteId());
            }

            return await GetByIdAsync(pedido.Id) ?? throw new InvalidOperationException("Erro ao fechar conta");
        }

        public async Task<PedidoDto> CancelarAsync(int id)
        {
            var pedido = await _context.Pedidos
                .Where(p => p.Id == id && p.RestauranteId == _currentUserService.GetRestauranteId())
                .FirstOrDefaultAsync();
            if (pedido != null)
            {
                var statusAnterior = pedido.Status;
                pedido.Status = StatusPedido.Cancelado;
                pedido.DataFinalizacao = DateTime.Now;
                await _context.SaveChangesAsync();

                // Log de auditoria
                _logger.LogWarning("Pedido cancelado - ID: {PedidoId}, Status anterior: {StatusAnterior}, Usuário: {Usuario}, Restaurante: {RestauranteId}", 
                    id, statusAnterior, _currentUserService.GetUserName(), _currentUserService.GetRestauranteId());
            }

            return await GetByIdAsync(id) ?? throw new InvalidOperationException("Pedido não encontrado");
        }

        public async Task<PedidoDto> AlterarStatusAsync(int id, StatusPedido novoStatus)
        {
            var pedido = await _context.Pedidos
                .Where(p => p.Id == id && p.RestauranteId == _currentUserService.GetRestauranteId())
                .FirstOrDefaultAsync();
            if (pedido == null)
            {
                throw new ArgumentException("Pedido não encontrado", nameof(id));
            }

            // Validações de negócio para mudança de status - Fluxo melhorado
            if (!IsTransicaoStatusValida(pedido.Status, novoStatus))
            {
                throw new ArgumentException($"Não é possível alterar status de {pedido.Status} para {novoStatus}");
            }

            var statusAnterior = pedido.Status;
            pedido.Status = novoStatus;

            // Definir data de finalização quando necessário
            if (novoStatus == StatusPedido.Fechado || novoStatus == StatusPedido.Cancelado)
            {
                pedido.DataFinalizacao = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            // Log de auditoria
            _logger.LogInformation("Status de pedido alterado - ID: {PedidoId}, De: {StatusAnterior}, Para: {NovoStatus}, Usuário: {Usuario}, Restaurante: {RestauranteId}", 
                id, statusAnterior, novoStatus, _currentUserService.GetUserName(), _currentUserService.GetRestauranteId());

            return await GetByIdAsync(id) ?? throw new InvalidOperationException("Erro ao alterar status do pedido");
        }

        private static bool IsTransicaoStatusValida(StatusPedido statusAtual, StatusPedido novoStatus)
        {
            // Status finais não podem ser alterados
            if (statusAtual == StatusPedido.Cancelado || statusAtual == StatusPedido.Fechado)
            {
                return false;
            }

            // Transições válidas por status atual
            return statusAtual switch
            {
                StatusPedido.Aberto => novoStatus is StatusPedido.Preparando or StatusPedido.Cancelado,
                StatusPedido.Preparando => novoStatus is StatusPedido.Pronto or StatusPedido.Cancelado,
                StatusPedido.Pronto => novoStatus is StatusPedido.Entregue or StatusPedido.Fechado or StatusPedido.Cancelado,
                StatusPedido.Entregue => novoStatus is StatusPedido.Fechado,
                _ => false
            };
        }

        public async Task<bool> ImprimirAsync(int id)
        {
            var pedido = await GetByIdAsync(id);
            if (pedido == null) return false;

            // Usar o serviço de impressão real
            return await _impressaoService.ImprimirPedidoAsync(id);
        }

        public async Task<IEnumerable<PedidoDto>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            // Consulta otimizada - sem itens para relatórios de período
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Where(p => p.RestauranteId == _currentUserService.GetRestauranteId() && 
                           p.DataCriacao >= dataInicio && p.DataCriacao <= dataFim.AddDays(1))
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return pedidos.Select(ConvertToDtoSemItens);
        }

        public async Task<IEnumerable<PedidoDto>> ObterPorClienteAsync(int clienteId)
        {
            // Consulta otimizada - sem itens para histórico do cliente
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Where(p => p.RestauranteId == _currentUserService.GetRestauranteId() && 
                           p.ClienteId == clienteId)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return pedidos.Select(ConvertToDtoSemItens);
        }

        public async Task<IEnumerable<PedidoDto>> ObterPorTipoAsync(TipoPedido tipo)
        {
            // Consulta otimizada - sem itens para filtro por tipo
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Where(p => p.RestauranteId == _currentUserService.GetRestauranteId() && 
                           p.Tipo == tipo)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return pedidos.Select(ConvertToDtoSemItens);
        }

        public async Task<IEnumerable<PedidoDto>> ObterEmAbertoAsync()
        {
            // Consulta otimizada - sem itens para status de pedidos em aberto
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Where(p => p.RestauranteId == _currentUserService.GetRestauranteId() && 
                           p.Status == StatusPedido.Aberto)
                .OrderBy(p => p.DataCriacao)
                .ToListAsync();

            return pedidos.Select(ConvertToDtoSemItens);
        }

        private async Task RecalcularTotaisAsync(int pedidoId)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .Where(p => p.Id == pedidoId && p.RestauranteId == _currentUserService.GetRestauranteId())
                .FirstOrDefaultAsync();

            if (pedido != null)
            {
                pedido.CalcularValores();
                await _context.SaveChangesAsync();
            }
        }

        private static PedidoDto ConvertToDto(Pedido pedido)
        {
            return new PedidoDto
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                NomeCliente = pedido.Cliente?.Nome,
                Tipo = pedido.Tipo,
                Status = pedido.Status,
                DataCriacao = pedido.DataCriacao,
                DataFinalizacao = pedido.DataFinalizacao,
                SubTotal = pedido.SubTotal,
                PercentualGarcom = pedido.PercentualGarcom,
                ValorGarcom = pedido.ValorGarcom,
                TaxaEntrega = pedido.TaxaEntrega,
                ValorTotal = pedido.ValorTotal,
                Observacoes = pedido.Observacoes,
                Itens = pedido.Itens?.Select(i => new PedidoItemDto
                {
                    Id = i.Id,
                    ProdutoId = i.ProdutoId,
                    NomeProduto = i.Produto?.Nome,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    SubTotal = i.SubTotal,
                    Observacoes = i.Observacoes
                }).ToList() ?? new List<PedidoItemDto>()
            };
        }

        private static PedidoDto ConvertToDtoSemItens(Pedido pedido)
        {
            return new PedidoDto
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                NomeCliente = pedido.Cliente?.Nome,
                Tipo = pedido.Tipo,
                Status = pedido.Status,
                DataCriacao = pedido.DataCriacao,
                DataFinalizacao = pedido.DataFinalizacao,
                SubTotal = pedido.SubTotal,
                PercentualGarcom = pedido.PercentualGarcom,
                ValorGarcom = pedido.ValorGarcom,
                TaxaEntrega = pedido.TaxaEntrega,
                ValorTotal = pedido.ValorTotal,
                Observacoes = pedido.Observacoes,
                Itens = new List<PedidoItemDto>() // Lista vazia para performance
            };
        }

        // Métodos adicionais da nova interface
        public async Task<PedidoDto> RemoveItemAsync(int pedidoId, int itemId)
        {
            return await RemoverItemAsync(pedidoId, itemId);
        }

        public async Task<PedidoDto> UpdateItemAsync(int pedidoId, int itemId, AdicionarItemDto itemDto)
        {
            return await AtualizarItemAsync(pedidoId, itemId, itemDto);
        }

        public async Task<PedidoDto> CloseAccountAsync(FecharContaDto fecharContaDto)
        {
            return await FecharContaAsync(fecharContaDto);
        }

        public async Task<PedidoDto> CancelAsync(int id)
        {
            return await CancelarAsync(id);
        }

        public async Task<PedidoDto> ChangeStatusAsync(int id, StatusPedido novoStatus)
        {
            return await AlterarStatusAsync(id, novoStatus);
        }

        public async Task<bool> PrintAsync(int id)
        {
            return await ImprimirAsync(id);
        }

        public async Task<IEnumerable<PedidoDto>> GetByPeriodAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await ObterPorPeriodoAsync(dataInicio, dataFim);
        }

        public async Task<IEnumerable<PedidoDto>> GetByClientAsync(int clienteId)
        {
            return await ObterPorClienteAsync(clienteId);
        }

        public async Task<IEnumerable<PedidoDto>> GetByTypeAsync(TipoPedido tipo)
        {
            return await ObterPorTipoAsync(tipo);
        }

        public async Task<IEnumerable<PedidoDto>> GetOpenOrdersAsync()
        {
            return await ObterEmAbertoAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var restauranteId = await _currentUserService.GetRestauranteIdAsync();
                
                return await _context.Pedidos
                    .AnyAsync(p => p.Id == id && p.RestauranteId == restauranteId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do pedido {PedidoId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<PedidoDto>> SearchAsync(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo))
                    return new List<PedidoDto>();

                var restauranteId = await _currentUserService.GetRestauranteIdAsync();
                
                var pedidos = await _context.Pedidos
                    .AsNoTracking()
                    .Include(p => p.Cliente)
                    .Where(p => p.RestauranteId == restauranteId &&
                               (p.Id.ToString().Contains(termo) ||
                                (p.Cliente != null && p.Cliente.Nome.ToLower().Contains(termo.ToLower())) ||
                                (p.Observacoes != null && p.Observacoes.ToLower().Contains(termo.ToLower()))))
                    .OrderByDescending(p => p.DataCriacao)
                    .Select(p => new PedidoDto
                    {
                        Id = p.Id,
                        ClienteId = p.ClienteId,
                        NomeCliente = p.Cliente != null ? p.Cliente.Nome : string.Empty,
                        Tipo = p.Tipo,
                        Status = p.Status,
                        DataCriacao = p.DataCriacao,
                        DataFinalizacao = p.DataFinalizacao,
                        SubTotal = p.SubTotal,
                        PercentualGarcom = p.PercentualGarcom,
                        ValorGarcom = p.ValorGarcom,
                        TaxaEntrega = p.TaxaEntrega,
                        ValorTotal = p.ValorTotal,
                        Observacoes = p.Observacoes,
                        Itens = new List<PedidoItemDto>()
                    })
                    .ToListAsync();

                return pedidos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos por termo: {Termo}", termo);
                throw;
            }
        }
    }
}
