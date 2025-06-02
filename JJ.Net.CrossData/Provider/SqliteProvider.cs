using System;
using System.Data;
using System.IO;
using JJ.Net.CrossData.DTO;
using JJ.Net.CrossData.Interfaces;
using Microsoft.Data.Sqlite;

namespace JJ.Net.CrossData.Provider
{
    internal class SqliteProvider : IBancoDadosProvider
    {
        private const string NOME_ARQUIVO_PADRAO = "{0}_dbsqlite.db";

        public IDbConnection CriarConexao(ParametrosConfiguracao parametros)
        {
            if (string.IsNullOrWhiteSpace(parametros.NomeAplicacao))
                throw new ArgumentException("Nome da aplicação é obrigatório para SQLite");

            string caminhoBanco = ObterCaminhoPadraoBanco(parametros.NomeAplicacao);

            if (File.Exists(caminhoBanco) && !EhArquivoSqliteValido(caminhoBanco))
                throw new InvalidOperationException($"O arquivo {Path.GetFileName(caminhoBanco)} existe mas não é um banco SQLite válido");

            CriarDiretorioSeNaoExistir(caminhoBanco);

            string connectionString = $"Data Source={caminhoBanco};";
            var conexao = new SqliteConnection(connectionString);

            TestarConexao(conexao);

            return conexao;
        }
        private string ObterCaminhoPadraoBanco(string nomeAplicacao)
        {
            string pastaBase = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string pastaApp = Path.Combine(pastaBase, nomeAplicacao);
            string nomeArquivo = string.Format(NOME_ARQUIVO_PADRAO, SanitizeFileName(nomeAplicacao));

            return Path.Combine(pastaApp, nomeArquivo);
        }

        private bool EhArquivoSqliteValido(string caminho)
        {
            try
            {
                byte[] header = new byte[16];
                using (var stream = File.OpenRead(caminho))
                {
                    stream.Read(header, 0, 16);
                }
                return System.Text.Encoding.UTF8.GetString(header).StartsWith("SQLite format 3");
            }
            catch
            {
                return false;
            }
        }

        private void CriarDiretorioSeNaoExistir(string caminhoBanco)
        {
            string diretorio = Path.GetDirectoryName(caminhoBanco);

            if (!Directory.Exists(diretorio))
            {
                try
                {
                    Directory.CreateDirectory(diretorio);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Não foi possível criar o diretório para o banco de dados em {diretorio}", ex);
                }
            }
        }

        private void TestarConexao(SqliteConnection conexao)
        {
            try
            {
                conexao.Open();
                using (var cmd = conexao.CreateCommand())
                {
                    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' LIMIT 1;";
                    cmd.ExecuteScalar();
                }
            }
            finally
            {
                if (conexao.State == ConnectionState.Open)
                    conexao.Close();
            }
        }

        private string SanitizeFileName(string nome)
        {
            var invalidos = Path.GetInvalidFileNameChars();
            foreach (char c in invalidos)
                nome = nome.Replace(c, '_');
            return nome;
        }
    }
}