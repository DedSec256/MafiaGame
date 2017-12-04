using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using TelegramBot.Bot;
using TelegramBot.Kernel.CIO;
using TelegramBot.Kernel.System;
using TelegramBot.Properties;

namespace TelegramBot.Kernel
{
    public static class BotMain
    {
        /* Пространоство пользовательских функций */
        const string DevNamespace = "TelegramBot.Modules";

        private static TelegramBot Bot;
        private static Thread consoleThread;

        static void Main()
        { 
            InitalizeConsole();
            Global.InitalizeEnvironment();
            Run();
        }

        public static void Run()
        {
            BotConsole.Write("---------------------------------------------------------------------\n" +
                             $"BotApi v{Assembly.GetExecutingAssembly().GetName().Version}\n" +
                             "---------------------------------------------------------------------",
                MessageType.System);

            ExecuteModules();

            Bot = new TelegramBot(Global.Settings.TOKEN);
            Bot.Run();

            consoleThread = new Thread(ConsoleCommander);
            consoleThread.Start();
        }


        private static void InitalizeConsole()
        {
            Console.OutputEncoding = Encoding.UTF8;
            BotConsole.SetWriter((text, type) =>
            {
                if (type == MessageType.Error)
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    catch { }
                }
                else if (type == MessageType.Warning) Console.ForegroundColor = ConsoleColor.Yellow;
                else if (type == MessageType.Info) Console.ForegroundColor = ConsoleColor.Green;
                else if (type == MessageType.System) Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(text);
                Console.ResetColor();
            });

            BotConsole.SetReader(Console.ReadLine, true);
            BotConsole.SetNotifyer((caption, text) =>
            {
                MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                BotConsole.Write($"{caption}: {text}", MessageType.Warning);
            });
        }

        /// <summary>
        /// Функция - обработчик команд, поступающих в консоль
        /// </summary>
        private static void ConsoleCommander()
        {
            /* Обрабатываем команды, поступающие в консоль */
            while (true)
            {
                BotConsole.StartReading();
            }
        }

        /// <summary>
        /// Функция, подключающая все модули бота
        /// </summary>
        static void ExecuteModules()
        {
            BotConsole.Write("[Подключение модулей...]", MessageType.System);

            /* Подключаем модули, создавая обьекты их классов */
            String[] typelist =
                Assembly.GetExecutingAssembly().GetTypes().AsParallel()
                    .Where(t => t.Namespace == DevNamespace).OrderBy(t => t.FullName)
                    .Select(t => t.FullName).ToArray();
            foreach (var type in typelist)
            {
                {
                    Activator.CreateInstance(Type.GetType(type));
                    BotConsole.Write("Подключение " + type + "...");
                }
            }
            BotConsole.Write("Модули подключены.", MessageType.Info);
        }
    }


}

