using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Sineva.VHL.IF.OCS
{
    [Serializable()]
    public class OCSCommManager
    {
        #region Singleton
        public static readonly OCSCommManager Instance = new OCSCommManager();
        #endregion

        #region Fields
        private string m_FileName;
        private string m_FilePath;
        private bool m_Initialized = false;

        private OCSStatus m_OcsStatus = new OCSStatus();
        private OCSComm m_OcsComm = new OCSComm();

        private Thread m_ConnectionThread;
        private bool m_ThreadState = true;
        #endregion

        #region Properties
        [XmlIgnore, Browsable(false)]
        public OCSStatus OcsStatus
        {
            get { return m_OcsStatus; }
            set { m_OcsStatus = value; }
        }
        [Category("! OCS Communication"), DeviceSetting(true, true)]
        public OCSComm OcsComm
        {
            get { return m_OcsComm; }
            set { m_OcsComm = value; }
        }
        [XmlIgnore, Browsable(false)]
        public bool ConnectRequest { get;set; }
        [XmlIgnore, Browsable(false)]
        public bool DisconnectRequest { get; set; }
        #endregion

        #region Constructor
        public OCSCommManager()
        {

        }
        #endregion
        #region Methods
        public bool Initialize()
        {
            if (m_Initialized) return true;

            ReadXml();
            OcsComm.Initialize();

            m_ConnectionThread = new Thread(new ThreadStart(ConnectionThread));
            m_ConnectionThread.IsBackground = true;
            m_ConnectionThread.Start();

            m_Initialized = true;
            return m_Initialized;
        }
        public void Dispose()
        {
            m_ThreadState = false;
            m_Initialized = false;
        }
        #endregion
        #region Thread
        private void ConnectionThread()
        {
            string temp = string.Empty;
            int SeqNo = 0;
            while (m_ThreadState)
            {
                try
                {
                    int seqNo = SeqNo;
                    switch (seqNo)
                    {
                        case 0:
                            {
                                if (!AppConfig.AppMainInitiated) break;
                                if (OcsComm.ConnectIng == false && OcsComm.DisconnectIng == false)
                                {
                                    if (ConnectRequest)
                                    {
                                        ConnectRequest = false;
                                        OcsComm.Connect();
                                    }
                                    else if (DisconnectRequest)
                                    {
                                        DisconnectRequest = false;
                                        OcsComm.Disconnect();
                                    }
                                }
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                finally
                {
                    Thread.Sleep(10);
                }
            }
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

                var helperXml = new XmlHelper<OCSCommManager>();
                OCSCommManager mng = helperXml.Read(fileName);
                if (mng != null)
                {
                    this.OcsComm = mng.OcsComm;
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
            if (string.IsNullOrEmpty(m_FileName))
            {
                string dirName = AppConfig.DefaultConfigFilePath;
                Directory.CreateDirectory(dirName);

                m_FilePath = string.Format("{0}\\{1}.xml", dirName, GetDefaultFileName());
            }

            WriteXml(m_FileName);
        }
        public void WriteXml(string fileName)
        {
            try
            {
                var helperXml = new XmlHelper<OCSCommManager>();
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
                    filePath = AppConfig.Instance.XmlOCSPath;
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
