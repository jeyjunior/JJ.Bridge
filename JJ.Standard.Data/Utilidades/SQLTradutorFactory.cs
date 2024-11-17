using JJ.Standard.Core.Atributos;
using JJ.Standard.Data.Enum;
using System;
using System.Linq;
using System.Reflection;

namespace JJ.Standard.Data.Utilidades
{
    public static class SQLTradutorFactory
    {
        public static string ObterUltimoInsert()
        {
            string query = "";

            switch (Config.Conexao)
            {
                case eConexao.SQLite: query = "SELECT last_insert_rowid();"; break;
                case eConexao.SQLServer: query = "SELECT SCOPE_IDENTITY();"; break;
                case eConexao.MySql: query = "SELECT LAST_INSERT_ID();"; break;
            }

            return query;
        }

        public static object TratarData(object value)
        {
            DateTime dateTimeValue = (DateTime)value;

            switch (Config.Conexao)
            {
                case eConexao.SQLite:
                case eConexao.MySql:
                case eConexao.SQLServer:
                    // Tratar a data conforme necessário para cada DB
                    break;
            }

            return dateTimeValue;
        }

        public static string ObterSintaxeChavePrimaria()
        {
            switch (Config.Conexao)
            {
                case eConexao.SQLite:
                    return "PRIMARY KEY AUTOINCREMENT"; 
                case eConexao.SQLServer:
                    return "PRIMARY KEY IDENTITY"; 
                case eConexao.MySql:
                    return "PRIMARY KEY AUTO_INCREMENT";
                default:
                    throw new InvalidOperationException("Banco de dados não suportado para sintaxe de chave primária.");
            }
        }

        public static string ObterSintaxeForeignKey(string columnName, string tabelaReferenciada, string chavePrimaria)
        {
            switch (Config.Conexao)
            {
                case eConexao.SQLite:
                    return $"FOREIGN KEY ({columnName}) REFERENCES {tabelaReferenciada}({chavePrimaria})";
                case eConexao.SQLServer:
                    return $"FOREIGN KEY ({columnName}) REFERENCES {tabelaReferenciada}({chavePrimaria})";
                case eConexao.MySql:
                    return $"FOREIGN KEY ({columnName}) REFERENCES {tabelaReferenciada}({chavePrimaria})";
                default:
                    throw new InvalidOperationException("Banco de dados não suportado para criação de chaves estrangeiras.");
            }
        }

        public static string ObterTipoColuna(PropertyInfo propriedade)
        {
            Type propertyType = Nullable.GetUnderlyingType(propriedade.PropertyType) ?? propriedade.PropertyType;

            // Verificar o tipo de dado genérico
            string tipoColuna = propertyType.Name.ToLower() switch
            {
                "string" => "TEXT",            
                "int32" => "INTEGER",          
                "int64" => "BIGINT",           
                "decimal" => "DECIMAL",        
                "double" => "DOUBLE",          
                "float" => "FLOAT",            
                "datetime" => "DATETIME",      
                "boolean" => "BOOLEAN",        
                "int16" => "SMALLINT",         
                _ => throw new ArgumentException($"Tipo de propriedade não suportado: {propriedade.PropertyType.Name}")
            };

            switch (Config.Conexao)
            {
                case eConexao.SQLite:
                    if (propertyType == typeof(bool))
                    {
                        tipoColuna = "INTEGER"; 
                    }
                    else if (propertyType == typeof(decimal))
                    {
                        tipoColuna = "REAL";
                    }
                    break;

                case eConexao.SQLServer:
                    if (propertyType == typeof(bool))
                    {
                        tipoColuna = "BIT"; 
                    }
                    break;

                case eConexao.MySql:
                    if (propertyType == typeof(bool))
                    {
                        tipoColuna = "TINYINT(1)";
                    }
                    break;

                default:
                    throw new InvalidOperationException("Banco de dados não suportado para tipos de dados.");
            }

            if (Config.Conexao == eConexao.MySql || Config.Conexao == eConexao.SQLServer)
            {
                if (propertyType == typeof(string))
                {
                    var tamanhoAttr = propriedade.GetCustomAttribute<TamanhoString>();
                    tipoColuna = tamanhoAttr != null ? $"VARCHAR({tamanhoAttr.Tamanho})" : "TEXT";
                }
                else if (propertyType == typeof(decimal))
                {
                    var tamanhoAttr = propriedade.GetCustomAttribute<TamanhoDecimal>();
                    tipoColuna = tamanhoAttr != null
                        ? $"DECIMAL({tamanhoAttr.Tamanho},{tamanhoAttr.Decimais})"
                        : "DECIMAL(18,2)";
                }
            }

            return tipoColuna;
        }

    }
}
