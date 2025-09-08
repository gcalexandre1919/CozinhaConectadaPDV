using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RelatorioService> _logger;

        public RelatorioService(PDVDbContext context, ILogger<RelatorioService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Modern Sales Report Methods

        public async Task<IEnumerable<RelatorioVendaDto>> GetSalesReportAsync(DateTime startDate, DateTime endDate, int? restaurantId = null)
        {
            try
            {
                _logger.LogInformation("Generating sales report for period {StartDate} to {EndDate}, Restaurant: {RestaurantId}", 
                    startDate, endDate, restaurantId);

                var query = _context.Pedidos
                    .AsNoTracking()
                    .Where(p => p.DataCriacao.Date >= startDate.Date && 
                               p.DataCriacao.Date <= endDate.Date &&
                               p.Status != StatusPedido.Cancelado);

                if (restaurantId.HasValue)
                    query = query.Where(p => p.RestauranteId == restaurantId.Value);

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

                // Optimized query for order types per day
                var pedidosPorTipoPorDia = await query
                    .GroupBy(p => new { Data = p.DataCriacao.Date, Tipo = p.Tipo })
                    .Select(g => new { g.Key.Data, Tipo = g.Key.Tipo.ToString(), Quantidade = g.Count() })
                    .ToListAsync();

                foreach (var venda in vendas)
                {
                    venda.PedidosPorTipo = pedidosPorTipoPorDia
                        .Where(p => p.Data == venda.Data)
                        .ToDictionary(p => p.Tipo, p => p.Quantidade);
                }

                _logger.LogInformation("Successfully generated sales report with {Count} records", vendas.Count);
                return vendas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sales report for period {StartDate} to {EndDate}", startDate, endDate);
                throw;
            }
        }

        public async Task<IEnumerable<RelatorioProdutoDto>> GetProductsSoldReportAsync(DateTime startDate, DateTime endDate, int? restaurantId = null)
        {
            try
            {
                _logger.LogInformation("Generating products sold report for period {StartDate} to {EndDate}, Restaurant: {RestaurantId}", 
                    startDate, endDate, restaurantId);

                var query = _context.PedidoItens
                    .AsNoTracking()
                    .Include(pi => pi.Produto)
                        .ThenInclude(p => p!.Categoria)
                    .Include(pi => pi.Pedido)
                    .Where(pi => pi.Pedido!.DataCriacao.Date >= startDate.Date &&
                                pi.Pedido.DataCriacao.Date <= endDate.Date &&
                                pi.Pedido.Status != StatusPedido.Cancelado);

                if (restaurantId.HasValue)
                    query = query.Where(pi => pi.Pedido!.RestauranteId == restaurantId.Value);

                var produtos = await query
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

                _logger.LogInformation("Successfully generated products report with {Count} products", produtos.Count);
                return produtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating products sold report for period {StartDate} to {EndDate}", startDate, endDate);
                throw;
            }
        }

        public async Task<IEnumerable<RelatorioClienteDto>> GetClientsReportAsync(int? restaurantId = null)
        {
            try
            {
                _logger.LogInformation("Generating clients report for Restaurant: {RestaurantId}", restaurantId);

                var query = _context.Clientes
                    .AsNoTracking()
                    .Include(c => c.Pedidos)
                    .Where(c => c.Ativo && c.Pedidos.Any());

                if (restaurantId.HasValue)
                    query = query.Where(c => c.Pedidos.Any(p => p.RestauranteId == restaurantId.Value));

                var clientes = await query
                    .Select(c => new RelatorioClienteDto
                    {
                        ClienteId = c.Id,
                        Nome = c.Nome,
                        Telefone = c.Telefone,
                        Email = c.Email,
                        TotalPedidos = c.Pedidos.Count(p => restaurantId == null || p.RestauranteId == restaurantId.Value),
                        ValorTotal = c.Pedidos
                            .Where(p => p.Status != StatusPedido.Cancelado && (restaurantId == null || p.RestauranteId == restaurantId.Value))
                            .Sum(p => p.ValorTotal),
                        UltimoPedido = c.Pedidos
                            .Where(p => restaurantId == null || p.RestauranteId == restaurantId.Value)
                            .OrderByDescending(p => p.DataCriacao)
                            .Select(p => p.DataCriacao)
                            .FirstOrDefault(),
                        TicketMedio = c.Pedidos
                            .Where(p => p.Status != StatusPedido.Cancelado && (restaurantId == null || p.RestauranteId == restaurantId.Value))
                            .Any() ? 
                            c.Pedidos
                                .Where(p => p.Status != StatusPedido.Cancelado && (restaurantId == null || p.RestauranteId == restaurantId.Value))
                                .Average(p => p.ValorTotal) : 0
                    })
                    .OrderByDescending(c => c.ValorTotal)
                    .ToListAsync();

                _logger.LogInformation("Successfully generated clients report with {Count} clients", clientes.Count);
                return clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating clients report for Restaurant: {RestaurantId}", restaurantId);
                throw;
            }
        }

        #endregion
        #region Modern PDF Generation Methods

        public async Task<byte[]> GenerateSalesReportPdfAsync(DateTime startDate, DateTime endDate, int? restaurantId = null)
        {
            try
            {
                _logger.LogInformation("Generating sales PDF report for period {StartDate} to {EndDate}", startDate, endDate);
                
                var vendas = await GetSalesReportAsync(startDate, endDate, restaurantId);
                var html = GerarHtmlRelatorioVendas(vendas, startDate, endDate);
                
                _logger.LogInformation("Successfully generated sales PDF report");
                return await ConverterHtmlParaPdfAsync(html);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sales PDF report");
                throw;
            }
        }

        public async Task<byte[]> GenerateProductsReportPdfAsync(DateTime startDate, DateTime endDate, int? restaurantId = null)
        {
            try
            {
                _logger.LogInformation("Generating products PDF report for period {StartDate} to {EndDate}", startDate, endDate);
                
                var produtos = await GetProductsSoldReportAsync(startDate, endDate, restaurantId);
                var html = GerarHtmlRelatorioProdutos(produtos, startDate, endDate);
                
                _logger.LogInformation("Successfully generated products PDF report");
                return await ConverterHtmlParaPdfAsync(html);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating products PDF report");
                throw;
            }
        }

        public async Task<byte[]> GenerateClientsReportPdfAsync(int? restaurantId = null)
        {
            try
            {
                _logger.LogInformation("Generating clients PDF report");
                
                var clientes = await GetClientsReportAsync(restaurantId);
                var html = GerarHtmlRelatorioClientes(clientes);
                
                _logger.LogInformation("Successfully generated clients PDF report");
                return await ConverterHtmlParaPdfAsync(html);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating clients PDF report");
                throw;
            }
        }

        public async Task<byte[]> GenerateOrderReportPdfAsync(int orderId)
        {
            try
            {
                _logger.LogInformation("Generating order PDF report for Order: {OrderId}", orderId);
                
                var pedido = await _context.Pedidos
                    .AsNoTracking()
                    .Include(p => p.Cliente)
                    .Include(p => p.Itens)
                        .ThenInclude(i => i.Produto)
                    .Include(p => p.Restaurante)
                    .FirstOrDefaultAsync(p => p.Id == orderId);

                if (pedido == null)
                {
                    _logger.LogWarning("Order not found: {OrderId}", orderId);
                    throw new ArgumentException($"Pedido com ID {orderId} não encontrado");
                }

                var html = GerarHtmlPedido(pedido);
                
                _logger.LogInformation("Successfully generated order PDF report for Order: {OrderId}", orderId);
                return await ConverterHtmlParaPdfAsync(html);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating order PDF report for Order: {OrderId}", orderId);
                throw;
            }
        }

        #endregion

        #region Modern Statistics Methods

        public async Task<RelatorioEstatisticasDto> GetStatisticsAsync(DateTime startDate, DateTime endDate, int? restaurantId = null)
        {
            try
            {
                _logger.LogInformation("Generating statistics for period {StartDate} to {EndDate}, Restaurant: {RestaurantId}", 
                    startDate, endDate, restaurantId);

                var query = _context.Pedidos
                    .AsNoTracking()
                    .Where(p => p.DataCriacao.Date >= startDate.Date && 
                               p.DataCriacao.Date <= endDate.Date);

                if (restaurantId.HasValue)
                    query = query.Where(p => p.RestauranteId == restaurantId.Value);

                var pedidosValidos = query.Where(p => p.Status != StatusPedido.Cancelado);

                var estatisticas = new RelatorioEstatisticasDto
                {
                    DataInicio = startDate,
                    DataFim = endDate,
                    TotalPedidos = await query.CountAsync(),
                    TotalVendas = await pedidosValidos.SumAsync(p => p.ValorTotal),
                    TicketMedio = await pedidosValidos.AnyAsync() ? await pedidosValidos.AverageAsync(p => p.ValorTotal) : 0
                };

                // Optimized parallel queries for better performance
                var pedidosPorTipoTask = query
                    .GroupBy(p => p.Tipo)
                    .Select(g => new { Tipo = g.Key.ToString(), Quantidade = g.Count() })
                    .ToListAsync();

                var vendasPorTipoTask = pedidosValidos
                    .GroupBy(p => p.Tipo)
                    .Select(g => new { Tipo = g.Key.ToString(), Valor = g.Sum(p => p.ValorTotal) })
                    .ToListAsync();

                var pedidosPorHoraTask = query
                    .GroupBy(p => p.DataCriacao.Hour)
                    .Select(g => new { Hora = g.Key, Quantidade = g.Count() })
                    .ToListAsync();

                await Task.WhenAll(pedidosPorTipoTask, vendasPorTipoTask, pedidosPorHoraTask);

                estatisticas.PedidosPorTipo = (await pedidosPorTipoTask).ToDictionary(p => p.Tipo, p => p.Quantidade);
                estatisticas.VendasPorTipo = (await vendasPorTipoTask).ToDictionary(v => v.Tipo, v => v.Valor);
                estatisticas.PedidosPorHora = (await pedidosPorHoraTask).ToDictionary(p => p.Hora, p => p.Quantidade);

                // Load related data
                estatisticas.VendasPorDia = (await GetSalesReportAsync(startDate, endDate, restaurantId)).ToList();
                estatisticas.TopProdutos = (await GetTopProductsAsync(startDate, endDate, restaurantId, 10)).ToList();
                estatisticas.TopClientes = (await GetTopClientsAsync(restaurantId, 10)).ToList();

                _logger.LogInformation("Successfully generated statistics with {TotalOrders} orders and {TotalSales:C} in sales", 
                    estatisticas.TotalPedidos, estatisticas.TotalVendas);

                return estatisticas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating statistics for period {StartDate} to {EndDate}", startDate, endDate);
                throw;
            }
        }

        #endregion

        #region Advanced Filter Methods

        public async Task<IEnumerable<RelatorioVendaDto>> GetSalesReportWithFiltersAsync(DateTime startDate, DateTime endDate, int? restaurantId = null, string? orderType = null)
        {
            try
            {
                _logger.LogInformation("Generating filtered sales report for period {StartDate} to {EndDate}, Restaurant: {RestaurantId}, OrderType: {OrderType}", 
                    startDate, endDate, restaurantId, orderType);

                var query = _context.Pedidos
                    .AsNoTracking()
                    .Where(p => p.DataCriacao.Date >= startDate.Date && 
                               p.DataCriacao.Date <= endDate.Date &&
                               p.Status != StatusPedido.Cancelado);

                if (restaurantId.HasValue)
                    query = query.Where(p => p.RestauranteId == restaurantId.Value);

                if (!string.IsNullOrEmpty(orderType) && Enum.TryParse<TipoPedido>(orderType, true, out var tipoEnum))
                    query = query.Where(p => p.Tipo == tipoEnum);

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

                _logger.LogInformation("Successfully generated filtered sales report with {Count} records", vendas.Count);
                return vendas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating filtered sales report");
                throw;
            }
        }

        public async Task<IEnumerable<RelatorioProdutoDto>> GetTopProductsAsync(DateTime startDate, DateTime endDate, int? restaurantId = null, int take = 10)
        {
            try
            {
                var produtos = await GetProductsSoldReportAsync(startDate, endDate, restaurantId);
                return produtos.Take(take);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top products");
                throw;
            }
        }

        public async Task<IEnumerable<RelatorioClienteDto>> GetTopClientsAsync(int? restaurantId = null, int take = 10)
        {
            try
            {
                var clientes = await GetClientsReportAsync(restaurantId);
                return clientes.Take(take);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top clients");
                throw;
            }
        }

        public async Task<bool> OrderExistsAsync(int orderId)
        {
            try
            {
                return await _context.Pedidos
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if order exists: {OrderId}", orderId);
                throw;
            }
        }

        #endregion

        #region Legacy Methods (Backward Compatibility)

        [Obsolete("Use GetSalesReportAsync instead")]
        public async Task<IEnumerable<RelatorioVendaDto>> GerarRelatorioVendasAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null)
        {
            return await GetSalesReportAsync(dataInicio, dataFim, restauranteId);
        }

        [Obsolete("Use GetProductsSoldReportAsync instead")]
        public async Task<IEnumerable<RelatorioProdutoDto>> GerarRelatorioProdutosVendidosAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null)
        {
            return await GetProductsSoldReportAsync(dataInicio, dataFim, restauranteId);
        }

        [Obsolete("Use GetClientsReportAsync instead")]
        public async Task<IEnumerable<RelatorioClienteDto>> GerarRelatorioClientesAsync(int? restauranteId = null)
        {
            return await GetClientsReportAsync(restauranteId);
        }

        [Obsolete("Use GenerateSalesReportPdfAsync instead")]
        public async Task<byte[]> GerarRelatorioVendasPdfAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null)
        {
            return await GenerateSalesReportPdfAsync(dataInicio, dataFim, restauranteId);
        }

        [Obsolete("Use GenerateProductsReportPdfAsync instead")]
        public async Task<byte[]> GerarRelatorioProdutosPdfAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null)
        {
            return await GenerateProductsReportPdfAsync(dataInicio, dataFim, restauranteId);
        }

        [Obsolete("Use GenerateClientsReportPdfAsync instead")]
        public async Task<byte[]> GerarRelatorioClientesPdfAsync(int? restauranteId = null)
        {
            return await GenerateClientsReportPdfAsync(restauranteId);
        }

        [Obsolete("Use GenerateOrderReportPdfAsync instead")]
        public async Task<byte[]> GerarRelatorioPedidoPdfAsync(int pedidoId)
        {
            return await GenerateOrderReportPdfAsync(pedidoId);
        }

        [Obsolete("Use GetStatisticsAsync instead")]
        public async Task<RelatorioEstatisticasDto> GerarEstatisticasAsync(DateTime dataInicio, DateTime dataFim, int? restauranteId = null)
        {
            return await GetStatisticsAsync(dataInicio, dataFim, restauranteId);
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
