using Sineva.VHL.Data.Process;
using Sineva.VHL.Library.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.Remoting
{
    [Serializable]
    public class WebGUI
    {
        private List<ServoStatus> m_ServoStatusList = new List<ServoStatus>();
        private List<IoStatus> m_IOList = new List<IoStatus>();
        private List<TeachingPortData> m_TeachingPortDataList = new List<TeachingPortData>();
        private DateTime m_UpdateTime = DateTime.Now;
        private TransferCommand m_CurrentTransferCommand = null;
        private List<CommandData> m_TransferCommands = null;
        private VehicleStatus M_VehicleStatus = null;
        private enSteerDirection m_RearSteerDirection = enSteerDirection.DontCare;
        private enSteerDirection m_FrontSteerDirection = enSteerDirection.DontCare;
        private bool m_EqpInitComp = false;
        private bool m_EqpInitIng = false;
        private EqpRunMode m_EqpRunMode = EqpRunMode.Stop;
        private bool m_IsAutoTeachingMode = false;
        private string m_OcsState = string.Empty;
        private string m_JcsState = string.Empty;
        private bool m_JcsPermit = false;
        private bool m_AutoDoorPermit = false;
        private bool m_Pbs1 = false;
        private bool m_FrontAntiDropLock = false;
        private bool m_RearAntiDropLock = false;
        private bool m_FrontAntiDropUnLock = false;
        private bool m_RearAntiDropUnLock = false;
        private bool m_GripperOpen = false;
        private bool m_GripperClose = false;
        private bool m_HoistHome = false;
        private bool m_HoistUp = false;
        private bool m_HoistLimit = false;
        private bool m_LeftProductExist = false;
        private bool m_RightProductExist = false;
        private bool m_DlgNotifyErrorShow = false;
        private bool m_DlgOpCallSingleMessageShow = false;
        private bool m_DlgOpCallCommandShow = false;
        private bool m_DlgEqpInitShow = false;
        private bool m_FoupExist = false;
        private string m_OpCallMsg = "";
        private List<IniTag> m_IniTags = new List<IniTag>();
        public TransferCommand TransferCommand { get => m_CurrentTransferCommand; set => m_CurrentTransferCommand = value; }
        public List<CommandData> TransferCommands { get => m_TransferCommands; set => m_TransferCommands = value; }
        public List<IniTag> IniTags { get => m_IniTags; set => m_IniTags = value; }
        public bool EqpInitComp { get => m_EqpInitComp; set => m_EqpInitComp = value; }
        public bool EqpInitIng { get => m_EqpInitIng; set => m_EqpInitIng = value; }
        public EqpRunMode EqpRunMode { get => m_EqpRunMode; set => m_EqpRunMode = value; }
        public bool IsAutoTeachingMode { get => m_IsAutoTeachingMode; set => m_IsAutoTeachingMode = value; }
        public VehicleStatus VehicleStatus { get => M_VehicleStatus; set => M_VehicleStatus = value; }
        public DateTime UpdateTime { get => m_UpdateTime; set => m_UpdateTime = value; }
        public enSteerDirection FrontSteerDirection { get => m_FrontSteerDirection; set => m_FrontSteerDirection = value; }
        public enSteerDirection RearSteerDirection { get => m_RearSteerDirection; set => m_RearSteerDirection = value; }
        public List<ServoStatus> ServoStatusList { get => m_ServoStatusList; set => m_ServoStatusList = value; }
        public List<IoStatus> IOList { get => m_IOList; set => m_IOList = value; }
        public List<TeachingPortData> TeachingPortDataList { get => m_TeachingPortDataList; set => m_TeachingPortDataList = value; }
        public string OcsState { get => m_OcsState; set => m_OcsState = value; }
        public string JcsState { get => m_JcsState; set => m_JcsState = value; }
        public bool JcsPermit { get => m_JcsPermit; set => m_JcsPermit = value; }
        public bool AutoDoorPermit { get => m_AutoDoorPermit; set => m_AutoDoorPermit = value; }
        public bool Pbs1 { get => m_Pbs1; set => m_Pbs1 = value; }
        public bool FrontAntiDropLock { get => m_FrontAntiDropLock; set => m_FrontAntiDropLock = value; }
        public bool RearAntiDropLock { get => m_RearAntiDropLock; set => m_RearAntiDropLock = value; }
        public bool FrontAntiDropUnLock { get => m_FrontAntiDropUnLock; set => m_FrontAntiDropUnLock = value; }
        public bool RearAntiDropUnLock { get => m_RearAntiDropUnLock; set => m_RearAntiDropUnLock = value; }
        public bool GripperOpen { get => m_GripperOpen; set => m_GripperOpen = value; }
        public bool GripperClose { get => m_GripperClose; set => m_GripperClose = value; }
        public bool HoistHome { get => m_HoistHome; set => m_HoistHome = value; }

        public bool HoistUp { get => m_HoistUp; set => m_HoistUp = value; }

        public bool HoistLimit { get => m_HoistLimit; set => m_HoistLimit = value; }

        public bool LeftProductExist { get => m_LeftProductExist; set => m_LeftProductExist = value; }

        public bool RightProductExist { get => m_RightProductExist; set => m_RightProductExist = value; }
        public bool DlgNotifyErrorShow { get => m_DlgNotifyErrorShow; set => m_DlgNotifyErrorShow = value; }
        public bool DlgOpCallSingleMessageShow { get => m_DlgOpCallSingleMessageShow; set => m_DlgOpCallSingleMessageShow = value; }
        public bool DlgOpCallCommandShow { get => m_DlgOpCallCommandShow; set => m_DlgOpCallCommandShow = value; }
        public bool DlgEqpInitShow { get => m_DlgEqpInitShow; set => m_DlgEqpInitShow = value; }

        public string OpCallMsg { get => m_OpCallMsg; set => m_OpCallMsg = value; }
        public bool FoupExist { get => m_FoupExist; set => m_FoupExist = value; }


    }
    [Serializable]
    public class ServoStatus
    {
        private int m_AxisId = 0;
        private string m_AxisName = string.Empty;
        private double m_Position = 0.0;
        private double m_Speed = 0.0;
        private double m_Torque = 0.0;
        private bool m_IsAlarm = false;
        private bool m_InPosition = false;
        private bool m_HomeEnd = false;
        private bool m_ServoOn = false;
        private bool m_IsOrg = false;
        private bool m_LimitN = false;
        private bool m_LimitP = false;
        private string m_AlarmCode = string.Empty;

        public int AxisId { get => m_AxisId; set => m_AxisId = value; }
        public string AxisName { get => m_AxisName; set => m_AxisName = value; }
        public double Position { get => m_Position; set => m_Position = value; }
        public double Speed { get => m_Speed; set => m_Speed = value; }
        public double Torque { get => m_Torque; set => m_Torque = value; }
        public bool IsAlarm { get => m_IsAlarm; set => m_IsAlarm = value; }
        public bool InPosition { get => m_InPosition; set => m_InPosition = value; }
        public bool HomeEnd { get => m_HomeEnd; set => m_HomeEnd = value; }
        public bool ServoOn { get => m_ServoOn; set => m_ServoOn = value; }
        public bool IsOrg { get => m_IsOrg; set => m_IsOrg = value; }
        public bool LimitN { get => m_LimitN; set => m_LimitN = value; }
        public bool LimitP { get => m_LimitP; set => m_LimitP = value; }
        public string AlarmCode { get => m_AlarmCode; set => m_AlarmCode = value; }

    }
    [Serializable]
    public class CommandData
    {
        private bool m_IsValid = false; // 데이터 유효성 판단.
        private int m_ProcessCommand = 0;
        private string m_CommandID = string.Empty;
        private string m_CassetteID = string.Empty;
        private int m_SourceID = 0;
        private int m_DestinationID = 0;
        private enGoCommandType m_TypeOfDestination = enGoCommandType.None;
        private double m_TargetNodeToDistance = 0.0f;

        public bool IsValid { get => m_IsValid; set => m_IsValid = value; }
        public int ProcessCommand { get => m_ProcessCommand; set => m_ProcessCommand = value; }

        public string CommandID { get => m_CommandID; set => m_CommandID = value; }
        public string CassetteID { get => m_CassetteID; set => m_CassetteID = value; }
        public int SourceID { get => m_SourceID; set => m_SourceID = value; }

        public int DestinationID { get => m_DestinationID; set => m_DestinationID = value; }
        public enGoCommandType TypeOfDestination { get => m_TypeOfDestination; set => m_TypeOfDestination = value; }
        public double TargetNodeToDistance
        {
            get { return m_TargetNodeToDistance; }
            set { m_TargetNodeToDistance = value; }
        }
    }
    [Serializable]
    public class IoStatus
    {
        private string m_Name = string.Empty;
        private bool m_IsBContact = false;
        private IoType m_IoType = IoType.DI;
        private int m_Id = 0;
        private string m_Description = string.Empty;
        private string m_WiringNo = string.Empty;
        private string m_State = string.Empty;

        public int ID { get => m_Id; set => m_Id = value; }

        public string Name { get => m_Name; set => m_Name = value; }
        public string Description { get => m_Description; set => m_Description = value; }
        public string WiringNo { get => m_WiringNo; set => m_WiringNo = value; }
        public bool IsBContact { get => m_IsBContact; set => m_IsBContact = value; }
        public IoType IoType { get => m_IoType; set => m_IoType = value; }
        public string State { get => m_State; set => m_State = value; }
    }

    [Serializable]
    public class TeachingPortData
    {
        public bool PIOUsed { get; set; }
        public int State { get; set; }
        public bool PortProhibition { get; set; }
        public bool OffsetUsed { get; set; }
        public int PortType { get; set; }
        public int PortID { get; set; }
        public int LinkID { get; set; }
        public int NodeID { get; set; }
        public int PIOID { get; set; }
        public int PIOCH { get; set; }
        public int PIOCS { get; set; }
        public double SlidePosition { get; set; }
        public double RotatePosition { get; set; }
        public double BeforeHoistPosition { get; set; }
        public double HoistPosition { get; set; }
        public double UnloadSlidePosition { get; set; }
        public double UnloadRotatePosition { get; set; }
        public double BeforeUnloadHoistPosition { get; set; }
        public double UnloadHoistPosition { get; set; }
        public double BarcodeLeft { get; set; }
        public double BarcodeRight { get; set; }
        public int PBSSelectNo { get; set; }
        public bool PBSUsed { get; set; }
        public double DriveLeftOffset { get; set; }
        public double DriveRightOffset { get; set; }
        public double SlideOffset { get; set; }
        public double HoistOffset { get; set; }
        public double RotateOffset { get; set; }
        public int ProfileExistPosition { get; set; }
    }

    [Serializable]
    public class IniTag
    {
        public string ItemName { get; set; }
        public InitState State { get; set; }
        public string CheckStatus { get; set; }
    }
}
