using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPDV.Core.Entities
{
    /// <summary>
    /// Entidade de usuário do sistema com funcionalidades modernas de autenticação
    /// </summary>
    [Table("ApplicationUsers")]
    public class ApplicationUser
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        public int RestauranteId { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public bool IsAdmin { get; set; } = false;
        
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        
        public DateTime? ModificadoEm { get; set; }
        
        public DateTime? UltimoLogin { get; set; }
        
        [StringLength(45)]
        public string? UltimoIpLogin { get; set; }
        
        public int TentativasLoginFalhas { get; set; } = 0;
        
        public DateTime? BloqueadoAte { get; set; }
        
        [StringLength(500)]
        public string? Observacoes { get; set; }

        // Navigation Properties
        [ForeignKey("RestauranteId")]
        public virtual Restaurante? Restaurante { get; set; }
        
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        
        // Propriedades calculadas
        [NotMapped]
        public bool IsLocked => BloqueadoAte.HasValue && BloqueadoAte > DateTime.UtcNow;
        
        [NotMapped]
        public string StatusDescription => IsActive switch
        {
            false => "Inativo",
            true when IsLocked => "Bloqueado",
            true => "Ativo"
        };

        // Compatibilidade com código existente
        [NotMapped]
        public DateTime DataCriacao 
        { 
            get => CriadoEm; 
            set => CriadoEm = value; 
        }
        
        [NotMapped]
        public bool Ativo 
        { 
            get => IsActive; 
            set => IsActive = value; 
        }
    }
}
