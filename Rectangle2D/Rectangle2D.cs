using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rectangle2D
{
    class Rectangle2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        public string Name => "Rectangle";

        public UIElement Draw()
        {
            var rect = new Rectangle()
            {
                Width = Math.Abs(_rightBottom.X - _leftTop.X),
                Height = Math.Abs(_rightBottom.Y - _leftTop.Y),
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 1
            };

            System.Windows.Controls.Canvas.SetLeft(rect, _leftTop.X);
            Canvas.SetTop(rect, _leftTop.Y);

            return rect;
        }

        public void HandleStart(double x, double y)
        {
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleEnd(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
        }

        public IShape Clone()
        {
            return new Rectangle2D();
        }
    }
}
