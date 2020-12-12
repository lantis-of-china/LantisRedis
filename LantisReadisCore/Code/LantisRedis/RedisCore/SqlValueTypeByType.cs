using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lantis.Redis
{
    public class SqlValueTypeByType
    {
        public static Dictionary<string, string> sqlTypeDictionary = new Dictionary<string, string>
        {
            { typeof(Boolean).Name,"BIT"},
            { typeof(Byte).Name,"TINYINT"},
            { typeof(SByte).Name,"TINYINT"},
            { typeof(UInt16).Name,"SMALLINT"},
            { typeof(Int16).Name,"SMALLINT"},
            { typeof(UInt32).Name,"INT"},
            { typeof(Int32).Name,"INT"},
            { typeof(UInt64).Name,"BIGINT"},
            { typeof(Int64).Name,"BIGINT"},
            { typeof(Single).Name,"FLOAT"},
            { typeof(Double).Name,"FLOAT"},
            { typeof(String).Name,"nvarchar(max)"},
        };

        public static string GetSqlType(Type type)
        {
            return GetSqlType(type.Name);
        }

        public static string GetSqlType(string type)
        {
            if (sqlTypeDictionary.ContainsKey(type))
            {
                return sqlTypeDictionary[type];
            }

            return "Text";
        }
    }

}
