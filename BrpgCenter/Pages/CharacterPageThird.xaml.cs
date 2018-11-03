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
    /// Логика взаимодействия для CharacterPageThird.xaml
    /// </summary>
    public partial class CharacterPageThird : Page
    {
        private MainPocket pocket;
        private Character character;
        public CharacterPageThird(MainPocket pocket, Character character)
        {
            InitializeComponent();
            this.pocket = pocket;
            this.character = character;

            raceTextBox.Text = character.Race;
            birthdayTextBox.Text = character.Birthday;
            advaDisTextBox.Text = character.AdvantagesDisadvantages;
            skillsTextBox.Text = character.Skills;
            equipTextBox.Text = character.Equip;
        }

        private void BeforeButtonClick(object sender, RoutedEventArgs e)
        {
            ApplyChanged();
            pocket.MainWindow.Content = new CharacterPageTwo(pocket, character);
        }

        private void AfterButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
            ApplyChanged();
            if (pocket.CurrentRoom != null)
            {
                pocket.MainWindow.Content = pocket.CurrentRoom;
            }
            else
            {
                pocket.MainWindow.Content = new CharactersPage(pocket);
            }
        }

        private void ApplyChanged()
        {
            try
            {
                character.Race = raceTextBox.Text;
                character.Birthday = birthdayTextBox.Text;
                character.AdvantagesDisadvantages = advaDisTextBox.Text;
                character.Skills = skillsTextBox.Text;
                character.Equip = equipTextBox.Text;
            }
            catch (Exception)
            {
                MessageBox.Show("Не все поля заполнены верно!");
            }
        }
    }
}
