using Sineva.VHL.Library;
using System;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_ErrorHistory : DataItem
    {
        #region Fields
        private DateTime m_OccurredTime = DateTime.Now;
        private DateTime? m_ClearedTime;
        private int m_ID = 0;
        private string m_LocationID = string.Empty;
        private int m_Code = 0;
        private AlarmType m_Level = AlarmType.Alarm;
        private string m_Unit = string.Empty;
        private string m_Description = string.Empty;
        private string m_Comment = string.Empty;
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public DateTime OccurredTime
        {
            get { return m_OccurredTime; }
            set { m_OccurredTime = value; }
        }
        [DatabaseSettingAttribute(true)]
        public DateTime? ClearedTime
        {
            get { return m_ClearedTime; }
            set { m_ClearedTime = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string LocationID
        {
            get { return m_LocationID; }
            set { m_LocationID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int Code
        {
            get { return m_Code; }
            set { m_Code = value; }
        }
        [DatabaseSettingAttribute(true)]
        public AlarmType Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Unit
        {
            get { return m_Unit; }
            set { m_Unit = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Comment
        {
            get { return m_Comment; }
            set { m_Comment = value; }
        }
        #endregion

        #region Constructors
        public DataItem_ErrorHistory()
        {
        }
        public DataItem_ErrorHistory(DateTime occurredTime, int id, string location, int code, AlarmType level, string unit, string desctiption, string comment)
        {
            try
            {
                this.m_OccurredTime = occurredTime;
                this.m_ID = id;
                this.m_LocationID = location;
                this.m_Code = code;
                this.m_Level = level;
                this.m_Unit = unit;
                this.m_Description = desctiption;
                this.m_Comment = comment;
            }
            catch
            {
            }
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_ErrorHistory source)
        {
            try
            {
                this.m_OccurredTime = source.OccurredTime;
                this.m_ClearedTime = source.ClearedTime;
                this.m_ID = source.ID;
                this.m_LocationID = source.LocationID;
                this.m_Code = source.Code;
                this.m_Level = source.Level;
                this.m_Unit = source.Unit;
                this.m_Description = source.Description;
                this.m_Comment = source.Comment;
            }
            catch
            {
            }
        }
        public DataItem_ErrorHistory GetCopyOrNull()
        {
            try
            {
                return (DataItem_ErrorHistory)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public bool CompareWith(DataItem_ErrorHistory target)
        {
            try
            {
                bool result = true;

                result &= (m_OccurredTime == target.OccurredTime);
                result &= (m_ClearedTime == target.ClearedTime);
                result &= (m_ID == target.ID);
                result &= (m_LocationID == target.LocationID);
                result &= (m_Code == target.Code);
                result &= (m_Level == target.Level);
                result &= (m_Unit == target.Unit);
                result &= (m_Description == target.Description);
                result &= (m_Comment == target.Comment);

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
