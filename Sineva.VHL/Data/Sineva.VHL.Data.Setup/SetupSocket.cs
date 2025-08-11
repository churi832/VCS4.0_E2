using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Data.Setup
{
    [Serializable]
    public partial class SetupSocket
    {
        #region Fields
        private string m_Touch_IPAddress = "127.0.0.1";
        private int m_Touch_PortNumber = 502;

        private string m_GL310_IPAddress = "10.110.1.3";
        private int m_GL310_PortNumber = 9004;
        #endregion

        #region Properties
        [Category("Touch"), DisplayName("IP Address")]
        public string Touch_IPAddress
        {
            get { return m_Touch_IPAddress; }
            set { m_Touch_IPAddress = value; }
        }
        [Category("Touch"), DisplayName("Port Number")]
        public int Touch_PortNumber
        {
            get { return m_Touch_PortNumber; }
            set { m_Touch_PortNumber = value; }
        }
        [Category("SOS_GL310"), DisplayName("IP Address")]
        public string GL310_IPAddress 
        {
            get { return m_GL310_IPAddress; }
            set { m_GL310_IPAddress = value; }
        }
        [Category("SOS_GL310"), DisplayName("Port Number")]
        public int GL310_PortNumber 
        {
            get { return m_GL310_PortNumber; }
            set { m_GL310_PortNumber = value; }
        }
        #endregion

        #region Constructor
        public SetupSocket()
        {

        }
        #endregion

        #region Methods
        #endregion

    }
}
