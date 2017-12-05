using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using MafiaGame.Structures.FileManager;
using Telegram.Bot.Types;
using TelegramBot.Kernel;
using TelegramBot.Kernel.CIO;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Models;
using TelegramBot.Kernel.Standart;
using TelegramBot.Kernel.System;

namespace TelegramBot.Modules
{
    class VoteSystem : ICommandsManager
    {
        private static SerializableDictionary<string, Vote> Votes;
        private static Dictionary<string, string> Buttons;

        static Mutex mutex = new Mutex();

        public static int Count
        {
            get => Votes.Count; 
        }
        public void AddInCommandCenter()
        {
        }

        public VoteSystem()
        {
            AddInCommandCenter();
            Read();
        }

        private void Read()
        {

            Votes = new SerializableDictionary<string, Vote>();
            Buttons = new Dictionary<string, string>();

            var xmlSer = new XmlSerializer(typeof(Vote));
            foreach (var file in new DirectoryInfo(Global.VOTES_PATH).GetFiles())
            {
                try
                {
                    using (var stream = file.OpenRead())
                    {
                        var vote = (Vote) xmlSer.Deserialize(stream);

                        Votes.Add(vote.Caption, vote);
                        CommandsCenter.Add(
                            new ReplyButton(vote.Caption, TelegramBot.Modules.Votes.NewVoteButtonCallback));

                        foreach (var buttonName in vote.Points.Keys)
                        {
                            CommandsCenter.Add(new ReplyButton(buttonName,
                                TelegramBot.Modules.Votes.VoteButtonCallback));
                            Buttons.Add(buttonName, vote.Caption);
                        }
                        int hash = vote.Caption.GetHashCode();
                        CommandsCenter.Add(new InlineButton("❌ Удалить", hash.ToString() + "delete",
                            TelegramBot.Modules.Votes.DeleteVote));
                        CommandsCenter.Add(new InlineButton("🔄 Ещё раз ", hash.ToString() + "retry",
                            TelegramBot.Modules.Votes.RetryVote));
                    }
                }
                catch (Exception ex)
                {
                    BotConsole.Write($"Ошибка при десериализации голосований {file.Name}: " + ex.Message,
                        MessageType.Error);
                }
            }
        }

        public static void Save()
        {
            try
            {

                var xmlSer = new XmlSerializer(typeof(Vote));
                int i = 1;
                foreach (var vote in Votes.Values)
                {
                    try
                    {
                        using (var writer =
                            new XmlTextWriter(new StreamWriter(Path.Combine(Global.VOTES_PATH, $"vote{i++}.txt"))))
                        {
                            xmlSer.Serialize(writer, vote);
                        }
                    }
                    catch (Exception ex)
                    {
                        BotConsole.Write("Ошибка при сохранении голосований: " + ex.Message, MessageType.Error);
                    }
                }

            }
            finally {mutex.ReleaseMutex();}
        }

        public static IEnumerable<string> GetVotesNames(string voteName = "")
        {
            try
            {
                mutex.WaitOne();

                if (String.IsNullOrEmpty(voteName))
                {
                    return Votes.Keys;
                }
                else
                {
                    return new[] {Votes[voteName].ToString()};
                }
            }
            finally { mutex.ReleaseMutex();}

        }

        public static Vote GetVote(string voteName)
        {
            try
            {
                mutex.WaitOne();
                return Votes[voteName];
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public static Vote GetVote(int hash)
        {
            return Votes.Where(vote => vote.Value.Caption.GetHashCode() == hash).Select(v => v.Value).First();
        }

        public static KeyboardButton[][] AddNewVote(string caption, string[] points)
        {
            try
            {
                mutex.WaitOne();

                Votes.Add(caption, new Vote(caption, points));

                KeyboardButton[][] keyb = new KeyboardButton[points.Length + 1][];

                int hash = caption.GetHashCode();
                CommandsCenter.Add(new InlineButton("❌ Удалить", hash.ToString() + "delete", TelegramBot.Modules.Votes.DeleteVote));
                CommandsCenter.Add(new InlineButton("🔄 Ещё раз ", hash.ToString() + "retry", TelegramBot.Modules.Votes.RetryVote));

                for (int i = 0; i < points.Length; i++)
                {
                    string text = points[i] = "• " + points[i].Trim();
                    if (CommandsCenter.GetReplyButton(text) != null)
                        throw new Exception(
                            $"кнопка {text} • уже добавлена или зарезервирована ботом. Попробуйте изменить её название.");

                    var button = CommandsCenter.Add(new ReplyButton(text, TelegramBot.Modules.Votes.VoteButtonCallback));
                    keyb[i] = new KeyboardButton[]
                    {
                        button.Button
                    };

                    Buttons.Add(points[i], caption);
                }

                keyb[points.Length] = new KeyboardButton[] {new KeyboardButton("[Пропустить]")};
                return keyb;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
        public static void AddUserVote(string buttonName, long userId)
        {
            try
            {
                mutex.WaitOne();

                Vote vote = Votes[Buttons[buttonName]];
                if (!vote.UserIsVoted(userId))
                {
                    vote.AddVote(buttonName, userId);
                }
                else throw new ArgumentException("*Вы уже голосовали!*");

            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public static void RemoveVote(int hash)
        {
            try
            {
                mutex.WaitOne();

                var Vote = Votes.Where(vote => vote.Value.Caption.GetHashCode() == hash).Select(v => v.Value).First();

                foreach (var point in Vote.Points.Keys)
                {
                    CommandsCenter.RemoveReplyButton(point);
                }
                CommandsCenter.RemoveReplyButton(Vote.Caption);
                CommandsCenter.RemoveInlineButton(hash.ToString() + "delete");
                CommandsCenter.RemoveInlineButton(hash.ToString() + "retry");

                Directory.Delete(Global.VOTES_PATH, true);
                Directory.CreateDirectory(Global.VOTES_PATH);
                Votes.Remove(Vote.Caption);
                Save();
            }
            finally
            {
                if (!Directory.Exists(Global.VOTES_PATH))
                {
                    Directory.CreateDirectory(Global.VOTES_PATH);
                    Save();
                }
                mutex.ReleaseMutex();
            }
        } 

    }

    [Serializable]
    public class Vote
    {
        public int VotesCount { get; set; }
        public string Caption { get; set; }
        public HashSet<long> Ids;
        public SerializableDictionary<string, int> Points;

        public Vote() { }
        public Vote(string caption, string[] points)
        {
            Caption = caption;
            VotesCount = 0;
            Points = new SerializableDictionary<string, int>();
            Ids = new HashSet<long>();

            for (int i =0 ; i <points.Length; i++)
            {
                Points.Add("• " + points[i].Trim(), 0);
            }

        }

        public bool UserIsVoted(long userId) => Ids.Contains(userId);

        public void AddVote(string point, long id)
        {
            ++Points[point];
            ++VotesCount;
            Ids.Add(id);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Тема: *{Caption}*\n");

            sb.AppendLine($"Всего `{VotesCount}` голосов:");

            foreach (var value in Points)
            {
                double count = Points[value.Key];
                sb.AppendLine(value.Key + " - *" + count + " голосов* = `" + ((count * 100) / (VotesCount == 0? 1 : VotesCount)).ToString() + "%`");
            }

            return sb.ToString();
        }
    }
}
