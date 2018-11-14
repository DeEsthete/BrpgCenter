using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BrpgCenter
{
    class StorageClient : Client
    {
        public List<FileInfo> FileList { get; set; }

        public StorageClient(string address, int port, Player player, Character character) : base(address, port, player, character)
        {
            try
            {
                FirstMessage message = new FirstMessage(ClientType.FileClient, Player, Character);
                string serialized = JsonConvert.SerializeObject(message);
                byte[] data = Encoding.Unicode.GetBytes(serialized);
                Stream.Write(data, 0, data.Length);
                IsConnected = true;
                FileList = new List<FileInfo>();
            }
            catch (Exception)
            {
                IsConnected = false;
                MessageBox.Show("Хост не найден");
            }
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
                        FileMessage characterMessage = new FileMessage
                        {
                            Type = FileMessageType.AcceptFileList
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
                        FileMessage message = JsonConvert.DeserializeObject<FileMessage>(serialized);
                        if (message.Type == FileMessageType.SendFileList && message.FileList != null)
                        {
                            FileList = message.FileList;
                        }
                    }
                    catch (Exception)
                    {
                        Disconnect();
                    }
                }
            });
        }

        public async void DownloadFile(FileInfo file)
        {
            await DownloadFileWork(file);
        }

        private Task DownloadFileWork(FileInfo file)
        {
            return Task.Run(() =>
            {
                FileMessage messageForSend = new FileMessage
                {
                    Type = FileMessageType.AcceptThisFile,
                    FileInfo = file
                };
                string serializedForSend = JsonConvert.SerializeObject(messageForSend);
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
                FileMessage message = JsonConvert.DeserializeObject<FileMessage>(serialized);
                if (message.Type == FileMessageType.SendThisFile && message.FileContent != null)
                {
                    WriteFile(message.FileContent, message.FileInfo);
                }
            });
        }

        public async void UploadFile(FileInfo file)
        {
            await UploadFileWork(file);
        }

        private Task UploadFileWork(FileInfo file)
        {
            return Task.Run(() =>
            {
                FileMessage messageForSend = new FileMessage
                {
                    Type = FileMessageType.UploadFile,
                    FileInfo = file,
                    FileContent = ReadFile(file)
                };
                string serializedForSend = JsonConvert.SerializeObject(messageForSend);
                byte[] dataForSend = Encoding.Unicode.GetBytes(serializedForSend);
                Stream.Write(dataForSend, 0, dataForSend.Length);
            });
        }

        #region FileReadAndWrite
        public static void WriteFile(byte[] content, FileInfo pack)
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\" + "Downloads");
            using (FileStream fstream = new FileStream(Directory.GetCurrentDirectory() + @"\Downloads\" + pack.Name + pack.Extension, FileMode.CreateNew))
            {
                byte[] array = content;
                fstream.Write(array, 0, array.Length);
            }
        }
        public static byte[] ReadFile(FileInfo file)
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
        #endregion
    }
}
