using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.Remoting
{
    [Serializable]
    public class TouchGUI : MarshalByRefObject
    {
        #region Fields
        private static ConcurrentQueue<object> m_WebDataQueue = new ConcurrentQueue<object>();
        private static bool m_HeartBitAlarm = false;
        private static bool m_HeartBitRun = false;
        private static int m_HeartBitCount = 0;
        private static OperateMode m_OpMode = OperateMode.None;
        private static EqpState m_EqState = EqpState.None;
        private static double m_Velocity = 0.0f;
        private static bool m_IsAlarm = false;
        private static string m_AlarmIds = string.Empty;
        private static int m_ObsUpArea = 0;
        private static bool m_ObsUp1 = false;
        private static bool m_ObsUp2 = false;
        private static bool m_ObsUp3 = false;
        private static bool m_ObsUp4 = false;
        private static int m_ObsDownArea = 0;
        private static bool m_ObsDown1 = false;
        private static bool m_ObsDown2 = false;
        private static bool m_ObsDown3 = false;
        private static bool m_ObsDown4 = false;
        private static string m_Destination = "";
        private static string m_CarrierID = "";
        private static bool m_SetOverrideZero = false;
        private static WebGUI m_WebGUI = new WebGUI();
        private WebAction m_WebAction = new WebAction(WebDeviceType.None, WebActionType.None);
        private static string m_ResponseMsg = string.Empty;
        #endregion

        #region Properties
        public bool HeartBitAlarm { get => m_HeartBitAlarm; set => m_HeartBitAlarm = value; }
        public bool HeartBitRun { get => m_HeartBitRun; set => m_HeartBitRun = value; }
        public int HeartBitCount { get => m_HeartBitCount; set => m_HeartBitCount = value; }
        public OperateMode OpMode { get => m_OpMode; set => m_OpMode = value; }
        public EqpState EqState { get => m_EqState; set => m_EqState = value; }
        public double Velocity { get => m_Velocity; set => m_Velocity = value; }
        public bool IsAlarm { get => m_IsAlarm; set => m_IsAlarm = value; }
        public string AlarmIds { get => m_AlarmIds; set => m_AlarmIds = value; }
        public int ObsUpArea { get => m_ObsUpArea; set => m_ObsUpArea = value; }
        public bool ObsUp1 { get => m_ObsUp1; set => m_ObsUp1 = value; }
        public bool ObsUp2 { get => m_ObsUp2; set => m_ObsUp2 = value; }
        public bool ObsUp3 { get => m_ObsUp3; set => m_ObsUp3 = value; }
        public bool ObsUp4 { get => m_ObsUp4; set => m_ObsUp4 = value; }
        public int ObsDownArea { get => m_ObsDownArea; set => m_ObsDownArea = value; }
        public bool ObsDown1 { get => m_ObsDown1; set => m_ObsDown1 = value; }
        public bool ObsDown2 { get => m_ObsDown2; set => m_ObsDown2 = value; }
        public bool ObsDown3 { get => m_ObsDown3; set => m_ObsDown3 = value; }
        public bool ObsDown4 { get => m_ObsDown4; set => m_ObsDown4 = value; }
        public string Destination { get => m_Destination; set => m_Destination = value; }
        public string CarrierID { get => m_CarrierID; set => m_CarrierID = value; }
        public bool SetOverrideZero { get => m_SetOverrideZero = false; set => m_SetOverrideZero = value; }

        public WebGUI Web { get => m_WebGUI; set => m_WebGUI = value; }

        public WebAction WebAction { get => m_WebAction; set => m_WebAction = value; }

        public string ResponseMsg { get => m_ResponseMsg; set => m_ResponseMsg = value; }
        #endregion

        #region Constructor
        public TouchGUI()
        {
            m_OpMode = OperateMode.None;
            m_EqState = EqpState.None;
            m_AlarmIds = string.Empty;
        }

        public void Initialize()
        {
        }
        public void AddWebData()
        {
            try
            {
                while (m_WebDataQueue.Count >= 1)
                {
                    object result = null;
                    m_WebDataQueue.TryDequeue(out result);
                }
                //lock (_SendToServerQueue)
                {
                    m_WebDataQueue.Enqueue(this);
                }
            }
            catch
            {
            }
        }
        public int WebDataCount()
        {
            try
            {
                //lock (_SendToServerQueue)
                {
                    return m_WebDataQueue.Count;
                }
            }
            catch
            {
            }
            return 0;
        }
        public object GetWebData()
        {
            try
            {
                object result = null;
                if (m_WebDataQueue.TryDequeue(out var item))
                {
                    result = item;
                }
                return result;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
    [Serializable]
    public enum WebDeviceType
    {
        None = 0,
        MasterTransfer = 1,
        Slide = 2,
        Rotate = 3,
        Hoist = 4,
        FrontAntiDrop = 5,
        RearAntiDrop = 6,
        FrontSteer = 7,
        RearSteer = 8,
        Gripper = 9,
        AllAntiDrop = 10,
        AllSteer = 11,
    }
    public enum WebActionType
    {
        None = 0,
        JogPlus = 1,
        JogMinus = 2,
        JogStop = 3,
        StepMove = 4,
        ServoOn = 5,
        ServoOff = 6,
        ErrorReset = 7,
        Origin = 8,
        SteerLeft = 9,
        SteerRight = 10,
        AntiDropLock = 11,
        AntiDropUnLock = 12,
        GripperClose = 13,
        GripperOpen = 14,
        Manual = 15,
        Auto = 16,
        Ready = 17,
        Abort = 18,
        Start = 19,
        Stop = 20,
        Pause = 21,
        SteerRepeat = 22,
        AntiDropRepeat = 23,
        GripperRepeat = 24,
        MoveAxis = 25,
        DataRefresh = 26,
        OffsetUpdate = 27,
        CommandAdd = 28,
        CommandDelete = 29,
        OpCallConfirm = 30,
        OpCallBuzzerOff = 31,
        CarrierInstall = 32,
        CarrierNone = 33,
    }
    [Serializable]
    public class WebVelocitySelect
    {
        public double Velocity { get; set; }
        public double Distance { get; set; }
    }
    [Serializable]
    public class WebOffsetUpdate
    {
        public int PortID { get; set; }
        public double DriveLeftOffset { get; set; }
        public double DriveRightOffset { get; set; }
        public double SlideOffset { get; set; }
        public double HoistOffset { get; set; }
        public double RotateOffset { get; set; }
    }

    [Serializable]
    public class WebCommand
    {
        public bool IsValid { get; set; }
        public int CommandType { get; set; }
        public string CommandID { get; set; }
        public string CassetteID { get; set; }
        public int SourceID { get; set; }
        public int DestinationID { get; set; }
        public int TypeOfDestination { get; set; }
        public double TargetNodeToDistance { get; set; }
        public int WaitTime { get; set; }
        public int TotalCount { get; set; }
    }
    [Serializable]
    public class WebAction
    {

        private WebDeviceType m_DeviceType = WebDeviceType.None;
        private WebActionType m_ActionType = WebActionType.None;
        private WebVelocitySelect m_VelocitySelect = new WebVelocitySelect();
        private WebOffsetUpdate m_OffsetUpdate = new WebOffsetUpdate();
        private DateTime m_UpdateTime = DateTime.MinValue;
        private WebCommand m_WebCommand = new WebCommand();


        public WebDeviceType RemoteDevice { get => m_DeviceType; set => m_DeviceType = value; }
        public WebActionType RemoteAction { get => m_ActionType; set => m_ActionType = value; }
        public WebVelocitySelect RemoteVelocity { get => m_VelocitySelect; set => m_VelocitySelect = value; }
        public WebOffsetUpdate OffsetUpdate { get => m_OffsetUpdate; set => m_OffsetUpdate = value; }
        public DateTime UpdateTime { get => m_UpdateTime; set => m_UpdateTime = value; }
        public WebCommand WebCommand { get => m_WebCommand; set => m_WebCommand = value; }



        public WebAction(WebDeviceType deviceType, WebActionType actionType, WebVelocitySelect velocitySelect)
        {
            m_DeviceType = deviceType;
            m_ActionType = actionType;
            m_VelocitySelect = velocitySelect;
            m_UpdateTime = DateTime.Now;
        }
        public WebAction(WebDeviceType deviceType, WebActionType actionType, WebOffsetUpdate offsetUpdate)
        {
            m_DeviceType = deviceType;
            m_ActionType = actionType;
            m_OffsetUpdate = offsetUpdate;
            m_UpdateTime = DateTime.Now;
        }
        public WebAction(WebDeviceType deviceType, WebActionType actionType)
        {
            m_DeviceType = deviceType;
            m_ActionType = actionType;
            m_UpdateTime = DateTime.Now;
        }
    }
}
