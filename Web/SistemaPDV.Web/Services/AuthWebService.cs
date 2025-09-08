using SistemaPDV.Core.DTOs;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SistemaPDV.Web.Services
{
	public interface IAuthWebService
	{
		Task<AuthResponseDto> LoginAsync(LoginDto dto);
		Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
		string? GetToken();
		void SetToken(string? token);
	}

	public class AuthWebService : IAuthWebService
	{
		private readonly HttpClient _httpClient;
		private readonly ILogger<AuthWebService> _logger;
		private readonly JsonSerializerOptions _jsonOptions = new()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = false
		};

		private string? _token;

		public AuthWebService(HttpClient httpClient, ILogger<AuthWebService> logger)
		{
			_httpClient = httpClient;
			_logger = logger;
		}

		public string? GetToken() => _token;

		public void SetToken(string? token)
		{
			_token = token;
			if (!string.IsNullOrWhiteSpace(token))
			{
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}
			else
			{
				_httpClient.DefaultRequestHeaders.Authorization = null;
			}
		}

		public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
		{
			try
			{
				_logger.LogInformation("Tentando login para: {Email}", dto.Email);
				
				var content = new StringContent(JsonSerializer.Serialize(dto, _jsonOptions), Encoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync("api/auth/login", content);
				var json = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("Resposta da API: Status={Status}, Body={Body}", response.StatusCode, json);

				if (response.IsSuccessStatusCode)
				{
					var result = JsonSerializer.Deserialize<AuthResponseDto>(json, _jsonOptions) ?? new AuthResponseDto { Sucesso = false, Erro = "Resposta inválida" };
					if (!string.IsNullOrWhiteSpace(result.Token))
					{
						SetToken(result.Token);
						_logger.LogInformation("Login bem-sucedido para: {Email}", dto.Email);
					}
					return result;
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
				{
					_logger.LogWarning("Login negado para: {Email}", dto.Email);
					return new AuthResponseDto { Sucesso = false, Erro = "Email ou senha incorretos" };
				}
				else
				{
					_logger.LogError("Erro na API: {Status} - {Body}", response.StatusCode, json);
					return new AuthResponseDto { Sucesso = false, Erro = $"Erro do servidor: {response.StatusCode}" };
				}
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "Erro de conexão na API");
				throw; // Re-throw para indicar que API está offline
			}
			catch (TaskCanceledException ex)
			{
				_logger.LogError(ex, "Timeout na API");
				return new AuthResponseDto { Sucesso = false, Erro = "Timeout na conexão com servidor" };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro inesperado no login");
				return new AuthResponseDto { Sucesso = false, Erro = "Erro interno do sistema" };
			}
		}

		public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
		{
			try
			{
				var content = new StringContent(JsonSerializer.Serialize(dto, _jsonOptions), Encoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync("api/auth/register", content);
				var json = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					var result = JsonSerializer.Deserialize<AuthResponseDto>(json, _jsonOptions) ?? new AuthResponseDto { Sucesso = false, Erro = "Resposta inválida" };
					return result;
				}

				_logger.LogWarning("Falha no registro: {Status} - {Body}", response.StatusCode, json);
				return new AuthResponseDto { Sucesso = false, Erro = json };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro no registro");
				return new AuthResponseDto { Sucesso = false, Erro = "Erro interno" };
			}
		}
	}
}

