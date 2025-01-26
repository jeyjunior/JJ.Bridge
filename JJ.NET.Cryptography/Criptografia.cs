using JJ.NET.Cryptography.AES;
using JJ.NET.Cryptography.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JJ.NET.Cryptography
{
    /// <summary>
    /// Classe responsável por fornecer métodos para criptografar e descriptografar dados
    /// utilizando diferentes algoritmos de criptografia.
    /// </summary>
    public static class Criptografia
    {
        /// <summary>
        /// Método que criptografa os dados fornecidos no objeto de requisição.
        /// Suporta os algoritmos AES, RSA, e DES. Atualmente, apenas AES está implementado.
        /// </summary>
        /// <param name="criptografarRequest">Objeto contendo as informações necessárias para criptografar os dados.</param>
        /// <returns>Resultado da criptografia, incluindo o valor criptografado, o IV e mensagens de erro se houverem.</returns>
        /// <exception cref="NotImplementedException">Caso um algoritmo não implementado seja solicitado.</exception>
        public static CriptografarResult Criptografar(CriptografarRequest criptografarRequest)
        {
            // Resultado inicial com valores vazios
            var result = new CriptografarResult() { Valor = "", IV = "", Erro = "" };

            try
            {
                switch (criptografarRequest.TipoCriptografia)
                {
                    case TipoCriptografia.AES: result = CriptografiaAES.Criptografar(criptografarRequest); break;
                    case TipoCriptografia.RSA: break;
                    case TipoCriptografia.DES: break;
                    default: throw new NotImplementedException($"Algoritmo de criptografia {criptografarRequest.TipoCriptografia} não implementado.");
                }
            }
            catch (CryptographicException ex)
            {
                result.Erro = "Erro ao processar a criptografia. Tente novamente ou verifique a chave de criptografia.\n" + ex.Message;
            }
            catch (IOException ex)
            {
                result.Erro = "Erro ao tentar acessar dados locais. Por favor, verifique a permissão ou a integridade dos arquivos.\n" + ex.Message;
            }
            catch (FormatException ex)
            {
                result.Erro = "Formato inválido detectado. Verifique os dados fornecidos e tente novamente.\n" + ex.Message;
            }
            catch (Exception ex)
            {
                result.Erro = "Ocorreu um erro inesperado. Tente novamente mais tarde.\n" + ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Método que descriptografa os dados fornecidos no objeto de requisição.
        /// Suporta os algoritmos AES, RSA, e DES. Atualmente, apenas AES está implementado.
        /// </summary>
        /// <param name="descriptografarRequest">Objeto contendo as informações necessárias para descriptografar os dados.</param>
        /// <returns>Resultado da descriptografia, incluindo o valor descriptografado e mensagens de erro se houverem.</returns>
        /// <exception cref="NotImplementedException">Caso um algoritmo não implementado seja solicitado.</exception>
        public static DescriptografarResult Descriptografar(DescriptografarRequest descriptografarRequest)
        {
            var result = new DescriptografarResult { Valor = "", Erro = "" };

            try
            {
                switch (descriptografarRequest.TipoCriptografia)
                {
                    case TipoCriptografia.AES: result = CriptografiaAES.Descriptografar(descriptografarRequest); break;
                    case TipoCriptografia.RSA: break;
                    case TipoCriptografia.DES: break;
                    default: throw new NotImplementedException($"Algoritmo de criptografia {descriptografarRequest.TipoCriptografia} não implementado.");
                }
            }
            catch (CryptographicException ex)
            {
                result.Erro = "Erro ao processar a criptografia. Tente novamente ou verifique a chave de criptografia.\n" + ex.Message;
            }
            catch (IOException ex)
            {
                result.Erro = "Erro ao tentar acessar dados locais. Por favor, verifique a permissão ou a integridade dos arquivos.\n" + ex.Message;
            }
            catch (FormatException ex)
            {
                result.Erro = "Formato inválido detectado. Verifique os dados fornecidos e tente novamente.\n" + ex.Message;
            }
            catch (Exception ex)
            {
                result.Erro = "Ocorreu um erro inesperado. Tente novamente mais tarde.\n" + ex.Message;
            }

            return result;
        }
    }

    /// <summary>
    /// Resultado da operação de criptografia.
    /// Contém o valor criptografado, o IV (vetor de inicialização) e uma mensagem de erro, se houver.
    /// </summary>
    public class CriptografarResult
    {
        public string Valor { get; set; }
        public string IV { get; set; }
        public string Erro { get; set; }
    }

    /// <summary>
    /// Dados necessários para realizar uma operação de criptografia.
    /// Contém o tipo de criptografia, o valor a ser criptografado e o IV (se necessário).
    /// </summary>
    public class CriptografarRequest
    {
        public TipoCriptografia TipoCriptografia { get; set; }
        public string Valor { get; set; }
        public string IV { get; set; }
    }

    /// <summary>
    /// Dados necessários para realizar uma operação de descriptografia.
    /// Contém o tipo de criptografia, o valor a ser descriptografado e o IV (se necessário).
    /// </summary>
    public class DescriptografarRequest
    {
        public TipoCriptografia TipoCriptografia { get; set; } // Tipo de criptografia utilizada
        public string Valor { get; set; } // Valor a ser descriptografado
        public string IV { get; set; }    // Vetor de inicialização (se necessário)
    }

    /// <summary>
    /// Resultado da operação de descriptografia.
    /// Contém o valor descriptografado e uma mensagem de erro, se houver.
    /// </summary>
    public class DescriptografarResult
    {
        public string Valor { get; set; }
        public string Erro { get; set; }
    }
}
