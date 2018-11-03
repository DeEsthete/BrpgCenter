using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainPocket pocket;

        public MainWindow()
        {
            InitializeComponent();
            Closed += MainWindowClosed;
            pocket = new MainPocket
            {
                MainWindow = this,
                Context = new BrpgCenterContext()
            };

            pocket.LanguageManager.LanguageNames = LanguageManager.ReadFileLanguageList();
            //pocket.LanguageManager.AddNewLanguage();

            pocket.Rooms = pocket.Context.Rooms.Local;
            pocket.Characters = pocket.Context.Characters.Local;
            pocket.Player.CountCharactaers = pocket.Context.Characters.Count();
            pocket.Player.CountRooms = pocket.Context.Rooms.Count();
            //pocket.Context.Characters.Add(new Character());

            pocket.Player = ReadPlayerFile();
            if (pocket.Player == null)
            {
                pocket.Player = new Player();
            }

            if (pocket.Player.NickName == null)
            {
                Content = new ProfileEditPage(pocket);
            }
            else
            {
                Content = new MainMenuPage(pocket);
            }
        }

        private void MainWindowClosed(object sender, EventArgs e)
        {
            pocket.Context.SaveChanges();
        }

        #region dataWork
        public static void WritePlayerFile(Player pack)
        {
            string serialized = JsonConvert.SerializeObject(pack);
            using (FileStream fstream = new FileStream(Directory.GetCurrentDirectory() + @"\" + "PlayerInfo" + ".json", FileMode.OpenOrCreate))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(serialized);
                fstream.Write(array, 0, array.Length);
            }
        }
        public static Player ReadPlayerFile()
        {
            string json;
            try
            {
                using (FileStream fstream = File.OpenRead(Directory.GetCurrentDirectory() + @"\" + "PlayerInfo" + ".json"))
                {
                    byte[] array = new byte[fstream.Length];
                    fstream.Read(array, 0, array.Length);
                    string textFromFile = System.Text.Encoding.Default.GetString(array);
                    json = textFromFile;
                    return JsonConvert.DeserializeObject<Player>(json);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}