
using System;

namespace PowerGridApi
{
    public enum LogType
    {
        Info,
        Error
    }

    public enum LogDestination
    {
        Console
    }

    public interface ILogger
    {
        void Log(string message);

        void Log(string message, params object[] parameters);

        void Log(LogDestination destination, LogType type, string message, params object[] parameters);

        void LogError(LogDestination destination, Exception ex);

        void Log(LogDestination destination, LogType type, byte[] bytes);
    }
}
