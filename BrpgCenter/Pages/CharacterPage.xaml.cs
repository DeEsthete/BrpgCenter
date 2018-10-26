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
    /// Логика взаимодействия для CharacterPage.xaml
    /// </summary>
    public partial class CharacterPage : Page
    {
        private MainPocket pocket;
        private Character character;
        public CharacterPage(MainPocket pocket)
        {
            InitializeComponent();
            this.pocket = pocket;
            character = new Character();
        }

        private void BeforeButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void AfterButtonClick(object sender, RoutedEventArgs e)
        {
            ApplyChanged();
            pocket.MainWindow.Content = new CharacterPageOne(pocket);
            //
            //
            //
            //Current   ToDo
            //
            //
            //
        }

        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new CharactersPage(pocket);
        }

        private void ApplyChanged()
        {
            try
            {
                character.FullName = fullNameTextBox.Text;
                character.Race = raceTextBox.Text;
                character.Status = statusTextBox.Text;
                character.SkinColor = leatherTextBox.Text;
                character.WorldName = worldTextBox.Text;
                character.Age = int.Parse(ageTextBox.Text);
                character.MainNote = mainNoteTextBox.Text;
            }
            catch (Exception)
            {
                MessageBox.Show("Не все поля заполнены верно!");
            }
        }
    }
}
