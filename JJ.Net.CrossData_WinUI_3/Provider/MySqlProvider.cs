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
    internal class MySqlProvider : IBancoDadosProvider
    {
        public IDbConnection CriarConexao(ParametrosConfiguracao parametros)
        {
            return new MySqlConnector.MySqlConnection();
        }

        public Task<IDbConnection> CriarConexaoAsync(ParametrosConfiguracao parametros)
        {
            // Lógica para MySql
            // Se string conexão fornecida, usa
            // Senão, tenta carregar de arquivo config
            return Task.FromResult<IDbConnection>(new MySqlConnector.MySqlConnection());
        }
    }
}
