using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BrpgCenter
{
    public class ChatClient : Client
    {
        public List<ChatMessage> Messages { get; set; }

        public ChatClient(string address, int port, Player player, Character character) : base(address, port, player, character)
        {
            try
            {
                FirstMessage message = new FirstMessage(ClientType.ChatClient, Player, Character);
                string serialized = JsonConvert.SerializeObject(message);
                byte[] data = Encoding.Unicode.GetBytes(serialized);
                Stream.Write(data, 0, data.Length);
                IsConnected = true;
                Messages = new List<ChatMessage>();
            }
            catch (Exception)
            {
                IsConnected = false;
                MessageBox.Show("Хост не найден");
            }
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
                while (IsConnected)
                {
                    try
                    {
                        byte[] data = new byte[DATA_LENGTH];
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = Stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        } while (Stream.DataAvailable);

                        string serialized = builder.ToString();
                        ChatMessage message = JsonConvert.DeserializeObject<ChatMessage>(serialized);
                        Messages.Add(message);
                    }
                    catch (Exception)
                    {
                        Disconnect();
                    }
                }
            });
        }
    }
}
