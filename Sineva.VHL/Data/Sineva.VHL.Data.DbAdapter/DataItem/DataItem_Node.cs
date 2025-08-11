using Sineva.VHL.Library;
using System;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_Node : DataItem
    {
        #region Fields
        private int m_NodeID = 0;
        private bool m_UseFlag = false;
        private double m_LocationValue1 = 0;
        private double m_LocationValue2 = 0;
        private NodeType m_Type = NodeType.Normal;
        private int m_CPSZoneID = 0;
        private int m_UBSLevel = 0;
        private int m_UBSCheckSensor = 0;
        private int m_JCSCheck = 0;
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public int NodeID
        {
            get { return m_NodeID; }
            set { m_NodeID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public bool UseFlag
        {
            get { return m_UseFlag; }
            set { m_UseFlag = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double LocationValue1
        {
            get { return m_LocationValue1; }
            set { m_LocationValue1 = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double LocationValue2
        {
            get { return m_LocationValue2; }
            set { m_LocationValue2 = value; }
        }
        [DatabaseSettingAttribute(true)]
        public NodeType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int CPSZoneID
        {
            get { return m_CPSZoneID; }
            set { m_CPSZoneID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int UBSLevel
        {
            get { return m_UBSLevel; }
            set { m_UBSLevel = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int UBSCheckSensor
        {
            get { return m_UBSCheckSensor; }
            set { m_UBSCheckSensor = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int JCSCheck
        {
            get { return m_JCSCheck; }
            set { m_JCSCheck = value; }
        }
        #endregion

        #region Constructors
        public DataItem_Node()
        {
        }
        public DataItem_Node(int id, bool useFlag, double locationValue1, double locationValue2, NodeType type, int ubslevel = 0, int ubssensor = 0, int jcs_check = 0)
        {
            try
            {
                this.m_NodeID = id;
                this.m_UseFlag = useFlag;
                this.m_LocationValue1 = locationValue1;
                this.m_LocationValue2 = locationValue2;
                this.m_Type = type;
                this.m_UBSLevel = ubslevel;
                this.m_UBSCheckSensor = ubssensor;
                this.m_JCSCheck = jcs_check;
            }
            catch
            {
            }
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_Node source)
        {
            try
            {
                this.m_NodeID = source.NodeID;
                this.m_UseFlag = source.UseFlag;
                this.m_LocationValue1 = source.LocationValue1;
                this.m_LocationValue2 = source.LocationValue2;
                this.m_Type = source.Type;
                this.m_UBSLevel = source.UBSLevel;
                this.m_UBSCheckSensor = source.UBSCheckSensor;
                this.m_JCSCheck = source.JCSCheck;
            }
            catch
            {
            }
        }
        public DataItem_Node GetCopyOrNull()
        {
            try
            {
                return (DataItem_Node)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public bool CompareWith(DataItem_Node target)
        {
            try
            {
                bool result = true;

                result &= (this.m_NodeID == target.NodeID);
                result &= (this.m_UseFlag == target.UseFlag);
                result &= (this.m_LocationValue1 == target.LocationValue1);
                result &= (this.m_LocationValue2 == target.LocationValue2);
                result &= (this.m_Type == target.Type);
                result &= (this.m_UBSLevel == target.UBSLevel);
                result &= (this.m_UBSCheckSensor == target.UBSCheckSensor);
                result &= (this.m_JCSCheck == target.JCSCheck);

                return result;
            }
            catch
            {
                return false;
            }
        }
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}, {4}", m_NodeID, m_Type, m_LocationValue1, m_LocationValue2, m_JCSCheck);
        }
        #endregion
    }
}
