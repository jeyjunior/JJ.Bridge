using JJ.Standard.Data.DTO;
using JJ.Standard.Data.Enum;
using System;
using System.IO;
using System.Text.Json;

namespace JJ.Standard.Data
{
    public static class Config
    {
        public static eConexao Conexao { get; private set; }
        public static Parametros? ConfiguracoesBanco { get; private set; } = null;

        public static void DefinirConexao(eConexao conexao)
        {
            Conexao = conexao;

            CarregarConfiguracoes();
        }

        public static void CarregarConfiguracoes()
        {
            string diretorioExecucao = AppDomain.CurrentDomain.BaseDirectory;
            string nomeProjeto = AppDomain.CurrentDomain.FriendlyName;
            var diretorioAtual = new DirectoryInfo(diretorioExecucao);

            while (diretorioAtual != null && diretorioAtual.Name != nomeProjeto)
                diretorioAtual = diretorioAtual.Parent;
            
            if (diretorioAtual == null)
                throw new Exception("Falha ao identificar o diretório do projeto.");
            
            string raizProjeto = diretorioAtual.FullName;
            string caminhoArquivoJson = Path.Combine(raizProjeto, "parametros.json");

            if (!File.Exists(caminhoArquivoJson))
                throw new Exception("Arquivo de configurações 'parametros.json' não encontrado.");
            
            string jsonString = File.ReadAllText(caminhoArquivoJson);

            var configuracoesBanco = JsonSerializer.Deserialize<Parametros>(jsonString);

            if (configuracoesBanco == null)
                throw new Exception("Falha ao carregar as configurações do banco.");

            if(Conexao == eConexao.SQLite)
            {
                string pathSqlite = Path.Combine(raizProjeto, "DBCore");
                Directory.CreateDirectory(pathSqlite);
                configuracoesBanco.Sqlite = Path.Combine(pathSqlite, "dbsqlite.db");
            }

            ConfiguracoesBanco = configuracoesBanco;
        }

        public static System.Data.IDbConnection ObterConexao()
        {
            switch (Conexao)
            {
                case eConexao.SQLite:       return ConectarSqlite();
                case eConexao.SQLServer:    return ConectarSqlServer();
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

        private static Microsoft.Data.SqlClient.SqlConnection ConectarSqlServer()
        {
            string connString = @ConfiguracoesBanco.SqlServer;
            return new Microsoft.Data.SqlClient.SqlConnection(connString);
        }

        private static MySqlConnector.MySqlConnection ConectarMySql()
        {
            string connString = @ConfiguracoesBanco.MySql;
            return new MySqlConnector.MySqlConnection(connString);
        }
    }
}
