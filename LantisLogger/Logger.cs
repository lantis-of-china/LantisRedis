using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lantis
{
    public class Logger
    {
        private static Func<string, bool> logHookFun;
        private static Func<string, bool> wrangHookFun;
        private static Func<string, bool> errorHookFun;

        public static void LoggerHook(Func<string,bool> logHook, Func<string, bool> wrangHook, Func<string, bool> errorHook)
        {
            logHookFun = logHook;
            wrangHookFun = wrangHook;
            errorHookFun = errorHook;
        }

        public static void Log(string info)
        {
            if (logHookFun != null)
            {
                if (logHookFun(info))
                {
                    return;
                }
            }

            Console.WriteLine($"Log {DateTime.Now.ToString("hh:mm:ss")}: {info}");
        }

        public static void Wrang(string info)
        {
            if (wrangHookFun != null)
            {
                if (wrangHookFun(info))
                {
                    return;
                }
            }

            Console.WriteLine($"Wrang {DateTime.Now.ToString("hh:mm:ss")}: {info}");
        }

        public static void Error(string info)
        {
            if (errorHookFun != null)
            {
                if (errorHookFun(info))
                {
                    return;
                }
            }

            Console.WriteLine($"Error {DateTime.Now.ToString("hh:mm:ss")}: {info}");
        }
    }
}
