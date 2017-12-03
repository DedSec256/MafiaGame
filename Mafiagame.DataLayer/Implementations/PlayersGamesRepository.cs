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
    public class PlayersGamesRepository : IPlayersGamesRepository
    {
        private readonly string _connectionString;

        public PlayersGamesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public PlayerGame Create(PlayerGame game)
        {
            using (var db = new DataContext(_connectionString))
            {
                var checkPlayersRooms = (from r in db.GetTable<PlayerGame>()
                    where r.UserId == game.UserId
                    select r).FirstOrDefault();

                if (checkPlayersRooms != default(PlayerGame))
                {
                    throw new ArgumentException($"Пользователь с id ({game.UserId}) уже играет в другой комнате.");
                }

                db.GetTable<PlayerGame>().InsertOnSubmit(game);
                db.SubmitChanges();
                return game;
            }
        }

        public void Delete(long userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "delete from PlayersGames where UserId = @id";
                    command.Parameters.AddWithValue("@id", userId);

                    command.ExecuteNonQuery();

                    // NLogger.Logger.Trace("База данных:удалено из таблицы:{0}:где PlayersRoomsID:{1}", "[ListOfPlayersRoomss]", id);
                }
            }
        }

        public PlayerGame Get(long userId)
        {
            using (var db = new DataContext(_connectionString))
            {
                var playersRooms = (from r in db.GetTable<PlayerGame>()
                    where r.UserId == userId
                    select r).FirstOrDefault();

                if (playersRooms == default(PlayerGame))
                    throw new ArgumentException($"Пользователь с id {userId} не участвует ни в одной из игр");

                return playersRooms;
            }
        }

        public IEnumerable<PlayerGame> GetAll()
        {
            using (var db = new DataContext(_connectionString))
            {
                return db.GetTable<PlayerGame>().Select(u => u).ToArray();
            }
        }

        public void UpdatePlayerGame(PlayerGame game)
        {
            using (var db = new DataContext(_connectionString))
            {
                var playerRoomFromDb = (from u in db.GetTable<PlayerGame>()
                    where u.UserId == game.UserId
                    select u).FirstOrDefault();

                playerRoomFromDb.Role = game.Role;
                db.SubmitChanges();
            }
        }
    }
}
