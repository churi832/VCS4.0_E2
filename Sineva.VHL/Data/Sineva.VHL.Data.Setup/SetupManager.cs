using Sineva.VHL.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Data.Setup
{
    public class SetupManager
    {
        #region Fields
        public readonly static SetupManager Instance = new SetupManager();
        private bool m_Initialized = false;
        private string m_FileName;
        private string m_FilePath;

        private SetupOperation m_SetupOperation = new SetupOperation();
        private SetupPIO m_SetupPIO = new SetupPIO();
        private SetupSafty m_SetupSafty = new SetupSafty();
        private SetupHoist m_SetupHoist = new SetupHoist();
        private SetupSocket m_SetupSocket = new SetupSocket();
        //private SetupTeachingOffset m_SetupTeachingOffset = new SetupTeachingOffset();
        private SetupVision m_SetupVision = new SetupVision();
        private SetupCommon m_SetupCommon = new SetupCommon();
        private SetupJCS m_SetupJCS = new SetupJCS();
        private SetupOCS m_SetupOCS = new SetupOCS();
        private SetupWheel m_SetupWheel = new SetupWheel();
        private static Mutex m_Mutex = new Mutex();
        #endregion

        #region Properties
        public SetupOperation SetupOperation
        {
            get { return m_SetupOperation; }
            set { m_SetupOperation = value; }
        }
        public SetupPIO SetupPIO
        {
            get { return m_SetupPIO; }
            set { m_SetupPIO = value; }
        }
        public SetupSafty SetupSafty
        {
            get { return m_SetupSafty; }
            set { m_SetupSafty = value; }
        }
        public SetupHoist SetupHoist
        {
            get { return m_SetupHoist; }
            set { m_SetupHoist = value; }   
        }
        public SetupSocket SetupSocket
        {
            get { return m_SetupSocket; }
            set { m_SetupSocket = value; }
        }
        //public SetupTeachingOffset SetupTeachingOffset
        //{
        //    get { return m_SetupTeachingOffset; }
        //    set { m_SetupTeachingOffset = value; }
        //}
        public SetupVision SetupVision
        {
            get { return m_SetupVision; }
            set { m_SetupVision = value; }
        }
        public SetupCommon SetupCommon
        {
            get { return m_SetupCommon; }
            set { m_SetupCommon = value; }
        }
        public SetupJCS SetupJCS
        {
            get { return m_SetupJCS; }
            set { m_SetupJCS = value; }
        }
        public SetupOCS SetupOCS
        {
            get { return m_SetupOCS; }
            set { m_SetupOCS = value; }
        }
        public SetupWheel SetupWheel
        {
            get { return m_SetupWheel; }
            set { m_SetupWheel = value; }
        }
        #endregion

        #region Constructor
        public SetupManager() 
        { 
        }
        #endregion

        #region Method
        public bool Initialize()
        {
            m_Initialized = ReadXml();
            return m_Initialized;
        }
        #endregion

        #region [Xml Read/Write]
        public bool ReadXml()
        {
            if (CheckPath())
            {
                return ReadXml(m_FilePath);
            }
            else return false;
        }
        public bool ReadXml(string filePath)
        {
            try
            {
                
                if (Directory.Exists(filePath))
                {
                    string s = m_FilePath + "\\" + m_SetupOperation.GetType().Name + ".xml";
                    if (File.Exists(s))
                    {
                        var helperOperation = new XmlHelper<SetupOperation>();
                        SetupOperation setup_operation = helperOperation.Read(s);
                        if (setup_operation != null) m_SetupOperation = setup_operation;
                    }
                    s = m_FilePath + "\\" + m_SetupPIO.GetType().Name + ".xml";
                    if (File.Exists(s))
                    {
                        var helperPIO = new XmlHelper<SetupPIO>();
                        SetupPIO setup_pio = helperPIO.Read(s);
                        if (setup_pio != null) m_SetupPIO = setup_pio;
                    }
                    s = m_FilePath + "\\" + m_SetupSafty.GetType().Name + ".xml";
                    if (File.Exists(s))
                    {
                        var helperSafty = new XmlHelper<SetupSafty>();
                        SetupSafty setup_safty = helperSafty.Read(s);
                        if (setup_safty != null) m_SetupSafty = setup_safty;
                    }
                    s = m_FilePath + "\\" + m_SetupHoist.GetType().Name + ".xml";
                    if (File.Exists(s))
                    {
                        var helperHoist = new XmlHelper<SetupHoist>();
                        SetupHoist setup_hoist = helperHoist.Read(s);
                        if (setup_hoist != null) m_SetupHoist = setup_hoist;
                    }
                    s = m_FilePath + "\\" + m_SetupSocket.GetType().Name + ".xml";
                    if (File.Exists(s))
                    {
                        var helperSocket = new XmlHelper<SetupSocket>();
                        SetupSocket setup_socket = helperSocket.Read(s);
                        if (setup_socket != null) m_SetupSocket = setup_socket;
                    }
                    //s = m_FilePath + "\\" + m_SetupTeachingOffset.GetType().Name + ".xml";
                    //if (File.Exists(s))
                    //{
                    //    var helperTeachingOffset = new XmlHelper<SetupTeachingOffset>();
                    //    SetupTeachingOffset setup_teachingoffset = helperTeachingOffset.Read(s);
                    //    if (setup_teachingoffset != null) m_SetupTeachingOffset = setup_teachingoffset;
                    //}
                    s = m_FilePath + "\\" + m_SetupVision.GetType().Name + ".xml";
                    if (File.Exists(s))
                    {
                        var helperVision = new XmlHelper<SetupVision>();
                        SetupVision setup_vision = helperVision.Read(s);
                        if (setup_vision != null) m_SetupVision = setup_vision;
                    }
                    s = m_FilePath + "\\" + m_SetupCommon.GetType().Name + ".xml";
                    if (File.Exists(s))
                    {
                        var helperCommon = new XmlHelper<SetupCommon>();
                        SetupCommon setup_common = helperCommon.Read(s);
                        if (setup_common != null) m_SetupCommon = setup_common;
                    }
                    s = m_FilePath + "\\" + m_SetupJCS.GetType().Name + ".xml";
                    if (File.Exists(s))
                    {
                        var helperJcs = new XmlHelper<SetupJCS>();
                        SetupJCS setup_jcs = helperJcs.Read(s);
                        if (setup_jcs != null) m_SetupJCS = setup_jcs;
                    }
                    s = m_FilePath + "\\" + m_SetupOCS.GetType().Name + ".xml";
                    if (File.Exists(s))
                    {
                        var helperOcs = new XmlHelper<SetupOCS>();
                        SetupOCS setup_ocs = helperOcs.Read(s);
                        if (setup_ocs != null) m_SetupOCS = setup_ocs;
                    }
                    s = m_FilePath + "\\" + m_SetupWheel.GetType().Name + ".xml";
                    if (File.Exists(s))
                    {
                        var helperWheel = new XmlHelper<SetupWheel>();
                        SetupWheel setup_wheel = helperWheel.Read(s);
                        if (setup_wheel != null) m_SetupWheel = setup_wheel;
                    }
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
            // 2020.12.13 : Mapping (Full Map) 중 SetupFrame / Frame Template / Stick Template 사용 중복 접근 Exception 발생
            // Monitoring 필요..
            m_Mutex.WaitOne();
            {
                if (string.IsNullOrEmpty(m_FilePath))
                {
                    string dirName = AppConfig.DefaultConfigFilePath;
                    Directory.CreateDirectory(dirName);
                }

                WriteXml(m_FilePath);
            }
            m_Mutex.ReleaseMutex();
        }
        public void SaveSetupOperation()
        {
            try
            {
                string s = this.m_FilePath + "\\" + m_SetupOperation.GetType().Name + ".xml";
                var helperOperation = new XmlHelper<SetupOperation>();
                helperOperation.Save(s, m_SetupOperation);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void SaveSetupPIO()
        {
            try
            {
                string s = this.m_FilePath + "\\" + m_SetupPIO.GetType().Name + ".xml";
                var helperPIO = new XmlHelper<SetupPIO>();
                helperPIO.Save(s, m_SetupPIO);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void SaveSetupCommon()
        {
            try
            {
                string s = this.m_FilePath + "\\" + m_SetupCommon.GetType().Name + ".xml";
                var helperCommon = new XmlHelper<SetupCommon>();
                helperCommon.Save(s, m_SetupCommon);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void SaveSetupSafty()
        {
            try
            {
                string s = this.m_FilePath + "\\" + m_SetupSafty.GetType().Name + ".xml";
                var helperSafty = new XmlHelper<SetupSafty>();
                helperSafty.Save(s, m_SetupSafty);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void SaveSetupHoist()
        {
            try
            {
                string s = this.m_FilePath + "\\" + m_SetupHoist.GetType().Name + ".xml";
                var helperHoist = new XmlHelper<SetupHoist>();
                helperHoist.Save(s, m_SetupHoist);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void SaveSetupSocket()
        {
            try
            {
                string s = this.m_FilePath + "\\" + m_SetupSocket.GetType().Name + ".xml";
                var helperSocket = new XmlHelper<SetupSocket>();
                helperSocket.Save(s, m_SetupSocket);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void SaveSetupTeachingOffset()
        {
            try
            {
                //string s = this.m_FilePath + "\\" + m_SetupTeachingOffset.GetType().Name + ".xml";
                //var helperTeachingOffset = new XmlHelper<SetupTeachingOffset>();
                //helperTeachingOffset.Save(s, m_SetupTeachingOffset);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void SaveSetupVision()
        {
            try
            {
                string s = this.m_FilePath + "\\" + m_SetupVision.GetType().Name + ".xml";
                var helperVision = new XmlHelper<SetupVision>();
                helperVision.Save(s, m_SetupVision);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void SaveSetupJcs()
        {
            try
            {
                string s = this.m_FilePath + "\\" + m_SetupJCS.GetType().Name + ".xml";
                var helperJcs = new XmlHelper<SetupJCS>();
                helperJcs.Save(s, m_SetupJCS);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void SaveSetupOcs()
        {
            try
            {
                string s = this.m_FilePath + "\\" + m_SetupOCS.GetType().Name + ".xml";
                var helperOcs = new XmlHelper<SetupOCS>();
                helperOcs.Save(s, m_SetupOCS);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void SaveSetupWheel()
        {
            try
            {
                string s = this.m_FilePath + "\\" + m_SetupWheel.GetType().Name + ".xml";
                var helperWheel = new XmlHelper<SetupWheel>();
                helperWheel.Save(s, m_SetupWheel);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void WriteXml(string filePath)
        {
            try
            {
                SaveSetupOperation();
                SaveSetupPIO();
                SaveSetupCommon();
                SaveSetupSafty();
                SaveSetupHoist();
                SaveSetupSocket();
                SaveSetupTeachingOffset();
                SaveSetupVision();
                SaveSetupJcs();
                SaveSetupOcs();
                SaveSetupWheel();
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
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
                    filePath = AppConfig.Instance.XmlSetupPath;
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
                    ok = true;
                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
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
