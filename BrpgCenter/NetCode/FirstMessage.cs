using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class FirstMessage
    {
        public Player Player { get; set; }
        public Character Character { get; set; }

        public FirstMessage(Player player, Character character)
        {
            Player = player;
            Character = character;
        }
    }
}
