namespace SistemaPDV.Web.Services
{
    public interface IUserContextService
    {
        Task<int> GetRestauranteIdAsync();
        Task<int> GetUserIdAsync();
        Task<string> GetUserEmailAsync();
    }

    public class UserContextService : IUserContextService
    {
        private readonly IAuthWebService _authService;
        private readonly ILogger<UserContextService> _logger;

        public UserContextService(IAuthWebService authService, ILogger<UserContextService> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public async Task<int> GetRestauranteIdAsync()
        {
            try
            {
                // Por enquanto retorna 1, mas deveria vir do token JWT decodificado
                // TODO: Implementar decodificação do token JWT para extrair RestauranteId
                var token = _authService.GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Token não encontrado, usando RestauranteId padrão");
                    return 1;
                }

                // Decodificar token JWT aqui
                // Por enquanto, retorna valor padrão
                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter RestauranteId do contexto");
                return 1; // Fallback
            }
        }

        public async Task<int> GetUserIdAsync()
        {
            try
            {
                var token = _authService.GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    return 0;
                }

                // TODO: Implementar decodificação do token JWT
                return 1; // Valor temporário
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter UserId do contexto");
                return 0;
            }
        }

        public async Task<string> GetUserEmailAsync()
        {
            try
            {
                var token = _authService.GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    return "unknown@sistema.com";
                }

                // TODO: Implementar decodificação do token JWT
                return "admin@sistema.com"; // Valor temporário
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter email do contexto");
                return "unknown@sistema.com";
            }
        }
    }
}
