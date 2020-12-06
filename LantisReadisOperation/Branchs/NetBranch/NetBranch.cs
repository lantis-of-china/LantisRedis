using Lantis.EntityComponentSystem;
using Lantis.Network;
using Lantis.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lantis.ReadisOperation
{
    public class NetBranch : ComponentEntity
    {
        private string ip = "127.0.0.1";
        private int port = 9980;
        private NetMessageDriverComponents netMessageDriverComponent;
        private NetClientComponents netClientComponent;
        public NetClientComponents NetClientComponentHandle
        {
            get
            {
                return netClientComponent;
            }
        }
        private Action onSocketConnect;
        private Action onSocketException;

        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();
        }

        public static object[] ParamCreate(Action socketConnect, Action socketException)
        {
            return new object[] { socketConnect, socketException };
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);

            SafeRun(delegate
            {
                onSocketConnect = paramsData[0] as Action;
                onSocketException = paramsData[1] as Action;
                netMessageDriverComponent = AddComponentEntity<NetMessageDriverComponents>(NetMessageDriverComponents.ParamCreate(Assembly.GetExecutingAssembly(), new string[] { "Lantis.ReadisOperation.NetProcess" }));
                netClientComponent = AddComponentEntity<NetClientComponents>(NetClientComponents.ParamCreate(ip, port, null, null, OnSocketConnectSucess,OnSocketException));
            });
        }

        private void OnSocketConnectSucess()
        {
            SafeRun(delegate 
            {
                if (onSocketConnect != null)
                {
                    onSocketConnect();
                }
            });
        }
        
        private void OnSocketException()
        {
            SafeRun(delegate
            {
                if (onSocketException != null)
                {
                    onSocketException();
                }
            });
        }
    }
}
