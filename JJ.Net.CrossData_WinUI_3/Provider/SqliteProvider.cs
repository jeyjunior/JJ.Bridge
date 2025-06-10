using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using JJ.Net.CrossData_WinUI_3.DTO;
using JJ.Net.CrossData_WinUI_3.Interfaces;
using Microsoft.Data.Sqlite;
using Windows.Storage;

namespace JJ.Net.CrossData.Provider
{
    internal class SqliteProvider : IBancoDadosProvider
    {
        private const string NOME_ARQUIVO_PADRAO = "{0}_dbsqlite.db";

        public async Task<IDbConnection> CriarConexaoAsync(ParametrosConfiguracao parametros)
        {
            if (string.IsNullOrWhiteSpace(parametros.NomeAplicacao))
                throw new ArgumentException("Nome da aplicação é obrigatório para SQLite");

            string caminhoBanco = await ObterCaminhoPadraoBancoAsync(parametros.NomeAplicacao);

            if (await FileExistsAsync(caminhoBanco) && !await EhArquivoSqliteValidoAsync(caminhoBanco))
                throw new InvalidOperationException($"O arquivo {Path.GetFileName(caminhoBanco)} existe mas não é um banco SQLite válido");

            await CriarDiretorioSeNaoExistirAsync(caminhoBanco);

            string connectionString = $"Data Source={caminhoBanco};";
            var conexao = new SqliteConnection(connectionString);

            return conexao;
        }
        public IDbConnection CriarConexao(ParametrosConfiguracao parametros)
        {
            if (string.IsNullOrWhiteSpace(parametros.NomeAplicacao))
                throw new ArgumentException("Nome da aplicação é obrigatório para SQLite");

            string caminhoBanco = ObterCaminhoPadraoBanco(parametros.NomeAplicacao);

            if (FileExists(caminhoBanco) && !EhArquivoSqliteValido(caminhoBanco))
                throw new InvalidOperationException($"O arquivo {Path.GetFileName(caminhoBanco)} existe mas não é um banco SQLite válido");

            CriarDiretorioSeNaoExistir(caminhoBanco);

            string connectionString = $"Data Source={caminhoBanco};";
            var conexao = new SqliteConnection(connectionString);

            return conexao;
        }

        private async Task<string> ObterCaminhoPadraoBancoAsync(string nomeAplicacao)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string nomeArquivo = string.Format(NOME_ARQUIVO_PADRAO, SanitizeFileName(nomeAplicacao));

            return Path.Combine(localFolder.Path, nomeArquivo);
        }
        private string ObterCaminhoPadraoBanco(string nomeAplicacao)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string nomeArquivo = string.Format(NOME_ARQUIVO_PADRAO, SanitizeFileName(nomeAplicacao));

            return Path.Combine(localFolder.Path, nomeArquivo);
        }

        private async Task<bool> EhArquivoSqliteValidoAsync(string caminho)
        {
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(caminho);
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    byte[] header = new byte[16];
                    await stream.ReadAsync(header, 0, 16);
                    return System.Text.Encoding.UTF8.GetString(header).StartsWith("SQLite format 3");
                }
            }
            catch
            {
                return false;
            }
        }
        private bool EhArquivoSqliteValido(string caminho)
        {
            try
            {
                var file = StorageFile.GetFileFromPathAsync(caminho).GetAwaiter().GetResult();
                using (var stream = file.OpenStreamForReadAsync().GetAwaiter().GetResult())
                {
                    byte[] header = new byte[16];
                    stream.Read(header, 0, 16);
                    return System.Text.Encoding.UTF8.GetString(header).StartsWith("SQLite format 3");
                }
            }
            catch
            {
                return false;
            }
        }

        private async Task CriarDiretorioSeNaoExistirAsync(string caminhoBanco)
        {
            string diretorio = Path.GetDirectoryName(caminhoBanco);

            try
            {
                // Em WinUI3, o LocalFolder já está disponível e não precisamos criar manualmente
                // Mas verificamos se podemos acessar
                var folder = await StorageFolder.GetFolderFromPathAsync(diretorio);
            }
            catch (FileNotFoundException)
            {
                // Se a pasta não existir, criamos
                await ApplicationData.Current.LocalFolder.CreateFolderAsync(
                    Path.GetDirectoryName(caminhoBanco),
                    CreationCollisionOption.ReplaceExisting);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Não foi possível criar/aceder o diretório para o banco de dados em {diretorio}", ex);
            }
        }
        private void CriarDiretorioSeNaoExistir(string caminhoBanco)
        {
            string diretorio = Path.GetDirectoryName(caminhoBanco);

            try
            {
                var folder = StorageFolder.GetFolderFromPathAsync(diretorio).GetAwaiter().GetResult();
            }
            catch (FileNotFoundException)
            {
                ApplicationData.Current.LocalFolder.CreateFolderAsync(
                    Path.GetDirectoryName(caminhoBanco),
                    CreationCollisionOption.ReplaceExisting).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Não foi possível criar/aceder o diretório para o banco de dados em {diretorio}", ex);
            }
        }
        private string SanitizeFileName(string nome)
        {
            var invalidos = Path.GetInvalidFileNameChars();
            foreach (char c in invalidos)
                nome = nome.Replace(c, '_');
            return nome;
        }
        private async Task<bool> FileExistsAsync(string path)
        {
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(path);
                return file != null;
            }
            catch
            {
                return false;
            }
        }
        private bool FileExists(string path)
        {
            try
            {
                var file = StorageFile.GetFileFromPathAsync(path).GetAwaiter().GetResult();
                return file != null;
            }
            catch
            {
                return false;
            }
        }
    }
}