using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Paint_
{
    class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, System.Type targetType,
                                object parameter, System.Globalization.CultureInfo culture)
        {
            bool IsChecked = (bool)value;

            if (IsChecked == true)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, System.Type targetType,
                                    object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
