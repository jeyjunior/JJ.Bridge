using Dapper;
using JJ.Net.CrossData.Atributo;
using JJ.Net.CrossData.Dicionario;
using JJ.Net.CrossData.DTO;
using JJ.Net.CrossData.Enumerador;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData.Extensao
{
    public static class DapperExtension
    {
        public static TipoBancoDados TipoBancoDados { get; set; }

        public static bool VerificarTabelaExistente<T>(this IDbConnection connection)
        {
            Type entidade = typeof(T);
            string tabela = entidade.Name;

            string query = "";

            switch (TipoBancoDados)
            {
                case TipoBancoDados.SQLite:
                    query = $"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tabela}';";
                    break;

                case TipoBancoDados.SQLServer:
                    query = $"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tabela}'";
                    break;

                case TipoBancoDados.MySQL:
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

        public static IEnumerable<EntidadeValidacao> VerificarEntidadeExiste(this IDbConnection connection, IEnumerable<Type> entidades)
        {
            var resultado = new List<EntidadeValidacao>();

            if (entidades.Count() <= 0)
                return resultado;

            if (connection == null)
                return resultado;

            foreach (var item in entidades)
            {
                var entidade = new EntidadeValidacao
                {
                    TipoEntidade = item,
                    Existe = false,
                };

                try
                {
                    string query = TipoBancoDados switch
                    {
                        TipoBancoDados.SQLite => $"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{entidade.Nome}';",
                        TipoBancoDados.SQLServer => $"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{entidade.Nome}'",
                        TipoBancoDados.MySQL => $"SELECT count(*) FROM information_schema.tables WHERE table_name = '{entidade.Nome}'",
                        _ => throw new InvalidOperationException("Banco de dados não suportado")
                    };

                    entidade.Existe = connection.ExecuteScalar<int>(query) > 0;
                }
                catch
                {
                    entidade.Existe = false;
                }

                resultado.Add(entidade);
            }

            return resultado;
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

        public static int DeletarTudo<T>(this IDbConnection connection, IDbTransaction transaction = null)
        {
            string tabela = typeof(T).Name;
            string sqlDeletarTudo = $"DELETE FROM {tabela}";
            return connection.Execute(sqlDeletarTudo, transaction: transaction);
        }

        public static bool CriarTabela(this IDbConnection connection, Type entityType, IDbTransaction transaction = null)
        {
            var atributoEntidade = entityType.GetCustomAttribute<EntidadeAttribute>();
            string tableName = atributoEntidade?.NomeTabela ?? entityType.Name;

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

                bool ehObrigatorio = true;

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
                        ehObrigatorio = false;
                    }
                }

                // Verificar se é uma Foreign Key
                var relacionamento = property.GetCustomAttribute<Relacionamento>();
                if (relacionamento != null)
                {
                    // Adicionar a definição da chave estrangeira com o nome da chave primária referenciada
                    string fk = SQLTradutorFactory.ObterSintaxeForeignKey(columnName, relacionamento.Tabela, relacionamento.ChavePrimaria);

                    if (!ehObrigatorio)
                        fk += " ON DELETE SET NULL";

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
