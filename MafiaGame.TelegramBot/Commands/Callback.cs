using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Kernel.CIO;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Standart;

namespace TelegramBot.Kernel
{
    public delegate void CallbackSignature(Message message, TelegramBotClient Bot, object arg = null);

    public abstract class Callback
    {
        private readonly CallbackSignature CallbackSignature;

        public Callback(CallbackSignature callback)
        {
            CallbackSignature = callback;
        }

        protected abstract void BeforeExecute(Message message, TelegramBotClient bot, object arg = null);

        public virtual void Execute(Message message, TelegramBotClient bot, object arg = null)
        {
            try
            {
                BeforeExecute(message, bot, arg);
                CallbackSignature(message, bot, arg);
                AfterExecute(message, bot, arg);
            }
            catch (Exception ex)
            {
                BotConsole.Write($"Ошибка в {CallbackSignature.Method.Name}: {ex.Message}\n" +
                          $"StackTrace: {ex.StackTrace}", MessageType.Error);
            }
        }

        protected abstract void AfterExecute(Message message, TelegramBotClient bot, object arg = null);
    }
    public class ReplyCallback : Callback
    {
        public ReplyCallback(CallbackSignature callback) : base(callback) { }

        protected override void BeforeExecute(Message message, TelegramBotClient bot, object arg = null)
        {
            string info =
                $"ReplyCommand: [{message.Chat.Id}] {message.Chat.FirstName} {message.Chat.LastName} " +
                $"- {message.Text} ({message.Date.TimeOfDay})";

            BotConsole.Write(info);
        }

        protected override void AfterExecute(Message message, TelegramBotClient bot, object arg = null)
        {
        }
    }
    public class InlineCallback : Callback
    {
        private readonly string _buttonName;

        public InlineCallback(CallbackSignature callback, string buttonName) : base(callback)
        {
            _buttonName = buttonName;
        }

        protected override void BeforeExecute(Message message, TelegramBotClient bot, object arg = null)
        {
            var query = arg as CallbackQuery;
            string info =
                $"InlineButton: [{query.Message.Chat.Id}] {query.Message.Chat.FirstName} {query.Message.Chat.LastName} " +
                $"- {_buttonName} ({query.Message.Date.TimeOfDay})";

            BotConsole.Write(info);
        }
        protected override void AfterExecute(Message message, TelegramBotClient bot, object arg = null)
        {
            //if (arg != null) CommandsCenter.TryInlineCommand("callbackQueryAnswer", message, bot, arg);
        }
    }
}
