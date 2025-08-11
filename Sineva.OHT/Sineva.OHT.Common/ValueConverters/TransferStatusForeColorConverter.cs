using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.OHT.Common
{
    public class TransferStatusForeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (TransferStatus)value;
                switch (input)
                {
                    case TransferStatus.Queued:
                        return Brushes.LightGray;
                    case TransferStatus.Waiting:
                        return Brushes.Black;
                    case TransferStatus.Assigned:
                    case TransferStatus.Transferring:
                        return Brushes.Black;
                    case TransferStatus.Paused:
                        return Brushes.White;
                    case TransferStatus.Canceling:
                    case TransferStatus.Aborting:
                    case TransferStatus.Canceled:
                    case TransferStatus.Aborted:
                        return Brushes.Black;
                    case TransferStatus.Completed:
                        return Brushes.Black;
                    case TransferStatus.Deleted:
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
