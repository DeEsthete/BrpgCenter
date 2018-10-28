using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для CreateRoomPage.xaml
    /// </summary>
    public partial class CreateRoomPage : Page
    {
        private MainPocket pocket;
        public CreateRoomPage(MainPocket pocket)
        {
            InitializeComponent();
            this.pocket = pocket;
        }

        private void CreateButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                pocket.Rooms.Add(new Room
                {
                    Name = nameTextBox.Text,
                    GameMaster = pocket.Player,
                    Ip = ipTextBox.Text,
                    Port = int.Parse(portTextBox.Text)
                });
                Room room = pocket.Rooms.Last();
                pocket.Server = new ServerObject(room.Ip, room.Port);
                pocket.Server.Listen();
                Client client = new Client(room.Ip, room.Port, pocket.Player);
                pocket.MainWindow.Content = new RoomPage(pocket, client, room, true);
            }
            catch (Exception)
            {
                MessageBox.Show("Не все поля заполнены верно!");
            }
        }

        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new RoomsPage(pocket);
        }
    }
}
