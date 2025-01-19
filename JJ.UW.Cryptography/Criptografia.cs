using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.UW.Cryptography.AES;
using JJ.UW.Cryptography.Enumerador;

namespace JJ.UW.Cryptography
{
    public static class Criptografia
    {
        public static string Criptografar(TipoCriptografia tipo, string valor)
        {
            string ret = "";

            switch (tipo)
            {
                case TipoCriptografia.AES: ret = CriptografiaAES.Criptografar(valor); break;
                case TipoCriptografia.RSA: break;
                case TipoCriptografia.DES: break;
                default: throw new NotImplementedException($"Algoritmo de criptografia {tipo} não implementado.");
            }

            return ret; 
        }

        public static string Descriptografar(TipoCriptografia tipo, string valor)
        {
            string ret = "";

            switch (tipo)
            {
                case TipoCriptografia.AES: ret = CriptografiaAES.Descriptografar(valor); break; 
                case TipoCriptografia.RSA: break; 
                case TipoCriptografia.DES: break;
                default: throw new NotImplementedException($"Algoritmo de criptografia {tipo} não implementado.");
            }

            return ret;
        }
    }

}
