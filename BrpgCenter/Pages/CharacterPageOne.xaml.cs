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
        private Character character;
        public CharacterPageOne(MainPocket pocket, Character character)
        {
            InitializeComponent();
            this.pocket = pocket;
            this.character = character;
        }

        private void BeforeButtonClick(object sender, RoutedEventArgs e)
        {
            ApplyChanged();
            pocket.MainWindow.Content = new CharacterPage(pocket);
        }

        private void AfterButtonClick(object sender, RoutedEventArgs e)
        {
            ApplyChanged();
            pocket.MainWindow.Content = new CharacterPageTwo(pocket, character);
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
                character.Eyes = eyeDescriptionTextBox.Text;
                character.Hair = hairTextBox.Text;
                character.MainHand = mainHandTextBox.Text;
                character.Religion = religionTextBox.Text;
                character.Gender = genderTextBox.Text;
                character.Growth = int.Parse(growthTextBox.Text);
                character.Weight = int.Parse(weightTextBox.Text);
                character.SM = int.Parse(smTextBox.Text);
                character.TL = int.Parse(tlTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Не все поля заданы верно!");
            }
        }
    }
}
