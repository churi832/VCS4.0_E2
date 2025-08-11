using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.VHL.Library.Common
{
    public class TimeSpanColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = ((TimeSpan)value).TotalSeconds;

                if (input < 300)
                {
                    return Brushes.Transparent;
                }
                else if (input >= 300 && input < 900)
                {
                    return Brushes.Yellow;
                }
                else
                {
                    return Brushes.Pink;
                }
            }
            catch
            {
                return Brushes.Transparent;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
