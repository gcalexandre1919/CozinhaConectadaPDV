using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;
using SistemaPDV.Infrastructure.Data;

namespace SistemaPDV.Infrastructure.Services
{
    public class ClienteService : IClienteService
    {
        private readonly PDVDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(PDVDbContext context, ICurrentUserService currentUserService, ILogger<ClienteService> logger)
        {
            _context = context;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        #region Modern Client Methods

        public async Task<IEnumerable<ClienteDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all clients for restaurant: {RestaurantId}", _currentUserService.GetRestauranteId());

                var clientes = await _context.Clientes
                    .AsNoTracking()
                    .Where(c => c.Ativo && c.RestauranteId == _currentUserService.GetRestauranteId())
                    .OrderBy(c => c.Nome)
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

                _logger.LogInformation("Successfully retrieved {Count} clients", clientes.Count());
                return clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clients for restaurant: {RestaurantId}", _currentUserService.GetRestauranteId());
                throw;
            }
        }

        public async Task<ClienteDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving client: {ClientId} for restaurant: {RestaurantId}", id, _currentUserService.GetRestauranteId());

                var cliente = await _context.Clientes
                    .AsNoTracking()
                    .Where(c => c.Id == id && c.RestauranteId == _currentUserService.GetRestauranteId())
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
                {
                    _logger.LogWarning("Client not found: {ClientId} for restaurant: {RestaurantId}", id, _currentUserService.GetRestauranteId());
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved client: {ClientId}", id);
                }

