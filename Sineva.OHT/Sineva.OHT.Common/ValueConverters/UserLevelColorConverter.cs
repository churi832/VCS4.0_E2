using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.OHT.Common
{
    public class UserLevelColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (AuthorizationLevel)value;
                switch (input)
                {
                    case AuthorizationLevel.Developer:
                        return Brushes.Violet;
                    case AuthorizationLevel.Supervisor:
                        return Brushes.LightCyan;
                    case AuthorizationLevel.Administrator:
                        return Brushes.LightGreen;
                    case AuthorizationLevel.Maintenance:
                        return Brushes.LightBlue;
                    case AuthorizationLevel.Operator:
                        return Brushes.LightPink;
                    default:
                        return Brushes.Transparent;
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
