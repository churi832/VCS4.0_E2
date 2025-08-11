using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.VHL.Library.Common
{
    public class TimeSpanForeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = ((TimeSpan)value).TotalSeconds;

                if (input < 300)
                {
                    return Brushes.LightGray;
                }
                else if (input >= 300 && input < 900)
                {
                    return Brushes.Black;
                }
                else
                {
                    return Brushes.Black;
                }
            }
            catch
            {
                return Brushes.White;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
