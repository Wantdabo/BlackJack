using System;

namespace SimpleLog
{
    public class Log
    {
        public static void Print(string _msg, ConsoleColor _color = ConsoleColor.White, bool _isLine = true)
        {
            Console.ForegroundColor = _color;
            Console.WriteLine(DateTime.UtcNow + "\t" + _msg + (_isLine ? "\n" : ""));
        }
    }
}
