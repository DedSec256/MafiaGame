using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using TelegramBot.Kernel.CIO;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Standart;
using TelegramBot.Modules;
using MessageType = TelegramBot.Kernel.CIO.MessageType;

namespace TelegramBot.Kernel
{
    /* Bot Behaviour */
    public class TelegramBot: IDisposable
    {
        private readonly string Key;
        private static TelegramBotClient Bot;

        public TelegramBot(string key)
        {
            Key = key;
        }

        private Thread botThread;

        public void Run()
        {
            botThread = new Thread(BotWorkAsync);
            botThread.Start();
        }

        private async void BotWorkAsync()
        {
            try
            {
                Bot = new TelegramBotClient(Key);
                await Bot.SetWebhookAsync("");
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                BotConsole.Write(ex.Message, MessageType.Error);
                Thread.Sleep(5000);
                Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                BotConsole.Write("BotWorkAsync: " + ex.Message, MessageType.Error);
                Thread.Sleep(5000);
                Environment.Exit(-1);
            }

            Bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Bot.OnUpdate += Bot_OnUpdate;
            Bot.StartReceiving();
            BotConsole.Write("Бот запущен.", MessageType.Info);

        }

        private  void Bot_OnUpdate(object sender, UpdateEventArgs e)
        {
            string info = "Неизвестная ошибка.";
                
            try
            {
                if (e.Update.CallbackQuery != null || e.Update.InlineQuery != null)
                    return;
                var update = e.Update;
                var message = update.Message;
                if (message == null) return;

                var user = UserDatabase.GetUser(message.Chat.Id);
                if (user == null)
                {
                    CommandsCenter.GetReplyButton("/start").Callback.Execute(message, Bot, null);
                    return;
                }
                if (message.Text.StartsWith("/send"))
                {
                    CommandsCenter.GetReplyButton("/send").Callback.Execute(message, Bot, null);
                    return;
                }
                else if (message.Text.StartsWith("/vote"))
                {
                    CommandsCenter.GetReplyButton("/vote").Callback.Execute(message, Bot, null);
                    return;
                }

                if (user.CommandRegex.regex.Match(message.Text).Value == message.Text)
                {
                    user.CommandRegex.done.Execute(message, Bot);
                }
                else user.CommandRegex.failed.Execute(message, Bot);

            }
            catch (Exception ex)
            {
                BotConsole.Write($"Ошибка в {info}:\n" 
                    +ex.Message + "\nStackStrace:\n" + ex.StackTrace, MessageType.Error);
            }
        }

        private async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            try
            {
                var message = e.CallbackQuery.Message;
                message.Caption = e.CallbackQuery.Data;

                await Task.Run(
                    () =>
                    CommandsCenter.TryInlineCommand(e.CallbackQuery.Data, message, Bot, e.CallbackQuery)
                );
            }
            catch (Exception ex)
            {
                BotConsole.Write($"Ошибка в :\n"
                                 + ex.Message + "\nStackStrace:\n" + ex.StackTrace, MessageType.Error);
            }
        }

        ~TelegramBot()
        {
            ReleaseUnmanagedResources();
        }
        public void Stop()
        {
            if (Bot != null && Bot.IsReceiving)
            {
                Bot.StopReceiving();
                Bot = null;
            }
            try
            {
                botThread?.Abort();
            }
            catch
            {
                BotConsole.Write("В боте был вызван Dispose()", MessageType.Info);
            }
        }

