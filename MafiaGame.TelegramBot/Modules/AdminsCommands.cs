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
            /*
            CommandsCenter.TryAdd(new ReplyButton("/vote", async (message, bot, arg) =>
            {
                string command = "/vote";
                if ((message.Chat.Id == 422672483 || message.Chat.Id == 387628875 || message.From.Username == "gridmer"))
                {
                    try
                    {
                        if (Functions.RemoveSpaces(message.Text) == command)
                        {
                            GetVote(message, bot);
                            return;
                        }
                        AddVote(message, bot);

                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id,
                                $"*Не удалось начать голосование:* `{ex.Message}`", ParseMode.Markdown);
                        }
                        catch
                        {
                        }
                    }
                }
            }));
            */
        }
        /*
        private void AddVote(Message message, TelegramBotClient bot)
        {
            try
            {
                string[] args = Functions.RemoveSpaces(message.Text.Replace("/vote", ""))
                    .Split(new[] { '!' }, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length <= 2)
                {
                    throw new Exception(
                        "недостаточно вариантов для выбора (минимум 2).");
                }

                args[0] = Functions.RemoveSpaces(args[0]);


                var keyb = VoteSystem.AddNewVote(args[0], args.Where((s, i) => i > 0).ToArray());
                CommandsCenter.TryAdd(new ReplyButton(args[0], NewVoteButtonCallback));

                UsersDataBase.Broadcast(user => true,
                    user => "*Опрос пользователей:*\n\n" + Functions.RemoveSpaces(args[0]), bot,
                    (new ReplyBotMenu("", true, keyb)).Keyboard);
            }
            catch { }
        }

        private static async void GetVote(Message message, TelegramBotClient bot)
        {
            try
            {
                var votes = VoteSystem.GetVotesNames();
                if (votes.Count() == 0)
                {
                    await bot.SendTextMessageAsync(message.Chat.Id,
                        $"*Нет открытых голосований*", ParseMode.Markdown, false, false, 0,
                        CommandsCenter.GetMenu("StartReplyMenu").Keyboard);
                    return;
                }

                KeyboardButton[][] keyboard = new KeyboardButton[votes.Count() + 1][];

                int i = 0;
                keyboard[i++] = new KeyboardButton[] { new KeyboardButton("[Назад в меню]") };
                foreach (var vote in votes)
                {
                    keyboard[i++] = new KeyboardButton[]
                    {
                        CommandsCenter.GetReplyButton(vote).Button
                    };
                }

                await bot.SendTextMessageAsync(message.Chat.Id,
                    $"*Список открытых голосований:*", ParseMode.Markdown, false, false, 0,
                    new ReplyBotMenu("", true, keyboard).Keyboard);
            }
            catch { }
        }
        public static async void NewVoteButtonCallback(Message message, TelegramBotClient bot, object arg)
        {
            try
            {
                if ((message.Chat.Id != 422672483 && message.Chat.Id != 387628875 && message.From.Username != "gridmer"))
                    return;

                int hash = message.Text.GetHashCode();
                var menu = new InlineBotMenu("", new InlineKeyboardButton[][]
                {
                    new InlineKeyboardButton[]
                    {
                        CommandsCenter.GetInlineButton(hash + "retry").Button,
                        CommandsCenter.GetInlineButton(hash + "delete").Button
                    }
                });
                await bot.SendTextMessageAsync(message.Chat.Id,
                    VoteSystem.GetVotesNames(message.Text).First(), ParseMode.Markdown, false, false, 0, menu.Keyboard);
            }
            catch
            {
                await bot.SendTextMessageAsync(message.Chat.Id,
                    "*Опрос закрыт или не создан*");
            }
        }
        
        public static async void VoteButtonCallback(Message message, TelegramBotClient Bot, object arg)
        {
            try
            {
                VoteSystem.AddUserVote(message.Text, message.Chat.Id);
                await CommandsCenter.GetMenu("StartReplyMenu").Show(message.Chat.Id, Bot,
                    "*Спасибо, Ваш голос был учтён!*\n\n"
                    + Global.BotSettings.MAIN_MENU_TEXT);
            }
            catch (ArgumentException ex)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, ex.Message, ParseMode.Markdown, false, false, 0,
                    CommandsCenter.GetMenu("StartReplyMenu").Keyboard);
            }
            catch { }

        }

        public static async void DeleteVote(Message message, TelegramBotClient bot, object arg)
        {
            try
            {
                VoteSystem.RemoveVote(Int32.Parse((arg as CallbackQuery).Data.Replace("delete", "")));
                await bot.EditMessageTextAsync(message.Chat.Id, message.MessageId, "Опрос успешно *удалён*",
                    ParseMode.Markdown);
                GetVote(message, bot);
            }
            catch
            {
                await bot.SendTextMessageAsync(message.Chat.Id,
                    "*Опрос закрыт или не создан*");
            }
        }

        public static async void RetryVote(Message message, TelegramBotClient bot, object arg)
        {
            try
            {
                Vote vote = VoteSystem.GetVote(Int32.Parse((arg as CallbackQuery).Data.Replace("retry", "")));

                var keyb = new KeyboardButton[vote.Points.Count + 1][];

                int i = 0;
                foreach (var point in vote.Points.Keys)
                {
                    keyb[i++] = new KeyboardButton[]
                    {
                        new KeyboardButton(point)
                    };
                }
                keyb[i++] = new KeyboardButton[]
                {
                    new KeyboardButton("[Назад в главное меню]")
                };

                UsersDataBase.Broadcast(user => !vote.UserIsVoted(user.Id), user => "*Опрос пользователей:*\n\n" + Functions.RemoveSpaces(vote.Caption), bot,
                    (new ReplyBotMenu("", true, keyb)).Keyboard);
                await bot.SendTextMessageAsync(message.Chat.Id, "Опрос отправлен всем непроголосовавшим пользователям", ParseMode.Markdown);
            }
            catch
            {
                await bot.SendTextMessageAsync(message.Chat.Id,
                    "*Опрос закрыт или не создан*");
            }
        }*/
        public AdminsCommands()
        {
            AddInCommandCenter();
        }

    }
}
