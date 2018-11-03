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
    /// Логика взаимодействия для ConnectRoomPage.xaml
    /// </summary>
    public partial class ConnectRoomPage : Page
    {
        private MainPocket pocket;

        public ConnectRoomPage(MainPocket pocket)
        {
            InitializeComponent();
            this.pocket = pocket;
        }

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.Context.Rooms.Add(new Room
            {
                Ip = ipTextBox.Text,
                Port = int.Parse(portTextBox.Text)
            });
            pocket.Rooms.Add(new Room
            {
                Ip = ipTextBox.Text,
                Port = int.Parse(portTextBox.Text)
            });
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
                Client client = new Client(pocket.Rooms.Last().Ip, pocket.Rooms.Last().Port, pocket.Player, character);
                if (client.IsConnected)
                {
                    pocket.MainWindow.Content = new RoomPage(pocket, client, pocket.Rooms.Last(), false, character);
                }
            }
            else
            {
                MessageBox.Show("Персонаж с таким id не найден");
            }
        }

        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new RoomsPage(pocket);
        }
    }
}
