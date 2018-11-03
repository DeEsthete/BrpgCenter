using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class ServerObject
    {
        public string Address { get; set; }
        public int Port { get; set; }
        public TcpListener TcpListener { get; set; } // сервер для прослушивания
        public List<ClientObject> Clients { get; set; } // все подключения

        public ServerObject(string address, int port)
        {
            Clients = new List<ClientObject>();
            Address = address;
            Port = port;
        }

        public void AddConnection(ClientObject clientObject)
        {
            Clients.Add(clientObject);
        }
        public void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject client = Clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null)
                Clients.Remove(client);
        }

        // прослушивание входящих подключений
        public async void Listen()
        {
            await ListenWork();
        }

        private Task ListenWork()
        {
            return Task.Run(() =>
            {
                try
                {
                    TcpListener = new TcpListener(IPAddress.Parse(Address), Port);
                    TcpListener.Start();
                    Console.WriteLine("Сервер запущен. Ожидание подключений...");

                    while (true)
                    {
                        TcpClient tcpClient = TcpListener.AcceptTcpClient();

                        ClientObject clientObject = new ClientObject(tcpClient, this);
                        Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                        clientThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Disconnect();
                }
            });
        }

        // трансляция сообщения подключенным клиентам
        public void BroadcastMessage(ChatMessage message, string id)
        {
            string serialized = JsonConvert.SerializeObject(message);
            byte[] data = Encoding.Unicode.GetBytes(serialized);
            for (int i = 0; i < Clients.Count; i++)
            {
                if (Clients[i].Id != id) // если id клиента не равно id отправляющего
                {
                    Clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
        }

        public void SendServerFirstMessage(ClientObject client)
        {
            ServerFirstMessage message = new ServerFirstMessage();
            for (int i = 0; i < Clients.Count; i++)
            {
                message.CharacterInRoom.Add(Clients[i].Character);
            }
            
            string serialized = JsonConvert.SerializeObject(message);
            byte[] data = Encoding.Unicode.GetBytes(serialized);
            client.Stream.Write(data, 0, data.Length); //передача данных
        }

        // отключение всех клиентов
        public void Disconnect()
        {
            TcpListener.Stop(); //остановка сервера

            for (int i = 0; i < Clients.Count; i++)
            {
                Clients[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }
    }
}
