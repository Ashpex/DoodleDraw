using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Paint_
{
    public class TransparentToBool : IValueConverter
    {
        public object Convert(object value, System.Type targetType,
                                object parameter, System.Globalization.CultureInfo culture)
        {
            Color transColor = Colors.Transparent;
            Color color = (Color)value;

            if (color != null)
            {
                return (color.A == transColor.A &&
                    color.R == transColor.R &&
                    color.G == transColor.G &&
                    color.B == transColor.B);
            }

            return false;
        }

        public object ConvertBack(object value, System.Type targetType,
                                    object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
