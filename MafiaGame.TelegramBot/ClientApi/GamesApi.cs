using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Mafiagame.DataLayer.Models;
using MafiaGame.DataLayer.Models;
using TelegramBot.ClientApi;

namespace TelegramBot.ClientApi
{
    class GamesApi : BaseApi
    {
        public GamesApi(string connectionString) : base(connectionString) { }
        public async Task<GameRoomJson> CreateGameAsync(GameRoom roomCreation)
        {
            var response = await client.PostAsJsonAsync($"{client.BaseAddress}/games/create", roomCreation);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.StatusCode.ToString() + "\n" + response.ReasonPhrase);
            return await response.Content.ReadAsAsync<GameRoomJson>();
        }
        public async Task<bool> DeleteGameAsync(long id)
        {
            var response = await client.DeleteAsync($"{client.BaseAddress}/games/{id}/delete");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.StatusCode.ToString() + "\n" + response.ReasonPhrase);
            return response.IsSuccessStatusCode;
        }
        public async Task<GameRoomJson> AddPlayerInGame(long id, JsonRole role)
        {
            var response = await client.PostAsJsonAsync($"{client.BaseAddress}/games/{id}/players/add", role);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.StatusCode.ToString() + "\n" + response.ReasonPhrase);
            return await response.Content.ReadAsAsync<GameRoomJson>();
        }
        public async Task<GameRoomJson> DeletePlayerFromGame(long id, long playerId)
        {
            var response = await client.DeleteAsync($"{client.BaseAddress}/games/{id}/players/{playerId}/delete");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.StatusCode.ToString() + "\n" + response.ReasonPhrase);
            return await response.Content.ReadAsAsync<GameRoomJson>();
        }
        public async Task<Role[]> GetPlayersFromGame(long id)
        {
            var response = await client.GetAsync($"{client.BaseAddress}/games/{id}/players");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.StatusCode.ToString() + "\n" + response.ReasonPhrase);
            return await response.Content.ReadAsAsync<Role[]>();
        }
        public async Task<GameRoomJson> GetGameAsync(long id)
        {
            var response = await client.GetAsync($"{client.BaseAddress}/games/{id}");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.StatusCode.ToString() + "\n" + response.ReasonPhrase);
            return await response.Content.ReadAsAsync<GameRoomJson>();
        }
        public async Task<GameRoomJson[]> GetAllGamesAsync()
        {
            var response = await client.GetAsync($"{client.BaseAddress}/games");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.StatusCode.ToString() + "\n" + response.ReasonPhrase);
            return await response.Content.ReadAsAsync<GameRoomJson[]>();
        }
    }
}
