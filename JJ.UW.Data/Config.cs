using JJ.UW.Core.Extensoes;
using JJ.UW.Data.DTO;
using JJ.UW.Data.Enumerador;
using System;
using System.IO;
using Windows.Storage;

namespace JJ.UW.Data
{
    public static class Config
    {
        public static eConexao Conexao { get; private set; }
        public static Parametros ConfiguracoesBanco { get; private set; } = null;

        public static void DefinirConexao(eConexao conexao)
        {
            Conexao = conexao;
            CarregarConfiguracoes();
        }

        public static void CarregarConfiguracoes()
        {
            Parametros configuracoesBanco = new Parametros 
            { 
                Sqlite = "", 
                SqlServer = "", 
                MySql = "", 
            };

            var tempFolder = ApplicationData.Current.TemporaryFolder;
            string caminhoBancoTemp = Path.Combine(tempFolder.Path, "dbsqlite.db");
            configuracoesBanco.Sqlite = caminhoBancoTemp;

            ConfiguracoesBanco = configuracoesBanco;
        }

        public static System.Data.IDbConnection ObterConexao()
        {
            switch (Conexao)
            {
                case eConexao.SQLite:       return ConectarSqlite();
                //case eConexao.SQLServer:    return ConectarSqlServer();
                case eConexao.MySql:        return ConectarMySql();
                default: throw new Exception("Nenhuma conexão identificada.");
            }
        }

        private static Microsoft.Data.Sqlite.SqliteConnection ConectarSqlite()
        {
            SQLitePCL.Batteries.Init();

            if (!File.Exists(ConfiguracoesBanco.Sqlite))
            {
                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={ConfiguracoesBanco.Sqlite}"))
                {
                    connection.Open();
                    connection.Close();
                }
            }

            return new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={ConfiguracoesBanco.Sqlite}");
        }

        //private static System.Data.SqlClient.SqlConnection ConectarSqlServer()
        //{
        //    string connString = @ConfiguracoesBanco.SqlServer;
        //    return new System.Data.SqlClient.SqlConnection(connString);
        //}

        private static MySqlConnector.MySqlConnection ConectarMySql()
        {
            string connString = @ConfiguracoesBanco.MySql;
            return new MySqlConnector.MySqlConnection(connString);
        }
    }
}
