using Sineva.VHL.Library.RegistryKey;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using AsyncTcpSock;
using ModBus;
using System.Threading;
using System.Windows.Forms;
using static ModBus.ModBus_TCP;

namespace Sineva.VHL.GUI.Touch
{
    public class TouchManager
    {
        #region singleton
        public static readonly TouchManager Instance = new TouchManager();
        #endregion

        #region Fields
        private string m_IpAddress = "192.168.3.100";
        private int m_IpPort = 9000;
        private int m_WrInsTimeOut = 5;
        private int m_SendInterval = 1;
        private int m_ConnectRetryInterval = 5;
        private byte m_DevAddr = 1;
        private int m_HeartBitCheckTime = 1000;

        private TcpClientSock _SocketClient;
        private ushort m_StatusStartAddress = 3;
        private ushort m_AlarmStartAddress = 1000;
        private object m_LockSerialNum = new object();
        private ushort m_SerialNum;
        private DataCodeResolver _DataCodeResolver;
        private Queue<byte[]> _SendQueue = new Queue<byte[]>();
        private Dictionary<ushort, InsMsg> _DicInsMsg = new Dictionary<ushort, InsMsg>();

        private bool m_Initialized = false;
        private UInt32 m_SlaveNo = 0;
        private UInt16 m_Offset = 0;
        private UInt16 m_StartAddress = 0;
        #endregion

        #region Properties
        public string IpAddress
        {
            get { return m_IpAddress; }
            set { m_IpAddress = value; }
        }
        public int IpPort
        {
            get { return m_IpPort; }
            set { m_IpPort = value; }
        }
        public int WrInsTimeOut
        {
            get { return m_WrInsTimeOut; }
            set { m_WrInsTimeOut = value; }
        }
        public int SendInterval
        {
            get { return m_SendInterval; }
            set { m_SendInterval = value; }
        }
        public int ConnectRetryInterval
        {
            get { return m_ConnectRetryInterval; }
            set { m_ConnectRetryInterval = value; }
        }
        public byte DevAddr
        {
            get { return m_DevAddr; }
            set { m_DevAddr = value; }
        }
        public ushort StatusStartAddress
        {
            get { return m_StatusStartAddress; }
            set { m_StatusStartAddress = value; }
        }
        public ushort AlarmStartAddress
        {
            get { return m_AlarmStartAddress; }
            set { m_AlarmStartAddress = value; }
        }
        public int SendQueueCount
        {
            get { return _SendQueue.Count; }
        }
        public int HeartBitCheckTime
        {
            get { return m_HeartBitCheckTime; }
            set { m_HeartBitCheckTime = value; }
        }
        public UInt32 SlaveNo
        {
            get { return m_SlaveNo; }
            set { m_SlaveNo = value; }
        }
        public UInt16 Offset
        {
            get { return m_Offset; }
            set { m_Offset = value; }
        }
        public UInt16 StartAddress
        {
            get { return m_StartAddress; }
            set { m_StartAddress = value; }
        }
        #endregion

        #region Constructor
        private TouchManager()
        {
            Initialize();
        }
        #endregion

