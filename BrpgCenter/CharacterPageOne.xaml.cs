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
    /// Логика взаимодействия для CharacterPageOne.xaml
    /// </summary>
    public partial class CharacterPageOne : Page
    {
        private MainPocket pocket;
        public CharacterPageOne(MainPocket pocket)
        {
            InitializeComponent();
            this.pocket = pocket;
        }

        private void BeforeButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new CharacterPage(pocket);
        }
    }
}
