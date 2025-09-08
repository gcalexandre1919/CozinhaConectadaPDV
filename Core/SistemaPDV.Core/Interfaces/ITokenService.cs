using SistemaPDV.Core.Entities;
using System.Security.Claims;

namespace SistemaPDV.Core.Interfaces
{
    /// <summary>
    /// Interface para serviços de gerenciamento de tokens JWT
    /// Implementa geração, validação e refresh de tokens
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Gera token JWT para o usuário autenticado
        /// </summary>
        /// <param name="user">Usuário autenticado</param>
        /// <returns>Token JWT</returns>
        string GenerateAccessToken(ApplicationUser user);

        /// <summary>
        /// Gera token de refresh para renovação
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <returns>Refresh token</returns>
        Task<string> GenerateRefreshTokenAsync(int userId);

        /// <summary>
        /// Valida token JWT e retorna claims
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Claims do usuário ou null se inválido</returns>
        ClaimsPrincipal? ValidateToken(string token);

        /// <summary>
        /// Valida refresh token
        /// </summary>
        /// <param name="refreshToken">Token de refresh</param>
        /// <returns>ID do usuário se válido</returns>
        Task<int?> ValidateRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Revoga refresh token específico
        /// </summary>
        /// <param name="refreshToken">Token a revogar</param>
        /// <returns>Sucesso da operação</returns>
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Revoga todos os refresh tokens do usuário
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <returns>Sucesso da operação</returns>
        Task<bool> RevokeAllRefreshTokensAsync(int userId);

        /// <summary>
        /// Obtém tempo restante do token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Tempo restante ou null se expirado</returns>
        TimeSpan? GetRemainingTokenTime(string token);

        /// <summary>
        /// Extrai ID do usuário do token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>ID do usuário</returns>
        int? GetUserIdFromToken(string token);
    }
}
