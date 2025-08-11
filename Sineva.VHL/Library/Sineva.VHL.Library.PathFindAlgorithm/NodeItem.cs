using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.PathFindAlgorithm
{
    [Serializable]
    public class NodeItem
    {
        #region Fields
        private LinkItem m_FromLink = new LinkItem();
        private LinkItem m_ToLink = new LinkItem();
        private int m_Id;
        private int m_X;
        private int m_Y;
        #endregion

        #region Properties
        public LinkItem FromLink
        {
            get { return m_FromLink; }
            set { m_FromLink = value; }
        }
        public LinkItem ToLink
        {
            get { return m_ToLink; }
            set { m_ToLink = value; }
        }
        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        public int X
        {
            get { return m_X; }
            set { m_X = value; }
        }
        public int YId
        {
            get { return m_Y; }
            set { m_Y = value; }
        }
        #endregion

        #region Constructor
        public NodeItem(LinkItem fromLink, LinkItem toLink)
        {
            if (fromLink != null) m_FromLink = fromLink;
            if (ToLink != null) m_ToLink = toLink;
        }
        public NodeItem(int id, int x, int y)
        {
            m_Id = id;
            m_X = x;
            m_Y = y;
        }
        #endregion

        #region override
        public override string ToString()
        {
            return m_X.ToString() + "," + m_Y.ToString();
        }
        #endregion
    }
}
