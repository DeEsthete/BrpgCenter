using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class FileMessage
    {
        public FileMessageType Type { get; set; }
        public List<FileInfo> FileList { get; set; }
        public FileInfo FileInfo { get; set; }
        public byte[] FileContent { get; set; }
    }
}
