using Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProjectPaint
{
    [Serializable]
    public class Line2D : IShape
    {
        private Point2D _start = new Point2D();
        private Point2D _end = new Point2D();
        private string _outlineColor = "#000000";
        private int _size;
        private double[] dashes = { };
        [NonSerialized()]
        Line line = null;
        public string Name => "Line";
        public Point2D GetPoint1()
        {
            return _start;
        }
        public Point2D GetPoint2()
        {
            return _end;
        }
        public void HandleStart(double x, double y)
        {
            _start = new Point2D() { X = x, Y = y };
        }

        public void HandleEnd(double x, double y)
        {
            _end = new Point2D() { X = x, Y = y };
        }

        public UIElement Draw()
        {
            line = new Line()
            {
                X1 = _start.X,
                Y1 = _start.Y,
                X2 = _end.X,
                Y2 = _end.Y,
                StrokeThickness = _size,
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_outlineColor)),
                StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes)
            };

            return line;
        }

        public IShape Clone()
        {
            return new Line2D();
        }
        public String toString()
        {
            return Name + " " + _start.toString() + " " + _end.toString();
        }

        public void setColor(Color color)
        {
            _outlineColor = color.ToString();
        }

        public void setWidth(int widthStroke)
        {
            _size = widthStroke;
        }
        public void DrawMove(Canvas canvas)
        {

            Debug.WriteLine("here2");

            if (line == null)
            {
                this.Draw();
                Debug.WriteLine("here");
                canvas.Children.Add(line);
            }
            line.X2 = _end.X;
            line.Y2 = _end.Y;

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