using System;
using System.Data;

namespace Sineva.OHT.Common
{
    public delegate void NonQueryCallback(int nResult);
    public delegate void ReaderCallback(int nResult, DataTable dt);
    public delegate void ScalarCallback(object objResult);
    public delegate void ProcedureCallBack(DataTable dt);

    [Serializable()]
    public enum JobMode
    {
        NONQUERY,
        READER,
        SCALAR,
        PROCEDURE,

        BLOB_NONQUERY,
        BLOB_READER
    }
    [Serializable()]
    public enum DBDataType
    {
        STRING = 0,
        INT = 1,
        FLOAT = 2,
        DATETIME = 3,
    }
    [Serializable()]
    public enum MelsecTableType
    {
        JCU,
        MTL,
        VHL,
    }
    [Serializable()]
    public enum GeneralInformationItemName
    {
        NodeDataVersion,
        LinkDataVersion,
        PortDataVersion,
        PIODeviceDataVersion,
    }
    [Serializable()]
    public enum ErrorListType
    {
        OCS,
        VHL,
        CPS,
    }
}
