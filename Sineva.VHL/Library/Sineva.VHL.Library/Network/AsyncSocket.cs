
/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.16 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Interop;

namespace Sineva.VHL.Library
{
	public class StateObject
	{
        private const int BUFFER_SIZE = 327680;

        // Client socket.
        private Socket worker = null;
        // Size of receive buffer.
        private byte[] buffer;
        // Received data string.
        private string msg;

        public StateObject(Socket worker)
        {
            this.worker = worker;
            this.buffer = new byte[BUFFER_SIZE];
            msg = string.Empty;
        }

        public Socket Worker
        {
            get { return this.worker; }
            set { this.worker = value; }
        }

        public byte[] Buffer
        {
            get { return this.buffer; }
            set { this.buffer = value; }
        }

        public int BufferSize
        {
            get { return BUFFER_SIZE; }
        }

        public string Msg
        {
            get { return this.msg; }
            set { this.msg = value; }
        }
	}

    /// <summary>
    /// 비동기 소켓에서 발생한 에러 처리를 위한 이벤트 Argument Class
    /// </summary>
    public class AsyncSocketErrorEventArgs : EventArgs
    {
        private readonly Exception exception;
        private readonly int id = 0;

        public AsyncSocketErrorEventArgs(int id, Exception exception)
        {
            this.id = id;
            this.exception = exception;
        }

        public Exception AsyncSocketException
        {
            get { return this.exception; }
        }

        public int ID
        {
            get { return this.id; }
        }
    }

    /// <summary>
    /// 비동기 소켓의 연결 및 연결해제 이벤트 처리를 위한 Argument Class
    /// </summary>
    public class AsyncSocketConnectionEventArgs : EventArgs
    {
        private readonly int id = 0;

        public AsyncSocketConnectionEventArgs(int id)
        {
            this.id = id;
        }

        public int ID
        {
            get { return this.id; }
        }
    }

    /// <summary>
    /// 비동기 소캣의 데이터 전송 이벤트 처리를 위한 Argument Class
    /// </summary>
    public class AsyncSocketSendEventArgs : EventArgs
    {
        private readonly int id = 0;
        private readonly int sendBytes;
        private readonly string sendData;
        private readonly byte[] sendBuffer;

        public AsyncSocketSendEventArgs(int id, int sendBytes, string Data, byte[] sendBuffer)
        {
            this.id = id;
            this.sendBytes = sendBytes;
            this.sendData = Data;
            this.sendBuffer = sendBuffer;
        }

        public int SendBytes
        {
            get { return this.sendBytes; }
        }

        public string SendData
        {
            get { return sendData; }
        } 

        public byte[] SendBuffer
        {
            get { return sendBuffer; }
        }

        public int ID
        {
            get { return this.id; }
        }
    }

    /// <summary>
    /// 비동기 소켓의 데이터 수신 이벤트 처리를 위한 Argument Class
    /// </summary>
    public class AsyncSocketReceiveEventArgs : EventArgs
    {
        private readonly int id = 0;
        private readonly int receiveBytes;
        private readonly string receiveData;
        private readonly byte[] receiveBuffer;

        public AsyncSocketReceiveEventArgs(int id, int receiveBytes, string Data, byte[] receiveBuffer)
        {
            this.id = id;
            this.receiveBytes = receiveBytes;
            this.receiveData = Data;
            this.receiveBuffer = receiveBuffer;
        }

        public int ReceiveBytes
        {
            get { return this.receiveBytes; }
        }

        public string ReceiveData
        {
            get { return this.receiveData; }
        }

        public byte[] ReceiveBuffer
        {
            get { return this.receiveBuffer; }
        }

        public int ID
        {
            get { return this.id; }
        }
    }

    /// <summary>
    /// 비동기 서버의 Accept 이벤트를 위한 Argument Class
    /// </summary>
    public class AsyncSocketAcceptEventArgs : EventArgs
    {
        private readonly Socket conn;

        public AsyncSocketAcceptEventArgs(Socket conn)
        {
            this.conn = conn;
        }

        public Socket Worker
        {
            get { return this.conn; }
        }
    }

