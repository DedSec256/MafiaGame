using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using NLog.Config;
using NLog.Fluent;
using TelegramBot.Kernel.CIO;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Modules;
using MainMenu = TelegramBot.Modules.MainMenu;

namespace TelegramBot.Kernel
{
    public class Log
    {
        private static  Logger log = LogManager.GetCurrentClassLogger();
        private const string configFilename = "LogConfig.xml";

        public Log()
        {
            if (log == null)
            {
                log = LogManager.GetCurrentClassLogger();
                log.Factory.Configuration = new XmlLoggingConfiguration(configFilename);
            }
        }

        public void Write(string data)
        {
            log.Log(LogLevel.Info, data);
        }

        public void Write(Exception ex)
        {
            log.Error(ex);
        }

    }

}
