using Sineva.VHL.Library;
using Sineva.VHL.Library.Common;
using System;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_PIODevice : DataItem
    {
        #region Fields
        PIODeviceType m_DeviceType;
        int m_NodeD;
        int m_PIOID;
        int m_PIOCH;
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public PIODeviceType DeviceType
        {
            get { return m_DeviceType; }
            set { m_DeviceType = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int NodeID
        {
            get { return m_NodeD; }
            set { m_NodeD = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int PIOID
        {
            get { return m_PIOID; }
            set { m_PIOID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int PIOCH
        {
            get { return m_PIOCH; }
            set { m_PIOCH = value; }
        }
        #endregion
        #region Constructor
        public DataItem_PIODevice()
        {

        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_PIODevice source)
        {
            try
            {
                this.m_DeviceType = source.DeviceType;
                this.m_NodeD = source.NodeID;
                this.m_PIOID = source.PIOID;
                this.m_PIOCH = source.PIOCH;
            }
            catch
            {
            }
        }
        public DataItem_PIODevice GetCopyOrNull()
        {
            try
            {
                return (DataItem_PIODevice)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }

        public bool CompareWith(DataItem_PIODevice target)
        {
            try
            {
                bool result = true;
                result &= (this.m_DeviceType == target.DeviceType);
                result &= (this.m_NodeD == target.NodeID);
                result &= (this.m_PIOID == target.PIOID);
                result &= (this.m_PIOCH == target.PIOCH);
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
