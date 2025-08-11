using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Library.IO
{
    public partial class ucLed : UCon, IUpdateUCon
    {
        #region Fields
        private Bitmap m_OnImage;
        private Bitmap m_OffImage;
        private IoTag m_LedIOTag;
        #endregion

        #region Properties
        [Category("! I/O Tag Select")]
        public IoTag LedIOTag
        {
            get { return m_LedIOTag; }
            set { m_LedIOTag = value; }
        }
        #endregion

        #region Constructor
        public ucLed()
        {
            InitializeComponent();
            pbLamp.BackgroundImage = Sineva.VHL.Library.IO.Properties.Resources.BasicLamp_Off;
            pbLamp.BackgroundImageLayout = ImageLayout.Stretch;
        }
        #endregion

        #region Methods - Interface
        public bool Initialize()
        {
            bool rv = true;
            m_OffImage = Sineva.VHL.Library.IO.Properties.Resources.BasicLamp_Off;
            m_OnImage = Sineva.VHL.Library.IO.Properties.Resources.BasicLamp_On;
            return rv;
        }
        public void UpdateState()
        {
            if (this.Visible == false) return;
            if (this.m_LedIOTag == null || this.m_LedIOTag.GetChannel() == null) return;

            bool on = m_LedIOTag.GetChannel().GetDi();
            if (on) pbLamp.BackgroundImage = m_OnImage;
            else pbLamp.BackgroundImage = m_OffImage;
        }
        #endregion

        #region Methods
        private void pbLamp_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.m_LedIOTag == null || this.m_LedIOTag.GetChannel() == null) return;
            if (this.m_LedIOTag.IoType == IoType.DO)
            {
                bool on = m_LedIOTag.GetChannel().GetDi();
                m_LedIOTag.GetChannel().SetDo(!on);
            }
        }
        #endregion

    }
}
