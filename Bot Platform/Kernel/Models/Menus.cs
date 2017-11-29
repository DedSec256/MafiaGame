using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using TelegramBot.Kernel.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Kernel.CIO;
using TelegramBot.Kernel.Standart;

namespace TelegramBot.Kernel.Models
{
    public abstract class BotMenu : IMenu
    {
        public readonly string Name;
        public readonly string Info;

        public IReplyMarkup Keyboard { get; protected set; }
        protected bool ResizeKeyboard;

        protected BotMenu(string name, IReplyMarkup keyboard, bool resKeyb, string info)
        {
            Name = name;
            Keyboard = keyboard;
            ResizeKeyboard = resKeyb;
            Info = info;
        }

        public void AddInCommandCenter()
        {
            CommandsCenter.Add(this);
        }

        public virtual Task<Message> ShowAsync(long chatId, TelegramBotClient Bot, string text = "...",
            bool showInfo = false, int? messageId = null)
        {
            text = String.IsNullOrEmpty(text) ? "..." : text;
            if (showInfo && !String.IsNullOrEmpty(Info)) text = Info + (text == "..." ? "" : "\n\n" + text);

            if (messageId.HasValue)
            {
                return  Bot.EditMessageTextAsync(chatId, messageId.Value, text,
                    ParseMode.Markdown, false, Keyboard);
            }
            return  Bot.SendTextMessageAsync(chatId, text, ParseMode.Markdown, false, false, 0, Keyboard);
        }

        public abstract BotMenu GetCopy();

    }

    public class ReplyMenu : BotMenu
    {
        public ReplyMenu(string name, bool resizeKeyboard, KeyboardButton[][] buttons, string info = null) 
            : base(name, new ReplyKeyboardMarkup(buttons, resizeKeyboard), resizeKeyboard, info)
        {
        }

        public KeyboardButton this[int i, int j]
        {
            get => ((ReplyKeyboardMarkup)Keyboard).Keyboard[i][j];
            set
            {
                var keyboard = ((ReplyKeyboardMarkup)Keyboard).Keyboard;
                keyboard[i][j] = value;
                ChangeMenu(keyboard);
            }
        }
        public void ChangeMenu(KeyboardButton[][] buttons)
        {
            Keyboard = new ReplyKeyboardMarkup(buttons, ResizeKeyboard);
        }
        public KeyboardButton[] this[int i]
        {
            get => ((ReplyKeyboardMarkup)Keyboard).Keyboard[i];
            set
            {
                var keyboard = ((ReplyKeyboardMarkup)Keyboard).Keyboard;
                keyboard[i] = value;
                ChangeMenu(keyboard);
            }
        }
        public override BotMenu GetCopy()
        {
            return new ReplyMenu(Name, ResizeKeyboard, ((ReplyKeyboardMarkup)Keyboard).Keyboard.GetCopy());
        }

    }

    public class InlineMenu : BotMenu
    {
        public InlineMenu(string name,
             InlineKeyboardButton[][] buttons, string info = null)
            : base(name, new InlineKeyboardMarkup(buttons), false, info)
        {
        }

        public void ChangeMenu(InlineKeyboardButton[][] buttons)
        {
            Keyboard = new InlineKeyboardMarkup(buttons);
        }

        public InlineKeyboardButton this[int i, int j]
        {
            get { return ((InlineKeyboardMarkup)Keyboard).InlineKeyboard[i][j]; }
            set
            {
                var keyboard = ((InlineKeyboardMarkup)Keyboard).InlineKeyboard;
                keyboard[i][j] = value;
                ChangeMenu(keyboard);
            }
        }

        public InlineKeyboardButton[] this[int i]
        {
            get { return ((InlineKeyboardMarkup)Keyboard).InlineKeyboard[i]; }
            set
            {
                var keyboard = ((InlineKeyboardMarkup)Keyboard).InlineKeyboard;
                keyboard[i] = value;
                ChangeMenu(keyboard);
            }
        }


        public override BotMenu GetCopy()
        {
            return new InlineMenu(this.Name, ((InlineKeyboardMarkup)Keyboard).InlineKeyboard.GetCopy());
        }
        
    }

    public class MenuBlock : IEnumerator<BotMenu>, IEnumerable<BotMenu>
    {
        public readonly string Name;
        private BotMenu[] Menus;
        private int Position;

        public int Length
        {
            get => Menus.Length;
        }
        public MenuBlock(BotMenu[] menus, string name)
        {
            Menus = menus;
            Position = 0;
            Name = name;
        }

        public MenuBlock(BotMenu[][] menus, string name) : this(menus.Sum(), name)
        {
        }

        public void Dispose()
        {
            Menus = null;
            Reset();
        }

        public bool MoveNext()
        {
            if (Position < Menus.Length - 1)
            {
                ++Position;
                return true;
            }
            return false;
        }


        public bool MoveLast()
        {
            if (Position > 0)
            {
                --Position;
                return true;
            }
            return false;
        }

        public BotMenu ToLast()
        {
            if (Position > 0)
            {
                --Position;
            }
            return Menus[Position];
        }

        public BotMenu ToNext()
        {
            if (Position < Menus.Length - 1)
            {
                ++Position;
            }
            return Menus[Position];
        }

        public void Reset()
        {
            Position = 0;
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public BotMenu Current
        {
            get => Menus[Position];
            set => Menus[Position] = value;
        }

        public IEnumerator<BotMenu> GetEnumerator()
        {
            return (IEnumerator<BotMenu>)this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public BotMenu this[int i]
        {
            get => Menus[i];
            set => Menus[i] = value;
        }

        public MenuBlock GetCopy()
        {
            BotMenu[] resMenus = new BotMenu[Menus.Length];
            for (int i = 0; i < Menus.Length; i++)
            {
                resMenus[i] = Menus[i].GetCopy();
            }

            return new MenuBlock(resMenus, Name);
        }
    }

}
