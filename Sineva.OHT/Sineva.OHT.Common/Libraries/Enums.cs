using System;

namespace Sineva.OHT.Common
{
    [Serializable()]
    public enum PathAlgorithm : int
    {
        Dijkstra = 0,
        AStar = 1,
    }
    [Serializable()]
    public enum ConfigType : int
    {
        SystemConfig = 0,
        OperationConfig = 1,
    }
    [Serializable()]
    public enum DBQueryType : int
    {
        Select = 0,
        Insert = 1,
        Update = 2,
        Delete = 4,
    }
    [Serializable()]
    public enum DBTableType : int
    {
        LinkTable = 0,
        NodeTable = 1,
        TeachingTable = 2,
        OCSErrorTable = 3,
        VHLErrorTable = 4,
        CPSErrorTable = 5,
        PortTable = 6,
        VehicleTable = 7,
        PartsTable = 8,
        UserTable = 9,
        ZoneTable = 10,
        CarrierTable = 11,
    }
    [Serializable()]
    public enum PathFindMethod : int
    {
        ByDistance = 1,
        ByTime = 2,
    }
    [Serializable()]
    public enum LogFileSplitType : int
    {
        Day = 0,
        DayAndHour = 1,
        DayAndLine = 2,
    }
    [Serializable()]
    public enum Language : int
    {
        Chinese = 0,
        English = 1,
        Korean = 2,
    }
    [Serializable()]
    public enum UserState : int
    {
        Logout = 0,
        Login = 1,
    }
    [Serializable()]
    public enum AlarmType : int
    {
        Warning = 0x01,
        Alarm = 0x02,
    }
    [Serializable()]
    public enum AlarmSetCode : int
    {
        Reset = 0x0,
        Set = 0x1,
    }
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
    public enum TSCState : int
    {
        Paused = 0,
        Pausing = 1,
        Auto = 2,
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
    public enum VehicleInRailStatus : int
    {
        OutOfRail = 0,
        InRail = 1,
    }
    [Serializable()]
    public enum VehicleAutoTeachingStatus : int
    {
        NotAutoTeaching = 0,
        ExecutingAutoTeaching = 1,
    }
    [Serializable()]
    public enum VehicleBusyStatus : int
    {
        Idle = 0,
        Busy = 1,
    }
    [Serializable()]
    public enum VehiclePowerStatus : int
    {
        PowerOff = 0,
        PowerOn = 1,
    }
    [Serializable()]
    public enum VehicleAutoRunStatus : int
    {
        Init = -1, //2022.10.10 초기화를 위해..by HS
        Manual = 0,
        Auto = 1,
    }
    [Serializable()]
    public enum VehiclePauseStatus : int
    {
        Released = 0,
        Paused = 1,
    }
    [Serializable()]
    public enum VehicleErrorStatus : int
    {
        NoError = 0,
        Error = 1,
    }
    [Serializable()]
    public enum VehicleMovingStatus : int
    {
        Stopped = 0,
        Moving = 1,
    }
    [Serializable()]
    public enum CarrierState : int
    {
        None = 0,
        Installed = 1,
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
    }
    [Serializable()]
    public enum OCSCommand : int
    {
        TransferCommandSend = 0x01,
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
        AutoTeachingStart = 0x11,
        AutoTeachingStop = 0x12,
        FDCReportStart = 0x13,
        FDCReportStop = 0x14,
        TeachingDataRequest = 0x15,
        PermitInstall = 0x16,
        RefuseInstall = 0x17,
        DataSendIntervalChange = 0xFF,
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
        AutoTeachingStarted = 0x15,
        AutoTeachingStopped = 0x16,
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
    }
    [Serializable()]
    public enum IFAcknowledge : int
    {
        ACK = 0x00,
        NAK = 0x01,
    }
    [Serializable()]
    public enum UserLoginResult : int
    {
        Success = 0,
        UnRegisteredUser = 1,
        InvalidPassword = 2,
        NotDefinedError = 3,
    }
    [Serializable()]
    public enum VehicleUserLoginLevel : int
    {
        Operator = 0,
        Maintenance = 1,
        Administrator = 2,
    }
    [Serializable()]
    public enum AuthorizationLevel : int
    {
        Developer = 0,
        Supervisor = 1,
        Administrator = 2,
        Maintenance = 3,
        Operator = 4,
    }
    [Serializable()]
    public enum MapDataType : int
    {
        NodeData = 0x1,
        LinkData = 0x2,
        PortData = 0x3,
        PIODeviceData = 0x4,
        Both = 0x5,
    }
    [Serializable()]
    public enum DataManagementType : int
    {
        Add = 1,
        Modify = 2,
        Delete = 3,
    }
    [Serializable()]
    public enum TransferType : int
    {
        None = 0,
        Transfer = 1,
        Acquire = 2,
        Deposit = 3,
        Cancel = 4,
        Abort = 5,
        Go = 6,
    }
    [Serializable()]
    public enum TransferStatus : int
    {
        None = 0,
        Queued = 1,
        Assigned = 2,
        Transferring = 3,
        Paused = 4,
        Canceling = 5,
        Aborting = 6,
        Waiting = 7,
        Completed = 11,
        Canceled = 12,
        Aborted = 13,
        Deleted = 14,
    }
    [Serializable()]
    public enum VehicleState : int
    {
        Removed = 1,
        NotAssigned = 2,
        EnRoute = 3,
        Parked = 4,
        Acquiring = 5,
        Depositing = 6,
        Paused = 7,
    }
    [Serializable()]
    public enum PortType : int
    {
        NotDefined = 0,
        LeftEQPort = 1,
        RightEQPort = 2,
        LeftBuffer = 3,
        RightBuffer = 4,
        UnderLeftEQPort = 5,
        UnderRightEQPort = 6,
        LeftIOPort = 7,
        RightIOPort = 8,
    }
    [Serializable()]
    public enum PortStatus : int
    {
        OutOfService = 0,
        LDRQ = 1,
        LDCM = 2,
        UDRQ = 3,
        UDCM = 4,
    }
    [Serializable()]
    public enum NodeType : int
    {
        Normal = 0,
        Port = 1,
        Park = 2,
        Branch = 3,
        Junction = 4,
        LifterIn = 90,
        Lifter = 91,
        MTLIn = 98,
        MTL = 99,
    }
    [Serializable()]
    public enum LinkType : int
    {
        Straight = 0,
        LeftCurve = 1,
        RightCurve = 2,
        LeftBranch = 3,
        RightBranch = 4,
        LeftSBranch = 5,
        RightSBranch = 6,
        LeftJunction = 7,
        RightJunction = 8,
        LeftSJunction = 9,
        RightSJunction = 10,
        LeftBranchStraight = 11,
        RightBranchStraight = 12,
        LeftJunctionStraight = 13,
        RightJunctionStraight = 14,
        Ascent = 15,
        Descent = 16,
        LeftCompositedSCurveBranch = 17,
        RightCompositedSCurveBranch = 18,
        LeftCompositedSCurveJunction = 19,
        RightCompositedSCurveJunction = 20,
        //2023.1.9 임시 Link ..by HS
        JunctionStraight = 21,
        //2023.2.9 벽 구분 Link ..by HS
        SideStraight = 22,
        SideLeftJunctionStraight = 23,
        SideRightJunctionStraight = 24,

