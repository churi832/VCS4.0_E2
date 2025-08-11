using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Net;
using System.Windows.Forms;
using System.IO;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Library;
using Sineva.VHL.Data.Alarm;
using static System.Collections.Specialized.BitVector32;
using System.Drawing.Design;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data;

namespace Sineva.VHL.Device
{
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class ObsTable
    {
        #region Fields
        #endregion
        #region Properties
        public int Far { get; set; } // 8000
        public int Center { get; set; } // 4000
        public int Near { get; set; } // 1000
        #endregion

        #region Constructor
        public ObsTable()
        {
        }
        #endregion
    }

    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class DevOBS : _Device
    {
        private const string DevName = "DevOBS";

        #region Fields
        private _DevOBS m_OBS = new _DevOBS();

        /////////////////////////////////////////
        private AlarmData m_ALM_Command_Set = null;
        private int m_SetDelayTime = 100; // Level Change Delay Time
        private SeqAction m_SeqAction = null;
        private SeqMonitor m_SeqMonitor = null;

        private enFrontDetectState m_FrontDetectState = enFrontDetectState.enNone;
        private uint m_SetOBS = 0;
        private bool m_UsigOBS = true;

        private List<ObsTable> m_DistanceTable = new List<ObsTable>();
        #endregion

        #region Property - Device
        [Category("!Setting Device (I/O Setting)"), Description("OBS I/O"), DeviceSetting(true)]
        public _DevOBS OBS { get { return m_OBS; } set { m_OBS = value; } }
        #endregion
        #region Property - Timer Setting
        [Category("!Setting Device (SetTime)"), DisplayName("Level Set Delay Time(msec)"), Description("Level Set Time(msec)"), DeviceSetting(true)]
        public int SetDelayTime { get { return m_SetDelayTime; } set { m_SetDelayTime = value; } }
        [Category("!Setting Device"), DisplayName("Level Set Distance(mm)"), Description("Level Set Distance(mm)"), DeviceSetting(true)]
        public List<ObsTable> DistanceTable
        {
            get { return m_DistanceTable; }
            set { m_DistanceTable = value; }
        }
        #endregion

        #region AlarmData
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Command_Set
        {
            get { return m_ALM_Command_Set; }
            set { m_ALM_Command_Set = value; }
        }

        //public double[,] OBSDistanceTable
        //{
        //    get { return m_OBSDistanceTable; }
        //    set { m_OBSDistanceTable = value; }
        //}
        #endregion

        #region Constructor
        public DevOBS()
        {
            this.MyName = "DevOBS";
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
            if (m_OBS.IsValid) ok &= m_OBS.Initialize(this.ToString(), false, heavy_alarm);

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
            m_ALM_Command_Set = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, "Lower.OBS", "Command Setting Alarm");
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            SetOBS(1);
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            if (ok) m_SeqAction = new SeqAction(this);
            if (ok) m_SeqMonitor = new SeqMonitor(this);
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

        #region Methods - public
        public void SetNoUse()
        {
            m_UsigOBS = false;
            m_OBS.SetAllOff();
        }
        public bool SetOBS(uint type) 
        {
            m_UsigOBS = true;
            bool rv = m_SetOBS != type;
            m_SetOBS = type;
            return rv;
        }
        public uint GetOBS() { return m_SetOBS; }
        public enFrontDetectState GetFrontDetectState() { return m_FrontDetectState; }
        public double GetTableDistanceOfDetectState(enFrontDetectState beforeState)
        {
            double rv = double.MaxValue;
            int row = (int)m_SetOBS;
            if (row < DistanceTable.Count)
            {
                // near Case
                if (beforeState < m_FrontDetectState)
                {
                    if (m_FrontDetectState == enFrontDetectState.enDeccelation1) rv = DistanceTable[row].Far; // 8000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation2) rv = DistanceTable[row].Center; // 4000
                    else if (m_FrontDetectState == enFrontDetectState.enStop) rv = DistanceTable[row].Near; // 1000
                }
                // far away Case
                else
                {
                    if (m_FrontDetectState == enFrontDetectState.enStop) rv = DistanceTable[row].Near; // 1000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation2) rv = DistanceTable[row].Near; // 1000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation1) rv = DistanceTable[row].Center; // 4000
                    else if (m_FrontDetectState == enFrontDetectState.enNone) rv = DistanceTable[row].Far; // > 8000
                }
            }
            return rv;
        }
        public double GetTableAreaMaxDistance()
        {
            double rv = 10000.0f; //straight type 최대거리 10m
            int row = (int)m_SetOBS;
            if (row < DistanceTable.Count)
            {
                rv = DistanceTable[row].Far;
            }
            return rv;
        }
        public double GetEnableAccelerationMinDistance()
        {
            double rv = 10000.0f; //straight type 최대거리 10m
            int row = (int)m_SetOBS;
            if (row < DistanceTable.Count)
            {
                rv = DistanceTable[row].Center;
            }
            return rv;
        }

        public bool IsUsingObs()
        {
            return m_UsigOBS;
        }
        #endregion

        #region Sequence
        private class SeqAction : XSeqFunc
        {
            #region Field
            private DevOBS m_Device = null;
            #endregion

            #region Constructor
            public SeqAction(DevOBS device)
            {
                this.SeqName = $"SeqAction{device.MyName}";
                m_Device = device;
                StartTicks = XFunc.GetTickCount();

                TaskDeviceControl.Instance.RegSeq(this);
            }
            #endregion

            #region Override
            public override int Do()
            {
                if (!m_Device.Initialized) return -1;

                int seqNo = SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_Device.m_SetOBS != m_Device.OBS.GetOBSType())
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0} OBS Change [Before={1}, After={2}]", m_Device.MyName, m_Device.OBS.GetOBSType(), m_Device.m_SetOBS));
                                m_Device.OBS.SetOBSType(m_Device.m_SetOBS);
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                        }
                        break;

