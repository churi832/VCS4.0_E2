using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System;
using System.Drawing.Design;

namespace Sineva.VHL.Library
{
    #region Enum
    [Serializable()]
    public enum enProjectType
    {
        DEMO,
        ESWIN,
        CRRC,
        NEXCHIP,
    }
    #endregion

    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    [Serializable]
    public class Simul
    {
        #region Fields
        private bool m_MY_DEBUG = true;
        private bool m_MP = true;
        private bool m_ACS = true;
        private bool m_AXT = true;
        private bool m_MAXON = true;
        private bool m_SERIAL = true;
        private bool m_MELSEC = true;
        private bool m_CIM = true;
        private bool m_IO = true;
        private bool m_MOTION = true;
        private bool m_STK = true;
        private bool m_BNR = true;
        private bool m_MXP = true;
        #endregion

        #region Properties
        public bool MY_DEBUG
        {
            get { return m_MY_DEBUG; }
            set { m_MY_DEBUG = value; }
        }
        public bool MP
        {
            get { return m_MP; }
            set { m_MP = value; }
        }
        public bool ACS
        {
            get { return m_ACS; }
            set { m_ACS = value; }
        }
        public bool AXT
        {
            get { return m_AXT; }
            set { m_AXT = value; }
        }
        public bool MAXON
        {
            get { return m_MAXON; }
            set { m_MAXON = value; }
        }
        public bool SERIAL
        {
            get { return m_SERIAL; }
            set { m_SERIAL = value; }
        }
        public bool MELSEC
        {
            get { return m_MELSEC; }
            set { m_MELSEC = value; }
        }
        public bool CIM
        {
            get { return m_CIM; }
            set { m_CIM = value; }
        }
        public bool IO
        {
            get { return m_IO; }
            set { m_IO = value; }
        }
        public bool MOTION
        {
            get { return m_MOTION; }
            set { m_MOTION = value; }
        }
        public bool STK
        {
            get { return m_STK; }
            set { m_STK = value; }
        }

        public bool BNR
        {
            get { return m_BNR; }
            set { m_BNR = value; }
        }
        public bool MXP
        {
            get { return m_MXP; }
            set { m_MXP = value; }
        }
        #endregion

        #region Constructor
        public Simul()
        {
        }
        #endregion

        #region Override
        public override string ToString()
        {
            return "Set simulation flags";
        }
        #endregion
    }

    public class AppConfig
    {
        public static readonly AppConfig Instance = new AppConfig();
        #region Fields
        public static string m_Version = "Ver4.0";

        private static bool m_Initialized = false;
        private static bool m_UseDefaultFilePath = false;
        private static bool m_UseIOLogRecord = false;

        private static readonly string[] ignores = new string[] { "Assembly", "bin", "Debug" };
        private static string m_AppRootPath = "";
        public static readonly string DefaultConfigFilePath = GetAppRootPath() + "\\Configuration";
        public static readonly string DefaultTroubleshootingManualPath = DefaultConfigFilePath + "\\Troubleshooting";
        public static readonly string DefaultLogFilePath = GetAppRootPath() + "\\Log";

        private static FolderSelect m_ConfigPath = new FolderSelect();
        private static FolderSelect m_TroubleshootingManualPath = new FolderSelect();

        private static FolderSelect m_LogPath = new FolderSelect();
        private static LogFileSplitType m_LogFileSplit = LogFileSplitType.Day;
        private static int m_OriginalLogKeepingDays = 30;
        private static int m_CompressedLogKeepingDays = 180;
        private static int m_MaxRecordLines = 10000;
        private static int m_TpdLogScanTime = 100; // 100msec 주기

        private static ushort m_StorageAvailableRatio = 5;
        private static bool m_UseCpuTemperatureCheckFunction = true;
        private static Language m_AppLanguage = Language.English;
        private static VehicleType m_VehicleType = VehicleType.Normal;

