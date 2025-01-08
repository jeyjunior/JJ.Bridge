using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JJ.Standard.Core.Atributos;
using JJ.Standard.Core.Enumerador;

namespace JJ.Standard.Data.Utilidades
{
    public static class SQLTradutorFactory
    {
        public static string ObterUltimoInsert()
        {
            string query = "";

            switch (Config.ConexaoSelecionada)
            {
                case Conexao.SQLite: query = "SELECT last_insert_rowid();"; break;
                case Conexao.SQLServer: query = "SELECT SCOPE_IDENTITY();"; break;
                case Conexao.MySql: query = "SELECT LAST_INSERT_ID();"; break;
            }

            return query;
        }

        public static object TratarData(object value)
        {
            DateTime dateTimeValue = (DateTime)value;

            switch (Config.ConexaoSelecionada)
            {
                case Conexao.SQLite:
                case Conexao.MySql:
                case Conexao.SQLServer:
                    // Tratar a data conforme necessário para cada DB
                    break;
            }

            return dateTimeValue;
        }

        public static string ObterSintaxeChavePrimaria()
        {
            switch (Config.ConexaoSelecionada)
            {
                case Conexao.SQLite:
                    return "PRIMARY KEY AUTOINCREMENT";
                case Conexao.SQLServer:
                    return "PRIMARY KEY IDENTITY";
                case Conexao.MySql:
                    return "PRIMARY KEY AUTO_INCREMENT";
                default:
                    throw new InvalidOperationException("Banco de dados não suportado para sintaxe de chave primária.");
            }
        }

        public static string ObterSintaxeForeignKey(string columnName, string tabelaReferenciada, string chavePrimaria)
        {
            switch (Config.ConexaoSelecionada)
            {
                case Conexao.SQLite:
                    return $"FOREIGN KEY ({columnName}) REFERENCES {tabelaReferenciada}({chavePrimaria})";
                case Conexao.SQLServer:
                    return $"FOREIGN KEY ({columnName}) REFERENCES {tabelaReferenciada}({chavePrimaria})";
                case Conexao.MySql:
                    return $"FOREIGN KEY ({columnName}) REFERENCES {tabelaReferenciada}({chavePrimaria})";
                default:
                    throw new InvalidOperationException("Banco de dados não suportado para criação de chaves estrangeiras.");
            }
        }

        public static string ObterTipoColuna(PropertyInfo propriedade)
        {
            Type propertyType = Nullable.GetUnderlyingType(propriedade.PropertyType) ?? propriedade.PropertyType;

            string tipoColuna;

            switch (propertyType.Name.ToLower())
            {
                case "string":
                    tipoColuna = "TEXT";
                    break;
                case "int32":
                    tipoColuna = "INTEGER";
                    break;
                case "int64":
                    tipoColuna = "BIGINT";
                    break;
                case "decimal":
                    tipoColuna = "DECIMAL";
                    break;
                case "double":
                    tipoColuna = "DOUBLE";
                    break;
                case "float":
                    tipoColuna = "FLOAT";
                    break;
                case "datetime":
                    tipoColuna = "DATETIME";
                    break;
                case "boolean":
                    tipoColuna = "BOOLEAN";
                    break;
                case "int16":
                    tipoColuna = "SMALLINT";
                    break;
                default:
                    throw new ArgumentException($"Tipo de propriedade não suportado: {propriedade.PropertyType.Name}");
            }

            switch (Config.ConexaoSelecionada)
            {
                case Conexao.SQLite:
                    if (propertyType == typeof(bool))
                    {
                        tipoColuna = "INTEGER";
                    }
                    else if (propertyType == typeof(decimal))
                    {
                        tipoColuna = "REAL";
                    }
                    break;

                case Conexao.SQLServer:
                    if (propertyType == typeof(bool))
                    {
                        tipoColuna = "BIT";
                    }
                    break;

                case Conexao.MySql:
                    if (propertyType == typeof(bool))
                    {
                        tipoColuna = "TINYINT(1)";
                    }
                    break;

                default:
                    throw new InvalidOperationException("Banco de dados não suportado para tipos de dados.");
            }

            if (Config.ConexaoSelecionada == Conexao.MySql || Config.ConexaoSelecionada == Conexao.SQLServer)
            {
                if (propertyType == typeof(string))
                {
                    var tamanhoAttr = propriedade.GetCustomAttributes(typeof(TamanhoString), false).FirstOrDefault() as TamanhoString;
                    tipoColuna = tamanhoAttr != null ? $"VARCHAR({tamanhoAttr.Tamanho})" : "TEXT";
                }
                else if (propertyType == typeof(decimal))
                {
                    var tamanhoAttr = propriedade.GetCustomAttributes(typeof(TamanhoDecimal), false).FirstOrDefault() as TamanhoDecimal;
                    tipoColuna = tamanhoAttr != null
                        ? $"DECIMAL({tamanhoAttr.Tamanho},{tamanhoAttr.Decimais})"
                        : "DECIMAL(18,2)";
                }
            }

            return tipoColuna;
        }

    }
}
