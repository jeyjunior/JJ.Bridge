using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.Standard.Data.Enum;

namespace JJ.Standard.Data.Utilidades
{
    public static class SQLTradutorFactory
    {
        public static string ObterUltimoInsert()
        {
            string query = "";

            switch (Config.Conexao)
            {
                case eConexao.SQLite:    query = "SELECT last_insert_rowid();";  break;
                case eConexao.SQLServer: query = "SELECT SCOPE_IDENTITY();";     break;
                case eConexao.MySql:     query = "SELECT LAST_INSERT_ID();";     break;
            }

            return query;
        }

        public static object TratarData(object value)
        {
            DateTime dateTimeValue = (DateTime)value;

            switch (Config.Conexao)
            {
                case eConexao.SQLite:       break;
                case eConexao.MySql:        break;
                case eConexao.SQLServer:    break;
            }

            return dateTimeValue;
        }
    }
}
