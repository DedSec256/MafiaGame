using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaGame;
using MafiaGame.DataLayer;
using System.Data.Linq;
using Mafiagame.DataLayer.Interfaces;
using Mafiagame.DataLayer.Models;
using MafiaGame.DataLayer.Interfaces;
using MafiaGame.DataLayer.Models;

namespace Mafiagame.DataLayer.Implementations
{
    public class PlayersRoomsRepository : IPlayersRoomsRepository
    {
        private readonly string _connectionString;

        public PlayersRoomsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public PlayersRooms Create(User user, string role)
        {
            using (var db = new DataContext(_connectionString))
            {
                var checkPlayersRooms = (from r in db.GetTable<PlayersRooms>()
                    where r.UserId == user.Id
                    select r).FirstOrDefault();

                if (checkPlayersRooms != default(PlayersRooms))
                {
                    throw new ArgumentException($"Пользователь с id ({user.Id}) уже играет в другой комнате.");
                }

                var PlayersRooms = new PlayersRooms()
                {
                    UserId = user.Id,
                    GameId = user.ActiveGameId.Value,
                    Role = role
                };
                db.GetTable<PlayersRooms>().InsertOnSubmit(PlayersRooms);
                db.SubmitChanges();
                return PlayersRooms;
            }
        }

        public void Delete(long userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    //command.CommandText = "delete from ListOfPlayersRoomss where id = @id";

                    //command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();

                    // NLogger.Logger.Trace("База данных:удалено из таблицы:{0}:где PlayersRoomsID:{1}", "[ListOfPlayersRoomss]", id);
                }
            }
        }

        public PlayersRooms Get(long userId)
        {
            using (var db = new DataContext(_connectionString))
            {

                var playersRooms = (from r in db.GetTable<PlayersRooms>()
                    where r.UserId == userId
                    select r).FirstOrDefault();

                if (playersRooms == default(PlayersRooms))
                    throw new ArgumentException($"Пользователь с id {userId} сейчас не участвует ни в одной из игр");

                return playersRooms;
            }
        }

        public IEnumerable<PlayersRooms> GetAll()
        {
            using (var db = new DataContext(_connectionString))
            {
                return db.GetTable<PlayersRooms>().Select(u => u).ToArray();
            }
        }

        public PlayersRooms UpdatePlayerRoom(PlayersRooms playerRoom)
        {
            using (var db = new DataContext(_connectionString))
            {
                var playerRoomFromDb = (from u in db.GetTable<PlayersRooms>()
                    where u.UserId == playerRoom.UserId
                    select u).FirstOrDefault();

                if (playerRoomFromDb == default(PlayersRooms))
                    throw new ArgumentException($"Пользователь с id {playerRoom.UserId} сейчас не участвует ни в одной из игр");

                UpdatePlayersRoomsContent(playerRoom, playerRoomFromDb);
                db.SubmitChanges();
                return playerRoomFromDb;
            }
        }

        private void UpdatePlayersRoomsContent(PlayersRooms sourcePlayersRooms, PlayersRooms destinationPlayersRooms)
        {
            destinationPlayersRooms.Role = sourcePlayersRooms.Role;
        }
    }
}
