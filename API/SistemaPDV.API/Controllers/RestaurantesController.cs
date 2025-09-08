using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Interfaces;

namespace SistemaPDV.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantesController : ControllerBase
    {
        private readonly IRestauranteService _restauranteService;
        private readonly ILogger<RestaurantesController> _logger;

        public RestaurantesController(IRestauranteService restauranteService, ILogger<RestaurantesController> logger)
        {
            _restauranteService = restauranteService;
            _logger = logger;
        }

        /// <summary>
        /// Obter todos os restaurantes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestauranteDto>>> ObterTodos()
        {
            try
            {
                _logger.LogInformation("Obtaining all restaurants");

                var restaurantes = await _restauranteService.GetAllAsync();

                _logger.LogInformation("Successfully obtained {Count} restaurants", restaurantes.Count());
                return Ok(restaurantes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter restaurantes");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter restaurante por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RestauranteDto>> ObterPorId(int id)
        {
            try
            {
                _logger.LogInformation("Obtaining restaurant by ID: {RestaurantId}", id);

                var restaurante = await _restauranteService.GetByIdAsync(id);
                
                if (restaurante == null)
                {
                    _logger.LogWarning("Restaurant not found: {RestaurantId}", id);
                    return NotFound($"Restaurante com ID {id} não encontrado");
                }

                _logger.LogInformation("Successfully obtained restaurant: {RestaurantId} - {Nome}", id, restaurante.Nome);
                return Ok(restaurante);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter restaurante {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Criar novo restaurante
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<RestauranteDto>> Criar([FromBody] RestauranteCriacaoDto restauranteDto)
        {
            try
            {
                _logger.LogInformation("Attempting to create restaurant: {Nome}", restauranteDto.Nome);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid ModelState for restaurant creation: {Nome}", restauranteDto.Nome);
                    return BadRequest(ModelState);
                }

                var restauranteResult = await _restauranteService.CreateAsync(restauranteDto);

                _logger.LogInformation("Successfully created restaurant: {RestaurantId} - {Nome}", restauranteResult.Id, restauranteResult.Nome);
                return CreatedAtAction(nameof(ObterPorId), new { id = restauranteResult.Id }, restauranteResult);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating restaurant: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar restaurante");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Atualizar restaurante
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<RestauranteDto>> Atualizar(int id, [FromBody] RestauranteAtualizacaoDto restauranteDto)
        {
            try
            {
                _logger.LogInformation("Attempting to update restaurant: {RestaurantId}", id);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid ModelState for restaurant update: {RestaurantId}", id);
                    return BadRequest(ModelState);
                }

                var restauranteResult = await _restauranteService.UpdateAsync(id, restauranteDto);
                
                if (restauranteResult == null)
                {
                    _logger.LogWarning("Restaurant not found for update: {RestaurantId}", id);
                    return NotFound($"Restaurante com ID {id} não encontrado");
                }

                _logger.LogInformation("Successfully updated restaurant: {RestaurantId} - {Nome}", id, restauranteResult.Nome);
                return Ok(restauranteResult);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error updating restaurant {RestaurantId}: {Message}", id, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar restaurante {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Desativar restaurante (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Desativar(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete restaurant: {RestaurantId}", id);

                var sucesso = await _restauranteService.DeleteAsync(id);
                
                if (!sucesso)
                {
                    _logger.LogWarning("Restaurant not found for deletion: {RestaurantId}", id);
                    return NotFound($"Restaurante com ID {id} não encontrado");
                }

                _logger.LogInformation("Successfully deleted restaurant: {RestaurantId}", id);
                return Ok(new { mensagem = "Restaurante removido/desativado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar restaurante {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Ativar restaurante
        /// </summary>
        [HttpPost("{id}/ativar")]
        public async Task<ActionResult> Ativar(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to activate restaurant: {RestaurantId}", id);

                var sucesso = await _restauranteService.ActivateAsync(id);
                
                if (!sucesso)
                {
                    _logger.LogWarning("Restaurant not found for activation: {RestaurantId}", id);
                    return NotFound($"Restaurante com ID {id} não encontrado");
                }

                _logger.LogInformation("Successfully activated restaurant: {RestaurantId}", id);
                return Ok(new { mensagem = "Restaurante ativado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao ativar restaurante {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter estatísticas do restaurante
        /// </summary>
        [HttpGet("{id}/estatisticas")]
        public async Task<ActionResult<RestauranteEstatisticasDto>> ObterEstatisticas(int id)
        {
            try
            {
                _logger.LogInformation("Obtaining statistics for restaurant: {RestaurantId}", id);

                var estatisticas = await _restauranteService.GetStatisticsAsync(id);
                
                if (estatisticas == null)
                {
                    _logger.LogWarning("Restaurant not found for statistics: {RestaurantId}", id);
                    return NotFound($"Restaurante com ID {id} não encontrado");
                }

                _logger.LogInformation("Successfully obtained statistics for restaurant: {RestaurantId} - {Nome}", id, estatisticas.Nome);
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas do restaurante {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        #region Modern API Endpoints (English)

        /// <summary>
        /// Get all restaurants (modern endpoint)
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<RestauranteDto>>> GetAll()
        {
            try
            {
                var restaurantes = await _restauranteService.GetAllAsync();
                return Ok(restaurantes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all restaurants");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get only active restaurants
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<RestauranteDto>>> GetActive()
        {
            try
            {
                var restaurantes = await _restauranteService.GetActiveAsync();
                return Ok(restaurantes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active restaurants");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Check if restaurant exists by ID
        /// </summary>
        [HttpGet("{id}/exists")]
        public async Task<ActionResult<bool>> Exists(int id)
        {
            try
            {
                var exists = await _restauranteService.ExistsAsync(id);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking restaurant existence");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Check if restaurant name exists
        /// </summary>
        [HttpGet("name-exists")]
        public async Task<ActionResult<bool>> NameExists([FromQuery] string name, [FromQuery] int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest("Name is required");

                var exists = await _restauranteService.NameExistsAsync(name, excludeId);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking name existence");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Check if restaurant CNPJ exists
        /// </summary>
        [HttpGet("cnpj-exists")]
        public async Task<ActionResult<bool>> CnpjExists([FromQuery] string cnpj, [FromQuery] int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cnpj))
                    return BadRequest("CNPJ is required");

                var exists = await _restauranteService.CnpjExistsAsync(cnpj, excludeId);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking CNPJ existence");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deactivate restaurant (modern endpoint)
        /// </summary>
        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult> Deactivate(int id)
        {
            try
            {
                var success = await _restauranteService.DeactivateAsync(id);
                
                if (!success)
                    return NotFound($"Restaurant with ID {id} not found");

                return Ok(new { message = "Restaurant deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating restaurant");
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion
    }
}
