using System;

namespace Sineva.OHT.Common
{
    [Serializable()]
    public enum MelsecProtocolType : int
    {
        MXComponent = 1,
        TCP = 6,
        UDP = 17,
    }
    [Serializable()]
    public enum MelsecCommandType
    {
        Read,
        Write,
    }
    [Serializable()]
    public enum MelsecAccessType
    {
        BIT = 0x0001,
        WORD = 0x0000,
    }
    [Serializable()]
    public enum MelsecDeviceType : byte
    {
        None = 0,
        X = 0x9C,
        Y = 0x9D,
        L = 0x92,
        M = 0x90,
        F = 0x93,
        D = 0xA8,
        R = 0xAF,
        B = 0xA0,
        W = 0xB4,
        ZR = 0xB0,
    };
    [Serializable()]
    public enum IOType : int
    {
        BitInput = 0,
        BitOutput = 1,
        WordInput = 2,
        WordOutput = 3,
    }
    [Serializable()]
    public enum MelsecUnitType : int
    {
        MTL = 0,
        JCU = 1,
    }
}
