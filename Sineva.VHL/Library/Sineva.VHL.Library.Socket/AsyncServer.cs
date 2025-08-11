
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Sineva.VHL.Library.OcsSocket
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

                                if ((MessageReceived != null))
                                {
                                    MessageReceived(_PortNumber, clientObject.ClientSocket.RemoteEndPoint as IPEndPoint, temp);
                                }

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

        #region Methods - Log Write
        private void SetErrorLog(string errorText)
        {
            try
            {
                List<string> logData = new List<string>();

                logData.Add(errorText);
                OcsCommLog.WriteLog(logData.ToArray());
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
                OcsCommLog.WriteLog(logData.ToArray());
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

                    int vehicleNo = client.VehicleNo;
                    OcsCommLog.WriteLog(logData.ToArray());
                }
            }
            catch
            {
            }
        }
        #endregion
    }
}