                return cliente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving client: {ClientId}", id);
                throw;
            }
        }

        public async Task<ClienteDto> CreateAsync(ClienteCriacaoDto clienteDto)
        {
            try
            {
                _logger.LogInformation("Creating new client: {Nome} for restaurant: {RestaurantId}", 
                    clienteDto.Nome, _currentUserService.GetRestauranteId());

                // Validations
                await ValidateUniqueFieldsAsync(clienteDto.Email, clienteDto.CPF, clienteDto.CNPJ);

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
                    RestauranteId = _currentUserService.GetRestauranteId()
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

                _logger.LogInformation("Successfully created client: {ClientId} - {Nome}", cliente.Id, cliente.Nome);
                return clienteResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client: {Nome}", clienteDto.Nome);
                throw;
            }
        }

        public async Task<ClienteDto?> UpdateAsync(int id, ClienteAtualizacaoDto clienteDto)
        {
            try
            {
                _logger.LogInformation("Updating client: {ClientId} for restaurant: {RestaurantId}", 
                    id, _currentUserService.GetRestauranteId());

                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.Id == id && c.RestauranteId == _currentUserService.GetRestauranteId());

                if (cliente == null)
                {
                    _logger.LogWarning("Client not found for update: {ClientId}", id);
                    return null;
                }

                // Validations (excluding current record)
                await ValidateUniqueFieldsAsync(clienteDto.Email, clienteDto.CPF, clienteDto.CNPJ, id);

                // Update properties
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

                _logger.LogInformation("Successfully updated client: {ClientId} - {Nome}", id, cliente.Nome);
                return clienteResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client: {ClientId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting client: {ClientId} for restaurant: {RestaurantId}", 
                    id, _currentUserService.GetRestauranteId());

                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.Id == id && c.RestauranteId == _currentUserService.GetRestauranteId());

                if (cliente == null)
                {
                    _logger.LogWarning("Client not found for deletion: {ClientId}", id);
                    return false;
                }

                cliente.Ativo = false;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted (soft delete) client: {ClientId} - {Nome}", id, cliente.Nome);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting client: {ClientId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ClienteDto>> SearchAsync(string termo)
        {
            try
            {
                _logger.LogInformation("Searching clients with term: {Termo} for restaurant: {RestaurantId}", 
                    termo, _currentUserService.GetRestauranteId());

                if (string.IsNullOrWhiteSpace(termo))
                {
                    return await GetAllAsync();
                }

                var clientes = await _context.Clientes
                    .AsNoTracking()
                    .Where(c => c.Ativo && c.RestauranteId == _currentUserService.GetRestauranteId() &&
                               (c.Nome.Contains(termo) ||
                                (c.Email != null && c.Email.Contains(termo)) ||
                                (c.Telefone != null && c.Telefone.Contains(termo)) ||
                                (c.CPF != null && c.CPF.Contains(termo)) ||
                                (c.CNPJ != null && c.CNPJ.Contains(termo))))
                    .OrderBy(c => c.Nome)
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

                _logger.LogInformation("Search completed with {Count} results for term: {Termo}", clientes.Count(), termo);
                return clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching clients with term: {Termo}", termo);
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                var exists = await _context.Clientes
                    .AsNoTracking()
                    .AnyAsync(c => c.Email == email && 
                                  (excludeId == null || c.Id != excludeId) && 
                                  c.RestauranteId == _currentUserService.GetRestauranteId());

                _logger.LogInformation("Email existence check: {Email} - Exists: {Exists}", email, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email existence: {Email}", email);
                throw;
            }
        }

        public async Task<bool> CpfExistsAsync(string cpf, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cpf))
                    return false;

                var exists = await _context.Clientes
                    .AsNoTracking()
                    .AnyAsync(c => c.CPF == cpf && 
                                  (excludeId == null || c.Id != excludeId) && 
                                  c.RestauranteId == _currentUserService.GetRestauranteId());

                _logger.LogInformation("CPF existence check: {CPF} - Exists: {Exists}", cpf, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking CPF existence: {CPF}", cpf);
                throw;
            }
        }

        public async Task<bool> CnpjExistsAsync(string cnpj, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cnpj))
                    return false;

                var exists = await _context.Clientes
                    .AsNoTracking()
                    .AnyAsync(c => c.CNPJ == cnpj && 
                                  (excludeId == null || c.Id != excludeId) && 
                                  c.RestauranteId == _currentUserService.GetRestauranteId());

                _logger.LogInformation("CNPJ existence check: {CNPJ} - Exists: {Exists}", cnpj, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking CNPJ existence: {CNPJ}", cnpj);
                throw;
            }
        }

        #endregion

        #region Advanced Filter Methods

        public async Task<IEnumerable<ClienteDto>> GetActiveClientsAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving active clients for restaurant: {RestaurantId}", _currentUserService.GetRestauranteId());

                var clientes = await _context.Clientes
                    .AsNoTracking()
                    .Where(c => c.Ativo && c.RestauranteId == _currentUserService.GetRestauranteId())
                    .OrderBy(c => c.Nome)
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

                _logger.LogInformation("Successfully retrieved {Count} active clients", clientes.Count());
                return clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active clients");
                throw;
            }
        }

        public async Task<IEnumerable<ClienteDto>> GetRecentClientsAsync(int days = 30)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-days);
                _logger.LogInformation("Retrieving clients registered in last {Days} days for restaurant: {RestaurantId}", 
                    days, _currentUserService.GetRestauranteId());

                var clientes = await _context.Clientes
                    .AsNoTracking()
                    .Where(c => c.Ativo && 
                               c.RestauranteId == _currentUserService.GetRestauranteId() &&
                               c.DataCadastro >= cutoffDate)
                    .OrderByDescending(c => c.DataCadastro)
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

                _logger.LogInformation("Successfully retrieved {Count} recent clients", clientes.Count());
                return clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent clients");
                throw;
            }
        }

        public async Task<bool> ClientExistsAsync(int id)
        {
            try
            {
                var exists = await _context.Clientes
                    .AsNoTracking()
                    .AnyAsync(c => c.Id == id && c.RestauranteId == _currentUserService.GetRestauranteId());

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking client existence: {ClientId}", id);
                throw;
            }
        }

        #endregion

        #region Legacy Methods (Backward Compatibility)

        [Obsolete("Use GetAllAsync instead")]
        public async Task<IEnumerable<ClienteDto>> ObterTodosAsync()
        {
            return await GetAllAsync();
        }

        [Obsolete("Use GetByIdAsync instead")]
        public async Task<ClienteDto?> ObterPorIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        [Obsolete("Use CreateAsync instead")]
        public async Task<ClienteDto> CriarAsync(ClienteCriacaoDto clienteDto)
        {
            return await CreateAsync(clienteDto);
        }

        [Obsolete("Use UpdateAsync instead")]
        public async Task<ClienteDto?> AtualizarAsync(int id, ClienteAtualizacaoDto clienteDto)
        {
            return await UpdateAsync(id, clienteDto);
        }

        [Obsolete("Use DeleteAsync instead")]
        public async Task<bool> DeletarAsync(int id)
        {
            return await DeleteAsync(id);
        }

        [Obsolete("Use SearchAsync instead")]
        public async Task<IEnumerable<ClienteDto>> BuscarAsync(string termo)
        {
            return await SearchAsync(termo);
        }

        [Obsolete("Use EmailExistsAsync instead")]
        public async Task<bool> ExisteEmailAsync(string email, int? idExcluir = null)
        {
            return await EmailExistsAsync(email, idExcluir);
        }

        [Obsolete("Use CpfExistsAsync instead")]
        public async Task<bool> ExisteCPFAsync(string cpf, int? idExcluir = null)
        {
            return await CpfExistsAsync(cpf, idExcluir);
        }

        [Obsolete("Use CnpjExistsAsync instead")]
        public async Task<bool> ExisteCNPJAsync(string cnpj, int? idExcluir = null)
        {
            return await CnpjExistsAsync(cnpj, idExcluir);
        }

        #endregion

        #region Private Helper Methods

        private async Task ValidateUniqueFieldsAsync(string? email, string? cpf, string? cnpj, int? excludeId = null)
        {
            if (!string.IsNullOrWhiteSpace(email) && await EmailExistsAsync(email, excludeId))
            {
                throw new ArgumentException("Email já cadastrado para outro cliente");
            }

            if (!string.IsNullOrWhiteSpace(cpf) && await CpfExistsAsync(cpf, excludeId))
            {
                throw new ArgumentException("CPF já cadastrado para outro cliente");
            }

            if (!string.IsNullOrWhiteSpace(cnpj) && await CnpjExistsAsync(cnpj, excludeId))
            {
                throw new ArgumentException("CNPJ já cadastrado para outro cliente");
            }
        }

        #endregion
    }
}