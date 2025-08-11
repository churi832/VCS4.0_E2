using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.OHT.Common
{
    public class VehicleErrorStateForeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (VehicleErrorStatus)value;
                switch (input)
                {
                    case VehicleErrorStatus.NoError:
                        return Brushes.LightGray;
                    case VehicleErrorStatus.Error:
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
