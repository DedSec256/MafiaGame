using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaGame.DataLayer.Models;

namespace Mafiagame.DataLayer.Models
{
    [Table(Name = "GameRooms")]
    public class GameRoom
    {
        [Column(IsPrimaryKey = true)]
        public long Id { get; set; }
        [Column]
        public byte MaxPlayers { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public string ActiveRoles { get; set; }
        public IEnumerable<Role> Players { get; set; }

        public GameRoomJson ToJson()
        {
            return new GameRoomJson()
            {
                ActiveRoles = ActiveRoles,
                Id = Id,
                MaxPlayers = MaxPlayers,
                Name = Name,
                Players = Players.Select(r => r.ToJson())
            };
        }

        public static GameRoom CreateGameRoom(GameRoomCreation gameRoom)
        {
            return new GameRoom()
            {
                Id = gameRoom.AdminId,
                MaxPlayers = gameRoom.MaxPlayers,
                Name = gameRoom.Name,
                ActiveRoles = gameRoom.Roles
            };
        }
    }

    public class GameRoomJson
    {
        [Column(IsPrimaryKey = true)]
        public long Id { get; set; }
        [Column]
        public byte MaxPlayers { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public string ActiveRoles { get; set; }
        public IEnumerable<JsonRole> Players { get; set; }

        public  GameRoom ToGameRoom()
        {
            return new GameRoom()
            {
                ActiveRoles = ActiveRoles,
                Id = Id,
                MaxPlayers = MaxPlayers,
                Name = Name,
                Players = Players.Select(RoleFactory.GetRole)
            };
        }
    }
}
