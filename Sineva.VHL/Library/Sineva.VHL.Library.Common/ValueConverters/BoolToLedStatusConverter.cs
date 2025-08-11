using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.VHL.Library.Common
{
    public class BoolToLedStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!bool.TryParse(value.ToString(), out bool isTrue))
            {
                return new SolidColorBrush(Colors.White);
            }

            if (isTrue)
            {
                return new SolidColorBrush(Colors.Green);
            }
            else
            {
                return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush colorBrush = (SolidColorBrush)value;

            if (colorBrush.Color == Colors.Green)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
