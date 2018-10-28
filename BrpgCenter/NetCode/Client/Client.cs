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
        public Player Player { get; set; }
        public TcpClientObject ChatClient { get; set; }
        public TcpClientObject FileClient { get; set; }
        public TcpClientObject ChangedClient { get; set; }

        public List<ChatMessage> Messages { get; set; }
        public List<Character> AllCharactersInRoom { get; set; }

        public string Address { get; set; }
        public int Port { get; set; }
        public bool ServerIsConnect { get; set; }

        public Client(Player player)
        {
            Player = player;
            Address = "127.0.0.1";
            Port = 8888;
            
            Messages = new List<ChatMessage>();
            AllCharactersInRoom = new List<Character>();
        }

        public Client(string address, int port, Player player)
        {
            Player = player;
            Address = address;
            Port = port;

            ChatClient = new TcpClientObject();
            FileClient = new TcpClientObject();
            ChangedClient = new TcpClientObject();

            Messages = new List<ChatMessage>();
            AllCharactersInRoom = new List<Character>();
        }

        public void Connect()
        {
            ChatClient.TcpClient = new TcpClient(Address, Port);
            ChatClient.Stream = ChatClient.TcpClient.GetStream();
            SendFirstMessageWork(ChatClient, new FirstMessage(Player, TcpTypeEnum.TcpChat));

            FileClient.TcpClient = new TcpClient(Address, Port);
            FileClient.Stream = FileClient.TcpClient.GetStream();
            SendFirstMessage(FileClient, new FirstMessage(Player, TcpTypeEnum.TcpFile));

            ChangedClient.TcpClient = new TcpClient(Address, Port);
            ChangedClient.Stream = ChatClient.TcpClient.GetStream();
            SendFirstMessage(ChangedClient, new FirstMessage(Player, TcpTypeEnum.TcpChanged));

            ServerIsConnect = true;
        }

        #region SendMessage

        public async void SendFirstMessage(TcpClientObject client, FirstMessage message)
        {
            await SendFirstMessageWork(client, message);
        }

        private Task SendFirstMessageWork(TcpClientObject client, FirstMessage message)
        {
            return Task.Run(() =>
            {
                string serialized = JsonConvert.SerializeObject(message);
                byte[] bytes = new byte[serialized.Length * sizeof(char)];
                System.Buffer.BlockCopy(serialized.ToCharArray(), 0, bytes, 0, bytes.Length);
                client.Stream.Write(bytes, 0, bytes.Length);
            });
        }

        public async void SendTextMessage(ChatMessage chatMessage) //один раз отправляем
        {
            await SendTextMessageWork(chatMessage);
        }

        private Task SendTextMessageWork(ChatMessage chatMessage)
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

        private Task SendFileMessageWork(FileMessage fileMessage)
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
