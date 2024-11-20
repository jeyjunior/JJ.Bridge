using System;
using System.Collections.Generic;
using System.Text;

namespace JJ.UW.Core.Extensoes
{
    public static class DateTimeExtension
    {
        public static string ObterValorOuPadrao(this DateTime? d, string padrao, string formato = "d")
        {
            if (formato.Length > 0)
                return d != null ? d.Value.ToString(formato) : padrao;

            return d != null ? d.Value.ToString() : padrao;
        }
    }
}