    ///
    /// delegate 정의
    /// 
    public delegate void AsyncSocketErrorEventHandler(object sender, AsyncSocketErrorEventArgs e);
    public delegate void AsyncSocketConnectEventHandler(object sender, AsyncSocketConnectionEventArgs e);
    public delegate void AsyncSocketCloseEventHandler(object sender, AsyncSocketConnectionEventArgs e);
    public delegate void AsyncSocketSendEventHandler(object sender, AsyncSocketSendEventArgs e);
    public delegate void AsyncSocketReceiveEventHandler(object sender, AsyncSocketReceiveEventArgs e);
    public delegate void AsyncSocketAcceptEventHandler(object sender, AsyncSocketAcceptEventArgs e);

    public class AsyncSocketClass
    {
        protected int id;
        private Encoding m_Encoding = Encoding.Default;
        private IPAddress m_IpAddress;
        private int m_PortNo = 5010;
        private ProtocolType m_PortocolType = ProtocolType.Tcp;

        // Event Handler
        public event AsyncSocketErrorEventHandler OnError;
        public event AsyncSocketConnectEventHandler OnConnet;
        public event AsyncSocketCloseEventHandler OnClose;
        public event AsyncSocketSendEventHandler OnSend;
        public event AsyncSocketReceiveEventHandler OnReceive;
        public event AsyncSocketAcceptEventHandler OnAccept;

        protected static ManualResetEvent m_LockAsyncConnect = new ManualResetEvent(false);
        protected static ManualResetEvent m_LockAsyncDisconnect = new ManualResetEvent(false);
        protected static ManualResetEvent m_LockAsyncAccept = new ManualResetEvent(false);

        public AsyncSocketClass()
        {
            this.id = -1;
        }

        public AsyncSocketClass(int id)
        {
            this.id = id;
        }

        public int ID
        {
            get { return this.id; }
        }
        public IPAddress IpAddress
        {
            get { return m_IpAddress; }
            set { m_IpAddress = value; }
        }
        public int PortNo
        {
            get { return m_PortNo; }
            set { m_PortNo = value; }
        }

        public void SetEncoding(Encoding code)
        {
            m_Encoding = code;
        }

        public Encoding _Encoding()
        {
            return m_Encoding;
        }

        public void SetProtocolType(ProtocolType protocol)
        {
            m_PortocolType = protocol;
        }

        public ProtocolType _ProtocolType()
        {
            return m_PortocolType;
        }

        protected virtual void ErrorOccured(AsyncSocketErrorEventArgs e, int error_code)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            ExceptionLog.WriteLog(method, String.Format("{0},ID:{1}, ErrorCode={2}", e.ToString(),e.ID, error_code));

