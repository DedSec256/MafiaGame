using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelegramBot.Kernel.CIO;

namespace TelegramBot.Bot
{
    public class BotSettings
    {
        public string TOKEN { get; private set; }

        private const char COMMENTS = '$';
        public static BotSettings ReadFrom(string filename)
        {
            StringBuilder dataLine = new StringBuilder();
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string tempText = reader.ReadToEnd();

                    #region поиск комментариев

                    bool findComments = false;
                    for (int i = 0; i < tempText.Length; i++)
                    {
                        if (tempText[i] == '\r' ||
                            tempText[i] == '\n' ||
                            tempText[i] == '\t') continue;

                        if (tempText[i] == COMMENTS)
                        {
                            findComments = !findComments;
                            continue;
                        }
                        if (findComments == true) continue;

                        dataLine.Append(tempText[i]);
                    }

                    #endregion
                }
                string[] KEYS =
                {
                    "[TOKEN]"
                };

                string[] param =
                    dataLine.ToString().Split(KEYS, StringSplitOptions.RemoveEmptyEntries); //switch to remove
                string token = param[0].Trim();

                return new BotSettings()
                {
                    TOKEN = token
                };
            }
            catch (Exception ex)
            {
                BotConsole.Write("Ошибка при чтении настроек бота:\n" + ex.Message,
                    MessageType.Error);
                Thread.Sleep(5000);
                Environment.Exit(1);
            }
            return null;
        }
    }
}
