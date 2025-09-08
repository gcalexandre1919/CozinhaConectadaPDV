using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.DTOs
{
	public class LoginDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		public string Password { get; set; } = string.Empty;
	}

	public class RegisterDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		[MinLength(6)]
		public string Password { get; set; } = string.Empty;

		// Nome do usuário (obrigatório para admin)
		[Required]
		public string Nome { get; set; } = string.Empty;

		// Restaurante ao qual o usuário pertence (pode ser obrigatório no fluxo)
		public int RestauranteId { get; set; }
	}

	public class RegistroCompletoDto
	{
		public RegisterDto Usuario { get; set; } = new();
		public RestauranteRegistroDto Restaurante { get; set; } = new();
	}

	public class RestauranteRegistroDto
	{
		[Required]
		public string Nome { get; set; } = string.Empty;
		public string? CNPJ { get; set; }
		public string? Telefone { get; set; }
		public string? Email { get; set; }
		public string? Endereco { get; set; }
		public string? Numero { get; set; }
		public string? Complemento { get; set; }
		public string? Bairro { get; set; }
		public string? Cidade { get; set; }
		public string? UF { get; set; }
		public string? CEP { get; set; }
	}

	public class AuthResponseDto
	{
		public bool Sucesso { get; set; }
		public string? Token { get; set; }
		public string? RefreshToken { get; set; }
		public string? Erro { get; set; }
		public string? Nome { get; set; }
		public int? UserId { get; set; }
		public int? RestauranteId { get; set; }
		public DateTime? ExpiraEm { get; set; }
		public DateTime? RefreshExpiraEm { get; set; }
		public List<string> Permissions { get; set; } = new();
	}

	public class ChangePasswordDto
	{
		[Required]
		public string CurrentPassword { get; set; } = string.Empty;

		[Required]
		[MinLength(8)]
		public string NewPassword { get; set; } = string.Empty;

		[Required]
		[Compare("NewPassword")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}

	public class RefreshTokenDto
	{
		[Required]
		public string RefreshToken { get; set; } = string.Empty;
	}

	public class ResetPasswordDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		public string ResetToken { get; set; } = string.Empty;

		[Required]
		[MinLength(8)]
		public string NewPassword { get; set; } = string.Empty;

		[Required]
		[Compare("NewPassword")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}

	public class ForgotPasswordDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;
	}

	public class LoginAttemptDto
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string Email { get; set; } = string.Empty;
		public bool Success { get; set; }
		public string? FailureReason { get; set; }
		public string IpAddress { get; set; } = string.Empty;
		public string UserAgent { get; set; } = string.Empty;
		public DateTime AttemptedAt { get; set; }
		public string? Location { get; set; }
	}

	public class TokenValidationDto
	{
		public bool IsValid { get; set; }
		public int? UserId { get; set; }
		public string? Email { get; set; }
		public DateTime? ExpiresAt { get; set; }
		public List<string> Claims { get; set; } = new();
		public string? ErrorMessage { get; set; }
	}
}
