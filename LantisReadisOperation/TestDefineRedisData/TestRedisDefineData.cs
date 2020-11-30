using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LantisRedisCore;

namespace LantisReadisOperation
{
    [Serializable]
    [RedisTableDefineAttribute("LoginDatabase")]
    public class TestRedisDefineData
    {
        public string token;
        public string account;
        public string code;
        public int areaCode;
        public TestDe2 test;
        public List<string> infoList;
        public List<TestDe2> dbInfoList;
    }

    [Serializable]
    [RedisTableDefineAttribute("")]
    public class TestDe2
    {
        public string showTest;
    }
}
