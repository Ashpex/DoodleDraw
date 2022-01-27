using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Paint_
{
    public enum TypeShape
    {
        None,
        Rectangle,
        Line,
        Ellipse,
        PolyLine,
        PolyGon,
        Text,
        Fill,
        Eraser
    }
    /// <summary>
    /// Interaction logic for ItemLayer.xaml
    /// </summary>
    public partial class ItemLayer : UserControl
    {
        #region Members
        // Object properties
        public BaseObject objectBase;
        private TypeShape typeShape;

        public MouseButtonEventHandler MainCanvas_MouseDown;
        public MouseButtonEventHandler MainCanvas_MouseUp;
        public MouseEventHandler MainCanvas_MouseMove;

        #endregion

        #region Properties
        /// <summary>
        /// Selection flag
        /// </summary>
        public bool Selected
        {
            get
            {
                return objectBase.selected;
            }
            set
            {
                objectBase.selected = value;
                SelectHide();
                if (objectBase.selected == true) SelectShow();
                else SelectHide();
            }
        }

        /// <summary>
        /// ColorStroke
        /// </summary>
        public Color ColorStroke
        {
            get
            {
                return objectBase.colorStroke;
            }
            set
            {

                objectBase.setColor(value);
                objectBase.colorStroke = value;
                
            }
        }

        /// <summary>
        /// ColorFill
        /// </summary>
        public Color ColorFill
        {
            get
            {
                return objectBase.colorFill;
            }
            set
            {
                objectBase.colorFill = value;
                objectBase.setColorFill(value);
            }
        }

        /// <summary>
        /// Pen width
        /// </summary>
        public int PenWidth
        {
            get
            {
                return objectBase.penWidth;

            }
            set
            {
                objectBase.penWidth = value;
                objectBase.setSize(value);
            }
        }

        /// <summary>
        /// TypeShape
        /// </summary>
        public TypeShape Type
        {
            get
            {
                return typeShape;
            }
            set
            {
                typeShape = value;
            }
        }

        /// <summary>
        /// ChangePart
        /// </summary>
        public bool ChangePart
        {
            get
            {
                return objectBase.changePart;
            }
            set
            {
                objectBase.changePart = value;
            }
        }

        /// <summary>
        /// StyleLine
        /// </summary>
        public StyleLines StyleLine
        {
            get
            {
                return objectBase.styleLine;
            }
            set
            {
                objectBase.styleLine = value;
                objectBase.setStyle(value);
            }
        }

        /// <summary>
        /// RectText
        /// </summary>
        public string RectText
        {
            get
            {
                if (typeShape == TypeShape.Text)
                    return ((TextObject)objectBase).rect.Text;
                else
                    return "";
            }
            set
            {
                if (typeShape == TypeShape.Text)
                    ((TextObject)objectBase).rect.Text = value;
            }
        }

        /// <summary>
        /// RectFontSize
        /// </summary>
        public double RectFontSize
        {
            get
            {
                if (typeShape == TypeShape.Text)
                    return ((TextObject)objectBase).rect.FontSize;
                else
                    return -1;
            }
            set
            {
                if (typeShape == TypeShape.Text)
                    ((TextObject)objectBase).rect.FontSize = value;
            }
        }

        /// <summary>
        /// RectTextDecorations
        /// </summary>
        public TextDecorationCollection RectTextDecorations
        {
            get
            {
                if (typeShape == TypeShape.Text)
                    return ((TextObject)objectBase).rect.TextDecorations;
                else
                    return TextDecorations.Baseline;
            }
            set
            {
                if (typeShape == TypeShape.Text)
                    ((TextObject)objectBase).rect.TextDecorations = value;
            }
        }

        /// <summary>
        /// RectFontStyle
        /// </summary>
        public FontStyle RectFontStyle
        {
            get
            {
                if (typeShape == TypeShape.Text)
                    return ((TextObject)objectBase).rect.FontStyle;
                else
                    return FontStyles.Normal;
            }
            set
            {
                if (typeShape == TypeShape.Text)
                    ((TextObject)objectBase).rect.FontStyle = value;
            }
        }

        /// <summary>
        /// RectFontWeight
        /// </summary>
        public FontWeight RectFontWeight
        {
            get
            {
                if (typeShape == TypeShape.Text)
                    return ((TextObject)objectBase).rect.FontWeight;
                else
                    return FontWeights.Normal;
            }
            set
            {
                if (typeShape == TypeShape.Text)
                    ((TextObject)objectBase).rect.FontWeight = value;
            }
        }

        /// <summary>
        /// RectFontFamily
        /// </summary>
        public FontFamily RectFontFamily
        {
            get
            {
                if (typeShape == TypeShape.Text)
                    return ((TextObject)objectBase).rect.FontFamily;
                else
                    return null;
            }
            set
            {
                if (typeShape == TypeShape.Text)
                    ((TextObject)objectBase).rect.FontFamily = value;
            }
        }
        #endregion

        #region Delegate

        #endregion

        #region Contructor
        public ItemLayer()
        {
            InitializeComponent();
        }
        public ItemLayer(TypeShape type)
        {
            InitializeComponent();
            typeShape = type;
            if (type == TypeShape.Line)
            {
                objectBase = new LineObject();
            }
            else if (type == TypeShape.Rectangle)
            {
                objectBase = new RectangleObject();
            }
            else if (type == TypeShape.Ellipse)
            {
                objectBase = new EllipseObject();
            }
            else if (type == TypeShape.PolyLine)
            {
                objectBase = new PolyLineObject();
            }
            else if (type == TypeShape.PolyGon)
            {
                objectBase = new PolyGonObject();
            }
            else if (type == TypeShape.Text)
            {
                objectBase = new TextObject();
            }
        }
        #endregion

        #region Function
        public void Create()
        {
            if (typeShape == TypeShape.Rectangle)
            {
                UIElement rect = ((RectangleObject)objectBase).Create();
                ItemCanvas.Children.Add(rect);
                ItemCanvas.Background = null;
            }
            else if (typeShape == TypeShape.Ellipse)
            {
                UIElement rect = ((EllipseObject)objectBase).Create();
                ItemCanvas.Children.Add(rect);
                ItemCanvas.Background = null;
            }
            else if (typeShape == TypeShape.Line)
            {
                UIElement rect = ((LineObject)objectBase).Create();
                ItemCanvas.Children.Add(rect);
                ItemCanvas.Background = null;

                //Temp sử dụng cho Click
                Line Temp = new Line();
                Temp.Stroke = Brushes.Transparent;
                Temp.Fill = Brushes.Transparent;
                Temp.X1 = 0;
                Temp.X2 = this.Width;
                Temp.Y1 = 0;
                Temp.Y2 = this.Height;
                if (objectBase.penWidth < 10)
                    Temp.StrokeThickness = 10;
                else
                    Temp.StrokeThickness = objectBase.penWidth;
                Canvas.SetLeft(Temp, 0);
                Canvas.SetTop(Temp, 0);
                ItemCanvas.Children.Add(Temp);
            }
            else if (typeShape == TypeShape.PolyLine)
            {
                UIElement rect = ((PolyLineObject)objectBase).Create();
                ItemCanvas.Background = null;
                ItemCanvas.Children.Add(rect);
            }
            else if (typeShape == TypeShape.PolyGon)
            {
                UIElement rect = ((PolyGonObject)objectBase).Create();
                ItemCanvas.Background = null;
                ItemCanvas.Children.Add(rect);
            }
            else if (typeShape == TypeShape.Text)
            {
                TextBlock rect = ((TextObject)objectBase).Create();
                ItemCanvas.Background = Brushes.Transparent;
                ItemCanvas.Children.Add(rect);
            }
            //RotateTransform rotateTransform1 = new RotateTransform(45, 0.5, 0.5);

            //this.RenderTransform = rotateTransform1;
        }

        public bool GetMouseOver()
        {
            if (typeShape == TypeShape.Line)
            {
                return ((Line)ItemCanvas.Children[1]).IsMouseOver;
            }
            if (typeShape == TypeShape.PolyGon)
            {
                return ((Polygon)ItemCanvas.Children[0]).IsMouseOver;
            }
            return false;
        }

        public HitType GetPartOver()
        {
            if (typeShape == TypeShape.Line)
            {
                if ((ItemCanvas.Children[1]).IsMouseOver == true) return HitType.Body;
                for (int i = 2; i < ItemCanvas.Children.Count; ++i)
                {
                    if ((ItemCanvas.Children[i]).IsMouseOver == true)
                    {
                        Rectangle item = (Rectangle)ItemCanvas.Children[i];
                        return (HitType)item.Tag;
                    }
                }
                return HitType.None;
            }
            else if (typeShape != TypeShape.PolyLine && typeShape != TypeShape.PolyGon)
            {
                if ((ItemCanvas.Children[0]).IsMouseOver == true) return HitType.Body;
                for (int i = 1; i < ItemCanvas.Children.Count; ++i)
                {
                    if ((ItemCanvas.Children[i]).IsMouseOver == true)
                    {
                        Rectangle item = (Rectangle)ItemCanvas.Children[i];
                        return (HitType)item.Tag;
                    }
                }
                return HitType.None;
            }
            else return HitType.None;
        }

        private Rectangle CreateDot(double Left, double Right)
        {
            Rectangle rectTopLeft = new Rectangle();
            rectTopLeft.Fill = new SolidColorBrush(Colors.Black);
            rectTopLeft.Width = 6;
            rectTopLeft.Height = 6;
            rectTopLeft.MouseLeftButtonDown += MainCanvas_MouseDown;
            rectTopLeft.MouseMove += MainCanvas_MouseMove;
            rectTopLeft.MouseUp += MainCanvas_MouseUp;
            Canvas.SetLeft(rectTopLeft, Left);
            Canvas.SetTop(rectTopLeft, Right);
            return rectTopLeft;
        }

        public void SelectShow()
        {
            if (typeShape == TypeShape.PolyLine || typeShape == TypeShape.PolyGon) return;

            if (objectBase.changePart == false)
            {
                Rectangle rectTopLeft = CreateDot(-3, -3);
                rectTopLeft.Tag = HitType.UL;
                ItemCanvas.Children.Add(rectTopLeft);
            }

            if (this.Type != TypeShape.Line || objectBase.changePart == true)
            {
                Rectangle rectTopRight = CreateDot(this.Width - 3, -3);
                rectTopRight.Tag = HitType.UR;
                ItemCanvas.Children.Add(rectTopRight);
            }

            if (this.Type != TypeShape.Line)
            {
                Rectangle rectTopMid = CreateDot(this.Width / 2, -3);
                rectTopMid.Tag = HitType.T;
                ItemCanvas.Children.Add(rectTopMid);
            }

            if (this.Type != TypeShape.Line)
            {
                Rectangle rectLeftMid = CreateDot(-3, this.Height / 2 - 3);
                rectLeftMid.Tag = HitType.L;
                ItemCanvas.Children.Add(rectLeftMid);
            }
            if (this.Type != TypeShape.Line)
            {
                Rectangle rectRightMid = CreateDot(this.Width - 3, this.Height / 2 - 3);
                rectRightMid.Tag = HitType.R;
                ItemCanvas.Children.Add(rectRightMid);
            }

            if (this.Type != TypeShape.Line || objectBase.changePart == true)
            {
                Rectangle rectBotLeft = CreateDot(-3, this.Height - 3);
                rectBotLeft.Tag = HitType.LL;
                ItemCanvas.Children.Add(rectBotLeft);
            }
            if (objectBase.changePart == false)
            {
                Rectangle rectBotRight = CreateDot(this.Width - 3, this.Height - 3);
                rectBotRight.Tag = HitType.LR;
                ItemCanvas.Children.Add(rectBotRight);
            }

            if (this.Type != TypeShape.Line)
            {
                Rectangle rectBotMid = CreateDot(this.Width / 2, this.Height - 3);
                rectBotMid.Tag = HitType.B;
                ItemCanvas.Children.Add(rectBotMid);
            }

            //Rectangle rectRotate = CreateDot();
            //Canvas.SetLeft(rectRotate, this.Width / 2 - 3);
            //Canvas.SetTop(rectRotate, -15);
            //ItemCanvas.Children.Add(rectRotate);
        }

        public void SelectHide()
        {
            int end = 1;
            if (TypeShape.Line == typeShape)
            {
                end = 2;
            }
            for (int i = ItemCanvas.Children.Count; i > end; i--)
            {
                ItemCanvas.Children.Remove(ItemCanvas.Children[i - 1]);
            }
        }

        private void ItemCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (typeShape == TypeShape.Rectangle)
            {
                ((Rectangle)ItemCanvas.Children[0]).Width = this.Width;
                ((Rectangle)ItemCanvas.Children[0]).Height = this.Height;
            }
            else if (typeShape == TypeShape.Ellipse)
            {
                ((Ellipse)ItemCanvas.Children[0]).Width = this.Width;
                ((Ellipse)ItemCanvas.Children[0]).Height = this.Height;
            }
            else if (typeShape == TypeShape.Line)
            {
                if (objectBase.changePart == false)
                {

                    ((Line)ItemCanvas.Children[0]).X1 = 0;
                    ((Line)ItemCanvas.Children[0]).X2 = this.Width;
                    ((Line)ItemCanvas.Children[0]).Y1 = 0;
                    ((Line)ItemCanvas.Children[0]).Y2 = this.Height;

                    ((Line)ItemCanvas.Children[1]).X1 = 0;
                    ((Line)ItemCanvas.Children[1]).X2 = this.Width;
                    ((Line)ItemCanvas.Children[1]).Y1 = 0;
                    ((Line)ItemCanvas.Children[1]).Y2 = this.Height;
                }
                else
                {
                    ((Line)ItemCanvas.Children[0]).X1 = 0;
                    ((Line)ItemCanvas.Children[0]).X2 = this.Width;
                    ((Line)ItemCanvas.Children[0]).Y1 = this.Height;
                    ((Line)ItemCanvas.Children[0]).Y2 = 0;

                    ((Line)ItemCanvas.Children[1]).X1 = 0;
                    ((Line)ItemCanvas.Children[1]).X2 = this.Width;
                    ((Line)ItemCanvas.Children[1]).Y1 = this.Height;
                    ((Line)ItemCanvas.Children[1]).Y2 = 0;
                }
            }
            else if (typeShape == TypeShape.Text)
            {
                ((TextObject)objectBase).rect.Width = this.Width;
                ((TextObject)objectBase).rect.Height = this.Height;
            }
            if (objectBase.selected == true)
            {
                SelectHide();
                UpdateLayout();
                SelectShow();
            }
        }

        #endregion
    }
}
