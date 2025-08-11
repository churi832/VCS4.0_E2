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
using System.Xml.Serialization;

namespace Sineva.VHL.Device
{
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class DevCleaner : _Device
    {
        private const string DevName = "DevCleaner";

        #region Fields
        private _DevOutput m_DoCleanerStart = new _DevOutput();
        private _DevInput m_DiDoorClose = new _DevInput();
        private _DevInput m_DiCleanerStart = new _DevInput();
        private _DevInput m_DiTempAlarm1 = new _DevInput();
        private _DevInput m_DiTempAlarm2 = new _DevInput();

        private AlarmData m_ALM_CleanerDoorOpenAlarm = null;
        private AlarmData m_ALM_CleanerStartTimeout = null;
        private AlarmData m_ALM_CleanerStopTimeout = null;
        private AlarmData m_ALM_CleanerTemperatureAlarm = null;

        private SeqAction m_SeqAction = null;
        private SeqMonitor m_SeqMonitor = null;

        private int m_CleanerUsingTimes = 0;
        #endregion

        #region Properties
        [Category("I/O Setting (Digital Output)"), Description("Cleaner Start"), DeviceSetting(true)]
        public _DevOutput DoCleanerStart { get { return m_DoCleanerStart; } set { m_DoCleanerStart = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Cleaner Door Close"), DeviceSetting(true)]
        public _DevInput DiDoorClose { get { return m_DiDoorClose; } set { m_DiDoorClose = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Cleaner Start"), DeviceSetting(true)]
        public _DevInput DiCleanerStart { get { return m_DiCleanerStart; } set { m_DiCleanerStart = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Cleaner Temp Alarm1"), DeviceSetting(true)]
        public _DevInput DiTempAlarm1 { get { return m_DiTempAlarm1; } set { m_DiTempAlarm1 = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Cleaner Temp Alarm2"), DeviceSetting(true)]
        public _DevInput DiTempAlarm2 { get { return m_DiTempAlarm2; } set { m_DiTempAlarm2 = value; } }
        [Category("!LifeTime Manager"), DisplayName("Using Time"), Description("Device Life Using Time"), DeviceSetting(false, true)]
        public int CleanerUsingTimes { get { return m_CleanerUsingTimes; } set { SaveCurState = m_CleanerUsingTimes != value; m_CleanerUsingTimes = value; } }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_CleanerDoorOpenAlarm
        {
            get { return m_ALM_CleanerDoorOpenAlarm; }
            set { m_ALM_CleanerDoorOpenAlarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_CleanerStartTimeout
        {
            get { return m_ALM_CleanerStartTimeout; }
            set { m_ALM_CleanerStartTimeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_CleanerStopTimeout
        {
            get { return m_ALM_CleanerStopTimeout; }
            set { m_ALM_CleanerStopTimeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_CleanerTemperatureAlarm
        {
            get { return m_ALM_CleanerTemperatureAlarm; }
            set { m_ALM_CleanerTemperatureAlarm = value; }
        }
        #endregion

        #region Constructor
        public DevCleaner()
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
            if (DoCleanerStart.IsValid) ok &= DoCleanerStart.Initialize(this.ToString(), false, false);
            if (DiDoorClose.IsValid) ok &= DiDoorClose.Initialize(this.ToString(), false, false);
            if (DiCleanerStart.IsValid) ok &= DiCleanerStart.Initialize(this.ToString(), false, false);
            if (DiTempAlarm1.IsValid) ok &= DiTempAlarm1.Initialize(this.ToString(), false, false);
            if (DiTempAlarm2.IsValid) ok &= DiTempAlarm2.Initialize(this.ToString(), false, false);

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
            ALM_CleanerDoorOpenAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, MyName, ParentName, "Door Open Alarm");
            ALM_CleanerStartTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, false, MyName, ParentName, "Start Timeout");
            ALM_CleanerStopTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, false, MyName, ParentName, "Stop Timeout");
            ALM_CleanerTemperatureAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, false, MyName, ParentName, "Temperature Alarm");
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Cleaner Used Time(sec)", this, "GetUsingTime", 1000));
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
            if (AppConfig.Instance.VehicleType != VehicleType.Clean) return;
            if (!Initialized) return;

            m_SeqAction.SeqAbort();
            m_SeqMonitor.SeqAbort();
        }
        #endregion

        #region Methods - Life Time
        public int GetUsingTime()
        {
            if (DiCleanerStart.IsDetected) m_CleanerUsingTimes++;
            return m_CleanerUsingTimes;
        }
        #endregion

        #region Methods
        public void Start()
        {
            if (AppConfig.Instance.VehicleType != VehicleType.Clean) return;
            if (!Initialized) return;
            if (IsAlarm()) return;
            if (!IsDoorClose()) return;

            if (m_DoCleanerStart.IsValid) m_DoCleanerStart.SetDo(true);
            if (AppConfig.Instance.Simulation.MY_DEBUG) m_DiCleanerStart.SetDi(true);
        }
        public void Stop()
        {
            if (AppConfig.Instance.VehicleType != VehicleType.Clean) return;
            if (!Initialized) return;

            if (m_DoCleanerStart.IsValid) m_DoCleanerStart.SetDo(false);
            if (AppConfig.Instance.Simulation.MY_DEBUG) m_DiCleanerStart.SetDi(false);
        }
        public bool IsRun()
        {
            if (AppConfig.Instance.VehicleType != VehicleType.Clean) return false;
            if (!Initialized) return false;

            bool run = m_DiCleanerStart.IsValid ? m_DiCleanerStart.IsDetected : false;
            return run;
        }
        public bool IsDoorClose()
        {
            if (AppConfig.Instance.VehicleType != VehicleType.Clean) return false;
            if (!Initialized) return false;

            bool run = m_DiDoorClose.IsValid ? m_DiDoorClose.IsDetected : false;
            return run;
        }
        public bool IsAlarm()
        {
            if (AppConfig.Instance.VehicleType != VehicleType.Clean) return false;
            if (!Initialized) return false;

            bool rv = false;
            rv |= m_DiTempAlarm1.IsValid ? m_DiTempAlarm1.IsDetected : false;
            rv |= m_DiTempAlarm1.IsValid ? m_DiTempAlarm2.IsDetected : false;
            return rv;
        }
        #endregion

        #region Sequence
        private class SeqMonitor : XSeqFunc
        {
            #region Field
            private bool m_SetAlarm = false;
            private bool m_DoorOpen = false;
            private DevCleaner m_Device = null;
            #endregion

            public SeqMonitor(DevCleaner device)
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

                if (AppConfig.Instance.VehicleType != VehicleType.Clean) return rv;
                if (!m_Device.Initialized) return rv;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                m_Device.DiDoorClose.SetDi(true);
                            }

                            if (m_Device.IsAlarm() && !m_SetAlarm && XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_Device.Stop();

                                m_SetAlarm = true;
                                SequenceDeviceLog.WriteLog(string.Format("{0} Cleaner Temperature Alarm On", m_Device.MyName));
                                AlarmId = m_Device.ALM_CleanerTemperatureAlarm.ID;
                                EqpAlarm.Set(AlarmId);
                                StartTicks = XFunc.GetTickCount();
                            }
                            else if (!m_Device.IsAlarm() && m_SetAlarm && XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_SetAlarm = false;
                                SequenceDeviceLog.WriteLog(string.Format("{0} Cleaner Temperature Alarm Off", m_Device.MyName));
                                AlarmId = m_Device.ALM_CleanerTemperatureAlarm.ID;
                                EqpAlarm.Reset(AlarmId);
                                StartTicks = XFunc.GetTickCount();
                            }

                            if (!m_Device.IsDoorClose() && !m_DoorOpen && XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_DoorOpen = true;
                                m_Device.Stop();

                                SequenceDeviceLog.WriteLog(string.Format("{0} Cleaner Door Open Alarm On", m_Device.MyName));
                                AlarmId = m_Device.ALM_CleanerDoorOpenAlarm.ID;
                                EqpAlarm.Set(AlarmId);

                                GV.CleanerDoorOpenInterlock = true;
                                StartTicks = XFunc.GetTickCount();
                            }
                            else if (m_Device.IsDoorClose() && m_DoorOpen && XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_DoorOpen = false;

                                SequenceDeviceLog.WriteLog(string.Format("{0} Cleaner Door Open Alarm Off", m_Device.MyName));
                                AlarmId = m_Device.ALM_CleanerDoorOpenAlarm.ID;
                                EqpAlarm.Reset(AlarmId);

                                GV.CleanerDoorOpenInterlock = false;
                                StartTicks = XFunc.GetTickCount();
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
            private DevCleaner m_Device = null;
            #endregion

            #region Constructor
            public SeqAction(DevCleaner device)
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
                m_Device.Stop();
            }
            #endregion

            #region override
            public override int Do()
            {
                int seqNo = SeqNo;
                int rv = -1;

                if (AppConfig.Instance.VehicleType != VehicleType.Clean) return rv;
                if (!m_Device.Initialized) return rv;

                switch (seqNo)
                {
                    case 0:
                        {
                            bool run_condition = true;
                            run_condition &= m_Device.IsDoorClose();
                            run_condition &= !m_Device.IsAlarm();
                            run_condition &= GV.WheelBusy;
                            run_condition &= !m_Device.IsRun() && EqpStateManager.Instance.OpMode == OperateMode.Auto;

                            bool stop_condition = true;
                            stop_condition &= m_Device.IsDoorClose();
                            stop_condition &= !m_Device.IsAlarm();
                            stop_condition &= !GV.WheelBusy;
                            stop_condition &= m_Device.IsRun() && EqpStateManager.Instance.OpMode == OperateMode.Auto;

                            if (run_condition)
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0} Cleaner Start Check Confirm", m_Device.MyName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else if (stop_condition)
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0} Cleaner Stop Check Confirm", m_Device.MyName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 100;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;
                            bool run_condition = true;
                            run_condition &= m_Device.IsDoorClose();
                            run_condition &= !m_Device.IsAlarm();
                            run_condition &= GV.WheelBusy;
                            run_condition &= !m_Device.IsRun();
                            if (run_condition)
                            {
                                m_Device.Start();
                                SequenceDeviceLog.WriteLog(string.Format("{0} Cleaner Start", m_Device.MyName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0} Cleaner Start Repeat Check", m_Device.MyName));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (m_Device.IsRun())
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0} Cleaner Start OK", m_Device.MyName));
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 5000)
                            {
                                m_Device.Stop();
                                SequenceDeviceLog.WriteLog(string.Format("{0} Cleaner Start Timeout", m_Device.MyName));
                                AlarmId = m_Device.ALM_CleanerStartTimeout.ID;
                                EqpAlarm.Set(AlarmId);
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 100:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 5000) break;

                            bool stop_condition = true;
                            stop_condition &= m_Device.IsDoorClose();
                            stop_condition &= !m_Device.IsAlarm();
                            stop_condition &= !GV.WheelBusy;
                            stop_condition &= m_Device.IsRun();
                            if (stop_condition)
                            {
                                m_Device.Stop();
                                SequenceDeviceLog.WriteLog(string.Format("{0} Cleaner Stop", m_Device.MyName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 120;
                            }
                            else
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0} Cleaner Stop Repeat Check", m_Device.MyName));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 120:
                        {
                            if (!m_Device.IsRun())
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0} Cleaner Stop OK", m_Device.MyName));
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 5000)
                            {
                                m_Device.Stop();
                                SequenceDeviceLog.WriteLog(string.Format("{0} Cleaner Stop Timeout", m_Device.MyName));
                                AlarmId = m_Device.ALM_CleanerStopTimeout.ID;
                                EqpAlarm.Set(AlarmId);
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 1000:
                        {
                            if (IsPushedSwitch.IsAlarmReset)
                            {
                                IsPushedSwitch.m_AlarmRstPushed = false;
                                EqpAlarm.Reset(AlarmId);
                                SequenceDeviceLog.WriteLog(string.Format("Reset Alarm : Code[{0}]", AlarmId));

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

                var helperXml = new XmlHelper<DevCleaner>();
                DevCleaner dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.DoCleanerStart = dev.DoCleanerStart;
                    this.DiDoorClose = dev.DiDoorClose;
                    this.DiCleanerStart = dev.DiCleanerStart;
                    this.DiTempAlarm1 = dev.DiTempAlarm1;
                    this.DiTempAlarm2 = dev.DiTempAlarm2;

                    this.CleanerUsingTimes = dev.CleanerUsingTimes;
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
                var helperXml = new XmlHelper<DevCleaner>();
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
