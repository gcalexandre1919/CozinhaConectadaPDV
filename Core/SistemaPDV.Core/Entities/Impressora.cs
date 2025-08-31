using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.Entities
{
    public enum TipoImpressora
    {
        Termica = 1,
        Matricial = 2,
        Jato = 3,
        Laser = 4
    }

    public class Impressora
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Caminho { get; set; } = string.Empty;
        
        [Required]
        public TipoImpressora Tipo { get; set; } = TipoImpressora.Termica;
        
        public bool Ativa { get; set; } = true;
        
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        
        // Configurações específicas
        public int LarguraPapel { get; set; } = 80; // mm
        public bool CortarPapel { get; set; } = true;
        public bool AbrirGaveta { get; set; } = false;
        
        [StringLength(500)]
        public string? Observacoes { get; set; }
    }
}