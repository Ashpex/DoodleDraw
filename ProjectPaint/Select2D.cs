using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProjectPaint
{
    [Serializable]
    class Select2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        private Point2D _leftTopForDraw = new Point2D();
        private string _outlineColor = "#000000";
        private int _size;
        private double[] dashes = { };
        public double width { get => rect == null ? 0 : rect.Width; }
        public double height { get => rect == null ? 0 : rect.Height; }

        [NonSerialized]
        Image img=null;
        [NonSerialized]
        Rectangle rect = null;
        public Point2D GetPoint1()
        {
            return _leftTop;
        }
        public Point2D GetPoint2()
        {
            return _rightBottom;
        }
        public string Name => "Select";
        public void setImage(Image img,double x, double y)
        {
            this.img = img;
            _leftTopForDraw = new Point2D() { X = x, Y = y };

        }
        public UIElement Draw()
        {
            if (img != null)
            {
                Canvas.SetLeft(img, _leftTopForDraw.X);
                Canvas.SetTop(img, _leftTopForDraw.Y);
                return img;
            }
            Rectangle replace = new Rectangle()
            {
                Width = Math.Abs(_leftTop.X - _rightBottom.X),
                Height = Math.Abs(_leftTop.Y - _rightBottom.Y),
                Stroke = Brushes.Transparent,
                StrokeThickness = 0,
                Fill = Brushes.White
            };

            Canvas.SetLeft(replace, _leftTop.X);
            Canvas.SetTop(replace, _leftTop.Y);
            
            return replace;
        }
        public UIElement DrawRect()
        {
            rect = new Rectangle()
            {
                Width = Math.Abs(_rightBottom.X - _leftTop.X),
                Height = Math.Abs(_rightBottom.Y - _leftTop.Y),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                StrokeDashArray = new System.Windows.Media.DoubleCollection(new double[] { 4, 4 })
            };

            System.Windows.Controls.Canvas.SetLeft(rect, _leftTop.X);
            Canvas.SetTop(rect, _leftTop.Y);

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
            return new Select2D();
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
            if (rect == null)
            {
                this.DrawRect();
                canvas.Children.Add(rect);
                
            }

            var x = Math.Min(_rightBottom.X, _leftTop.X);
            var y = Math.Min(_rightBottom.Y, _leftTop.Y);

            var w = Math.Max(_rightBottom.X, _leftTop.X) - x;
            var h = Math.Max(_rightBottom.Y, _leftTop.Y) - y;

            rect.Width = w;
            rect.Height = h;

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
        }
        public void RemoveOutline(Canvas canvas)
        {
            if (rect != null)
            {
                canvas.Children.Remove(rect);
            }
        }

        internal void ReplaceNewRectangle(Canvas canvas)
        {
            Rectangle replace = new Rectangle()
            {
                Width = rect.Width,
                Height = rect.Height,
                Stroke = Brushes.Transparent,
                StrokeThickness = 0,
                Fill = Brushes.White
            };
            Canvas.SetLeft(replace, _leftTop.X);
            Canvas.SetTop(replace, _leftTop.Y);

            canvas.Children.Add(replace);
        }
    }
}