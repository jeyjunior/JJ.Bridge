using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.UW.Core.Atributos;

namespace AppTesteUWP
{
    public class Pessoa
    {
        [ChavePrimaria]
        public int PK_Pessoa { get; set; }
        public string Nome {  get; set; }   
    }
}
