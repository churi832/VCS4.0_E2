using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.IF.OCS
{
    [Serializable()]
    public enum VehicleOperationCommand : int
    {
        None = 0,
        PowerOn = 1,
        PowerOff = 2,
        ErrorReset = 3,
        BuzzerOff = 4,
        VehiclePause = 5,
        VehicleResume = 6,
        ServoOn = 7,
        ServoOff = 8,
        ServoReset = 9,
        ManualMoveStart = 10,
        JogMoveStartToPos = 11,
        JogMoveStartToNeg = 12,
        JogMoveStop = 13,
    }
    [Serializable()]
    public enum VehicleOperationResult : int
    {
        Success = 0,
        Fail = 1,
    }
    [Serializable()]
    public enum HostState : int
    {
        Offline = 0,
        Remote = 1,
        Local = 2,
    }
    [Serializable()]
    public enum AutoTeachingRequestCode : int
    {
        Stop = 0,
        Start = 1,
    }
    [Serializable()]
    public enum ManualOperationResult : int
    {
        Accepted = 0,
        Denied = 1,
    }
    [Serializable()]
    public enum CarrierState : int
    {
        None = 0,
        Installed = 1,
    }
    [Serializable()]
    public enum CommandStatusCode : int
    {
        NotReceived = 0x0,
        MovingToSource = 0x1,
        Acquiring = 0x2,
        MovingToDestination = 0x3,
        Depositing = 0x4,
        Completed = 0x5,
        SourceEmpty = 0x6,
        AcquireFailed = 0x7,
        DestinationDouble = 0x8,
        DepositFailed = 0x9,
    }
    [Serializable()]
    public enum IFMessage : int
    {
        CommandSend = 0x01,
        CommandReply = 0x02,
        EventSend = 0x03,
        EventReply = 0x04,
        AlarmEventSend = 0x05,
        AlarmEventReply = 0x06,
        MapDataSend = 0x07,
        MapDataReply = 0x08,
        TeachingDataSend = 0x09,
        TeachingDataReply = 0x0A,
        DataVersionRequest = 0x0B,
        DataVersionReply = 0x0C,
        MapDataRequest = 0x0D,
        MapDataRequestAcknowledge = 0x0E,
        StatusDataSend = 0x11,
        StatusDataReply = 0x12,
        FDCDataSend = 0x13,
        FDCDataReply = 0x14,
        OperationConfigDataSend = 0x15,
        OperationConfigDataReply = 0x16,
        AutoTeachingResultSend = 0x17,
        AutoTeachingResultReply = 0x18,
        UserLoginRequest = 0x19,
        UserLoginReply = 0x1A,
        PathRequest = 0x1B,
        PathSend = 0x1C,
        CommandStatusRequest = 0x1D,
        CommandStatusReply = 0x1E,
        LocationInformationSend = 0x21,
        LocationInformationSendReply = 0x22,
        IDReadResultSend = 0x23,
        IDReadResultReply = 0x24,
    }
    [Serializable()]
    public enum OCSCommand : int
    {
        None = 0x00,
        Transfer = 0x01,
        DestinationChange = 0x02,
        RouteChange = 0x03,
        Go = 0x04,
        Pause = 0x05,
        Resume = 0x06,
        Cancel = 0x07,
        Abort = 0x08,
        PowerOn = 0x09,
        PowerOff = 0x0A,
        ErrorReset = 0x0B,
        BuzzerOff = 0x0C,
        Teaching = 0x0D,
        CarrierIdScan = 0x0E,
        AutoTeachingStart = 0x11,
        AutoTeachingStop = 0x12,
        FDCReportStart = 0x13,
        FDCReportStop = 0x14,
        TeachingDataRequest = 0x15,
        PermitInstall = 0x16,
        RefuseInstall = 0x17,
        VehicleRemove = 0x18,
        DateTimeSet = 0x019,
        DataSendIntervalChange = 0xFF,
        // Vehicle Cycle Test Command
        CycleHoistAging = 0x100,
        CycleSteerAging = 0x101,
        CycleAntiDropAging = 0x102,
        CycleWheelMoveAging = 0x103,
    }
    [Serializable()]
    public enum VehicleEvent : int
    {
        None = 0x00,
        DepartToFromPosition = 0x01,
        ArrivedAtFromPosition = 0x02,
        AcquireStart = 0x03,
        CarrierInstalled = 0x04,
        AcquireCompleted = 0x05,
        DepartToToPosition = 0x06,
        ArrivedAtToPosition = 0x07,
        DepositStart = 0x08,
        CarrierRemoved = 0x09,
        DepositCompleted = 0x0A,
        ErrorReset = 0x0B,
        GoCompleted = 0x0C,
        CancelCompleted = 0x0D,
        AbortCompleted = 0x0E,
        DestinationChangeCompleted = 0x0F,
        PowerOnCompleted = 0x11,
        PowerOffCompleted = 0x12,
        PauseCompleted = 0x13,
        ResumeCompleted = 0x14,
        //AutoTeachingStarted = 0x15,
        //AutoTeachingStopped = 0x16,
        AutoTeachingCompleted = 0x17,
        Installed = 0x21,
        Removed = 0x22,
        InstallRequest = 0x23,
        PIOErrorHappened = 0x31,
        DataRequest = 0x32,
        AcquireFailed = 0x41,
        DepositFailed = 0x42,
        SourceEmpty = 0x51,
        DestinationDoubleStorage = 0x52,
        CarrierIDMismatch = 0x53,
        AcquireInterfaceFail = 0x61,
        DepositInterfaceFail = 0x62,
    }

    [Serializable()]
    public enum IFAcknowledge : int
    {
        ACK = 0x00,
        NAK = 0x01,
    }

    [Serializable()]
    public enum MapDataType : int
    {
        Unknown = 0x0,
        NodeData = 0x1,
        LinkData = 0x2,
        PortData = 0x3,
        PIODeviceData = 0x4,
        JCSData = 0x5,
        ErrorList = 0x06,
        ALL = 0xF,
    }
    [Serializable()]
    public enum DataManagementType : int
    {
        Add = 1,
        Modify = 2,
        Delete = 3,
    }


    [Serializable()]
    public enum Position : int
    {
        East = 0,
        West = 1,
        South = 2,
        North = 3,
    }
    [Serializable()]
    public enum Direction : int
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
    }

    [Serializable()]
    public enum RailDirection : int
    {
        None = 0,
        LeftToRight = 1,
        RightToLeft = 2,
        TopToBotton = 3,
        BottomToTop = 4,
    }
    [Serializable()]
    public enum CycleTestMode : int
    {
        None = 0,
        ScenarioMode = 1,
        RandomMode = 2,
    }
    [Serializable()]
    public enum CycleRunMode : int
    {
        ScenarioMode = 1,
        RandomMode = 2,
    }
    [Serializable()]
    public enum OperationMode : int
    {
        NormalMode = 0,
        CycleMode_Scenario = 1,
        CycleMode_Random = 2,
    }
    [Serializable()]
    public enum CPSStatus : int
    {
        RUN = 0,
        STOP = 1,
        FAULT = 2,
        WARNING = 3,
        FAIL_OVER_OPERATION = 4,
        COMM_FAIL = 5,
    }

    [Serializable()]
    public enum PauseState : int
    {
        Release = 0,
        Pause = 1,
    }

    [Serializable()]
    public enum MOTORDIRECTION : int
    {
        Minus = -1,
        Plus = 1,
    }
    [Serializable()]
    public enum JobStatus : int
    {
        None = 0,
        Init,
        Wait,
        SourcePortCheck,
        DestPortCheck,
        CreateJobComplete,
        AssignVehicle,
        Wait_Move_To_Source,
        Move_To_Source,
        Arrival_To_Source,
        Wait_Acquire,
        Acquiring,
        Acquired,
        Wait_Move_To_Destination,
        Move_To_Destination,
        Arrival_To_Destination,
        Wait_Deposit,
        Depositing,
        Deposited,
        JobComplete,
        ReleaseJob,
    }

    [Serializable()]
    public enum DataType : int
    {
        BOOL = 0,
        CHAR,
        INT,
        FLOAT,
        DOUBLE,
        STRING,
    }

    [Serializable()]
    public enum BufferFindMode : int
    {
        Random = 0,
        NearByCurrentPosition = 1,
        NearByNextDestination = 2,
    }
    [Serializable()]
    public enum PathRequestType : int
    {
        None = 0,
        Go = 1,
        MoveToSource = 2,
        MoveToDestination = 3,
    }
    [Serializable()]
    public enum LocationInformationChangeCode : int
    {
        Add = 1,
        Modify = 2,
        Remove = 3,
    }
    [Serializable()]
    public enum IDReadResult : int
    {
        Success = 0x1,
        ReadFail = 0x2,
        Empty = 0x3,
    }
    [Serializable()]
    public enum DRIVEACTION : int
    {
        Move = 0,
        CurrentToFrom,
        FromToTarget,
        PickUp,
        Place,
    }
    [Serializable()]
    public enum InterfaceFlag
    {
        PauseRequest,
        ResumeRequest,
        CancelRequest,
        AbortRequest,
        PowerOnRequest,
        PowerOffRequest,
        AlarmClearRequest,
        BuzzerOffRequest,
        AutoTeachingStart,
        AutoTeachingStop,
        TeachingDataRequest,
        NodeDataReceived,
        LinkDataReceived,
        PortDataReceived,
        ErrorListReceived,
        PIODataReceived,
        VersionRequest,
        MapDataRequest,
        InstallPermit,
        InstallRefuse,
        PathRequest,
        PathReceived,
        PathReceivedNG,
        CarrierIdReadingComp,
        WaitCancelRequest,
        WaitAbortRequest,
        RouteChange,
    }
    [Serializable()]
    public enum FlagValue : int
    {
        NG = -1,
        OFF = 0,
        ON = 1,
    }

}
