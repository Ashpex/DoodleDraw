using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Square2D
{
    [Serializable]
    public class Square2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        public string Name => "Square";
        public System.Windows.Media.Color Color = Colors.Black;
        public double StrokeThickness = 1;
        public double Border = 0;
        public UIElement Draw()
        {
            Rectangle square;
            if (Border != 0)
            {
                square = new Rectangle()
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
                square = new Rectangle()
                {
                    Width = Math.Sqrt((Math.Pow(_rightBottom.X - _leftTop.X, 2) + Math.Pow(_rightBottom.Y - _leftTop.Y, 2)) / 2),
                    Height = Math.Sqrt((Math.Pow(_rightBottom.X - _leftTop.X, 2) + Math.Pow(_rightBottom.Y - _leftTop.Y, 2)) / 2),
                    Stroke = new SolidColorBrush(Color),
                    StrokeThickness = StrokeThickness,
                    StrokeDashArray = DoubleCollection.Parse(Border.ToString())
                };
            }

            double temp = Math.Sqrt((Math.Pow(_rightBottom.X - _leftTop.X, 2) + Math.Pow(_rightBottom.Y - _leftTop.Y, 2)) / 2);

            if (_rightBottom.X >= _leftTop.X)
            {
                if (_rightBottom.Y >= _leftTop.Y)
                {
                    Canvas.SetLeft(square, _leftTop.X);
                    Canvas.SetTop(square, _leftTop.Y);
                }
                else
                {
                    Canvas.SetLeft(square, _leftTop.X);
                    Canvas.SetTop(square, _leftTop.Y - temp);
                }
            }
            else
            {
                if (_rightBottom.Y >= _leftTop.Y)
                {
                    Canvas.SetLeft(square, _leftTop.X - temp);
                    Canvas.SetTop(square, _leftTop.Y);
                }
                else
                {
                    Canvas.SetLeft(square, _leftTop.X - temp);
                    Canvas.SetTop(square, _leftTop.Y - temp);
                }
            }
            return square;
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
        public void setValue(Color color, double strokeThickness, double border)
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
