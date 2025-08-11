using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.PathFindAlgorithm
{
    [Serializable]
    public class LinkItem
    {
        #region Fields
        private NodeItem m_FromNode = null;
        private NodeItem m_ToNode = null;
        private int m_Id;
        private double m_Weight;
        private double m_Distance = 0.0f;
        private double m_Time = 0.0f;
        #endregion
        #region Properties
        public NodeItem FromNode
        {
            get { return m_FromNode; }
            set { m_FromNode = value; }
        }
        public NodeItem ToNode
        {
            get { return m_ToNode; }
            set { m_ToNode = value; }
        }
        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        public double Weight
        {
            get { return m_Weight; }
            set { m_Weight = value; }
        }
        public double Distance
        {
            get { return m_Distance; }
            set { m_Distance = value; }
        }
        public double Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }
        #endregion
        #region Constructor
        public LinkItem()
        {
            m_FromNode = new NodeItem(null, this);
            m_ToNode = new NodeItem(this, null);
        }
        public LinkItem(int id, double d, double t, double w)
        {
            m_Id = id;
            m_Distance = d;
            m_Time = t;
            m_Weight = w;

            m_FromNode = new NodeItem(null, this);
            m_ToNode = new NodeItem(this, null);
        }
        #endregion
        #region Methods
        #endregion
    }
}
