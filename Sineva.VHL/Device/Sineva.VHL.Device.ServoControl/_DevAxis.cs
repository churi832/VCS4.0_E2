using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel;
using Sineva.VHL.Data.Alarm;

namespace Sineva.VHL.Device.ServoControl
{
    #region XmlInclude
    [XmlInclude(typeof(AxisTag))]
    [XmlInclude(typeof(ServoUnitTag))]
    #endregion
    
    [Serializable]
    public class _DevAxis : _Device
    {
        private const string DevName = "DevAxis";
        #region Fields
        private double m_OldVelocity = 0.0f; //Filter 처리하자! 속도값이 너무 들쭉날쭉
        private double m_FilterRatio = 0.25;
        private bool m_Homing = false;

        // Axis
        private AxisTag m_AxisTag = null;
        private ServoUnitTag m_ServoUnitTag = null;
        // Sequence
        private SeqStop m_SeqStop = null;
        private SeqAlarmClear m_SeqAlarmClear = null;
        private SeqServoOff m_SeqServoOff = null;
        private SeqServoOn m_SeqServoOn = null;
        private SeqOrigin m_SeqOrigin = null;
        private SeqHome m_SeqHome = null;
        private SeqMove m_SeqMove = null;
        private SeqRelativeMove m_SeqRelativeMove = null;
        private SeqZeroSet m_SeqZeroSet = null;
        // Axis Alarm Define
        /// <summary>
        //Alarm = 2,
        //Timeover = 3,
        //CmdError = 4,
        //NotReadyError = 5,
        //VelSettingError = 6,
        //PosSettingError = 7,
        //IntrError = 8,
        //StopTimeout = 9,
        //AlarmClearTimeout = 10,
        //ServoOnTimeout = 11,
        //ServoOffTimeout = 12,
        //HomeTimeout = 13,
        //MoveTimeout = 14,
        //SoftwareLimitPos = 15,
        //SoftwareLimitNeg = 16,
        //SoftwareLimitSpeed = 17,
        //SoftwareLimitAcc = 18,
        //SoftwareLimitDec = 19,
        //SoftwareLimitJerk = 20,
        //ZeroSetTimeout = 21,
        //SequenceMoveTimeout = 22,
        //OverrideAbnormalStop = 23,

        /// </summary>
        private AlarmData m_ALM_ServoAlarm = null;
        private AlarmData m_ALM_Timeover = null;
        private AlarmData m_ALM_CmdError = null;
        private AlarmData m_ALM_NotReadyError = null;
        private AlarmData m_ALM_VelSettingError = null;
        private AlarmData m_ALM_PosSettingError = null;
        private AlarmData m_ALM_IntrError = null;
        private AlarmData m_ALM_StopTimeout = null;
        private AlarmData m_ALM_AlarmClearTimeout = null;
        private AlarmData m_ALM_ServoOnTimeout = null;
        private AlarmData m_ALM_ServoOffTimeout = null;
        private AlarmData m_ALM_HomeTimeout = null;
        private AlarmData m_ALM_MoveTimeout = null;
        private AlarmData m_ALM_SoftwareLimitPos = null;
        private AlarmData m_ALM_SoftwareLimitNeg = null;
        private AlarmData m_ALM_SoftwareLimitSpeed = null;
        private AlarmData m_ALM_SoftwareLimitAcc = null;
        private AlarmData m_ALM_SoftwareLimitDec = null;
        private AlarmData m_ALM_SoftwareLimitJerk = null;
        private AlarmData m_ALM_ZeroSetTimeout = null;
        private AlarmData m_ALM_SequenceMoveTimeout = null;
        private AlarmData m_ALM_OverrideAbnormalStop = null;
        private AlarmData m_ALM_AutoModeServoNotReadyError = null;
        private AlarmData m_ALM_InRangeError = null;
		private AlarmData m_ALM_HomeNotDetectError = null;
		
        //Servo Alarm Segment
        private AlarmData m_ALM_IPMHardWareOvercurrent = null; //HW 과전류(IPM)
        private AlarmData m_ALM_Overcurrent = null; //과전류
        private AlarmData m_ALM_HardWareOvercurrentLimit = null; //HW 과전류(한계초과)
        private AlarmData m_ALM_CurrentOffsetAbnormal = null; //전류옵셋이상
        private AlarmData m_ALM_ContinuousOverload = null; //과부하
        private AlarmData m_ALM_RegenerationOverload = null; //회생 과부하
        private AlarmData m_ALM_MotorDriverTemperature1 = null; //모터 드라이버 과열1
        private AlarmData m_ALM_MotorDriverTemperature2 = null; //모터 드라이버 과열2
        private AlarmData m_ALM_MotorEncoderTemperature = null; //모터 엔코더 과열
        private AlarmData m_ALM_MotorCableOpenCircuit = null; //모터 케이블 단선
        private AlarmData m_ALM_MotorEncoderCommunication = null; //엔코더 통신 에러
        private AlarmData m_ALM_MotorEncoderCableOpenCircuit = null; //모터 엔코더 케이블 단선
        private AlarmData m_ALM_UnderVoltage = null; //저전압
        private AlarmData m_ALM_MotorMainPowerAbnormal = null; //모터 주전원 이상

        private AlarmData m_ALM_EthercatCommunication = null; //이더켓 통신 이상
        private AlarmData m_ALM_FollowingError = null; //Following Error
        #endregion

