using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Linq;
using System.Linq;
using Mafiagame.DataLayer.Implementations;
using Mafiagame.DataLayer.Models;
using MafiaGame.DataLayer.Implementations;
using MafiaGame.DataLayer.Interfaces;
using MafiaGame.DataLayer.Models;

namespace MafiaGame.DataLayer.Tests
{

    [TestClass]
    public class GameRoomsTests
    {
        private const string ConnectionString =
            @"Data Source=.;Initial Catalog=MafiaGame;Integrated Security=true";

        private IUserRepository userRepository = new UserRepository(ConnectionString);

        [TestInitialize()]
        public void MyTestInitialize()
        {
            using (var db = new DataContext(ConnectionString))
            {
                db.GetTable<User>().DeleteAllOnSubmit(db.GetTable<User>());
                db.GetTable<PlayerGame>().DeleteAllOnSubmit(db.GetTable<PlayerGame>());
                db.GetTable<GameRoom>().DeleteAllOnSubmit(db.GetTable<GameRoom>());
                db.SubmitChanges();
            }
        }

        [TestMethod]
        public void HardloadCreatingAndDeletingTest()
        {
            var gameRoomRepository = new GameRoomRepository(ConnectionString);

            const int COUNT = 10000;
            for (int i = 1; i <= COUNT; i++)
            {

                userRepository.Create(i);

                gameRoomRepository.Create(
                    GameRoom.CreateGameRoom(
                        new GameRoomCreation()
                        {
                            AdminId = i,
                            MaxPlayers = 15,
                            Name = $"testGame{i}"
                        }));
            }
            int dbGamesCount = gameRoomRepository.GetAll().Count();
            Assert.AreEqual(dbGamesCount, COUNT, $"dbGamesCount = {dbGamesCount}");

            for (int i = 1; i <= COUNT; i++)
            {
                gameRoomRepository.Delete(i);
            }
            Assert.AreEqual(gameRoomRepository.GetAll().Count(), 0);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MaxPlayersTest()
        {
            var gameRoomRepository = new GameRoomRepository(ConnectionString);

            for (int i = 1; i <= 11; i++)
            {
                userRepository.Create(i);
            }

            gameRoomRepository.Create(
                GameRoom.CreateGameRoom(
                    new GameRoomCreation()
                    {
                        AdminId = 1,
                        MaxPlayers = 10,
                        Name = $"testGame"
                    }));

            for (int i = 2; i <= 11; i++)
            {
                gameRoomRepository.AddPlayer(new PlayerGame()
                { GameId = 1, Role = Roles.Dead.ToString(), UserId = i});
            }

        }

        [TestCleanup]
        public void CleanData()
        {
            MyTestInitialize();
        }
    }
}
