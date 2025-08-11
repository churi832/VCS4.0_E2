using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.IF.JCS
{
    [Serializable()]
    public enum JCSMode : int
    {
        Manual = 0x1,
        Auto = 0x2,
    }
    [Serializable()]
    public enum MessageCode : int
    {
        None = 0x0,
        IR = 0x1,
        IRR = 0x2,
        PR = 0x3,
        PRR = 0x4,
        PS = 0x5,
        PSR = 0x6,
        PC = 0x7,
        PCR = 0x8,
        SC = 0x9,
        SCR = 0xA,
        CR = 0xB,
        CRR = 0xC,
        SR = 0xD,
        SRR = 0xE,
    }
    [Serializable()]
    public enum PositionInJunction : int
    {
        NotEntered = 0x1,
        Passing = 0x2,
        Passed = 0x3,
        Working = 0x4,
    }
    [Serializable()]
    public enum PassState : int
    {
        None = 0x0,
        NotInJunctionPoint = 0x1,
        Passing = 0x2,
        Waiting = 0x3,
        Wait = 0x04,
        WorkingInPassArea = 0x05,
        WorkingInWaitArea = 0x6,
        WaitReply = 0x07,
    }
    [Serializable()]
    public enum PassRequestResult : int
    {
        No_Result = 0x0,
        PassPermit = 0x1,
        Wait = 0x2,
    }
    [Serializable()]
    public enum PassSendCode : int
    {
        PassPermit = 0x1,
        PassWait = 0x2,
    }
    [Serializable()]
    public enum PassSendReply : int
    {
        PassPossible = 0x0,
        PassWait = 0x1,
    }
    [Serializable()]
    public enum PassRequestReply : int
    {
        OK = 0x0,
        NG = 0x1,
    }
    [Serializable()]
    public enum JunctionPassStatus : int
    {
        PBSDetectStop = 0x1,
        VehicleAlarmStop = 0x2,
    }
    [Serializable()]
    public enum VehicleCurrentPosition : int
    {
        JCA = 0x1,
        JPA = 0x2,
    }
    [Serializable()]
    public enum VehiclePassState : int
    {
        WaitReply = 0x0,
        Passing = 0x1,
        Waiting = 0x2,
    }

    public enum LinkDirection
    {
        North = 0,
        NorthEast = 1,
        East = 2,
        SouthEast = 3,
        South = 4,
        SouthWest = 5,
        West = 6,
        NorthWest = 7,
    }
    [Serializable()]
    public enum JunctionType : int
    {
        Straight = 0,
        Curve = 1,
    }

    [Serializable()]
    public enum JCSIFMessage : int
    {
        InitialStateRequest = 0x01,
        InitialStateReply = 0x02,
        JunctionPassRequest = 0x03,
        JunctionPassRequestReply = 0x04,
        JunctionPassSend = 0x05,
        JunctionPassSendReply = 0x06,
        JunctionPassCompleteSend = 0x07,
        JunctionPassCompleteSendReply = 0x08,
        JunctionPassStatusChangeSend = 0x09,
        JunctionPassStatusChangeSendReply = 0x0A,
        JunctionDistanceChangeSend = 0x0B,
        JunctionDistanceChangeSendReply = 0x0C,
    }
    [Serializable()]
    public enum JunctionPointNumber : int
    {
        NotInJPArea = 0x00,
        InSpecifiedJPArea = 0x01,
    }
    [Serializable()]
    public enum JunctionState : int
    {
        None = 0x00,
        NotInJPArea = 0x01,
        Passing = 0x02,
        Waiting = 0x03,
        Wait = 0x04,
        PassAreaWorking = 0x05,
        WaitAreaWorking = 0x06,
        NeedInitReport = 0x07,
    }
    [Serializable()]
    public enum JunctionPassRequestResult : int
    {
        No_Result = 0x00,
        PassPermit = 0x01,
        Wait = 0x02,
    }
    [Serializable()]
    public enum JunctionPassSendCode : int
    {
        PassPermit = 0x1,
    }
    [Serializable()]
    public enum JunctionPassSendReply : int
    {
        PassPossible = 0x00,
        Wait = 0x01,
    }

    [Serializable()]
    public enum JunctionStopState : int
    {
        NotStopState = 0x00,
        PBSDetectStop = 0x01,
        AlarmStop = 0x02,
    }
    [Serializable()]
    public enum JunctionPosition : int
    {
        None = 0x00,
        JunctionCheckArea = 0x01,
        JunctionPassArea = 0x02,
    }
}