        #region Properties
        [Category("!Setting Device (Axis)"), Description(""), DeviceSetting(true, true)]
        public AxisTag AxisTag
        {
            get { return m_AxisTag; }
            set { m_AxisTag = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public ServoUnitTag ServoUnitTag
        {
            get { return m_ServoUnitTag; }
            set { m_ServoUnitTag = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public bool Homing
        {
            get { return m_Homing; }
            set { m_Homing = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ServoAlarm
        {
            get { return m_ALM_ServoAlarm; }
            set { m_ALM_ServoAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Timeover
        {
            get { return m_ALM_Timeover; }
            set { m_ALM_Timeover = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_CmdError
        {
            get { return m_ALM_CmdError; }
            set { m_ALM_CmdError = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_NotReadyError
        {
            get { return m_ALM_NotReadyError; }
            set { m_ALM_NotReadyError = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_VelSettingError
        {
            get { return m_ALM_VelSettingError; }
            set { m_ALM_VelSettingError = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_PosSettingError
        {
            get { return m_ALM_PosSettingError; }
            set { m_ALM_PosSettingError = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_IntrError
        {
            get { return m_ALM_IntrError; }
            set { m_ALM_IntrError = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_StopTimeout
        {
            get { return m_ALM_StopTimeout; }
            set { m_ALM_StopTimeout = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_AlarmClearTimeout
        {
            get { return m_ALM_AlarmClearTimeout; }
            set { m_ALM_AlarmClearTimeout = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ServoOnTimeout
        {
            get { return m_ALM_ServoOnTimeout; }
            set { m_ALM_ServoOnTimeout = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ServoOffTimeout
        {
            get { return m_ALM_ServoOffTimeout; }
            set { m_ALM_ServoOffTimeout = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_HomeTimeout
        {
            get { return m_ALM_HomeTimeout; }
            set { m_ALM_HomeTimeout = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MoveTimeout
        {
            get { return m_ALM_MoveTimeout; }
            set { m_ALM_MoveTimeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_SoftwareLimitPos
        {
            get { return m_ALM_SoftwareLimitPos; }
            set { m_ALM_SoftwareLimitPos = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_SoftwareLimitNeg
        {
            get { return m_ALM_SoftwareLimitNeg; }
            set { m_ALM_SoftwareLimitNeg = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_SoftwareLimitSpeed
        {
            get { return m_ALM_SoftwareLimitSpeed; }
            set { m_ALM_SoftwareLimitSpeed = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_SoftwareLimitAcc
        {
            get { return m_ALM_SoftwareLimitAcc; }
            set { m_ALM_SoftwareLimitAcc = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_SoftwareLimitDec
        {
            get { return m_ALM_SoftwareLimitDec; }
            set { m_ALM_SoftwareLimitDec = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_SoftwareLimitJerk
        {
            get { return m_ALM_SoftwareLimitJerk; }
            set { m_ALM_SoftwareLimitJerk = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ZeroSetTimeout
        {
            get { return m_ALM_ZeroSetTimeout; }
            set { m_ALM_ZeroSetTimeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_SequenceMoveTimeout
        {
            get { return m_ALM_SequenceMoveTimeout; }
            set { m_ALM_SequenceMoveTimeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_OverrideAbnormalStop
        {
            get { return m_ALM_OverrideAbnormalStop; }
            set { m_ALM_OverrideAbnormalStop = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_AutoModeServoNotReadyError
        {
            get { return m_ALM_AutoModeServoNotReadyError; }
            set { m_ALM_AutoModeServoNotReadyError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_InRangeError
        {
            get { return m_ALM_InRangeError; }
            set { m_ALM_InRangeError = value; }
        }
		[XmlIgnore(), Browsable(false)]
        public AlarmData ALM_HomeNotDetectError
        {
            get { return m_ALM_HomeNotDetectError; }
            set { m_ALM_HomeNotDetectError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_IPMHardWareOvercurrent
        {
            get { return m_ALM_IPMHardWareOvercurrent; }
            set { m_ALM_IPMHardWareOvercurrent = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Overcurrent
        {
            get { return m_ALM_Overcurrent; }
            set { m_ALM_Overcurrent = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_HardWareOvercurrentLimit
        {
            get { return m_ALM_HardWareOvercurrentLimit; }
            set { m_ALM_HardWareOvercurrentLimit = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_CurrentOffsetAbnormal
        {
            get { return m_ALM_CurrentOffsetAbnormal; }
            set { m_ALM_CurrentOffsetAbnormal = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ContinuousOverload
        {
            get { return m_ALM_ContinuousOverload; }
            set { m_ALM_ContinuousOverload = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_RegenerationOverload
        {
            get { return m_ALM_RegenerationOverload; }
            set { m_ALM_RegenerationOverload = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MotorDriverTemperature1
        {
            get { return m_ALM_MotorDriverTemperature1; }
            set { m_ALM_MotorDriverTemperature1 = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MotorDriverTemperature2
        {
            get { return m_ALM_MotorDriverTemperature2; }
            set { m_ALM_MotorDriverTemperature2 = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MotorEncoderTemperature
        {
            get { return m_ALM_MotorEncoderTemperature; }
            set { m_ALM_MotorEncoderTemperature = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MotorCableOpenCircuit
        {
            get { return m_ALM_MotorCableOpenCircuit; }
            set { m_ALM_MotorCableOpenCircuit = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MotorEncoderCommunication
        {
            get { return m_ALM_MotorEncoderCommunication; }
            set { m_ALM_MotorEncoderCommunication = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MotorEncoderCableOpenCircuit
        {
            get { return m_ALM_MotorEncoderCableOpenCircuit; }
            set { m_ALM_MotorEncoderCableOpenCircuit = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_UnderVoltage
        {
            get { return m_ALM_UnderVoltage; }
            set { m_ALM_UnderVoltage = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MotorMainPowerAbnormal
        {
            get { return m_ALM_MotorMainPowerAbnormal; }
            set { m_ALM_MotorMainPowerAbnormal = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_EthercatCommunication
        {
            get { return m_ALM_EthercatCommunication; }
            set { m_ALM_EthercatCommunication = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_FollowingError
        {
            get { return m_ALM_FollowingError; }
            set { m_ALM_FollowingError = value; }
        }
        #endregion

        #region Constructor
        public _DevAxis()
        {
            if (!Initialized)
            {
                this.MyName = DevName;
            }
        }
        public _DevAxis(AxisTag tag)
        {
            if (!Initialized)
            {
                m_AxisTag = tag;
                this.MyName = m_AxisTag.AxisName;
            }
        }
        public _DevAxis(ServoUnitTag tag1, AxisTag tag2)
        {
            if (!Initialized)
            {
                m_ServoUnitTag = tag1;
                m_AxisTag = tag2;
                this.MyName = m_AxisTag.AxisName;
            }
        }
        #endregion

        #region Override
        public override bool Initialize(string name = "", bool read_xml = true, bool heavy_alarm = true)
        {
            // 신규 Device 생성 시, _Device.Initialize() 내용 복사 후 붙여넣어서 사용하시오
            if (name != "") this.ParentName = name;
            //if (read_xml) ReadXml();

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
            ok &= m_AxisTag != null;
            ok &= m_AxisTag.GetAxis() != null;
            if (!ok)
            {
                ExceptionLog.WriteLog(string.Format("Initialize Fail : Indispensable I/O is not assigned({0})", name));
                return false;
            }
            MyName = m_AxisTag.AxisName;
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 3. Alarm Item 생성
            //AlarmExample = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            //if(Condition) AlarmConditionable = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            if (ALM_ServoAlarm == null) ALM_ServoAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Motor Servo Alarm");
            if (ALM_Timeover == null) ALM_Timeover = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Timeover");
            if (ALM_CmdError == null) ALM_CmdError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Command Error");
            if (ALM_NotReadyError == null) ALM_NotReadyError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Not Ready Error");
            if (ALM_VelSettingError == null) ALM_VelSettingError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Velocity Setting Error");
            if (ALM_PosSettingError == null) ALM_PosSettingError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Position Setting Error");
            if (ALM_IntrError == null) ALM_IntrError = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, this.ToString(), MyName, "Motor Interlock Error");
            if (ALM_StopTimeout == null) ALM_StopTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Stop Timeout");
            if (ALM_AlarmClearTimeout == null) ALM_AlarmClearTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Alarm Clear Timeout");
            if (ALM_ServoOnTimeout == null) ALM_ServoOnTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Servo On Timeout");
            if (ALM_ServoOffTimeout == null) ALM_ServoOffTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Servo Off Timeout");
            if (ALM_HomeTimeout == null) ALM_HomeTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Home Timeout");
            if (ALM_MoveTimeout == null) ALM_MoveTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Motor Move Timeout");
            if (ALM_SoftwareLimitPos == null) ALM_SoftwareLimitPos = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Software Pos Limit Error");
            if (ALM_SoftwareLimitNeg == null) ALM_SoftwareLimitNeg = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Software Neg Limit Error");
            if (ALM_SoftwareLimitSpeed == null) ALM_SoftwareLimitSpeed = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Software Speed Limit Error");
            if (ALM_SoftwareLimitAcc == null) ALM_SoftwareLimitAcc = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Software Acceleration Limit Error");
            if (ALM_SoftwareLimitDec == null) ALM_SoftwareLimitDec = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Software Deceleration Limit Error");
            if (ALM_SoftwareLimitJerk == null) ALM_SoftwareLimitJerk = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Motor Software Jerk Limit Error");
            if (ALM_ZeroSetTimeout == null) ALM_ZeroSetTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, this.ToString(), MyName, "Zero Set Timeout");
            if (ALM_SequenceMoveTimeout == null) ALM_SequenceMoveTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Sequence Move Timeout");
            if (ALM_OverrideAbnormalStop == null) ALM_OverrideAbnormalStop = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Override Abnormal Stop Error");
            if (ALM_AutoModeServoNotReadyError == null) ALM_AutoModeServoNotReadyError = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Auto Mode Servo Not Ready Error");
            if (ALM_InRangeError == null) ALM_InRangeError = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Motor InRange Over Error");
            if (ALM_HomeNotDetectError == null) ALM_HomeNotDetectError = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, this.ToString(), MyName, "Home Sensor Not Detect Error");

			//Servo Alarm Segment
            if (ALM_IPMHardWareOvercurrent == null) ALM_IPMHardWareOvercurrent = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "IPM HardWare Overcurrent Alarm AL-10");
            if (ALM_Overcurrent == null) ALM_Overcurrent = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Overcurrent Alarm AL-14");
            if (ALM_HardWareOvercurrentLimit == null) ALM_HardWareOvercurrentLimit = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "HardWare Overcurrent Limit Alarm AL-16");
            if (ALM_CurrentOffsetAbnormal == null) ALM_CurrentOffsetAbnormal = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "CurrentOffset Abnormal Alarm AL-15");
            if (ALM_ContinuousOverload == null) ALM_ContinuousOverload = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Continuous Overload Alarm AL-21");
            if (ALM_RegenerationOverload == null) ALM_RegenerationOverload = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Regeneration Overload Alarm AL-23");
            if (ALM_MotorDriverTemperature1 == null) ALM_MotorDriverTemperature1 = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Motor Driver Temperature1 Alarm AL-22");
            if (ALM_MotorDriverTemperature2 == null) ALM_MotorDriverTemperature2 = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Motor Driver Temperature2 Alarm AL-25");
            if (ALM_MotorEncoderTemperature == null) ALM_MotorEncoderTemperature = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Motor Encoder Temperature Alarm AL-26");
            if (ALM_MotorCableOpenCircuit == null) ALM_MotorCableOpenCircuit = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Motor Cable Open Circuit Alarm AL-24");
            if (ALM_MotorEncoderCommunication == null) ALM_MotorEncoderCommunication = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Motor Encoder Communication Alarm AL-30");
            if (ALM_MotorEncoderCableOpenCircuit == null) ALM_MotorEncoderCableOpenCircuit = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Motor Encoder Cable Open Circuit Alarm AL-31");
            if (ALM_UnderVoltage == null) ALM_UnderVoltage = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Motor Under Voltage Alarm AL-40");
            if (ALM_MotorMainPowerAbnormal == null) ALM_MotorMainPowerAbnormal = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Motor Main Power Abnormal Alarm AL-42");
            if (ALM_EthercatCommunication == null) ALM_EthercatCommunication = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Ethercat Communication Alarm MXP AL-555");
            if (ALM_FollowingError == null) ALM_FollowingError = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, this.ToString(), MyName, "Motor Following Error Alarm MXP AL-514");
            //
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;
            m_AxisTag.GetAxis().AlarmStartId = ALM_ServoAlarm.ID;
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            m_SeqStop = new SeqStop(this);
            m_SeqAlarmClear = new SeqAlarmClear(this);
            m_SeqServoOff = new SeqServoOff(this);
            m_SeqServoOn = new SeqServoOn(this);
            m_SeqOrigin = new SeqOrigin(this);
            m_SeqHome = new SeqHome(this);
            m_SeqMove = new SeqMove(this);
            m_SeqRelativeMove = new SeqRelativeMove(this);
            m_SeqZeroSet = new SeqZeroSet(this);
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
        #endregion

        #region Methods
        public _Axis GetAxis()
        {
            if (m_AxisTag == null) return null;
            return m_AxisTag.GetAxis();
        }
        public string GetName()
        {
            if (m_AxisTag == null) return string.Empty;
            if (GetAxis() == null) return string.Empty;
            return GetAxis().AxisName;
        }
        public int GetId()
        {
            if (m_AxisTag == null) return -1;
            if (GetAxis() == null) return -1;
            return GetAxis().AxisId;
        }
        public ServoUnit GetServoUnit()
        {
            if (m_ServoUnitTag == null) return null;
            return m_ServoUnitTag.GetServoUnit();
        }
        public double GetCurPosition()
        {
            double curVal = 0.0f;
            if (m_AxisTag != null)
            {
                curVal = GetAxis().CurPos; // (GetAxis() as IAxisCommand).GetAxisCurPos();
            }
            return curVal;
        }
        public double GetCurTorque()
        {
            double curVal = 0.0f;
            if (m_AxisTag != null)
            {
                curVal = GetAxis().CurTorque; // (GetAxis() as IAxisCommand).GetAxisCurTorque();
            }
            return curVal;
        }
        public double GetCurVelocity()
        {
            double curVal = 0.0f;
            if (m_AxisTag != null)
            {
                double curVel = GetAxis().CurSpeed; // (GetAxis() as IAxisCommand).GetAxisCurSpeed();
                if (AppConfig.Instance.Simulation.MY_DEBUG)
                    curVel = GetCommandVelocity();
                curVal = (1 - m_FilterRatio) * curVel + m_FilterRatio * m_OldVelocity;
                m_OldVelocity = curVel;
            }
            return curVal;
        }
        public double GetTrajectoryVelocity()
        {
            double curVal = 0.0f;
            if (m_AxisTag != null)
            {
                curVal = (GetAxis() as MpAxis).TrajectoryTargetVelocity;
            }
            return curVal;
        }
        public double GetCommandPosition()
        {
            double curVal = 0.0f;
            if (m_AxisTag != null)
            {
                curVal = (GetAxis() as MpAxis).CommandPos;
            }
            return curVal;
        }
        public double GetCommandVelocity()
        {
            double curVal = 0.0f;
            if (m_AxisTag != null)
            {
                curVal = (GetAxis() as MpAxis).CommandSpeed;
            }
            return curVal;
        }
        public double GetPdoVelocity()
        {
            double curVal = 0.0f;
            if (m_AxisTag != null)
            {
                curVal = (GetAxis() as MpAxis).CommandSpeedPdo;
            }
            return curVal;
        }
        public double GetFollowingError()
        {
            double curVal = 0.0f;
            if (m_AxisTag != null)
            {
                curVal = (GetAxis() as MpAxis).FollowingError;
            }
            return curVal;
        }
        public int GetCurPoint()
        {
            return GetServoUnit().GetCurPoint(GetAxis());
        }
        public double GetTeachingPosition(int point)
        {
            return GetServoUnit().GetCurAxisPoint(GetAxis(), point);
        }
        public VelSet GetTeachingVel(int prop)
        {
            return GetServoUnit().GetVel(GetAxis(), prop);
        }
        public double GetTargetPos()
        {
            double rv = 0.0f;
            rv = GetAxis().TargetPos;
            return rv;
        }
        public int GetAlarmID(enAxisResult rslt)
        {
            int rv = ALM_ServoAlarm.ID;
            if (rslt == enAxisResult.Alarm) rv = MXPErrorSegment();
            else if (rslt == enAxisResult.Timeover) rv = ALM_Timeover.ID;
            else if (rslt == enAxisResult.CmdError) rv = ALM_CmdError.ID;
            else if (rslt == enAxisResult.NotReadyError) rv = ALM_NotReadyError.ID;
            else if (rslt == enAxisResult.VelSettingError) rv = ALM_VelSettingError.ID;
            else if (rslt == enAxisResult.PosSettingError) rv = ALM_PosSettingError.ID;
            else if (rslt == enAxisResult.IntrError) rv = ALM_IntrError.ID;
            else if (rslt == enAxisResult.StopTimeout) rv = ALM_StopTimeout.ID;
            else if (rslt == enAxisResult.AlarmClearTimeout) rv = ALM_AlarmClearTimeout.ID;
            else if (rslt == enAxisResult.ServoOnTimeout) rv = ALM_ServoOnTimeout.ID;
            else if (rslt == enAxisResult.ServoOffTimeout) rv = ALM_ServoOffTimeout.ID;
            else if (rslt == enAxisResult.HomeTimeout) rv = ALM_HomeTimeout.ID;
            else if (rslt == enAxisResult.MoveTimeout) rv = ALM_MoveTimeout.ID;
            else if (rslt == enAxisResult.SoftwareLimitPos) rv = ALM_SoftwareLimitPos.ID;
            else if (rslt == enAxisResult.SoftwareLimitNeg) rv = ALM_SoftwareLimitNeg.ID;
            else if (rslt == enAxisResult.SoftwareLimitSpeed) rv = ALM_SoftwareLimitSpeed.ID;
            else if (rslt == enAxisResult.SoftwareLimitAcc) rv = ALM_SoftwareLimitAcc.ID;
            else if (rslt == enAxisResult.SoftwareLimitDec) rv = ALM_SoftwareLimitDec.ID;
            else if (rslt == enAxisResult.SoftwareLimitJerk) rv = ALM_SoftwareLimitJerk.ID;
            else if (rslt == enAxisResult.ZeroSetTimeout) rv = ALM_ZeroSetTimeout.ID;
            else if (rslt == enAxisResult.SequenceMoveTimeout) rv = ALM_SequenceMoveTimeout.ID;
            else if (rslt == enAxisResult.OverrideAbnormalStop) rv = ALM_OverrideAbnormalStop.ID;
            else if (rslt == enAxisResult.InrangeError) rv = ALM_InRangeError.ID;
            return rv;
        }
        public int MXPErrorSegment()
        {
            int rv = ALM_ServoAlarm.ID;

            if (m_AxisTag == null)
                rv = ALM_ServoAlarm.ID;
            
            //Driver Alarm보다 MXP 알람이 먼저 뜰때가 있음.. 501은 Driver에서 알람 발생했다는 알람임..
            else if ((GetAxis() as MpAxis).DriverAlarmCount > 0 || (GetAxis() as MpAxis).ControllerAlarmIdList.Contains(501)) 
            {
                if ((GetAxis() as MpAxis).DriverAlarmIdList.Contains(24))
                    rv = ALM_MotorCableOpenCircuit.ID;
                else if ((GetAxis() as MpAxis).DriverAlarmIdList.Contains(30))
                    rv = ALM_MotorEncoderCommunication.ID;
                else if ((GetAxis() as MpAxis).DriverAlarmIdList.Contains(31))
                    rv = ALM_MotorEncoderCableOpenCircuit.ID;
                else if ((GetAxis() as MpAxis).DriverAlarmIdList.Contains(21))
                    rv = ALM_ContinuousOverload.ID;
                else if ((GetAxis() as MpAxis).DriverAlarmIdList.Contains(23))
                    rv = ALM_RegenerationOverload.ID;
                else if ((GetAxis() as MpAxis).DriverAlarmIdList.Contains(10))
                    rv = ALM_IPMHardWareOvercurrent.ID;
                else if ((GetAxis() as MpAxis).DriverAlarmIdList.Contains(14))
                    rv = ALM_Overcurrent.ID;
                else if ((GetAxis() as MpAxis).DriverAlarmIdList.Contains(16))
                    rv = ALM_HardWareOvercurrentLimit.ID;
                else if ((GetAxis() as MpAxis).DriverAlarmIdList.Contains(15))
                    rv = ALM_CurrentOffsetAbnormal.ID;
                else if ((GetAxis() as MpAxis).DriverAlarmIdList.Contains(40))
                    rv = ALM_UnderVoltage.ID;
                else if ((GetAxis() as MpAxis).DriverAlarmIdList.Contains(42))
                    rv = ALM_MotorMainPowerAbnormal.ID;
                else if ((GetAxis() as MpAxis).DriverAlarmIdList.Contains(22))
                    rv = ALM_MotorDriverTemperature1.ID;
                else if ((GetAxis() as MpAxis).DriverAlarmIdList.Contains(25))
                    rv = ALM_MotorDriverTemperature2.ID;
                else if ((GetAxis() as MpAxis).DriverAlarmIdList.Contains(26))
                    rv = ALM_MotorEncoderTemperature.ID;
            }
            else if ((GetAxis() as MpAxis).ControllerAlarmCount > 0) 
            {
                if ((GetAxis() as MpAxis).ControllerAlarmIdList.Contains(555))
                    rv = ALM_EthercatCommunication.ID;
                else if ((GetAxis() as MpAxis).ControllerAlarmIdList.Contains(514))
                    rv = ALM_FollowingError.ID;
                else
                    rv = ALM_ServoAlarm.ID;
            }
            return rv;
        }
        public bool IsCheckTeachingPoint(TeachingData used_teach)
        {
            bool rv = false;
            // Teaching Position의 ID, NAME이 다를 경우 Device 설정이 잘 못 되어 있다....!
            foreach (TeachingData data in GetServoUnit().TeachingTable)
            {
                if (used_teach.PosId == data.PosId && used_teach.PosName == data.PosName)
                {
                    rv = true;
                    break;
                }
            }
            return rv;
        }
        public bool IsInRange()
        {
            bool rv = false;
            rv = Math.Abs(GetAxis().TargetPos - GetCurPosition()) < GetAxis().InRangeValue;
            return rv;
        }

        public double GetAxisCurLeftBarcode()
        {
            double curVal = 0.0f;
            if (m_AxisTag != null)
            {
                curVal = (GetAxis() as IAxisCommand).GetAxisCurLeftBarcode();
            }
            return curVal;
        }
        public void SetAxisLeftBarcode(double val)
        {
            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                if (m_AxisTag != null)
                {
                    GetAxis().LeftBcrPos = val;
                }
            }
        }
        public double GetAxisCurRightBarcode()
        {
            double curVal = 0.0f;
            if (m_AxisTag != null)
            {
                curVal = (GetAxis() as IAxisCommand).GetAxisCurRightBarcode();
            }
            return curVal;
        }
        public void SetAxisRightBarcode(double val)
        {
            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                if (m_AxisTag != null)
                {
                    GetAxis().RightBcrPos = val;
                }
            }
        }
        public bool IsAxisReady(bool moveOrigin = false, bool inMotion = false)
        {
            if (GetAxis() == null) return false;

            enAxisInFlag status = GetAxis().AxisStatus;  //(GetAxis() as IAxisCommand).GetAxisCurStatus();
            bool ready = true;
            ready &= (status & enAxisInFlag.SvOn) == enAxisInFlag.SvOn;
            ready &= !((status & enAxisInFlag.Alarm) == enAxisInFlag.Alarm);
            ready &= !((status & enAxisInFlag.OverrideAbnormalStop) == enAxisInFlag.OverrideAbnormalStop);
            ready &= !((status & enAxisInFlag.InRange_Error) == enAxisInFlag.InRange_Error);

            if (moveOrigin == false)
            {
                ready &= (status & enAxisInFlag.HEnd) == enAxisInFlag.HEnd;
            }
            if (inMotion == false)
            {
                if (GetAxis().InPosCheckSkip == false)
                    ready &= (status & enAxisInFlag.InPos) == enAxisInFlag.InPos;
                ready &= !((status & enAxisInFlag.Cmd_Confirm) == enAxisInFlag.Cmd_Confirm);
            }

            return ready;
        }
        public bool IsAxisContinuousReady(bool moveOrigin = false)
        {
            if (GetAxis() == null) return false;

            enAxisInFlag status = GetAxis().AxisStatus;  //(GetAxis() as IAxisCommand).GetAxisCurStatus();
            bool ready = true;
            ready &= (status & enAxisInFlag.SvOn) == enAxisInFlag.SvOn;
            ready &= !((status & enAxisInFlag.Alarm) == enAxisInFlag.Alarm);
            ready &= !((status & enAxisInFlag.OverrideAbnormalStop) == enAxisInFlag.OverrideAbnormalStop);
            ready &= !((status & enAxisInFlag.InRange_Error) == enAxisInFlag.InRange_Error);

            if (moveOrigin == false)
            {
                ready &= (status & enAxisInFlag.HEnd) == enAxisInFlag.HEnd;
            }

            return ready;
        }
        public bool IsMoving()
        {
            bool isMoving = false;
            if (GetAxis() == null) return isMoving;
            enAxisInFlag status = GetAxis().AxisStatus; // (GetAxis() as IAxisCommand).GetAxisCurStatus();
            isMoving = (status & enAxisInFlag.Busy) == enAxisInFlag.Busy;
            return isMoving;
        }
        public bool IsAlarm()
        {
            bool isAlarm = false;
            if (GetAxis() == null) return isAlarm;
            enAxisInFlag status = GetAxis().AxisStatus; // (GetAxis() as IAxisCommand).GetAxisCurStatus();
            isAlarm = (status & enAxisInFlag.Alarm) == enAxisInFlag.Alarm;
            return isAlarm;
        }
        public bool IsHomeSensor()
        {
            bool org = false;
            if (GetAxis() == null) return org;
            org = (GetAxis().AxisStatus & enAxisInFlag.Org) == enAxisInFlag.Org;
            return org;
        }
        public bool IsInpos()
        {
            bool isinpos = false;
            if (GetAxis() == null) return isinpos;
            enAxisInFlag status = GetAxis().AxisStatus; // (GetAxis() as IAxisCommand).GetAxisCurStatus();
            if (GetAxis().InPosCheckSkip == false)
                isinpos = (status & enAxisInFlag.InPos) == enAxisInFlag.InPos;
            else
                isinpos = true;

            return isinpos;
        }
        #endregion

        #region Methods - Action
        public override void SeqAbort()
        {
            if (!Initialized) return;

            //Trace.WriteLine(string.Format("DevAxis - {0}, SeqAbort()", DevName));

            m_SeqStop.SeqAbort();
            m_SeqAlarmClear.SeqAbort();
            m_SeqServoOff.SeqAbort();
            m_SeqServoOn.SeqAbort();
            m_SeqOrigin.SeqAbort();
            m_SeqHome.SeqAbort();
            m_SeqMove.SeqAbort();
            m_SeqRelativeMove.SeqAbort();
            m_SeqZeroSet.SeqAbort();
            SetHolding(false);

            enAxisInFlag status = GetAxis().AxisStatus; // (GetAxis() as IAxisCommand).GetAxisCurStatus();
            if ((status & enAxisInFlag.InPos) != enAxisInFlag.InPos) GetServoUnit().Stop(GetAxis()); //움직이고 있는 Axis만  Stop 시키자...!
        }
        // Override 0으로 셋팅
        public void SetHolding(bool hold)
        {
            (GetAxis() as IAxisCommand).SetHolding(hold);
        }
        public void SetPause(bool pause)
        {
            (GetAxis() as IAxisCommand).SetPause(pause);
        }
        public void SetSequenceMoveCommand(bool set)
        {
            (GetAxis() as IAxisCommand).SetSequenceMoveCommand(set);
        }
        public int Stop()
        {
            if (m_SeqStop == null) return m_ALM_CmdError.ID;
            return m_SeqStop.Do();
        }
        public int AlarmClear()
        {
            if (m_SeqAlarmClear == null) return m_ALM_CmdError.ID;
            return m_SeqAlarmClear.Do();
        }

        public int ZeroSet()
        {
            if (m_SeqZeroSet == null) return m_ALM_CmdError.ID;
            return m_SeqZeroSet.Do();
        }

        public int ServoOff()
        {
            if (m_SeqServoOff == null) return m_ALM_CmdError.ID;
            return m_SeqServoOff.Do();
        }

        public int ServoOn()
        {
            if (m_SeqServoOn == null) return m_ALM_CmdError.ID;
            return m_SeqServoOn.Do();
        }

        public int ServoOrigin()
        {
            if (m_SeqOrigin == null) return m_ALM_CmdError.ID;
            return m_SeqOrigin.Do();
        }

        public int Home()
        {
            if (m_SeqHome == null) return m_ALM_CmdError.ID;
            return m_SeqHome.Do();
        }

        public int Move(ushort point, ushort prop, bool safty)
        {
            if (m_SeqMove == null) return m_ALM_CmdError.ID;
            return m_SeqMove.Do(point, prop, safty);
        }

        public int Move(double pos, VelSet set, bool safty)
        {
            if (m_SeqMove == null) return m_ALM_CmdError.ID;
            return m_SeqMove.Do(pos, set, safty);
        }
        public int ContinuousMove(double pos, VelSet set, bool safty)
        {
            if (m_SeqMove == null) return m_ALM_CmdError.ID;
            return m_SeqMove.Do_Continuous(pos, set, safty);
        }

        public int RelativeMove(double offset, VelSet set, bool safty)
        {
            if (m_SeqRelativeMove == null) return m_ALM_CmdError.ID;
            return m_SeqRelativeMove.Do(offset, set, safty);
        }

        public int JogMove(enAxisOutFlag cmd, VelSet set)
        {
            GetServoUnit().JogMove(GetAxis(), cmd, set);
            return 0;
        }
        public int JogStop()
        {
            GetServoUnit().JogStop(GetAxis());
            return 0;
        }
        #endregion

        #region Sequence
        private class SeqStop : XSeqFunc
        {
            #region Fields
            _DevAxis m_Device = null;
            _Axis m_Axis = null;
            ServoUnit m_Servo = null;
            int nRetry = 0;
            #endregion

            #region Constructor
            public SeqStop(_DevAxis device)
            {
                this.SeqName = $"SeqStop{device.MyName}";
                m_Device = device;

                m_Servo = m_Device.GetServoUnit();
                m_Axis = m_Device.GetAxis();
            }

            public override void SeqAbort()
            {
                nRetry = 0;
                this.InitSeq();
            }
            #endregion

            #region override
            public override int Do()
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            //Axis Stop
                            enAxisResult stop_start = m_Servo.Stop(m_Axis);
                            if (stop_start == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Start", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else if (stop_start > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), stop_start);
                                
                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Start Alarm[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(stop_start);
                                nRetry = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 20) break; // Command이 완료되는 시간 대기

                            //Axis Done
                            enAxisResult stop_end = m_Servo.MotionDoneAxis(m_Axis);
                            if (stop_end == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop OK", m_Servo.ServoName, m_Axis.AxisName));
                                rv = 0;
                                nRetry = 0;
                                seqNo = 0;
                            }
                            else if (stop_end == enAxisResult.CmdError && nRetry < 5)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Command Set Abnormal. Retry !", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else if (stop_end > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), stop_end);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Error[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(stop_end);
                                nRetry = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.StopTimeout);
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                rv = m_Device.GetAlarmID(enAxisResult.StopTimeout);
                                nRetry = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                nRetry++;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            #endregion
        }

        private class SeqServoOff : XSeqFunc
        {
            #region Fields
            _DevAxis m_Device = null;
            _Axis m_Axis = null;
            ServoUnit m_Servo = null;
            int nRetry = 0;
            #endregion

            #region Constructor
            public SeqServoOff(_DevAxis device)
            {
                this.SeqName = $"SeqServoOff{device.MyName}";
                m_Device = device;

                m_Servo = m_Device.GetServoUnit();
                m_Axis = m_Device.GetAxis();
            }

            public override void SeqAbort()
            {
                nRetry = 0;
                this.InitSeq();
            }
            #endregion

            #region override
            public override int Do()
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            //Axis Servo Off
                            enAxisResult servo_off_start = m_Servo.ServoOff(m_Axis);
                            if (servo_off_start == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Servo Off Start", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else if (servo_off_start > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), servo_off_start);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Servo Off Start Alarm[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(servo_off_start);
                                nRetry = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 20) break; // Command이 완료되는 시간 대기

                            //Axis Done
                            enAxisResult servo_off_end = m_Servo.MotionDoneAxis(m_Axis);
                            if (servo_off_end == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Servo Off OK", m_Servo.ServoName, m_Axis.AxisName));
                                rv = 0;
                                nRetry = 0;
                                seqNo = 0;
                            }
                            else if (servo_off_end == enAxisResult.CmdError && nRetry < 5)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Servo Off Command Set Abnormal. Retry !", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else if (servo_off_end > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), servo_off_end);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Servo Off Error[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(servo_off_end);
                                nRetry = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.ServoOffTimeout);
                                ServoLog.WriteLog(string.Format("{0}.{1} Servo Off Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                rv = m_Device.GetAlarmID(enAxisResult.ServoOffTimeout);
                                nRetry = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                nRetry++;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            #endregion
        }

        private class SeqServoOn : XSeqFunc
        {
            #region Fields
            _DevAxis m_Device = null;
            _Axis m_Axis = null;
            ServoUnit m_Servo = null;
            int nRetry = 0;
            #endregion

            #region Constructor
            public SeqServoOn(_DevAxis device)
            {
                this.SeqName = $"SeqServoOn{device.MyName}";
                m_Device = device;

                m_Servo = m_Device.GetServoUnit();
                m_Axis = m_Device.GetAxis();
            }

            public override void SeqAbort()
            {
                nRetry = 0;
                this.InitSeq();
            }
            #endregion

            #region override
            public override int Do()
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            //Axis Servo On
                            enAxisResult servo_on_start = m_Servo.ServoOn(m_Axis);
                            if (servo_on_start == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Servo On Start", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else if (servo_on_start > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), servo_on_start);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Servo On Start Alarm[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(servo_on_start);
                                nRetry = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 20) break; // Command이 완료되는 시간 대기

                            //Axis Done
                            enAxisResult servo_on_end = m_Servo.MotionDoneAxis(m_Axis);
                            if (servo_on_end == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Servo On OK", m_Servo.ServoName, m_Axis.AxisName));
                                rv = 0;
                                nRetry = 0;
                                seqNo = 0;
                            }
                            else if (servo_on_end == enAxisResult.CmdError && nRetry < 5)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Servo On Command Set Abnormal. Retry !", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else if (servo_on_end > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), servo_on_end);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Servo On Error[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(servo_on_end);
                                nRetry = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.ServoOnTimeout);
                                ServoLog.WriteLog(string.Format("{0}.{1} Servo On Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                rv = m_Device.GetAlarmID(enAxisResult.ServoOnTimeout);
                                nRetry = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                nRetry++;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            #endregion
        }

        private class SeqAlarmClear : XSeqFunc
        {
            #region Fields
            _DevAxis m_Device = null;
            _Axis m_Axis = null;
            ServoUnit m_Servo = null;
            int nRetry = 0;
            #endregion

            #region Constructor
            public SeqAlarmClear(_DevAxis device)
            {
                this.SeqName = $"SeqAlarmClear{device.MyName}";
                m_Device = device;

                m_Servo = m_Device.GetServoUnit();
                m_Axis = m_Device.GetAxis();
            }

            public override void SeqAbort()
            {
                nRetry = 0;
                this.InitSeq();
            }
            #endregion

            #region override
            public override int Do()
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            //Axis Alarm Clear
                            enAxisResult alarm_clear_start = m_Servo.AlarmClear(m_Axis);
                            if (alarm_clear_start == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Alarm Clear Start", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else if (alarm_clear_start > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), alarm_clear_start);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Alarm Clear Start Alarm[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(alarm_clear_start);
                                nRetry = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 20) break; // Command이 완료되는 시간 대기

                            //Axis Done
                            enAxisResult alarm_clear_end = m_Servo.MotionDoneAxis(m_Axis);
                            if (alarm_clear_end == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Alarm Clear OK", m_Servo.ServoName, m_Axis.AxisName));
                                rv = 0;
                                nRetry = 0;
                                seqNo = 0;
                            }
                            else if (alarm_clear_end == enAxisResult.CmdError && nRetry < 5)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Alarm Clear Command Set Abnormal. Retry !", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else if (alarm_clear_end > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), alarm_clear_end);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Alarm Clear Error[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(alarm_clear_end);
                                nRetry = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.AlarmClearTimeout);
                                ServoLog.WriteLog(string.Format("{0}.{1} Alarm Clear Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                rv = m_Device.GetAlarmID(enAxisResult.AlarmClearTimeout);
                                nRetry = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                nRetry++;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            #endregion
        }

        private class SeqZeroSet : XSeqFunc
        {
            #region Fields
            _DevAxis m_Device = null;
            _Axis m_Axis = null;
            ServoUnit m_Servo = null;
            int nRetry = 0;
            #endregion

            #region Constructor
            public SeqZeroSet(_DevAxis device)
            {
                this.SeqName = $"SeqZeroSet{device.MyName}";
                m_Device = device;

                m_Servo = m_Device.GetServoUnit();
                m_Axis = m_Device.GetAxis();
            }

            public override void SeqAbort()
            {
                nRetry = 0;
                this.InitSeq();
            }
            #endregion

            #region override
            public override int Do()
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            //Axis Alarm Clear
                            enAxisResult zero_set_start = m_Servo.ZeroSet(m_Axis);
                            if (zero_set_start == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Zero Set Start", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else if (zero_set_start > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), zero_set_start);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Zero Set Start Alarm[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(zero_set_start);
                                nRetry = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 20) break; // Command이 완료되는 시간 대기

                            //Axis Done
                            enAxisResult zero_set_end = m_Servo.MotionDoneAxis(m_Axis);
                            if (zero_set_end == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Zero Set OK", m_Servo.ServoName, m_Axis.AxisName));
                                rv = 0;
                                nRetry = 0;
                                seqNo = 0;
                            }
                            else if (zero_set_end == enAxisResult.CmdError && nRetry < 5)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Zero Set Command Set Abnormal. Retry !", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else if (zero_set_end > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), zero_set_end);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Zero Set Error[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(zero_set_end);
                                nRetry = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.AlarmClearTimeout);
                                ServoLog.WriteLog(string.Format("{0}.{1} Zero Set Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                rv = m_Device.GetAlarmID(enAxisResult.ZeroSetTimeout);
                                nRetry = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                nRetry++;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            #endregion
        }

        private class SeqOrigin : XSeqFunc
        {
            #region Fields
            _DevAxis m_Device = null;
            _Axis m_Axis = null;
            ServoUnit m_Servo = null;
            int nRetry = 0;
            #endregion

            #region Constructor
            public SeqOrigin(_DevAxis device)
            {
                this.SeqName = $"SeqOrigin{device.MyName}";
                m_Device = device;

                m_Servo = m_Device.GetServoUnit();
                m_Axis = m_Device.GetAxis();
            }

            public override void SeqAbort()
            {
                nRetry = 0;
                this.InitSeq();
            }
            #endregion

            #region override
            public override int Do()
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            //Axis Home
                            enAxisResult home_start = m_Servo.Home(m_Axis);
                            if (home_start == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Home Start", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else if (home_start > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), home_start);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Home Start Alarm[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(home_start);
                                nRetry = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 20) break; // Command이 완료되는 시간 대기

                            //Axis Done
                            enAxisResult home_end = m_Servo.MotionDoneAxis(m_Axis);
                            if (home_end == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Home OK", m_Servo.ServoName, m_Axis.AxisName));
                                rv = 0;
                                nRetry = 0;
                                seqNo = 0;
                            }
                            else if (home_end == enAxisResult.CmdError && nRetry < 5)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Home Command Set Abnormal. Retry !", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else if (home_end > enAxisResult.Success)
                            {
                                if ((home_end == enAxisResult.MoveTimeout ||
                                    home_end == enAxisResult.Timeover ||
                                    home_end == enAxisResult.HomeTimeout ||
                                    home_end == enAxisResult.ZeroSetTimeout) && nRetry < 5)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Home Command Set Timeover. Retry !", m_Servo.ServoName, m_Axis.AxisName));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 20;
                                }
                                else
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), home_end);

                                    List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                    List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                    if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                    string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                    ServoLog.WriteLog(string.Format("{0}.{1} Home Error[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                    rv = m_Device.GetAlarmID(home_end);
                                    nRetry = 0;
                                    seqNo = 0;
                                }
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 500 * 1000)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.HomeTimeout);
                                ServoLog.WriteLog(string.Format("{0}.{1} Home Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                rv = m_Device.GetAlarmID(enAxisResult.HomeTimeout);
                                nRetry = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 20:
                        {
                            //Axis Stop
                            enAxisResult stop_start = m_Servo.Stop(m_Axis);
                            if (stop_start == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Start", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                            else if (stop_start > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), stop_start);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Start Alarm[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(stop_start);
                                nRetry = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 30:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 100) break; // Command이 완료되는 시간 대기

                            //Axis Done
                            enAxisResult stop_end = m_Servo.MotionDoneAxis(m_Axis);
                            if (stop_end == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop OK", m_Servo.ServoName, m_Axis.AxisName));
                                seqNo = 40;
                            }
                            else if (stop_end > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), stop_end);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Error[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(stop_end);
                                nRetry = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.StopTimeout);
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                rv = m_Device.GetAlarmID(enAxisResult.StopTimeout);
                                nRetry = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 40:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 500)
                            {
                                nRetry++;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            #endregion
        }

        private class SeqHome : XSeqFunc
        {
            #region Fields
            _DevAxis m_Device = null;
            int m_Retry = 0; // Alarm Clear가 잘 않된다. 5번정도 반복해 보자....
            #endregion

            #region Constructor
            public SeqHome(_DevAxis device)
            {
                this.SeqName = $"SeqHome{device.MyName}";
                m_Device = device;
            }

            public override void SeqAbort()
            {
                m_Device.Homing = false;
                this.InitSeq();
            }
            #endregion

            #region override
            public override int Do()
            {
                int rv = -1;
                int rv1 = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_Device.GetAxis().CommandSkip)
                            {
                                rv = 0;
                            }
                            else
                            {
                                int curPoint = m_Device.GetCurPoint();

                                //Wait Pos면서 Home Sensor가 들어와 있으면 Home 잡을 필요 없을듯..
                                if (m_Device.IsAxisReady() && curPoint == 0 && (m_Device.GetAxis().AxisStatus & enAxisInFlag.Org) == enAxisInFlag.Org) // cann't need home
                                {
                                    rv = 0;
                                }
                                else
                                {
                                    m_Device.Homing = true;
                                    if (m_Device.IsAxisReady())
                                    {
                                        seqNo = 50;
                                    }
                                    else
                                    {
                                        if (((m_Device.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn)
                                        {
                                            m_Retry = 0;
                                            seqNo = 1;
                                        }
                                        else
                                        {
                                            //Servo Off 상태에서 Stop을 하면 Alarm이 발생한다...
                                            m_Retry = 0;
                                            seqNo = 10;
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case 1:
                        {
                            //Axis Stop
                            rv1 = m_Device.Stop();
                            if (rv1 == 0)
                            {
                                if (m_Device.GetAxis().HomeMethod == enHomeMethod.ABS_ZERO) seqNo = 20;
                                else seqNo = 10;
                            }
                            else if (rv1 > 0)
                            {
                                m_Device.Homing = false;
                                rv = rv1;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            //Axis Servo Off
                            rv1 = m_Device.ServoOff();
                            if (rv1 == 0)
                            {
                                seqNo = 20;
                            }
                            else if (rv1 > 0)
                            {
                                m_Device.Homing = false;
                                rv = rv1;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 20:
                        {
                            //Axis Alarm Clear
                            rv1 = m_Device.AlarmClear();
                            if (rv1 == 0)
                            {
                                if (((m_Device.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn) seqNo = 50;
                                else  seqNo = 30;
                            }
                            else if (rv1 > 0)
                            {
                                if (m_Retry < 5)
                                {
                                    m_Retry++;
                                    seqNo = 10;
                                }
                                else
                                {
                                    m_Device.Homing = false;
                                    rv = rv1;
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 30:
                        {
                            //Axis Servo On
                            rv1 = m_Device.ServoOn();
                            if (rv1 == 0)
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 40;
                            }
                            else if (rv1 > 0)
                            {
                                m_Device.Homing = false;
                                rv = rv1;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 40:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                if (((m_Device.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn)
                                    seqNo = 50;
                                else
                                    seqNo = 30;
                            }
                        }
                        break;

                    case 50:
                        {
                            //Axis Origin
                            rv1 = m_Device.ServoOrigin();
                            if (rv1 == 0)
                            {
                                m_Device.Homing = false;
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (rv1 > 0)
                            {
                                m_Device.Homing = false;
                                rv = rv1;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            #endregion
        }

        private class SeqMove : XSeqFunc
        {
            #region Fields
            _DevAxis m_Device = null;
            _Axis m_Axis = null;
            ServoUnit m_Servo = null;

            private int m_RetryCount = 0; // Inposition Retry
            private int m_SetTimeout = 60; // 30 sec
            private int m_RetryCmdError = 0; // CmdError Retry
            private int m_RetryReady = 0; // Alarm이 발생되어 있으면 AlarmClear Try하자~~
            #endregion

            #region Constructor
            public SeqMove(_DevAxis device)
            {
                this.SeqName = $"SeqMove{device.MyName}";
                m_Device = device;

                m_Servo = m_Device.GetServoUnit();
                m_Axis = m_Device.GetAxis();
            }

            public override void SeqAbort()
            {
                m_RetryCount = 0;
                m_RetryCmdError = 0;
                m_RetryReady = 0;
                this.InitSeq();
            }
            #endregion

            #region Methods
            public int Do(ushort point, ushort prop, bool safty_check)
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_Axis.CommandSkip)
                            {
                                rv = 0;
                            }
                            else
                            {
                                // Move Skip 판단
                                // CurPosition == Teaching Position 이면 굳이 이동하지 말자
                                bool move_skip = true;
                                double target_pos = m_Servo.GetCurAxisPoint(m_Axis, point);
                                double cur_pos = (m_Axis as IAxisCommand).GetAxisCurPos();
                                double in_range = m_Axis.InRangeValue;
                                move_skip &= (Math.Abs(target_pos - cur_pos) < in_range) ? true : false;
                                if (move_skip)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Position Move Skip[Target:{2},Cur:{3}]", m_Servo.ServoName, m_Axis.AxisName, target_pos, cur_pos));
                                    rv = 0;
                                    seqNo = 0;
                                }
                                else
                                {
                                    m_RetryCmdError = 0;
                                    m_RetryCount = 0;
                                    m_RetryReady = 0;
                                    m_Servo.SetTargetPosition(m_Axis, point, prop);
                                    seqNo = 5;
                                }
                            }
                        }
                        break;

                    case 5:
                        {
                            // safty check
                            if (safty_check)
                            {
                                bool isSafe = true;
                                double target = m_Servo.GetCurAxisPoint(m_Axis, point);
                                isSafe &= InterlockManager.IsSafe(m_Axis, target);
                                if (isSafe)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Safty Check OK[Target={2}]", m_Servo.ServoName, m_Axis.AxisName, target));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 10;
                                }
                                else
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.IntrError);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Safty Check NG[{2},Target={3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, target));
                                    rv = m_Device.GetAlarmID(enAxisResult.IntrError);
                                    seqNo = 0;
                                }
                            }
                            else
                            {
                                seqNo = 10;
                            }
                        }
                        break;

                    case 10:
                        {
                            //이때는 무조건 Ready 상태여야 한다.
                            if (m_Device.IsAxisReady())
                            {
                                //Axis Move
                                enAxisResult move_start = m_Servo.MoveAxisStart(m_Axis, point, prop);
                                if (move_start == enAxisResult.Success)
                                {
                                    double cur_pos = (m_Axis as IAxisCommand).GetAxisCurPos();
                                    double end_pos = m_Servo.GetCurAxisPoint(m_Axis, point);
                                    VelSet set = m_Servo.GetVel(m_Axis, prop);
                                    m_SetTimeout = (int)(Math.Abs(end_pos - cur_pos) / set.Vel + m_Axis.SetTimeoutMargin / 1000.0f);

                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Start _ SetTime={2}", m_Servo.ServoName, m_Axis.AxisName, m_SetTimeout));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 20;
                                }
                                else if (move_start > enAxisResult.Success)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), move_start);

                                    List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                    List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                    if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                    string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Start Alarm[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                    rv = m_Device.GetAlarmID(move_start);
                                    seqNo = 0;
                                }
                            }
                            else
                            {
                                enAxisInFlag status = (m_Axis as IAxisCommand).GetAxisCurStatus();
                                bool recover_alarm = false;
                                recover_alarm |= ((status & enAxisInFlag.Alarm) == enAxisInFlag.Alarm);
                                recover_alarm |= ((status & enAxisInFlag.OverrideAbnormalStop) == enAxisInFlag.OverrideAbnormalStop);
                                recover_alarm |= ((status & enAxisInFlag.InRange_Error) == enAxisInFlag.InRange_Error);
                                recover_alarm &= (status & enAxisInFlag.SvOn) == enAxisInFlag.SvOn;
                                if (recover_alarm && m_RetryReady < 5)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Alarm Status, Try Alarm Clear", m_Servo.ServoName, m_Axis.AxisName));
                                    seqNo = 100;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > m_Axis.InposWaitTime)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.NotReadyError);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                    rv = m_Device.GetAlarmID(enAxisResult.NotReadyError);
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 20:
                        {
                            bool timer_over = XFunc.GetTickCount() - StartTicks > 100; // Motion이 완료되는 시간 대기

                            if (timer_over)
                            {
                                //Axis Done
                                enAxisResult move_end = m_Servo.MotionDoneAxis(m_Axis);
                                if (move_end == enAxisResult.Success)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move OK", m_Servo.ServoName, m_Axis.AxisName));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 30;
                                }
                                else if (move_end == enAxisResult.CmdError && m_RetryCmdError < 5)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Command Set Abnormal. Retry !", m_Servo.ServoName, m_Axis.AxisName));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 50;
                                }
                                else if (move_end > enAxisResult.Success)
                                {
                                    if (move_end == enAxisResult.MoveTimeout && m_RetryCmdError < 5)
                                    {
                                        ServoLog.WriteLog(string.Format("{0}.{1} Move Command Set Timeover. Retry !", m_Servo.ServoName, m_Axis.AxisName));
                                        StartTicks = XFunc.GetTickCount();
                                        seqNo = 50;
                                    }
                                    else
                                    {
                                        string code_name = Enum.GetName(typeof(enAxisResult), move_end);

                                        List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                        List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                        if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                        string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                        ServoLog.WriteLog(string.Format("{0}.{1} Move Error[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                        rv = m_Device.GetAlarmID(move_end);
                                        seqNo = 0;
                                    }
                                }
                                else if (XFunc.GetTickCount() - StartTicks > m_SetTimeout * 1000)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.MoveTimeout);
                                    rv = m_Device.GetAlarmID(enAxisResult.MoveTimeout);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, rv));
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 30:
                        {
                            //Position Check
                            bool pos_check = true;
                            double target_pos = m_Servo.GetCurAxisPoint(m_Axis, point);
                            double cur_torque = (m_Axis as IAxisCommand).GetAxisCurTorque();
                            double cur_pos = (m_Axis as IAxisCommand).GetAxisCurPos();
                            double in_range = m_Axis.InRangeValue;
                            pos_check &= (Math.Abs(target_pos - cur_pos) < in_range) ? true : false;
                            // Clamp 20g 이내일때 OK
                            // Counter Force 100g OK

                            if (pos_check || m_Axis.CommandSkip)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Position Check[Target:{2},Cur:{3}]", m_Servo.ServoName, m_Axis.AxisName, target_pos, cur_pos));
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Axis.InposWaitTime)
                            {
                                if (m_RetryCount < 5)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Position Check({2})[Target:{3},Cur:{4}]", m_Servo.ServoName, m_Axis.AxisName, m_RetryCount, target_pos, cur_pos));
                                    m_RetryCount++;
                                    StartTicks = XFunc.GetTickCount();
                                    if (Math.Abs(target_pos - cur_pos) > in_range) seqNo = 40;
                                    else seqNo = 10;
                                }
                                else
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.MoveTimeout);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                    rv = m_Device.GetAlarmID(enAxisResult.MoveTimeout);
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 40:
                        {
                            // HJYOU  - 혹시나 모르지 .... Safty Check를 새로 하자.
                            if (safty_check)
                            {
                                bool isSafe = true;
                                double target = m_Servo.GetCurAxisPoint(m_Axis, point);
                                isSafe &= InterlockManager.IsSafe(m_Axis, target);
                                if (isSafe)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Safty Check OK[Target={2}]", m_Servo.ServoName, m_Axis.AxisName, target));
                                    seqNo = 10;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > m_Axis.InposWaitTime)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.IntrError);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Safty Check NG[{2},Target={3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, target));
                                    rv = m_Device.GetAlarmID(enAxisResult.IntrError);
                                    seqNo = 0;
                                }
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Axis.InposWaitTime)
                            {
                                seqNo = 10;
                            }
                        }
                        break;

                    case 50:
                        {
                            //Axis Stop
                            enAxisResult stop_start = m_Servo.Stop(m_Axis);
                            if (stop_start == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Start", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 60;
                            }
                            else if (stop_start > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), stop_start);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Start Alarm[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(stop_start);
                                m_RetryCmdError = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 60:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 100) break; // Command이 완료되는 시간 대기

                            //Axis Done
                            enAxisResult stop_end = m_Servo.MotionDoneAxis(m_Axis);
                            if (stop_end == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop OK", m_Servo.ServoName, m_Axis.AxisName));
                                seqNo = 70;
                            }
                            else if (stop_end > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), stop_end);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Error[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(stop_end);
                                m_RetryCmdError = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.StopTimeout);
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                rv = m_Device.GetAlarmID(enAxisResult.StopTimeout);
                                m_RetryCmdError = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 70:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 500)
                            {
                                m_RetryCmdError++;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 100:
                        {
                            //Axis Alarm Clear
                            int rv1 = m_Device.AlarmClear();
                            if (rv1 == 0)
                            {
                                m_RetryReady++;
                                seqNo = 10;
                            }
                            else if (rv1 > 0)
                            {
                                rv = rv1;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            /// <summary>
            /// moving : 이동중 신규명령을 내릴 경우
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="set"></param>
            /// <param name="safty_check"></param>
            /// <param name="moving"></param>
            /// <returns></returns>
            public int Do(double pos, VelSet set, bool safty_check = true, bool moving = false)
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_Axis.CommandSkip)
                            {
                                rv = 0;
                            }
                            else
                            {
                                // Move Skip 판단
                                // CurPosition == Teaching Position 이면 굳이 이동하지 말자
                                bool move_skip = true;
                                double target_pos = pos;
                                double cur_pos = (m_Axis as IAxisCommand).GetAxisCurPos();
                                double in_range = m_Axis.InRangeValue;
                                move_skip &= (Math.Abs(target_pos - cur_pos) < in_range) ? true : false;
                                if (move_skip)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Position Move Skip[Target:{2},Cur:{3}]", m_Servo.ServoName, m_Axis.AxisName, target_pos, cur_pos));
                                    rv = 0;
                                    seqNo = 0;
                                }
                                else
                                {
                                    m_RetryReady = 0;
                                    m_RetryCmdError = 0;
                                    m_Servo.SetTargetPosition(m_Axis, pos, set);
                                    seqNo = 5;
                                }
                            }
                        }
                        break;

                    case 5:
                        {
                            // safty check
                            if (safty_check)
                            {
                                bool isSafe = true;
                                isSafe &= InterlockManager.IsSafe(m_Axis, pos);
                                if (isSafe)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Safty Check OK[Target={2}]", m_Servo.ServoName, m_Axis.AxisName, pos));
                                    StartTicks = XFunc.GetTickCount();
                                    m_RetryCount = 0;
                                    seqNo = 10;
                                }
                                else
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.IntrError);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Safty Check NG[{2},Target={3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, pos));
                                    rv = m_Device.GetAlarmID(enAxisResult.IntrError);
                                    seqNo = 0;
                                }
                            }
                            else
                            {
                                m_RetryCount = 0;
                                seqNo = 10;
                            }
                        }
                        break;

                    case 10:
                        {
                            //이때는 무조건 Ready 상태여야 한다.
                            if (m_Device.IsAxisReady(false, moving))
                            {
                                //Axis Move
                                if (set.Vel > m_Axis.SpeedLimit) set.Vel = m_Axis.SpeedLimit;
                                enAxisResult move_start = m_Servo.MoveAxisStart(m_Axis, pos, set);
                                if (move_start == enAxisResult.Success)
                                {
                                    double cur_pos = (m_Axis as IAxisCommand).GetAxisCurPos();
                                    double end_pos = pos;
                                    double speed = set.Vel;
                                    m_SetTimeout = (int)(Math.Abs(end_pos - cur_pos) / speed + m_Axis.SetTimeoutMargin / 1000.0f);

                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Start _ SetTime={2}", m_Servo.ServoName, m_Axis.AxisName, m_SetTimeout));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 20;
                                }
                                else if (move_start > enAxisResult.Success)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), move_start);

                                    List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                    List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                    if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                    string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Start Alarm[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                    rv = m_Device.GetAlarmID(move_start);
                                    seqNo = 0;
                                }
                            }
                            else
                            {
                                enAxisInFlag status = (m_Axis as IAxisCommand).GetAxisCurStatus();
                                bool recover_alarm = false;
                                recover_alarm |= ((status & enAxisInFlag.Alarm) == enAxisInFlag.Alarm);
                                recover_alarm |= ((status & enAxisInFlag.OverrideAbnormalStop) == enAxisInFlag.OverrideAbnormalStop);
                                recover_alarm |= ((status & enAxisInFlag.InRange_Error) == enAxisInFlag.InRange_Error);
                                recover_alarm &= (status & enAxisInFlag.SvOn) == enAxisInFlag.SvOn;
                                if (recover_alarm && m_RetryReady < 5)
                                {
								    if(((status & enAxisInFlag.Alarm) == enAxisInFlag.Alarm))
                                        ServoLog.WriteLog(string.Format("{0}.{1} Axis Servo Alarm", m_Servo.ServoName, m_Axis.AxisName));
                                    else if((status & enAxisInFlag.OverrideAbnormalStop) == enAxisInFlag.OverrideAbnormalStop)
                                        ServoLog.WriteLog(string.Format("{0}.{1} Axis Override Abnormal Stop", m_Servo.ServoName, m_Axis.AxisName));
                                    else if((status & enAxisInFlag.InRange_Error) == enAxisInFlag.InRange_Error)
                                        ServoLog.WriteLog(string.Format("{0}.{1} Axis In Range Error", m_Servo.ServoName, m_Axis.AxisName));
                                    ServoLog.WriteLog(string.Format("{0}.{1} Alarm Status, Try Alarm Clear", m_Servo.ServoName, m_Axis.AxisName));
                                    seqNo = 100;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > m_Axis.InposWaitTime)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.NotReadyError);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                    rv = m_Device.GetAlarmID(enAxisResult.NotReadyError);
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 20:
                        {
                            bool timer_over = XFunc.GetTickCount() - StartTicks > 100;

                            if (timer_over)
                            {
                                //Axis Done
                                enAxisResult move_end = m_Servo.MotionDoneAxis(m_Axis);
                                if (move_end == enAxisResult.Success)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move OK", m_Servo.ServoName, m_Axis.AxisName));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 30;
                                }
                                else if (move_end == enAxisResult.CmdError && m_RetryCmdError < 5)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Command Set Abnormal. Retry !", m_Servo.ServoName, m_Axis.AxisName));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 50;
                                }
                                else if (move_end > enAxisResult.Success)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), move_end);

                                    List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                    List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                    if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                    string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Error[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                    rv = m_Device.GetAlarmID(move_end);
                                    seqNo = 0;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > m_SetTimeout * 1000)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.MoveTimeout);
                                    rv = m_Device.GetAlarmID(enAxisResult.MoveTimeout);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name, rv));
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 30:
                        {
                            //Position Check
                            bool pos_check = true;
                            double target_pos = pos;
                            double cur_torque = (m_Axis as IAxisCommand).GetAxisCurTorque();
                            double cur_pos = (m_Axis as IAxisCommand).GetAxisCurPos();
                            double in_range = m_Axis.InRangeValue;
                            pos_check &= (Math.Abs(target_pos - cur_pos) < in_range) ? true : false;
                            if (pos_check || m_Axis.CommandSkip)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Position Check[Target:{2},Cur:{3}]", m_Servo.ServoName, m_Axis.AxisName, target_pos, cur_pos));
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Axis.InposWaitTime)
                            {
                                if (m_RetryCount < 5)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Position Check({2})[Target:{3},Cur:{4}]", m_Servo.ServoName, m_Axis.AxisName, m_RetryCount, target_pos, cur_pos));
                                    m_RetryCount++;
                                    StartTicks = XFunc.GetTickCount();
                                    if (Math.Abs(target_pos - cur_pos) > in_range) seqNo = 40;
                                    else seqNo = 10;
                                }
                                else
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.MoveTimeout);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                    rv = m_Device.GetAlarmID(enAxisResult.MoveTimeout);
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 40:
                        {
                            // HJYOU  - 혹시나 모르지 .... Safty Check를 새로 하자.
                            if (safty_check)
                            {
                                bool isSafe = true;
                                isSafe &= InterlockManager.IsSafe(m_Axis, pos);
                                if (isSafe)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Safty Check OK[Target={2}]", m_Servo.ServoName, m_Axis.AxisName, pos));
                                    seqNo = 10;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > m_Axis.InposWaitTime)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.IntrError);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Safty Check NG[{2},Target={3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, pos));
                                    rv = m_Device.GetAlarmID(enAxisResult.IntrError);
                                    seqNo = 0;
                                }
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Axis.InposWaitTime)
                            {
                                seqNo = 10;
                            }
                        }
                        break;

                    case 50:
                        {
                            //Axis Stop
                            enAxisResult stop_start = m_Servo.Stop(m_Axis);
                            if (stop_start == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Start", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 60;
                            }
                            else if (stop_start > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), stop_start);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Start Alarm[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(stop_start);
                                m_RetryCmdError = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 60:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 100) break; // Command이 완료되는 시간 대기

                            //Axis Done
                            enAxisResult stop_end = m_Servo.MotionDoneAxis(m_Axis);
                            if (stop_end == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop OK", m_Servo.ServoName, m_Axis.AxisName));
                                seqNo = 70;
                            }
                            else if (stop_end > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), stop_end);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Error[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(stop_end);
                                m_RetryCmdError = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.StopTimeout);
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                rv = m_Device.GetAlarmID(enAxisResult.StopTimeout);
                                m_RetryCmdError = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 70:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 500)
                            {
                                m_RetryCmdError++;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 100:
                        {
                            //Axis Alarm Clear
                            int rv1 = m_Device.AlarmClear();
                            if (rv1 == 0)
                            {
                                m_RetryReady++;
                                seqNo = 10;
                            }
                            else if (rv1 > 0)
                            {
                                rv = rv1;
                                seqNo = 0;
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return rv;
            }
            /// <summary>
            /// moving : 이동중 신규명령을 내릴 경우
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="set"></param>
            /// <param name="safty_check"></param>
            /// <param name="moving"></param>
            /// <returns></returns>
            public int Do_Continuous(double pos, VelSet set, bool safty_check = true)
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_Axis.CommandSkip)
                            {
                                rv = 0;
                            }
                            else
                            {
                                // Move Skip 판단
                                // CurPosition == Teaching Position 이면 굳이 이동하지 말자
                                bool move_skip = true;
                                double target_pos = pos;
                                double cur_pos = (m_Axis as IAxisCommand).GetAxisCurPos();
                                double in_range = m_Axis.InRangeValue;
                                move_skip &= (Math.Abs(target_pos - cur_pos) < in_range) ? true : false;
                                if (move_skip)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Position Move Skip[Target:{2},Cur:{3}]", m_Servo.ServoName, m_Axis.AxisName, target_pos, cur_pos));
                                    rv = 0;
                                    seqNo = 0;
                                }
                                else
                                {
                                    m_RetryReady = 0;
                                    m_RetryCmdError = 0;
                                    m_Servo.SetTargetPosition(m_Axis, pos, set);
                                    seqNo = 5;
                                }
                            }
                        }
                        break;

                    case 5:
                        {
                            // safty check
                            if (safty_check)
                            {
                                bool isSafe = true;
                                isSafe &= InterlockManager.IsSafe_ContinuousMove(m_Axis, pos);
                                if (isSafe)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Safty Check OK[Target={2}]", m_Servo.ServoName, m_Axis.AxisName, pos));
                                    StartTicks = XFunc.GetTickCount();
                                    m_RetryCount = 0;

                                    seqNo = 10;
                                }
                                else
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.IntrError);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Safty Check NG[{2},Target={3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, pos));
                                    rv = m_Device.GetAlarmID(enAxisResult.IntrError);
                                    seqNo = 0;
                                }
                            }
                            else
                            {
                                m_RetryCount = 0;
                                seqNo = 10;
                            }
                        }
                        break;

                    case 10:
                        {
                            //이때는 무조건 Ready 상태여야 한다.
                            if (m_Device.IsAxisContinuousReady(false))
                            {
                                //Axis Move
                                if (set.Vel > m_Axis.SpeedLimit) set.Vel = m_Axis.SpeedLimit;
                                enAxisResult move_start = m_Servo.ContinuousMoveAxisStart(m_Axis, pos, set);
                                if (move_start == enAxisResult.Success)
                                {
                                    double cur_pos = (m_Axis as IAxisCommand).GetAxisCurPos();
                                    double end_pos = pos;
                                    double speed = set.Vel;
                                    m_SetTimeout = (int)(Math.Abs(end_pos - cur_pos) / speed + m_Axis.SetTimeoutMargin / 1000.0f);

                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Start _ SetTime={2}", m_Servo.ServoName, m_Axis.AxisName, m_SetTimeout));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 20;
                                }
                                else if (move_start > enAxisResult.Success)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), move_start);

                                    List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                    List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                    if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                    string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Start Alarm[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                    rv = m_Device.GetAlarmID(move_start);
                                    seqNo = 0;
                                }
                            }
                            else
                            {
                                enAxisInFlag status = (m_Axis as IAxisCommand).GetAxisCurStatus();
                                bool recover_alarm = false;
                                recover_alarm |= ((status & enAxisInFlag.Alarm) == enAxisInFlag.Alarm);
                                recover_alarm |= ((status & enAxisInFlag.OverrideAbnormalStop) == enAxisInFlag.OverrideAbnormalStop);
                                recover_alarm |= ((status & enAxisInFlag.InRange_Error) == enAxisInFlag.InRange_Error);
                                recover_alarm &= (status & enAxisInFlag.SvOn) == enAxisInFlag.SvOn;
                                if (recover_alarm && m_RetryReady < 5)
                                {
                                    if (((status & enAxisInFlag.Alarm) == enAxisInFlag.Alarm))
                                        ServoLog.WriteLog(string.Format("{0}.{1} Axis Servo Alarm", m_Servo.ServoName, m_Axis.AxisName));
                                    else if ((status & enAxisInFlag.OverrideAbnormalStop) == enAxisInFlag.OverrideAbnormalStop)
                                        ServoLog.WriteLog(string.Format("{0}.{1} Axis Override Abnormal Stop", m_Servo.ServoName, m_Axis.AxisName));
                                    else if ((status & enAxisInFlag.InRange_Error) == enAxisInFlag.InRange_Error)
                                        ServoLog.WriteLog(string.Format("{0}.{1} Axis In Range Error", m_Servo.ServoName, m_Axis.AxisName));
                                    ServoLog.WriteLog(string.Format("{0}.{1} Alarm Status, Try Alarm Clear", m_Servo.ServoName, m_Axis.AxisName));
                                    seqNo = 100;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > m_Axis.InposWaitTime)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.NotReadyError);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                    rv = m_Device.GetAlarmID(enAxisResult.NotReadyError);
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 20:
                        {
                            bool timer_over = XFunc.GetTickCount() - StartTicks > 100;

                            if (timer_over)
                            {
                                //Axis Done
                                enAxisResult move_end = m_Servo.MotionDoneAxis(m_Axis);
                                if (move_end == enAxisResult.Success)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move OK", m_Servo.ServoName, m_Axis.AxisName));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 30;
                                }
                                else if (move_end == enAxisResult.CmdError && m_RetryCmdError < 5)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Command Set Abnormal. Retry !", m_Servo.ServoName, m_Axis.AxisName));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 50;
                                }
                                else if (move_end > enAxisResult.Success)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), move_end);

                                    List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                    List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                    if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                    string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Error[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                    rv = m_Device.GetAlarmID(move_end);
                                    seqNo = 0;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > m_SetTimeout * 1000)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.MoveTimeout);
                                    rv = m_Device.GetAlarmID(enAxisResult.MoveTimeout);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name, rv));
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 30:
                        {
                            //Position Check
                            bool pos_check = true;
                            double target_pos = pos;
                            double cur_torque = (m_Axis as IAxisCommand).GetAxisCurTorque();
                            double cur_pos = (m_Axis as IAxisCommand).GetAxisCurPos();
                            double in_range = m_Axis.InRangeValue;
                            pos_check &= (Math.Abs(target_pos - cur_pos) < in_range) ? true : false;
                            if (pos_check || m_Axis.CommandSkip)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Position Check[Target:{2},Cur:{3}]", m_Servo.ServoName, m_Axis.AxisName, target_pos, cur_pos));
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Axis.InposWaitTime)
                            {
                                if (m_RetryCount < 5)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Position Check({2})[Target:{3},Cur:{4}]", m_Servo.ServoName, m_Axis.AxisName, m_RetryCount, target_pos, cur_pos));
                                    m_RetryCount++;
                                    StartTicks = XFunc.GetTickCount();
                                    if (Math.Abs(target_pos - cur_pos) > in_range) seqNo = 40;
                                    else seqNo = 10;
                                }
                                else
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.MoveTimeout);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                    rv = m_Device.GetAlarmID(enAxisResult.MoveTimeout);
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 40:
                        {
                            // HJYOU  - 혹시나 모르지 .... Safty Check를 새로 하자.
                            if (safty_check)
                            {
                                bool isSafe = true;
                                isSafe &= InterlockManager.IsSafe(m_Axis, pos);
                                if (isSafe)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Safty Check OK[Target={2}]", m_Servo.ServoName, m_Axis.AxisName, pos));
                                    seqNo = 10;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > m_Axis.InposWaitTime)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.IntrError);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Safty Check NG[{2},Target={3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, pos));
                                    rv = m_Device.GetAlarmID(enAxisResult.IntrError);
                                    seqNo = 0;
                                }
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Axis.InposWaitTime)
                            {
                                seqNo = 10;
                            }
                        }
                        break;

                    case 50:
                        {
                            //Axis Stop
                            enAxisResult stop_start = m_Servo.Stop(m_Axis);
                            if (stop_start == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Start", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 60;
                            }
                            else if (stop_start > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), stop_start);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Start Alarm[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(stop_start);
                                m_RetryCmdError = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 60:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 100) break; // Command이 완료되는 시간 대기

                            //Axis Done
                            enAxisResult stop_end = m_Servo.MotionDoneAxis(m_Axis);
                            if (stop_end == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop OK", m_Servo.ServoName, m_Axis.AxisName));
                                seqNo = 70;
                            }
                            else if (stop_end > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), stop_end);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Error[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(stop_end);
                                m_RetryCmdError = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.StopTimeout);
                                ServoLog.WriteLog(string.Format("{0}.{1} Stop Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                rv = m_Device.GetAlarmID(enAxisResult.StopTimeout);
                                m_RetryCmdError = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 70:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 500)
                            {
                                m_RetryCmdError++;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 100:
                        {
                            //Axis Alarm Clear
                            int rv1 = m_Device.AlarmClear();
                            if (rv1 == 0)
                            {
                                m_RetryReady++;
                                seqNo = 10;
                            }
                            else if (rv1 > 0)
                            {
                                rv = rv1;
                                seqNo = 0;
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return rv;
            }
            #endregion
        }

        private class SeqRelativeMove : XSeqFunc
        {
            #region Fields
            _DevAxis m_Device = null;
            _Axis m_Axis = null;
            ServoUnit m_Servo = null;

            private double m_TargetPos = 0.0f;
            private int m_RetryCount = 0;
            private int nRetry = 0; // CmdError Retry
            #endregion

            #region Constructor
            public SeqRelativeMove(_DevAxis device)
            {
                this.SeqName = $"SeqRelativeMove{device.MyName}";
                m_Device = device;

                m_Servo = m_Device.GetServoUnit();
                m_Axis = m_Device.GetAxis();
            }

            public override void SeqAbort()
            {
                nRetry = 0;
                this.InitSeq();
            }
            #endregion

            #region Methods
            public int Do(double offset, VelSet set, bool safty_check)
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_Axis.CommandSkip)
                            {
                                rv = 0;
                            }
                            else
                            {
                                // safty check
                                if (safty_check)
                                {
                                    bool isSafe = true;
                                    m_TargetPos = (m_Axis as IAxisCommand).GetAxisCurPos() + offset;
                                    isSafe &= InterlockManager.IsSafe(m_Axis, m_TargetPos);
                                    if (isSafe)
                                    {
                                        ServoLog.WriteLog(string.Format("{0}.{1} Safty Check OK[Target={2}]", m_Servo.ServoName, m_Axis.AxisName, m_TargetPos));
                                        nRetry = 0;
                                        m_RetryCount = 0;
                                        seqNo = 10;
                                    }
                                    else
                                    {
                                        string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.IntrError);
                                        ServoLog.WriteLog(string.Format("{0}.{1} Safty Check NG[{2},Target={3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, m_TargetPos));
                                        rv = m_Device.GetAlarmID(enAxisResult.IntrError);
                                        seqNo = 0;
                                    }
                                }
                                else
                                {
                                    nRetry = 0;
                                    m_RetryCount = 0;
                                    seqNo = 10;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            //Axis Move
                            if (set.Vel > m_Axis.SpeedLimit) set.Vel = m_Axis.SpeedLimit;
                            enAxisResult move_start = m_Servo.MoveRelativeStart(m_Axis, offset, set);
                            if (move_start == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Relative Move Start", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else if (move_start > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), move_start);

                                List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                ServoLog.WriteLog(string.Format("{0}.{1} Relative Move Start Alarm[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                rv = m_Device.GetAlarmID(move_start);
                                seqNo = 0;
                            }
                        }
                        break;

                    case 20:
                        {
                            bool timer_over = XFunc.GetTickCount() - StartTicks > 100 ? true : false;

                            if (timer_over)
                            {
                                //Axis Done
                                enAxisResult move_end = m_Servo.MotionDoneAxis(m_Axis);
                                if (move_end == enAxisResult.Success)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Relative Move OK", m_Servo.ServoName, m_Axis.AxisName));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 30;
                                }
                                else if (move_end == enAxisResult.CmdError && nRetry < 5)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Command Set Abnormal. Retry !", m_Servo.ServoName, m_Axis.AxisName));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 60;
                                }
                                else if (move_end > enAxisResult.Success)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), move_end);

                                    List<int> con_list = (m_Axis as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                    List<int> dri_list = (m_Axis as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                    if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                    string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Axis as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                    ServoLog.WriteLog(string.Format("{0}.{1} Relative Move Error[code={2}][{3}]", m_Servo.ServoName, m_Axis.AxisName, code_name, axis_alarm_code));
                                    rv = m_Device.GetAlarmID(move_end);
                                    seqNo = 0;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > 300 * 1000)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.MoveTimeout);
                                    rv = m_Device.GetAlarmID(enAxisResult.MoveTimeout);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Relative Move Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name, rv));
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 30:
                        {
                            //Position Check
                            bool pos_check = true;
                            double target_pos = m_TargetPos;
                            double cur_torque = (m_Axis as IAxisCommand).GetAxisCurTorque();
                            double cur_pos = (m_Axis as IAxisCommand).GetAxisCurPos();
                            double in_range = m_Axis.InRangeValue;
                            pos_check &= (Math.Abs(target_pos - cur_pos) < in_range) ? true : false;

                            if (pos_check || m_Axis.CommandSkip)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Position Check[Target:{2},Cur:{3}]", m_Servo.ServoName, m_Axis.AxisName, target_pos, cur_pos));
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Axis.InposWaitTime)
                            {
                                if (m_RetryCount < 5)
                                {
                                    ServoLog.WriteLog(string.Format("{0}.{1} Position Check({2})[Target:{3},Cur:{4}]", m_Servo.ServoName, m_Axis.AxisName, m_RetryCount, target_pos, cur_pos));
                                    m_RetryCount++;
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 40;
                                }
                                else
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), enAxisResult.MoveTimeout);
                                    ServoLog.WriteLog(string.Format("{0}.{1} Move Timeout Error[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                    rv = m_Device.GetAlarmID(enAxisResult.MoveTimeout);
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 40:
                        {
                            if (XFunc.GetTickCount() - StartTicks > m_Axis.InposWaitTime)
                            {
                                seqNo = 50;
                            }
                        }
                        break;

                    case 50:
                        {
                            //Axis Move
                            if (set.Vel > m_Axis.SpeedLimit) set.Vel = m_Axis.SpeedLimit;
                            enAxisResult move_start = m_Servo.MoveAxisStart(m_Axis, m_TargetPos, set);
                            if (move_start == enAxisResult.Success)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} Move Start", m_Servo.ServoName, m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else if (move_start > enAxisResult.Success)
                            {
                                string code_name = Enum.GetName(typeof(enAxisResult), move_start);
                                ServoLog.WriteLog(string.Format("{0}.{1} Move Start Alarm[code={2}]", m_Servo.ServoName, m_Axis.AxisName, code_name));
                                rv = m_Device.GetAlarmID(move_start);
                                seqNo = 0;
                            }
                        }
                        break;

                    case 60:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                nRetry++;
                                seqNo = 10;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            #endregion
        }

        #endregion

    }
}
