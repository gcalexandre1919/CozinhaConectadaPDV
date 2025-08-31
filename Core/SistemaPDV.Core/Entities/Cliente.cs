using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.Entities
{
    public class Cliente
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }
        
        [Required(ErrorMessage = "Telefone é obrigatório")]
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string Telefone { get; set; } = string.Empty;
        
        [StringLength(11, ErrorMessage = "CPF deve ter no máximo 11 caracteres")]
        public string? CPF { get; set; }
        
        public DateTime? DataNascimento { get; set; }
        
        [StringLength(14, ErrorMessage = "CNPJ deve ter no máximo 14 caracteres")]
        public string? CNPJ { get; set; }
        
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        
        public bool Ativo { get; set; } = true;
        
        [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
        public string? Observacoes { get; set; }
        
        // Multi-tenant
        public int RestauranteId { get; set; }
        public virtual Restaurante? Restaurante { get; set; }
        
        // Navigation properties
        public virtual ICollection<ClienteEndereco> Enderecos { get; set; } = new List<ClienteEndereco>();
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}