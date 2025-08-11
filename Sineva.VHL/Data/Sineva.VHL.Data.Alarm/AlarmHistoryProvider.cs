using Sineva.VHL.Data.DbAdapter;
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
    public class AlarmHistoryProvider
    {
        #region Fields
        private readonly static object m_LockKey = new object();
        private Dictionary<DateTime, AlarmHistoryData> m_AlarmHistoryTable = new Dictionary<DateTime, AlarmHistoryData>();
        private AlarmHistoryView m_Viewer = null;
        #endregion

        #region Properties
        public Dictionary<DateTime, AlarmHistoryData> AlarmHistoryTable
        {
            get { return m_AlarmHistoryTable; }
            set { m_AlarmHistoryTable = value; }
        }
        public AlarmHistoryView Viewer
        {
            get { return m_Viewer; }
            set { m_Viewer = value; }
        }
        #endregion

        #region Singleton code...
        public static readonly AlarmHistoryProvider Instance = new AlarmHistoryProvider();
        #endregion

        #region Constructor
        public AlarmHistoryProvider()
        {
        }
        #endregion

        #region Methods
        // Call by sequence
        public void Add(AlarmHistoryData history)
        {
            lock (m_LockKey)
            {
                try
                {
                    if (m_Viewer != null)
                    {
                        m_Viewer.AddAlarm(history);
                    }
                    else
                    {
                        InvokeAdd(history);
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message + this.ToString());
                    ExceptionLog.WriteLog(err.ToString());
                }
            }
        }

        // Call by viewer
        public void InvokeAdd(AlarmHistoryData history)
        {
            try
            {
                DataItem_ErrorHistory item = new DataItem_ErrorHistory();
                item.ID = history.Id;
                item.Code = (int)history.Alarm.Code;
                item.Unit = history.Alarm.Unit;
                item.Level = (AlarmType)history.Level;
                item.Description = history.Alarm.Name;
                item.Comment = history.Alarm.Comment;
                item.OccurredTime = history.Time;

                m_AlarmHistoryTable.Add(history.Time, history);
                DatabaseHandler.Instance.AddAlarmHistory(item);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }

        // Call by sequence
        public void Remove(List<AlarmHistoryData> alarms)
        {
            lock (m_LockKey)
            {
                try
                {
                    foreach (AlarmHistoryData alarm in alarms)
                    {
                        if (m_AlarmHistoryTable.ContainsKey(alarm.Time))
                        {
                            m_AlarmHistoryTable.Remove(alarm.Time);
                        }

                        DataItem_ErrorHistory item = new DataItem_ErrorHistory();
                        item.ID = alarm.Id;
                        item.Code = (int)alarm.Alarm.Code;
                        item.Unit = alarm.Alarm.Unit;
                        item.Level = (AlarmType)alarm.Level;
                        item.Description = alarm.Alarm.Name;
                        item.Comment = alarm.Alarm.Comment;
                        item.OccurredTime = alarm.Time;
                        DatabaseHandler.Instance.RemoveAlarmHistory(item);
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message + this.ToString());
                    ExceptionLog.WriteLog(err.ToString());
                }
            }
        }

        // Call by viewer
        public void Remove(DataGridViewSelectedRowCollection selectedItems)
        {
            try
            {
                List<AlarmHistoryData> alarms = new List<AlarmHistoryData>();
                foreach (DataGridViewRow viewRow in selectedItems)
                {
                    AlarmData data = new AlarmData();
                    data.ID = (int)viewRow.Cells[1].Value;
                    data.Name = (string)viewRow.Cells[2].Value;
                    data.Level = (AlarmLevel)Enum.Parse(typeof(AlarmLevel), (string)viewRow.Cells[3].Value, true);
                    AlarmHistoryData history = new AlarmHistoryData(data, ((DateTime)viewRow.Cells[0].Value));
                    alarms.Add(history);
                }

                Remove(alarms);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }

        public void LoadFromDB()
        {
            lock (m_LockKey)
            {
                m_AlarmHistoryTable.Clear();
                foreach (KeyValuePair<DateTime, DataItem_ErrorHistory> data in DatabaseHandler.Instance.DictionaryErrorHistory)
                {
                    AlarmData alarm = new AlarmData();
                    alarm.ID = data.Value.ID;
                    alarm.Code = (AlarmCode)data.Value.Code;
                    alarm.Level = (AlarmLevel)data.Value.Level;
                    alarm.Unit = data.Value.Unit;
                    alarm.Name = data.Value.Description;
                    alarm.Comment = data.Value.Comment;
                    AlarmHistoryData alarm_history = new AlarmHistoryData(alarm, data.Key);
                    if (!m_AlarmHistoryTable.ContainsKey(data.Key)) 
                        m_AlarmHistoryTable.Add(data.Key, alarm_history);
                }
            }
        }

        public void WriteText(string fileName)
        {
            try
            {
                StreamWriter sw = File.CreateText(fileName);
                sw.AutoFlush = true;

                string txt = "No.\tTime\t\t\tId\tLevel\tDescription";
                sw.WriteLine(txt);
                sw.WriteLine("================================================================");

                int i = 0;
                foreach (AlarmHistoryData item in m_AlarmHistoryTable.Values)
                {
                    txt = string.Format("{0}\t{1}\t{2:d4}\t{3}\t{4}", ++i, item.Time, item.Id, item.Level, item.Name);
                    sw.WriteLine(txt);
                }

                sw.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + this.ToString());
                ExceptionLog.WriteLog(e.ToString());
            }
        }

        public void WriteText()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save As...";
            dlg.CreatePrompt = true;
            dlg.OverwritePrompt = true;
            dlg.FileName = "AlarmHistory.txt";
            dlg.DefaultExt = "txt";
            dlg.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dlg.RestoreDirectory = true;//2009.10.14 okt
            if (DialogResult.OK == dlg.ShowDialog())
            {
                try
                {
                    this.WriteText(dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + this.ToString());
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }
        }
        #endregion
    }
}
