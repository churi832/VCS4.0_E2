using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Data.EventLog
{
    public class EventLogProvider
    {
        #region Singleton code...
        public static readonly EventLogProvider Instance = new EventLogProvider();
        #endregion

        #region Fields
        private readonly static object m_LockKey = new object();
        private Dictionary<DateTime, EventLogData> m_EventLogTable = new Dictionary<DateTime, EventLogData>();
        private EventLogView m_Viewer = null;
        #endregion
        #region Properties
        public Dictionary<DateTime, EventLogData> EventLogTable
        {
            get { return m_EventLogTable; }
            set { m_EventLogTable = value; }
        }
        public EventLogView Viewer
        {
            get { return m_Viewer; }
            set { m_Viewer = value; }
        }
        #endregion
        #region Constructor
        public EventLogProvider()
        {

        }
        #endregion

        #region Methods
        // Call by sequence
        public void Add(EventLogData data)
        {
            lock (m_LockKey)
            {
                if (m_Viewer != null)
                {
                    m_Viewer.AddData(data);
                }
                else // without view update
                {
                    InvokeAdd(data);
                }
            }
        }
        public void Add(string module1, string module2, string message)
        {
            //lock (m_LockKey)
            {
                EventLogData data = new EventLogData();
                data.Time = DateTime.Now;
                data.Module1 = module1;
                data.Module2 = module2;
                data.Message = message;

                if (m_Viewer != null)
                {
                    m_Viewer.AddData(data);
                }
                else // without view update
                {
                    InvokeAdd(data);
                }
            }
        }
        // Call by viewer
        public void InvokeAdd(EventLogData data)
        {
            lock (m_LockKey)
            {
                try
                {
                    DataItem_EventLog item = new DataItem_EventLog();
                    item.Time = data.Time;
                    item.Module1 = data.Module1;
                    item.Module2 = data.Module2;
                    item.Message = data.Message;

                    m_EventLogTable.Add(data.Time, data);
                    DatabaseHandler.Instance.AddEventLog(item);

                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message + this.ToString());
                    ExceptionLog.WriteLog(err.ToString());
                }
            }
        }
        // Call by sequence
        public void Remove(List<EventLogData> datas)
        {
            lock (m_LockKey)
            {
                try
                {
                    foreach (EventLogData data in datas)
                    {
                        if (m_EventLogTable.ContainsKey(data.Time))
                        {
                            m_EventLogTable.Remove(data.Time);
                        }

                        DataItem_EventLog item = new DataItem_EventLog();
                        item.Time = data.Time;
                        item.Module1 = data.Module1;
                        item.Module2 = data.Module2;
                        item.Message = data.Message;
                        DatabaseHandler.Instance.RemoveEventLog(item);
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
                List<EventLogData> datas = new List<EventLogData>();
                foreach (DataGridViewRow viewRow in selectedItems)
                {
                    EventLogData data = new EventLogData();
                    data.Time = (DateTime)viewRow.Cells[1].Value;
                    data.Module1 = (string)viewRow.Cells[2].Value;
                    data.Module2 = (string)viewRow.Cells[3].Value;
                    data.Message = (string)viewRow.Cells[4].Value;

                    datas.Add(data);
                }

                Remove(datas);
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
                m_EventLogTable.Clear();
                foreach (KeyValuePair<DateTime, DataItem_EventLog> item in DatabaseHandler.Instance.DictionaryEventLog)
                {
                    EventLogData data = new EventLogData();
                    data.Time = item.Value.Time;
                    data.Module1 = item.Value.Module1;
                    data.Module2 = item.Value.Module2;
                    data.Message = item.Value.Message;

                    m_EventLogTable.Add(item.Key, data);
                }
            }
        }
        public void WriteText(string fileName)
        {
            try
            {
                StreamWriter sw = File.CreateText(fileName);
                sw.AutoFlush = true;

                string txt = "Time\t\t\tModule1\tModule2\tMessage";
                sw.WriteLine(txt);
                sw.WriteLine("================================================================");

                int i = 0;
                foreach (EventLogData item in m_EventLogTable.Values)
                {
                    txt = string.Format("{0}\t\t\t{1}\t{2}\t{3}", ++i, item.Time, item.Module1, item.Module2, item.Message);
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