                    case 10:
                        {
                            // 전방감지 설정할 시간을 주자
                            if (GetElapsedTicks() > m_Device.SetDelayTime)
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0} OBS Change Delay [{1}(ms)]", m_Device.MyName, m_Device.SetDelayTime));
                                seqNo = 0;
                            }
                        }
                        break;
                }

                SeqNo = seqNo;
                return -1;
            }
            #endregion
        }
        private class SeqMonitor : XSeqFunc
        {
            #region Field
            private DevOBS m_Device = null;
            #endregion

            #region Constructor
            public SeqMonitor(DevOBS device)
            {
                this.SeqName = $"SeqMonitor{device.MyName}";
                m_Device = device;
                StartTicks = XFunc.GetTickCount();

                TaskDeviceControl.Instance.RegSeq(this);
            }
            #endregion

            #region Override
            public override int Do()
            {
                if (!m_Device.Initialized) return -1;

                int seqNo = SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            // Real Time Monitoring
                            enFrontDetectState detect_state = enFrontDetectState.enNone;
                            if (m_Device.OBS.GetUSB3()) detect_state = enFrontDetectState.enStop;
                            else if (m_Device.OBS.GetUSB2()) detect_state = enFrontDetectState.enDeccelation2;
                            else if (m_Device.OBS.GetUSB1()) detect_state = enFrontDetectState.enDeccelation1;
                            if (detect_state != m_Device.m_FrontDetectState)
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0} OBS State Change [Before={1}, After={2}]", m_Device.MyName, m_Device.m_FrontDetectState, detect_state));
                                m_Device.m_FrontDetectState = detect_state;
                            }

                            seqNo = 0;
                        }
                        break;
                }

                SeqNo = seqNo;
                return -1;
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

                var helperXml = new XmlHelper<DevOBS>();
                DevOBS dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;
                    this.SetDelayTime = dev.SetDelayTime;

                    this.OBS = dev.OBS;
                    this.DistanceTable = dev.DistanceTable;
                }
            }
            catch (Exception err)
            {
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
                var helperXml = new XmlHelper<DevOBS>();
                helperXml.Save(fileName, this);
            }
            catch (Exception err)
            {
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
