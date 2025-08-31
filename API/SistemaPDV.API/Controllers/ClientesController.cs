using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;
using SistemaPDV.Infrastructure.Data;

namespace SistemaPDV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly PDVDbContext _context;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(PDVDbContext context, ILogger<ClientesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todos os clientes ativos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> ObterTodos()
        {
            try
            {
                var clientes = await _context.Clientes
                    .Where(c => c.Ativo)
                    .Select(c => new ClienteDto
                    {
                        Id = c.Id,
                        Nome = c.Nome,
                        Email = c.Email,
                        Telefone = c.Telefone,
                        CPF = c.CPF,
                        CNPJ = c.CNPJ,
                        DataCadastro = c.DataCadastro,
                        Ativo = c.Ativo,
                        Observacoes = c.Observacoes
                    })
                    .ToListAsync();

                return Ok(clientes);
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
                var cliente = await _context.Clientes
                    .Where(c => c.Id == id)
                    .Select(c => new ClienteDto
                    {
                        Id = c.Id,
                        Nome = c.Nome,
                        Email = c.Email,
                        Telefone = c.Telefone,
                        CPF = c.CPF,
                        CNPJ = c.CNPJ,
                        DataCadastro = c.DataCadastro,
                        Ativo = c.Ativo,
                        Observacoes = c.Observacoes
                    })
                    .FirstOrDefaultAsync();

                if (cliente == null)
                    return NotFound($"Cliente com ID {id} não encontrado");

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
                _logger.LogInformation("Tentativa de criar cliente: {@ClienteDto}", clienteDto);
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState inválido: {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                // Validações de duplicidade
                if (!string.IsNullOrWhiteSpace(clienteDto.Email))
                {
                    var emailExiste = await _context.Clientes
                        .AnyAsync(c => c.Email == clienteDto.Email);
                    if (emailExiste)
                        return BadRequest("Email já cadastrado para outro cliente");
                }

                if (!string.IsNullOrWhiteSpace(clienteDto.CPF))
                {
                    var cpfExiste = await _context.Clientes
                        .AnyAsync(c => c.CPF == clienteDto.CPF);
                    if (cpfExiste)
                        return BadRequest("CPF já cadastrado para outro cliente");
                }

                if (!string.IsNullOrWhiteSpace(clienteDto.CNPJ))
                {
                    var cnpjExiste = await _context.Clientes
                        .AnyAsync(c => c.CNPJ == clienteDto.CNPJ);
                    if (cnpjExiste)
                        return BadRequest("CNPJ já cadastrado para outro cliente");
                }

                var cliente = new Cliente
                {
                    Nome = clienteDto.Nome,
                    Email = clienteDto.Email,
                    Telefone = clienteDto.Telefone ?? string.Empty,
                    CPF = clienteDto.CPF,
                    CNPJ = clienteDto.CNPJ,
                    DataCadastro = DateTime.Now,
                    Ativo = true,
                    Observacoes = clienteDto.Observacoes,
                    RestauranteId = 1 // Por enquanto, usar um valor padrão
                };

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                var clienteResult = new ClienteDto
                {
                    Id = cliente.Id,
                    Nome = cliente.Nome,
                    Email = cliente.Email,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF,
                    CNPJ = cliente.CNPJ,
                    DataCadastro = cliente.DataCadastro,
                    Ativo = cliente.Ativo,
                    Observacoes = cliente.Observacoes
                };

                return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, clienteResult);
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
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                    return NotFound($"Cliente com ID {id} não encontrado");

                // Validações de duplicidade (excluindo o próprio registro)
                if (!string.IsNullOrWhiteSpace(clienteDto.Email))
                {
                    var emailExiste = await _context.Clientes
                        .AnyAsync(c => c.Email == clienteDto.Email && c.Id != id);
                    if (emailExiste)
                        return BadRequest("Email já cadastrado para outro cliente");
                }

                if (!string.IsNullOrWhiteSpace(clienteDto.CPF))
                {
                    var cpfExiste = await _context.Clientes
                        .AnyAsync(c => c.CPF == clienteDto.CPF && c.Id != id);
                    if (cpfExiste)
                        return BadRequest("CPF já cadastrado para outro cliente");
                }

                if (!string.IsNullOrWhiteSpace(clienteDto.CNPJ))
                {
                    var cnpjExiste = await _context.Clientes
                        .AnyAsync(c => c.CNPJ == clienteDto.CNPJ && c.Id != id);
                    if (cnpjExiste)
                        return BadRequest("CNPJ já cadastrado para outro cliente");
                }

                // Atualizar propriedades
                cliente.Nome = clienteDto.Nome;
                cliente.Email = clienteDto.Email;
                cliente.Telefone = clienteDto.Telefone ?? string.Empty;
                cliente.CPF = clienteDto.CPF;
                cliente.CNPJ = clienteDto.CNPJ;
                cliente.Ativo = clienteDto.Ativo;
                cliente.Observacoes = clienteDto.Observacoes;

                await _context.SaveChangesAsync();

                var clienteResult = new ClienteDto
                {
                    Id = cliente.Id,
                    Nome = cliente.Nome,
                    Email = cliente.Email,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF,
                    CNPJ = cliente.CNPJ,
                    DataCadastro = cliente.DataCadastro,
                    Ativo = cliente.Ativo,
                    Observacoes = cliente.Observacoes
                };

                return Ok(clienteResult);
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
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                    return NotFound($"Cliente com ID {id} não encontrado");

                cliente.Ativo = false;
                await _context.SaveChangesAsync();

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
        public async Task<ActionResult<IEnumerable<ClienteDto>>> Buscar([FromQuery] string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo))
                {
                    return await ObterTodos();
                }

                var clientes = await _context.Clientes
                    .Where(c => c.Ativo && (
                        c.Nome.Contains(termo) ||
                        (c.Email != null && c.Email.Contains(termo)) ||
                        (c.Telefone != null && c.Telefone.Contains(termo)) ||
                        (c.CPF != null && c.CPF.Contains(termo)) ||
                        (c.CNPJ != null && c.CNPJ.Contains(termo))
                    ))
                    .Select(c => new ClienteDto
                    {
                        Id = c.Id,
                        Nome = c.Nome,
                        Email = c.Email,
                        Telefone = c.Telefone,
                        CPF = c.CPF,
                        CNPJ = c.CNPJ,
                        DataCadastro = c.DataCadastro,
                        Ativo = c.Ativo,
                        Observacoes = c.Observacoes
                    })
                    .ToListAsync();

                return Ok(clientes);
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

                var existe = await _context.Clientes
                    .AnyAsync(c => c.Email == email && (idExcluir == null || c.Id != idExcluir));

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

                var existe = await _context.Clientes
                    .AnyAsync(c => c.CPF == cpf && (idExcluir == null || c.Id != idExcluir));

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

                var existe = await _context.Clientes
                    .AnyAsync(c => c.CNPJ == cnpj && (idExcluir == null || c.Id != idExcluir));

                return Ok(new { existe });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar CNPJ {CNPJ}", cnpj);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
