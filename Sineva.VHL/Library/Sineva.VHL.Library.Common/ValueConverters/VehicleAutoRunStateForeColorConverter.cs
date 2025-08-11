using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.VHL.Library.Common
{
    public class AutoRunStateForeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (OperateMode)value;
                switch (input)
                {
                    case OperateMode.Manual:
                        return Brushes.LightGray;
                    case OperateMode.Auto:
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
