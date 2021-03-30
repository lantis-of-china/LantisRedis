using Lantis.EntityComponentSystem;
using Lantis.Network;
using Lantis.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lantis.Network
{
    public class NetClientBranch : ComponentEntity
    {
        private string ip = "";
        private int port = 0;
        private string[] processNameSpace;

        private NetMessageDriverComponents netMessageDriverComponent;
        private NetMessageDriverComponents NetMessageDriverComponent
        {
            get 
            {
                return netMessageDriverComponent;
            }
        }
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

        public static object[] ParamCreate(string ip,int port,string[] processNamespace,Action socketConnect, Action socketException, Assembly processAssembly)
        {
            return new object[] { ip, port, processNamespace, socketConnect, socketException, processAssembly };
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);

            SafeRun(delegate
            {
                ip = paramsData[0] as string;
                port = (int)paramsData[1];
                processNameSpace = paramsData[2] as string[];
                onSocketConnect = paramsData[3] as Action;
                onSocketException = paramsData[4] as Action;
                var assembly = paramsData[5] as Assembly;
                netMessageDriverComponent = AddComponentEntity<NetMessageDriverComponents>(NetMessageDriverComponents.ParamCreate(assembly, processNameSpace));
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
