using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mafiagame.DataLayer.Models;
using MafiaGame.DataLayer.Models;

namespace TelegramBot.Kernel
{
    static class ClientApi
    {
        private const string BASE_ADRESS = "http://localhost:25575/api";
        public static Task<HttpResponseMessage> Register(long userId)
        {
            HttpClient _client = new HttpClient
            {
                BaseAddress = new Uri(BASE_ADRESS)
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return _client.PostAsJsonAsync($"{_client.BaseAddress}/user/register", userId);
        }

        public static Task<HttpResponseMessage> GetAllUsers()
        {
            HttpClient _client = new HttpClient
            {
                BaseAddress = new Uri(BASE_ADRESS)
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return _client.GetAsync($"{_client.BaseAddress}/users");
        }

        public static Task<HttpResponseMessage> GetAllGames()
        {
            HttpClient _client = new HttpClient
            {
                BaseAddress = new Uri(BASE_ADRESS)
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return _client.GetAsync($"{_client.BaseAddress}/games");
        }

        public static Task<HttpResponseMessage> UpdateUser(User user)
        {
            HttpClient _client = new HttpClient
            {
                BaseAddress = new Uri(BASE_ADRESS)
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return _client.PutAsJsonAsync($"{_client.BaseAddress}/user/update", user);
        }

        public static Task<HttpResponseMessage> CreateGame(GameRoom room)
        {
            HttpClient _client = new HttpClient
            {
                BaseAddress = new Uri(BASE_ADRESS)
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return _client.PostAsJsonAsync($"{_client.BaseAddress}/game/new", room);
        }
    }
}
