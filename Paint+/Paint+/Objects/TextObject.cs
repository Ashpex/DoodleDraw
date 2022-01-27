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
    public class TextObject : BaseObject
    {
        public TextBlock rect;

        public TextBlock Create()
        {
            rect = new TextBlock();

            rect.TextWrapping = TextWrapping.Wrap;
 
            //rect.Foreground = new SolidColorBrush(colorStroke);

            Canvas.SetLeft(rect, 0);

            Canvas.SetTop(rect, 0);

            return rect;
        }
        public override void setColor(Color color)
        {
            rect.Foreground = new SolidColorBrush(color);
        }


    }
}
