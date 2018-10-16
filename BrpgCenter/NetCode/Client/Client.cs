using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class Client
    {
        private const int BYTE_COUNT = 1024;

        public TcpClient TcpClientChat { get; set; }
        public NetworkStream StreamChat { get; set; }

        public TcpClient TcpClientFile { get; set; }
        public NetworkStream StreamFile { get; set; }

        public List<ChatMessage> Messages { get; set; }

        public string Address { get; set; }
        public int PortChat { get; set; }
        public int PortFile { get; set; }
        public bool ServerIsConnect { get; set; }

        public Client()
        {
            Address = "127.0.0.1";

            PortChat = 8888;
            PortFile = 8887;
            
            Messages = new List<ChatMessage>();
        }

        public void Connect()
        {
            TcpClientChat = new TcpClient(Address, PortChat);
            StreamChat = TcpClientChat.GetStream();

            TcpClientFile = new TcpClient(Address, PortFile);
            StreamFile = TcpClientFile.GetStream();
            ServerIsConnect = true;
        }

        #region SendMessage
        public async void SendTextMessage(ChatMessage chatMessage) //один раз отправляем
        {
            await SendTextMessageWork(chatMessage);
        }

        public Task SendTextMessageWork(ChatMessage chatMessage)
        {
            return Task.Run(() =>
            {
                string serialized = JsonConvert.SerializeObject(chatMessage);
                byte[] bytes = new byte[serialized.Length * sizeof(char)];
                System.Buffer.BlockCopy(serialized.ToCharArray(), 0, bytes, 0, bytes.Length);
                StreamChat.Write(bytes, 0, bytes.Length);
            });
        }

        public async void SendFileMessage(FileMessage fileMessage)
        {
            await SendFileMessageWork(fileMessage);
        }

        public Task SendFileMessageWork(FileMessage fileMessage)
        {
            return Task.Run(() =>
            {
                string serialized = JsonConvert.SerializeObject(fileMessage);
                byte[] bytes = new byte[serialized.Length * sizeof(char)];
                System.Buffer.BlockCopy(serialized.ToCharArray(), 0, bytes, 0, bytes.Length);
                StreamChat.Write(bytes, 0, bytes.Length);
            });
        }
        #endregion

        #region accept
        public async void AcceptText() //всегда принимаем сообщения
        {
            await AcceptTextWork();
        }

        public Task AcceptTextWork()
        {
            return Task.Run(() =>
            {
                while (ServerIsConnect)
                {
                    byte[] buffer = new byte[BYTE_COUNT];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = StreamChat.Read(buffer, 0, buffer.Length);
                        builder.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                    }
                    while (StreamChat.DataAvailable);

                    string serialized = builder.ToString();
                    ChatMessage chatMessage = JsonConvert.DeserializeObject(serialized) as ChatMessage;
                    Messages.Add(chatMessage);
                }
            });
        }

        public async void AcceptFile() //всегда принимаем сообщения
        {
            await AcceptFileWork();
        }

        public Task AcceptFileWork()
        {
            return Task.Run(() =>
            {
                while (ServerIsConnect)
                {
                    byte[] buffer = new byte[BYTE_COUNT];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = StreamChat.Read(buffer, 0, buffer.Length);
                        builder.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                    }
                    while (StreamChat.DataAvailable);

                    string serialized = builder.ToString();
                    ChatMessage chatMessage = JsonConvert.DeserializeObject(serialized) as ChatMessage;
                    Messages.Add(chatMessage);
                }
            });
        }
        #endregion

        public void Close()
        {
            TcpClientChat.Close();
            TcpClientFile.Close();
            ServerIsConnect = false;
        }
    }
}
