using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mafiagame.DataLayer.Models;
using System.Data.Linq;
using System.Data.SqlClient;
using Mafiagame.DataLayer.Implementations;
using Mafiagame.DataLayer.Interfaces;
using MafiaGame.DataLayer.Interfaces;
using MafiaGame.DataLayer.Models;

namespace MafiaGame.DataLayer.Implementations
{
    public class GameRoomRepository : IGameRoomRepository
    {
        private readonly string _connectionString;
        private readonly IPlayersGamesRepository _playersGamesRepository;

        /* Пользователь вышел из игры */
        public static event EventHandler<long> UserExitGame;
        /* Пользователь вошёл в игровую комнату */
        public static event EventHandler<(long userId, long gameId)> UserEnterGame;

        public GameRoomRepository(string connectionString)
        {
            _connectionString = connectionString;
            _playersGamesRepository = new PlayersGamesRepository(_connectionString);
            new UserRepository(connectionString);
        }
        public GameRoom Create(GameRoom room)
        {
            using (var db = new DataContext(_connectionString))
            {
                var checkGame = (from g in db.GetTable<GameRoom>()
                                 where g.Id == room.Id select g).FirstOrDefault();

                if (checkGame != default(GameRoom))
                {
                    throw new ArgumentException($"Игровая комната c id{room.Id} уже существует");
                }

                AddPlayerGaranty(new PlayerGame(){
                    GameId = room.Id,
                    UserId = room.Id,
                    Role = Roles.Dead.ToString()});

                db.GetTable<GameRoom>().InsertOnSubmit(room);
                db.SubmitChanges();

                room.Players = GetGamePlayers(room.Id).Select(RoleFactory.GetRole);
                return room;
            }
        }

        public IEnumerable<GameRoom> GetAll()
        {
            using (var db = new DataContext(_connectionString))
            {
                /* Получаем список игроков */
                return db.GetTable<GameRoom>().Select(g => g).AsEnumerable().Select((g) =>
                {
                    var game = g;
                    game.Players = GetGamePlayers(game.Id).Select(RoleFactory.GetRole);
                    return game;       
                }).ToArray(); 
            }
        }


        public void Delete(long gameId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                /* Удаляем игроков комнаты из общего списка */
                var users = GetGamePlayers(gameId);
                foreach (var u in users)
                {
                    _playersGamesRepository.Delete(u.UserId);
                    UserExitGame.Invoke(this, u.UserId);
                }

                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "delete from GameRooms where Id = @id";
                    command.Parameters.AddWithValue("@id", gameId);
                    command.ExecuteNonQuery();

                    // NLogger.Logger.Trace("База данных:удалено из таблицы:{0}:где UserID:{1}", "[ListOfUsers]", id);
                }
            }
        }

        public GameRoom Get(long gameId)
        {
            using (var db = new DataContext(_connectionString))
            {
                var game = (from g in db.GetTable<GameRoom>()
                    where g.Id == gameId
                    select g).FirstOrDefault();

                if (game == default(GameRoom))
                    throw new ArgumentException($"Игровая комната с id{gameId} не существует");

                game.Players = GetGamePlayers(game.Id).Select(RoleFactory.GetRole);
                return game;
            }
        }

        public GameRoom AddPlayer(PlayerGame game)
        {
            using (var db = new DataContext(_connectionString))
            {
                var gameFromDb = (from g in db.GetTable<GameRoom>()
                    where g.Id == game.GameId
                    select g).FirstOrDefault();
                if (gameFromDb == default(GameRoom))
                    throw new ArgumentException($"Игровая комната с id {game.GameId} не существует");

                gameFromDb.Players = GetGamePlayers(game.GameId).Select(RoleFactory.GetRole);
                if (gameFromDb.Players.Count() == gameFromDb.MaxPlayers)
                {
                    throw new ArgumentException($"В игре с id {game.GameId} максимум игроков");
                }

                _playersGamesRepository.Create(game);
                /* Отправляем в Users Id новой игры */
                UserEnterGame.Invoke(this, (game.UserId, game.GameId));

                gameFromDb.Players = GetGamePlayers(game.GameId).Select(RoleFactory.GetRole);
                return gameFromDb;
            }
        }

        public GameRoom RemovePlayer(long gameId, long userId)
        {
            using (var db = new DataContext(_connectionString))
            {
                var gameFromDb = (from g in db.GetTable<GameRoom>()
                    where g.Id == gameId
                    select g).FirstOrDefault();
                if (gameFromDb == default(GameRoom))
                    throw new ArgumentException($"Игровая комната с id {gameId} не существует");

                _playersGamesRepository.Delete(userId);
                /* Отправляем в Users Id вышедшего пользователя */
                UserExitGame.Invoke(this, userId);

                gameFromDb.Players = GetGamePlayers(gameId).Select(RoleFactory.GetRole);
                return gameFromDb;
            }
        }

        /* AddPlayer с гарантией существования */
        public void AddPlayerGaranty(PlayerGame game)
        {
            _playersGamesRepository.Create(game);
            /* Отправляем в Users Id новой игры */
            UserEnterGame.Invoke(this, (game.UserId, game.GameId));
        }

        public IEnumerable<PlayerGame> GetGamePlayers(long gameId)
        {
            using (var db = new DataContext(_connectionString))
            {
                var gameFromDb = (from g in db.GetTable<PlayerGame>()
                    where g.GameId == gameId
                    select g).ToArray();

                if (gameFromDb.Length == 0)
                    throw new ArgumentException($"Ни один игрок не участвует в игре с id{gameId}");

                return gameFromDb;
            }
        }


    }
}
