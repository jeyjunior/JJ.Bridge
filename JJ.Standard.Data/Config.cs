using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using JJ.Standard.Data.DTO;
using JJ.Standard.Core.Enumerador;
using JJ.Standard.Core.Extensoes;

namespace JJ.Standard.Data
{
    public static class Config
    {
        private static string caminhoArquivos = "";
        private static string arquivoParametros;
        public static Conexao ConexaoSelecionada { get; private set; }
        public static Parametros ConfiguracoesBanco { get; private set; } = null;

        static Config()
        {
        }

        private static void CarregarCaminhoArquivoParametros(string nomeAplicacao)
        {
            string caminhoDiretorioAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            caminhoArquivos = Path.Combine(caminhoDiretorioAppData, nomeAplicacao);

            arquivoParametros = Path.Combine(caminhoArquivos, "configuracoes.json");
        }

        public static void Iniciar(Conexao conexao, string nomeAplicacao = "JeyJunior")
        {
            CarregarCaminhoArquivoParametros(nomeAplicacao);
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
                    Valor = Path.Combine(caminhoArquivos, "dbsqlite.db"),
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
