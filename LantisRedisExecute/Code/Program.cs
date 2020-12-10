using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis;
using Lantis.Network;
using System.Threading;
using Lantis.EntityComponentSystem;
using Lantis.DatabaseLinks;

namespace Lantis.RedisExecute
{
    class Program
    {
        static void Main(string[] args)
        {
            LogicTrunkEntity.Instance.AddComponentEntity<DatabaseBranch>(DatabaseBranch.ParamCreate("link string"));
            LogicTrunkEntity.Instance.AddComponentEntity<NetBranch>(NetBranch.ParamCreate(
            ()=> 
            {
                Logger.Log("server run sucess");
                LogicTrunkEntity.Instance.AddComponentEntity<RedisExecuteBranch>();
            },
            ()=> 
            {
                Logger.Error("server run exception");
            }));
            Console.ReadLine();
        }
    }
}
