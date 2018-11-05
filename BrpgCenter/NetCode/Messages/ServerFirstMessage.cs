using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class ServerFirstMessage
    {
        public Player Master { get; set; }
        public List<Player> Players { get; set; }
        public List<Character> CharacterInRoom { get; set; }

        public ServerFirstMessage()
        {
            Players = new List<Player>();
            CharacterInRoom = new List<Character>();
        }
    }
}
