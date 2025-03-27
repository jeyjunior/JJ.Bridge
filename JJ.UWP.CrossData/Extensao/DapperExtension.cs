using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using JJ.UWP.CrossData.Atributo;
using JJ.UWP.CrossData.Dicionario;
using JJ.UWP.CrossData.Enumerador;

namespace JJ.UWP.CrossData.Extensao
{
    public static class DapperExtension
    {
        public static bool VerificarTabelaExistente<T>(this IDbConnection connection)
        {
            Type entidade = typeof(T);
            string tabela = entidade.Name;

            string query = "";

            switch (ConfiguracaoBancoDados.TipoConexaoSelecionada)
            {
                case Conexao.SQLite:
                    query = $"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tabela}';";
                    break;

                case Conexao.SQLServer:
                    query = $"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tabela}'";
                    break;

                case Conexao.MySql:
                    query = $"SELECT count(*) FROM information_schema.tables WHERE table_name = '{tabela}'";
                    break;

                default:
                    throw new InvalidOperationException("Banco de dados não suportado para verificação de tabelas.");
            }

            try
            {
                var resultado = connection.ExecuteScalar<int>(query);

                return resultado > 0;
            }
            catch
            {
                return false;
            }
        }

        public static T Obter<T>(this IDbConnection connection, int id)
        {
            Type entidade = typeof(T);
            string tabela = entidade.Name;

            PropertyInfo chavePrimaria = ObterChavePrimaria(entidade);
            string coluna = chavePrimaria.Name;

            string sql = $"SELECT * FROM {tabela} WHERE {coluna} = @Id";

            var ret = connection.QuerySingleOrDefault<T>(sql, new { Id = id });

            if (ret == null)
                throw new Exception("Nenhum resultado encontrado.");

            return ret;
        }

        public static IEnumerable<T> ObterLista<T>(this IDbConnection connection, string condicao = "", object parametros = null)
        {
            Type entidade = typeof(T);
            string tabela = entidade.Name;

            string sql = $"SELECT * FROM {tabela}";

            if (!string.IsNullOrWhiteSpace(condicao))
            {
                sql += $" WHERE {condicao}";
            }

            return parametros == null ? connection.Query<T>(sql) : connection.Query<T>(sql, parametros);
        }

        public static int Adicionar<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null)
        {
            Type entidade = typeof(T);
            string tabela = entidade.Name;

            PropertyInfo chavePrimaria = ObterChavePrimaria(entidade);
            string coluna = chavePrimaria.Name;

            var colunas = new List<string>();
            var parametros = new DynamicParameters();

            foreach (PropertyInfo propriedade in entidade.GetProperties())
            {
                if (propriedade.GetCustomAttribute<ChavePrimaria>() != null)
                    continue;

                if (propriedade.GetCustomAttribute<Editavel>(false) != null)
                    continue;

                if (propriedade.GetCustomAttribute<Obrigatorio>() != null && propriedade.GetValue(entity) == null)
                    throw new InvalidOperationException($"A propriedade {propriedade.Name} é obrigatória e não foi preenchida.");

                var valor = propriedade.GetValue(entity);
                if (valor is DateTime dateTimeValue)
                    valor = SQLTradutorFactory.TratarData(valor);

                colunas.Add($"{propriedade.Name}");
                parametros.Add(propriedade.Name, valor);
            }

            string sql = $"INSERT INTO {tabela} ({string.Join(", ", colunas)}) VALUES ({string.Join(", ", colunas.Select(p => "@" + p))});";
            sql += SQLTradutorFactory.ObterUltimoInsert();

            var result = connection.ExecuteScalar<int>(sql, parametros, transaction);

            if (result == 0)
            {
                throw new InvalidOperationException("Nenhuma linha foi inserida no banco de dados.");
            }

            return result;
        }

