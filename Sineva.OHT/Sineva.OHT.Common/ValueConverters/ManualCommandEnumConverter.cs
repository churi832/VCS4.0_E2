using System;
using System.Globalization;
using System.Windows.Data;

namespace Sineva.OHT.Common
{
    public class ManualCommandEnumConverter : IValueConverter
    {
        #region Methods
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int v = (int)value;
            int p = int.Parse(parameter.ToString());
            if (v == p)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
            {
                return (VehicleOperationCommand)int.Parse(parameter.ToString());
            }
            else
            {
                return Binding.DoNothing;
            }
        }
        #endregion
    }
}
