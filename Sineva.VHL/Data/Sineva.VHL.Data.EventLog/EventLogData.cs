using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Data.EventLog
{
    public class EventLogData
    {
        #region Fields
        private DateTime m_Time;
        private string m_Module1;
        private string m_Module2;
        private string m_Message;
        #endregion

        #region Properties
        public DateTime Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }

        public string Module1
        {
            get { return m_Module1; }
            set { m_Module1 = value; }
        }

        public string Module2
        {
            get { return m_Module2; }
            set { m_Module2 = value; }
        }

        public string Message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }
        #endregion
        public EventLogData()
        {
        }
    }
}
