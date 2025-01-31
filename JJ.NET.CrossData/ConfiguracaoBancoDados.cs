using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JJ.NET.CrossData.Enumerador;
using JJ.NET.CrossData.DTO;

namespace JJ.NET.CrossData
{
    /// <summary>
    /// Classe responsável por carregar e gerenciar a configuração de conexões com os bancos de dados.
    /// </summary>
    public static class ConfiguracaoBancoDados
    {
        private static string _diretorioArquivosConfig = string.Empty;
        private static string _caminhoArquivoConfiguracoes;
        public static Conexao TipoConexaoSelecionada { get; private set; }
        public static Parametros ConfiguracaoAtual { get; private set; } = null;

        static ConfiguracaoBancoDados() { }

        /// <summary>
        /// Método responsável por iniciar a configuração das conexões de banco de dados.
        /// </summary>
        /// <param name="tipoConexao">Tipo de conexão selecionado.</param>
        /// <param name="nomeAplicacao">Nome da aplicação para organizar os arquivos de configuração.</param>
        /// <param name="caminhoDestino">Caminho de destino para os arquivos de configuração.</param>
        public static void IniciarConfiguracao(Conexao tipoConexao, string nomeAplicacao, string caminhoDestino)
        {
            DefinirCaminhoArquivoConfiguracoes(nomeAplicacao, caminhoDestino);
            CarregarParametrosBancoDados();
            DefinirConexaoAtiva(tipoConexao);
            CarregarConfiguracoesBanco();
        }

        /// <summary>
        /// Define o caminho do arquivo de configurações e os diretórios relacionados.
        /// </summary>
        private static void DefinirCaminhoArquivoConfiguracoes(string nomeAplicacao, string caminhoDestino)
        {
            _diretorioArquivosConfig = Path.Combine(caminhoDestino, nomeAplicacao);
            _caminhoArquivoConfiguracoes = Path.Combine(_diretorioArquivosConfig, "configuracoes.json");
        }

        /// <summary>
        /// Carrega os parâmetros padrão para as conexões de banco de dados e salva em um arquivo de configuração.
        /// </summary>
        private static void CarregarParametrosBancoDados()
        {
            try
            {
                var sqlite = new Parametro
                {
                    ID = 1,
                    Nome = "Sqlite",
                    Valor = Path.Combine(_diretorioArquivosConfig, "dbsqlite.db"),
                };

                var sqlServer = new Parametro
                {
                    ID = 2,
                    Nome = "SqlServer",
                    Valor = string.Empty,
                };

                var mySql = new Parametro
                {
                    ID = 3,
                    Nome = "MySql",
                    Valor = string.Empty,
                };

                var parametros = new Parametros
                {
                    BaseAtiva = sqlite,
                    BaseDados = new List<Parametro> { sqlite, sqlServer, mySql }
                };

                string json = JsonConvert.SerializeObject(parametros, Formatting.Indented);

                // Verifica se o diretório de configuração existe, caso contrário, cria
                Directory.CreateDirectory(Path.GetDirectoryName(_caminhoArquivoConfiguracoes));

                File.WriteAllText(_caminhoArquivoConfiguracoes, json);

                ConfiguracaoAtual = parametros;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar parâmetros de configuração.\n" + ex.Message, ex);
            }
        }

        /// <summary>
        /// Define a base de dados ativa a partir do tipo de conexão selecionado.
        /// </summary>
        /// <param name="conexao">Tipo da conexão que será configurada como ativa.</param>
        private static void DefinirConexaoAtiva(Conexao conexao)
        {
            try
            {
                Parametro baseEscolhida = ConfiguracaoAtual.BaseDados.FirstOrDefault(i => (Conexao)i.ID == conexao);

                if (baseEscolhida != null)
                {
                    ConfiguracaoAtual.BaseAtiva = baseEscolhida;

                    string json = JsonConvert.SerializeObject(ConfiguracaoAtual, Formatting.Indented);
                    File.WriteAllText(_caminhoArquivoConfiguracoes, json);

                    TipoConexaoSelecionada = conexao;
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

        /// <summary>
        /// Carrega as configurações do arquivo JSON de configurações.
        /// </summary>
        private static void CarregarConfiguracoesBanco()
        {
            try
            {
                string json = File.ReadAllText(_caminhoArquivoConfiguracoes);
                ConfiguracaoAtual = JsonConvert.DeserializeObject<Parametros>(json);

                TipoConexaoSelecionada = (Conexao)ConfiguracaoAtual.BaseAtiva.ID;
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception("Arquivo de configurações não encontrado: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar as configurações: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtém a conexão do banco de dados conforme o tipo de conexão selecionado.
        /// </summary>
        /// <returns>Uma conexão de banco de dados baseada no tipo configurado.</returns>
        public static System.Data.IDbConnection ObterConexao()
        {
            switch (TipoConexaoSelecionada)
            {
                case Conexao.SQLite: return CriarConexaoSqlite();
                case Conexao.SQLServer: return CriarConexaoSqlServer();
                case Conexao.MySql: return CriarConexaoMySql();
                default: throw new Exception("Tipo de conexão não suportado.");
            }
        }

        /// <summary>
        /// Cria e retorna uma conexão com o banco de dados SQLite.
        /// </summary>
        private static Microsoft.Data.Sqlite.SqliteConnection CriarConexaoSqlite()
        {
            SQLitePCL.Batteries_V2.Init();

            var sqlite = ConfiguracaoAtual.BaseDados.FirstOrDefault(i => i.ID == 1);

            if (string.IsNullOrEmpty(sqlite.Valor))
                sqlite.Valor = string.Empty;

            if (!File.Exists(sqlite.Valor))
            {
                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={sqlite.Valor}"))
                {
                    connection.Open();
                    connection.Close();
                }
            }

            return new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={sqlite.Valor}");
        }

        /// <summary>
        /// Cria e retorna uma conexão com o banco de dados SQL Server.
        /// </summary>
        private static Microsoft.Data.SqlClient.SqlConnection CriarConexaoSqlServer()
        {
            string connString = ConfiguracaoAtual.BaseDados.FirstOrDefault(i => i.ID == 2).Valor.ToString();
            return new Microsoft.Data.SqlClient.SqlConnection(connString);
        }

        /// <summary>
        /// Cria e retorna uma conexão com o banco de dados MySQL.
        /// </summary>
        private static MySqlConnector.MySqlConnection CriarConexaoMySql()
        {
            string connString = ConfiguracaoAtual.BaseDados.FirstOrDefault(i => i.ID == 3).Valor.ToString();
            return new MySqlConnector.MySqlConnection(connString);
        }
    }
}
