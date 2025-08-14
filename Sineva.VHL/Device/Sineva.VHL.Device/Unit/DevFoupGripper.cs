using Sineva.VHL.Data;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Library.MXP;
using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Sineva.VHL.Device
{
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class DevFoupGripper : _Device
    {
        private const string DevName = "DevFoupGripper";

        #region Fields
        private DevAxisTag m_AxisHoist = new DevAxisTag();
        private DevAxisTag m_AxisTurn = new DevAxisTag();
        private DevAxisTag m_AxisSlide = new DevAxisTag();
        private _DevOutput m_DoHoistBrake = new _DevOutput();
        private _DevInput m_DiHoistBrake = new _DevInput();
        private _DevInput m_DiHoistSwing = new _DevInput();

        private _DevInput m_DiLeftStbDoubleCheck = new _DevInput();
        private _DevInput m_DiRightStbDoubleCheck = new _DevInput();
        private _DevInput m_DiFoupCapCheck = new _DevInput();
        private _DevInput m_DiFoupOpenCheck = new _DevInput();

        #region Acquire Deposit Hoist Offset
        public double m_Offset_HoistHomeSensorDetect = 0.0f;  //자꾸만 Hoist 높이가 달라지기 때문에 Home Sensor를 감지한 위치를 저장하여 반송할때 사용한다.
        #endregion

        private TeachingData m_TeachingPointWait = new TeachingData();
        private TeachingData m_TeachingPointAcquireBufUp = new TeachingData();
        private TeachingData m_TeachingPointAcquireBufDown = new TeachingData();
        private TeachingData m_TeachingPointAcquireBufGrip = new TeachingData();
        private TeachingData m_TeachingPointAcquirePortUp = new TeachingData();
        private TeachingData m_TeachingPointAcquirePortDown = new TeachingData();
        private TeachingData m_TeachingPointAcquirePortGrip = new TeachingData();
        private TeachingData m_TeachingPointAcquireStkUp = new TeachingData();
        private TeachingData m_TeachingPointAcquireStkDown = new TeachingData();
        private TeachingData m_TeachingPointAcquireStkGrip = new TeachingData();
        private TeachingData m_TeachingPointDepositBufUp = new TeachingData();
        private TeachingData m_TeachingPointDepositBufDown = new TeachingData();
        private TeachingData m_TeachingPointDepositBufGrip = new TeachingData();
        private TeachingData m_TeachingPointDepositPortUp = new TeachingData();
        private TeachingData m_TeachingPointDepositPortDown = new TeachingData();
        private TeachingData m_TeachingPointDepositPortGrip = new TeachingData();
        private TeachingData m_TeachingPointDepositStkUp = new TeachingData();
        private TeachingData m_TeachingPointDepositStkDown = new TeachingData();
        private TeachingData m_TeachingPointDepositStkGrip = new TeachingData();

        private VelocityData m_TeachingVelocityLow = null;
        private VelocityData m_TeachingVelocityMid = null;
        private VelocityData m_TeachingVelocityHigh = null;
        private VelocityData m_TeachingVelocityUp = null;
        private VelocityData m_TeachingVelocityDown = null;
        private VelocityData m_TeachingVelocityBufUp = null;
        private VelocityData m_TeachingVelocityBufDown = null;

        private AlarmData m_ALM_NotDefine = null;
        private AlarmData m_ALM_SettingError = null;
        private AlarmData m_ALM_SlideHomeConditionError = null;
        private AlarmData m_ALM_SlideMoveInterlockError = null;
        private AlarmData m_ALM_HoistHomeConditionError = null;
        private AlarmData m_ALM_HoistMoveInterlockError = null;
        private AlarmData m_ALM_HoistBrakeReleaseError = null;
        private AlarmData m_ALM_HoistSwingSensorError = null;
        private AlarmData m_ALM_LeftDoubleStorageError = null;
        private AlarmData m_ALM_RightDoubleStorageError = null;
        private AlarmData m_ALM_FoupCapNotDetectError = null;
        private AlarmData m_ALM_FoupCapOpenError = null;

        private double m_HoistWorkingDistance = 0.0f;
        private double m_SlideWorkingDistance = 0.0f;
        private double m_RotateWorkingDistance = 0.0f;

        private SeqAction m_SeqAction = null;
        private SeqMonitor m_SeqMonitor = null;

        private bool m_CommandBusy = false;
        private bool m_CommandMoveWaitPosition = false;
        #endregion

        #region Properties
        [Category("!Setting Device (Axis)"), Description("Vehicle Hoist Motor"), DeviceSetting(true)]
        public DevAxisTag AxisHoist { get { return m_AxisHoist; } set { m_AxisHoist = value; } }
        [Category("!Setting Device (Axis)"), Description("Vehicle Turn Motor"), DeviceSetting(true)]
        public DevAxisTag AxisTurn { get { return m_AxisTurn; } set { m_AxisTurn = value; } }
        [Category("!Setting Device (Axis)"), Description("Vehicle Slide Motor"), DeviceSetting(true)]
        public DevAxisTag AxisSlide { get { return m_AxisSlide; } set { m_AxisSlide = value; } }
        [Category("I/O Setting (Digital Output)"), Description("Hoist Motor Brake"), DeviceSetting(true)]
        public _DevOutput DoHoistBrake { get { return m_DoHoistBrake; } set { m_DoHoistBrake = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Hoist Motor Brake Release"), DeviceSetting(true)]
        public _DevInput DiHoistBrake { get { return m_DiHoistBrake; } set { m_DiHoistBrake = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Hoist Swing"), DeviceSetting(true)]
        public _DevInput DiHoistSwing { get { return m_DiHoistSwing; } set { m_DiHoistSwing = value; } }

        [Category("I/O Setting (Digital Input)"), Description("Left STB Doubel Check"), DeviceSetting(true)]
        public _DevInput DiLeftStbDoubleCheck { get { return m_DiLeftStbDoubleCheck; } set { m_DiLeftStbDoubleCheck = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Right STB Doubel Check"), DeviceSetting(true)]
        public _DevInput DiRightStbDoubleCheck { get { return m_DiRightStbDoubleCheck; } set { m_DiRightStbDoubleCheck = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Foup Cap Check"), DeviceSetting(true)]
        public _DevInput DiFoupCapCheck { get { return m_DiFoupCapCheck; } set { m_DiFoupCapCheck = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Foup Open Check"), DeviceSetting(true)]
        public _DevInput DiFoupOpenCheck { get { return m_DiFoupOpenCheck; } set { m_DiFoupOpenCheck = value; } }


        [Category("Teaching"), Description("Wait Point"), DeviceSetting(true)]
        public TeachingData TeachingPointWait
        {
            get { return m_TeachingPointWait; } set { m_TeachingPointWait = value; }
        }
        [Category("Teaching"), Description("Acquire Buffer UP Point"), DeviceSetting(true)]
        public TeachingData TeachingPointAcquireBufUp
        {
            get { return m_TeachingPointAcquireBufUp; } set { m_TeachingPointAcquireBufUp = value; }
        }
        [Category("Teaching"), Description("Acquire Buffer Down Point"), DeviceSetting(true)]
        public TeachingData TeachingPointAcquireBufDown
        {
            get { return m_TeachingPointAcquireBufDown; } set { m_TeachingPointAcquireBufDown = value; }
        }
        [Category("Teaching"), Description("Acquire Buffer Grip Point"), DeviceSetting(true)]
        public TeachingData TeachingPointAcquireBufGrip
        {
            get { return m_TeachingPointAcquireBufGrip; } set { m_TeachingPointAcquireBufGrip = value; }
        }
        [Category("Teaching"), Description("Acquire Port UP Point"), DeviceSetting(true)]
        public TeachingData TeachingPointAcquirePortUp
        {
            get { return m_TeachingPointAcquirePortUp; } set { m_TeachingPointAcquirePortUp = value; }
        }
        [Category("Teaching"), Description("Acquire Port Down Point"), DeviceSetting(true)]
        public TeachingData TeachingPointAcquirePortDown
        {
            get { return m_TeachingPointAcquirePortDown; } set { m_TeachingPointAcquirePortDown = value; }
        }
        [Category("Teaching"), Description("Acquire Port Grip Point"), DeviceSetting(true)]
        public TeachingData TeachingPointAcquirePortGrip
        {
            get { return m_TeachingPointAcquirePortGrip; } set { m_TeachingPointAcquirePortGrip = value; }
        }
        [Category("Teaching"), Description("Acquire STK UP Point"), DeviceSetting(true)]
        public TeachingData TeachingPointAcquireStkUp
        {
            get { return m_TeachingPointAcquireStkUp; } set { m_TeachingPointAcquireStkUp = value; }
        }
        [Category("Teaching"), Description("Acquire STK Down Point"), DeviceSetting(true)]
        public TeachingData TeachingPointAcquireStkDown
        {
            get { return m_TeachingPointAcquireStkDown; } set { m_TeachingPointAcquireStkDown = value; }
        }
        [Category("Teaching"), Description("Acquire STK Grip Point"), DeviceSetting(true)]
        public TeachingData TeachingPointAcquireStkGrip
        {
            get { return m_TeachingPointAcquireStkGrip; } set { m_TeachingPointAcquireStkGrip = value; }
        }
        [Category("Teaching"), Description("Deposit Buffer UP Point"), DeviceSetting(true)]
        public TeachingData TeachingPointDepositBufUp
        {
            get { return m_TeachingPointDepositBufUp; }
            set { m_TeachingPointDepositBufUp = value; }
        }
        [Category("Teaching"), Description("Deposit Buffer Down Point"), DeviceSetting(true)]
        public TeachingData TeachingPointDepositBufDown
        {
            get { return m_TeachingPointDepositBufDown; }
            set { m_TeachingPointDepositBufDown = value; }
        }
        [Category("Teaching"), Description("Deposit Buffer Grip Point"), DeviceSetting(true)]
        public TeachingData TeachingPointDepositBufGrip
        {
            get { return m_TeachingPointDepositBufGrip; }
            set { m_TeachingPointDepositBufGrip = value; }
        }
        [Category("Teaching"), Description("Deposit Port UP Point"), DeviceSetting(true)]
        public TeachingData TeachingPointDepositPortUp
        {
            get { return m_TeachingPointDepositPortUp; }
            set { m_TeachingPointDepositPortUp = value; }
        }
        [Category("Teaching"), Description("Deposit Port Down Point"), DeviceSetting(true)]
        public TeachingData TeachingPointDepositPortDown
        {
            get { return m_TeachingPointDepositPortDown; }
            set { m_TeachingPointDepositPortDown = value; }
        }
        [Category("Teaching"), Description("Deposit Port Grip Point"), DeviceSetting(true)]
        public TeachingData TeachingPointDepositPortGrip
        {
            get { return m_TeachingPointDepositPortGrip; }
            set { m_TeachingPointDepositPortGrip = value; }
        }
        [Category("Teaching"), Description("Deposit STK UP Point"), DeviceSetting(true)]
        public TeachingData TeachingPointDepositStkUp
        {
            get { return m_TeachingPointDepositStkUp; }
            set { m_TeachingPointDepositStkUp = value; }
        }
        [Category("Teaching"), Description("Deposit STK Down Point"), DeviceSetting(true)]
        public TeachingData TeachingPointDepositStkDown
        {
            get { return m_TeachingPointDepositStkDown; }
            set { m_TeachingPointDepositStkDown = value; }
        }
        [Category("Teaching"), Description("Deposit STK Grip Point"), DeviceSetting(true)]
        public TeachingData TeachingPointDepositStkGrip
        {
            get { return m_TeachingPointDepositStkGrip; }
            set { m_TeachingPointDepositStkGrip = value; }
        }


        [Category("Velocity"), Description("LOW Prop"), DeviceSetting(true)]
        public VelocityData TeachingVelocityLow
        {
            get { return m_TeachingVelocityLow; } set { m_TeachingVelocityLow = value; }
        }
        [Category("Velocity"), Description("MID Prop"), DeviceSetting(true)]
        public VelocityData TeachingVelocityMid
        {
            get { return m_TeachingVelocityMid; } set { m_TeachingVelocityMid = value; }
        }
        [Category("Velocity"), Description("HIGH Prop"), DeviceSetting(true)]
        public VelocityData TeachingVelocityHigh
        {
            get { return m_TeachingVelocityHigh; } set { m_TeachingVelocityHigh = value; }
        }
        [Category("Velocity"), Description("UP Prop"), DeviceSetting(true)]
        public VelocityData TeachingVelocityUp
        {
            get { return m_TeachingVelocityUp; } set { m_TeachingVelocityUp = value; }
        }
        [Category("Velocity"), Description("DOWN Prop"), DeviceSetting(true)]
        public VelocityData TeachingVelocityDown
        {
            get { return m_TeachingVelocityDown; } set { m_TeachingVelocityDown = value; }
        }
        [Category("Velocity"), Description("Buf UP Prop"), DeviceSetting(true)]
        public VelocityData TeachingVelocityBufUp
        {
            get { return m_TeachingVelocityBufUp; } set { m_TeachingVelocityBufUp = value; }
        }
        [Category("Velocity"), Description("Foup Exist DOWN Prop"), DeviceSetting(true)]
        public VelocityData TeachingVelocityBufDown
        {
            get { return m_TeachingVelocityBufDown; } set { m_TeachingVelocityBufDown = value; }
        }
        [Category("OFFSET"), Description("Hoist Home Sensor Detect Offset"), DeviceSetting(true)]
        public double Offset_HoistHomeSensorDetect
        {
            get { return m_Offset_HoistHomeSensorDetect; } set { m_Offset_HoistHomeSensorDetect = value; }
        }
        [Category("!LifeTime Manager"), DisplayName("Hoist Working Distance"), Description("Device Life Hoist Working Distance"), DeviceSetting(false, true)]
        public double HoistWorkingDistance
        {
            get { return m_HoistWorkingDistance; } set { SaveCurState = m_HoistWorkingDistance != value; m_HoistWorkingDistance = value; } 
        }
        [Category("!LifeTime Manager"), DisplayName("Slide Working Distance"), Description("Device Life Slide Working Distance"), DeviceSetting(false, true)]
        public double SlideWorkingDistance
        {
            get { return m_SlideWorkingDistance; }
            set { SaveCurState = m_SlideWorkingDistance != value; m_SlideWorkingDistance = value; }
        }
        [Category("!LifeTime Manager"), DisplayName("Rotate Working Distance"), Description("Rotate Life Slide Working Distance"), DeviceSetting(false, true)]
        public double RotateWorkingDistance
        {
            get { return m_RotateWorkingDistance; }
            set { SaveCurState = m_RotateWorkingDistance != value; m_RotateWorkingDistance = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_NotDefine
        {
            get { return m_ALM_NotDefine; }
            set { m_ALM_NotDefine = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_SettingError
        {
            get { return m_ALM_SettingError; }
            set { m_ALM_SettingError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_SlideHomeConditionError
        {
            get { return m_ALM_SlideHomeConditionError; }
            set { m_ALM_SlideHomeConditionError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_SlideMoveInterlockError
        {
            get { return m_ALM_SlideMoveInterlockError; }
            set { m_ALM_SlideMoveInterlockError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_HoistHomeConditionError
        {
            get { return m_ALM_HoistHomeConditionError; }
            set { m_ALM_HoistHomeConditionError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_HoistMoveInterlockError
        {
            get { return m_ALM_HoistMoveInterlockError; }
            set { m_ALM_HoistMoveInterlockError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_HoistBrakeReleaseError
        {
            get { return m_ALM_HoistBrakeReleaseError; }
            set { m_ALM_HoistBrakeReleaseError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_HoistSwingSensorError
        {
            get { return m_ALM_HoistSwingSensorError; }
            set { m_ALM_HoistSwingSensorError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_LeftDoubleStorageError
        {
            get { return m_ALM_LeftDoubleStorageError; }
            set { m_ALM_LeftDoubleStorageError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_RightDoubleStorageError
        {
            get { return m_ALM_RightDoubleStorageError; }
            set { m_ALM_RightDoubleStorageError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_FoupCapNotDetectError
        {
            get { return m_ALM_FoupCapNotDetectError; }
            set { m_ALM_FoupCapNotDetectError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_FoupCapOpenError
        {
            get { return m_ALM_FoupCapOpenError; }
            set { m_ALM_FoupCapOpenError = value; }
        }
       #endregion

        #region Constructor
        public DevFoupGripper()
        {
            if (!Initialized)
            {
                this.MyName = DevName;
            }
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
            if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis() != null ? true : false;
            if (m_AxisTurn.IsValid) ok &= m_AxisTurn.GetDevAxis() != null ? true : false;
            if (m_AxisSlide.IsValid) ok &= m_AxisSlide.GetDevAxis() != null ? true : false;
            if (m_DoHoistBrake.IsValid) ok &= m_DoHoistBrake.Initialize(this.ToString(), false, false);
            if (m_DiHoistBrake.IsValid) ok &= m_DiHoistBrake.Initialize(this.ToString(), false, false);
            if (m_DiHoistSwing.IsValid) ok &= m_DiHoistSwing.Initialize(this.ToString(), false, false);
            if (m_DiLeftStbDoubleCheck.IsValid) ok &= m_DiLeftStbDoubleCheck.Initialize(this.ToString(), false, false) ? true : false;
            if (m_DiRightStbDoubleCheck.IsValid) ok &= m_DiRightStbDoubleCheck.Initialize(this.ToString(), false, false) ? true : false;
            if (m_DiFoupCapCheck.IsValid) ok &= m_DiFoupCapCheck.Initialize(this.ToString(), false, false) ? true : false;
            if (m_DiFoupOpenCheck.IsValid) ok &= m_DiFoupOpenCheck.Initialize(this.ToString(), false, false) ? true : false;

            if (m_AxisHoist.GetDevAxis() != null)
            {
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointAcquireBufUp);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointAcquireBufDown);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointAcquireBufGrip);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointAcquirePortUp);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointAcquirePortDown);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointAcquirePortGrip);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointAcquireStkUp);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointAcquireStkDown);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointAcquireStkGrip);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointDepositBufUp);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointDepositBufDown);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointDepositBufGrip);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointDepositPortUp);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointDepositPortDown);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointDepositPortGrip);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointDepositStkUp);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointDepositStkDown);
                if (m_AxisHoist.IsValid) ok &= m_AxisHoist.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointDepositStkGrip);
            }
            if (m_AxisHoist.IsValid) ok &= m_TeachingVelocityLow != null;
            if (m_AxisHoist.IsValid) ok &= m_TeachingVelocityMid != null;
            if (m_AxisHoist.IsValid) ok &= m_TeachingVelocityHigh != null;
            if (m_AxisHoist.IsValid) ok &= m_TeachingVelocityUp != null;
            if (m_AxisHoist.IsValid) ok &= m_TeachingVelocityDown != null;
            if (m_AxisHoist.IsValid) ok &= m_TeachingVelocityBufUp != null;
            if (m_AxisHoist.IsValid) ok &= m_TeachingVelocityBufDown != null;

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
            //if(Condition) AlarmConditionable = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            m_ALM_NotDefine = AlarmListProvider.Instance.NewAlarm(AlarmCode.ParameterControlError, true, MyName, ParentName, "Device Not Define Alarm");
            m_ALM_SettingError = AlarmListProvider.Instance.NewAlarm(AlarmCode.ParameterControlError, true, MyName, ParentName, "Parameter Device Setting Alarm");
            m_ALM_SlideHomeConditionError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, "Interlock", "Slide Home Condition Alarm");
            m_ALM_SlideMoveInterlockError = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Interlock", "Slide Move Interlock Alarm");
            m_ALM_HoistHomeConditionError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, "Interlock", "Hoist Home Condition Alarm");
            m_ALM_HoistMoveInterlockError = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Interlock", "Hoist Move Interlock Alarm");
            m_ALM_HoistBrakeReleaseError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, "Interlock", "Hoist Brake Release Alarm");
            m_ALM_HoistSwingSensorError = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Interlock", "Hoist Swing Sensor Alarm");
            m_ALM_LeftDoubleStorageError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, "Interlock", "Left Double Storage Alarm");
            m_ALM_RightDoubleStorageError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, "Interlock", "Right Double Storage Alarm");
            m_ALM_FoupCapNotDetectError = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Interlock", "Foup Cap Not Detect Alarm");
            m_ALM_FoupCapOpenError = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Interlock", "Foup Cover Open Alarm");
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Hoist Working Distance", this, "GetWorkingDistance", 1000, 0));
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Slide Working Distance", this, "GetWorkingDistance", 1000, 1));
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Rotate Working Distance", this, "GetWorkingDistance", 1000, 2));
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            if (ok)
            {
                m_SeqAction = new SeqAction(this);
                m_SeqMonitor = new SeqMonitor(this);
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
        public override void SeqAbort()
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return;
            if (!Initialized) return;

            m_SeqAction.SeqAbort();

            m_AxisHoist.GetDevAxis().SeqAbort();
            m_AxisTurn.GetDevAxis().SeqAbort();
            m_AxisSlide.GetDevAxis().SeqAbort();
            SetBrake(false);
        }
        #endregion

        #region Methods - Life Time
        // 0:Hoist, 1:Slide, 2:Rotate
        public double GetWorkingDistance(int axis)
        {
            double rv = 0.0f;
            if (axis == 0) rv = m_HoistWorkingDistance;
            else if (axis == 1) rv = m_SlideWorkingDistance;
            else if (axis == 2) rv = m_RotateWorkingDistance;
            return rv;
        }
        #endregion

        #region Methods
        public XyztPosition GetTeachingPosition(int propId)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return null;
            if (!Initialized) return null;

            XyztPosition pos = new XyztPosition
            {
                X = 0.0f,
                Y = m_AxisSlide.GetDevAxis().GetTeachingPosition(propId),
                Z = m_AxisHoist.GetDevAxis().GetTeachingPosition(propId),
                T = m_AxisTurn.GetDevAxis().GetTeachingPosition(propId),
            };
            return pos;
        }
        public XyztPosition GetTeachingVelocity(int propId)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return null;
            if (!Initialized) return null;

            XyztPosition pos = new XyztPosition
            {
                X = 0.0f,
                Y = m_AxisSlide.GetDevAxis().GetTeachingVel(propId).Vel,
                Z = m_AxisHoist.GetDevAxis().GetTeachingVel(propId).Vel,
                T = m_AxisTurn.GetDevAxis().GetTeachingVel(propId).Vel,
            };
            return pos;
        }
        public XyztPosition GetTeachingAcceleration(int propId)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return null;
            if (!Initialized) return null;

            XyztPosition pos = new XyztPosition
            {
                X = 0.0f,
                Y = m_AxisSlide.GetDevAxis().GetTeachingVel(propId).Acc,
                Z = m_AxisHoist.GetDevAxis().GetTeachingVel(propId).Acc,
                T = m_AxisTurn.GetDevAxis().GetTeachingVel(propId).Acc,
            };
            return pos;
        }
        public XyztPosition GetTeachingDeceleration(int propId)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return null;
            if (!Initialized) return null;

            XyztPosition pos = new XyztPosition
            {
                X = 0.0f,
                Y = m_AxisSlide.GetDevAxis().GetTeachingVel(propId).Dec,
                Z = m_AxisHoist.GetDevAxis().GetTeachingVel(propId).Dec,
                T = m_AxisTurn.GetDevAxis().GetTeachingVel(propId).Dec,
            };
            return pos;
        }
        public XyztPosition GetTeachingJerk(int propId)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return null;
            if (!Initialized) return null;

            XyztPosition pos = new XyztPosition
            {
                X = 0.0f,
                Y = m_AxisSlide.GetDevAxis().GetTeachingVel(propId).Jerk,
                Z = m_AxisHoist.GetDevAxis().GetTeachingVel(propId).Jerk,
                T = m_AxisTurn.GetDevAxis().GetTeachingVel(propId).Jerk,
            };
            return pos;
        }
        public List<VelSet> GetTeachingVelSets(int propId)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return null;
            if (!Initialized) return null;

            List<VelSet> sets = new List<VelSet>();
            VelSet velSlide = m_AxisSlide.GetDevAxis().GetTeachingVel(propId); sets.Add(velSlide);
            VelSet velHoist = m_AxisHoist.GetDevAxis().GetTeachingVel(propId); sets.Add(velHoist);
            VelSet velRotate = m_AxisTurn.GetDevAxis().GetTeachingVel(propId); sets.Add(velRotate);
            return sets;
        }
        public List<VelSet> GetHoistSensorSearchVelSet(int propId)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return null;
            if (!Initialized) return null;

            try
            {
                List<VelSet> sets = new List<VelSet>();
                VelSet velSlide = m_AxisSlide.GetDevAxis().GetTeachingVel(propId); sets.Add(velSlide);
                VelSet velHoist = new VelSet
                {
                    Vel = SetupManager.Instance.SetupHoist.HoistSensorSearchSpeed,
                    Acc = SetupManager.Instance.SetupHoist.HoistSensorSearchAcc,
                    Dec = SetupManager.Instance.SetupHoist.HoistSensorSearchDec,
                    Jerk = SetupManager.Instance.SetupHoist.HoistSensorSearchJerk,
                };
                sets.Add(velHoist);
                VelSet velRotate = m_AxisTurn.GetDevAxis().GetTeachingVel(propId); sets.Add(velRotate);
                return sets;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return null;
        }

        public XyztPosition GetBeforeDownPos(int portID, bool foupExist)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return null;
            if (!Initialized) return null;

            try
            {
                if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(portID) == false) return null;
                DataItem_Port port = DatabaseHandler.Instance.DictionaryPortDataList[portID];
                double slide_pos = foupExist ? port.SlidePosition : port.UnloadSlidePosition;
                slide_pos += port.SlideOffset;
                double hoist_pos = foupExist ? port.BeforeHoistPosition : port.BeforeUnloadHoistPosition;
                hoist_pos += port.HoistOffset;
                double rotate_pos = foupExist ? port.RotatePosition : port.UnloadRotatePosition;
                rotate_pos += port.RotateOffset;

                XyztPosition pos = new XyztPosition
                {
                    X = 0.0f,
                    Y = slide_pos,
                    Z = hoist_pos,
                    T = rotate_pos,
                };
                return pos;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return null;
        }
        public XyztPosition GetDownPos(int portID, bool foupExist, bool acquire)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return null;
            if (!Initialized) return null;

            try
            {
                if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(portID) == false) return null;

                //////////////////////
                double home_offset = 0.0f;
                //if (SetupManager.Instance.SetupHoist.HoistHomeSensorDetectOffset == Use.Use)
                //{
                //    if (m_Offset_HoistHomeSensorDetect < 0 && m_Offset_HoistHomeSensorDetect > -6)
                //        home_offset = m_Offset_HoistHomeSensorDetect;//흠.. 혹시나 잘못된 값이 들어갈까봐..
                //}
                double move_distance = SetupManager.Instance.SetupHoist.HoistSensorDetectMoveDistance;
                double tracking_offset = TeachingOffsetAdapter.Instance.GetAutoTrackingValue(acquire, portID);
                //////////////////////
                DataItem_Port port = DatabaseHandler.Instance.DictionaryPortDataList[portID];
                double slide_pos = foupExist ? port.SlidePosition : port.UnloadSlidePosition;
                slide_pos += port.SlideOffset;
                double hoist_pos = foupExist ? port.HoistPosition : port.UnloadHoistPosition; //UpSensor Detect Position
                hoist_pos += port.HoistOffset; // 호기별 오차
                hoist_pos += move_distance; // UpSensor 감지 후 이동량
                hoist_pos += tracking_offset; // OHB 별 오차
                hoist_pos += home_offset; // HomeSensor Detect 시점에 따른 오차
                double rotate_pos = foupExist ? port.RotatePosition : port.UnloadRotatePosition;
                rotate_pos += port.RotateOffset;

                XyztPosition pos = new XyztPosition
                {
                    X = 0.0f,
                    Y = slide_pos,
                    Z = hoist_pos,
                    T = rotate_pos,
                };
                return pos;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return null;
        }
        public XyztPosition GetUpPos(int portID, bool foupExist)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return null;
            if (!Initialized) return null;

            if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(portID) == false) return null;
            DataItem_Port port = DatabaseHandler.Instance.DictionaryPortDataList[portID];
            double slide_pos = foupExist ? port.SlidePosition : port.UnloadSlidePosition;
            slide_pos += port.SlideOffset;
            double hoist_pos = 0.0f;
            double rotate_pos = foupExist ? port.RotatePosition : port.UnloadRotatePosition;
            rotate_pos += port.RotateOffset;

            XyztPosition pos = new XyztPosition
            {
                X = 0.0f,
                Y = slide_pos,
                Z = hoist_pos,
                T = rotate_pos,
            };
            return pos;
        }
        public XyztPosition GetTeachingOffset(int portID)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return null;
            if (!Initialized) return null;

            if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(portID) == false) return null;
            DataItem_Port port = DatabaseHandler.Instance.DictionaryPortDataList[portID];

            XyztPosition pos = new XyztPosition
            {
                X = 0.0f,
                Y = port.SlideOffset,
                Z = port.HoistOffset,
                T = port.RotateOffset,
            };
            return pos;
        }
        public bool ResetTeachingOffset(int portID)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return false;
            if (!Initialized) return false;
            if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(portID) == false) return false;

            bool rv = false;
            DataItem_Port port = DatabaseHandler.Instance.DictionaryPortDataList[portID];
            port.DriveLeftOffset = 0.0f;
            port.DriveRightOffset = 0.0f;
            port.SlideOffset = 0.0f;
            port.HoistOffset= 0.0f;
            port.RotateOffset = 0.0f;
            rv = DatabaseHandler.Instance.UpdatePort(port);
            return rv;
        }
        #endregion

        #region Transfer Unit Move Command
        public int Home(enAxisMask mask)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return 0;
            if (!Initialized) return m_ALM_NotDefine.ID;

            int rv = -1;
            rv = m_SeqAction.Home(mask);
            return rv;
        }
        public int Move(enAxisMask mask, ushort point, ushort prop)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return 0;
            if (!Initialized) return m_ALM_NotDefine.ID;

            int rv = -1;
            rv = m_SeqAction.Move(mask, point, prop);
            return rv;
        }
        public int Move(enAxisMask mask, XyztPosition pos, List<VelSet> sets)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return 0;
            if (!Initialized) return m_ALM_NotDefine.ID;

            int rv = -1;
            rv = m_SeqAction.Move(mask, pos, sets);
            return rv;
        }
        public int ContinuousMove(enAxisMask mask, XyztPosition pos, List<VelSet> sets, bool diffPos = false)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return 0;
            if (!Initialized) return m_ALM_NotDefine.ID;

            int rv = -1;
            rv = m_SeqAction.ContinuousMove(mask, pos, sets, diffPos);
            return rv;
        }
        public void SetCommandWaitMove()
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return;
            if (!Initialized) return;

            if (m_CommandBusy) m_SeqAction.SeqAbort();
            m_CommandBusy = false;
            m_CommandMoveWaitPosition = true;
            return;
        }
        #endregion

        #region Methods
        /// <summary>
        /// flg : On(Release), Off(brake)
        /// </summary>
        /// <param name="flag"></param>
        public void SetBrake(bool flag)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return;
            if (!Initialized) return;

            if (m_DiHoistBrake.IsDetected && flag) return;
            m_DoHoistBrake.SetDo(flag);
            if (AppConfig.Instance.Simulation.IO) m_DiHoistBrake.SetDi(flag);
        }
        public bool IsLeftDoubleStorage()
        {
            if (SetupManager.Instance.SetupSafty.STBDoubleCheckUse == Use.NoUse) return false;
            if (!Initialized) return false;

            bool rv = m_DiLeftStbDoubleCheck.IsValid ? m_DiLeftStbDoubleCheck.IsDetected : false;
            return rv;
        }
        public bool IsRightDoubleStorage()
        {
            if (SetupManager.Instance.SetupSafty.STBDoubleCheckUse == Use.NoUse) return false;
            if (!Initialized) return false;

            bool rv = m_DiRightStbDoubleCheck.IsValid ? m_DiRightStbDoubleCheck.IsDetected : false;
            return rv;
        }
        //2024.12.31 Foup Cover Not
        public bool IsFoupCoverDetect()
        {
            //if (SetupManager.Instance.SetupSafty.FoupCoverExistCheckUse == Use.NoUse) return true;
            if (!Initialized) return true;

            bool rv = m_DiFoupCapCheck.IsValid ? m_DiFoupCapCheck.IsDetected : true;
            return rv;
        }
        public bool IsFoupCoverOpen()
        {
            //if (SetupManager.Instance.SetupSafty.FoupCoverOpenCheckUse == Use.NoUse) return false;
            if (!Initialized) return false;

            bool rv = m_DiFoupOpenCheck.IsValid ? m_DiFoupOpenCheck.IsDetected : false;
            return rv;
        }
        #endregion

        #region Sequence
        private class SeqMonitor : XSeqFunc
        {
            #region Field
            private DevFoupGripper m_Device = null;
            private bool[] m_CommandComp = null;
            private List<_DevAxis> m_DevAxes = new List<_DevAxis>();
            #endregion

            public SeqMonitor(DevFoupGripper device)
            {
                this.SeqName = $"SeqMonitor{device.MyName}";
                m_Device = device;
                m_DevAxes.Clear();
                if (m_Device.AxisHoist.IsValid) m_DevAxes.Add(m_Device.AxisHoist.GetDevAxis());
                if (m_Device.AxisSlide.IsValid) m_DevAxes.Add(m_Device.AxisSlide.GetDevAxis());
                if (m_Device.AxisTurn.IsValid) m_DevAxes.Add(m_Device.AxisTurn.GetDevAxis());
                m_CommandComp = new bool[m_DevAxes.Count];

                TaskDeviceControl.Instance.RegSeq(this);
            }
            public override void SeqAbort()
            {
                this.InitSeq();
            }
            public override int Do()
            {
                int seqNo = SeqNo;
                int rv = -1;

                if (!m_Device.Initialized) return rv;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                m_Device.DiHoistBrake.SetDi(m_Device.AxisHoist.GetDevAxis().IsAxisReady());
                            }
                            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.Use)
                            {
                                // Motor Alarm 발생할 경우 Unit Abort 하자!
                                string msg = string.Empty;
                                bool alarm = false;
                                foreach (_DevAxis axis in m_DevAxes)
                                {
                                    if (axis.IsAlarm())
                                    {
                                        msg += string.Format("{0}.Motor Alarm, ", axis.GetName());
                                        alarm = true;
                                    }
                                }
                                if (alarm)
                                {
                                    for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                    SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("DevFoupGripper Motor Check ! {0}", msg));
                                    seqNo = 10;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            int index = 0;
                            foreach (_DevAxis axis in m_DevAxes)
                            {
                                if (m_CommandComp[index] == false)
                                {
                                    int rv1 = axis.Stop();
                                    if (rv1 >= 0) 
                                    {
                                        m_CommandComp[index] = true;
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("{0}.Motor Stop !", axis.GetName())); 
                                    }
                                }
                                index++;
                            }
                            if (m_CommandComp.All(x => x == true))
                            {
                                for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                seqNo = 20;
                            }
                        }
                        break;

                    case 20:
                        {
                            bool alarm = false;
                            foreach (_DevAxis axis in m_DevAxes)
                            {
                                if (axis.IsAlarm())
                                {
                                    alarm = true;
                                }
                            }
                            if (!alarm)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("DevFoupGripper Motor Recovery !"));
                                seqNo = 0;
                            }
                        }
                        break;
                }

                SeqNo = seqNo;
                return rv;
            }
        }
        private class SeqAction : XSeqFunc
        {
            #region Fields
            private const int m_MAX_Axis = 3;
            private DevFoupGripper m_Device = null;
            private List<_DevAxis> m_DevAxes = new List<_DevAxis>();

            private _DevAxis m_Hoist = null;
            private _DevAxis m_Turn = null;
            private _DevAxis m_Slide = null;

            private List<_DevAxis> m_TargetDevAxes = new List<_DevAxis>();
            private List<_DevAxis> m_UsedDevAxes = new List<_DevAxis>();

            private enAxisMask m_SetMask = enAxisMask.aZ | enAxisMask.aY | enAxisMask.aT;
            private enAxisMask m_CurMask = enAxisMask.aZ;

            private List<ushort> m_TargetPoint = null;
            private List<ushort> m_TargetProp = null;
            private List<ushort> m_UsedPoint = null;
            private List<ushort> m_UsedProp = null;
            private List<double> m_TargetPos = null;
            private List<VelSet> m_TargetVelSet = null;
            private List<double> m_UsedPos = null;
            private List<VelSet> m_UsedVelSet = null;

            private List<VelSet> m_WaitVelSets = new List<VelSet>();
            private XyztPosition m_WaitPosition = new XyztPosition(); // wait

            private bool[] m_CommandComp = null;
            private int m_device_order = 0;
            private int m_moveorder = 0;
            private int m_moveNo = 0;
            private int m_retryCNT = 0;
            #endregion

            #region Constructor
            public SeqAction(DevFoupGripper device)
            {
                this.SeqName = $"SeqAction{device.MyName}";
                m_Device = device;
                if (m_Device.AxisHoist.IsValid) m_Hoist = m_Device.AxisHoist.GetDevAxis();
                if (m_Device.AxisSlide.IsValid) m_Slide = m_Device.AxisSlide.GetDevAxis();
                if (m_Device.AxisTurn.IsValid) m_Turn = m_Device.AxisTurn.GetDevAxis();

                //Find Used Axis
                m_DevAxes.Clear();
                if (m_Device.AxisHoist.IsValid) m_DevAxes.Add(m_Hoist);
                if (m_Device.AxisSlide.IsValid) m_DevAxes.Add(m_Slide);
                if (m_Device.AxisTurn.IsValid) m_DevAxes.Add(m_Turn);

                m_CommandComp = new bool[m_DevAxes.Count];
                m_TargetPoint = new List<ushort>();
                m_TargetProp = new List<ushort>();
                m_UsedPoint = new List<ushort>();
                m_UsedProp = new List<ushort>();
                m_TargetPos = new List<double>();
                m_TargetVelSet = new List<VelSet>();
                m_UsedPos = new List<double>();
                m_UsedVelSet = new List<VelSet>();
                TaskDeviceControl.Instance.RegSeq(this);
            }

            public override void SeqAbort()
            {
                this.InitSeq();
                m_moveNo = 0;
                m_retryCNT = 0;
                m_Device.m_CommandBusy = false;
                m_Device.m_CommandMoveWaitPosition = false;
            }
            #endregion

            #region Methods
            public int Home(enAxisMask mask)
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.Use)
                            {
                                if (mask.HasFlag(enAxisMask.aZ) && AppConfig.Instance.Simulation.MY_DEBUG)
                                {
                                    m_Device.SetBrake(true);
                                }
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 5;
                            }
                        }
                        break;

                    case 5:
                        {
                            bool safty = true;
                            safty &= mask.HasFlag(enAxisMask.aZ) ? GV.HoistMoveEnable : true;
                            safty &= mask.HasFlag(enAxisMask.aY) ? GV.SlideMoveEnable : true;
                            if (safty)
                            {
                                m_SetMask = mask;
                                m_CurMask = enAxisMask.aZ; // Z축부터 시작

                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Home Start"));
                                seqNo = 10;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_Device.SetBrake(false);
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Home Safty Alarm, HoistEnable={0}, HoistEnableCode={1},SlideEnable={2},SlideEnableCode={3}", GV.HoistMoveEnable, GV.HoistMoveEnableCode, GV.SlideMoveEnable, GV.SlideMoveEnableCode));
                                if (GV.HoistMoveEnable == false)
                                {
                                    if(GV.HoistMoveEnableCode == 1) //BeltCut
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to the beltCut Sensor."));
                                    else if (GV.HoistMoveEnableCode == 2) //Swing Sensor
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to the Swing Sensor."));
                                    else if (GV.HoistMoveEnableCode == 3) //Vehicle Moveing
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Vehicle Current State Moving."));
                                    else if (GV.HoistMoveEnableCode == 4) //Hoist Brake
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Hoist Brake."));
                                    else if (GV.HoistMoveEnableCode == 5) //Abnormal Foup Exist
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Abnormal Foup Exist"));
                                    rv = m_Device.ALM_HoistHomeConditionError.ID;
                                }
                                if (GV.SlideMoveEnable == false)
                                {
                                    if(GV.SlideMoveEnableCode == 1)
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Vehicle Current State Moving."));
                                    rv = m_Device.ALM_SlideHomeConditionError.ID;
                                }
                                seqNo = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (m_SetMask == 0)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Home Complete"));
                                rv = 0;
                                seqNo = 0;
                            }
                            else
                            {
                                m_UsedDevAxes.Clear();
                                m_moveorder = 0;
                                if ((m_SetMask & enAxisMask.aZ) == enAxisMask.aZ)
                                {
                                    m_Device.SetBrake(true);
                                    if (m_Device.AxisHoist.IsValid) m_UsedDevAxes.Add(m_Hoist);
                                    m_CurMask = enAxisMask.aZ;
                                }
                                else if ((m_SetMask & enAxisMask.aY) == enAxisMask.aY)
                                {
                                    if (m_Device.AxisSlide.IsValid) m_UsedDevAxes.Add(m_Slide);
                                    m_CurMask = enAxisMask.aY;
                                }
                                else if ((m_SetMask & enAxisMask.aT) == enAxisMask.aT)
                                {
                                    if (m_Device.AxisTurn.IsValid) m_UsedDevAxes.Add(m_Turn);
                                    m_CurMask = enAxisMask.aT;
                                }

                                m_device_order = 0;
                                foreach (_DevAxis axis in m_UsedDevAxes) m_device_order = m_device_order > axis.GetAxis().HomeOrder ? m_device_order : axis.GetAxis().HomeOrder;
                                seqNo = 20;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (m_moveorder <= m_device_order && m_UsedDevAxes.Count > 0) //Z -> X -> Y
                            {
                                string logMsg = "";
                                // Move Order 계산.
                                m_TargetDevAxes.Clear();
                                foreach (_DevAxis axis in m_UsedDevAxes)
                                {
                                    if (axis.GetAxis().HomeOrder == m_moveorder)
                                    {
                                        m_TargetDevAxes.Add(axis);
                                        logMsg += string.Format("{0},", axis.GetName());
                                    }
                                }
                                if (m_TargetDevAxes.Count > 0)
                                {
                                    for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                    SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Home [{0}]", logMsg));
                                    seqNo = 100;
                                }
                                else
                                {
                                    m_moveorder++;
                                }
                            }
                            else
                            {
                                m_SetMask ^= m_CurMask;
                                seqNo = 10;
                            }
                        }
                        break;

                    case 100:
                        {
                            bool alarm = false;
                            int almId = 0;
                            bool complete = true;
                            int index = 0;
                            foreach (_DevAxis axis in m_TargetDevAxes)
                            {
                                if (!m_CommandComp[index])
                                {
                                    int home_status = -1;
                                    home_status = axis.Home();
                                    if (home_status == 0)
                                    {
                                        m_CommandComp[index] = true;
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Home OK [{0}]", axis.GetName()));
                                    }
                                    else if (home_status > 0)
                                    {
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Home NG [{0}][alarm={1}]", axis.GetName(), home_status));
                                        almId = home_status;
                                        alarm = true;
                                    }
                                }

                                complete &= m_CommandComp[index];
                                index++;
                            }

                            if (complete || m_TargetDevAxes.Count == 0)
                            {
                                m_Device.SetBrake(false);
                                m_moveorder++;
                                seqNo = 20;
                            }
                            else if (alarm)
                            {
                                m_Device.SetBrake(false);
                                foreach (_DevAxis axis in m_TargetDevAxes) axis.SeqAbort();
                                for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                rv = almId;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }

            public int Move(enAxisMask mask, ushort point, ushort prop)
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.Use)
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 5;
                            }
                        }
                        break;

                    case 5:
                        {
                            bool safty = true;
                            safty &= mask.HasFlag(enAxisMask.aZ) ? GV.HoistMoveEnable : true;
                            safty &= mask.HasFlag(enAxisMask.aY) ? GV.SlideMoveEnable : true;
                            if (safty)
                            {
                                m_SetMask = mask;
                                m_CurMask = enAxisMask.aZ; // Z축부터 시작

                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move Start"));
                                seqNo = 10;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_Device.SetBrake(false);

                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move Interlock Alarm, HoistEnable={0}, HoistEnableCode={1},SlideEnable={2},SlideEnableCode={3}", GV.HoistMoveEnable, GV.HoistMoveEnableCode, GV.SlideMoveEnable, GV.SlideMoveEnableCode));
                                if (GV.HoistMoveEnable == false)
                                {
                                    if (GV.HoistMoveEnableCode == 1) //BeltCut
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to the beltCut Sensor."));
                                    else if (GV.HoistMoveEnableCode == 2) //Swing Sensor
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to the Swing Sensor."));
                                    else if (GV.HoistMoveEnableCode == 3) //Vehicle Moveing
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Vehicle Current State Moving."));
                                    else if (GV.HoistMoveEnableCode == 4) //Hoist Brake
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Hoist Brake."));
                                    else if (GV.HoistMoveEnableCode == 5) //Abnormal Foup Exist
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Abnormal Foup Exist"));
                                    rv = m_Device.ALM_HoistMoveInterlockError.ID;
                                }
                                if (GV.SlideMoveEnable == false)
                                {
                                    if (GV.SlideMoveEnableCode == 1)
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Vehicle Current State Moving."));
                                    rv = m_Device.ALM_SlideMoveInterlockError.ID;
                                }

                                seqNo = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (m_SetMask == 0)
                            {
                                m_Device.SetBrake(false);
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move Complete"));
                                rv = 0;
                                seqNo = 0;
                            }
                            else
                            {
                                m_UsedDevAxes.Clear();
                                m_UsedPoint.Clear();
                                m_UsedProp.Clear();
                                m_moveorder = 0;

                                if ((m_SetMask & enAxisMask.aZ) == enAxisMask.aZ)
                                {
                                    m_Device.SetBrake(true);
                                    if (m_Device.AxisHoist.IsValid)
                                    {
                                        m_UsedDevAxes.Add(m_Hoist);
                                        m_UsedPoint.Add(point);
                                        m_UsedProp.Add(prop);
                                    }
                                    m_CurMask = enAxisMask.aZ;
                                }
                                else if ((m_SetMask & enAxisMask.aY) == enAxisMask.aY)
                                {
                                    if (m_Device.AxisSlide.IsValid)
                                    {
                                        m_UsedDevAxes.Add(m_Slide);
                                        m_UsedPoint.Add(point);
                                        m_UsedProp.Add(prop);
                                    }
                                    m_CurMask = enAxisMask.aY;
                                }
                                else if ((m_SetMask & enAxisMask.aT) == enAxisMask.aT)
                                {
                                    if (m_Device.AxisTurn.IsValid)
                                    {
                                        m_UsedDevAxes.Add(m_Turn);
                                        m_UsedPoint.Add(point);
                                        m_UsedProp.Add(prop);
                                    }
                                    m_CurMask = enAxisMask.aT;
                                }

                                m_device_order = 0;
                                foreach (_DevAxis axis in m_UsedDevAxes) m_device_order = m_device_order > axis.GetAxis().MoveOrder ? m_device_order : axis.GetAxis().MoveOrder;
                                seqNo = 20;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (m_moveorder <= m_device_order && m_UsedDevAxes.Count > 0) //Z -> X -> Y
                            {
                                string logMsg = "";
                                // Move Order 계산.
                                m_TargetDevAxes.Clear();
                                m_TargetPoint.Clear();
                                m_TargetProp.Clear();
                                int index = 0;
                                foreach (_DevAxis axis in m_UsedDevAxes)
                                {
                                    if (axis.GetAxis().MoveOrder == m_moveorder)
                                    {
                                        m_TargetDevAxes.Add(axis);
                                        m_TargetPoint.Add(m_UsedPoint[index]);
                                        m_TargetProp.Add(m_UsedProp[index]);
                                        logMsg += string.Format("[{0}={1}],", axis.GetName(), m_UsedPoint[index]);
                                    }
                                    index++;
                                }
                                if (m_TargetDevAxes.Count > 0)
                                {
                                    for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                    SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move [{0}]", logMsg));
                                    seqNo = 100;
                                }
                                else
                                {
                                    m_moveorder++;
                                }
                            }
                            else
                            {
                                if (m_SetMask.HasFlag(m_CurMask)) m_SetMask ^= m_CurMask;
                                seqNo = 10;
                            }
                        }
                        break;

                    case 100:
                        {
                            bool alarm = false;
                            int almId = 0;
                            bool complete = true;
                            int index = 0;
                            foreach (_DevAxis axis in m_TargetDevAxes)
                            {
                                if (!m_CommandComp[index])
                                {
                                    int move_status = -1;
                                    move_status = axis.Move(m_TargetPoint[index], m_TargetProp[index], true);
                                    if (move_status == 0)
                                    {
                                        m_CommandComp[index] = true;
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move OK [{0}]", axis.GetName()));
                                    }
                                    else if (move_status > 0)
                                    {
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move NG [{0}][alarm={1}]", axis.GetName(), move_status));
                                        almId = move_status;
                                        alarm = true;
                                    }
                                }

                                complete &= m_CommandComp[index];
                                index++;
                            }

                            if (complete || m_TargetDevAxes.Count == 0)
                            {
                                m_moveorder++;
                                seqNo = 20;
                            }
                            else if (alarm)
                            {
                                m_Device.SetBrake(false);
                                foreach (_DevAxis axis in m_TargetDevAxes) axis.SeqAbort();
                                for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                rv = almId;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            /// <summary>
            /// sets : order => 
            /// </summary>
            /// <param name="mask"></param>
            /// <param name="pos"></param>
            /// <param name="sets"></param>
            /// <returns></returns>
            public int Move(enAxisMask mask, XyztPosition pos, List<VelSet> sets)
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.Use)
                            {
                                double diff_z = Math.Abs(pos.Z - m_Hoist.GetCurPosition());
                                double diff_y = Math.Abs(pos.Y - m_Slide.GetCurPosition());
                                double diff_t = Math.Abs(pos.T - m_Turn.GetCurPosition());
                                m_Device.HoistWorkingDistance += diff_z;
                                m_Device.SlideWorkingDistance += diff_y;
                                m_Device.RotateWorkingDistance += diff_t;
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 5;
                            }
                        }
                        break;

                    case 5:
                        {
                            bool safty = true;
                            safty &= mask.HasFlag(enAxisMask.aZ) ? GV.HoistMoveEnable : true;
                            safty &= mask.HasFlag(enAxisMask.aY) ? GV.SlideMoveEnable : true;
                            if (safty)
                            {
                                m_SetMask = mask;
                                m_CurMask = enAxisMask.aZ; // Z축부터 시작

                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move Start"));
                                seqNo = 10;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_Device.SetBrake(false);

                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move Interlock Alarm, HoistEnable={0}, HoistEnableCode={1},SlideEnable={2},SlideEnableCode={3}", GV.HoistMoveEnable, GV.HoistMoveEnableCode, GV.SlideMoveEnable, GV.SlideMoveEnableCode));
                                if (GV.HoistMoveEnable == false)
                                {
                                    if (GV.HoistMoveEnableCode == 1) //BeltCut
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to the beltCut Sensor."));
                                    else if (GV.HoistMoveEnableCode == 2) //Swing Sensor
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to the Swing Sensor."));
                                    else if (GV.HoistMoveEnableCode == 3) //Vehicle Moveing
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Vehicle Current State Moving."));
                                    else if (GV.HoistMoveEnableCode == 4) //Hoist Brake
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Hoist Brake."));
                                    else if (GV.HoistMoveEnableCode == 5) //Abnormal Foup Exist
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Abnormal Foup Exist"));
                                    rv = m_Device.ALM_HoistMoveInterlockError.ID;
                                }
                                if (GV.SlideMoveEnable == false)
                                {
                                    if (GV.SlideMoveEnableCode == 1)
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Vehicle Current State Moving."));
                                    rv = m_Device.ALM_SlideMoveInterlockError.ID;
                                }

                                seqNo = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (m_SetMask == 0)
                            {
                                m_Device.SetBrake(false);
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move Complete"));
                                rv = 0;
                                seqNo = 0;
                            }
                            else
                            {
                                m_UsedDevAxes.Clear();
                                m_UsedPos.Clear();
                                m_UsedVelSet.Clear();
                                m_moveorder = 0;

                                if ((m_SetMask & enAxisMask.aZ) == enAxisMask.aZ)
                                {
                                    m_Device.SetBrake(true);
                                    VelSet set = sets.Find(x => x.AxisCoord == enAxisCoord.Z);
                                    if (set.Vel == 0.0f) set = m_Device.AxisHoist.GetDevAxis().GetTeachingVel(m_Device.TeachingVelocityMid.PropId);
                                    if (m_Device.AxisHoist.IsValid)
                                    {
                                        m_UsedDevAxes.Add(m_Hoist);
                                        m_UsedPos.Add(pos.Z);
                                        m_UsedVelSet.Add(set);
                                    }
                                    m_CurMask = enAxisMask.aZ;
                                }
                                else if ((m_SetMask & enAxisMask.aY) == enAxisMask.aY)
                                {
                                    VelSet set = sets.Find(x => x.AxisCoord == enAxisCoord.Y);
                                    if (set.Vel == 0.0f) set = m_Device.AxisSlide.GetDevAxis().GetTeachingVel(m_Device.TeachingVelocityMid.PropId);
                                    if (m_Device.AxisSlide.IsValid)
                                    {
                                        m_UsedDevAxes.Add(m_Slide);
                                        m_UsedPos.Add(pos.Y);
                                        m_UsedVelSet.Add(set);
                                    }
                                    m_CurMask = enAxisMask.aY;
                                }
                                else if ((m_SetMask & enAxisMask.aT) == enAxisMask.aT)
                                {
                                    VelSet set = sets.Find(x => x.AxisCoord == enAxisCoord.T);
                                    if (set.Vel == 0.0f) set = m_Device.AxisTurn.GetDevAxis().GetTeachingVel(m_Device.TeachingVelocityMid.PropId);
                                    if (m_Device.AxisTurn.IsValid)
                                    {
                                        m_UsedDevAxes.Add(m_Turn);
                                        m_UsedPos.Add(pos.T);
                                        m_UsedVelSet.Add(set);
                                    }
                                    m_CurMask = enAxisMask.aT;
                                }
                                else if ((m_SetMask & enAxisMask.aYT) == enAxisMask.aYT)
                                {
                                    VelSet set = sets.Find(x => x.AxisCoord == enAxisCoord.T);
                                    if (set.Vel == 0.0f) set = m_Device.AxisTurn.GetDevAxis().GetTeachingVel(m_Device.TeachingVelocityMid.PropId);
                                    if (m_Device.AxisTurn.IsValid)
                                    {
                                        m_UsedDevAxes.Add(m_Turn);
                                        m_UsedPos.Add(pos.T);
                                        m_UsedVelSet.Add(set);
                                    }

                                    set = sets.Find(x => x.AxisCoord == enAxisCoord.Y);
                                    if (set.Vel == 0.0f) set = m_Device.AxisSlide.GetDevAxis().GetTeachingVel(m_Device.TeachingVelocityMid.PropId);
                                    if (m_Device.AxisSlide.IsValid)
                                    {
                                        m_UsedDevAxes.Add(m_Slide);
                                        m_UsedPos.Add(pos.Y);
                                        m_UsedVelSet.Add(set);
                                    }
                                    m_CurMask = enAxisMask.aYT;
                                }

                                m_device_order = 0;
                                foreach (_DevAxis axis in m_UsedDevAxes) m_device_order = m_device_order > axis.GetAxis().MoveOrder ? m_device_order : axis.GetAxis().MoveOrder;
                                seqNo = 20;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (m_moveorder <= m_device_order && m_UsedDevAxes.Count > 0) //Z -> X -> Y
                            {
                                string logMsg = "";
                                // Move Order 계산.
                                m_TargetDevAxes.Clear();
                                m_TargetPos.Clear();
                                m_TargetVelSet.Clear();
                                int index = 0;
                                foreach (_DevAxis axis in m_UsedDevAxes)
                                {
                                    if (axis.GetAxis().MoveOrder == m_moveorder)
                                    {
                                        m_TargetDevAxes.Add(axis);
                                        m_TargetPos.Add(m_UsedPos[index]);
                                        m_TargetVelSet.Add(m_UsedVelSet[index]);
                                        logMsg += string.Format("[{0}={1}],", axis.GetName(), m_UsedPos[index]);
                                    }
                                    index++;
                                }
                                if (m_TargetDevAxes.Count > 0)
                                {
                                    for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                    SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move [{0}]", logMsg));
                                    seqNo = 100;
                                }
                                else
                                {
                                    m_moveorder++;
                                }
                            }
                            else
                            {
                                if (m_SetMask.HasFlag(m_CurMask)) m_SetMask ^= m_CurMask;
                                seqNo = 10;
                            }
                        }
                        break;

                    case 100:
                        {
                            bool alarm = false;
                            int almId = 0;
                            bool complete = true;
                            int index = 0;
                            foreach (_DevAxis axis in m_TargetDevAxes)
                            {
                                if (!m_CommandComp[index])
                                {
                                    int move_status = -1;
                                    move_status = axis.Move(m_TargetPos[index], m_TargetVelSet[index], true);
                                    if (move_status == 0)
                                    {
                                        m_CommandComp[index] = true;
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move OK [{0}]", axis.GetName()));
                                    }
                                    else if (move_status > 0)
                                    {
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move NG [{0}][alarm={1}]", axis.GetName(), move_status));
                                        almId = move_status;
                                        alarm = true;
                                    }
                                }

                                complete &= m_CommandComp[index];
                                index++;
                            }

                            if (complete || m_TargetDevAxes.Count == 0)
                            {
                                m_moveorder++;
                                seqNo = 20;
                            }
                            else if (alarm)
                            {
                                m_Device.SetBrake(false);
                                foreach (_DevAxis axis in m_TargetDevAxes) axis.SeqAbort();
                                for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                rv = almId;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            public int ContinuousMove(enAxisMask mask, XyztPosition pos, List<VelSet> sets, bool diffPos = false)
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.Use)
                            {
                                double diff_z = Math.Abs(pos.Z - m_Hoist.GetCurPosition());
                                double diff_y = Math.Abs(pos.Y - m_Slide.GetCurPosition());
                                double diff_t = Math.Abs(pos.T - m_Turn.GetCurPosition());
                                m_Device.HoistWorkingDistance += diff_z;
                                m_Device.SlideWorkingDistance += diff_y;
                                m_Device.RotateWorkingDistance += diff_t;
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 5;
                            }
                        }
                        break;

                    case 5:
                        {
                            bool safty = true;
                            safty &= GV.HoistMoveEnable;
                            if (safty)
                            {
                                m_SetMask = mask;
                                m_CurMask = enAxisMask.aZ; // Z축부터 시작

                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move Start"));
                                seqNo = 10;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_Device.SetBrake(false);
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move Interlock Alarm, HoistEnable={0}, HoistEnableCode={1}", GV.HoistMoveEnable, GV.HoistMoveEnableCode));

								if (!GV.HoistMoveEnable)
								{
                                	if (GV.HoistMoveEnableCode == 1) //BeltCut
                                    	SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to the beltCut Sensor."));
                                	else if (GV.HoistMoveEnableCode == 2) //Swing Sensor
                                    	SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to the Swing Sensor."));
                                	else if (GV.HoistMoveEnableCode == 3) //Vehicle Moveing
                                    	SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Vehicle Current State Moving."));
                                	else if (GV.HoistMoveEnableCode == 4) //Hoist Brake
                                    	SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Hoist Brake."));
                                	else if (GV.HoistMoveEnableCode == 5) //Abnormal Foup Exist
                                    	SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Home operation cannot be performed due to Abnormal Foup Exist"));

                                	rv = m_Device.ALM_HoistMoveInterlockError.ID;
								}
                                seqNo = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (m_SetMask == 0)
                            {
                                m_Device.SetBrake(false);
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move Complete"));
                                rv = 0;
                                seqNo = 0;
                            }
                            else
                            {
                                m_UsedDevAxes.Clear();
                                m_UsedPos.Clear();
                                m_UsedVelSet.Clear();
                                m_moveorder = 0;

                                if ((m_SetMask & enAxisMask.aZ) == enAxisMask.aZ)
                                {
                                    m_Device.SetBrake(true);
                                    VelSet set = sets.Find(x => x.AxisCoord == enAxisCoord.Z);
                                    if (set.Vel == 0.0f) set = m_Device.AxisHoist.GetDevAxis().GetTeachingVel(m_Device.TeachingVelocityMid.PropId);
                                    if (m_Device.AxisHoist.IsValid)
                                    {
                                        m_UsedDevAxes.Add(m_Hoist);
                                        m_UsedPos.Add(pos.Z);
                                        m_UsedVelSet.Add(set);
                                    }
                                    m_CurMask = enAxisMask.aZ;
                                }
                                else if ((m_SetMask & enAxisMask.aY) == enAxisMask.aY)
                                {
                                    VelSet set = sets.Find(x => x.AxisCoord == enAxisCoord.Y);
                                    if (set.Vel == 0.0f) set = m_Device.AxisSlide.GetDevAxis().GetTeachingVel(m_Device.TeachingVelocityMid.PropId);
                                    if (m_Device.AxisSlide.IsValid)
                                    {
                                        m_UsedDevAxes.Add(m_Slide);
                                        m_UsedPos.Add(pos.Y);
                                        m_UsedVelSet.Add(set);
                                    }
                                    m_CurMask = enAxisMask.aY;
                                }
                                else if ((m_SetMask & enAxisMask.aT) == enAxisMask.aT)
                                {
                                    VelSet set = sets.Find(x => x.AxisCoord == enAxisCoord.T);
                                    if (set.Vel == 0.0f) set = m_Device.AxisTurn.GetDevAxis().GetTeachingVel(m_Device.TeachingVelocityMid.PropId);
                                    if (m_Device.AxisTurn.IsValid)
                                    {
                                        m_UsedDevAxes.Add(m_Turn);
                                        m_UsedPos.Add(pos.T);
                                        m_UsedVelSet.Add(set);
                                    }
                                    m_CurMask = enAxisMask.aT;
                                }
                                else if ((m_SetMask & enAxisMask.aYT) == enAxisMask.aYT)
                                {
                                    VelSet set = sets.Find(x => x.AxisCoord == enAxisCoord.T);
                                    if (set.Vel == 0.0f) set = m_Device.AxisTurn.GetDevAxis().GetTeachingVel(m_Device.TeachingVelocityMid.PropId);
                                    if (m_Device.AxisTurn.IsValid)
                                    {
                                        m_UsedDevAxes.Add(m_Turn);
                                        m_UsedPos.Add(pos.T);
                                        m_UsedVelSet.Add(set);
                                    }

                                    set = sets.Find(x => x.AxisCoord == enAxisCoord.Y);
                                    if (set.Vel == 0.0f) set = m_Device.AxisSlide.GetDevAxis().GetTeachingVel(m_Device.TeachingVelocityMid.PropId);
                                    if (m_Device.AxisSlide.IsValid)
                                    {
                                        m_UsedDevAxes.Add(m_Slide);
                                        m_UsedPos.Add(pos.Y);
                                        m_UsedVelSet.Add(set);
                                    }
                                    m_CurMask = enAxisMask.aYT;
                                }
                                m_device_order = 0;
                                foreach (_DevAxis axis in m_UsedDevAxes)
                                {
                                    m_device_order = m_device_order > axis.GetAxis().MoveOrder ? m_device_order : axis.GetAxis().MoveOrder;
                                    axis.SeqAbort();
                                }
                                seqNo = 20;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (m_moveorder <= m_device_order && m_UsedDevAxes.Count > 0) //Z -> X -> Y
                            {
                                string logMsg = "";
                                // Move Order 계산.
                                m_TargetDevAxes.Clear();
                                m_TargetPos.Clear();
                                m_TargetVelSet.Clear();
                                int index = 0;
                                foreach (_DevAxis axis in m_UsedDevAxes)
                                {
                                    if (axis.GetAxis().MoveOrder == m_moveorder)
                                    {
                                        m_TargetDevAxes.Add(axis);
                                        m_TargetPos.Add(m_UsedPos[index]);
                                        m_TargetVelSet.Add(m_UsedVelSet[index]);
                                        logMsg += string.Format("[{0}={1}],", axis.GetName(), m_UsedPos[index]);
                                    }
                                    index++;
                                }
                                if (m_TargetDevAxes.Count > 0)
                                {
                                    for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                    SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move [{0}]", logMsg));
                                    seqNo = 100;
                                }
                                else
                                {
                                    m_moveorder++;
                                }
                            }
                            else
                            {
                                if (m_SetMask.HasFlag(m_CurMask)) m_SetMask ^= m_CurMask;
                                seqNo = 10;
                            }
                        }
                        break;

                    case 100:
                        {
                            bool alarm = false;
                            int almId = 0;
                            bool complete = true;
                            int index = 0;
                            foreach (_DevAxis axis in m_TargetDevAxes)
                            {
                                if (!m_CommandComp[index])
                                {
                                    int move_status = -1;
                                    move_status = axis.ContinuousMove(m_TargetPos[index], m_TargetVelSet[index], true);
                                    if (move_status == 0)
                                    {
                                        m_CommandComp[index] = true;
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move OK [{0}]", axis.GetName()));
                                    }
                                    else if (move_status > 0)
                                    {
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Move NG [{0}][alarm={1}]", axis.GetName(), move_status));
                                        almId = move_status;
                                        alarm = true;
                                    }
                                }
                                complete &= m_CommandComp[index];
                                index++;
                            }
                            bool reSchedule = false;
                            reSchedule |= !m_SetMask.HasFlag(mask);
                            reSchedule |= diffPos;
                            if (reSchedule)
                            {
                                m_SetMask |= mask;
                                m_CurMask = enAxisMask.aZ;
                                seqNo = 10;
                            }
                            else if (complete || m_TargetDevAxes.Count == 0)
                            {
                                m_moveorder++;
                                seqNo = 20;
                            }
                            else if (alarm)
                            {
                                m_Device.SetBrake(false);
                                foreach (_DevAxis axis in m_TargetDevAxes) axis.SeqAbort();
                                for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                rv = almId;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }

            public override int Do()
            {
                int seqNo = m_moveNo;
                int rv = -1;

                if (!m_Device.Initialized) return rv;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.Use)
                            {
                                if (m_Device.m_CommandMoveWaitPosition)
                                {
                                    seqNo = 10;
                                    m_WaitVelSets = m_Device.GetTeachingVelSets(m_Device.TeachingVelocityUp.PropId);
                                    m_WaitPosition = m_Device.GetTeachingPosition(m_Device.TeachingPointWait.PosId);
                                }
                            }
                            else
                            {
                                m_Device.m_CommandMoveWaitPosition = false;
                                m_Device.m_CommandBusy = false;
                            }
                        }
                        break;

                    case 10:
                        {
                            //이거는 무조건 연속동작으로 해야되지않을까..?
                            //Anti Drop에 간섭되지않을까..?
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                                rv = m_Device.ContinuousMove(enAxisMask.aYT, m_WaitPosition, m_WaitVelSets);
                            else
                                rv = m_Device.Move(enAxisMask.aY | enAxisMask.aT, m_WaitPosition, m_WaitVelSets);

                            if (rv == 0)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("FoupGripper Motion Complete!"));
                                m_Device.m_CommandBusy = false;
                                m_Device.m_CommandMoveWaitPosition = false;
                                seqNo = 0;
                            }
                            else if (rv > 0)
                            {
                                AlarmId = rv;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (m_retryCNT < 3)
                            {
                                m_retryCNT++;
                                SequenceDeviceLog.WriteLog(m_Device.MyName, $"FoupGripper Motion Fail! Count : {m_retryCNT}");
                                seqNo = 10;
                            }
                            else
                            {
                                seqNo = 0;
                                EqpAlarm.Set(rv);
                                m_Device.SeqAbort();
                            }
                        }
                        break;
                }

                m_moveNo = seqNo;
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

                var helperXml = new XmlHelper<DevFoupGripper>();
                DevFoupGripper dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.AxisHoist = dev.AxisHoist;
                    this.AxisTurn = dev.AxisTurn;
                    this.AxisSlide = dev.AxisSlide;
                    this.DoHoistBrake = dev.DoHoistBrake;
                    this.DiHoistBrake = dev.DiHoistBrake;
                    this.DiHoistSwing = dev.DiHoistSwing;
                    this.DiLeftStbDoubleCheck = dev.DiLeftStbDoubleCheck;
                    this.DiRightStbDoubleCheck = dev.DiRightStbDoubleCheck;
                    this.DiFoupCapCheck = dev.DiFoupCapCheck;
                    this.DiFoupOpenCheck = dev.DiFoupOpenCheck;
                    this.Offset_HoistHomeSensorDetect = dev.Offset_HoistHomeSensorDetect;

                    this.TeachingPointWait = dev.TeachingPointWait;
                    this.TeachingPointAcquireBufUp = dev.TeachingPointAcquireBufUp;
                    this.TeachingPointAcquireBufDown = dev.TeachingPointAcquireBufDown;
                    this.TeachingPointAcquireBufGrip = dev.TeachingPointAcquireBufGrip;
                    this.TeachingPointAcquirePortUp = dev.TeachingPointAcquirePortUp;
                    this.TeachingPointAcquirePortDown = dev.TeachingPointAcquirePortDown;
                    this.TeachingPointAcquirePortGrip = dev.TeachingPointAcquirePortGrip;
                    this.TeachingPointAcquireStkUp = dev.TeachingPointAcquireStkUp;
                    this.TeachingPointAcquireStkDown = dev.TeachingPointAcquireStkDown;
                    this.TeachingPointAcquireStkGrip = dev.TeachingPointAcquireStkGrip;
                    this.TeachingPointDepositBufUp = dev.TeachingPointDepositBufUp;
                    this.TeachingPointDepositBufDown = dev.TeachingPointDepositBufDown;
                    this.TeachingPointDepositBufGrip = dev.TeachingPointDepositBufGrip;
                    this.TeachingPointDepositPortUp = dev.TeachingPointDepositPortUp;
                    this.TeachingPointDepositPortDown = dev.TeachingPointDepositPortDown;
                    this.TeachingPointDepositPortGrip = dev.TeachingPointDepositPortGrip;
                    this.TeachingPointDepositStkUp = dev.TeachingPointDepositStkUp;
                    this.TeachingPointDepositStkDown = dev.TeachingPointDepositStkDown;
                    this.TeachingPointDepositStkGrip = dev.TeachingPointDepositStkGrip;

                    this.TeachingVelocityLow = dev.TeachingVelocityLow;
                    this.TeachingVelocityMid = dev.TeachingVelocityMid;
                    this.TeachingVelocityHigh = dev.TeachingVelocityHigh;
                    this.TeachingVelocityUp = dev.TeachingVelocityUp;
                    this.TeachingVelocityDown = dev.TeachingVelocityDown;
                    this.TeachingVelocityBufUp = dev.TeachingVelocityBufUp;
                    this.TeachingVelocityBufDown = dev.TeachingVelocityBufDown;

                    this.HoistWorkingDistance = dev.HoistWorkingDistance;
                    this.SlideWorkingDistance = dev.SlideWorkingDistance;
                    this.RotateWorkingDistance = dev.RotateWorkingDistance;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
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
                var helperXml = new XmlHelper<DevFoupGripper>();
                helperXml.Save(fileName, this);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString() + this.ToString());
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
