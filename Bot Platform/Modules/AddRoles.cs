using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mafiagame.DataLayer.Models;
using MafiaGame.DataLayer.Models;
using Microsoft.SqlServer.Server;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using TelegramBot.Kernel;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Models;
using TelegramBot.Kernel.Standart;
using TelegramBot.Modules.Generators;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBot.Modules
{
    class AddRoles : ICommandsManager
    {
        public AddRoles()
        {
            AddInCommandCenter();
        }
        public void AddInCommandCenter()
        {

            CommandsCenter.Add(new InlineButton($"❌ Комиссар {Roles.Comissar.GetRoleIcon()}", 
                Roles.Comissar.ToString() + "Add", AddRoleCallback));
            CommandsCenter.Add(new InlineButton($"❌ Доктор {Roles.Doctor.GetRoleIcon()}",
                Roles.Doctor.ToString() + "Add", AddRoleCallback));
            CommandsCenter.Add(new InlineButton($"❌ Маньяк {Roles.Maniac.GetRoleIcon()}",
                Roles.Maniac.ToString() + "Add", AddRoleCallback));

            CommandsCenter.Add(new InlineButton($"✅ Комиссар {Roles.Comissar.GetRoleIcon()}",
                Roles.Comissar.ToString() + "Remove", AddRoleCallback));
            CommandsCenter.Add(new InlineButton($"✅ Доктор {Roles.Doctor.GetRoleIcon()}",
                Roles.Doctor.ToString() + "Remove", AddRoleCallback));
            CommandsCenter.Add(new InlineButton($"✅ Маньяк {Roles.Maniac.GetRoleIcon()}",
                Roles.Maniac.ToString() + "Remove", AddRoleCallback));

            CommandsCenter.Add(new ReplyButton("🌘 Создать игру!", CreateGameCallback));

        }

        private void CreateGameCallback(Message message, TelegramBotClient Bot, object arg)
        {
            var user = UserDatabase.GetUser(message.Chat.Id);

            if (user.GameRoomCreation == null)
            {
                GameNotFound(message, Bot, user);
                return;
            }

            else
            {
                var res = ClientApi.CreateGame(GameRoom.CreateGameRoom(user.GameRoomCreation)).Result;
                if (res.IsSuccessStatusCode)
                {
                    var game = res.Content.ReadAsStringAsync().Result.FromJSON<GameRoom>();
                    user.SetGame(game.Id);

                    Bot.SendTextMessageAsync(message.Chat.Id,
                        "Комната успешно создана!\n*Ожидание игроков...*", ParseMode.Markdown);
                }
                else
                {
                    Bot.SendTextMessageAsync(message.Chat.Id,
                        "Ошибка при создании игры 😢: " + res.ReasonPhrase);
                    
                }
                user.GameRoomCreation = null;
            }
        }

        private void AddRoleCallback(Message message, TelegramBotClient Bot, object arg)
        {
            var user = UserDatabase.GetUser(message.Chat.Id);
            string buttonText = CommandsCenter.GetInlineButton((arg as CallbackQuery).Data).Button.Text;

            if (buttonText.StartsWith("❌"))
            {
                if (user.GameRoomCreation == null)
                {
                    GameNotFound(message, Bot, user);
                    return;
                }

                else user.GameRoomCreation
                        .AddRole(buttonText.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries)[2]);
            }
            else if(buttonText.StartsWith("✅"))
            {
                user.GameRoomCreation
                    .DeleteRole(buttonText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2]);
            }

            new RolesListGenerator(user.GameRoomCreation.Roles).GenerateMenu()
                .ShowAsync(message.Chat.Id, Bot, user.GameRoomCreation.ToString(), true, message.MessageId);
        }

        private void GameNotFound(Message message, TelegramBotClient bot, LocalUser user)
        {
            var menu = new ReplyMenu("", true, new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("« Назад в главное меню")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("🌃 Создать игру")
                }
            });

            bot.SendTextMessageAsync(message.Chat.Id,
                "Сначала создайте игру, чтобы её настроить",
                ParseMode.Markdown, false, false, 0, menu.Keyboard);
            return;
        }
    }
}
