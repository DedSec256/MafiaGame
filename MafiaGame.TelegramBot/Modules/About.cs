using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Models;
using TelegramBot.Kernel.Standart;

namespace TelegramBot.Modules
{
    class About : ICommandsManager
    {
        public About()
        {
            AddInCommandCenter();
        }

        public void AddInCommandCenter()
        {
            CommandsCenter.Add(new InlineMenu("AboutMenu",
                new InlineKeyboardButton[][]
                {
                    new InlineKeyboardButton[]
                    {
                        new InlineKeyboardUrlButton("Правила игры", "http://analytic-spy.com/support")
                    },
                    new InlineKeyboardButton[]
                    {
                        new InlineKeyboardUrlButton("💬 Нашли баг или бот не работает?", "https://t.me/DedSec256")
                    }
                }));
        }

    }
}