        private static string m_FtpServerAddress = string.Empty;
        private static string m_FtpUserName = string.Empty;
        private static string m_FtpPassword = string.Empty;
        private static string m_FtpFolderPath = string.Empty;
        private static bool m_FtpUploadUserShell = false;
        private static int m_VehicleNumber = 1;
        private static string m_RemotingIP = "127.0.0.1";
        private static uint m_AutoScreenLockTime = 60;
        private static uint m_AutoLogoutTime = 30;
        private static enProjectType m_ProjectType = enProjectType.DEMO;

        public static string m_OperatorLoginUserID = "";

        public static string m_WifiNetworkConnectionID = "Wi-Fi";
        public static string m_WifiName = "SinevaOHT";

        private static Mutex m_Mutex = new Mutex();
        private static Simul m_Simulation = new Simul();
        #endregion

        #region Properties
        [XmlIgnore(), Browsable(false)]
        public static bool AppMainInitiated = false;
        [XmlIgnore(), Browsable(false)]
        public static bool AppMainDisposed = false;
        [Category("Option")]
        [DisplayName("EQP Version")]
        public string Version
        {
            get { return m_Version; }
            set { m_Version = value; }
        }

        [Category("FilePath")]
        public bool UseDefaultFilePath
        {
            get { return m_UseDefaultFilePath; }
            set { m_UseDefaultFilePath = value; }
        }
        [Category("Option")]
        public bool UseIOLogRecord
        {
            get { return m_UseIOLogRecord; }
            set { m_UseIOLogRecord = value; }
        }
        
        [Category("Option")]
        public ushort StorageAvailableRatio
        {
            get { return m_StorageAvailableRatio; }
            set { m_StorageAvailableRatio = value; }
        }
        [Category("Option")]
        public bool UseCpuTemperatureCheck
        {
            get { return m_UseCpuTemperatureCheckFunction; }
            set { m_UseCpuTemperatureCheckFunction = value; }
        }

        [Category("FilePath : Configfile")]
        public FolderSelect ConfigPath
        {
            get { return m_ConfigPath; }
            set { m_ConfigPath = value; }
        }
        [Category("FilePath : TroubleshootingManualFile")]
        [DisplayName("Troubleshooting Manual Path")]
        public FolderSelect TroubleshootingManualPath
        {
            get { return m_TroubleshootingManualPath; }
            set { m_TroubleshootingManualPath = value; }
        }

        [Browsable(false)]
        public string AppConfigPath
        {
            get
            {
                if (m_UseDefaultFilePath) return DefaultConfigFilePath;
                else return m_ConfigPath.SelectedFolder;
            }
        }
        [Browsable(false)]
        public string AppConfigFile
        {
            get
            {
                return AppConfigPath + "\\AppConfig.xml";
            }
        }

        [Browsable(false)]
        public string XmlSetupPath
        {
            get
            {
                if (m_UseDefaultFilePath) return DefaultConfigFilePath + "\\Setup";
                else return AppConfigPath + "\\Setup";
            }
        }
        [Browsable(false)]
        public string XmlDevicesPath
        {
            get
            {
                if (m_UseDefaultFilePath) return DefaultConfigFilePath + "\\Devices";
                else return AppConfigPath + "\\Devices";
            }
        }
        [Browsable(false)]
        public string XmlServoParameterPath
        {
            get
            {
                if (m_UseDefaultFilePath) return DefaultConfigFilePath + "\\ServoPara";
                else return AppConfigPath + "\\ServoPara";
            }
        }
        [Browsable(false)]
        public string XmlIoDefinePath
        {
            get
            {
                if (m_UseDefaultFilePath) return DefaultConfigFilePath;
                else return AppConfigPath;
            }
        }
        [Browsable(false)]
        public string XmlAxisBlockPath
        {
            get
            {
                if (m_UseDefaultFilePath) return DefaultConfigFilePath;
                else return AppConfigPath;
            }
        }
        [Browsable(false)]
        public string XmlAlarmListPath
        {
            get
            {
                if (m_UseDefaultFilePath) return DefaultConfigFilePath + "\\Database";
                else return AppConfigPath + "\\Database";
            }
        }
        [Browsable(false)]
        public string XmlOCSPath
        {
            get
            {
                if (m_UseDefaultFilePath) return DefaultConfigFilePath;
                else return AppConfigPath;
            }
        }
        [Browsable(false)]
        public string XmlJCSPath
        {
            get
            {
                if (m_UseDefaultFilePath) return DefaultConfigFilePath;
                else return AppConfigPath;
            }
        }
        [Browsable(false)]
        public string XmlDatabasePath
        {
            get
            {
                if (m_UseDefaultFilePath) return DefaultConfigFilePath + "\\Database";
                else return AppConfigPath + "\\Database";
            }
        }
        [Browsable(false)]
        public string XmlInterlockManagerPath
        {
            get
            {
                if (m_UseDefaultFilePath) return DefaultConfigFilePath;
                else return AppConfigPath;
            }
        }

