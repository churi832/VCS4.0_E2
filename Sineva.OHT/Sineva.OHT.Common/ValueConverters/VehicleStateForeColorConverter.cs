using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.OHT.Common
{
    public class VehicleStateForeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (VehicleState)value;
                switch (input)
                {
                    case VehicleState.Removed:
                        return Brushes.LightGray;
                    case VehicleState.NotAssigned:
                    case VehicleState.Parked:
                        return Brushes.Black;
                    case VehicleState.EnRoute:
                    case VehicleState.Acquiring:
                    case VehicleState.Depositing:
                        return Brushes.Black;
                    case VehicleState.Paused:
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
