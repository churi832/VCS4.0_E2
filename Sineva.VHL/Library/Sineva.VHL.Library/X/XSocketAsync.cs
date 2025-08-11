/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.17 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace Sineva.VHL.Library
{
    public class AsyncSocketStateObject
    {
        private Socket m_WorkSocket = null;
        private Encoding m_WorkEncoding = Encoding.ASCII;

        public Socket WorkSocket
        {
            get { return m_WorkSocket; }
            set { m_WorkSocket = value; }
        }
        public Encoding WorkEncoding
        {
            get { return m_WorkEncoding; }
            set { m_WorkEncoding = value; }
        }
    }

    public class XSocketAsync : IDisposable
    {
        #region delegate
        public delegate void DelStreamReceived(string stream);

        public delegate void DelSocketWorking(AsyncSocketStateObject obj);
        public event DelSocketWorking OnClientAccepted = null;
        public event DelSocketWorking OnServerConnected = null;
        public event DelVoid_Void OnServerConnectFailed = null;
        public event DelVoid_Void OnServerDisconnected = null;
        public event DelStreamReceived OnStreamReceived = null;
        #endregion

        #region Field
        protected static ManualResetEvent m_LockAsyncConnect = new ManualResetEvent(false);
        protected static ManualResetEvent m_LockAsyncAccept = new ManualResetEvent(false);

        private AsyncSocketStateObject m_SocketStateObject = new AsyncSocketStateObject();
        private Encoding m_StreamEncoding = null;
        private bool m_ClientStarted = false;
        private bool m_ServerStarted = false;

        private List<byte> m_RecvBytes = new List<byte>();
        private byte[] m_RecvOneByteBuf = new byte[1];
        private string m_ReceivedStream = string.Empty;
        private Queue<string> m_SendStreamQueue = new Queue<string>();

        private byte m_TerminatorByte = 0x03;
        private string m_TerminatorCode = string.Empty;

        private IPEndPoint m_RemotePoint = null;
        private AddressFamily m_SocketAddressFamily = AddressFamily.InterNetwork;
        private SocketType m_SocketTypeInfo = SocketType.Stream;
        private ProtocolType m_SocketProtocolType = ProtocolType.Tcp;

        private Thread m_Listener = null;
        private IPEndPoint m_ServerIpEndPoint = null;
        #endregion

        #region Property
        public AddressFamily SocketAddressFamily
        {
            get { return m_SocketAddressFamily; }
            set { m_SocketAddressFamily = value; }
        }
        public SocketType SocketTypeInfo
        {
            get { return m_SocketTypeInfo; }
            set { m_SocketTypeInfo = value; }
        }
        public ProtocolType SocketProtocalType
        {
            get { return m_SocketProtocolType; }
            set { m_SocketProtocolType = value; }
        }
        #endregion

        #region Constructor
        public XSocketAsync(AddressFamily addressfamily, SocketType sockettype, ProtocolType protocoltype, byte terminator)
        {
            m_SocketAddressFamily = addressfamily;
            m_SocketTypeInfo = sockettype;
            m_SocketProtocolType = protocoltype;

            m_TerminatorCode = string.Empty;
            m_TerminatorByte = terminator;
        }
        public XSocketAsync(AddressFamily addressfamily, SocketType sockettype, ProtocolType protocoltype, string terminator)
        {
            m_SocketAddressFamily = addressfamily;
            m_SocketTypeInfo = sockettype;
            m_SocketProtocolType = protocoltype;

            m_TerminatorByte = 0x00;
            m_TerminatorCode = terminator;
        }
        public void Dispose()
        {
            if (m_ServerStarted || m_Listener != null)
            {
                m_ServerStarted = false;
            }
            if (m_ClientStarted)
            {
                Disconnect(false);
            }
        }
        #endregion

        #region Method
        public void OpenServer(IPEndPoint serverEndPoint)
        {
            if (!m_ClientStarted && !m_ServerStarted)
            {
                m_ServerIpEndPoint = serverEndPoint;

                m_Listener = new Thread(new ThreadStart(ThreadLoopServer));
                m_Listener.IsBackground = true;
                m_Listener.Start();
            }
        }
        private void ThreadLoopServer()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            Socket socket = new Socket(m_SocketAddressFamily, m_SocketTypeInfo, m_SocketProtocolType);
            try
            {
                socket.Bind(m_ServerIpEndPoint);
                socket.Listen(100);
                m_ServerStarted = true;
            }
            catch (SocketException ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }

            while (m_ServerStarted)
            {
                Thread.Sleep(10);
                try
                {
                    m_LockAsyncAccept.Reset();
                    socket.BeginAccept(new AsyncCallback(AcceptCallbck), socket);
                    m_LockAsyncAccept.WaitOne();
                }
                catch (SocketException ex)
                {
                    ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
                }
            }
        }
        public void Connect(IPEndPoint ipep)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                if (!m_ClientStarted && !m_ServerStarted)
                {
                    m_RemotePoint = ipep;
                    m_StreamEncoding = Encoding.ASCII;

                    m_ClientStarted = true;
                    BeginConnect();
                }
            }
            catch (SocketException ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
        }
        public void Connect(IPEndPoint ipep, Encoding textEncoding)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                if (!m_ClientStarted && !m_ServerStarted)
                {
                    m_RemotePoint = ipep;
                    m_StreamEncoding = textEncoding;

                    m_ClientStarted = true;
                    BeginConnect();
                }
            }
            catch (SocketException ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
        }
        public void Disconnect(bool reuse = false)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                if (m_SocketStateObject != null && m_SocketStateObject.WorkSocket != null)// && m_SocketStateObject.WorkSocket.Connected)
                {
                    m_SocketStateObject.WorkSocket.BeginDisconnect(reuse, new AsyncCallback(DisconnectCallback), m_SocketStateObject.WorkSocket);
                }
            }
            catch (SocketException ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
        }
        private void BeginConnect()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                m_LockAsyncConnect.Reset();

                Socket socket = new Socket(m_SocketAddressFamily, m_SocketTypeInfo, m_SocketProtocolType);
                socket.BeginConnect(m_RemotePoint, new AsyncCallback(ConnectCallback), socket);

                m_LockAsyncConnect.WaitOne();
            }
            catch (SocketException ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
        }
        private void BeginReceive(AsyncSocketStateObject state)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                if (state.WorkSocket != null && state.WorkSocket.Connected)
                    state.WorkSocket.BeginReceive(m_RecvOneByteBuf, 0, m_RecvOneByteBuf.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
            }
            catch (SocketException ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
        }
        private void BeginSend()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                if (m_SendStreamQueue.Count > 0)
                {
                    byte[] byteStream = m_StreamEncoding.GetBytes(m_SendStreamQueue.Dequeue());
                    m_SocketStateObject.WorkSocket.BeginSend(byteStream, 0, byteStream.Length, SocketFlags.None, new AsyncCallback(OnSendCallback), m_SocketStateObject.WorkSocket);
                }
            }
            catch (SocketException ex)
            {
                m_SendStreamQueue.Clear();
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
            catch (Exception ex)
            {
                m_SendStreamQueue.Clear();
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
        }
        public void SendMessage(string stream)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                if (m_SocketStateObject != null && m_SocketStateObject.WorkSocket != null && m_SocketStateObject.WorkSocket.Connected)
                {
                    m_SendStreamQueue.Enqueue(stream);
                    BeginSend();
                }
            }
            catch (SocketException ex)
            {
                m_SendStreamQueue.Clear();
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
            catch (Exception ex)
            {
                m_SendStreamQueue.Clear();
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
        }
        protected virtual void ConnectServerCallbackMethod(AsyncSocketStateObject state)
        {
        }
        protected virtual void DisconnectServerCallbackMethod()
        {
        }
        protected virtual void AcceptClientCallbackMethod(AsyncSocketStateObject state)
        {
        }
        protected virtual void ReceiveCallbackMethod(AsyncSocketStateObject state)
        {
        }
        #endregion

        #region Callback
        private void AcceptCallbck(IAsyncResult ar)
        {
            m_LockAsyncAccept.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket socket = listener.EndAccept(ar);

            AsyncSocketStateObject state = new AsyncSocketStateObject();
            state.WorkSocket = socket;

            if (OnClientAccepted != null)
                OnClientAccepted.Invoke(state);

            AcceptClientCallbackMethod(state);
            BeginReceive(state);
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                m_LockAsyncConnect.Set();

                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);

                AsyncSocketStateObject state = new AsyncSocketStateObject();
                state.WorkSocket = socket;
                state.WorkEncoding = m_StreamEncoding;
                m_SocketStateObject = state;

                if (OnServerConnected != null)
                    OnServerConnected.Invoke(state);

                ConnectServerCallbackMethod(state);
                BeginReceive(state);
            }
            catch (SocketException ex)
            {
                if (OnServerConnectFailed != null)
                {
                    m_ClientStarted = false;
                    OnServerConnectFailed.Invoke();
                }
                else
                {
                    BeginConnect();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void DisconnectCallback(IAsyncResult ar)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndDisconnect(ar);

                m_SocketStateObject.WorkSocket.Close();
                m_SocketStateObject.WorkSocket = null;
                m_ClientStarted = false;

                if (OnServerConnected != null)
                    OnServerDisconnected.Invoke();

                DisconnectServerCallbackMethod();
            }
            catch (SocketException ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            AsyncSocketStateObject state = (AsyncSocketStateObject)ar.AsyncState;
            Socket socket = state.WorkSocket;
            if (socket == null || !socket.Connected)
            {
                state = null;
                SocketCloseMethod();

                return;
            }

            try
            {
                if (socket.EndReceive(ar) > 0)
                {
                    m_RecvBytes.AddRange(m_RecvOneByteBuf);
                    bool checkTerminatorString = !string.IsNullOrEmpty(m_TerminatorCode);
                    string stream = string.Empty;
                    bool recvEnd = false;
                    if (checkTerminatorString)
                    {
                        stream = m_StreamEncoding.GetString(m_RecvBytes.ToArray());
                        recvEnd = stream.Contains(m_TerminatorCode);
                    }
                    else
                    {
                        recvEnd = m_RecvBytes.Contains(m_TerminatorByte);
                    }

                    if (recvEnd)
                    {
                        try
                        {
                            if (checkTerminatorString)
                                m_ReceivedStream = stream.Remove(stream.IndexOf(m_TerminatorCode));
                            else
                                m_ReceivedStream = m_StreamEncoding.GetString(m_RecvBytes.ToArray(), 0, m_RecvBytes.IndexOf(m_TerminatorByte));
                            m_RecvBytes.Clear();

                            if (OnStreamReceived != null)
                                OnStreamReceived.Invoke(m_ReceivedStream);

                            ReceiveCallbackMethod(state);
                        }
                        catch (Exception)
                        {
                            m_RecvBytes = new List<byte>();
                        }
                    }

                    BeginReceive(state);
                }
                else
                {
                    SocketCloseMethod();
                    if (m_SocketStateObject != null && m_SocketStateObject.WorkSocket != null)
                    {
                        m_SocketStateObject.WorkSocket.Shutdown(SocketShutdown.Both);
                        m_SocketStateObject.WorkSocket.BeginDisconnect(true, new AsyncCallback(DisconnectCallback), m_SocketStateObject.WorkSocket);
                    }
                }
            }
            catch (SocketException ex)
            {
                BeginConnect();
            }
            catch (Exception ex)
            {

            }
        }
        private void OnSendCallback(IAsyncResult ar)
        {
            try
            {
                Socket sock = (Socket)ar.AsyncState;
                int byteSend = sock.EndSend(ar);

                BeginSend();
            }
            catch (SocketException ex)
            {

            }
            catch (Exception ex)
            {
            }
        }

        protected virtual void SocketCloseMethod()
        {
        }
        #endregion
    }
}
