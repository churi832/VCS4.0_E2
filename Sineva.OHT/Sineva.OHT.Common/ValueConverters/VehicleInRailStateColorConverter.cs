using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.OHT.Common
{
    public class VehicleInRailStateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (VehicleInRailStatus)value;
                switch (input)
                {
                    case VehicleInRailStatus.OutOfRail:
                        return Brushes.Transparent;
                    case VehicleInRailStatus.InRail:
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
