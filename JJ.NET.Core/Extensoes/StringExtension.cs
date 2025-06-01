using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.Core.Extensoes
{
    public static class StringExtension
    {
        /// <summary>
        /// Formata o valor de uma string representando um saldo, aplicando a formatação numérica de acordo com a cultura fornecida.
        /// </summary>
        /// <param name="valor">A string representando o valor do saldo.</param>
        /// <param name="cultureInfo">A cultura usada para formatar o saldo (ex.: moeda).</param>
        /// <returns>A string formatada conforme a cultura.</returns>
        public static string FormatarSaldo(this string valor, CultureInfo cultureInfo)
        {
            if (valor.ObterValorOuPadrao("").Trim() != "")
                return valor;

            if (decimal.TryParse(valor, out decimal decimalValue))
                return decimalValue.ToString("N2", cultureInfo);

            return valor;
        }

        /// <summary>
        /// Limita o tamanho de uma string com base no tamanho de outra string fornecida.
        /// </summary>
        /// <param name="valor">A string a ser limitada.</param>
        /// <param name="limite">A string cuja o comprimento será o limite.</param>
        /// <returns>A string limitada ao tamanho da string de limite.</returns>
        public static string LimitarTamanho(this string valor, string limite)
        {
            return LimitarTamanho(valor, limite.Length);
        }

        /// <summary>
        /// Limita o tamanho de uma string ao comprimento especificado.
        /// </summary>
        /// <param name="valor">A string a ser limitada.</param>
        /// <param name="limite">O tamanho máximo permitido para a string.</param>
        /// <returns>A string limitada ao tamanho especificado.</returns>
        public static string LimitarTamanho(this string valor, int limite)
        {
            if (valor.ObterValorOuPadrao("").Trim() != "")
                return valor;

            return (valor.Length > limite) ? valor.Substring(0, limite) : valor;
        }

        /// <summary>
        /// Atribui o símbolo monetário correspondente ao valor da string, conforme a cultura fornecida.
        /// </summary>
        /// <param name="valor">A string representando o valor monetário.</param>
        /// <param name="cultureInfo">A cultura usada para determinar o símbolo monetário.</param>
        /// <returns>A string com o valor monetário seguido pelo símbolo da moeda.</returns>
        public static string AtribuirSimboloMonetario(this string valor, CultureInfo cultureInfo)
        {
            if (valor.ObterValorOuPadrao("").Trim() != "")
                return valor;

            return $"{valor} ({cultureInfo.NumberFormat.CurrencySymbol})";
        }

        /// <summary>
        /// Converte a primeira letra de uma string para maiúscula, deixando o restante inalterado.
        /// </summary>
        /// <param name="valor">A string a ser modificada.</param>
        /// <returns>A string com a primeira letra em maiúscula.</returns>
        public static string PrimeiraLetraMaiuscula(this string valor)
        {
            if (valor.ObterValorOuPadrao("").Trim() != "")
                return valor;

            return char.ToUpper(valor[0]) + valor.Substring(1);
        }

        /// <summary>
        /// Converte uma string para inteiro, retornando um valor padrão se a conversão falhar ou a string for nula ou em branco.
        /// </summary>
        /// <param name="valor">A string a ser convertida.</param>
        /// <param name="valorPadrao">O valor padrão a ser retornado caso a conversão falhe.</param>
        /// <returns>O valor convertido para inteiro ou o valor padrão.</returns>
        public static int ConverterParaInt32(this string valor, int valorPadrao = 0)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valorPadrao;

            return int.TryParse(valor, out int result) ? result : valorPadrao;
        }

        /// <summary>
        /// Converte uma string para decimal, retornando um valor padrão se a conversão falhar ou a string for nula ou em branco.
        /// </summary>
        /// <param name="valor">A string a ser convertida.</param>
        /// <param name="valorPadrao">O valor padrão a ser retornado caso a conversão falhe.</param>
        /// <returns>O valor convertido para decimal ou o valor padrão.</returns>
        public static decimal ConverterParaDecimal(this string valor, decimal valorPadrao = 0m)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valorPadrao;

            return decimal.TryParse(valor, out decimal result) ? result : valorPadrao;
        }

        /// <summary>
        /// Verifica se a string é um número inteiro válido.
        /// </summary>
        /// <param name="valor">A string a ser verificada.</param>
        /// <returns>Retorna true se a string for um número inteiro válido, caso contrário, false.</returns>
        public static bool EhNumero(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return false;

            return int.TryParse(valor, out _);
        }

        /// <summary>
        /// Formata a string para um formato adequado para ser utilizado em uma consulta SQL.
        /// Remove linhas em branco e espaços extras antes e depois de cada linha.
        /// </summary>
        /// <param name="input">A string a ser processada.</param>
        /// <returns>A string formatada para ser utilizada em SQL.</returns>
        public static string ToSQL(this string input)
        {
            var stringBuilder = new StringBuilder();
            var lines = input.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
                stringBuilder.AppendLine(line.Trim());

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Substitui os caracteres de uma string por um caractere específico, geralmente utilizado para ocultar senhas.
        /// </summary>
        /// <param name="input">A string a ser modificada.</param>
        /// <param name="caracterOculto">O caractere que substituirá os caracteres da string (por padrão, '*').</param>
        /// <returns>A string com os caracteres substituídos pelo caractere especificado.</returns>
        public static string Ocultar(this string input, char caracterOculto = '*')
        {
            if (input.ObterValorOuPadrao("").Trim() == "")
                return "";

            return new string(caracterOculto, input.Length);
        }

        /// <summary>
        /// Limpa a string de palavras-chave SQL que podem ser usadas em ataques de injeção SQL.
        /// </summary>
        /// <param name="input">A string a ser limpa.</param>
        /// <returns>A string limpa de palavras-chave SQL perigosas.</returns>
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
