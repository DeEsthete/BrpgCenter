using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BrpgCenter
{
    public class Language
    {
        public Dictionary<string, string> WordLibrary { get; set; }

        public Language()
        {
            WordLibrary = new Dictionary<string, string>();
        }

        public virtual void Apllying(MainMenuPage window)
        {
            //Смена языка по начальному названию
            (window.FindName("roomsButton") as Button).Content = WordLibrary["roomsButton"];
            (window.FindName("charactersButton") as Button).Content = WordLibrary["charactersButton"];
            (window.FindName("literatureButton") as Button).Content = WordLibrary["literatureButton"];
            (window.FindName("settingsButton") as Button).Content = WordLibrary["settingsButton"];
            (window.FindName("exitButton") as Button).Content = WordLibrary["exitButton"];
            (window.FindName("profileSettingsButton") as Button).Content = WordLibrary["profileSettingsButton"];
            (window.FindName("countRoomsPredictionAnTextBlock") as TextBlock).Text = WordLibrary["countRoomsPredictionAnTextBlock"];
            (window.FindName("countCharactersAnTextBlock") as TextBlock).Text = WordLibrary["countCharactersAnTextBlock"];
        }
    }
}
