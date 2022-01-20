using Contract;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Line2D
{
    public class Line2D : IShape
    {
        Line l;
        private Point2D _start = new Point2D();
        private Point2D _end = new Point2D();
        private Color _outlineColor = new Color();

        public string Name => "Line";

        public void HandleStart(double x, double y)
        {
            _start = new Point2D() { X = x, Y = y };
        }

        public void HandleEnd(double x, double y)
        {
            _end = new Point2D() { X = x, Y = y };
        }

        public void setColor(Color color)
        {
            _outlineColor = color;
        }

        public UIElement Draw()
        {
            l = new Line();
            {
                X1 = _start.X,
                Y1 = _start.Y,
                X2 = _end.X,
                Y2 = _end.Y,
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(_outlineColor),
            };
            return l;
        }

        public IShape Clone()
        {
            return new Line2D();
        }
    }
}
