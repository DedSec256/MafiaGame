using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Kernel.Models;
using TelegramBot.Kernel.Standart;

namespace TelegramBot.Kernel.Interfaces
{
    public static class Extensions
    {
        public static T[] Sum<T>(this T[][] obj)
        {
            int size = 0;
            for (int i = 0; i < obj.Length; i++)
            {
                size += obj[i].Length;
            }

            T[] res = new T[size];

            for (int i = 0, index = 0; i < obj.Length; i++)
            {
                for (int j = 0; j < obj[i].Length; j++, ++index)
                {
                    res[index] = obj[i][j];
                }
            }
            return res;
        }
        public static InlineKeyboardButton[][] Union(this InlineKeyboardButton[][] arr,
            InlineKeyboardButton[] line)
        {
            InlineKeyboardButton[][] res = new InlineKeyboardButton[arr.Length + 1][];
            for(int i=0; i<arr.Length; i++)
            {
                res[i] = arr[i];
            }
            res[arr.Length] = line;
            return res;
        }

        public static InlineKeyboardButton[][] Union(this InlineKeyboardButton[] line, 
            InlineKeyboardButton[][] arr)
        {
            InlineKeyboardButton[][] res = new InlineKeyboardButton[arr.Length + 1][];
            res[0] = line;
            for (int i = 1; i <= arr.Length; i++)
            {
                res[i] = arr[i];
            }
            
            return res;
        }

        public static InlineKeyboardButton[][] GetCopy(this InlineKeyboardButton[][] arr)
        {
            InlineKeyboardButton[][] res = new InlineKeyboardButton[arr.Length][];

            for (int i = 0; i < arr.Length; i++)
            {
                res[i] = new InlineKeyboardButton[arr[i].Length];
                for (int j = 0; j < arr[i].Length; j++)
                {
                    var callButt = arr[i][j] as InlineKeyboardCallbackButton;
                    var urlButt = arr[i][j] as InlineKeyboardUrlButton;

                    if (callButt != null)
                    {
                        res[i][j] = new InlineKeyboardCallbackButton(callButt.Text, callButt.CallbackData);
                    }
                    else
                    {
                        //res[i][j] = new InlineKeyboardUrlButton(callButt.Text, (callButt as InlineKeyboardUrlButton).Url);
                    }
                }
            }

            return res;
        }

        public static KeyboardButton[][] GetCopy(this KeyboardButton[][] arr)
        {
            KeyboardButton[][] res = new KeyboardButton[arr.Length][];

            for (int i = 0; i < arr.Length; i++)
            {
                res[i] = new KeyboardButton[arr[i].Length];
                for (int j = 0; j < arr[i].Length; j++)
                {

                    res[i][j] = new KeyboardButton(arr[i][j].Text);

                }
            }

            return res;
        }

        public static (int x, int y) GetButton(this InlineMenu menu, string callName)
        {
            for (int i = 0; i < ((InlineKeyboardMarkup)menu.Keyboard).InlineKeyboard.Length; i++)
            {
                for (int j = 0; j < menu[i].Length; j++)
                {
                    if ((menu[i, j] as InlineKeyboardCallbackButton).CallbackData == callName)
                    {
                        return (i, j);
                    }
                }
            }
            return (-1, -1);
        }
        public static (int x, int y, int z) GetInlineButton(this MenuBlock block, string callName)
        {
            for (int k = 0; k < block.Length; k++)
            {
                InlineMenu menu = block[k] as InlineMenu;
                var pos = menu.GetButton(callName);
                if (pos.x != -1 && pos.y != -1)
                {
                    return (k, pos.x, pos.y);
                }
            }
            return (-1, -1, -1);
        }
    }
    public static class Functions
    {  
        public static double ToDouble(this string text)
        {
            return Double.Parse(text.Replace(',', '.'), CultureInfo.InvariantCulture);
        }

        public static void ShowAnswerMessage(this TelegramBotClient Bot, string queryId, string message)
        {
            CommandsCenter.TryInlineCommand("callbackQueryAnswer", new Message(){Text = message}, Bot, queryId);
        }

        public static ReplyCallback AsReplyCallback(this CallbackSignature callback)
        {
            return new ReplyCallback(callback);
        }
        public static InlineCallback AsInlineCallback(this CallbackSignature callback, string butName)
        {
            return new InlineCallback(callback, butName);
        }
    }

    
}
