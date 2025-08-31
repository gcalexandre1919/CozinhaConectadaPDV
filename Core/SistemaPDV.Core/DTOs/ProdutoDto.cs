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
        public string? NomeCategoria { get; set; }
        public string? CategoriaNome { get; set; } // Alias para compatibilidade
        
        public string Codigo { get; set; } = string.Empty;
        
        [Range(0, int.MaxValue, ErrorMessage = "Quantidade em estoque deve ser maior ou igual a zero")]
        public int QuantidadeEstoque { get; set; }
        
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.Now;
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
    }
}
