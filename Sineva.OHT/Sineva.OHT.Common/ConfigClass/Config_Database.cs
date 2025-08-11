using System;
using System.ComponentModel;

namespace Sineva.OHT.Common
{
    [Serializable()]
    public class Config_Database : Config, IConfig
    {
        #region Fields
        private string _Address = string.Empty;
        private string _DBName = string.Empty;
        private string _UserID = string.Empty;
        private string _Password = string.Empty;
        #endregion

        #region Properties
        [Category("Database Connection")]
        [Description("IP Address of OCS Database.")]
        [DisplayName("IP Address")]
        [ConfigAttribute(SaveName = "IPAddress")]
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }
        [Category("Database Connection")]
        [Description("Name of OCS Database.")]
        [ConfigAttribute(SaveName = "DatabaseName")]
        public string DBName
        {
            get { return _DBName; }
            set { _DBName = value; }
        }
        [Category("Database Connection")]
        [Description("User ID for access to OCS Database.")]
        [ConfigAttribute(SaveName = "User")]
        public string UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        [Category("Database Connection")]
        [Description("Password for access to OCS Database.")]
        [DisplayName("Log-In Password")]
        [ConfigAttribute(SaveName = "Password")]
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }
        #endregion

        #region Constructors
        public Config_Database()
        {
            _Category = "Database";
        }
        #endregion

        #region Methods
        public string GetDBConnectionString()
        {
            try
            {
                string connectionString = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}", _Address, _DBName, _UserID, _Password);

                //if (_Address == "localhost")
                //{
                //    connectionString = string.Format("Data Source={0};Initial Catalog={1}", _Address, _DBName);
                //}
                //else
                //{
                //    connectionString = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}", _Address, _DBName, _UserID, _Password);
                //}

                return connectionString;
            }
            catch
            {
                return string.Empty;
            }
        }
        public Config_Database GetCopyOrNull()
        {
            try
            {
                Config_Database clone = new Config_Database();

                clone.Address = this._Address;
                clone.DBName = this._DBName;
                clone.UserID = this._UserID;
                clone.Password = this._Password;

                return clone;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
