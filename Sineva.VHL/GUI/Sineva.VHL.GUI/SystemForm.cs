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
using Sineva.VHL.Library.IO;
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.GUI
{
    public partial class SystemForm : GeneralForm, IFormUpdate
    {
        private bool m_UpdateNeed = false;

        public SystemForm()
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
            this.timer1.Enabled = false;
        }

        #endregion
        private void SystemForm_Load(object sender, EventArgs e)
        {
            if (XFunc.IsRunTime())
            {
                this.timer1.Interval = 100;
                this.timer1.Start();
                this.timer1.Enabled = true;

                this.viewIoEdit1.Initialize(IoManager.Instance, (IFormUpdate)this, true);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.viewIoEdit1.UpdateState();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public void SetUserControlAuthority(AuthorizationLevel level)
        {
            this.toolAddChart.Enabled = level <= AuthorizationLevel.Maintenance;
            this.toolRemoveAll.Enabled = level <= AuthorizationLevel.Maintenance;
            this.viewIoEdit1.ForceEnable = level <= AuthorizationLevel.Maintenance;
        }

        private void toolAddChart_Click(object sender, EventArgs e)
        {
            try
            {
                Sineva.VHL.Library.IO.IoChannel ch = this.viewIoEdit1.GetSelectedCh();
                if (ch != null)
                {
                    this.trendGraph1.AddDevice(ch);
                    this.trendGraph1.SamplingEnable = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void toolRemoveAll_Click(object sender, EventArgs e)
        {
            try
            {
                this.trendGraph1.RemoveAllDevice();
                this.trendGraph1.SamplingEnable = false;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
    }
}
