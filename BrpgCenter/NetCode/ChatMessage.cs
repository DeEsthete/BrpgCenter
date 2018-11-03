using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class ChatMessage
    {
        public DateTime DepartureTime { get; set; } //время отправки
        public string SenderName { get; set; }
        public string Content { get; set; }

        public ChatMessage(string senderName, string content)
        {
            DepartureTime = DateTime.Now;
            SenderName = senderName;
            Content = content;
        }
    }
}
