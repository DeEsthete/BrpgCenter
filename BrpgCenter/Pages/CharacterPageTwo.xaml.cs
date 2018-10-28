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

        public void ApplyChanged()
        {
            try
            {
                character.Move = int.Parse(mvTextBox.Text);
                character.Speed = int.Parse(spTextBox.Text);
                character.Will = int.Parse(wlTextBox.Text);
                character.Per = int.Parse(prTextBox.Text);
                character.FP = int.Parse(fpTextBox.Text);
                character.ST = int.Parse(stTextBox.Text);
                character.DX = int.Parse(dxTextBox.Text);
                character.IQ = int.Parse(iqTextBox.Text);
                character.HT = int.Parse(htTextBox.Text);
                character.HP = int.Parse(hpTextBox.Text);
                character.Wounds = woundsTextBox.Text;
                character.Fatigue = fatigueTextBox.Text;
            }
            catch (Exception)
            {
                MessageBox.Show("Не все поля заполнены верно!");
            }
        }

        private void BeforeButtonClick(object sender, RoutedEventArgs e)
        {
            ApplyChanged();
            pocket.MainWindow.Content = new CharacterPageOne(pocket, character);
        }

        private void AfterButtonClick(object sender, RoutedEventArgs e)
        {
            ApplyChanged();
            pocket.MainWindow.Content = new CharacterPageThird(pocket, character);
        }
    }
}
