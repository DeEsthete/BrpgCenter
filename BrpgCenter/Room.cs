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
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }

        [NotMapped]
        public Player GameMaster { get; set; }
        [NotMapped]
        public List<Player> Players { get; set; }

        public Room()
        {
            Players = new List<Player>();
        }

        public Room(int id, string name, string ip, int port, Player gameMaster, List<Player> players)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Ip = ip ?? throw new ArgumentNullException(nameof(ip));
            Port = port;
            GameMaster = gameMaster ?? throw new ArgumentNullException(nameof(gameMaster));
            Players = players ?? throw new ArgumentNullException(nameof(players));
        }
    }
}
