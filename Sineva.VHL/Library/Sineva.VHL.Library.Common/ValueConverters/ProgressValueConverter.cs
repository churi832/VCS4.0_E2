using System;
using System.Globalization;
using System.Windows.Data;

namespace Sineva.VHL.Library.Common
{
    public class ProgressValueConverter : IValueConverter
    {
        #region Methods
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int v = (int)value;
            switch (v)
            {
                case -1:
                case 1:
                case 2:
                    return 100;
                case 0:
                default:
                    return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
