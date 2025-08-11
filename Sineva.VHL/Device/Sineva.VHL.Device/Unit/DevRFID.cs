using Sineva.VHL.Data;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Library;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Sineva.VHL.Device
{
    [Serializable]
    public enum enRFIDCommType
    {
        TCP,
        SerialPort,
    }
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class DevRFID : _Device
    {
        private const string DevName = "DevRFID";

        #region Fields
        private static byte[] readPacket = new byte[] { 0x55, 0xAA, 0x02, 0x01, 0x03 };
        private enRFIDCommType m_Type = enRFIDCommType.TCP;
        private AsyncSocketClient m_Client = null;
        private bool m_TcpClientConnected = false;
        private string m_IPStr = "192.168.1.199";
        private int m_IPPort = 12001;
        private int m_ReadInterval = 100;
        private int m_RFIDStringLength = 8;
        private int m_ReadRetryMaxCount = 3;
        private XComm m_Comm = null;
        private string m_RFIDTag = string.Empty;
        private bool m_RFIDReadStart = false;
        private bool m_RFIDReading = false;
        private bool m_RFIDReadCompleted = false;
        private bool m_RFIDReadingNg = false;
        private bool m_RFIDConnectedNg = false;
        private bool m_DataReceivedOk = false;
        private bool m_DataReceivedFormatNg = false;
        private SeqMonitor m_SeqMonitor;
        private SeqAction m_SeqAction;

        private AlarmData m_ALM_NotDefine;
        private AlarmData m_ALM_ConnectFailed;
        private AlarmData m_ALM_ReadTagTimeout;
        private int m_ReadCount = 0;
        #endregion

        #region Properties
        [Category("Configuration"), Description("Type"), DeviceSetting(true)]
        public enRFIDCommType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }
        [Category("Configuration"), Description("Com"), DeviceSetting(true)]
        public XComm Comm
        {
            get { return m_Comm; }
            set { m_Comm = value; }
        }
        [Category("Configuration"), Description("IPAdress"), DeviceSetting(true)]
        public string IPStr
        {
            get { return m_IPStr; }
            set { m_IPStr = value; }
        }

        [Category("Configuration"), Description("IPPort"), DeviceSetting(true)]
        public int IPPort
        {
            get { return m_IPPort; }
            set { m_IPPort = value; }
        }

        [Category("Configuration"), Description("ReadRetryMaxCount"), DeviceSetting(true, true)]
        public int ReadRetryMaxCount
        {
            get { return m_ReadRetryMaxCount; }
            set { m_ReadRetryMaxCount = value; }
        }

        [Category("Configuration"), Description("ReadInterval(ms)"), DeviceSetting(true)]
        public int ReadInterval
        {
            get { return m_ReadInterval; }
            set { m_ReadInterval = value; }
        }
        [Category("Configuration"), Description("Read String Length"), DeviceSetting(true)]
        public int RFIDStringLength
        {
            get { return m_RFIDStringLength; }
            set { m_RFIDStringLength = value; }
        }
        [Category("!LifeTime Manager"), DisplayName("RFID Read Count"), Description("Device Life Read Count"), DeviceSetting(false, true)]
        public int ReadCount
        {
            get { return m_ReadCount; }
            set { SaveCurState = m_ReadCount != value; m_ReadCount = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public string RFIDTag
        {
            get { return m_RFIDTag; }
            set { m_RFIDTag = value; }
        }        
        [XmlIgnore(), Browsable(false)]
        public bool RFIDReadStart
        {
            get { return m_RFIDReadStart; }
            set { m_RFIDReadStart = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public bool RFIDReading
        {
            get { return m_RFIDReading; }
            set { m_RFIDReading = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public bool RFIDReadCompleted
        {
            get { return m_RFIDReadCompleted; }
            set { m_RFIDReadCompleted = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public bool RFIDReadingNg
        {
            get { return m_RFIDReadingNg; }
            set { m_RFIDReadingNg = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public bool RFIDConnectedNg
        {
            get { return m_RFIDConnectedNg; }
            set { m_RFIDConnectedNg = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_NotDefine
        {
            get { return m_ALM_NotDefine; }
            set { m_ALM_NotDefine = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ConnectFailed
        {
            get { return m_ALM_ConnectFailed; }
            set { m_ALM_ConnectFailed = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ReadTagTimeout
        {
            get { return m_ALM_ReadTagTimeout; }
            set { m_ALM_ReadTagTimeout = value; }
        }

        #endregion

        #region Constructor
        public DevRFID()
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
            ALM_ConnectFailed = AlarmListProvider.Instance.NewAlarm(AlarmCode.DataIntegrity, true, MyName, ParentName, "Connect Failed");
            ALM_ReadTagTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.DataIntegrity, true, MyName, ParentName, "Tag Read Timeout");
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;
            //ok &= Connect();
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Read Times", this, "GetReadCount", 1000));
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            if (ok)
            {
                if (m_Type == enRFIDCommType.TCP)
                {
                    m_Client = new AsyncSocketClient(0);
                    m_Client.OnConnet += new AsyncSocketConnectEventHandler(OnConnet);
                    m_Client.OnClose += new AsyncSocketCloseEventHandler(OnClose);
                    m_Client.OnReceive += new AsyncSocketReceiveEventHandler(OnReceive);
                    m_Client.OnError += new AsyncSocketErrorEventHandler(OnError);
                }
                else
                {
                    m_Comm = new XComm();
                    m_Comm.ReceivedData += new XComm.ReceivedDataEventHandler(ReceivedData);
                }

                m_SeqMonitor = new SeqMonitor(this);
                m_SeqAction = new SeqAction(this);
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
            if (!Initialized) return;
            ResetFlag();
            m_SeqMonitor.SeqAbort();
            m_SeqAction.SeqAbort();
        }

        public bool ReadStart()
        {
            bool rv = false;
            if (!Initialized)
            {
                ResetFlag();
            }
            else
            {
                SequenceDeviceLog.WriteLog("RFID", "m_RFIDReadStart = true");
                ResetFlag();
                m_SeqAction.SeqAbort();

                m_RFIDReadStart = true;
                rv = true;
            }
            return rv;
        }
        public void ReadStop()
        {
            if (!Initialized) return;

            SequenceDeviceLog.WriteLog("RFID", "m_RFIDReadStart = false");
            ResetFlag();
            m_SeqAction.SeqAbort();
        }
        private void ResetFlag()
        {
            m_RFIDReadStart = false;
            m_RFIDReading = false;
            m_RFIDReadCompleted = false;
            m_RFIDReadingNg = false;
            m_RFIDConnectedNg = false;
        }
        private bool IsConnected()
        {
            if (!Initialized) return false;
            bool rv = false;
            if (m_Type == enRFIDCommType.TCP) rv = m_TcpClientConnected;
            else rv = m_Comm.IsOpen();
            return rv;
        }
        private void Connect()
        {
            if (!Initialized) return;

            try
            {
                if (m_Type == enRFIDCommType.TCP)
                {
                    // Event를 추가만 하면 통신이 끊어질 경우 재연결이 않될 수 있다...                  
                    m_Client.Connect(IPStr, IPPort);
                }
                else
                {
                    m_Comm.Open();
                    m_Comm.FlushInBuffer();
                    m_Comm.FlushOutBuffer();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void Disconnect()
        {
            if (!Initialized) return;

            try
            {
                if (m_Type == enRFIDCommType.TCP) m_Client?.Close();
                else m_Comm?.Close();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void ReceivedData(object sender)
        {
            if (!Initialized) return;

            byte[] readResult = new byte[m_Comm.SerialPort.BytesToRead];
            int r = m_Comm.Read(readResult, 0, readResult.Length);
            if (r == 22 && readResult[0] == 0x55 && readResult[1] == 0xAA && readResult[5] == 0x00)
            {
                //m_RFIDTag = Encoding.ASCII.GetString(readResult, 6, 16).Trim();
                m_RFIDTag = Encoding.ASCII.GetString(readResult, 6, 8).Trim();
                m_DataReceivedOk = true;
            }
            else
            {
                m_RFIDTag = String.Empty;
                m_DataReceivedFormatNg = true;
            }
        }
        private void OnConnet(object sender, AsyncSocketConnectionEventArgs e)
        {
            SequenceDeviceLog.WriteLog("RFID", "Socket Connected");
            m_TcpClientConnected = true;
        }
        private void OnClose(object sender, AsyncSocketConnectionEventArgs e)
        {
            SequenceDeviceLog.WriteLog("RFID", "Socket Closed");
            m_TcpClientConnected = false;
        }
        private void OnError(object sender, AsyncSocketErrorEventArgs e)
        {
            SequenceDeviceLog.WriteLog("RFID", "Socket Error");
            m_TcpClientConnected = false;
        }
        private void OnReceive(object sender, AsyncSocketReceiveEventArgs e)
        {
            if (!Initialized) return;

            try
            {
                int readSize = e.ReceiveBytes;
                if (readSize >= 22)
                {
                    byte[] readResult = new byte[readSize];
                    Array.Copy(e.ReceiveBuffer, readResult, readSize);
                    if (readResult[0] == 0x55 && readResult[1] == 0xAA && readResult[5] == 0x00)
                    {
                        //m_RFIDTag = Encoding.ASCII.GetString(readResult, 6, 16).Trim();
                        m_RFIDTag = Encoding.ASCII.GetString(readResult, 6, 8).Trim();
                    }
                    else
                    {
                        m_RFIDTag = String.Empty;
                    }
                    EventHandlerManager.Instance.InvokeCarrierIDReadingConfirm(m_RFIDTag);
                }
                else
                {
                    m_DataReceivedFormatNg = true;
                }
                m_DataReceivedOk = true;
                SequenceDeviceLog.WriteLog("RFID", string.Format("Recv : {0}", Encoding.ASCII.GetString(e.ReceiveBuffer, 0, readSize).Trim()));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private bool SendCommand()
        {
            bool rv = false;
            if (!Initialized) return rv;
            try
            {
                m_RFIDTag = string.Empty;
                m_DataReceivedOk = false;
                m_DataReceivedFormatNg = false;

                m_Client?.Send(readPacket);
                rv = true;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }
        #region Methods - Life Time
        public int GetReadCount()
        {
            return m_ReadCount;
        }
        #endregion
        #endregion

        #region Sequence
        private class SeqMonitor : XSeqFunc
        {
            #region Field
            public static readonly string FuncName = "RFID";
            private DevRFID m_Device = null;
            #endregion

            #region Constructor
            public SeqMonitor(DevRFID device)
            {
                this.SeqName = $"SeqMonitor{device.MyName}";
                m_Device = device;
                TaskRFIDControl.Instance.RegSeq(this);
            }
            public override void SeqAbort()
            {
                if (AlarmId > 0)
                {
                    EqpAlarm.Reset(AlarmId);
                    AlarmId = 0;
                }
                this.InitSeq();
            }
            #endregion

            #region override
            public override int Do()
            {
                if (!m_Device.Initialized) return -1;
                if (AppConfig.Instance.Simulation.MY_DEBUG) return -1;

                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (!m_Device.IsConnected())
                            {
                                seqNo = 10;
                            }
                        }
                        break;

                    case 10:
                        {
                            m_Device.Connect();
                            SequenceDeviceLog.WriteLog(FuncName, "Tcp Begin Connect", seqNo);
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        break;

                    case 20:
                        {
                            if (m_Device.IsConnected())
                            {
                                SequenceDeviceLog.WriteLog(FuncName, "Tcp Connected", seqNo);
                                seqNo = 0;
                                rv = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 5 * 1000)
                            {
                                SequenceDeviceLog.WriteLog(FuncName, "Tcp Connection Timeover", seqNo);
                                seqNo = 30;
                            }
                        }
                        break;

                    case 30:
                        {
                            // Disconnect
                            m_Device.Disconnect();
                            SequenceDeviceLog.WriteLog(FuncName, "Tcp Begin Disconnect", seqNo);
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 40;
                        }
                        break;

                    case 40:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 2 * 1000) break;
                            SequenceDeviceLog.WriteLog(FuncName, "Tcp Disconnected", seqNo);
                            seqNo = 0;
                            rv = 0;
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            #endregion
        }
        private class SeqAction : XSeqFunc
        {
            #region Field
            public static readonly string FuncName = "RFID";

            private DevRFID m_Device = null;
            private int m_ReadRetryCount = 0;
            #endregion

            #region Constructor
            public SeqAction(DevRFID device)
            {
                this.SeqName = $"SeqAction{device.MyName}";
                m_Device = device;
                TaskRFIDControl.Instance.RegSeq(this);
            }
            public override void SeqAbort()
            {
                if (AlarmId > 0)
                {
                    EqpAlarm.Reset(AlarmId);
                    AlarmId = 0;
                }
                this.InitSeq();
            }
            #endregion

            #region override
            public override int Do()
            {
                if (!m_Device.Initialized) return -1;

                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_Device.RFIDReadStart)
                            {
							    m_Device.ReadCount++;
                                m_ReadRetryCount = 0;
                                if (m_Device.Type != enRFIDCommType.TCP)
                                {
                                    m_Device.ResetFlag();
                                    m_Device.RFIDConnectedNg = true;
                                }
                                else if (m_Device.IsConnected())
                                {
                                    m_Device.SendCommand();
                                    m_Device.RFIDReading = true;
                                    SequenceDeviceLog.WriteLog(FuncName, "Reading [Send Command]", seqNo);
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 100;
                                }
                                else
                                {
                                    SequenceDeviceLog.WriteLog(FuncName, "Reading [Disconnected Status]", seqNo);
                                    m_Device.m_SeqMonitor.SeqAbort(); // 재연결 시도
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 10;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            if (m_Device.IsConnected())
                            {
                                SequenceDeviceLog.WriteLog(FuncName, "Reading [Connected Status]", seqNo);
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 5 * 1000)
                            {
                                m_Device.ResetFlag();
                                m_Device.RFIDConnectedNg = true;
                                SequenceDeviceLog.WriteLog(FuncName, "Reading Cancel! [Disconnected Status]", seqNo);
                                seqNo = 0;
                            }
                        }
                        break;

                    case 100:
                        {
                            bool data_received_ok = false;
                            bool data_received_fail = false;
                            if (m_Device.m_DataReceivedOk)
                            {
                                if (m_Device.m_DataReceivedFormatNg) SequenceDeviceLog.WriteLog(FuncName, "Reading NG (Received Format)", seqNo);
                                else if (string.IsNullOrEmpty(m_Device.m_RFIDTag)) SequenceDeviceLog.WriteLog(FuncName, "Reading NG (Null Data)", seqNo);

                                data_received_fail |= m_Device.m_DataReceivedFormatNg;
                                data_received_fail |= string.IsNullOrEmpty(m_Device.m_RFIDTag);
                                data_received_ok = data_received_fail ? false : true;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Device.ReadInterval)
                            {
                                SequenceDeviceLog.WriteLog(FuncName, string.Format("Read Timeout Error"), seqNo);
                                data_received_fail = true;
                            }

                            if (data_received_ok)
                            {
                                m_Device.ResetFlag();
                                SequenceDeviceLog.WriteLog(FuncName, "Reading OK", seqNo);
                                m_Device.RFIDReadCompleted = true;
                                seqNo = 0;
                                rv = 0;
                            }
                            else if (data_received_fail)
                            {
                                if (m_ReadRetryCount < m_Device.ReadRetryMaxCount)
                                {
                                    SequenceDeviceLog.WriteLog(FuncName, string.Format("Read Timeout Retry [{0}/{1}]", m_ReadRetryCount, m_Device.ReadRetryMaxCount), seqNo);
                                    m_ReadRetryCount++;
                                    seqNo = 0;
                                }
                                else
                                {
                                    SequenceDeviceLog.WriteLog(FuncName, string.Format("Read Timeout Retry Over"), seqNo);

                                    m_ReadRetryCount = 0;
                                    m_Device.ResetFlag();
                                    m_Device.RFIDReadingNg = true;
                                    rv = 0;
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 1000:
                        {
                            if (IsPushedSwitch.IsAlarmReset)
                            {
                                IsPushedSwitch.m_AlarmRstPushed = false;
                                EqpAlarm.Reset(AlarmId);
                                SequenceDeviceLog.WriteLog(FuncName, string.Format("Reset Alarm : Code[{0}]", AlarmId));
                                m_Device.ResetFlag();
                                AlarmId = 0;
                                seqNo = 0;
                            }
                            break;
                        }
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

                var helperXml = new XmlHelper<DevRFID>();
                DevRFID dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.Type = dev.Type;
                    this.IPStr = dev.IPStr;
                    this.IPPort = dev.IPPort;
                    this.Comm = dev.Comm;
                    this.ReadInterval = dev.ReadInterval;
                    this.RFIDStringLength = dev.RFIDStringLength;
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
                var helperXml = new XmlHelper<DevRFID>();
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
