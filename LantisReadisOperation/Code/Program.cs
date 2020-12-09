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
                MemoryReadisOperation.CheckTable();
            },
            () =>
            {
                Logger.Error("socket exception");
            }));

            Console.ReadKey();
        }
    }
}
