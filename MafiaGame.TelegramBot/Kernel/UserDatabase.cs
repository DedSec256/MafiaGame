using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaGame;
using MafiaGame.DataLayer.Models;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Kernel.CIO;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Models;
using MessageType = TelegramBot.Kernel.CIO.MessageType;

namespace TelegramBot.Kernel
{
    public static class UserDatabase
    {
        static volatile Dictionary<long, LocalUser> userDatabase = new Dictionary<long, LocalUser>();

        public static void Broadcast(Func<LocalUser, bool> patternPredicate, 
            Func<LocalUser, string> messagePattern, TelegramBotClient bot,
            IReplyMarkup keyboard = null)
        {
            var userList = userDatabase.Values.Where(patternPredicate);
            Parallel.ForEach(userList, user => 
            {
                try
                {
                    string message = messagePattern(user);
                    if (message != null)
                    {
                        bot.SendTextMessageAsync(user.User.Id, message, ParseMode.Markdown, false, false, 0, keyboard);
                        BotConsole.Write($"Уведомление было отправлено пользователю с id {user.User.Id}",
                            MessageType.System);
                    }
                }
                catch (Exception ex)
                {
                    BotConsole.Write(ex.Message, MessageType.Error);
                }
            });
        }
        public static void LoadUsers()
        {
            var result = ClientApi.GetAllUsers();
            var users = result.Result.Content.ReadAsStringAsync().Result.FromJSON<User[]>();

            foreach (var user in users)
            {
                try
                {
                    userDatabase.Add(user.Id, new LocalUser(user));
                }
                catch (Exception ex)
                {
                    BotConsole.Write(ex.Message, MessageType.Error);
                }
            }
        }

        public static void SaveUsers()
        {
            foreach (var user in userDatabase)
            {
                try
                {
                    user.Value.Save();
                }
                catch (Exception ex)
                {
                    BotConsole.Write(ex.Message, MessageType.Error);
                }
            }
        }

        public static int UsersCount() => userDatabase.Count;

        public static LocalUser GetUser(long id)
        {
            if (userDatabase.ContainsKey(id))
                return userDatabase[id];
            else return null;
        }

        public static bool AddUser(User user)
        {
            if (!userDatabase.ContainsKey(user.Id))
            {
                userDatabase.Add(user.Id, new LocalUser(user));
                return true;
            }
            return false;
        } 
    }

}