        #region Methods
        public bool Initialize()
        {
            if (!m_Initialized)
            {
                m_IpAddress = RegistryConfiguration.Instance.ReadEntry("IPADDRESS", m_IpAddress);
                m_IpPort = RegistryConfiguration.Instance.ReadEntry("PORTNUMBER", m_IpPort); // Port Number = 502는 MXP에서 사용중. 충돌 발생
                m_WrInsTimeOut = RegistryConfiguration.Instance.ReadEntry("WrInsTimeOut", m_WrInsTimeOut);
                m_SendInterval = RegistryConfiguration.Instance.ReadEntry("SendInterval", m_SendInterval);
                m_ConnectRetryInterval = RegistryConfiguration.Instance.ReadEntry("ConnectRetryInterval", m_ConnectRetryInterval);
                m_DevAddr = RegistryConfiguration.Instance.ReadEntry("DEVADDR", m_DevAddr);
                m_StatusStartAddress = RegistryConfiguration.Instance.ReadEntry("STATUSSTARTADDRESS", m_StatusStartAddress);
                m_AlarmStartAddress = RegistryConfiguration.Instance.ReadEntry("ALARMSTARTADDRESS", m_AlarmStartAddress);
                m_HeartBitCheckTime = RegistryConfiguration.Instance.ReadEntry("HEARTBITCHECKTIME", m_HeartBitCheckTime);
                m_SlaveNo = RegistryConfiguration.Instance.ReadEntry("SLAVENO", m_SlaveNo);
                m_Offset = RegistryConfiguration.Instance.ReadEntry("OFFSET", m_Offset);
                m_StartAddress = RegistryConfiguration.Instance.ReadEntry("STARTADDRESS", m_StartAddress);

                _DataCodeResolver = new DataCodeResolver(DataCodeResolver.codingIDenum.Base64);
                _SocketClient = new TcpClientSock(_DataCodeResolver);
                _SocketClient.connToSvrEvent += TcpClient_connToSvrEvent;
                _SocketClient.disconnSvrEvent += TcpClient_disconnSvrEvent;
                _SocketClient.recvGramsEvent += TcpClient_recvGramsEvent;
                m_Initialized = true;
            }
            return m_Initialized;
        }
        public void Save()
        {
            RegistryConfiguration.Instance.WriteEntry("IPADDRESS", m_IpAddress);
            RegistryConfiguration.Instance.WriteEntry("PORTNUMBER", m_IpPort);
            RegistryConfiguration.Instance.WriteEntry("WrInsTimeOut", m_WrInsTimeOut);
            RegistryConfiguration.Instance.WriteEntry("SendInterval", m_SendInterval);
            RegistryConfiguration.Instance.WriteEntry("ConnectRetryInterval", m_ConnectRetryInterval);
            RegistryConfiguration.Instance.WriteEntry("DEVADDR", m_DevAddr);
            RegistryConfiguration.Instance.WriteEntry("STATUSSTARTADDRESS", m_StatusStartAddress);
            RegistryConfiguration.Instance.WriteEntry("ALARMSTARTADDRESS", m_AlarmStartAddress);
            RegistryConfiguration.Instance.WriteEntry("HEARTBITCHECKTIME", m_HeartBitCheckTime);
            RegistryConfiguration.Instance.WriteEntry("SLAVENO", m_SlaveNo);
            RegistryConfiguration.Instance.WriteEntry("OFFSET", m_Offset);
            RegistryConfiguration.Instance.WriteEntry("STARTADDRESS", m_StartAddress);
        }
        #endregion

        #region Methods - Communication
        private void TcpClient_connToSvrEvent(object sender, SockNetEventArgs e)
        {
        }

        private void TcpClient_disconnSvrEvent(object sender, SockNetEventArgs e)
        {
        }

