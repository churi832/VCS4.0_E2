//////////////////////////////////////////////////////
///////// MXP 충돌로 인하여 사용불가함.
//////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels.Tcp;
using Sineva.VHL.Library;
using System.Threading;
using System.Runtime.Remoting.Channels;
using System.Collections;

namespace Sineva.VHL.Library.Remoting
{    
    /// <summary>
     /// Remoting Channel Type
     /// </summary>
    [Serializable()]
    public enum CHANNEL_TYPE
    {
        IPC,
        TCP,
        HTTP,
    }
    public class RemoteManager
    {
        #region Singleton
        public static readonly RemoteDevice TouchInstance = new RemoteDevice();
        public static readonly RemoteDevice PadInstance = new RemoteDevice();
        #endregion

    }
    public class RemoteDevice
    {
        #region Fields
        private bool m_Initialzied = false;
        private ConnectionMode m_ConnectionMode = ConnectionMode.Server;
        private CHANNEL_TYPE m_ChannelType = CHANNEL_TYPE.IPC;
        private bool m_Conneted = false;
        private bool m_Disconnected = false;
        // IPC Remote
        private IChannel m_ServerChannel = null;
        private IChannel m_ClientChannel = null;
        private RemoteObject m_Remoting = null;
        private string m_ServerIPAddress;

        public event DelVoid_Void ReconnectedRuntimeInvoke = null; // Server가 새로 생성될 경우.... Runtime.RemoteObject.RemoteObject를 재연결 해야 함.
        #endregion

        #region Properties
        public RemoteObject Remoting
        {
            get { return m_Remoting; }
        }
        public bool Conneted
        {
            get { return m_Conneted; }
            set { m_Conneted = value; }
        }
        public bool Disconnected
        {
            get { return m_Disconnected; }
            set { m_Disconnected = value; }
        }
        public ConnectionMode ConndectionMode
        {
            get { return m_ConnectionMode; }
            set { m_ConnectionMode = value; }
        }
        #endregion

        #region Constructor
        public RemoteDevice()
        {
        }
        #endregion

