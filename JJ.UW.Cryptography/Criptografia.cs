using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using JJ.UW.Cryptography.AES;
using JJ.UW.Cryptography.Enumerador;
using Windows.UI.WebUI;

namespace JJ.UW.Cryptography
{
    public static class Criptografia
    {
        public static CriptografarResult Criptografar(CriptografarRequest criptografarRequest)
        {
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
                result.Erro = "Erro ao processar a criptografia. Tente novamente ou verifique a chave de criptografia.\n" +ex.Message ;
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

    public class CriptografarResult
    {
        public string Valor { get; set; }
        public string IV { get; set; }

        public string Erro { get; set; }
    }

    public class CriptografarRequest
    {
        public TipoCriptografia TipoCriptografia { get; set; }
        public string Valor { get; set; }
        public string IV { get; set; }
    }

    public class DescriptografarRequest
    {
        public TipoCriptografia TipoCriptografia { get; set; }
        public string Valor { get; set; }
        public string IV { get; set; }
    }
    public class DescriptografarResult
    {
        public string Valor { get; set; }
        public string Erro { get; set; }
    }
}
