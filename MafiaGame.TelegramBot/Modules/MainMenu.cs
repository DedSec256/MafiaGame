using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineKeyboardButtons;
using TelegramBot.Kernel;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Standart;
using System.Web.Http;
using System.Windows.Forms;
using Mafiagame.DataLayer.Models;
using MafiaGame.DataLayer.Models;
using Telegram.Bot.Types.Enums;
using TelegramBot.ClientApi;
using TelegramBot.Kernel.Models;
using TelegramBot.Modules.Generators;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBot.Modules
{

    public class MainMenu : ICommandsManager
    {
        private MafiaService service = MafiaService.Create();
        public MainMenu()
        {
            AddInCommandCenter();
        }

        public virtual void AddInCommandCenter()
        {
            CommandsCenter.Add(new ReplyMenu("StartMenu", true, new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    CommandsCenter.Add(new ReplyButton("🌃 Создать игру", CreateGameCallback)).Button
                },
                new KeyboardButton[]
                {
                    CommandsCenter.Add(new ReplyButton("💣 Присоединиться к игре", JoinGameCallback)).Button
                },
                new KeyboardButton[]
                {
                    CommandsCenter.Add(new ReplyButton("👥 О боте", JoinGameCallback)).Button
                }
            }));

            CommandsCenter.Add("/start", RegisterUserCallback);
        }

        private async void RegisterUserCallback(Message message, TelegramBotClient bot, object arg)
        {
            try
            {
                UserDatabase.AddUser(service.Users.RegisterUserAsync(message.Chat.Id).Result);
            }
            catch (HttpRequestException ex)
            {
                await bot.SendTextMessageAsync(message.Chat.Id, $"Ошибка при регистрации пользователя: {ex.Message}", ParseMode.Markdown);
                return;
            }
            await CommandsCenter.GetMenu("StartMenu")
                .ShowAsync(message.Chat.Id, bot, $"Добро пожаловать в мафию...");
        }
        private async void JoinGameCallback(Message message, TelegramBotClient Bot, object arg)
        {
            var user = UserDatabase.GetUser(message.Chat.Id);
            if (user.User.ActiveGameId != null)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id,
                    "Вы уже участвуете в игре. Закончите предыдущую игру или введите команду /exit",
                    ParseMode.Markdown);
                return;
            }
            var games = service.Games.GetAllGamesAsync().Result.Select(g => g.ToGameRoom())
                        .ToArray();

            if (!games.Any())
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

                await Bot.SendTextMessageAsync(message.Chat.Id,
                    "Список игр *пуст* ",
                    ParseMode.Markdown, false, false, 0, menu.Keyboard);
                return;
            }

            else
            {
                await new GamesListGenerator(games).GenerateMenu().ShowAsync(message.Chat.Id, Bot, "Список доступных игр");
            }
        }

        private async void CreateGameCallback(Message message, TelegramBotClient Bot, object arg)
        {
            var user = UserDatabase.GetUser(message.Chat.Id);

            if (user.User.ActiveGameId!=null)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id,
                    "Вы уже участвуете в игре. Закончите предыдущую игру или введите команду /exit",
                    ParseMode.Markdown);
                return;
            }

            else
            {
                var menu = new ReplyMenu("", true, new KeyboardButton[][]
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("« Назад в главное меню")
                    }
                });

                await Bot.SendTextMessageAsync(message.Chat.Id,
                    "Введите *название* игры (до 20 символов):",
                    ParseMode.Markdown, false, false, 0, menu.Keyboard);

                user.CommandRegex = (new Regex(".{0,20}"), new ReplyCallback(CorrectNameCallback), 
                                                           new ReplyCallback(UncorrectNameCallback));
            }
        }
        private void UncorrectNameCallback(Message message, TelegramBotClient Bot, object arg)
        {
            var user = UserDatabase.GetUser(message.Chat.Id);

            if (message.Text == "« Назад в главное меню")
            {
                user.CommandRegex = LocalUser.DefaultRegex;
                CommandsCenter.GetMenu("StartMenu").ShowAsync(message.Chat.Id, Bot, "");
                return;
            }
            else
            {
                var menu = new ReplyMenu("", true, new KeyboardButton[][]
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("« Назад в главное меню")
                    }
                });

                Bot.SendTextMessageAsync(message.Chat.Id,
                    "*Слишком длинное название игры!* Введите название игры (до 20 символов):",
                    ParseMode.Markdown, false, false, 0, menu.Keyboard);
            }
        }
        private void CorrectNameCallback(Message message, TelegramBotClient Bot, object arg)
        {
            var user = UserDatabase.GetUser(message.Chat.Id);
            user.GameRoomCreation = new GameRoomCreation() {Name = message.Text, AdminId = message.Chat.Id};

            var menu = new ReplyMenu("", true, new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("« Назад в главное меню")
                }
            }, user.GameRoomCreation.ToString());

            menu.ShowAsync(message.Chat.Id, Bot,
                "Укажите *максимальное количество игроков* игры (4-19 игроков):", true);

            user.CommandRegex = (new Regex("[4-9]|([1][1-9])"), new ReplyCallback(CorrectMaxPlayersCallback), 
                new ReplyCallback(UncorrectMaxPlayersCallback));
        }
        private void CorrectMaxPlayersCallback(Message message, TelegramBotClient Bot, object arg)
        {
            var user = UserDatabase.GetUser(message.Chat.Id);
            user.CommandRegex = LocalUser.DefaultRegex;
            user.GameRoomCreation.MaxPlayers = Byte.Parse(message.Text);

            var menu = new ReplyMenu("", true, new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("« Назад в главное меню")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("🌘 Создать игру!")
                }
            });

            new RolesListGenerator(user.GameRoomCreation.Roles).GenerateMenu()
                .ShowAsync(message.Chat.Id, Bot, user.GameRoomCreation.ToString(), true);
            menu.ShowAsync(message.Chat.Id, Bot, "Выберите специальные роли:", true);

        }
        private void UncorrectMaxPlayersCallback(Message message, TelegramBotClient Bot, object arg)
        {
            var user = UserDatabase.GetUser(message.Chat.Id);

            if (message.Text == "« Назад в главное меню")
            {
                user.CommandRegex = LocalUser.DefaultRegex;
                CommandsCenter.GetMenu("StartMenu").ShowAsync(message.Chat.Id, Bot);
                return;
            }
            else
            {
                var menu = new ReplyMenu("", true, new KeyboardButton[][]
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("« Назад в главное меню")
                    }
                });

                Bot.SendTextMessageAsync(message.Chat.Id,
                    "*Неверное количество*. Укажите максимальное количество игроков (4-19 игроков):",
                    ParseMode.Markdown, false, false, 0, menu.Keyboard);
            }
        }
    }
}
