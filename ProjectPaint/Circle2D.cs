using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProjectPaint
{
    [Serializable]
    class Circle2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        private string _outlineColor = "#000000";
        private int _size;
        private double[] dashes = { };
        [NonSerialized()]
        Ellipse circle;

        public string Name => "Circle";
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
            circle = new Ellipse()
            {
                Width = Math.Min(Math.Abs(_rightBottom.X - _leftTop.X), Math.Abs(_rightBottom.Y - _leftTop.Y)),
                Height = Math.Min(Math.Abs(_rightBottom.X - _leftTop.X), Math.Abs(_rightBottom.Y - _leftTop.Y)),
                StrokeThickness = _size,
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_outlineColor)),
                StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes)
            };
            Canvas.SetLeft(circle, _leftTop.X);
            Canvas.SetTop(circle, _leftTop.Y);

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
            if (circle == null)
            {
                this.Draw();
                canvas.Children.Add(circle);
            }

            var x = Math.Min(_rightBottom.X, _leftTop.X);
            var y = Math.Min(_rightBottom.Y, _leftTop.Y);

            var w = Math.Max(_rightBottom.X, _leftTop.X) - x;
            var h = Math.Max(_rightBottom.Y, _leftTop.Y) - y;

            circle.Width = Math.Min(w,h);
            circle.Height = Math.Min(w, h);

            Canvas.SetLeft(circle, x);
            Canvas.SetTop(circle, y);
        }
    }
}