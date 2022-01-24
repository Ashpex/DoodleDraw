using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Paint_
{

    public enum StyleLines
    {
        Soild,
        Dash,
        Dot,
        DashDot,
        DashDotDot
    };
    public class BaseObject
    {
        #region Members
        // Object properties
        public  bool selected = false;
        public Color colorStroke;
        public Color colorFill;
        public int penWidth = 1;
        public Brush brush;
        public bool changePart;
        public Point StartPoint;
        public Point EndPoint;
        public StyleLines styleLine;
        #endregion

        public virtual UIElement Create() { return null; }
        public virtual void setColor(Color color) { }
        public virtual void setColorFill(Color color) { }
        public virtual void setSize(double size) { }
        public virtual void setStyle(StyleLines style) { }

    }
}
