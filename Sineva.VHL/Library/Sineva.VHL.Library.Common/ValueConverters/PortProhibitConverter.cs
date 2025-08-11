using System;
using System.Globalization;
using System.Windows.Data;

namespace Sineva.VHL.Library.Common
{
    public class PortProhibitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = System.Convert.ToInt32(value);
                switch (input)
                {
                    case 1:
                        return "Prohibitted";
                    case 0:
                        return "Released";
                    default:
                        return string.Empty;
                }
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
