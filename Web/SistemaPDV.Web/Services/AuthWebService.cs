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
				var content = new StringContent(JsonSerializer.Serialize(dto, _jsonOptions), Encoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync("api/auth/login", content);
				var json = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					var result = JsonSerializer.Deserialize<AuthResponseDto>(json, _jsonOptions) ?? new AuthResponseDto { Sucesso = false, Erro = "Resposta inválida" };
					if (!string.IsNullOrWhiteSpace(result.Token))
					{
						SetToken(result.Token);
					}
					return result;
				}

				_logger.LogWarning("Falha no login: {Status} - {Body}", response.StatusCode, json);
				return new AuthResponseDto { Sucesso = false, Erro = json };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro no login");
				return new AuthResponseDto { Sucesso = false, Erro = "Erro interno" };
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

