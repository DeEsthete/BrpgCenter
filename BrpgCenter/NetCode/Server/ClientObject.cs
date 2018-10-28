using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BrpgCenter
{
    public class ClientObject
    {
        private const int BYTE_COUNT = 1024;

        public Player Player { get; set; }
        public Character Character { get; set; }
        public ServerObject Server { get; set; } // объект сервера
        public TcpClientObject UnknownClient { get; set; }

        public TcpClientObject ChatClient { get; set; }
        public TcpClientObject FileClient { get; set; }
        public TcpClientObject ChangedClient { get; set; }

        public ClientObject(TcpClientObject client, ServerObject serverObject)
        {
            UnknownClient = client;
            Server = serverObject;
            serverObject.AddConnection(this);
        }

        public void ProcessStart()
        {
            try
            {
                UnknownClient.Stream = UnknownClient.TcpClient.GetStream();
                FirstMessage firstMessage = GetFirstMessage();
                Player = firstMessage.Player;
                if (firstMessage.TcpType == TcpTypeEnum.TcpChat)
                {
                    ChatClient = UnknownClient;
                    Thread chatThread = new Thread(new ThreadStart(ProcessChat));
                    chatThread.Start();
                }
                else if (firstMessage.TcpType == TcpTypeEnum.TcpFile)
                {
                    FileClient = UnknownClient;
                    Thread fileThread = new Thread(new ThreadStart(ProcessFile));
                    fileThread.Start();
                }
                else if (firstMessage.TcpType == TcpTypeEnum.TcpChanged)
                {
                    ChangedClient = UnknownClient;
                    Thread fileThread = new Thread(new ThreadStart(ProcessChanged));
                    fileThread.Start();
                }
                UnknownClient = null;

                if (Server.SearchPlayer(this))
                {
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        #region Processes
        public void ProcessChat()
        {
            ChatMessage chatMessage = new ChatMessage(Player.NickName, "Вошел в чат!");
            Server.SendMessage(chatMessage, Player.Id);
            while (true)
            {
                chatMessage = GetChatMessage();
                Server.SendMessage(chatMessage, Player.Id);
            }
        }

        public void ProcessFile()
        {
            FileMessage message;
            while (true)
            {
                message = GetFileMessage();
                if (message.Content == null)
                {
                    //поиск по файлам на сервере, и скинуть его клиенту
                }
                else
                {
                    //загрузить на сервер новый файл
                }
            }
        }

        public void ProcessChanged()
        {
            Character message;
            while (true)
            {
                message = GetChangedMessage();
                Character = message;
                Server.SendAllChanged();
            }
        }
        #endregion

        #region GetMessages
        private FirstMessage GetFirstMessage()
        {
            byte[] data = new byte[BYTE_COUNT]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = UnknownClient.Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (UnknownClient.Stream.DataAvailable);

            string serialized = builder.ToString();
            FirstMessage message = JsonConvert.DeserializeObject<FirstMessage>(serialized);

            return message;
        }

        private ChatMessage GetChatMessage()
        {
            byte[] data = new byte[BYTE_COUNT]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;

            do
            {
                bytes = ChatClient.Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (ChatClient.Stream.DataAvailable);

            string serialized = builder.ToString();
            ChatMessage chatMessage = JsonConvert.DeserializeObject<ChatMessage>(serialized);

            return chatMessage;
        }

        private FileMessage GetFileMessage()
        {
            byte[] data = new byte[BYTE_COUNT]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;

            do
            {
                bytes = FileClient.Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (FileClient.Stream.DataAvailable);

            string serialized = builder.ToString();
            FileMessage fileMessage = JsonConvert.DeserializeObject<FileMessage>(serialized);

            return fileMessage;
        }

        private Character GetChangedMessage()
        {
            byte[] data = new byte[BYTE_COUNT]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;

            do
            {
                bytes = ChangedClient.Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (ChangedClient.Stream.DataAvailable);

            string serialized = builder.ToString();
            Character message = JsonConvert.DeserializeObject<Character>(serialized);

            return message;
        }
        #endregion

        protected internal void Close()
        {
            Server.RemoveConnection(Player.Id);

            ChatClient.TcpClient.Close();
            FileClient.TcpClient.Close();
            ChangedClient.TcpClient.Close();
        }
    }
}
