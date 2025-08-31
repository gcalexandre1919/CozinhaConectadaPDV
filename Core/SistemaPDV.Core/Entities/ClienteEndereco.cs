using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.Entities
{
    public class ClienteEndereco
    {
        public int Id { get; set; }
        
        [Required]
        public int ClienteId { get; set; }
        public virtual Cliente? Cliente { get; set; }
        
        [Required]
        [StringLength(200, ErrorMessage = "Endereço deve ter no máximo 200 caracteres")]
        public string Endereco { get; set; } = string.Empty;
        
        [StringLength(10, ErrorMessage = "Número deve ter no máximo 10 caracteres")]
        public string? Numero { get; set; }
        
        [StringLength(100, ErrorMessage = "Complemento deve ter no máximo 100 caracteres")]
        public string? Complemento { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "Bairro deve ter no máximo 100 caracteres")]
        public string Bairro { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
        public string Cidade { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2, ErrorMessage = "UF deve ter no máximo 2 caracteres")]
        public string UF { get; set; } = string.Empty;
        
        [StringLength(8, ErrorMessage = "CEP deve ter no máximo 8 caracteres")]
        public string? CEP { get; set; }
        
        [StringLength(200, ErrorMessage = "Referência deve ter no máximo 200 caracteres")]
        public string? Referencia { get; set; }
        
        public bool EnderecoPrincipal { get; set; } = false;
        
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}
