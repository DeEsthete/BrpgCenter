using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class FileMessage
    {
        public byte[] Content { get; set; }
        public DateTime DepartureTime { get; set; } //время отправки
        public string FullFileName { get; set; }
    }
}
