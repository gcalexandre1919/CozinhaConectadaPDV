using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;
using SistemaPDV.Infrastructure.Data;

namespace SistemaPDV.Infrastructure.Services
{
    public class RestauranteService : IRestauranteService
    {
        private readonly PDVDbContext _context;
        private readonly ILogger<RestauranteService> _logger;

        public RestauranteService(PDVDbContext context, ILogger<RestauranteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Modern Restaurant Methods (English)

        public async Task<IEnumerable<RestauranteDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all restaurants");

                var restaurantes = await _context.Restaurantes
                    .AsNoTracking()
                    .OrderBy(r => r.Nome)
                    .Select(r => new RestauranteDto
                    {
                        Id = r.Id,
                        Nome = r.Nome,
                        CNPJ = r.CNPJ,
                        Endereco = r.Endereco,
                        Numero = r.Numero,
                        Complemento = r.Complemento,
                        Bairro = r.Bairro,
                        Cidade = r.Cidade,
                        UF = r.UF,
                        CEP = r.CEP,
                        Telefone = r.Telefone,
                        Email = r.Email,
                        DataCadastro = r.DataCadastro,
                        Ativo = r.Ativo
                    })
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} restaurants", restaurantes.Count());
                return restaurantes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all restaurants");
                throw;
            }
        }

        public async Task<IEnumerable<RestauranteDto>> GetActiveAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving active restaurants");

                var restaurantes = await _context.Restaurantes
                    .AsNoTracking()
                    .Where(r => r.Ativo)
                    .OrderBy(r => r.Nome)
                    .Select(r => new RestauranteDto
                    {
                        Id = r.Id,
                        Nome = r.Nome,
                        CNPJ = r.CNPJ,
                        Endereco = r.Endereco,
                        Numero = r.Numero,
                        Complemento = r.Complemento,
                        Bairro = r.Bairro,
                        Cidade = r.Cidade,
                        UF = r.UF,
                        CEP = r.CEP,
                        Telefone = r.Telefone,
                        Email = r.Email,
                        DataCadastro = r.DataCadastro,
                        Ativo = r.Ativo
                    })
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} active restaurants", restaurantes.Count());
                return restaurantes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active restaurants");
                throw;
            }
        }

        public async Task<RestauranteDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving restaurant by ID: {RestaurantId}", id);

                var restaurante = await _context.Restaurantes
                    .AsNoTracking()
                    .Where(r => r.Id == id)
                    .Select(r => new RestauranteDto
                    {
                        Id = r.Id,
                        Nome = r.Nome,
                        CNPJ = r.CNPJ,
                        Endereco = r.Endereco,
                        Numero = r.Numero,
                        Complemento = r.Complemento,
                        Bairro = r.Bairro,
                        Cidade = r.Cidade,
                        UF = r.UF,
                        CEP = r.CEP,
                        Telefone = r.Telefone,
                        Email = r.Email,
                        DataCadastro = r.DataCadastro,
                        Ativo = r.Ativo
                    })
                    .FirstOrDefaultAsync();

                if (restaurante == null)
                {
                    _logger.LogWarning("Restaurant not found: {RestaurantId}", id);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved restaurant: {RestaurantId} - {Nome}", id, restaurante.Nome);
                }

                return restaurante;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving restaurant: {RestaurantId}", id);
                throw;
            }
        }

        public async Task<RestauranteDto> CreateAsync(RestauranteCriacaoDto restauranteDto)
        {
            try
            {
                _logger.LogInformation("Creating new restaurant: {Nome}", restauranteDto.Nome);

                // Validations
                await ValidateUniqueFieldsAsync(restauranteDto.Nome, restauranteDto.CNPJ);

                var restaurante = new Restaurante
                {
                    Nome = restauranteDto.Nome,
                    CNPJ = restauranteDto.CNPJ,
                    Endereco = restauranteDto.Endereco,
                    Numero = restauranteDto.Numero,
                    Complemento = restauranteDto.Complemento,
                    Bairro = restauranteDto.Bairro,
                    Cidade = restauranteDto.Cidade,
                    UF = restauranteDto.UF,
                    CEP = restauranteDto.CEP,
                    Telefone = restauranteDto.Telefone,
                    Email = restauranteDto.Email,
                    DataCadastro = DateTime.Now,
                    Ativo = true
                };

                _context.Restaurantes.Add(restaurante);
                await _context.SaveChangesAsync();

                var restauranteResult = new RestauranteDto
                {
                    Id = restaurante.Id,
                    Nome = restaurante.Nome,
                    CNPJ = restaurante.CNPJ,
                    Endereco = restaurante.Endereco,
                    Numero = restaurante.Numero,
                    Complemento = restaurante.Complemento,
                    Bairro = restaurante.Bairro,
                    Cidade = restaurante.Cidade,
                    UF = restaurante.UF,
                    CEP = restaurante.CEP,
                    Telefone = restaurante.Telefone,
                    Email = restaurante.Email,
                    DataCadastro = restaurante.DataCadastro,
                    Ativo = restaurante.Ativo
                };

                _logger.LogInformation("Successfully created restaurant: {RestaurantId} - {Nome}", restaurante.Id, restaurante.Nome);
                return restauranteResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating restaurant: {Nome}", restauranteDto.Nome);
                throw;
            }
        }

        public async Task<RestauranteDto?> UpdateAsync(int id, RestauranteAtualizacaoDto restauranteDto)
        {
            try
            {
                _logger.LogInformation("Updating restaurant: {RestaurantId}", id);

                var restaurante = await _context.Restaurantes
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (restaurante == null)
                {
                    _logger.LogWarning("Restaurant not found for update: {RestaurantId}", id);
                    return null;
                }

                // Validations (excluding current record)
                await ValidateUniqueFieldsAsync(restauranteDto.Nome, restauranteDto.CNPJ, id);

                // Update properties
                restaurante.Nome = restauranteDto.Nome;
                restaurante.CNPJ = restauranteDto.CNPJ;
                restaurante.Endereco = restauranteDto.Endereco;
                restaurante.Numero = restauranteDto.Numero;
                restaurante.Complemento = restauranteDto.Complemento;
                restaurante.Bairro = restauranteDto.Bairro;
                restaurante.Cidade = restauranteDto.Cidade;
                restaurante.UF = restauranteDto.UF;
                restaurante.CEP = restauranteDto.CEP;
                restaurante.Telefone = restauranteDto.Telefone;
                restaurante.Email = restauranteDto.Email;
                restaurante.Ativo = restauranteDto.Ativo;

                await _context.SaveChangesAsync();

                var restauranteResult = new RestauranteDto
                {
                    Id = restaurante.Id,
                    Nome = restaurante.Nome,
                    CNPJ = restaurante.CNPJ,
                    Endereco = restaurante.Endereco,
                    Numero = restaurante.Numero,
                    Complemento = restaurante.Complemento,
                    Bairro = restaurante.Bairro,
                    Cidade = restaurante.Cidade,
                    UF = restaurante.UF,
                    CEP = restaurante.CEP,
                    Telefone = restaurante.Telefone,
                    Email = restaurante.Email,
                    DataCadastro = restaurante.DataCadastro,
                    Ativo = restaurante.Ativo
                };

                _logger.LogInformation("Successfully updated restaurant: {RestaurantId} - {Nome}", id, restaurante.Nome);
                return restauranteResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating restaurant: {RestaurantId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting restaurant: {RestaurantId}", id);

                var restaurante = await _context.Restaurantes
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (restaurante == null)
                {
                    _logger.LogWarning("Restaurant not found for deletion: {RestaurantId}", id);
                    return false;
                }

                // Check if restaurant has orders
                var temPedidos = await _context.Pedidos.AnyAsync(p => p.RestauranteId == id);
                
                if (temPedidos)
                {
                    // Soft delete if has orders
                    restaurante.Ativo = false;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Restaurant soft deleted (has orders): {RestaurantId} - {Nome}", id, restaurante.Nome);
                }
                else
                {
                    // Hard delete if no orders
                    _context.Restaurantes.Remove(restaurante);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Restaurant hard deleted (no orders): {RestaurantId} - {Nome}", id, restaurante.Nome);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting restaurant: {RestaurantId}", id);
                throw;
            }
        }

        public async Task<bool> ActivateAsync(int id)
        {
            try
            {
                _logger.LogInformation("Activating restaurant: {RestaurantId}", id);

                var restaurante = await _context.Restaurantes
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (restaurante == null)
                {
                    _logger.LogWarning("Restaurant not found for activation: {RestaurantId}", id);
                    return false;
                }

                restaurante.Ativo = true;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully activated restaurant: {RestaurantId} - {Nome}", id, restaurante.Nome);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating restaurant: {RestaurantId}", id);
                throw;
            }
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deactivating restaurant: {RestaurantId}", id);

                var restaurante = await _context.Restaurantes
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (restaurante == null)
                {
                    _logger.LogWarning("Restaurant not found for deactivation: {RestaurantId}", id);
                    return false;
                }

                restaurante.Ativo = false;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deactivated restaurant: {RestaurantId} - {Nome}", id, restaurante.Nome);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating restaurant: {RestaurantId}", id);
                throw;
            }
        }

        public async Task<RestauranteEstatisticasDto?> GetStatisticsAsync(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving statistics for restaurant: {RestaurantId}", id);

                var restaurante = await _context.Restaurantes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (restaurante == null)
                {
                    _logger.LogWarning("Restaurant not found for statistics: {RestaurantId}", id);
                    return null;
                }

                var hoje = DateTime.Today;
                var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

                var estatisticas = new RestauranteEstatisticasDto
                {
                    Id = restaurante.Id,
                    Nome = restaurante.Nome,
                    PedidosHoje = await _context.Pedidos
                        .AsNoTracking()
                        .CountAsync(p => p.RestauranteId == id && p.DataCriacao.Date == hoje),
                    PedidosMes = await _context.Pedidos
                        .AsNoTracking()
                        .CountAsync(p => p.RestauranteId == id && p.DataCriacao >= inicioMes),
                    VendasHoje = await _context.Pedidos
                        .AsNoTracking()
                        .Where(p => p.RestauranteId == id && p.DataCriacao.Date == hoje && p.Status != StatusPedido.Cancelado)
                        .SumAsync(p => (decimal?)p.ValorTotal) ?? 0,
                    VendasMes = await _context.Pedidos
                        .AsNoTracking()
                        .Where(p => p.RestauranteId == id && p.DataCriacao >= inicioMes && p.Status != StatusPedido.Cancelado)
                        .SumAsync(p => (decimal?)p.ValorTotal) ?? 0,
                    ClientesAtivos = await _context.Clientes
                        .AsNoTracking()
                        .Where(c => c.Pedidos.Any(p => p.RestauranteId == id) && c.Ativo)
                        .CountAsync(),
                    DataConsulta = DateTime.Now
                };

                _logger.LogInformation("Successfully retrieved statistics for restaurant: {RestaurantId} - {Nome}", id, restaurante.Nome);
                return estatisticas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics for restaurant: {RestaurantId}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var exists = await _context.Restaurantes
                    .AsNoTracking()
                    .AnyAsync(r => r.Id == id);

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking restaurant existence: {RestaurantId}", id);
                throw;
            }
        }

        public async Task<bool> NameExistsAsync(string nome, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nome))
                    return false;

                var exists = await _context.Restaurantes
                    .AsNoTracking()
                    .AnyAsync(r => r.Nome.ToLower() == nome.ToLower() && 
                                  (excludeId == null || r.Id != excludeId) && 
                                  r.Ativo);

                _logger.LogInformation("Name existence check: {Nome} - Exists: {Exists}", nome, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking name existence: {Nome}", nome);
                throw;
            }
        }

        public async Task<bool> CnpjExistsAsync(string cnpj, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cnpj))
                    return false;

                var exists = await _context.Restaurantes
                    .AsNoTracking()
                    .AnyAsync(r => r.CNPJ == cnpj && 
                                  (excludeId == null || r.Id != excludeId) && 
                                  r.Ativo);

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

        #region Legacy Methods (Backward Compatibility)

        [Obsolete("Use GetAllAsync instead")]
        public async Task<IEnumerable<RestauranteDto>> ObterTodosAsync()
        {
            return await GetAllAsync();
        }

        [Obsolete("Use GetActiveAsync instead")]
        public async Task<IEnumerable<RestauranteDto>> ObterAtivosAsync()
        {
            return await GetActiveAsync();
        }

        [Obsolete("Use GetByIdAsync instead")]
        public async Task<RestauranteDto?> ObterPorIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        [Obsolete("Use CreateAsync instead")]
        public async Task<RestauranteDto> CriarAsync(RestauranteCriacaoDto restauranteDto)
        {
            return await CreateAsync(restauranteDto);
        }

        [Obsolete("Use UpdateAsync instead")]
        public async Task<RestauranteDto?> AtualizarAsync(int id, RestauranteAtualizacaoDto restauranteDto)
        {
            return await UpdateAsync(id, restauranteDto);
        }

        [Obsolete("Use DeleteAsync instead")]
        public async Task<bool> DeletarAsync(int id)
        {
            return await DeleteAsync(id);
        }

        [Obsolete("Use ActivateAsync instead")]
        public async Task<bool> AtivarAsync(int id)
        {
            return await ActivateAsync(id);
        }

        [Obsolete("Use DeactivateAsync instead")]
        public async Task<bool> DesativarAsync(int id)
        {
            return await DeactivateAsync(id);
        }

        [Obsolete("Use GetStatisticsAsync instead")]
        public async Task<RestauranteEstatisticasDto?> ObterEstatisticasAsync(int id)
        {
            return await GetStatisticsAsync(id);
        }

        [Obsolete("Use ExistsAsync instead")]
        public async Task<bool> ExisteAsync(int id)
        {
            return await ExistsAsync(id);
        }

        [Obsolete("Use NameExistsAsync instead")]
        public async Task<bool> ExisteNomeAsync(string nome, int? idExcluir = null)
        {
            return await NameExistsAsync(nome, idExcluir);
        }

        [Obsolete("Use CnpjExistsAsync instead")]
        public async Task<bool> ExisteCNPJAsync(string cnpj, int? idExcluir = null)
        {
            return await CnpjExistsAsync(cnpj, idExcluir);
        }

        #endregion

        #region Private Helper Methods

        private async Task ValidateUniqueFieldsAsync(string nome, string? cnpj, int? excludeId = null)
        {
            if (await NameExistsAsync(nome, excludeId))
            {
                throw new ArgumentException("Já existe um restaurante com este nome");
            }

            if (!string.IsNullOrWhiteSpace(cnpj) && await CnpjExistsAsync(cnpj, excludeId))
            {
                throw new ArgumentException("Já existe um restaurante com este CNPJ");
            }
        }

        #endregion
    }
}
