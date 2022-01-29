using Contract;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SimplePaint
{
    public class Data
    {
        public Point2D _leftTop = new Point2D();
        public Point2D _rightBottom = new Point2D();

        public string Name;

        public Color Color;
        public double StrokeThickness;
        public double Border;
    }
}
