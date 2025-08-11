using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.VHL.Library.Common
{
    public class IntegerBitColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = System.Convert.ToInt32(value);
                switch (input)
                {
                    case 0:
                        return Brushes.Transparent;
                    case 1:
                        return Brushes.LightGreen;
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
