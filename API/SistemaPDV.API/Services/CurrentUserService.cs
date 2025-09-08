using System.Security.Claims;
using SistemaPDV.Core.Interfaces;

namespace SistemaPDV.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CurrentUserService> _logger;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public int GetRestauranteId()
        {
            try
            {
                var restauranteIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("RestauranteId");
                if (restauranteIdClaim != null && int.TryParse(restauranteIdClaim.Value, out int restauranteId))
                {
                    return restauranteId;
                }

                _logger.LogWarning("RestauranteId não encontrado no contexto do usuário, usando valor padrão");
                return 1; // Valor padrão por enquanto
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter RestauranteId do contexto");
                return 1;
            }
        }

        public async Task<int> GetRestauranteIdAsync()
        {
            return await Task.FromResult(GetRestauranteId());
        }

        public int? GetUserId()
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter UserId do contexto");
                return null;
            }
        }

        public string GetUserAgent()
        {
            try
            {
                return _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString() ?? "Unknown";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter User-Agent");
                return "Unknown";
            }
        }

        public string GetUserIpAddress()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null) return "0.0.0.0";

                // Verificar se existe proxy reverso
                var xForwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(xForwardedFor))
                {
                    return xForwardedFor.Split(',')[0].Trim();
                }

                var xRealIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(xRealIp))
                {
                    return xRealIp;
                }

                // IP direto
                return httpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter IP do usuário");
                return "0.0.0.0";
            }
        }

        public string GetUserEmail()
        {
            try
            {
                var emailClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email);
                return emailClaim?.Value ?? "unknown@sistema.com";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter email do contexto");
                return "unknown@sistema.com";
            }
        }

        public string GetUserName()
        {
            try
            {
                var nameClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name);
                return nameClaim?.Value ?? "Unknown";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter nome do contexto");
                return "Unknown";
            }
        }

        public bool IsAuthenticated()
        {
            try
            {
                return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar autenticação");
                return false;
            }
        }
    }
}
