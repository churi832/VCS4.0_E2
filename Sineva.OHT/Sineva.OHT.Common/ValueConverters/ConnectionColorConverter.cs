using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.OHT.Common
{
    public class ConnectionColorConverter : IValueConverter
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
                        return Brushes.LightPink;
                    default:
                        return Brushes.LightPink;
                }
            }
            catch
            {
                return Brushes.Red;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
