using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Net;
using System.Windows.Forms;
using System.IO;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Library;
using Sineva.VHL.Data.Alarm;
using System.Drawing.Design;
using Sineva.VHL.IF.Vision;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Data;
using Sineva.VHL.Data.Process;
using Sineva.OHT.IF.EEIP.NET;

namespace Sineva.VHL.Device
{
    [Serializable]
    public enum ThetaDirection
    {
        CenterVertical,     // X방향 중심의 수직선
        CenterHorizontal,   // Y방향 중심의 수평선
    }

    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class DevAutoTeaching : _Device
    {
        private const string DevName = "DevAutoTeaching";

        #region Fields
        private VisionClient m_VisionClient = null;
        private EEIPClient m_HeightSensorClient = null;

        private int m_T3Timeout = 45;
        private int m_FindRetry = 5;
        private string m_VisionIpAddress = "127.0.0.1"; //hjyou
        private ushort m_VisionPortNumber = 7001;//hjyou

        private int m_ReflectivePIOID = 201;
        private int m_ReflectivePIOCH = 255;

        /// <summary> //////////////////////////
        /// Vision Command
        /// </summary> /////////////////////////
        private V_Trigger1 _vcmTrigger1 = new V_Trigger1();
        private V_Trigger2 _vcmTrigger2 = new V_Trigger2();
        private V_Trigger3 _vcmTrigger3 = new V_Trigger3();
        private V_Trigger4 _vcmTrigger4 = new V_Trigger4();
        private V_TriggerAll _vcmTriggerAll = new V_TriggerAll();
        private V_DrivingMode _vcmDrivingMode = new V_DrivingMode();
        private V_SettingMode _vcmSettingMode = new V_SettingMode();
        private V_Reset _vcmReset = new V_Reset();
        private V_ReBooting _vcmReBooting = new V_ReBooting();
        private V_SettingSave _vcmSettingSave = new V_SettingSave();
        private V_ErrorClear _vcmErrorClear = new V_ErrorClear();
        private V_SwitchDisplay _vcmSwitchDisplay = new V_SwitchDisplay();
        private V_TriggerReset _vcmTriggerReset = new V_TriggerReset();
        private V_ReadMode _vcmReadMode = new V_ReadMode();
        private V_InspectionSetting _vcmInspectionSetting = new V_InspectionSetting();
        private V_InspectionRead _vcmInspectionRead = new V_InspectionRead();
        private V_ShutterSpeed _vcmShutterSpeed = new V_ShutterSpeed();
        private V_CameraSensitivity _vcmCameraSensitivity = new V_CameraSensitivity();
        private V_TriggerDelay _vcmTriggerDelay = new V_TriggerDelay();
        private V_BasicScreen _vcmBasicScreen = new V_BasicScreen();
        private V_ExecutionCondition _vcmExecutionCondition = new V_ExecutionCondition();
        private V_ExecutionConditionRead _vcmExecutionConditionRead = new V_ExecutionConditionRead();
        private V_ChangeTriggerInterlock _vcmChangeTriggerInterlock = new V_ChangeTriggerInterlock();
        private V_VersionInformationRead _vcmVersionInformationRead = new V_VersionInformationRead();
        ////////////////////////////////////////
        /// <summary> //////////////////////////
        /// Vision Alarm
        /// </summary> /////////////////////////
        private AlarmData m_ALM_NotDefine = null;
        private AlarmData m_ALM_SettingError = null;
        private AlarmData m_ALM_VisionCommandT3Alarm = null;
        private AlarmData m_ALM_VisionDisconnectAlarm = null;
        private AlarmData m_ALM_VisionTrigger1Alarm = null;
        private AlarmData m_ALM_VisionTrigger2Alarm = null;
        private AlarmData m_ALM_VisionTrigger3Alarm = null;
        private AlarmData m_ALM_VisionTrigger4Alarm = null;
        private AlarmData m_ALM_VisionTriggerAllAlarm = null;
        private AlarmData m_ALM_VisionDrivingModeAlarm = null;
        private AlarmData m_ALM_VisionSettingModeAlarm = null;
        private AlarmData m_ALM_VisionResetAlarm = null;
        private AlarmData m_ALM_VisionRebootingAlarm = null;
        private AlarmData m_ALM_VisionSettingSaveAlarm = null;
        private AlarmData m_ALM_VisionErrorClearAlarm = null;
        private AlarmData m_ALM_VisionSwitchDisplayAlarm = null;
        private AlarmData m_ALM_VisionTriggerResetAlarm = null;
        private AlarmData m_ALM_VisionReadModeAlarm = null;
        private AlarmData m_ALM_VisionInspectionSettingAlarm = null;
        private AlarmData m_ALM_VisionInspectionReadAlarm = null;
        private AlarmData m_ALM_VisionShutterSpeedAlarm = null;
        private AlarmData m_ALM_VisionCameraSensitivityAlarm = null;
        private AlarmData m_ALM_VisionTriggerDelayAlarm = null;
        private AlarmData m_ALM_VisionBasicScreenAlarm = null;
        private AlarmData m_ALM_VisionExecutionConditionAlarm = null;
        private AlarmData m_ALM_VisionChangeTriggerInterlockAlarm = null;
        private AlarmData m_ALM_VisionFindAlarm = null;

        #region Fields - I/O
        private DevEqPIO m_EqPio = new DevEqPIO();
        #endregion

        #region Fileds - Height Sensor
        private string m_HeightSensorIpAddress = "192.168.1.221"; //hjyou
        private ushort m_HeightSensorPortNumber = 44188;//hjyou
        #endregion

        private SeqRemoteControl m_SeqRemoteControl = null;
        private SeqMonitor m_SeqMonitor = null;
        private bool m_Start = false;
        private bool m_Stop = false;
        private bool m_Ready = false;

        private double m_AlignDiffX = 0.0f;
        private double m_AlignDiffY = 0.0f;
        private double m_AlignDiffT = 0.0f;

        private bool m_OnlySensorOffsetFind = false;

        #endregion

        #region Property - Device
        [Category("T3 Timeout Setting(sec)"), DeviceSetting(true)]
        public int T3Timeout
        {
            get { return m_T3Timeout; }
            set { m_T3Timeout = value; }
        }
        [Category("Vision Find Retry Count"), DeviceSetting(true)]
        public int FindRetry
        {
            get { return m_FindRetry; }
            set { m_FindRetry = value; }
        }
        [Category("VISION PC IP Address")]
        public string VisionIpAddress
        {
            get { return m_VisionIpAddress; }
            set { m_VisionIpAddress = value; }
        }
        [Category("VISION PC Port Number")]
        public ushort VisionPortNumber
        {
            get { return m_VisionPortNumber; }
            set { m_VisionPortNumber = value; }
        }
        [Category("\"Height Sensor IP Address")]
        public string HeightSensorIpAddress
        {
            get { return m_HeightSensorIpAddress; }
            set { m_HeightSensorIpAddress = value; }
        }
        [Category("Height Sensor Port Number")]
        public ushort HeightSensorPortNumber
        {
            get { return m_HeightSensorPortNumber; }
            set { m_HeightSensorPortNumber = value; }
        }
        [Category("Reflective Sensor"), DisplayName("PIO ID")]
        public int ReflectivePIOID { get => m_ReflectivePIOID; set => m_ReflectivePIOID = value; }
        [Category("Reflective Sensor"), DisplayName("PIO Channel")]
        public int ReflectivePIOCH { get => m_ReflectivePIOCH; set => m_ReflectivePIOCH = value; }
        #endregion

        #region Property - Vision Command
        [XmlIgnore(), Browsable(false)]
        public V_Trigger1 VcmTrigger1
        {
            get { return _vcmTrigger1; }
            set { _vcmTrigger1 = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_Trigger2 VcmTrigger2
        {
            get { return _vcmTrigger2; }
            set { _vcmTrigger2 = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_Trigger3 VcmTrigger3
        {
            get { return _vcmTrigger3; }
            set { _vcmTrigger3 = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_Trigger4 VcmTrigger4
        {
            get { return _vcmTrigger4; }
            set { _vcmTrigger4 = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_TriggerAll VcmTriggerAll
        {
            get { return _vcmTriggerAll; }
            set { _vcmTriggerAll = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_DrivingMode VcmDrivingMode
        {
            get { return _vcmDrivingMode; }
            set { _vcmDrivingMode = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_SettingMode VcmSettingMode
        {
            get { return _vcmSettingMode; }
            set { _vcmSettingMode = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_Reset VcmReset
        {
            get { return _vcmReset; }
            set { _vcmReset = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_ReBooting VcmReBooting
        {
            get { return _vcmReBooting; }
            set { _vcmReBooting = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_SettingSave VcmSettingSave
        {
            get { return _vcmSettingSave; }
            set { _vcmSettingSave = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_ErrorClear VcmErrorClear
        {
            get { return _vcmErrorClear; }
            set { _vcmErrorClear = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_SwitchDisplay VcmSwitchDisplay
        {
            get { return _vcmSwitchDisplay; }
            set { _vcmSwitchDisplay = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_TriggerReset VcmTriggerReset
        {
            get { return _vcmTriggerReset; }
            set { _vcmTriggerReset = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_ReadMode VcmReadMode
        {
            get { return _vcmReadMode; }
            set { _vcmReadMode = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_InspectionSetting VcmInspectionSetting
        {
            get { return _vcmInspectionSetting; }
            set { _vcmInspectionSetting = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_InspectionRead VcmInspectionRead
        {
            get { return _vcmInspectionRead; }
            set { _vcmInspectionRead = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_ShutterSpeed VcmShutterSpeed
        {
            get { return _vcmShutterSpeed; }
            set { _vcmShutterSpeed = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_CameraSensitivity VcmCameraSensitivity
        {
            get { return _vcmCameraSensitivity; }
            set { _vcmCameraSensitivity = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_TriggerDelay VcmTriggerDelay
        {
            get { return _vcmTriggerDelay; }
            set { _vcmTriggerDelay = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_BasicScreen VcmBasicScreen
        {
            get { return _vcmBasicScreen; }
            set { _vcmBasicScreen = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_ExecutionCondition VcmExecutionCondition
        {
            get { return _vcmExecutionCondition; }
            set { _vcmExecutionCondition = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_ExecutionConditionRead VcmExecutionConditionRead
        {
            get { return _vcmExecutionConditionRead; }
            set { _vcmExecutionConditionRead = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_ChangeTriggerInterlock VcmChangeTriggerInterlock
        {
            get { return _vcmChangeTriggerInterlock; }
            set { _vcmChangeTriggerInterlock = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public V_VersionInformationRead VcmVersionInformationRead
        {
            get { return _vcmVersionInformationRead; }
            set { _vcmVersionInformationRead = value; }
        }
        #endregion

        #region AlarmData
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_NotDefine
        {
            get { return m_ALM_NotDefine; }
            set { m_ALM_NotDefine = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_SettingError 
        {
            get { return m_ALM_SettingError;}
            set { m_ALM_SettingError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionCommandT3Alarm 
        {
            get { return m_ALM_VisionCommandT3Alarm; }
            set { m_ALM_VisionCommandT3Alarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionDisconnectAlarm
        {
            get { return m_ALM_VisionDisconnectAlarm; }
            set { m_ALM_VisionDisconnectAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionTrigger1Alarm
        {
            get { return m_ALM_VisionTrigger1Alarm; }
            set { m_ALM_VisionTrigger1Alarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionTrigger2Alarm
        {
            get { return m_ALM_VisionTrigger2Alarm; }
            set { m_ALM_VisionTrigger2Alarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionTrigger3Alarm
        {
            get { return m_ALM_VisionTrigger3Alarm; }
            set { m_ALM_VisionTrigger3Alarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionTrigger4Alarm
        {
            get { return m_ALM_VisionTrigger4Alarm; }
            set { m_ALM_VisionTrigger4Alarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionTriggerAllAlarm
        {
            get { return m_ALM_VisionTriggerAllAlarm; }
            set { m_ALM_VisionTriggerAllAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionDrivingModeAlarm
        {
            get { return m_ALM_VisionDrivingModeAlarm; }
            set { m_ALM_VisionDrivingModeAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionSettingModeAlarm
        {
            get { return m_ALM_VisionSettingModeAlarm; }
            set { m_ALM_VisionSettingModeAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionResetAlarm
        {
            get { return m_ALM_VisionResetAlarm; }
            set { m_ALM_VisionResetAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionRebootingAlarm
        {
            get { return m_ALM_VisionRebootingAlarm; }
            set { m_ALM_VisionRebootingAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionSettingSaveAlarm
        {
            get { return m_ALM_VisionSettingSaveAlarm;}
            set { m_ALM_VisionSettingSaveAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionErrorClearAlarm
        {
            get { return m_ALM_VisionErrorClearAlarm;}
            set { m_ALM_VisionErrorClearAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionSwitchDisplayAlarm
        {
            get { return m_ALM_VisionSwitchDisplayAlarm;}
            set { m_ALM_VisionSwitchDisplayAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionTriggerResetAlarm
        {
            get { return m_ALM_VisionTriggerResetAlarm; }
            set { m_ALM_VisionTriggerResetAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionReadModeAlarm
        {
            get { return m_ALM_VisionReadModeAlarm; }
            set { m_ALM_VisionReadModeAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionInspectionSettingAlarm
        {
            get { return m_ALM_VisionInspectionSettingAlarm;}
            set { m_ALM_VisionInspectionSettingAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionInspectionReadAlarm
        {
            get { return m_ALM_VisionInspectionReadAlarm;  }
            set { m_ALM_VisionInspectionReadAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionShutterSpeedAlarm
        {
            get { return m_ALM_VisionShutterSpeedAlarm; }
            set { m_ALM_VisionShutterSpeedAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionCameraSensitivityAlarm
        {
            get { return m_ALM_VisionCameraSensitivityAlarm;}
            set { m_ALM_VisionCameraSensitivityAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionTriggerDelayAlarm
        {
            get { return m_ALM_VisionTriggerDelayAlarm;}
            set { m_ALM_VisionTriggerDelayAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionBasicScreenAlarm
        {
            get { return m_ALM_VisionBasicScreenAlarm;}
            set { m_ALM_VisionBasicScreenAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionExecutionConditionAlarm
        {
            get { return m_ALM_VisionExecutionConditionAlarm;}
            set { m_ALM_VisionExecutionConditionAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionChangeTriggerInterlockAlarm
        {
            get { return m_ALM_VisionChangeTriggerInterlockAlarm; }
            set { m_ALM_VisionChangeTriggerInterlockAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VisionFindAlarm
        {
            get { return m_ALM_VisionFindAlarm; }
            set { m_ALM_VisionFindAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double AlignDiffX { get => m_AlignDiffX; set => m_AlignDiffX = value; }
        [XmlIgnore(), Browsable(false)]
        public double AlignDiffY { get => m_AlignDiffY; set => m_AlignDiffY = value; }
        [XmlIgnore(), Browsable(false)]
        public double AlignDiffT { get => m_AlignDiffT; set => m_AlignDiffT = value; }
        [XmlIgnore(), Browsable(false)]
        public bool OnlySensorOffsetFind { get => m_OnlySensorOffsetFind; set => m_OnlySensorOffsetFind = value; }
        #endregion

        #region Constructor
        public DevAutoTeaching()
        {
            this.MyName = "DevAutoTeaching";
        }
        #endregion

        #region Override
        public override bool Initialize(string name = "", bool read_xml = true, bool heavy_alarm = true)
        {
            // 신규 Device 생성 시, _Device.Initialize() 내용 복사 후 붙여넣어서 사용하시오
            this.ParentName = name;
            if (read_xml) ReadXml();
            if (this.IsValid == false) return true;

            //////////////////////////////////////////////////////////////////////////////
            #region 1. 이미 초기화 완료된 상태인지 확인
            if (Initialized)
            {
                if (false)
                {
                    // 초기화된 상태에서도 변경이 가능한 항목을 추가
                }
                return true;
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 2. 필수 I/O 할당 여부 확인
            bool ok = true;
            //ok &= new object() != null;
            //ok &= m_SubDevice.Initiated;

            if (!ok)
            {
                ExceptionLog.WriteLog(string.Format("Initialize Fail : Indispensable I/O is not assigned({0})", name));
                return false;
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 3. Alarm Item 생성
            //AlarmExample = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            m_ALM_NotDefine = AlarmListProvider.Instance.NewAlarm(AlarmCode.ParameterControlError, true, MyName, ParentName, "Not Define Alarm");
            m_ALM_SettingError = AlarmListProvider.Instance.NewAlarm(AlarmCode.ParameterControlError, true, MyName, ParentName, "Parameter Setting Alarm");
            m_ALM_VisionCommandT3Alarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Command Timeout Alarm");
            m_ALM_VisionDisconnectAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Socket Disconnect Alarm");
            m_ALM_VisionTrigger1Alarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Trigger1 CMD Alarm");
            m_ALM_VisionTrigger2Alarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Trigger2 CMD Alarm");
            m_ALM_VisionTrigger3Alarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Trigger3 CMD Alarm");
            m_ALM_VisionTrigger4Alarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Trigger4 CMD Alarm");
            m_ALM_VisionTriggerAllAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Trigger All CMD Alarm");
            m_ALM_VisionDrivingModeAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Driving Mode CMD Alarm");
            m_ALM_VisionSettingModeAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Setting Mode CMD Alarm");
            m_ALM_VisionResetAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Reset CMD Alarm");
            m_ALM_VisionRebootingAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Rebooting CMD Alarm");
            m_ALM_VisionSettingSaveAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Setting Save CMD Alarm");
            m_ALM_VisionErrorClearAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Error Clear CMD Alarm");
            m_ALM_VisionSwitchDisplayAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Switch Display CMD Alarm");
            m_ALM_VisionTriggerResetAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Trigger Reset CMD Alarm");
            m_ALM_VisionReadModeAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Read Mode CMD Alarm");
            m_ALM_VisionInspectionSettingAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Inspection Setting CMD Alarm");
            m_ALM_VisionInspectionReadAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Inspection Read CMD Alarm");
            m_ALM_VisionShutterSpeedAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Shutter Speed CMD Alarm");
            m_ALM_VisionCameraSensitivityAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Camera Sensitivity CMD Alarm");
            m_ALM_VisionTriggerDelayAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Trigger Delay CMD Alarm");
            m_ALM_VisionBasicScreenAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Basic Screen CMD Alarm");
            m_ALM_VisionExecutionConditionAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Execution Condition CMD Alarm");
            m_ALM_VisionChangeTriggerInterlockAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Change Trigger CMD Alarm");
            m_ALM_VisionFindAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Vision Find Alarm");
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            m_VisionClient = VisionClient.Instance;
            if (ok) ok &= m_VisionClient.Initialize(m_VisionPortNumber, m_VisionIpAddress);

            m_HeightSensorClient = EEIPClient.Instance;
            if (ok) ok &= m_HeightSensorClient.Initialize(m_HeightSensorIpAddress, m_HeightSensorPortNumber);

            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            if (ok)
            {
                m_SeqMonitor = new SeqMonitor(this);
                m_SeqRemoteControl = new SeqRemoteControl(this);
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 6. Initialize 마무으리
            Initialized = true;
            Initialized &= ok;
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            return Initialized;
        }
        public void SetEqpPio(DevEqPIO eq_pio)
        {
            m_EqPio = eq_pio;
            m_SeqMonitor.SetEqpPio(eq_pio);
        }
        public override void SeqAbort()
        {
            if (SetupManager.Instance.SetupVision.AutoTeachingUse == Use.NoUse) return;
            if (!Initialized) return;

            m_Start = false;
            m_Stop = false;
            //m_SeqMonitor.SeqAbort();
            m_SeqRemoteControl.SeqAbort();
        }
        #endregion

        #region Methods - public
        public void AutoTeachingStart()
        {
            m_Start = true;
        }
        public void AutoTeachingFinished()
        {
            m_Stop = true;
        }
        public bool IsReady()
        {
            if (SetupManager.Instance.SetupVision.AutoTeachingUse == Use.NoUse) return false;
            if (!Initialized) return false;

            if (SetupManager.Instance.SetupVision.AutoTeachingUse != Use.Use) return false;
            m_Ready = m_VisionClient != null ? m_VisionClient.IsConnected() : false;
            m_Ready &= m_EqPio.PioComm.IsValid ? m_EqPio.PioComm.IsOpen() : false;
            m_Ready &= m_EqPio.PioComm.IsValid  ? m_EqPio.PioComm.IsGo() : false;
            return m_Ready;
        }
        public bool IsSocketConnected()
        {
            if (SetupManager.Instance.SetupVision.AutoTeachingUse == Use.NoUse) return false;
            if (!Initialized) return false;
            if (m_VisionClient == null) return false;

            return m_VisionClient.IsConnected();
        }
        public bool IsPioConnected()
        {
            if (SetupManager.Instance.SetupVision.AutoTeachingUse == Use.NoUse) return false;
            if (!Initialized) return false;
            if (!m_EqPio.PioComm.IsValid) return false;

            return m_EqPio.PioComm.IsOpen();
        }
        public bool IsPioGo()
        {
            if (SetupManager.Instance.SetupVision.AutoTeachingUse == Use.NoUse) return false;
            if (!Initialized) return false;
            if (!m_EqPio.PioComm.IsValid) return false;

            return m_EqPio.PioComm.IsGo();
        }
        public bool IsReflectiveSensorOn()
        {
            if (SetupManager.Instance.SetupVision.AutoTeachingUse == Use.NoUse) return false;
            if (!Initialized) return false;

            bool rv = m_EqPio.IsValid ? m_EqPio.GetReflectiveSensor() : false; 
            return rv;
        }
        public bool IsEEIPSocketConnected()
        {
            if (SetupManager.Instance.SetupVision.AutoTeachingUse == Use.NoUse) return false;
            if (!Initialized) return false;

            bool rv = m_HeightSensorClient.IsConnected();
            return rv;
        }
        public int GetHeight()
        {
            if (SetupManager.Instance.SetupVision.AutoTeachingUse == Use.NoUse) return 0;
            if (!Initialized) return 0;
            if (m_HeightSensorClient.IsConnected() == false) return 0;

            int rv = m_HeightSensorClient.HeightSensorValue(m_HeightSensorClient.nuObject.getInstance((int)EEIPAttribute.CurDistance));
            return rv;
        }
        public void CalculateXYT(XyPosition designP1, XyPosition designP2, XyPosition resultP1, XyPosition resultP2, ref double dx, ref double dy, ref double dt, ThetaDirection td = ThetaDirection.CenterHorizontal)
        {
            if (SetupManager.Instance.SetupVision.AutoTeachingUse == Use.NoUse) return;
            if (!Initialized) return;

            XyPosition designCenter = new XyPosition();
            XyPosition resultCenter = new XyPosition();

            designCenter.X = (designP1.X + designP2.X) / 2.0;
            designCenter.Y = (designP1.Y + designP2.Y) / 2.0;
            resultCenter.X = (resultP1.X + resultP2.X) / 2.0;
            resultCenter.Y = (resultP1.Y + resultP2.Y) / 2.0;

            dx = Math.Round(resultCenter.X - designCenter.X, 2);
            dy = Math.Round(resultCenter.Y - designCenter.Y, 2);

            switch (td)
            {
                case ThetaDirection.CenterVertical:
                    {
                        double designT = Math.Atan((designP1.X - designP2.X) / (designP1.Y - designP2.Y));
                        double resultT = Math.Atan((resultP1.X - resultP2.X) / (resultP1.Y - resultP2.Y));
                        dt = Math.Round(resultT - designT, 2);
                        dt *= -1;
                    }
                    break;
                case ThetaDirection.CenterHorizontal:
                    {
                        double designT = Math.Atan((designP1.Y - designP2.Y) / (designP1.X - designP2.X));
                        double resultT = Math.Atan((resultP1.Y - resultP2.Y) / (resultP1.X - resultP2.X));
                        dt = Math.Round(resultT - designT, 2);
                    }
                    break;
            }

            if (dt == double.NaN) dt = 0.0f;
        }
        public int VisionFind(enVisionDevice device)
        {
            if (SetupManager.Instance.SetupVision.AutoTeachingUse == Use.NoUse) return 0;
            if (!Initialized) return m_ALM_NotDefine.ID;
            if (m_VisionClient == null) return ALM_VisionDisconnectAlarm.ID;

            int rv = -1;
            rv = m_SeqRemoteControl.Do(device);
            return rv;
        }
        #endregion

        #region Sequence
        private class SeqMonitor : XSeqFunc
        {
            #region Field
            XTimer m_Timer1 = new XTimer("SocketConnection");

            private DevAutoTeaching m_Device = null;
            private VisionClient m_VisionClient = null;
            private EEIPClient m_HeightSensorClient = null;
            private DevEqPIO m_EqpPio = null;
            #endregion

            #region Constructor
            public SeqMonitor(DevAutoTeaching device)
            {
                this.SeqName = $"SeqMonitor{device.MyName}";
                m_Device = device;
                m_VisionClient = m_Device.m_VisionClient;
                m_HeightSensorClient = m_Device.m_HeightSensorClient;
                if (SetupManager.Instance.SetupVision.AutoTeachingUse == Use.Use)
                {
                    TaskDeviceControl.Instance.RegSeq(this);
                }
            }
            public override void SeqAbort()
            {
                if (AlarmId > 0)
                {
                    EqpAlarm.Reset(this.AlarmId);
                    AlarmId = 0;
                }
                this.InitSeq();
            }
            public void SetEqpPio(DevEqPIO eq_pio)
            {
                m_EqpPio = eq_pio;
            }
            #endregion

            #region Override
            public override int Do()
            {
                
                if (!m_Device.Initialized) return -1;

                int seqNo = SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupVision.AutoTeachingUse == Use.Use)
                            {
                                bool start = false;
                                start |= m_VisionClient.IsConnected() == false;
                                start |= m_EqpPio.PioComm.IsOpen() == false;
                                start |= m_EqpPio.PioComm.IsGo() == false;
                                start |= m_HeightSensorClient.IsConnected() == false;
                                start &= m_Device.m_Start;
                                bool stop = false;
                                stop |= m_VisionClient.IsConnected();
                                stop |= m_EqpPio.PioComm.IsOpen();
                                stop |= m_EqpPio.PioComm.IsGo();
                                stop |= m_HeightSensorClient.IsConnected();
                                stop &= m_Device.m_Stop;
                                if (start)
                                {
                                    seqNo = 100;
                                }
                                else if (stop)
                                {
                                    seqNo = 200;
                                }
                                else
                                {
                                    m_Device.m_Start = false;
                                    m_Device.m_Stop = false;
                                }
                            }
                        }
                        break;

                    case 100:
                        {
                            if (m_VisionClient.IsConnected() == false) seqNo = 110;
                            else if (m_EqpPio.PioComm.IsOpen() == false || m_EqpPio.PioComm.IsGo() == false) seqNo = 130;
                            else if (m_HeightSensorClient.IsConnected() == false)
                            {
                                m_HeightSensorClient.UnRegisterSession();
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 160;
                            }
                            else
                            {
                                m_Device.m_Start = false;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 110:
                        {
                            m_VisionClient.Connect();
                            string _msg = string.Format("{0} Vision Socket Connect", m_Device.MyName);
                            SequenceDeviceLog.WriteLog(_msg);
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 120;
                        }
                        break;

                    case 120:
                        {
                            if (m_VisionClient.IsConnected())
                            {
                                string _msg = string.Format("{0} Vision Socket Connected OK", m_Device.MyName);
                                SequenceDeviceLog.WriteLog(_msg);
                                seqNo = 100;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 45 * 1000)
                            {
                                this.AlarmId = m_Device.ALM_VisionDisconnectAlarm.ID;
                                EqpAlarm.Set(this.AlarmId);

                                string _msg = string.Format("{0} Vision Socket Connection Alarm", m_Device.MyName);
                                SequenceDeviceLog.WriteLog(_msg);
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 130:
                        {
                            int id = m_Device.ReflectivePIOID;
                            int ch = m_Device.ReflectivePIOCH;
                            int rv1 = m_EqpPio.SetChannelId(id, ch, true);
                            if (rv1 == 0)
                            {
                                SequenceDeviceLog.WriteLog(string.Format("AUTOTEACH_PIO [{0}, {1}] Set Channel OK", id, ch));
                                seqNo = 100;
                            }
                            else if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceDeviceLog.WriteLog(string.Format("AUTOTEACH_PIO [{0}, {1}] Set Channel Alarm - {2}", id, ch, EqpAlarm.GetAlarmMsg(AlarmId)));

                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 0;
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 160:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 500) break;
                            m_HeightSensorClient.RegisterSession();
                            StartTicks = XFunc.GetTickCount();
                            string _msg = string.Format("{0} HeightSensor connect", m_Device.MyName);
                            SequenceDeviceLog.WriteLog(_msg);
                            seqNo = 180;
                        }
                        break;

                    //case 170:
                    //    {
                    //        if (XFunc.GetTickCount() - StartTicks < 500) break;
                    //        m_HeightSensorClient.UnRegisterSession();
                    //        StartTicks = XFunc.GetTickCount();
                    //        seqNo = 180;
                    //    }
                    //    break;

                    case 180:
                        {
                            if (m_HeightSensorClient.IsConnected())
                            {
                                m_HeightSensorClient.O_T_InstanceID = 0xfe;              //Instance ID of the Output Assembly
                                m_HeightSensorClient.O_T_Length = 0;
                                m_HeightSensorClient.O_T_RealTimeFormat = EEIPRealTimeFormat.Header32Bit;   //Header Format
                                m_HeightSensorClient.O_T_OwnerRedundant = false;
                                m_HeightSensorClient.O_T_Priority = EEIPPriority.Low;
                                m_HeightSensorClient.O_T_VariableLength = false;
                                m_HeightSensorClient.O_T_ConnectionType = EEIPConnectionType.Point_to_Point;
                                m_HeightSensorClient.RequestedPacketRate_O_T = 500000;    //RPI in  500ms is the Standard value


                                //Parameters from Target -> Originator
                                m_HeightSensorClient.T_O_InstanceID = 0x64;
                                m_HeightSensorClient.T_O_Length = 128;
                                m_HeightSensorClient.T_O_RealTimeFormat = EEIPRealTimeFormat.Modeless;
                                m_HeightSensorClient.T_O_OwnerRedundant = false;
                                m_HeightSensorClient.T_O_Priority = EEIPPriority.Scheduled;
                                m_HeightSensorClient.T_O_VariableLength = false;
                                m_HeightSensorClient.T_O_ConnectionType = EEIPConnectionType.Point_to_Point;
                                m_HeightSensorClient.RequestedPacketRate_T_O = 500000;    //RPI in  500ms is the Standard value

                                //Forward open initiates the Implicit Messaging
                                m_HeightSensorClient.ForwardOpen();
                                StartTicks = XFunc.GetTickCount();
                                string _msg = string.Format("{0} HeightSensor connect OK", m_Device.MyName);
                                SequenceDeviceLog.WriteLog(_msg);
                                seqNo = 100;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_HeightSensorClient.UnRegisterSession();
                                StartTicks = XFunc.GetTickCount();
                                string _msg = string.Format("{0} HeightSensor connect NG", m_Device.MyName);
                                SequenceDeviceLog.WriteLog(_msg);

                                seqNo = 160;
                            }
                        }
                        break;

                    case 200:
                        {
                            if (m_EqpPio.PioComm.IsGo()) seqNo = 230;
                            else if (m_VisionClient.IsConnected()) seqNo = 210;
                            else if (m_HeightSensorClient.IsConnected()) seqNo = 260;
                            else
                            {
                                m_Device.m_Stop = false;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 210:
                        {
                            m_VisionClient.Disconnect();
                            string _msg = string.Format("{0} Vision Socket  Disconnect", m_Device.MyName);
                            SequenceDeviceLog.WriteLog(_msg);
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 220;
                        }
                        break;

                    case 220:
                        {
                            if (m_VisionClient.IsConnected() == false)
                            {
                                string _msg = string.Format("{0} Vision Socket Disonnected OK", m_Device.MyName);
                                SequenceDeviceLog.WriteLog(_msg);
                                seqNo = 200;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 45 * 1000)
                            {
                                this.AlarmId = m_Device.ALM_VisionDisconnectAlarm.ID;
                                EqpAlarm.Set(this.AlarmId);

                                string _msg = string.Format("{0} Vision Socket Connection Alarm", m_Device.MyName);
                                SequenceDeviceLog.WriteLog(_msg);
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 230:
                        {
                            int id = 0;
                            int ch = 0;
                            int rv1 = m_EqpPio.SetChannelId(id, ch);
                            if (rv1 == 0)
                            {
                                SequenceDeviceLog.WriteLog(string.Format("AUTOTEACH_PIO [{0}, {1}] Set Channel OK", id, ch));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 200;
                            }
                            else if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceDeviceLog.WriteLog(string.Format("AUTOTEACH [{0}, {1}] Set Channel Alarm - {2}", id, ch, EqpAlarm.GetAlarmMsg(AlarmId)));

                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 0;
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 260:
                        {
                            m_HeightSensorClient.UnRegisterSession();
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 270;
                        }
                        break;

                    case 270:
                        {
                            if (m_HeightSensorClient.IsConnected() == false)
                            {
                                seqNo = 200;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_HeightSensorClient.UnRegisterSession();
                                StartTicks = XFunc.GetTickCount();
                            }
                        }
                        break;

                    case 1000:
                        {
                            if (IsPushedSwitch.IsAlarmReset)
                            {
                                IsPushedSwitch.m_AlarmRstPushed = false;

                                string _msg = string.Format("{0} Alarm Reset", m_Device.MyName);
                                SequenceDeviceLog.WriteLog(_msg);
                                EqpAlarm.Reset(this.AlarmId);
                                seqNo = 0;
                            }
                        }
                        break;
                }

                SeqNo = seqNo;
                return -1;
            }
            #endregion
        }

        private class SeqRemoteControl : XSeqFunc
        {
            #region Fields
            public static readonly string FuncName = "[VisionRemoteControl]";
            private DevAutoTeaching m_AutoTeaching = null;
            #endregion

            #region Properties
            #endregion

            #region Constructor
            public SeqRemoteControl(DevAutoTeaching device)
            {
                this.SeqName = $"SeqRemoteControl{device.MyName}";
                m_AutoTeaching = device;
            }
            public override void SeqAbort()
            {
                this.InitSeq();
                if (AlarmId > 0)
                {
                    EqpAlarm.Reset(AlarmId);
                    AlarmId = 0;
                }
            }
            #endregion

            #region Methods
            /// <summary>
            /// para1 : enVisionDevice
            /// </summary>
            /// <param name="para1"></param>
            /// <returns></returns>
            public override int Do(object para1)
            {
                int rv = -1;
                int seqNo = SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupVision.AutoTeachingUse == Use.Use)
                            {
                                if (m_AutoTeaching.IsReady())
                                {
                                    seqNo = 10;
                                }
                                else
                                {
                                    AlarmId = m_AutoTeaching.ALM_VisionDisconnectAlarm.ID;
                                    SequenceDeviceLog.WriteLog(FuncName, string.Format("Auto Teaching Not Ready Status", EqpAlarm.GetAlarmMsg(AlarmId)));
                                    seqNo = 1000;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            m_AutoTeaching.VcmDrivingMode.SendPrimary();
                            StartTicks = XFunc.GetTickCount();
                            SequenceDeviceLog.WriteLog(FuncName, string.Format("VcmDrivingMode.SendPrimary"));
                            seqNo = 20;
                        }
                        break;

                    case 20:
                        {
                            enVisionResult rv1 = m_AutoTeaching.VcmDrivingMode.IsSecondaryRcvd();
                            if (rv1 == enVisionResult.OK)
                            {
                                SequenceDeviceLog.WriteLog(FuncName, string.Format("VcmDrivingMode.IsSecondaryRcvd"));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                            else if (rv1 > enVisionResult.OK)
                            {
                                SequenceDeviceLog.WriteLog(FuncName, string.Format("VcmDrivingMode Alarm - {0}", rv1.ToString()));
                                AlarmId = m_AutoTeaching.ALM_VisionDrivingModeAlarm.ID;
                                seqNo = 1000;
                            }
                            else if (m_AutoTeaching.m_VisionClient.IsConnected() == false)
                            {
                                SequenceDeviceLog.WriteLog(FuncName, string.Format("VcmDrivingMode Alarm - {0}", rv1.ToString()));
                                AlarmId = m_AutoTeaching.ALM_VisionDisconnectAlarm.ID;
                                seqNo = 1000;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 20000)
                            {
                                seqNo = 0;
                            }
                        }
                        break;

                    case 30:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;
                            m_AutoTeaching.VcmTrigger1.SendPrimary();
                            StartTicks = XFunc.GetTickCount();
                            SequenceDeviceLog.WriteLog(FuncName, string.Format("VcmTrigger1.SendPrimary"));
                            seqNo = 40;
                        }
                        break;

                    case 40:
                        {
                            XyPosition Camera1_LEFT = new XyPosition(0.0f, 0.0f);  // Port
                            XyPosition Camera1_RIGHT = new XyPosition(0.0f, 0.0f);
                            XyPosition Camera2_Left_Up = new XyPosition(0.0f, 0.0f);  // Buffer
                            XyPosition Camera2_Left_Down = new XyPosition(0.0f, 0.0f);
                            XyPosition Camera2_Right_Up = new XyPosition(0.0f, 0.0f);  // Buffer
                            XyPosition Camera2_Right_Down = new XyPosition(0.0f, 0.0f);

                            enVisionResult rv1 = m_AutoTeaching.VcmTrigger1.IsSecondaryRcvd(ref Camera1_LEFT, ref Camera1_RIGHT, ref Camera2_Left_Down, ref Camera2_Left_Up, ref Camera2_Right_Down, ref Camera2_Right_Up);
                            if (rv1 == enVisionResult.OK)
                            {
                                SequenceDeviceLog.WriteLog(FuncName, string.Format("VcmTrigger1.IsSecondaryRcvd"));
                                //Camera2_LEFT
                                //Camera2_RIGHT
                                //(+)사이 거리 : 13mm
                                double dx = 0.0, dy = 0.0, dt = 0.0;
                                XyPosition designP1 = new XyPosition(0.0, 0.0); //LEFT
                                XyPosition designP2 = new XyPosition(0.0, 0.0); //RIGHT
                                XyPosition resultP1 = new XyPosition(0.0, 0.0);
                                XyPosition resultP2 = new XyPosition(0.0, 0.0);
                                XyPosition findP1 = new XyPosition(0.0, 0.0);
                                XyPosition findP2 = new XyPosition(0.0, 0.0);

                                if ((enVisionDevice)para1 == enVisionDevice.EQPCamera)
                                {
                                    findP1 = Camera1_LEFT;
                                    findP2 = Camera1_RIGHT;
                                    designP1 = new XyPosition(-5, 0.0); //LEFT
                                    designP2 = new XyPosition(5, 0.0);  //RIGHT
                                    resultP1 = new XyPosition(designP1.X + Camera1_LEFT.X, designP1.Y + Camera1_LEFT.Y);
                                    resultP2 = new XyPosition(designP2.X + Camera1_RIGHT.X, designP2.Y + Camera1_RIGHT.Y);
                                    m_AutoTeaching.CalculateXYT(designP1, designP2, resultP1, resultP2, ref dx, ref dy, ref dt, ThetaDirection.CenterHorizontal);
                                }
                                else if ((enVisionDevice)para1 == enVisionDevice.OHBCamera_Left)//OHB
                                {
                                    findP1 = Camera2_Left_Up;
                                    findP2 = Camera2_Left_Down;
                                    designP1 = new XyPosition(0.0, 5); //LEFT
                                    designP2 = new XyPosition(0.0, -5);  //RIGHT
                                    resultP1 = new XyPosition(designP1.X + Camera2_Left_Up.X, designP1.Y + Camera2_Left_Up.Y);
                                    resultP2 = new XyPosition(designP2.X + Camera2_Left_Down.X, designP2.Y + Camera2_Left_Down.Y);
                                    m_AutoTeaching.CalculateXYT(designP1, designP2, resultP1, resultP2, ref dx, ref dy, ref dt, ThetaDirection.CenterVertical);
                                }
                                else //Right OHB
                                {
                                    findP1 = Camera2_Right_Up;
                                    findP2 = Camera2_Right_Down;
                                    designP1 = new XyPosition(0.0, 5); //LEFT
                                    designP2 = new XyPosition(0.0, -5);  //RIGHT
                                    resultP1 = new XyPosition(designP1.X + Camera2_Right_Up.X, designP1.Y + Camera2_Right_Up.Y);
                                    resultP2 = new XyPosition(designP2.X + Camera2_Right_Down.X, designP2.Y + Camera2_Right_Down.Y);
                                    m_AutoTeaching.CalculateXYT(designP1, designP2, resultP1, resultP2, ref dx, ref dy, ref dt, ThetaDirection.CenterVertical);
                                }

                                m_AutoTeaching.AlignDiffX = dx;
                                m_AutoTeaching.AlignDiffY = dy;
                                m_AutoTeaching.AlignDiffT = dt;
                                EventHandlerManager.Instance.InvokeAutoTeachingVisionResult(dx, dy, dt);

                                string msg = string.Format("VcmTrigger1 OK : UP.X={0}, UP.Y={1}, DOWN.X={2}, DOWN.Y={3}, Result=(dx={4},dy={5},dt={6})",
                                    findP1.X, findP1.Y, findP2.X, findP2.Y, dx, dy, dt);
                                SequenceDeviceLog.WriteLog(msg);
                                
                                if ((findP1.X == 0.0f && findP1.Y == 0.0f) || (findP2.X == 0.0f && findP2.Y == 0.0f))
                                {
                                    AlarmId = m_AutoTeaching.ALM_VisionFindAlarm.ID;
                                    seqNo = 1000;
                                }
                                else
                                {
                                    rv = 0;
                                    seqNo = 0;
                                }
                            }
                            else if (rv1 > enVisionResult.OK)
                            {
                                SequenceDeviceLog.WriteLog(FuncName, string.Format("VcmTrigger1 Alarm - {0}", rv1.ToString()));
                                AlarmId = m_AutoTeaching.ALM_VisionTrigger1Alarm.ID;
                                seqNo = 1000;
                            }
                            else if (m_AutoTeaching.m_VisionClient.IsConnected() == false)
                            {
                                SequenceDeviceLog.WriteLog(FuncName, string.Format("VcmTrigger1 Alarm - {0}", rv1.ToString()));
                                AlarmId = m_AutoTeaching.ALM_VisionDisconnectAlarm.ID;
                                seqNo = 1000;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 20000)
                            {
                                seqNo = 0;
                            }
                        }
                        break;

                    case 1000:
                        {
                            rv = AlarmId;
                            seqNo = 0;
                        }
                        break;
                }

                SeqNo = seqNo;
                return rv;
            }
            #endregion

        }
        #endregion

        #region [Xml Read/Write]
        public override bool ReadXml()
        {
            string fileName = "";
            bool fileCheck = CheckPath(ref fileName);
            if (fileCheck == false) return false;

            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists == false)
                {
                    WriteXml();
                }

                var helperXml = new XmlHelper<DevAutoTeaching>();
                DevAutoTeaching dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;

                    this.VisionIpAddress = dev.VisionIpAddress;
                    this.VisionPortNumber = dev.VisionPortNumber;
                    this.HeightSensorIpAddress = dev.HeightSensorIpAddress;
                    this.HeightSensorPortNumber = dev.HeightSensorPortNumber;
                    this.ReflectivePIOID = dev.ReflectivePIOID;
                    this.ReflectivePIOCH = dev.ReflectivePIOCH;

                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;
                }
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
            }

            return true;
        }

        public override void WriteXml()
        {
            string fileName = "";
            bool fileCheck = CheckPath(ref fileName);
            if (fileCheck == false) return;

            try
            {
                var helperXml = new XmlHelper<DevAutoTeaching>();
                helperXml.Save(fileName, this);
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
            }
        }

        public bool CheckPath(ref string fileName)
        {
            bool ok = false;
            string filePath = AppConfig.Instance.XmlDevicesPath;

            if (Directory.Exists(filePath) == false)
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.Description = "Configuration folder select";
                dlg.SelectedPath = AppConfig.GetSolutionPath();
                dlg.ShowNewFolderButton = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    filePath = dlg.SelectedPath;
                    if (MessageBox.Show("do you want to save seleted folder !", "SAVE", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        AppConfig.Instance.ConfigPath.SelectedFolder = filePath;
                        AppConfig.Instance.WriteXml();
                    }
                    fileName = string.Format("{0}\\{1}.xml", filePath, GetDefaultFileName());
                    ok = true;
                }
                else
                {
                    ok = false;
                }
            }
            else
            {
                fileName = string.Format("{0}\\{1}.xml", filePath, GetDefaultFileName());
                ok = true;
            }
            return ok;
        }

        public string GetDefaultFileName()
        {
            if (this.MyName == "") this.MyName = DevName;
            return this.ToString();
        }
        #endregion

    }
}
