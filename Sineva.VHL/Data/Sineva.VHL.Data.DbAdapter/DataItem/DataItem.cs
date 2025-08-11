using Sineva.VHL.Library;
using System;
using System.Reflection;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem
    {
        #region Fields
        private string m_DataName = string.Empty;
        #endregion

        #region Properties
        [DatabaseSettingAttribute(false)]
        public string DataName
        {
            get { return m_DataName; }
            set { m_DataName = value; }
        }
        #endregion

        #region Constructors
        public DataItem()
        {
        }
        #endregion

        #region Methods
        public PropertyInfo[] GetProperties()
        {
            if (this == null) return null;

            PropertyInfo[] propertyInfos = this.GetType().GetProperties();
            return propertyInfos;
        }
        #endregion
    }
}
