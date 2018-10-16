using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Password { get; set; }

        [NotMapped]
        public Player GameMaster { get; set; }
        [NotMapped]
        public List<Player> Players { get; set; }
    }
}
