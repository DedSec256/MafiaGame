using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mafiagame.DataLayer.Models;
using MafiaGame.DataLayer.Models;

namespace Mafiagame.DataLayer.Interfaces
{
    public interface IPlayersGamesRepository
    { 
        PlayerGame Create(PlayerGame game);
        void Delete(long userId);
        PlayerGame Get(long userId);
        IEnumerable<PlayerGame> GetAll();
        void UpdatePlayerGame(PlayerGame game);
    }
}
