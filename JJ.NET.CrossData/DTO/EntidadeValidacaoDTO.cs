using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.NET.CrossData.DTO
{
    public class EntidadeValidacaoDTO
    {
        public Type TipoEntidade { get; set; }
        public string Nome => TipoEntidade?.Name;
        public bool Existe { get; set; }
    }
}
