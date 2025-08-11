using Sineva.VHL.Library;
using Sineva.VHL.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_DestinationChange : DataItem
    {
        #region Fields
        private int m_ID = 0;
        private int m_StartNode = 0;
        private int m_EndNode = 0;
        private List<int> m_AreaList = new List<int>();
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int StartNode
        {
            get { return m_StartNode; }
            set { m_StartNode = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int EndNode
        {
            get { return m_EndNode; }
            set { m_EndNode = value; }
        }
        [DatabaseSettingAttribute(true)]
        public List<int> AreaList
        {
            get { return m_AreaList; }
            set { m_AreaList = value; }
        }
        #endregion

        #region Constructors
        public DataItem_DestinationChange()
        {
        }
        public DataItem_DestinationChange(int id, int start, int end)
        {
            try
            {
                this.m_ID = id;
                this.m_StartNode = start;
                this.m_EndNode = end;
            }
            catch
            {
            }
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_DestinationChange source)
        {
            try
            {
                ID = source.m_ID;
                StartNode = source.m_StartNode;
                EndNode = source.m_EndNode;
                AreaList = source.m_AreaList;
            }
            catch
            {
            }
        }
        public DataItem_DestinationChange GetCopyOrNull()
        {
            try
            {
                return (DataItem_DestinationChange)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public bool CompareWith(DataItem_DestinationChange target)
        {
            try
            {
                bool result = true;

                result &= (StartNode == target.StartNode);
                result &= (EndNode == target.EndNode);
                result &= (AreaList == target.AreaList);

                return result;
            }
            catch
            {
                return false;
            }
        }
        public override string ToString()
        {
            return string.Format("{0}_{1},{2}", m_StartNode, m_EndNode, string.Join("-", m_AreaList));
        }
        #endregion
    }
}
