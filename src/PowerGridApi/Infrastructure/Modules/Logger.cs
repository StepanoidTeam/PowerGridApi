
using System;

namespace PowerGridApi
{

    public class Logger : ILogger
    {
        public void Log(string message)
        {
        }

        public void Log(string message, params object[] parameters)
        {
        }

        public void Log(LogDestination destination, LogType type, string message, params object[] parameters)
        {
            switch (destination)
            {
                case LogDestination.Console:
                    SetConsoleColors(type);
                    Console.WriteLine(string.Format(message, parameters));
                    Console.ResetColor();
                    break;
            }
        }

        public void Log(LogDestination destination, LogType type, byte[] bytes)
        {
            switch (destination)
            {
                case LogDestination.Console:
                    SetConsoleColors(type);
                    Console.WriteLine(bytes);
                    Console.ResetColor();
                    break;
            }
        }

        public void LogError(LogDestination destination, Exception ex)
        {
            switch (destination)
            {
                case LogDestination.Console:
                    SetConsoleColors(LogType.Error);
                    Console.WriteLine(ex);
                    Console.ResetColor();
                    break;
            }
        }

        private void SetConsoleColors(LogType type)
        {
            switch (type)
            {
                case LogType.Info:
                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogType.Error:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }

    }
}
