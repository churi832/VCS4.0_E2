using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Data.Alarm
{
    public class AlarmCurrentData
    {
        #region Fields
        private int m_Id;
        private string m_Name;
        private AlarmLevel m_Level;
        private AlarmCode m_Code;
        #endregion

        #region Properties
        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public AlarmLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }
        public AlarmCode Code
        {
            get { return m_Code; }
            set { m_Code = value; }
        }
        #endregion

        #region Constructor
        public AlarmCurrentData()
        {
        }
        public AlarmCurrentData(int id, string name, AlarmLevel level, AlarmCode code)
        {
            this.m_Id = id;
            this.m_Name = name;
            this.m_Level = level;
            this.m_Code = code;
        }
        #endregion

        #region Methods
        public void Clone(AlarmCurrentData alarm)
        {
            this.m_Id = alarm.m_Id;
            this.m_Name = alarm.m_Name;
            this.m_Level = alarm.m_Level;
        }
        #endregion
    }

    public class CurrentAlarms
    {
        #region Fields
        private readonly static object m_LockKey = new object();
        private Dictionary<int, AlarmCurrentData> m_Items = new Dictionary<int, AlarmCurrentData>();
        #endregion

        #region Properties
        public Dictionary<int, AlarmCurrentData> Items
        {
            get { return m_Items; }
            set { m_Items = value; }
        }
        public int Count
        {
            get { return m_Items.Count; }
        }
        #endregion

        #region Constructor
        public CurrentAlarms()
        {
        }
        #endregion

        #region Methods
        public void Clear()
        {
            lock (m_LockKey)
            {
                m_Items.Clear();
            }
        }
        public bool IsAlarm(int id)
        {
            bool exist = false;

            lock (m_LockKey)
            {
                exist = m_Items.ContainsKey(id);
            }
            return exist;
        }
        public void Add(AlarmData alarm)
        {
            lock (m_LockKey)
            {
                if (m_Items.ContainsKey(alarm.ID) == false)
                {
                    AlarmCurrentData currentAlarm = new AlarmCurrentData(alarm.ID, alarm.Name, alarm.Level, alarm.Code);
                    m_Items.Add(alarm.ID, currentAlarm);
                }
            }
        }

        public void Add(AlarmCurrentData alarm)
        {
            lock (m_LockKey)
            {
                if (m_Items.ContainsKey(alarm.Id) == false)
                {
                    m_Items.Add(alarm.Id, alarm);
                }
            }
        }

        public void Remove(int alarmId)
        {
            lock (m_LockKey)
            {
                if (m_Items.ContainsKey(alarmId))
                {
                    m_Items.Remove(alarmId);
                }
            }
        }

        public bool IsHeavyAlarm()
        {
            bool rv = false;
            lock (m_LockKey)
            {
                List<AlarmCurrentData> lists = m_Items.Values.Where(x => x.Level == AlarmLevel.S).ToList();
                if (lists != null && lists.Count > 0)
                {
                    rv = true;
                }
            }
            return rv;
        }
        public bool IsWarningAlarm()
        {
            bool rv = false;
            lock (m_LockKey)
            {
                List<AlarmCurrentData> lists = m_Items.Values.Where(x => x.Level == AlarmLevel.L).ToList();
                if (lists != null && lists.Count > 0)
                {
                    rv = true;
                }
            }
            return rv;
        }

        public List<int> GetCurrentAlarmIds()
        {
            return m_Items.Values.Select(x=>x.Id).ToList();
        }

        public AlarmCurrentData[] GetCurrentAlarms()
        {
            return m_Items.Values.ToArray();
        }
        #endregion
    }

}
