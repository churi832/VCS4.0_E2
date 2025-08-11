using System;
using System.Globalization;
using System.Windows.Data;

namespace Sineva.VHL.Library.Common
{
    public class IsNotNULLConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
