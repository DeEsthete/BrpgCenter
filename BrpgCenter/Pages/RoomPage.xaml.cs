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
        private ChatClient chatClient;
        private StateClient stateClient;

        public RoomPage(MainPocket pocket, ChatClient chatClient, Room room, bool isHost, Character character)
        {
            InitializeComponent();
            this.pocket = pocket;
            this.room = room;
            this.isHost = isHost;
            this.stateClient = new StateClient(room.Ip, room.Port, pocket.Player, character);
            this.chatClient = chatClient;
            
            AcceptServerFirstMessage();
            chatClient.ReceiveMessage();
            stateClient.StartReceive();
            ApplyPlayersInRoom();

            AddMessageToListBox();

            if (room.GameMaster != null)
            {
                masterPlayerName.Text = room.GameMaster.NickName;
            }
        }

        private async void AcceptServerFirstMessage()
        {
            chatClient.AcceptServerFirstMessage();
            if (chatClient.IsConnected)
            {
                await AcceptServerFirstMessageWork();
            }
        }

        private Task AcceptServerFirstMessageWork()
        {
            return Task.Run(() =>
            {
                ServerFirstMessage firstMessage = chatClient.ServerFirstMessage;
                bool serverMessageIsOk = false;
                while (!serverMessageIsOk)
                {
                    Thread.Sleep(500);
                    firstMessage = chatClient.ServerFirstMessage;
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

        private async void AddMessageToListBox()
        {
            if (chatClient.IsConnected)
            {
                await AddMessageToListBoxWork(null);
            }
        }

        private Task AddMessageToListBoxWork(object obj)
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    foreach (var i in chatClient.Messages)
                    {
                        Dispatcher.Invoke(() => chatListBox.Items.Insert(0,i.SenderName + ": " + i.Content));
                    }
                    chatClient.Messages.Clear();
                }
            });
        }

        private async void ApplyPlayersInRoom()
        {
            await ApplyPlayersInRoomWork();
        }

        private Task ApplyPlayersInRoomWork()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    Dispatcher.Invoke(() => playersListBox.Items.Clear());
                    foreach (var i in stateClient.PlayersInRoom)
                    {
                        Dispatcher.Invoke(() => playersListBox.Items.Insert(0, i.NickName));
                    }
                    stateClient.PlayersInRoom.Clear();
                    Thread.Sleep(5100);
                }
            });
        }
        #endregion

        #region ButtonMethods
        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            chatClient.SendMessage(new ChatMessage(pocket.Player.NickName, messageFieldTextBox.Text));
            chatListBox.Items.Add("Вы: " + messageFieldTextBox.Text);
            messageFieldTextBox.Text = "";
        }

        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new RoomsPage(pocket);
        }

        private void GoChatButtonClick(object sender, RoutedEventArgs e)
        {
            charactersGrid.Visibility = Visibility.Hidden;
            chatGrid.Visibility = Visibility.Visible;
        }
        #endregion

        private void goCharactersButton_Click(object sender, RoutedEventArgs e)
        {
            chatGrid.Visibility = Visibility.Hidden;
            charactersGrid.Visibility = Visibility.Visible;
        }
    }
}
