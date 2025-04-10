using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.NET.Core.Extensoes
{
    public static class EnumerableExtensions
    {
        public static T? ObterPorID<T>(this IEnumerable<T> itens, object id, Func<T, object> seletorID)
        {
            if (itens == null || !itens.Any())
                return default;

            string _id = id.ToString()?.Trim() ?? "0";
            return itens.FirstOrDefault(i => seletorID(i)?.ToString()?.Trim() == _id);
        }

        public static bool ExisteItem<T>(this IEnumerable<T> itens, object id, Func<T, object> seletorID)
        {
            return itens.ObterPorID(id, seletorID) != null;
        }

        public static T SelecionarOuPadrao<T>(this IEnumerable<T> itens, object id, Func<T, object> seletorID, T padrao = default)
        {
            return itens.ObterPorID(id, seletorID) ?? padrao;
        }
    }
}
