using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace BrpgCenter
{
    public class Memento
    {
        public List<Line> Lines { get; set; }

        public Memento(List<Line> lines)
        {
            Lines = new List<Line>();
            foreach (var line in lines)
            {
                Line copy = new Line
                {
                    X1 = line.X1,
                    X2 = line.X2,

                    Y1 = line.Y1,
                    Y2 = line.Y2,
                    Stroke = line.Stroke
                };

                Lines.Add(copy);
            }
        }
    }
}
