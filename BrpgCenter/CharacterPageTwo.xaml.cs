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
    /// Логика взаимодействия для CharacterPageTwo.xaml
    /// </summary>
    public partial class CharacterPageTwo : Page
    {
        private MainPocket pocket;
        private Character character;

        public CharacterPageTwo(MainPocket pocket, Character character)
        {
            InitializeComponent();
            this.pocket = pocket;
            this.character = character;
        }

        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
