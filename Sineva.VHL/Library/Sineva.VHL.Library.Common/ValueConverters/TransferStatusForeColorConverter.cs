using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.VHL.Library.Common
{
    public class TransferStatusForeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (ProcessStatus)value;
                switch (input)
                {
                    case ProcessStatus.Queued:
                        return Brushes.LightGray;
                    case ProcessStatus.Waiting:
                        return Brushes.Black;
                    case ProcessStatus.Assigned:
                    case ProcessStatus.Processing:
                        return Brushes.Black;
                    case ProcessStatus.Paused:
                        return Brushes.White;
                    case ProcessStatus.Canceling:
                    case ProcessStatus.Aborting:
                    case ProcessStatus.Canceled:
                    case ProcessStatus.Aborted:
                        return Brushes.Black;
                    case ProcessStatus.Completed:
                        return Brushes.Black;
                    case ProcessStatus.Deleted:
                        return Brushes.White;
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
