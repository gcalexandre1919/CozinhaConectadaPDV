using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;

namespace SistemaPDV.Core.Interfaces
{
    /// <summary>
    /// Interface para serviços de autenticação e autorização
    /// Implementa funcionalidades modernas de segurança e gerenciamento de usuários
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Realiza login do usuário com validações de segurança
        /// </summary>
        /// <param name="loginDto">Dados de login</param>
        /// <returns>Resultado da autenticação com token JWT</returns>
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Registra novo usuário para restaurante existente
        /// </summary>
        /// <param name="registerDto">Dados do usuário</param>
        /// <returns>Resultado do registro</returns>
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);

        /// <summary>
        /// Registro completo: restaurante + usuário administrador
        /// </summary>
        /// <param name="registroCompletoDto">Dados completos</param>
        /// <returns>Resultado do registro</returns>
        Task<AuthResponseDto> RegisterCompleteAsync(RegistroCompletoDto registroCompletoDto);

        /// <summary>
        /// Altera senha do usuário atual
        /// </summary>
        /// <param name="changePasswordDto">Dados para alteração</param>
        /// <returns>Sucesso da operação</returns>
        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);

        /// <summary>
        /// Valida se o token JWT é válido
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Dados do usuário se válido</returns>
        Task<ApplicationUser?> ValidateTokenAsync(string token);

        /// <summary>
        /// Gera novo token de acesso usando refresh token
        /// </summary>
        /// <param name="refreshToken">Token de refresh</param>
        /// <returns>Novo token de acesso</returns>
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Revoga todos os tokens do usuário (logout completo)
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <returns>Sucesso da operação</returns>
        Task<bool> RevokeAllTokensAsync(int userId);

        /// <summary>
        /// Verifica se a conta está bloqueada por tentativas de login
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <returns>True se bloqueada</returns>
        Task<bool> IsAccountLockedAsync(string email);

        /// <summary>
        /// Obtém histórico de tentativas de login
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <param name="take">Quantidade de registros</param>
        /// <returns>Lista de tentativas</returns>
        Task<List<LoginAttemptDto>> GetLoginHistoryAsync(int userId, int take = 10);
    }
}
