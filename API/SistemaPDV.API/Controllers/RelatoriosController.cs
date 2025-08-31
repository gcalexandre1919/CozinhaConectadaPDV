using Microsoft.AspNetCore.Mvc;
using SistemaPDV.Core.Interfaces;

namespace SistemaPDV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        /// Relatório de vendas por período
        /// </summary>
        [HttpGet("vendas")]
        public async Task<ActionResult> RelatorioVendas(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? restauranteId = null)
        {
            try
            {
                var vendas = await _relatorioService.GerarRelatorioVendasAsync(dataInicio, dataFim, restauranteId);
                return Ok(vendas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de vendas");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Relatório de produtos vendidos
        /// </summary>
        [HttpGet("produtos")]
        public async Task<ActionResult> RelatorioProdutos(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? restauranteId = null)
        {
            try
            {
                var produtos = await _relatorioService.GerarRelatorioProdutosVendidosAsync(dataInicio, dataFim, restauranteId);
                return Ok(produtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de produtos");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Relatório de clientes
        /// </summary>
        [HttpGet("clientes")]
        public async Task<ActionResult> RelatorioClientes([FromQuery] int? restauranteId = null)
        {
            try
            {
                var clientes = await _relatorioService.GerarRelatorioClientesAsync(restauranteId);
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de clientes");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Estatísticas gerais
        /// </summary>
        [HttpGet("estatisticas")]
        public async Task<ActionResult> Estatisticas(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? restauranteId = null)
        {
            try
            {
                var estatisticas = await _relatorioService.GerarEstatisticasAsync(dataInicio, dataFim, restauranteId);
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar estatísticas");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Download de relatório de vendas em PDF
        /// </summary>
        [HttpGet("vendas/pdf")]
        public async Task<ActionResult> RelatorioVendasPdf(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? restauranteId = null)
        {
            try
            {
                var pdf = await _relatorioService.GerarRelatorioVendasPdfAsync(dataInicio, dataFim, restauranteId);
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
        /// Download de relatório de produtos em PDF
        /// </summary>
        [HttpGet("produtos/pdf")]
        public async Task<ActionResult> RelatorioProdutosPdf(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? restauranteId = null)
        {
            try
            {
                var pdf = await _relatorioService.GerarRelatorioProdutosPdfAsync(dataInicio, dataFim, restauranteId);
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
        /// Download de relatório de clientes em PDF
        /// </summary>
        [HttpGet("clientes/pdf")]
        public async Task<ActionResult> RelatorioClientesPdf([FromQuery] int? restauranteId = null)
        {
            try
            {
                var pdf = await _relatorioService.GerarRelatorioClientesPdfAsync(restauranteId);
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
        /// Download de pedido específico em PDF
        /// </summary>
        [HttpGet("pedido/{id}/pdf")]
        public async Task<ActionResult> PedidoPdf(int id)
        {
            try
            {
                var pdf = await _relatorioService.GerarRelatorioPedidoPdfAsync(id);
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
    }
}