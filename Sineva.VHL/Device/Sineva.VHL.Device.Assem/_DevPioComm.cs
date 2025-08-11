using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Net;
using System.Windows.Forms;
using System.Drawing.Design;
using System.IO;
using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data;
using System.Security.Permissions;
using System.Runtime.Remoting.Channels;

namespace Sineva.VHL.Device.Assem
{
    /// <summary>
    /// //////////////////////////////////// 
    /// Serial Port : Gripper=COM2, EQP=COM3(ver1), COM4(ver2)
    /// BaudRate : 38400
    /// DataBits : 8
    /// ParityBit : None
    /// StopBit : One
    /// ////////////////////////////////////
    /// PIO_ID : Gripper=4 , EQP=101
    /// PIO_CH : Gripper=128 , EQP=255
    /// </summary>
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class _DevPioComm : _Device
    {
        private const string DevName = "PIOComm";

        #region Fields
        /////////////////////////////////////////
        private bool m_AutoConnection = true; // 자동 연결할건지 ? 아님 필요한 시점에 연결할건지 ?
        private XComm m_Comm = new XComm();
        private float m_CommOpenWaitTime = 10;
        private float m_TimerSelectDelayTime = 0.1f; // PIO Signal Confirm Timeout
        private AlarmData m_ALM_CommOpenTimeoutError = null;
        private AlarmData m_ALM_MessageResponseError = null;
        private AlarmData m_ALM_MessageFormatError = null;
        private AlarmData m_ALM_PioGoResponseError = null;
        /////////////////////////////////////////
        /////////////////////////////////////////
        private bool m_IsConnected = false;
        private string m_ReceivedMessage = string.Empty;
        private int m_PIO_ID = 0;
        private int m_PIO_CH = 0;
        /////////////////////////////////////////
        private SeqConnection m_SeqConnection = null;
        private SeqChannelSelect m_SeqChannelSelect = null;
        #endregion
        #region Fields - I/O
        private _DevOutput m_DoSelect = new _DevOutput();
        private _DevOutput m_DoMode = new _DevOutput();
        private _DevInput m_DiGo = new _DevInput();
        #endregion
        #region Property - I/O, Device Relation       

        [Category("I/O Setting (Digital Output)"), Description("PIO Select"), DeviceSetting(true)]
        public _DevOutput DoSelect { get { return m_DoSelect; } set { m_DoSelect = value; } }
        [Category("I/O Setting (Digital Output)"), Description("PIO Mode"), DeviceSetting(true)]
        public _DevOutput DoMode { get { return m_DoMode; } set { m_DoMode = value; } }
        [Category("I/O Setting (Digital Input)"), Description("PIO GO"), DeviceSetting(true)]
        public _DevInput DiGo { get { return m_DiGo; } set { m_DiGo = value; } }
        #endregion

        #region Properties
        [Category("!Setting Device")]
        public XComm Comm
        {
            get { return m_Comm; }
            set { m_Comm = value; }
        }
        [Category("!Setting Device"), Description("COM Port Open Timeout (sec)")]
        public float CommOpenWaitTime
        {
            get { return m_CommOpenWaitTime; }
            set { m_CommOpenWaitTime = value; }
        }
        [Category("!Setting Device"), Description("Select Signal On/Off Delay (sec)")]
        public float TimerSelectDelayTime
        {
            get { return m_TimerSelectDelayTime; }
            set { m_TimerSelectDelayTime = value; }
        }
        [Category("!Setting Device"), Description("PIO ID")]
        public int PIO_ID
        {
            get { return m_PIO_ID;}
            set { m_PIO_ID = value;}
        }
        [Category("!Setting Device"), Description("PIO CH")]
        public int PIO_CH
        {
            get { return m_PIO_CH; }
            set { m_PIO_CH = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public bool IsConnected
        {
            get 
            {
                bool rv = m_IsConnected;
                if (AppConfig.Instance.Simulation.SERIAL) rv = true;
                return rv; 
            } 
            set { m_IsConnected = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public string ReceivedMessage
        {
            get { return m_ReceivedMessage; }
            set { m_ReceivedMessage = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_CommOpenTimeoutError
        {
            get { return m_ALM_CommOpenTimeoutError; }
            set { m_ALM_CommOpenTimeoutError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MessageResponseError
        {
            get { return m_ALM_MessageResponseError; }
            set { m_ALM_MessageResponseError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MessageFormatError
        {
            get { return m_ALM_MessageFormatError; }
            set { m_ALM_MessageFormatError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_PioGoResponseError
        {
            get { return m_ALM_PioGoResponseError; }
            set { m_ALM_PioGoResponseError = value; }
        }
        
        #endregion

        #region Event
        public event DelVoid_String UpdateData;
        #endregion

        #region Constructor
        public _DevPioComm()
        {
            this.MyName = "_DevPIO";
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
            if (DoSelect.IsValid) ok &= DoSelect.Initialize(this.ToString(), false);
            if (DoMode.IsValid) ok &= DoMode.Initialize(this.ToString(), false);
            if (DiGo.IsValid) ok &= DiGo.Initialize(this.ToString(), false);
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
            ALM_CommOpenTimeoutError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, heavy_alarm, ParentName, MyName, "Connect Timeout");
            ALM_MessageResponseError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, heavy_alarm, ParentName, MyName, "Response Timeout");
            ALM_MessageFormatError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, heavy_alarm, ParentName, MyName, "Message Format Timeout");
            ALM_PioGoResponseError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, heavy_alarm, ParentName, MyName, "PIO GO On Timeout");
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            m_Comm.Initialize();
            m_Comm.ReceivedData += new XComm.ReceivedDataEventHandler(ReceivedData);
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            m_SeqConnection = new SeqConnection(this);
            m_SeqChannelSelect = new SeqChannelSelect(this);
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

            m_IsConnected = false;
            m_SeqChannelSelect.SeqAbort();
            //m_SeqConnection.SeqAbort();
        }
        #endregion

        #region Methods - public
        public bool IsOpen()
        {
            if (AppConfig.Instance.Simulation.SERIAL) return true;
            return m_Comm.IsOpen();
        }
        public void OpenPort()
        {
            if (AppConfig.Instance.Simulation.SERIAL) return;
            m_Comm.Open();
        }
        public void ClosePort()
        {
            if (AppConfig.Instance.Simulation.SERIAL) return;
            m_Comm.Close();
        }
        public int SelectChannel(int id = -1, int channel = -1, bool spl = false)
        {
            if (AppConfig.Instance.Simulation.SERIAL)
            {
                if (DiGo.IsValid) DiGo.SetDi(true);
                return 0;
            }

            if (id == -1) id = PIO_ID;
            if (channel == -1) channel = PIO_CH;
            int rv = m_SeqChannelSelect.Do(id, channel, spl);
            return rv;
        }
        public void SetAutoConnection(bool connect)
        {
            m_AutoConnection = connect;
        }
        public bool IsGo()
        {
            bool rv = DiGo.IsValid ? DiGo.IsDetected : false;
            return rv;
        }
        #endregion

        #region Methods
        private void ReceivedData(object sender)
        {
            string readData = string.Empty;
            string readTemp = string.Empty;
            do
            {
                System.Threading.Thread.Sleep(10);
                readTemp = m_Comm.ReadExisting();
                readData += readTemp;
            } while (!string.IsNullOrEmpty(readTemp));
            m_ReceivedMessage = readData;
            SerialCommLog.WriteLog(string.Format("RECV : {0}", m_ReceivedMessage));

            this.UpdateData?.Invoke(readData);
        }
        private bool SendChannelID(int id, int channel)
        {
            try
            {
                if (!IsOpen()) return false;
                m_ReceivedMessage = string.Empty;
                //Old style  BC=2:%06d:%03d:0:OHT1234%01d
                string cmd = string.Format("BC=2:{0:D6}:{1:D3}:0:OHT{2:D3}", id, channel, AppConfig.Instance.VehicleNumber);
                string msg = string.Format("<{0}{1}>", cmd, CheckSum(cmd));
                m_Comm.Write(msg);
                SerialCommLog.WriteLog(string.Format("SEND : {0}", msg));
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return false;
            }
            return true;
        }
        private string CheckSum(string inputData)
        {
            string CSData = string.Empty;
            ushort USValue = 0;
            for (int i = 0; i < inputData.Length; i++)
            {
                USValue += (ushort)inputData[i];
                USValue &= 0xff;
            }
            CSData = USValue.ToString("X");
            return CSData;
        }
        #endregion

        #region Sequence
        private class SeqConnection : XSeqFunc
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();

            #region Field
            private _DevPioComm m_Device = null;
            private bool m_ChannelDisconnet = false;
            private int m_SeqConnectionSeqNo = 0;

            private int m_RetryCount = 0;
            #endregion

            #region Constructor
            public SeqConnection(_DevPioComm device)
            {
                this.SeqName = $"SeqConnection{device.MyName}";
                m_Device = device;
                TaskSerialControl.Instance.RegSeq(this);
            }
            public override void SeqAbort()
            {
                m_SeqConnectionSeqNo = 0;
                this.InitSeq();
                if (AlarmId != 0)
                {
                    EqpAlarm.Reset(AlarmId);
                    SerialCommLog.WriteLog(string.Format("{0} Reset Alarm : Code[{1}]", method, AlarmId));
                }
                AlarmId = 0;
                if (m_Device.m_AutoConnection == false)
                    m_ChannelDisconnet = true;// Channel Disconnect
            }
            #endregion

            #region Override
            public override int Do()
            {
                int rv = -1;
                int seqNo = m_SeqConnectionSeqNo;

                if (!m_Device.Initialized) return rv;

                if (AppConfig.Instance.Simulation.SERIAL && m_Device.m_AutoConnection)
                {
                    if (m_Device.IsGo() == false) m_Device.DiGo.SetDi(true);
                    return rv;
                }

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_ChannelDisconnet)
                            {
                                SerialCommLog.WriteLog(string.Format("PIO Disconnection"));
                                m_Device.m_SeqChannelSelect.SeqAbort();
                                if (m_Device.IsConnected)
                                    seqNo = 10;
                                else m_ChannelDisconnet = false;
                            }
                            else if (m_Device.m_AutoConnection)
                            {
                                bool disconnected = false;
                                disconnected |= m_Device.IsOpen() == false;
                                disconnected |= m_Device.IsGo() == false;
                                if (disconnected)
                                {
                                    m_RetryCount = 0;
                                    SerialCommLog.WriteLog(string.Format("PIO Auto Connection"));
                                    m_Device.m_SeqChannelSelect.SeqAbort();
                                    seqNo = 20;
                                }
                                else
                                {
                                    m_Device.IsConnected = true;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            int rv1 = m_Device.SelectChannel(0, 0);
                            if (rv1 == 0)
                            {
                                m_ChannelDisconnet = false;
                                SerialCommLog.WriteLog(string.Format("PIO Disconnection OK"));
                                seqNo = 0;
                            }
                            else if (rv1 > 0)
                            {
                                SerialCommLog.WriteLog(string.Format("PIO Disconnection Alarm"));
                                AlarmId = rv1;
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 0;
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 20:
                        {
                            int rv1 = m_Device.SelectChannel();
                            if (rv1 == 0)
                            {
                                SerialCommLog.WriteLog(string.Format("PIO Auto Connection OK"));
                                seqNo = 0;
                            }
                            else if (rv1 > 0)
                            {
                                if (m_RetryCount < 5)
                                {
                                    SerialCommLog.WriteLog(string.Format("PIO Auto Connection Alarm - Retry({0})", m_RetryCount));
                                    m_RetryCount++;
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 30;
                                }
                                else
                                {
                                    SerialCommLog.WriteLog(string.Format("PIO Auto Connection Alarm"));
                                    AlarmId = rv1;
                                    EqpAlarm.Set(AlarmId);
                                    ReturnSeqNo = 0;
                                    seqNo = 1000;
                                }
                            }
                        }
                        break;

                    case 30:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                seqNo = 20;
                            }
                        }
                        break;

                    case 1000:
                        {
                            if (IsPushedSwitch.IsAlarmReset)
                            {
                                IsPushedSwitch.m_AlarmRstPushed = false;

                                SerialCommLog.WriteLog(string.Format("{0} Reset Alarm : Code[{1}]", method, AlarmId));
                                EqpAlarm.Reset(AlarmId);
                                AlarmId = 0;
                                seqNo = ReturnSeqNo;
                            }
                        }
                        break;
                }

                m_SeqConnectionSeqNo = seqNo;
                return -1;
            }
            #endregion
        }

        private class SeqChannelSelect : XSeqFunc
        {
            #region Field
            private _DevPioComm m_Device = null;
            private int m_ResRetryCount = 0;
            private int m_MsgRetryCount = 0;
            private int m_PioRetryCount = 0;
            private int m_MaxRetryCount = 5;
            #endregion

            #region Constructor
            public SeqChannelSelect(_DevPioComm device)
            {
                this.SeqName = $"SeqChannelSelect{device.MyName}";
                m_Device = device;
            }
            public override void SeqAbort()
            {
                m_MsgRetryCount = 0;
                m_ResRetryCount = 0;
                m_PioRetryCount = 0;
                this.InitSeq();
            }
            #endregion

            #region Override
            public override int Do(object para1, object para2, object para3)
            {
                int id = (int)para1;
                int channel = (int)para2;
                bool spl = (bool)para3;

                int rv = -1;
                int seqNo = SeqNo;

                if (!m_Device.Initialized) return rv;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (id == 0 || channel == 0) // Disconnect
                            {
                                if (m_Device.IsOpen() || m_Device.IsGo())
                                {
                                    m_ResRetryCount = 0;
                                    m_MsgRetryCount = 0;
                                    m_PioRetryCount = 0;
                                    if (spl) m_MaxRetryCount = 60;
                                    else m_MaxRetryCount = 10;

                                    SerialCommLog.WriteLog(string.Format("{0} PIO GO", m_Device.MyName));
                                    seqNo = 100;
                                }
                                else
                                {
                                    rv = 0;
                                    seqNo = 0;
                                }
                            }
                            else if (m_Device.IsOpen() == false)
                            {
                                SerialCommLog.WriteLog(string.Format("{0} Serial Connection", m_Device.MyName));
                                seqNo = 10;
                            }
                            else if (m_Device.IsGo() == false)
                            {
                                m_ResRetryCount = 0;
                                m_MsgRetryCount = 0;
                                m_PioRetryCount = 0;
                                if (spl) m_MaxRetryCount = 60;
                                else m_MaxRetryCount = 10;

                                SerialCommLog.WriteLog(string.Format("{0} PIO GO", m_Device.MyName));
                                seqNo = 100;
                            }
                            else
                            {
                                SerialCommLog.WriteLog(string.Format("{0} PIO GO OK", m_Device.MyName));
                                m_Device.IsConnected = true;
                                rv = 0;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            m_Device.OpenPort();
                            SerialCommLog.WriteLog(string.Format("{0} Serial[{1}] Open", m_Device.MyName, m_Device.Comm._CommPortNo));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        break;

                    case 20:
                        {
                            if (m_Device.IsOpen())
                            {
                                SerialCommLog.WriteLog(string.Format("{0} Serial[{1}] Open OK", m_Device.MyName, m_Device.Comm._CommPortNo));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Device.CommOpenWaitTime * 1000)
                            {
                                m_Device.ClosePort();
                                rv = m_Device.ALM_CommOpenTimeoutError.ID;
                                SerialCommLog.WriteLog(string.Format("{0} Serial[{1}] Open Timeout Alarm", m_Device.MyName, m_Device.Comm._CommPortNo));
                                seqNo = 0; //1초 후 다시 해봐라
                            }
                        }
                        break;

                    case 100:
                        {
                            m_Device.DoSelect.SetDo(true);
                            SerialCommLog.WriteLog(string.Format("{0} SELECT ON", m_Device.MyName));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;

                        }
                        break;

                    case 110:
                        {
                            if (XFunc.GetTickCount() - StartTicks < m_Device.TimerSelectDelayTime * 1000.0f) break; //wait 100msec, test 후 시간 변경 필요
                            SerialCommLog.WriteLog(string.Format("{0} Set Channel [ID={1}, Channel={2}]", m_Device.MyName, id, channel));
                            m_Device.SendChannelID(id, channel);
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 120;

                        }
                        break;

                    case 120:
                        {
                            if (m_Device.ReceivedMessage != string.Empty)
                            {
                                string sRcv = m_Device.ReceivedMessage.Substring(1, m_Device.ReceivedMessage.Length - 2);
                                if (sRcv.Contains(string.Format("OHT{0:D3}", AppConfig.Instance.VehicleNumber))) // Vehicle Number를 가지고 있나 ?
                                {
                                    SerialCommLog.WriteLog(string.Format("{0} Set Channel OK [ID={1}, Channel={2}]", m_Device.MyName, id, channel));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 130;
                                }
                                else
                                {
                                    m_Device.DoSelect.SetDo(false);
                                    SerialCommLog.WriteLog(string.Format("{0} Set Channel Message Format NG [ID={1}, Channel={2}], RECV : [{3}]", m_Device.MyName, id, channel, m_Device.ReceivedMessage));
                                    AlarmId = m_Device.ALM_MessageFormatError.ID;
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 150;
                                }
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 200)
                            {
                                m_Device.DoSelect.SetDo(false);
                                SerialCommLog.WriteLog(string.Format("{0} SELECT OFF", m_Device.MyName));
                                SerialCommLog.WriteLog(string.Format("{0} Set Channel Not Response NG [ID={1}, Channel={2}]", m_Device.MyName, id, channel));
                                AlarmId = m_Device.ALM_MessageResponseError.ID;
                                seqNo = 160;
                            }
                        }
                        break;

                    case 130:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 100) break;

                            m_MsgRetryCount = 0;
                            m_ResRetryCount = 0;
                            m_Device.DoSelect.SetDo(false);
                            SerialCommLog.WriteLog(string.Format("{0} SELECT OFF", m_Device.MyName));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 140;
                        }
                        break;

                    case 140:
                        {
                            bool isGo = (id == 0 || channel == 0) ? !m_Device.IsGo() : m_Device.IsGo(); // 연결할때는 GO ON, 연결해제 GO OFF
                            if (isGo)
                            {
                                SerialCommLog.WriteLog(string.Format("{0} PIO GO ON", m_Device.MyName));
                                if (m_Device.IsGo()) m_Device.IsConnected = true;
                                else m_Device.IsConnected = false;
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                if (m_PioRetryCount < m_MaxRetryCount)
                                {
                                    // retry send command
                                    SerialCommLog.WriteLog(string.Format("{0} PIO GO ON Timeout Retry [{1}/{2}]", m_Device.MyName, m_PioRetryCount, m_MaxRetryCount));
                                    m_PioRetryCount++;
                                    seqNo = 100;
                                }
                                else
                                {
                                    SerialCommLog.WriteLog(string.Format("{0} PIO GO ON Timeout Error [ID={1}, Channel={2}][Recv={3}]", m_Device.MyName, id, channel, m_Device.ReceivedMessage));
                                    m_PioRetryCount = 0;
                                    rv = m_Device.ALM_PioGoResponseError.ID;
                                    seqNo = 0;
                                }

                            }
                        }
                        break;

                    case 150:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 200) break;

                            // Message Retry
                            if (m_MsgRetryCount < m_MaxRetryCount)
                            {
                                // retry send command
                                SerialCommLog.WriteLog(string.Format("{0} Set Channel Retry [{1}/{2}]", m_Device.MyName, m_MsgRetryCount, m_MaxRetryCount));
                                m_MsgRetryCount++;
                                seqNo = 100;
                            }
                            else
                            {
                                m_MsgRetryCount = 0;
                                m_ResRetryCount = 0;
                                SerialCommLog.WriteLog(string.Format("{0} SELECT OFF", m_Device.MyName));
                                rv = AlarmId;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 160:
                        {
                            // Not Response Retry
                            if (m_ResRetryCount < m_MaxRetryCount)
                            {
                                SerialCommLog.WriteLog(string.Format("{0} Set Channel Retry [{1}/{2}]", m_Device.MyName, m_ResRetryCount, m_MaxRetryCount));
                                m_ResRetryCount++;
                                seqNo = 100;
                            }
                            else
                            {
                                m_MsgRetryCount = 0;
                                m_ResRetryCount = 0;
                                SerialCommLog.WriteLog(string.Format("{0} SELECT OFF", m_Device.MyName));
                                rv = m_Device.ALM_MessageResponseError.ID;
                                seqNo = 0;
                            }
                        }
                        break;
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

                var helperXml = new XmlHelper<_DevPioComm>();
                _DevPioComm dev = helperXml.Read(fileName);
                if (dev == null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.DoSelect = dev.DoSelect;
                    this.DoMode = dev.DoMode;
                    this.Comm = dev.Comm;
                    this.CommOpenWaitTime = dev.CommOpenWaitTime;
                    this.PIO_ID = dev.PIO_ID;
                    this.PIO_CH = dev.PIO_CH;
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
                var helperXml = new XmlHelper<_DevPioComm>();
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
