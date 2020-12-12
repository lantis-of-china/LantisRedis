using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Redis;
using Lantis.Pool;
using Lantis.EntityComponentSystem;
using System.Threading;

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
                MemoryReadisOperation.CheckTable();
                Thread.Sleep(1000);
                var testData = new TestRedisDataSleep3();
                testData.id = "12";
                MemoryReadisOperation.SetData(testData.id,testData, null);
            },
            () =>
            {
                Logger.Error("socket exception");
            }));

            Console.ReadKey();
        }
    }
}
