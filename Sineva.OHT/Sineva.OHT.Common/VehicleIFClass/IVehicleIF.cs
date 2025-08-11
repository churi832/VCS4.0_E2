using System.Collections.Generic;

namespace Sineva.OHT.Common
{
    public interface IVehicleIF
    {
        bool SetDataFromStream(byte[] stream);
        byte[] GetStreamFromDataOrNull();
        List<string> GetLogData();
    }
}
