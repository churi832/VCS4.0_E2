using Sineva.VHL.Data;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Sineva.VHL.Device
{
    [Serializable]
    public enum enAntiDropType
    {
        IO,
        Motor,
        Cylinder,
    }
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class DevAntiDrop : _Device
    {
        private const string DevName = "DevAntiDrop";

        #region Fields
        private enAntiDropType m_Type = enAntiDropType.Motor;
        private DevAxisTag m_Axis = new DevAxisTag();

        private _DevOutput m_DoForward = new _DevOutput();
        private _DevOutput m_DoBackward = new _DevOutput();
        private _DevOutput m_DoStart = new _DevOutput();
        private _DevOutput m_DoStop = new _DevOutput();
        private _DevOutput m_DoAlarmReset = new _DevOutput();
        private _DevInput m_DiForward = new _DevInput();
        private _DevInput m_DiBackward = new _DevInput();
        private _DevInput m_DiAlarm = new _DevInput();

        private float m_AntiDropChangeTime = 1.0f;

        private TeachingData m_TeachingPointLock = new TeachingData();
        private TeachingData m_TeachingPointUnlock = new TeachingData();
        private VelocityData m_TeachingVelocityMove = null;

        private AlarmData m_ALM_NotDefine = null;
        private AlarmData m_ALM_AntiDropAlarm = null;
        private AlarmData m_ALM_AntiDropNotDetectFWAlarm = null;
        private AlarmData m_ALM_AntiDropNotDetectBWAlarm = null;

        private SeqAction m_SeqAction = null;
        private SeqMonitor m_SeqMonitor = null;
        private SeqCylinderAction m_SeqCylinder = null;
        private SeqMotorAction m_SeqMotor = null;
        private bool m_AlarmReset = false;

        private bool m_CommandLock = false;
        private bool m_CommandUnlock = false;
        private bool m_CommandBusy = false;

        private int m_UnlockCount = 0;
        private int m_LockCount = 0;
        #endregion

        #region Properties
        [Category("Configuration"), Description("Type"), DeviceSetting(true)]
        public enAntiDropType Type 
        {
            get { return m_Type; }
            set { m_Type = value; } 
        }
        [Category("Motor Setting"), Description("Motor"), DeviceSetting(true)]
        public DevAxisTag Axis
        {
            get { return m_Axis; }
            set { m_Axis = value; }
        }
        [Category("Teaching"), Description("Lock Point"), DeviceSetting(true)]
        public TeachingData TeachingPointLock
        {
            get { return m_TeachingPointLock; }
            set { m_TeachingPointLock = value; }
        }
        [Category("Teaching"), Description("Unlock Point"), DeviceSetting(true)]
        public TeachingData TeachingPointUnlock
        {
            get { return m_TeachingPointUnlock; }
            set { m_TeachingPointUnlock = value; }
        }
        [Category("Velocity"), Description("Moving Prop"), DeviceSetting(true)]
        public VelocityData TeachingVelocityMove
        {
            get { return m_TeachingVelocityMove; }
            set { m_TeachingVelocityMove = value; }
        }
        [Category("I/O Setting (Digital Output)"), Description("[Cylinder Type] Forward"), DeviceSetting(true)]
        public _DevOutput DoForward { get { return m_DoForward; } set { m_DoForward = value; } }
        [Category("I/O Setting (Digital Output)"), Description("[Cylinder Type] Backward"), DeviceSetting(true)]
        public _DevOutput DoBackward { get { return m_DoBackward; } set { m_DoBackward = value; } }
        [Category("I/O Setting (Digital Output)"), Description("[Cylinder Type] Start"), DeviceSetting(true)]
        public _DevOutput DoStart { get { return m_DoStart; } set { m_DoStart = value; } }
        [Category("I/O Setting (Digital Output)"), Description("[Cylinder Type] Stop"), DeviceSetting(true)]
        public _DevOutput DoStop { get { return m_DoStop; } set { m_DoStop = value; } }
        [Category("I/O Setting (Digital Output)"), Description("[Cylinder Type] AlarmReset"), DeviceSetting(true)]
        public _DevOutput DoAlarmReset { get { return m_DoAlarmReset; } set { m_DoAlarmReset = value; } }

        [Category("I/O Setting (Digital Input)"), Description("[Cylinder Type] Forward"), DeviceSetting(true)]
        public _DevInput DiForward { get { return m_DiForward; } set { m_DiForward = value; } }
        [Category("I/O Setting (Digital Input)"), Description("[Cylinder Type] Backward"), DeviceSetting(true)]
        public _DevInput DiBackward { get { return m_DiBackward; } set { m_DiBackward = value; } }
        [Category("I/O Setting (Digital Input)"), Description("[Cylinder Type] Alarm"), DeviceSetting(true)]
        public _DevInput DiAlarm { get { return m_DiAlarm; } set { m_DiAlarm = value; } }

        [Category("!Setting Parameter"), Description("[Cylinder Type] Move Timeover(sec)"), DeviceSetting(true, true)]
        public float AntiDropChangeTime
        {
            get { return m_AntiDropChangeTime; }
            set { m_AntiDropChangeTime = value; }
        }
        [Category("!LifeTime Manager"), DisplayName("Unlock Count"), Description("Device Life Unlock Count"), DeviceSetting(false, true)]
        public int UnlockCount
        {
            get { return m_UnlockCount; }
            set { SaveCurState = m_UnlockCount != value; m_UnlockCount = value; }
        }
        [Category("!LifeTime Manager"), DisplayName("Lock Count"), Description("Device Life Lock Count"), DeviceSetting(false, true)]
        public int LockCount
        {
            get { return m_LockCount; }
            set { SaveCurState = m_LockCount != value; m_LockCount = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_NotDefine
        {
            get { return m_ALM_NotDefine; }
            set { m_ALM_NotDefine = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_AntiDropAlarm
        {
            get { return m_ALM_AntiDropAlarm; }
            set { m_ALM_AntiDropAlarm = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_AntiDropNotDetectFWAlarm
        {
            get { return m_ALM_AntiDropNotDetectFWAlarm; }
            set { m_ALM_AntiDropNotDetectFWAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_AntiDropNotDetectBWAlarm
        {
            get { return m_ALM_AntiDropNotDetectBWAlarm; }
            set { m_ALM_AntiDropNotDetectBWAlarm = value; }
        }
        #endregion

        #region Constructor
        public DevAntiDrop()
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
            if (m_Type == enAntiDropType.Motor)
            {
                if (m_Axis.IsValid) ok &= m_Axis.GetDevAxis() != null ? true : false;
                if (m_Axis.GetDevAxis() != null)
                {
                    if (m_Axis.IsValid) ok &= m_Axis.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointLock);
                    if (m_Axis.IsValid) ok &= m_Axis.GetDevAxis().IsCheckTeachingPoint(m_TeachingPointUnlock);
                    if (m_DiForward.IsValid) ok &= m_DiForward.Initialize(this.ToString(), false, false);
                    if (m_DiBackward.IsValid) ok &= m_DiBackward.Initialize(this.ToString(), false, false);
                }
                if (m_Axis.IsValid) ok &= m_TeachingVelocityMove != null;
            }
            else if (m_Type == enAntiDropType.IO)
            {
                if (m_DoForward.IsValid) ok &= m_DoForward.Initialize(this.ToString(), false, false);
                if (m_DoBackward.IsValid) ok &= m_DoBackward.Initialize(this.ToString(), false, false);
                if (m_DoStart.IsValid) ok &= m_DoStart.Initialize(this.ToString(), false, false);
                if (m_DoStop.IsValid) ok &= m_DoStop.Initialize(this.ToString(), false, false);
                if (m_DoAlarmReset.IsValid) ok &= m_DoAlarmReset.Initialize(this.ToString(), false, false);

                if (m_DiForward.IsValid) ok &= m_DiForward.Initialize(this.ToString(), false, false);
                if (m_DiBackward.IsValid) ok &= m_DiBackward.Initialize(this.ToString(), false, false);
                if (m_DiAlarm.IsValid) ok &= m_DiAlarm.Initialize(this.ToString(), false, false);
            }

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
            ALM_NotDefine = AlarmListProvider.Instance.NewAlarm(AlarmCode.ParameterControlError, true, MyName, ParentName, "Not Define Alarm");
            ALM_AntiDropAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, MyName, ParentName, "Unit Alarm");
            ALM_AntiDropNotDetectFWAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, ParentName, "FW Sensor Not Detect Alarm");
            ALM_AntiDropNotDetectBWAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, ParentName, "BW Sensor Not Detect Alarm");
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;
            //m_GenDataEqpTemperature.Add(new KpsGeneralObject(svName, description, this, "GetCurTemp", i));
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Close Times", this, "GetLockCount", 1000));
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Open Times", this, "GetUnlockCount", 1000));
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            if (ok)
            {
                m_SeqAction = new SeqAction(this);
                if (Type == enAntiDropType.IO)
                {
                    m_SeqCylinder = new SeqCylinderAction(this);
                    m_SeqMonitor = new SeqMonitor(this);
                }
                else if (Type == enAntiDropType.Motor)
                {
                    m_SeqMotor = new SeqMotorAction(this);
                }
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
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return;
            if (!Initialized) return;

            //m_SeqAction.SeqAbort();
            if (Type == enAntiDropType.IO)
            {
                m_SeqCylinder.SeqAbort();
                //m_SeqMonitor.SeqAbort();
            }
            else if (Type == enAntiDropType.Motor)
            {
                m_SeqMotor.SeqAbort();
            }
            m_AlarmReset = true;
            m_CommandBusy = false;
            m_CommandLock = false;
            m_CommandUnlock = false;
        }
        #endregion

        #region Methods - Life Time
        public int GetUnlockCount()
        {
            return m_UnlockCount;
        }
        public int GetLockCount()
        {
            return m_LockCount;
        }
        #endregion

        #region Methods
        public int Lock()
        {
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return 0;
            if (!Initialized) return ALM_NotDefine.ID;

            int rv = -1;
            if (m_Type == enAntiDropType.IO)
            {
                rv = m_SeqCylinder.Lock();
            }
            else if (m_Type == enAntiDropType.Motor)
            {
                if (AppConfig.Instance.Simulation.MY_DEBUG)
                {
                    m_DiBackward.SetDi(false);
                    m_DiForward.SetDi(true);
                }

                rv = Move(m_TeachingPointLock.PosId, m_TeachingVelocityMove.PropId);
            }
            return rv;
        }
        public int Unlock()
        {
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return 0;
            if (!Initialized) return ALM_NotDefine.ID;

            int rv = -1;
            if (m_Type == enAntiDropType.IO)
            {
                rv = m_SeqCylinder.Unlock();
            }
            else if (m_Type == enAntiDropType.Motor)
            {
                if (AppConfig.Instance.Simulation.MY_DEBUG)
                {
                    m_DiForward.SetDi(false);
                    m_DiBackward.SetDi(true);
                }

                rv = Move(m_TeachingPointUnlock.PosId, m_TeachingVelocityMove.PropId);
            }
            return rv;
        }
        public void SetLock()
        {
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return;
            if (!Initialized) return;

            if (m_Type == enAntiDropType.IO)
            {
                if (m_DoBackward.IsValid) m_DoBackward.SetDo(false);
                if (m_DoForward.IsValid) m_DoForward.SetDo(true);
                if (m_DoStop.IsValid) m_DoStop.SetDo(false);
                if (m_DoStart.IsValid) m_DoStart.SetDo(true);

                if (AppConfig.Instance.Simulation.MY_DEBUG)
                {
                    m_DiBackward.SetDi(false);
                    m_DiForward.SetDi(true);
                }
            }
        }
        public void SetUnlock()
        {
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return;
            if (!Initialized) return;

            if (m_Type == enAntiDropType.IO)
            {
                if (m_DoForward.IsValid) m_DoForward.SetDo(false);
                if (m_DoBackward.IsValid) m_DoBackward.SetDo(true);
                if (m_DoStop.IsValid) m_DoStop.SetDo(false);
                if (m_DoStart.IsValid) m_DoStart.SetDo(true);

                if (AppConfig.Instance.Simulation.MY_DEBUG)
                {
                    m_DiForward.SetDi(false);
                    m_DiBackward.SetDi(true);
                }
            }
        }
        public bool GetLock()
        {
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return true;
            if (!Initialized) return false;

            bool rv = true;
            if (m_Type == enAntiDropType.IO)
            {
                rv &= m_DiForward.IsValid ? m_DiForward.IsDetected : false;
                rv &= m_DiBackward.IsValid ? !m_DiBackward.IsDetected : false;
            }
            else if (m_Type == enAntiDropType.Motor)
            {
                if (AppConfig.Instance.Simulation.MY_DEBUG == false)
                {
                    rv &= m_DiForward.IsValid ? m_DiForward.IsDetected : false;
                    rv &= m_DiBackward.IsValid ? !m_DiBackward.IsDetected : false;
                }
                double teachingPos = m_Axis.GetDevAxis().GetTeachingPosition(m_TeachingPointLock.PosId);
                double curPos = m_Axis.GetDevAxis().GetCurPosition();
                rv &= Math.Abs(teachingPos - curPos) < 1.0f;
            }
            return rv;
        }
        public bool GetUnlock()
        {
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return true;
            if (!Initialized) return false;

            bool rv = true;
            if (m_Type == enAntiDropType.IO)
            {
                rv &= m_DiForward.IsValid ? !m_DiForward.IsDetected : false;
                rv &= m_DiBackward.IsValid ? m_DiBackward.IsDetected : false;
            }
            else if (m_Type == enAntiDropType.Motor)
            {
                if (AppConfig.Instance.Simulation.MY_DEBUG == false)
                {
                    rv &= m_DiForward.IsValid ? !m_DiForward.IsDetected : false;
                    rv &= m_DiBackward.IsValid ? m_DiBackward.IsDetected : false;
                }
                double teachingPos = m_Axis.GetDevAxis().GetTeachingPosition(m_TeachingPointUnlock.PosId);
                double curPos = m_Axis.GetDevAxis().GetCurPosition();
                rv &= Math.Abs(teachingPos - curPos) < 1.0f;
            }
            return rv;
        }
        public bool IsAlarm()
        {
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return false;
            if (!Initialized) return false;

            bool rv = true;
            rv &= m_DiAlarm.IsValid ? m_DiAlarm.IsDetected : false;
            return rv;
        }
        public void AlarmReset(bool reset)
        {
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return;
            if (!Initialized) return;

            m_DoAlarmReset.SetDo(reset);
        }
        public void Reset()
        {
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return;
            if (!Initialized) return;

            if (m_Type == enAntiDropType.IO)
            {
                if (m_DoBackward.IsValid) m_DoBackward.SetDo(false);
                if (m_DoForward.IsValid) m_DoForward.SetDo(false);
                if (m_DoStop.IsValid) m_DoStop.SetDo(false);
                if (m_DoStart.IsValid) m_DoStart.SetDo(false);
                m_AlarmReset = true;
            }
        }
        #endregion

        #region Move Command
        public int Home()
        {
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return 0;
            if (!Initialized) return m_ALM_NotDefine.ID;

            int rv = -1;
            if (m_Type == enAntiDropType.Motor)
                rv = m_SeqMotor.Home();
            else rv = 0;
            return rv;
        }
        public int Move(ushort point, ushort prop)
        {
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return 0;
            if (!Initialized) return m_ALM_NotDefine.ID;

            int rv = -1;
            if (m_Type == enAntiDropType.Motor)
                rv = m_SeqMotor.Move(point, prop);
            else rv = 0;
            return rv;
        }
        public int Move(double pos, VelSet set)
        {
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return 0;
            if (!Initialized) return m_ALM_NotDefine.ID;

            int rv = -1;
            if (m_Type == enAntiDropType.Motor)
                rv = m_SeqMotor.Move(pos, set);
            else rv = 0;
            return rv;
        }
        public void SetCommandLock()
        {
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return;
            if (!Initialized) return;

            if (m_CommandBusy)
            {
                // 갑자기 Unlock하라고 하면 힘들것 같은데...확인후 기다려야 할수도 있음.
                SeqAbort();
            }
            m_CommandLock = true;
            m_CommandUnlock = false;
            m_CommandBusy = true;
        }
        public void SetCommandUnlock()
        {
            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.NoUse) return;
            if (!Initialized) return;

            if (m_CommandBusy)
            {
                // 갑자기 Unlock하라고 하면 힘들것 같은데...확인후 기다려야 할수도 있음.
                SeqAbort();
            }
            m_CommandLock = false;
            m_CommandUnlock = true;
            m_CommandBusy = true;
        }
        public bool IsCommandBusy()
        {
            return m_CommandBusy;
        }
        #endregion

        #region Sequence
        private class SeqMonitor : XSeqFunc
        {
            #region Field
            private DevAntiDrop m_Device = null;
            #endregion

            public SeqMonitor(DevAntiDrop device)
            {
                this.SeqName = $"SeqMonitor{device.MyName}";
                m_Device = device;
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
                            if (m_Device.Type != enAntiDropType.IO) break;

                            if (m_Device.m_AlarmReset)
                            {
                                m_Device.m_AlarmReset = false;
                                if (m_Device.IsAlarm())
                                {
                                    m_Device.AlarmReset(true);
                                    StartTicks = XFunc.GetTickCount();
                                    SequenceDeviceLog.WriteLog(string.Format("{0} Alarm Reset", m_Device.MyName));
                                    seqNo = 10;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            if (!m_Device.IsAlarm())
                            {
                                m_Device.AlarmReset(false);
                                SequenceDeviceLog.WriteLog(string.Format("{0} Alarm Clear", m_Device.MyName));
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_Device.AlarmReset(false);
                                SequenceDeviceLog.WriteLog(string.Format("{0} Alarm Reset NG", m_Device.MyName));
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
            #region Field
            private DevAntiDrop m_Device = null;
            #endregion

            #region Constructor
            public SeqAction(DevAntiDrop device)
            {
                this.SeqName = $"SeqAction{device.MyName}";
                m_Device = device;
                TaskDeviceControl.Instance.RegSeq(this);
            }
            public override void SeqAbort()
            {
                if (AlarmId > 0)
                {
                    EqpAlarm.Reset(AlarmId);
                    AlarmId = 0;
                }
                this.InitSeq();
            }
            #endregion

            #region override
            public override int Do()
            {
                int seqNo = SeqNo;
                int rv = -1;

                if (!m_Device.Initialized) return rv;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.Use)
                            {
                                if (m_Device.m_CommandLock)
                                {
                                    SequenceDeviceLog.WriteLog(string.Format("AntiDrop Lock Start"));
                                    seqNo = 10;
                                }
                                else if (m_Device.m_CommandUnlock)
                                {
                                    SequenceDeviceLog.WriteLog(string.Format("AntiDrop Unlock Start"));
                                    seqNo = 20;
                                }
                            }
                            else
                            {
                                m_Device.m_CommandLock = false;
                                m_Device.m_CommandUnlock = false;
                                m_Device.m_CommandBusy = false;
                            }
                        }
                        break;

                    case 10:
                        {
                            rv = m_Device.Lock();
                            if (rv == 0)
                            {
                                m_Device.m_CommandLock = false;
                                m_Device.m_CommandBusy = false;
                                SequenceDeviceLog.WriteLog(string.Format("AntiDrop Lock OK"));
                                seqNo = 0;
                            }
                            else if (rv > 0)
                            {
                                AlarmId = rv;
                                EqpAlarm.Set(AlarmId);
                                SequenceDeviceLog.WriteLog(string.Format("AntiDrop Lock Alarm : Code[{0}], Message[{1}]", AlarmId, EqpAlarm.GetAlarmMsg(AlarmId)));
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 20:
                        {
                            rv = m_Device.Unlock();
                            if (rv == 0)
                            {
                                m_Device.m_CommandUnlock = false;
                                m_Device.m_CommandBusy = false;
                                SequenceDeviceLog.WriteLog(string.Format("AntiDrop Unlock OK"));
                                seqNo = 0;
                            }
                            else if (rv > 0)
                            {
                                AlarmId = rv;
                                EqpAlarm.Set(AlarmId);
                                SequenceDeviceLog.WriteLog(string.Format("AntiDrop Unlock Alarm : Code[{0}], Message[{1}]", AlarmId, EqpAlarm.GetAlarmMsg(AlarmId)));
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 1000:
                        {
                            if (IsPushedSwitch.IsAlarmReset)
                            {
                                IsPushedSwitch.m_AlarmRstPushed = false;
                                SequenceDeviceLog.WriteLog(string.Format("Reset Alarm : Code[{0}]", AlarmId));
                                EqpAlarm.Reset(AlarmId);
                                AlarmId = 0;
                                seqNo = 0;
                            }
                            break;
                        }
                }

                SeqNo = seqNo;
                return rv;
            }
            #endregion
        }
        private class SeqCylinderAction : XSeqFunc
        {
            #region Field
            private DevAntiDrop m_Device = null;
            #endregion

            #region Constructor
            public SeqCylinderAction(DevAntiDrop device)
            {
                this.SeqName = $"SeqCylinderAction{device.MyName}";
                m_Device = device;
            }
            public override void SeqAbort()
            {
                if (AlarmId > 0)
                {
                    EqpAlarm.Reset(AlarmId);
                    AlarmId = 0;
                }
                this.InitSeq();
            }
            #endregion

            #region override
            public int Lock()
            {
                if (!m_Device.Initialized) return -1;

                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.Use)
                            {
                                m_Device.SetLock();
                                StartTicks = XFunc.GetTickCount();
                                SequenceDeviceLog.WriteLog(string.Format("{0} AntiDrop Forward Start", m_Device.MyName));
                                seqNo = 10;
                            }
                            else
                            {
                                rv = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (m_Device.GetLock())
                            {
                                m_Device.LockCount++;
                                m_Device.Reset();
                                SequenceDeviceLog.WriteLog(string.Format("{0} AntiDrop Forward OK", m_Device.MyName));
                                seqNo = 0;
                            }
                            else if (m_Device.IsAlarm())
                            {
                                m_Device.Reset();
                                rv = m_Device.ALM_AntiDropAlarm.ID;
                                SequenceDeviceLog.WriteLog(string.Format("{0} AntiDrop Alarm", m_Device.MyName));
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Device.AntiDropChangeTime * 1000)
                            {
                                m_Device.Reset();
                                rv = m_Device.DiForward.ALM_OnTimeout.ID;
                                SequenceDeviceLog.WriteLog(string.Format("{0} AntiDrop Forward Timeout", m_Device.MyName));
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            public int Unlock()
            {
                if (!m_Device.Initialized) return -1;

                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.Use)
                            {
                                m_Device.SetUnlock();
                                StartTicks = XFunc.GetTickCount();
                                SequenceDeviceLog.WriteLog(string.Format("{0} AntiDrop Backward Start", m_Device.MyName));
                                seqNo = 10;
                            }
                            else
                            {
                                rv = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (m_Device.GetUnlock())
                            {
                                m_Device.UnlockCount++;
                                m_Device.Reset();
                                SequenceDeviceLog.WriteLog(string.Format("{0} AntiDrop Backward OK", m_Device.MyName));
                                seqNo = 0;
                            }
                            else if (m_Device.IsAlarm())
                            {
                                m_Device.Reset();
                                rv = m_Device.ALM_AntiDropAlarm.ID;
                                SequenceDeviceLog.WriteLog(string.Format("{0} AntiDrop Alarm", m_Device.MyName));
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Device.AntiDropChangeTime * 1000)
                            {
                                m_Device.Reset();
                                rv = m_Device.DiBackward.ALM_OnTimeout.ID;
                                SequenceDeviceLog.WriteLog(string.Format("{0} AntiDrop Backward Timeout", m_Device.MyName));
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
        private class SeqMotorAction : XSeqFunc
        {
            #region Fields
            private const int m_MAX_Axis = 1;
            private DevAntiDrop m_Device = null;
            private List<_DevAxis> m_DevAxes = new List<_DevAxis>();

            private _DevAxis m_Axis = null;

            private List<_DevAxis> m_TargetDevAxes = new List<_DevAxis>();
            private List<_DevAxis> m_UsedDevAxes = new List<_DevAxis>();

            private List<ushort> m_TargetPoint = null;
            private List<ushort> m_TargetProp = null;
            private List<double> m_TargetPos = null;
            private List<VelSet> m_TargetVelSet = null;

            private bool[] m_CommandComp = null;

            private int m_HomeSeqNo = 0;
            #endregion

            #region Constructor
            public SeqMotorAction(DevAntiDrop device)
            {
                this.SeqName = $"SeqMotorAction{device.MyName}";
                m_Device = device;
                if (m_Device.Axis.IsValid) m_Axis = m_Device.Axis.GetDevAxis();

                //Find Used Axis
                m_DevAxes.Clear();
                if (m_Device.Axis.IsValid) m_DevAxes.Add(m_Axis);

                m_CommandComp = new bool[m_DevAxes.Count];
                m_TargetPoint = new List<ushort>();
                m_TargetProp = new List<ushort>();
                m_TargetPos = new List<double>();
                m_TargetVelSet = new List<VelSet>();
            }

            public override void SeqAbort()
            {
                m_HomeSeqNo = 0;
                this.InitSeq();
                this.SeqNo = 0;
                for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                if(m_TargetDevAxes.Count > 0)
                    foreach (_DevAxis axis in m_TargetDevAxes) axis.SeqAbort();

                m_TargetDevAxes.Clear();
                m_TargetPoint.Clear();
                m_TargetProp.Clear();
            }
            #endregion

            #region Methods
            public int Home()
            {
                int rv = -1;
                int seqNo = m_HomeSeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.Use)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("AntiDrop Home Start"));

                                m_TargetDevAxes.Clear();
                                foreach (_DevAxis axis in m_DevAxes) m_TargetDevAxes.Add(axis);
                                for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                seqNo = 100;
                            }
                            else
                            {
                                rv = 0;
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
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("AntiDrop Home OK [{0}]", axis.GetName()));
                                    }
                                    else if (home_status > 0)
                                    {
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("AntiDrop Home NG [{0}][alarm={1}]", axis.GetName(), home_status));
                                        almId = home_status;
                                        alarm = true;
                                    }
                                }

                                complete &= m_CommandComp[index];
                                index++;
                            }

                            if (complete)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("AntiDrop Home Complete"));
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (alarm)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("AntiDrop Home Alarm"));
                                foreach (_DevAxis axis in m_TargetDevAxes) axis.SeqAbort();
                                for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                rv = almId;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                m_HomeSeqNo = seqNo;
                return rv;
            }
            public int Move(ushort point, ushort prop)
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.Use)
                            {
                                bool ready = true;
                                foreach (_DevAxis axis in m_DevAxes)
                                    ready &= axis.IsAxisReady();

                                if (ready)
                                {
                                    if (m_Device.IsCommandBusy())
                                    {
                                        //멈추고 진행합시다...
                                        SeqAbort();
                                    }
                                    string logMsg = "";
                                    // Move Order 계산.
                                    m_TargetDevAxes.Clear();
                                    m_TargetPoint.Clear();
                                    m_TargetProp.Clear();
                                    foreach (_DevAxis axis in m_DevAxes)
                                    {
                                        m_TargetDevAxes.Add(axis);
                                        m_TargetPoint.Add(point);
                                        m_TargetProp.Add(prop);
                                        logMsg += string.Format("[{0}={1}],", axis.GetName(), point);
                                    }

                                    for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                    SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("AntiDrop Move Start [{0}]", logMsg));
                                    seqNo = 100;
                                }
                                else
                                {
                                    SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("AntiDrop Home Start - [Not Ready]"));
                                    seqNo = 10;
                                }
                            }
                            else
                            {
                                rv = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            int home_status = m_Device.Home();
                            if (home_status == 0)
                            {
                                seqNo = 0;
                            }
                            else if (home_status > 0)
                            {
                                rv = home_status;
                                seqNo = 0;
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
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move OK [{0}]", axis.GetName()));
                                    }
                                    else if (move_status > 0)
                                    {
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move NG [{0}][alarm={1}]", axis.GetName(), move_status));
                                        almId = move_status;
                                        alarm = true;
                                    }
                                }

                                complete &= m_CommandComp[index];
                                index++;
                            }

                            if (complete)
                            {
                                if (m_Device.GetLock()) m_Device.LockCount++;
                                else m_Device.UnlockCount++;
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move Complete"));
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (alarm)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move Alarm"));
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
            public int Move(double pos, VelSet set)
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupOperation.AntiDropUse == Use.Use)
                            {
                                bool ready = true;
                                foreach (_DevAxis axis in m_DevAxes)
                                    ready &= axis.IsAxisReady();

                                if (ready)
                                {
                                    string logMsg = "";
                                    // Move Order 계산.
                                    m_TargetDevAxes.Clear();
                                    m_TargetPos.Clear();
                                    m_TargetVelSet.Clear();
                                    foreach (_DevAxis axis in m_DevAxes)
                                    {
                                        m_TargetDevAxes.Add(axis);
                                        m_TargetPos.Add(pos);
                                        m_TargetVelSet.Add(set);
                                        logMsg += string.Format("[{0}={1}],", axis.GetName(), pos);
                                    }

                                    for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                    SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move Start [{0}]", logMsg));
                                    seqNo = 100;
                                }
                                else
                                {
                                    SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("AntiDrop Home Start - [Not Ready]"));
                                    seqNo = 10;
                                }
                            }
                            else
                            {
                                rv = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            int home_status = m_Device.Home();
                            if (home_status == 0)
                            {
                                seqNo = 0;
                            }
                            else if (home_status > 0)
                            {
                                rv = home_status;
                                seqNo = 0;
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
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move OK [{0}]", axis.GetName()));
                                    }
                                    else if (move_status > 0)
                                    {
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move NG [{0}][alarm={1}]", axis.GetName(), move_status));
                                        almId = move_status;
                                        alarm = true;
                                    }
                                }

                                complete &= m_CommandComp[index];
                                index++;
                            }

                            if (complete)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move Complete"));
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (alarm)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move Alarm"));
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

                var helperXml = new XmlHelper<DevAntiDrop>();
                DevAntiDrop dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.Type = dev.Type;
                    this.Axis = dev.Axis;
                    this.TeachingPointLock = dev.TeachingPointLock;
                    this.TeachingPointUnlock = dev.TeachingPointUnlock;
                    this.TeachingVelocityMove = dev.TeachingVelocityMove;

                    this.DoForward = dev.DoForward;
                    this.DoBackward = dev.DoBackward;
                    this.DoStart = dev.DoStart;
                    this.DoStop = dev.DoStop;
                    this.DoAlarmReset = dev.DoAlarmReset;

                    this.DiForward = dev.DiForward;
                    this.DiBackward = dev.DiBackward;
                    this.DiAlarm = dev.DiAlarm;

                    this.AntiDropChangeTime = dev.AntiDropChangeTime;

                    this.UnlockCount = dev.UnlockCount;
                    this.LockCount = dev.LockCount;
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
                var helperXml = new XmlHelper<DevAntiDrop>();
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
