using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Data.Alarm
{
    public delegate void Del_AlarmSet(AlarmData alarm);
    public delegate void Del_AlarmReset(AlarmData alarm);

    public class AlarmCurrentProvider
    {
        #region Fields
        private readonly static object m_LockKey = new object();

        private CurrentAlarms m_AlarmTable = new CurrentAlarms();
        private AlarmCurrentView m_Viewer = null;

        private string m_FilePath = "";
        #endregion

        public event Del_AlarmSet eventSetAlarm = null;
        public event Del_AlarmReset eventResetAlarm = null;

        #region Properties
        public AlarmCurrentView Viewer
        {
            get { return m_Viewer; }
            set { m_Viewer = value; }
        }

        public CurrentAlarms AlarmTable 
        {
            get => m_AlarmTable; 
            set => m_AlarmTable = value; 
        }
        #endregion

        #region Singleton code...
        public static readonly AlarmCurrentProvider Instance = new AlarmCurrentProvider();
        #endregion

        #region Constructor
        private AlarmCurrentProvider()
        {
            AlarmTable.Items.Clear();
        }
        #endregion

        #region Methods
        public bool IsAlarm(AlarmData alarm)
        {
            bool rv = false;
            lock (m_LockKey)
            {
                rv = m_AlarmTable.IsAlarm(alarm.ID);
            }
            return rv;
        }

        public bool IsHeavyAlarm()
        {
            bool rv = false;
            lock (m_LockKey)
            {
                rv = m_AlarmTable.IsHeavyAlarm();
            }
            return rv;
        }

        public bool IsWarningAlarm()
        {
            bool rv = false;
            lock (m_LockKey)
            {
                rv = m_AlarmTable.IsWarningAlarm();
            }
            return rv;
        }

        // Call by sequence
        public void Add(AlarmData alarm)
        {
            lock (m_LockKey)
            {
                try
                {
                    if (m_AlarmTable.IsAlarm(alarm.ID) == false)
                    {
                        if (eventSetAlarm != null)
                            eventSetAlarm(alarm);
                    }
                }
                catch (Exception err)
                {
                    ExceptionLog.WriteLog(err.ToString());
                }
            }
        }

        public void fireResetAlarm(AlarmData alarm)
        {
            if (eventResetAlarm != null) eventResetAlarm(alarm);
        }
        // Call by sequence
        public void Remove(AlarmData alarm)
        {
            lock (m_LockKey)
            {
                try
                {
                    if (eventResetAlarm != null) eventResetAlarm(alarm);
                }
                catch (Exception err)
                {
                    ExceptionLog.WriteLog(err.ToString());
                }
            }
        }
        // Call by viewer
        public void InvokeAdd(AlarmData alarm)
        {
            m_AlarmTable.Add(alarm);
        }

        // Call by viewer
        public void InvokeRemove(AlarmData alarm)
        {
            m_AlarmTable.Remove(alarm.ID);
        }

        public void ResetAllAlarm()
        {
            lock (m_LockKey)
            {
                m_AlarmTable.Clear();
            }
        }

        public List<int> GetCurrentAlarmIds()
        {
            lock (m_LockKey)
            {
                return m_AlarmTable.GetCurrentAlarmIds();
            }
        }
        public AlarmCurrentData[] GetCurrentAlarms()
        {
            return m_AlarmTable.GetCurrentAlarms();
        }
        #endregion

        #region Method - Save
        public void WriteCsv()
        {
            try
            {
                if (CheckPath())
                {
                    string fileName = m_FilePath + "\\" + this.GetType().Name + ".csv";
                    if (File.Exists(fileName))
                        fileName = m_FilePath + "\\" + this.GetType().Name + "_bak.csv";

                    using (StreamWriter sw = new StreamWriter(fileName))
                    {
                        AlarmCurrentData[] curLists = null;//GetCurrentAlarms();
                        for (int i = 0; i < curLists.Length; i++)
                        {
                            string text = string.Format("{0},{1}",
                                    curLists[i].Id,
                                    curLists[i].Name);
                            sw.WriteLine(text);
                        }
                        sw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private bool CheckPath()
        {
            bool ok = false;

            try
            {
                string filePath = m_FilePath;

                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = AppConfig.Instance.XmlAlarmListPath;
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
                        m_FilePath = filePath;
                        ok = true;

                        if (MessageBox.Show("do you want to save seleted folder !", "SAVE", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            AppConfig.Instance.ConfigPath.SelectedFolder = filePath;
                            AppConfig.Instance.WriteXml();
                        }
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
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }

            return ok;
        }
        #endregion
    }
}
