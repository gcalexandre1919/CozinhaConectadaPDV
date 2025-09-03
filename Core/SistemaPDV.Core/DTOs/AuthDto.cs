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

		// Nome do usuário (opcional)
		public string? Nome { get; set; }

		// Restaurante ao qual o usuário pertence (pode ser obrigatório no fluxo)
		public int RestauranteId { get; set; }
	}

	public class AuthResponseDto
	{
		public bool Sucesso { get; set; }
		public string? Token { get; set; }
		public string? Erro { get; set; }
		public string? Nome { get; set; }
		public int? RestauranteId { get; set; }
		public DateTime? ExpiraEm { get; set; }
	}
}
