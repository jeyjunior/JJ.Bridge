using JJ.Net.WinUI3.CrossData.DTO;
using JJ.Net.WinUI3.CrossData.Enumerador;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Shapes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace JJ.Net.WinUI3.CrossData
{
    public static class ConfiguracaoBancoDados
    {
        private static StorageFolder _diretorioArquivosConfig;
        private static StorageFile _arquivoConfiguracoes;
        private static string _nomeAplicacao;
        public static Conexao TipoConexaoSelecionada { get; private set; } 
        public static ParametroBaseDados ConfiguracaoAtual { get; private set; }

        public static async Task<IDbConnection> IniciarConfiguracaoAsync(Conexao tipoConexao, string nomeAplicacao = "dbstd")
        {
            TipoConexaoSelecionada = tipoConexao;
            _nomeAplicacao = nomeAplicacao.Replace(" ", "").Replace(".", "");

            await DefinirCaminhoArquivoConfiguracoesAsync();
            await CarregarParametrosBancoDadosAsync();
            await DefinirConexaoAtivaAsync();
            await CarregarConfiguracoesBancoAsync();

            return TipoConexaoSelecionada switch
            {
                Conexao.SQLite => await CriarConexaoSqliteAsync(),
                Conexao.SQLServer => CriarConexaoSqlServer(),
                Conexao.MySql => CriarConexaoMySql(),
                _ => await CriarConexaoSqliteAsync(),
            };
        }
        private static async Task DefinirCaminhoArquivoConfiguracoesAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_nomeAplicacao))
                    _nomeAplicacao = "dbstd";

                var pastaLocal = ApplicationData.Current.LocalFolder;

                _diretorioArquivosConfig = await pastaLocal.CreateFolderAsync(_nomeAplicacao, CreationCollisionOption.OpenIfExists);
                _arquivoConfiguracoes = await _diretorioArquivosConfig.CreateFileAsync("configuracoes.json", CreationCollisionOption.OpenIfExists);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao definir caminhos de configurações.\n" + ex.Message, ex);
            }
        }
        private static async Task CarregarParametrosBancoDadosAsync()
        {
            try
            {
                var storageFolder = await _diretorioArquivosConfig.CreateFolderAsync("dbsqlite", CreationCollisionOption.OpenIfExists);

                var sqlite = new InfoBaseDados
                {
                    ID = 1,
                    Nome = "Sqlite",
                    Valor = storageFolder.Path,
                };

                var sqlServer = new InfoBaseDados
                {
                    ID = 2,
                    Nome = "SqlServer",
                    Valor = string.Empty
                };

                var mySql = new InfoBaseDados
                {
                    ID = 3,
                    Nome = "MySql",
                    Valor = string.Empty
                };

                var parametros = new ParametroBaseDados
                {
                    BaseAtiva = sqlite,
                    BaseDados = new List<InfoBaseDados> { sqlite, sqlServer, mySql }
                };

                string json = JsonConvert.SerializeObject(parametros, Formatting.Indented);
                await FileIO.WriteTextAsync(_arquivoConfiguracoes, json);

                ConfiguracaoAtual = parametros;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar parâmetros de configuração.\n" + ex.Message, ex);
            }
        }
        private static async Task DefinirConexaoAtivaAsync()
        {
            try
            {
                InfoBaseDados baseDadosSelecionada = ConfiguracaoAtual.BaseDados.FirstOrDefault(i => (Conexao)i.ID == TipoConexaoSelecionada);

                if (baseDadosSelecionada != null)
                {
                    ConfiguracaoAtual.BaseAtiva = baseDadosSelecionada;

                    string json = JsonConvert.SerializeObject(ConfiguracaoAtual, Formatting.Indented);

                    await FileIO.WriteTextAsync(_arquivoConfiguracoes, json); 
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
        private static async Task CarregarConfiguracoesBancoAsync()
        {
            try
            {
                string json = await FileIO.ReadTextAsync(_arquivoConfiguracoes);
                ConfiguracaoAtual = JsonConvert.DeserializeObject<ParametroBaseDados>(json);

                TipoConexaoSelecionada = (Conexao)ConfiguracaoAtual.BaseAtiva.ID;
            }
            catch (System.IO.FileNotFoundException ex)
            {
                throw new Exception("Arquivo de configurações não encontrado: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar as configurações: " + ex.Message, ex);
            }
        }

        #region Criar Conexoes Banco de Dados
        private static async Task<SqliteConnection> CriarConexaoSqliteAsync()
        {
            SQLitePCL.Batteries_V2.Init();

            var sqlite = ConfiguracaoAtual.BaseDados.FirstOrDefault(i => i.ID == 1);

            if (string.IsNullOrEmpty(sqlite.Valor))
                sqlite.Valor = string.Empty;

            var file = await StorageFile.GetFileFromPathAsync(sqlite.Valor);

            if (file == null)
            {
                using var connection = new SqliteConnection($"Data Source={sqlite.Valor}");
                await connection.OpenAsync();
                connection.Close();
            }

            return new SqliteConnection($"Data Source={sqlite.Valor}");
        }
        private static Microsoft.Data.SqlClient.SqlConnection CriarConexaoSqlServer()
        {
            string connString = ConfiguracaoAtual.BaseDados.FirstOrDefault(i => i.ID == 2).Valor.ToString();
            return new Microsoft.Data.SqlClient.SqlConnection(connString);
        }
        private static MySqlConnector.MySqlConnection CriarConexaoMySql()
        {
            string connString = ConfiguracaoAtual.BaseDados.FirstOrDefault(i => i.ID == 3).Valor.ToString();
            return new MySqlConnector.MySqlConnection(connString);
        }
        #endregion
    }
}
