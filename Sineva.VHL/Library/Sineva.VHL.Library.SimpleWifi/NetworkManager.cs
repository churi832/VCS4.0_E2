using Sineva.VHL.Library.SimpleWifi.Win32.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Threading;

namespace Sineva.VHL.Library.SimpleWifi
{
    public class NetworkManager
    {
        public static readonly NetworkManager Instance = new NetworkManager();

        #region Fields
        private NetworkInfo m_NetworkInformation = null;
        private NetworkInterface m_MyNetworkInterface = null;
        private Wifi m_Wifi = null;
        private AccessPoint m_WifiAccessPoint = null;
        private WlanBssEntry m_WifiBssEntry = new WlanBssEntry();

        private Dictionary<string, NetworkInfo> m_Informations = new Dictionary<string, NetworkInfo>();
        private Thread m_Thread;
        private bool m_IsAlive;

        private string m_NetworkName;
        private string m_WifiName;
        private bool m_NetworkRestart = false;
        private bool m_NetworkRestarting = false;
        private int m_SeqRestartNo = 0;

        private long m_LngBytesSend = 0;
        private long m_LngBytesReceived = 0;

        private Stopwatch m_StopWatch = new Stopwatch();
        private UInt32 m_StartTicks = 0;
        #endregion

        #region Properties
        public NetworkInterface MyNetworkInterface
        {
            get { return m_MyNetworkInterface; }
        }
        public NetworkInfo NetworkInformation
        {
            get { return m_NetworkInformation; }
        }
        public ulong NetSpeed //Mbps
        {
            get
            {
                ulong speed = 0;
                try
                {
                    bool valid = true;
                    if (valid) valid &= m_NetworkInformation != null;
                    if (valid) speed = m_NetworkInformation.Speed;
                }
                catch
                { }
                return speed;
            }
        }
        public bool NetworkUseEnable
        {
            get
            {
                bool connect = false;
                try
                {
                    bool valid = true;
                    if (valid) valid &= m_NetworkInformation != null;
                    if (valid)
                    {
                        connect |= m_NetworkInformation.Status == NetConnectionStatus.Connected;
                        connect |= m_NetworkInformation.Status == NetConnectionStatus.MediaDisconnected;
                    }
                }
                catch
                { }
                return connect;
            }
        }
		public bool WifiConnected
        {
            get
            {
                bool connect = false;
                try
                {
                    bool valid = true;
                    if (valid) valid &= m_WifiAccessPoint != null;
                    if (valid) connect = m_WifiAccessPoint.IsConnected;
                }
                catch
                { }
                return connect;
            }
        }
        public string WifiName
        {
            get
            {
                string name = string.Empty;
                try
                {
                    bool valid = true;
                    if (valid) valid &= m_WifiAccessPoint != null;
                    if (valid) name = m_WifiAccessPoint.Name;
                }
                catch
                { }
                return name;
            }
        }
        public uint WLanSignalQuality
        {
            get
            {
                uint quality = 0;
                try
                {
                    bool valid = true;
                    if (valid) valid &= m_WifiAccessPoint != null;
                    if (valid) quality = m_WifiAccessPoint.SignalStrength;
                }
                catch
                { }
                return quality;
            }
        }
        public uint LinkQuality
        {
            get { return m_WifiBssEntry.linkQuality; }
        }
        #endregion

        public NetworkManager()
        {
           
        }
        ~NetworkManager()
        {
            m_IsAlive = false;
        }

        static string ParseProperty(object data)
        {
            if (data != null)
                return data.ToString();
            return "";
        }

