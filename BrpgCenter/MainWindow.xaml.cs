using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            pocket = new MainPocket();
            pocket.MainWindow = this;

            pocket.Context = new BrpgCenterContext();

            pocket.Player = ReadPlayerFile();
            if (pocket.Player == null)
            {
                Content = new ProfileEditPage(pocket);
            }
            pocket.Player.CountCharactaers = pocket.Characters.Count;
            pocket.Player.CountRooms = pocket.Rooms.Count;

            Content = new MainMenuPage(pocket);
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
