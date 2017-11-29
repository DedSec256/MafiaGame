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
        User UpdateUser(User user);
    }
}
