using Sineva.VHL.Data;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data.Process;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Sineva.VHL.Device
{
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class DevSteer : _Device
    {
        private const string DevName = "DevSteer";

        #region Fields
        private _DevCylinder m_FrontSteer = new _DevCylinder();
        private _DevCylinder m_RearSteer = new _DevCylinder();
        private float m_FrontRearOffset = 340.0f;
        private float m_ChangeDelayDistance = 0.0f; // BCR Node 변경 시점에서 바로 바꾸지 말고 조금 여유를 두자...S Curve는 어떻게 하지 ?
        private float m_SteerChangeTime = 0.5f;
        private float m_SteerOutputOffTime = 5.0f;
        private bool m_FrontRearSyncChange = false;
        private float m_SteerOutputRemainDistance = 200.0f;

        private AlarmData m_ALM_NotDefine = null;
        private AlarmData m_ALM_FrontSteerLeftChangedAlarm = null;
        private AlarmData m_ALM_RearSteerLeftChangedAlarm = null;
        private AlarmData m_ALM_FrontSteerRightChangedAlarm = null;
        private AlarmData m_ALM_RearSteerRightChangedAlarm = null;
        private AlarmData m_ALM_FrontSteerLeftChangedInterlockAlarm = null;
        private AlarmData m_ALM_RearSteerLeftChangedInterlockAlarm = null;
        private AlarmData m_ALM_FrontSteerRightChangedInterlockAlarm = null;
        private AlarmData m_ALM_RearSteerRightChangedInterlockAlarm = null;
        private AlarmData m_ALM_FrontSteerOutputOffTimeoverAlarm = null;
        private AlarmData m_ALM_RearSteerOutputOffTimeoverAlarm = null;

        private SeqAction m_SeqActionFront = null;
        private SeqAction m_SeqActionRear = null;
        private SeqMonitor m_SeqMonitorFront = null;
        private SeqMonitor m_SeqMonitorRear = null;
        #endregion

        #region Properties
        [Category("!Setting Device(Cylinder)"), Description("Front Steer Cylinder"), DeviceSetting(true)]
        public _DevCylinder FrontSteer
        {
            get { return m_FrontSteer; }
            set { m_FrontSteer = value; }
        }
        [Category("!Setting Device(Cylinder)"), Description("Rear Steer Cylinder"), DeviceSetting(true)]
        public _DevCylinder RearSteer
        {
            get { return m_RearSteer; }
            set { m_RearSteer = value; }
        }
        [Category("!Setting Parameter"), Description("Front/Rear Steer Offset Distance"), DeviceSetting(true, true)]
        public float FrontRearOffset
        {
            get { return m_FrontRearOffset; }
            set { m_FrontRearOffset = value; }
        }
        [Category("!Setting Parameter"), Description("Change Delay Distance(mm)"), DeviceSetting(true, true)]
        public float ChangeDelayDistance
        {
            get { return m_ChangeDelayDistance; }
            set { m_ChangeDelayDistance = value; }
        }
        [Category("!Setting Parameter"), Description("Change NotChange Interlock Timeover(sec)"), DeviceSetting(true, true)]
        public float SteerChangeTime
        {
            get { return m_SteerChangeTime; }
            set { m_SteerChangeTime = value; }
        }
        [Category("!Setting Parameter"), Description("Steer Output Off Time(sec)"), DeviceSetting(true, true)]
        public float SteerOutputOffTime
        {
            get { return m_SteerOutputOffTime; }
            set { m_SteerOutputOffTime = value; }
        }
        [Category("!Setting Parameter"), Description("Front&Rear Sync Change Mode"), DeviceSetting(true, true)]
        public bool FrontRearSyncChange
        {
            get { return m_FrontRearSyncChange; }
            set { m_FrontRearSyncChange = value; }
        }
        [Category("!Setting Parameter"), Description("Steer Output Remain Distance(mm)"), DeviceSetting(true, true)]
        public float SteerOutputRemainDistance
        {
            get { return m_SteerOutputRemainDistance; }
            set { m_SteerOutputRemainDistance = value; }
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
        public AlarmData ALM_FrontSteerLeftChangedAlarm
        {
            get { return m_ALM_FrontSteerLeftChangedAlarm; }
            set { m_ALM_FrontSteerLeftChangedAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_RearSteerLeftChangedAlarm
        {
            get { return m_ALM_RearSteerLeftChangedAlarm; }
            set { m_ALM_RearSteerLeftChangedAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_FrontSteerRightChangedAlarm
        {
            get { return m_ALM_FrontSteerRightChangedAlarm; }
            set { m_ALM_FrontSteerRightChangedAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_RearSteerRightChangedAlarm
        {
            get { return m_ALM_RearSteerRightChangedAlarm; }
            set { m_ALM_RearSteerRightChangedAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_FrontSteerLeftChangedInterlockAlarm
        {
            get { return m_ALM_FrontSteerLeftChangedInterlockAlarm; }
            set { m_ALM_FrontSteerLeftChangedInterlockAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_RearSteerLeftChangedInterlockAlarm
        {
            get { return m_ALM_RearSteerLeftChangedInterlockAlarm; }
            set { m_ALM_RearSteerLeftChangedInterlockAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_FrontSteerRightChangedInterlockAlarm
        {
            get { return m_ALM_FrontSteerRightChangedInterlockAlarm; }
            set { m_ALM_FrontSteerRightChangedInterlockAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_RearSteerRightChangedInterlockAlarm
        {
            get { return m_ALM_RearSteerRightChangedInterlockAlarm; }
            set { m_ALM_RearSteerRightChangedInterlockAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_FrontSteerOutputOffTimeoverAlarm
        {
            get { return m_ALM_FrontSteerOutputOffTimeoverAlarm; }
            set { m_ALM_FrontSteerOutputOffTimeoverAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_RearSteerOutputOffTimeoverAlarm
        {
            get { return m_ALM_RearSteerOutputOffTimeoverAlarm; }
            set { m_ALM_RearSteerOutputOffTimeoverAlarm = value; }
        }
        #endregion

        #region Constructor
        public DevSteer()
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
            if (m_FrontSteer.IsValid) ok &= m_FrontSteer.Initialize(this.ToString(), false, heavy_alarm) ? true : false;
            if (m_RearSteer.IsValid) ok &= m_RearSteer.Initialize(this.ToString(), false, heavy_alarm) ? true : false;

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
            m_ALM_NotDefine = AlarmListProvider.Instance.NewAlarm(AlarmCode.ParameterControlError, true, MyName, ParentName, "Not Define Alarm");
            m_ALM_FrontSteerLeftChangedAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, ParentName, "Front Left Changed Alarm");
            m_ALM_RearSteerLeftChangedAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, ParentName, "Rear Left Changed Alarm");
            m_ALM_FrontSteerRightChangedAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, ParentName, "Front Right Changed Alarm");
            m_ALM_RearSteerRightChangedAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, ParentName, "Rear Right Changed Alarm");

            m_ALM_FrontSteerLeftChangedInterlockAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, ParentName, "Front Left Changed Interlock Alarm");
            m_ALM_RearSteerLeftChangedInterlockAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, ParentName, "Rear Left Changed Interlock Alarm");
            m_ALM_FrontSteerRightChangedInterlockAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, ParentName, "Front Right Changed Interlock Alarm");
            m_ALM_RearSteerRightChangedInterlockAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, ParentName, "Rear Right Changed Interlock Alarm");

            m_ALM_FrontSteerOutputOffTimeoverAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, false, MyName, ParentName, "Front Output Off Timeover Alarm");
            m_ALM_RearSteerOutputOffTimeoverAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, false, MyName, ParentName, "Rear Output Off Timeover Alarm");
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            if (ok)
            {
                m_SeqActionFront = new SeqAction(this, true, 0.0f);
                m_SeqActionRear = new SeqAction(this, false, FrontRearOffset); // 동시 변경할려면 FrontRearOffet을 0으로 하면 된다.
                m_SeqMonitorFront = new SeqMonitor(this, true, 0.0f); // Front
                m_SeqMonitorRear = new SeqMonitor(this, false, FrontRearOffset); // Rear
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
            if (!Initialized) return;

            //m_SeqActionFront.SeqAbort();
            //m_SeqActionRear.SeqAbort();
            //m_SeqMonitorFront.SeqAbort();
            //m_SeqMonitorRear.SeqAbort();
        }
        public enSteerDirection GetFrontSteerDirection()
        {
            enSteerDirection cur_dir = m_FrontSteer.GetFw(-1) ? enSteerDirection.Left :
                            m_FrontSteer.GetBw(-1) ? enSteerDirection.Right : enSteerDirection.DontCare;
            return cur_dir;
        }
        public enSteerDirection GetRearSteerDirection()
        {
            enSteerDirection cur_dir = m_RearSteer.GetFw(-1) ? enSteerDirection.Left :
                            m_RearSteer.GetBw(-1) ? enSteerDirection.Right : enSteerDirection.DontCare;
            return cur_dir;
        }
        public int FrontLeft()
        {
            if (!Initialized) return ALM_NotDefine.ID;
            int rv = -1;
            rv = m_FrontSteer.SetFw();
            return rv;
        }
        public int FrontRight()
        {
            if (!Initialized) return ALM_NotDefine.ID;
            int rv = -1;
            rv = m_FrontSteer.SetBw();
            return rv;
        }
        public int RearLeft()
        {
            if (!Initialized) return ALM_NotDefine.ID;
            int rv = -1;
            rv = m_RearSteer.SetFw();
            return rv;
        }
        public int RearRight()
        {
            if (!Initialized) return ALM_NotDefine.ID;
            int rv = -1;
            rv = m_RearSteer.SetBw();
            return rv;
        }
        #endregion

        #region Sequence
        private class SeqMonitor : XSeqFunc
        {
            #region Field
            private DevSteer m_Device = null;
            private _DevCylinder m_Steer = null;
            private double m_SteerOffset = 0.0f;
            private bool m_FrontSteer = false;
            private double m_SteerChangeTime = 0.5f;
            private double m_SteerOutputOffTime = 2.0f;
            private double m_SteerChangeDelayDistance = 0.0f;

            Data.Process.Path m_BeforePath = new Data.Process.Path();
            private int m_OldLinkID = 0;
            #endregion

            public SeqMonitor(DevSteer device, bool frontSteer, double offset)
            {
                this.SeqName = $"SeqMonitor{device.MyName}";
                m_Device = device;
                m_FrontSteer = frontSteer;
                m_SteerOffset = offset; //동시 동작하기 위해서는 offset을  
                m_Steer = frontSteer ? m_Device.FrontSteer : m_Device.RearSteer;
                m_SteerChangeTime = m_Device.SteerChangeTime;
                m_SteerOutputOffTime = m_Device.SteerOutputOffTime;
                m_SteerChangeDelayDistance = m_Device.ChangeDelayDistance;
                TaskDeviceControlHighSpeed.Instance.RegSeq(this);
            }
            public override void SeqAbort()
            {
                // Reset Alarm을 하니까 자동 Recovery가 되어 버린다.
                if (GV.SteerNotChangeInterlock == false)
                {
                    if (AlarmId > 0)
                    {
                        EqpAlarm.Reset(AlarmId);
                        AlarmId = 0;
                    }
                    m_Steer.SeqAbort();
                    this.InitSeq();
                }
            }
            public override int Do()
            {
                int seqNo = SeqNo;
                int rv = -1;

                if (!m_Device.Initialized) return rv;

                Sineva.VHL.Data.Process.Path myPath = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath;
                BcrStatus curBcrStatus = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus;
                int linkID = myPath.LinkID;
                if (linkID == 0) return rv;
                if (m_OldLinkID != linkID)
                {
                    m_OldLinkID = linkID;
                    Data.Process.Path beforePath = ProcessDataHandler.Instance.CurTransferCommand.GetFromPath(myPath);
                    if (beforePath == null) m_BeforePath = ObjectCopier.Clone(myPath);
                    else m_BeforePath = ObjectCopier.Clone(beforePath);
                    if (AppConfig.Instance.Simulation.MY_DEBUG)
                        SequenceDeviceLog.WriteLog(string.Format("SeqMonitor SteerChangeEnable Change Path : {0} => {1}", beforePath, myPath));
                }

                switch (seqNo)
                {
                    case 0:
                        {
                            int set_alarmId = m_FrontSteer ? m_Device.ALM_FrontSteerOutputOffTimeoverAlarm.ID : m_Device.ALM_RearSteerOutputOffTimeoverAlarm.ID;
                            // 출력 상태가 변하면 무조건 설정 시간내에 OFF 시켜야 한다.
                            bool output_left = false;
                            bool output_right = false;
                            foreach (IoTag io in m_Steer.DoSolFw) if (io.GetChannel() != null) output_left |= io.GetDi();
                            foreach (IoTag io in m_Steer.DoSolBw) if (io.GetChannel() != null) output_right |= io.GetDi();
                            if (output_left || output_right)
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} Output State [LEFT={2}, RIGHT={3}]",
                                    m_Device.MyName, m_Steer.MyName, output_left ? "ON" : "OFF", output_right ? "ON" : "OFF"));

                                StartTicks = XFunc.GetTickCount(); // 记录开始时间
                                seqNo = 10;
                            }
                            else if (AlarmId == set_alarmId)
                            {
                                if (XFunc.GetTickCount() - StartTicks > m_SteerOutputOffTime * 1000.0f)
                                {
                                    SequenceDeviceLog.WriteLog(string.Format("Alarm Reset : Steer Output Off Timeover Alarm"));
                                    // Reset Alarm
                                    AlarmId = set_alarmId;
                                    EqpAlarm.Reset(AlarmId);
                                    AlarmId = 0;
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            bool check_enable = true;
                            if (linkID != 0)
                            {
                                // 짧은 Link 같은 경우 이어 붙이는 작업을 해야 할것 같다....
                                double beforePathDistance = 0.0f;
                                if (m_BeforePath != null && m_BeforePath.LinkID != 0)
                                {
                                    bool apply_before = true;
                                    apply_before &= m_BeforePath.SteerDirection == myPath.SteerDirection;
                                    apply_before &= m_BeforePath.Type == myPath.Type;
								    apply_before &= myPath.Type == LinkType.Straight;
                                    if (m_BeforePath.Distance < 1.1f * m_SteerOffset || apply_before) 
                                    {
                                        beforePathDistance = m_BeforePath.Distance;
                                    }
                                }

                                double curBcrPos = myPath.CurrentPositionOfLink;
                                double change_position = 0.0f;
                                if (myPath.SteerChangeLeftBCR > 0 || myPath.SteerChangeRightBCR > 0)
                                {
                                    double midDistance = myPath.BcrDirection == enBcrCheckDirection.Right ?
                                        myPath.SteerChangeRightBCR - myPath.RightBCRBegin : myPath.SteerChangeLeftBCR - myPath.LeftBCRBegin;
                                    change_position = curBcrPos - (midDistance + m_SteerOffset);
                                }
                                else
                                {
                                    curBcrPos += beforePathDistance;
                                    change_position = curBcrPos - (myPath.SteerGuideLengthFromNode + m_SteerOffset + m_SteerChangeDelayDistance);
                                }

                                 check_enable &= change_position > 0.0f;
                            }
                            // 전방감지로 이동을 못하는 경우에는 거리가 변하지 않을건데 이때는 어떡하지 ? 시간을 잡아두자 ~~
                            if (ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.MxpOverrideRatio == 0.0f || !check_enable)
                            {
                                StartTicks = XFunc.GetTickCount();
                            }
                            // 출력이 나갔고 정상적으로 입력이 들어왔을 경우
                            // 출력은 나갔는데 시간이 지났는데 출력이 계속 나가면 강제 OFF
                            bool time_over = XFunc.GetTickCount() - StartTicks > m_SteerOutputOffTime * 1000.0f;

                            bool input_left = false;
                            bool input_right = false;
                            bool output_left = false;
                            bool output_right = false;
                            foreach (IoTag io in m_Steer.DiSensorFw) if (io.GetChannel() != null) input_left |= io.GetDi();
                            foreach (IoTag io in m_Steer.DiSensorBw) if (io.GetChannel() != null) input_right |= io.GetDi();
                            foreach (IoTag io in m_Steer.DoSolFw) if (io.GetChannel() != null) output_left |= io.GetDi();
                            foreach (IoTag io in m_Steer.DoSolBw) if (io.GetChannel() != null) output_right |= io.GetDi();

                            if (output_left && check_enable && !time_over)
                            {
                                uint elapsedTicks = XFunc.GetTickCount() - StartTicks; // 计算经过的时间
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} State Check Confirm [Output={2}, Input={3}] - Time: {4}ms",
                                    m_Device.MyName, m_Steer.MyName, output_left ? "LEFT" : "RIGHT", input_left ? "LEFT" : "RIGHT", elapsedTicks));
                                StartTicks = XFunc.GetTickCount(); // 重置开始时间
                                seqNo = 100;
                            }
                            else if (output_right && check_enable && !time_over)
                            {
                                uint elapsedTicks = XFunc.GetTickCount() - StartTicks; // 计算经过的时间
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} State Check Confirm [Output={2}, Input={3}] - Time: {4}ms",
                                    m_Device.MyName, m_Steer.MyName, output_right ? "RIGHT" : "LEFT", input_right ? "RIGHT" : "LEFT", elapsedTicks));
                                StartTicks = XFunc.GetTickCount(); // 重置开始时间
                                seqNo = 150;
                            }
                            else if (time_over)
                            {
                                bool normal = false;
                                enSteerDirection mySteerDirection = myPath.SteerDirection;
                                if (myPath.SteerChangeLeftBCR > 0 || myPath.SteerChangeRightBCR > 0)
                                {
                                    if (check_enable && myPath.SteerDirection != enSteerDirection.DontCare)
                                    {
                                        mySteerDirection = myPath.SteerDirection == enSteerDirection.Left ? enSteerDirection.Right : enSteerDirection.Left;
                                    }
                                }
                                normal |= (mySteerDirection == enSteerDirection.DontCare || mySteerDirection == enSteerDirection.Left) && input_left;
                                normal |= (mySteerDirection == enSteerDirection.DontCare || mySteerDirection == enSteerDirection.Right) && input_right;
                                if (normal)
                                {
                                    m_Steer.ResetOutput();
                                    SequenceDeviceLog.WriteLog(string.Format("{0}-{1} State Check OK [Output={2}, Input={3}]", m_Device.MyName, m_Steer.MyName, output_left ? "LEFT" : "RIGHT", input_left ? "LEFT" : "RIGHT"));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 0;
                                }
                                else
                                {
                                    m_Steer.ResetOutput();

                                    // Set Alarm
                                    SequenceDeviceLog.WriteLog(string.Format("{0}-{1} Output Off Timeover. Input State [LEFT={2}, RIGHT={3}], Output State [LEFT={4}, RIGHT={5}]",
                                        m_Device.MyName, m_Steer.MyName, input_left ? "ON" : "OFF", input_right ? "ON" : "OFF", output_left ? "ON" : "OFF", output_right ? "ON" : "OFF"));

                                    int set_alarmId = m_FrontSteer ? m_Device.ALM_FrontSteerOutputOffTimeoverAlarm.ID : m_Device.ALM_RearSteerOutputOffTimeoverAlarm.ID;
                                    AlarmId = set_alarmId;
                                    EqpAlarm.Set(AlarmId);
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 0; // 처리가 애매하네... off 되었어야 하는데 왜 않돼었지 ? ... 수동으로 ON 되었나 ? 경고만 주자 !
                                }
                            }
                        }
                        break;

                    case 100:
                        {
                            bool output_right = false;
                            foreach (IoTag io in m_Steer.DoSolBw) if (io.GetChannel() != null) output_right |= io.GetDi();

                            bool change_ok = m_Steer.GetFw(-1);
                            //if (AppConfig.Instance.Simulation.MY_DEBUG) change_ok = false; // Steer change interlock 확인해 보자~~
                            if (change_ok)
                            {
                                uint elapsedTicks = XFunc.GetTickCount() - StartTicks; // 计算经过的时间
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} State Check OK [Output={2}, Input={3}] - Time: {4}ms",
                                    m_Device.MyName, m_Steer.MyName, m_Steer.GetFwState(-1) ? "LEFT" : "RIGHT", m_Steer.GetFw(-1) ? "LEFT" : "RIGHT", elapsedTicks));
                                GV.SteerNotChangeInterlock = false;
                                seqNo = 200;
                            }
                            else if (myPath.SteerDirection == enSteerDirection.Right || output_right)
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} Steer Output Changed [Output={2}, Input={3}]", 
                                    m_Device.MyName, m_Steer.MyName, 
                                    m_Steer.GetFwState(-1) ? "LEFT" : "RIGHT", m_Steer.GetFw(-1) ? "LEFT" : "RIGHT"));
                                GV.SteerNotChangeInterlock = false;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_SteerChangeTime * 1000.0f)
                            {
                                // Set Alarm
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} State Check Timeover [Output={2}, Input={3}]", 
                                    m_Device.MyName, m_Steer.MyName, 
                                    m_Steer.GetFwState(-1) ? "LEFT" : "RIGHT", m_Steer.GetFw(-1) ? "LEFT" : "RIGHT"));
                                GV.SteerNotChangeInterlock = true;
                                m_Steer.SeqAbort();

                                if (m_FrontSteer) AlarmId = m_Device.ALM_FrontSteerLeftChangedAlarm.ID;
                                else AlarmId = m_Device.ALM_RearSteerLeftChangedAlarm.ID;
                                EqpAlarm.Set(AlarmId);
                                SequenceDeviceLog.WriteLog(string.Format("Alarm Set : {0} Left Not Changed Alarm", m_Steer.MyName));
                                StartTicks = XFunc.GetTickCount();

                                seqNo = 120;
                            }
                        }
                        break;

                    case 120:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break; // SetAlarm 처리될 시간을 기다리자 ~~ 그래야 Recovery가 가능.

                            bool output_right = false;
                            foreach (IoTag io in m_Steer.DoSolBw) if (io.GetChannel() != null) output_right |= io.GetDi();

                            bool change_ok = m_Steer.GetFw(-1);
                            change_ok |= output_right;
                            change_ok &= IsPushedSwitch.m_AlarmRstPushed;
                            //if (AppConfig.Instance.Simulation.MY_DEBUG) change_ok = false; // Steer change interlock 확인해 보자~~
                            if (change_ok)
                            {
                                IsPushedSwitch.m_AlarmRstPushed = false;
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} State Check OK [Output={2}, Input={3}]", 
                                    m_Device.MyName, m_Steer.MyName, 
                                    m_Steer.GetFwState(-1) ? "LEFT" : "RIGHT", m_Steer.GetFw(-1) ? "LEFT" : "RIGHT"));
                                GV.SteerNotChangeInterlock = false;

                                if (m_FrontSteer) AlarmId = m_Device.ALM_FrontSteerLeftChangedAlarm.ID;
                                else AlarmId = m_Device.ALM_RearSteerLeftChangedAlarm.ID;
                                EqpAlarm.Reset(AlarmId);
                                SequenceDeviceLog.WriteLog(string.Format("Alarm Reset : {0} Left Not Changed Alarm", m_Steer.MyName));

                                AlarmId = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 150:
                        {
                            bool output_left = false;
                            foreach (IoTag io in m_Steer.DoSolFw) if (io.GetChannel() != null) output_left |= io.GetDi();

                            bool change_ok = m_Steer.GetBw(-1);
                            if (change_ok)
                            {
                                uint elapsedTicks = XFunc.GetTickCount() - StartTicks; // 计算经过的时间
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} State Check OK [Output={2}, Input={3}] - Time: {4}ms",
                                    m_Device.MyName, m_Steer.MyName, m_Steer.GetBwState(-1) ? "RIGHT" : "LEFT", m_Steer.GetBw(-1) ? "RIGHT" : "LEFT", elapsedTicks));
                                GV.SteerNotChangeInterlock = false;
                                seqNo = 200;
                            }
                            else if (myPath.SteerDirection == enSteerDirection.Left || output_left)
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} Steer Output Changed [Output={2}, Input={3}]", 
                                    m_Device.MyName, m_Steer.MyName,
                                    m_Steer.GetBwState(-1) ? "RIGHT" : "LEFT", m_Steer.GetBw(-1) ? "RIGHT" : "LEFT"));
                                GV.SteerNotChangeInterlock = false;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_SteerChangeTime * 1000.0f)
                            {
                                // Set Alarm
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} State Check Timeover [Output={2}, Input={3}]", 
                                    m_Device.MyName, m_Steer.MyName, 
                                    m_Steer.GetBwState(-1) ? "RIGHT" : "LEFT", m_Steer.GetBw(-1) ? "RIGHT" : "LEFT"));
                                GV.SteerNotChangeInterlock = true;
                                m_Steer.SeqAbort();

                                if (m_FrontSteer) AlarmId = m_Device.ALM_FrontSteerRightChangedAlarm.ID;
                                else AlarmId = m_Device.ALM_RearSteerRightChangedAlarm.ID;
                                EqpAlarm.Set(AlarmId);
                                SequenceDeviceLog.WriteLog(string.Format("Alarm Set : {0} Right Not Changed Alarm", m_Steer.MyName));

                                StartTicks = XFunc.GetTickCount();
                                seqNo = 160;
                            }
                        }
                        break;

                    case 160:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break; // SetAlarm 처리될 시간을 기다리자 ~~ 그래야 Recovery가 가능.
                            bool output_left = false;
                            foreach (IoTag io in m_Steer.DoSolFw) if (io.GetChannel() != null) output_left |= io.GetDi();

                            bool change_ok = m_Steer.GetBw(-1);
                            change_ok |= output_left;
                            change_ok &= IsPushedSwitch.m_AlarmRstPushed;
                            if (change_ok)
                            {
                                IsPushedSwitch.m_AlarmRstPushed = false;
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} State Check OK [Output={2}, Input={3}]",
                                    m_Device.MyName, m_Steer.MyName,
                                    m_Steer.GetBwState(-1) ? "RIGHT" : "LEFT", m_Steer.GetBw(-1) ? "RIGHT" : "LEFT"));
                                GV.SteerNotChangeInterlock = false;

                                if (m_FrontSteer) AlarmId = m_Device.ALM_FrontSteerRightChangedAlarm.ID;
                                else AlarmId = m_Device.ALM_RearSteerRightChangedAlarm.ID;
                                EqpAlarm.Reset(AlarmId);
                                SequenceDeviceLog.WriteLog(string.Format("Alarm Reset : {0} Right Not Changed Alarm", m_Steer.MyName));

                                AlarmId = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 200:
                        {
                            // Output Off wait
                            bool output_left = false;
                            bool output_right = false;
                            foreach (IoTag io in m_Steer.DoSolFw) if (io.GetChannel() != null) output_left |= io.GetDi();
                            foreach (IoTag io in m_Steer.DoSolBw) if (io.GetChannel() != null) output_right |= io.GetDi();
                            if (!output_left && !output_right)
                            {
                                uint elapsedTicks = XFunc.GetTickCount() - StartTicks; // 计算经过的时间
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} Output State OFF Confirm [LEFT={2}, RIGHT={3}] - Time: {4}ms",
                                    m_Device.MyName, m_Steer.MyName, output_left ? "ON" : "OFF", output_right ? "ON" : "OFF", elapsedTicks));
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
            private DevSteer m_Device = null;
            private _DevCylinder m_Steer = null;
            private double m_SteerOffset = 0.0f;
            private double m_SteerChangeDelay = 0.0f;
            private double m_SCurveSteerOffset = 0.0f;
            private int m_OldLinkID = 0;
            private Data.Process.Path m_BeforePath = new Data.Process.Path();
            private bool m_FrontSteer = false;
            #endregion

            public SeqAction(DevSteer device, bool frontSteer, double offset)
            {
                this.SeqName = $"SeqAction{device.MyName}";
                m_Device = device;
                m_FrontSteer = frontSteer;
                m_Steer = frontSteer ? m_Device.FrontSteer : m_Device.RearSteer;
                m_SteerOffset = offset; //동시 동작하기 위해서는 offset을 0.0으로 
                m_SCurveSteerOffset = offset;
                m_SteerChangeDelay = m_Device.ChangeDelayDistance;
                //EventHandlerManager.Instance.LinkChanged += Instance_LinkChanged;
                TaskDeviceControlHighSpeed.Instance.RegSeq(this);
            }
            public override void SeqAbort()
            {
                if (AlarmId > 0)
                {
                    EqpAlarm.Reset(AlarmId);
                    AlarmId = 0;
                }
                m_Steer.SeqAbort();
                this.InitSeq();
            }
            private void Instance_LinkChanged(object obj)
            {
                Data.Process.Path myPath = (Data.Process.Path)obj;
                if (myPath.LinkID == ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LinkID)
                {
                    if (myPath.Type == LinkType.Straight) SeqAbort(); // Alarm 상황에서 Link가 바뀌면 SeqAction을 처음부터 돌리자 !
                }
            }

            public override int Do()
            {
                int rv = -1;
                if (!m_Device.Initialized) return rv;

                Sineva.VHL.Data.Process.Path myPath = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath;
                BcrStatus curBcrStatus = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus;
                int linkID = myPath.LinkID;
                if (linkID == 0) return rv;
                if (m_OldLinkID != linkID)
                {
                    m_OldLinkID = linkID;
                    Data.Process.Path beforePath = ProcessDataHandler.Instance.CurTransferCommand.GetFromPath(myPath);
                    if (beforePath == null) m_BeforePath = ObjectCopier.Clone(myPath);
                    else m_BeforePath = ObjectCopier.Clone(beforePath);
                    if (AppConfig.Instance.Simulation.MY_DEBUG)
                        SequenceDeviceLog.WriteLog(string.Format("SeqAction SteerChangeEnable Change Path : {0} => {1}", beforePath, myPath));

                    if (myPath.Type == LinkType.Straight) SeqAbort(); // Alarm 상황에서 Link가 바뀌면 SeqAction을 처음부터 돌리자 !
                }
                enBcrCheckDirection bcrDirection = curBcrStatus.BcrDirection;
                double leftBcr = curBcrStatus.LeftBcr;
                double rightBcr = curBcrStatus.RightBcr;
                bool sCurve = false;
                bool isChangeEnable = IsSteerChangeEnableArea(myPath, ref sCurve);

                int seqNo = SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            if (EqpStateManager.Instance.OpMode == OperateMode.Auto)
                            //if (EqpStateManager.Instance.RunMode == EqpRunMode.Start)
                            {
                                // Aging ///////////////////////////////////////////////////////////////////////////////////////////////////
                                bool aging = ProcessDataHandler.Instance.CurTransferCommand.IsValid;
                                aging &= ProcessDataHandler.Instance.CurTransferCommand.ProcessCommand == IF.OCS.OCSCommand.CycleSteerAging;
                                if (aging) break;
                                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
 
                                if (isChangeEnable)
                                {
                                    enSteerDirection set_dir = myPath.SteerDirection;
                                    if (sCurve) set_dir = set_dir == enSteerDirection.Left ? enSteerDirection.Right :
                                                                set_dir == enSteerDirection.Right ? enSteerDirection.Left : enSteerDirection.DontCare;  // S-Curve 중간점에서 Steer 변경해야 할 경우.
                                    enSteerDirection cur_dir = m_Steer.GetFw(-1) ? enSteerDirection.Left :
                                                                m_Steer.GetBw(-1) ? enSteerDirection.Right : enSteerDirection.DontCare;
                                    if (set_dir != cur_dir && set_dir != enSteerDirection.DontCare)
                                    {
                                        SequenceDeviceLog.WriteLog(string.Format("{0}-{1} Change [Steer={2}, BCR=({3},{4}), Use={5}]", m_Device.MyName, m_Steer.MyName, set_dir, leftBcr, rightBcr, bcrDirection));
                                        m_Steer.SeqAbort();
                                        StartTicks = XFunc.GetTickCount();
                                        if (set_dir == enSteerDirection.Right) seqNo = 20;
                                        else seqNo = 10;
                                    }
                                    //else if (remainBcrPos < m_Device.SteerOutputRemainDistance && cur_velocity > 100.0f && myPath.Distance > 3000.0f)
                                    //{
                                    //    Sineva.VHL.Data.Process.Path nextPath = ProcessDataHandler.Instance.CurTransferCommand.GetToPath(myPath);
                                    //    if (nextPath != null)
                                    //    {
                                    //        bool steerOut = false;
                                    //        steerOut |= nextPath.Type == LinkType.LeftCurve;
                                    //        steerOut |= nextPath.Type == LinkType.RightCurve;
                                    //        steerOut |= nextPath.Type == LinkType.LeftBranch;
                                    //        steerOut |= nextPath.Type == LinkType.RightBranch;
                                    //        steerOut |= nextPath.Type == LinkType.LeftSBranch;
                                    //        steerOut |= nextPath.Type == LinkType.RightSBranch;
                                    //        steerOut |= nextPath.Type == LinkType.LeftBranchStraight;
                                    //        steerOut |= nextPath.Type == LinkType.RightBranchStraight;
                                    //        if (steerOut)
                                    //        {
                                    //            SequenceDeviceLog.WriteLog(string.Format("{0}-{1} Change Path End [Steer={2}, BCR=({3},{4}), Use={5}]", m_Device.MyName, m_Steer.MyName, set_dir, leftBcr, rightBcr, bcrDirection));
                                    //            m_Steer.SeqAbort();
                                    //            StartTicks = XFunc.GetTickCount();
                                    //            if (set_dir == enSteerDirection.Right) seqNo = 20;
                                    //            else seqNo = 10;
                                    //        }
                                    //    }
                                    //}
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            rv = m_Steer.SetFw(-1);
                            if (rv == 0)
                            {
                                UInt32 time = XFunc.GetTickCount() - StartTicks;
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} Change Left OK [BCR=({2},{3}), Use={4}, Time={5}ms]", m_Device.MyName, m_Steer.MyName, leftBcr, rightBcr, bcrDirection, time));
                                seqNo = 30;
                            }
                            else if (rv > 0)
                            {
                                // Set Alarm
                                UInt32 time = XFunc.GetTickCount() - StartTicks;
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} Change Left NG [BCR=({2},{3}), Use={4}, Time={5}ms]", m_Device.MyName, m_Steer.MyName, leftBcr, rightBcr, bcrDirection, time));
                                m_Steer.SeqAbort();

                                if (m_FrontSteer) AlarmId = m_Device.ALM_FrontSteerLeftChangedAlarm.ID;
                                else AlarmId = m_Device.ALM_RearSteerLeftChangedAlarm.ID;
                                EqpAlarm.Set(AlarmId);
                                seqNo = 1000;
                            }
                            else if (!isChangeEnable)
                            {
                                UInt32 time = XFunc.GetTickCount() - StartTicks;
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} Steer Change Invalid Area, Reschedule [BCR=({2},{3}), Use={4}, Time={5}ms]", m_Device.MyName, m_Steer.MyName, leftBcr, rightBcr, bcrDirection, time));

                                if (m_FrontSteer) m_Device.m_SeqMonitorFront.SeqAbort();
                                else m_Device.m_SeqMonitorRear.SeqAbort();
                                seqNo = 0;
                            }
                        }
                        break;

                    case 20:
                        {
                            rv = m_Steer.SetBw(-1);
                            if (rv == 0)
                            {
                                UInt32 time = XFunc.GetTickCount() - StartTicks;
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} Change Right OK [BCR=({2},{3}), Use={4}, Time={5}ms]]", m_Device.MyName, m_Steer.MyName, leftBcr, rightBcr, bcrDirection, time));
                                seqNo = 30;
                            }
                            else if (rv > 0)
                            {
                                // Set Alarm
                                UInt32 time = XFunc.GetTickCount() - StartTicks;
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} Change Right NG [BCR=({2},{3}), Use={4}, Time={5}ms]]", m_Device.MyName, m_Steer.MyName, leftBcr, rightBcr, bcrDirection, time));
                                m_Steer.SeqAbort();

                                if (m_FrontSteer) AlarmId = m_Device.ALM_FrontSteerRightChangedAlarm.ID;
                                else AlarmId = m_Device.ALM_RearSteerRightChangedAlarm.ID;
                                EqpAlarm.Set(AlarmId);
                                seqNo = 1000;
                            }
                            else if (!isChangeEnable)
                            {
                                UInt32 time = XFunc.GetTickCount() - StartTicks;
                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} Steer Change Invalid Area, Reschedule [BCR=({2},{3}), Use={4}, Time={5}ms]", m_Device.MyName, m_Steer.MyName, leftBcr, rightBcr, bcrDirection, time));

                                if (m_FrontSteer) m_Device.m_SeqMonitorFront.SeqAbort();
                                else m_Device.m_SeqMonitorRear.SeqAbort();
                                seqNo = 0;
                            }
                        }
                        break;

                    case 30:
                        {
                            enSteerDirection set_dir = myPath.SteerDirection;
                            enSteerDirection cur_dir = m_Steer.GetFw(-1) ? enSteerDirection.Left :
                            m_Steer.GetBw(-1) ? enSteerDirection.Right : enSteerDirection.DontCare;
                            if (sCurve) set_dir = cur_dir;  // S-Curve 구간에서는 생각할 필요가 없다.

                            if (XFunc.GetTickCount() - StartTicks > m_Device.SteerOutputOffTime * 1000.0f)
                            {
                                m_Steer.ResetOutput();
                                seqNo = 0;
                            }
                            else if (set_dir != cur_dir)
                            {
                                seqNo = 0; //혹시 구간이 짧아서 설정시간이 지나기 전에 다음 link에 도착할 경우 착각할수 있다.
                            }
                        }
                        break;

                    case 1000:
                        {
                            if (IsPushedSwitch.IsAlarmReset)
                            {
                                IsPushedSwitch.m_AlarmRstPushed = false;

                                SequenceDeviceLog.WriteLog(string.Format("{0}-{1} Alarm Reset", m_Device.MyName, m_Steer.MyName));
                                EqpAlarm.Reset(AlarmId);
                                AlarmId = 0;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                SeqNo = seqNo;
                return rv;
            }

            private bool IsSteerChangeEnableArea(Sineva.VHL.Data.Process.Path myPath, ref bool sCurve)
            {
                bool rv = false;
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Steer Change Enable Area 판단을 하자!!!
                double cur_velocity = ProcessDataHandler.Instance.CurVehicleStatus.MasterWheelCMDVelocity; //ProcessDataHandler.Instance.CurVehicleStatus.MasterWheelVelocity;
                double cur_override = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.MxpOverrideRatio;
                /// Next Path가 Curve or Branch 일 경우 myPath.RemainDistance가 <200일때 속도가 50mm/sec 이상일때 Steer 출력을 주자~~
                double remainBcrPos = myPath.RemainDistanceOfLink;
                // Check 가능한 영역을 정하자 ~~ 전방감지일때는 시간이 넉넉하니 SteerOffset 적용 
                double steer_offset = m_SteerOffset;
                bool TypeSCurve = false;
                TypeSCurve |= myPath.Type == LinkType.LeftCompositedSCurveBranch;
                TypeSCurve |= myPath.Type == LinkType.LeftCompositedSCurveJunction;
                TypeSCurve |= myPath.Type == LinkType.RightCompositedSCurveBranch;
                TypeSCurve |= myPath.Type == LinkType.RightCompositedSCurveJunction;
                TypeSCurve |= myPath.Type == LinkType.LeftSBranch;
                TypeSCurve |= myPath.Type == LinkType.RightSBranch;
                TypeSCurve |= myPath.Type == LinkType.LeftSJunction;
                TypeSCurve |= myPath.Type == LinkType.RightSJunction;
                double beforePathDistance = 0.0f;
                double beforeGuideLength = 0.0f;
                if (m_Device.FrontRearSyncChange && cur_override == 1.0f && cur_velocity > 0.0f && !TypeSCurve)
                {
                    steer_offset = 0.0f; //동시 변경.
                }
                else if (m_FrontSteer == false) // Rear Steer일 경우....LinkDistance가 짧은 경우....
                {
                    if (m_BeforePath != null && m_BeforePath.LinkID != 0 && m_BeforePath.Type == LinkType.Straight) // Before가 Straight인 경우 
                    {
                        double need_distance = m_BeforePath.Velocity * m_Device.SteerChangeTime + 1.1f * m_SteerOffset;

                        bool apply_before = true;
                        apply_before &= m_BeforePath.SteerDirection == myPath.SteerDirection;
                        apply_before &= m_BeforePath.Type == myPath.Type;
                        if (apply_before) // Straight 두개가 이어진 경우
                        {
                            beforePathDistance = m_BeforePath.Distance;
                            //beforeGuideLength = m_BeforePath.SteerGuideLengthFromNode;
                            if (m_BeforePath.Distance < m_BeforePath.SteerGuideLengthFromNode)
                                beforeGuideLength = m_BeforePath.Distance;
                            else
                                beforeGuideLength = m_BeforePath.SteerGuideLengthFromNode;
                        }
                        else if (m_BeforePath.Distance < need_distance) // Before Distance가 짧은 경우
                        {
                            beforePathDistance = m_BeforePath.Distance;
                            //beforeGuideLength = m_BeforePath.SteerGuideLengthFromNode;
                            if (m_BeforePath.Distance < m_BeforePath.SteerGuideLengthFromNode)
                                beforeGuideLength = m_BeforePath.Distance;
                            else
                                beforeGuideLength = m_BeforePath.SteerGuideLengthFromNode;
                        }
                    }
                }

                double curMotorPos = myPath.CurrentMotorPositionOfLink;
                double curBcrPos = myPath.CurrentPositionOfLink;
                double change_position = 0.0f;
                if (myPath.SteerChangeLeftBCR > 0 || myPath.SteerChangeRightBCR > 0)
                {
                    double midDistance = myPath.BcrDirection == enBcrCheckDirection.Right ?
                        myPath.SteerChangeRightBCR - myPath.RightBCRBegin : myPath.SteerChangeLeftBCR - myPath.LeftBCRBegin;
                    change_position = curBcrPos - (midDistance + m_SCurveSteerOffset);
                    sCurve = true;
                }
                else
                {
                    curBcrPos += beforePathDistance;
                    change_position = curBcrPos - (myPath.SteerGuideLengthFromNode + beforeGuideLength + steer_offset + m_SteerChangeDelay);
                }
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                rv = change_position > 0.0f ? true : false;

                if (AppConfig.Instance.Simulation.MY_DEBUG)
                {
                    if (GV.WheelBusy && rv)
                    {
                        enSteerDirection cur_dir = m_Steer.GetFw(-1) ? enSteerDirection.Left :
                                                    m_Steer.GetBw(-1) ? enSteerDirection.Right : enSteerDirection.DontCare;
                        if (myPath.SteerDirection != cur_dir)
                        {
                            SequenceDeviceLog.WriteLog(string.Format("SteerChangeEnable {0} : {1}-{2:F1}-{3:F1}-{4:F1}-{5:F1}-{6:F1}-{7:F1}-{8}-{9}", m_FrontSteer ? "Front" : "Rear", myPath.FromNodeID, change_position, myPath.CurrentPositionOfLink, beforePathDistance, myPath.SteerGuideLengthFromNode, steer_offset, cur_override, myPath.SteerDirection, cur_dir));
                        }
                    }
                }
                return rv;
            }
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

                var helperXml = new XmlHelper<DevSteer>();
                DevSteer dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.FrontSteer = dev.FrontSteer;
                    this.RearSteer = dev.RearSteer;
                    this.FrontRearOffset = dev.FrontRearOffset;
                    this.SteerChangeTime = dev.SteerChangeTime;
                    this.SteerOutputOffTime = dev.SteerOutputOffTime;
                    this.ChangeDelayDistance = dev.ChangeDelayDistance;
                    this.SteerOutputRemainDistance = dev.SteerOutputRemainDistance;

                    this.FrontRearSyncChange = dev.FrontRearSyncChange;
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
                var helperXml = new XmlHelper<DevSteer>();
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
