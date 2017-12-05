using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaGame.DataLayer.Models;
using Telegram.Bot.Types.InlineKeyboardButtons;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Models;
using TelegramBot.Kernel.Standart;

namespace TelegramBot.Modules.Generators
{
    class RolesListGenerator : IGenerator
    {
        private string roles;
        public RolesListGenerator(string _roles)
        {
            roles = _roles;
        }

        public BotMenu GenerateMenu()
        {
            InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[4][];
            keyboard[0] = new InlineKeyboardButton[]
            {
                !roles.Contains(Roles.Comissar.GetRoleIcon())
                    ? CommandsCenter.GetInlineButton(Roles.Comissar.ToString() + "Add").Button
                    : CommandsCenter.GetInlineButton(Roles.Comissar.ToString() + "Remove").Button
            };
            keyboard[1] = new InlineKeyboardButton[]
            {
                !roles.Contains(Roles.Doctor.GetRoleIcon())
                    ? CommandsCenter.GetInlineButton(Roles.Doctor.ToString() + "Add").Button
                    : CommandsCenter.GetInlineButton(Roles.Doctor.ToString() + "Remove").Button
            };
            keyboard[2] = new InlineKeyboardButton[]
            {
                !roles.Contains(Roles.Maniac.GetRoleIcon())
                    ? CommandsCenter.GetInlineButton(Roles.Maniac.ToString() + "Add").Button
                    : CommandsCenter.GetInlineButton(Roles.Maniac.ToString() + "Remove").Button
            };
            keyboard[3] = new InlineKeyboardButton[]
            {
                CommandsCenter.GetInlineButton("createGame").Button
            };

            return new InlineMenu("Выберите специальные роли:", keyboard);
        }
    }
}
