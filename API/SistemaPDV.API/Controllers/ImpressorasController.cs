using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;

namespace SistemaPDV.API.Controllers
{
    /// <summary>
    /// Controller para operações de impressoras com segurança multi-tenant
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
                var impressoras = await _impressaoService.GetAllAsync();
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
                var impressora = await _impressaoService.GetByIdAsync(id);
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

                var novaImpressora = await _impressaoService.CreateAsync(impressora);
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

                var impressoraAtualizada = await _impressaoService.UpdateAsync(impressora);
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
                await _impressaoService.DeleteAsync(id);
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
                var sucesso = await _impressaoService.TestAsync(id);
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

        /// <summary>
        /// Buscar impressoras por termo
        /// </summary>
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Impressora>>> Buscar([FromQuery] string termo)
        {
            try
            {
                var impressoras = await _impressaoService.SearchAsync(termo);
                return Ok(impressoras);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar impressoras por termo: {Termo}", termo);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter impressoras ativas
        /// </summary>
        [HttpGet("ativas")]
        public async Task<ActionResult<IEnumerable<Impressora>>> ObterAtivas()
        {
            try
            {
                var impressoras = await _impressaoService.GetActiveAsync();
                return Ok(impressoras);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter impressoras ativas");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter impressoras por tipo
        /// </summary>
        [HttpGet("tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<Impressora>>> ObterPorTipo(TipoImpressora tipo)
        {
            try
            {
                var impressoras = await _impressaoService.GetByTypeAsync(tipo);
                return Ok(impressoras);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter impressoras por tipo {Tipo}", tipo);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Verificar se impressora existe
        /// </summary>
        [HttpGet("{id}/existe")]
        public async Task<ActionResult<bool>> VerificarExistencia(int id)
        {
            try
            {
                var existe = await _impressaoService.ExistsAsync(id);
                return Ok(existe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência da impressora {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}