using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis;
using Lantis.Network;
using System.Threading;
using Lantis.EntityComponentSystem;

namespace Lantis.RedisExecute
{
    class Program
    {
        static void Main(string[] args)
        {
            LogicTrunkEntity.Instance.AddComponentEntity<BranchEntity>();
            Console.ReadLine();


        }
    }
}
