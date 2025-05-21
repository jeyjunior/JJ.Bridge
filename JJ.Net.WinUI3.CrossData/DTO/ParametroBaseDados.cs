using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.WinUI3.CrossData.DTO
{
    public class ParametroBaseDados
    {
        public InfoBaseDados BaseAtiva { get; set; }
        public List<InfoBaseDados> BaseDados { get; set; }
    }

    public class InfoBaseDados
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string Valor { get; set; }
    }
}
