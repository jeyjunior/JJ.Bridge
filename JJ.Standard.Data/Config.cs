using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using JJ.Standard.Core.Enumerador;
using JJ.Standard.Core.Extensoes;
using JJ.Standard.Data.DTO;

namespace JJ.Standard.Data
{
    public static class Config
    {
        private static string arquivoParametros;
        public static Conexao ConexaoSelecionada { get; private set; }
        public static Parametros ConfiguracoesBanco { get; private set; } = null;

        static Config()
        {
            CarregarCaminhoArquivoParametros();
        }

        private static void CarregarCaminhoArquivoParametros()
        {
            // No .NET Standard, você pode usar o AppData do usuário ou um diretório específico
            string caminhoDiretorioAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            arquivoParametros = Path.Combine(caminhoDiretorioAppData, "configuracoes.json");
        }

        public static void Iniciar(Conexao conexao)
        {
            CarregarParametros();
            DefinirConexaoAtiva(conexao);
            CarregarConfiguracoes();
        }

        private static void CarregarParametros()
        {
            try
            {
                string caminhoArquivo = arquivoParametros;

                var sqlite = new Parametro
                {
                    ID = 1,
                    Nome = "Sqlite",
                    Valor = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "dbsqlite.db"),
                };

                var sqlServer = new Parametro
                {
                    ID = 2,
                    Nome = "SqlServer",
                    Valor = "",
                };

                var mySql = new Parametro
                {
                    ID = 3,
                    Nome = "MySql",
                    Valor = "",
                };

                var parametros = new Parametros
                {
                    BaseAtiva = sqlite,
                    BaseDados = new List<Parametro>
                    {
                        sqlite,
                        sqlServer,
                        mySql,
                    }
                };

                string json = JsonConvert.SerializeObject(parametros, Formatting.Indented);

                // Verifica se o diretório de configuração existe, caso contrário, cria
                Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivo));

                File.WriteAllText(caminhoArquivo, json);

                ConfiguracoesBanco = parametros;
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao carregar parâmetros de configuração.\n" + ex.Message, ex);
            }
        }

        private static void DefinirConexaoAtiva(Conexao conexao)
        {
            try
            {
                Parametro baseEscolhida = ConfiguracoesBanco.BaseDados.FirstOrDefault(i => (Conexao)i.ID == conexao);

                if (baseEscolhida != null)
                {
                    ConfiguracoesBanco.BaseAtiva = baseEscolhida;

                    string json = JsonConvert.SerializeObject(ConfiguracoesBanco, Formatting.Indented);
                    File.WriteAllText(arquivoParametros, json);

                    ConexaoSelecionada = conexao;
                }
                else
                {
                    throw new Exception("Base de dados não encontrada para a conexão selecionada.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao definir a conexão ativa.\n" + ex.Message, ex);
            }
        }

        private static void CarregarConfiguracoes()
        {
            try
            {
                string json = File.ReadAllText(arquivoParametros);
                ConfiguracoesBanco = JsonConvert.DeserializeObject<Parametros>(json);

                ConexaoSelecionada = (Conexao)ConfiguracoesBanco.BaseAtiva.ID;
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception("Arquivo de configurações não encontrado: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler as configurações: " + ex.Message, ex);
            }
        }

        public static System.Data.IDbConnection ObterConexao()
        {
            switch (ConexaoSelecionada)
            {
                case Conexao.SQLite: return ConectarSqlite();
                case Conexao.SQLServer: return ConectarSqlServer;
                case Conexao.MySql: return ConectarMySql();
            }

            return null;
        }

        private static Microsoft.Data.Sqlite.SqliteConnection ConectarSqlite()
        {
            SQLitePCL.Batteries_V2.Init();

            var sqlite = ConfiguracoesBanco.BaseDados.FirstOrDefault(i => i.ID == 1);

            if (!File.Exists(sqlite.Valor.ObterValorOuPadrao("").Trim()))
            {
                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={sqlite.Valor}"))
                {
                    connection.Open();
                    connection.Close();
                }
            }

            return new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={sqlite.Valor}");
        }

        private static Microsoft.Data.SqlClient.SqlConnection ConectarSqlServer
        {
            get
            {
                string connString = ConfiguracoesBanco.BaseDados.FirstOrDefault(i => i.ID == 2).Valor.ObterValorOuPadrao("").Trim();
                return new Microsoft.Data.SqlClient.SqlConnection(connString);
            }
        }

        private static MySqlConnector.MySqlConnection ConectarMySql()
        {
            string connString = ConfiguracoesBanco.BaseDados.FirstOrDefault(i => i.ID == 3).Valor.ObterValorOuPadrao("").Trim();
            return new MySqlConnector.MySqlConnection(connString);
        }
    }
}