            AsyncSocketErrorEventHandler handler = OnError;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void Connected(AsyncSocketConnectionEventArgs e)
        {
            AsyncSocketConnectEventHandler handler = OnConnet;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Closed(AsyncSocketConnectionEventArgs e)
        {
            AsyncSocketCloseEventHandler handler = OnClose;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Sent(AsyncSocketSendEventArgs e)
        {
            AsyncSocketSendEventHandler handler = OnSend;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Received(AsyncSocketReceiveEventArgs e)
        {
            AsyncSocketReceiveEventHandler handler = OnReceive;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Accepted(AsyncSocketAcceptEventArgs e)
        {
            AsyncSocketAcceptEventHandler handler = OnAccept;

            if (handler != null)
                handler(this, e);
        }

    } // end of class AsyncSocketClass

    /// <summary>
    /// 비동기 소켓
    /// </summary>
    public class AsyncSocketClient : AsyncSocketClass
    {
        // connection socket
        private Socket m_ClientSocket = null;
        private object m_SendData = null;
        private object m_SendBuffer = null;

        public AsyncSocketClient(int id) // Client Socket 생성자
        {
            this.id = id;
        }

        public AsyncSocketClient(int id, Socket conn) // Server Socket 생성자
        {
            this.id = id;
            this.m_ClientSocket = conn;
        }

        public Socket ClientSocket
        {
            get { return this.m_ClientSocket; }
            set { this.m_ClientSocket = value; }
        }

        /// <summary>
        /// 연결을 시도한다.
        /// </summary>
        /// <param name="hostAddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Connect(string hostAddress, int port)
        {
            try
            {
                m_LockAsyncConnect.Reset();

                if (m_ClientSocket != null) m_ClientSocket.Close();

                IPAddress[] ips = Dns.GetHostAddresses(hostAddress);
                IpAddress = ips[0];
                PortNo = port;

                IPEndPoint remoteEP = new IPEndPoint(IpAddress, PortNo);
                ProtocolType protocol = _ProtocolType() == ProtocolType.Tcp ?  ProtocolType.Tcp : ProtocolType.Udp;
                SocketType sockettype = _ProtocolType() == ProtocolType.Tcp ? SocketType.Stream : SocketType.Dgram;
                Socket client = new Socket(AddressFamily.InterNetwork, sockettype, protocol);

                client.BeginConnect(remoteEP, new AsyncCallback(OnConnectCallback), client);

                m_LockAsyncConnect.WaitOne();
            }
            catch (SocketException e)
            {
                m_LockAsyncConnect.Set();
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), 0);
                return false;
            }

            return true;

        }

        /// <summary>
        /// 연결 요청 처리 콜백 함수
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnectCallback(IAsyncResult ar)
        {
            try
            {
                m_LockAsyncConnect.Set();

                Socket client = (Socket)ar.AsyncState;
                if (client == null) return;
                if (client.Connected == false) return;
                
                // 보류 중인 연결을 완성한다.
                client.EndConnect(ar);

                m_ClientSocket = client;

                // 연결에 성공하였다면, 데이터 수신을 대기한다.
                Receive();

                // 연결 성공 이벤트를 날린다.
                Connected(new AsyncSocketConnectionEventArgs(this.id));
            }
            catch (SocketException e)
            {
                m_LockAsyncConnect.Set();
                if (IsSocketClosed(e.SocketErrorCode))
                {
                    Closed(new AsyncSocketConnectionEventArgs(this.id));
                }
                else ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), (int)e.SocketErrorCode);
            }
            catch (Exception e)
            {
                m_LockAsyncConnect.Set();
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), 0);
            }
        }
        /// <summary>
        /// 데이터 수신을 비동기적으로 처리
        /// </summary>
        public void Receive()
        {
            try
            {
                StateObject so = new StateObject(m_ClientSocket);
                if (so == null) return;

                so.Worker.BeginReceive(so.Buffer, 0, so.BufferSize, 0, new AsyncCallback(OnReceiveCallBack), so);
            }
            catch (SocketException e)
            {
                if (IsSocketClosed(e.SocketErrorCode))
                {
                    Closed(new AsyncSocketConnectionEventArgs(this.id));
                }
                else ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), (int)e.SocketErrorCode);
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), 0);
            }
        }

        /// <summary>
        /// 데이터 수신 처리 콜백 함수
        /// </summary>
        /// <param name="ar"></param>
        private void OnReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                StateObject so = (StateObject)ar.AsyncState;
                if (so == null) return;

                int bytesRead = so.Worker.EndReceive(ar);

                // 데이터 수신 이벤트를 처리한다.
                if (bytesRead > 0)
                {
                    string msg = _Encoding().GetString(so.Buffer, 0, bytesRead);
                    AsyncSocketReceiveEventArgs rev = new AsyncSocketReceiveEventArgs(this.id, bytesRead, msg, so.Buffer);
                    Received(rev);
                }

                // 다음 읽을 데이터를 처리한다.
                Receive();
            }
            catch (SocketException e)
            {
                if (IsSocketClosed(e.SocketErrorCode))
                {
                    Closed(new AsyncSocketConnectionEventArgs(this.id));
                }
                else ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), (int)e.SocketErrorCode);
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), 0);
            }
        }

        /// <summary>
        /// 데이터 송신을 비동기적으로 처리
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public bool Send(string msg)
        {
            try
            {
                Socket client = m_ClientSocket;
                if (client == null) return false;

                byte[] buffer = _Encoding().GetBytes(msg.ToCharArray());
                m_SendData = msg;
                m_SendBuffer = buffer;

                client.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(OnSendCallBack), client);
            }
            catch (SocketException e)
            {
                if (IsSocketClosed(e.SocketErrorCode))
                {
                    Closed(new AsyncSocketConnectionEventArgs(this.id));
                }
                else ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), (int)e.SocketErrorCode);
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), 0);
            }

            return true;
        }
        public bool Send(byte[] data)
        {
            try
            {
                Socket client = m_ClientSocket;
                if (client == null) return false;

                string str = _Encoding().GetString(data, 0, data.Length);
                m_SendData = str;
                m_SendBuffer = data;

                client.BeginSend(data, 0, data.Length, 0, new AsyncCallback(OnSendCallBack), client);
            }
            catch (SocketException e)
            {
                if (IsSocketClosed(e.SocketErrorCode))
                {
                    Closed(new AsyncSocketConnectionEventArgs(this.id));
                }
                else ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), (int)e.SocketErrorCode);
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), 0);
            }
            return true;

        }
        /// <summary>
        /// 데이터 송신 처리 콜백 함수
        /// </summary>
        /// <param name="ar"></param>
        private void OnSendCallBack(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                if (client == null) return;

                int bytesWritten = client.EndSend(ar);

                AsyncSocketSendEventArgs sev = new AsyncSocketSendEventArgs(this.id, bytesWritten, (string)m_SendData, (byte[])m_SendBuffer);

                Sent(sev);
            }
            catch (SocketException e)
            {
                if (IsSocketClosed(e.SocketErrorCode))
                {
                    Closed(new AsyncSocketConnectionEventArgs(this.id));
                }
                else ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), (int)e.SocketErrorCode);
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), 0);
            }
        }

        /// <summary>
        /// 소켓 연결을 비동기적으로 종료
        /// </summary>
        public void Close()
        {
            try
            {
                Socket client = m_ClientSocket;
                if (client == null) return;

                //client.Shutdown(SocketShutdown.Both);
                client.BeginDisconnect(false, new AsyncCallback(OnCloseCallBack), client);
            }
            catch (SocketException e)
            {
                if (IsSocketClosed(e.SocketErrorCode))
                {
                    Closed(new AsyncSocketConnectionEventArgs(this.id));
                }
                else ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), (int)e.SocketErrorCode);
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), 0);
            }
        }

        /// <summary>
        /// 소켓 연결 종료를 처리하는 콜백 함수
        /// </summary>
        /// <param name="ar"></param>
        private void OnCloseCallBack(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                if (client == null) return;

                client.EndDisconnect(ar);
                client.Close();

                Closed(new AsyncSocketConnectionEventArgs(this.id));
            }
            catch (SocketException e)
            {
                if (IsSocketClosed(e.SocketErrorCode))
                {
                    Closed(new AsyncSocketConnectionEventArgs(this.id));
                }
                else ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), (int)e.SocketErrorCode);
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), 0);
            }
        }

        private bool IsSocketClosed(SocketError error_code)
        {
            bool rv = false;
            rv |= error_code == SocketError.NotConnected;
            rv |= error_code == SocketError.ConnectionReset;
            rv |= error_code == SocketError.ConnectionRefused;
            rv |= error_code == SocketError.Shutdown;
            rv |= error_code == SocketError.HostUnreachable;
            rv |= error_code == SocketError.HostDown;
            rv |= error_code == SocketError.NotInitialized;
            return rv;
        }

    } // end of class AsyncSocketClient

    /// <summary>
    /// 비동기 방식의 서버 
    /// </summary>
    public class AsyncSocketServer : AsyncSocketClass
    {
        private const int backLog = 100;
        private Socket listener;

        public AsyncSocketServer(int port)
        {
            PortNo = port;
        }

        public void Listen()
        {
            try
            {
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(new IPEndPoint(IPAddress.Any, PortNo));
                listener.Listen(backLog);

                StartAccept();
            }
            catch (SocketException e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), (int)e.SocketErrorCode);
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), 0);
            }
        }

        /// <summary>
        /// Client의 접속을 비동기적으로 대기한다.
        /// </summary>
        /// <returns></returns>
        private void StartAccept()
        {
            try
            {
                listener.BeginAccept(new AsyncCallback(OnListenCallBack), listener);
            }
            catch (SocketException e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), (int)e.SocketErrorCode);
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), 0);
            }
        }

        /// <summary>
        /// Client의 비동기 접속을 처리한다.
        /// </summary>
        /// <param name="ar"></param>
        private void OnListenCallBack(IAsyncResult ar)
        {
            try
            {
                Socket listener = (Socket)ar.AsyncState;
                Socket worker = listener.EndAccept(ar);

                // Client를 Accept 했다고 Event를 발생시킨다.
                Accepted(new AsyncSocketAcceptEventArgs(worker));

                // 다시 새로운 클라이언트의 접속을 기다린다.
                StartAccept();
            }
            catch (SocketException e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), (int)e.SocketErrorCode);
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), 0);
            }
        }

        public void Stop()
        {
            try
            {
                if (listener != null)
                {
                    if (listener.IsBound)
                        listener.Close(100);
                }
            }
            catch (SocketException e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), (int)e.SocketErrorCode);
            }
            catch (System.Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(this.id, e), 0);
            }
        }

    } // end of class AsyncSocketServer

    public class AsyncSocketManager : IDisposable
    {
        #region Fields
        private bool m_Initialize = false;
        private bool m_Connected = false;
        private ConnectionMode m_ConntionMode = ConnectionMode.Client;
        private Encoding m_Encoding = Encoding.Default;
        private ProtocolType m_PortocolType = ProtocolType.Tcp;

        // Client
        private AsyncSocketClient m_Client = null;
        private string m_ServerIpAddress = string.Format("127.0.0.1");
        private ushort m_ServerPortNo = 7456;

        // Server
        private AsyncSocketServer m_Server = null;
        private List<AsyncSocketClient> m_ClientLists;
        private int m_ClientId = 0; //Client가 접속 할때마다 증가
        private const int m_ClientMaxNo = 100;

        //private XLog Log = new XLog("AsyncSocket", XLog.LogStampType.UseStamp, true);
        #endregion

        #region Properties
        public bool Connected
        {
            get { return m_Connected; }
            set { m_Connected = value; }
        }
        public AsyncSocketClient Client
        {
            get { return m_Client; }
            set { m_Client = value; }
        }
        public AsyncSocketServer Server
        {
            get { return m_Server; }
            set { m_Server = value; }
        }
        #endregion

        #region Constructor
        public AsyncSocketManager()
        {

        }
        public AsyncSocketManager(ConnectionMode conMode, string ipAddr, ushort portNo, ProtocolType protocol, Encoding encoding)
        {
            m_ConntionMode = conMode;
            m_ServerIpAddress = ipAddr;
            m_ServerPortNo = portNo;
            m_PortocolType = protocol;
            m_Encoding = encoding;

            Initialize();
        }

        public bool Initialize()
        {
            if (!m_Initialize)
            {
                m_Initialize = true;
                Connect();
            }

            return true;
        }
        public void Connect()
        {
            if (m_ConntionMode == ConnectionMode.Client) NewClient();
            else NewServer();
        }
        private void NewClient()
        {
            m_Client = new AsyncSocketClient(0);
            m_Client.OnConnet += new AsyncSocketConnectEventHandler(OnConnet);
            m_Client.OnClose += new AsyncSocketCloseEventHandler(OnClose);
            m_Client.OnSend += new AsyncSocketSendEventHandler(OnSend);
            m_Client.OnReceive += new AsyncSocketReceiveEventHandler(OnReceive);
            m_Client.OnError += new AsyncSocketErrorEventHandler(OnError);

            m_Client.SetEncoding(m_Encoding);
            m_Client.SetProtocolType(m_PortocolType);
            m_Client.Connect(m_ServerIpAddress, m_ServerPortNo);
        }

        private void NewServer()
        {
            m_Server = new AsyncSocketServer(m_ServerPortNo);
            m_Server.OnAccept += new AsyncSocketAcceptEventHandler(OnAccept);
            m_Server.OnError += new AsyncSocketErrorEventHandler(OnError);
            m_ClientLists = new List<AsyncSocketClient>(m_ClientMaxNo);

            m_Server.Listen();
        }
        #endregion

        #region IDisposable 멤버
        public void Dispose()
        {
            if (m_Client != null)
            {
                m_Client.Close();
                m_Client = null;
            }
            if (m_Server != null)
            {
                m_Server.Stop();
                m_Server = null;
            }
        }
        #endregion

        #region Methods - AsyncSocket Client Event
        private void OnAccept(object sender, AsyncSocketAcceptEventArgs e)
        {
            AsyncSocketClient worker = new AsyncSocketClient(m_ClientId++, e.Worker);

            worker.OnConnet += new AsyncSocketConnectEventHandler(OnConnet);
            worker.OnClose += new AsyncSocketCloseEventHandler(OnClose);
            worker.OnError += new AsyncSocketErrorEventHandler(OnError);
            worker.OnSend += new AsyncSocketSendEventHandler(OnSend);
            worker.OnReceive += new AsyncSocketReceiveEventHandler(OnReceive);
            // 데이터 수신을 대기한다.
            worker.SetEncoding(m_Encoding);
            worker.SetProtocolType(m_PortocolType);
            worker.Receive();

            m_Connected = true;

            // 접속한 클라이언트를 List에 포함한다.
            m_ClientLists.Add(worker);            
            string msg = string.Format("{0} Connected ID:{1} IP:{2} PORT:{3}", m_ConntionMode.ToString(), worker.ID, worker.IpAddress, worker.PortNo);
            //Log.TextOut(msg);
        }

        private void OnConnet(object sender, AsyncSocketConnectionEventArgs e)
        {
            m_Connected = true;
            //Log.TextOut(string.Format("{0} Client{1} Connected", m_ConntionMode.ToString(), e.ID));
        }

        private void OnClose(object sender, AsyncSocketConnectionEventArgs e)
        {
            m_Connected = false;
            if (m_ConntionMode == ConnectionMode.Server)
            {
                for (int i = 0; i < m_ClientLists.Count; i++)
                {
                    if (m_ClientLists[i].ID == e.ID) { m_ClientLists.Remove(m_ClientLists[i]); break; }
                }
                if (m_ClientLists.Count == 0) m_Connected = false;
            }
            
            //Log.TextOut(string.Format("{0} Client{1} Disconnected", m_ConntionMode.ToString(), e.ID));
        }

        private void OnSend(object sender, AsyncSocketSendEventArgs e)
        {
            //Log.TextOut(string.Format("{0}-{1} Sent Msg : {2}", m_ConntionMode.ToString(), e.ID, e.SendData.ToString()));
        }

        private void OnReceive(object sender, AsyncSocketReceiveEventArgs e)
        {
            //Log.TextOut(string.Format("{0}-{1} Recv Msg : {2}", m_ConntionMode.ToString(), e.ID, e.ReceiveData.ToString()));
        }

        private void OnError(object sender, AsyncSocketErrorEventArgs e)
        {
            //Log.TextOut(string.Format("{0}-{1} OnError : {2}", m_ConntionMode.ToString(), e.ID, e.AsyncSocketException.ToString()));

            if (m_ConntionMode == ConnectionMode.Client)
            {
                if (m_Connected == false)
                {
                    m_Client.Close();
                    m_Client = null;
                }
            }
            else
            {
                for (int i=0; i<m_ClientLists.Count; i++)
                {
                    if (m_ClientLists[i].ID == e.ID) { m_ClientLists.Remove(m_ClientLists[i]); break; }
                }
            }
        }
        #endregion

        #region Method
        public void SendData(string msg)
        {
            if (m_ConntionMode == ConnectionMode.Client)
            {
                m_Client.Send(msg);
            }
            else
            {
                for (int i=0; i<m_ClientLists.Count; i++) m_ClientLists[i].Send(msg);
            }
        }
        #endregion
    }
}
