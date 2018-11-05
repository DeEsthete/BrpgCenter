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
                Room room = new Room
                {
                    Name = nameTextBox.Text,
                    GameMaster = pocket.Player,
                    Ip = ipTextBox.Text,
                    Port = int.Parse(portTextBox.Text)
                };
                pocket.Context.Rooms.Add(room);

                pocket.Server = new ServerObject(room.Ip, room.Port);
                pocket.Server.Listen();
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
                    ChatClient client = new ChatClient(room.Ip, room.Port, pocket.Player, character);
                    pocket.MainWindow.Content = new RoomPage(pocket, client, room, true, character);
                }
                else
                {
                    MessageBox.Show("Персонаж с таким id не найден");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("Не все поля заполнены верно!");
            }
        }

        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new RoomsPage(pocket);
        }
    }
}
