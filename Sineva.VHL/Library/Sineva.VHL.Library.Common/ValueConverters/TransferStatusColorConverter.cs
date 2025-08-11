using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.VHL.Library.Common
{
    public class TransferStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (ProcessStatus)value;
                switch (input)
                {
                    case ProcessStatus.Queued:
                        return Brushes.Transparent;
                    case ProcessStatus.Waiting:
                        return Brushes.LightYellow;
                    case ProcessStatus.Assigned:
                    case ProcessStatus.Processing:
                        return Brushes.LightGreen;
                    case ProcessStatus.Paused:
                        return Brushes.DarkGray;
                    case ProcessStatus.Canceling:
                    case ProcessStatus.Aborting:
                    case ProcessStatus.Canceled:
                    case ProcessStatus.Aborted:
                        return Brushes.Pink;
                    case ProcessStatus.Completed:
                        return Brushes.LightBlue;
                    case ProcessStatus.Deleted:
                        return Brushes.Red;
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
