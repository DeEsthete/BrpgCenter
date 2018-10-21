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
    /// Логика взаимодействия для ProfileEditPage.xaml
    /// </summary>
    public partial class ProfileEditPage : Page
    {
        private MainPocket pocket;
        public ProfileEditPage(MainPocket pocket)
        {
            InitializeComponent();
            nickNameTextBox.Text = pocket.Player.NickName;
        }

        private void ChangeIamgeClick(object sender, RoutedEventArgs e)
        {

        }

        private void SaveChangedClick(object sender, RoutedEventArgs e)
        {
            pocket.Player.NickName = nickNameTextBox.Text;
        }
    }
}
