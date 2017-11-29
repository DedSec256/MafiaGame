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
        private const string ConnectionString = @"Data Source=.;Initial Catalog=MafiaGame;Integrated Security=true";

        public GameRoomsController()
        {
            _gameRoomsRepository = new GameRoomRepository(ConnectionString);
        }

        [HttpPost]
        [Route("api/game/new")]
        [ArgumentExceptionFilter]
        public GameRoom Create([FromBody] GameRoom room)
        {
            try
            {
                return _gameRoomsRepository.Create(room);
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
        [Route("api/game/{id}")]
        [ArgumentExceptionFilter]
        public GameRoom Get(long id)
        {
            try
            {
                return _gameRoomsRepository.Get(id);
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
        public IEnumerable<GameRoom> GetAll()
        {
            try
            {
                return _gameRoomsRepository.GetAll();
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
        /// <param name="GameRoom">Новые данные пользователя</param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/game/update")]
        [ArgumentExceptionFilter]
        public GameRoom Update([FromBody] GameRoom GameRoom)
        {
            // Logger.Log.Instance.Info("Изменение пользователя с именем: {0}", GameRoom.Name);
            try
            {
                return _gameRoomsRepository.UpdateGameRoom(GameRoom);
            }
            catch (ArgumentException e)
            {
                //Logger.Log.Instance.Error(e.Message);
                throw new ArgumentException(e.Message);
            }
        }
    }
}
