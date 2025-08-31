using Microsoft.AspNetCore.Mvc;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;

namespace SistemaPDV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImpressorasController : ControllerBase
    {
        private readonly IImpressaoService _impressaoService;
        private readonly ILogger<ImpressorasController> _logger;

        public ImpressorasController(IImpressaoService impressaoService, ILogger<ImpressorasController> logger)
        {
            _impressaoService = impressaoService;
            _logger = logger;
        }

        /// <summary>
        /// Listar todas as impressoras
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Impressora>>> ObterTodas()
        {
            try
            {
                var impressoras = await _impressaoService.ObterImpressorasAsync();
                return Ok(impressoras);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter impressoras");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter impressora por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Impressora>> ObterPorId(int id)
        {
            try
            {
                var impressora = await _impressaoService.ObterImpressoraPorIdAsync(id);
                if (impressora == null)
                    return NotFound($"Impressora com ID {id} não encontrada");

                return Ok(impressora);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter impressora {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Cadastrar nova impressora
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Impressora>> Criar([FromBody] Impressora impressora)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var novaImpressora = await _impressaoService.CadastrarImpressoraAsync(impressora);
                return CreatedAtAction(nameof(ObterPorId), new { id = novaImpressora.Id }, novaImpressora);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar impressora");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Atualizar impressora
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Impressora>> Atualizar(int id, [FromBody] Impressora impressora)
        {
            try
            {
                if (id != impressora.Id)
                    return BadRequest("ID da impressora não confere");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var impressoraAtualizada = await _impressaoService.AtualizarImpressoraAsync(impressora);
                return Ok(impressoraAtualizada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar impressora {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Remover impressora
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Remover(int id)
        {
            try
            {
                await _impressaoService.RemoverImpressoraAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover impressora {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Testar impressora
        /// </summary>
        [HttpPost("{id}/testar")]
        public async Task<ActionResult> TestarImpressora(int id)
        {
            try
            {
                var sucesso = await _impressaoService.TestarImpressoraAsync(id);
                if (sucesso)
                    return Ok(new { mensagem = "Impressora testada com sucesso" });
                else
                    return BadRequest("Erro ao testar impressora");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao testar impressora {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}