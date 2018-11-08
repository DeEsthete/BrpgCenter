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
        public const int DATA_LENGTH = 1024;

        public string Id { get; private set; }
        public ClientType Type { get; set; }
        public NetworkStream Stream { get; private set; }
        public Player Player { get; set; }
        public Character Character { get; set; }
        public TcpClient Client { get; set; }
        public ServerObject Server { get; set; }

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
            FirstMessage firstMessage = GetFirstMessage();
            Player = firstMessage.Player;
            Character = firstMessage.Character;
            Type = firstMessage.Type;
            if (Type == ClientType.ChatClient)
            {
                ChatProcess();
            }
            else if (Type == ClientType.StateClient)
            {
                StateProcess();
            }
            else if (Type == ClientType.CharactersClient)
            {
                CharactersProcess();
            }
        }
        #region Processes

        private void ChatProcess()
        {
            try
            {
                Server.SendServerFirstMessage(this);

                ChatMessage message = new ChatMessage("Server: ", Player.NickName + " вошел в чат");
                Server.BroadcastMessage(message, this.Id);
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
                Server.RemoveConnection(this.Id);
                Close();
            }
        }

        private void StateProcess()
        {
            try
            {
                while (true)
                {
                    Server.SendState(this);
                    Thread.Sleep(5000);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                Server.RemoveConnection(this.Id);
                Close();
            }
        }

        private void CharactersProcess()
        {
            try
            {
                while (true)
                {
                    CharacterMessage message = GetCharacterMessage();
                    if (message.Type == CharacterMessageType.SendCharacter)
                    {
                        if (message.Character != null)
                        {
                            Server.UploadCharacterToOwner(message);
                        }
                    }
                    else if (message.Type == CharacterMessageType.AcceptCharacters)
                    {
                        message.Characters = Server.GetAllCharacters();
                        message.Type = CharacterMessageType.SendCharacters;
                        string serialized = JsonConvert.SerializeObject(message);
                        byte[] data = Encoding.Unicode.GetBytes(serialized);
                        Stream.Write(data, 0, data.Length);
                    }
                    else if (message.Type == CharacterMessageType.AcceptCharacter)
                    {
                        if (message.CharacterOwner != null)
                        {
                            message = Server.GetCharacterByOwner(message);
                            message.Type = CharacterMessageType.SendCharacter;
                            string serialized = JsonConvert.SerializeObject(message);
                            byte[] data = Encoding.Unicode.GetBytes(serialized);
                            Stream.Write(data, 0, data.Length);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                Server.RemoveConnection(this.Id);
                Close();
            }
        }
        #endregion
        #region Gets

        private CharacterMessage GetCharacterMessage()
        {
            string serialized = GetSerializedString();
            return JsonConvert.DeserializeObject<CharacterMessage>(serialized);
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
            byte[] data = new byte[DATA_LENGTH];
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
        #endregion

        public void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (Client != null)
                Client.Close();
        }
    }
}
