using Contract;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProjectPaint
{
    [Serializable]
    class Ellipse2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        private string _outlineColor = "#000000";
        private int _size;
        private double[] dashes = { };

        [NonSerialized]
        Ellipse Ellipse = null;
        public string Name => "Ellipse";
        public Point2D GetPoint1()
        {
            return _leftTop;
        }
        public Point2D GetPoint2()
        {
            return _rightBottom;
        }
        public void DrawMove(Canvas canvas)
        {
            if (Ellipse == null)
            {
                this.Draw();
                canvas.Children.Add(Ellipse);
            }

            var x = Math.Min(_rightBottom.X, _leftTop.X);
            var y = Math.Min(_rightBottom.Y, _leftTop.Y);

            var w = Math.Max(_rightBottom.X, _leftTop.X) - x;
            var h = Math.Max(_rightBottom.Y, _leftTop.Y) - y;

            Ellipse.Width = w;
            Ellipse.Height = h;

            Canvas.SetLeft(Ellipse, x);
            Canvas.SetTop(Ellipse, y);
        }

        public UIElement Draw()
        {
            Ellipse = new Ellipse()
            {
                Width = Math.Abs(_rightBottom.X - _leftTop.X),
                Height = Math.Abs(_rightBottom.Y - _leftTop.Y),
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_outlineColor)),
                StrokeThickness = _size,
                StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes)
            };
            Canvas.SetLeft(Ellipse, _leftTop.X);
            Canvas.SetTop(Ellipse, _leftTop.Y);

            return Ellipse;
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
        public String toString()
        {
            return Name + " " + _leftTop.toString() + " " + _rightBottom.toString();
        }

        public void setColor(Color color)
        {
            this._outlineColor = color.ToString();
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
    }
}