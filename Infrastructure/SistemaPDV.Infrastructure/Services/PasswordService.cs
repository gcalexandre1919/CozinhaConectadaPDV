using Microsoft.Extensions.Logging;
using SistemaPDV.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SistemaPDV.Infrastructure.Services
{
    /// <summary>
    /// Serviço para gerenciamento seguro de senhas
    /// Implementa BCrypt, validação de força e verificação de vazamentos
    /// </summary>
    public class PasswordService : IPasswordService
    {
        private readonly ILogger<PasswordService> _logger;
        private readonly HttpClient _httpClient;

        // Configurações de segurança
        private const int BcryptWorkFactor = 12; // Cost factor para BCrypt
        private const int MinPasswordLength = 8;
        private const int MaxPasswordLength = 128;

        public PasswordService(
            ILogger<PasswordService> logger,
            HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Gera hash seguro da senha usando BCrypt
        /// </summary>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Senha não pode ser vazia", nameof(password));

            try
            {
                var hash = BCrypt.Net.BCrypt.HashPassword(password, BcryptWorkFactor);
                
                _logger.LogDebug("Hash de senha gerado com sucesso usando BCrypt work factor {WorkFactor}", 
                    BcryptWorkFactor);
                
                return hash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar hash da senha");
                throw new InvalidOperationException("Erro interno ao processar senha", ex);
            }
        }

        /// <summary>
        /// Verifica se a senha corresponde ao hash
        /// </summary>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            try
            {
                var isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                
                _logger.LogDebug("Verificação de senha: {Result}", isValid ? "válida" : "inválida");
                
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar senha");
                return false;
            }
        }

        /// <summary>
        /// Valida força da senha conforme políticas de segurança
        /// </summary>
        public PasswordValidationResult ValidatePasswordStrength(string password)
        {
            var result = new PasswordValidationResult
            {
                Errors = new List<string>()
            };

            if (string.IsNullOrWhiteSpace(password))
            {
                result.Errors.Add("Senha é obrigatória");
                result.IsValid = false;
                result.Strength = PasswordStrength.VeryWeak;
                return result;
            }

            var score = 0;

            // Verificar comprimento
            if (password.Length < MinPasswordLength)
            {
                result.Errors.Add($"Senha deve ter pelo menos {MinPasswordLength} caracteres");
            }
            else if (password.Length >= MinPasswordLength)
            {
                score += 20;
                if (password.Length >= 12) score += 10;
                if (password.Length >= 16) score += 10;
            }

            if (password.Length > MaxPasswordLength)
            {
                result.Errors.Add($"Senha deve ter no máximo {MaxPasswordLength} caracteres");
            }

            // Verificar caracteres maiúsculos
            if (Regex.IsMatch(password, @"[A-Z]"))
            {
                score += 15;
            }
            else
            {
                result.Errors.Add("Senha deve conter pelo menos uma letra maiúscula");
            }

            // Verificar caracteres minúsculos
            if (Regex.IsMatch(password, @"[a-z]"))
            {
                score += 15;
            }
            else
            {
                result.Errors.Add("Senha deve conter pelo menos uma letra minúscula");
            }

            // Verificar números
            if (Regex.IsMatch(password, @"[0-9]"))
            {
                score += 15;
            }
            else
            {
                result.Errors.Add("Senha deve conter pelo menos um número");
            }

            // Verificar caracteres especiais
            if (Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
            {
                score += 15;
            }
            else
            {
                result.Errors.Add("Senha deve conter pelo menos um caractere especial");
            }

            // Verificar padrões comuns (sequências, repetições)
            if (HasCommonPatterns(password))
            {
                score -= 20;
                result.Errors.Add("Senha contém padrões comuns (sequências ou repetições)");
            }
            else
            {
                score += 10;
            }

            // Definir força da senha baseado no score
            result.Score = Math.Max(0, Math.Min(100, score));
            result.Strength = result.Score switch
            {
                < 30 => PasswordStrength.VeryWeak,
                < 50 => PasswordStrength.Weak,
                < 70 => PasswordStrength.Medium,
                < 85 => PasswordStrength.Strong,
                _ => PasswordStrength.VeryStrong
            };

            result.IsValid = result.Errors.Count == 0 && result.Strength >= PasswordStrength.Medium;

            _logger.LogDebug("Validação de força da senha: Score={Score}, Força={Strength}, Válida={IsValid}", 
                result.Score, result.Strength, result.IsValid);

            return result;
        }

        /// <summary>
        /// Gera senha temporária segura
        /// </summary>
        public string GenerateTemporaryPassword(int length = 12)
        {
            if (length < 8 || length > 50)
                throw new ArgumentException("Comprimento deve estar entre 8 e 50 caracteres", nameof(length));

            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*";

            var password = new StringBuilder();
            var random = new Random();

            // Garantir pelo menos um de cada tipo
            password.Append(upperCase[random.Next(upperCase.Length)]);
            password.Append(lowerCase[random.Next(lowerCase.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(specialChars[random.Next(specialChars.Length)]);

            // Preencher o restante
            const string allChars = upperCase + lowerCase + digits + specialChars;
            for (int i = 4; i < length; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Embaralhar a senha
            var chars = password.ToString().ToCharArray();
            for (int i = chars.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }

            var generatedPassword = new string(chars);
            
            _logger.LogInformation("Senha temporária gerada com {Length} caracteres", length);
            
            return generatedPassword;
        }

        /// <summary>
        /// Verifica se a senha foi comprometida em vazamentos conhecidos
        /// Utiliza API Have I Been Pwned (k-anonymity)
        /// </summary>
        public async Task<bool> IsPasswordComprommisedAsync(string password)
        {
            try
            {
                // Gerar hash SHA-1 da senha
                using var sha1 = SHA1.Create();
                var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashString = Convert.ToHexString(hashBytes);

                // Usar k-anonymity: enviar apenas os primeiros 5 caracteres
                var prefix = hashString[..5];
                var suffix = hashString[5..];

                // Consultar API HaveIBeenPwned
                var response = await _httpClient.GetStringAsync($"https://api.pwnedpasswords.com/range/{prefix}");
                
                // Verificar se o hash completo está na resposta
                var lines = response.Split('\n');
                var isCompromised = lines.Any(line => 
                    line.StartsWith(suffix, StringComparison.OrdinalIgnoreCase));

                if (isCompromised)
                {
                    _logger.LogWarning("Senha comprometida detectada em vazamentos conhecidos");
                }

                return isCompromised;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se senha foi comprometida");
                // Em caso de erro, não bloquear o usuário
                return false;
            }
        }

        /// <summary>
        /// Verifica padrões comuns na senha
        /// </summary>
        private static bool HasCommonPatterns(string password)
        {
            // Verificar sequências numéricas
            if (Regex.IsMatch(password, @"(012|123|234|345|456|567|678|789|890)"))
                return true;

            // Verificar sequências alfabéticas
            if (Regex.IsMatch(password, @"(abc|bcd|cde|def|efg|fgh|ghi|hij|ijk|jkl|klm|lmn|mno|nop|opq|pqr|qrs|rst|stu|tuv|uvw|vwx|wxy|xyz)", RegexOptions.IgnoreCase))
                return true;

            // Verificar repetições excessivas
            if (Regex.IsMatch(password, @"(.)\1{2,}"))
                return true;

            // Verificar padrões de teclado
            var keyboardPatterns = new[] { "qwert", "asdf", "zxcv", "1234", "abcd" };
            if (keyboardPatterns.Any(pattern => password.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
                return true;

            return false;
        }
    }
}
