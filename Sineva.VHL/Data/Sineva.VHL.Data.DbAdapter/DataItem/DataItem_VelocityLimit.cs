using Sineva.VHL.Library;
using System;
using static System.Windows.Forms.LinkLabel;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_VelocityLimit : DataItem
    {
        #region Fields
        private int m_ID = 0;
        private int m_LinkId = 0;
        private int m_ToLinkId = 0;
        private int m_MaxVelocity = 0;
        private double m_LeftBcrStart = 0;
        private double m_LeftBcrEnd = 0;
        private double m_RightBcrStart = 0;
        private double m_RightBcrEnd = 0;
        #endregion

        #region Properties
        [DatabaseSettingAttribute(false)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int LinkId
        {
            get { return m_LinkId; }
            set { m_LinkId = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int ToLinkId
        {
            get { return m_ToLinkId; }
            set { m_ToLinkId = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int MaxVelocity
        {
            get { return m_MaxVelocity; }
            set { m_MaxVelocity = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double LeftBcrStart
        {
            get { return m_LeftBcrStart; }
            set { m_LeftBcrStart = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double LeftBcrEnd
        {
            get { return m_LeftBcrEnd; }
            set { m_LeftBcrEnd = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double RightBcrStart
        {
            get { return m_RightBcrStart; }
            set { m_RightBcrStart = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double RightBcrEnd
        {
            get { return m_RightBcrEnd; }
            set { m_RightBcrEnd = value; }
        }
        #endregion

        #region Constructors
        public DataItem_VelocityLimit()
        {
        }
        public DataItem_VelocityLimit(int id, int linkId, int toLinkId, int maxVelocity, double left_bcr0, double left_bcr1, double right_bcr0, double right_bcr1)
        {
            try
            {
                this.m_ID = id;
                this.m_LinkId = linkId;
                this.m_ToLinkId = toLinkId;
                this.m_MaxVelocity = maxVelocity;
                this.m_LeftBcrStart = left_bcr0;
                this.m_LeftBcrEnd = left_bcr1;
                this.m_RightBcrStart = right_bcr0;
                this.m_RightBcrEnd = right_bcr1;
            }
            catch
            {
            }
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_VelocityLimit source)
        {
            try
            {
                this.m_LinkId = source.m_LinkId;
                this.m_ToLinkId= source.m_ToLinkId;
                this.m_MaxVelocity = source.m_MaxVelocity;
                this.m_LeftBcrStart = source.m_LeftBcrStart;
                this.m_LeftBcrEnd = source.m_LeftBcrEnd;
                this.m_RightBcrStart = source.m_RightBcrStart;
                this.m_RightBcrEnd = source.m_RightBcrEnd;
            }
            catch
            {
            }
        }
        public DataItem_VelocityLimit GetCopyOrNull()
        {
            try
            {
                return (DataItem_VelocityLimit)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public bool CompareWith(DataItem_VelocityLimit target)
        {
            try
            {
                bool result = true;
                result &= (this.m_ID == target.m_ID);
                result &= (this.m_LinkId == target.m_LinkId);
                result &= (this.m_ToLinkId == target.m_ToLinkId);
                result &= (this.m_MaxVelocity == target.m_MaxVelocity);
                result &= (this.m_LeftBcrStart == target.m_LeftBcrStart);
                result &= (this.m_LeftBcrEnd == target.m_LeftBcrEnd);
                result &= (this.m_RightBcrStart == target.m_RightBcrStart);
                result &= (this.m_RightBcrEnd == target.m_RightBcrEnd);

                return result;
            }
            catch
            {
                return false;
            }
        }
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5}, {6}, {7}", m_ID,  m_LinkId, m_ToLinkId, m_MaxVelocity, m_LeftBcrStart, m_LeftBcrEnd, m_RightBcrStart, m_RightBcrEnd);
        }
        #endregion
    }
}
