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
    public class DevInterlockBeltCut : _Device
    {
        private const string DevName = "DevInterlockBeltCut";

        #region Field
        private List<_DevInterlockSensor> m_BeltCuts = new List<_DevInterlockSensor>();
        private bool m_Alarm = false;
        private SeqAction m_SeqAction = null;
        #endregion

        #region Property - I/O, Device Relation
        [Category("!Setting Device (BeltCut))"), Description(""), DeviceSetting(true, true)]
        public List<_DevInterlockSensor> BeltCuts
        {
            get { return m_BeltCuts; }
            set { m_BeltCuts = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public bool Alarm
        {
            get { return m_Alarm; }
            set { m_Alarm = value; }
        }
        #endregion

        #region Constructor
        public DevInterlockBeltCut()
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
            ok &= m_BeltCuts != null;
            foreach (_DevInterlockSensor dc in m_BeltCuts)
                if (dc.IsValid) ok &= dc.Initialize(this.ToString(), false, true);

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
        public bool GetDi(int i)
        {
            if (SetupManager.Instance.SetupSafty.BeltCutCheckUse == Use.NoUse) return false;
            if (!Initialized) return false;
            if (i >= m_BeltCuts.Count) return false;
            return m_BeltCuts[i].DiSensor.GetChannel().GetDi();
        }
        #endregion

        #region Sequence
        private class SeqAction : XSeqFunc
        {
            #region Fields
            DevInterlockBeltCut m_Device = null;
            private bool[] m_SetAlarm = null;
            private uint[] m_TicksCount = null;
            #endregion

            #region Constructor
            public SeqAction(DevInterlockBeltCut device)
            {
                this.SeqName = $"SeqAction{device.MyName}";
                if (device != null && device.m_BeltCuts != null)
                {
                    m_Device = device;
                    m_SetAlarm = new bool[m_Device.m_BeltCuts.Count];
                    m_TicksCount = new uint[m_Device.m_BeltCuts.Count];
                    TaskDeviceControlInterlock.Instance.RegSeq(this);
                }
            }
            #endregion

            #region override
            public override int Do()
            {
                if (!IoManager.Instance.UpdateRun) return -1;
                if (!m_Device.Initialized) return -1;

                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupSafty.BeltCutCheckUse == Use.Use)
                            {
                                uint curTicks = XFunc.GetTickCount();
                                bool alarm = false;
                                for (int i = 0; i < m_Device.m_BeltCuts.Count; i++)
                                {
                                    if (m_Device.m_BeltCuts[i] == null) return -1;
                                    bool sensor_on = m_Device.m_BeltCuts[i].GetState() == SensorAction.On;
                                    alarm |= sensor_on ? true : false;

                                    if (sensor_on &&
                                        m_Device.m_BeltCuts[i].Fault &&
                                        m_SetAlarm[i] == false)
                                    {
                                        string _msg = string.Format("{0} Alarm On", m_Device.m_BeltCuts[i].MyName);
                                        SequenceDeviceLog.WriteLog(string.Format("{0} {1}", m_Device.MyName, _msg));
                                        m_SetAlarm[i] = true;
                                        EqpAlarm.Set(m_Device.BeltCuts[i].ALM_Interlock.ID);

                                        GV.BeltCutInterlock = true;
                                        m_TicksCount[i] = curTicks;
                                    }

                                    if (!sensor_on &&
                                        m_SetAlarm[i] == true &&
                                        curTicks - m_TicksCount[i] > 1000)
                                    {
                                        string _msg = string.Format("{0} Alarm Off", m_Device.m_BeltCuts[i].MyName);
                                        SequenceDeviceLog.WriteLog(string.Format("{0} {1}", m_Device.MyName, _msg));
                                        m_SetAlarm[i] = false;
                                        EqpAlarm.Reset(m_Device.BeltCuts[i].ALM_Interlock.ID);

                                        if (alarm == false) GV.BeltCutInterlock = false;
                                    }
                                }
                                if (m_Device.Alarm != alarm) EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.SaftyInterlock, alarm ? "SHOW" : "HIDE");

                                m_Device.Alarm = alarm;
                            }
                            else if (m_Device.Alarm)
                            {
                                m_Device.Alarm = false;
                                for (int i = 0; i < m_Device.m_BeltCuts.Count; i++)
                                {
                                    if (m_Device.m_BeltCuts[i] == null) return -1;
                                    if (m_SetAlarm[i] == true)
                                    {
                                        m_SetAlarm[i] = false;
                                        string _msg = string.Format("{0} Alarm Off", m_Device.m_BeltCuts[i].MyName);
                                        SequenceDeviceLog.WriteLog(string.Format("{0} {1}", m_Device.MyName, _msg));
                                        EqpAlarm.Reset(m_Device.m_BeltCuts[i].ALM_Interlock.ID);
                                    }
                                }
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
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

                var helperXml = new XmlHelper<DevInterlockBeltCut>();
                DevInterlockBeltCut dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.m_BeltCuts = dev.m_BeltCuts;
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
                var helperXml = new XmlHelper<DevInterlockBeltCut>();
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
