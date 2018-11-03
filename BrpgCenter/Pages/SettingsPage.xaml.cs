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
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private MainPocket pocket;
        public SettingsPage(MainPocket pocket)
        {
            InitializeComponent();
            this.pocket = pocket;

            pocket.LanguageManager = new LanguageManager();
            pocket.LanguageManager.LanguageNames = LanguageManager.ReadFileLanguageList();

            languagesComboBox.Items.Add("Russian");
            for (int i = 0; i < pocket.LanguageManager.LanguageNames.Count; i++)
            {
                languagesComboBox.Items.Add(pocket.LanguageManager.LanguageNames[i]);
            }
        }

        private void SaveChangedClick(object sender, RoutedEventArgs e)
        {
            pocket.LanguageManager.CurrentLanguage = languagesComboBox.SelectedItem as string;

            if (languagesComboBox.SelectedItem as string != "Russian")
            {
                bool isTrue = false;

                foreach (var i in pocket.LanguageManager.Languages)
                {
                    if (i.Key == pocket.LanguageManager.CurrentLanguage)
                    {
                        isTrue = true;
                    }
                }
                if (!isTrue)
                {
                    pocket.LanguageManager.Languages.Add(pocket.LanguageManager.CurrentLanguage, LanguageManager.ReadFileLanguage(pocket.LanguageManager.CurrentLanguage));
                }
            }
            pocket.MainWindow.Content = new MainMenuPage(pocket);
        }
    }
}
