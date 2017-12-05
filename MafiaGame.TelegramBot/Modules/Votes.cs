using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using TelegramBot.Kernel;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Models;
using TelegramBot.Kernel.Standart;
using TelegramBot.Kernel.System;

namespace TelegramBot.Modules
{
    class Votes: ICommandsManager
    {
        public void AddInCommandCenter()
        {
            CommandsCenter.Add(new ReplyButton("/vote", async (message, bot, arg) =>
            {
                string command = "/vote";
                if ((message.Chat.Id == 422672483))
                {
                    try
                    {
                        if (message.Text.Trim() == command)
                        {
                            GetVote(message, bot);
                            return;
                        }
                        AddVote(message, bot);

                    }
                    catch (Exception ex)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id,
                            $"*Не удалось начать голосование:* `{ex.Message}`", ParseMode.Markdown);
                    }
                }
            }));
        }

        private void AddVote(Message message, TelegramBotClient bot)
        {
            string[] args = message.Text.Replace("/vote", "").Trim()
                .Split(new[] {'!'}, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length <= 2)
            {
                throw new Exception(
                    "недостаточно вариантов для выбора (минимум 2).");
            }

            args[0] = args[0].Trim();

            var keyb = VoteSystem.AddNewVote(args[0], args.Where((s, i) => i > 0).ToArray());
            CommandsCenter.Add(new ReplyButton(args[0], NewVoteButtonCallback));

            UserDatabase.Broadcast(user => true,
                user => "*Опрос пользователей:*\n\n" + args[0].Trim(), bot,
                (new ReplyMenu("", true, keyb)).Keyboard);
        }

        private static async void GetVote(Message message, TelegramBotClient bot)
        {
            try
            {
                var votes = VoteSystem.GetVotesNames();
                if (!votes.Any())
                {
                    await bot.SendTextMessageAsync(message.Chat.Id,
                        $"*Нет открытых голосований*", ParseMode.Markdown, false, false, 0,
                        CommandsCenter.GetMenu("StartMenu").Keyboard);
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
                    new ReplyMenu("", true, keyboard).Keyboard);
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
                var menu = new InlineMenu("", new InlineKeyboardButton[][]
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

        public Votes()
        {
            AddInCommandCenter();
        }

        public static async void VoteButtonCallback(Message message, TelegramBotClient Bot, object arg)
        {
            try
            {
                VoteSystem.AddUserVote(message.Text, message.Chat.Id);
                await CommandsCenter.GetMenu("StartMenu").ShowAsync(message.Chat.Id, Bot,
                    "*Спасибо, Ваш голос был учтён!*");
            }
            catch (ArgumentException ex)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, ex.Message, ParseMode.Markdown, false, false, 0,
                    CommandsCenter.GetMenu("StartMenu").Keyboard);
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

                UserDatabase.Broadcast(u => !vote.UserIsVoted(u.User.Id), user => "*Опрос пользователей:*\n\n" + vote.Caption.Trim(), bot,
                    (new ReplyMenu("", true, keyb)).Keyboard);
                await bot.SendTextMessageAsync(message.Chat.Id, "Опрос отправлен всем непроголосовавшим пользователям", ParseMode.Markdown);
            }
            catch
            {
                await bot.SendTextMessageAsync(message.Chat.Id,
                    "*Опрос закрыт или не создан*");
            }
        }
    }
}