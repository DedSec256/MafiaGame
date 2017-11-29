using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Models;
using TelegramBot.Kernel.Standart;

namespace TelegramBot.Modules
{
    class HelpModule : ICommandsManager
    {
        public void AddInCommandCenter()
        {
            CommandsCenter.Add("/help", ShowHelpCallback);
            CommandsCenter.Add(new ReplyMenu("HelpMenu", true, new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    CommandsCenter.Add(new ReplyButton("Быстрые команды", FastCommandsCallback)).Button
                } 
            }));

        }

        private async void FastCommandsCallback(Message message, TelegramBotClient Bot, object arg)
        {
            await Bot.SendTextMessageAsync(message.Chat.Id,"",
                ParseMode.Markdown);
        }

        public HelpModule()
        {
            AddInCommandCenter();
        }

        public async void ShowHelpCallback(Message message, TelegramBotClient bot, object arg = null)
        {
            await CommandsCenter.GetMenu("HelpMenu").ShowAsync(message.Chat.Id, bot, "");
        }
    }
}
