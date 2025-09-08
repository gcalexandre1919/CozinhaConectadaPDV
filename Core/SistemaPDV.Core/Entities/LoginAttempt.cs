using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPDV.Core.Entities
{
    /// <summary>
    /// Entidade para auditoria de tentativas de login
    /// Permite rastreamento de segurança e detecção de ataques
    /// </summary>
    [Table("LoginAttempts")]
    public class LoginAttempt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        public int? UserId { get; set; }

        [Required]
        public bool Success { get; set; }

        [StringLength(200)]
        public string? FailureReason { get; set; }

        [Required]
        [StringLength(45)]
        public string IpAddress { get; set; } = string.Empty;

        [StringLength(500)]
        public string? UserAgent { get; set; }

        [Required]
        public DateTime AttemptedAt { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        public bool IsBlocked { get; set; }

        [StringLength(500)]
        public string? AdditionalInfo { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        // Índices para performance
        public static void ConfigureIndexes(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginAttempt>()
                .HasIndex(e => e.Email);

            modelBuilder.Entity<LoginAttempt>()
                .HasIndex(e => e.AttemptedAt);

            modelBuilder.Entity<LoginAttempt>()
                .HasIndex(e => new { e.Email, e.AttemptedAt });
        }
    }
}
