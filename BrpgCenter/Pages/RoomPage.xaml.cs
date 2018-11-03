using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BrpgCenter
{
    /// <summary>
    /// Логика взаимодействия для RoomPage.xaml
    /// </summary>
    public partial class RoomPage : Page
    {
        private MainPocket pocket;
        private Room room;
        private bool isHost;
        private Client client;

        public RoomPage(MainPocket pocket, Client client, Room room, bool isHost, Character character)
        {
            InitializeComponent();
            this.pocket = pocket;
            this.client = client;
            this.room = room;
            this.isHost = isHost;

            client.AcceptServerFirstMessage();
            AcceptServerFirstMessage();
            client.ReceiveMessage();

            AddMessageToListBox();

            if (room.GameMaster != null)
            {
                masterPlayerName.Text = room.GameMaster.NickName;
            }
        }

        private async void AcceptServerFirstMessage()
        {
            if (client.IsConnected)
            {
                await AcceptServerFirstMessageWork();
            }
        }

        private Task AcceptServerFirstMessageWork()
        {
            return Task.Run(() =>
            {
                ServerFirstMessage firstMessage = client.ServerFirstMessage;
                bool serverMessageIsOk = false;
                while (!serverMessageIsOk)
                {
                    Thread.Sleep(500);
                    firstMessage = client.ServerFirstMessage;
                    if (firstMessage != null)
                    {
                        serverMessageIsOk = true;
                        if (firstMessage.Master != null && firstMessage.Master.NickName != "")
                        {
                            Dispatcher.Invoke(() => masterPlayerName.Text = firstMessage.Master.NickName);
                        }
                    }
                }
                foreach (var i in firstMessage.CharacterInRoom)
                {
                    Dispatcher.Invoke(() => playersListBox.Items.Add("Id: " + i.Id + " Имя: " + i.FullName));
                }
            });
        }

        #region timerMethods

        public async void AddMessageToListBox()
        {
            if (client.IsConnected)
            {
                await AddMessageToListBoxWork(null);
            }
        }

        public Task AddMessageToListBoxWork(object obj)
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    foreach (var i in client.Messages)
                    {
                        //Dispatcher.Invoke(() => chatListBox.Items.Add(i.Content));
                        Dispatcher.Invoke(() => chatListBox.Items.Insert(0,i.SenderName + ": " + i.Content));
                    }
                    client.Messages.Clear();
                }
            });
        }
        #endregion

        #region ButtonMethods
        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            client.SendMessage(new ChatMessage(pocket.Player.NickName, messageFieldTextBox.Text));
        }

        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new RoomsPage(pocket);
        }

        private void GoChatButtonClick(object sender, RoutedEventArgs e)
        {
            chatGrid.Visibility = Visibility.Visible;
        }
        #endregion
    }
}
