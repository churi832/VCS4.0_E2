using Sineva.VHL.Data;
using Sineva.VHL.Device.Assem;
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
    public partial class ucDevGripperPio : UCon, IUpdateUCon
    {
        #region Fields
        private DevGripperPIO m_devPIO = null;
        #endregion

        #region Constructor
        public ucDevGripperPio() : base(OperateMode.Manual)
        {
            InitializeComponent();
        }
        #endregion

        #region Methods - Interface
        public bool Initialize()
        {
            bool rv = true;
            m_devPIO = DevicesManager.Instance.DevGripperPIO;

            // Name Update
            this.lbInput1.Text = string.Format("{0}", m_devPIO.DiGripperOpen.MyName);
            this.lbInput2.Text = string.Format("{0}", m_devPIO.DiGripperClose.MyName);
            this.lbInput3.Text = string.Format("{0}", m_devPIO.DiHoistHome.MyName);
            this.lbInput4.Text = string.Format("{0}", m_devPIO.DiHoistUp.MyName);
            this.lbInput5.Text = string.Format("{0}", m_devPIO.DiHoistLimit.MyName);
            this.lbInput6.Text = string.Format("{0}", m_devPIO.DiLeftProductExist.MyName);
            this.lbInput7.Text = string.Format("{0}", m_devPIO.DiRightProductExist.MyName);
            this.lbInput8.Text = string.Format("{0}", m_devPIO.DoGripperOpen.MyName);
            this.lbOutput1.Text = string.Format("{0}", m_devPIO.DoGripperClose.MyName);
            this.lbOutput2.Text = string.Format("{0}", "NONE");
            this.lbOutput3.Text = string.Format("{0}", "NONE");
            this.lbOutput4.Text = string.Format("{0}", "NONE");
            this.lbOutput5.Text = string.Format("{0}", "NONE");
            this.lbOutput6.Text = string.Format("{0}", "NONE");
            this.lbOutput7.Text = string.Format("{0}", "NONE");
            this.lbOutput8.Text = string.Format("{0}", "NONE");

            // IO Tag
            ucLed1.LedIOTag = m_devPIO.DiGripperOpen.IsValid ? m_devPIO.DiGripperOpen.Di : null;
            ucLed2.LedIOTag = m_devPIO.DiGripperClose.IsValid ? m_devPIO.DiGripperClose.Di : null;
            ucLed3.LedIOTag = m_devPIO.DiHoistHome.IsValid ? m_devPIO.DiHoistHome.Di : null;
            ucLed4.LedIOTag = m_devPIO.DiHoistUp.IsValid ? m_devPIO.DiHoistUp.Di : null;
            ucLed5.LedIOTag = m_devPIO.DiHoistLimit.IsValid ? m_devPIO.DiHoistLimit.Di : null;
            ucLed6.LedIOTag = m_devPIO.DiLeftProductExist.IsValid ? m_devPIO.DiLeftProductExist.Di : null;
            ucLed7.LedIOTag = m_devPIO.DiRightProductExist.IsValid ? m_devPIO.DiRightProductExist.Di : null;
            ucLed8.LedIOTag = m_devPIO.DoGripperOpen.IsValid ? m_devPIO.DoGripperOpen.Do : null;
            ucLed9.LedIOTag = m_devPIO.DoGripperClose.IsValid ? m_devPIO.DoGripperClose.Do : null;
            ucLed10.LedIOTag = null;
            ucLed11.LedIOTag = null;
            ucLed12.LedIOTag = null;
            ucLed13.LedIOTag = null;
            ucLed14.LedIOTag = null;
            ucLed15.LedIOTag = null;
            ucLed16.LedIOTag = null;

            ucLedGo.LedIOTag = m_devPIO.PioComm.DiGo.IsValid ? m_devPIO.PioComm.DiGo.Di : null;
            ucLedSelect.LedIOTag = m_devPIO.PioComm.DoSelect.IsValid ? m_devPIO.PioComm.DoSelect.Do : null;
            return rv;
        }

        public void UpdateState()
        {
            try
            {
                bool enable = EqpStateManager.Instance.OpMode == OperateMode.Manual;
                ucLed9.Enabled = enable;
                ucLed10.Enabled = enable;
                ucLed11.Enabled = enable;
                ucLed12.Enabled = enable;
                ucLed13.Enabled = enable;
                ucLed14.Enabled = enable;
                ucLed15.Enabled = enable;
                ucLed16.Enabled = enable;

                lbPioID.Text = string.Format("{0}", m_devPIO.PioComm.PIO_ID);
                lbPioCH.Text = string.Format("{0}", m_devPIO.PioComm.PIO_CH);
            }
            catch (Exception ex)
            {
                string log = string.Format("UserControl : [{0}]-[{1}]\n{2}", this.GetType(), this.Name, ex.Message);
                ExceptionLog.WriteLog(log);
            }
        }
        #endregion

        #region Methods
        #endregion
    }
}
