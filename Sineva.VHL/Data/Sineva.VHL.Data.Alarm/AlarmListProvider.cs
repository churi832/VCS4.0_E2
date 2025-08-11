using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Library;

namespace Sineva.VHL.Data.Alarm
{
    public class AlarmListProvider
    {
        public static readonly AlarmListProvider Instance = new AlarmListProvider();

        #region Fields
        private readonly static object m_LockKey = new object();
        private Dictionary<int, AlarmData> m_AlarmTable = new Dictionary<int, AlarmData>();
        private string m_FilePath = "";
        #endregion

        #region Properties
        public Dictionary<int, AlarmData> AlarmTable { get => m_AlarmTable; set => m_AlarmTable = value; }
        #endregion

        private AlarmListProvider()
        {
        }

        public void LoadFromDb()
        {
            lock (m_LockKey)
            {
                AlarmTable.Clear();
                foreach (KeyValuePair<int, DataItem_ErrorList> error in DatabaseHandler.Instance.DictionaryErrorList)
                {
                    int key = error.Key;
                    DataItem_ErrorList errorItem = error.Value;

                    AlarmData new_alarm = new AlarmData();
                    new_alarm.ID = errorItem.ID;
                    new_alarm.Code = (AlarmCode)errorItem.Code;
                    new_alarm.Level = (AlarmLevel)errorItem.Level;
                    new_alarm.Unit = errorItem.Unit;
                    new_alarm.Name = errorItem.Description;
                    new_alarm.Comment = errorItem.Comment;
                    AlarmTable.Add(new_alarm.ID, new_alarm);
                }
            }
        }
 
        public Dictionary<int, AlarmData> GetAlarm()
        {
            return AlarmTable;
        }
        public AlarmData GetAlarm(int id)
        {
            AlarmData alarm = null;
            lock (m_LockKey)
            {
                if (AlarmTable.ContainsKey(id))
                {
                    alarm = new AlarmData();
                    alarm.ID = AlarmTable[id].ID;
                    alarm.Code = AlarmTable[id].Code;
                    alarm.Level = AlarmTable[id].Level;
                    alarm.Unit = AlarmTable[id].Unit;
                    alarm.Name = AlarmTable[id].Name;
                    alarm.Comment = AlarmTable[id].Comment;
                }
            }
            return alarm;
        }
        public AlarmData GetAlarm(string name)
        {
            AlarmData alarm = null;
            lock (m_LockKey)
            {
                List<AlarmData> items = AlarmTable.Values.Where(x => x.Name == name).ToList();
                if (items != null && items.Count > 0)
                {
                    alarm = new AlarmData();
                    alarm.ID = items.First().ID;
                    alarm.Code = items.First().Code;
                    alarm.Level = items.First().Level;
                    alarm.Unit = items.First().Unit;
                    alarm.Name = items.First().Name;
                    alarm.Comment = items.First().Comment;
                }
            }
            return alarm;
        }
        public bool IsAlarmExist(string name)
        {
            bool rv = false;
            lock (m_LockKey)
            {
                List<AlarmData> items = AlarmTable.Values.Where(x => x.Name == name).ToList();
                if (items != null && items.Count > 0) rv = true;
            }
            return rv;
        }
        public AlarmData NewAlarm(AlarmCode code, bool heavy, string unit, string name, string comment = "")
        {
            AlarmData newAlarm = null;
            lock (m_LockKey)
            {
                try
                {
                    string buf = string.Format("{0}_{1}", name, comment);
                    string alarm_name = buf.Replace(' ', '_').Replace('.','_').ToUpper();
                    if (IsAlarmExist(alarm_name) == false)
                    {
                        newAlarm = new AlarmData();
                        int alarm_id = 1;
                        if (AlarmTable.Count > 0) alarm_id = AlarmTable.Keys.Last() + 1;
                        while (true)
                        {
                            if (AlarmTable.Keys.Contains(alarm_id) == false) break;
                            alarm_id++;
                        }
                        newAlarm.ID = alarm_id;
                        newAlarm.Code = code;
                        newAlarm.Level = heavy ? AlarmLevel.S : AlarmLevel.L;
                        newAlarm.Unit = unit;
                        newAlarm.Name = alarm_name;
                        newAlarm.Comment = comment;
                        AlarmTable.Add(alarm_id, newAlarm);
                        DatabaseHandler.Instance.AddAlarmList(new DataItem_ErrorList(alarm_id, (int)code, heavy ? AlarmType.Alarm : AlarmType.Warning, unit, alarm_name, comment));
                    }
                    else
                    {
                        newAlarm = GetAlarm(alarm_name);
                        bool update = false;
                        if (newAlarm.Code != code) { newAlarm.Code = code; update = true; }
                        AlarmLevel newLevel = heavy ? AlarmLevel.S : AlarmLevel.L;
                        if (newAlarm.Level != newLevel) { newAlarm.Level = newLevel; update = true; }
                        if (newAlarm.Unit != unit) { newAlarm.Unit = unit; update = true; }
                        if (update)
                        {
                            DataItem_ErrorList item = new DataItem_ErrorList();
                            item.ID = newAlarm.ID;
                            item.Code = (int)newAlarm.Code;
                            item.Level = newAlarm.Level == AlarmLevel.S ? AlarmType.Alarm : AlarmType.Warning;
                            item.Unit = newAlarm.Unit;
                            item.Description = newAlarm.Name;
                            item.Comment = newAlarm.Comment;
                            DatabaseHandler.Instance.QueryErrorList.Update(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }
            return newAlarm;
        }
        #region Method - Save
        public void WriteCsv()
        {
            try
            {
                if (CheckPath())
                {
                    string fileName = m_FilePath + "\\AlarmList" + DateTime.Now.ToString("(yyMMdd_HHmmdd)") + ".csv";
                    if (File.Exists(fileName))
                        fileName = m_FilePath + "\\" + this.GetType().Name + "_bak.csv";
                    using (StreamWriter sw = new StreamWriter(fileName))
                    {
                        sw.WriteLine("ID,Text,Level,Category,UserCode");
                        foreach (AlarmData alarmData in AlarmTable.Values)
                        {
                            string text = string.Format("{0},{1},{2},{3},{4}",
                                alarmData.ID,
                                alarmData.Unit,
                                alarmData.Level,
                                alarmData.Code,
                                alarmData.Comment);
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
            return ok;
        }
        #endregion
    }
}
