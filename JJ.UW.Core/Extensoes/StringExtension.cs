using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace JJ.UW.Core.Extensoes
{
    public static class StringExtension
    {
        public static string ObterValorOuPadrao(this string valor, string padrao)
        {
            return string.IsNullOrWhiteSpace(valor) ? padrao : valor;
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

        public static string ToSQL(this string input)
        {
            var stringBuilder = new StringBuilder();
            var lines = input.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
                stringBuilder.AppendLine(line.Trim());

            return stringBuilder.ToString();
        }
    }
}
