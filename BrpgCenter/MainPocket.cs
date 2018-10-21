using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class MainPocket
    {
        public Player Player { get; set; }
        public List<Room> Rooms { get; set; }
        public List<Character> Characters { get; set; }

        public MainPocket()
        {
            Player = new Player();
            Rooms = new List<Room>();
            Characters = new List<Character>();
        }
    }
}
