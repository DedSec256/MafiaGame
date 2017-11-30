using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using Telegram.Bot;
using TelegramBot.Kernel;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineKeyboardButtons;
using TelegramBot.Kernel.CIO;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Models;
using TelegramBot.Modules;

namespace TelegramBot.Kernel.Standart
{
    

    public static partial class CommandsCenter
    {

        private static SortedDictionary<string, ReplyButton>  ReplyButtons =
                new SortedDictionary<string, ReplyButton>();
        private static SortedDictionary<string, InlineButton> InlineButtons =
                new SortedDictionary<string, InlineButton>();
        private static SortedDictionary<string, BotMenu>      Menus =
                new SortedDictionary<string, BotMenu>();

        public static ReplyButton GetReplyButton(string text)
        {
            return
            ReplyButtons.ContainsKey(text) ? ReplyButtons[text] : null;
        }
        public static InlineButton GetInlineButton(string callbackText)
        {
            return
                InlineButtons.ContainsKey(callbackText) ? InlineButtons[callbackText] : null;
        }

        public static bool RemoveReplyButton(string text)
        {
            if(ReplyButtons.ContainsKey(text))
            {
                ReplyButtons.Remove(text);
                return true;
            }
            return false;
        }
        public static bool RemoveInlineButton(string text)
        {
            if (InlineButtons.ContainsKey(text))
            {
                InlineButtons.Remove(text);
                return true;
            }
            return false;
        }

        public static ReplyButton Add(ReplyButton rb)
        {
            if (!ReplyButtons.ContainsKey(rb.Button.Text))
            {
                ReplyButtons.Add(rb.Button.Text, rb);
            }
            else
                throw new InvalidOperationException($"Не удалось добавить ReplyButton [{rb.Button.Text}].\n" +
                                                    "ReplyButton с таким же названием уже была добавлена.\n");
            return rb;
        }
        public static ReplyButton Add(string command, CallbackSignature callback)
        {
            if (!ReplyButtons.ContainsKey(command))
            {
                ReplyButton rb = new ReplyButton(command, callback.AsReplyCallback());
                ReplyButtons.Add(command, rb);
                return rb;
            }
            else
                throw new InvalidOperationException($"Не удалось добавить команду {command}.\n" +
                                                    "Команда с таким же названием уже была добавлена.\n");
        }
        public static InlineButton Add(InlineButton button)
        {
            if (!InlineButtons.ContainsKey(button.CallbackName))
            {
                InlineButtons.Add(button.CallbackName, button);
            }
                
            return button;
        }

        public static BotMenu Add(BotMenu botMenu)
        {

            if (!Menus.ContainsKey(botMenu.Name))
            {
                Menus.Add(botMenu.Name, botMenu);
            }
            else
                throw new InvalidOperationException($"Не удалось добавить BotMenu [{botMenu.Name}].\n" +
                                                    "BotMenu с таким же названием уже была добавлена.\n");
            return botMenu;
        }
        public static BotMenu GetMenu(string title) => Menus[title];

        public static async void TryReplyCommand(Message message, TelegramBotClient bot, object arg = null)
        {
            try
            {
                if (ReplyButtons.TryGetValue(message.Text, out ReplyButton button))
                {
                    button.Callback.Execute(message, bot, arg);
                }
                else
                {
                    await CommandsCenter.GetMenu("StartMenu")
                        .ShowAsync(message.Chat.Id, bot, Global.Settings.MAIN_MENU_TEXT);
                }
            }
            catch
            {
                //TODO:
            }
        }
        public static void TryInlineCommand(string callback, Message message, TelegramBotClient bot, object arg = null)
        {
            if (InlineButtons.TryGetValue(callback, out InlineButton button))
            {
                button.Callback.Execute(message, bot, arg);
            }
            else
            {
                //TODO: throw?
            }
        }

    }
}