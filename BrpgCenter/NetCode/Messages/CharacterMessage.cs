using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class CharacterMessage
    {
        public CharacterMessageType Type { get; set; }

        public List<Character> Characters { get; set; }

        public Character Character { get; set; }
        public Player CharacterOwner { get; set; }
    }
}