        [Category("Log Configuration")]
        [Description("Log file path.")]
        [DisplayName("Log File Save Path")]
        [ConfigAttribute(SaveName = "LogPath")]
        [Editor(typeof(UIEditorFolderSelect), typeof(UIEditorFolderSelect))]
        public FolderSelect LogPath
        {
            get { return m_LogPath; }
            set { m_LogPath = value; }
        }
        [Category("Log Configuration")]
        [Description("Log file split type.")]
        [ConfigAttribute(SaveName = "LogSplitType")]
        public LogFileSplitType LogFileSplit
        {
            get { return m_LogFileSplit; }
            set { m_LogFileSplit = value; }
        }
        [Category("Log Configuration")]
        [Description("Original log files keeping days. (30 ~ CompressedLogKeepingDays days)")]
        [ConfigAttribute(SaveName = "LogKeepingDays")]
        public int OriginalLogKeepingDays
        {
            get { return m_OriginalLogKeepingDays; }
            set
            {
                if ((value < 30) || (value > m_CompressedLogKeepingDays))
                {
                    return;
                }
                m_OriginalLogKeepingDays = value;
            }
        }
        [Category("Log Configuration")]
        [Description("Compressed log files keeping days. (OriginalLogKeepingDays ~ 360 days)")]
        [DisplayName("Compressed Log File Keeping Days")]
        [ConfigAttribute(SaveName = "CompressedLogKeepingDays")]
        public int CompressedLogKeepingDays
        {
            get { return m_CompressedLogKeepingDays; }
            set
            {
                if ((value < m_OriginalLogKeepingDays) || (value > 360))
                {
                    return;
                }
                m_CompressedLogKeepingDays = value;
            }
        }
        [Category("Log Configuration")]
        [Description("Log file max lines. (100,000 ~ 2,000,000 lines)\r\nOnly in LogFileSplitType is DayAndLines.")]
        [DisplayName("Max Record Lines")]
        [ConfigAttribute(SaveName = "MaxRecordLines")]
        public int MaxRecordLines
        {
            get { return m_MaxRecordLines; }
            set
            {
                if ((value < 100000) || (value > 2000000))
                {
                    return;
                }
                m_MaxRecordLines = value;
            }
        }
        
        [DisplayName("TPD Log Scan Time")]
        public int TpdLogScanTime
        {
            get { return m_TpdLogScanTime; }
            set { m_TpdLogScanTime = value; }
        }
        
        [DisplayName("Project Type")]
        public enProjectType ProjectType
        {
            get { return m_ProjectType; }
            set { m_ProjectType = value; }
        }
        [DisplayName("Vehicle Number")]
        public int VehicleNumber
        { 
            get { return m_VehicleNumber; } 
            set { m_VehicleNumber = value; } 
        }

