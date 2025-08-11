using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sineva.VHL.GUI.Touch
{
    public enum StatusAddress
    {
        Com = 0,
        Year = 3,
        Month = 4,
        Day = 5,
        Hour = 6,
        Minute = 7,
        Second = 8,
        Mode = 10,
        Run = 11,
        //Error = 12,
        Velocity = 13,
        ObsUpArea = 20,
        ObsUpOut1 = 21,
        ObsUpOut2 = 22,
        ObsUpOut3 = 23,
        ObsDownArea = 25,
        ObsDownOut1 = 26,
        ObsDownOut2 = 27,
        ObsDownOut3 = 28,
        Destination = 30,
        CarrierID = 40,
        HeartBitCount = 50
    }
    public enum AddressType
    {
        Coil,
        InputBit,
        InputRegister,
        HoldRegister
    }
    public enum DataType
    {
        Bool,
        Short,
        UShort,
        Int,
        UInt,
        Long,
        ULong,
        Float,
        Double,
        String
    }
    public enum DecodeWay
    {
        Default,
        SmallBefore,
        ReverseWord,
        Both
    }
    public class InsMsg
    {
        public ManualResetEvent sendEvent = new ManualResetEvent(false);
        public bool res;
        public object val;
        public string errorMsg;
        public DateTime sendTime = DateTime.Now;
    }
}
