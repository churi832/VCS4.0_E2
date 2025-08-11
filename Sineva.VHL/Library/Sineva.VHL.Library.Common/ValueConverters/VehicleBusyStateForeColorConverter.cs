using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.VHL.Library.Common
{
    public class VehicleBusyStateForeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (VehicleBusyStatus)value;
                switch (input)
                {
                    case VehicleBusyStatus.Idle:
                        return Brushes.LightGray;
                    case VehicleBusyStatus.Busy:
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
