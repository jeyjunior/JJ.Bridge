using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Standard.Core.Extensoes
{
    public static class BooleanExtension
    {
        public static bool ConverterParaBolean(this int valor)
        {
            return valor != 0;
        }

        public static bool ConverterParaBoolean(this int valor, int valorPadraoVerdadeiro = 1)
        {
            return valor == valorPadraoVerdadeiro;
        }

        public static bool ObterValorOuPadrao(this bool? valor, bool padrao = false)
        {
            if (valor == null)
                return padrao;

            return valor.Value;
        }
    }
}