        [DisplayName("Remoting IP")]
        public string RemotingIP
        {
            get { return m_RemotingIP; }
            set { m_RemotingIP = value; }
        }
        [Category("Option")]
        public uint AutoScreenLockTime
        {
            get { return AppConfig.m_AutoScreenLockTime; }
            set
            {
                uint lockTime = value;
                if (lockTime < 30) lockTime = 30;
                AppConfig.m_AutoScreenLockTime = lockTime;

                uint logoutTime = AppConfig.m_AutoLogoutTime;
                if (logoutTime > lockTime - 10) AppConfig.m_AutoLogoutTime = lockTime - 10;
            }
        }
        [Category("Option")]
        public uint AutoLogoutTime
        {
            get { return AppConfig.m_AutoLogoutTime; }
            set
            {
                uint logoutTime = value;
                if (logoutTime > AppConfig.m_AutoScreenLockTime - 10) logoutTime = AppConfig.m_AutoScreenLockTime - 10;
                AppConfig.m_AutoLogoutTime = logoutTime;
            }
        }
        [XmlIgnore(), Browsable(false)]
        public string OperatorLoginUserID
        {
            get { return m_OperatorLoginUserID; }
            set { m_OperatorLoginUserID = value; }
        }
        [Category("Option")]
        public string WifiNetworkConnectionID
        {
            get { return m_WifiNetworkConnectionID; }
            set { m_WifiNetworkConnectionID = value; }
        }
        [Category("Option")]
        public string WifiName
        {
            get { return m_WifiName; }
            set { m_WifiName = value; }
        }
        #endregion

        #region Option
        [Category("Option : EQP Configure"), DisplayName("Language Select")]
        public Language AppLanguage
        {
            get { return m_AppLanguage; }
            set { m_AppLanguage = value; }
        }
        [Category("Option : EQP Configure"), DisplayName("Vehicle Type Select")]
        public VehicleType VehicleType
        {
            get { return m_VehicleType; }
            set { m_VehicleType = value; }
        }
        #endregion
        #region FTP
        [Category("FTP Configure")]
        [DisplayName("FTP Server Address")]
        public string FtpServerAddress
        {
            get { return m_FtpServerAddress; }
            set { m_FtpServerAddress = value; }
        }
        [Category("FTP Configure")]
        [DisplayName("FTP Server UserName")]
        public string FtpUserName
        {
            get { return m_FtpUserName; }
            set { m_FtpUserName = value; }
        }
        [Category("FTP Configure")]
        [DisplayName("FTP Server Password")]
        public string FtpPassword
        {
            get { return m_FtpPassword; }
            set { m_FtpPassword = value; }
        }
        [Category("FTP Configure")]
        [DisplayName("FTP Server Folder Path for Equipment")]
        public string FtpFolderPath
        {
            get { return m_FtpFolderPath; }
            set { m_FtpFolderPath = value; }
        }
        [Category("FTP Configure"), DisplayName("FTP Upload using User Shell Executing")]
        public bool FtpUploadUserShell
        {
            get { return m_FtpUploadUserShell; }
            set { m_FtpUploadUserShell = value; }
        }
        #endregion

        #region Properties - Simulation
        [Category("Simulation")]
        public Simul Simulation
        {
            get { return AppConfig.m_Simulation; }
            set { AppConfig.m_Simulation = value; }
        }
        #endregion

        #region Constructor
        public AppConfig()
        {
        }
        public bool Initialize()
        {
            if (!m_Initialized)
            {
                m_Initialized = true;

                m_ConfigPath.SelectedFolder = GetSolutionPath();
                m_Initialized = ReadXml();
            }

            return m_Initialized;
        }
        #endregion

        #region Methods
        public static string GetSolutionPath()
        {
            string solutionPath = Application.StartupPath + "\\Configuration";
            return solutionPath;
        }
        public static string GetAppRootPath()
        {
            m_AppRootPath = Application.StartupPath;// GetSolutionPath();
            return m_AppRootPath;
        }

