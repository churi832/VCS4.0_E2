using System;
using System.Globalization;
using System.Windows.Data;

namespace Sineva.VHL.Library.Common
{
    public class StringDateTimeSubConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string dateTime = string.Empty;
            string strVal = value as string;
            if (!string.IsNullOrEmpty(strVal))
            {
                DateTime temp;
                bool res = DateTime.TryParse(strVal, out temp);
                if (res)
                {
                    dateTime = temp.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            return dateTime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
