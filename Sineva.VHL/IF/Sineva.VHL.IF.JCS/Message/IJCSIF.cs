using System.Collections.Generic;

namespace Sineva.VHL.IF.JCS
{
    public interface IJCSIF
    {
        bool SetDataFromStream(byte[] stream);
        byte[] GetStreamFromDataOrNull();
        List<string> GetLogData();
    }
}
