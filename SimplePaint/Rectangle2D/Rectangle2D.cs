using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rectangle2D
{
    public class Rectangle2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        public string Name => "Rectangle";
        public System.Windows.Media.Color Color = Colors.Black;
        public double StrokeThickness = 1;
        public double Border = 0;
        public UIElement Draw()
        {
            Rectangle rect;
            if (Border != 0)
            {
                rect = new Rectangle()
                {
                    Width = Math.Abs(_rightBottom.X - _leftTop.X),
                    Height = Math.Abs(_rightBottom.Y - _leftTop.Y),
                    Stroke = new SolidColorBrush(Color),
                    StrokeThickness = StrokeThickness,
                    StrokeDashArray = DoubleCollection.Parse(Border.ToString())
                };
            }
            else
            {
                rect = new Rectangle()
                {
                    Width = Math.Abs(_rightBottom.X - _leftTop.X),
                    Height = Math.Abs(_rightBottom.Y - _leftTop.Y),
                    Stroke = new SolidColorBrush(Color),
                    StrokeThickness = StrokeThickness,
                   
                };
            }
            if (_leftTop.X - _rightBottom.X < 0 && _leftTop.Y - _rightBottom.Y < 0)
            {
                Canvas.SetLeft(rect, _leftTop.X);
                Canvas.SetTop(rect, _leftTop.Y);
            }
            else if (_leftTop.X - _rightBottom.X > 0 && _leftTop.Y - _rightBottom.Y > 0)
            {
                Canvas.SetLeft(rect, _rightBottom.X);
                Canvas.SetTop(rect, _rightBottom.Y);
            }
            else if (_leftTop.X - _rightBottom.X < 0 && _leftTop.Y - _rightBottom.Y > 0)
            {
                Canvas.SetLeft(rect, _leftTop.X);
                Canvas.SetTop(rect, _rightBottom.Y);
            }
            else if (_leftTop.X - _rightBottom.X > 0 && _leftTop.Y - _rightBottom.Y < 0)
            {
                Canvas.SetLeft(rect, _rightBottom.X);
                Canvas.SetTop(rect, _leftTop.Y);
            }

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
        public void setValue(Color color, double strokeThickness, double border)
        {
            Color = color;
            StrokeThickness = strokeThickness;
            Border = border;
        }
    }
}
