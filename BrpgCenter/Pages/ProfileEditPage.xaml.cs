using System;
using System.Collections.Generic;
using System.IO;
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
        private string pathToCopy;

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

            if (player.PathToImage != "none" && player.PathToImage != null)
            {
                avatarImage.Source = new BitmapImage(new Uri(pocket.Player.PathToImage, UriKind.RelativeOrAbsolute));
            }
            nickNameTextBox.Text = player.NickName;
        }

        private void ChangeIamgeClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = "Картинки(*.JPG;*.PNG)|*.JPG;*.PNG" + "|Все файлы (*.*)|*.* ";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = true;
            if (myDialog.ShowDialog() == true)
            {
                string extension = System.IO.Path.GetExtension(myDialog.FileName);
                pathToCopy = Directory.GetCurrentDirectory() + @"\PlayerData\" + "PlayerAvatar" + extension;
                if (player.PathToImage != "none" && player.PathToImage != null)
                {
                    avatarImage.Source = new BitmapImage(new Uri(@"/BrpgCenter;component/Images/noAvatar.png", UriKind.RelativeOrAbsolute));
                    //File.Delete(player.PathToImage);
                }

                try
                {
                    File.Copy(myDialog.FileName, pathToCopy);
                }
                catch
                {
                    File.Delete(pathToCopy);
                    File.Copy(myDialog.FileName, pathToCopy);
                }
                if (pathToCopy != "none" && pathToCopy != null)
                {
                    avatarImage.Source = new BitmapImage(new Uri(pathToCopy, UriKind.RelativeOrAbsolute));
                }
            }
            else
            {
                MessageBox.Show("Во время выбора файла возникли некоторые ошибки, попробуйте еще раз!");
            }
        }

        private void SaveChangedClick(object sender, RoutedEventArgs e)
        {
            if (pathToCopy != null)
            {
                player.PathToImage = pathToCopy;
            }

            pocket.Player = player;
            pocket.MainWindow.Content = new MainMenuPage(pocket);
        }
    }
}
