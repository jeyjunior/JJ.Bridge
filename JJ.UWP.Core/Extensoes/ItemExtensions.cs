using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.UWP.Core.DTO;

namespace JJ.UWP.Core.Extensoes
{
    public static class ItemExtensions
    {
        /// <summary>
        /// Busca um item na coleção pelo ID.
        /// </summary>
        public static Item ObterPorID(this IEnumerable<Item> itens, object id)
        {
            if (itens == null || !itens.Any())
                return null;

            string _id = id.ObterValorOuPadrao("0").Trim();

            return itens.FirstOrDefault(i => i.ID.Equals(_id));
        }

        /// <summary>
        /// Verifica se um item com determinado ID existe na coleção.
        /// </summary>
        public static bool ExisteItem(this IEnumerable<Item> itens, object id)
        {
            return itens.ObterPorID(id) != null;
        }

        /// <summary>
        /// Seleciona um item na coleção e o retorna (ou um novo se não existir).
        /// </summary>
        public static Item SelecionarOuPadrao(this IEnumerable<Item> itens, object id, Item padrao = null)
        {
            return itens.ObterPorID(id) ?? padrao;
        }
    }
}
