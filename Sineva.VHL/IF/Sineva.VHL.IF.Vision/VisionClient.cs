using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Sineva.VHL.Library;
using Sineva.VHL.Library.EasySocket;

namespace Sineva.VHL.IF.Vision
{
    [Serializable]
    public class VisionClient : IDisposable
    {
        #region
        public static readonly VisionClient Instance = new VisionClient();
        #endregion

        #region Fields
        private bool m_Initialize = false;

        private EasyClient m_Client = null;
        private bool m_bConnected = false;

        private string m_VisionIpAddress = "127.0.0.1"; //hjyou
        private ushort m_VisionPortNumber = 7001;//hjyou

        private List<VisionCommand> m_VisionCommandLists = new List<VisionCommand>();
        private List<VisionCommand> m_PrimaryOutCommandLists = new List<VisionCommand>();
        #endregion

        #region Event
        public event DelVoid_String UpdateVisionMsg;
        #endregion

        #region Properties
        [XmlIgnore()]
        public bool Connected
        {
            get { return m_bConnected; }
            set { m_bConnected = value; }
        }
        [XmlIgnore()]
        public EasyClient Client
        {
            get { return m_Client; }
        }

        public List<VisionCommand> VisionCommandLists
        {
            get { return m_VisionCommandLists; }
            set { m_VisionCommandLists = value; }
        }
        #endregion

        #region Constructor
        public VisionClient() 
        { 
        }
        ~VisionClient()
        {
            m_Client?.Dispose();
        }
        #endregion

        #region IDisposable 멤버

        public void Dispose()
        {
            this.Close();
        }
        #endregion

        #region Methods
        public bool Initialize(ushort remote_port_no, string remote_ip_address)
        {
            if (!m_Initialize)
            {
                m_VisionPortNumber = remote_port_no;
                m_VisionIpAddress = remote_ip_address;
                NewClient();
            }

            m_Initialize = true;
            return true;
        }
        public bool NewClient()
        {
            m_Client = new EasyClient(new ServerInfo(m_VisionIpAddress, m_VisionPortNumber, true));
            m_Client.DataArrived += new DataArrived2Client_EventHandler(client_envetReceived);
            m_Client.Connected += new Connected_EventHandler(eventConnected);
            m_Client.Disconnected += new Disconnected_EventHandler(eventDisconnected);
            m_Client.ConnectionTimeOut += new ConnectionTimeOutError_EventHandler(eventConnectionTimeout);
            m_Client.ScanSleepTime = 1000; // ScanData Delay Time = 1000 / 100 = 10 (msec)
            return true;
        }
        public void client_envetReceived(object msg)
        {
            try
            {
                string sMsg = msg as string;

                string[] split = null;
                if (sMsg != null)
                    split = sMsg.Split(new char[] { ',' });
                else
                    return;

                string command = split[0];

                bool isSecondaryRcvd = false;
                // Secondary Recv Case
                foreach (var set in m_PrimaryOutCommandLists)
                {
                    if (set.m_SecondaryInString == command || command == "ER")
                    {
                        set.m_RcvData = sMsg;
                        set.m_SecondaryIn = true;
                        m_PrimaryOutCommandLists.Remove(set);
                        isSecondaryRcvd = true;
                        break;
                    }
                }
                // Primary Recv Case
                if (isSecondaryRcvd == false)
                {
                    foreach (var set in VisionCommandLists)
                    {
                        if (set.m_PrimaryInString == command || command == "ER")
                        {
                            set.RcvData = sMsg;
                            set.m_PrimaryIn = true;
                        }
                    }
                }
                VisionLog.WriteLog((string)msg);
                UpdateVisionMsg?.Invoke((string)msg);
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public bool SendData(VisionCommand set)
        {
            bool rv = true;
            try
            {
                if (m_Client == null) return false;
                // binaray formatter use : 
                bool binary_formatter = false;
                m_Client.SendDataAsync(set.SndData, binary_formatter);

                bool SameExist = false;
                foreach (var a in m_PrimaryOutCommandLists)
                {
                    if (a == null) continue; // 2020.12.15 Command List에 Null 이 들어온 경우 발생.. 방어코드
                    if (a.m_PrimaryOutCode == set.m_PrimaryOutCode) { SameExist = true; break; }
                }
                if (SameExist == false)
                {
                    m_PrimaryOutCommandLists.Add(set);
                    VisionLog.WriteLog(set.SndData);
                    if (UpdateVisionMsg != null)
                        UpdateVisionMsg((string)set.SndData);
                }
            }
            catch (Exception err) 
            {
                rv = false;
                ExceptionLog.WriteLog(err.ToString());
            }

            return rv;
        }
        public void eventConnectionTimeout()
        {
            m_bConnected = false;
        }
        public void eventConnected()
        {
            m_bConnected = true;
        }
        public void eventDisconnected()
        {
            m_bConnected = false;
        }

        public bool IsConnected()
        {
            if (m_Client == null)
                return false;

            Connected = m_Client.IsConnected;
            return Connected;
        }
        public bool IsConnectionReady()
        {
            bool ready = false;
            if (m_Client.IsReadyToConnect && !m_Client.IsConnected) ready = true;

            return ready;
        }
        public bool Connect()
        {
            bool connect = false;
            try
            {
                if (m_Client == null)
                {
                    NewClient();
                }
                else
                {
                    if (m_Client.ClientSocket != null)
                    {
                        Disconnect();
                    }
                }
            }
            catch (EasySocketException ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            finally
            {
                NewClient();
            }

            if (IsConnectionReady())
            {
                try
                {
                    m_Client.ConnectToServerAsync();
                    connect = true;
                }
                catch (EasySocketException e)
                {
                    MessageBox.Show(e.Message);
                    ExceptionLog.WriteLog(e.ToString());
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    ExceptionLog.WriteLog(e.ToString());
                }
            }
            return connect;
        }
        public void Disconnect()
        {
            try
            {
                if (m_Client != null)
                {
                    m_Client.DisconnectFromServerAsync();
                }
                m_bConnected = false;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.Message);
            }
        }
        public void Close()
        {
            if (m_Client != null) m_Client.Dispose();
            m_Client = null;
        }
        #endregion
    }
}
