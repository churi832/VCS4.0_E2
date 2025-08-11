using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.OHT.Common
{
    public class PortStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (PortStatus)value;
                switch (input)
                {
                    case PortStatus.OutOfService:
                        return Brushes.Red;
                    case PortStatus.LDRQ:
                        return Brushes.Yellow;
                    case PortStatus.LDCM:
                        return Brushes.LightGreen;
                    case PortStatus.UDRQ:
                        return Brushes.Pink;
                    case PortStatus.UDCM:
                        return Brushes.LightBlue;
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