        public bool ReadXml()
        {
            string fileName = AppConfigFile;
            if (string.IsNullOrEmpty(AppConfigFile))
            {
                string dirName = AppConfig.DefaultConfigFilePath;
                fileName = string.Format("{0}\\{1}.xml", dirName, this.GetType().Name);
            }
            return ReadXml(fileName);
        }
        public bool ReadXml(string fileName)
        {
            bool rv = false;
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);

                string select_file = fileName;
                if (fileInfo.Exists == false)
                {
                    MessageBox.Show(this.GetType().Name + " File not found\n[File Path : " + fileName + "]");
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Title = "Select XML file : " + this.GetType().Name;
                    dlg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

                    dlg.InitialDirectory = GetAppRootPath();

                    if (DialogResult.OK == dlg.ShowDialog())
                    {
                        select_file = dlg.FileName;
                        ConfigPath.SelectedFolder = select_file;
                    }
                    else
                    {
                        if (DialogResult.Yes == MessageBox.Show("Do you want create new AppConfig file?", this.GetType().Name, MessageBoxButtons.YesNo))
                        {
                            FolderBrowserDialog dlg2 = new FolderBrowserDialog();
                            dlg2.Description = "AppConfig File Folder Path to create";
                            dlg2.SelectedPath = GetAppRootPath();
                            if (DialogResult.OK == dlg2.ShowDialog())
                            {
                                select_file = string.Format("{0}\\{1}.xml", dlg2.SelectedPath, this.GetType().Name);
                                ConfigPath.SelectedFolder = select_file;
                                this.WriteXml();
                            }
                        }
                    }
                }
                StreamReader sr = new StreamReader(select_file);
                XmlSerializer xmlSer = new XmlSerializer(typeof(AppConfig));
                AppConfig appConfig = xmlSer.Deserialize(sr) as AppConfig;
                string version = appConfig.Version;
                FolderSelect configPath = appConfig.ConfigPath;
                sr.Close();

                // Configuration Folder에 있는 AppConfig를 읽어와서 parameter update
                sr = new StreamReader(AppConfigFile);
                xmlSer = new XmlSerializer(typeof(AppConfig));
                AppConfig config = xmlSer.Deserialize(sr) as AppConfig;
                config.Version = version;
                config.ConfigPath = configPath;
                sr.Close();
                ////////////////////////////////////////////////////////////////////////////////////

                rv = true;
            }
            catch(Exception err) //(Exception err)
            {
                ExceptionLog.WriteLog(method, String.Format(err.ToString()));
            }

            return rv;
        }

        public bool WriteXml()
        {
            if (m_Mutex != null) m_Mutex.WaitOne();

            string fileName = AppConfigFile;
            if (string.IsNullOrEmpty(AppConfigFile))
            {
                string dirName = AppConfig.DefaultConfigFilePath;
                Directory.CreateDirectory(dirName);
                fileName = string.Format("{0}\\{1}.xml", dirName, this.GetType().Name);
            }

            bool rv = WriteXml(fileName);
            if (m_Mutex != null) m_Mutex.ReleaseMutex();

            return rv;
        }
        public bool WriteXml(string fileName)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            StreamWriter sw = null;
            XmlSerializer xmlSer = new XmlSerializer(this.GetType());

            try
            {
                FileInfo file = new FileInfo(fileName);
                if (file.Exists)
                {
                    file.CopyTo(fileName + ".old", true);
                }

                sw = new StreamWriter(fileName);
                xmlSer.Serialize(sw, this);
                sw.Close();

                ConfigPath.SelectedFolder = file.DirectoryName;
                return true;
            }
            catch (Exception err)
            {
                err.ToString();
                m_Mutex.ReleaseMutex();
                ExceptionLog.WriteLog(method, String.Format(err.ToString()));

                return false;
            }
        }

        #endregion
    }
}
