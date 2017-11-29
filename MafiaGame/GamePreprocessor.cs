using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaGame.DataLayer.Models;

namespace MafiaGame
{
    public class GamePreprocessor
    {
        private User[] _players;
        public GamePreprocessor(User[] players)
        {
            _players = players;
        }

        public GamePreprocessor(IEnumerable<User> arr) : this(arr.ToArray())
        {
            
        }

        public IEnumerable<Role> InitializeRoles()
        {
            return null;
        }
    }
}
