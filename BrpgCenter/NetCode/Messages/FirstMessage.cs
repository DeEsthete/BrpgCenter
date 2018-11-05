using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class FirstMessage
    {
        public ClientType Type { get; set; }
        public Player Player { get; set; }
        public Character Character { get; set; }

        public FirstMessage(ClientType type, Player player, Character character)
        {
            Type = type;
            Player = player;
            Character = character;
        }
    }
}
