using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_RouteChange : DataItem
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
        public DataItem_RouteChange()
        {
        }
        public DataItem_RouteChange(int id, int start, int end)
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
        public void SetCopy(DataItem_RouteChange source)
        {
            try
            {
                m_ID = source.ID;
                m_StartNode = source.StartNode;
                m_EndNode = source.EndNode;
                m_AreaList = source.AreaList;
            }
            catch
            {
            }
        }
        public DataItem_RouteChange GetCopyOrNull()
        {
            try
            {
                return (DataItem_RouteChange)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public bool CompareWith(DataItem_RouteChange target)
        {
            try
            {
                bool result = true;

                result &= (m_StartNode == target.StartNode);
                result &= (m_EndNode == target.EndNode);
                result &= (m_AreaList == target.AreaList);

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
