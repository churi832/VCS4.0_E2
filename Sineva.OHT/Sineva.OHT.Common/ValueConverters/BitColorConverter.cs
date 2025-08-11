using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.OHT.Common
{
    public class BitColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = bool.Parse(value.ToString());
                switch (input)
                {
                    case true:
                        return Brushes.LightGreen;
                    case false:
                        return Brushes.Transparent;
                    default:
                        return Brushes.Transparent;
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
