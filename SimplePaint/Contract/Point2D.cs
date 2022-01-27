using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Contract
{
    public class Point2D : IShape
    {
        public double X { get; set; }
        public double Y { get; set; }

        public string Name => "Point";

        public void HandleStart(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void HandleEnd(double x, double y)
        {
            X = x;
            Y = y;
        }

        public UIElement Draw()
        {
            Line l = new Line()
            {
                X1 = X,
                Y1 = Y,
                X2 = X,
                Y2 = Y,
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Red),
            };

            return l;
        }

        public IShape Clone()
        {
            return new Point2D();
        }

        UIElement IShape.Draw()
        {
            throw new NotImplementedException();
        }
    }
}
