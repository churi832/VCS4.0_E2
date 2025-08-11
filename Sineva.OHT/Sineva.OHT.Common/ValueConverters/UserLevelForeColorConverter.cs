using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.OHT.Common
{
    public class UserLevelForeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (AuthorizationLevel)value;
                switch (input)
                {
                    case AuthorizationLevel.Developer:
                    case AuthorizationLevel.Supervisor:
                    case AuthorizationLevel.Administrator:
                    case AuthorizationLevel.Maintenance:
                    case AuthorizationLevel.Operator:
                        return Brushes.Black;
                    default:
                        return Brushes.LightGreen;
                }
            }
            catch
            {
                return Brushes.Red;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
