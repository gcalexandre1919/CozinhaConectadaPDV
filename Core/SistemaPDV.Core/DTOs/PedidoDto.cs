using SistemaPDV.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.DTOs
{
    public class PedidoDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Cliente é obrigatório")]
        public int ClienteId { get; set; }
        public string? NomeCliente { get; set; }
        
        [Required(ErrorMessage = "Tipo de pedido é obrigatório")]
        public TipoPedido Tipo { get; set; }
        
        public StatusPedido Status { get; set; }
        
        public DateTime DataCriacao { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        
        public decimal SubTotal { get; set; }
        
        // Para Refeição no Local
        [Range(0, 100, ErrorMessage = "Percentual do garçom deve estar entre 0% e 100%")]
        public decimal? PercentualGarcom { get; set; }
        public decimal? ValorGarcom { get; set; }
        
        // Para Entrega
        [Range(0, double.MaxValue, ErrorMessage = "Taxa de entrega deve ser maior ou igual a zero")]
        public decimal? TaxaEntrega { get; set; }
        
        public decimal ValorTotal { get; set; }
        
        [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
        public string? Observacoes { get; set; }
        
        public List<PedidoItemDto> Itens { get; set; } = new List<PedidoItemDto>();
    }
    
    public class PedidoItemDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string? NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal SubTotal { get; set; }
        public string? Observacoes { get; set; }
    }
    
    public class CriarPedidoDto
    {
        [Required(ErrorMessage = "Cliente é obrigatório")]
        public int ClienteId { get; set; }
        
        [Required(ErrorMessage = "Tipo de pedido é obrigatório")]
        public TipoPedido Tipo { get; set; }
        
        // Para Refeição no Local
        [Range(0, 100, ErrorMessage = "Percentual do garçom deve estar entre 0% e 100%")]
        public decimal? PercentualGarcom { get; set; } = 10; // Padrão 10%
        
        // Para Entrega
        [Range(0, double.MaxValue, ErrorMessage = "Taxa de entrega deve ser maior ou igual a zero")]
        public decimal? TaxaEntrega { get; set; }
        
        [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
        public string? Observacoes { get; set; }
        
        public List<AdicionarItemDto> Itens { get; set; } = new List<AdicionarItemDto>();
    }
    
    public class AdicionarItemDto
    {
        [Required(ErrorMessage = "Produto é obrigatório")]
        public int ProdutoId { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }
        
        [StringLength(200, ErrorMessage = "Observações deve ter no máximo 200 caracteres")]
        public string? Observacoes { get; set; }
    }
    
    public class FecharContaDto
    {
        [Required(ErrorMessage = "ID do pedido é obrigatório")]
        public int PedidoId { get; set; }
        
        [Range(0, 100, ErrorMessage = "Percentual do garçom deve estar entre 0% e 100%")]
        public decimal? PercentualGarcomFinal { get; set; }
    }
}
