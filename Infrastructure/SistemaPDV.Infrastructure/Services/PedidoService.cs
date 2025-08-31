using Microsoft.EntityFrameworkCore;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;
using SistemaPDV.Infrastructure.Data;

namespace SistemaPDV.Infrastructure.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly PDVDbContext _context;
        private readonly IImpressaoService _impressaoService;

        public PedidoService(PDVDbContext context, IImpressaoService impressaoService)
        {
            _context = context;
            _impressaoService = impressaoService;
        }

        public async Task<IEnumerable<PedidoDto>> ObterTodosAsync()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return pedidos.Select(ConvertToDto);
        }

        public async Task<PedidoDto?> ObterPorIdAsync(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.Id == id);

            return pedido != null ? ConvertToDto(pedido) : null;
        }

        public async Task<PedidoDto> CriarAsync(CriarPedidoDto pedidoDto)
        {
            var pedido = new Pedido
            {
                ClienteId = pedidoDto.ClienteId,
                Tipo = pedidoDto.Tipo,
                Status = StatusPedido.Aberto,
                Observacoes = pedidoDto.Observacoes,
                RestauranteId = 1 // TODO: Pegar do contexto de autenticação
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

            // Adicionar itens se fornecidos
            foreach (var itemDto in pedidoDto.Itens)
            {
                await AdicionarItemInternoAsync(pedido.Id, itemDto);
            }

            return await ObterPorIdAsync(pedido.Id) ?? throw new InvalidOperationException("Erro ao criar pedido");
        }

        public async Task<PedidoDto> AdicionarItemAsync(int pedidoId, AdicionarItemDto itemDto)
        {
            await AdicionarItemInternoAsync(pedidoId, itemDto);
            return await ObterPorIdAsync(pedidoId) ?? throw new InvalidOperationException("Pedido não encontrado");
        }

        private async Task AdicionarItemInternoAsync(int pedidoId, AdicionarItemDto itemDto)
        {
            var produto = await _context.Produtos.FindAsync(itemDto.ProdutoId);
            if (produto == null)
                throw new ArgumentException("Produto não encontrado");

            var item = new PedidoItem
            {
                PedidoId = pedidoId,
                ProdutoId = itemDto.ProdutoId,
                Quantidade = itemDto.Quantidade,
                PrecoUnitario = produto.Preco,
                Observacoes = itemDto.Observacoes
            };

            _context.PedidoItens.Add(item);
            await _context.SaveChangesAsync();

            // Recalcular totais do pedido
            await RecalcularTotaisAsync(pedidoId);
        }

        public async Task<PedidoDto> RemoverItemAsync(int pedidoId, int itemId)
        {
            var item = await _context.PedidoItens
                .FirstOrDefaultAsync(i => i.Id == itemId && i.PedidoId == pedidoId);

            if (item != null)
            {
                _context.PedidoItens.Remove(item);
                await _context.SaveChangesAsync();
                await RecalcularTotaisAsync(pedidoId);
            }

            return await ObterPorIdAsync(pedidoId) ?? throw new InvalidOperationException("Pedido não encontrado");
        }

        public async Task<PedidoDto> AtualizarItemAsync(int pedidoId, int itemId, AdicionarItemDto itemDto)
        {
            var item = await _context.PedidoItens
                .FirstOrDefaultAsync(i => i.Id == itemId && i.PedidoId == pedidoId);

            if (item != null)
            {
                item.Quantidade = itemDto.Quantidade;
                item.Observacoes = itemDto.Observacoes;
                await _context.SaveChangesAsync();
                await RecalcularTotaisAsync(pedidoId);
            }

            return await ObterPorIdAsync(pedidoId) ?? throw new InvalidOperationException("Pedido não encontrado");
        }

        public async Task<PedidoDto> FecharContaAsync(FecharContaDto fecharContaDto)
        {
            var pedido = await _context.Pedidos.FindAsync(fecharContaDto.PedidoId);
            if (pedido == null)
                throw new ArgumentException("Pedido não encontrado");

            if (pedido.Tipo == TipoPedido.RefeicaoNoLocal)
            {
                if (fecharContaDto.PercentualGarcomFinal.HasValue)
                {
                    pedido.PercentualGarcom = fecharContaDto.PercentualGarcomFinal.Value;
                }
                
                pedido.FecharConta();
                await RecalcularTotaisAsync(pedido.Id);
                await _context.SaveChangesAsync();
            }

            return await ObterPorIdAsync(pedido.Id) ?? throw new InvalidOperationException("Erro ao fechar conta");
        }

        public async Task<PedidoDto> CancelarAsync(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                pedido.Status = StatusPedido.Cancelado;
                await _context.SaveChangesAsync();
            }

            return await ObterPorIdAsync(id) ?? throw new InvalidOperationException("Pedido não encontrado");
        }

        public async Task<bool> ImprimirAsync(int id)
        {
            var pedido = await ObterPorIdAsync(id);
            if (pedido == null) return false;

            // Usar o serviço de impressão real
            return await _impressaoService.ImprimirPedidoAsync(id);
        }

        public async Task<IEnumerable<PedidoDto>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Where(p => p.DataCriacao >= dataInicio && p.DataCriacao <= dataFim.AddDays(1))
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return pedidos.Select(ConvertToDto);
        }

        public async Task<IEnumerable<PedidoDto>> ObterPorClienteAsync(int clienteId)
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Where(p => p.ClienteId == clienteId)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return pedidos.Select(ConvertToDto);
        }

        public async Task<IEnumerable<PedidoDto>> ObterPorTipoAsync(TipoPedido tipo)
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Where(p => p.Tipo == tipo)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return pedidos.Select(ConvertToDto);
        }

        public async Task<IEnumerable<PedidoDto>> ObterEmAbertoAsync()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Where(p => p.Status == StatusPedido.Aberto)
                .OrderBy(p => p.DataCriacao)
                .ToListAsync();

            return pedidos.Select(ConvertToDto);
        }

        private async Task RecalcularTotaisAsync(int pedidoId)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == pedidoId);

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
    }
}