        private void ReleaseUnmanagedResources()
        {
            Stop();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }
    }

    public class Settings
    {
        public struct Point
        {
            public int X { get; private set; }
            public int Y { get; private set; }

            public Point(int x, int y)
            {
                if (x <= 0 || y <= 0)
                    throw new ArgumentException($"Неверно указаны размеры списков монет [*_MENU_SIZE]: {x};{y}");
                X = x;
                Y = y;
            }

            public static Point Parse(string text)
            {
                string[] tempText = text.Split(new[] {";", ","}, StringSplitOptions.RemoveEmptyEntries);
                return new Point(UInt16.Parse(tempText[0].Trim()), 
                    UInt16.Parse((tempText[1]).Trim()));
            }
        }

        public string TOKEN { get; private set; }
        public long SAVE_TIME { get; private set; }
        public long UPDATE_TIME { get; private set; }
        public uint MAX_FAV_COINS { get; private set; }
        public Point BTC_MENU_SIZE { get; private set; }
        public Point USDT_MENU_SIZE { get; private set; }
        public string MAIN_MENU_TEXT { get; private set; }
        public string INFO_MENU_TEXT { get; private set; }
        public string START_MENU_TEXT { get; private set; }

        private const char COMMENTS = '$';
        public static Settings ReadFrom(string filename)
        {
            StringBuilder dataLine = new StringBuilder();
            try
            {
                using (StreamReader reader = new StreamReader(filename))
            {
                string tempText = reader.ReadToEnd();
                #region поиск комментариев
                    bool findComments = false;
                for (int i = 0; i < tempText.Length; i++)
                {
                    if (tempText[i] == '\r' ||
                        tempText[i] == '\n' ||
                        tempText[i] == '\t') continue;

                    if (tempText[i] == COMMENTS)
                    {
                        findComments = !findComments;
                        continue;
                    }
                    if (findComments == true) continue;

                    dataLine.Append(tempText[i]);
                }
                #endregion
                }
                string[] KEYS =
            {
                "[TOKEN]",
                "[SAVE_TIME]",
                "[UPDATE_TIME]",
                "[MAX_FAV_COINS]",
                "[BTC_MENU_SIZE]",
                "[USDT_MENU_SIZE]",
                "[MAIN_MENU_TEXT]",
                "[INFO_MENU_TEXT]",
                "[START_MENU_TEXT]"
            };

                string[] param = dataLine.ToString().Split(KEYS, StringSplitOptions.RemoveEmptyEntries); //switch to remove
                string token = param[0].Trim();

                long saveTime = Int64.Parse(param[1].Trim());
                if (saveTime <= 0) throw new ArgumentException("Неверно указан параметр 'интервал сохранения' [SAVE_TIME]: число должно быть > 0.\n" +
                                                               "Рекомендуется число не меньше 1800000");

                long updateTime = Int64.Parse(param[2].Trim());
                if (updateTime <= 0) throw new ArgumentException("Неверно указан параметр 'интервал обновления' [UPDATE_TIME]: число должно быть > 0.\n");

                uint maxFavCoins = UInt16.Parse(param[3].Trim());
                if (maxFavCoins <= 0) throw new ArgumentException("Неверно указано максимальное число монет в избранном [MAX_FAV_COINS]: число должно быть > 0.");

                Point btcMenuSize = Point.Parse(param[4].Trim());
                Point usdtMenuSize = Point.Parse(param[5].Trim());

                string mainMenuText = param[6].Trim();
                string infoMenuText = param[7].Trim();
                string startMenuText = param[8].Trim();
                return new Settings()
                {
                    TOKEN = token,
                    SAVE_TIME = saveTime,
                    UPDATE_TIME = updateTime,
                    MAX_FAV_COINS = maxFavCoins,
                    BTC_MENU_SIZE = btcMenuSize,
                    USDT_MENU_SIZE = usdtMenuSize,
                    MAIN_MENU_TEXT = mainMenuText,
                    INFO_MENU_TEXT = infoMenuText,
                    START_MENU_TEXT = startMenuText
                };
            }
            catch (Exception ex)
            {
                BotConsole.Write("Ошибка при чтении настроек бота:\n" + ex.Message,
                    MessageType.Error);
                Thread.Sleep(5000);
                Environment.Exit(1);
            }
            return null;
        }
    }
}
