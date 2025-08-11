using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.VHL.Library.Common
{
    public class IntegerBitValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                //int v = (int)value;
                //if (v == 0)
                //{
                //    return "OFF";
                //}
                //else if (v == 1)
                //{
                //    return "ON";
                //}
                //else
                //{
                //    return "OFF";
                //}

                var input = System.Convert.ToInt32(value);
                switch (input)
                {
                    case 0:
                        return "OFF";
                    case 1:
                        return "ON";
                    default:
                        return "OFF";
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
