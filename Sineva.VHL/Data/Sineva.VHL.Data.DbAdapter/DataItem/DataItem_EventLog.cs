using Sineva.VHL.Library;
using System;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_EventLog : DataItem
    {
        #region Fields
        private DateTime m_Time = DateTime.Now;
        private string m_Module1 = string.Empty;
        private string m_Module2 = string.Empty;
        private string m_Message = string.Empty;
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public DateTime Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Module1
        {
            get { return m_Module1; }
            set { m_Module1 = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Module2
        {
            get { return m_Module2; }
            set { m_Module2 = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }
        #endregion

        #region Constructors
        public DataItem_EventLog()
        {
        }
        public DataItem_EventLog(DateTime occurredTime, string module1, string module2, string msg)
        {
            try
            {
                this.m_Time = occurredTime;
                this.m_Module1 = module1;
                this.m_Module2 = module2;
                this.m_Message = msg;
            }
            catch
            {
            }
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_EventLog source)
        {
            try
            {
                this.m_Time = source.Time;
                this.m_Module1 = source.Module1;
                this.m_Module2 = source.Module2;
                this.m_Message = source.Message;
            }
            catch
            {
            }
        }
        public DataItem_EventLog GetCopyOrNull()
        {
            try
            {
                return (DataItem_EventLog)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public bool CompareWith(DataItem_EventLog target)
        {
            try
            {
                bool result = true;

                result &= (m_Time == target.Time);
                result &= (m_Module1 == target.Module1);
                result &= (m_Module2 == target.Module2);
                result &= (m_Message == target.Message);

                return result;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
