using System.Collections.Generic;

namespace Sineva.VHL.IF.OCS
{
    public interface IVehicleIF
    {
        bool SetDataFromStream(byte[] stream);
        byte[] GetStreamFromDataOrNull();
        List<string> GetLogData();
    }
}
