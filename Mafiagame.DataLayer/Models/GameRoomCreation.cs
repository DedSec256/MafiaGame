using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaGame.DataLayer.Models;

namespace Mafiagame.DataLayer.Models
{
    public class GameRoomCreation
    {
        public string Name { get; set; }

        public string Roles { get; private set; } = MafiaGame.DataLayer.Models.Roles.Citizen.GetRoleIcon() +
                                                    MafiaGame.DataLayer.Models.Roles.Mafia.GetRoleIcon();
        public byte MaxPlayers { get; set; }
        public long AdminId { get; set; }

        public void AddRole(string role)
        {
            if (!Roles.Contains(role)) Roles += role;
        }

        public void DeleteRole(string role)
        {
            if (Roles.Contains(role)) Roles = Roles.Replace(role, "");
        }

        public override string ToString()
        {
            StringBuilder sB = new StringBuilder();
            if (!String.IsNullOrEmpty(Name)) sB.Append("*Название:* " + Name);
            sB.Append("\n*Роли:* " + Roles);
            if (MaxPlayers != 0) sB.Append("\n*Максимум игроков:* " + MaxPlayers.ToString());

            return sB.ToString();
        }
    }
}