        public bool StartMonitor(string network_name = "", string wifi_name = "")
        {
            if (m_IsAlive) return true;
            try
            {
                m_NetworkName = network_name;
				m_WifiName = wifi_name;
                m_SeqRestartNo = 0;

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionID IS NOT NULL");
                foreach (ManagementObject mo in searcher.Get())
                {
                    NetworkInfo info = new NetworkInfo();
                    info.DeviceName = ParseProperty(mo["Description"]);
                    info.AdapterType = ParseProperty(mo["AdapterType"]);
                    info.MacAddress = ParseProperty(mo["MACAddress"]);
                    info.ConnectionID = ParseProperty(mo["NetConnectionID"]);
                    info.Status = (NetConnectionStatus)Convert.ToInt32(mo["NetConnectionStatus"]);
                    SetIP(info);
                    m_Informations.Add(info.ConnectionID, info);
                }

                if (m_Informations.ContainsKey(network_name))
                {
                    m_NetworkInformation = m_Informations[network_name];
                }
                else
                {
                    m_NetworkInformation = null;
                    return false;
                }

                foreach (NetworkInterface currentNetworkInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (currentNetworkInterface.Name.ToLower() == m_NetworkName.ToLower())
                    {
                        m_MyNetworkInterface = currentNetworkInterface;
                    }
                }

				if (m_WifiName != string.Empty)
				{
					m_Wifi = new Wifi();
                	List<AccessPoint> accessPoints = m_Wifi.GetAccessPoints();
                	m_WifiAccessPoint = accessPoints.Find(x => x.Name == wifi_name);
	                if (m_WifiAccessPoint != null)
                	{
                    	SimpleWifi.Win32.Interop.WlanBssEntry[] wlanBssEntries = m_WifiAccessPoint.Interface.GetNetworkBssList();
                    	m_WifiBssEntry = wlanBssEntries.Where(x => System.Text.ASCIIEncoding.ASCII.GetString(x.dot11Ssid.SSID).TrimEnd('\0') == m_WifiAccessPoint.Name).FirstOrDefault();
                	}
				}

                m_IsAlive = true;
                m_Thread = new Thread(new ThreadStart(Monitor));
                m_Thread.Start();
            }
            catch (Exception ex)
            {
                m_IsAlive = false;
                ExceptionLog.WriteLog(ex.ToString());
            }

            return m_IsAlive;
        }

