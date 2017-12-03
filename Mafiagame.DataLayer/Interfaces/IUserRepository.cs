using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaGame.DataLayer.Models;

namespace MafiaGame.DataLayer.Interfaces
{
    public interface IUserRepository
    {
        User Create(long userId);
        void Delete(long userId);
        User Get(long userId);
        IEnumerable<User> GetAll();

        /* Пользователь вышел из игры */
        void UserExitGameHandler(object sender, long userId);
        /* Пользователь вошёл в игровую комнату */
        void UserEnterGameHandler(object sender, (long userId, long gameId) args);
    }
}
