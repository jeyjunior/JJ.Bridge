using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Windows.ApplicationModel;

namespace JJ.UW.Core.Extensoes
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

        public static string ToSQL(this string input)
        {
            var stringBuilder = new StringBuilder();
            var lines = input.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
                stringBuilder.AppendLine(line.Trim());

            return stringBuilder.ToString();
        }

        public static int ConverterParaInt32(this string valor, int valorPadrao = 0)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valorPadrao;  

            return int.TryParse(valor, out int result) ? result : valorPadrao;
        }

        public static bool EhNumero(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return false;

            return int.TryParse(valor, out _);
        }
    }
}
