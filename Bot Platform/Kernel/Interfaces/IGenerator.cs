using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Kernel.Models;

namespace TelegramBot.Kernel.Interfaces
{
    interface IGenerator
    {
        BotMenu GenerateMenu();
    }
}