        private void TcpClient_recvGramsEvent(object sender, SockNetEventArgs e)
        {
            object obj = ModBus_TCP.Resolve(_DataCodeResolver.getEncodingBytes(e._session.dataGram));
            Parse(obj);
        }
        public void UpdateStatus(ushort[] datas, ushort[] alarms)
        {
            #region Heart Beat
            WriteRegister((ushort)StatusAddress.Com, (ushort)1);
            #endregion
            WriteMultiRegister(m_StatusStartAddress, datas);
            if (alarms[0] == 1)
            {
                WriteCoilRegister(m_AlarmStartAddress, true);
                WriteRegister((ushort)(m_AlarmStartAddress + 1), alarms[1]);
            }
            else
            {
                WriteCoilRegister(m_AlarmStartAddress, false);
            }
        }
        public bool IsConnected()
        {
            return _SocketClient.isConnected;
        }
        public void Connect()
        {
            try
            {
                if (IpPort == 502) IpPort = 7001; // 502 => MXP Use..
                //if (IpAddress != "127.0.0.1")
                //{
                if (_SocketClient.IsAllowConnect)
                    _SocketClient.connect(IpAddress, IpPort);
                //}
            }
            catch (Exception ex)
            {
                //System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                //ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }
        private ushort GetSerialNum()
        {
            lock (m_LockSerialNum)
            {
                if (m_SerialNum == ushort.MaxValue)
                {
                    m_SerialNum = 0;
                    return ushort.MaxValue;
                }
                else
                    return m_SerialNum++;
            }
        }
        private void AddInsMsg(ushort serialNum, InsMsg insMsg)
        {
            lock (_DicInsMsg)
            {
                if (_DicInsMsg.ContainsKey(serialNum))
                {
                    _DicInsMsg[serialNum] = insMsg;
                }
                else
                {
                    _DicInsMsg.Add(serialNum, insMsg);

                }
            }
        }
        private void RemoveInsMsg(ushort serialNum)
        {
            lock (_DicInsMsg)
            {
                if (_DicInsMsg.ContainsKey(serialNum))
                {
                    _DicInsMsg.Remove(serialNum);
                }
            }
        }
        private InsMsg GetInsMsg(ushort serialNum)
        {
            lock (_DicInsMsg)
            {
                if (_DicInsMsg.ContainsKey(serialNum))
                {
                    return _DicInsMsg[serialNum];
                }
                return null;
            }
        }
        public ushort[] GetRegVal(AddressType addressType, DataType dataType, string val, DecodeWay decodeWay = DecodeWay.Default)
        {
            ushort[] regVal = null;
            switch (addressType)
            {
                case AddressType.Coil:
                    break;
                case AddressType.HoldRegister:
                    switch (dataType)
                    {
                        case DataType.Bool:
                            break;
                        case DataType.Short:
                            {
                                regVal = new ushort[1];
                                decimal wrtVal = decimal.Parse(val);
                                byte[] byt = BitConverter.GetBytes((short)wrtVal);
                                switch (decodeWay)
                                {
                                    case DecodeWay.SmallBefore:
                                    case DecodeWay.Both:
                                        Array.Reverse(byt);
                                        break;
                                    case DecodeWay.ReverseWord:
                                        break;
                                    case DecodeWay.Default:
                                    default:
                                        break;
                                }
                                regVal[0] = BitConverter.ToUInt16(byt, 0);
                            }
                            break;
                        case DataType.UShort:
                            {
                                regVal = new ushort[1];
                                decimal wrtVal = decimal.Parse(val);
                                byte[] byt = BitConverter.GetBytes((ushort)wrtVal);
                                switch (decodeWay)
                                {
                                    case DecodeWay.SmallBefore:
                                    case DecodeWay.Both:
                                        Array.Reverse(byt);
                                        break;
                                    case DecodeWay.ReverseWord:
                                        break;
                                    default:
                                        break;
                                }
                                regVal[0] = BitConverter.ToUInt16(byt, 0);
                            }
                            break;
                        case DataType.Int:
                            {
                                regVal = new ushort[2];
                                decimal wrtVal = decimal.Parse(val);
                                byte[] byt = BitConverter.GetBytes((int)wrtVal);
                                byte[] byt1 = new byte[2];
                                byte[] byt2 = new byte[2];
                                switch (decodeWay)
                                {
                                    case DecodeWay.SmallBefore:
                                        Array.Reverse(byt);
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt1, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt2, 0);
                                        break;
                                    case DecodeWay.ReverseWord:
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt1, 0);
                                        break;
                                    case DecodeWay.Both:
                                        Array.Reverse(byt);
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt1, 0);
                                        break;
                                    default:
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt1, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt2, 0);
                                        break;
                                }
                            }
                            break;
                        case DataType.UInt:
                            {
                                regVal = new ushort[2];
                                decimal wrtVal = decimal.Parse(val);
                                byte[] byt = BitConverter.GetBytes((uint)wrtVal);
                                byte[] byt1 = new byte[2];
                                byte[] byt2 = new byte[2];
                                switch (decodeWay)
                                {
                                    case DecodeWay.SmallBefore:
                                        Array.Reverse(byt);
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt1, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt2, 0);
                                        break;
                                    case DecodeWay.ReverseWord:
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt1, 0);
                                        break;
                                    case DecodeWay.Both:
                                        Array.Reverse(byt);
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt1, 0);
                                        break;
                                    default:
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt1, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt2, 0);
                                        break;
                                }
                            }
                            break;
                        case DataType.Long:
                            {
                                regVal = new ushort[4];
                                decimal wrtVal = decimal.Parse(val);
                                byte[] byt = BitConverter.GetBytes((long)wrtVal);
                                byte[] byt1 = new byte[2];
                                byte[] byt2 = new byte[2];
                                byte[] byt3 = new byte[2];
                                byte[] byt4 = new byte[2];
                                switch (decodeWay)
                                {
                                    case DecodeWay.SmallBefore:
                                        Array.Reverse(byt);
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt1, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt4, 0);
                                        break;
                                    case DecodeWay.ReverseWord:
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt4, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt1, 0);
                                        break;
                                    case DecodeWay.Both:
                                        Array.Reverse(byt);
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt4, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt1, 0);
                                        break;
                                    default:
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt1, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt4, 0);
                                        break;
                                }
                            }
                            break;
                        case DataType.ULong:
                            {
                                regVal = new ushort[4];
                                decimal wrtVal = decimal.Parse(val);
                                byte[] byt = BitConverter.GetBytes((ulong)wrtVal);
                                byte[] byt1 = new byte[2];
                                byte[] byt2 = new byte[2];
                                byte[] byt3 = new byte[2];
                                byte[] byt4 = new byte[2];
                                switch (decodeWay)
                                {
                                    case DecodeWay.SmallBefore:
                                        Array.Reverse(byt);
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt1, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt4, 0);
                                        break;
                                    case DecodeWay.ReverseWord:
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt4, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt1, 0);
                                        break;
                                    case DecodeWay.Both:
                                        Array.Reverse(byt);
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt4, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt1, 0);
                                        break;
                                    default:
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt1, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt4, 0);
                                        break;
                                }
                            }
                            break;
                        case DataType.Float:
                            {
                                regVal = new ushort[2];
                                float wrtVal = float.Parse(val);
                                byte[] byt = BitConverter.GetBytes(wrtVal);
                                byte[] byt1 = new byte[2];
                                byte[] byt2 = new byte[2];
                                switch (decodeWay)
                                {
                                    case DecodeWay.SmallBefore:
                                        Array.Reverse(byt);
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt1, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt2, 0);
                                        break;
                                    case DecodeWay.ReverseWord:
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt1, 0);
                                        break;
                                    case DecodeWay.Both:
                                        Array.Reverse(byt);
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt1, 0);
                                        break;
                                    default:
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt1, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt2, 0);
                                        break;
                                }
                            }
                            break;
                        case DataType.Double:
                            {
                                regVal = new ushort[4];
                                double wrtVal = double.Parse(val);
                                byte[] byt = BitConverter.GetBytes(wrtVal);
                                byte[] byt1 = new byte[2];
                                byte[] byt2 = new byte[2];
                                byte[] byt3 = new byte[2];
                                byte[] byt4 = new byte[2];
                                switch (decodeWay)
                                {
                                    case DecodeWay.SmallBefore:
                                        Array.Reverse(byt);
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt1, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt4, 0);
                                        break;
                                    case DecodeWay.ReverseWord:
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt4, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt1, 0);
                                        break;
                                    case DecodeWay.Both:
                                        Array.Reverse(byt);
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt4, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt1, 0);
                                        break;
                                    default:
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt1, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt4, 0);
                                        break;
                                }
                            }
                            break;

                        case DataType.String:
                            {
                                regVal = new ushort[10];
                                string wrtVal = (val).ToString();
                                byte[] byt = new byte[20];
                                var temp = Encoding.UTF8.GetBytes(wrtVal);
                                Array.Copy(Encoding.UTF8.GetBytes(wrtVal), 0, byt, 0, wrtVal.Length);
                                byte[] byt1 = new byte[2];
                                byte[] byt2 = new byte[2];
                                byte[] byt3 = new byte[2];
                                byte[] byt4 = new byte[2];
                                byte[] byt5 = new byte[2];
                                byte[] byt6 = new byte[2];
                                byte[] byt7 = new byte[2];
                                byte[] byt8 = new byte[2];
                                byte[] byt9 = new byte[2];
                                byte[] byt10 = new byte[2];
                                switch (decodeWay)
                                {
                                    case DecodeWay.SmallBefore:
                                        Array.Reverse(byt);
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt1, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt4, 0);
                                        break;
                                    case DecodeWay.ReverseWord:
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt4, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt1, 0);
                                        break;
                                    case DecodeWay.Both:
                                        Array.Reverse(byt);
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt4, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt1, 0);
                                        break;
                                    default:
                                        Array.Copy(byt, byt1, 2);
                                        Array.Copy(byt, 2, byt2, 0, 2);
                                        Array.Copy(byt, 4, byt3, 0, 2);
                                        Array.Copy(byt, 6, byt4, 0, 2);
                                        Array.Copy(byt, 8, byt5, 0, 2);
                                        Array.Copy(byt, 10, byt6, 0, 2);
                                        Array.Copy(byt, 12, byt7, 0, 2);
                                        Array.Copy(byt, 14, byt8, 0, 2);
                                        Array.Copy(byt, 16, byt9, 0, 2);
                                        Array.Copy(byt, 18, byt10, 0, 2);
                                        regVal[0] = BitConverter.ToUInt16(byt1, 0);
                                        regVal[1] = BitConverter.ToUInt16(byt2, 0);
                                        regVal[2] = BitConverter.ToUInt16(byt3, 0);
                                        regVal[3] = BitConverter.ToUInt16(byt4, 0);
                                        regVal[4] = BitConverter.ToUInt16(byt4, 0);
                                        regVal[5] = BitConverter.ToUInt16(byt4, 0);
                                        regVal[6] = BitConverter.ToUInt16(byt4, 0);
                                        regVal[7] = BitConverter.ToUInt16(byt4, 0);
                                        regVal[8] = BitConverter.ToUInt16(byt4, 0);
                                        regVal[9] = BitConverter.ToUInt16(byt4, 0);
                                        break;
                                }
                            }
                            break;

                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return regVal;
        }

        private bool WriteCoilRegister(ushort addr, bool status)
        {
            bool res = false;
            ushort serialNum = GetSerialNum();
            InsMsg insMsg = new InsMsg();
            AddInsMsg(serialNum, insMsg);
            byte[] bytCmd = ModBus_TCP.GetCmd_WriteCoil(serialNum, m_DevAddr, addr, status);
            SendEnqueue(bytCmd);
            TouchManager.Instance.SendMsg();
            //if (insMsg.sendEvent.WaitOne(m_WrInsTimeOut * 100))
            //{
            res = insMsg.res;
            //}
            //RemoveInsMsg(serialNum);
            return res;
        }
        private bool WriteRegister(ushort addr, ushort regVal)
        {
            bool res = false;
            ushort serialNum = GetSerialNum();
            InsMsg insMsg = new InsMsg();
            AddInsMsg(serialNum, insMsg);
            byte[] bytCmd = ModBus_TCP.GetCmd_WriteRegister(serialNum, m_DevAddr, addr, regVal);
            SendEnqueue(bytCmd);
            TouchManager.Instance.SendMsg();
            //if (insMsg.sendEvent.WaitOne(m_WrInsTimeOut * 100))
            //{
            res = insMsg.res;
            //}
            //RemoveInsMsg(serialNum);
            return res;
        }
        public bool WriteMultiRegister(ushort addr, ushort[] regVal)
        {
            bool res = false;
            ushort serialNum = GetSerialNum();
            InsMsg insMsg = new InsMsg();
            AddInsMsg(serialNum, insMsg);
            byte[] bytCmd = ModBus_TCP.GetCmd_WriteMultiRegister(serialNum, m_DevAddr, addr, regVal);
            SendEnqueue(bytCmd);
            TouchManager.Instance.SendMsg();
            //if (insMsg.sendEvent.WaitOne(m_WrInsTimeOut * 100))
            //{
            res = insMsg.res;
            //}
            //RemoveInsMsg(serialNum);
            return res;
        }
        public void SendEnqueue(byte[] bytCmd)
        {
            lock (_SendQueue)
            {
                _SendQueue.Enqueue(bytCmd);
                //if (_SendQueue.Count == 1)
                //    Monitor.PulseAll(_SendQueue);
            }
        }
        public byte[] SendDequeue()
        {
            lock (_SendQueue)
            {
                if (_SendQueue.Count > 0)
                {
                    return _SendQueue.Dequeue();
                }
                return new byte[] { };
            }
        }
        public void SendMsg()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                byte[] bytCmd = SendDequeue();
                if (IsConnected() && bytCmd.Count() > 0)
                    _SocketClient.sendDataGrams(_DataCodeResolver.getEncodingStr(bytCmd, bytCmd.Length));
            }
            catch (SocketException ex)
            {
                //ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                Connect();
            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            finally
            {
                Thread.Sleep(50);
            }
        }
        private void Parse(object obj)
        {
            try
            {
                if (obj != null)
                {
                    if (obj is ModBus_TCP.WriteRegisterReply)//normal respond for set single register
                    {
                        ModBus_TCP.WriteRegisterReply writeRegisterReply = (ModBus_TCP.WriteRegisterReply)obj;
                        InsMsg insMsg = GetInsMsg(writeRegisterReply.serialNum);
                        if (insMsg != null)
                        {
                            //insMsg.res = true;
                            //insMsg.sendEvent.Set();
                            RemoveInsMsg(writeRegisterReply.serialNum);
                        }
                    }
                    else if (obj is ModBus_TCP.WriteMultiRegisterReply)//normal respond for set multi register
                    {
                        ModBus_TCP.WriteMultiRegisterReply writeMultiRegisterReply = (ModBus_TCP.WriteMultiRegisterReply)obj;
                        InsMsg insMsg = GetInsMsg(writeMultiRegisterReply.serialNum);
                        if (insMsg != null)
                        {
                            //insMsg.res = true;
                            //insMsg.sendEvent.Set();
                            RemoveInsMsg(writeMultiRegisterReply.serialNum);
                        }
                    }
                    else if (obj is ModBus_TCP.ExceptionReply)//error respond
                    {
                        ModBus_TCP.ExceptionReply exceptionReply = (ModBus_TCP.ExceptionReply)obj;
                        InsMsg insMsg = GetInsMsg(exceptionReply.serialNum);
                        if (insMsg != null)
                        {
                            insMsg.res = false;
                            insMsg.errorMsg = "Error";
                            insMsg.sendEvent.Set();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                //ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }
        #endregion
    }
}
