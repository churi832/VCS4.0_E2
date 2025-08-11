using System;
using System.Globalization;
using System.Windows.Data;

namespace Sineva.OHT.Common
{
    public class AlarmTypeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((AlarmType)value)
            {
                case AlarmType.Alarm:
                    return "Alarm";
                case AlarmType.Warning:
                    return "Warning";
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
