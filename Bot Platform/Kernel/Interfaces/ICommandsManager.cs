using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Kernel.Interfaces
{
    public interface ICommandsManager
    {
        void AddInCommandCenter();
    }
    public interface IMenu: ICommandsManager
    {
        Task<Message> ShowAsync(long chatId, TelegramBotClient Bot, string text, 
                           bool showInfo = false, int ? messageId = null);
    }
}
