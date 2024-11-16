using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using JJ.Standard.Core.Atributos;
using JJ.Standard.Data.Utilidades;

namespace JJ.Standard.Data.Extensoes
{
    public static class DapperExtension
    {
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
                {
                    valor = dateTimeValue.Date.ToString("yyyy-MM-dd");
                }

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
                {
                    valor = SQLTradutorFactory.TratarData(valor);
                }

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