//using JJ.Standard.Core.Extensoes;
//using JJ.Standard.Data.DTO;
//using JJ.Standard.Data.Enum;
//using System;
//using System.IO;
//using System.Text.Json;

//namespace JJ.Standard.Data
//{
//    public static class Config
//    {
//        private static string _caminhoServidor = "";

//        public static eConexao Conexao { get; private set; }
//        public static Parametros? ConfiguracoesBanco { get; private set; } = null;

//        public static void DefinirConexao(eConexao conexao)
//        {
//            Conexao = conexao;
//            CarregarConfiguracoes();
//        }

//        public static void DefinirCaminhoServidor(string caminho)
//        {
//            _caminhoServidor = caminho;
//        }

//        public static void CarregarConfiguracoes()
//        {
//            string diretorioExecucao = AppDomain.CurrentDomain.BaseDirectory;
//            string nomeProjeto = AppDomain.CurrentDomain.FriendlyName;
//            var diretorioAtual = new DirectoryInfo(diretorioExecucao);

//            while (diretorioAtual != null && diretorioAtual.Name != nomeProjeto)
//                diretorioAtual = diretorioAtual.Parent;

//            if (diretorioAtual == null)
//                throw new Exception("Falha ao identificar o diretório do projeto.");

//            string raizProjeto = diretorioAtual.FullName;
//            string caminhoArquivoJson = Path.Combine(raizProjeto, "parametros.json");

//            if (!File.Exists(caminhoArquivoJson))
//                throw new Exception("Arquivo de configurações 'parametros.json' não encontrado.");

//            string jsonString = File.ReadAllText(caminhoArquivoJson);

//            var configuracoesBanco = JsonSerializer.Deserialize<Parametros>(jsonString);

//            if (configuracoesBanco == null)
//                throw new Exception("Falha ao carregar as configurações do banco.");

//            if (Conexao == eConexao.SQLite)
//            {
//                if(_caminhoServidor.ObterValorOuPadrao("").Trim() != "")
//                {
//                    configuracoesBanco.Sqlite = Path.Combine(_caminhoServidor, "dbsqlite.db");
//                }
//                else
//                {
//                    string caminhoTemp = Path.Combine(Path.GetTempPath(), "DBCore");
//                    Directory.CreateDirectory(caminhoTemp); 
//                    configuracoesBanco.Sqlite = Path.Combine(caminhoTemp, "dbsqlite.db");
//                }
//            }

//            ConfiguracoesBanco = configuracoesBanco;
//        }

//        public static System.Data.IDbConnection ObterConexao()
//        {
//            switch (Conexao)
//            {
//                case eConexao.SQLite:       return ConectarSqlite();
//                case eConexao.SQLServer:    return ConectarSqlServer();
//                case eConexao.MySql:        return ConectarMySql();
//                default: throw new Exception("Nenhuma conexão identificada.");
//            }
//        }

//        private static Microsoft.Data.Sqlite.SqliteConnection ConectarSqlite()
//        {
//            SQLitePCL.Batteries.Init();

//            if (!File.Exists(ConfiguracoesBanco.Sqlite))
//            {
//                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={ConfiguracoesBanco.Sqlite}"))
//                {
//                    connection.Open();
//                    connection.Close();
//                }
//            }

//            return new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={ConfiguracoesBanco.Sqlite}");
//        }

//        private static Microsoft.Data.SqlClient.SqlConnection ConectarSqlServer()
//        {
//            string connString = @ConfiguracoesBanco.SqlServer;
//            return new Microsoft.Data.SqlClient.SqlConnection(connString);
//        }

//        private static MySqlConnector.MySqlConnection ConectarMySql()
//        {
//            string connString = @ConfiguracoesBanco.MySql;
//            return new MySqlConnector.MySqlConnection(connString);
//        }
//    }
//}
