using Sineva.VHL.Library;
using Sineva.VHL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Sineva.VHL.IF.JCS
{
    [Serializable]
    public class JCSComm
    {
        #region Fields
        private bool m_JcsUse = true;
        private AsyncSocketClient m_Client = null;
        private UInt32 m_ReplyTimeoutInterval = 5000;
        private ConnectionMode m_ConnectionMode = ConnectionMode.Client;
        private Encoding m_Encoding = Encoding.Default;
        private ProtocolType m_PortocolType = ProtocolType.Tcp;
        private string m_ServerIpAddress = string.Format("127.0.0.1");
        private ushort m_ServerPortNo = 7456;

        private UInt16 _SystemByte = 0x1;
        private ConcurrentDictionary<UInt16, DateTime> _SentMessageList = new ConcurrentDictionary<UInt16, DateTime>();
        private ConcurrentDictionary<UInt16, JCSIF> _RetryMessageList = new ConcurrentDictionary<ushort, JCSIF>();

        private const int LENGTH_BYTE_INDEX = 1;
        private const int CODE_BYTE_INDEX = 5;
        private const int SYSTEM_BYTE_INDEX = 6;
        #endregion

        #region Properties
        public bool JcsUse
        {
            get { return m_JcsUse; }
            set { m_JcsUse = value; }
        }
        [XmlIgnore]
        public AsyncSocketClient Client
        {
            get { return m_Client; } 
            set { m_Client = value; }
        }
        public UInt32 ReplyTimeoutInterval
        {
            get { return m_ReplyTimeoutInterval; }
            set { m_ReplyTimeoutInterval = value; }
        }
        public ConnectionMode _ConnectionMode
        {
            get { return m_ConnectionMode; }
            set { m_ConnectionMode = value; }
        }
        [XmlIgnore]
        public UInt16 SystemByte
        {
            get { return _SystemByte; }
            set { _SystemByte = value; }
        }
        [XmlIgnore]
        public Encoding _Encoding
        {
            get { return m_Encoding; }
            set { m_Encoding = value; }
        }
        [XmlIgnore]
        public ProtocolType _ProtocolType
        {
            get { return m_PortocolType; }
            set { m_PortocolType = value; }
        }
        public string ServerIpAddress
        {
            get { return m_ServerIpAddress; }
            set { m_ServerIpAddress = value; }
        }
        public ushort ServerPortNo
        {
            get { return m_ServerPortNo; }
            set { m_ServerPortNo = value;}
        }
        [XmlIgnore, Browsable(false)]
        public bool ConnectIng { get; set; }
        [XmlIgnore, Browsable(false)]
        public bool DisconnectIng { get; set; }
        #endregion

        #region Constructor
        public JCSComm() 
        {

        }
        #endregion

        #region Methods
        public bool Initialize()
        {
            try
            {
                _SentMessageList.Clear();
                _RetryMessageList.Clear();

                //Connect();
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
                return false;
            }
            return true;
        }

        public bool Connect()
        {
            ConnectIng = true;
            try
            {
                JCSCommManager.Instance.JcsStatus.Connected = false;
                JCSCommManager.Instance.JcsStatus.ConnectError = false;

                if (m_Client != null)
                {
                    m_Client.OnConnet -= new AsyncSocketConnectEventHandler(OnConnet);
                    m_Client.OnClose -= new AsyncSocketCloseEventHandler(OnClose);
                    m_Client.OnSend -= new AsyncSocketSendEventHandler(OnSend);
                    m_Client.OnReceive -= new AsyncSocketReceiveEventHandler(OnReceive);
                    m_Client.OnError -= new AsyncSocketErrorEventHandler(OnError);
                }
                else
                {
                    m_Client = new AsyncSocketClient(0);
                }
                m_Client.OnConnet += new AsyncSocketConnectEventHandler(OnConnet);
                m_Client.OnClose += new AsyncSocketCloseEventHandler(OnClose);
                m_Client.OnSend += new AsyncSocketSendEventHandler(OnSend);
                m_Client.OnReceive += new AsyncSocketReceiveEventHandler(OnReceive);
                m_Client.OnError += new AsyncSocketErrorEventHandler(OnError);

                m_Client.SetEncoding(m_Encoding);
                m_Client.SetProtocolType(m_PortocolType);

                if (m_JcsUse)
                    m_Client?.Connect(m_ServerIpAddress, m_ServerPortNo);
                
                ConnectIng = false;
            }
            catch (Exception ex)
            {
                ConnectIng = false;
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
                return false;
            }
            return true;
        }
        public bool Disconnect()
        {
            DisconnectIng = true;
            try
            {
                m_Client?.Close();
                DisconnectIng = false;
                if (m_Client == null)
                {
                    JCSCommManager.Instance.JcsStatus.ConnectError = false;
                    JCSCommManager.Instance.JcsStatus.Connected = false;
                }
            }
            catch (Exception ex)
            {
                DisconnectIng = false;
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
                return false;
            }
            return true;
        }
        #endregion

        #region Methods - AsyncSocket Client Event
        private void OnConnet(object sender, AsyncSocketConnectionEventArgs e)
        {
            ConnectIng = false;
            JCSCommManager.Instance.JcsStatus.ConnectError = false;
            JCSCommManager.Instance.JcsStatus.Connected = true;
            JcsCommLog.WriteLog(string.Format("JCS Comm Connected : {0}, {1}", (sender as AsyncSocketClient).IpAddress, (sender as AsyncSocketClient).PortNo));
        }
        private void OnClose(object sender, AsyncSocketConnectionEventArgs e)
        {
            try
            {
                DisconnectIng = false;
                JCSCommManager.Instance.JcsStatus.ConnectError = false;
                JCSCommManager.Instance.JcsStatus.Connected = false;
                if (m_Client != null)
                {
                    m_Client.OnConnet -= new AsyncSocketConnectEventHandler(OnConnet);
                    m_Client.OnClose -= new AsyncSocketCloseEventHandler(OnClose);
                    m_Client.OnSend -= new AsyncSocketSendEventHandler(OnSend);
                    m_Client.OnReceive -= new AsyncSocketReceiveEventHandler(OnReceive);
                    m_Client.OnError -= new AsyncSocketErrorEventHandler(OnError);

                    m_Client = null;
                }

                JCSCommManager.Instance.JcsStatus.FireSocketClose(this);
                JcsCommLog.WriteLog(string.Format("JCS Comm Disconnected : {0}, {1}", (sender as AsyncSocketClient).IpAddress, (sender as AsyncSocketClient).PortNo));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void OnSend(object sender, AsyncSocketSendEventArgs e)
        {
            //JcsCommLog.WriteLog(string.Format("VHL->JCS Sent Msg : {0}", e.SendData.ToString()));
        }
        private void OnReceive(object sender, AsyncSocketReceiveEventArgs e)
        {
            try
            {
                int readSize = e.ReceiveBytes;
                if (readSize < CODE_BYTE_INDEX) return;
                byte[] temp = new byte[4];
                temp[0] = (byte)e.ReceiveBuffer[1];
                temp[1] = (byte)e.ReceiveBuffer[2];
                temp[2] = (byte)e.ReceiveBuffer[3];
                temp[3] = (byte)e.ReceiveBuffer[4];
                int length = (int)BitConverter.ToUInt32(temp, 0);
                if (length > readSize) return;

                temp = new byte[length];
                Array.Copy(e.ReceiveBuffer, temp, length);
                MessageReceived(temp);

                //JcsCommLog.WriteLog(string.Format("VHL<-JCS Recv Msg : {0}", e.ReceiveData.ToString()));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void OnError(object sender, AsyncSocketErrorEventArgs e)
        {
            try
            {
                ConnectIng = false;
                DisconnectIng = false;

                JCSCommManager.Instance.JcsStatus.ConnectError = true;
                JcsCommLog.WriteLog(string.Format("JCS Comm OnError : {0}", e.AsyncSocketException.Message.ToString()));
                //if (JCSCommManager.Instance.JcsStatus.Connected)
                //{
                //    Disconnect();
                //}
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion

        #region JCS Process Message
        public UInt16 GetCurrentSystemByte()
        {
            try
            {
                UInt16 returnValue = _SystemByte;

                _SystemByte += 1;
                if (_SystemByte == 0)
                {
                    _SystemByte = 1;
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
                return 0;
            }
        }
        public void ReceivedReply(UInt16 systemByte)
        {
            try
            {
                //lock (_SentMessageList)
                {
                    DateTime removedItem = DateTime.Now;
                    if (_SentMessageList.ContainsKey(systemByte))
                    {
                        _SentMessageList.TryRemove(systemByte, out removedItem);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        public MessageCode GetMessageCode(byte[] receivedStream)
        {
            return (MessageCode)receivedStream[CODE_BYTE_INDEX];
        }
        public void MessageReceived(byte[] receivedStream)
        {
            try
            {
                if (receivedStream.Length < 6) return;

                MessageCode messageCode = GetMessageCode(receivedStream);
                UInt16 systemByte = BitConverter.ToUInt16(receivedStream, SYSTEM_BYTE_INDEX);
                if (((byte)messageCode) % 2 == 0)
                {
                    ReceivedReply(systemByte);
                }
                JCSIF receivedMessage = null;

                switch (messageCode)
                {
                    case MessageCode.PRR:
                        {
                            receivedMessage = new JCSIF_PRR(systemByte);
                            (receivedMessage as JCSIF_PRR).SetDataFromStream(receivedStream);
                        }
                        break;
                    case MessageCode.PS:
                        {
                            receivedMessage = new JCSIF_PS(systemByte);
                            (receivedMessage as JCSIF_PS).SetDataFromStream(receivedStream);
                        }
                        break;
                    case MessageCode.SRR:
                        {
                            receivedMessage = new JCSIF_SRR(systemByte);
                            (receivedMessage as JCSIF_SRR).SetDataFromStream(receivedStream);
                        }
                        break;
                    case MessageCode.CRR:
                        {
                            receivedMessage = new JCSIF_CRR(systemByte);
                            (receivedMessage as JCSIF_CRR).SetDataFromStream(receivedStream);
                        }
                        break;
                    default:
                        {
                            receivedMessage = null;
                        }
                        break;
                }
                if (receivedMessage != null)
                {
                    SetLog("VHL<-JCS", receivedMessage);
                    JCSCommManager.Instance.JcsStatus.FireMessageReceived(this, receivedMessage);
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        public void JCSMessageSent(JCSIF message)
        {
            try
            {
                SetLog("VHL->JCS", message);
                JCSCommManager.Instance.JcsStatus.FireMessageSent(this, message);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        public void JCSSendMessage(JCSIF message)
        {
            try
            {
                if (message != null && JCSCommManager.Instance.JcsStatus.Connected)
                {
                    message.VehicleNumber = AppConfig.Instance.VehicleNumber;
                    if (((byte)message.Code) % 2 == 1)
                    {
                        message.SetSystemByte(GetCurrentSystemByte());
                    }
                    byte[] sendStream = ((IJCSIF)message).GetStreamFromDataOrNull();
                    if (sendStream != null)
                    {
                        m_Client?.Send(sendStream);
                        JCSMessageSent(message);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        private void SetLog(string header, JCSIF message)
        {
            if (message == null) return;
            //if (message.GetType() == typeof(JCSIF_SR)) return;
            //if (message.GetType() == typeof(JCSIF_SRR)) return;

            try
            {
                List<string> msg = new List<string>();
                if (message.GetType() == typeof(JCSIF_SR))
                {
                    msg.Add(string.Format("[{0}] {1} {2} {3}", header, message.GetType(), message.SystemByte, (message as JCSIF_SR).CurrentNode));
                    JcsCommStatusLog.WriteLog(msg.ToArray());
                }
                else if (message.GetType() == typeof(JCSIF_SRR))
                {
                    msg.Add(string.Format("[{0}] {1} {2}", header, message.GetType(), message.SystemByte));
                    JcsCommStatusLog.WriteLog(msg.ToArray());
                }
                else
                {
                    msg.Add(string.Format("[{0}] {1}", header, message.GetType()));
                    msg.AddRange((message as IJCSIF).GetLogData());
                    JcsCommLog.WriteLog(msg.ToArray());
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        #endregion
    }
}
