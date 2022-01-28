using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ellipse2D
{
    public class Ellipse2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        public string Name => "Ellipse";
        public Color Color = Colors.Black;
        public double StrokeThickness = 1;
        public double Border = 0;
        public UIElement Draw()
        {
            Ellipse ellipse;
            if (Border != 0)
            {
                ellipse = new Ellipse()
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
                ellipse = new Ellipse()
                {
                    Width = Math.Abs(_rightBottom.X - _leftTop.X),
                    Height = Math.Abs(_rightBottom.Y - _leftTop.Y),
                    Stroke = new SolidColorBrush(Color),
                    StrokeThickness = StrokeThickness,
                    
                };

            }
            if (_leftTop.X - _rightBottom.X < 0 && _leftTop.Y - _rightBottom.Y < 0)
            {
                Canvas.SetLeft(ellipse, _leftTop.X);
                Canvas.SetTop(ellipse, _leftTop.Y);
            }
            else if (_leftTop.X - _rightBottom.X > 0 && _leftTop.Y - _rightBottom.Y > 0)
            {
                Canvas.SetLeft(ellipse, _rightBottom.X);
                Canvas.SetTop(ellipse, _rightBottom.Y);
            }
            else if (_leftTop.X - _rightBottom.X < 0 && _leftTop.Y - _rightBottom.Y > 0)
            {
                Canvas.SetLeft(ellipse, _leftTop.X);
                Canvas.SetTop(ellipse, _rightBottom.Y);
            }
            else if (_leftTop.X - _rightBottom.X > 0 && _leftTop.Y - _rightBottom.Y < 0)
            {
                Canvas.SetLeft(ellipse, _rightBottom.X);
                Canvas.SetTop(ellipse, _leftTop.Y);
            }

            return ellipse;
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
            return new Ellipse2D();
        }
        public void setValue(Color color, double strokeThickness, double border)
        {
            Color = color;
            StrokeThickness = strokeThickness;
            Border = border;
        }
    }
}
