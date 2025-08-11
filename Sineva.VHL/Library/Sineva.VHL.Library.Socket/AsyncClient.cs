
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Sineva.VHL.Library.OcsSocket
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
                _ServerAddress = serverAddress;
                _ServerPort = serverPort;

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

                        if ((MessageReceived != null))
                        {
                            MessageReceived(_ServerSocket.RemoteEndPoint as IPEndPoint, temp);
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
                if (MessageSent != null)
                {
                    MessageSent(_ServerSocket.RemoteEndPoint as IPEndPoint, stream);
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

        #region Methods - Log Write
        private void SetErrorLog(string errorText)
        {
            try
            {
                OcsCommLog.WriteLog(errorText);
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
                List<string> logData = new List<string>();

                logData.Add(string.Format("Reply Timeout : {0}", (byte)eventData));
                logData.Add(string.Format("\t\tIP Address : {0}", ipEndPoint.ToString()));
                logData.Add(string.Format("\t\tSystem Byte : {0}", (byte)eventData));

                OcsCommLog.WriteLog(logData.ToArray());
            }
            catch
            {
            }
        }
        public void Exit()
        {
            //2022.9.28 by..HS
            if (_ClientThread != null)
            {
                _ClientThread.Abort();
            }
        }
        #endregion
    }
}
