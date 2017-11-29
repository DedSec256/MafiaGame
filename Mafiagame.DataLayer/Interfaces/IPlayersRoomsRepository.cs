using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mafiagame.DataLayer.Models;
using MafiaGame.DataLayer.Models;

namespace Mafiagame.DataLayer.Interfaces
{
    public interface IPlayersRoomsRepository
    { 
        PlayersRooms Create(User user, string role);
        void Delete(long userId);
        PlayersRooms Get(long userId);
        IEnumerable<PlayersRooms> GetAll();
        PlayersRooms UpdatePlayerRoom(PlayersRooms room);
    }
}
