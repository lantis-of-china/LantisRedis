using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Redis;
using Lantis.Pool;
using Lantis.EntityComponentSystem;

namespace Lantis.ReadisOperation
{
    public class Program
    {
        private static NetBranch netBranch;
        public static NetBranch NetBranchHandle
        {
            get
            {
                return netBranch;
            }
        }

        static void Main(string[] args)
        {
            netBranch = LogicTrunkEntity.Instance.AddComponentEntity<NetBranch>(NetBranch.ParamCreate(
            () =>
            {
                Logger.Error("socket connect finish");
                //var testData = new TestRedisDefineData();
                //testData.token = "xyzufakjfkal";
                //testData.account = "lantis-of-china";
                //testData.code = "001897";
                //testData.areaCode = 1;
                //testData.test = new TestDe2()
                //{
                //    showTest = "nmb"
                //};
                //testData.infoList = new List<string>();
                //testData.infoList.Add("1234567");
                //testData.dbInfoList = new List<TestDe2>()
                //{
                //    new TestDe2() {  showTest = "KKSX" }
                //};
                //MemoryReadisOperation.SetData<TestRedisDefineData>(0, testData, null);
                MemoryReadisOperation.CheckTable();
            },
            () =>
            {
                Logger.Error("socket exception");
            }));



            //var netBytes = RedisSerializable.SerializableToBytes(testData);
            //var serializable = RedisSerializable.BytesToSerializable(netBytes);

            //var redisTableData = LantisPoolSystem.GetPool<RedisTableData>().NewObject();
            //RedisCore.DataToRedisTableData(netBytes, redisTableData);
            //netBytes = RedisCore.RedisTableDataToData(redisTableData);
            //var redisTableDataOut = LantisPoolSystem.GetPool<RedisTableData>().NewObject();
            //RedisCore.DataToRedisTableData(netBytes, redisTableDataOut);
            Console.ReadKey();
        }
    }
}
