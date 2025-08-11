using Sineva.VHL.Data.Setup;
using Sineva.VHL.Device;
using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.GUI
{
    public partial class SetupForm : GeneralForm, IFormUpdate
    {
        #region Fields
        private bool m_UpdateNeed = false;
        #endregion

        #region Constructor
        public SetupForm()
        {
            InitializeComponent();
        }
        #endregion

        #region IFormUpdate 멤버
        public bool UpdateNeed
        {
            get { return m_UpdateNeed; }
            set { m_UpdateNeed = value; }
        }
        public void KillTimer()
        {
        }
        #endregion
        #region Methods
        private void SetupForm_Load(object sender, EventArgs e)
        {
            if (XFunc.IsRunTime())
            {
                this.viewDevProperty1.Initialize();
                this.viewSetup1.Initialize();
            }
        }
        public void SetUserControlAuthority(AuthorizationLevel level)
        {
            this.viewSetup1.SetAuthority(level);
            this.toolStripButtonSave.Enabled = level <= AuthorizationLevel.Maintenance;
        }
        #endregion

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            this.viewSetup1.Save();
            this.viewDevProperty1.Save();
        }
    }
}
