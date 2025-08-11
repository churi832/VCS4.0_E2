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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Sineva.VHL.Device.Assem
{
    public enum SolenoidAction
    {
        None,
        On,
        Off,
        Busy,
        Alarm,
    }

    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class _DevSolenoidValve : _Device
    {
        private const string DevName = "Solenoid";

        #region Field
        private bool m_IsVirtualInput = false;
        private bool m_IsVirtualOutput = false;
        private bool m_VirtualStatus = false;
        private IoTag m_DoSolenoid = new IoTag();
        private IoTag m_DiSensor = new IoTag();

        private uint m_SolenoidActionTime = 30;
        private float m_SensorConfirmTime = 0.5F;
        private AlarmData m_ALM_ActOnTimeout = null;
        private AlarmData m_ALM_ActOffTimeout = null;
        private AlarmData m_LastAlarm = null;

        private SeqAction m_SeqAction = null;
        private SolenoidAction m_Cmd = SolenoidAction.None;
        private SolenoidAction m_State = SolenoidAction.None;
        #endregion

        #region I/O, Device Relation
        [Category("!Setting Device (Output)"), Description(""), DeviceSetting(true)]
        public IoTag DoSolenoid { get { return m_DoSolenoid; } set { m_DoSolenoid = value; } }
        [Category("!Setting Device (Input)"), Description(""), DeviceSetting(true)]
        public IoTag DiSensor { get { return m_DiSensor; } set { m_DiSensor = value; } }
        #endregion

        #region Property
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ActOnTimeout { get { return m_ALM_ActOnTimeout; } }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ActOffTimeout { get { return m_ALM_ActOffTimeout; } }

        [Category("!Setting Device"), Description("Solenoid Actuator Action Timeout (sec)"), DeviceSetting(false, true)]
        public uint SolenoidActionTime
        {
            get { return m_SolenoidActionTime; }
            set { m_SolenoidActionTime = value; }
        }
        [Category("!Setting Device"), Description("Sensor Detection Confirm Delay Time (sec)"), DeviceSetting(false, true)]
        public float SensorConfirmTime
        {
            get { return m_SensorConfirmTime; }
            set { m_SensorConfirmTime = value; }
        }
        [Category("!Setting Device (Virtual I/O)"), DisplayName("Virtual Input"), DeviceSetting(true, false)]
        public bool IsVirtualInput { get { return m_IsVirtualInput; } set { m_IsVirtualInput = value; } }
        [Category("!Setting Device (Virtual I/O)"), DisplayName("Virtual Output"), DeviceSetting(true, false)]
        public bool IsVirtualOutput { get { return m_IsVirtualOutput; } set { m_IsVirtualOutput = value; } }
        #endregion

        #region Constructor
        public _DevSolenoidValve()
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
            if (m_IsVirtualInput == false)
                ok &= DoSolenoid.GetChannel() != null ? true : false;
            if (m_IsVirtualInput == false)
                ok &= DiSensor.GetChannel() != null ? true : false;

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
            m_ALM_ActOnTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, heavy_alarm, ParentName, MyName, "ON Timeout");
            m_ALM_ActOffTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, heavy_alarm, ParentName, MyName, "OFF Timeout");
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;
            if (SolenoidActionTime == 0) SolenoidActionTime = 30; // default 30sec
            if (SensorConfirmTime == 0) SensorConfirmTime = 30; // default 30sec           
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

        public override void SeqAbort()
        {
            if (!Initialized) return;

            m_State = SolenoidAction.None;
            m_Cmd = SolenoidAction.None;
            //m_SeqAction.InitSeq();
        }
        #endregion

        #region Method
        public void StateInit()
        {
            m_State = SolenoidAction.None;
        }
        public bool SetOn()
        {
            if (!Initialized) return false;
            if (m_State == SolenoidAction.Busy) return false;
            if (m_State == SolenoidAction.On) return false;

            m_State = SolenoidAction.None;
            m_Cmd = SolenoidAction.On;
            return true;
        }
        public bool SetOff()
        {
            if (!Initialized) return false;
            if (m_State == SolenoidAction.Busy) return false;
            if (m_State == SolenoidAction.Off) return false;

            m_State = SolenoidAction.None;
            m_Cmd = SolenoidAction.Off;
            return true;
        }
        public bool IsOn()
        {
            if (!Initialized) return false;
            return _GetOn();
        }
        public bool IsOff()
        {
            if (!Initialized) return false;
            return _GetOff();
        }
        public SolenoidAction GetState()
        {
            if (m_State == SolenoidAction.Alarm)
            {
                if (m_IsVirtualOutput)
                {
                    if (IsOn()) m_State = SolenoidAction.On;
                    else if (IsOff()) m_State = SolenoidAction.Off;
                }
                else
                {
                    if (IsOn() && m_DoSolenoid.GetChannel().GetDi()) m_State = SolenoidAction.On;
                    else if (IsOff() && !m_DoSolenoid.GetChannel().GetDi()) m_State = SolenoidAction.Off;
                }
            }
            return m_State;
        }
        public AlarmData GetLastAlarm()
        {
            return m_LastAlarm;
        }
        private void _SetOn()
        {
            if (!Initialized) return;

            if (!m_IsVirtualOutput)
            {
                DoSolenoid.GetChannel().SetDo(true);
                if (AppConfig.Instance.Simulation.IO && !m_IsVirtualInput) DiSensor.GetChannel().SetDo(true);
            }
            if (m_IsVirtualInput) m_VirtualStatus = true;
        }
        private void _SetOff()
        {
            if (!Initialized) return;

            if (!m_IsVirtualOutput)
            {
                DoSolenoid.GetChannel().SetDo(false);
                if (AppConfig.Instance.Simulation.IO && !m_IsVirtualInput) DiSensor.GetChannel().SetDo(false);
            }
            if (m_IsVirtualInput) m_VirtualStatus = false;
        }
        private bool _GetOn()
        {
            if (!Initialized) return false;
            bool rv = false;
            if (m_IsVirtualInput)
                rv = m_VirtualStatus ? true : false;
            else
                rv = DiSensor.GetChannel().GetDi() ? true : false;

            return rv;
        }
        private bool _GetOff()
        {
            if (!Initialized) return false;
            bool rv = false;
            if (m_IsVirtualInput)
                rv = m_VirtualStatus ? false : true;
            else
                rv = DiSensor.GetChannel().GetDi() ? false : true;

            return rv;
        }
        #endregion

        #region Sequence
        private class SeqAction : XSeqFunc
        {
            #region Field
            private _DevSolenoidValve m_Device = null;
            private uint ConfirmTime = 0;
            private bool SensorOnCheck = false;
            private bool SensorOffCheck = false;
            #endregion

            #region Property
            #endregion

            #region Constructor
            public SeqAction(_DevSolenoidValve device)
            {
                this.SeqName = $"SeqAction{device.MyName}";
                m_Device = device;
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

                int seqNo = SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_Device.m_Cmd == SolenoidAction.On)
                            {
                                m_Device._SetOn();
                                SensorOnCheck = false;
                                m_Device.m_State = SolenoidAction.Busy;
                                m_Device.m_Cmd = SolenoidAction.None;
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else if (m_Device.m_Cmd == SolenoidAction.Off)
                            {
                                m_Device._SetOff();
                                SensorOffCheck = false;
                                m_Device.m_State = SolenoidAction.Busy;
                                m_Device.m_Cmd = SolenoidAction.None;
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 100;
                            }
                        }
                        break;

                    case 10:
                        {
                            // sensor ON
                            if (m_Device._GetOn() && !SensorOnCheck)
                            {
                                SensorOnCheck = true;
                                ConfirmTime = XFunc.GetTickCount();
                            }
                            else if (m_Device._GetOff())
                            {
                                SensorOnCheck = false;
                            }

                            if (SensorOnCheck && XFunc.GetTickCount() - ConfirmTime > m_Device.SensorConfirmTime * 1000)
                            {
                                m_Device.m_State = SolenoidAction.On;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Device.SolenoidActionTime * 1000)
                            {
                                m_Device.m_LastAlarm = m_Device.m_ALM_ActOnTimeout;
                                m_Device.m_State = SolenoidAction.Alarm;
                                seqNo = 0;
                            }
                            else if (m_Device.m_Cmd == SolenoidAction.On || m_Device.m_Cmd == SolenoidAction.Off)
                            {
                                seqNo = 0;
                            }
                        }
                        break;

                    case 100:
                        {
                            // sensor OFF
                            if (m_Device._GetOff() && !SensorOffCheck)
                            {
                                SensorOffCheck = true;
                                ConfirmTime = XFunc.GetTickCount();
                            }
                            else if (m_Device._GetOn())
                            {
                                SensorOffCheck = false;
                            }

                            if (SensorOffCheck && XFunc.GetTickCount() - ConfirmTime > m_Device.SensorConfirmTime * 1000)
                            {
                                m_Device.m_State = SolenoidAction.Off;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Device.SolenoidActionTime * 1000)
                            {
                                m_Device.m_LastAlarm = m_Device.m_ALM_ActOffTimeout;
                                m_Device.m_State = SolenoidAction.Alarm;
                                seqNo = 0;
                            }
                            else if (m_Device.m_Cmd == SolenoidAction.On || m_Device.m_Cmd == SolenoidAction.Off)
                            {
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

                var helperXml = new XmlHelper<_DevSolenoidValve>();
                _DevSolenoidValve dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.IsVirtualInput = dev.IsVirtualInput;
                    this.IsVirtualOutput = dev.IsVirtualOutput;
                    this.DiSensor = dev.DiSensor;
                    this.DoSolenoid = dev.DoSolenoid;
                    this.SensorConfirmTime = dev.SensorConfirmTime;
                    this.SolenoidActionTime = dev.SolenoidActionTime;
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
                var helperXml = new XmlHelper<_DevSolenoidValve>();
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
