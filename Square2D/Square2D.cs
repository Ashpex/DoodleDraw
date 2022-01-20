using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Square2D
{
    class Square2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        public string Name => "Square";

        public UIElement Draw()
        {
            var rect = new Rectangle()
            {
                Width = Math.Min(Math.Abs(_rightBottom.X - _leftTop.X), Math.Abs(_rightBottom.Y - _leftTop.Y)),
                Height = Math.Min(Math.Abs(_rightBottom.X - _leftTop.X), Math.Abs(_rightBottom.Y - _leftTop.Y)),
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 1
            };

            Canvas.SetLeft(rect, _leftTop.X);
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
            return new Square2D();
        }
    }

}
