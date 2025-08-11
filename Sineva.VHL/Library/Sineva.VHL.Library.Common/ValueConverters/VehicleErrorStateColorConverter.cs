using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.VHL.Library.Common
{
    public class VehicleErrorStateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (VehicleErrorStatus)value;
                switch (input)
                {
                    case VehicleErrorStatus.NoError:
                        return Brushes.Transparent;
                    case VehicleErrorStatus.Error:
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
