using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProjectPaint
{
    [Serializable]
    class Square2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        private string _outlineColor = "#000000";
        private int _size;
        private double[] dashes = { };
        [NonSerialized()]
        Rectangle square;

        public string Name => "Square";
        public Point2D GetPoint1()
        {
            return _leftTop;
        }
        public Point2D GetPoint2()
        {
            return _rightBottom;
        }
        public UIElement Draw()
        {
            square = new Rectangle()
            {
                Width = Math.Min(Math.Abs(_rightBottom.X - _leftTop.X), Math.Abs(_rightBottom.Y - _leftTop.Y)),
                Height = Math.Min(Math.Abs(_rightBottom.X - _leftTop.X), Math.Abs(_rightBottom.Y - _leftTop.Y)),
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_outlineColor)),
                StrokeThickness = _size,
                StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes)

            };

            Canvas.SetLeft(square, _leftTop.X);
            Canvas.SetTop(square, _leftTop.Y);

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
        public String toString()
        {
            return Name + " " + _leftTop.toString() + " " + _rightBottom.toString();
        }

        public void setColor(Color color)
        {
            _outlineColor = color.ToString();
        }

        public void setWidth(int widthStroke)
        {
            _size = widthStroke;
        }

        public void setStyle(string style)
        {
            if (style == "Dash") dashes = new double[] { 4, 4 };
            else if (style == "Dot") dashes = new double[] { 1, 1 };
            else if (style == "Dash Dot") dashes = new double[] { 4, 1, 1, 1 };
            else if (style == "Dash Dot Dot") dashes = new double[] { 4, 1, 1, 1, 1, 1 };
            else dashes = new double[] { };

        }
        public void DrawMove(Canvas canvas)
        {
            if (square == null)
            {
                this.Draw();
                canvas.Children.Add(square);
            }

            var x = Math.Min(_rightBottom.X, _leftTop.X);
            var y = Math.Min(_rightBottom.Y, _leftTop.Y);

            var w = Math.Max(_rightBottom.X, _leftTop.X) - x;
            var h = Math.Max(_rightBottom.Y, _leftTop.Y) - y;

            square.Width = Math.Min(w,h);
            square.Height = Math.Min(w, h);

            Canvas.SetLeft(square, x);
            Canvas.SetTop(square, y);
        }
    }

}