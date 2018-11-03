using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BrpgCenter
{
    public class Client
    {
        public const int DATA_LENGTH = 1024;

        public bool IsConnected { get; set; }
        public Player Player { get; set; }
        public Character Character { get; set; }
        public string Address{ get; set; }
        public int Port { get; set; }
        public TcpClient TcpClient { get; set; }
        public NetworkStream Stream { get; set; }
        public List<ChatMessage> Messages { get; set; }
        public ServerFirstMessage ServerFirstMessage { get; set; }

        public Client(string address, int port, Player player, Character character)
        {
            Player = player;
            Character = character;
            Address = address;
            Port = port;
            Messages = new List<ChatMessage>();
            TcpClient = new TcpClient();
            try
            {
                TcpClient.Connect(Address, Port); //подключение клиента
                Stream = TcpClient.GetStream(); // получаем поток
                FirstMessage message = new FirstMessage(Player, Character);
                string serialized = JsonConvert.SerializeObject(message);
                byte[] data = Encoding.Unicode.GetBytes(serialized);
                Stream.Write(data, 0, data.Length);
                IsConnected = true;
            }
            catch (Exception)
            {
                IsConnected = false;
                MessageBox.Show("Хост не найден");
            }
        }

        public async void AcceptServerFirstMessage()
        {
            if (IsConnected)
            {
                await AcceptServerFirstMessageWork();
            }
        }
        private Task AcceptServerFirstMessageWork()
        {
            return Task.Run(() =>
            {
                byte[] data = new byte[DATA_LENGTH]; // буфер для получаемых данных
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = Stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (Stream.DataAvailable);

                string serialized = builder.ToString();
                ServerFirstMessage = JsonConvert.DeserializeObject<ServerFirstMessage>(serialized);
            });
        }

        // отправка сообщений
        public async void SendMessage(ChatMessage message)
        {
            if (IsConnected)
            {
                await SendMessageWork(message);
            }
        }

        private Task SendMessageWork(ChatMessage message)
        {
            return Task.Run(() =>
            {
                string serialized = JsonConvert.SerializeObject(message);
                byte[] data = Encoding.Unicode.GetBytes(serialized);
                Stream.Write(data, 0, data.Length);
            });
        }

        // получение сообщений
        public async void ReceiveMessage()
        {
            if (IsConnected)
            {
                await RecieveMessageWork();
            }
        }

        private Task RecieveMessageWork()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] data = new byte[DATA_LENGTH]; // буфер для получаемых данных
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = Stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        } while (Stream.DataAvailable);

                        string serialized = builder.ToString();
                        ChatMessage message = JsonConvert.DeserializeObject<ChatMessage>(serialized);
                        Messages.Add(message); //вывод сообщения
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        Disconnect();
                    }
                }
            });
        }

        void Disconnect()
        {
            if (Stream != null)
                Stream.Close();//отключение потока
            if (TcpClient != null)
                TcpClient.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }
    }
}
