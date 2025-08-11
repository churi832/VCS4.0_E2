using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
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
    public partial class DatabaseForm : GeneralForm, IFormUpdate
    {
        private bool m_UpdateNeed = false;
        public DatabaseForm()
        {
            InitializeComponent();
        }

        #region IFormUpdate 멤버

        public bool UpdateNeed
        {
            get
            {
                return m_UpdateNeed;
            }
            set
            {
                m_UpdateNeed = value;
            }
        }
        public void KillTimer()
        {
        }
        #endregion

        private void DatabaseForm_Load(object sender, EventArgs e)
        {
            if (XFunc.IsRunTime())
            {
                this.databaseView1.Initialize();
            }
        }

        public void SetUserControlAuthority(AuthorizationLevel level)
        {
        }
    }
}
