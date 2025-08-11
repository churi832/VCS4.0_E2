using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Data.Alarm
{
    public class AlarmHistoryData
    {
        #region Fields
        private AlarmData alarm = new AlarmData();
        private DateTime time;
        private const string _DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        private static string m_OldDateTime = DateTime.Now.ToString(_DateTimeFormat);
        #endregion

        #region Properties
        public AlarmData Alarm
        {
            get { return alarm; }
            set { alarm = value; }
        }
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
        public int Id
        {
            get { return alarm.ID; }
        }
        public AlarmLevel Level
        {
            get { return alarm.Level; }
        }
        public string Name
        {
            get { return alarm.Name; }
        }
        #endregion

        #region Constructor
        public AlarmHistoryData(AlarmData alarm, DateTime time)
        {
            DateTime temp = time;
            string currentTime = temp.ToString(_DateTimeFormat);
            m_OldDateTime = currentTime;

            this.alarm.Clone(alarm);
            this.time = time;
        }
        #endregion

        #region Methods
        #endregion
    }

}
