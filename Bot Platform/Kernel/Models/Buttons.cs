using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineKeyboardButtons;
using TelegramBot.Kernel.Standart;
using TelegramBot.Kernel;
using TelegramBot.Kernel.Interfaces;

namespace TelegramBot.Kernel.Models
{
    public abstract class TelegramButton
    {
        public readonly Callback Callback;
        public readonly KeyboardButton Button;
        public readonly InlineKeyboardButton InlineButton;
        protected TelegramButton(KeyboardButton button, CallbackSignature callbackFunc)
        {
            Button = button;
            InlineButton = null;
            Callback = callbackFunc.AsReplyCallback();
        }


        protected TelegramButton(InlineKeyboardButton button, CallbackSignature callbackFunc)
        {
            InlineButton = button;
            Button = null;
            Callback = callbackFunc.AsInlineCallback(button.Text);
        }

        protected TelegramButton(KeyboardButton button, Callback callbackFunc)
        {
            Button = button;
            InlineButton = null;
            Callback = callbackFunc;
        }

        protected TelegramButton(InlineKeyboardButton button, Callback callbackFunc)
        {
            InlineButton = button;
            Button = null;
            Callback = callbackFunc;
        }

    }
    public class ReplyButton : TelegramButton
    {
        public new KeyboardButton Button
        {
            get => base.Button;
        }
        public ReplyButton(string buttonText, CallbackSignature callbackFunc)
            : base(new KeyboardButton(buttonText), callbackFunc)
        {
        }

        public ReplyButton(string buttonText, Callback callbackFunc)
            : base(new KeyboardButton(buttonText), callbackFunc)
        {
        }

        public ReplyButton GetCopy()
        {
            return new ReplyButton(this.Button.Text, Callback);
        }
        
    }

    public class InlineButton : TelegramButton
    {
        public new InlineKeyboardButton Button
        {
            get => base.InlineButton;
        }
        public readonly string CallbackName;

        public InlineButton(string buttonText, string callbackName, Callback callbackFunc)
            : base(InlineKeyboardButton.WithCallbackData(buttonText, callbackName), callbackFunc)
        {
            CallbackName = callbackName;
        }

        public InlineButton(string buttonText, string callbackName, CallbackSignature callbackFunc)
            : base(InlineKeyboardButton.WithCallbackData(buttonText, callbackName), callbackFunc)
        {
            CallbackName = callbackName;
        }


        public InlineButton GetCopy()
        {
            return new InlineButton(this.Button.Text, this.CallbackName, Callback);
        }

        public static implicit operator ReplyButton(InlineButton button)
        {
            return new ReplyButton(button.Button.Text, button.Callback);
        }

    }
}
