using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Mafiagame.DataLayer.Models;
using MafiaGame.DataLayer.Models;

namespace TelegramBot.ClientApi
{
    public class UsersApi : BaseApi
    {
        internal UsersApi(string connectionString) : base(connectionString) { }

        public async Task<User> RegisterUserAsync(long id)
        {
            var response = await client.PostAsJsonAsync($"{client.BaseAddress}/users/register", id);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.StatusCode.ToString() + "\n" + response.ReasonPhrase);
            return await response.Content.ReadAsAsync<User>();
        }
        public async Task<bool> DeleteUserAsync(long id)
        {
            var response = await client.DeleteAsync($"{client.BaseAddress}/users/{id}/delete");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.StatusCode.ToString() + "\n" + response.ReasonPhrase);
            return response.IsSuccessStatusCode;
        }
        public async Task<User[]> GetAllUsersAsync()
        {
            var response = await client.GetAsync($"{client.BaseAddress}/users");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.StatusCode.ToString() + "\n" + response.ReasonPhrase);
            return await response.Content.ReadAsAsync<User[]>();
        }
    }
}
