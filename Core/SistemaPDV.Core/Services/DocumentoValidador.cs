using System.Text.RegularExpressions;

namespace SistemaPDV.Core.Services
{
    public static class DocumentoValidador
    {
        /// <summary>
        /// Valida um CPF verificando dígitos verificadores
        /// </summary>
        public static bool ValidarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            // Remove caracteres não numéricos
            cpf = Regex.Replace(cpf, @"[^\d]", "");

            // Verifica se tem 11 dígitos
            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais (CPF inválido)
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Calcula o primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(cpf[i].ToString()) * (10 - i);

            int digito1 = 11 - (soma % 11);
            if (digito1 >= 10)
                digito1 = 0;

            // Verifica o primeiro dígito
            if (int.Parse(cpf[9].ToString()) != digito1)
                return false;

            // Calcula o segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(cpf[i].ToString()) * (11 - i);

            int digito2 = 11 - (soma % 11);
            if (digito2 >= 10)
                digito2 = 0;

            // Verifica o segundo dígito
            return int.Parse(cpf[10].ToString()) == digito2;
        }

        /// <summary>
        /// Valida um CNPJ verificando dígitos verificadores
        /// </summary>
        public static bool ValidarCNPJ(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            // Remove caracteres não numéricos
            cnpj = Regex.Replace(cnpj, @"[^\d]", "");

            // Verifica se tem 14 dígitos
            if (cnpj.Length != 14)
                return false;

            // Verifica se todos os dígitos são iguais (CNPJ inválido)
            if (cnpj.All(c => c == cnpj[0]))
                return false;

            // Calcula o primeiro dígito verificador
            int[] multiplicadores1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(cnpj[i].ToString()) * multiplicadores1[i];

            int digito1 = soma % 11;
            digito1 = digito1 < 2 ? 0 : 11 - digito1;

            // Verifica o primeiro dígito
            if (int.Parse(cnpj[12].ToString()) != digito1)
                return false;

            // Calcula o segundo dígito verificador
            int[] multiplicadores2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(cnpj[i].ToString()) * multiplicadores2[i];

            int digito2 = soma % 11;
            digito2 = digito2 < 2 ? 0 : 11 - digito2;

            // Verifica o segundo dígito
            return int.Parse(cnpj[13].ToString()) == digito2;
        }

        /// <summary>
        /// Formata um CPF
        /// </summary>
        public static string FormatarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return string.Empty;

            cpf = Regex.Replace(cpf, @"[^\d]", "");
            
            if (cpf.Length != 11)
                return cpf;

            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }

        /// <summary>
        /// Formata um CNPJ
        /// </summary>
        public static string FormatarCNPJ(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return string.Empty;

            cnpj = Regex.Replace(cnpj, @"[^\d]", "");
            
            if (cnpj.Length != 14)
                return cnpj;

            return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
        }

        /// <summary>
        /// Remove formatação de documento (CPF ou CNPJ)
        /// </summary>
        public static string LimparDocumento(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return string.Empty;

            return Regex.Replace(documento, @"[^\d]", "");
        }
    }
}
