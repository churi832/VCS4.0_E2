using Sineva.VHL.Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sineva.VHL.IF.OCS
{
    [Serializable]
    public class OCSComm
    {
        #region Fields
        private bool m_OcsUse = true;
        private AsyncSocketClient m_Client = null;
        private UInt32 m_ReplyTimeoutInterval = 5000;
        private ConnectionMode m_ConnectionMode = ConnectionMode.Client;
        private Encoding m_Encoding = Encoding.Default;
        private ProtocolType m_PortocolType = ProtocolType.Tcp;
        private string m_ServerIpAddress = string.Format("127.0.0.1");
        private ushort m_ServerPortNo = 7456;

        private UInt16 _SystemByte = 0x1;
        private ConcurrentDictionary<UInt16, DateTime> _SentMessageList = new ConcurrentDictionary<UInt16, DateTime>();
        private ConcurrentDictionary<UInt16, VehicleIF> _RetryMessageList = new ConcurrentDictionary<ushort, VehicleIF>();

        private const int BUFFER_SIZE = 1024 * 1024;
        private const int _MessageLengthIndex = 1;
        private const int _MessageCodeIndex = 5;
        private const int _SystemByteIndex = 6;
        #endregion

        #region Properties
        public bool OcsUse
        {
            get { return m_OcsUse; }
            set { m_OcsUse = value; }
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
            get {return _SystemByte; }
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
            set { m_ServerPortNo = value; }
        }
        [XmlIgnore, Browsable(false)]
        public bool ConnectIng { get; set; }
        [XmlIgnore, Browsable(false)]
        public bool DisconnectIng { get; set; }
        #endregion

        #region Constructor
        public OCSComm()
        {

        }
        #endregion

        #region Methods
        public bool Initialize()
        {
            try
            {
                bool result = true;

                //Connect();
                return result;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
                return false;
            }
        }
        public bool Connect()
        {
            ConnectIng = true;
            try
            {
                OCSCommManager.Instance.OcsStatus.Connected = false;
                OCSCommManager.Instance.OcsStatus.ConnectError = false;

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

                if (m_OcsUse)
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
                    OCSCommManager.Instance.OcsStatus.ConnectError = false;
                    OCSCommManager.Instance.OcsStatus.Connected = false;
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
            try
            {
                ConnectIng = false;
                OCSCommManager.Instance.OcsStatus.ConnectError = false;
                OCSCommManager.Instance.OcsStatus.Connected = true;
                OcsCommLog.WriteLog(string.Format("OCS Comm Connected : {0}, {1}", (sender as AsyncSocketClient).IpAddress, (sender as AsyncSocketClient).PortNo));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void OnClose(object sender, AsyncSocketConnectionEventArgs e)
        {
            try
            {
                DisconnectIng = false;
                OCSCommManager.Instance.OcsStatus.ConnectError = false;
                OCSCommManager.Instance.OcsStatus.Connected = false;
                if (m_Client != null)
                {
                    m_Client.OnConnet -= new AsyncSocketConnectEventHandler(OnConnet);
                    m_Client.OnClose -= new AsyncSocketCloseEventHandler(OnClose);
                    m_Client.OnSend -= new AsyncSocketSendEventHandler(OnSend);
                    m_Client.OnReceive -= new AsyncSocketReceiveEventHandler(OnReceive);
                    m_Client.OnError -= new AsyncSocketErrorEventHandler(OnError);
                    m_Client = null;
                }

                OCSCommManager.Instance.OcsStatus.FireSocketClose(this);
                OcsCommLog.WriteLog(string.Format("OCS Comm Disconnected : {0}, {1}", (sender as AsyncSocketClient).IpAddress, (sender as AsyncSocketClient).PortNo));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void OnSend(object sender, AsyncSocketSendEventArgs e)
        {
            //OcsCommLog.WriteLog(string.Format("VHL->OCS Sent Msg : {0}", e.SendData.ToString()));
        }
        private void OnReceive(object sender, AsyncSocketReceiveEventArgs e)
        {
            try
            {
                int readSize = e.ReceiveBytes;
                if (readSize < _MessageCodeIndex) return;
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

                //OcsCommLog.WriteLog(string.Format("VHL<-OCS Recv Msg : {0}", e.ReceiveData.ToString()));
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

                OCSCommManager.Instance.OcsStatus.ConnectError = true;
                OcsCommLog.WriteLog(string.Format("OCS Comm OnError : {0}", e.AsyncSocketException.ToString()));
                //if (OCSCommManager.Instance.OcsStatus.Connected)
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

        #region OCS Process Message
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
        public void OCSMessageSent(VehicleIF message)
        {
            try
            {
                SetLog("VHL->OCS", message);
                OCSCommManager.Instance.OcsStatus.FireMessageSent(this, message);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        public void SendIFMessage(VehicleIF message)
        {
            try
            {
                if (message != null && OCSCommManager.Instance.OcsStatus.Connected)
                {
                    message.VehicleNumber = AppConfig.Instance.VehicleNumber;

                    if (((byte)message.MessageCode) % 2 == 1)
                    {
                        message.SetSystemByte(GetCurrentSystemByte());
                    }
                    byte[] sendStream = ((IVehicleIF)message).GetStreamFromDataOrNull();
                    if (sendStream != null)
                    {
                        m_Client?.Send(sendStream);

                        OCSMessageSent(message);

                        Type msgType = message.GetType();
                        if (msgType == typeof(VehicleIF_EventSend))
                        {
                            VehicleIF_EventSend temp = (VehicleIF_EventSend)message;
                            if (temp.Event == VehicleEvent.AcquireCompleted || temp.Event == VehicleEvent.DepositCompleted)
                            {
                                ResetMessageProgressTime((UInt16)message.SystemByte);
                                if (_RetryMessageList.ContainsKey(Convert.ToUInt16(message.SystemByte)) == false)
                                {
                                    _RetryMessageList.TryAdd(Convert.ToUInt16(message.SystemByte), message);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        private void MessageReceived(byte[] receivedStream)
        {
            try
            {
                if (receivedStream.Length < 7) return;

                IFMessage messageCode = (IFMessage)receivedStream[_MessageCodeIndex];
                UInt16 systemByte = BitConverter.ToUInt16(receivedStream, _SystemByteIndex);
                if (((byte)messageCode) % 2 == 0) // Reply message일 경우 _SentMessageList, _RetryMessageList 확인하여 지우자!!!
                {
                    ReceivedReply(systemByte);
                }

                VehicleIF receivedMessage = null;
                switch (messageCode)
                {
                    case IFMessage.CommandSend:
                        {
                            receivedMessage = new VehicleIF_CommandSend();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                            OCSCommManager.Instance.OcsStatus.FireCommandProcessRequest(this, receivedMessage);
                        }
                        break;
                    case IFMessage.EventReply:
                        {
                            receivedMessage = new VehicleIF_EventReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                            OCSCommManager.Instance.OcsStatus.FireEventReportReply(this, receivedMessage);
                        }
                        break;
                    case IFMessage.AlarmEventReply:
                        {
                            receivedMessage = new VehicleIF_AlarmEventReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                            OCSCommManager.Instance.OcsStatus.FireAlarmReportReply(this, receivedMessage);
                        }
                        break;
                    case IFMessage.MapDataSend:
                        {
                            receivedMessage = new VehicleIF_MapDataSend();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                            //OCSCommManager.Instance.OcsStatus.FireCommandProcessRequest(this, receivedMessage);
                            OCSCommManager.Instance.OcsStatus.FireMapDataProcessRequest(this, receivedMessage);
                        }
                        break;
                    case IFMessage.MapDataReply:
                        {
                            receivedMessage = new VehicleIF_MapDataReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.MapDataRequest:
                        {
                            receivedMessage = new VehicleIF_MapDataRequest();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                            //OCSCommManager.Instance.OcsStatus.FireCommandProcessRequest(this, receivedMessage);
                            OCSCommManager.Instance.OcsStatus.FireMapDataProcessRequest(this, receivedMessage);
                        }
                        break;
                    case IFMessage.MapDataRequestAcknowledge:
                        {
                            receivedMessage = new VehicleIF_MapDataRequestAcknowledge();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.TeachingDataReply:
                        {
                            //ReceivedReply(systemByte);
                            receivedMessage = new VehicleIF_TeachingDataReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.DataVersionRequest:
                        {
                            receivedMessage = new VehicleIF_DataVersionRequest();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                            OCSCommManager.Instance.OcsStatus.FireCommonProcessRequest(this, receivedMessage);
                        }
                        break;
                    case IFMessage.StatusDataReply:
                        {
                            receivedMessage = new VehicleIF_VehicleStatusDataReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                            OCSCommManager.Instance.OcsStatus.FireStatusSendReply(this, receivedMessage);
                        }
                        break;
                    case IFMessage.FDCDataReply:
                        {
                            receivedMessage = new VehicleIF_FDCDataReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.OperationConfigDataSend:
                        {
                            receivedMessage = new VehicleIF_OperationConfigDataSend();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                            OCSCommManager.Instance.OcsStatus.FireCommonProcessRequest(this, receivedMessage);
                        }
                        break;
                    case IFMessage.AutoTeachingResultReply:
                        {
                            receivedMessage = new VehicleIF_AutoTeachingResultReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.UserLoginReply:
                        {
                            receivedMessage = new VehicleIF_UserLoginReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.PathSend:
                        {
                            receivedMessage = new VehicleIF_PathSend();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                            OCSCommManager.Instance.OcsStatus.FireCommonProcessRequest(this, receivedMessage);
                        }
                        break;
                    case IFMessage.CommandStatusRequest:
                        {
                            receivedMessage = new VehicleIF_CommandStatusRequest();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                            OCSCommManager.Instance.OcsStatus.FireCommonProcessRequest(this, receivedMessage);
                        }
                        break;
                    case IFMessage.LocationInformationSend:
                        {
                            receivedMessage = new VehicleIF_LocationInformationSend();
                            (receivedMessage as IVehicleIF).SetDataFromStream(receivedStream);
                            OCSCommManager.Instance.OcsStatus.FireCommonProcessRequest(this, receivedMessage);
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
                    SetLog("VHL<-OCS", receivedMessage);
                    if (messageCode == IFMessage.MapDataReply ||
                        messageCode == IFMessage.MapDataRequestAcknowledge ||
                        messageCode == IFMessage.TeachingDataReply ||
                        messageCode == IFMessage.FDCDataReply ||
                        messageCode == IFMessage.AutoTeachingResultReply ||
                        messageCode == IFMessage.UserLoginReply)
                        OCSCommManager.Instance.OcsStatus.FireMessageReceived(this, receivedMessage);
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        public void ResetMessageProgressTime(UInt16 systemByte)
        {
            try
            {
                if (_SentMessageList.ContainsKey(systemByte) == true)
                {
                    _SentMessageList[systemByte] = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        public void ReceivedReply(UInt16 systemByte)
        {
            try
            {
                if (_SentMessageList.ContainsKey(systemByte) == true)
                {
                    DateTime remove = DateTime.Now;
                    _SentMessageList.TryRemove(systemByte, out remove);
                }
                VehicleIF removeIF = new VehicleIF();
                if (_RetryMessageList.ContainsKey(systemByte) == true)
                {
                    _RetryMessageList.TryRemove(systemByte, out removeIF);
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }

        private void SetLog(string header, VehicleIF message)
        {
            if (message == null) return;
            //if (message.GetType() == typeof(VehicleIF_VehicleStatusDataSend)) return;
            //if (message.GetType() == typeof(VehicleIF_VehicleStatusDataReply)) return;

            try
            {
                List<string> msg = new List<string>();
                if (message.GetType() == typeof(VehicleIF_VehicleStatusDataSend))
                {
                    VehicleIF_VehicleStatusDataSend sds = (message as VehicleIF_VehicleStatusDataSend);
                    string cmd = sds.CurrentCommandID.Replace('\0', ' ').Trim();
                    bool auto = sds.AutoRunStatus;
                    bool carrier = sds.CarrierExistStatus;
                    int node = sds.CurrentNodeNo;
                    bool moving = sds.MovingStatus;
                    bool error = sds.ErrorStatus;
                    string info = string.Format("CMD:{0} MODE:{1} FOUP:{2} CUR_NODE:{3} MOVING:{4} ERROR:{5}", cmd, auto, carrier, node, moving, error);
                    msg.Add(string.Format("[{0}] {1} {2} {3}", header, message.GetType(), message.SystemByte, info));
                    OcsCommStatusLog.WriteLog(msg.ToArray());
                }
                else if (message.GetType() == typeof(VehicleIF_VehicleStatusDataReply))
                {
                    msg.Add(string.Format("[{0}] {1} {2}", header, message.GetType(), message.SystemByte));
                    OcsCommStatusLog.WriteLog(msg.ToArray());
                }
                else
                {
                    msg.Add(string.Format("[{0}] {1}", header, message.GetType()));
                    msg.AddRange((message as IVehicleIF).GetLogData());
                    OcsCommLog.WriteLog(msg.ToArray());
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
