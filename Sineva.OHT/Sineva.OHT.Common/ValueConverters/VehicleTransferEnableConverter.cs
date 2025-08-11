using System;
using System.Globalization;
using System.Windows.Data;

namespace Sineva.OHT.Common
{
    public class VehicleTransferEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = System.Convert.ToInt32(value);
                switch (input)
                {
                    case 0:
                        return "Disabled";
                    case 1:
                        return "Enabled";
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
