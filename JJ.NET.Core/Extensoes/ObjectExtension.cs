using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.Core.Extensoes
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Obtém o valor de uma propriedade de um objeto de forma dinâmica.
        /// </summary>
        /// <param name="objeto">Objeto do qual a propriedade será extraída.</param>
        /// <param name="nomePropriedade">Nome da propriedade a ser extraída.</param>
        /// <returns>Valor da propriedade como um `object`, ou `null` se não existir.</returns>
        public static object ObterPropriedade(this object objeto, string nomePropriedade)
        {
            if (objeto == null || string.IsNullOrWhiteSpace(nomePropriedade))
                return null;

            PropertyInfo propriedade = objeto.GetType().GetProperty(nomePropriedade);
            return propriedade?.GetValue(objeto);
        }

        /// <summary>
        /// Obtém o valor de uma propriedade e o converte para um tipo específico.
        /// </summary>
        /// <typeparam name="T">Tipo de retorno esperado.</typeparam>
        /// <param name="objeto">Objeto do qual a propriedade será extraída.</param>
        /// <param name="nomePropriedade">Nome da propriedade a ser extraída.</param>
        /// <param name="valorPadrao">Valor padrão a ser retornado caso a conversão falhe.</param>
        /// <returns>Valor convertido para o tipo `T` ou o valor padrão.</returns>
        public static T ObterPropriedade<T>(this object objeto, string nomePropriedade, T valorPadrao = default)
        {
            object valor = objeto.ObterPropriedade(nomePropriedade);
            if (valor == null)
                return valorPadrao;

            try
            {
                return (T)Convert.ChangeType(valor, typeof(T));
            }
            catch
            {
                return valorPadrao;
            }
        }

        /// <summary>
        /// Converte um objeto para o tipo especificado. Se a conversão falhar, retorna um valor padrão.
        /// </summary>
        /// <typeparam name="T">O tipo de destino</typeparam>
        /// <param name="valor">O valor a ser convertido</param>
        /// <param name="padrao">O valor padrão caso a conversão falhe</param>
        /// <returns>O valor convertido ou o valor padrão</returns>
        public static T ObterValorOuPadrao<T>(this object valor, T padrao)
        {
            if (valor == null || valor == DBNull.Value)
                return padrao;

            if (typeof(T) == typeof(string))
            {
                if (valor is string strValor && string.IsNullOrWhiteSpace(strValor) &&
                    padrao is string strPadrao && !string.IsNullOrWhiteSpace(strPadrao))
                {
                    return padrao;
                }
            }

            try
            {
                if (valor is T convertido)
                    return convertido;

                return (T)Convert.ChangeType(valor, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {
                return padrao;
            }
        }

        /// <summary>
        /// Converte um objeto para o tipo especificado. Se a conversão falhar, retorna um valor padrão.
        /// </summary>
        /// <typeparam name="T">O tipo de destino</typeparam>
        /// <param name="valor">O valor a ser convertido</param>
        /// <param name="padrao">O valor padrão caso a conversão falhe</param>
        /// <returns>O valor convertido ou o valor padrão</returns>
        public static T Converter<T>(this object valor, T padrao = default)
        {
            if (valor == null || valor == DBNull.Value)
                return padrao;

            try
            {
                if (valor is T convertido)
                    return convertido;

                return (T)Convert.ChangeType(valor, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {
                return padrao;
            }
        }

        /// <summary>
        /// Define o valor de uma propriedade de um objeto de forma dinâmica.
        /// </summary>
        /// <param name="objeto">Objeto no qual a propriedade será definida.</param>
        /// <param name="nomePropriedade">Nome da propriedade a ser definida.</param>
        /// <param name="valor">Valor a ser atribuído à propriedade.</param>
        /// <returns>True se a propriedade foi definida com sucesso, False caso contrário.</returns>
        public static bool DefinirValorParaPropriedade(this object objeto, string nomePropriedade, object valor)
        {
            if (objeto == null || string.IsNullOrWhiteSpace(nomePropriedade))
                return false;

            // Obtém a propriedade do objeto
            PropertyInfo propriedade = objeto.GetType().GetProperty(nomePropriedade);

            if (propriedade == null || !propriedade.CanWrite)
                return false;

            try
            {
                // Converte o valor para o tipo da propriedade, se necessário
                object valorConvertido = Convert.ChangeType(valor, propriedade.PropertyType);
                propriedade.SetValue(objeto, valorConvertido);
                return true;
            }
            catch
            {
                // Se a conversão falhar, retorna false
                return false;
            }
        }
    }
}
