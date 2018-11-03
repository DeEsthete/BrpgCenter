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
    /// Логика взаимодействия для CharactersPage.xaml
    /// </summary>
    public partial class CharactersPage : Page
    {
        private MainPocket pocket;
        public CharactersPage(MainPocket pocket)
        {
            InitializeComponent();
            this.pocket = pocket;
            pocket.Context.SaveChanges();
            foreach (var i in pocket.Context.Characters)
            {
                charactersListBox.Items.Add("Id: " + i.Id + " Имя: " + i.FullName);
            }
        }

        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new MainMenuPage(pocket);
        }

        private void CreateNewCharacterClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new CharacterPage(pocket);
        }

        private void EditCharacterClick(object sender, RoutedEventArgs e)
        {
            if (charactersListBox.SelectedIndex != -1)
            {
                pocket.MainWindow.Content = new CharacterPage(pocket, pocket.Characters[charactersListBox.SelectedIndex]);
            }
            else
            {
                MessageBox.Show("Персонаж не выбран!");
            }
        }
    }
}
