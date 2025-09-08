using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPDV.Core.Entities
{
    public class Restaurante
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        [StringLength(14)]
        public string? CNPJ { get; set; }
        
        [StringLength(200)]
        public string? Endereco { get; set; }
        
        [StringLength(10)]
        public string? Numero { get; set; }
        
        [StringLength(100)]
        public string? Complemento { get; set; }
        
        [StringLength(100)]
        public string? Bairro { get; set; }
        
        [StringLength(100)]
        public string? Cidade { get; set; }
        
        [StringLength(2)]
        public string? UF { get; set; }
        
        [StringLength(8)]
        public string? CEP { get; set; }
        
        [StringLength(20)]
        public string? Telefone { get; set; }
        
        [StringLength(100)]
        public string? Email { get; set; }
        
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        
        public bool Ativo { get; set; } = true;

        // Propriedades modernas para compatibilidade
        [NotMapped]
        public bool IsActive 
        { 
            get => Ativo; 
            set => Ativo = value; 
        }

        [NotMapped]
        public DateTime CriadoEm 
        { 
            get => DataCadastro; 
            set => DataCadastro = value; 
        }
        
        // Navigation properties
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}