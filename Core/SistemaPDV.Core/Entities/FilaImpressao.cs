using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.Entities
{
    public enum StatusImpressao
    {
        Pendente = 1,
        Processando = 2,
        Impresso = 3,
        Erro = 4,
        Cancelado = 5
    }

    public class FilaImpressao
    {
        public int Id { get; set; }
        
        public int? PedidoId { get; set; }
        public virtual Pedido? Pedido { get; set; }
        
        [Required]
        public string Conteudo { get; set; } = string.Empty;
        
        public StatusImpressao Status { get; set; } = StatusImpressao.Pendente;
        
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataProcessamento { get; set; }
        
        public int TentativasImpressao { get; set; } = 0;
        
        public int? ImpressoraId { get; set; }
        public virtual Impressora? Impressora { get; set; }
        
        [StringLength(500)]
        public string? MensagemErro { get; set; }
    }
}