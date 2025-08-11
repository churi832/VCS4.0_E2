/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.16 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library
{
    public enum IoInOutType
    {
        In,
        Out
    }

    public enum IoDataType
    {
        Digital,
        Analog
    }

    //public enum IoType
    //{
    //    DI,
    //    DO,
    //    AI,
    //    AO
    //}

    public enum ActiveType
    {
        A,
        B
    }

    public enum ServerMode
    {
        Local,
        Remoting
    }

    //public enum OpMode
    //{
    //    Local,
    //    Remote
    //}

    [Serializable()]
    public enum ConnectionType
    {
        IPC,
        TCP,
        UDP,
    }

    [Serializable()]
    public enum ConnectionMode
    {
        Server,
        Client
    }

    public enum InitState
    {
        Noop, Init, Fail, Comp, Skip
    }

    public enum IoUpdateMode
    {
        Polling,
        EventDriven
    }

    // Io Status Watch 할때 어떤 기반으로 돌릴것인지 정의
    public enum WatchMode
    {
        Thread,
        FormTimer,
        SystemTimer,
        ThreadingTimer
    }

    public enum DeviceNetState
    {
        Invalid = 0,
        Idle = 1,
        Reset = 2,
        Init = 3,
        Start = 4,
        Run = 5,
        Stop = 6,
        SaveConfig = 7,
        LoadConfig = 8,
        PowerFailure = 9,
        PowerGood = 10,
        Error = 11,
        Shutdown = 12,
        Suspend = 13,
        Resume = 14,
        Config = 15
    }

    public enum UnitType
    {
        None,
        lpm,
        MPa,
        kPa,
        Pa,
        rpm,
        Bar,
        mmps,
        Mohm,
        mW,
        V,
        A,
        L,
        Hz,
        Celsius,
        hour,
        min,
        sec,
        msec,
        mm,
        um,
        EA,
        Percent,
        kV,
        kVA,
        kVAh,
        kVAR,
        kVARh,
        mmpm,
        Times,
        Torr,
        SCCM,
        pH,
        ohm,
        kW,
        kWh,
        W,
        ccm,
        Sec,
        MB,
        uS,
        ppm,
        Nm,
        회,
    }

    public enum BuzzerType
    {
        None,
        Alarm,
        Warning,
        OperatorCall,
        SafetyAlert,
    }

    public enum NumericFormat
    {
        Currency,
        Decimal,
        Exponential,
        Fixed_point,
        General,
        Number,
        Percent,
        Hexa,
    }

    public struct TagUnit
    {
        private UnitType unit;
        public UnitType Unit
        {
            get { return unit; }
            set { unit = value; }
        }
        public string Name
        {
            get
            {
                string name = "";
                switch (unit)
                {
                    case UnitType.None:
                        name = "";
                        break;
                    case UnitType.mmps:
                        name = "mm/s";
                        break;
                    case UnitType.mmpm:
                        name = "mm/min";
                        break;
                    case UnitType.Percent:
                        name = "%";
                        break;
                    case UnitType.Celsius:
                        name = "'C"; //℃는 폰트에 따라 깨짐
                        break;
                    default:
                        name = unit.ToString();
                        break;
                }

                return name;
            }
        }

        public TagUnit(UnitType unit)
        {
            this.unit = unit;
        }
    }

    public enum LampState
    {
        Off,
        On,
        Toggle
    }

    public enum Melody
    {
        Off,
        On,
    }

    public enum ActiveState
    {
        UnKnown,
        Initialized,
        Run,        // Sequence는 Run일때만 돌아야 한다.
        Stop,
        Uninitialized,
        Error
    }

    public enum UserLevels
    {
        //Logout,
        Operator,
        Technician,
        Engineer,
        Administrator
    }

    public enum Logic
    {
        AND,
        OR
    }

    public enum Format
    {
        ASCII,
        NUMBER,
        BCD,
        HEX
    }

    public enum PortNo
    {
        COM1,
        COM2,
        COM3,
        COM4,
        COM5,
        COM6,
        COM7,
        COM8,
        COM9,
        COM10,
        COM11,
        COM12,
        COM13,
        COM14,
        COM15,
        COM16,
        COM17,
        COM18,
        COM19,
        COM20,
        COM21,
        COM22,
        COM23,
        COM24,
        COM25,
        COM26,
        COM27,
        COM28,
        COM29,
        COM30,
        COM31,
        COM32,
        COM33,
        COM34,
        COM35,
        COM36,
        COM37,
        COM38,
        COM39,
        COM40,
        COM41,
        COM42,
        COM43,
        COM44,
        COM45,
        COM46,
        COM47,
        COM48,
        COM49,
        COM50,
        _Emulate,
    }

    public enum WindowScreen
    {
        DISPLAY1,
        DISPLAY2,
        DISPLAY3,
        DISPLAY4,
    }

    public enum CommType
    {
        Analog,
        RS232,
        AnalogRS232
    }

    public enum BaudRate
    {
        Low = 9600,
        Middle = 12400,
        Fast = 15200
    }

    public enum StorageType
    {
        DataBase,
        Xml,
        Text
    }

    public enum HostControlMode
    {
        Offline,
        Control,
        Monitor,
    }
    public enum HostMode
    {
        Offline = 0,
        OnlineControl = 1,
    }
    public enum TrsMode
    {
        MGV,
        AGV,
    }

    public enum PortMode
    {
        Enable,
        Disable,
    }

    public enum CassetteStatus // Mask Frame
    {
        None,
        CstNoneExist,
        CstWaitData,
        CstWaitStart,
        CstWaitProcess,
        CstInProcess,
        CstPause,
        CstCompleteProcess,
        CstRemap,
        CstAbort,
    }

    public enum OperateMode : int
    {
        None = -1,
        Manual = 0,
        SemiAuto = 1,
        Auto = 2,
        Recovery = 3,
    }

    public enum OperatorCallKind
    {
        HostMessage,
        OperationEnd,
        OperationSelect,
        OperationConfirm,
        InterlockMessage,
        SaftyInterlock,
        SilencePopup,
        Hide,
    }

    public enum ProcessDataKind
    {
        NONE,
        EQP_INFO,
        MATERIAL_INFO,
    }

    public enum EqpRunMode : int
    {
        None = -1,
        Stop = 0,
        Start = 1 << 1,
        Pause = 1 << 2,
        Abort = 1 << 3,
    }

    public enum CimPpidType
    {
        NAME,
        ID,
    }

    public enum RcpModifyReason
    {
        Noop = 0,
        Create = 1,     // CIM Specification
        Delete = 2,     // CIM Specification
        Modify = 3,     // CIM Specification
        CimQuery = 4,   // CIM Specification
        Rename = 5,
        Selected = 6,
    }
    public enum EqpState : int
    {
        None,
        Idle,
        Run,
        Down,
        Pause,
        Stop,
        PM,
    }
    public enum SeqState
    {
        READY,
        RUN,
        ALARM,
        STOP,
    }

    public enum AlarmCondition
    {
        AlarmSet,
        AlarmReset
    }
    public enum AlarmLevel
    {
        L = 1,
        S = 2,
    }
    public enum ERRORSTATE : int
    {
        NoError = 0,
        ErrorReported,      // Error reported to OCS
    }

    public static class InitCheckState
    {
        public const string NotReady = "Not Ready";
        public const string Checking = "Checking...";
        public const string ServoHoming = "Homing...";
        public const string ServoEStop = "E-Stop";
        public const string ServoReset = "Reset";
        public const string OK = "OK";
        public const string NG = "NG";
        public const string NoUse = "No Use";
    }

    public enum ProgressAct
    {
        PROC_WAIT, PROC_ING, PROC_END, PROC_ABORT,
    }

    public enum PcSystemType
    {
        General,
        APC, //B&R Automation PC Series
    }

    // for MultipleMode(멀티플렉서 모드) - 1:1 or 1:N
    public enum MuxMode
    {
        Single,
        MultiDrop
    }
    public enum GridViewMode
    {
        OnlyRead,
        ReadWrite,
    }
    public enum TimeUnit
    {
        Sec = 1,
        Min = 60,
        Hour = 3600
    }
    public enum ProcessControlBy
    {
        Setup,
        Recipe
    }
    public enum ProcessCondition
    {
        Stop,
        Run,
        DontCare
    }

    public enum VarType
    {
        None,
        In,
        Out,
        Di,
        Do,
        Ai,
        Ao
    }

    public enum enMaterialAction
    {
        NONE,
        GET,
        PUT,
    }
    public enum enMeasureProc
    {
        Begin,
        Ing,
        End,
    }

    [Flags]
    [Serializable]
    public enum enWorkingFlow
    {
        None = 0,
        Wait = 1 << 1,
        Transfer = 1 << 2,
        Acquire = 1 << 3,
        Deposit = 1 << 4,
    }

    public enum enMaterialKind
    {
        none,
        Foup,
    }

    [Flags]
    public enum enAxisMask : ushort
    {
        aZ = 1 << 0,
        aX = 1 << 1,
        aY = 1 << 2,
        aT = 1 << 3,
        aYT = 1 << 4,
    }
    #region MainStatus
    public enum ControlStatus
    {
        Auto,
        Semi,
        Manual
    }
    public enum RemoteStatus
    {
        Offline,
        Monitor,
        Control
    }

    public enum EqpStatus
    {
        Normal,
        Fault,
        PM
    }
    #endregion
}
