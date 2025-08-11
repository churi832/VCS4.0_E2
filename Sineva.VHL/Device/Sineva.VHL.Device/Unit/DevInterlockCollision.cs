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
    public class DevInterlockCollision : _Device
    {
        private const string DevName = "DevInterlockCollision";

        #region Field
        private List<_DevInterlockSensor> m_BumperCollisions = new List<_DevInterlockSensor>();
        private bool m_Alarm = false;
        private SeqAction m_SeqAction = null;
        #endregion

        #region Property - I/O, Device Relation
        [Category("!Setting Device (EMO))"), Description(""), DeviceSetting(true, true)]
        public List<_DevInterlockSensor> BumperCollisions
        {
            get { return m_BumperCollisions; }
            set { m_BumperCollisions = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public bool Alarm
        {
            get { return m_Alarm; }
            set { m_Alarm = value; }
        }
        #endregion

        #region Constructor
        public DevInterlockCollision()
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
            ok &= m_BumperCollisions != null;
            foreach (_DevInterlockSensor dc in m_BumperCollisions)
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
            if (SetupManager.Instance.SetupSafty.BumpCollisionCheckUse == Use.NoUse) return false;
            if (!Initialized) return false;
            if (i >= m_BumperCollisions.Count) return false;
            return m_BumperCollisions[i].DiSensor.GetChannel().GetDi();
        }
        #endregion

        #region Sequence
        private class SeqAction : XSeqFunc
        {
            #region Fields
            DevInterlockCollision m_Device = null;
            private bool[] m_SetAlarm = null;
            private uint[] m_TicksCount = null;
            #endregion

            #region Constructor
            public SeqAction(DevInterlockCollision device)
            {
                this.SeqName = $"SeqAction{device.MyName}";
                if (device != null && device.m_BumperCollisions != null)
                {
                    m_Device = device;
                    m_SetAlarm = new bool[m_Device.m_BumperCollisions.Count];
                    m_TicksCount = new uint[m_Device.m_BumperCollisions.Count];
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
                            if (SetupManager.Instance.SetupSafty.BumpCollisionCheckUse == Use.Use)
                            {
                                uint curTicks = XFunc.GetTickCount();
                                bool alarm = false;
                                for (int i = 0; i < m_Device.m_BumperCollisions.Count; i++)
                                {
                                    if (m_Device.m_BumperCollisions[i] == null) return -1;
                                    alarm |= m_Device.m_BumperCollisions[i].GetState() == SensorAction.On ? true : false;

                                    if (m_Device.m_BumperCollisions[i].GetState() == SensorAction.On &&
                                        m_Device.m_BumperCollisions[i].Fault &&
                                        m_SetAlarm[i] == false)
                                    {
                                        string _msg = string.Format("{0} Alarm On", m_Device.m_BumperCollisions[i].MyName);
                                        SequenceDeviceLog.WriteLog(string.Format("{0} {1}", m_Device.MyName, _msg));
                                        m_SetAlarm[i] = true;
                                        EqpAlarm.Set(m_Device.BumperCollisions[i].ALM_Interlock.ID);

                                        GV.BumpCollisionInterlock = true;
                                        m_TicksCount[i] = curTicks;
                                    }

                                    if (m_Device.m_BumperCollisions[i].GetState() == SensorAction.Off &&
                                        m_SetAlarm[i] == true &&
                                        curTicks - m_TicksCount[i] > 1000)
                                    {
                                        string _msg = string.Format("{0} Alarm Off", m_Device.m_BumperCollisions[i].MyName);
                                        SequenceDeviceLog.WriteLog(string.Format("{0} {1}", m_Device.MyName, _msg));
                                        m_SetAlarm[i] = false;
                                        EqpAlarm.Reset(m_Device.BumperCollisions[i].ALM_Interlock.ID);

                                        if (alarm == false) GV.BumpCollisionInterlock = false;
                                    }
                                }
                                if (m_Device.Alarm != alarm) EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.SaftyInterlock, alarm ? "SHOW" : "HIDE");

                                m_Device.Alarm = alarm;
                            }
                            else if (m_Device.Alarm)
                            {
                                m_Device.Alarm = false;
                                for (int i = 0; i < m_Device.m_BumperCollisions.Count; i++)
                                {
                                    if (m_Device.m_BumperCollisions[i] == null) return -1;
                                    if (m_SetAlarm[i] == true)
                                    {
                                        m_SetAlarm[i] = false;
                                        string _msg = string.Format("{0} Alarm Off", m_Device.m_BumperCollisions[i].MyName);
                                        SequenceDeviceLog.WriteLog(string.Format("{0} {1}", m_Device.MyName, _msg));
                                        EqpAlarm.Reset(m_Device.BumperCollisions[i].ALM_Interlock.ID);
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

                var helperXml = new XmlHelper<DevInterlockCollision>();
                DevInterlockCollision dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.m_BumperCollisions = dev.m_BumperCollisions;
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
                var helperXml = new XmlHelper<DevInterlockCollision>();
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
