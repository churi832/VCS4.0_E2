using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.VHL.Library.Common
{
    public class PortStatusForeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (PortStatus)value;
                switch (input)
                {
                    case PortStatus.OutOfService:
                        return Brushes.LightGray;
                    case PortStatus.LDRQ:
                        return Brushes.Black;
                    case PortStatus.LDCM:
                        return Brushes.Black;
                    case PortStatus.UDRQ:
                        return Brushes.Black;
                    case PortStatus.UDCM:
                        return Brushes.Black;
                    default:
                        return Brushes.LightGray;
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
