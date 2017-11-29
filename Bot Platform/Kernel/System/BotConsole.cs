using System;
using System.Runtime.InteropServices;
using TelegramBot.Kernel.Interfaces;
using TelegramBot.Kernel.Standart;

namespace TelegramBot.Kernel.CIO /* CIO - Console In-Out */
{
    public enum  MessageType 
    {
        Error,
        Info,
        Warning,
        System,
        Default
    }

    public static class BotConsole
    {
        private static bool alwaysRead;
        private static Log log = new Log();
        static BotConsole()
        {
            Writer = null;
            Reader = null;
            Notifyer = null;
        }
/*
        public static void Read()
        {
            void ExecuteCommand(string newCommand)
            {
                try
                {
                    /*
                    CommandInfo info = GetCommandFromMessage(newCommand);
                    if (!String.IsNullOrEmpty(info.Callback) ||
                        !String.IsNullOrEmpty(info.Param))/
                    
                }
                catch (Exception ex)
                {
                    BotConsole.Write("[ERROR][SYSTEM " + DateTime.Now.ToLongTimeString() + "]:\n" + ex.Message + "\n",
                        MessageType.Error);
                }
            }

            if (alwaysRead)
                while (true)
                {
                    ExecuteCommand(Reader());
                }
            else ExecuteCommand(Reader());
        }
*/

        private static Action<string, MessageType> Writer;
        private static Action<string, string> Notifyer;
        private static Func<string> Reader;

        public static void SetWriter(Action<string, MessageType> _writer)
        {
            Writer = _writer;
        }

        public static void SetReader(Func<string> _reader, bool _alwaysRead = false)
        {
            Reader = _reader;
            alwaysRead = _alwaysRead;
        }

        public static void SetNotifyer(Action<string, string> _notifyer)
        {
            Notifyer = _notifyer;
        }

        private static object lockObj = new object();
        public static void Write(string text, MessageType type = MessageType.Default)
        {
            //lock(lockObj) Writer(text, type);
            Writer(text, type);
            log.Write(text);
        }

        public static void Write(Exception ex)
        {
            //lock(lockObj) Writer(text, type);
            Writer(ex.Message, MessageType.Error);
            log.Write(ex);
        }

        internal static void StartReading()
        {
            //if(alwaysRead) Read();
            Reader();
        }

        public static void Notify(string caption, string text)
        {
            Notifyer(caption, text);
        }
        public struct CommandInfo
        {
            public string Command { get; private set; }
            public string Param { get; private set; }

            public CommandInfo(string command, string param)
            {
                this.Command = command;
                this.Param = param;
            }
        }
        
        /*
        static CommandInfo GetCommandFromMessage(string consoleMessage)
        {
            int index = consoleMessage.IndexOf('(');

            string command = consoleMessage;
            string param = null;

            if (index != -1)
            {
                command = consoleMessage.Substring(0, index);
                param = consoleMessage.Substring(index + 1);
                int ind = param.LastIndexOf(')');
                if (ind == -1)
                {
                    param += ')';
                    param = param.Remove(param.Length - 1);
                }
                else param = param.Remove(ind);
            }
            command = command.Trim();

            if (!String.IsNullOrEmpty(command) &&
                Char.IsUpper(command[0])) command = Char.ToLower(command[0]) + command.Substring(1);
            return new CommandInfo(command, param);
        }
        */
    }

}
