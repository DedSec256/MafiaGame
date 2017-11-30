using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mafiagame.DataLayer.Models;
using MafiaGame;
using TelegramBot.Kernel.Standart;
using MafiaGame.DataLayer.Models;
using TelegramBot.Kernel.Interfaces;

namespace TelegramBot.Kernel.Models
{
    public class LocalUser
    {
        public User User { get; }
        private bool _notSaved = false;
        public GameRoomCreation GameRoomCreation { get; set; }

        public (Regex regex, Callback done,
            Callback failed) CommandRegex;

        public static (Regex regex, Callback done,
            Callback failed) DefaultRegex = (new Regex("[^☠]*"), new ReplyCallback(async (message, bot, arg3) => 
                {
                    if (message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
                    {
                        await Task.Run(() => CommandsCenter.TryReplyCommand(message, bot));
                    }
                    else
                    {
                        await Task.Run(() => CommandsCenter.GetMenu("StartReplyMenu")
                            .ShowAsync(message.Chat.Id, bot, Global.Settings.MAIN_MENU_TEXT, false));
                    }
                }
                ), null);

        public LocalUser(User user)
        {
            User = user;
            CommandRegex = DefaultRegex;
        }

        public void SetGame(long gameId)
        {
            User.ActiveGameId = gameId;
            _notSaved = true;
        }

        public void Save()
        {
            try
            {
                if (_notSaved) 
                    ClientApi.UpdateUser(this.User);
            }
            catch { }
        }
    }
}
