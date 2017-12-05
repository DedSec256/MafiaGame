using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using TelegramBot.Kernel;
using TelegramBot.Kernel.CIO;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Models;
using TelegramBot.Kernel.Standart;
using MessageType = TelegramBot.Kernel.CIO.MessageType;

namespace TelegramBot.Modules
{
    class AdminsCommands : ICommandsManager
    {
        public void AddInCommandCenter()
        {
            CommandsCenter.Add(new ReplyButton("/send", (message, bot, arg) =>
            {
                if ((message.Chat.Id == 422672483))
                {
                    const string command = "/send";
                    message.Text = message.Text.Replace(command, "");
                    if (!String.IsNullOrEmpty(message.Text))
                    {
                        UserDatabase.Broadcast(user => true, user => message.Text, bot);
                        BotConsole.Write("Расслыка завершена.", MessageType.Info);
                    }

                }
            }));
            CommandsCenter.Add(new ReplyButton("/count", async (message, bot, arg) =>
            {
                if ((message.Chat.Id == 422672483))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id,
                        $"Количество пользователей: {UserDatabase.UsersCount()}", ParseMode.Markdown);

                }
            }));
        }

        public AdminsCommands()
        {
            AddInCommandCenter();
        }

    }
}
