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

        public LanguageManager()
        {
            Languages = new Dictionary<string, Language>();
        }

        public void SetLanguage(string current, MainWindow window)
        {
            //string current = languageComboBox.SelectedItem as string;
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

        #region DataManagerMethods
        public static void WriteFileLanguage(Language pack, string name)
        {
            string serialized = JsonConvert.SerializeObject(pack);
            using (FileStream fstream = new FileStream(Directory.GetCurrentDirectory() + @"\" + name + ".json", FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                byte[] array = System.Text.Encoding.Default.GetBytes(serialized);
                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
            }
        }
        public static Language ReadFileLanguage(string name)
        {
            string json;
            using (FileStream fstream = File.OpenRead(Directory.GetCurrentDirectory() + @"\" + name + ".json"))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // считываем данные
                fstream.Read(array, 0, array.Length);
                // декодируем байты в строку
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
                // преобразуем строку в байты
                byte[] array = System.Text.Encoding.Default.GetBytes(serialized);
                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
            }
        }
        public static List<string> ReadFileLanguageList()
        {
            string json;
            using (FileStream fstream = File.OpenRead(Directory.GetCurrentDirectory() + @"\" + "Languages" + ".json"))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // считываем данные
                fstream.Read(array, 0, array.Length);
                // декодируем байты в строку
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                json = textFromFile;
            }
            return JsonConvert.DeserializeObject<List<string>>(json);
        }
        #endregion
    }
}
