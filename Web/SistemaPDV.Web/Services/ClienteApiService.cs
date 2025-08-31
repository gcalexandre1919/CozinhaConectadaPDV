using SistemaPDV.Core.DTOs;
using System.Text.Json;
using System.Text;

namespace SistemaPDV.Web.Services
{
    public class ClienteApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ClienteApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ClienteApiService(HttpClient httpClient, ILogger<ClienteApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<List<ClienteDto>> ObterTodosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/clientes");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<ClienteDto>>(json, _jsonOptions) ?? new List<ClienteDto>();
                }
                
                _logger.LogWarning("Erro ao obter clientes: {StatusCode}", response.StatusCode);
                return new List<ClienteDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter clientes");
                return new List<ClienteDto>();
            }
        }

        public async Task<ClienteDto?> ObterPorIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/clientes/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ClienteDto>(json, _jsonOptions);
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                    
                _logger.LogWarning("Erro ao obter cliente {Id}: {StatusCode}", id, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter cliente {Id}", id);
                return null;
            }
        }

        public async Task<(bool Sucesso, ClienteDto? Cliente, string? Erro)> CriarAsync(ClienteCriacaoDto clienteDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(clienteDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("api/clientes", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    var cliente = JsonSerializer.Deserialize<ClienteDto>(responseContent, _jsonOptions);
                    return (true, cliente, null);
                }
                
                return (false, null, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cliente");
                return (false, null, "Erro interno do sistema");
            }
        }

        public async Task<(bool Sucesso, ClienteDto? Cliente, string? Erro)> AtualizarAsync(int id, ClienteAtualizacaoDto clienteDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(clienteDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"api/clientes/{id}", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    var cliente = JsonSerializer.Deserialize<ClienteDto>(responseContent, _jsonOptions);
                    return (true, cliente, null);
                }
                
                return (false, null, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente {Id}", id);
                return (false, null, "Erro interno do sistema");
            }
        }

        public async Task<(bool Sucesso, string? Erro)> DeletarAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/clientes/{id}");
                
                if (response.IsSuccessStatusCode)
                    return (true, null);
                
                var erro = await response.Content.ReadAsStringAsync();
                return (false, erro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar cliente {Id}", id);
                return (false, "Erro interno do sistema");
            }
        }

        public async Task<List<ClienteDto>> BuscarAsync(string termo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/clientes/buscar?termo={Uri.EscapeDataString(termo ?? "")}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<ClienteDto>>(json, _jsonOptions) ?? new List<ClienteDto>();
                }
                
                _logger.LogWarning("Erro ao buscar clientes: {StatusCode}", response.StatusCode);
                return new List<ClienteDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar clientes");
                return new List<ClienteDto>();
            }
        }

        public async Task<bool> ExisteEmailAsync(string email, int? idExcluir = null)
        {
            try
            {
                var url = $"api/clientes/verificar-email?email={Uri.EscapeDataString(email)}";
                if (idExcluir.HasValue)
                    url += $"&idExcluir={idExcluir.Value}";
                    
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);
                    return result.GetProperty("existe").GetBoolean();
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar email");
                return false;
            }
        }

        public async Task<bool> ExisteCPFAsync(string cpf, int? idExcluir = null)
        {
            try
            {
                var url = $"api/clientes/verificar-cpf?cpf={Uri.EscapeDataString(cpf)}";
                if (idExcluir.HasValue)
                    url += $"&idExcluir={idExcluir.Value}";
                    
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);
                    return result.GetProperty("existe").GetBoolean();
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar CPF");
                return false;
            }
        }

        public async Task<bool> ExisteCNPJAsync(string cnpj, int? idExcluir = null)
        {
            try
            {
                var url = $"api/clientes/verificar-cnpj?cnpj={Uri.EscapeDataString(cnpj)}";
                if (idExcluir.HasValue)
                    url += $"&idExcluir={idExcluir.Value}";
                    
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);
                    return result.GetProperty("existe").GetBoolean();
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar CNPJ");
                return false;
            }
        }
    }
}
