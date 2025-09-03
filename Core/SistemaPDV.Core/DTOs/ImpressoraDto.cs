using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.DTOs
{
    public class ImpressoraDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Caminho é obrigatório")]
        [StringLength(200, ErrorMessage = "Caminho deve ter no máximo 200 caracteres")]
        public string Caminho { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "Tipo deve ter no máximo 100 caracteres")]
        public string? Tipo { get; set; }
        
        public bool Ativa { get; set; } = true;
        
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
    
    public class ImpressoraCriacaoDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Caminho é obrigatório")]
        [StringLength(200, ErrorMessage = "Caminho deve ter no máximo 200 caracteres")]
        public string Caminho { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "Tipo deve ter no máximo 100 caracteres")]
        public string? Tipo { get; set; }
    }
    
    public class ImpressoraAtualizacaoDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Caminho é obrigatório")]
        [StringLength(200, ErrorMessage = "Caminho deve ter no máximo 200 caracteres")]
        public string Caminho { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "Tipo deve ter no máximo 100 caracteres")]
        public string? Tipo { get; set; }
        
        public bool Ativa { get; set; } = true;
    }
}
