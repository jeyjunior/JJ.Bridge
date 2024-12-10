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

        public static void Iniciar(eConexao eConexao)
        {
            CarregarParametros();
            DefinirConexaoAtiva(eConexao);
            CarregarConfiguracoes();
        }

        private static void CarregarParametros()
        {
            try
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
            catch (ArgumentNullException ex)
            {
                throw new Exception("Erro ao carregar parâmetros: valor nulo encontrado.\n" + ex.Message, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("Erro de permissão ao acessar arquivos ou pastas.\n" + ex.Message, ex);
            }
            catch (IOException ex)
            {
                throw new Exception("Erro ao ler ou escrever no arquivo.\n" + ex.Message, ex);
            }
            catch (JsonSerializationException ex)
            {
                throw new Exception("Erro ao serializar os parâmetros para JSON.\n" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao carregar parametros de configuração.\n" + ex.Message, ex);
            }
        }

        private static void DefinirConexaoAtiva(eConexao eConexao)
        {
            try
            {
                Parametro baseEscolhida = ConfiguracoesBanco.BaseDados.FirstOrDefault(i => (eConexao)i.ID == eConexao);

                if (baseEscolhida != null)
                {
                    ConfiguracoesBanco.BaseAtiva = baseEscolhida;

                    string json = JsonConvert.SerializeObject(ConfiguracoesBanco, Formatting.Indented);
                    File.WriteAllText(arquivoParametros, json);

                    Conexao = eConexao;
                }
                else
                {
                    throw new Exception("Base de dados não encontrada para a conexão selecionada.");
                }
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception("Erro: algum parâmetro necessário está nulo ao serializar ou salvar as configurações.\n" + ex.Message, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("Erro de permissão: não foi possível acessar o arquivo para salvar as configurações.\n" + ex.Message, ex);
            }
            catch (IOException ex)
            {
                throw new Exception("Erro ao tentar ler ou escrever no arquivo de configurações.\n" + ex.Message, ex);
            }
            catch (JsonSerializationException ex)
            {
                throw new Exception("Erro ao serializar as configurações para JSON.\n" + ex.Message, ex);
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

                Conexao = (eConexao)ConfiguracoesBanco.BaseAtiva.ID;
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception("Arquivo de configurações não encontrado: " + ex.Message, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("Erro de permissão ao tentar acessar o arquivo de configurações.\n" + ex.Message, ex);
            }
            catch (IOException ex)
            {
                throw new Exception("Erro ao ler o arquivo de configurações.\n" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler as configurações: " + ex.Message, ex);
            }
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