        public void Destory()
        {
            m_IsAlive = false;
            try
            {
                m_Thread.Abort();
                m_Thread = null;
            }
            catch (Exception ex)
            {
                m_IsAlive = false;
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void Monitor()
        {
            while (m_IsAlive)
            {
                try
                {
                    Update();
                    SeqNetworkRestart();
                }
                catch (Exception ex)
                {
                    m_IsAlive = false;
                    ExceptionLog.WriteLog(ex.ToString());
                }
                Thread.Sleep(100);
            }
        }

        private void Update()
        {
            if (m_StopWatch.IsRunning == false) m_StopWatch.Start();
            float lastScanMilliSec = (float)((double)m_StopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency);

            if (m_NetworkInformation == null) return;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionID IS NOT NULL");
                foreach (ManagementObject mo in searcher.Get())
                {
                    string key = ParseProperty(mo["NetConnectionID"]);
                    if (m_Informations.ContainsKey(key))
                    {
                        NetworkInfo info = m_Informations[key];
                        info.DeviceName = ParseProperty(mo["Description"]);
                        info.AdapterType = ParseProperty(mo["AdapterType"]);
                        info.MacAddress = ParseProperty(mo["MACAddress"]);
                        info.ConnectionID = ParseProperty(mo["NetConnectionID"]);
                        info.Status = (NetConnectionStatus)Convert.ToInt32(mo["NetConnectionStatus"]);

                        if (info.Status != NetConnectionStatus.Connected)
                        {
                            info.Speed = 0;
                            info.IP = "0.0.0.0";
                            info.Mask = "0.0.0.0";
                            info.DefaultGateway = "0.0.0.0";
                        }
                        else
                        {
                            info.Speed = Convert.ToUInt64(mo["Speed"]) / 1000000;
                            SetIP(info);
                        }
                        if (key == m_NetworkInformation.ConnectionID)
                            m_NetworkInformation = info;
                    }
                }

                if (lastScanMilliSec > 1000.0f)
                {
                    SetNetworkSpeed();
                    m_StopWatch.Reset();
                }
				
				if (m_Wifi != null)
				{
				    List<AccessPoint> accessPoints = m_Wifi.GetAccessPoints();
                	if (accessPoints.Count > 0)
                	{
                	    AccessPoint point = accessPoints.Find(x => x.Name == m_WifiName);
            	        if (point != null)
        	            {
    	                    m_WifiAccessPoint = point;
	                        SimpleWifi.Win32.Interop.WlanBssEntry[] wlanBssEntries = m_WifiAccessPoint.Interface.GetNetworkBssList();
                        	m_WifiBssEntry = wlanBssEntries.Where(x => System.Text.ASCIIEncoding.ASCII.GetString(x.dot11Ssid.SSID).TrimEnd('\0') == m_WifiAccessPoint.Name).FirstOrDefault();
                    	}
                	}
				}

            }
            catch (Exception ex)
            {
                m_IsAlive = false;
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void SetIP(NetworkInfo info)
        {
            try
            {
                ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                foreach (ManagementObject mo in objMC.GetInstances())
                {
                    if (!(bool)mo["ipEnabled"])
                        continue;
                    if (mo["MACAddress"].ToString().Equals(info.MacAddress))
                    {
                        string[] ip = (string[])mo["IPAddress"];
                        if(ip != null)
                            info.IP = ip[0];
                        string[] mask = (string[])mo["IPSubnet"];
                        if (mask != null)
                            info.Mask = mask[0];
                        string[] gateway = (string[])mo["DefaultIPGateway"];
                        if (gateway != null)
                            info.DefaultGateway = gateway[0];
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                m_IsAlive = false;
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void SetNetworkSpeed()
        {
            try
            {
                if (m_MyNetworkInterface != null)
                {
                    if (m_NetworkInformation.ConnectionID == m_MyNetworkInterface.Name)
                    {
                        m_NetworkInformation.OperationStatus = m_MyNetworkInterface.OperationalStatus;

                        IPv4InterfaceStatistics interfaceStatistic = m_MyNetworkInterface.GetIPv4Statistics();
                        float bytesSentSpeed = (float)(interfaceStatistic.BytesSent - m_LngBytesSend) / 1024;
                        float bytesReceivedSpeed = (float)(interfaceStatistic.BytesReceived - m_LngBytesReceived) / 1024;
                        //m_NetworkInformation.Speed = (ulong)(m_MyNetworkInterface.Speed);
                        m_NetworkInformation.PacketReceived = interfaceStatistic.UnicastPacketsReceived;
                        m_NetworkInformation.PacketSend = interfaceStatistic.UnicastPacketsSent;
                        m_NetworkInformation.UploadSpeed = bytesSentSpeed;
                        m_NetworkInformation.DownloadSpeed = bytesReceivedSpeed;
                        m_LngBytesReceived = interfaceStatistic.BytesReceived;
                        m_LngBytesSend = interfaceStatistic.BytesSent;
                    }
                }
            }
            catch (Exception ex)
            {
                m_IsAlive = false;
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        public void NetworkRestart()
        {
            if (m_NetworkRestarting) return;
            m_SeqRestartNo = 0;
            m_NetworkRestart = true;
            m_NetworkRestarting = true;
        }
        private void SeqNetworkRestart()
        {
            int seqNo = m_SeqRestartNo;
            switch (seqNo)
            {
                case 0:
                    {
                        if (m_NetworkRestart)
                        {
                            m_NetworkRestart = false;
                            //Debug.WriteLine("NetworkRestart");
                            OcsCommLog.WriteLog(string.Format("{0} NetworkRestart", m_NetworkName));
                            seqNo = 10;
                        }
                    }
                    break;

                case 10:
                    {
                        //Debug.WriteLine("NetworkDisable");
                        NetworkDisable();
                        OcsCommLog.WriteLog(string.Format("{0} NetworkDisable", m_NetworkName));
                        m_StartTicks = XFunc.GetTickCount();
                        seqNo = 20;
                    }
                    break;

                case 20:
                    {
                        if (XFunc.GetTickCount() - m_StartTicks < 500) break;
                        if (m_NetworkInformation.Status == NetConnectionStatus.Disconnected)
                        {
                            //Debug.WriteLine("NetworkEnable");
                            OcsCommLog.WriteLog(string.Format("{0} NetworkDisable OK", m_NetworkName));
                            NetworkEnable();
                            OcsCommLog.WriteLog(string.Format("{0} NetworkEnable", m_NetworkName));
                            m_StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                        else if (XFunc.GetTickCount() - m_StartTicks > 2000)
                        {
                            m_NetworkRestarting = false;
                            OcsCommLog.WriteLog(string.Format("{0} NetworkDisable Timeover", m_NetworkName));
                            seqNo = 0;
                        }
                    }
                    break;

                case 30:
                    {
                        if (XFunc.GetTickCount() - m_StartTicks < 500) break;
                        if (m_NetworkInformation.Status == NetConnectionStatus.Connected || m_NetworkInformation.Status == NetConnectionStatus.MediaDisconnected)
                        {
							if (m_Wifi != null)
							{
						        OcsCommLog.WriteLog(string.Format("{0} NetworkEnable OK", m_NetworkName));
        	                    WifiConnect();
    	                        OcsCommLog.WriteLog(string.Format("{0} WifiConnect", m_WifiName));
	                            m_StartTicks = XFunc.GetTickCount();
                            	seqNo = 40;
							}
							else
							{
	                            m_NetworkRestart = false;
    	                        m_NetworkRestarting = false;
        	                    OcsCommLog.WriteLog(string.Format("{0} NetworkEnable OK", m_NetworkName));
            	                seqNo = 0;							
							}							
                        }
                        else if (XFunc.GetTickCount() - m_StartTicks > 2000)
                        {
                            m_NetworkRestarting = false;
                            OcsCommLog.WriteLog(string.Format("{0} NetworkEnable Timeover", m_NetworkName));
                            seqNo = 0;
                        }
                    }
                    break;					
				
                case 40:
                    {
                        if (XFunc.GetTickCount() - m_StartTicks < 500) break;
                        if (WifiConnected)
                        {
                            m_NetworkRestart = false;
                            m_NetworkRestarting = false;
                            //Debug.WriteLine("WifiConnected");
                            OcsCommLog.WriteLog(string.Format("{0} WifiConnect OK", m_WifiName));
                            seqNo = 0;
                        }
                        else if (XFunc.GetTickCount() - m_StartTicks > 2000)
                        {
                            m_NetworkRestarting = false;
                            OcsCommLog.WriteLog(string.Format("{0} WifiConnect Timeover", m_NetworkName));
                            seqNo = 0;
                        }
                    }
                    break;	
            }
            m_SeqRestartNo = seqNo;
        }
        public void NetworkEnable()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("netsh", "interface set interface \"" + m_NetworkInformation.ConnectionID + "\" admin=enable");
                psi.Verb = "runas"; // 관리자 권한으로 실행
                Process.Start(psi).WaitForExit();
            }
            catch
            {
            }
        }
        public void NetworkDisable()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("netsh", "interface set interface \"" + m_NetworkInformation.ConnectionID + "\" admin=disable");
                psi.Verb = "runas"; // 관리자 권한으로 실행
                Process.Start(psi).WaitForExit();
            }
            catch
            {
            }
        }
        public void WifiConnect()
        {
            if (m_NetworkInformation.Status == NetConnectionStatus.Disconnected) return; // Network Disable 상태
            try
            {
                if (m_WifiAccessPoint != null)
                {
                    AuthRequest auth = new AuthRequest(m_WifiAccessPoint);
                    m_WifiAccessPoint.Connect(auth);
                }
            }
            catch
            { }
        }
        public void WifiDisconnect()
        {
            if (m_Wifi == null) return;
            try
            {
                m_Wifi.Disconnect();
            }
            catch
            { }
        }
        public List<NetworkInfo> GetNetInformations()
        {
            return m_Informations.Values.ToList();
        }
        public List<AccessPoint> GetAccessPoints()
        {
            return m_Wifi.GetAccessPoints();
        }
        public void ConsoleWriteAllStatus()
        {
            foreach (NetworkInfo info in m_Informations.Values)
            {
                Console.WriteLine("=========================================");
                Console.WriteLine("Device Name:" + info.DeviceName);
                Console.WriteLine("Adapter Type:" + info.AdapterType);
                Console.WriteLine("MAC ID:" + info.MacAddress);
                Console.WriteLine("Connection Name:" + info.ConnectionID);
                Console.WriteLine("IP Address:" + info.IP);
                Console.WriteLine("Connection Status:" + info.Status.ToString());
                Console.WriteLine("=========================================");
            }
        }
        public void ConsoleWriteStatus(NetworkInfo info)
        {
            Console.WriteLine("=========================================");
            Console.WriteLine("Device Name:" + info.DeviceName);
            Console.WriteLine("Adapter Type:" + info.AdapterType);
            Console.WriteLine("MAC ID:" + info.MacAddress);
            Console.WriteLine("Connection Name:" + info.ConnectionID);
            Console.WriteLine("IP Address:" + info.IP);
            Console.WriteLine("Connection Status:" + info.Status.ToString());
            Console.WriteLine("=========================================");
        }

    }
}
