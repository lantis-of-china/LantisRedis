using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Locker
{
    public class SafeLogerChecker
    {
        private static object lockHandel = new object();
        private static Dictionary<object, string> recordMap = new Dictionary<object, string>();

        public static void Record(object action, string name)
        {
            lock (lockHandel)
            {
                recordMap.Add(action, name);
                Logger.Log($"Record lock:{name} count:{recordMap.Count}");
            }
        }

        public static void Remove(object action,string name)
        {
            lock (lockHandel)
            {
                recordMap.Remove(action);
                Logger.Log($"Remove lock:{name} count:{recordMap.Count}");
            }
        }
    }
}
