using System;
using System.Collections.Generic;
using System.Text;
using Lantis.Extend;

namespace Lantis.ReadisOperation
{
    public class NetCallbackSystem
    {
        private static object lockHander = new object();
        private static LantisDictronaryList<int, Action<object>> dictionaryData = new LantisDictronaryList<int, Action<object>>();

        public static void AddNetCallback(int id,Action<object> netCallback)
        {
            lock (lockHander)
            {
                if (!dictionaryData.HasKey(id))
                {
                    dictionaryData.AddValue(id, netCallback);
                }
            }
        }

        public static Action<object> GetRemoveNetCallback(int id)
        {
            lock (lockHander)
            {
                if (dictionaryData.HasKey(id))
                {
                    var callFunback = dictionaryData[id];
                    dictionaryData.RemoveKey(id);
                    return callFunback;
                }

                return null;
            }
        }

        public static void CallAndRemoveById(int id,object data)
        {
            lock (lockHander)
            {
                var netCall = GetRemoveNetCallback(id);

                if (netCall != null)
                {
                    netCall(data);
                }
            }
        }
    }
}
