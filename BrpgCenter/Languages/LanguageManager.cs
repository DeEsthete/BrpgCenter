using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class LanguageManager
    {
        public Dictionary<string, Language> Languages { get; set; }
        public List<string> LanguageNames { get; set; }
        public string CurrentLanguage { get; set; }

        public LanguageManager()
        {
            Languages = new Dictionary<string, Language>();
        }

        public void SetLanguage(string current, MainMenuPage window)
        {
            bool isTrue = false;

            foreach (var i in Languages)
            {
                if (i.Key == current)
                {
                    isTrue = true;
                }
            }
            if (!isTrue)
            {
                Languages.Add(current, ReadFileLanguage(current));
            }

            Languages[current].Apllying(window);
        }

        public void AddNewLanguage()
        {
            string languageName = "English";
            LanguageNames.Add(languageName);
            Language english = new Language();
            english.WordLibrary.Add("roomsButton", "Rooms");
            english.WordLibrary.Add("charactersButton", "Characters");
            english.WordLibrary.Add("literatureButton", "Literature");
            english.WordLibrary.Add("settingsButton", "Settings");
            english.WordLibrary.Add("exitButton", "Exit");
            english.WordLibrary.Add("profileSettingsButton", "Profile edit");
            english.WordLibrary.Add("countRoomsPredictionAnTextBlock", "Count rooms");
            english.WordLibrary.Add("countCharactersAnTextBlock", "Count characters");
            Languages.Add(languageName, english);
            WriteFileLanguage(english, languageName);
            WriteFileLanguageList(LanguageNames);
        }

        #region DataManagerMethods
        public static void WriteFileLanguage(Language pack, string name)
        {
            string serialized = JsonConvert.SerializeObject(pack);
            using (FileStream fstream = new FileStream(Directory.GetCurrentDirectory() + @"\" + name + ".json", FileMode.OpenOrCreate))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(serialized);
                fstream.Write(array, 0, array.Length);
            }
        }
        public static Language ReadFileLanguage(string name)
        {
            string json;

            using (FileStream fstream = File.OpenRead(Directory.GetCurrentDirectory() + @"\" + name + ".json"))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                json = textFromFile;
            }
            return JsonConvert.DeserializeObject<Language>(json);
        }

        public static void WriteFileLanguageList(List<string> vs)
        {
            string serialized = JsonConvert.SerializeObject(vs);
            using (FileStream fstream = new FileStream(Directory.GetCurrentDirectory() + @"\" + "Languages" + ".json", FileMode.OpenOrCreate))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(serialized);
                fstream.Write(array, 0, array.Length);
            }
        }
        public static List<string> ReadFileLanguageList()
        {
            try
            {
                string json;
                using (FileStream fstream = File.OpenRead(Directory.GetCurrentDirectory() + @"\" + "Languages" + ".json"))
                {
                    byte[] array = new byte[fstream.Length];
                    fstream.Read(array, 0, array.Length);
                    string textFromFile = System.Text.Encoding.Default.GetString(array);
                    json = textFromFile;
                }
                return JsonConvert.DeserializeObject<List<string>>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<string>();
            }
            
        }
        #endregion
    }
}
