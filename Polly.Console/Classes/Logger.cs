using System;
using System.Collections.Generic;
using System.Text;

namespace Polly.Console.Classes
{
    public class Logger
    {
        public void WriteInfo(string message, params object[] param)
        {
            System.Console.WriteLine($"{DateTime.Now}: {message}", param);
        }

        public void WriteError(string message, params object[] param)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"{DateTime.Now}: {message}", param);
            System.Console.ResetColor();
        }
    }
}
