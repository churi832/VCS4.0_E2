using Sineva.VHL.Data;
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

namespace Sineva.VHL.Device.Assem
{
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class _DevSwitch : _Device
    {
        private const string DevName = "LampSwitch";
        #region Field
        private IoTag m_DiSwitchPushed = new IoTag();
        private IoTag m_DoSwitchLamp = new IoTag();

        private uint m_ToggleInterval = 500;

        private LampState m_LampCommand = LampState.Off;
        private bool m_IsPushedSwitch = false; // Program Set
        private bool m_PushedSwitch = false;   // I/O Set
        private bool m_NoneIoType = true;

        private SeqAction m_SeqAction = null;
        #endregion

        #region Property I/O Setting
        [Category("!Setting Device (IO is NONE)"), Description(""), DeviceSetting(true)]
        public bool NoneIoType
        {
            get { return m_NoneIoType; }
            set { m_NoneIoType = value; }
        }
        [Category("!Setting Device (Lamp Switch Input)"), Description(""), DeviceSetting(true)]
        public IoTag DiSwitchPushed { get { return m_DiSwitchPushed; } set { m_DiSwitchPushed = value; } }
        [Category("!Setting Device (Lamp Switch Output)"), Description(""), DeviceSetting(true)]
        public IoTag DoSwitchLamp { get { return m_DoSwitchLamp; } set { m_DoSwitchLamp = value; } }
        #endregion

        #region Property Variable
        [Category("!Setting Device"), Description("Lamp On/Off Toggle Interval (ms)"), DeviceSetting(true)]
        public uint ToggleInterval
        {
            get { return m_ToggleInterval; }
            set
            {
                m_ToggleInterval = value;
                if (value > 5000) m_ToggleInterval = 5000;
                else if (value < 100) m_ToggleInterval = 100;
            }
        }

        [Browsable(false), XmlIgnore()]
        public bool IsPushedSwitch
        {
            get
            {
                if (Initialized) m_PushedSwitch = _GetSwitch();
                return m_PushedSwitch || m_IsPushedSwitch;
            }
            set
            {
                m_IsPushedSwitch = value;
                if (m_IsPushedSwitch) SetLamp(LampState.On);
                else SetLamp(LampState.Off);
            }
        }
        #endregion

        #region Constructor
        public _DevSwitch()
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
            if (name != "") this.ParentName = name;
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
            if (NoneIoType == false)
            {
                ok &= DoSwitchLamp.GetChannel() != null ? true : false;
                ok &= DiSwitchPushed.GetChannel() != null ? true : false;
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
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;
            if (ToggleInterval == 0) ToggleInterval = 500; // default 1sec
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            if (ok) m_SeqAction = new SeqAction(this);
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

        #region Method
        public LampState GetLampState()
        {
            return m_LampCommand;
        }
        public void SetLamp(LampState lamp)
        {
            m_LampCommand = lamp;
        }
        private void _SetLamp(bool flag)
        {
            if (!Initialized || NoneIoType) return;

            if (this.DoSwitchLamp != null && this.DoSwitchLamp.GetChannel() != null)
            {
                this.DoSwitchLamp.GetChannel().SetDo(flag);
            }
        }
        private bool _GetSwitch()
        {
            if (!Initialized || NoneIoType) return false;

            bool rv = false;
            if (this.DiSwitchPushed != null && this.DiSwitchPushed.GetChannel() != null)
                rv = this.DiSwitchPushed.GetChannel().GetDi() ? true : false;
            return rv;
        }
        #endregion

        #region Sequence
        private class SeqAction : XSeqFunc
        {
            #region Field
            private _DevSwitch m_Device = null;
            private bool m_ToggleOn = false;
            #endregion

            #region Constructor
            public SeqAction(_DevSwitch device)
            {
                this.SeqName = $"SeqAction{device.MyName}";
                m_Device = device;
                StartTicks = XFunc.GetTickCount();

                TaskDeviceControl.Instance.RegSeq(this);
            }
            #endregion

            #region Override
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                if (!m_Device.Initialized) return -1;

                SyncDeviceFlag();
                int seqNo = SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_Device.m_LampCommand == LampState.Toggle)
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else
                            {
                                m_ToggleOn = false;
                            }
                        }
                        break;
                    case 10:
                        {
                            if (GetElapsedTicks() > m_Device.ToggleInterval)
                            {
                                m_ToggleOn = !m_ToggleOn;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                SeqNo = seqNo;
                return -1;
            }
            #endregion

            #region Method
            private void SyncDeviceFlag()
            {
                // IsPushedSwitch를 계속 Update 하면 않됨...
                //m_Device.IsPushedSwitch = m_Device._GetSwitch();
                if (m_Device.m_LampCommand == LampState.On || m_ToggleOn || m_Device.IsPushedSwitch) m_Device._SetLamp(true);
                else if (m_Device.m_LampCommand == LampState.Off || !m_ToggleOn) m_Device._SetLamp(false);
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

                var helperXml = new XmlHelper<_DevSwitch>();
                _DevSwitch dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.DiSwitchPushed = dev.DiSwitchPushed;
                    this.DoSwitchLamp = dev.DoSwitchLamp;
                    this.ToggleInterval = dev.ToggleInterval;
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
                var helperXml = new XmlHelper<_DevSwitch>();
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
