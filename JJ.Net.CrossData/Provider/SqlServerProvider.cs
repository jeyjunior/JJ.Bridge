using JJ.Net.CrossData.DTO;
using JJ.Net.CrossData.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData.Provider
{
    internal class SqlServerProvider : IBancoDadosProvider
    {
        public IDbConnection CriarConexao(ParametrosConfiguracao parametros)
        {
            // Lógica para SQL Server
            // Se string conexão fornecida, usa
            // Senão, tenta carregar de arquivo config
            return new Microsoft.Data.SqlClient.SqlConnection();
        }
    }
}
