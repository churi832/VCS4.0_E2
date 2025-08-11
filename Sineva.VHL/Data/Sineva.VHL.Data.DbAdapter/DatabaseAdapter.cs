using Sineva.VHL.Library;
using Sineva.VHL.Library.DBProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Data.DbAdapter
{
    public class DatabaseAdapter
    {
        public readonly static DatabaseAdapter Instance = new DatabaseAdapter();

        #region Fields
        private bool m_Initialized = false;
        private string m_FileName;
        private string m_FilePath;

        private string m_VHLAddress = string.Format("127.0.0.1");
        private string m_VHLDBName = string.Format("VHLDB4.0");
        private string m_VHLUserID = string.Format("VHL");
        private string m_VHLPassword = string.Format("VHL12345");

        private Use m_FDCUse = Use.NoUse;
        private string m_FDCAddress = string.Format("127.0.0.1");
        private string m_FDCDBName = string.Format("VHLDB4.0");
        private string m_FDCUserID = string.Format("VHL");
        private string m_FDCPassword = string.Format("VHL12345");

        private JobSession m_JobSession = null;
        private JobSession m_FdcSession = null;
        #endregion

        #region Properties - Database Access
        [Category("VHL Database"), Description("IP Address of VHL Database"), DisplayName("VHL Address")]
        public string VHLAddress
        {
            get { return m_VHLAddress; }
            set { m_VHLAddress = value; }
        }
        [Category("VHL Database"), Description("Name of VHL Database"), DisplayName("VHL Database Name")]
        public string VHLDBName
        {
            get { return m_VHLDBName; }
            set { m_VHLDBName = value; }
        }
        [Category("VHL Database"), Description("User ID of VHL Database"), DisplayName("VHL Log-In User ID")]
        public string VHLUserID
        {
            get { return m_VHLUserID; }
            set { m_VHLUserID = value; }
        }
        [Category("VHL Database"), Description("Password of VHL Database"), DisplayName("VHL Log-In Password")]
        public string VHLPassword
        {
            get { return m_VHLPassword; }
            set { m_VHLPassword = value; }
        }
        [Category("FDC Database"), Description("Usage of FDC Database"), DisplayName("FDC Usage")]
        public Use FDCUse
        {
            get { return m_FDCUse; }
            set { m_FDCUse = value; }
        }
        [Category("FDC Database"), Description("IP Address of FDC Database"), DisplayName("FDC IP Address")]
        public string FDCAddress
        {
            get { return m_FDCAddress; }
            set { m_FDCAddress = value; }
        }
        [Category("FDC Database"), Description("Name of FDC Database"), DisplayName("FDC  Database Name")]
        public string FDCDBName
        {
            get { return m_FDCDBName; }
            set { m_FDCDBName = value; }
        }
        [Category("FDC Database"), Description("User ID of FDC Database"), DisplayName("FDC Log-In User ID")]
        public string FDCUserID
        {
            get { return m_FDCUserID; }
            set { m_FDCUserID = value; }
        }
        [Category("FDC Database"), Description("Password of FDC Database"), DisplayName("FDC Log-In Password")]
        public string FDCPassword
        {
            get { return m_FDCPassword; }
            set { m_FDCPassword = value; }
        }
        #endregion

        #region Constructor
        public DatabaseAdapter()
        {

        }
        #endregion

        #region Methods
        public bool Initialize()
        {
            if (m_Initialized) return true;

            ReadXml();
            JobSession job0 = JobSession.NewInstanceOrNull(GetDBConnectionString());
            if (job0 != null) m_JobSession = job0;
            m_Initialized = job0 != null;

            //if (FDCUse == Use.Use)
            {
                JobSession job1 = JobSession.NewInstanceOrNull(GetDBConnectionFDCString());
                if (job1 != null) m_FdcSession = job1;
                m_Initialized = job1 != null;
            }
            return m_Initialized;
        }
        public string GetDBConnectionString()
        {
            try
            {
                string connectionString = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}", m_VHLAddress, m_VHLDBName, m_VHLUserID, m_VHLPassword);
                return connectionString;
            }
            catch
            {
                return string.Empty;
            }
        }
        public string GetDBConnectionFDCString()
        {
            try
            {
                string connectionString = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}", m_FDCAddress, m_FDCDBName, m_FDCUserID, m_FDCPassword);
                return connectionString;
            }
            catch
            {
                return string.Empty;
            }
        }
        public JobSession GetJobSession()
        {
            return m_JobSession;
        }
        public JobSession GetFDCSession()
        {
            return m_FdcSession;
        }
        #endregion

        #region [Xml Read/Write]
        public bool ReadXml()
        {
            if (CheckPath())
            {
                return ReadXml(m_FileName);
            }
            else return false;
        }
        public bool ReadXml(string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists)
                {
                    m_FileName = fileName;
                }
                else
                {
                    WriteXml();
                }

                var helperXml = new XmlHelper<DatabaseAdapter>();
                DatabaseAdapter mng = helperXml.Read(fileName);
                if (mng != null)
                {
                    this.VHLAddress = mng.VHLAddress;
                    this.VHLDBName = mng.VHLDBName;
                    this.VHLUserID = mng.VHLUserID;
                    this.VHLPassword = mng.VHLPassword;
                    this.FDCUse = mng.FDCUse;
                    this.FDCAddress = mng.FDCAddress;
                    this.FDCDBName = mng.FDCDBName;
                    this.FDCUserID = mng.FDCUserID;
                    this.FDCPassword = mng.FDCPassword;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }

            return true;
        }
        public void WriteXml()
        {
            try
            {
                WriteXml(m_FileName);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void WriteXml(string fileName)
        {
            try
            {
                var helperXml = new XmlHelper<DatabaseAdapter>();
                helperXml.Save(fileName, this);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public bool CheckPath()
        {
            bool ok = false;

            try
            {
                string filePath = m_FilePath;

                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = AppConfig.Instance.XmlDatabasePath;
                }

                if (Directory.Exists(filePath) == false)
                {
                    FolderBrowserDialog dlg = new FolderBrowserDialog();
                    dlg.Description = "Configuration folder select";
                    dlg.SelectedPath = AppConfig.GetSolutionPath();
                    dlg.ShowNewFolderButton = true;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        filePath = dlg.SelectedPath;
                        if (MessageBox.Show("do you want to save seleted folder !", "SAVE", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            AppConfig.Instance.ConfigPath.SelectedFolder = filePath;
                            AppConfig.Instance.WriteXml();
                        }
                        m_FilePath = filePath;
                        m_FileName = string.Format("{0}\\{1}.xml", filePath, GetDefaultFileName());
                        ok = true;
                    }
                    else
                    {
                        ok = false;
                    }
                }
                else
                {
                    m_FilePath = filePath;
                    m_FileName = string.Format("{0}\\{1}.xml", filePath, GetDefaultFileName());
                    ok = true;
                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return ok;
        }
        public string GetDefaultFileName()
        {
            string fileName;
            fileName = this.GetType().Name;
            return fileName;
        }
        #endregion

    }
}
