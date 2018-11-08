using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class CharactersClient : Client
    {
        private const int SLEEP_TIME_COMMON = 2000;

        public List<Character> CharactersInRoom { get; set; }

        public CharactersClient(string address, int port, Player player, Character character) : base(address, port, player, character)
        {
            FirstMessage message = new FirstMessage(ClientType.CharactersClient, Player, Character);
            string serialized = JsonConvert.SerializeObject(message);
            byte[] data = Encoding.Unicode.GetBytes(serialized);
            Stream.Write(data, 0, data.Length);
            IsConnected = true;
            CharactersInRoom = new List<Character>();
        }

        public async void StartReceive()
        {
            await ReceiveWork();
        }

        private Task ReceiveWork()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(SLEEP_TIME_COMMON);
                        CharacterMessage characterMessage = new CharacterMessage
                        {
                            Type = CharacterMessageType.AcceptCharacters
                        };
                        string serializedForSend = JsonConvert.SerializeObject(characterMessage);
                        byte[] dataForSend = Encoding.Unicode.GetBytes(serializedForSend);
                        Stream.Write(dataForSend, 0, dataForSend.Length);

                        byte[] data = new byte[DATA_LENGTH];
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = Stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        } while (Stream.DataAvailable);

                        string serialized = builder.ToString();
                        CharacterMessage message = JsonConvert.DeserializeObject<CharacterMessage>(serialized);
                        if (message.Type == CharacterMessageType.SendCharacters && message.Characters != null)
                        {
                            CharactersInRoom = message.Characters;
                        }
                    }
                    catch (Exception)
                    {
                        Disconnect();
                    }
                }
            });
        }

        public void SendCharacterChanged(Character character)
        {
            CharacterMessage message = new CharacterMessage
            {
                Type = CharacterMessageType.SendCharacter,
                Character = character,
                CharacterOwner = character.Owner
            };
            string serialized = JsonConvert.SerializeObject(message);
            byte[] data = Encoding.Unicode.GetBytes(serialized);
            Stream.Write(data, 0, data.Length);
        }
    }
}
