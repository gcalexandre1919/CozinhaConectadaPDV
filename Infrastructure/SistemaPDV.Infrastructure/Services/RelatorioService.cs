using Microsoft.EntityFrameworkCore;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;
using SistemaPDV.Infrastructure.Data;
using System.Text;
using System.Text.Json;

namespace SistemaPDV.Infrastructure.Services
{
    public class RelatorioService : IRelatorioService
    {
        private readonly PDVDbContext _context;

        public RelatorioService(PDVDbContext context)
        {
            _context = context;
        }

        #region Relatórios Básicos

        public async Task<IEnumerable<RelatorioVendaDto>> GerarRelatorioVendasAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null)
        {
            var query = _context.Pedidos
                .Where(p => p.DataCriacao.Date >= dataInicio.Date && 
                           p.DataCriacao.Date <= dataFim.Date &&
                           p.Status != StatusPedido.Cancelado);

            if (restauranteId.HasValue)
                query = query.Where(p => p.RestauranteId == restauranteId.Value);

            var vendas = await query
                .GroupBy(p => p.DataCriacao.Date)
                .Select(g => new RelatorioVendaDto
                {
                    Data = g.Key,
                    NumeroPedidos = g.Count(),
                    ValorTotal = g.Sum(p => p.ValorTotal),
                    TicketMedio = g.Average(p => p.ValorTotal)
                })
                .OrderBy(v => v.Data)
                .ToListAsync();

            // Adicionar dados por tipo de pedido
            foreach (var venda in vendas)
            {
                var pedidosDia = await query
                    .Where(p => p.DataCriacao.Date == venda.Data)
                    .GroupBy(p => p.Tipo)
                    .Select(g => new { Tipo = g.Key.ToString(), Quantidade = g.Count() })
                    .ToListAsync();

                venda.PedidosPorTipo = pedidosDia.ToDictionary(p => p.Tipo, p => p.Quantidade);
            }

            return vendas;
        }

