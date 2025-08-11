using Sineva.OHT.Common;
using Sineva.OHT.LogProvider;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Sineva.OHT.IF.Socket
{
    [Guid("F74A850E-5962-4F7D-B4A4-45E6C347E86A")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AsyncClient
    {
        #region Fields
        private bool _Connected = false;
        private bool _ThreadState = true;
        private bool _WatchdogErrorState = false;
        private const int BUFFER_SIZE = 4096;
        private System.Net.Sockets.Socket _ServerSocket = null;
        private byte[] _ReceiveBuffer;
        private string _ServerAddress = string.Empty;
        private int _ServerPort = 0;
        private List<byte> _Buffer = new List<byte>();

        private UInt32 _ReplyTimeoutInterval = 5000;

        private DateTime _LastWatchdogTime;
        private byte _SystemByte = 0x1;
        private Dictionary<byte, DateTime> _SentMessageList = new Dictionary<byte, DateTime>();

        private Thread _ClientThread;
        //2022.12.5 Bytelog 미사용..by HS
        //private LogWriter _ByteLog;
        private LogWriter _DataLog;
        private LogWriter _ErrorLog;
        private string _LogPath = string.Empty;
        private bool _JCSSocket = false;
        private readonly static object _LockKey = new object(); //230301 HJYOU Lock
        #endregion

        #region Properties
        public System.Net.Sockets.Socket ServerSocket
        {
            get { return _ServerSocket; }
        }
        public bool WatchdogErrorState
        {
            get { return _WatchdogErrorState; }
        }
        #endregion

        #region Events
        public delegate void AsyncClientEvent(IPEndPoint target, object eventData);
        public event AsyncClientEvent ServerConnected;
        public event AsyncClientEvent ServerDisconnected;
        public event AsyncClientEvent ReplyTimeoutHappened;
        public event AsyncClientEvent MessageSent;
        public event AsyncClientEvent MessageReceived;
        #endregion

        #region Constructors
        public AsyncClient()
        {
            try
            {
                _ReceiveBuffer = new byte[BUFFER_SIZE];
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        public AsyncClient(string logPath)
        {
            try
            {
                _ReceiveBuffer = new byte[BUFFER_SIZE];
                _LogPath = logPath;
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        #endregion

        #region Methods
        public bool IsConnected()
        {
            return (_ServerSocket == null) ? false : _ServerSocket.Connected;
        }
        public void BeginConnect(string serverAddress, int serverPort, bool jcsSocket = false)
        {
            try
            {
                //_ByteLog = new LogWriter(_LogPath, string.Format("[{0}]ByteLog", serverPort));
                if (jcsSocket == true)
                {
                    _DataLog = new LogWriter(_LogPath, string.Format("[{0}]DataLog", serverPort));
                    _ErrorLog = new LogWriter(_LogPath, string.Format("[{0}]ErrorLog", serverPort));
                }
                else
                {
                    _DataLog = new LogWriter(_LogPath, string.Format("[{0}]DataLog", serverPort));
                    _ErrorLog = new LogWriter(_LogPath, string.Format("[{0}]ErrorLog", serverPort));
                }
                _ServerAddress = serverAddress;
                _ServerPort = serverPort;
                //_ServerSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //_ServerSocket.BeginConnect(serverAddress, serverPort, new AsyncCallback(ConnectCallBack), _ServerSocket);

                //2023.2.8 JCS 구분..by HS
                _JCSSocket = jcsSocket;

                _ThreadState = true;
                _ClientThread = new Thread(new ThreadStart(AsyncClientThread))
                {
                    IsBackground = true
                };
                _ClientThread.Start();
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        private void ConnectCallBack(IAsyncResult result)
        {
            try
            {
                System.Net.Sockets.Socket temporarySocket = result.AsyncState as System.Net.Sockets.Socket;
                IPEndPoint serverEndPoint = temporarySocket.RemoteEndPoint as IPEndPoint;
                temporarySocket.EndConnect(result);

                //_ByteLog = new LogWriter(_LogPath, string.Format("[{0}]ByteLog", (temporarySocket.LocalEndPoint as IPEndPoint).Port));
                //_DataLog = new LogWriter(_LogPath, string.Format("[{0}]DataLog", (temporarySocket.LocalEndPoint as IPEndPoint).Port));
                //_ErrorLog = new LogWriter(_LogPath, string.Format("[{0}]ErrorLog", (temporarySocket.LocalEndPoint as IPEndPoint).Port));

                SetConnectionLog(serverEndPoint, true);

                //_ThreadState = true;
                //_ClientThread = new Thread(new ThreadStart(AsyncClientThread))
                //{
                //    IsBackground = true
                //};
                //_ClientThread.Start();

                _ServerSocket = temporarySocket;
                _ServerSocket.BeginReceive(_ReceiveBuffer, 0, _ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _ServerSocket);

                UpdateLastWatchdog();

                //Event for C#
                if (ServerConnected != null)
                {
                    ServerConnected(serverEndPoint, null);
                }

                _Connected = true;
            }
            catch (Exception ex)
            {
                BeginDisconnect();
                SetErrorLog(ex.ToString());
            }
        }
        private void ReconnecteCallBack(IAsyncResult result)
        {
            try
            {
                System.Net.Sockets.Socket temporarySocket = result.AsyncState as System.Net.Sockets.Socket;
                IPEndPoint serverEndPoint = temporarySocket.RemoteEndPoint as IPEndPoint;
                temporarySocket.EndConnect(result);

                //_ByteLog = new LogWriter(_LogPath, string.Format("[{0}]ByteLog", (temporarySocket.LocalEndPoint as IPEndPoint).Port));
                _DataLog = new LogWriter(_LogPath, string.Format("[{0}]DataLog", (temporarySocket.LocalEndPoint as IPEndPoint).Port));
                _ErrorLog = new LogWriter(_LogPath, string.Format("[{0}]ErrorLog", (temporarySocket.LocalEndPoint as IPEndPoint).Port));

                SetConnectionLog(serverEndPoint, true);

                _ServerSocket = temporarySocket;
                _ServerSocket.BeginReceive(_ReceiveBuffer, 0, _ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _ServerSocket);

                UpdateLastWatchdog();

                //Event for C#
                if (ServerConnected != null)
                {
                    ServerConnected(serverEndPoint, null);
                }

                _Connected = true;
            }
            catch
            {
            }
        }
        public void BeginDisconnect()
        {
            try
            {
                if (_ServerSocket.Connected)
                {
                    _ServerSocket.BeginDisconnect(true, new AsyncCallback(DisconnectCallBack), _ServerSocket);

                }
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        private void DisconnectCallBack(IAsyncResult result)
        {
            try
            {
                //_ServerSocket.EndReceive(result);
                _ServerSocket = result.AsyncState as System.Net.Sockets.Socket;

                SetConnectionLog((_ServerSocket.RemoteEndPoint as IPEndPoint), false);

                _ThreadState = false;
                if (_ClientThread != null)
                {
                    _ClientThread.Join(1000);
                    _ClientThread = null;
                }

                //Event for C#
                if (ServerDisconnected != null)
                {
                    ServerDisconnected(_ServerSocket.RemoteEndPoint as IPEndPoint, null);
                }

                _ServerAddress = string.Empty;
                _ServerPort = 0;
                _ServerSocket = null;

                _Connected = false;
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        private void BeginReceive()
        {
            try
            {
                if (_ServerSocket.Connected == true)
                {
                    _ServerSocket.BeginReceive(_ReceiveBuffer, 0, _ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _ServerSocket);
                }
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        private void ReceiveCallBack(IAsyncResult result)
        {
            try
            {
                System.Net.Sockets.Socket temporarySocket = result.AsyncState as System.Net.Sockets.Socket;

                if (temporarySocket == null) return;

                int readSize = temporarySocket.EndReceive(result);

                if (temporarySocket.Connected == false) return;

                if (readSize > 0)
                {
                    byte[] readByte = new byte[readSize];

                    Array.Copy(_ReceiveBuffer, 0, readByte, 0, readSize);

                    _Buffer.AddRange(readByte);

                    int length = 0;
                    while (true)
                    {
                        if (_Buffer.Count < 3) break;
                        byte[] temp = new byte[2];
                        temp[0] = _Buffer[1];
                        temp[1] = _Buffer[2];

                        length = (int)BitConverter.ToUInt16(temp, 0);

                        if (length > _Buffer.Count) break;

                        temp = new byte[length];
                        _Buffer.CopyTo(0, temp, 0, length);

                        //SetReceivedByteLog((temporarySocket.RemoteEndPoint as IPEndPoint), temp);
                        if (_JCSSocket == true)
                        {
                            ProcessReceivedJCSMessage(temp);
                        }
                        else
                        {
                            ProcessReceivedMessage(temp);
                        }
                        _Buffer.RemoveRange(0, length);
                    }
                }

                BeginReceive();
            }
            catch (SocketException ex)
            {
                if ((ex.SocketErrorCode == SocketError.Disconnecting) || (ex.SocketErrorCode == SocketError.NotConnected))
                {
                }
                else if (ex.SocketErrorCode == SocketError.ConnectionReset)
                {
                    //BeginConnect(_ServerAddress, _ServerPort);
                }
                SetErrorLog(ex.ToString());
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        private void BeginSend(byte[] message)
        {
            try
            {
                if (_ServerSocket == null) return;

                if (_ServerSocket.Connected)
                {
                    byte messageCode = message[3];
                    byte systemByte = message[4];

                    if ((messageCode % 0x2) == 0x1)
                    {
                        SentMessage(systemByte);
                    }

                    //SetSentByteLog((_ServerSocket.RemoteEndPoint as IPEndPoint), message);

                    _ServerSocket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(SendCallBack), message);
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionReset)
                {
                    //BeginConnect(_ServerAddress, _ServerPort);
                }
                SetErrorLog(ex.ToString());
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        private void SendCallBack(IAsyncResult result)
        {
            try
            {
                byte[] stream = (byte[])result.AsyncState;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionReset)
                {
                    //BeginConnect(_ServerAddress, _ServerPort);
                }
                SetErrorLog(ex.ToString());
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        #endregion

        #region Methods - Send/Reply Message Control
        public void UpdateLastWatchdog()
        {
            try
            {
                _LastWatchdogTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        public Int64 GetCurrentWatchdogProgressTime()
        {
            try
            {
                return Convert.ToInt64((DateTime.Now - _LastWatchdogTime).TotalMilliseconds);
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
                return 0;
            }
        }
        public byte GetCurrentSystemByte()
        {
            try
            {
                byte returnValue = _SystemByte;

                _SystemByte += 1;
                if (_SystemByte == 0)
                {
                    _SystemByte = 1;
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
                return 0;
            }
        }
        public void SentMessage(byte systemByte)
        {
            try
            {
                lock (_SentMessageList)
                {
                    if (_SentMessageList.ContainsKey(systemByte) == false)
                    {
                        _SentMessageList.Add(systemByte, DateTime.Now);
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        public void ReceivedReply(byte systemByte)
        {
            try
            {
                lock (_SentMessageList)
                {
                    if (_SentMessageList.ContainsKey(systemByte) == true)
                    {
                        _SentMessageList.Remove(systemByte);
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        public IEnumerable<byte> SentMessageList()
        {
            return _SentMessageList.Keys;
        }
        public UInt32 GetMessageProgressTime(byte systemByte)
        {
            try
            {
                lock (_SentMessageList)
                {
                    if (_SentMessageList.ContainsKey(systemByte) == true)
                    {
                        return Convert.ToUInt32((DateTime.Now - _SentMessageList[systemByte]).TotalMilliseconds);
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
                return 0;
            }
        }
        private void AsyncClientThread()
        {
            List<byte> timeoutHappenedList = new List<byte>();
            string lastError = string.Empty;
            while (_ThreadState)
            {
                try
                {
                    if (_ServerSocket == null)
                    {
                        _ServerSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    }
                    
                    //if (_ServerSocket.Connected == false) continue;
                    if (_ServerSocket.Connected == false)
                    {
                        try
                        {
                            _ServerSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            _ServerSocket.BeginConnect(_ServerAddress, _ServerPort, new AsyncCallback(ReconnecteCallBack), _ServerSocket);
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                        finally
                        {
                            Thread.Sleep(10000);
                        }
                    }
                    else
                    {
                        lock (_SentMessageList)
                        {
                            timeoutHappenedList.Clear();
                            foreach (byte systemByte in SentMessageList())
                            {
                                UInt32 progressedTime = GetMessageProgressTime(systemByte);
                                if (progressedTime > _ReplyTimeoutInterval)
                                {
                                    timeoutHappenedList.Add(systemByte);

                                    SetReplyTimeoutLog((_ServerSocket.RemoteEndPoint as IPEndPoint), systemByte);

                                    //Event for C#
                                    if (ReplyTimeoutHappened != null)
                                    {
                                        ReplyTimeoutHappened((_ServerSocket.RemoteEndPoint as IPEndPoint), systemByte);
                                    }
                                }
                            }
                        }

                        foreach (byte systemByte in timeoutHappenedList)
                        {
                            _SentMessageList.Remove(systemByte);
                        }

                        if (GetCurrentWatchdogProgressTime() > _ReplyTimeoutInterval)
                        {
                            if (_WatchdogErrorState == false) _WatchdogErrorState = true;
                        }
                        else
                        {
                            if (_WatchdogErrorState == true) _WatchdogErrorState = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (lastError != ex.ToString())
                    {
                        SetErrorLog(ex.ToString());
                        lastError = ex.ToString();
                    }
                }
                finally
                {
                    Thread.Sleep(10);
                }
            }
        }
        #endregion
        #region JCS
        public void SendIJCSFMessage(JCSIF messageObject)
        {
            try
            {
                lock (_LockKey)
                {
                    if (((byte)messageObject.MessageCode) % 2 == 1)
                    {
                        messageObject.SetSystemByte(GetCurrentSystemByte());
                    }

                    byte[] sendStream = ((IJCSIF)messageObject).GetStreamFromDataOrNull();

                    if (sendStream != null)
                    {
                        BeginSend(sendStream);

                        SetSentDataLog((_ServerSocket.RemoteEndPoint as IPEndPoint), messageObject);

                        if (MessageSent != null)
                        {
                            MessageSent((_ServerSocket.RemoteEndPoint as IPEndPoint), messageObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());

                if (_Connected == true)
                {
                    _Connected = false;

                    if (ServerDisconnected != null)
                    {
                        ServerDisconnected(_ServerSocket.RemoteEndPoint as IPEndPoint, null);
                    }
                }
            }
        }
        #endregion
        #region Methods - Receive/Send Interface Messaage
        public void SendIFMessage(VehicleIF messageObject)
        {
            try
            {
                if (((byte)messageObject.MessageCode) % 2 == 1)
                {
                    messageObject.SetSystemByte(GetCurrentSystemByte());
                }

                byte[] sendStream = ((IVehicleIF)messageObject).GetStreamFromDataOrNull();

                if (sendStream != null)
                {
                    BeginSend(sendStream);

                    if (messageObject.GetType() != typeof(VehicleIF_VehicleStatusDataSend))
                    {
                        SetSentDataLog((_ServerSocket.RemoteEndPoint as IPEndPoint), messageObject);
                    }

                    if (MessageSent != null)
                    {
                        MessageSent((_ServerSocket.RemoteEndPoint as IPEndPoint), messageObject);
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());

                if (_Connected == true)
                {
                    _Connected = false;

                    if (ServerDisconnected != null)
                    {
                        ServerDisconnected(_ServerSocket.RemoteEndPoint as IPEndPoint, null);
                    }
                }
            }
        }
        private void ProcessReceivedJCSMessage(byte[] receivedStream)
        {
            try
            {
                if (receivedStream.Length < 5) return;

                JCSIFMessage messageCode = (JCSIFMessage)receivedStream[3];
                byte systemByte = receivedStream[4];

                JCSIF receivedMessage = null;

                switch (messageCode)
                {
                    case JCSIFMessage.InitialStateRequest:
                        {
                            receivedMessage = new JCSIF_InitialStateRequest(systemByte);
                            (receivedMessage as JCSIF_InitialStateRequest).SetDataFromStream(receivedStream);
                        }
                        break;
                    case JCSIFMessage.JunctionPassRequestReply:
                        {
                            ReceivedReply(systemByte);
                            receivedMessage = new JCSIF_PassRequestReply(systemByte);
                            (receivedMessage as JCSIF_PassRequestReply).SetDataFromStream(receivedStream);
                        }
                        break;
                    case JCSIFMessage.JunctionPassSend:
                        {
                            receivedMessage = new JCSIF_PassSend(systemByte);
                            (receivedMessage as JCSIF_PassSend).SetDataFromStream(receivedStream);
                        }
                        break;
                    case JCSIFMessage.JunctionPassCompleteSendReply:
                        {
                            ReceivedReply(systemByte);
                            receivedMessage = new JCSIF_PassCompleteSendReply(systemByte);
                            (receivedMessage as JCSIF_PassCompleteSendReply).SetDataFromStream(receivedStream);
                        }
                        break;
                    case JCSIFMessage.JunctionPassStatusChangeSendReply:
                        {
                            ReceivedReply(systemByte);
                            receivedMessage = new JCSIF_PassStatusChangeSendReply(systemByte);
                            (receivedMessage as JCSIF_PassStatusChangeSendReply).SetDataFromStream(receivedStream);
                        }
                        break;
                    case JCSIFMessage.JunctionDistanceChangeSendReply:
                        {
                            ReceivedReply(systemByte);
                            receivedMessage = new JCSIF_DistanceChangeSendReply(systemByte);
                            (receivedMessage as JCSIF_DistanceChangeSendReply).SetDataFromStream(receivedStream);
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
                    SetReceivedDataLog(_ServerSocket.RemoteEndPoint as IPEndPoint, receivedMessage);
                }

                if ((receivedMessage != null) && (MessageReceived != null))
                {
                    MessageReceived(_ServerSocket.RemoteEndPoint as IPEndPoint, receivedMessage);
                }
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        private void ProcessReceivedMessage(byte[] receivedStream)
        {
            try
            {
                if (receivedStream.Length < 5) return;

                IFMessage messageCode = (IFMessage)receivedStream[3];
                byte systemByte = receivedStream[4];

                VehicleIF receivedMessage = null;

                switch (messageCode)
                {
                    case IFMessage.CommandSend:
                        {
                            receivedMessage = new VehicleIF_CommandSend();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.EventReply:
                        {
                            ReceivedReply(systemByte);
                            receivedMessage = new VehicleIF_EventReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.AlarmEventReply:
                        {
                            ReceivedReply(systemByte);
                            receivedMessage = new VehicleIF_AlarmEventReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.MapDataSend:
                        {
                            receivedMessage = new VehicleIF_MapDataSend();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.TeachingDataReply:
                        {
                            ReceivedReply(systemByte);
                            receivedMessage = new VehicleIF_TeachingDataReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.DataVersionRequest:
                        {
                            receivedMessage = new VehicleIF_DataVersionRequest();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.MapDataRequestAcknowledge:
                        {
                            ReceivedReply(systemByte);
                            receivedMessage = new VehicleIF_MapDataRequestAcknowledge();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.StatusDataReply:
                        {
                            ReceivedReply(systemByte);
                            UpdateLastWatchdog();
                            receivedMessage = new VehicleIF_VehicleStatusDataReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.FDCDataReply:
                        {
                            ReceivedReply(systemByte);
                            receivedMessage = new VehicleIF_FDCDataReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.OperationConfigDataSend:
                        {
                            receivedMessage = new VehicleIF_OperationConfigDataSend();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.AutoTeachingResultReply:
                        {
                            ReceivedReply(systemByte);
                            receivedMessage = new VehicleIF_AutoTeachingResultReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.UserLoginReply:
                        {
                            ReceivedReply(systemByte);
                            receivedMessage = new VehicleIF_UserLoginReply();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    case IFMessage.PathSend:
                        {
                            ReceivedReply(systemByte);
                            receivedMessage = new VehicleIF_PathSend();
                            ((IVehicleIF)receivedMessage).SetDataFromStream(receivedStream);
                        }
                        break;
                    default:
                        {
                            receivedMessage = null;
                        }
                        break;
                }

                if ((receivedMessage != null) && (receivedMessage.GetType() != typeof(VehicleIF_VehicleStatusDataReply)))
                {
                    SetReceivedDataLog((_ServerSocket.RemoteEndPoint as IPEndPoint), receivedMessage);
                }

                if ((receivedMessage != null) && (MessageReceived != null))
                {
                    MessageReceived((_ServerSocket.RemoteEndPoint as IPEndPoint), receivedMessage);
                }
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        #endregion

        #region Methods - Log Write
        private void SetErrorLog(string errorText)
        {
            try
            {
                List<string> logData = new List<string>();

                logData.Add(errorText);

                _ErrorLog.SetLog(logData.ToArray());
            }
            catch
            {
            }
        }
        private void SetConnectionLog(IPEndPoint ipEndPoint, bool connected)
        {
            try
            {
                List<string> logData = new List<string>();

                if (connected == true)
                {
                    logData.Add(string.Format("Server Connected : {0}", ipEndPoint.ToString()));
                }
                else
                {
                    logData.Add(string.Format("Server Disconnected : {0}", ipEndPoint.ToString()));
                }

                //_ByteLog.SetLog(logData.ToArray());
                _DataLog.SetLog(logData.ToArray());
            }
            catch
            {
            }
        }
        private void SetReplyTimeoutLog(IPEndPoint ipEndPoint, object eventData)
        {
            try
            {
                List<string> logData = new List<string>();

                logData.Add(string.Format("Reply Timeout : {0}", (byte)eventData));
                logData.Add(string.Format("\t\tIP Address : {0}", ipEndPoint.ToString()));
                logData.Add(string.Format("\t\tSystem Byte : {0}", (byte)eventData));

                //_ByteLog.SetLog(logData.ToArray());
                _DataLog.SetLog(logData.ToArray());
            }
            catch
            {
            }
        }
        private void SetSentByteLog(IPEndPoint ipEndPoint, object sentByte)
        {
            try
            {
                //List<string> logData = new List<string>();
                //byte[] byteStream = (byte[])sentByte;

                //logData.Add(string.Format("Sent (SystemByte : {0}) - {1}", byteStream[4], ipEndPoint.ToString()));
                //logData.Add(string.Format("            [Byte Length : {0}]", byteStream.Length));
                //string temp = string.Format("            ");
                //int splitCount = 0;

                //for (int byteOffset = 0; byteOffset < byteStream.Length; byteOffset++)
                //{
                //    temp += string.Format("{0:X2}  ", byteStream[byteOffset]);
                //    splitCount += 1;

                //    if (splitCount == 10)
                //    {
                //        logData.Add(temp);
                //        temp = string.Format("            ");
                //        splitCount = 0;
                //    }
                //}

                //if (temp.Trim() != string.Empty)
                //{
                //    logData.Add(temp);
                //}

                //if (_ByteLog != null)
                //{
                //    _ByteLog.SetLog(logData.ToArray());
                //}
            }
            catch
            {
            }
        }
        private void SetReceivedByteLog(IPEndPoint ipEndPoint, object receivedByte)
        {
            try
            {
                //List<string> logData = new List<string>();
                //byte[] byteStream = (byte[])receivedByte;

                //logData.Add(string.Format("Received (SystemByte : {0}) - {1}", byteStream[4], ipEndPoint.ToString()));
                //logData.Add(string.Format("            [Byte Length : {0}]", byteStream.Length));
                //string temp = string.Format("            ");
                //int splitCount = 0;

                //for (int byteOffset = 0; byteOffset < byteStream.Length; byteOffset++)
                //{
                //    temp += string.Format("{0:X2}  ", byteStream[byteOffset]);
                //    splitCount += 1;

                //    if (splitCount == 10)
                //    {
                //        logData.Add(temp);
                //        temp = string.Format("            ");
                //        splitCount = 0;
                //    }
                //}

                //if (temp.Trim() != string.Empty)
                //{
                //    logData.Add(temp);
                //}

                //if (_ByteLog != null)
                //{
                //    _ByteLog.SetLog(logData.ToArray());
                //}
            }
            catch
            {
            }
        }
        private List<string> GetHeaderLogData(IPEndPoint ipEndPoint, VehicleIF eventData, bool isReceived)
        {
            try
            {
                List<string> logData = new List<string>();

                logData.Add(string.Format("{0} {1}", (isReceived == true) ? "Received" : "Sent", eventData.GetType()));
                logData.Add(string.Format("\t<HEADER>"));
                logData.Add(string.Format("\t\tMessage Length : {0}", eventData.MessageLength));
                logData.Add(string.Format("\t\tMessage Code : {0}", eventData.MessageCode));
                logData.Add(string.Format("\t\tSystem Byte : {0}", eventData.SystemByte));

                return logData;
            }
            catch
            {
                return new List<string>();
            }
        }
        private void SetReceivedDataLog(IPEndPoint ipEndPoint, VehicleIF eventData)
        {
            try
            {
                List<string> logData = new List<string>();

                //Add Header Log String
                //logData.AddRange(GetHeaderLogData(ipEndPoint, eventData, true));
                //logData.Add(string.Format("\t<BODY>"));

                ////Add Body Log String
                //if (eventData.GetType() == typeof(VehicleIF_CommandSend))
                //{
                //    VehicleIF_CommandSend receivedData = (VehicleIF_CommandSend)eventData;

                //    logData.Add(string.Format("\t\tCommand : {0}", receivedData.Command));
                //    logData.Add(string.Format("\t\tVehicle Number : {0}", receivedData.VehicleNumber));
                //    logData.Add(string.Format("\t\tData Send Interval : {0}", receivedData.DataSendInterval));
                //    logData.Add(string.Format("\t\tTransfer Command ID : {0}", receivedData.TransferCommandID));
                //    logData.Add(string.Format("\t\tCassette ID : {0}", receivedData.CassetteID));
                //    logData.Add(string.Format("\t\tSource ID : {0}", receivedData.SourceID));
                //    logData.Add(string.Format("\t\tDestination ID : {0}", receivedData.DestinationID));
                //    logData.Add(string.Format("\t\tCount of Nodes : {0}", receivedData.PathNodes.Count));

                //    StringBuilder log = new StringBuilder(string.Format("\t\tPath Node : "));
                //    foreach (int node in receivedData.PathNodes)
                //    {
                //        log.Append(string.Format("{0} - ", node));
                //    }
                //    logData.Add(log.ToString());
                //}
                //else if (eventData.GetType() == typeof(VehicleIF_EventReply))
                //{
                //    VehicleIF_EventReply receivedData = (VehicleIF_EventReply)eventData;

                //    logData.Add(string.Format("\t\tAcknowledge : {0}", receivedData.Acknowledge));
                //    logData.Add(string.Format("\t\tEvent : {0}", receivedData.Event));
                //    logData.Add(string.Format("\t\tVehicle Number : {0}", receivedData.VehicleNumber));
                //}
                //else if (eventData.GetType() == typeof(VehicleIF_AlarmEventReply))
                //{
                //    VehicleIF_AlarmEventReply receivedData = (VehicleIF_AlarmEventReply)eventData;

                //    logData.Add(string.Format("\t\tAcknowledge : {0}", receivedData.Acknowledge));
                //}
                //else if (eventData.GetType() == typeof(VehicleIF_MapDataSend))
                //{
                //    VehicleIF_MapDataSend receivedData = (VehicleIF_MapDataSend)eventData;

                //    logData.Add(string.Format("\t\tData Type : {0}", receivedData.DataType));
                //    logData.Add(string.Format("\t\tData Version : {0}", receivedData.DataVersion));
                //}
                //else if (eventData.GetType() == typeof(VehicleIF_TeachingDataReply))
                //{
                //    VehicleIF_TeachingDataReply receivedData = (VehicleIF_TeachingDataReply)eventData;

                //    logData.Add(string.Format("\t\tAcknowledge : {0}", receivedData.Acknowledge));
                //    logData.Add(string.Format("\t\tVehicle Number : {0}", receivedData.VehicleNumber));
                //    logData.Add(string.Format("\t\tData Version : {0}", receivedData.DataVersion));
                //}
                //else if (eventData.GetType() == typeof(VehicleIF_MapDataRequestAcknowledge))
                //{
                //    VehicleIF_MapDataRequestAcknowledge receivedData = (VehicleIF_MapDataRequestAcknowledge)eventData;

                //    logData.Add(string.Format("\t\tAcknowledge : {0}", receivedData.Acknowledge));
                //    logData.Add(string.Format("\t\tData Type : {0}", receivedData.DataType));
                //}
                ////else if (eventData.GetType() == typeof(VehicleIF_VehicleStatusDataReply))
                ////{
                ////    VehicleIF_VehicleStatusDataReply receivedData = (VehicleIF_VehicleStatusDataReply)eventData;

                ////}
                //else if (eventData.GetType() == typeof(VehicleIF_FDCDataReply))
                //{
                //    VehicleIF_FDCDataReply receivedData = (VehicleIF_FDCDataReply)eventData;

                //    logData.Add(string.Format("\t\tAcknowledge : {0}", receivedData.Acknowledge));
                //}

                logData.Add(string.Format("Received {0}", eventData.GetType()));
                logData.AddRange((eventData as IVehicleIF).GetLogData());

                if (_DataLog != null)
                {
                    _DataLog.SetLog(logData.ToArray());
                }
            }
            catch
            {
            }
        }
        private void SetReceivedDataLog(IPEndPoint ipEndPoint, JCSIF eventData)
        {
            try
            {
                List<string> logData = new List<string>();

                logData.Add(string.Format("Received {0}", eventData.GetType()));
                logData.AddRange((eventData as IJCSIF).GetLogData());

                if (_DataLog != null)
                {
                    _DataLog.SetLog(logData.ToArray());
                }
            }
            catch
            {
            }
        }
        private void SetSentDataLog(IPEndPoint ipEndPoint, JCSIF eventData)
        {
            try
            {
                List<string> logData = new List<string>();

                logData.Add(string.Format("Sent {0}", eventData.GetType()));
                logData.AddRange((eventData as IJCSIF).GetLogData());

                if (_DataLog != null)
                {
                    _DataLog.SetLog(logData.ToArray());
                }
            }
            catch
            {
            }
        }

        private void SetSentDataLog(IPEndPoint ipEndPoint, VehicleIF eventData)
        {
            try
            {
                List<string> logData = new List<string>();

                //Add Header Log String
                //logData.AddRange(GetHeaderLogData(ipEndPoint, eventData, false));
                //logData.Add(string.Format("\t<BODY>"));

                ////Add Body Log String
                //if (eventData.GetType() == typeof(VehicleIF_CommandReply))
                //{
                //    VehicleIF_CommandReply sentData = (VehicleIF_CommandReply)eventData;

                //    logData.Add(string.Format("\t<BODY>"));
                //    logData.Add(string.Format("\t\tAcknowledge : {0}", sentData.Acknowledge));
                //    logData.Add(string.Format("\t\tCommand : {0}", sentData.Command));
                //    logData.Add(string.Format("\t\tVehicle Number : {0}", sentData.VehicleNumber));
                //}
                //else if (eventData.GetType() == typeof(VehicleIF_EventSend))
                //{
                //    VehicleIF_EventSend sentData = (VehicleIF_EventSend)eventData;

                //    logData.Add(string.Format("\t\tEvent : {0}", sentData.Event));
                //    logData.Add(string.Format("\t\tVehicle Number : {0}", sentData.VehicleNumber));
                //}
                //else if (eventData.GetType() == typeof(VehicleIF_AlarmEventSend))
                //{
                //    VehicleIF_AlarmEventSend sentData = (VehicleIF_AlarmEventSend)eventData;

                //    logData.Add(string.Format("\t\tVehicle Number : {0}", sentData.VehicleNumber));
                //    logData.Add(string.Format("\t\tAlarm ID : {0}", sentData.AlarmID));
                //    logData.Add(string.Format("\t\tAlarm Status : {0}", sentData.AlarmStatus));
                //    logData.Add(string.Format("\t\tAlarm Type : {0}", sentData.AlarmType));
                //    logData.Add(string.Format("\t\tAlarm Code : {0}", sentData.AlarmCode));
                //}
                //else if (eventData.GetType() == typeof(VehicleIF_MapDataReply))
                //{
                //    VehicleIF_MapDataReply sentData = (VehicleIF_MapDataReply)eventData;

                //    logData.Add(string.Format("\t\tAcknowledge : {0}", sentData.Acknowledge));
                //    logData.Add(string.Format("\t\tData Type : {0}", sentData.DataType));
                //}
                //else if (eventData.GetType() == typeof(VehicleIF_TeachingDataSend))
                //{
                //    VehicleIF_TeachingDataSend sentData = (VehicleIF_TeachingDataSend)eventData;

                //    logData.Add(string.Format("\t\tVehicle Number : {0}", sentData.VehicleNumber));
                //}
                //else if (eventData.GetType() == typeof(VehicleIF_MapDataRequest))
                //{
                //    VehicleIF_MapDataRequest receivedData = (VehicleIF_MapDataRequest)eventData;

                //    logData.Add(string.Format("\t\tData Type : {0}", receivedData.DataType));
                //}
                ////else if (eventData.GetType() == typeof(VehicleIF_VehicleStatusDataSend))
                ////{
                ////    VehicleIF_VehicleStatusDataSend sentData = (VehicleIF_VehicleStatusDataSend)eventData;

                ////    logData.Add(string.Format("\t\tVehicle Number : {0}", sentData.VehicleNumber));
                ////    logData.Add(string.Format("\t\tIn-Rail Status : {0}", sentData.InRailStatus));
                ////    logData.Add(string.Format("\t\tPower On Status : {0}", sentData.PowerOnStatus));
                ////    logData.Add(string.Format("\t\tAuto Run Status : {0}", sentData.AutoRunStatus));
                ////    logData.Add(string.Format("\t\tPause Status : {0}", sentData.PauseStatus));
                ////    logData.Add(string.Format("\t\tError Status : {0}", sentData.ErrorStatus));
                ////    logData.Add(string.Format("\t\tMoving Status : {0}", sentData.MovingStatus));

                ////    StringBuilder inputStatus = new StringBuilder(string.Format("\t\tInput Status : "));
                ////    StringBuilder outputStatus = new StringBuilder(string.Format("\t\tOutput Status : "));
                ////    for (int i = 0; i < sentData.InputStatus.Length; i++)
                ////    {
                ////        inputStatus.Append((sentData.InputStatus[i] == true) ? "1" : "0");
                ////        outputStatus.Append((sentData.OutputStatus[i] == true) ? "1" : "0");
                ////    }
                ////    logData.Add(inputStatus.ToString());
                ////    logData.Add(outputStatus.ToString());
                ////    logData.Add(string.Format("\t\tCurrent Speed : {0}", sentData.CurrentSpeed));
                ////    logData.Add(string.Format("\t\tBarcode #1 Value : {0}", sentData.BarcodeValue1));
                ////    logData.Add(string.Format("\t\tBarcode #2 Value : {0}", sentData.BarcodeValue2));
                ////}
                //else if (eventData.GetType() == typeof(VehicleIF_FDCDataSend))
                //{
                //    VehicleIF_FDCDataSend receivedData = (VehicleIF_FDCDataSend)eventData;


                //}

                logData.Add(string.Format("Sent {0}", eventData.GetType()));
                logData.AddRange((eventData as IVehicleIF).GetLogData());

                if (_DataLog != null)
                {
                    _DataLog.SetLog(logData.ToArray());
                }
            }
            catch
            {
            }
        }
        public void Exit()
        {
            //_ByteLog.Exit();
            _DataLog.Exit();
            _ErrorLog.Exit();
            //2022.9.28 by..HS
            if(_ClientThread != null)
			{
            _ClientThread.Abort();
        	}
		}
        #endregion
    }
}
