using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BrpgCenter
{
    public class Paint
    {
        public Color Color { get; set; }
        public List<Memento> Mementos { get; set; }
        public List<Memento> RemovedMemento { get; set; }
        public Point CurrentPoint { get; set; }

        public Paint()
        {
            Color = Colors.Black;

            CurrentPoint = new Point();
            Mementos = new List<Memento>();
            RemovedMemento = new List<Memento>();

            Mementos.Add(new Memento(new List<Line>()));
        }
    }
}
