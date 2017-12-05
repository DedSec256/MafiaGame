using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Mafiagame.DataLayer.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.ClientApi;
using TelegramBot.Kernel;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Models;
using TelegramBot.Kernel.Standart;

namespace TelegramBot.Modules
{
    class InGame : ICommandsManager
    {
        MafiaService service = MafiaService.Create();
        public void AddInCommandCenter()
        {
            CommandsCenter.Add("/exit", ExitGameCallback);
            CommandsCenter.Add(new ReplyMenu("ExitGameMenu", true,
                new KeyboardButton[][]
                {
                    new KeyboardButton[]
                    { 
                        CommandsCenter.Add(new ReplyButton("Выйти из игры", ExitGameCallback )).Button
                    }
                }));
        }

        private async void ExitGameCallback(Message message, TelegramBotClient Bot, object arg)
        {
            var user = UserDatabase.GetUser(message.Chat.Id);
            if (user.User.ActiveGameId == null)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, "Вы не участвуете ни в одной из игр :c", ParseMode.Markdown);
                return;
            }
            var game = service.Games.GetGameAsync(user.User.ActiveGameId.Value).Result;
            if (game.Players.Count() == 1 || game.Id == user.User.ActiveGameId)
            {
                DeleteGame(message, Bot, user, game);
            }
            else
            {
                DeleteUser(message, Bot, user, game);
            }
        }

        private async void DeleteUser(Message message, TelegramBotClient Bot, LocalUser user, GameRoomJson game)
        {
            try
            {
                await service.Games.DeletePlayerFromGame(game.Id, user.User.Id);
                await CommandsCenter.GetMenu("StartMenu").ShowAsync(message.Chat.Id, Bot,
                    $"Вы успешно покинули игру.");
                UserDatabase.Broadcast(u => game.Players.Select(s => s.UserId == u.User.Id).Any(),
                    u => $"Игрок {message.From.Username} покинул игру", Bot);
                user.SetRoom(null);
            }
            catch (HttpRequestException ex)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, $"Не удалось выйти из игры {game.Name}: {ex.Message}",
                    ParseMode.Markdown);
            }
        }

        private async void DeleteGame(Message message, TelegramBotClient Bot, LocalUser user, GameRoomJson game)
        {
            bool done = service.Games.DeleteGameAsync(game.Id).Result;
            UserDatabase.Broadcast(u => game.Players.Select(s => s.UserId == u.User.Id).Any(),
                u => $"Администратор {game.Name} завершил игру", Bot);
            user.SetRoom(null);

            if (done)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, $"Игра *{game.Name}* успешно удалена!",
                    ParseMode.Markdown);
            }
            else
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, $"Ошибка при удалении игры :c\nПовторите попытку позже",
                    ParseMode.Markdown);
            }
            return;
        }

        public InGame()
        {
            LoadUserGames();
            AddInCommandCenter();
        }
        private  void LoadUserGames()
        {
            var games = service.Games.GetAllGamesAsync();

            foreach (var game in games.Result)
            {
                CommandsCenter.Add(new InlineButton($"{game.Name}| {game.ActiveRoles} |{game.Players.Count() +"/" + game.MaxPlayers}",
                    game.Id.ToString(), EnterRoomCallback));
            }

        }

        public async void EnterRoomCallback(Message message, TelegramBotClient Bot, object arg)
        {
            var game = await service.Games.GetGameAsync(Int64.Parse((arg as CallbackQuery).Data));

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Добро пожаловать в *{game.Name}*!");
            sb.AppendLine($"Активные роли: {game.ActiveRoles}");
            sb.AppendLine($"В игре: *{game.Players.Count()}/{game.MaxPlayers}* игроков");

            await CommandsCenter.GetMenu("ExitGameMenu").ShowAsync(message.Chat.Id, Bot, sb.ToString());

            Bot.ShowAnswerMessage((arg as CallbackQuery).Id,
                "Вы зашли на сервер. Однако, для презентации проекта время ограничено, поэтому кина не будет");
        }
    }
}
