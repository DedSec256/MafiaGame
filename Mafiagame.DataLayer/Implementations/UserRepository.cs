using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaGame;
using MafiaGame.DataLayer;
using System.Data.Linq;
using MafiaGame.DataLayer.Interfaces;
using MafiaGame.DataLayer.Models;

namespace Mafiagame.DataLayer.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User Create(long id)
        {
            using (var db = new DataContext(_connectionString))
            {
                var checkUser = (from u in db.GetTable<User>()
                    where u.Id == id
                    select u).FirstOrDefault();

                if (checkUser != default(User))
                {
                    throw new ArgumentException($"Пользователь с id ({id}) уже существует.");
                }

                var user = new User()
                {
                    Id = id,
                    ActiveGameId = null
                };
                db.GetTable<User>().InsertOnSubmit(user);
                db.SubmitChanges();
                return user;
            }
        }

        public void Delete(long userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    //command.CommandText = "delete from ListOfUsers where id = @id";

                    //command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();

                   // NLogger.Logger.Trace("База данных:удалено из таблицы:{0}:где UserID:{1}", "[ListOfUsers]", id);
                }
            }
        }
        public User Get(long userId)
        {
            using (var db = new DataContext(_connectionString))
            {

                var user = (from u in db.GetTable<User>()
                    where u.Id == userId
                    select u).FirstOrDefault();

                if (user == default(User))
                    throw new ArgumentException($"Пользователь с id {userId} не найден");

                return user;
            }
        }

        public IEnumerable<User> GetAll()
        {
            using(var db = new DataContext(_connectionString))
            {
                return db.GetTable<User>().Select(u => u).ToArray();
            }
        }

        public User UpdateUser(User user)
        {
            using (var db = new DataContext(_connectionString))
            {
                var userFromDb = (from u in db.GetTable<User>()
                                  where u.Id == user.Id
                                  select u).FirstOrDefault();
                if (userFromDb == default(User))
                    throw new ArgumentException($"Пользователь с id {user.Id} не найден");

                UpdateUserContent(user, userFromDb);
                db.SubmitChanges();
                return userFromDb;
            }
        }
        private void UpdateUserContent(User sourceUser, User destinationUser)
        {
            destinationUser.ActiveGameId = sourceUser.ActiveGameId;
        }
    }
}
