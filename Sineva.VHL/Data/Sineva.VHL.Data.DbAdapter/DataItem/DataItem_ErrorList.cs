using Sineva.VHL.Library;
using System;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_ErrorList : DataItem
    {
        #region Fields
        private int m_ID = 0;
        private int m_Code = 0;
        private AlarmType m_Level = AlarmType.Alarm;
        private string m_Unit = string.Empty;
        private string m_Description = string.Empty;
        private string m_Comment = string.Empty;
        private ERRORSTATE m_State = ERRORSTATE.NoError;
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
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
        [DatabaseSettingAttribute(true)]
        public ERRORSTATE State
        {
            get { return m_State; }
            set { m_State = value; }
        }
        #endregion

        #region Constructors
        public DataItem_ErrorList()
        {
        }
        public DataItem_ErrorList(int id, int code, AlarmType level, string unit, string description, string comment)
        {
            try
            {
                this.m_ID = id;
                this.m_Code = code;
                this.m_Level = level;
                this.m_Unit = unit;
                this.m_Description = description;
                this.m_Comment = comment;
                this.m_State = ERRORSTATE.NoError;
            }
            catch
            {
            }
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_ErrorList source)
        {
            try
            {
                this.m_ID = source.ID;
                this.m_Code = source.Code;
                this.m_Level = source.Level;
                this.m_Unit = source.Unit;
                this.m_Description = source.Description;
                this.m_Comment = source.Comment;
                this.m_State = source.State;
            }
            catch
            {
            }
        }
        public DataItem_ErrorList GetCopyOrNull()
        {
            try
            {
                return (DataItem_ErrorList)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public bool CompareWith(DataItem_ErrorList target)
        {
            try
            {
                bool result = true;

                result &= (this.m_ID == target.ID);
                result &= (this.m_Code == target.Code);
                result &= (this.m_Level == target.Level);
                result &= (this.m_Unit == target.Unit);
                result &= (this.m_Description == target.Description);
                result &= (this.m_Comment == target.Comment);
                result &= (this.m_State == target.State);
                return result;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region override
        public override string ToString()
        {
            return string.Format("Name:{0} Code:{1}, Level:{2}, State:{3}", m_Description, m_Code, m_Level, m_State);
        }
        #endregion

    }
}
