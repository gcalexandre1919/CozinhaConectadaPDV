using SistemaPDV.Core.Entities;

namespace SistemaPDV.Core.Interfaces
{
    /// <summary>
    /// Interface para serviços de gerenciamento de senhas seguras
    /// Implementa hash BCrypt e validações de força
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Gera hash seguro da senha usando BCrypt
        /// </summary>
        /// <param name="password">Senha em texto claro</param>
        /// <returns>Hash da senha</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifica se a senha corresponde ao hash
        /// </summary>
        /// <param name="password">Senha em texto claro</param>
        /// <param name="hashedPassword">Hash armazenado</param>
        /// <returns>True se corresponde</returns>
        bool VerifyPassword(string password, string hashedPassword);

        /// <summary>
        /// Valida força da senha conforme políticas de segurança
        /// </summary>
        /// <param name="password">Senha a validar</param>
        /// <returns>Resultado da validação</returns>
        PasswordValidationResult ValidatePasswordStrength(string password);

        /// <summary>
        /// Gera senha temporária segura
        /// </summary>
        /// <param name="length">Tamanho da senha</param>
        /// <returns>Senha temporária</returns>
        string GenerateTemporaryPassword(int length = 12);

        /// <summary>
        /// Verifica se a senha foi comprometida em vazamentos conhecidos
        /// </summary>
        /// <param name="password">Senha a verificar</param>
        /// <returns>True se comprometida</returns>
        Task<bool> IsPasswordComprommisedAsync(string password);
    }

    /// <summary>
    /// Resultado da validação de força da senha
    /// </summary>
    public class PasswordValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public PasswordStrength Strength { get; set; }
        public int Score { get; set; } // 0-100
    }

    /// <summary>
    /// Níveis de força da senha
    /// </summary>
    public enum PasswordStrength
    {
        VeryWeak = 1,
        Weak = 2,
        Medium = 3,
        Strong = 4,
        VeryStrong = 5
    }
}
