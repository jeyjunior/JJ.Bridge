using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.WinUI3.CrossData.DTO
{
    public class EntidadeValidacao
    {
        public Type TipoEntidade { get; set; }
        public string Nome => TipoEntidade?.Name;
        public bool Existe { get; set; }
    }
}
