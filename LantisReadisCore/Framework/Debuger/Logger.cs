using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Redis
{
    public class Logger
    {
        public static void DefaultColorSet()
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void Log(string info)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Write(info);

            DefaultColorSet();
        }

        public static void Wring(string info)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Write(info);

            DefaultColorSet();
        }

        public static void Error(string info)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Write(info);

            DefaultColorSet();
        }

        private static void Write(string info)
        {
            Console.WriteLine(info);
        }
    }
}
