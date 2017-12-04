﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mafiagame.DataLayer.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineKeyboardButtons;
using TelegramBot.Kernel;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Models;
using TelegramBot.Kernel.Standart;

namespace TelegramBot.Modules.Generators
{
    class GamesListGenerator : IGenerator
    {
        private GameRoom[] games;
        public GamesListGenerator(GameRoom[] _games)
        {
            games = _games;
        }

        public BotMenu GenerateMenu()
        {
            InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[games.Length][];

            int i = 0;
            foreach (var game in games)
            {
                keyboard[i++] = new InlineKeyboardButton[]
                {
                    CommandsCenter.GetInlineButton(game.Id.ToString()).Button
                };
            }
            return new InlineMenu("", keyboard);
        }
    }
}
