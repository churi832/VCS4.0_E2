using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Sineva.VHL.Data;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Data.Process;

namespace Sineva.VHL.Device
{
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class DevGripperPIO : _Device
    {
        private const string DevName = "DevGripperPio";

        #region Fields - I/O
        private _DevPioComm m_PioComm = new _DevPioComm();

        private _DevOutput m_DoGripperOpen = new _DevOutput();
        private _DevOutput m_DoGripperClose = new _DevOutput();
        private _DevOutput m_DoGripperAlarmReset = new _DevOutput();

        private _DevInput m_DiGripperOpen = new _DevInput();
        private _DevInput m_DiGripperClose = new _DevInput();
        private _DevInput m_DiHoistHome = new _DevInput();
        private _DevInput m_DiHoistUp = new _DevInput();
        private _DevInput m_DiHoistLimit = new _DevInput();
        private _DevInput m_DiLeftProductExist = new _DevInput();
        private _DevInput m_DiRightProductExist = new _DevInput();
        private _DevInput m_DiPioStepBusy = new _DevInput();

        private AlarmData m_ALM_NotDefine = null;
        private AlarmData m_ALM_GripperOpenHoistPositionConditionAlarm = null; // Gripper Open Interlock(Home ON)
        private AlarmData m_ALM_GripperOpenHomeSensorConditionAlarm = null; // Gripper Open Interlock(Home ON)
        private AlarmData m_ALM_GripperOpenUpSensorConditionAlarm = null; // Gripper Open Interlock(Up ON)
        private AlarmData m_ALM_GripperOpenLimitSensorConditionAlarm = null; // Gripper Open Interlock(Limit ON)
        private AlarmData m_ALM_GripperOpenLeftExistConditionAlarm = null; // Gripper Open Interlock(Left Exist ON)
        private AlarmData m_ALM_GripperOpenRightExistConditionAlarm = null; // Gripper Open Interlock(Right Exist ON)
        private AlarmData m_ALM_GripperOpenTimeout = null; // Gripper Open Timeout
        private AlarmData m_ALM_GripperCloseTimeout = null; // Gripper Close Timeout
        private AlarmData m_ALM_ProductLeftExistAlarm = null; // Product Exist Alarm
        private AlarmData m_ALM_ProductRightExistAlarm = null; // Product Exist Alarm
        private AlarmData m_ALM_ProductLeftNotExistAlarm = null; // Product Not Exist Alarm
        private AlarmData m_ALM_ProductRightNotExistAlarm = null; // Product Not Exist Alarm
        private AlarmData m_ALM_ProductLeftExistMoveAlarm = null; // Product Exist Alarm
        private AlarmData m_ALM_ProductRightExistMoveAlarm = null; // Product Exist Alarm
        private AlarmData m_ALM_ProductLeftNotExistMoveAlarm = null; // Product Not Exist Alarm
        private AlarmData m_ALM_ProductRightNotExistMoveAlarm = null; // Product Not Exist Alarm
        private AlarmData m_ALM_PioGoOnAlarm = null; // PIO GO Signal On Alarm
        private AlarmData m_ALM_PioGoOffAlarm = null; // PIO GO Signal Off Alarm
        private AlarmData m_ALM_PioBusyOnAlarm = null; // PIO Busy Signal On Alarm
        private AlarmData m_ALM_PioBusyOffAlarm = null; // PIO Busy Signal Off Alarm

        private float m_TimerSensorDelayTime = 10.0f; // Sensor On/Off Check Delay Time
        private float m_TimerPIODelayTime = 0.1f; // PIO Signal On/Off Check Delay Time
        private float m_TimerSensorTimeout = 10.0f; // Sensor On/Off Confirm Timeout
        private float m_TimerPIOTimeout = 0.1f; // PIO Signal Confirm Timeout
        private int m_OpenCount = 0;
        private int m_CloseCount = 0;
        #endregion
        #region Fields -variable
        private SeqGripper m_SeqGripper = null;
        #endregion

        #region Property - I/O, Device Relation
        [Category("I/O Setting (Serial Comm)"), Description("Serial Comm"), DeviceSetting(true, true)]
        public _DevPioComm PioComm { get { return m_PioComm; } set { m_PioComm = value; } }
        [Category("I/O Setting (Digital Output)"), Description("Gripper Open"), DeviceSetting(true)]
        public _DevOutput DoGripperOpen { get { return m_DoGripperOpen; } set { m_DoGripperOpen = value; } }
        [Category("I/O Setting (Digital Output)"), Description("Gripper Close"), DeviceSetting(true)]
        public _DevOutput DoGripperClose { get { return m_DoGripperClose; } set { m_DoGripperClose = value; } }

        [Category("I/O Setting (Digital Output)"), Description("Gripper Alarm Reset"), DeviceSetting(true)]
        public _DevOutput DoGripperAlarmReset { get { return m_DoGripperAlarmReset; } set { m_DoGripperAlarmReset = value; } }       
        [Category("I/O Setting (Digital Input)"), Description("Gripper Open"), DeviceSetting(true)]
        public _DevInput DiGripperOpen { get { return m_DiGripperOpen; } set { m_DiGripperOpen = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Gripper Close"), DeviceSetting(true)]
        public _DevInput DiGripperClose { get { return m_DiGripperClose; } set { m_DiGripperClose = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Hoist Home Sensor"), DeviceSetting(true)]
        public _DevInput DiHoistHome { get { return m_DiHoistHome; } set { m_DiHoistHome = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Hoist Up Sensor"), DeviceSetting(true)]
        public _DevInput DiHoistUp { get { return m_DiHoistUp; } set { m_DiHoistUp = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Hoist Limit Sensor"), DeviceSetting(true)]
        public _DevInput DiHoistLimit { get { return m_DiHoistLimit; } set { m_DiHoistLimit = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Product Exist Sensor"), DeviceSetting(true)]
        public _DevInput DiLeftProductExist { get { return m_DiLeftProductExist; } set { m_DiLeftProductExist = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Product Exist Sensor"), DeviceSetting(true)]
        public _DevInput DiRightProductExist { get { return m_DiRightProductExist; } set { m_DiRightProductExist = value; } }
        [Category("I/O Setting (Digital Input)"), Description("PIO Busy"), DeviceSetting(true)]
        public _DevInput DiPioStepBusy { get { return m_DiPioStepBusy; } set { m_DiPioStepBusy = value; } }
        #endregion

        #region Property - Timer Setting
        [Category("!Setting Device (Delay Time)"), DisplayName("Sensor Check Delay Time(sec)"), Description("Sensor Check Delay Time(sec)"), DeviceSetting(true)]
        public float TimerSensorDelayTime { get { return m_TimerSensorDelayTime; } set { m_TimerSensorDelayTime = value; } }
        [Category("!Setting Device (Delay Time)"), DisplayName("PIO Signal Check Delay Time(sec)"), Description("PIO Signal Check Delay Time(sec)"), DeviceSetting(true)]
        public float TimerPIODelayTime { get { return m_TimerPIODelayTime; } set { m_TimerPIODelayTime = value; } }
        [Category("!Setting Device (Active Timeout)"), DisplayName("Sensor On/Off Timeout(sec)"), Description("Sensor On/Off Timeout(sec)"), DeviceSetting(true)]
        public float TimerSensorTimeout { get { return m_TimerSensorTimeout; } set { m_TimerSensorTimeout = value; } }
        [Category("!Setting Device (Active Timeout)"), DisplayName("PIO On/Off Timeout(sec)"), Description("PIO On/Off Timeout(sec)"), DeviceSetting(false)]
        public float TimerPIOTimeout { get { return m_TimerPIOTimeout; } set { m_TimerPIOTimeout = value; } }
        #endregion

        #region Property - LifeTime Manager
        [Category("!LifeTime Manager"), DisplayName("Open Count"), Description("Device Life Open Count"), DeviceSetting(false, true)]
        public int OpenCount
        {
            get { return m_OpenCount; }
            set { SaveCurState = m_OpenCount != value; m_OpenCount = value; }
        }
        [Category("!LifeTime Manager"), DisplayName("Close Count"), Description("Device Life Close Count"), DeviceSetting(false, true)]
        public int CloseCount
        {
            get { return m_CloseCount; }
            set { SaveCurState = m_CloseCount != value; m_CloseCount = value; }
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
        public AlarmData ALM_GripperOpenHoistPositionConditionAlarm
        {
            get { return m_ALM_GripperOpenHoistPositionConditionAlarm; }
            set { m_ALM_GripperOpenHoistPositionConditionAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_GripperOpenHomeSensorConditionAlarm
        {
            get { return m_ALM_GripperOpenHomeSensorConditionAlarm; }
            set { m_ALM_GripperOpenHomeSensorConditionAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_GripperOpenUpSensorConditionAlarm
        {
            get { return m_ALM_GripperOpenUpSensorConditionAlarm; }
            set { m_ALM_GripperOpenUpSensorConditionAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_GripperOpenLimitSensorConditionAlarm
        {
            get { return m_ALM_GripperOpenLimitSensorConditionAlarm; }
            set { m_ALM_GripperOpenLimitSensorConditionAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_GripperOpenLeftExistConditionAlarm
        {
            get { return m_ALM_GripperOpenLeftExistConditionAlarm; }
            set { m_ALM_GripperOpenLeftExistConditionAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_GripperOpenRightExistConditionAlarm
        {
            get { return m_ALM_GripperOpenRightExistConditionAlarm; }
            set { m_ALM_GripperOpenRightExistConditionAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_GripperOpenTimeout
        {
            get { return m_ALM_GripperOpenTimeout; }
            set { m_ALM_GripperOpenTimeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_GripperCloseTimeout
        {
            get { return m_ALM_GripperCloseTimeout; }
            set { m_ALM_GripperCloseTimeout = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ProductLeftExistAlarm
        {
            get { return m_ALM_ProductLeftExistAlarm; }
            set { m_ALM_ProductLeftExistAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ProductRightExistAlarm
        {
            get { return m_ALM_ProductRightExistAlarm; }
            set { m_ALM_ProductRightExistAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ProductLeftNotExistAlarm
        {
            get { return m_ALM_ProductLeftNotExistAlarm; }
            set { m_ALM_ProductLeftNotExistAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ProductRightNotExistAlarm
        {
            get { return m_ALM_ProductRightNotExistAlarm; }
            set { m_ALM_ProductRightNotExistAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ProductLeftExistMoveAlarm
        {
            get { return m_ALM_ProductLeftExistMoveAlarm; }
            set { m_ALM_ProductLeftExistMoveAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ProductRightExistMoveAlarm
        {
            get { return m_ALM_ProductRightExistMoveAlarm; }
            set { m_ALM_ProductRightExistMoveAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ProductLeftNotExistMoveAlarm
        {
            get { return m_ALM_ProductLeftNotExistMoveAlarm; }
            set { m_ALM_ProductLeftNotExistMoveAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ProductRightNotExistMoveAlarm
        {
            get { return m_ALM_ProductRightNotExistMoveAlarm; }
            set { m_ALM_ProductRightNotExistMoveAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_PioGoOnAlarm
        {
            get { return m_ALM_PioGoOnAlarm; }
            set { m_ALM_PioGoOnAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_PioGoOffAlarm
        {
            get { return m_ALM_PioGoOffAlarm; }
            set { m_ALM_PioGoOffAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_PioBusyOnAlarm
        {
            get { return m_ALM_PioBusyOnAlarm; }
            set { m_ALM_PioBusyOnAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_PioBusyOffAlarm
        {
            get { return m_ALM_PioBusyOffAlarm; }
            set { m_ALM_PioBusyOffAlarm = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public string ErrorMessage { get; set; }
        #endregion

        #region Constructor
        public DevGripperPIO()
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
            if (m_PioComm.IsValid) ok &= m_PioComm.Initialize(this.ToString(), false, false);

            if (m_DoGripperOpen.IsValid) ok &= DoGripperOpen.Initialize(this.ToString(), false, false);
            if (m_DoGripperClose.IsValid) ok &= DoGripperClose.Initialize(this.ToString(), false, false);
            if (m_DoGripperAlarmReset.IsValid) ok &= DoGripperAlarmReset.Initialize(this.ToString(), false, false);
            if (m_DiGripperOpen.IsValid) ok &= DiGripperOpen.Initialize(this.ToString(), false, false);
            if (m_DiGripperClose.IsValid) ok &= DiGripperClose.Initialize(this.ToString(), false, false);
            if (m_DiHoistHome.IsValid) ok &= DiHoistHome.Initialize(this.ToString(), false, false);
            if (m_DiHoistUp.IsValid) ok &= DiHoistUp.Initialize(this.ToString(), false, false);
            if (m_DiHoistLimit.IsValid) ok &= DiHoistLimit.Initialize(this.ToString(), false, false);
            if (m_DiLeftProductExist.IsValid) ok &= DiLeftProductExist.Initialize(this.ToString(), false, false);
            if (m_DiRightProductExist.IsValid) ok &= DiRightProductExist.Initialize(this.ToString(), false, false);
            if (m_DiPioStepBusy.IsValid) ok &= DiPioStepBusy.Initialize(this.ToString(), false, false);

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
            m_ALM_GripperOpenHoistPositionConditionAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, MyName, "Gripper Open", "Hoist Position Abnormal Alarm");
            m_ALM_GripperOpenHomeSensorConditionAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, MyName, "Gripper Open", "Home Sensor On Alarm");
            m_ALM_GripperOpenUpSensorConditionAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, MyName, "Gripper Open", "Up Sensor On Alarm");
            m_ALM_GripperOpenLimitSensorConditionAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, MyName, "Gripper Open", "Limit Sensor On Alarm");
            m_ALM_GripperOpenLeftExistConditionAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, MyName, "Gripper Open ", "Left Foup Exist On Alarm");
            m_ALM_GripperOpenRightExistConditionAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, MyName, "Gripper Open", "Right Foup Exist On Alarm");
            m_ALM_GripperOpenTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Gripper", "Open Timeout");
            m_ALM_GripperCloseTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Gripper", "Close Timeout");
            m_ALM_ProductLeftExistAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Gripper", "Left Detected Alarm");
            m_ALM_ProductRightExistAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Gripper", "Right Detected Alarm");
            m_ALM_ProductLeftNotExistAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Gripper", "Left Not Detected Alarm");
            m_ALM_ProductRightNotExistAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Gripper", "Right Not Detected Alarm");
            m_ALM_ProductLeftExistMoveAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Gripper", "Left Detected Move Alarm");
            m_ALM_ProductRightExistMoveAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Gripper", "Right Detected Move Alarm");
            m_ALM_ProductLeftNotExistMoveAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Gripper", "Left Not Detected Move Alarm");
            m_ALM_ProductRightNotExistMoveAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "Gripper", "Right Not Detected Move Alarm");
            m_ALM_PioGoOnAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Go On Timeoutt");
            m_ALM_PioGoOffAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Go Off Timeout");
            m_ALM_PioBusyOnAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Busy On Timeout");
            m_ALM_PioBusyOffAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Busy Off Timeout");

            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;

            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Open Times", this, "GetOpenCount", 1000));
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Close Times", this, "GetCloseCount", 1000));
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            if (ok)
            {
                m_SeqGripper = new SeqGripper(this);
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
        #endregion

        #region Methods
        public override void SeqAbort()
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return;
            if (!Initialized) return;

            m_PioComm.SeqAbort();
            m_SeqGripper.SeqAbort();
        }
        public int SetChannelId(int id, int channel)
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return 0;
            if (!Initialized) return m_ALM_NotDefine.ID;

            int rv = -1;
            rv = m_PioComm.SelectChannel(id, channel);
            return rv;
        }
        public int GripperOpen() // flg : open=1, close=0
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return 0;
            if (!Initialized) return m_ALM_NotDefine.ID;

            int rv = -1;
            rv = m_SeqGripper.Do(true);
            return rv;
        }
        public int GripperClose() // flg : open=1, close=0
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return 0;
            if (!Initialized) return m_ALM_NotDefine.ID;

            int rv = -1;
            rv = m_SeqGripper.Do(false);
            return rv;
        }
        public bool IsGripperOpen()
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return false;
            if (!Initialized) return false;

            bool open = true;
            open &= m_DiGripperOpen.IsDetected ? true : false;
            open &= m_DiGripperClose.IsDetected ? false : true;
            return open;
        }
        public bool IsGripperClose()
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return false;
            if (!Initialized) return false;

            bool close = true;
            close &= m_DiGripperClose.IsDetected ? true : false;
            close &= m_DiGripperOpen.IsDetected ? false : true;
            return close;
        }
        public bool IsHoistHome()
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return false;
            if (!Initialized) return false;

            return DiHoistHome.IsDetected;
        }
        public bool IsHoistUp()
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return false;
            if (!Initialized) return false;

            return DiHoistUp.IsDetected;
        }
        public bool IsHoistLimit()
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return false;
            if (!Initialized) return false;

            return DiHoistLimit.IsDetected;
        }
        public bool IsProductExist()
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return false;
            if (!Initialized) return false;

            bool exist = DiLeftProductExist.IsDetected && DiRightProductExist.IsDetected;
            return exist;
        }
        public bool IsProductNotExist()
        {
            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.NoUse) return false;
            if (!Initialized) return false;

            bool exist = !DiLeftProductExist.IsDetected && !DiRightProductExist.IsDetected;
            return exist;
        }
        #region Methods - Life Time
        public int GetOpenCount()
        {
            return m_OpenCount;
        }
        public int GetCloseCount()
        {
            return m_CloseCount;
        }
        #endregion
        #endregion

        #region Sequence
        private class SeqGripper : XSeqFunc
        {
            #region Field
            private DevGripperPIO m_Device = null;
            private const int m_MaxRetryCount = 2;
            private int m_RetryCount = 0;
            #endregion

            #region Constructor
            public SeqGripper(DevGripperPIO device)
            {
                this.SeqName = $"SeqGripper{device.MyName}";
                m_Device = device;
            }
            public override void SeqAbort()
            {
                this.InitSeq();
            }
            #endregion

            #region Override
            public int Do(bool Open)
            {
                int rv = -1;
                int seqNo = SeqNo;

                if (!m_Device.Initialized) return rv;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupHoist.FoupGripperUse == Use.Use)
                            {
                                bool step_busy = m_Device.DiPioStepBusy.IsValid ? m_Device.DiPioStepBusy.IsDetected : false;
                                rv = m_Device.PioComm.IsOpen() == false ? m_Device.PioComm.ALM_CommOpenTimeoutError.ID :
                                        step_busy ? m_Device.ALM_PioBusyOnAlarm.ID : -1;

                                if (rv < 0)
                                {
                                    // Normal Case
                                    if (Open)
                                    {
                                        if (m_Device.IsGripperOpen())
                                        {
                                            SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Already Open OK", m_Device.MyName));
                                            rv = 0;
                                            seqNo = 0;
                                        }
                                        else
                                        {
                                            if (GV.GripOpenEnable)
                                            {
                                                m_Device.OpenCount++;
                                                m_Device.m_DoGripperClose.SetDo(false);
                                                m_Device.m_DoGripperOpen.SetDo(true);
                                                if (AppConfig.Instance.Simulation.IO)
                                                {
                                                    m_Device.DiGripperClose.SetDi(false);
                                                    m_Device.DiGripperOpen.SetDi(true);
                                                }
                                                SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Open", m_Device.MyName));
                                                StartTicks = XFunc.GetTickCount();
                                                seqNo = 10;
                                            }
                                            else
                                            {
                                                bool sensor0 = m_Device.DiHoistHome.IsDetected;
                                                bool sensor1 = m_Device.DiHoistUp.IsDetected;
                                                bool sensor2 = m_Device.DiHoistLimit.IsDetected;
                                                bool sensor3 = m_Device.DiLeftProductExist.IsDetected;
                                                bool sensor4 = m_Device.DiRightProductExist.IsDetected;
                                                string data_status = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus.ToString();
                                                string msg = string.Format("DiHoistHome[{0}], DiHoistUp[{1}], DiHoistLimit[{2}], DiLeftProductExist[{3}], DiRightProductExist[{4}], FoupData[{5}]", sensor0, sensor1, sensor2, sensor3, sensor4, data_status);
                                                m_Device.ErrorMessage = msg;

                                                bool carrier_installed = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
                                                if (carrier_installed == false)
                                                {
                                                    if (sensor0) rv = m_Device.ALM_GripperOpenHomeSensorConditionAlarm.ID;
                                                    else if (sensor1) rv = m_Device.ALM_GripperOpenUpSensorConditionAlarm.ID;
                                                    else if (sensor2) rv = m_Device.ALM_GripperOpenLimitSensorConditionAlarm.ID;
                                                    else if (sensor3) rv = m_Device.ALM_GripperOpenLeftExistConditionAlarm.ID;
                                                    else if (sensor4) rv = m_Device.ALM_GripperOpenRightExistConditionAlarm.ID;
                                                }
                                                else
                                                {
                                                    if (sensor0 == false) rv = m_Device.ALM_GripperOpenHomeSensorConditionAlarm.ID;
                                                    else if (sensor1 == false) rv = m_Device.ALM_GripperOpenUpSensorConditionAlarm.ID;
                                                    else if (sensor2) rv = m_Device.ALM_GripperOpenLimitSensorConditionAlarm.ID;
                                                    else rv = m_Device.ALM_GripperOpenHoistPositionConditionAlarm.ID;
                                                }
                                                SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Open Condition Alarm - {1}", m_Device.MyName, msg));
                                                seqNo = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (m_Device.IsGripperClose())
                                        {
                                            SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Already Close OK", m_Device.MyName));
                                            rv = 0;
                                            seqNo = 0;
                                        }
                                        else
                                        {
                                            m_Device.CloseCount++;
                                            m_Device.m_DoGripperOpen.SetDo(false);
                                            m_Device.m_DoGripperClose.SetDo(true);
                                            if (AppConfig.Instance.Simulation.IO)
                                            {
                                                m_Device.DiGripperOpen.SetDi(false);
                                                m_Device.DiGripperClose.SetDi(true);
                                            }

                                            SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Close", m_Device.MyName));
                                            StartTicks = XFunc.GetTickCount();
                                            seqNo = 100;
                                        }
                                    }
                                }
                                else
                                {
                                    string msg = string.Empty;
                                    if (rv == m_Device.PioComm.ALM_CommOpenTimeoutError.ID) msg = string.Format("{0} Serial Connection Timeout", m_Device.MyName);
                                    else if (rv == m_Device.ALM_PioGoOffAlarm.ID) msg = string.Format("{0} PIO GO Off Alarm", m_Device.MyName);
                                    else if (rv == m_Device.ALM_PioBusyOnAlarm.ID) msg = string.Format("{0} PIO Busy On Alarm", m_Device.MyName);
                                    m_Device.ErrorMessage = msg;
                                    SequenceDeviceLog.WriteLog(string.Format("{0} {1}", m_Device.MyName, msg));
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            // Open Check
                            if (XFunc.GetTickCount() - StartTicks < m_Device.m_TimerSensorDelayTime * 1000.0f) break;
                            if (m_Device.IsGripperOpen())
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Open OK", m_Device.MyName));
                                m_RetryCount = 0;
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Device.m_TimerSensorTimeout * 1000.0f)
                            {
                                //m_Device.DoGripperAlarmReset.SetDo(true);
                                //SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Open Timeout ! DoGripperAlarmReset = ON", m_Device.MyName));
                                //StartTicks = XFunc.GetTickCount();
                                //seqNo = 20;

                                m_Device.m_DoGripperOpen.SetDo(false);
                                m_Device.m_DoGripperClose.SetDo(false);

                                SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Open Timeout ! Do Gripper Open, Close = Off", m_Device.MyName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 15;

                            }
                        }
                        break;

                    case 15:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;

							if (m_Device.DoGripperAlarmReset.IsValid)
                            	m_Device.DoGripperAlarmReset.SetDo(true);
                            SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Open Timeout ! DoGripperAlarmReset = ON", m_Device.MyName));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        break;

                    case 20:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;

                            if (m_RetryCount < m_MaxRetryCount)
                            {
                                m_RetryCount++;
                                if (m_Device.DoGripperAlarmReset.IsValid)
                                    m_Device.DoGripperAlarmReset.SetDo(false);
                                SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Open Timeout({1}) ! DoGripperAlarmReset = OFF", m_Device.MyName, m_RetryCount));
                                seqNo = 0;
                            }
                            else
                            {
                                m_RetryCount = 0;
                                SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Open NG", m_Device.MyName));
                                //rv = m_Device.DiGripperOpen.ALM_OnTimeout.ID;
                                rv = m_Device.ALM_GripperOpenTimeout.ID;
                                seqNo = 0;

                            }
                        }
                        break;

                    case 100:
                        {
                            // Close Check
                            if (XFunc.GetTickCount() - StartTicks < m_Device.m_TimerSensorDelayTime * 1000.0f) break;
                            if (m_Device.IsGripperClose())
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Close OK", m_Device.MyName));
                                m_RetryCount = 0;
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Device.m_TimerSensorTimeout * 1000.0f)
                            {
                                //m_Device.DoGripperAlarmReset.SetDo(true);
                                //SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Close Timeout ! DoGripperAlarmReset = ON", m_Device.MyName));
                                //StartTicks = XFunc.GetTickCount();
                                //seqNo = 110;

                                m_Device.m_DoGripperOpen.SetDo(false);
                                m_Device.m_DoGripperClose.SetDo(false);

                                SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Close Timeout ! Do Gripper Close, Open = Off", m_Device.MyName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 105;
                            }
                        }
                        break;

                    case 105:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;
							
							if (m_Device.DoGripperAlarmReset.IsValid)
	                            m_Device.DoGripperAlarmReset.SetDo(true);
                            SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Close Timeout ! DoGripperAlarmReset = ON", m_Device.MyName));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        break;

                    case 110:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;

                            if (m_RetryCount < m_MaxRetryCount)
                            {
                                m_RetryCount++;
                                if (m_Device.DoGripperAlarmReset.IsValid)
                                    m_Device.DoGripperAlarmReset.SetDo(false);
                                SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Close Timeout({1}) ! DoGripperAlarmReset = OFF", m_Device.MyName, m_RetryCount));
                                seqNo = 0;
                            }
                            else
                            {
                                m_RetryCount = 0;
                                SequenceDeviceLog.WriteLog(string.Format("{0} Gripper Close NG", m_Device.MyName));
                                //rv = m_Device.DiGripperClose.ALM_OnTimeout.ID;
                                rv = m_Device.ALM_GripperCloseTimeout.ID;

                                seqNo = 0;
                            }
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

                var helperXml = new XmlHelper<DevGripperPIO>();
                DevGripperPIO dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.PioComm = dev.PioComm;
                    this.DoGripperOpen = dev.DoGripperOpen;
                    this.DoGripperClose = dev.DoGripperClose;
                    this.DiGripperOpen = dev.DiGripperOpen;
                    this.DiGripperClose = dev.DiGripperClose;
                    this.DiHoistHome = dev.DiHoistHome;
                    this.DiHoistUp = dev.DiHoistUp;
                    this.DiHoistLimit = dev.DiHoistLimit;
                    this.DiLeftProductExist = dev.DiLeftProductExist;
                    this.DiRightProductExist = dev.DiRightProductExist;
                    this.DiPioStepBusy = dev.DiPioStepBusy;

                    this.TimerSensorDelayTime = dev.TimerSensorDelayTime;
                    this.TimerPIODelayTime = dev.TimerPIODelayTime;
                    this.TimerSensorTimeout = dev.TimerSensorTimeout;
                    this.TimerPIOTimeout = dev.TimerPIOTimeout;
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
                var helperXml = new XmlHelper<DevGripperPIO>();
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
