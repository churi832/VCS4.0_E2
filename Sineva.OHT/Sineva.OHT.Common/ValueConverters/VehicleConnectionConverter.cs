using System;
using System.Globalization;
using System.Windows.Data;

namespace Sineva.OHT.Common
{
    public class VehicleConnectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = bool.Parse(value.ToString());
                switch (input)
                {
                    case true:
                        return "Connected";
                    case false:
                        return "Disconnected";
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
