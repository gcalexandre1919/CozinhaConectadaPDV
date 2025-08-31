using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.Entities
{
    public class ConfiguracaoImpressao
    {
        public int Id { get; set; }
        
        public int? ImpressoraPadraoId { get; set; }
        public virtual Impressora? ImpressoraPadrao { get; set; }
        
        public bool ImprimirAutomaticamente { get; set; } = true;
        
        [Range(1, 5)]
        public int NumeroVias { get; set; } = 1;
        
        [Range(6, 20)]
        public int TamanhoFonte { get; set; } = 9;
        
        [Range(0, 50)]
        public int MargemEsquerda { get; set; } = 5;
        
        [Range(0, 50)]
        public int MargemSuperior { get; set; } = 5;
        
        public bool CortarPapelAutomaticamente { get; set; } = true;
        
        public bool AbrirGavetaAutomaticamente { get; set; } = false;
        
        [StringLength(500)]
        public string? CabecalhoPersonalizado { get; set; }
        
        [StringLength(500)]
        public string? RodapePersonalizado { get; set; }
    }
}