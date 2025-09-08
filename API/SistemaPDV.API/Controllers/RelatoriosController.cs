using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPDV.Core.Interfaces;

namespace SistemaPDV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RelatoriosController : ControllerBase
    {
        private readonly IRelatorioService _relatorioService;
        private readonly ILogger<RelatoriosController> _logger;

        public RelatoriosController(IRelatorioService relatorioService, ILogger<RelatoriosController> logger)
        {
            _relatorioService = relatorioService;
            _logger = logger;
        }

        /// <summary>
        /// Relatório de vendas por período (Modern API)
        /// </summary>
        [HttpGet("vendas")]
        public async Task<ActionResult> RelatorioVendas(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? restauranteId = null)
        {
            try
            {
                var vendas = await _relatorioService.GetSalesReportAsync(dataInicio, dataFim, restauranteId);
                return Ok(vendas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de vendas");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Relatório de produtos vendidos (Modern API)
        /// </summary>
        [HttpGet("produtos")]
        public async Task<ActionResult> RelatorioProdutos(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? restauranteId = null)
        {
            try
            {
                var produtos = await _relatorioService.GetProductsSoldReportAsync(dataInicio, dataFim, restauranteId);
                return Ok(produtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de produtos");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Relatório de clientes (Modern API)
        /// </summary>
        [HttpGet("clientes")]
        public async Task<ActionResult> RelatorioClientes([FromQuery] int? restauranteId = null)
        {
            try
            {
                var clientes = await _relatorioService.GetClientsReportAsync(restauranteId);
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de clientes");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Estatísticas gerais (Modern API)
        /// </summary>
        [HttpGet("estatisticas")]
        public async Task<ActionResult> Estatisticas(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? restauranteId = null)
        {
            try
            {
                var estatisticas = await _relatorioService.GetStatisticsAsync(dataInicio, dataFim, restauranteId);
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar estatísticas");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Download de relatório de vendas em PDF (Modern API)
        /// </summary>
        [HttpGet("vendas/pdf")]
        public async Task<ActionResult> RelatorioVendasPdf(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? restauranteId = null)
        {
            try
            {
                var pdf = await _relatorioService.GenerateSalesReportPdfAsync(dataInicio, dataFim, restauranteId);
                var fileName = $"relatorio-vendas-{dataInicio:yyyyMMdd}-{dataFim:yyyyMMdd}.html";
                
                // Por enquanto retornando como HTML até implementar PDF real
                return File(pdf, "text/html", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar PDF de vendas");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Download de relatório de produtos em PDF (Modern API)
        /// </summary>
        [HttpGet("produtos/pdf")]
        public async Task<ActionResult> RelatorioProdutosPdf(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? restauranteId = null)
        {
            try
            {
                var pdf = await _relatorioService.GenerateProductsReportPdfAsync(dataInicio, dataFim, restauranteId);
                var fileName = $"relatorio-produtos-{dataInicio:yyyyMMdd}-{dataFim:yyyyMMdd}.html";
                
                return File(pdf, "text/html", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar PDF de produtos");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Download de relatório de clientes em PDF (Modern API)
        /// </summary>
        [HttpGet("clientes/pdf")]
        public async Task<ActionResult> RelatorioClientesPdf([FromQuery] int? restauranteId = null)
        {
            try
            {
                var pdf = await _relatorioService.GenerateClientsReportPdfAsync(restauranteId);
                var fileName = $"relatorio-clientes-{DateTime.Now:yyyyMMdd}.html";
                
                return File(pdf, "text/html", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar PDF de clientes");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Download de pedido específico em PDF (Modern API)
        /// </summary>
        [HttpGet("pedido/{id}/pdf")]
        public async Task<ActionResult> PedidoPdf(int id)
        {
            try
            {
                var pdf = await _relatorioService.GenerateOrderReportPdfAsync(id);
                var fileName = $"pedido-{id}.html";
                
                return File(pdf, "text/html", fileName);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar PDF do pedido {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Relatório de vendas com filtros avançados
        /// </summary>
        [HttpGet("vendas/filtros")]
        public async Task<ActionResult> RelatorioVendasComFiltros(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? restauranteId = null,
            [FromQuery] string? tipoPedido = null)
        {
            try
            {
                var vendas = await _relatorioService.GetSalesReportWithFiltersAsync(dataInicio, dataFim, restauranteId, tipoPedido);
                return Ok(vendas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de vendas com filtros");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Top produtos mais vendidos
        /// </summary>
        [HttpGet("produtos/top")]
        public async Task<ActionResult> TopProdutos(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? restauranteId = null,
            [FromQuery] int take = 10)
        {
            try
            {
                var produtos = await _relatorioService.GetTopProductsAsync(dataInicio, dataFim, restauranteId, take);
                return Ok(produtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter top produtos");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Top clientes mais ativos
        /// </summary>
        [HttpGet("clientes/top")]
        public async Task<ActionResult> TopClientes(
            [FromQuery] int? restauranteId = null,
            [FromQuery] int take = 10)
        {
            try
            {
                var clientes = await _relatorioService.GetTopClientsAsync(restauranteId, take);
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter top clientes");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Verificar se pedido existe
        /// </summary>
        [HttpGet("pedido/{id}/exists")]
        public async Task<ActionResult> VerificarPedidoExiste(int id)
        {
            try
            {
                var existe = await _relatorioService.OrderExistsAsync(id);
                return Ok(new { exists = existe, orderId = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se pedido existe: {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}