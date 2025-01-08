using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace JJ.Standard.Core.Extensoes
{
    public static class StringExtension
    {
        public static string ObterValorOuPadrao(this string valor, string padrao)
        {
            return string.IsNullOrWhiteSpace(valor) ? padrao : valor;
        }

        public static int ObterValorOuPadrao(this string valor, int padrao)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return padrao;

            return valor.ConverterParaInt32();
        }

        public static string FormatarSaldo(this string valor, CultureInfo cultureInfo)
        {
            if (ObterValorOuPadrao(valor, "").Trim() == "")
                return valor;

            if (decimal.TryParse(valor, out decimal decimalValue))
                return decimalValue.ToString("N2", cultureInfo);
            
            return valor;
        }

        public static string LimitarTamanho(this string valor, string limite)
        {
            return LimitarTamanho(valor, limite.Length);
        }
        
        public static string LimitarTamanho(this string valor, int limite)
        {
            if (ObterValorOuPadrao(valor, "").Trim() == "")
                return valor;

            return (valor.Length > limite) ? valor.Substring(0, limite) : valor;
        }

        public static string AtribuirSimboloMonetario(this string valor, CultureInfo cultureInfo)
        {
            if (ObterValorOuPadrao(valor, "").Trim() == "")
                return valor;

            return $"{valor} ({cultureInfo.NumberFormat.CurrencySymbol})";
        }

        public static string PrimeiraLetraMaiuscula(this string valor)
        {
            if (ObterValorOuPadrao(valor, "").Trim() == "")
                return valor;

            return char.ToUpper(valor[0]) + valor.Substring(1);
        }

        public static int ConverterParaInt32(this string valor, int valorPadrao = 0)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valorPadrao;  

            return int.TryParse(valor, out int result) ? result : valorPadrao;
        }

        public static decimal ConverterParaDecimal(this string valor, decimal valorPadrao = 0m)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valorPadrao;

            return decimal.TryParse(valor, out decimal result) ? result : valorPadrao;
        }


        public static bool EhNumero(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return false;

            return int.TryParse(valor, out _);
        }

        /// <summary>
        /// Formata uma string, removendo linhas em branco e espaços extras antes e depois de cada linha,
        /// de modo a garantir que a string resultante esteja pronta para ser utilizada em uma consulta SQL.
        /// </summary>
        /// <param name="input">A string de entrada que será processada.</param>
        /// <returns>Uma string formatada, onde cada linha foi "limpa" de espaços em excesso e linhas em branco foram removidas.</returns>
        /// <example>
        /// <code>
        /// var query = "SELECT * FROM Tabela\n WHERE coluna = 'valor'  \n  ";
        /// Resultado: "SELECT * FROM Tabela\nWHERE coluna = 'valor'"
        /// var result = query.ToSQL();
        /// </code>
        /// </example>
        public static string ToSQL(this string input)
        {
            var stringBuilder = new StringBuilder();
            var lines = input.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
                stringBuilder.AppendLine(line.Trim());

            return stringBuilder.ToString();
        }

        public static string PasswordChar(this string input, char passwordChar = '*')
        {
            if (input.ObterValorOuPadrao("").Trim() == "")
                return "";

            return new string(passwordChar, input.Length);
        }

        public static string LimparEntradaSQL(this string input)
        {
            if (input.ObterValorOuPadrao("").Trim() == "")
                return "";

            string[] palavrasChaveSQL = { "DROP", "DELETE", "INSERT", "UPDATE", "SELECT", "TRUNCATE", "ALTER", "GRANT", "REVOKE", "--", ";", "/*", "*/" };

            foreach (var palavra in palavrasChaveSQL)
                input = input.Replace(palavra, "", StringComparison.OrdinalIgnoreCase);

            input = input.Replace("'", "''"); 

            return input;
        }
    }
}
