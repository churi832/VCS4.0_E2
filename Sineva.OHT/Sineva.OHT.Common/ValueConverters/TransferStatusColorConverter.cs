using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sineva.OHT.Common
{
    public class TransferStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = (TransferStatus)value;
                switch (input)
                {
                    case TransferStatus.Queued:
                        return Brushes.Transparent;
                    case TransferStatus.Waiting:
                        return Brushes.LightYellow;
                    case TransferStatus.Assigned:
                    case TransferStatus.Transferring:
                        return Brushes.LightGreen;
                    case TransferStatus.Paused:
                        return Brushes.DarkGray;
                    case TransferStatus.Canceling:
                    case TransferStatus.Aborting:
                    case TransferStatus.Canceled:
                    case TransferStatus.Aborted:
                        return Brushes.Pink;
                    case TransferStatus.Completed:
                        return Brushes.LightBlue;
                    case TransferStatus.Deleted:
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
