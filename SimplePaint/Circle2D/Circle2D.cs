using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Circle2D
{
    [Serializable]
    class Circle2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        public string Name => "Circle";

        public Color Color = Colors.Black;
        public double StrokeThickness = 1;
        public double Border = 5;

       
        public UIElement Draw()
        {
            Ellipse circle;
            if (Border != 0)
            {
                circle = new Ellipse()
                {
                    Width = Math.Sqrt((Math.Pow(_rightBottom.X - _leftTop.X, 2) + Math.Pow(_rightBottom.Y - _leftTop.Y, 2)) / 2),
                    Height = Math.Sqrt((Math.Pow(_rightBottom.X - _leftTop.X, 2) + Math.Pow(_rightBottom.Y - _leftTop.Y, 2)) / 2),
                    Stroke = new SolidColorBrush(Color),
                    StrokeThickness = StrokeThickness,
                    StrokeDashArray = DoubleCollection.Parse(Border.ToString())
                };
            }
            else
            {
                circle = new Ellipse()
                {
                    Width = Math.Sqrt((Math.Pow(_rightBottom.X - _leftTop.X, 2) + Math.Pow(_rightBottom.Y - _leftTop.Y, 2)) / 2),
                    Height = Math.Sqrt((Math.Pow(_rightBottom.X - _leftTop.X, 2) + Math.Pow(_rightBottom.Y - _leftTop.Y, 2)) / 2),
                    Stroke = new SolidColorBrush(Color),
                    StrokeThickness = StrokeThickness,
                    
                };
            }

            double temp = Math.Sqrt((Math.Pow(_rightBottom.X - _leftTop.X, 2) + Math.Pow(_rightBottom.Y - _leftTop.Y, 2)) / 2);
            if (_rightBottom.X >= _leftTop.X)
            {
                if (_rightBottom.Y >= _leftTop.Y)
                {
                    Canvas.SetLeft(circle, _leftTop.X);
                    Canvas.SetTop(circle, _leftTop.Y);
                }
                else
                {
                    Canvas.SetLeft(circle, _leftTop.X);
                    Canvas.SetTop(circle, _leftTop.Y - temp);
                }
            }
            else
            {
                if (_rightBottom.Y >= _leftTop.Y)
                {
                    Canvas.SetLeft(circle, _leftTop.X - temp);
                    Canvas.SetTop(circle, _leftTop.Y);
                }
                else
                {
                    Canvas.SetLeft(circle, _leftTop.X - temp);
                    Canvas.SetTop(circle, _leftTop.Y - temp);
                }
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

        public void  setValue(Color color, double strokeThickness,double border)
        {
            Color = color;
            StrokeThickness = strokeThickness;
            Border = border;
        }

        public void getValueSave(ref Color color, ref Point2D leftTop, ref Point2D rightBottom, ref double strokeThickness, ref double border)
        {
            color = Color;
            leftTop = _leftTop;
            rightBottom = _rightBottom;
            strokeThickness = StrokeThickness;
            border = Border;
        }

        public void setValueSave(ref Color color, ref Point2D leftTop, ref Point2D rightBottom, ref double strokeThickness, ref double border)
        {
            _leftTop = leftTop;
            _rightBottom = rightBottom;
            Color = color;
            StrokeThickness = strokeThickness;
            Border = border;
        }
    }
}
