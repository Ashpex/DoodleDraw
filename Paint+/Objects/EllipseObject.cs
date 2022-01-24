using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Paint_
{
    public class EllipseObject : BaseObject
    {
        public Ellipse rect;
        public UIElement Create()
        {
            rect = new Ellipse();
            rect.Width = 300;
            rect.Height = 300;
            Canvas.SetLeft(rect, 0);
            Canvas.SetTop(rect, 0);
            return rect;
        }
        public override void setColor(Color color)
        {
            rect.Stroke = new SolidColorBrush(color);
        }
        public override void setColorFill(Color color)
        {
            rect.Fill = new SolidColorBrush(color);
        }
        public override void setSize(double size)
        {
            rect.StrokeThickness = penWidth;
        }
        public override void setStyle(StyleLines style)
        {
            if (styleLine == StyleLines.Dash)
            {
                double[] dashes = { 4, 4 };
                rect.StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes);
            }
            else if (styleLine == StyleLines.Dot)
            {
                double[] dashes = { 1, 1 };
                rect.StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes);
            }
            else if (styleLine == StyleLines.DashDot)
            {
                double[] dashes = { 4, 1, 1, 1 };
                rect.StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes);
            }
            else if (styleLine == StyleLines.DashDotDot)
            {
                double[] dashes = { 4, 1, 1, 1, 1, 1 };
                rect.StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes);
            }
        }

    }
}
