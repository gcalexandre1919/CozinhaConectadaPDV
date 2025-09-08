using System.ComponentModel.DataAnnotations;
using SistemaPDV.Core.Services;

namespace SistemaPDV.Core.Attributes
{
    /// <summary>
    /// Atributo para validação de CPF
    /// </summary>
    public class CPFValidoAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return true; // Permite valores vazios - usar [Required] para obrigatório

            return DocumentoValidador.ValidarCPF(value.ToString()!);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"O campo {name} deve conter um CPF válido.";
        }
    }

    /// <summary>
    /// Atributo para validação de CNPJ
    /// </summary>
    public class CNPJValidoAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return true; // Permite valores vazios - usar [Required] para obrigatório

            return DocumentoValidador.ValidarCNPJ(value.ToString()!);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"O campo {name} deve conter um CNPJ válido.";
        }
    }
}
