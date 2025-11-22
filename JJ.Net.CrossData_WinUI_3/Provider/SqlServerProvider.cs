using JJ.Net.CrossData_WinUI_3.DTO;
using JJ.Net.CrossData_WinUI_3.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData_WinUI_3.Provider
{
    internal class SqlServerProvider : IBancoDadosProvider
    {
        public IDbConnection CriarConexao(ParametrosConfiguracao parametros)
        {
            if (parametros != null)
                return new Microsoft.Data.SqlClient.SqlConnection(parametros.StringConexao);

            return new Microsoft.Data.SqlClient.SqlConnection();
        }

        public Task<IDbConnection> CriarConexaoAsync(ParametrosConfiguracao parametros)
        {
            if (parametros != null)
                return Task.FromResult<IDbConnection>(new Microsoft.Data.SqlClient.SqlConnection(parametros.StringConexao));

            return Task.FromResult<IDbConnection>(new Microsoft.Data.SqlClient.SqlConnection()); 
        }
    }
}
