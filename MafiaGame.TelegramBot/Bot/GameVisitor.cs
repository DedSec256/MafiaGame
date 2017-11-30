using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Bot
{

    public interface IVisitor<T>
    {
        void Visit(T obj);
    }

    public enum GameEvents
    {
        PlayerExit
    }

    public class GameEventArgs : EventArgs
    {
        
    }
    /*
    class GameVisitor : IVisitor<GameRoom>
    {

        public GameVisitor(GameEvents ev)
        {
            
        }
        public void Visit(GameRoom obj)
        {
            obj.Accept(this);
        }
    }
    */
}
