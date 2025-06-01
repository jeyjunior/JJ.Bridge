using JJ.Net.CrossData.DTO;
using JJ.Net.CrossData.Interfaces;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData.Provider
{
    internal class SqliteProvider : IBancoDadosProvider
    {
        public IDbConnection CriarConexao(ParametrosConfiguracao parametros)
        {
            // Lógica para SQLite
            // Se caminho fornecido, usa existente
            // Senão, cria na pasta da aplicação
            return new SqliteConnection(/* connection string */);
        }
    }
}
