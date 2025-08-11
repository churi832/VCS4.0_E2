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
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.GUI
{
    public partial class MotorForm : GeneralForm, IFormUpdate
    {
        private bool m_UpdateNeed = false;
        public MotorForm()
        {
            InitializeComponent();
            SetUserControlAuthority(AuthorizationLevel.Operator);
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
            this.timer1.Enabled = false;
        }

        #endregion

        private void MotorForm_Load(object sender, EventArgs e)
        {
            if (XFunc.IsRunTime())
            {
                this.timer1.Interval = 100;
                this.timer1.Start();
                this.timer1.Enabled = true;

                this.servoControlView1.Initialize();
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.servoControlView1.UpdateState();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public void SetUserControlAuthority(AuthorizationLevel level)
        {
            this.servoControlView1.SetAuthority(level);
            this.toolStripButtonSave.Enabled = level >= AuthorizationLevel.Maintenance;
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            this.servoControlView1.Save();
            ServoManager.Instance.WriteXml();
        }
    }
}
