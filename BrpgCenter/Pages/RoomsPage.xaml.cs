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
            foreach (var i in pocket.Context.Rooms)
            {
                roomsListBox.Items.Add("Id: " + i.Id + "Ip: " + i.Ip + "Port: " + i.Port);
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
            if (roomsListBox.SelectedIndex != -1)
            {
                List<Room> rooms = pocket.Context.Rooms.ToList();
                ConnectRoom(rooms[roomsListBox.SelectedIndex]);
            }
            else
            {
                MessageBox.Show("Комната не выбрана!");
            }
        }

        private void ConnectRoom(Room room)
        {
            Character character = null;
            try
            {
                foreach (var i in pocket.Context.Characters)
                {
                    if (i.Id == int.Parse(characterIdTextBox.Text))
                    {
                        character = i;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не все поля заполнены верно!");
            }

            if (character != null)
            {
                Client client = new Client(room.Ip, room.Port, pocket.Player, character);
                if (client.IsConnected)
                {
                    pocket.MainWindow.Content = new RoomPage(pocket, client, room, false, character);
                }
            }
            else
            {
                MessageBox.Show("Персонажа с таким id не найден!");
            }
        }

        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new MainMenuPage(pocket);
        }
    }
}
