using System;
using System.Collections.Generic;
using System.Text;

namespace JJ.UW.Core.Extensoes
{
    public static class ObjectExtension
    {
        public static bool EhNulo(this object valor)
        {
            return (valor == null);
        }

        public static int ConverterParaInt32(this object valor, int valorPadrao = 0 )
        {
            return int.TryParse(valor.ToString(), out int result) ? result : valorPadrao;
        }

        public static int ObterValorInt32(this object obj, string propriedade, int valorPadrao = 0)
        {
            if (obj == null || propriedade.ObterValorOuPadrao("").Trim() == "")
                return valorPadrao;

            var propInfo = obj.GetType().GetProperty(propriedade);

            if (propInfo == null)
                return valorPadrao;

            var valor = propInfo.GetValue(obj);   

            if(valor == null) 
                return valorPadrao;

            return valor.ConverterParaInt32();
        }
    }
}
