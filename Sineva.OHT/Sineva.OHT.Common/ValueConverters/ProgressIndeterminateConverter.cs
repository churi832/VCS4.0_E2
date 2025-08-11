using System;
using System.Globalization;
using System.Windows.Data;

namespace Sineva.OHT.Common
{
    public class ProgressIndeterminateConverter : IValueConverter
    {
        #region Methods
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int v = (int)value;
            switch (v)
            {
                case 1:
                    return true;
                default:
                    return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
