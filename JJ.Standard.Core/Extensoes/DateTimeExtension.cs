using System;
using System.Collections.Generic;
using System.Text;

namespace JJ.Standard.Core.Extensoes
{
    public static class DateTimeExtension
    {
        public static string ObterValorOuPadrao(this DateTime? d, string padrao, string formato = "d")
        {
            if (formato.Length > 0)
                return d != null ? d.Value.ToString(formato) : padrao;

            return d != null ? d.Value.ToString() : padrao;
        }

        public static DateTime ObterValorOuPadrao(this DateTime? d, DateTime padrao = default)
        {
            if (d == null)
                return padrao;

            return d.Value;
        }

        public static DateTime ObterValorOuPadrao(this DateTimeOffset? d, DateTime padrao = default) 
        {
            if (d == null)
                return padrao;

            return d.Value.DateTime;
        }

        public static DateTime? ConverterParaDateTime(this DateTimeOffset? d)
        {
            return d?.DateTime;
        }

        public static DateTime ObterPrimeiroDiaDoMes(this DateTime data)
        {
            return new DateTime(data.Year, data.Month, 1);
        }

        public static DateTime ObterUltimoDiaDoMes(this DateTime data)
        {
            return new DateTime(data.Year, data.Month, 1).AddMonths(1).AddDays(-1);
        }

        public static DateTime ObterPrimeiroDiaDoMesSeguinte(this DateTime data)
        {
            return new DateTime(data.Year, data.Month, 1).AddMonths(1);
        }
    }
}
