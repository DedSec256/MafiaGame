using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaGame.DataLayer.Models;

namespace Mafiagame.DataLayer.Models
{
    [Table(Name = "PlayersRooms")]
    public class PlayersRooms
    {
        [Column(IsPrimaryKey = true)]
        public long UserId;
        [Column]
        public long GameId;
        [Column]
        public string Role;
    }
}
