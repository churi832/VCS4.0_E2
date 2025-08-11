using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Data.Process
{
    [Serializable]
    public class BcrStatus
    {
        #region Fields
        private enBcrCheckDirection m_BcrDirection = enBcrCheckDirection.Both;
        private double m_LeftBcr = 0.0f;
        private double m_RightBcr = 0.0f;
        private double m_VirtualBcr = 0.0f; // PathMaps
        private double m_VirtualRunBcr = 0.0f; // RunPathMaps
        private double m_VirtualStartBcr = 0.0f; // RunPathMaps 시작 시점의 VirtualBcr
        #endregion

        #region Properties
        public enBcrCheckDirection BcrDirection
        {
            get { return m_BcrDirection; }
            set { m_BcrDirection = value; }
        }
        public double LeftBcr
        {
            get { return m_LeftBcr; }
            set { m_LeftBcr = value; }
        }
        public double RightBcr 
        {
            get { return m_RightBcr; }
            set { m_RightBcr = value; }
        }
        public double VirtualBcr 
        {
            get { return m_VirtualBcr; }
            set { m_VirtualBcr = value; }
        }
        public double VirtualRunBcr
        {
            get { return m_VirtualBcr - m_VirtualStartBcr; }
        }
        #endregion

        #region Constructor
        public BcrStatus() 
        { 
        }
        #endregion

        #region Methods
        // SequenceMove 명령 시점
        public void SetVirtualStartBcr()
        {
            m_VirtualStartBcr = m_VirtualBcr;
        }
        #endregion
    }
}
