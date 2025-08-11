using Sineva.VHL.Library;
using System;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_ManualActionHistory : DataItem
    {
        #region Fields
        private DateTime m_ActionTime = DateTime.Now;
        private string m_UserID = string.Empty;
        private string m_ViewName = string.Empty;
        private string m_Description = string.Empty;
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public DateTime ActionTime
        {
            get { return m_ActionTime; }
            set { m_ActionTime = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string UserID
        {
            get { return m_UserID; }
            set { m_UserID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string ViewName
        {
            get { return m_ViewName; }
            set { m_ViewName = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }
        #endregion

        #region Constructors
        public DataItem_ManualActionHistory()
        {
        }
        public DataItem_ManualActionHistory(DateTime actionTime, string userID, string viewName, string description)
        {
            try
            {
                this.m_ActionTime = actionTime;
                this.m_UserID = userID;
                this.m_ViewName = viewName;
                this.m_Description = description;
            }
            catch
            {
            }
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_ManualActionHistory source)
        {
            try
            {
                this.m_ActionTime = source.ActionTime;
                this.m_UserID = source.UserID;
                this.m_ViewName = source.ViewName;
                this.m_Description = source.Description;
            }
            catch
            {
            }
        }
        public DataItem_ManualActionHistory GetCopyOrNull()
        {
            try
            {
                return (DataItem_ManualActionHistory)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public bool CompareWith(DataItem_ManualActionHistory target)
        {
            try
            {
                bool result = true;

                result &= (this.m_ActionTime == target.ActionTime);
                result &= (this.m_UserID == target.UserID);
                result &= (this.m_ViewName == target.ViewName);
                result &= (this.m_Description == target.Description);

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
