using System;
using System.Globalization;
using System.Windows.Data;

namespace Sineva.VHL.Library.Common
{
    public class TimeSpanValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return ((TimeSpan)value).ToString(@"dd\.hh\:mm\:ss");
            }
            catch
            {
                return string.Empty;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
