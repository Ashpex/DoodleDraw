using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Circle2D
{
    class Circle2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        public string Name => "Circle";

        public Color Color = Colors.Black;
        public double StrokeThickness = 1;

       
        public UIElement Draw()
        {
            var circle = new Ellipse()
            {
                Width = Math.Abs(_rightBottom.X - _leftTop.X),
                Height = Math.Abs(_rightBottom.X - _leftTop.X),
                Stroke = new SolidColorBrush(Color),
                StrokeThickness = StrokeThickness
            };
            if (_leftTop.X - _rightBottom.X < 0 && _leftTop.Y - _rightBottom.Y < 0)
            {
                Canvas.SetLeft(circle, _leftTop.X);
                Canvas.SetTop(circle, _leftTop.Y);
            }
            else if (_leftTop.X - _rightBottom.X > 0 && _leftTop.Y - _rightBottom.Y > 0)
            {
                Canvas.SetLeft(circle, _rightBottom.X);
                Canvas.SetTop(circle, _rightBottom.Y);
            }
            else if (_leftTop.X - _rightBottom.X < 0 && _leftTop.Y - _rightBottom.Y > 0)
            {
                Canvas.SetLeft(circle, _leftTop.X);
                Canvas.SetTop(circle, _rightBottom.Y);
            }
            else if (_leftTop.X - _rightBottom.X > 0 && _leftTop.Y - _rightBottom.Y < 0)
            {
                Canvas.SetLeft(circle, _rightBottom.X);
                Canvas.SetTop(circle, _leftTop.Y);
            }

            return circle;
        }

        public void HandleStart(double x, double y)
        {
            _leftTop.X = x;
            _leftTop.Y = y;
        }

        public void HandleEnd(double x, double y)
        {
            _rightBottom.X = x;
            _rightBottom.Y = y;
        }

        public IShape Clone()
        {
            return new Circle2D();
        }

        public void  setValue(Color color, double strokeThickness)
        {
            Color = color;
            StrokeThickness = strokeThickness;
        }
    }
}
