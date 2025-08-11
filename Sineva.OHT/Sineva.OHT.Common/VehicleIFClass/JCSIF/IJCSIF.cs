using System.Collections.Generic;

namespace Sineva.OHT.Common
{
    public interface IJCSIF
    {
        bool SetDataFromStream(byte[] stream);
        byte[] GetStreamFromDataOrNull();
        List<string> GetLogData();
    }
}
