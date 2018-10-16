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

        public virtual void Apllying(MainWindow window)
        {
            //Смена языка по начальному названию
            (window.FindName("fattyButton") as Button).Content = WordLibrary["Fatty"];
            (window.FindName("italicsButton") as Button).Content = WordLibrary["Italics"];
            (window.FindName("underlinedButton") as Button).Content = WordLibrary["Underlined"];
            (window.FindName("saveButton") as Button).Content = WordLibrary["Save"];
        }
    }
}
