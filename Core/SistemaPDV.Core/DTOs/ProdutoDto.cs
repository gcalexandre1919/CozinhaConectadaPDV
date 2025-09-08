using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.DTOs
{
    public class ProdutoDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Preco { get; set; }
        
        [Required(ErrorMessage = "Categoria é obrigatória")]
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
        
        public string? Codigo { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Estoque mínimo deve ser maior ou igual a zero")]
        public decimal EstoqueMinimo { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Estoque atual deve ser maior ou igual a zero")]
        public decimal EstoqueAtual { get; set; }
        
        public bool Ativo { get; set; } = true;
        
        public int RestauranteId { get; set; }
        
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        
        // Propriedades calculadas
        public bool BaixoEstoque => EstoqueAtual <= EstoqueMinimo;
        public string StatusEstoque => BaixoEstoque ? "Baixo" : "Normal";
        
        // Propriedade de compatibilidade com versão anterior
        public int QuantidadeEstoque 
        { 
            get => (int)EstoqueAtual; 
            set => EstoqueAtual = value; 
        }
    }
    
    public class ProdutoCriacaoDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Preco { get; set; }
        
        [Required(ErrorMessage = "Categoria é obrigatória")]
        public int CategoriaId { get; set; }
        
        public string? Codigo { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Estoque mínimo deve ser maior ou igual a zero")]
        public decimal EstoqueMinimo { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Estoque atual deve ser maior ou igual a zero")]
        public decimal EstoqueAtual { get; set; }
        
        public bool Ativo { get; set; } = true;
    }
}
