using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class StateClient : Client
    {
        public List<Player> PlayersInRoom { get; set; }

        public StateClient(string address, int port, Player player, Character character) : base(address, port, player, character)
        {
            FirstMessage message = new FirstMessage(ClientType.StateClient, Player, Character);
            string serialized = JsonConvert.SerializeObject(message);
            byte[] data = Encoding.Unicode.GetBytes(serialized);
            Stream.Write(data, 0, data.Length);
            IsConnected = true;
            PlayersInRoom = new List<Player>();
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
                        byte[] data = new byte[DATA_LENGTH];
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = Stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        } while (Stream.DataAvailable);

                        string serialized = builder.ToString();
                        PlayersInRoom = JsonConvert.DeserializeObject<List<Player>>(serialized);
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
