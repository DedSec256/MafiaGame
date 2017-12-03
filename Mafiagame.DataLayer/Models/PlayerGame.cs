using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaGame.DataLayer.Models;

namespace Mafiagame.DataLayer.Models
{
    [Table(Name = "PlayersGames")]
    public class PlayerGame
    {
        [Column(IsPrimaryKey = true)]
        public long UserId { get; set; }
        [Column]
        public long GameId { get; set; }
        [Column]
        public string Role { get; set; }
    }
}
