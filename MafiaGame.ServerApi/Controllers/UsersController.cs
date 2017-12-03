using MafiaGame.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using Mafiagame.DataLayer;
using Mafiagame.DataLayer.Implementations;
using MafiaGame.DataLayer.Interfaces;
using MafiaGame.DataLayer.Models;
using MafiaGame.ServerApi.Filters;
//TODO DELETE

namespace MafiaGame.ServerApi.Controllers
{
    public class UsersController : ApiController
    {
        private IUserRepository _userRepository;
        private const string ConnectionString = @"Data Source=.;Initial Catalog=MafiaGame;Integrated Security=true";

        public UsersController()
        {
            _userRepository = new UserRepository(ConnectionString);
        }

        [HttpPost]
        [Route("api/user/register")]
        [ArgumentExceptionFilter]
        public User Create([FromBody] long userId)
        {
            try
            {
                return _userRepository.Create(userId);
            }
            catch (ArgumentException e)
            {
                //Logger.Log.Instance.Error(e.Message);
                throw new ArgumentException(e.Message);
            }
        }
        /// <summary>
        /// Получение пользователя по уникальному идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/users/{id}")]
        [ArgumentExceptionFilter]
        public User Get(long id)
        {
            try
            {
                return _userRepository.Get(id);
            }
            catch (ArgumentException e)
            {
                //Logger.Log.Instance.Error(e.Message);
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        /// Получение пользователя по уникальному идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/users")]
        [ArgumentExceptionFilter]
        public IEnumerable<User> GetAll()
        {
            try
            {
                return _userRepository.GetAll();
            }
            catch (ArgumentException e)
            {
                //Logger.Log.Instance.Error(e.Message);
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        /// Получение пользователя по уникальному идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/users/{id}/delete")]
        [ArgumentExceptionFilter]
        public void Delete(long id)
        {
            try
            {
                _userRepository.Delete(id);
            }
            catch (ArgumentException e)
            {
                //Logger.Log.Instance.Error(e.Message);
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        /// Получение активной сессии пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Группы пользователя</returns>
        [HttpGet]
        [Route("api/users/{id}/game")]
        public long? GetUserActivaGame(long id)
        {
            return _userRepository.Get(id).ActiveGameId;
        }
    }
}
