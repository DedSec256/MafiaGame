using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using Mafiagame.DataLayer.Implementations;
using MafiaGame.DataLayer.Models;
using System.Data.SqlClient;
using Mafiagame.DataLayer.Models;

namespace MafiaGame.DataLayer.Tests
{
    [TestClass]
    public class UserRepositoryTest
    {
        private const string ConnectionString = 
            @"Data Source=.;Initial Catalog=MafiaGame;Integrated Security=true";

        [TestMethod]
        public void HardloadCreatingAndDeletingTest()
        {
            var userRepository  = new UserRepository(ConnectionString);

            const int COUNT = 10000;
            for (int i = 1; i <= COUNT; i++)
            {
                userRepository.Create(i);
            }
            int dbUsersCount = userRepository.GetAll().Count();
            Assert.AreEqual(dbUsersCount, COUNT, $"dbUsersCount = {dbUsersCount}");

            for (int i = 1; i <= COUNT; i++)
            {
                userRepository.Delete(i);
            }
            Assert.AreEqual(userRepository.GetAll().Count(), 0);

        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            using (var db = new DataContext(ConnectionString))
            {
                db.GetTable<User>().DeleteAllOnSubmit(db.GetTable<User>());
                db.SubmitChanges();
            }
        }

        [TestCleanup]
        public void CleanData()
        {
            
        }
    }
}
