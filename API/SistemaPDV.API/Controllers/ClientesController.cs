using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Interfaces;

namespace SistemaPDV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IClienteService clienteService, ILogger<ClientesController> logger)
        {
            _clienteService = clienteService;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todos os clientes ativos com paginação
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginacaoResultadoDto<ClienteDto>>> ObterTodos([FromQuery] PaginacaoParametrosDto parametros)
        {
            try
            {
                _logger.LogInformation("Obtaining all clients with pagination for restaurant");

                // Para o método clássico com paginação, vamos buscar todos e aplicar paginação no controlador
                var todosClientes = await _clienteService.GetAllAsync();

                // Aplicar filtro de pesquisa se fornecido
                if (!string.IsNullOrWhiteSpace(parametros.Termo))
                {
                    todosClientes = await _clienteService.SearchAsync(parametros.Termo);
                }

                // Aplicar ordenação
                var query = todosClientes.AsQueryable();
                if (!string.IsNullOrWhiteSpace(parametros.OrdenarPor))
                {
                    switch (parametros.OrdenarPor.ToLower())
                    {
                        case "nome":
                            query = parametros.OrdemDecrescente ? query.OrderByDescending(c => c.Nome) : query.OrderBy(c => c.Nome);
                            break;
                        case "email":
                            query = parametros.OrdemDecrescente ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email);
                            break;
                        case "datacadastro":
                            query = parametros.OrdemDecrescente ? query.OrderByDescending(c => c.DataCadastro) : query.OrderBy(c => c.DataCadastro);
                            break;
                        default:
                            query = query.OrderBy(c => c.Nome);
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(c => c.Nome);
                }

                // Contar total de itens
                var totalItens = query.Count();

                // Aplicar paginação
                var clientes = query
                    .Skip((parametros.Pagina - 1) * parametros.TamanhoPagina)
                    .Take(parametros.TamanhoPagina)
                    .ToList();

                var resultado = new PaginacaoResultadoDto<ClienteDto>
                {
                    Dados = clientes,
                    TotalItens = totalItens,
                    PaginaAtual = parametros.Pagina,
                    TamanhoPagina = parametros.TamanhoPagina
                };

                _logger.LogInformation("Successfully obtained {TotalItens} clients with pagination", totalItens);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter clientes");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obtém um cliente por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> ObterPorId(int id)
        {
            try
            {
                _logger.LogInformation("Obtaining client by ID: {ClientId}", id);

                var cliente = await _clienteService.GetByIdAsync(id);

                if (cliente == null)
                {
                    _logger.LogWarning("Client not found: {ClientId}", id);
                    return NotFound($"Cliente com ID {id} não encontrado");
                }

                _logger.LogInformation("Successfully obtained client: {ClientId} - {Nome}", id, cliente.Nome);
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter cliente {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> Criar([FromBody] ClienteCriacaoDto clienteDto)
        {
            try
            {
                _logger.LogInformation("Attempting to create client: {@ClienteDto}", clienteDto);
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid ModelState: {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                var clienteResult = await _clienteService.CreateAsync(clienteDto);

                _logger.LogInformation("Successfully created client: {ClientId} - {Nome}", clienteResult.Id, clienteResult.Nome);
                return CreatedAtAction(nameof(ObterPorId), new { id = clienteResult.Id }, clienteResult);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating client: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cliente");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Atualiza um cliente existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ClienteDto>> Atualizar(int id, [FromBody] ClienteAtualizacaoDto clienteDto)
        {
            try
            {
                _logger.LogInformation("Attempting to update client: {ClientId}", id);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid ModelState for client update: {ClientId}", id);
                    return BadRequest(ModelState);
                }

                var clienteResult = await _clienteService.UpdateAsync(id, clienteDto);
                
                if (clienteResult == null)
                {
                    _logger.LogWarning("Client not found for update: {ClientId}", id);
                    return NotFound($"Cliente com ID {id} não encontrado");
                }

                _logger.LogInformation("Successfully updated client: {ClientId} - {Nome}", id, clienteResult.Nome);
                return Ok(clienteResult);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error updating client {ClientId}: {Message}", id, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Exclui um cliente (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Deletar(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete client: {ClientId}", id);

                var sucesso = await _clienteService.DeleteAsync(id);
                
                if (!sucesso)
                {
                    _logger.LogWarning("Client not found for deletion: {ClientId}", id);
                    return NotFound($"Cliente com ID {id} não encontrado");
                }

                _logger.LogInformation("Successfully deleted client: {ClientId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar cliente {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Busca clientes por termo (nome, email, telefone, CPF ou CNPJ)
        /// </summary>
        [HttpGet("buscar")]
        public async Task<ActionResult<PaginacaoResultadoDto<ClienteDto>>> Buscar([FromQuery] string termo, [FromQuery] PaginacaoParametrosDto? parametros = null)
        {
            try
            {
                _logger.LogInformation("Searching clients with term: {Termo}", termo);

                // Se não há termo, usar parâmetros padrão
                if (string.IsNullOrWhiteSpace(termo))
                {
                    parametros ??= new PaginacaoParametrosDto();
                    return await ObterTodos(parametros);
                }

                // Usar parâmetros fornecidos ou padrão, mas com o termo de pesquisa
                parametros ??= new PaginacaoParametrosDto();
                parametros.Termo = termo;
                
                return await ObterTodos(parametros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar clientes com termo {Termo}", termo);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Verifica se um email já está cadastrado
        /// </summary>
        [HttpGet("verificar-email")]
        public async Task<ActionResult<bool>> VerificarEmail([FromQuery] string email, [FromQuery] int? idExcluir = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest("Email é obrigatório");

                var existe = await _clienteService.EmailExistsAsync(email, idExcluir);

                return Ok(new { existe });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar email {Email}", email);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Verifica se um CPF já está cadastrado
        /// </summary>
        [HttpGet("verificar-cpf")]
        public async Task<ActionResult<bool>> VerificarCPF([FromQuery] string cpf, [FromQuery] int? idExcluir = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cpf))
                    return BadRequest("CPF é obrigatório");

                var existe = await _clienteService.CpfExistsAsync(cpf, idExcluir);

                return Ok(new { existe });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar CPF {CPF}", cpf);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Verifica se um CNPJ já está cadastrado
        /// </summary>
        [HttpGet("verificar-cnpj")]
        public async Task<ActionResult<bool>> VerificarCNPJ([FromQuery] string cnpj, [FromQuery] int? idExcluir = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cnpj))
                    return BadRequest("CNPJ é obrigatório");

                var existe = await _clienteService.CnpjExistsAsync(cnpj, idExcluir);

                return Ok(new { existe });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar CNPJ {CNPJ}", cnpj);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        #region Modern API Endpoints (English)

        /// <summary>
        /// Get all active clients (modern endpoint)
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
        {
            try
            {
                var clientes = await _clienteService.GetAllAsync();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all clients");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get recent clients (last 30 days)
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetRecent([FromQuery] int days = 30)
        {
            try
            {
                var clientes = await _clienteService.GetRecentClientsAsync(days);
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent clients");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Search clients by term (modern endpoint)
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> Search([FromQuery] string term)
        {
            try
            {
                var clientes = await _clienteService.SearchAsync(term);
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching clients");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Check if client exists by ID
        /// </summary>
        [HttpGet("{id}/exists")]
        public async Task<ActionResult<bool>> Exists(int id)
        {
            try
            {
                var exists = await _clienteService.ClientExistsAsync(id);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking client existence");
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion
    }
}