        //LeftSlide = 15,
        //RightSlide = 16,
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
    public enum PortListViewType : int
    {
        Port = 0,
        Buffer = 1,
    }
    [Serializable()]
    public enum Result : int
    {
        Fail = -1,
        Success = 1,
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
    public enum ConnectionType : int
    {
        Server = 0,
        Client = 1,
    }
    [Serializable()]
    public enum PauseState : int
    {
        Release = 0,
        Pause = 1,
    }

    [Serializable()]
    public enum GUICommand : int
    {
        None = 1000,
        TransferCommandSend = 1000 + 0x01,
        DestinationChange = 1000 + 0x02,
        RouteChange = 1000 + 0x03,
        Go = 1000 + 0x04,
        Pause = 1000 + 0x05,
        Resume = 1000 + 0x06,
        Cancel = 1000 + 0x07,
        Abort = 1000 + 0x08,
        PowerOn = 1000 + 0x09,
        PowerOff = 1000 + 0x0A,
        ErrorReset = 1000 + 0x0B,
        BuzzerOff = 1000 + 0x0C,
        AutoTeachingStart = 1000 + 0x11,
        AutoTeachingStop = 1000 + 0x12,
        FDCReportStart = 1000 + 0x13,
        FDCReportStop = 1000 + 0x14,
        TeachingDataRequest = 1000 + 0x15,
        PermitInstall = 1000 + 0x16,
        RefuseInstall = 1000 + 0x17,
        DataSendIntervalChange = 1000 + 0xFF,
        TestFunction,

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

    public enum PIODeviceType : int
    {
        EQPIO = 0,
        MTL = 1,
        SPL = 2,
        JCU = 3,
    }

    public enum PathRequestType : int
    {
        Go = 1,
        MoveToSource = 2,
        MoveToDestination = 3,
    }

    public enum DRIVEACTION : int
    {
        Move = 0,
        CurrentToFrom,
        FromToTarget,
        PickUp,
        Place,
    }
    //2023.1.16 JCS
    [Serializable()]
    public enum JCSType : int
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

    //2023.3.3 Lamp Color ..by HS
    public enum LampColor : int
    {
        None = 0,
        RED = 1,
        YELLOW = 2,
        GREEN = 3,
        FLICKER_NORMAL = 4,
        FLICKER_ALARM = 5,
        FLICKER_FRONT_DETECT = 6,
    }
    //2023.3.8 VHL State..by HS
    [Serializable()]
    public enum Machine_State : int
    {
        Init = 0,
        Pause = 1,
        Auto = 2,
        Pausing = 3,
    }
}
