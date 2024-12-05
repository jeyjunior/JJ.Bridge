using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.UW.Core.Extensoes
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
    }
}
