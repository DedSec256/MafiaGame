using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mafiagame.DataLayer.Models;
using MafiaGame.DataLayer.Models;

namespace MafiaGame.DataLayer.Interfaces
{
    public interface IGameRoomRepository
    {
        GameRoom Create(GameRoom room);
        void Delete(long gameId);
        GameRoom Get(long gameId);
        IEnumerable<GameRoom> GetAll();
        GameRoom UpdateGameRoom(GameRoom gameRoom);
        void AddPlayersGames(IEnumerable<Role> players, long gameId);
        void UpdatePlayersGames(IEnumerable<Role> players, long gameId);
    }
}
