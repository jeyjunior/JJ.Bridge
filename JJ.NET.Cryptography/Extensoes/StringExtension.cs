using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.NET.Cryptography.Enumerador;

namespace JJ.NET.Cryptography.Extensoes
{
    public static class StringExtension
    {
        public static string Descriptografar(this string valor, string IV, TipoCriptografia tipoCriptografia)
        {
            try
            {
                if (string.IsNullOrEmpty(valor) || string.IsNullOrEmpty(IV))
                    return "";

                CriptografiaRequest criptografiaRequest = new CriptografiaRequest
                {
                    Valor = valor,
                    IV = IV,
                    TipoCriptografia = tipoCriptografia,
                };

                var ret = Criptografia.Descriptografar(criptografiaRequest);
                return ret.Valor;
            }
            catch 
            {
                return "";
            }
        }

        public static string Criptografar(this string valor, string IV, TipoCriptografia tipoCriptografia)
        {
            try
            {
                if (string.IsNullOrEmpty(valor) || string.IsNullOrEmpty(IV))
                    return "";

                CriptografiaRequest criptografarRequest = new CriptografiaRequest
                {
                    Valor = valor,
                    IV = IV,
                    TipoCriptografia = tipoCriptografia,
                };

                var ret = Criptografia.Criptografar(criptografarRequest);
                return ret.Valor;
            }
            catch
            {
                return "";
            }
        }
    }
}
