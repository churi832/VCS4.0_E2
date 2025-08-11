using Sineva.VHL.Data;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
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

namespace Sineva.VHL.Device
{
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class DevOpPanel : _Device
    {
        private const string DevName = "DevOpPanel";

        #region Field
        // Teaching Mode Sol
        private _DevSwitch m_SaftyKeySwitch = new _DevSwitch();
        private _DevOutput m_DoSaftyReset = new _DevOutput(); // Safty Relay
        // Tower Lamp
        private _DevOutput m_DoTowerLampRed1 = new _DevOutput();
        private _DevOutput m_DoTowerLampGreen1 = new _DevOutput();
        private _DevOutput m_DoTowerLampYellow1 = new _DevOutput();
        private _DevOutput m_DoTowerLampBlue1 = new _DevOutput();
        private _DevOutput m_DoTowerLampRed2 = new _DevOutput();
        private _DevOutput m_DoTowerLampGreen2 = new _DevOutput();
        private _DevOutput m_DoTowerLampYellow2 = new _DevOutput();
        private _DevOutput m_DoTowerLampBlue2 = new _DevOutput();
        // Melody
        private _DevOutput m_MelodyAlert = new _DevOutput();
        private _DevOutput m_MelodyWarning = new _DevOutput();
        private _DevOutput m_MelodyAlarm = new _DevOutput();
        private _DevOutput m_MelodyOpCall = new _DevOutput();
        // input
        private _DevInput m_DiPowerOn = new _DevInput();

        private SeqSwitchMonitor m_SeqMonitor = null;
        private SeqTowerLamp m_SeqTowerLamp = null;
        private SeqMelody m_SeqMelody = null;
        private SeqSaftyPlc m_SeqTeachingLock = null;

        private bool m_ForcedBuzzerOff = false;
        #endregion

        #region I/O, Device Relation
        [Category("!Setting Device (Switch)"), Description("Safty Key Reset Switch"), DeviceSetting(true)]
        public _DevSwitch SaftyKeySwitch
        {
            get { return m_SaftyKeySwitch; }
            set { m_SaftyKeySwitch = value; }
        }
        [Category("!Setting Device (Digital Output)"), Description("Safty Reset"), DeviceSetting(true)]
        public _DevOutput DoSaftyReset
        {
            get { return m_DoSaftyReset; }
            set { m_DoSaftyReset = value; }
        }
        [Category("!Setting Device (Digital Output)"), Description("Front Tower Lamp Red"), DeviceSetting(true)]
        public _DevOutput DoTowerLampRed1
        {
            get { return m_DoTowerLampRed1; }
            set { m_DoTowerLampRed1 = value; }
        }
        [Category("!Setting Device (Digital Output)"), Description("Front Tower Lamp Green"), DeviceSetting(true)]
        public _DevOutput DoTowerLampGreen1
        {
            get { return m_DoTowerLampGreen1; }
            set { m_DoTowerLampGreen1 = value; }
        }
        [Category("!Setting Device (Digital Output)"), Description("Front Tower Lamp Blue"), DeviceSetting(true)]
        public _DevOutput DoTowerLampBlue1
        {
            get { return m_DoTowerLampBlue1; }
            set { m_DoTowerLampBlue1 = value; }
        }
        [Category("!Setting Device (Digital Output)"), Description("Front Tower Lamp Yellow"), DeviceSetting(true)]
        public _DevOutput DoTowerLampYellow1
        {
            get { return m_DoTowerLampYellow1; }
            set { m_DoTowerLampYellow1 = value; }
        }
        [Category("!Setting Device (Digital Output)"), Description("Rear Tower Lamp Red"), DeviceSetting(true)]
        public _DevOutput DoTowerLampRed2
        {
            get { return m_DoTowerLampRed2; }
            set { m_DoTowerLampRed2 = value; }
        }
        [Category("!Setting Device (Digital Output)"), Description("Rear Tower Lamp Green"), DeviceSetting(true)]
        public _DevOutput DoTowerLampGreen2
        {
            get { return m_DoTowerLampGreen2; }
            set { m_DoTowerLampGreen2 = value; }
        }
        [Category("!Setting Device (Digital Output)"), Description("Rear Tower Lamp Blue"), DeviceSetting(true)]
        public _DevOutput DoTowerLampBlue2
        {
            get { return m_DoTowerLampBlue2; }
            set { m_DoTowerLampBlue2 = value; }
        }
        [Category("!Setting Device (Digital Output)"), Description("Rear Tower Lamp Yellow"), DeviceSetting(true)]
        public _DevOutput DoTowerLampYellow2
        {
            get { return m_DoTowerLampYellow2; }
            set { m_DoTowerLampYellow2 = value; }
        }

        [Category("!Setting Device (Melody Alert)"), Description(""), DeviceSetting(true)]
        public _DevOutput MelodyAlert
        {
            get { return m_MelodyAlert; }
            set { m_MelodyAlert = value; }
        }
        [Category("!Setting Device (Melody Warning)"), Description(""), DeviceSetting(true)]
        public _DevOutput MelodyWarning
        {
            get { return m_MelodyWarning; }
            set { m_MelodyWarning = value; }
        }
        [Category("!Setting Device (Melody Alarm)"), Description(""), DeviceSetting(true)]
        public _DevOutput MelodyAlarm
        {
            get { return m_MelodyAlarm; }
            set { m_MelodyAlarm = value; }
        }
        [Category("!Setting Device (Melody Op Call)"), Description(""), DeviceSetting(true)]
        public _DevOutput MelodyOpCall
        {
            get { return m_MelodyOpCall; }
            set { m_MelodyOpCall = value; }
        }

        [Category("!Setting Device (Digital Input)"), Description("Power ON"), DeviceSetting(true)]
        public _DevInput DiPowerOn
        {
            get { return m_DiPowerOn; }
            set { m_DiPowerOn = value; }
        }
        #endregion
        
        #region variable
        public bool ForcedBuzzerOff { get { return m_ForcedBuzzerOff; } set { m_ForcedBuzzerOff = value; } }
        #endregion

        #region Constructor
        public DevOpPanel()
        {
            if (!Initialized)
                this.MyName = DevName;
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
            if (SaftyKeySwitch.IsValid) ok &= SaftyKeySwitch.Initialize(this.ToString(), false, true) ? true : false;

            if (DoSaftyReset.IsValid) ok &= DoSaftyReset.Initialize(this.ToString(), false, true) ? true : false;
            if (DoTowerLampRed1.IsValid) ok &= DoTowerLampRed1.Initialize(this.ToString(), false, false) ? true : false;
            if (DoTowerLampGreen1.IsValid) ok &= DoTowerLampGreen1.Initialize(this.ToString(), false, false) ? true : false;
            if (DoTowerLampBlue1.IsValid) ok &= DoTowerLampBlue1.Initialize(this.ToString(), false, false) ? true : false;
            if (DoTowerLampYellow1.IsValid) ok &= DoTowerLampYellow1.Initialize(this.ToString(), false, false) ? true : false;
            if (DoTowerLampRed2.IsValid) ok &= DoTowerLampRed2.Initialize(this.ToString(), false, false) ? true : false;
            if (DoTowerLampGreen2.IsValid) ok &= DoTowerLampGreen2.Initialize(this.ToString(), false, false) ? true : false;
            if (DoTowerLampBlue2.IsValid) ok &= DoTowerLampBlue2.Initialize(this.ToString(), false, false) ? true : false;
            if (DoTowerLampYellow2.IsValid) ok &= DoTowerLampYellow2.Initialize(this.ToString(), false, false) ? true : false;

            if (MelodyAlert.IsValid) ok &= MelodyAlert.Initialize(this.ToString(), false, false) ? true : false;
            if (MelodyWarning.IsValid) ok &= MelodyWarning.Initialize(this.ToString(), false, false) ? true : false;
            if (MelodyAlarm.IsValid) ok &= MelodyAlarm.Initialize(this.ToString(), false, false) ? true : false;
            if (MelodyOpCall.IsValid) ok &= MelodyOpCall.Initialize(this.ToString(), false, false) ? true : false;
            if (DiPowerOn.IsValid) ok &= DiPowerOn.Initialize(this.ToString(), false, true) ? true : false;

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
                m_SeqMonitor = new SeqSwitchMonitor(this);
                m_SeqTowerLamp = new SeqTowerLamp(this);
                m_SeqMelody = new SeqMelody(this);
                m_SeqTeachingLock = new SeqSaftyPlc(this);
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

        #region Method - Teaching Lock Sol
        public void SetSaftyPlcReset(bool on)
        {
            if (!Initialized) return;
            if (m_DoSaftyReset.IsValid) m_DoSaftyReset.SetDo(on);
        }
        public bool IsPowerOn()
        {
            if (!Initialized) return false;
            bool rv = m_DiPowerOn.IsValid ? m_DiPowerOn.IsDetected : true;
            return rv;
        }
        public bool IsSaftyKeySwitch()
        {
            if (!Initialized) return false;
            bool rv = m_SaftyKeySwitch.IsValid ? m_SaftyKeySwitch.IsPushedSwitch : true;
            return rv;
        }
        #endregion

        #region Sequence
        private class SeqSwitchMonitor : XSeqFunc
        {
            #region Fields
            DevOpPanel m_Device = null;

            private _DevSwitch m_SwitchSaftyKey = null;
            #endregion

            #region Constructor
            public SeqSwitchMonitor(DevOpPanel device)
            {
                this.SeqName = $"SeqSwitchMonitor{device.MyName}";
                m_Device = device;
                m_SwitchSaftyKey = device.SaftyKeySwitch;

                TaskDeviceControl.Instance.RegSeq(this);
            }

            public override void SeqAbort()
            {
                this.InitSeq();
            }
            #endregion

            #region Methods
            public override int Do()
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                if (!m_Device.Initialized) return rv;

                switch (seqNo)
                {
                    case 0:
                        {
                            bool ok = true;
                            ok &= m_SwitchSaftyKey != null;
                            ok &= m_SwitchSaftyKey.IsValid;
                            ok &= IoManager.Instance.UpdateRun;

                            if (ok) seqNo = 10;
                        }
                        break;

                    case 10:
                        {
                            // Data.GV에서 BUZZER OFF, ALARM RESET을 감지하기 힘들다 그래서 여기서 꺼꾸로 알려 주자....
                            if (m_Device.SaftyKeySwitch.IsPushedSwitch)
                            {
                                if (m_Device.SaftyKeySwitch.GetLampState() != LampState.On)
                                {
                                    m_Device.SaftyKeySwitch.SetLamp(LampState.On);
                                    m_Device.DoSaftyReset.SetDo(true);
                                }
                            }
                            else
                            {
                                if (m_Device.SaftyKeySwitch.GetLampState() != LampState.Off)
                                {
                                    m_Device.SaftyKeySwitch.SetLamp(LampState.Off);
                                    m_Device.DoSaftyReset.SetDo(false);
                                }
                            }
                        }
                        break;
                }

                SeqNo = seqNo;
                return rv;
            }
            #endregion
        }

        private class SeqTowerLamp : XSeqFunc
        {
            #region Fields
            DevOpPanel m_Device = null;

            private List<_DevOutput> m_TowerLampRed = new List<_DevOutput>();
            private List<_DevOutput> m_TowerLampGreen = new List<_DevOutput>();
            private List<_DevOutput> m_TowerLampYellow = new List<_DevOutput>();
            private List<_DevOutput> m_TowerLampBlue = new List<_DevOutput>();

            private uint ToggleTicks = 0;
            private bool m_LampToggle = false;
            private bool m_LampRedToggle = false;
            private bool m_LampGreenToggle = false;
            private bool m_LampYellowToggle = false;
            private bool m_LampBlueToggle = false;
            private uint m_FrontDetectTicks = 0;
            private bool m_FrontDetectStart = false;
            private bool m_FistChange = false;
            #endregion

            #region Constructor
            public SeqTowerLamp(DevOpPanel device)
            {
                this.SeqName = $"SeqTowerLamp{device.MyName}";
                m_Device = device;

                if (device.m_DoTowerLampRed1.IsValid) m_TowerLampRed.Add(device.DoTowerLampRed1);
                if (device.m_DoTowerLampRed2.IsValid) m_TowerLampRed.Add(device.DoTowerLampRed2);
                if (device.m_DoTowerLampGreen1.IsValid) m_TowerLampGreen.Add(device.DoTowerLampGreen1);
                if (device.m_DoTowerLampGreen2.IsValid) m_TowerLampGreen.Add(device.DoTowerLampGreen2);
                if (device.m_DoTowerLampYellow1.IsValid) m_TowerLampYellow.Add(device.DoTowerLampYellow1);
                if (device.m_DoTowerLampYellow2.IsValid) m_TowerLampYellow.Add(device.DoTowerLampYellow2);
                if (device.m_DoTowerLampBlue1.IsValid) m_TowerLampBlue.Add(device.DoTowerLampBlue1);
                if (device.m_DoTowerLampBlue2.IsValid) m_TowerLampBlue.Add(device.DoTowerLampBlue2);

                TaskDeviceControl.Instance.RegSeq(this);
            }

            public override void SeqAbort()
            {
                this.InitSeq();
            }
            #endregion

            #region Methods
            public override int Do()
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                if (!m_Device.Initialized) return rv;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                                seqNo = 110;
                            else if (AppConfig.Instance.ProjectType == enProjectType.ESWIN)
                                seqNo = 210;
                            else
                                seqNo = 10;

                            ToggleTicks = XFunc.GetTickCount();
                        }
                        break;

                    #region Standard
                    case 10:
                        {
                            if (EqpStateManager.Instance.OpMode == OperateMode.Auto)
                            {
                                if (EqpStateManager.Instance.State == EqpState.Run)
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(false); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(true); }
                                }
                                else if (EqpStateManager.Instance.State == EqpState.Idle)
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(false); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(true); }
                                }
                                else if (EqpStateManager.Instance.State == EqpState.Down)
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(true); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(false); }
                                }
                            }
                            else
                            {
                                if (EqpStateManager.Instance.State == EqpState.PM)
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(false); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(true); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(false); }
                                }
                                else
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(true); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(false); }
                                }
                            }

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        break;

                    case 20:
                        if (GetElapsedTicks() > 200)
                        {
                            if (EqpStateManager.Instance.State == EqpState.Idle)
                            {
                                m_LampGreenToggle = true;
                            }
                            else if (EqpStateManager.Instance.State == EqpState.Down)
                            {
                                m_LampRedToggle = true;
                                m_LampYellowToggle = true;
                            }
                            else
                            {
                                m_LampRedToggle = false;
                                m_LampYellowToggle = false;
                                m_LampGreenToggle = false;
                            }

                            if (XFunc.GetTickCount() - ToggleTicks > 1000)
                            {
                                SyncDeviceFlag(!m_LampToggle);
                                ToggleTicks = XFunc.GetTickCount();
                            }

                            seqNo = 10;
                        }
                        break; 
                    #endregion

                    #region CRRC
                    //반송
                    //Blink Green
                    //Idle Green
                    //-----------
                    //Manual
                    //Blink Yellow
                    //-----------
                    //Down
                    //Recovery Blink Red
                    //유지보수 혹은 고장 점검 Red
                    case 110:
                        {
                            if (EqpStateManager.Instance.OpMode == OperateMode.Auto)
                            {
                                if (EqpStateManager.Instance.State == EqpState.Run)
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(false); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(true); }
                                }
                                else if (EqpStateManager.Instance.State == EqpState.Idle || EqpStateManager.Instance.State == EqpState.Stop)
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(false); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(true); }
                                }
                                else if (EqpStateManager.Instance.State == EqpState.Down)
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(true); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(false); }
                                }
                            }
                            else
                            {
                                if (EqpStateManager.Instance.State == EqpState.PM)
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(true); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(false); }
                                }
                                else
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(false); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(true); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(false); }
                                }
                            }

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 120;
                        }
                        break;

                    case 120:
                        if (GetElapsedTicks() > 200)
                        {
                            if (EqpStateManager.Instance.State == EqpState.Run)
                            {
                                m_LampGreenToggle = true;
                            }
                            else if (EqpStateManager.Instance.OpMode == OperateMode.Manual && EqpStateManager.Instance.State != EqpState.PM)
                            {
                                m_LampYellowToggle = true;
                            }
                            else if (EqpStateManager.Instance.OpMode == OperateMode.Manual && EqpStateManager.Instance.State == EqpState.PM)
                            {
                                m_LampRedToggle = true;
                            }
                            else
                            {
                                m_LampRedToggle = false;
                                m_LampYellowToggle = false;
                                m_LampGreenToggle = false;
                            }

                            if (XFunc.GetTickCount() - ToggleTicks > 1000)
                            {
                                SyncDeviceFlag(!m_LampToggle);
                                ToggleTicks = XFunc.GetTickCount();
                            }

                            seqNo = 110;
                        }
                        break;
                    #endregion

                    #region ESWIN 수정중..
                    //Manual 
                    // Y Flicker
                    //Auto
                    // Exist Error
                    //  R/Y & Buzzer
                    // Exist Warning
                    //  OCS/JCS Disconnect
                    //   Red & Buzzer
                    // pause
                    //  Front
                    //   30sec<
                    //    =Y/G Flicker
                    // Idle
                    //  G 
                    // Excute
                    //  G Flicker
                    case 210:
                        {
                            if (EqpStateManager.Instance.OpMode == OperateMode.Auto)
                            {
                                bool isOCS_JCS_Disconnect = IF.OCS.OCSCommManager.Instance.OcsStatus.Connected == false;
                                isOCS_JCS_Disconnect |= IF.JCS.JCSCommManager.Instance.JcsStatus.Connected == false;

                                if (EqpStateManager.Instance.State == EqpState.Run && !isOCS_JCS_Disconnect)
                                {
                                    bool isOverride_Stop = Data.Process.ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.OverrideRatio == 0.0f;
                                    
                                    if (isOverride_Stop)//전방 감지 시 Y/G Flicker
                                    {
                                        if (m_FrontDetectStart)
                                        {
                                            if ((XFunc.GetTickCount() - m_FrontDetectTicks) > 30 * 1000)
                                            {
                                                if (m_FistChange == false)
                                                {
                                                    m_FistChange = true;
                                                    if (m_TowerLampRed.Count > 0) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(false); }
                                                    if (m_TowerLampBlue.Count > 0) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                                    if (m_TowerLampYellow.Count > 0) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                                    if (m_TowerLampGreen.Count > 0 ) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(true); }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            m_FrontDetectStart = true;
                                            m_FrontDetectTicks = XFunc.GetTickCount();
                                        }
                                    }
                                    else
                                    {
                                        if (m_FrontDetectStart) m_FrontDetectStart = false;
                                        GV.OperatorCallBuzzerOn = false;
                                    }
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(false); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(true); }
                                }
                                else if ((EqpStateManager.Instance.State == EqpState.Idle || EqpStateManager.Instance.State == EqpState.Stop) && !isOCS_JCS_Disconnect)
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(false); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(true); }
                                }
                                else if (EqpStateManager.Instance.State == EqpState.Down)
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(true); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(false); }
                                }
                                else if (isOCS_JCS_Disconnect)
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(true); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(false); }
                                    //GV.OperatorCallBuzzerOn = true;
                                }
                            }
                            else
                            {
                                if (EqpStateManager.Instance.State == EqpState.PM)
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(true); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(false); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(false); }
                                }
                                else
                                {
                                    if (m_TowerLampRed.Count > 0 && !m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(false); }
                                    if (m_TowerLampBlue.Count > 0 && !m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(false); }
                                    if (m_TowerLampYellow.Count > 0 && !m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(true); }
                                    if (m_TowerLampGreen.Count > 0 && !m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(false); }
                                }
                            }

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 220;
                        }
                        break;

                    case 220:
                        if (GetElapsedTicks() > 200)
                        {
                            if (EqpStateManager.Instance.State == EqpState.Run)
                            {
                                bool isOCS_JCS_Disconnect = IF.OCS.OCSCommManager.Instance.OcsStatus.Connected == false;
                                isOCS_JCS_Disconnect |= IF.JCS.JCSCommManager.Instance.JcsStatus.Connected == false;
                                if (isOCS_JCS_Disconnect)
                                {
                                    m_LampRedToggle = true;
                                    m_LampYellowToggle = false;
                                    m_LampGreenToggle = false;
                                }
                                else if (m_FrontDetectStart && (XFunc.GetTickCount() - m_FrontDetectTicks) > 30 * 1000)//전방 감지 30초 경과 시 Y/G Flicker
                                {
                                    m_LampRedToggle = false;
                                    m_LampYellowToggle = true;
                                    m_LampGreenToggle = true;
                                    GV.OperatorCallBuzzerOn = false;
                                }
                                else
                                {
                                    m_LampRedToggle = false;
                                    m_LampYellowToggle = false;
                                    m_LampGreenToggle = false;
                                    m_FistChange = false;
                                }
                            }
                            else if (EqpStateManager.Instance.OpMode == OperateMode.Manual && EqpStateManager.Instance.State != EqpState.PM)
                            {
                                m_LampRedToggle = false;
                                m_LampYellowToggle = true;
                                m_LampGreenToggle = false;
                                m_FistChange = false;
                                GV.OperatorCallBuzzerOn = false;
                            }
                            else if (EqpStateManager.Instance.OpMode == OperateMode.Manual && EqpStateManager.Instance.State == EqpState.PM)
                            {
                                m_LampRedToggle = true;
                                m_LampYellowToggle = true;
                                m_LampGreenToggle = false;
                                m_FistChange = false;
                                GV.OperatorCallBuzzerOn = false;
                            }
                            else
                            {
                                m_LampRedToggle = false;
                                m_LampYellowToggle = false;
                                m_LampGreenToggle = true;
                                GV.OperatorCallBuzzerOn = false;
                            }

                            if (XFunc.GetTickCount() - ToggleTicks > 1000)
                            {
                                SyncDeviceFlag();
                                ToggleTicks = XFunc.GetTickCount();
                            }

                            seqNo = 210;
                        }
                        break; 
                        #endregion
                }

                SeqNo = seqNo;
                return rv;
            }

            private void SyncDeviceFlag(bool flag)
            {
                m_LampToggle = flag;
                if (m_TowerLampRed.Count > 0 && m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(flag); }
                if (m_TowerLampBlue.Count > 0 && m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(flag); }
                if (m_TowerLampYellow.Count > 0 && m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(flag); }
                if (m_TowerLampGreen.Count > 0 && m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(flag); }
            }
			
            private void SyncDeviceFlag()
            {
                if (m_TowerLampRed.Count > 0 && m_LampRedToggle) foreach (_DevOutput lamp in m_TowerLampRed) { lamp.SetDo(!lamp.IsDetected); }
                if (m_TowerLampBlue.Count > 0 && m_LampBlueToggle) foreach (_DevOutput lamp in m_TowerLampBlue) { lamp.SetDo(!lamp.IsDetected); }
                if (m_TowerLampYellow.Count > 0 && m_LampYellowToggle) foreach (_DevOutput lamp in m_TowerLampYellow) { lamp.SetDo(!lamp.IsDetected); }
                if (m_TowerLampGreen.Count > 0 && m_LampGreenToggle) foreach (_DevOutput lamp in m_TowerLampGreen) { lamp.SetDo(!lamp.IsDetected); }
            }
            #endregion
        }

        private class SeqMelody : XSeqFunc
        {
            #region Fields
            DevOpPanel m_Device = null;

            private _DevOutput m_MelodyAlert = null;
            private _DevOutput m_MelodyWarning = null;
            private _DevOutput m_MelodyAlarm = null;
            private _DevOutput m_MelodyOpCall = null;
            private BuzzerType m_CurBuzzerType = BuzzerType.None;
            private List<int> m_OccupiedAlarms = new List<int>();

            private bool m_AlertSet = false;
            private bool m_AlarmSet = false;
            private bool m_WarningSet = false;
            private bool m_OpCallSet = false;

            private bool m_AreadyBuzzerOff = false;
            private bool m_ForcedBuzzerOff = false;
            #endregion

            #region Constructor
            public SeqMelody(DevOpPanel device)
            {
                this.SeqName = $"SeqMelody{device.MyName}";
                m_Device = device;

                if (device.MelodyAlert.IsValid) m_MelodyAlert = device.MelodyAlert;
                if (device.MelodyWarning.IsValid) m_MelodyWarning = device.MelodyWarning;
                if (device.MelodyAlarm.IsValid) m_MelodyAlarm = device.MelodyAlarm;
                if (device.MelodyOpCall.IsValid) m_MelodyOpCall = device.MelodyOpCall;

                TaskDeviceControl.Instance.RegSeq(this);
            }

            public override void SeqAbort()
            {
                this.InitSeq();
            }
            #endregion

            #region Methods
            public override int Do()
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                if (!m_Device.Initialized) return rv;

                switch (seqNo)
                {
                    case 0:
                        {
                            m_ForcedBuzzerOff = false;
                            // Buzzer Control Condition
                            bool ServoMovingAlert = true;
                            ServoMovingAlert &= EqpStateManager.Instance.OpMode == OperateMode.Manual ? true : false;
                            ServoMovingAlert &= SetupManager.Instance.SetupCommon.ServoMoveAlert ? true : false;
                            ServoMovingAlert &= GV.WheelBusy ? true : false;
                            bool EquipmentAlarm = true;
                            EquipmentAlarm &= AlarmCurrentProvider.Instance.IsHeavyAlarm();
                            bool EquipmentWarning = true;
                            EquipmentWarning &= AlarmCurrentProvider.Instance.IsWarningAlarm();
                            bool OperatorCall = GV.OperatorCallBuzzerOn;

                            // Buzzer Auto Off Condition
                            bool buzzer_auto_off = false;
                            buzzer_auto_off |= m_AlarmSet && !EquipmentAlarm ? true : false;
                            buzzer_auto_off |= m_WarningSet && !EquipmentWarning ? true : false;
                            buzzer_auto_off |= m_AlertSet && !ServoMovingAlert ? true : false;
                            buzzer_auto_off |= m_OpCallSet && !OperatorCall ? true : false;
                            // Buzzer Off Condition
                            bool buzzer_manual_off = true;
                            buzzer_manual_off &= IsPushedSwitch.IsBuzzerOff ? true : false;
                            buzzer_manual_off &= m_CurBuzzerType == BuzzerType.SafetyAlert ? false : true;
                            // Melody NoUse
                            bool melody_use = SetupManager.Instance.SetupCommon.MelodyUse == Use.Use;
                            if (buzzer_auto_off || buzzer_manual_off || !melody_use)
                            {
                                if (m_Device.m_MelodyAlarm.IsValid) m_MelodyAlarm.SetDo(false);
                                if (m_Device.m_MelodyWarning.IsValid) m_MelodyWarning.SetDo(false);
                                if (m_Device.m_MelodyAlert.IsValid) m_MelodyAlert.SetDo(false);
                                if (m_Device.m_MelodyOpCall.IsValid) m_MelodyOpCall.SetDo(false);

                                if (!EquipmentAlarm) m_AlarmSet = false;
                                if (!EquipmentWarning) m_WarningSet = false;
                                if (EquipmentAlarm || EquipmentWarning) m_AreadyBuzzerOff = true;
                                m_AlertSet = false;
                                m_OpCallSet = false;
                            }

                            if (melody_use)
                            {
                                bool buzzer_change = false;
                                if (OperatorCall) { m_CurBuzzerType = BuzzerType.OperatorCall; buzzer_change = true; }
                                else if (ServoMovingAlert) { m_CurBuzzerType = BuzzerType.SafetyAlert; buzzer_change = true; }
                                else if (EquipmentWarning) { m_CurBuzzerType = BuzzerType.Warning; buzzer_change = true; }
                                else if (EquipmentAlarm && !m_AreadyBuzzerOff) { m_CurBuzzerType = BuzzerType.Alarm; buzzer_change = true; }

                                if (GV.EmoAlarm)
                                {
                                    // 상시 조건 추가
                                    if (m_Device.m_MelodyAlarm.IsValid) m_MelodyAlarm.SetDo(true);
                                    else if (m_Device.m_MelodyWarning.IsValid) m_MelodyWarning.SetDo(true);
                                    else if (m_Device.m_MelodyAlert.IsValid) m_MelodyAlert.SetDo(true);
                                    else if (m_Device.m_MelodyOpCall.IsValid) m_MelodyOpCall.SetDo(true);

                                    seqNo = 300;
                                }
                                else if (buzzer_change)
                                {
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 10;
                                }
                                else
                                {
                                    seqNo = 100;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            if (GetElapsedTicks() > 100)
                            {
                                bool already_set = false;
                                already_set |= m_AlarmSet && m_CurBuzzerType == BuzzerType.Alarm ? true : false;
                                already_set |= m_WarningSet && m_CurBuzzerType == BuzzerType.Warning ? true : false;
                                already_set |= m_AlertSet && m_CurBuzzerType == BuzzerType.SafetyAlert ? true : false;
                                already_set |= m_OpCallSet && m_CurBuzzerType == BuzzerType.OperatorCall ? true : false;
                                if (already_set)
                                {
                                    if (m_AlarmSet) seqNo = 100;
                                    else if (m_WarningSet) seqNo = 200;
                                    else seqNo = 0;
                                }
                                else
                                {
                                    m_AlarmSet = m_CurBuzzerType == BuzzerType.Alarm ? true : false;
                                    m_WarningSet = m_CurBuzzerType == BuzzerType.Warning ? true : false;
                                    m_AlertSet = m_CurBuzzerType == BuzzerType.SafetyAlert ? true : false;
                                    m_OpCallSet = m_CurBuzzerType == BuzzerType.OperatorCall ? true : false;
                                    if (m_Device.m_MelodyAlarm.IsValid) m_MelodyAlarm.SetDo(m_AlarmSet);
                                    if (m_Device.m_MelodyWarning.IsValid) m_MelodyWarning.SetDo(m_WarningSet);
                                    if (m_Device.m_MelodyAlert.IsValid) m_MelodyAlert.SetDo(m_AlertSet);
                                    if (m_Device.m_MelodyOpCall.IsValid) m_MelodyOpCall.SetDo(m_OpCallSet);

                                    if (m_AlarmSet || m_WarningSet)
                                    {
                                        List<int> curAlarms = AlarmCurrentProvider.Instance.GetCurrentAlarmIds();
                                        m_OccupiedAlarms.Clear();
                                        m_OccupiedAlarms.AddRange(curAlarms.ToArray());
                                    }
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 100:
                        {
                            List<int> curAlarms = AlarmCurrentProvider.Instance.GetCurrentAlarmIds();
                            bool alarmChanged = false;
                            for (int i = 0; i < curAlarms.Count; i++)
                            {
                                if (!m_OccupiedAlarms.Contains(curAlarms[i]))
                                    alarmChanged = true;
                            }

                            if (alarmChanged)
                            {
                                m_OccupiedAlarms.Clear();
                                m_OccupiedAlarms.AddRange(curAlarms.ToArray());
                                m_AlarmSet = false;
                                m_AreadyBuzzerOff = false;
                            }
                            seqNo = 0;
                        }
                        break;

                    case 200:
                        {
                            List<int> curAlarms = AlarmCurrentProvider.Instance.GetCurrentAlarmIds();
                            bool alarmChanged = false;
                            for (int i = 0; i < curAlarms.Count; i++)
                            {
                                if (!m_OccupiedAlarms.Contains(curAlarms[i]))
                                    alarmChanged = true;
                            }

                            if (alarmChanged)
                            {
                                m_OccupiedAlarms.Clear();
                                m_OccupiedAlarms.AddRange(curAlarms.ToArray());
                                m_WarningSet = false;
                                m_AreadyBuzzerOff = false;
                            }
                            seqNo = 0;
                        }
                        break;

                    case 300:
                        if (!GV.EmoAlarm)
                        {
                            if (m_Device.MelodyAlarm.IsValid) m_Device.MelodyAlarm.SetDo(false);
                            if (m_Device.MelodyWarning.IsValid) m_Device.MelodyWarning.SetDo(false);
                            if (m_Device.MelodyOpCall.IsValid) m_Device.MelodyOpCall.SetDo(false);
                            if (m_Device.MelodyAlert.IsValid) m_Device.MelodyAlert.SetDo(false);
                            seqNo = 0;
                        }
                        break;
                }

                SeqNo = seqNo;
                return rv;
            }
            #endregion
        }
        private class SeqSaftyPlc : XSeqFunc
        {
            #region Fields
            private const string FuncName = "[SeqSaftyPlc]";

            DevOpPanel m_Device = null;
            #endregion

            #region Constructor
            public SeqSaftyPlc(DevOpPanel device)
            {
                this.SeqName = $"SeqSaftyPlc{device.MyName}";
                m_Device = device;

                TaskDeviceControl.Instance.RegSeq(this);
            }

            public override void SeqAbort()
            {
                this.InitSeq();
            }
            #endregion

            #region Methods
            public override int Do()
            {
                int rv = -1;
                int seqNo = this.SeqNo;

                if (!m_Device.Initialized) return rv;

                // Power On Monitor
                GV.PowerOn = m_Device.IsPowerOn();

                switch (seqNo)
                {
                    case 0:
                        {
                            if (IoManager.Instance.UpdateRun)
                            {
                                if (AppConfig.Instance.Simulation.IO) 
                                    if (m_Device.DiPowerOn.IsValid) m_Device.DiPowerOn.SetDi(true);
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                        }
                        break;

                    case 10:
                        {
                            // power off
                            bool power_off = m_Device.IsPowerOn() == false;

                            if (GV.EmoAlarm || power_off)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Abnormal Safety PLC Alarm Occurrence. EMO Alarm : {0}, Power Off : {1}", GV.EmoAlarm, power_off));

                                EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort);
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                        }
                        break;

                    case 20:
                        {
                            bool power_on = m_Device.IsPowerOn();
                            if (power_on)
                            {
                                seqNo = 10;
                            }
                            else if (GV.EmoAlarm == false)
                            {
                                bool safty_reset = m_Device.IsSaftyKeySwitch();
                                safty_reset |= XFunc.GetTickCount() - StartTicks > 5000;
                                if (safty_reset)
                                {
                                    m_Device.SetSaftyPlcReset(true);
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 30;
                                }
                            }
                        }
                        break;

                    case 30:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 500)
                            {
                                m_Device.SetSaftyPlcReset(false);
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 40;
                            }
                        }
                        break;

                    case 40:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                bool power_on = true;
                                power_on &= m_Device.IsPowerOn();

                                if (power_on)
                                {
                                    seqNo = 0;
                                }
                                else
                                {
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 20;
                                }
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

                var helperXml = new XmlHelper<DevOpPanel>();
                DevOpPanel dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.SaftyKeySwitch = dev.SaftyKeySwitch;
                    this.DoSaftyReset = dev.DoSaftyReset;
                    this.DoTowerLampRed1 = dev.DoTowerLampRed1;
                    this.DoTowerLampGreen1 = dev.DoTowerLampGreen1;
                    this.DoTowerLampYellow1 = dev.DoTowerLampYellow1;
                    this.DoTowerLampBlue1 = dev.DoTowerLampBlue1;
                    this.DoTowerLampRed2 = dev.DoTowerLampRed2;
                    this.DoTowerLampGreen2 = dev.DoTowerLampGreen2;
                    this.DoTowerLampYellow2 = dev.DoTowerLampYellow2;
                    this.DoTowerLampBlue2 = dev.DoTowerLampBlue2;
                    this.MelodyAlarm = dev.MelodyAlarm;
                    this.MelodyWarning = dev.MelodyWarning;
                    this.MelodyAlert = dev.MelodyAlert;
                    this.MelodyOpCall = dev.MelodyOpCall;
                    this.DiPowerOn = dev.DiPowerOn;
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
                var helperXml = new XmlHelper<DevOpPanel>();
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