        public static int Atualizar<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null)
        {
            Type entidade = typeof(T);
            string tabela = entidade.Name;

            PropertyInfo chavePrimaria = ObterChavePrimaria(entidade);
            string coluna = chavePrimaria.Name;

            var colunas = new List<string>();
            var parametros = new DynamicParameters();

            foreach (PropertyInfo propriedade in entidade.GetProperties())
            {
                if (propriedade.GetCustomAttribute<ChavePrimaria>() != null)
                    continue;

                if (propriedade.GetCustomAttribute<Editavel>(false) != null)
                    continue;

                if (propriedade.GetCustomAttribute<Obrigatorio>() != null && propriedade.GetValue(entity) == null)
                {
                    throw new InvalidOperationException($"A propriedade {propriedade.Name} é obrigatória e não foi preenchida.");
                }

                var valor = propriedade.GetValue(entity);
                if (valor is DateTime dateTimeValue)
                    valor = SQLTradutorFactory.TratarData(valor);

                colunas.Add($"{propriedade.Name} = @{propriedade.Name}");
                parametros.Add(propriedade.Name, valor);
            }

            parametros.Add("Id", chavePrimaria.GetValue(entity));

            string sql = $"UPDATE {tabela} SET {string.Join(", ", colunas)} WHERE {coluna} = @Id";

            var rowsAffected = connection.Execute(sql, parametros, transaction);

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException("Nenhuma linha foi atualizada no banco de dados. Verifique se o registro existe.");
            }

            return rowsAffected;
        }

        public static int Deletar<T>(this IDbConnection connection, object id, IDbTransaction transaction = null)
        {
            Type entidade = typeof(T);
            string tabela = entidade.Name;

            PropertyInfo chavePrimaria = ObterChavePrimaria(entidade);
            string coluna = chavePrimaria.Name;

            string sqlVerificacao = $"SELECT COUNT(1) FROM {tabela} WHERE {coluna} = @Id";
            var existeRegistro = connection.ExecuteScalar<int>(sqlVerificacao, new { Id = id }, transaction) > 0;

            if (!existeRegistro)
                throw new InvalidOperationException("O registro a ser excluído não existe.");

            string sqlDeletar = $"DELETE FROM {tabela} WHERE {coluna} = @Id";

            return connection.Execute(sqlDeletar, new { Id = id }, transaction);
        }

        public static bool CriarTabela<T>(this IDbConnection connection, IDbTransaction transaction = null)
        {
            Type entityType = typeof(T);
            string tableName = entityType.Name;

            StringBuilder createTableSql = new StringBuilder();
            createTableSql.Append($"CREATE TABLE {tableName} (");

            PropertyInfo[] properties = entityType.GetProperties();
            List<string> columns = new List<string>();
            List<string> foreignKeys = new List<string>(); // Lista para armazenar as FK

            foreach (PropertyInfo property in properties)
            {
                // Ignorar propriedades não editáveis
                if (property.GetCustomAttribute<Editavel>()?.HabilitarEdicao == false)
                    continue;

                Type propertyType = property.PropertyType;
                string columnName = property.Name;
                string columnType = SQLTradutorFactory.ObterTipoColuna(property);

                // Verificar se é Chave Primária
                if (property.GetCustomAttribute<ChavePrimaria>() != null)
                {
                    columns.Add($"{columnName} {columnType} {SQLTradutorFactory.ObterSintaxeChavePrimaria()}");
                }
                else
                {
                    // Verificar se a propriedade é obrigatória
                    if (property.GetCustomAttribute<Obrigatorio>() != null)
                    {
                        columns.Add($"{columnName} {columnType} NOT NULL");
                    }
                    else
                    {
                        columns.Add($"{columnName} {columnType}");
                    }
                }

                // Verificar se é uma Foreign Key
                var relacionamento = property.GetCustomAttribute<Relacionamento>();
                if (relacionamento != null)
                {
                    // Adicionar a definição da chave estrangeira com o nome da chave primária referenciada
                    string fk = SQLTradutorFactory.ObterSintaxeForeignKey(columnName, relacionamento.Tabela, relacionamento.ChavePrimaria);
                    foreignKeys.Add(fk);
                }
            }

            // Adicionar as colunas ao SQL
            createTableSql.Append(string.Join(", ", columns));

            // Adicionar as Foreign Keys (se houver)
            if (foreignKeys.Any())
            {
                createTableSql.Append(", ");
                createTableSql.Append(string.Join(", ", foreignKeys));
            }

            createTableSql.Append(");");

            try
            {
                var ret = connection.Execute(createTableSql.ToString(), transaction: transaction);
                return (ret > 0);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao criar a tabela '{tableName}': {createTableSql}", ex);
            }
        }

        public static bool CriarTabelas(this IDbConnection connection, string query, IDbTransaction transaction = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("A query para criação das tabelas não pode ser nula ou vazia.");

            var resultado = connection.Execute(query, transaction: transaction);

            return resultado > 0;
        }

        public static int ExecutarQuery(this IDbConnection connection, string query, object parametros = null, IDbTransaction transaction = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("A query fornecida não pode ser nula ou vazia.");

            return connection.Execute(query, parametros, transaction);
        }

        private static PropertyInfo ObterChavePrimaria(Type entidade)
        {
            var chavePrimaria = entidade.GetProperties().Where(i => i.GetCustomAttribute<ChavePrimaria>() != null).FirstOrDefault();

            if (chavePrimaria == null)
                throw new InvalidOperationException($"A entidade {entidade.Name} não possui uma chave primária definida.");

            return chavePrimaria;
        }
    }
}
