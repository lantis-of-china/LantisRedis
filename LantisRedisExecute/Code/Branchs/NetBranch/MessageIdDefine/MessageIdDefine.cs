using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lantis.RedisExecute
{
    public class MessageIdDefine
    {
        public const int CheckDatabase      = 1000000;
        public const int CheckDatabaseBack  = 1000001;
        public const int SetData            = 1000002;
        public const int SetDataBack        = 1000003;
        public const int GetData            = 1000004;
        public const int GetDataBack        = 1000005;
        public const int ExecuteCommand     = 1000006;
        public const int ExecuteCommandBack = 1000007;
    }
}
