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
    [Serializable]
    public class Point2D : IShape
    {
        public double X { get; set; }
        public double Y { get; set; }

        public string Name => "Point";

        public Color Color = Colors.Black;
        public double StrokeThickness = 1;
        public double Border = 0;
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
                StrokeThickness = StrokeThickness,
                Stroke = new SolidColorBrush(Color),
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

        public void setValue(Color color, double strokeThickness, double border)
        {
            Color = color;
            StrokeThickness = strokeThickness;
            Border = border;
        }

        public void getValueSave(ref Color color, ref Point2D leftTop, ref Point2D rightBottom, ref double strokeThickness, ref double border)
        {
            throw new NotImplementedException();
        }

        public void setValueSave(ref Color color, ref Point2D leftTop, ref Point2D rightBottom, ref double strokeThickness, ref double border)
        {
            throw new NotImplementedException();
        }
    }
}
