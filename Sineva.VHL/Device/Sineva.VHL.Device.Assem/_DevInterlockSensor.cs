using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Data.Alarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing.Design;
using static System.Collections.Specialized.BitVector32;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using Sineva.VHL.Data;

namespace Sineva.VHL.Device.Assem
{
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class _DevInterlockSensor : _Device
    {
        private const string DevName = "InterlockSensor";

        #region Field
        private uint m_SensorKeepTime = 100;
        private AlarmData m_ALM_Interlock = null;

        private SeqAction m_SeqAction = null;
        private SensorAction m_State = SensorAction.None;
        private bool m_Fault = false;

        private Use m_InterlockUsage = Use.Use;
        private AlarmLevel m_InterlockLevel = AlarmLevel.S;

        private IoTag m_DiSensor = new IoTag();
        private double m_SvHigh = 0.0f;
        private double m_SvLow = 0.0f;
        #endregion

        #region Property - Alarm
        [Category("!Setting Device (Sensor Keep Timeout(ms))"), Description("Timeout(ms)"), DeviceSetting(false, true)]
        public uint SensorKeepTime
        {
            get { return m_SensorKeepTime; }
            set { m_SensorKeepTime = value; }
        }
        [Category("!Setting Device (Sensor Interlock Usage)"), Description(""), DeviceSetting(false, true)]
        public Use InterlockUsage
        {
            get { return m_InterlockUsage; }
            set { m_InterlockUsage = value; }
        }
        [Category("!Setting Device (Interlock)"), DisplayName("Inerlock Level : Alarm or Warning"), Description("S: Alarm\nL: Warning"), DeviceSetting(true, false)]
        public AlarmLevel InterlockLevel
        {
            get { return m_InterlockLevel; }
            set { m_InterlockLevel = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public bool Fault
        {
            get { return m_Fault; }
            set { m_Fault = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Interlock { get { return m_ALM_Interlock; } }
        #endregion

        #region Property - I/O
        [Category("!Setting Device (Sensor Input)"), Description(""), DeviceSetting(true)]
        public IoTag DiSensor { get { return m_DiSensor; } set { m_DiSensor = value; } }
        [Category("!Setting Device (Sensor Input)"), Description(""), DeviceSetting(true)]
        public double SV_Hi { get { return m_SvHigh; } set { m_SvHigh = value; } }
        [Category("!Setting Device (Sensor Input)"), Description(""), DeviceSetting(true)]
        public double SV_Lo { get { return m_SvLow; } set { m_SvLow = value; } }
        #endregion

        #region Constructor
        public _DevInterlockSensor()
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
            // 1. 이미 초기화 완료된 상태인지 확인
            if (Initialized)
            {
                if (false)
                {
                    // 초기화된 상태에서도 변경이 가능한 항목을 추가
                }
                return true;
            }
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            // 2. 필수 I/O 할당 여부 확인
            bool ok = true;
            //ok &= new object() != null;
            //ok &= m_SubDevice.Initiated;
            ok &= DiSensor.GetChannel() != null ? true : false;
            if (!ok)
            {
                ExceptionLog.WriteLog(string.Format("Initialize Fail : Indispensable I/O is not assigned({0})", name));
                return false;
            }
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            // 3. Alarm Item 생성
            //AlarmExample = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            //if(Condition) AlarmConditionable = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            if (m_InterlockLevel == AlarmLevel.S)
            {
                m_ALM_Interlock = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, ParentName, MyName, "Interlock Sensor Check Alarm");
            }
            else
            {
                m_ALM_Interlock = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, ParentName, MyName, "Interlock Sensor Check Alarm");
            }
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            // 4. Device Variable 초기화
            //m_Variable = false;
            if (SensorKeepTime == 0) SensorKeepTime = 100;
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            m_SeqAction = new SeqAction(this);
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
        public SensorAction GetState()
        {
            if (InterlockUsage == Use.NoUse)
                m_State = SensorAction.Off;
            else
                m_State = _GetSensorState() ? SensorAction.On : SensorAction.Off;
            return m_State;
        }
        private bool _GetSensorState()
        {
            if (!Initialized) return false;

            bool rv = true;
            if (DiSensor.IoType == IoType.DI)
            {
                rv &= DiSensor.GetChannel().GetDi() ? true : false;
            }
            else if (DiSensor.IoType == IoType.AI)
            {
                double pv = DiSensor.GetChannel().GetAi();
                rv &= ((pv >= SV_Hi) || (pv <= SV_Lo)) ? true : false;
            }
            return rv;
        }

        public float GetPV()
        {
            float rv = 0.0f;
            if ((InterlockUsage == Use.Use) && (DiSensor.IoType == IoType.AI))
            {
                rv = (float)DiSensor.GetChannel().GetAi();
            }
            return rv;
        }
        #endregion

        #region Sequence
        private class SeqAction : XSeqFunc
        {
            #region Field
            private _DevInterlockSensor m_Device = null;
            private SensorAction m_CurState = SensorAction.None;
            #endregion
            public SeqAction(_DevInterlockSensor device)
            {
                this.SeqName = $"SeqAction{device.MyName}";
                m_Device = device;
                TaskDeviceControlInterlock.Instance.RegSeq(this);
            }

            public override int Do()
            {
                if (!m_Device.Initialized) return -1;

                int seqNo = SeqNo;
                int rv = -1;

                m_CurState = m_Device._GetSensorState() ? SensorAction.On : SensorAction.Off;

                switch (seqNo)
                {
                    case 0:
                        {
                            // Set Alarm - Sensor ON일때 Alarm
                            m_Device.Fault = false;
                            m_Device.m_State = m_CurState;
                            if (m_CurState == SensorAction.On)
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 100;
                            }
                        }
                        break;

                    case 100:
                        {
                            if (m_CurState == SensorAction.Off || m_Device.InterlockUsage == Use.NoUse)
                            {
                                m_Device.Fault = false;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Device.SensorKeepTime)
                            {
                                if (m_Device.m_State != SensorAction.Alarm)
                                {
                                    m_Device.Fault = true;
                                    m_Device.m_State = SensorAction.Alarm;
                                    StartTicks = XFunc.GetTickCount();
                                }
                            }
                        }
                        break;
                }

                SeqNo = seqNo;
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

                var helperXml = new XmlHelper<_DevInterlockSensor>();
                _DevInterlockSensor dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.DiSensor = dev.DiSensor;
                    this.SV_Hi = dev.SV_Hi;
                    this.SV_Lo = dev.SV_Lo;
                    this.SensorKeepTime = dev.SensorKeepTime;
                    this.InterlockUsage = dev.InterlockUsage;
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
                var helperXml = new XmlHelper<_DevInterlockSensor>();
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
