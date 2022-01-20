
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Contract
{
    [Serializable]
    public class Point2D : IShape
    {
        public double X { get; set; }
        public double Y { get; set; }
        private string _outlineColor = "#000000";
        private int _size;
        private double[] dashes = { };
        [NonSerialized()]
        Line line = null;
        public Point2D GetPoint1()
        {
            return this;
        }
        public Point2D GetPoint2()
        {
            return this;
        }
        public string Name => "Point";

        public void HandleStart(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void HandleEnd(double x, double y)
        {
            X = x;
            Y = y;
        }

        public UIElement Draw()
        {
            line = new Line()
            {
                X1 = X,
                Y1 = Y,
                X2 = X,
                Y2 = Y,
                StrokeThickness = _size,
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_outlineColor)),
                StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes)
            };

            return line;
        }

        public IShape Clone()
        {
            return new Point2D();
        }

        public String toString()
        {
            return X.ToString() + " " + Y.ToString();
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
            throw new NotImplementedException();
        }
    }
}