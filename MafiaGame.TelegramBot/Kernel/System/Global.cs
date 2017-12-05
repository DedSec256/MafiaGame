using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelegramBot.Bot;
using TelegramBot.Kernel.CIO;
using TelegramBot.Properties;

namespace TelegramBot.Kernel.System
{
    public static class Global
    {
        public static BotSettings Settings { get; private set; }
        public const string DIRECTORY_PATH = "Data";
        private static readonly string SETTINGS_FILENAME;
        public static readonly string VOTES_PATH;

        public static void InitalizeEnvironment()
        {
            try
            {
                if (!File.Exists(SETTINGS_FILENAME))
                {
                    using (StreamWriter writer = new StreamWriter(SETTINGS_FILENAME, false, Encoding.UTF8))
                    {
                        writer.Write(Resources.SettingsTemplate);
                    }
                }
                if (!Directory.Exists(VOTES_PATH))
                {
                    Directory.CreateDirectory(VOTES_PATH);
                }
            }
            catch (Exception ex)
            {
                BotConsole.Write("Ошибка при инициализации программы:\n" + ex.Message, MessageType.Warning);
                Thread.Sleep(5000);
                Environment.Exit(-1);
            }
            ReadSettings();
        }
        private static void ReadSettings()
        {
            Settings = BotSettings.ReadFrom(SETTINGS_FILENAME);
        }

        static Global()
        {
            ConsoleEventHooker.Closed += ConsoleEventHooker_Closed;
            SETTINGS_FILENAME = Path.Combine(DIRECTORY_PATH, "settings.ini");
            VOTES_PATH = Path.Combine(DIRECTORY_PATH, "Votes");

            UserDatabase.LoadUsers();
        }

        private static void ConsoleEventHooker_Closed(object sender, EventArgs e)
        {
        }




    }
}