        public async Task<IEnumerable<RelatorioProdutoDto>> GerarRelatorioProdutosVendidosAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null)
        {
            var query = _context.PedidoItens
                .Include(pi => pi.Produto)
                    .ThenInclude(p => p!.Categoria)
                .Include(pi => pi.Pedido)
                .Where(pi => pi.Pedido!.DataCriacao.Date >= dataInicio.Date &&
                            pi.Pedido.DataCriacao.Date <= dataFim.Date &&
                            pi.Pedido.Status != StatusPedido.Cancelado);

            if (restauranteId.HasValue)
                query = query.Where(pi => pi.Pedido!.RestauranteId == restauranteId.Value);

            return await query
                .GroupBy(pi => new { pi.ProdutoId, NomeProduto = pi.Produto!.Nome, NomeCategoria = pi.Produto.Categoria!.Nome })
                .Select(g => new RelatorioProdutoDto
                {
                    ProdutoId = g.Key.ProdutoId,
                    NomeProduto = g.Key.NomeProduto,
                    Categoria = g.Key.NomeCategoria,
                    QuantidadeVendida = g.Sum(pi => pi.Quantidade),
                    ValorTotal = g.Sum(pi => pi.SubTotal),
                    PrecoMedio = g.Average(pi => pi.PrecoUnitario)
                })
                .OrderByDescending(p => p.QuantidadeVendida)
                .ToListAsync();
        }

        public async Task<IEnumerable<RelatorioClienteDto>> GerarRelatorioClientesAsync(int? restauranteId = null)
        {
            var query = _context.Clientes
                .Include(c => c.Pedidos)
                .Where(c => c.Ativo && c.Pedidos.Any());

            if (restauranteId.HasValue)
                query = query.Where(c => c.Pedidos.Any(p => p.RestauranteId == restauranteId.Value));

            return await query
                .Select(c => new RelatorioClienteDto
                {
                    ClienteId = c.Id,
                    Nome = c.Nome,
                    Telefone = c.Telefone,
                    Email = c.Email,
                    TotalPedidos = c.Pedidos.Count(p => restauranteId == null || p.RestauranteId == restauranteId.Value),
                    ValorTotal = c.Pedidos
                        .Where(p => p.Status != StatusPedido.Cancelado && (restauranteId == null || p.RestauranteId == restauranteId.Value))
                        .Sum(p => p.ValorTotal),
                    UltimoPedido = c.Pedidos
                        .Where(p => restauranteId == null || p.RestauranteId == restauranteId.Value)
                        .OrderByDescending(p => p.DataCriacao)
                        .Select(p => p.DataCriacao)
                        .FirstOrDefault(),
                    TicketMedio = c.Pedidos
                        .Where(p => p.Status != StatusPedido.Cancelado && (restauranteId == null || p.RestauranteId == restauranteId.Value))
                        .Any() ? 
                        c.Pedidos
                            .Where(p => p.Status != StatusPedido.Cancelado && (restauranteId == null || p.RestauranteId == restauranteId.Value))
                            .Average(p => p.ValorTotal) : 0
                })
                .OrderByDescending(c => c.ValorTotal)
                .ToListAsync();
        }

        #endregion

        #region Geração de PDF

        public async Task<byte[]> GerarRelatorioVendasPdfAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null)
        {
            var vendas = await GerarRelatorioVendasAsync(dataInicio, dataFim, restauranteId);
            var html = GerarHtmlRelatorioVendas(vendas, dataInicio, dataFim);
            return await ConverterHtmlParaPdfAsync(html);
        }

        public async Task<byte[]> GerarRelatorioProdutosPdfAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null)
        {
            var produtos = await GerarRelatorioProdutosVendidosAsync(dataInicio, dataFim, restauranteId);
            var html = GerarHtmlRelatorioProdutos(produtos, dataInicio, dataFim);
            return await ConverterHtmlParaPdfAsync(html);
        }

        public async Task<byte[]> GerarRelatorioClientesPdfAsync(int? restauranteId = null)
        {
            var clientes = await GerarRelatorioClientesAsync(restauranteId);
            var html = GerarHtmlRelatorioClientes(clientes);
            return await ConverterHtmlParaPdfAsync(html);
        }

        public async Task<byte[]> GerarRelatorioPedidoPdfAsync(int pedidoId)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Include(p => p.Restaurante)
                .FirstOrDefaultAsync(p => p.Id == pedidoId);

            if (pedido == null)
                throw new ArgumentException("Pedido não encontrado");

            var html = GerarHtmlPedido(pedido);
            return await ConverterHtmlParaPdfAsync(html);
        }

        #endregion

        #region Estatísticas

        public async Task<RelatorioEstatisticasDto> GerarEstatisticasAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null)
        {
            var query = _context.Pedidos
                .Where(p => p.DataCriacao.Date >= dataInicio.Date && 
                           p.DataCriacao.Date <= dataFim.Date);

            if (restauranteId.HasValue)
                query = query.Where(p => p.RestauranteId == restauranteId.Value);

            var pedidosValidos = query.Where(p => p.Status != StatusPedido.Cancelado);

            var estatisticas = new RelatorioEstatisticasDto
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                TotalPedidos = await query.CountAsync(),
                TotalVendas = await pedidosValidos.SumAsync(p => p.ValorTotal),
                TicketMedio = await pedidosValidos.AnyAsync() ? await pedidosValidos.AverageAsync(p => p.ValorTotal) : 0
            };

            // Pedidos por tipo
            var pedidosPorTipo = await query
                .GroupBy(p => p.Tipo)
                .Select(g => new { Tipo = g.Key.ToString(), Quantidade = g.Count() })
                .ToListAsync();
            estatisticas.PedidosPorTipo = pedidosPorTipo.ToDictionary(p => p.Tipo, p => p.Quantidade);

            // Vendas por tipo
            var vendasPorTipo = await pedidosValidos
                .GroupBy(p => p.Tipo)
                .Select(g => new { Tipo = g.Key.ToString(), Valor = g.Sum(p => p.ValorTotal) })
                .ToListAsync();
            estatisticas.VendasPorTipo = vendasPorTipo.ToDictionary(v => v.Tipo, v => v.Valor);

            // Vendas por dia
            estatisticas.VendasPorDia = (await GerarRelatorioVendasAsync(dataInicio, dataFim, restauranteId)).ToList();

            // Top produtos
            estatisticas.TopProdutos = (await GerarRelatorioProdutosVendidosAsync(dataInicio, dataFim, restauranteId))
                .Take(10).ToList();

            // Top clientes
            estatisticas.TopClientes = (await GerarRelatorioClientesAsync(restauranteId))
                .Take(10).ToList();

            // Pedidos por hora
            var pedidosPorHora = await query
                .GroupBy(p => p.DataCriacao.Hour)
                .Select(g => new { Hora = g.Key, Quantidade = g.Count() })
                .ToListAsync();
            estatisticas.PedidosPorHora = pedidosPorHora.ToDictionary(p => p.Hora, p => p.Quantidade);

            return estatisticas;
        }

        #endregion

        #region Métodos Auxiliares de HTML

        private string GerarHtmlRelatorioVendas(IEnumerable<RelatorioVendaDto> vendas, DateTime dataInicio, DateTime dataFim)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><meta charset='utf-8'><title>Relatório de Vendas</title>");
            sb.AppendLine("<style>body{font-family:Arial,sans-serif;margin:20px}table{width:100%;border-collapse:collapse}th,td{border:1px solid #ddd;padding:8px;text-align:left}th{background-color:#f2f2f2}.total{font-weight:bold}</style>");
            sb.AppendLine("</head><body>");
            
            sb.AppendLine($"<h1>Relatório de Vendas</h1>");
            sb.AppendLine($"<p>Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}</p>");
            sb.AppendLine($"<p>Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Data</th><th>Nº Pedidos</th><th>Valor Total</th><th>Ticket Médio</th></tr>");
            
            foreach (var venda in vendas)
            {
                sb.AppendLine($"<tr>");
                sb.AppendLine($"<td>{venda.Data:dd/MM/yyyy}</td>");
                sb.AppendLine($"<td>{venda.NumeroPedidos}</td>");
                sb.AppendLine($"<td>R$ {venda.ValorTotal:F2}</td>");
                sb.AppendLine($"<td>R$ {venda.TicketMedio:F2}</td>");
                sb.AppendLine($"</tr>");
            }
            
            var totalPedidos = vendas.Sum(v => v.NumeroPedidos);
            var totalVendas = vendas.Sum(v => v.ValorTotal);
            var ticketMedio = totalPedidos > 0 ? totalVendas / totalPedidos : 0;
            
            sb.AppendLine($"<tr class='total'>");
            sb.AppendLine($"<td><strong>TOTAL</strong></td>");
            sb.AppendLine($"<td><strong>{totalPedidos}</strong></td>");
            sb.AppendLine($"<td><strong>R$ {totalVendas:F2}</strong></td>");
            sb.AppendLine($"<td><strong>R$ {ticketMedio:F2}</strong></td>");
            sb.AppendLine($"</tr>");
            
            sb.AppendLine("</table>");
            sb.AppendLine("</body></html>");
            
            return sb.ToString();
        }

        private string GerarHtmlRelatorioProdutos(IEnumerable<RelatorioProdutoDto> produtos, DateTime dataInicio, DateTime dataFim)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><meta charset='utf-8'><title>Relatório de Produtos</title>");
            sb.AppendLine("<style>body{font-family:Arial,sans-serif;margin:20px}table{width:100%;border-collapse:collapse}th,td{border:1px solid #ddd;padding:8px;text-align:left}th{background-color:#f2f2f2}</style>");
            sb.AppendLine("</head><body>");
            
            sb.AppendLine($"<h1>Relatório de Produtos Vendidos</h1>");
            sb.AppendLine($"<p>Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}</p>");
            sb.AppendLine($"<p>Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Produto</th><th>Categoria</th><th>Qtd Vendida</th><th>Valor Total</th><th>Preço Médio</th></tr>");
            
            foreach (var produto in produtos)
            {
                sb.AppendLine($"<tr>");
                sb.AppendLine($"<td>{produto.NomeProduto}</td>");
                sb.AppendLine($"<td>{produto.Categoria}</td>");
                sb.AppendLine($"<td>{produto.QuantidadeVendida}</td>");
                sb.AppendLine($"<td>R$ {produto.ValorTotal:F2}</td>");
                sb.AppendLine($"<td>R$ {produto.PrecoMedio:F2}</td>");
                sb.AppendLine($"</tr>");
            }
            
            sb.AppendLine("</table>");
            sb.AppendLine("</body></html>");
            
            return sb.ToString();
        }

        private string GerarHtmlRelatorioClientes(IEnumerable<RelatorioClienteDto> clientes)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><meta charset='utf-8'><title>Relatório de Clientes</title>");
            sb.AppendLine("<style>body{font-family:Arial,sans-serif;margin:20px}table{width:100%;border-collapse:collapse}th,td{border:1px solid #ddd;padding:8px;text-align:left}th{background-color:#f2f2f2}</style>");
            sb.AppendLine("</head><body>");
            
            sb.AppendLine($"<h1>Relatório de Clientes</h1>");
            sb.AppendLine($"<p>Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Cliente</th><th>Telefone</th><th>Total Pedidos</th><th>Valor Total</th><th>Ticket Médio</th><th>Último Pedido</th></tr>");
            
            foreach (var cliente in clientes)
            {
                sb.AppendLine($"<tr>");
                sb.AppendLine($"<td>{cliente.Nome}</td>");
                sb.AppendLine($"<td>{cliente.Telefone}</td>");
                sb.AppendLine($"<td>{cliente.TotalPedidos}</td>");
                sb.AppendLine($"<td>R$ {cliente.ValorTotal:F2}</td>");
                sb.AppendLine($"<td>R$ {cliente.TicketMedio:F2}</td>");
                sb.AppendLine($"<td>{cliente.UltimoPedido?.ToString("dd/MM/yyyy") ?? "N/A"}</td>");
                sb.AppendLine($"</tr>");
            }
            
            sb.AppendLine("</table>");
            sb.AppendLine("</body></html>");
            
            return sb.ToString();
        }

        private string GerarHtmlPedido(Pedido pedido)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><meta charset='utf-8'><title>Pedido #" + pedido.Id + "</title>");
            sb.AppendLine("<style>body{font-family:Arial,sans-serif;margin:20px}table{width:100%;border-collapse:collapse}th,td{border:1px solid #ddd;padding:8px;text-align:left}th{background-color:#f2f2f2}.header{text-align:center;margin-bottom:20px}.total{font-weight:bold}</style>");
            sb.AppendLine("</head><body>");
            
            sb.AppendLine("<div class='header'>");
            sb.AppendLine($"<h1>{pedido.Restaurante?.Nome ?? "Restaurante"}</h1>");
            sb.AppendLine($"<h2>PEDIDO #{pedido.Id}</h2>");
            sb.AppendLine($"<p>{DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            sb.AppendLine("</div>");
            
            sb.AppendLine($"<p><strong>Cliente:</strong> {pedido.Cliente?.Nome}</p>");
            sb.AppendLine($"<p><strong>Telefone:</strong> {pedido.Cliente?.Telefone}</p>");
            sb.AppendLine($"<p><strong>Tipo:</strong> {pedido.Tipo}</p>");
            sb.AppendLine($"<p><strong>Data:</strong> {pedido.DataCriacao:dd/MM/yyyy HH:mm}</p>");
            
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Item</th><th>Qtd</th><th>Valor Unit.</th><th>Total</th></tr>");
            
            foreach (var item in pedido.Itens)
            {
                sb.AppendLine($"<tr>");
                sb.AppendLine($"<td>{item.Produto?.Nome}</td>");
                sb.AppendLine($"<td>{item.Quantidade}</td>");
                sb.AppendLine($"<td>R$ {item.PrecoUnitario:F2}</td>");
                sb.AppendLine($"<td>R$ {item.SubTotal:F2}</td>");
                sb.AppendLine($"</tr>");
                
                if (!string.IsNullOrEmpty(item.Observacoes))
                {
                    sb.AppendLine($"<tr><td colspan='4'><em>Obs: {item.Observacoes}</em></td></tr>");
                }
            }
            
            sb.AppendLine($"<tr class='total'><td colspan='3'><strong>Subtotal</strong></td><td><strong>R$ {pedido.SubTotal:F2}</strong></td></tr>");
            
            if (pedido.ValorGarcom > 0)
                sb.AppendLine($"<tr><td colspan='3'>Garçom ({pedido.PercentualGarcom}%)</td><td>R$ {pedido.ValorGarcom:F2}</td></tr>");
                
            if (pedido.TaxaEntrega > 0)
                sb.AppendLine($"<tr><td colspan='3'>Taxa de Entrega</td><td>R$ {pedido.TaxaEntrega:F2}</td></tr>");
            
            sb.AppendLine($"<tr class='total'><td colspan='3'><strong>TOTAL</strong></td><td><strong>R$ {pedido.ValorTotal:F2}</strong></td></tr>");
            
            sb.AppendLine("</table>");
            
            if (!string.IsNullOrEmpty(pedido.Observacoes))
            {
                sb.AppendLine($"<p><strong>Observações:</strong><br>{pedido.Observacoes}</p>");
            }
            
            sb.AppendLine("</body></html>");
            
            return sb.ToString();
        }

        private Task<byte[]> ConverterHtmlParaPdfAsync(string html)
        {
            // Por enquanto, retornar o HTML como bytes UTF-8
            // TODO: Implementar conversão real para PDF usando biblioteca como wkhtmltopdf, Puppeteer, etc.
            return Task.FromResult(Encoding.UTF8.GetBytes(html));
        }

        #endregion
    }
}
