using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для LiteraturePage.xaml
    /// </summary>
    public partial class LiteraturePage : Page
    {
        private MainPocket pocket;
        private Dictionary<string, string> links;
        public LiteraturePage(MainPocket pocket)
        {
            InitializeComponent();
            this.pocket = pocket;
            links = new Dictionary<string, string>
            {
                { "Gurps RU", "https://vk.com/doc337304320_438358894?hash=75cce73307c1a99b66&dl=5c25cdb37d00a7d0b1" },
                { "Руководство Ксанатара обо Всём", "https://dungeonsanddragons.ru/xanathars-gude-to-everything-%D1%80%D1%83%D0%BA%D0%BE%D0%B2%D0%BE%D0%B4%D1%81%D1%82%D0%B2%D0%BE-%D0%BA%D1%81%D0%B0%D0%BD%D0%B0%D1%82%D0%B0%D1%80%D0%B0-%D0%BE%D0%B1%D0%BE-%D0%B2%D1%81%D1%91%D0%BC/" },
                { "Гайд по фракциям Фаэруна", "https://dungeonsanddragons.ru/adventurers-league-factions-guide/" },
                { "Стихийное зло — Путеводитель игрока", "https://dungeonsanddragons.ru/elemental-evil-players-companion/" }
            };

            foreach (var i in links)
            {
                literatureListBox.Items.Add(i.Key);
            }
        }

        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new MainMenuPage(pocket);
        }

        private void GoLinkButtonClick(object sender, RoutedEventArgs e)
        {
            if (literatureListBox.SelectedIndex != -1)
            {
                Process.Start(links[literatureListBox.SelectedItem.ToString()]);
            }
        }
    }
}
