using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis;
using Lantis.Network;

namespace LantisRedisExecute
{
    class Program
    {
        static void Main(string[] args)
        {
            LogicTrunkEntity.Instance.AddComponentEntity<NetMessageDriverComponents>();
            LogicTrunkEntity.Instance.AddComponentEntity<NetServerComponents>("127.0.0.1",1897,null,null);
        }
    }
}
