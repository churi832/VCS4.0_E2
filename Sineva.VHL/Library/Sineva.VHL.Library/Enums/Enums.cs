/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.16
 * Description	: 
 * 
 ****************************************************************/
using System;

namespace Sineva.VHL.Library
{
    [Serializable()]
    public enum VehicleType : int
    {
        Normal = 0,
        Clean = 1,
        Reticle = 2,
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
    //[Serializable()]
    //public enum MapDataType : int
    //{
    //    NodeData = 0x1,
    //    LinkData = 0x2,
    //    PortData = 0x3,
    //    PIODeviceData = 0x4,
    //    JCSData = 0x5,
    //    ALL = 0xF,
    //}
    [Serializable()]
    public enum DataManagementType : int
    {
        Add = 1,
        Modify = 2,
        Delete = 3,
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
    public enum PathFindMethod : int
    {
        ByDistance = 1,
        ByTime = 2,
    }
    [Serializable()]
    public enum VehicleState : int
    {
        Removed = 1,
        NotAssigned = 2,
        EnRouteToSource = 3,
        EnRouteToDest = 4,
        Parked = 5,
        Acquiring = 6,
        Depositing = 7,
        Paused = 8,
        SourceEmpty = 9,
        AcquireFailed = 10,
        DestinationDouble = 11,
        DepositFailed = 12,
        AutoTeaching = 13, // Auto Teaching Vision Find 동작
        Go = 14, // Go, AutoTeaching 하러 이동할때는 모두 GO 사용
        AcquireCompleted = 15,
        DepositeCompleted = 16,
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
        Teaching = 7,
    }
    [Serializable()]
    public enum ProcessStatus : int
    {
        None = 0,
        Queued = 1,
        Assigned = 2,
        Processing = 3,
        Paused = 4,
        Canceling = 5,
        Aborting = 6,
        Waiting = 7,
        Completed = 11,
        Canceled = 12,
        Aborted = 13,
        Deleted = 14,
        RouteChanging = 15,
    }
    [Serializable()]
    public enum transferUpdateTime : int
    {
        None = 0,
        Install,
        CommandAssigned,
        MoveToSource,
        ArrivedSource,
        AquireStart,
        AquireEnd,
        MoveToDestination,
        ArrivedDestination,
        DepositStart,
        DepositEnd,
        CommandCompleted,
    }
    [Serializable()]
    public enum PortType : int
    {
        NotDefined = 0,
        LeftEQPort = 1,
        RightEQPort = 2,
        LeftBuffer = 3,
        RightBuffer = 4,
        LeftSTKPort = 5,
        RightSTKPort = 6,
        LeftTeachingStation = 7, // Buffer Teaching
        RightTeachingStation = 8, // Buffer Teaching
        TeachingStation = 9, // EQ Teaching Port
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
        LifterOut = 92,
        MTLOut = 97,
        MTLIn = 98,
        MTL = 99,
        AutoDoorIn1 = 100,
        AutoDoorOut1 = 101,
        AutoDoorIn2 = 102,
        AutoDoorOut2 = 103,
        FrontCheck = 104,
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
        
        AutoDoorCurve = 25,       // AutoDoor가 Curve에 있는 경우
        AutoDoorStraight = 26,    // AutoDoor가 Straight에 있는 경우
        MTL = 27,
        SPL = 28,
    }
    public enum PIODeviceType : int
    {
        EQPIO = 0,
        MTL = 1,
        SPL = 2,
        JCU = 3,
        AutoDoor1 = 4,
        AutoDoor2 = 5,
        AutoDoor3 = 6,
        AutoDoor4 = 7,
        AutoDoor5 = 8,
    }
    [Serializable()]
    public enum Result : int
    {
        Fail = -1,
        No_Result = 0,
        Success = 1,
    }
    [Serializable()]
    public enum VehicleInRailStatus : int
    {
        OutOfRail = 0,
        InRail = 1,
    }
    [Serializable()]
    public enum TSCState : int
    {
        Paused = 0,
        Pausing = 1,
        Auto = 2,
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
    public enum enSteerDirection : int
    {
        DontCare = 0,
        Left = 1,
        Right = 2,
    }
    [Serializable()]
    public enum enBcrCheckDirection : int
    {
        Both = 0,
        Left = 1,
        Right = 2,
    }
    [Serializable()]
    public enum PathAlgorithm : int
    {
        Dijkstra = 0,
        AStar = 1,
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
    [Serializable()]
    public enum SensorAction
    {
        None,
        On,
        Off,
        Busy,
        Alarm,
    }
    [Serializable]
    public enum OBSType
    {
        UST,
        SICK,
        SOS,
    }
    [Serializable]
    public enum enProcState
    {
        none,
        skip,
        ready, // index
        wait, // loader
        inProc,
        finished,
        abort,
    }
    [Flags]
    [Serializable]
    public enum enTaskLayer : int
    {
        layerNone = 0,
        layerInit = 1 << 1,
        layerVehicleMove = 1 << 2,
        layerAcquire = 1 << 3,
        layerDeposit = 1 << 4,
        layerInterface = 1 << 5,
        layerSemiAuto = 1 << 6,
    }
    [Serializable]
    public enum enGoCommandType : int
    {
        None = 0,
        ByLocation = 1,
        ByDistance = 2,
    }
    [Serializable]
    public enum enMotionProc
    {
        none,
        skip,
        ready, // index
        wait, // loader
        inProc,
        finished,
        abort,
    }
    [Serializable]
    public enum enFrontDetectState
    {
        enNone = 0,
        enDeccelation1 = 1,
        enDeccelation2,
        enDeccelation3,
        enDeccelation4,
        enDeccelation5,
        enDeccelation6,
        enDeccelation7,
        enDeccelation8,
        enDeccelation9,
        enStop,
    }
    [Serializable]
    public enum enDeviceType
    {
        AntiDrop,
        Gripper,
        OutRigger,
        Steer,
    }
    [Serializable]
    public enum enProfileExistPosition
    {
        None = 0,
        FrontSide,
        RearSide,
    }
    [Flags]
    public enum enSimulationFlag
    {
        None = 0x0,
        UpRangeOver_Acquire = 1 << 1,
        FoupSingleDetect_Acquire = 1 << 2,
        ES_Signal_Off_Before_Acquire = 1 << 3,
        ES_Signal_Off_Acquiring = 1 << 4,
        HO_Signal_Off_Before_Acquire = 1 << 5,
        HO_Signal_Off_Acquiring = 1 << 6,
        Hoist_Swing_Interlock_Acquire = 1 << 7,
        UpRangeOver_Deposit= 1 << 8,
        FoupSingleDetect_Deposit = 1 << 9,
        ES_Signal_Off_Before_Deposit = 1 << 10,
        ES_Signal_Off_Depositing = 1 << 11,
        HO_Signal_Off_Before_Deposit = 1 << 12,
        HO_Signal_Off_Depositing = 1 << 13,
        Hoist_Swing_Interlock_Deposit = 1 << 14,
        PIO_TA1_Acquire = 1 << 15,
        PIO_TA2_Acquire = 1 << 16,
        PIO_TA3_Acquire = 1 << 17,
        PIO_TA1_Deposit = 1 << 18,
        PIO_TA2_Deposit = 1 << 19,
        PIO_TP3_Deposit = 1 << 20,
        PIO_TA3_Deposit = 1 << 21,
        HoistLimitDetect_Acquire = 1 << 22,
        HoistLimitDetect_Deposit = 1 << 23,
        InRangeOver_Acquire = 1 << 24,
        InRangeOver_Deposit = 1 << 25,
    }
}
