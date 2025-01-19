using System;
using System.Text;
using System.Collections.Generic;

namespace JJ.UW.Core.DTOs
{
    public class Item
    {
        public string ID { get; set; }
        public string Nome { get; set; }

        public Item() { }

        public Item(string id, string nome)
        {
            ID = id;
            Nome = nome;
        }
    }

    public class ItemCheck : Item
    {
        public bool Check { get; set; }

        public ItemCheck() { }

        public ItemCheck(string id, string nome, bool check) : base(id, nome)
        {
            Check = check;
        }
    }
}

