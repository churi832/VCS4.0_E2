using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library;
using Sineva.VHL.Data.Alarm;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing.Design;

namespace Sineva.VHL.Device.Assem
{
    public enum BcrConType
    {
        ETHERNET,
        SERIAL,
    }
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class _DevBCR : _Device
    {
        private const string DevName = "BCR";

        public event DelVoid_String ReceivedDataUpdateHandler;
        public event DelVoid_Void UserKeyInPopupHandler;

        #region Enum
        public enum BL_Command
        {
            NONE,
            LON,
            LOFF,
            RESET,
        }
        #endregion

        #region Fields
        private int m_BcrId = 0;
        protected BcrConType m_ConnectMode = BcrConType.SERIAL;
        protected string m_TriggerStart = "LON";
        protected string m_TriggerStop = "LOFF";
        protected string m_IgnoreText = "ERROR";

        private XComm m_Comm = new XComm();
        private bool m_InProcess = false;

        private AlarmData m_ALM_CommOpenTimeoutError = null;
        private AlarmData m_ALM_BcrcodeScanTimeoutError = null;
        private int m_TimerCommOpenWait = 10;
        private int m_TimerBcrScanWait = -1;

        private BL_Command m_Command;
        protected string m_ResponseData = string.Empty;
        protected string m_ResponseSimulCharSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        protected int m_ResponseSimulDataLength = 10;
        protected bool m_ResponseDataExist = false;
        protected bool m_ResponseDataIncludeTriggerMethod = false;

        protected static readonly char _Header = (char)Constants.STX;
        protected static readonly char _Delimiter = (char)Constants.ETX;
        #endregion

        #region Properties
        [Category("!Setting Device (SERIAL PORT)"), DeviceSetting(true)]
        public XComm Comm
        {
            get { return m_Comm; }
            set { m_Comm = value; }
        }
        [Category("!Setting Device (SERIAL PORT)"), DeviceSetting(true)]
        public int BcrId
        {
            get { return m_BcrId; }
            set { m_BcrId = value; }
        }

        [Category("!Setting Device"), DisplayName("Device Connect Type"), Description("Trigger Command for Barcode Scan Start\nDEFAULT : <LON>"), DeviceSetting(true)]
        public BcrConType ConnectMode
        {
            get { return m_ConnectMode; }
            set { m_ConnectMode = value; }
        }
        [Category("!Setting Device (COMM SETTING)"), Description("Trigger Command for Barcode Scan Start\nDEFAULT : <LON>"), DeviceSetting(true)]
        public string TriggerStart
        {
            get { return m_TriggerStart; }
            set { m_TriggerStart = value; }
        }
        [Category("!Setting Device (COMM SETTING)"), Description("Trigger Command for Barcode Scan Stop\nDEFAULT : <LOFF>"), DeviceSetting(true)]
        public string TriggerStop
        {
            get { return m_TriggerStop; }
            set { m_TriggerStop = value; }
        }
        [Category("!Setting Device (COMM SETTING)"), Description("Read Ignore Error Text\nDEFAULT : <ERROR>"), DeviceSetting(true)]
        public string IgnoreText
        {
            get { return m_IgnoreText; }
            set { m_IgnoreText = value; }
        }
        [Category("!Setting Device"), Description("Barcode Response Data [data, trigger_method]"), DeviceSetting(true)]
        public bool ResponseDataIncludeTriggerMethod
        {
            get { return m_ResponseDataIncludeTriggerMethod; }
            set { m_ResponseDataIncludeTriggerMethod = value; }
        }

        [Category("!Setting Device (Timeout)"), Description("COM Port Open Timeout (sec)"), DeviceSetting(true)]
        public int TimerCommOpenWait
        {
            get { return m_TimerCommOpenWait; }
            set { m_TimerCommOpenWait = value; }
        }
        [Category("!Setting Device (Timeout)"), Description("Barcode Scan Wait Delay (sec)\n[Infinity : Setting <= 0]"), DeviceSetting(true)]
        public int TimerBcrScanWait
        {
            get { return m_TimerBcrScanWait; }
            set { m_TimerBcrScanWait = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_CommOpenTimeoutError
        {
            get { return m_ALM_CommOpenTimeoutError; }
            set { m_ALM_CommOpenTimeoutError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_BcrcodeScanTimeoutError
        {
            get { return m_ALM_BcrcodeScanTimeoutError; }
            set { m_ALM_BcrcodeScanTimeoutError = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public bool InProcess
        {
            get { return m_InProcess; }
            set { m_InProcess = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public BL_Command Command
        {
            get { return m_Command; }
            set { m_Command = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public string ResponseData
        {
            get { return m_ResponseData; }
        }
        [Category("!Setting Device (Simulation)"), Description("Simulation Response Data Character Set")]
        [XmlIgnore(), Browsable(false)]
        public string ResponseSimul
        {
            get { return m_ResponseSimulCharSet; }
            set { m_ResponseSimulCharSet = value; }
        }
        [Category("!Setting Device (Simulation)"), Description("Simulation Response Data Text Length")]
        [XmlIgnore(), Browsable(false)]
        public int ResponseSimulDataLength
        {
            get { return m_ResponseSimulDataLength; }
            set { m_ResponseSimulDataLength = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public bool ResponseDataExist
        {
            get
            {
                if (m_ResponseDataExist)
                {
                    m_ResponseDataExist = false;
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Event
        public event DelVoid_String UpdateData;
        #endregion

        #region Constructor
        public _DevBCR()
        {
            this.MyName = "BCR __";
        }
        #endregion

        #region Override
        public override bool Initialize(string name = "", bool read_xml = true, bool heavy_alarm = true)
        {
            if (this.IsValid == false) return true;

            m_Comm.Initialize();
            m_Comm.ReceivedData += new XComm.ReceivedDataEventHandler(CommReceivedData);

            m_ALM_BcrcodeScanTimeoutError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, heavy_alarm, ParentName, MyName, "Scan Timeout Error");
            m_ALM_CommOpenTimeoutError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, heavy_alarm, ParentName, MyName, "Open Timeout Error"); 

            return true;
        }
        private void CommReceivedData(object sender)
        {
            string readData = string.Empty;
            string readTemp = string.Empty;
            do
            {
                System.Threading.Thread.Sleep(10);
                readTemp = m_Comm.ReadExisting();
                readData += readTemp;
            } while (!string.IsNullOrEmpty(readTemp));

            if (MakeData(readData) != m_IgnoreText)
            {
                if (this.ReceivedDataUpdateHandler != null)
                {
                    this.ReceivedDataUpdateHandler.Invoke(m_ResponseData);
                }
            }

            m_InProcess = false;
        }

        #endregion

        #region Methods
        public virtual void OpenPort()
        {
            if (AppConfig.Instance.Simulation.SERIAL) return;
            if (m_Comm._CommPortNo != PortNo._Emulate) m_Comm.Open();

            m_Comm.FlushInBuffer();
            m_Comm.FlushOutBuffer();
        }
        public virtual void ClosePort()
        {
            m_Comm.Close();
        }

        public virtual bool IsOpen()
        {
            return m_Comm.IsOpen();
        }
        public virtual bool SetTrigger()
        {
            m_InProcess = true;

            string cmd = _Header + m_TriggerStart + _Delimiter;
            bool rv = m_Comm.Write(cmd);

            if (AppConfig.Instance.Simulation.SERIAL)
            {
                m_ResponseData = SimulResponse((int)XFunc.GetTickCount());

                if (this.ReceivedDataUpdateHandler != null)
                    this.ReceivedDataUpdateHandler.Invoke(m_ResponseData);
            }

            return rv;
        }
        public virtual bool ResetTrigger()
        {
            m_ResponseData = string.Empty;
            m_InProcess = false;

            string cmd = _Header + m_TriggerStop + _Delimiter;
            return m_Comm.Write(cmd);
        }
        public virtual void KeyInResponse(string keyIn)
        {
            m_ResponseData = keyIn;
            if (this.ReceivedDataUpdateHandler != null)
                this.ReceivedDataUpdateHandler.Invoke(m_ResponseData);
            m_InProcess = false;
        }
        public virtual bool SendCommand(BL_Command cmd)
        {
            if (cmd != BL_Command.NONE && m_Comm.IsOpen())
            {
                string cmdText = (char)Constants.STX + cmd.ToString() + (char)Constants.ETX;
                return m_Comm.Write(cmdText);
            }
            else if (AppConfig.Instance.Simulation.SERIAL)
            {
                MakeData(m_ResponseSimulCharSet);
                return true;
            }

            return false;
        }
        protected string MakeData(string data)
        {
            if (data.Contains(_Header)) data = data.Remove(data.IndexOf(_Header), 1);
            if (data.Contains(_Delimiter)) data = data.Remove(data.IndexOf(_Delimiter), 1);
            if (data.Contains((char)Constants.CR)) data = data.Remove(data.IndexOf((char)Constants.CR), 1);
            if (data.Contains((char)Constants.LF)) data = data.Remove(data.IndexOf((char)Constants.LF), 1);

            return data;
        }
        protected string SimulResponse(int seed)
        {
            if (string.IsNullOrEmpty(m_ResponseSimulCharSet))
                m_ResponseSimulCharSet = "SIMULATION";

            int charSetCount = m_ResponseSimulCharSet.Length;
            char[] charSet = new char[charSetCount];
            for (int i = 0; i < charSetCount; i++)
            {
                charSet[i] = (char)m_ResponseSimulCharSet[i];
            }

            Random rand = new Random(seed);
            string rv = string.Empty;
            for (int i = 0; i < m_ResponseSimulDataLength; i++)
            {
                rv += charSet[rand.Next(charSetCount)];
            }

            return rv;
        }
        #endregion
    }
}
