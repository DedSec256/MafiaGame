using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.ClientApi
{
    public abstract class BaseApi
    {
        protected readonly HttpClient client;

        protected BaseApi(string connectionString)
        {
            client = new HttpClient
            {
                BaseAddress = new Uri(connectionString)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
