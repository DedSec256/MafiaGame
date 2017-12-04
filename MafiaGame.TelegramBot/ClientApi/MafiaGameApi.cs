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

namespace TelegramBot.ClientApi
{
    /* Api singletone */
    internal class MafiaService
    {
        private const string connetionString = "http://localhost:25575/api";

        private static MafiaService Api;
        public UsersApi Users { get; }
        public GamesApi Games { get; }

        private MafiaService()
        {
            Users = new UsersApi(connetionString);
            Games = new GamesApi(connetionString);
        }

        public static MafiaService Create()
        {
            return Api ?? (Api = new MafiaService());
        }

    }
}
