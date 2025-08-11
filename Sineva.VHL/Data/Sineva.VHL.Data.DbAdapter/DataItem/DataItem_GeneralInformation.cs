using Sineva.VHL.Library;
using System;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_GeneralInformation : DataItem
    {
        #region Fields
        private GeneralInformationItemName m_ItemName = GeneralInformationItemName.NodeDataVersion;
        private string m_ItemValue = string.Empty;
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public GeneralInformationItemName ItemName
        {
            get { return m_ItemName; }
            set { m_ItemName = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string ItemValue
        {
            get { return m_ItemValue; }
            set { m_ItemValue = value; }
        }
        #endregion

        #region Constructors
        public DataItem_GeneralInformation()
        {
        }
        public DataItem_GeneralInformation(GeneralInformationItemName itemName, string itemValue)
        {
            m_ItemName = itemName;
            m_ItemValue = itemValue;
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_GeneralInformation source)
        {
            try
            {
                this.m_ItemName = source.ItemName;
                this.m_ItemValue = source.ItemValue;
            }
            catch
            {
            }
        }
        public DataItem_GeneralInformation GetCopyOrNull()
        {
            try
            {
                return (DataItem_GeneralInformation)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public bool CompareWith(DataItem_GeneralInformation target)
        {
            try
            {
                bool result = true;

                result &= (this.m_ItemName == target.ItemName);
                result &= (this.m_ItemValue == target.ItemValue);

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
