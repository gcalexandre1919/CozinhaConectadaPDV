using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.Entities
{
    public class PedidoItem
    {
        public int Id { get; set; }
        
        [Required]
        public int PedidoId { get; set; }
        public virtual Pedido? Pedido { get; set; }
        
        [Required]
        public int ProdutoId { get; set; }
        public virtual Produto? Produto { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }
        
        public decimal PrecoUnitario { get; set; }
        
        public decimal SubTotal => Quantidade * PrecoUnitario;
        
        [StringLength(200)]
        public string? Observacoes { get; set; }
        
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}
