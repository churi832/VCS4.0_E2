using Sineva.VHL.Data;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Sineva.VHL.Device
{
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class DevInterlockEmo : _Device
    {
        private const string DevName = "DevInterlockEmo";

        #region Field
        private List<_DevInterlockSensor> m_Emos = new List<_DevInterlockSensor>();
        private bool m_Alarm = false;
        private SeqAction m_SeqAction = null;
        #endregion

        #region Property - I/O, Device Relation
        [Category("!Setting Device (EMO))"), Description(""), DeviceSetting(true, true)]
        public List<_DevInterlockSensor> Emos
        {
            get { return m_Emos; }
            set { m_Emos = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public bool Alarm
        {
            get { return m_Alarm; }
            set { m_Alarm = value; }
        }
        #endregion

        #region Constructor
        public DevInterlockEmo()
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
            ok &= m_Emos != null;
            foreach (_DevInterlockSensor dc in m_Emos)
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
            if (!Initialized) return false;
            if (i >= Emos.Count) return false;
            return Emos[i].DiSensor.GetChannel().GetDi();
        }
        #endregion

        #region Sequence
        private class SeqAction : XSeqFunc
        {
            #region Fields
            DevInterlockEmo m_Device = null;
            private bool[] m_SetAlarm = null;
            private uint[] m_TicksCount = null;
            #endregion

            #region Constructor
            public SeqAction(DevInterlockEmo device)
            {
                this.SeqName = $"SeqAction{device.MyName}";
                if (device != null && device.Emos != null)
                {
                    m_Device = device;
                    m_SetAlarm = new bool[m_Device.Emos.Count];
                    m_TicksCount = new uint[m_Device.Emos.Count];
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
                            uint curTicks = XFunc.GetTickCount();
                            bool alarm = false;
                            for (int i = 0; i < m_Device.Emos.Count; i++)
                            {
                                if (m_Device.Emos[i] == null) return -1;
                                alarm |= m_Device.Emos[i].GetState() == SensorAction.On ? true : false;

                                if (m_Device.Emos[i].GetState() == SensorAction.On && 
                                    m_Device.Emos[i].Fault &&
                                    m_SetAlarm[i] == false)
                                {
                                    string _msg = string.Format("{0} Alarm On", m_Device.Emos[i].MyName);
                                    SequenceDeviceLog.WriteLog(string.Format("{0} {1}", m_Device.MyName, _msg));
                                    m_SetAlarm[i] = true;
                                    EqpAlarm.Set(m_Device.Emos[i].ALM_Interlock.ID);

                                    GV.EmoAlarm = true;
                                    m_TicksCount[i] = curTicks;
                                }

                                if (m_Device.Emos[i].GetState() == SensorAction.Off &&
                                    m_SetAlarm[i] == true && 
                                    curTicks - m_TicksCount[i] > 1000)
                                {
                                    string _msg = string.Format("{0} Alarm Off", m_Device.Emos[i].MyName);
                                    SequenceDeviceLog.WriteLog(string.Format("{0} {1}", m_Device.MyName, _msg));
                                    m_SetAlarm[i] = false;
                                    EqpAlarm.Reset(m_Device.Emos[i].ALM_Interlock.ID);

                                    if (alarm == false) GV.EmoAlarm = false;
                                }
                            }
                            if (m_Device.Alarm != alarm) EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.SaftyInterlock, alarm ? "SHOW" : "HIDE");

                            m_Device.Alarm = alarm;
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

                var helperXml = new XmlHelper<DevInterlockEmo>();
                DevInterlockEmo dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.Emos = dev.Emos;
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
                var helperXml = new XmlHelper<DevInterlockEmo>();
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
