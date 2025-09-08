using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.DTOs
{
    public class RestauranteDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? CNPJ { get; set; }
        public string? Endereco { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? UF { get; set; }
        public string? CEP { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }
    }

    public class RestauranteCriacaoDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [StringLength(14, ErrorMessage = "CNPJ deve ter no máximo 14 caracteres")]
        public string? CNPJ { get; set; }
        
        [StringLength(200, ErrorMessage = "Endereço deve ter no máximo 200 caracteres")]
        public string? Endereco { get; set; }
        
        [StringLength(10, ErrorMessage = "Número deve ter no máximo 10 caracteres")]
        public string? Numero { get; set; }
        
        [StringLength(100, ErrorMessage = "Complemento deve ter no máximo 100 caracteres")]
        public string? Complemento { get; set; }
        
        [StringLength(100, ErrorMessage = "Bairro deve ter no máximo 100 caracteres")]
        public string? Bairro { get; set; }
        
        [StringLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
        public string? Cidade { get; set; }
        
        [StringLength(2, ErrorMessage = "UF deve ter no máximo 2 caracteres")]
        public string? UF { get; set; }
        
        [StringLength(8, ErrorMessage = "CEP deve ter no máximo 8 caracteres")]
        public string? CEP { get; set; }
        
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Telefone { get; set; }
        
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
        public string? Email { get; set; }
    }

    public class RestauranteAtualizacaoDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [StringLength(14, ErrorMessage = "CNPJ deve ter no máximo 14 caracteres")]
        public string? CNPJ { get; set; }
        
        [StringLength(200, ErrorMessage = "Endereço deve ter no máximo 200 caracteres")]
        public string? Endereco { get; set; }
        
        [StringLength(10, ErrorMessage = "Número deve ter no máximo 10 caracteres")]
        public string? Numero { get; set; }
        
        [StringLength(100, ErrorMessage = "Complemento deve ter no máximo 100 caracteres")]
        public string? Complemento { get; set; }
        
        [StringLength(100, ErrorMessage = "Bairro deve ter no máximo 100 caracteres")]
        public string? Bairro { get; set; }
        
        [StringLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
        public string? Cidade { get; set; }
        
        [StringLength(2, ErrorMessage = "UF deve ter no máximo 2 caracteres")]
        public string? UF { get; set; }
        
        [StringLength(8, ErrorMessage = "CEP deve ter no máximo 8 caracteres")]
        public string? CEP { get; set; }
        
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Telefone { get; set; }
        
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
        public string? Email { get; set; }
        
        public bool Ativo { get; set; } = true;
    }

    public class RestauranteEstatisticasDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int PedidosHoje { get; set; }
        public int PedidosMes { get; set; }
        public decimal VendasHoje { get; set; }
        public decimal VendasMes { get; set; }
        public int ClientesAtivos { get; set; }
        public DateTime DataConsulta { get; set; } = DateTime.Now;
    }
}
