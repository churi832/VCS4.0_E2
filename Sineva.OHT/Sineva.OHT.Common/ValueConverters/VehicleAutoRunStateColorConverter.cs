using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.OHT.Common
{
    public class VehicleAutoRunStateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (VehicleAutoRunStatus)value;
                switch (input)
                {
                    case VehicleAutoRunStatus.Manual:
                        return Brushes.Transparent;
                    case VehicleAutoRunStatus.Auto:
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
