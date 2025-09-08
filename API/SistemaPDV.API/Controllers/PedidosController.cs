using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;

namespace SistemaPDV.API.Controllers
{
    /// <summary>
    /// Controller para operações de pedidos com segurança multi-tenant
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly ILogger<PedidosController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public PedidosController(IPedidoService pedidoService, ILogger<PedidosController> logger, ICurrentUserService currentUserService)
        {
            _pedidoService = pedidoService;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Obter todos os pedidos com paginação
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginacaoResultadoDto<PedidoDto>>> ObterTodos([FromQuery] PaginacaoParametrosDto parametros)
        {
            try
            {
                var todosPedidos = await _pedidoService.GetAllAsync();
                var query = todosPedidos.AsQueryable();

                // Aplicar filtro de pesquisa se fornecido
                if (!string.IsNullOrWhiteSpace(parametros.Termo))
                {
                    query = query.Where(p => 
                        (p.NomeCliente != null && p.NomeCliente.Contains(parametros.Termo, StringComparison.OrdinalIgnoreCase)) ||
                        p.Id.ToString().Contains(parametros.Termo) ||
                        (p.Observacoes != null && p.Observacoes.Contains(parametros.Termo, StringComparison.OrdinalIgnoreCase))
                    );
                }

                // Aplicar ordenação
                if (!string.IsNullOrWhiteSpace(parametros.OrdenarPor))
                {
                    switch (parametros.OrdenarPor.ToLower())
                    {
                        case "datacriacao":
                            query = parametros.OrdemDecrescente ? query.OrderByDescending(p => p.DataCriacao) : query.OrderBy(p => p.DataCriacao);
                            break;
                        case "cliente":
                            query = parametros.OrdemDecrescente ? query.OrderByDescending(p => p.NomeCliente) : query.OrderBy(p => p.NomeCliente);
                            break;
                        case "status":
                            query = parametros.OrdemDecrescente ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status);
                            break;
                        case "valortotal":
                            query = parametros.OrdemDecrescente ? query.OrderByDescending(p => p.ValorTotal) : query.OrderBy(p => p.ValorTotal);
                            break;
                        default:
                            query = query.OrderByDescending(p => p.DataCriacao);
                            break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(p => p.DataCriacao);
                }

                // Contar total de itens
                var totalItens = query.Count();

                // Aplicar paginação
                var pedidos = query
                    .Skip((parametros.Pagina - 1) * parametros.TamanhoPagina)
                    .Take(parametros.TamanhoPagina)
                    .ToList();

                var resultado = new PaginacaoResultadoDto<PedidoDto>
                {
                    Dados = pedidos,
                    TotalItens = totalItens,
                    PaginaAtual = parametros.Pagina,
                    TamanhoPagina = parametros.TamanhoPagina
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedidos");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter pedido por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoDto>> ObterPorId(int id)
        {
            try
            {
                var pedido = await _pedidoService.GetByIdAsync(id);
                if (pedido == null)
                    return NotFound($"Pedido com ID {id} não encontrado");

                return Ok(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedido {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Criar novo pedido
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PedidoDto>> Criar([FromBody] CriarPedidoDto pedidoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var pedido = await _pedidoService.CreateAsync(pedidoDto);
                return CreatedAtAction(nameof(ObterPorId), new { id = pedido.Id }, pedido);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Adicionar item ao pedido
        /// </summary>
        [HttpPost("{id}/itens")]
        public async Task<ActionResult<PedidoDto>> AdicionarItem(int id, [FromBody] AdicionarItemDto itemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var pedido = await _pedidoService.AddItemAsync(id, itemDto);
                return Ok(pedido);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar item ao pedido {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Remover item do pedido
        /// </summary>
        [HttpDelete("{id}/itens/{itemId}")]
        public async Task<ActionResult<PedidoDto>> RemoverItem(int id, int itemId)
        {
            try
            {
                var pedido = await _pedidoService.RemoveItemAsync(id, itemId);
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover item {ItemId} do pedido {Id}", itemId, id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Atualizar item do pedido
        /// </summary>
        [HttpPut("{id}/itens/{itemId}")]
        public async Task<ActionResult<PedidoDto>> AtualizarItem(int id, int itemId, [FromBody] AdicionarItemDto itemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var pedido = await _pedidoService.UpdateItemAsync(id, itemId, itemDto);
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar item {ItemId} do pedido {Id}", itemId, id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Fechar conta (apenas para Refeição no Local)
        /// </summary>
        [HttpPost("{id}/fechar-conta")]
        public async Task<ActionResult<PedidoDto>> FecharConta(int id, [FromBody] FecharContaDto fecharContaDto)
        {
            try
            {
                fecharContaDto.PedidoId = id;
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var pedido = await _pedidoService.CloseAccountAsync(fecharContaDto);
                return Ok(pedido);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fechar conta do pedido {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Cancelar pedido
        /// </summary>
        [HttpPost("{id}/cancelar")]
        public async Task<ActionResult<PedidoDto>> Cancelar(int id)
        {
            try
            {
                var pedido = await _pedidoService.CancelAsync(id);
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar pedido {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Alterar status do pedido
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<ActionResult<PedidoDto>> AlterarStatus(int id, [FromBody] AlterarStatusDto alterarStatusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var pedido = await _pedidoService.ChangeStatusAsync(id, alterarStatusDto.NovoStatus);
                return Ok(pedido);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do pedido {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Imprimir pedido
        /// </summary>
        [HttpPost("{id}/imprimir")]
        public async Task<ActionResult> Imprimir(int id)
        {
            try
            {
                var sucesso = await _pedidoService.PrintAsync(id);
                if (sucesso)
                    return Ok(new { mensagem = "Pedido enviado para impressão" });
                else
                    return BadRequest("Erro ao imprimir pedido");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao imprimir pedido {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter pedidos em aberto
        /// </summary>
        [HttpGet("em-aberto")]
        public async Task<ActionResult<IEnumerable<PedidoDto>>> ObterEmAberto()
        {
            try
            {
                var pedidos = await _pedidoService.GetOpenOrdersAsync();
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedidos em aberto");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter pedidos por período
        /// </summary>
        [HttpGet("por-periodo")]
        public async Task<ActionResult<IEnumerable<PedidoDto>>> ObterPorPeriodo(
            [FromQuery] DateTime dataInicio, 
            [FromQuery] DateTime dataFim)
        {
            try
            {
                var pedidos = await _pedidoService.GetByPeriodAsync(dataInicio, dataFim);
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedidos por período");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter pedidos por cliente
        /// </summary>
        [HttpGet("por-cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<PedidoDto>>> ObterPorCliente(int clienteId)
        {
            try
            {
                var pedidos = await _pedidoService.GetByClientAsync(clienteId);
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedidos por cliente {ClienteId}", clienteId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter pedidos por tipo
        /// </summary>
        [HttpGet("por-tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<PedidoDto>>> ObterPorTipo(TipoPedido tipo)
        {
            try
            {
                var pedidos = await _pedidoService.GetByTypeAsync(tipo);
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedidos por tipo {Tipo}", tipo);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}