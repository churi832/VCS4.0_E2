using Sineva.VHL.Data;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Sineva.VHL.Device.Assem
{
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class _DevCylinder : _Device
    {
        private const string DevName = "Cylinder";

        #region Field
        private List<IoTag> m_DiSensorFw = new List<IoTag>();
        private List<IoTag> m_DiSensorBw = new List<IoTag>();
        private List<IoTag> m_DoSolFw = new List<IoTag>();
        private List<IoTag> m_DoSolBw = new List<IoTag>();

        private AlarmData m_ALM_NotDefine = null;
        private AlarmData m_ALM_CylinderFwTimeout = null;
        private AlarmData m_ALM_CylinderBwTimeout = null;

        private int m_CylinderCounts = 0;
        private double m_CylinderActionTime = 30;
        private double m_CylinderCheckDelayTime = 5;
        private float m_ForcesFw = 10.0f;
        private float m_ForcesBw = 0.0f;
        private SeqAction m_SeqAction = null;
        private int m_SteerLeftCount = 0;
        private int m_SteerRightCount = 0;
        #endregion

        #region Property - Setting
        [Category("!Setting Device"), Description("Cylinder Count"), DeviceSetting(false, true)]
        public int CylinderCounts
        {
            get
            {
                int count = m_CylinderCounts;
                if (count == 0) count = Math.Max(m_DoSolFw.Count, m_DoSolBw.Count);
                return count;
            }
            set { m_CylinderCounts = value; }
        }
        [Category("!Setting Device (Timeout)"), Description("Sensor Check Delay Timeout (sec)"), DeviceSetting(false, true)]
        public double CylinderCheckDelayTime
        {
            get { return m_CylinderCheckDelayTime; }
            set { m_CylinderCheckDelayTime = value; }
        }
        [Category("!Setting Device (Timeout)"), Description("Acuator Action Timeout (sec)"), DeviceSetting(false, true)]
        public double CylinderActionTime
        {
            get { return m_CylinderActionTime; }
            set { m_CylinderActionTime = value; }
        }
        [Category("!Setting Device (Forward Input)"), Description(""), DeviceSetting(true)]
        public List<IoTag> DiSensorFw
        {
            get { return m_DiSensorFw; }
            set { m_DiSensorFw = value; }
        }
        [Category("!Setting Device (Backward Input)"), Description(""), DeviceSetting(true)]
        public List<IoTag> DiSensorBw
        {
            get { return m_DiSensorBw; }
            set { m_DiSensorBw = value; }
        }
        [Category("!Setting Device (Forward Output)"), Description(""), DeviceSetting(true)]
        public List<IoTag> DoSolFw
        {
            get { return m_DoSolFw; }
            set { m_DoSolFw = value; }
        }
        [Category("!Setting Device (Backward Output)"), Description(""), DeviceSetting(true)]
        public List<IoTag> DoSolBw
        {
            get { return m_DoSolBw; }
            set { m_DoSolBw = value; }
        }
        [Category("!Setting Device"), Description("Cylinder Fw Force"), DeviceSetting(false, true)]
        public float ForcesFw
        {
            get { return m_ForcesFw; }
            set { m_ForcesFw = value; }
        }
        [Category("!Setting Device"), Description("Cylinder Bw Force"), DeviceSetting(false, true)]
        public float ForcesBw
        {
            get { return m_ForcesBw; }
            set { m_ForcesBw = value; }
        }
        [Category("!LifeTime Manager"), DisplayName("SteerLeft Count"), Description("Device Life SteerLeft Count"), DeviceSetting(false, true)]
        public int SteerLeftCount
        {
            get { return m_SteerLeftCount; }
            set { SaveCurState = m_SteerLeftCount != value; m_SteerLeftCount = value; }
        }
        [Category("!LifeTime Manager"), DisplayName("SteerRight Count"), Description("Device Life SteerRight Count"), DeviceSetting(false, true)]
        public int SteerRightCount
        {
            get { return m_SteerRightCount; }
            set { SaveCurState = m_SteerRightCount != value; m_SteerRightCount = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_NotDefine
        {
            get { return m_ALM_NotDefine; }
            set { m_ALM_NotDefine = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_CylinderFwTimeout
        {
            get { return m_ALM_CylinderFwTimeout; }
            set { m_ALM_CylinderFwTimeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_CylinderBwTimeout
        {
            get { return m_ALM_CylinderBwTimeout; }
            set { m_ALM_CylinderBwTimeout = value; }
        }
        #endregion

        #region Constructor
        public _DevCylinder()
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
            #region 2. Alarm Item 생성
            //AlarmExample = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            //if(Condition) AlarmConditionable = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            ALM_NotDefine = AlarmListProvider.Instance.NewAlarm(AlarmCode.ParameterControlError, heavy_alarm, ParentName, MyName, "Not Define Alarm");
            ALM_CylinderFwTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, heavy_alarm, ParentName, MyName, "Forward Timeout");
            ALM_CylinderBwTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, heavy_alarm, ParentName, MyName, "Backward Timeout");
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 3. 필수 I/O 할당 여부 확인
            bool ok = true;
            //ok &= new object() != null;
            //ok &= m_SubDevice.Initiated;

            ok &= DiSensorFw != null;
            if (ok) foreach (IoTag io in DiSensorFw) ok &= io.GetChannel() != null ? true : false;
            ok &= DiSensorBw != null;
            if (ok) foreach (IoTag io in DiSensorBw) ok &= io.GetChannel() != null ? true : false;
            ok &= DoSolFw != null;
            if (ok) foreach (IoTag io in DoSolFw) ok &= io.GetChannel() != null ? true : false;
            ok &= DoSolBw != null;
            if (ok) foreach (IoTag io in DoSolBw) ok &= io.GetChannel() != null ? true : false;

            if (!ok)
            {
                ExceptionLog.WriteLog(string.Format("Initialize Fail : Indispensable I/O is not assigned({0})", name));
                return false;
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;
            if (CylinderActionTime == 0) CylinderActionTime = 30; // default 30sec
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Left Times", this, "GetLeftCount", 1000));
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Right Times", this, "GetRightCount", 1000));
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
        public override void SeqAbort()
        {
            if (!Initialized) return;

            ResetOutput();
            m_SeqAction.SeqAbort();
        }
        #endregion

        #region Method - public
        public int SetFw(int id = -1) //Left
        {
            if (!Initialized) return ALM_NotDefine.ID;

            int rv = -1;
            rv = m_SeqAction.FW(id);
            return rv;
        }
        public int SetBw(int id = -1) //Right
        {
            if (!Initialized) return ALM_NotDefine.ID;

            int rv = -1;
            rv = m_SeqAction.BW(id);
            return rv;
        }
        public bool GetFw(int id = -1)
        {
            if (!Initialized) return false;
            bool rv = true;
            rv = _GetFw(id);
            return rv;
        }
        public bool GetBw(int id = -1)
        {
            if (!Initialized) return false;
            bool rv = true;
            rv = _GetBw(id);
            return rv;
        }
        public void SetSimulFw()
        {
            foreach (IoTag io in DiSensorBw) if (io.GetChannel() != null) io.SetDo(false);
            foreach (IoTag io in DiSensorFw) if (io.GetChannel() != null) io.SetDo(true);
        }
        public void SetSimulBw()
        {
            foreach (IoTag io in DiSensorFw) if (io.GetChannel() != null) io.SetDo(false);
            foreach (IoTag io in DiSensorBw) if (io.GetChannel() != null) io.SetDo(true);
        }
        public void ResetOutput(int id = -1)
        {
            if (!Initialized) return;
            if (id == -1)
            {
                foreach (IoTag io in DoSolBw) if (io.GetChannel() != null) io.SetDo(false);
                foreach (IoTag io in DoSolFw) if (io.GetChannel() != null) io.SetDo(false);
            }
            else
            {
                if (id >= DiSensorFw.Count || id >= DiSensorBw.Count) return;
                if (DoSolBw[id].GetChannel() != null) DoSolBw[id].SetDo(false);
                if (DoSolFw[id].GetChannel() != null) DoSolFw[id].SetDo(false);
            }
        }
        public bool GetFwState(int id = -1)
        {
            if (!Initialized) return false;
            bool rv = true;
            if (id == -1)
            {
                foreach (IoTag io in DoSolFw) if (io.GetChannel() != null) rv &= io.GetDi();
                foreach (IoTag io in DoSolBw) if (io.GetChannel() != null) rv &= !io.GetDi();
            }
            else
            {
                if (DoSolFw[id].GetChannel() != null) rv &= DoSolFw[id].GetDi();
                if (DoSolBw[id].GetChannel() != null) rv &= !DoSolBw[id].GetDi();
            }
            return rv;
        }
        public bool GetBwState(int id = -1)
        {
            if (!Initialized) return false;

            bool rv = true;
            if (id == -1)
            {
                foreach (IoTag io in DoSolFw) if (io.GetChannel() != null) rv &= !io.GetDi();
                foreach (IoTag io in DoSolBw) if (io.GetChannel() != null) rv &= io.GetDi();
            }
            else
            {
                if (DoSolFw[id].GetChannel() != null) rv &= !DoSolFw[id].GetDi();
                if (DoSolBw[id].GetChannel() != null) rv &= DoSolBw[id].GetDi();
            }
            return rv;
        }
        #region Methods - Life Time
        public int GetLeftCount()
        {
            return m_SteerLeftCount;
        }
        public int GetRightCount()
        {
            return m_SteerRightCount;
        }
        #endregion
        #endregion
        #region Method -
        public void _SetFw(int id = -1)
        {
            if (!Initialized) return;
            if (id == -1)
            {
                foreach (IoTag io in DoSolBw) if (io.GetChannel() != null) io.SetDo(false);
                foreach (IoTag io in DoSolFw) if (io.GetChannel() != null) io.SetDo(true);
            }
            else
            {
                if (id >= DiSensorFw.Count || id >= DiSensorBw.Count) return;
                if (DoSolBw[id].GetChannel() != null) DoSolBw[id].SetDo(false);
                if (DoSolFw[id].GetChannel() != null) DoSolFw[id].SetDo(true);
            }
            if (AppConfig.Instance.Simulation.IO) SetSimulFw();
        }
        public void _SetBw(int id = -1)
        {
            if (!Initialized) return;
            if (id == -1)
            {
                foreach (IoTag io in DoSolFw) if (io.GetChannel() != null) io.SetDo(false);
                foreach (IoTag io in DoSolBw) if (io.GetChannel() != null) io.SetDo(true);
            }
            else
            {
                if (id >= DiSensorFw.Count || id >= DiSensorBw.Count) return;
                if (DoSolFw[id].GetChannel() != null) DoSolFw[id].SetDo(false);
                if (DoSolBw[id].GetChannel() != null) DoSolBw[id].SetDo(true);
            }
            if (AppConfig.Instance.Simulation.IO) SetSimulBw();
        }
        private bool _GetFw(int id = -1)
        {
            if (!Initialized) return false;
            bool rv = true;
            if (id == -1)
            {
                foreach (IoTag io in DiSensorFw) if (io.GetChannel() != null) rv &= io.GetDi() ? true : false;
                foreach (IoTag io in DiSensorBw) if (io.GetChannel() != null) rv &= io.GetDi() ? false : true;
            }
            else
            {
                if (DiSensorBw[id].GetChannel() != null) rv &= DiSensorBw[id].GetDi() ? false : true;
                if (DiSensorFw[id].GetChannel() != null) rv &= DiSensorFw[id].GetDi() ? true : false;
            }
            return rv;
        }
        private bool _GetBw(int id = -1)
        {
            if (!Initialized) return false;

            bool rv = true;
            if (id == -1)
            {
                foreach (IoTag io in DiSensorFw) if (io.GetChannel() != null) rv &= io.GetDi() ? false : true;
                foreach (IoTag io in DiSensorBw) if (io.GetChannel() != null) rv &= io.GetDi() ? true : false;
            }
            else
            {
                if (DiSensorFw[id].GetChannel() != null) rv &= DiSensorFw[id].GetDi() ? false : true;
                if (DiSensorBw[id].GetChannel() != null) rv &= DiSensorBw[id].GetDi() ? true : false;
            }
            return rv;
        }
        #endregion

        #region Sequence
        private class SeqAction : XSeqFunc
        {
            #region Fields
            _DevCylinder m_Device = null;
            #endregion

            #region Constructor
            public SeqAction(_DevCylinder device)
            {
                this.SeqName = $"SeqAction{device.MyName}";
                m_Device = device;
            }

            public override void SeqAbort()
            {
                this.InitSeq();
            }
            #endregion

            #region override
            public int FW(int Id)
            {
                if (!m_Device.Initialized) return -1;

                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            m_Device.SteerLeftCount++;
                            m_Device._SetFw(Id);
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 10;
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks < m_Device.CylinderCheckDelayTime * 1000) break;

                            bool fw_check = m_Device._GetFw(Id);
                            // Forward Check
                            if (fw_check)
                            {
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Device.CylinderActionTime * 1000)
                            {
                                rv = m_Device.m_ALM_CylinderFwTimeout.ID;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            public int BW(int Id)
            {
                if (!m_Device.Initialized) return -1;

                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            m_Device.SteerRightCount++;
                            m_Device._SetBw(Id);
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 10;
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks < m_Device.CylinderCheckDelayTime * 1000) break;

                            bool bw_check = m_Device._GetBw();
                            // Backward Check
                            if (bw_check)
                            {
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Device.CylinderActionTime * 1000)
                            {
                                rv = m_Device.m_ALM_CylinderBwTimeout.ID;
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

                var helperXml = new XmlHelper<_DevCylinder>();
                _DevCylinder dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.CylinderActionTime = dev.CylinderActionTime;
                    this.DiSensorFw = dev.DiSensorFw;
                    this.DiSensorBw = dev.DiSensorBw;
                    this.DoSolFw = dev.DoSolFw;
                    this.DoSolBw = dev.DoSolBw;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
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
                var helperXml = new XmlHelper<_DevCylinder>();
                helperXml.Save(fileName, this);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
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
