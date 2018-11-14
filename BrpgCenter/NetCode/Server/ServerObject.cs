using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        public List<FileInfo> FileList { get; set; } //файлы находящиеся на сервере

        public ServerObject(string address, int port)
        {
            Clients = new List<ClientObject>();
            FileList = ReadFileList();
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

        #region ClientObjectMethods

        public void AddNewFile(FileInfo fileInfo, byte[] fileContent)
        {
            FileInfo newFile = WriteFile(fileContent, fileInfo);
            if (newFile != null)
            {
                FileList.Add(newFile);
            }

            WriteFileList(FileList);
        }

        public byte[] GetThisFile(FileInfo fileInfo)
        {
            bool isOk = false;
            foreach (var i in FileList)
            {
                if (i.FullName == fileInfo.FullName)
                {
                    isOk = true;
                }
            }

            if (isOk)
            {
                return ReadFile(fileInfo);
            }
            else
            {
                return null;
            }
        }

        public List<Character> GetAllCharacters()
        {
            List<Character> characters = new List<Character>();
            foreach (var i in Clients)
            {
                if (i.Type == ClientType.ChatClient)
                {
                    if (i.Character != null)
                    {
                        i.Character.Owner = i.Player;
                        characters.Add(i.Character);
                    }
                }
            }
            return characters;
        }

        public CharacterMessage GetCharacterByOwner(CharacterMessage message)
        {
            foreach (var i in Clients)
            {
                if (i.Player == message.CharacterOwner)
                {
                    message.Character = i.Character;
                }
            }
            return message;
        }

        public void UploadCharacterToOwner(CharacterMessage message)
        {
            foreach (var i in Clients)
            {
                if (i.Player.Id == message.CharacterOwner.Id)
                {
                    i.Character = message.Character;
                }
            }
        }

        public void BroadcastMessage(ChatMessage message, string id)
        {
            string serialized = JsonConvert.SerializeObject(message);
            byte[] data = Encoding.Unicode.GetBytes(serialized);
            for (int i = 0; i < Clients.Count; i++)
            {
                if (Clients[i].Id != id) // если id клиента не равно id отправляющего
                {
                    if (Clients[i].Type == ClientType.ChatClient)
                    {
                        Clients[i].Stream.Write(data, 0, data.Length);
                    }
                }
            }
        }

        public void SendState(ClientObject client)
        {
            List<Player> players = new List<Player>();
            for (int i = 0; i < Clients.Count; i++)
            {
                if (Clients[i].Type == ClientType.ChatClient && Clients[i].Player != null)
                {
                    players.Add(Clients[i].Player);
                }
            }

            string serialized = JsonConvert.SerializeObject(players);
            byte[] data = Encoding.Unicode.GetBytes(serialized);
            client.Stream.Write(data, 0, data.Length);
        }

        public void SendServerFirstMessage(ClientObject client)
        {
            ServerFirstMessage message = new ServerFirstMessage();
            for (int i = 0; i < Clients.Count; i++)
            {
                message.Players.Add(Clients[i].Player);
            }

            string serialized = JsonConvert.SerializeObject(message);
            byte[] data = Encoding.Unicode.GetBytes(serialized);
            client.Stream.Write(data, 0, data.Length);
        }
        #endregion

        #region FileReadAndWrite

        public FileInfo WriteFile(byte[] content, FileInfo pack)
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\" + "Room" + @"_files");
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\" + "Room" + @"_files\" + pack.Name + pack.Extension) == true)
            {
                using (FileStream fstream = new FileStream(Directory.GetCurrentDirectory() + @"\" + "Room" + @"_files\" + pack.Name + pack.Extension, FileMode.CreateNew))
                {
                    byte[] array = content;
                    fstream.Write(array, 0, array.Length);
                }
                FileInfo newFileInfo = new FileInfo(Directory.GetCurrentDirectory() + @"\" + "Room" + @"_files\" + pack.Name + pack.Extension);

                return newFileInfo;
            }
            else
            {
                return null;
            }
        }

        public byte[] ReadFile(FileInfo file)
        {
            try
            {
                using (FileStream fstream = File.OpenRead(file.FullName))
                {
                    byte[] array = new byte[fstream.Length];
                    fstream.Read(array, 0, array.Length);
                    return array;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void WriteFileList(List<FileInfo> vs)
        {
            string serialized = JsonConvert.SerializeObject(vs);
            using (FileStream fstream = new FileStream(Directory.GetCurrentDirectory() + @"\" + "Room" + @"_files\" + ".FilesInRoom" + ".json", FileMode.OpenOrCreate))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(serialized);
                fstream.Write(array, 0, array.Length);
            }
        }

        public List<FileInfo> ReadFileList()
        {
            try
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\" + "Room" + @"_files");
                string json;
                using (FileStream fstream = File.OpenRead(Directory.GetCurrentDirectory() + @"\" + "Room" + @"_files\" + ".FilesInRoom" + ".json"))
                {
                    byte[] array = new byte[fstream.Length];
                    fstream.Read(array, 0, array.Length);
                    string textFromFile = System.Text.Encoding.Default.GetString(array);
                    json = textFromFile;
                }
                return JsonConvert.DeserializeObject<List<FileInfo>>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<FileInfo>();
            }

        }
        #endregion

        public void Disconnect()
        {
            TcpListener.Stop();

            for (int i = 0; i < Clients.Count; i++)
            {
                Clients[i].Close();
            }
        }
    }
}
