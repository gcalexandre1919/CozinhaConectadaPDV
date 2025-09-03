using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.Entities
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public string? Nome { get; set; }
        
        [Required]
        public int RestauranteId { get; set; }
        
        public virtual Restaurante? Restaurante { get; set; }
        
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        
        public bool Ativo { get; set; } = true;
    }
}
