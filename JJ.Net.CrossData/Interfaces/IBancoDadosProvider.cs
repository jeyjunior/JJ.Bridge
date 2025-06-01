using JJ.Net.CrossData.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData.Interfaces
{
    internal interface IBancoDadosProvider
    {
        IDbConnection CriarConexao(ParametrosConfiguracao parametros);
    }
}
