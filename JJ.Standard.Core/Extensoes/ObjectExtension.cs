using System;
using System.Collections.Generic;
using System.Text;

namespace JJ.Standard.Core.Extensoes
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Verifica se o valor de um objeto é nulo.
        /// </summary>
        /// <param name="valor">O objeto a ser verificado.</param>
        /// <returns>Retorna true se o objeto for nulo, caso contrário, retorna false.</returns>
        public static bool EhNulo(this object valor)
        {
            return (valor == null);
        }

        /// <summary>
        /// Converte um objeto para inteiro, retornando um valor padrão caso o objeto seja nulo ou a conversão falhe.
        /// </summary>
        /// <param name="valor">O objeto a ser convertido para inteiro.</param>
        /// <param name="valorPadrao">O valor padrão a ser retornado caso a conversão falhe.</param>
        /// <returns>O valor convertido para inteiro ou o valor padrão caso a conversão falhe.</returns>
        public static int ConverterParaInt32(this object valor, int valorPadrao = 0)
        {
            if (valor == null)
                return valorPadrao;

            return int.TryParse(valor.ToString(), out int result) ? result : valorPadrao;
        }

        /// <summary>
        /// Converte um objeto para inteiro, mas retorna um valor nulo caso o objeto seja nulo ou a conversão falhe.
        /// </summary>
        /// <param name="valor">O objeto a ser convertido para inteiro.</param>
        /// <returns>O valor convertido para inteiro ou null caso a conversão falhe.</returns>
        public static int? ConverterParaInt32Nullable(this object valor)
        {
            if (valor == null)
                return null;

            return int.TryParse(valor.ToString(), out int result) ? result : (int?)null;
        }

        /// <summary>
        /// Obtém o valor de uma propriedade de um objeto e tenta convertê-lo para um valor do tipo <see cref="Int32"/>.
        /// Se a propriedade não existir, se o valor for nulo ou se não puder ser convertido, retorna o valor padrão fornecido.
        /// </summary>
        /// <param name="obj">O objeto do qual a propriedade será acessada.</param>
        /// <param name="propriedade">O nome da propriedade que será acessada no objeto.</param>
        /// <param name="valorPadrao">Valor retornado caso a propriedade não exista, seja nula ou não possa ser convertida para <see cref="Int32"/>. O valor padrão é 0, se não especificado.</param>
        /// <returns>O valor convertido para <see cref="Int32"/>, ou o valor padrão caso haja algum erro no processo.</returns>
        /// <example>
        /// <code>
        /// var valor = objeto.ObterValorInt32("Quantidade", 10);
        /// </code>
        /// </example>
        public static int ObterValorInt32(this object obj, string propriedade, int valorPadrao = 0)
        {
            if (obj == null || propriedade.ObterValorOuPadrao("").Trim() == "")
                return valorPadrao;

            var propInfo = obj.GetType().GetProperty(propriedade);

            if (propInfo == null)
                return valorPadrao;

            var valor = propInfo.GetValue(obj);

            if (valor == null)
                return valorPadrao;

            return valor.ConverterParaInt32();
        }

        /// <summary>
        /// Obtém o valor de uma propriedade de um objeto e tenta convertê-lo para uma string.
        /// Se a propriedade não existir, se o valor for nulo ou se não puder ser convertido, retorna o valor padrão fornecido.
        /// </summary>
        /// <param name="obj">O objeto do qual a propriedade será acessada.</param>
        /// <param name="propriedade">O nome da propriedade que será acessada no objeto.</param>
        /// <param name="valorPadrao">Valor retornado caso a propriedade não exista, seja nula ou não possa ser convertida para string. O valor padrão é uma string vazia, se não especificado.</param>
        /// <returns>O valor convertido para string, ou o valor padrão caso haja algum erro no processo.</returns>
        /// <example>
        /// <code>
        /// var valor = objeto.ObterValorString("Nome", "Valor Padrão");
        /// </code>
        /// </example>
        public static string ObterValorString(this object obj, string propriedade, string valorPadrao = "")
        {
            if (obj == null || string.IsNullOrWhiteSpace(propriedade))
                return valorPadrao;

            var propInfo = obj.GetType().GetProperty(propriedade);

            if (propInfo == null)
                return valorPadrao;

            var valor = propInfo.GetValue(obj);

            if (valor == null)
                return valorPadrao;

            return valor.ToString();
        }

        /// <summary>
        /// Obtém o valor de uma propriedade de um objeto e tenta convertê-lo para um valor do tipo <see cref="DateTime"/>.
        /// Se a propriedade não existir, se o valor for nulo ou se não puder ser convertido, retorna o valor padrão fornecido.
        /// </summary>
        /// <param name="obj">O objeto do qual a propriedade será acessada.</param>
        /// <param name="propriedade">O nome da propriedade que será acessada no objeto.</param>
        /// <param name="valorPadrao">Valor retornado caso a propriedade não exista, seja nula ou não possa ser convertida para <see cref="DateTime"/>. O valor padrão é o valor mínimo de <see cref="DateTime"/>, se não especificado.</param>
        /// <returns>O valor convertido para <see cref="DateTime"/>, ou o valor padrão caso haja algum erro no processo.</returns>
        /// <example>
        /// <code>
        /// var valor = objeto.ObterValorDateTime("DataNascimento", DateTime.MinValue);
        /// </code>
        /// </example>
        public static DateTime ObterValorDateTime(this object obj, string propriedade, DateTime valorPadrao = default)
        {
            if (obj == null || propriedade.ObterValorOuPadrao("").Trim() == "")
                return valorPadrao;

            var propInfo = obj.GetType().GetProperty(propriedade);

            if (propInfo == null)
                return valorPadrao;

            var valor = propInfo.GetValue(obj);

            if (valor == null)
                return valorPadrao;

            if (valor is DateTime)
                return (DateTime)valor;

            if (valor is string strValor && DateTime.TryParse(strValor, out DateTime parsedDate))
                return parsedDate;

            return valorPadrao;
        }
    }
}
