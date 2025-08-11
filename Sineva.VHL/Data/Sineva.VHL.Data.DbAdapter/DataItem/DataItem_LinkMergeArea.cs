using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_LinkMergeArea : DataItem
    {
        #region Fields
        private int m_AreaID;
        private double m_AreaVelocity;
        private List<int> m_NodeIdList = new List<int>();
        private List<int> m_LinkIdList = new List<int>();
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public int AreaID
        {
            get { return m_AreaID; }
            set { m_AreaID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double AreaVelocity
        {
            get { return m_AreaVelocity; }
            set { m_AreaVelocity = value; }
        }
        [DatabaseSettingAttribute(true)]
        public List<int> NodeIdList
        {
            get { return m_NodeIdList; }
            set { m_NodeIdList = value; }
        }
        [DatabaseSettingAttribute(true)]
        public List<int> LinkIdList
        {
            get { return m_LinkIdList; }
            set { m_LinkIdList = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int AreaCount
        {
            get { return LinkIdList.Count; }
        }
        #endregion
        #region Constructor
        public DataItem_LinkMergeArea()
        {

        }
        public DataItem_LinkMergeArea(int id, double velocity, string nodeLists)
        {
            this.AreaID = id;
            this.AreaVelocity = velocity;
            this.NodeIdList.Clear();

            string[] nodes = nodeLists.Split(',');
            int node = -1;
            foreach (string temp in nodes)
            {
                if (int.TryParse(temp, out node)) { this.NodeIdList.Add(node); }
            }
        }
        #endregion

        #region Methods
        public int CompareWithLinkID(List<int> linkIds)
        {
            try
            {
                int result = -1;
                if (m_LinkIdList.Count > 0)
                {
                    int first = m_LinkIdList.First();
                    int index = linkIds.IndexOf(first);
                    if (index != -1)
                    {
                        if (index + LinkIdList.Count <= linkIds.Count)
                        {
                            bool compare = true;
                            for (int j = 0; j < LinkIdList.Count; j++)
                            {
                                compare &= linkIds[index + j] == LinkIdList[j] ? true : false;
                            }
                            if (compare) result = index;
                        }
                    }
                }

                return result;
            }
            catch
            {
                return -1;
            }
        }
        #endregion

    }
}
