using Sineva.VHL.Library;
using System;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_UserInfo : DataItem
    {
        #region Fields
        private string m_UserName = string.Empty;
        private string m_Password = string.Empty;
        private string m_Department = string.Empty;
        private AuthorizationLevel m_Level = AuthorizationLevel.Operator;
        private string m_Description = string.Empty;
        private string m_ClientIp = string.Empty;
        private DateTime m_LoginTime;
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public string UserName
        {
            get { return m_UserName; }
            set { m_UserName = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Password
        {
            get { return m_Password; }
            set { m_Password = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Department
        {
            get { return m_Department; }
            set { m_Department = value; }
        }
        [DatabaseSettingAttribute(true)]
        public AuthorizationLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string ClientIp
        {
            get { return m_ClientIp; }
            set { m_ClientIp = value; }
        }
        [DatabaseSettingAttribute(true)]
        public DateTime LoginTime
        {
            get { return m_LoginTime; }
            set { m_LoginTime = value; }
        }
        #endregion

        #region Constructors
        public DataItem_UserInfo()
        {
        }
        public DataItem_UserInfo(string userName, string password, string department, string description, AuthorizationLevel level)
        {
            this.m_UserName = userName;
            this.m_Password = password;
            this.m_Department = department;
            this.m_Description = description;
            this.m_Level = level;
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_UserInfo source)
        {
            try
            {
                this.m_UserName = source.UserName;
                this.m_Password = source.Password;
                this.m_Department = source.Department;
                this.m_Level = source.Level;
                this.m_Description = source.Description;
            }
            catch
            {
            }
        }
        public DataItem_UserInfo GetCopyOrNull()
        {
            try
            {
                return (DataItem_UserInfo)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public bool CompareWith(DataItem_UserInfo target)
        {
            try
            {
                bool result = true;

                result &= (this.m_UserName == target.UserName);
                result &= (this.m_Password == target.Password);
                result &= (this.m_Department == target.Department);
                result &= (this.m_Level == target.Level);
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
