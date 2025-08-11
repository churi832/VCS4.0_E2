using Sineva.VHL.Data;
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

namespace Sineva.VHL.Device
{
    public partial class ucDevCleaner : UCon, IUpdateUCon
    {
        #region Fields
        private string m_ExceptionMessage = string.Empty;
        private DevCleaner m_devCleaner = null;
        private bool m_Start = false;
        private bool m_Wait = false;
        private UInt32 m_StartTick = 0;
        #endregion

        #region Constructor
        public ucDevCleaner() : base(OperateMode.Manual)
        {
            InitializeComponent();
        }
        #endregion

        #region Methods - Interface
        public bool Initialize()
        {
            bool rv = true;
            m_devCleaner = DevicesManager.Instance.DevCleaner;
            return rv;
        }
        public override void CancelOperation()
        {
            m_Start = false;
        }
        public void UpdateState()
        {
            try
            {
                bool run_enable = EqpStateManager.Instance.OpMode == OperateMode.Manual;
                // Button Enable
                if (m_devCleaner.IsValid)
                {
                    bool run = m_devCleaner.IsRun();
                    bool alarm1 = m_devCleaner.DiTempAlarm1.IsDetected;
                    bool alarm2 = m_devCleaner.DiTempAlarm2.IsDetected;
                    bool doorClose = m_devCleaner.IsDoorClose(); ;
                    bool startEnable = !alarm1 && !alarm2 && doorClose;
                    if (!startEnable && m_Start) m_Start = false;
                    btnStart.Enabled = startEnable;

                    if (run && lbRun.BackColor != Color.Green) lbRun.BackColor = Color.Green;
                    else if (!run && lbRun.BackColor != Color.LightGoldenrodYellow) lbRun.BackColor = Color.LightGoldenrodYellow;

                    if (alarm1 && lbTempAlarm1.BackColor != Color.Red) lbTempAlarm1.BackColor = Color.Red;
                    else if (!alarm1 && lbTempAlarm1.BackColor != Color.LightGoldenrodYellow) lbTempAlarm1.BackColor= Color.LightGoldenrodYellow;

                    if (alarm2 && lbTempAlarm2.BackColor != Color.Red) lbTempAlarm2.BackColor = Color.Red;
                    else if (!alarm2 && lbTempAlarm2.BackColor != Color.LightGoldenrodYellow) lbTempAlarm2.BackColor = Color.LightGoldenrodYellow;

                    if (doorClose && lbDoorClose.BackColor != Color.LightGoldenrodYellow) lbDoorClose.BackColor = Color.LightGoldenrodYellow;
                    else if (!doorClose && lbDoorClose.BackColor != Color.Red) lbDoorClose.BackColor = Color.Red;

                    if (run_enable)
                    {
                        if (m_Start && !run) { m_devCleaner.Start(); }
                        else if (!m_Start && run) { m_devCleaner.Stop(); }
                    }
                }
            }
            catch (Exception ex)
            {
                if (m_ExceptionMessage != ex.Message)
                {
                    m_ExceptionMessage = ex.Message;
                    string log = string.Format("UserControl : [{0}]-[{1}]\n{2}", this.GetType(), this.Name, ex.Message);
                    ExceptionLog.WriteLog(log);
                }
            }
        }
        #endregion

        #region - Methods
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (m_devCleaner.IsValid)
            {
                if (m_devCleaner.IsRun()) m_Start = false;
                else m_Start = true;
            }
        }
        #endregion
    }
}
