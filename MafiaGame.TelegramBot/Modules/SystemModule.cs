using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Kernel;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Models;
using TelegramBot.Kernel.Standart;

namespace TelegramBot.Modules
{
    class SystemModule : ICommandsManager
    {
        public void AddInCommandCenter()
        {
            CommandsCenter.Add(new InlineButton("", "callbackQueryAnswer", QueryAnswerCallback));
        }

        public SystemModule()
        {
            AddInCommandCenter();
        }

        public async void QueryAnswerCallback(Message message, TelegramBotClient Bot, object arg = null)
        {
            await Bot.AnswerCallbackQueryAsync(arg as string , message.Text, true);
        }
    }
}
