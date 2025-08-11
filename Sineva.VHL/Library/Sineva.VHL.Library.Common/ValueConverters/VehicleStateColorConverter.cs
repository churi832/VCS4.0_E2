using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.VHL.Library.Common
{
    public class VehicleStateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (VehicleState)value;
                switch (input)
                {
                    case VehicleState.Removed:
                        return Brushes.Red;
                    case VehicleState.NotAssigned:
                    case VehicleState.Parked:
                        return Brushes.LightYellow;
                    case VehicleState.EnRouteToSource:
                    case VehicleState.EnRouteToDest:
                    case VehicleState.Acquiring:
                    case VehicleState.Depositing:
                        return Brushes.LightGreen;
                    case VehicleState.Paused:
                        return Brushes.Pink;
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
