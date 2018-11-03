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
    public class ClientObject
    {
        public string Id { get; private set; }
        public NetworkStream Stream { get; private set; }
        public Player Player { get; set; }
        public Character Character { get; set; }
        public TcpClient Client { get; set; }
        public ServerObject Server { get; set; } // объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            Client = tcpClient;
            Stream = Client.GetStream();
            Server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                FirstMessage firstMessage = GetFirstMessage();
                Player = firstMessage.Player;
                Character = firstMessage.Character;
                Server.SendServerFirstMessage(this);

                ChatMessage message = new ChatMessage("Server: ", Player.NickName + " вошел в чат");
                // посылаем сообщение о входе в чат всем подключенным пользователям
                Server.BroadcastMessage(message, this.Id);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        message = GetChatMessage();
                        Server.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        message = new ChatMessage("Server: ", Player.NickName + " покинул чат");
                        Server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                Server.RemoveConnection(this.Id);
                Close();
            }
        }

        private FirstMessage GetFirstMessage()
        {
            string serialized = GetSerializedString();
            return JsonConvert.DeserializeObject<FirstMessage>(serialized);
        }

        private ChatMessage GetChatMessage()
        {
            string serialized = GetSerializedString();
            return JsonConvert.DeserializeObject<ChatMessage>(serialized);
        }
        // чтение входящего сообщения и преобразование в строку
        private string GetSerializedString()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // закрытие подключения
        public void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (Client != null)
                Client.Close();
        }
    }
}
