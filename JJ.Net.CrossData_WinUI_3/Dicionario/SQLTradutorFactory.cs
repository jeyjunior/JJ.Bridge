using JJ.Net.CrossData_WinUI_3.Atributo;
using JJ.Net.CrossData_WinUI_3.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData_WinUI_3.Dicionario
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

        public static string ObterSintaxeChavePrimaria(PropertyInfo propriedade = null)
        {
            // Verificar se há atributo Identity na propriedade
            var identityAttr = propriedade?.GetCustomAttribute<Identity>();
            bool habilitarIdentity = identityAttr?.HabilitarIdentity ?? true;

            switch (TipoBancoDados)
            {
                case TipoBancoDados.SQLite:
                    return habilitarIdentity ? "PRIMARY KEY AUTOINCREMENT" : "PRIMARY KEY";
                case TipoBancoDados.SQLServer:
                    return habilitarIdentity ? "PRIMARY KEY IDENTITY(1,1)" : "PRIMARY KEY";
                case TipoBancoDados.MySQL:
                    return habilitarIdentity ? "PRIMARY KEY AUTO_INCREMENT" : "PRIMARY KEY";
                default:
                    throw new InvalidOperationException("Banco de dados não suportado para sintaxe de chave primária.");
            }
        }

        public static string ObterSintaxeUnique()
        {
            switch (TipoBancoDados)
            {
                case TipoBancoDados.SQLite:
                case TipoBancoDados.SQLServer:
                case TipoBancoDados.MySQL:
                    return "UNIQUE";
                default:
                    throw new InvalidOperationException("Banco de dados não suportado para sintaxe UNIQUE.");
            }
        }

        public static string ObterValorPadrao(PropertyInfo propriedade)
        {
            var defaultValueAttr = propriedade.GetCustomAttribute<DefaultValue>();
            if (defaultValueAttr == null)
                return string.Empty;

            if (defaultValueAttr.UsarFuncaoBanco)
            {
                return $"DEFAULT {defaultValueAttr.Valor}";
            }
            else
            {
                // Para valores literais
                Type propertyType = Nullable.GetUnderlyingType(propriedade.PropertyType) ?? propriedade.PropertyType;

                switch (propertyType.Name.ToLower())
                {
                    case "string":
                        return $"DEFAULT '{defaultValueAttr.Valor}'";
                    case "boolean":
                    case "bool":
                        bool valorBool = bool.TryParse(defaultValueAttr.Valor, out var result) ? result :
                                       (defaultValueAttr.Valor == "1" || defaultValueAttr.Valor.ToLower() == "true");
                        switch (TipoBancoDados)
                        {
                            case TipoBancoDados.SQLite:
                                return $"DEFAULT {(valorBool ? "1" : "0")}";
                            case TipoBancoDados.SQLServer:
                                return $"DEFAULT {(valorBool ? "1" : "0")}";
                            case TipoBancoDados.MySQL:
                                return $"DEFAULT {(valorBool ? "1" : "0")}";
                            default:
                                return $"DEFAULT {(valorBool ? "1" : "0")}";
                        }
                    case "int32":
                    case "int64":
                    case "int16":
                    case "decimal":
                    case "double":
                    case "float":
                        return $"DEFAULT {defaultValueAttr.Valor}";
                    case "datetime":
                        // Se for uma data padrão específica
                        if (DateTime.TryParse(defaultValueAttr.Valor, out DateTime dataPadrao))
                        {
                            return $"DEFAULT '{dataPadrao:yyyy-MM-dd HH:mm:ss}'";
                        }
                        return string.Empty;
                    default:
                        return $"DEFAULT {defaultValueAttr.Valor}";
                }
            }
        }
    }
}
