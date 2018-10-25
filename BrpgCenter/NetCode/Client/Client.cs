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
        
        public TcpClientObject ChatClient { get; set; }
        public TcpClientObject FileClient { get; set; }
        public TcpClientObject ChangedClient { get; set; }

        public List<ChatMessage> Messages { get; set; }
        public List<Character> AllCharactersInRoom { get; set; }

        public string Address { get; set; }
        public int PortChat { get; set; }
        public int PortFile { get; set; }
        public int PortChanged { get; set; }
        public bool ServerIsConnect { get; set; }

        public Client()
        {
            Address = "127.0.0.1";

            PortChat = 8888;
            PortFile = 8887;
            PortChanged = 8886;
            
            Messages = new List<ChatMessage>();
            AllCharactersInRoom = new List<Character>();
        }

        public Client(string address, int portChat, int portFile, int portChanged)
        {
            Address = address;
            PortChat = portChat;
            PortFile = portFile;
            PortChanged = portChanged;

            ChatClient = new TcpClientObject();
            FileClient = new TcpClientObject();
            ChangedClient = new TcpClientObject();

            Messages = new List<ChatMessage>();
            AllCharactersInRoom = new List<Character>();
        }

        public void Connect()
        {
            ChatClient.TcpClient = new TcpClient(Address, PortChat);
            ChatClient.Stream = ChatClient.TcpClient.GetStream();

            FileClient.TcpClient = new TcpClient(Address, PortFile);
            FileClient.Stream = FileClient.TcpClient.GetStream();

            ChangedClient.TcpClient = new TcpClient(Address, PortChanged);
            ChangedClient.Stream = ChatClient.TcpClient.GetStream();

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
                ChatClient.Stream.Write(bytes, 0, bytes.Length);
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
                FileClient.Stream.Write(bytes, 0, bytes.Length);
            });
        }
        #endregion

        #region accept
        public async void AcceptText() //всегда принимаем сообщения
        {
            await AcceptTextWork();
        }

        private Task AcceptTextWork()
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
                        bytes = ChatClient.Stream.Read(buffer, 0, buffer.Length);
                        builder.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                    }
                    while (ChatClient.Stream.DataAvailable);

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

        private Task AcceptFileWork()
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
                        bytes = FileClient.Stream.Read(buffer, 0, buffer.Length);
                        builder.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                    }
                    while (FileClient.Stream.DataAvailable);

                    string serialized = builder.ToString();
                    FileMessage message = JsonConvert.DeserializeObject(serialized) as FileMessage;
                    //закинуть в папку загрузки
                }
            });
        }

        public async void AcceptChanged() //всегда принимаем сообщения
        {
            await AcceptFileWork();
        }

        private Task AcceptChangedWork()
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
                        bytes = FileClient.Stream.Read(buffer, 0, buffer.Length);
                        builder.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                    }
                    while (FileClient.Stream.DataAvailable);

                    string serialized = builder.ToString();
                    Character message = JsonConvert.DeserializeObject(serialized) as Character;

                    bool isFind = false;
                    foreach (var i in AllCharactersInRoom)
                    {
                        if (message.Owner == i.Owner)
                        {
                            AllCharactersInRoom.Remove(i);
                            AllCharactersInRoom.Add(message);
                            isFind = true;
                        }
                    }
                    if (!isFind)
                    {
                        AllCharactersInRoom.Add(message);
                    }
                }
            });
        }
        #endregion

        public void Close()
        {
            ChatClient.TcpClient.Close();
            FileClient.TcpClient.Close();
            ChangedClient.TcpClient.Close();

            ServerIsConnect = false;
        }
    }
}
