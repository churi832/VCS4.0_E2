using Sineva.VHL.Library;
using System;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_FrontDetectFilter : DataItem
    {
        #region Fields
        private int m_ID = 0;
        private int m_Area = 0;
        private int m_SensorLevel = 0;
        private int m_LinkId = 0;
        private bool m_UseFlag = false;
        private double m_LeftBcrStart = 0;
        private double m_LeftBcrEnd = 0;
        private double m_RightBcrStart = 0;
        private double m_RightBcrEnd = 0;
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int SensorLevel
        {
            get { return m_SensorLevel; }
            set { m_SensorLevel = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int Area
        {
            get { return m_Area; }
            set { m_Area = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int LinkId
        {
            get { return m_LinkId; }
            set { m_LinkId = value; }
        }
        [DatabaseSettingAttribute(true)]
        public bool UseFlag
        {
            get { return m_UseFlag; }
            set { m_UseFlag = value; }
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
        public DataItem_FrontDetectFilter()
        {
        }
        public DataItem_FrontDetectFilter(int id, int area, int sensor_level, int linkId, bool useflag, double left_bcr0, double left_bcr1, double right_bcr0, double right_bcr1)
        {
            try
            {
                this.m_ID = id;
                this.m_Area = area;
                this.m_SensorLevel = sensor_level;
                this.m_LinkId = linkId;
                this.m_UseFlag = useflag;
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
        public void SetCopy(DataItem_FrontDetectFilter source)
        {
            try
            {
                this.m_ID = source.m_ID;
                this.m_SensorLevel = source.m_SensorLevel;
                this.m_Area = source.m_Area;
                this.m_LinkId = source.m_LinkId;
                this.m_UseFlag = source.m_UseFlag;
                this.m_LeftBcrStart = source.m_LeftBcrStart;
                this.m_LeftBcrEnd = source.m_LeftBcrEnd;
                this.m_RightBcrStart = source.m_RightBcrStart;
                this.m_RightBcrEnd = source.m_RightBcrEnd;
            }
            catch
            {
            }
        }
        public DataItem_FrontDetectFilter GetCopyOrNull()
        {
            try
            {
                return (DataItem_FrontDetectFilter)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public bool CompareWith(DataItem_FrontDetectFilter target)
        {
            try
            {
                bool result = true;

                result &= (this.m_ID == target.m_ID);
                result &= (this.m_SensorLevel == target.m_SensorLevel);
                result &= (this.m_Area == target.m_Area);
                result &= (this.m_LinkId == target.m_LinkId);
                result &= (this.m_UseFlag == target.m_UseFlag);
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
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", m_ID, m_Area, m_SensorLevel, m_LinkId, m_UseFlag, m_LeftBcrStart, m_LeftBcrEnd, m_RightBcrStart, m_RightBcrEnd);
        }
        #endregion
    }
}
