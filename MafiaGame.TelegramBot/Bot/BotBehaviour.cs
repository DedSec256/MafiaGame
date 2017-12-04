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

                await Task.Run
                (
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

}
