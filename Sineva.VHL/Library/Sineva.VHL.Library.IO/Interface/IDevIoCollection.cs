using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.IO
{
    public interface IDevIoCollection : ICollection
    {
        IoType DevIoType { get; }
        _DeviceIo CreateNewItem();
    }
}
