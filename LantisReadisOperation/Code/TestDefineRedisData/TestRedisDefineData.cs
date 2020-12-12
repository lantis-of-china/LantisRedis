using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Redis;

namespace Lantis.ReadisOperation
{
    [Serializable]
    [RedisTableDefineAttribute("LoginDatabase")]
    public class TestRedisDefineData : RedisBase
    {
        public string token;
        public string account;
        public string code;
        public int areaCode;
        public TestDe2 test = new TestDe2();
        public List<string> infoList = new List<string>();
        public List<TestDe2> dbInfoList = new List<TestDe2>();
    }

    [Serializable]
    [RedisTableDefineAttribute("")]
    public class TestDe2
    {
        public string showTest;
    }
}
