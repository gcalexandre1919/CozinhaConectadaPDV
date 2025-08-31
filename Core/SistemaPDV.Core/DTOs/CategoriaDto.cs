using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.DTOs
{
    public class CategoriaDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }
        
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
    
    public class CategoriaCriacaoDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }
    }
    
    public class CategoriaAtualizacaoDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }
        
        public bool Ativo { get; set; } = true;
    }
}
