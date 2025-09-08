using System.ComponentModel.DataAnnotations;
using SistemaPDV.Core.Attributes;

namespace SistemaPDV.Core.DTOs
{
    public class ClienteDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }
        
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Telefone { get; set; }
        
        [CPFValido(ErrorMessage = "CPF inválido")]
        [StringLength(11, ErrorMessage = "CPF deve ter no máximo 11 caracteres")]
        public string? CPF { get; set; }
        
        [CNPJValido(ErrorMessage = "CNPJ inválido")]
        [StringLength(14, ErrorMessage = "CNPJ deve ter no máximo 14 caracteres")]
        public string? CNPJ { get; set; }
        
        public DateTime DataCadastro { get; set; }
        
        public bool Ativo { get; set; } = true;
        
        // Endereço
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
        
        [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
        public string? Observacoes { get; set; }
    }
    
    public class ClienteCriacaoDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }
        
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Telefone { get; set; }
        
        [CPFValido(ErrorMessage = "CPF inválido")]
        [StringLength(11, ErrorMessage = "CPF deve ter no máximo 11 caracteres")]
        public string? CPF { get; set; }
        
        [CNPJValido(ErrorMessage = "CNPJ inválido")]
        [StringLength(14, ErrorMessage = "CNPJ deve ter no máximo 14 caracteres")]
        public string? CNPJ { get; set; }
        
        // Endereço
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
        
        [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
        public string? Observacoes { get; set; }
    }
    
    public class ClienteAtualizacaoDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }
        
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Telefone { get; set; }
        
        [CPFValido(ErrorMessage = "CPF inválido")]
        [StringLength(11, ErrorMessage = "CPF deve ter no máximo 11 caracteres")]
        public string? CPF { get; set; }
        
        [CNPJValido(ErrorMessage = "CNPJ inválido")]
        [StringLength(14, ErrorMessage = "CNPJ deve ter no máximo 14 caracteres")]
        public string? CNPJ { get; set; }
        
        public bool Ativo { get; set; } = true;
        
        // Endereço
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
        
        [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
        public string? Observacoes { get; set; }
    }
}
