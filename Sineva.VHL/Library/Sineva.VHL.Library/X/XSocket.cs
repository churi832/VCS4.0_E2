/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.17 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Net.Sockets;
using System.Net;

namespace Sineva.VHL.Library
{
    [Serializable]
    public class XSocket
    {
        #region Fields
        private readonly static object m_LockKey = new object();
        private IPAddress m_RemoteIP;
        private IPEndPoint m_IPEndPoint;
        private EndPoint m_RemotePoint;
        private IPEndPoint m_IPLocalPoint;
        private EndPoint m_LocalPoint;
        private Socket m_Socket;
        private string m_IpAddress;
        private ushort m_PortNo;
        private ProtocolType m_ProtocolType = ProtocolType.Tcp;
        private bool m_IsOpened = false;
        private int m_SendTimeout = 1000;
        private int m_RecvTimeout = 1000;
        private byte[] m_SendBuff;
        private byte[] m_RecvBuff;
        #endregion

        #region Properties
        public ProtocolType ProtocolType
        {
            get { return m_ProtocolType; }
            set { m_ProtocolType = value; }
        }

        public ushort PortNo
        {
            get { return m_PortNo; }
            set { m_PortNo = value; }
        }

        public IPAddress RemoteIP
        {
            get { return m_RemoteIP; }
            set { m_RemoteIP = value; }
        }

        public int SendTimeout
        {
            get { return m_SendTimeout; }
            set { m_SendTimeout = value; }
        }

        public int RecvTimeout
        {
            get { return m_RecvTimeout; }
            set { m_RecvTimeout = value; }
        }
        #endregion

        #region Constructor
        public XSocket()
        {
        }

        public XSocket(string ipAddress, ushort portNo, ProtocolType protocol)
        {
            m_IpAddress = ipAddress;
            m_PortNo = portNo;
            m_ProtocolType = protocol;
        }
        #endregion

        public void Connect()
        {
            lock (m_LockKey)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                try
                {
                    if (m_ProtocolType == ProtocolType.Tcp)
                    {
                        m_RemoteIP = IPAddress.Parse(m_IpAddress);
                        m_IPEndPoint = new IPEndPoint(m_RemoteIP, (int)m_PortNo);

                        m_Socket = new Socket(m_RemoteIP.AddressFamily, SocketType.Stream, m_ProtocolType);
                        m_Socket.Connect(m_IPEndPoint);
                        m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, (int)1000);
                        m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, (int)1000);
                        m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Debug, 1);
                    }
                    else if (m_ProtocolType == ProtocolType.Udp)
                    {
                        m_RemoteIP = IPAddress.Parse(m_IpAddress);
                        m_IPEndPoint = new IPEndPoint(m_RemoteIP, (int)m_PortNo);
                        m_RemotePoint = (EndPoint)m_IPEndPoint;

                        m_IPLocalPoint = new IPEndPoint(IPAddress.Any, (int)m_PortNo);
                        m_LocalPoint = (EndPoint)m_IPLocalPoint;

                        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, m_ProtocolType);
                        //m_Socket.Connect(m_IPEndPoint);
                        m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, (int)1000);
                        m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, (int)1000);
                        m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Debug, 1);
                    }
                    m_IsOpened = true;
                }
                catch (Exception err)
                {
                    m_Socket = null;
                    m_IsOpened = false;
                    ExceptionLog.WriteLog(method, String.Format(err.ToString()));
                }
            }
        }

        public void Disconnect()
        {
            m_Socket.Close();
            m_Socket.Dispose();
        }

        public int SendTo(byte[] sendData)
        {
            var rv = 0;
            rv = m_Socket.SendTo(sendData, m_RemotePoint);
            return rv;
        }

        public int RecvFrom(ref byte[] recvData)
        {
            var rv = 0;
            rv = m_Socket.ReceiveFrom(recvData, ref m_LocalPoint);
            return rv;
        }

        public int Send(byte[] sendData)
        {
            int rv = 0;
            rv = m_Socket.Send(sendData);
            return rv;
        }

        public int Recv(ref byte[] recvData)
        {
            var rv = 0;
            rv = m_Socket.Receive(recvData);
            return rv;
        }
    }
}
