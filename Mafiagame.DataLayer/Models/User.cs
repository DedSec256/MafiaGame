using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MafiaGame.DataLayer.Models
{

    [Table(Name = "Users")]
    public class User
    {
        [Column(IsPrimaryKey = true)]
        public long Id { get; set; }
        [Column]
        public long? ActiveGameId { get; set; }

    }
}
