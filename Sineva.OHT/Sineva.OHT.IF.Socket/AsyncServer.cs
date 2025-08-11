using Sineva.OHT.Common;
using Sineva.OHT.LogProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Sineva.OHT.IF.Socket
{
    [Guid("5F476A80-1B05-4FE7-841D-D4E7CA9A45B5")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AsyncServer
    {
        #region Fields
        private const int BUFFER_SIZE = 8192;
        private System.Net.Sockets.Socket _ServerSocket;
        private int _MaxConnectionCount = 10;
        private List<ClientObject> _ClientList = new List<ClientObject>();

        private static ManualResetEvent _AllDone = new ManualResetEvent(false);
        private SocketAsyncEventArgs _AcceptEventArgs;

        private ThreadFunction _ServerThreadFunction;
        private UInt32 _ReplyTimeoutInterval = 5000;
        private bool _ThreadState = true;
        private int _PortNumber = 0;

        //private LogWriter _ByteLog;
        //private LogWriter _DataLog;
        private LogWriter _SystemLog;
        //private Dictionary<int, LogWriter> _ByteLogs = new Dictionary<int, LogWriter>();
        private Dictionary<int, LogWriter> _DataLogs = new Dictionary<int, LogWriter>();

        private bool _RecordSimpleLog = true;

        private string _LogPath = string.Empty;
        private int _LogSplitType = -1;
        private int _MaxLogLines = 0;
        #endregion

        #region Properties
        public System.Net.Sockets.Socket ServerSocket
        {
            get { return _ServerSocket; }
        }
        public List<ClientObject> ClientList
        {
            get { return _ClientList; }
        }
        #endregion

        #region Events
        public delegate void AsyncServerEvent(int portNumber, object sender, object eventData);
        public event AsyncServerEvent ReplyTimeoutHappened;
        public event AsyncServerEvent MessageSent;
        public event AsyncServerEvent MessageReceived;

        public delegate void AsyncServerConnectionEvent(int portNumber, IPEndPoint remoteEndPoint);
        public event AsyncServerConnectionEvent ClientConnect;
        public event AsyncServerConnectionEvent ClientDisconnect;

        public delegate void VehicleConnectionEvent(int vehicleNumber, bool connectionState);
        public event VehicleConnectionEvent VehicleConnectonStateChanged;
        #endregion

        #region Constructors
        public AsyncServer()
        {
            try
            {
                _ServerThreadFunction = new ThreadFunction();
                _ServerThreadFunction.Start(AsyncServerThread);
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        public AsyncServer(int portNo, int maxConnection, string logPath, int logFileSplitType, int maxLogLines, bool simpleLog = true)
        {
            try
            {
                _PortNumber = portNo;
                _MaxConnectionCount = maxConnection;

                _LogPath = logPath;
                _LogSplitType = logFileSplitType;
                _MaxLogLines = maxLogLines;

                //string byteLogName = string.Format("ByteLog[{0}]", portNo);
                string dataLogName = string.Format("DataLog[{0}]", portNo);
                string systemLogName = string.Format("SystemLog[{0}]", portNo);

                //_ByteLog = new LogWriter(_LogPath, "VehicleCommLog", _LogSplitType, _MaxLogLines, byteLogName);
                //_DataLog = new LogWriter(_LogPath, "VehicleCommLog", _LogSplitType, _MaxLogLines, dataLogName);
                _SystemLog = new LogWriter(_LogPath, "VehicleComm_SystemLog", _LogSplitType, _MaxLogLines, systemLogName);

                _RecordSimpleLog = simpleLog;

                _ServerThreadFunction = new ThreadFunction();
                _ServerThreadFunction.Start(AsyncServerThread);
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        #endregion

        #region Methods
        public void BeginListener()
        {
            try
            {
                _ServerSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _PortNumber);
                _ServerSocket.Bind(endPoint);
                _ServerSocket.Listen(10);
                _AcceptEventArgs = new SocketAsyncEventArgs();
                _AcceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
                _ServerSocket.AcceptAsync(_AcceptEventArgs);
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        private void AcceptCallBack(IAsyncResult result)
        {
            try
            {
                _AllDone.Set();

                System.Net.Sockets.Socket listener = result.AsyncState as System.Net.Sockets.Socket;
                System.Net.Sockets.Socket handler = listener.EndAccept(result);

                // Create the state object.  
                StateObject stateObject = new StateObject();
                stateObject.WorkSocket = handler;
                handler.BeginReceive(stateObject.Buffer, 0, stateObject.Buffer.Length, 0, new AsyncCallback(ReadCallBack), stateObject);
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        public void DisconnectAsync()
        {
            try
            {
                SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
                _ServerSocket.DisconnectAsync(eventArgs);

                lock (_ClientList)
                {
                    foreach (ClientObject client in _ClientList)
                    {
                        client.ClientSocket.Disconnect(true);
                        SetConnectionLog((client.ClientSocket.RemoteEndPoint as IPEndPoint), false);
                    }

                    _ClientList.Clear();

                    if (ClientDisconnect != null)
                    {
                        ClientDisconnect(_PortNumber, null);
                    }

                    _ServerSocket.AcceptAsync(eventArgs);
                }
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        private void Disconnect_Complete(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                System.Net.Sockets.Socket serverSocket = e.AcceptSocket;

                if (ClientDisconnect != null)
                {
                    ClientDisconnect(_PortNumber, null);
                }
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        private void ReadCallBack(IAsyncResult result)
        {
            try
            {
                string content = string.Empty;
                StateObject stateObject = result.AsyncState as StateObject;
                System.Net.Sockets.Socket handler = stateObject.WorkSocket;

                int bytesRead = handler.EndReceive(result);
                if (bytesRead > 0)
                {
                    stateObject.ReceivedString.Append(Encoding.ASCII.GetString(stateObject.Buffer, 0, bytesRead));
                    content = stateObject.ReceivedString.ToString();
                }
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                lock (_ClientList)
                {
                    if (_ClientList.Count >= _MaxConnectionCount)
                    {
                        e.AcceptSocket.Disconnect(false);
                    }
                    else
                    {
                        if (e.AcceptSocket != null)
                        {
                            ClientObject client = new ClientObject(e.AcceptSocket);
                            client.UpdateLastWatchdog();

                            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                            client.ReceiveBuffer = new byte[BUFFER_SIZE];
                            args.SetBuffer(client.ReceiveBuffer, 0, client.ReceiveBuffer.Length);
                            args.UserToken = client.ClientSocket;
                            args.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
                            client.ClientSocket.ReceiveAsync(args);

                            client.VehicleConnectionChanged += new ClientObject.ClientObjectEvent(VehicleConnectionChanged);

                            _ClientList.Add(client);

                            SetConnectionLog((client.ClientSocket.RemoteEndPoint as IPEndPoint), true);

                            if (ClientConnect != null)
                            {
                                ClientConnect(_PortNumber, (client.ClientSocket.RemoteEndPoint as IPEndPoint));
                            }
                        }
                    }
                }

                e.AcceptSocket = null;
                _ServerSocket.AcceptAsync(e);
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                lock (_ClientList)
                {
                    System.Net.Sockets.Socket client = (e.UserToken as System.Net.Sockets.Socket);

                    ClientObject clientObject = _ClientList.Where(item => item.ClientSocket == client).SingleOrDefault();

                    if (clientObject != null)
                    {
                        if (clientObject.ClientSocket.Connected && e.BytesTransferred > 0)
                        {
                            byte[] byteData = new byte[e.BytesTransferred];
                            Array.Copy(e.Buffer, 0, byteData, 0, e.BytesTransferred);

                            clientObject.Buffer.AddRange(byteData);

                            int length = 0;
                            while (true)
                            {
                                if (clientObject.Buffer.Count < 3) break;
                                byte[] temp = new byte[2];
                                temp[0] = clientObject.Buffer[1];
                                temp[1] = clientObject.Buffer[2];

                                length = (int)BitConverter.ToUInt16(temp, 0);

                                if (length > clientObject.Buffer.Count) break;

                                temp = new byte[length];
                                clientObject.Buffer.CopyTo(0, temp, 0, length);

                                //SetReceivedByteLog((clientObject.ClientSocket.RemoteEndPoint as IPEndPoint), temp);

                                ProcessReceivedMessage(clientObject.ClientSocket.RemoteEndPoint as IPEndPoint, temp);

                                clientObject.Buffer.RemoveRange(0, length);
                            }

                            clientObject.ClientSocket.ReceiveAsync(e);
                        }
                        else
                        {
                            clientObject.ClientSocket.Disconnect(true);

                            IPEndPoint clientPoint = clientObject.ClientSocket.RemoteEndPoint as IPEndPoint;

                            SetConnectionLog(clientPoint, false);

                            if (ClientDisconnect != null)
                            {
                                ClientDisconnect(_PortNumber, clientPoint);
                            }

                            _ClientList.Remove(clientObject);

                            //if (_ClientList.Count < _MaxConnectionCount)
                            //{
                            //    _ServerSocket.AcceptAsync(e);
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        private bool BeginSend(IPEndPoint ipEndPoint, byte[] message)
        {
            try
            {
                //lock(_ClientList)
                {
                    ClientObject client = _ClientList.Where(item => (item.ClientSocket.RemoteEndPoint as IPEndPoint) == ipEndPoint).SingleOrDefault();

                    if (client != null)
                    {
                        CallbackObject state = new CallbackObject();
                        state.Client = client;
                        state.Stream = message;

                        byte messageCode = message[3];
                        byte systemByte = message[4];

                        if ((messageCode % 0x2) == 0x1)
                        {
                            state.Client.SentMessage(systemByte);
                        }
                        //SetSentByteLog((state.Client.ClientSocket.RemoteEndPoint as IPEndPoint), state.Stream);

                        client.ClientSocket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(SendCallBack), state);

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
                return false;
            }
        }
        private void SendCallBack(IAsyncResult result)
        {
            try
            {
                CallbackObject state = (result.AsyncState as CallbackObject);
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        private void VehicleConnectionChanged(object sender, object eventData)
        {
            try
            {
                if (VehicleConnectonStateChanged != null)
                {
                    VehicleConnectonStateChanged((sender as ClientObject).VehicleNo, Convert.ToBoolean(eventData));
                }
            }
            catch
            {
            }
        }
        private void AsyncServerThread()
        {
            List<byte> timeoutHappenedList = new List<byte>();
            string lastError = string.Empty;
            while (_ThreadState)
            {
                try
                {
                    if (_ClientList.Count == 0) continue;

                    lock (_ClientList)
                    {
                        foreach (ClientObject client in _ClientList)
                        {
                            timeoutHappenedList.Clear();
                            lock (client.SentMessageList)
                            {
                                foreach (byte systemByte in client.SentMessageListKey())
                                {
                                    UInt32 progressedTime = client.GetMessageProgressTime(systemByte);
                                    if (progressedTime > _ReplyTimeoutInterval)
                                    {
                                        timeoutHappenedList.Add(systemByte);

                                        SetReplyTimeoutLog((client.ClientSocket.RemoteEndPoint as IPEndPoint), systemByte);

                                        if (ReplyTimeoutHappened != null)
                                        {
                                            ReplyTimeoutHappened(_PortNumber, client, systemByte);
                                        }
                                    }
                                }
                            }

                            foreach (byte systemByte in timeoutHappenedList)
                            {
                                client.ReceivedReply(systemByte);
                            }

                            if (client.GetCurrentWatchdogProgressTime() > _ReplyTimeoutInterval)
                            {
                                if (client.WatchdogErrorState == false) client.WatchdogErrorState = true;
                            }
                            else
                            {
                                if (client.WatchdogErrorState == true) client.WatchdogErrorState = false;
                            }
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

        #region Methods - Receive/Send Interface Message
        public void BroadcastIFMessage(VehicleIF messageObject)
        {
            try
            {
                foreach (ClientObject client in _ClientList)
                {
                    messageObject.VehicleNumber = client.VehicleNo;
                    SendIFMessage(client.ClientSocket.RemoteEndPoint as IPEndPoint, messageObject);
                }
            }
            catch
            {
            }
        }
        public void SendIFMessage(IPEndPoint ipEndPoint, VehicleIF messageObject)
        {
            try
            {
                //lock(_ClientList)
                {
                    ClientObject client = _ClientList.Where(item => (item.ClientSocket.RemoteEndPoint as IPEndPoint) == ipEndPoint).SingleOrDefault();

                    if (client != null)
                    {
                        if (((byte)messageObject.MessageCode) % 0x2 == 0x1)
                        {
                            messageObject.SetSystemByte(client.GetCurrentSystemByte());
                        }

                        byte[] sendStream = ((IVehicleIF)messageObject).GetStreamFromDataOrNull();

                        if (sendStream != null)
                        {
                            BeginSend((client.ClientSocket.RemoteEndPoint as IPEndPoint), sendStream);

                            SetSentDataLog((client.ClientSocket.RemoteEndPoint as IPEndPoint), messageObject);

                            if (MessageSent != null)
                            {
                                MessageSent(_PortNumber, ipEndPoint, messageObject);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        public void SendIFMessage(string ipAddress, VehicleIF messageObject)
        {
            try
            {
                //lock (_ClientList)
                {
                    ClientObject client = _ClientList.Where(item => (item.ClientSocket.RemoteEndPoint as IPEndPoint).Address.ToString() == ipAddress).SingleOrDefault();

                    if (client != null)
                    {
                        if (((byte)messageObject.MessageCode) % 0x2 == 0x1)
                        {
                            messageObject.SetSystemByte(client.GetCurrentSystemByte());
                        }

                        byte[] sendStream = ((IVehicleIF)messageObject).GetStreamFromDataOrNull();

                        if (sendStream != null)
                        {
                            BeginSend((client.ClientSocket.RemoteEndPoint as IPEndPoint), sendStream);

                            SetSentDataLog((client.ClientSocket.RemoteEndPoint as IPEndPoint), messageObject);

                            if (MessageSent != null)
                            {
                                MessageSent(_PortNumber, (client.ClientSocket.RemoteEndPoint as IPEndPoint), messageObject);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorLog(ex.ToString());
            }
        }
        public void SendIFMessage(int vehicleNo, VehicleIF messageObject)
        {
            try
            {
                //lock (_ClientList)
                {
                    ClientObject client = _ClientList.Where(item => item.VehicleNo == vehicleNo).SingleOrDefault();

                    if (client != null)
                    {
                        if (((byte)messageObject.MessageCode) % 0x2 == 0x1)
                        {
                            messageObject.SetSystemByte(client.GetCurrentSystemByte());
                        }

                        byte[] sendStream = ((IVehicleIF)messageObject).GetStreamFromDataOrNull();

                        if (sendStream != null)
                        {
                            BeginSend((client.ClientSocket.RemoteEndPoint as IPEndPoint), sendStream);

                            if (messageObject.MessageCode != IFMessage.StatusDataReply)
                            {
                                SetSentDataLog((client.ClientSocket.RemoteEndPoint as IPEndPoint), messageObject);
                            }

                            if (MessageSent != null)
                            {
                                MessageSent(_PortNumber, (client.ClientSocket.RemoteEndPoint as IPEndPoint), messageObject);
                            }

                            //if (messageObject.GetType() == typeof(VehicleIF_CommandSend))
                            //{
                            //    (messageObject as VehicleIF_CommandSend).SystemByte;
                            //    (messageObject as VehicleIF_CommandSend).Command;
                            //    (messageObject as VehicleIF_CommandSend).TransferCommandID;
                            //}
                        }
                    }
                }
            }
            catch
            {
            }
        }
        private void ProcessReceivedMessage(IPEndPoint ipEndPoint, byte[] receivedStream)
        {
            try
            {
                //lock (_ClientList)
                {
                    ClientObject client = _ClientList.Where(item => (item.ClientSocket.RemoteEndPoint as IPEndPoint) == ipEndPoint).SingleOrDefault();

                    if (client != null)
                    {
                        if (receivedStream.Length < 5) return;

                        IFMessage messageCode = (IFMessage)receivedStream[3];
                        byte systemByte = receivedStream[4];

                        VehicleIF receivedMessage = null;

                        switch (messageCode)
                        {
                            case IFMessage.CommandReply:
                                {
                                    client.ReceivedReply(systemByte);
                                    receivedMessage = new VehicleIF_CommandReply();
                                    (receivedMessage as IVehicleIF).SetDataFromStream(receivedStream);
                                }
                                break;
                            case IFMessage.EventSend:
                                {
                                    receivedMessage = new VehicleIF_EventSend();
                                    (receivedMessage as IVehicleIF).SetDataFromStream(receivedStream);
                                }
                                break;
                            case IFMessage.AlarmEventSend:
                                {
                                    receivedMessage = new VehicleIF_AlarmEventSend();
                                    (receivedMessage as IVehicleIF).SetDataFromStream(receivedStream);
                                }
                                break;
                            case IFMessage.MapDataReply:
                                {
                                    client.ReceivedReply(systemByte);
                                    receivedMessage = new VehicleIF_MapDataReply();
                                    (receivedMessage as IVehicleIF).SetDataFromStream(receivedStream);
                                }
                                break;
                            case IFMessage.TeachingDataSend:
                                {
                                    receivedMessage = new VehicleIF_TeachingDataSend();
                                    (receivedMessage as IVehicleIF).SetDataFromStream(receivedStream);
                                }
                                break;
                            case IFMessage.DataVersionReply:
                                {
                                    client.ReceivedReply(systemByte);
                                    receivedMessage = new VehicleIF_DataVersionReply();
                                    (receivedMessage as IVehicleIF).SetDataFromStream(receivedStream);
                                }
                                break;
                            case IFMessage.MapDataRequest:
                                {
                                    receivedMessage = new VehicleIF_MapDataRequest();
                                    (receivedMessage as IVehicleIF).SetDataFromStream(receivedStream);
                                }
                                break;
                            case IFMessage.StatusDataSend:
                                {
                                    client.UpdateLastWatchdog();
                                    receivedMessage = new VehicleIF_VehicleStatusDataSend();
                                    (receivedMessage as IVehicleIF).SetDataFromStream(receivedStream);
                                }
                                break;
                            case IFMessage.FDCDataSend:
                                {
                                    receivedMessage = new VehicleIF_FDCDataSend();
                                    (receivedMessage as IVehicleIF).SetDataFromStream(receivedStream);
                                }
                                break;
                            case IFMessage.OperationConfigDataReply:
                                {
                                    client.ReceivedReply(systemByte);
                                    receivedMessage = new VehicleIF_OperationConfigDataReply();
                                    (receivedMessage as IVehicleIF).SetDataFromStream(receivedStream);
                                }
                                break;
                            case IFMessage.AutoTeachingResultSend:
                                {
                                    receivedMessage = new VehicleIF_AutoTeachingResultSend();
                                    (receivedMessage as IVehicleIF).SetDataFromStream(receivedStream);
                                }
                                break;
                            case IFMessage.UserLoginRequest:
                                {
                                    receivedMessage = new VehicleIF_UserLoginRequest();
                                    (receivedMessage as IVehicleIF).SetDataFromStream(receivedStream);
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
                            client.VehicleNo = (receivedMessage as VehicleIF).VehicleNumber;
                        }

                        if ((receivedMessage != null) && (messageCode != IFMessage.StatusDataSend))
                        //if (receivedMessage != null)
                        {
                            SetReceivedDataLog(ipEndPoint, receivedMessage);
                        }

                        if ((receivedMessage != null) && (MessageReceived != null))
                        {
                            MessageReceived(_PortNumber, ipEndPoint, receivedMessage);
                        }

                        if (client.VehicleConnected == false)
                        {
                            client.VehicleConnected = true;
                        }
                    }
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

                if (_SystemLog != null) _SystemLog.SetLog(logData.ToArray());
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
                    logData.Add(string.Format("Client Connected : {0}", ipEndPoint.ToString()));
                }
                else
                {
                    logData.Add(string.Format("Client Disconnected : {0}", ipEndPoint.ToString()));
                }

                //if (_ByteLog != null) _ByteLog.SetLog(logData.ToArray());
                //if (_DataLog != null) _DataLog.SetLog(logData.ToArray());
                if (_SystemLog != null) _SystemLog.SetLog(logData.ToArray());
            }
            catch
            {
            }
        }
        private void SetReplyTimeoutLog(IPEndPoint ipEndPoint, object eventData)
        {
            try
            {
                ClientObject client = _ClientList.Where(item => (item.ClientSocket.RemoteEndPoint as IPEndPoint) == ipEndPoint).SingleOrDefault();

                if (client != null)
                {

                    List<string> logData = new List<string>();

                    logData.Add(string.Format("Reply Timeout : {0}", ipEndPoint.ToString()));
                    logData.Add(string.Format("\t\tIP Address : {0}", ipEndPoint.ToString()));
                    logData.Add(string.Format("\t\tSystem Byte : {0}", (byte)eventData));

                    //if (_ByteLog != null) _ByteLog.SetLog(logData.ToArray());
                    //if (_DataLog != null) _DataLog.SetLog(logData.ToArray());

                    int vehicleNo = client.VehicleNo;

                    if (_DataLogs.ContainsKey(vehicleNo) == false)
                    {
                        string logPath = string.Format("VehicleComm_DataLog[{0:000}]", vehicleNo);
                        string logName = string.Format("DataLog[{0:000}].log", vehicleNo);
                        _DataLogs.Add(vehicleNo, new LogWriter(_LogPath, logPath, _LogSplitType, _MaxLogLines, logName));
                    }

                    if (_DataLogs[vehicleNo] != null)
                    {
                        _DataLogs[vehicleNo].SetLog(logData.ToArray());
                    }
                }
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

                //if (_RecordSimpleLog == false)
                //{
                //    int splitCount = 0;

                //    for (int byteOffset = 0; byteOffset < byteStream.Length; byteOffset++)
                //    {
                //        temp += string.Format("{0:X2}  ", byteStream[byteOffset]);
                //        splitCount += 1;

                //        if (splitCount == 10)
                //        {
                //            logData.Add(temp);
                //            temp = string.Format("            ");
                //            splitCount = 0;
                //        }
                //    }
                //}

                //if (temp.Trim() != string.Empty)
                //{
                //    logData.Add(temp);
                //}

                //if (_ByteLog != null) _ByteLog.SetLog(logData.ToArray());
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

                //if (_RecordSimpleLog == false)
                //{
                //    int splitCount = 0;

                //    for (int byteOffset = 0; byteOffset < byteStream.Length; byteOffset++)
                //    {
                //        temp += string.Format("{0:X2}  ", byteStream[byteOffset]);
                //        splitCount += 1;

                //        if (splitCount == 10)
                //        {
                //            logData.Add(temp);
                //            temp = string.Format("            ");
                //            splitCount = 0;
                //        }
                //    }
                //}

                //if (temp.Trim() != string.Empty)
                //{
                //    logData.Add(temp);
                //}

                //if (_ByteLog != null) _ByteLog.SetLog(logData.ToArray());
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

                logData.Add(string.Format("{0} {1} - {2} (SystemByte - {3})", (isReceived == true) ? "Received" : "Sent", eventData.GetType(), ipEndPoint.ToString(), eventData.SystemByte));

                return logData;
            }
            catch
            {
                return new List<string>();
            }
        }
        private void SetSentDataLog(IPEndPoint ipEndPoint, VehicleIF eventData)
        {
            try
            {
                List<string> logData = new List<string>();

                int vehicleNo = eventData.VehicleNumber;

                if (_DataLogs.ContainsKey(vehicleNo) == false)
                {
                    string logPath = string.Format("VehicleComm_DataLog[{0:000}]", vehicleNo);
                    string logName = string.Format("DataLog[{0:000}].log", vehicleNo);
                    _DataLogs.Add(vehicleNo, new LogWriter(_LogPath, logPath, _LogSplitType, _MaxLogLines, logName));
                }

                //Add Header Log String
                logData.AddRange(GetHeaderLogData(ipEndPoint, eventData, false));

                if (_RecordSimpleLog == false)
                {
                    logData.AddRange((eventData as IVehicleIF).GetLogData());
                }

                //if (_DataLog != null) _DataLog.SetLog(logData.ToArray());
                if (_DataLogs.ContainsKey(vehicleNo) == true)
                {
                    if (_DataLogs[vehicleNo] != null)
                    {
                        _DataLogs[vehicleNo].SetLog(logData.ToArray());
                    }
                }
            }
            catch
            {
            }
        }
        private void SetReceivedDataLog(IPEndPoint ipEndPoint, VehicleIF eventData)
        {
            try
            {
                List<string> logData = new List<string>();

                int vehicleNo = eventData.VehicleNumber;

                if (_DataLogs.ContainsKey(vehicleNo) == false)
                {
                    string logPath = string.Format("VehicleComm_DataLog[{0:000}]", vehicleNo);
                    string logName = string.Format("DataLog[{0:000}].log", vehicleNo);
                    _DataLogs.Add(vehicleNo, new LogWriter(_LogPath, logPath, _LogSplitType, _MaxLogLines, logName));
                }

                //Add Header Log String
                logData.AddRange(GetHeaderLogData(ipEndPoint, eventData, true));

                if (_RecordSimpleLog == false)
                {
                    logData.AddRange((eventData as IVehicleIF).GetLogData());
                }

                //if (_DataLog != null) _DataLog.SetLog(logData.ToArray());
                if (_DataLogs.ContainsKey(vehicleNo) == true)
                {
                    if (_DataLogs[vehicleNo] != null)
                    {
                        _DataLogs[vehicleNo].SetLog(logData.ToArray());
                    }
                }
            }
            catch
            {
            }
        }
        #endregion
    }
}