        #region Methods
        public bool Initialize(ConnectionMode connectionMode, CHANNEL_TYPE channelType, string serverAddress)
        {
            try
            {
                m_ConnectionMode = connectionMode;
                m_ChannelType = channelType;

                if (m_ConnectionMode == ConnectionMode.Server)
                {
                    if (channelType == CHANNEL_TYPE.TCP)
                    {
                        IDictionary props = new Hashtable();
                        props["name"] = "tcpserver";
                        props["port"] = 7777;
                        if (AppConfig.Instance.Simulation.MY_DEBUG == true)
                        {
                            props["bindTo"] = "127.0.0.1";
                        }
                        else
                        {
                            props["bindTo"] = AppConfig.Instance.RemotingIP; // 指定绑定的IP地址
                        }

                        TcpServerChannel channel = new TcpServerChannel(props, new BinaryServerFormatterSinkProvider());
                        ChannelServices.RegisterChannel(channel, false);
                        RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteObject), "RemoteData", WellKnownObjectMode.Singleton);
                        m_Remoting = new RemoteObject();
                        RemotingLog.WriteLog(string.Format("Listening on TCP Port {0}", 7777));
                    }
                    else if (channelType == CHANNEL_TYPE.IPC)
                    {
                        IDictionary props = new Hashtable();
                        props["authorizedGroup"] = "Everyone";
                        props["portName"] = "Remote.Sineva";
                        BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
                        m_ServerChannel = new IpcServerChannel(props,serverProvider);
                        ChannelServices.RegisterChannel(m_ServerChannel, false);
                        RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteObject), "RemoteData", WellKnownObjectMode.Singleton);
                        m_Remoting = new RemoteObject();
                        RemotingLog.WriteLog(string.Format("Listening on IPC Remote.Sineva"));
                    }
                    else if (channelType == CHANNEL_TYPE.HTTP)
                    {
                        m_ServerChannel = new HttpServerChannel(7777);
                        ChannelServices.RegisterChannel(m_ServerChannel, false);
                        RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteObject), "RemoteData", WellKnownObjectMode.Singleton);
                        m_Remoting = new RemoteObject();
                        RemotingLog.WriteLog(string.Format("Listening on HTTP Port {0}", 7777));
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    m_ServerIPAddress = serverAddress;
                    if (channelType == CHANNEL_TYPE.TCP)
                    {
                        m_ClientChannel = new TcpClientChannel(DateTime.Now.ToString(), null);
                        ChannelServices.RegisterChannel(m_ClientChannel, false);
                        m_Remoting = (RemoteObject)Activator.GetObject(typeof(RemoteObject), "tcp://" + serverAddress + ":7777/RemoteData");
                        RemotingLog.WriteLog(string.Format("Conection to TCP, {0}", m_ClientChannel.ChannelName));
                    }
                    else if (channelType == CHANNEL_TYPE.IPC)
                    {
                        IDictionary props = new Hashtable();
                        props["authorizedGroup"] = "Everyone";
                        props["portName"] = "Remote.Sineva";
                        BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
                        m_ClientChannel = new IpcClientChannel(props, clientProvider);
                        ChannelServices.RegisterChannel(m_ClientChannel, false);
                        m_Remoting = (RemoteObject)Activator.GetObject(typeof(RemoteObject), "ipc://Remote.Sineva/RemoteData");
                        RemotingLog.WriteLog(string.Format("Conection to IPC, {0}", m_ClientChannel.ChannelName));
                    }
                    else if (channelType == CHANNEL_TYPE.HTTP)
                    {
                        m_ClientChannel = new HttpClientChannel();
                        ChannelServices.RegisterChannel(m_ClientChannel, false);
                        m_Remoting = (RemoteObject)Activator.GetObject(typeof(RemoteObject), "http://" + serverAddress + ":7777/RemoteData");
                        RemotingLog.WriteLog(string.Format("Conection to HTTP, {0}", m_ClientChannel.ChannelName));
                    }
                    else
                    {
                        return false;
                    }
                }

                TaskHandler.Instance.RegTask(new TaskThreadProc(this, m_Remoting), 10, System.Threading.ThreadPriority.Highest);
                m_Initialzied = true;
            }
            catch (Exception ex)
            {
                m_Conneted = false;
                ExceptionLog.WriteLog("Runtime Remoting :" + ex.Message);
            }

            return m_Initialzied;
        }

        private class TaskThreadProc : XSequence
        {
            public TaskThreadProc(RemoteDevice remoteDevice, RemoteObject remote)
            {
                RegSeq(new SeqPing(remoteDevice, remote));
            }
        }

        private class SeqPing : XSeqFunc
        {
            #region Fields
            private RemoteObject m_RemoteControl = null;
            private RemoteDevice m_RemoteDevice = null;
            private int m_OldPing = 0;
            private int m_Count = 0;
            #endregion

            #region Properties
            #endregion

            #region Constructor
            public SeqPing(RemoteDevice remoteDevice, RemoteObject remote)
            {
                this.SeqName = $"SeqPing_RemoteObject";
                m_RemoteControl = remote;
                m_RemoteDevice = remoteDevice;
            }
            #endregion

            #region Override
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                int rv = -1;
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_RemoteDevice.m_ConnectionMode == ConnectionMode.Server)
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 100;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                bool ok = TryPing();

                                if (m_RemoteDevice.Conneted != ok)
                                {
                                    if (ok) RemotingLog.WriteLog(string.Format("Connected Client"));
                                    else RemotingLog.WriteLog(string.Format("Disconnected Client"));
                                }

                                //EqpLog.WriteLog("RUNTIME", string.Format("{0}_{1}", ok ? "TRUE" : "FALSE", m_RemoteControl.Ping));
                                m_RemoteDevice.Conneted = ok;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 100:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 5000)
                            {
                                bool ok = PingOk();

                                if (m_RemoteDevice.Conneted != ok)
                                {
                                    if (ok) RemotingLog.WriteLog(string.Format("Connected Server"));
                                    else RemotingLog.WriteLog(string.Format("Disconnected Server"));
                                }

                                //EqpLog.WriteLog("RUNTIME", string.Format("{0}_{1}", ok ? "TRUE" : "FALSE", m_OldPing));
                                if (ok)
                                {
                                    m_RemoteDevice.Conneted = ok;
                                    if (m_RemoteDevice.Disconnected)
                                    {
                                        m_RemoteDevice.Disconnected = false;
                                        if (m_RemoteDevice.ReconnectedRuntimeInvoke != null)
                                            m_RemoteDevice.ReconnectedRuntimeInvoke.Invoke();

                                        m_Count = 0;
                                    }
                                }
                                else
                                {
                                    if (m_Count > 2)
                                    {
                                        m_RemoteDevice.Conneted = ok;
                                        //RemoteManager.Instance.Disconnected = true; // 500msec 동안 변화가 없네....!
                                    }
                                    m_Count++;
                                }
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }

            private bool TryPing()
            {
                bool rv = true;
                try
                {
                    if (m_RemoteControl.Ping < int.MaxValue) m_RemoteControl.Ping++;
                    else m_RemoteControl.Ping = int.MinValue;
                    rv = true;
                }
                catch (System.Runtime.Remoting.RemotingException err)
                {
                    m_RemoteDevice.Disconnected = true;
                    rv = false;
                }
                catch (Exception ex)
                {
                    m_RemoteDevice.Disconnected = true;
                    rv = false;
                }

                return rv;
            }

            private bool PingOk()
            {
                bool rv = true;
                try
                {
                    if (m_RemoteControl.Ping != m_OldPing) rv = true;
                    else rv = false;
                    m_OldPing = m_RemoteControl.Ping;
                }
                catch (System.Runtime.Remoting.RemotingException err)
                {
                    m_RemoteDevice.Disconnected = true;
                    rv = false;
                }
                catch (Exception ex)
                {
                    m_RemoteDevice.Disconnected = true;
                    rv = false;
                }
                return rv;
            }
            #endregion
        }
        #endregion
    }
}
