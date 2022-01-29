using Contract;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Line2D
{
    [Serializable]
    public class Line2D : IShape
    {
        private Point2D _start = new Point2D();
        private Point2D _end = new Point2D();

        public string Name => "Line";
        public System.Windows.Media.Color Color = Colors.Black;
        public double StrokeThickness = 1;
        public double Border = 0;
        public void HandleStart(double x, double y)
        {
            _start = new Point2D() { X = x, Y = y };
        }

        public void HandleEnd(double x, double y)
        {
            _end = new Point2D() { X = x, Y = y };
        }

        public UIElement Draw()
        {
            Line l;
            if (Border != 0)
            {
                l = new Line()
                {
                    X1 = _start.X,
                    Y1 = _start.Y,
                    X2 = _end.X,
                    Y2 = _end.Y,
                    StrokeThickness = StrokeThickness,
                    Stroke = new SolidColorBrush(Color),
                    StrokeDashArray = DoubleCollection.Parse(Border.ToString())
                };
            }
            else
            {
                l = new Line()
                {
                    X1 = _start.X,
                    Y1 = _start.Y,
                    X2 = _end.X,
                    Y2 = _end.Y,
                    StrokeThickness = StrokeThickness,
                    Stroke = new SolidColorBrush(Color),
                    
                };
            }

            return l;
        }

        public IShape Clone()
        {
            return new Line2D();
        }
        public void setValue(Color color, double strokeThickness, double border)
        {
            Color = color;
            StrokeThickness = strokeThickness;
            Border = border;
        }

        public void getValueSave(ref Color color, ref Point2D leftTop, ref Point2D rightBottom, ref double strokeThickness, ref double border)
        {
            color = Color;
            leftTop = _start;
            rightBottom = _end;
            strokeThickness = StrokeThickness;
            border = Border;
        }

        public void setValueSave(ref Color color, ref Point2D leftTop, ref Point2D rightBottom, ref double strokeThickness, ref double border)
        {
            _start = leftTop;
            _end = rightBottom;
            Color = color;
            StrokeThickness = strokeThickness;
            Border = border;
        }
    }
}
