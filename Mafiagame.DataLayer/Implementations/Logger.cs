using System;
using NLog;
using NLog.Fluent;
using ILogger = MafiaGame.Common.Interfaces.ILogger;

namespace MafiaGame.DataLayer.Implementations
{
    public class DataLog : ILogger
    {
        public static Logger Instance = LogManager.GetCurrentClassLogger();
        public void Log(Exception ex)
        {
        }

        public void Log(string message)
        {
        }
    }
}
