using JJ.Net.CrossData.Atributo;
using JJ.Net.CrossData.CrossData;
using JJ.Net.CrossData.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData.Dicionario
{
    public static class SQLTradutorFactory
    {
        public static TipoBancoDados TipoBancoDados { get; set; }

        public static string ObterUltimoInsert()
        {
            string query = "";

            switch (TipoBancoDados)
            {
                case TipoBancoDados.SQLite: query = "SELECT last_insert_rowid();"; break;
                case TipoBancoDados.SQLServer: query = "SELECT SCOPE_IDENTITY();"; break;
                case TipoBancoDados.MySQL: query = "SELECT LAST_INSERT_ID();"; break;
            }

            return query;
        }

        public static object TratarData(object value)
        {
            DateTime dateTimeValue = (DateTime)value;

            switch (TipoBancoDados)
            {
                case TipoBancoDados.SQLite:
                case TipoBancoDados.MySQL:
                case TipoBancoDados.SQLServer:
                    // Tratar a data conforme necessário para cada DB
                    break;
            }

            return dateTimeValue;
        }

        public static string ObterSintaxeChavePrimaria()
        {
            switch (TipoBancoDados)
            {
                case TipoBancoDados.SQLite:
                    return "PRIMARY KEY AUTOINCREMENT";
                case TipoBancoDados.SQLServer:
                    return "PRIMARY KEY IDENTITY";
                case TipoBancoDados.MySQL:
                    return "PRIMARY KEY AUTO_INCREMENT";
                default:
                    throw new InvalidOperationException("Banco de dados não suportado para sintaxe de chave primária.");
            }
        }

        public static string ObterSintaxeForeignKey(string columnName, string tabelaReferenciada, string chavePrimaria)
        {
            switch (TipoBancoDados)
            {
                case TipoBancoDados.SQLite:
                    return $"FOREIGN KEY ({columnName}) REFERENCES {tabelaReferenciada}({chavePrimaria})";
                case TipoBancoDados.SQLServer:
                    return $"FOREIGN KEY ({columnName}) REFERENCES {tabelaReferenciada}({chavePrimaria})";
                case TipoBancoDados.MySQL:
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

            switch (TipoBancoDados)
            {
                case TipoBancoDados.SQLite:
                    if (propertyType == typeof(bool))
                    {
                        tipoColuna = "INTEGER";
                    }
                    else if (propertyType == typeof(decimal))
                    {
                        tipoColuna = "REAL";
                    }
                    break;

                case TipoBancoDados.SQLServer:
                    if (propertyType == typeof(bool))
                    {
                        tipoColuna = "BIT";
                    }
                    break;

                case TipoBancoDados.MySQL:
                    if (propertyType == typeof(bool))
                    {
                        tipoColuna = "TINYINT(1)";
                    }
                    break;

                default:
                    throw new InvalidOperationException("Banco de dados não suportado para tipos de dados.");
            }

            if (TipoBancoDados == TipoBancoDados.MySQL || TipoBancoDados == TipoBancoDados.SQLServer)
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
