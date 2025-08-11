using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sineva.VHL.IF.OCS
{
    [Serializable]
    public class OCSStatus
    {
        #region Fields
        private bool m_Connected = false;
        private bool m_ConnectError = false;
        private bool m_StatusReportOk = false;
        private bool m_StatusReportStart = false;
        private bool m_OCSRestartReschedule = false;
        private int m_MessageSendInterval = 200;
        private List<int> m_PathRequestRecvNodes = new List<int>(); // Path Request 했을때 받은 Node List

        private int m_PathRequestStartNode = 0;
        private int m_PathRequestEndNode = 0;
        private PathRequestType m_PathRequestType = PathRequestType.None;

        private MapDataType m_MapDataRequestType = MapDataType.ALL;
        private Queue<VehicleEvent> m_EventQueue = new Queue<VehicleEvent>();
        private List<VehicleEvent> m_ReportedEvents = new List<VehicleEvent>();

        private VehicleOperationResult m_AutoTeachingResult = VehicleOperationResult.Success;
        private IDReadResult m_CarrierIDScanResult = IDReadResult.Success;
        private string m_CarrierIDScanRFIDTag = string.Empty;
        #endregion

        #region OCS Status Report Variable
        private int m_VehicleNumber = 1;
        private bool m_InRailStatus = false;
        private bool m_BusyStatus = false;
        private bool m_PowerOnStatus = false;
        private bool m_AutoRunStatus = false;
        private bool m_PauseStatus = false;
        private bool m_ErrorStatus = false;
        private bool m_MovingStatus = false;
        private bool m_AutoTeachingStatus = false;
        private bool m_CarrierExistStatus = false;
        private bool[] m_InputStatus = new bool[64];
        private bool[] m_OutputStatus = new bool[64];
        private int m_CurrentSpeed = 0;
        private int m_BarcodeValue1 = 0;
        private int m_BarcodeValue2 = 0;
        private ushort m_CurrentNodeNo = 0;
        private int m_CurrentPortId = 0;
        private int m_TargetPortId = 0;
        private ushort m_NextNodeNo = 0;
        private ushort m_TargetNodeNo = 0;
        private string m_CurrentCommandID = string.Empty;
        private bool m_BCRDirectionLeft = false;
        private int m_CurrentLinkNo = 0;
        private int m_CurrentVehiclePosition = 0;
        private int m_CurrentLinkRemainDistance = 0;
        private bool m_UseFrontDetectionSensor = false;
        private bool m_UseJCS = false;
        private string m_AbortCancelCommandID = string.Empty;
        #endregion

        #region Fields - Message Control Flag
        private Dictionary<InterfaceFlag, FlagValue> m_IFFlag = new Dictionary<InterfaceFlag, FlagValue>();
        #endregion

        #region Event
        public event DelVoid_ObjectObject delStatusSendReply;
        public event DelVoid_ObjectObject delAlarmReportReply;
        public event DelVoid_ObjectObject delEventReportReply;
        public event DelVoid_ObjectObject delCommonProcessRequest;
        public event DelVoid_ObjectObject delCommandProcessRequest;
        public event DelVoid_ObjectObject delMapDataProcessRequest;

        public event DelVoid_ObjectObject delMessageReceived;
        public event DelVoid_ObjectObject delMessageSent;
        public event DelVoid_Object delSocketClose;
        #endregion

        #region Properties
        public bool Connected
        {
            get { return m_Connected; }
            set { m_Connected = value; }
        }

        public bool ConnectError
        {
            get { return m_ConnectError; }
            set { m_ConnectError = value; }
        }
        public bool StatusReportOk
        {
            get { return m_StatusReportOk; }
            set { m_StatusReportOk = value; }
        }
        public bool StatusReportStart
        {
            get { return m_StatusReportStart; }
            set { m_StatusReportStart = value; }
        }
        public bool OCSRestartReschedule
        {
            get { return m_OCSRestartReschedule; }
            set { m_OCSRestartReschedule = value; }
        }
        public int MessageSendInterval
        {
            get { return m_MessageSendInterval; }
            set { m_MessageSendInterval = value; }
        }
        public List<int> PathRequestRecvNodes
        {
            get { return m_PathRequestRecvNodes; }
            set { m_PathRequestRecvNodes = value; }
        }
        public MapDataType MapDataRequestType
        {
            get { return m_MapDataRequestType; }
            set { m_MapDataRequestType = value; }
        }

        public int PathRequestStartNode
        {
            get { return m_PathRequestStartNode; }
            set { m_PathRequestStartNode = value; }
        }
        public int PathRequestEndNode
        {
            get { return m_PathRequestEndNode; }
            set { m_PathRequestEndNode = value; }
        }
        public PathRequestType PathRequestType
        {
            get { return m_PathRequestType; }
            set { m_PathRequestType = value; }
        }
        [Browsable(false), XmlIgnore]
        public Queue<VehicleEvent> EventQueue
        {
            get { return m_EventQueue; }
            set { m_EventQueue = value; }
        }
        [Browsable(false), XmlIgnore]
        public VehicleOperationResult AutoTeachingResult
        {
            get { return m_AutoTeachingResult; }
            set { m_AutoTeachingResult = value; }
        }
        [Browsable(false), XmlIgnore]
        public IDReadResult CarrierIDScanResult
        {
            get { return m_CarrierIDScanResult; }
            set { m_CarrierIDScanResult = value; }
        }
        [Browsable(false), XmlIgnore]
        public string CarrierIDScanRFIDTag
        {
            get { return m_CarrierIDScanRFIDTag; }
            set { m_CarrierIDScanRFIDTag = value; }
        }        
        #endregion

        #region Properties - Status Report Variable
        public int VehicleNumber
        {
            get { return m_VehicleNumber; }
            set { m_VehicleNumber = value; }
        }
        public bool InRailStatus
        {
            get { return m_InRailStatus; }
            set { m_InRailStatus = value; }
        }
        public bool BusyStatus
        {
            get { return m_BusyStatus; }
            set { m_BusyStatus = value; }
        }
        public bool PowerOnStatus
        {
            get { return m_PowerOnStatus; }
            set { m_PowerOnStatus = value; }
        }
        public bool AutoRunStatus
        {
            get { return m_AutoRunStatus; }
            set { m_AutoRunStatus = value; }
        }
        public bool PauseStatus
        {
            get { return m_PauseStatus; }
            set { m_PauseStatus = value; }
        }
        public bool ErrorStatus
        {
            get { return m_ErrorStatus; }
            set { m_ErrorStatus = value; }
        }
        public bool MovingStatus
        {
            get { return m_MovingStatus; }
            set { m_MovingStatus = value; }
        }
        public bool AutoTeachingStatus
        {
            get { return m_AutoTeachingStatus; }
            set { m_AutoTeachingStatus = value; }
        }
        public bool CarrierExistStatus
        {
            get { return m_CarrierExistStatus; }
            set { m_CarrierExistStatus = value; }
        }
        public bool[] InputStatus
        {
            get { return m_InputStatus; }
            set { m_InputStatus = value; }
        }
        public bool[] OutputStatus
        {
            get { return m_OutputStatus; }
            set { m_OutputStatus = value; }
        }
        public int CurrentSpeed
        {
            get { return m_CurrentSpeed; }
            set { m_CurrentSpeed = value; }
        }
        public int BarcodeValue1
        {
            get { return m_BarcodeValue1; }
            set { m_BarcodeValue1 = value; }
        }
        public int BarcodeValue2
        {
            get { return m_BarcodeValue2; }
            set { m_BarcodeValue2 = value; }
        }
        public ushort CurrentNodeNo
        {
            get { return m_CurrentNodeNo; }
            set { m_CurrentNodeNo = value; }
        }
        public int CurrentPortId
        {
            get { return m_CurrentPortId; }
            set { m_CurrentPortId = value; }
        }
        public ushort NextNodeNo
        {
            get { return m_NextNodeNo; }
            set { m_NextNodeNo = value; }
        }
        public ushort TargetNodeNo
        {
            get { return m_TargetNodeNo; }
            set { m_TargetNodeNo = value; }
        }
        public int TargetPortId
        {
            get { return m_TargetPortId; }
            set { m_TargetPortId = value; }
        }
        public string CurrentCommandID
        {
            get { return m_CurrentCommandID; }
            set { m_CurrentCommandID = value; }
        }
        public bool BCRDirectionLeft
        {
            get { return m_BCRDirectionLeft; }
            set { m_BCRDirectionLeft = value; }
        }
        public int CurrentLinkNo
        {
            get { return m_CurrentLinkNo; }
            set { m_CurrentLinkNo = value; }
        }
        public int CurrentVehiclePosition
        {
            get { return m_CurrentVehiclePosition; }
            set { m_CurrentVehiclePosition = value; }
        }
        public int CurrentLinkRemainDistance
        {
            get { return m_CurrentLinkRemainDistance; }
            set { m_CurrentLinkRemainDistance = value; }
        }
        public bool UseFrontDetectionSensor
        {
            get { return m_UseFrontDetectionSensor; }
            set { m_UseFrontDetectionSensor = value; }
        }
        public bool UseJCS
        {
            get { return m_UseJCS; }
            set { m_UseJCS = value; }
        }
        public string AbortCancelCommandID
        {
            get { return m_AbortCancelCommandID; }
            set { m_AbortCancelCommandID = value; }
        }
        #endregion


        #region Constructor
        public OCSStatus()
        {
            try
            {
                List<InterfaceFlag> flags = Enum.GetValues(typeof(InterfaceFlag)).Cast<InterfaceFlag>().ToList();
                foreach (InterfaceFlag flag in flags)
                {
                    m_IFFlag.Add(flag, FlagValue.OFF);
                }
            }
            catch
            {
            }
        }
        #endregion

        #region Methods - event
        public void FireStatusSendReply(object obj1, object obj2)
        {
            if (delStatusSendReply != null)
                delStatusSendReply?.Invoke(obj1, obj2);
        }
        public void FireAlarmReportReply(object obj1, object obj2)
        {
            if (delAlarmReportReply != null)
                delAlarmReportReply?.Invoke(obj1, obj2);
        }
        public void FireEventReportReply(object obj1, object obj2)
        {
            if (delEventReportReply != null)
                delEventReportReply?.Invoke(obj1, obj2);
        }
        public void FireCommonProcessRequest(object obj1, object obj2)
        {
            if (delCommonProcessRequest != null)
                delCommonProcessRequest?.Invoke(obj1, obj2);
        }
        public void FireCommandProcessRequest(object obj1, object obj2)
        {
            if (delCommandProcessRequest != null)
                delCommandProcessRequest?.Invoke(obj1, obj2);
        }
        public void FireMapDataProcessRequest(object obj1, object obj2)
        {
            if (delMapDataProcessRequest != null)
                delMapDataProcessRequest?.Invoke(obj1, obj2);
        }
        public void FireMessageReceived(object obj1, object obj2)
        {
            if (delMessageReceived != null)
                delMessageReceived?.Invoke(obj1, obj2);
        }
        public void FireMessageSent(object obj1, object obj2)
        {
            if (delMessageSent != null)
                delMessageSent?.Invoke(obj1, obj2);
        }
        public void FireSocketClose(object obj)
        {
            if (delSocketClose != null)
                delSocketClose?.Invoke(obj);
        }
        #endregion

        #region Methods - status report
        public VehicleIF_VehicleStatusDataSend GetStatusSendMessage()
        {
            try
            {
                VehicleIF_VehicleStatusDataSend message = new VehicleIF_VehicleStatusDataSend
                {
                    VehicleNumber = m_VehicleNumber,
                    InRailStatus = m_InRailStatus,
                    PowerOnStatus = m_PowerOnStatus,
                    AutoRunStatus = m_AutoRunStatus,
                    PauseStatus = m_PauseStatus,
                    ErrorStatus = m_ErrorStatus,
                    MovingStatus = m_MovingStatus,
                    BusyStatus = m_BusyStatus,
                    AutoTeachingStatus = m_AutoTeachingStatus,
                    CarrierExistStatus = m_CarrierExistStatus,
                    CurrentSpeed = m_CurrentSpeed,
                    BarcodeValue1 = m_BarcodeValue1,
                    BarcodeValue2 = m_BarcodeValue2,
                    CurrentNodeNo = m_CurrentNodeNo,
                    CurrentLinkNo = m_CurrentLinkNo,
                    CurrentVehiclePosition = m_CurrentVehiclePosition,
                    NextNodeNo = m_NextNodeNo,
                    TargetNodeNo = m_TargetNodeNo,
                    CurrentCommandID = m_CurrentCommandID,
                    UseFrontDetectionSensor = m_UseFrontDetectionSensor,
                    UseJCS = m_UseJCS,
                };

                Array.Copy(m_InputStatus, 0, message.InputStatus, 0, message.InputStatus.Length);
                Array.Copy(m_OutputStatus, 0, message.OutputStatus, 0, message.OutputStatus.Length);

                return message;
            }
            catch
            {
                return null;
            }
        }
        public void SendVehicleEvent(VehicleEvent eventCode)
        {
            if (!m_ReportedEvents.Contains(eventCode)) // 명령 시작후 한번씩만 보내달라고 하네!
            {
                m_ReportedEvents.Add(eventCode);
                EventQueue.Enqueue(eventCode);
            }
        }
        public void ClearVehicleEvent()
        {
            m_ReportedEvents.Clear();
        }
        #endregion

        #region Methods - Flag Control
        public FlagValue GetFlag(InterfaceFlag flag)
        {
            return m_IFFlag[flag];
        }
        public void SetFlag(InterfaceFlag flag, FlagValue value)
        {
            if (flag == InterfaceFlag.PathRequest && value == FlagValue.ON)
            {
                m_IFFlag[InterfaceFlag.PathReceived] = FlagValue.OFF;
            }
            m_IFFlag[flag] = value;
        }
        public void ResetFlag(InterfaceFlag flag)
        {
            m_IFFlag[flag] = FlagValue.OFF;
        }
        public void ResetAllFlag()
        {
            List<InterfaceFlag> flags = Enum.GetValues(typeof(InterfaceFlag)).Cast<InterfaceFlag>().ToList();
            foreach (InterfaceFlag flag in flags) m_IFFlag[flag] = FlagValue.OFF;
        }
        #endregion
    }
}
