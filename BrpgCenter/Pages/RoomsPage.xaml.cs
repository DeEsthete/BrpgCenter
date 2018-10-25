using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace BrpgCenter
{
    /// <summary>
    /// Логика взаимодействия для RoomsPage.xaml
    /// </summary>
    public partial class RoomsPage : Page
    {
        private MainPocket pocket;

        public RoomsPage(MainPocket pocket)
        {
            InitializeComponent();
            this.pocket = pocket;

            //добавление комнат
            foreach (var i in pocket.Rooms)
            {
                if (i.GameMaster.Id == pocket.Player.Id)
                {
                    createdRoomsListBox.Items.Add(i);
                }
                else
                {
                    connectedRoomsListBox.Items.Add(i);
                }
            }
        }

        private void ConnectToNewRoomButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new ConnectRoomPage(pocket);
        }

        private void CreateNewRoomButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new CreateRoomPage(pocket);
        }

        private void ConnectToRoomButtonClick(object sender, RoutedEventArgs e)
        {
            if (connectedRoomsListBox.SelectedIndex != -1)
            {
                ConnectRoom(connectedRoomsListBox.SelectedItem as Room);
            }
            else if (createdRoomsListBox.SelectedIndex != -1)
            {
                ConnectRoom(createdRoomsListBox.SelectedItems as Room);
            }
            else
            {
                MessageBox.Show("Комната не выбрана!");
            }
        }

        private void ConnectRoom(Room room)
        {
            Client client = new Client(room.Ip, room.PortChat, room.PortFile, room.PortChanged);
            pocket.MainWindow.Content = new RoomPage(pocket, client, room);
        }

        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new MainMenuPage(pocket);
        }
    }
}
