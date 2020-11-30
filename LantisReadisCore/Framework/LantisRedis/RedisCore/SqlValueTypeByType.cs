using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LantisRedisCore
{
    public class SqlValueTypeByType
    {
        public static Dictionary<Type, string> sqlTypeDictionary = new Dictionary<Type, string>
        {
            { typeof(Boolean),"BIT"},
            { typeof(Byte),"TINYINT"},
            { typeof(SByte),"TINYINT"},
            { typeof(UInt16),"SMALLINT"},
            { typeof(Int16),"SMALLINT"},
            { typeof(UInt32),"INT"},
            { typeof(Int32),"INT"},
            { typeof(UInt64),"BIGINT"},
            { typeof(Int64),"BIGINT"},
            { typeof(Single),"FLOAT"},
            { typeof(Double),"FLOAT"},
            { typeof(String),"Text"},
        };

        public static string GetSqlType(Type type)
        {
            if (sqlTypeDictionary.ContainsKey(type))
            {
                return sqlTypeDictionary[type];
            }

            return "Text";
        }
    }

}
