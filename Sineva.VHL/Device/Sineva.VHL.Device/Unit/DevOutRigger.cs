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
    public class DevOutRigger : _Device
    {
        private const string DevName = "DevOutRigger";

        #region Fields
        private _DevOutput m_DoLock = new _DevOutput();
        private _DevOutput m_DoUnlock = new _DevOutput();
        private _DevOutput m_DoStart = new _DevOutput();
        private _DevOutput m_DoStop = new _DevOutput();
        private _DevOutput m_DoAlarmReset = new _DevOutput();

        private _DevInput m_DiLock = new _DevInput();
        private _DevInput m_DiUnlock = new _DevInput();
        private _DevInput m_DiAlarm = new _DevInput();

        private float m_OutRiggerChangeTime = 1.0f;

        private AlarmData m_ALM_NotDefine = null;
        private AlarmData m_ALM_LockTimeout = null;
        private AlarmData m_ALM_UnlockTimeout = null;
        private AlarmData m_ALM_OutRiggerAlarm = null;

        private SeqAction m_SeqAction = null;
        private SeqMonitor m_SeqMonitor = null;
        private bool m_AlarmReset = false;
        #endregion

        #region Properties
        [Category("I/O Setting (Digital Output)"), Description("Lock"), DeviceSetting(true)]
        public _DevOutput DoLock { get { return m_DoLock; } set { m_DoLock = value; } }
        [Category("I/O Setting (Digital Output)"), Description("Unlock"), DeviceSetting(true)]
        public _DevOutput DoUnlock { get { return m_DoUnlock; } set { m_DoUnlock = value; } }
        [Category("I/O Setting (Digital Output)"), Description("Start"), DeviceSetting(true)]
        public _DevOutput DoStart { get { return m_DoStart; } set { m_DoStart = value; } }
        [Category("I/O Setting (Digital Output)"), Description("Stop"), DeviceSetting(true)]
        public _DevOutput DoStop { get { return m_DoStop; } set { m_DoStop = value; } }
        [Category("I/O Setting (Digital Output)"), Description("AlarmReset"), DeviceSetting(true)]
        public _DevOutput DoAlarmReset { get { return m_DoAlarmReset; } set { m_DoAlarmReset = value; } }

        [Category("I/O Setting (Digital Input)"), Description("Lock"), DeviceSetting(true)]
        public _DevInput DiLock { get { return m_DiLock; } set { m_DiLock = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Unlock"), DeviceSetting(true)]
        public _DevInput DiUnlock { get { return m_DiUnlock; } set { m_DiUnlock = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Alarm"), DeviceSetting(true)]
        public _DevInput DiAlarm { get { return m_DiAlarm; } set { m_DiAlarm = value; } }

        [Category("!Setting Parameter"), Description("Change Change Timeover(sec)"), DeviceSetting(true, true)]
        public float OutRiggerChangeTime
        {
            get { return m_OutRiggerChangeTime; }
            set { m_OutRiggerChangeTime = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_NotDefine
        {
            get { return m_ALM_NotDefine; }
            set { m_ALM_NotDefine = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ForwardTimeout
        {
            get { return m_ALM_LockTimeout; }
            set { m_ALM_LockTimeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_BackwardTimeout
        {
            get { return m_ALM_UnlockTimeout; }
            set { m_ALM_UnlockTimeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_OutRiggerAlarm
        {
            get { return m_ALM_OutRiggerAlarm; }
            set { m_ALM_OutRiggerAlarm = value; }
        }
        #endregion

        #region Constructor
        public DevOutRigger()
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
            if (DoLock.IsValid) ok &= DoLock.Initialize(this.ToString(), false, false);
            if (DoUnlock.IsValid) ok &= DoUnlock.Initialize(this.ToString(), false, false);
            if (DoStart.IsValid) ok &= DoStart.Initialize(this.ToString(), false, false);
            if (DoStop.IsValid) ok &= DoStop.Initialize(this.ToString(), false, false);
            if (DoAlarmReset.IsValid) ok &= DoAlarmReset.Initialize(this.ToString(), false, false);

            if (DiLock.IsValid) ok &= DiLock.Initialize(this.ToString(), false, false);
            if (DiUnlock.IsValid) ok &= DiUnlock.Initialize(this.ToString(), false, false);
            if (DiAlarm.IsValid) ok &= DiAlarm.Initialize(this.ToString(), false, false);

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
            ALM_ForwardTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, MyName, ParentName, "Lock Timeout");
            ALM_BackwardTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, MyName, ParentName, "Unlock Timeout");
            ALM_OutRiggerAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, MyName, ParentName, "Unit Alarm");
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
        #endregion

        #region Methods
        public override void SeqAbort()
        {
            if (SetupManager.Instance.SetupOperation.OutRiggerUse == Use.NoUse) return;
            if (!Initialized) return;

            //m_SeqAction.SeqAbort();
            //m_SeqMonitor.SeqAbort();
            m_AlarmReset = true;
        }
        public int Lock()
        {
            if (SetupManager.Instance.SetupOperation.OutRiggerUse == Use.NoUse) return 0;
            if (!Initialized) return ALM_NotDefine.ID;

            int rv = m_SeqAction.Lock();
            return rv;
        }
        public int Unlock()
        {
            if (SetupManager.Instance.SetupOperation.OutRiggerUse == Use.NoUse) return 0;
            if (!Initialized) return ALM_NotDefine.ID;

            int rv = m_SeqAction.Unlock();
            return rv;
        }
        public void SetLock()
        {
            if (SetupManager.Instance.SetupOperation.OutRiggerUse == Use.NoUse) return;
            if (!Initialized) return;

            m_DoUnlock.SetDo(false);
            m_DoLock.SetDo(true);
            m_DoStop.SetDo(false);
            m_DoStart.SetDo(true);
        }
        public void SetUnlock()
        {
            if (SetupManager.Instance.SetupOperation.OutRiggerUse == Use.NoUse) return;
            if (!Initialized) return;

            m_DoLock.SetDo(false);
            m_DoUnlock.SetDo(true);
            m_DoStop.SetDo(false);
            m_DoStart.SetDo(true);
        }
        public bool GetLock()
        {
            if (SetupManager.Instance.SetupOperation.OutRiggerUse == Use.NoUse) return true;
            if (!Initialized) return false;

            bool rv = true;
            rv &= m_DiLock.IsDetected ? true : false;
            rv &= m_DiUnlock.IsDetected ? false : true;
            return rv;
        }
        public bool GetUnlock()
        {
            if (SetupManager.Instance.SetupOperation.OutRiggerUse == Use.NoUse) return true;
            if (!Initialized) return false;

            bool rv = true;
            rv &= m_DiLock.IsDetected ? false : true;
            rv &= m_DiUnlock.IsDetected ? true : false;
            return rv;
        }
        public bool GetLockState()
        {
            if (SetupManager.Instance.SetupOperation.OutRiggerUse == Use.NoUse) return false;
            if (!Initialized) return false;

            bool rv = true;
            rv &= m_DoLock.IsDetected ? true : false;
            rv &= m_DoUnlock.IsDetected ? false : true;
            rv &= m_DoStart.IsDetected ? true : false;
            rv &= m_DoStop.IsDetected ? false : true;
            return rv;
        }
        public bool GetUnlockState()
        {
            if (SetupManager.Instance.SetupOperation.OutRiggerUse == Use.NoUse) return false;
            if (!Initialized) return false;

            bool rv = true;
            rv &= m_DoLock.IsDetected ? false : true;
            rv &= m_DoUnlock.IsDetected ? true : false;
            rv &= m_DoStart.IsDetected ? false : true;
            rv &= m_DoStop.IsDetected ? true : false;
            return rv;
        }
        public bool IsAlarm()
        {
            if (SetupManager.Instance.SetupOperation.OutRiggerUse == Use.NoUse) return false;
            if (!Initialized) return false;

            bool rv = true;
            rv &= m_DiAlarm.IsDetected ? true : false;
            return rv;
        }
        public void Reset()
        {
            if (SetupManager.Instance.SetupOperation.OutRiggerUse == Use.NoUse) return;
            if (!Initialized) return;

            m_DoUnlock.SetDo(false);
            m_DoLock.SetDo(false);
            m_DoStop.SetDo(false);
            m_DoStart.SetDo(false);
            m_AlarmReset = true;
        }
        #endregion

        #region Sequence
        private class SeqMonitor : XSeqFunc
        {
            #region Field
            private DevOutRigger m_Device = null;
            #endregion

            public SeqMonitor(DevOutRigger device)
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
                            if (SetupManager.Instance.SetupOperation.OutRiggerUse == Use.Use)
                            {
                                if (m_Device.m_AlarmReset)
                                {
                                    m_Device.m_AlarmReset = false;
                                    if (m_Device.IsAlarm())
                                    {
                                        m_Device.DoAlarmReset.SetDo(true);
                                        StartTicks = XFunc.GetTickCount();
                                        SequenceDeviceLog.WriteLog(string.Format("{0} Alarm Reset", m_Device.MyName));
                                        seqNo = 10;
                                    }
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            if (!m_Device.IsAlarm())
                            {
                                m_Device.DoAlarmReset.SetDo(false);
                                SequenceDeviceLog.WriteLog(string.Format("{0} Alarm Clear", m_Device.MyName));
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_Device.DoAlarmReset.SetDo(false);
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
            private DevOutRigger m_Device = null;
            #endregion

            #region Constructor
            public SeqAction(DevOutRigger device)
            {
                this.SeqName = $"SeqAction{device.MyName}";
                m_Device = device;
                TaskDeviceControl.Instance.RegSeq(this);
            }
            public override void SeqAbort()
            {
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
                            m_Device.SetLock();
                            StartTicks = XFunc.GetTickCount();

                            SequenceDeviceLog.WriteLog(string.Format("{0} OutRigger Forward Start", m_Device.MyName));
                            seqNo = 10;
                        }
                        break;

                    case 10:
                        {
                            if (m_Device.GetLock())
                            {
                                m_Device.Reset();
                                SequenceDeviceLog.WriteLog(string.Format("{0} OutRigger Forward OK", m_Device.MyName));
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (m_Device.IsAlarm())
                            {
                                m_Device.Reset();
                                rv = m_Device.ALM_OutRiggerAlarm.ID;
                                SequenceDeviceLog.WriteLog(string.Format("{0} OutRigger Alarm", m_Device.MyName));
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Device.OutRiggerChangeTime * 1000)
                            {
                                m_Device.Reset();
                                rv = m_Device.ALM_ForwardTimeout.ID;
                                SequenceDeviceLog.WriteLog(string.Format("{0} OutRigger Forward Timeout", m_Device.MyName));
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
                            m_Device.SetUnlock();
                            StartTicks = XFunc.GetTickCount();

                            SequenceDeviceLog.WriteLog(string.Format("{0} OutRigger Backward Start", m_Device.MyName));
                            seqNo = 10;
                        }
                        break;

                    case 10:
                        {
                            if (m_Device.GetUnlock())
                            {
                                m_Device.Reset();
                                SequenceDeviceLog.WriteLog(string.Format("{0} OutRigger Backward OK", m_Device.MyName));
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (m_Device.IsAlarm())
                            {
                                m_Device.Reset();
                                rv = m_Device.ALM_OutRiggerAlarm.ID;
                                SequenceDeviceLog.WriteLog(string.Format("{0} OutRigger Alarm", m_Device.MyName));
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Device.OutRiggerChangeTime * 1000)
                            {
                                m_Device.Reset();
                                rv = m_Device.ALM_BackwardTimeout.ID;
                                SequenceDeviceLog.WriteLog(string.Format("{0} OutRigger Backward Timeout", m_Device.MyName));
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

                var helperXml = new XmlHelper<DevOutRigger>();
                DevOutRigger dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.DoLock = dev.DoLock;
                    this.DoUnlock = dev.DoUnlock;
                    this.DoStart = dev.DoStart;
                    this.DoStop = dev.DoStop;
                    this.DoAlarmReset = dev.DoAlarmReset;

                    this.DiLock = dev.DiLock;
                    this.DiUnlock = dev.DiUnlock;
                    this.DiAlarm = dev.DiAlarm;

                    this.OutRiggerChangeTime = dev.OutRiggerChangeTime;
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
                var helperXml = new XmlHelper<DevOutRigger>();
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
