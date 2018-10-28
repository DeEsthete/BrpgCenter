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

        public RoomPage(MainPocket pocket, Client client, Room room, bool isHost) // конструктор для клиента
        {
            InitializeComponent();
            this.pocket = pocket;
            this.client = client;
            this.room = room;
            this.isHost = isHost;

            client.Connect();
            client.AcceptChanged();
            client.AcceptFile();
            client.AcceptText();
            
            Timer addCharactersTimer = new Timer(new TimerCallback(AddCharactersToListBox), null, 5000, 5000);
            Timer addChatMessageTimer = new Timer(new TimerCallback(AddMessageToListBox), null, 1000, 1000);

            masterPlayerName.Text = room.GameMaster.NickName;
            
        }

        #region timerMethods
        public void AddCharactersToListBox(object obj)
        {
            Dispatcher.Invoke(() => playersListBox.Items.Clear());

            foreach (var i in client.AllCharactersInRoom)
            {
                Dispatcher.Invoke(() => playersListBox.Items.Add(i.FullName));
            }
        }

        public void AddMessageToListBox(object obj)
        {
            foreach (var i in client.Messages)
            {
                Dispatcher.Invoke(() => chatListBox.Items.Add(i.Content));
            }
            client.Messages.Clear();
        }
        #endregion

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            client.SendTextMessage(new ChatMessage(pocket.Player.NickName, messageFieldTextBox.Text));
        }

        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new RoomsPage(pocket);
        }
    }
}
