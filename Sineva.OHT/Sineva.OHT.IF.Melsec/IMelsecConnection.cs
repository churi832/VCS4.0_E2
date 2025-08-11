using Sineva.OHT.Common;

namespace Sineva.OHT.IF.Melsec
{
    public interface IMelsecConnection
    {
        bool Open();
        bool Close();
        bool GetOpenStatus();
        int BlockReadDevice(MelsecDeviceType type, int index, ref short byteSize, ref short[] readData);
        int BlockWriteDevice(MelsecDeviceType type, int index, ref short byteSize, ref short[] writeData);
        int RandomWriteDevice(string[] devices, ref short[] writeData);
        int SetDevice(string address, short writeData);
        //short RandomWriteBitDevice(MelsecDeviceType[] type, int[] indexes, short[] size, short[] writeData);
        //short RandomWriteWordDevice(MelsecDeviceType[] type, int[] indexes, short[] size, short[] writeData);
        //short RandomReadDevice(MelsecDeviceType[] type, int[] indexes, ref short[] readData);
    }
}
