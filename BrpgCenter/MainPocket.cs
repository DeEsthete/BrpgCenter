using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class MainPocket
    {
        public Player Player { get; set; }
        public MainWindow MainWindow { get; set; }
        public BrpgCenterContext Context { get; set; }
        public ObservableCollection<Room> Rooms { get; set; }
        public ObservableCollection<Character> Characters { get; set; }
        public RoomPage CurrentRoom { get; set; }

        public ServerObject Server { get; set; }

        public MainPocket()
        {
            Player = new Player();
        }
    }
}
