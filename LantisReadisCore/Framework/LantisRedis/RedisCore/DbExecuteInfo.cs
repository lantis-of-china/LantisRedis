using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LantisRedisCore
{
    /// <summary>
    /// describe excute submit paramar
    /// </summary>
    public class DbExecuteInfo
    {
        public Type type;
        public int executeType;
        public RedisState data;
        public List<string> whereField = new List<string>();
        public List<string> whereOperator = new List<string>();
        public List<string> whereValue = new List<string>();
    }
}
