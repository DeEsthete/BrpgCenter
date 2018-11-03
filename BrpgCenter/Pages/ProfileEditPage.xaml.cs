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
using Microsoft.Win32;

namespace BrpgCenter
{
    /// <summary>
    /// Логика взаимодействия для ProfileEditPage.xaml
    /// </summary>
    public partial class ProfileEditPage : Page
    {
        private MainPocket pocket;
        private Player player;

        public ProfileEditPage(MainPocket pocket)
        {
            InitializeComponent();
            this.pocket = pocket;
            this.player = pocket.Player;
            if (player.NickName == null)
            {
                player = new Player
                {
                    CountCharactaers = 0,
                    CountRooms = 0,
                    NickName = "Nickname",
                    PathToImage = "none"
                };
            }
            nickNameTextBox.Text = player.NickName;
        }

        private void ChangeIamgeClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Данная функция пока не реализована!");
        }

        private void SaveChangedClick(object sender, RoutedEventArgs e)
        {
            pocket.Player = player;
            pocket.MainWindow.Content = new MainMenuPage(pocket);
        }
    }
}
