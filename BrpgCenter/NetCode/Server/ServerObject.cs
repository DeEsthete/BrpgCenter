using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BrpgCenter
{
    public class ServerObject
    {
        public TcpListener TcpListener { get; set; } // сервер для прослушивания
        public List<ClientObject> Clients { get; set; } // все подключения

        public ServerObject()
        {
            Clients = new List<ClientObject>();
        }

        public bool SearchPlayer(ClientObject client)   //существует ли уже ClintObject с таким Player, если да подключение закрепляется за ним
        {
            bool isTrue = false;
            foreach (var i in Clients)
            {
                if (i.Player.Id == client.Player.Id)
                {
                    isTrue = true;
                    if (i.ChatClient == null)
                    {
                        i.ChatClient = client.ChatClient;
                    }
                    if (i.FileClient == null)
                    {
                        i.FileClient = client.FileClient;
                    }
                    if (i.ChangedClient == null)
                    {
                        i.ChangedClient = client.ChangedClient;
                    }
                }
            }

            if (isTrue)
            {
                Clients.Remove(client);
            }

            return isTrue;
        }

        public void TransformTcpClientObject(ClientObject from, ClientObject to) //от куда //куда
        {
            if (to.ChatClient == null)
            {
                to.ChatClient = from.ChatClient;
            }
            if (to.FileClient == null)
            {
                to.FileClient = from.FileClient;
            }
            if (to.ChangedClient == null)
            {
                to.ChangedClient = from.ChangedClient;
            }
        }

        public void AddConnection(ClientObject clientObject)
        {
            Clients.Add(clientObject);
        }

        public void RemoveConnection(Guid id)
        {
            ClientObject client = Clients.FirstOrDefault(c => c.Player.Id == id);
            if (client != null)
                Clients.Remove(client);
        }

        // прослушивание входящих подключений
        public void Listen()
        {
            try
            {
                TcpListener = new TcpListener(IPAddress.Any, 8888);
                TcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClientObject tcpClientObject = new TcpClientObject();
                    TcpClient tcpClient = TcpListener.AcceptTcpClient();
                    tcpClientObject.TcpClient = tcpClient;

                    ClientObject clientObject = new ClientObject(tcpClientObject, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.ProcessStart));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // трансляция сообщения подключенным клиентам
        public void SendMessage(ChatMessage chatMessage, Guid id)
        {
            string serialized = JsonConvert.SerializeObject(chatMessage);
            byte[] data = Encoding.Unicode.GetBytes(serialized);
            for (int i = 0; i < Clients.Count; i++)
            {
                if (Clients[i].Player.Id != id) // если id клиента не равно id отправляющего
                {
                    foreach (var j in Clients)
                    {
                        j.ChatClient.Stream.Write(data, 0, data.Length);
                    }
                }
            }
        }

        public void SendAllChanged()
        {
            string serialized;
            byte[] data;
            foreach (var i in Clients)
            {
                serialized = JsonConvert.SerializeObject(i.Character);
                data = Encoding.Unicode.GetBytes(serialized);
                foreach (var j in Clients)
                {
                    j.ChangedClient.Stream.Write(data, 0, data.Length);
                }
            }
        }

        public void SendToOneClient(Guid id)
        {
            ClientObject client = Clients.FirstOrDefault(c => c.Player.Id == id);
            string serialized;
            byte[] data;
            foreach (var i in Clients)
            {
                serialized = JsonConvert.SerializeObject(i.Character);
                data = Encoding.Unicode.GetBytes(serialized);
                client.ChangedClient.Stream.Write(data, 0, data.Length);
            }
        }

        public void SendOneCharacterToAllClient(Guid id)
        {
            ClientObject client = Clients.FirstOrDefault(c => c.Player.Id == id);
            string serialized;
            byte[] data;
            serialized = JsonConvert.SerializeObject(client.Character);
            data = Encoding.Unicode.GetBytes(serialized);
            foreach (var i in Clients)
            {
                i.ChangedClient.Stream.Write(data, 0, data.Length);
            }
        }

        // отключение всех клиентов
        public void Disconnect()
        {
            TcpListener.Stop(); //остановка сервера

            for (int i = 0; i < Clients.Count; i++)
            {
                Clients[i].Close(); //отключение клиента
            }
        }
    }
}
