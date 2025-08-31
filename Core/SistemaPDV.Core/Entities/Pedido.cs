using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.Entities
{
    public enum TipoPedido
    {
        Retirada = 1,
        Entrega = 2,
        RefeicaoNoLocal = 3
    }

    public enum StatusPedido
    {
        Aberto = 1,
        Preparando = 2,
        Pronto = 3,
        Entregue = 4,
        Cancelado = 5,
        Fechado = 6
    }

    public class Pedido
    {
        public int Id { get; set; }
        
        [Required]
        public int ClienteId { get; set; }
        public virtual Cliente? Cliente { get; set; }
        
        [Required]
        public TipoPedido Tipo { get; set; }
        
        [Required]
        public StatusPedido Status { get; set; } = StatusPedido.Aberto;
        
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataFinalizacao { get; set; }
        
        // Valores monetários
        public decimal SubTotal { get; set; }
        
        // Taxa de Garçom (para Refeição no Local)
        public decimal? PercentualGarcom { get; set; } // 0-100%
        public decimal? ValorGarcom { get; set; }
        
        // Taxa de Entrega (para Entrega)
        public decimal? TaxaEntrega { get; set; }
        
        public decimal ValorTotal { get; set; }
        
        // Observações
        [StringLength(500)]
        public string? Observacoes { get; set; }
        
        // Multi-tenant
        public int RestauranteId { get; set; }
        public virtual Restaurante? Restaurante { get; set; }
        
        // Navigation properties
        public virtual ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
        
        // Métodos de cálculo
        public void CalcularValores()
        {
            SubTotal = Itens.Sum(i => i.SubTotal);
            
            // Calcular taxa de garçom (apenas para Refeição no Local)
            if (Tipo == TipoPedido.RefeicaoNoLocal && PercentualGarcom.HasValue)
            {
                ValorGarcom = SubTotal * (PercentualGarcom.Value / 100);
            }
            else
            {
                ValorGarcom = 0;
            }
            
            // Calcular total
            ValorTotal = SubTotal + (ValorGarcom ?? 0) + (TaxaEntrega ?? 0);
        }
        
        public void FecharConta()
        {
            if (Tipo == TipoPedido.RefeicaoNoLocal)
            {
                Status = StatusPedido.Fechado;
                DataFinalizacao = DateTime.Now;
            }
        }
    }
}
