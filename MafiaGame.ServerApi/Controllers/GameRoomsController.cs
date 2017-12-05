using MafiaGame.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using Mafiagame.DataLayer;
using Mafiagame.DataLayer.Models;
using MafiaGame.DataLayer.Implementations;
using MafiaGame.DataLayer.Interfaces;
using MafiaGame.DataLayer.Models;
using MafiaGame.ServerApi.Filters;

namespace MafiaGame.ServerApi.Controllers
{
    public class GameRoomsController : ApiController
    {
        private IGameRoomRepository _gameRoomsRepository;
        private const string ConnectionString = 
            @"Data Source=.;Initial Catalog=MafiaGame;Integrated Security=true";

        public GameRoomsController()
        {
            _gameRoomsRepository = new GameRoomRepository(ConnectionString);
        }

        [HttpPost]
        [Route("api/games/create")]
        [ArgumentExceptionFilter]
        public GameRoomJson Create([FromBody] GameRoom room)
        {
            try
            {
                return _gameRoomsRepository.Create(room).ToJson();
            }
            catch (ArgumentException e)
            {
                //Logger.Log.Instance.Error(e.Message);
                throw new ArgumentException(e.Message);
            }
        }
        /// <summary>
        /// Получение игры по уникальному идентификатору
        /// </summary>
        /// <param name="id">Идентификатор игры</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/games/{id}")]
        [ArgumentExceptionFilter]
        public GameRoomJson Get(long id)
        {
            try
            {
                return _gameRoomsRepository.Get(id).ToJson();
            }
            catch (ArgumentException e)
            {
                //Logger.Log.Instance.Error(e.Message);
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        /// Получение всех игр
        /// </summary>
        /// <param name="id">Идентификатор игры</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/games")]
        [ArgumentExceptionFilter]
        public IEnumerable<GameRoomJson> GetAll()
        {
            try
            {
                return _gameRoomsRepository.GetAll().Select(g => g.ToJson());
            }
            catch (ArgumentException e)
            {
                //Logger.Log.Instance.Error(e.Message);
                throw new ArgumentException(e.Message);
            }
        }

        [HttpGet]
        [Route("api/games/{id}/players")]
        [ArgumentExceptionFilter]
        public IEnumerable<Role> GetPlayers(long id)
        {
            // Logger.Log.Instance.Info("Изменение пользователя с именем: {0}", GameRoomJson.Name);
            try
            {
                return _gameRoomsRepository.Get(id).Players;
            }
            catch (ArgumentException e)
            {
                //Logger.Log.Instance.Error(e.Message);
                throw new ArgumentException(e.Message);
            }
        }

        [HttpPost]
        [Route("api/games/{id}/players/add")]
        [ArgumentExceptionFilter]
        public GameRoomJson AddPlayer(long id, [FromBody] JsonRole role)
        {
            // Logger.Log.Instance.Info("Изменение пользователя с именем: {0}", GameRoomJson.Name);
            try
            {
                return _gameRoomsRepository.AddPlayer(new PlayerGame(){
                    GameId = id, Role = role.Role, UserId = role.UserId}).ToJson();
            }
            catch (ArgumentException e)
            {
                //Logger.Log.Instance.Error(e.Message);
                throw new ArgumentException(e.Message);
            }
        }

        [HttpDelete]
        [Route("api/games/{id}/players/{playerId}/delete")]
        [ArgumentExceptionFilter]
        public GameRoomJson RemovePlayer(long id, long playerId)
        {
            // Logger.Log.Instance.Info("Изменение пользователя с именем: {0}", GameRoomJson.Name);
            try
            {
                return _gameRoomsRepository.RemovePlayer(id, playerId).ToJson();
            }
            catch (ArgumentException e)
            {
                //Logger.Log.Instance.Error(e.Message);
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        /// Обновление данных о пользователе
        /// </summary>
        /// <param name="GameRoomJson">Новые данные пользователя</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/games/{id}/delete")]
        [ArgumentExceptionFilter]
        public void Delete(long id)
        {
            // Logger.Log.Instance.Info("Изменение пользователя с именем: {0}", GameRoomJson.Name);
            try
            {
                _gameRoomsRepository.Delete(id);
            }
            catch (ArgumentException e)
            {
                //Logger.Log.Instance.Error(e.Message);
                throw new ArgumentException(e.Message);
            }
        }


    }
}
