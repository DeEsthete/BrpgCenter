using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class Player
    {
        public Guid Id { get; set; }
        public string NickName { get; set; }
        public int CountRooms { get; set; }
        public int CountCharactaers { get; set; }
        public int Rating { get; set; }
        public string PathToImage { get; set; }

        public Player()
        {
            Id = Guid.NewGuid();
        }
    }
}
