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
        public List<Character> CharacterInRoom { get; set; }

        public ServerFirstMessage()
        {
            CharacterInRoom = new List<Character>();
        }
    }
}
