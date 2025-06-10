using JJ.Net.CrossData_WinUI_3.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData_WinUI_3.Interfaces
{
    internal interface IBancoDadosProvider
    {
        Task<IDbConnection> CriarConexaoAsync(ParametrosConfiguracao parametros);
        IDbConnection CriarConexao(ParametrosConfiguracao parametros);
    }
}
