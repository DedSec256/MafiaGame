using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mafiagame.DataLayer.Models;
using System.Data.Linq;
using MafiaGame.DataLayer.Interfaces;
using MafiaGame.DataLayer.Models;

namespace MafiaGame.DataLayer.Implementations
{
    public class GameRoomRepository : IGameRoomRepository
    {
        private readonly string _connectionString;

        public GameRoomRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public GameRoom Create(GameRoom room)
        {
            using (var db = new DataContext(_connectionString))
            {
                var checkGame = (from g in db.GetTable<GameRoom>()
                                 where g.Id == room.Id select g).FirstOrDefault();

                if (checkGame != default(GameRoom))
                {
                    throw new ArgumentException($"Игра уже была создана. ");
                }

                AddPlayersGames(room.Players, room.Id);

                db.GetTable<GameRoom>().InsertOnSubmit(room);
                db.SubmitChanges();
                return room;
            }
        }

        public IEnumerable<GameRoom> GetAll()
        {
            using (var db = new DataContext(_connectionString))
            {
                return db.GetTable<GameRoom>().Select(g => g).ToArray();//Select((g) =>
                //{
                   // var game = g;
                   // game.Players = db.GetTable<PlayersRooms>().Where(r => r.GameId == g.Id).ToArray()
                   //                            .Select(r => RoleFactory.GetRole(r));
                   // return game;
                    
               // }); 
            }
        }

        public void Delete(long gameId)
        {
        }

        public GameRoom Get(long gameId)
        {
            using (var db = new DataContext(_connectionString))
            {

                var game = (from g in db.GetTable<GameRoom>()
                    where g.Id == gameId
                    select g).FirstOrDefault();

                if (game == default(GameRoom))
                    throw new ArgumentException($"Игровой стол с id {gameId} не найден");

                return game;
            }
        }

        public GameRoom UpdateGameRoom(GameRoom gameRoom)
        {
            throw new NotImplementedException();
        }

        private void RepositoryGameRoomContent(GameRoom sourceGame, GameRoom destinationGame)
        {
            destinationGame.ActiveRoles = sourceGame.ActiveRoles;
            destinationGame.MaxPlayers = sourceGame.MaxPlayers;
            destinationGame.Name = sourceGame.Name;
            destinationGame.Players = sourceGame.Players;
        }

        public void AddPlayersGames(IEnumerable<Role> players, long gameId)
        {
            /*
            using (var db = new DataContext(_connectionString))
            {
                var game = (from g in db.GetTable<PlayersRooms>()
                    where g. == gameId
                    select g).FirstOrDefault();

                if (game == default(GameRoom))
                    throw new ArgumentException($"Игровой стол с id {gameId} не найден");

                return game;
            }
            */
        }

        public void UpdatePlayersGames(IEnumerable<Role> players, long gameId)
        {
            throw new NotImplementedException();
        }
    }
}
