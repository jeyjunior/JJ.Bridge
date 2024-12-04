using System;
using System.IO;
using Windows.Storage;
using Newtonsoft.Json;
using JJ.UW.Core.Extensoes;
using JJ.UW.Data.DTO;
using System.Linq;
using JJ.UW.Core.Enumerador;

namespace JJ.UW.Data
{
    public static class Config
    {
        private static string arquivoParametros;

        public static eConexao Conexao { get; private set; }
        public static Parametros ConfiguracoesBanco { get; private set; } = null;

        static Config()
        {
            CarregarCaminhoArquivoParametros();
        }

        private static void CarregarCaminhoArquivoParametros()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            arquivoParametros = Path.Combine(localFolder.Path, "configuracoes.json");
        }

        public static bool PrimeiroAcesso()
        {
            try
            {
                bool arquivoExiste = File.Exists(arquivoParametros);
                
                CarregarParametros();

                return arquivoExiste;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static void DefinirConexaoAtiva(eConexao eConexao)
        {
            Parametro baseEscolhida = ConfiguracoesBanco.BaseDados.FirstOrDefault(i => (eConexao)i.ID == eConexao);

            if (baseEscolhida != null)
            {
                ConfiguracoesBanco.BaseAtiva = baseEscolhida;

                try
                {
                    string json = JsonConvert.SerializeObject(ConfiguracoesBanco, Formatting.Indented);
                    File.WriteAllText(arquivoParametros, json);
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao salvar as configurações: " + ex.Message);
                }

                Conexao = eConexao;
            }
            else
            {
                throw new Exception("Base de dados não encontrada para a conexão selecionada.");
            }
        }

        public static void CarregarConfiguracoes()
        {
            try
            {
                string json = File.ReadAllText(arquivoParametros);
                ConfiguracoesBanco = JsonConvert.DeserializeObject<Parametros>(json);

                Conexao = (eConexao)ConfiguracoesBanco.BaseAtiva.ID;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler as configurações: " + ex.Message);
            }
        }

        private static void CarregarParametros()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            string caminhoArquivo = Path.Combine(localFolder.Path, arquivoParametros);

            var sqlite = new Parametro
            {
                ID = 1,
                Nome = "Sqlite",
                Valor = Path.Combine(ApplicationData.Current.LocalFolder.Path, "dbsqlite.db"),
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
                BaseDados = new System.Collections.Generic.List<Parametro>() 
                {
                    sqlite,
                    sqlServer,
                    mySql,
                }
            };

            string json = JsonConvert.SerializeObject(parametros, Formatting.Indented);

            File.WriteAllText(caminhoArquivo, json);

            ConfiguracoesBanco = parametros;
        }

        public static System.Data.IDbConnection ObterConexao()
        {
            switch (Conexao)
            {
                case eConexao.SQLite: return ConectarSqlite();
                case eConexao.SQLServer: return ConectarSqlServer();
                case eConexao.MySql: return ConectarMySql();
                default: throw new Exception("Nenhuma conexão identificada.");
            }
        }

        private static Microsoft.Data.Sqlite.SqliteConnection ConectarSqlite()
        {
            SQLitePCL.Batteries.Init();

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

        private static System.Data.SqlClient.SqlConnection ConectarSqlServer()
        {
            string connString = ConfiguracoesBanco.BaseDados.FirstOrDefault(i => i.ID == 2).Valor.ObterValorOuPadrao("").Trim();
            return new System.Data.SqlClient.SqlConnection(connString);
        }

        private static MySqlConnector.MySqlConnection ConectarMySql()
        {
            string connString = ConfiguracoesBanco.BaseDados.FirstOrDefault(i => i.ID == 3).Valor.ObterValorOuPadrao("").Trim();
            return new MySqlConnector.MySqlConnection(connString);
        }
    }
}
