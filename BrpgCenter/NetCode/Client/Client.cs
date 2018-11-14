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
        public const int SLEEP_TIME_COMMON = 2000;

        public bool IsConnected { get; set; }
        public Player Player { get; set; }
        public Character Character { get; set; }
        public string Address{ get; set; }
        public int Port { get; set; }
        public TcpClient TcpClient { get; set; }
        public NetworkStream Stream { get; set; }
        public ServerFirstMessage ServerFirstMessage { get; set; }

        public Client(string address, int port, Player player, Character character)
        {
            Player = player;
            Character = character;
            Address = address;
            Port = port;
            TcpClient = new TcpClient();
            try
            {
                TcpClient.Connect(Address, Port); //подключение клиента
                Stream = TcpClient.GetStream(); // получаем поток
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

        protected void Disconnect()
        {
            if (Stream != null)
                Stream.Close();
            if (TcpClient != null)
                TcpClient.Close();
        }
    }
}
