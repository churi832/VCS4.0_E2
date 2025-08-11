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
    public partial class ucDevPio : UCon, IUpdateUCon
    {
        #region Fields
        private DevEqPIO m_devPIO = null;
        private OperateMode m_OldMode = OperateMode.None;
        #endregion

        #region Constructor
        public ucDevPio() : base(OperateMode.Manual)
        {
            InitializeComponent();
        }
        #endregion

        #region Methods - Interface
        public bool Initialize()
        {
            bool rv = true;
            m_devPIO = DevicesManager.Instance.DevEqpPIO;

            // Name Update
            this.lbInput1.Text = string.Format("{0}", m_devPIO.DiLDREQ.MyName);
            this.lbInput2.Text = string.Format("{0}", m_devPIO.DiULREQ.MyName);
            this.lbInput3.Text = string.Format("{0}", m_devPIO.DiVA.MyName);
            this.lbInput4.Text = string.Format("{0}", m_devPIO.DiREADY.MyName);
            this.lbInput5.Text = string.Format("{0}", m_devPIO.DiVS0.MyName);
            this.lbInput6.Text = string.Format("{0}", m_devPIO.DiVS1.MyName);
            this.lbInput7.Text = string.Format("{0}", m_devPIO.DiHOAVBL.MyName);
            this.lbInput8.Text = string.Format("{0}", m_devPIO.DiES.MyName);
            this.lbOutput1.Text = string.Format("{0}", m_devPIO.DoVALID.MyName);
            this.lbOutput2.Text = string.Format("{0}", m_devPIO.DoCS1.MyName);
            this.lbOutput3.Text = string.Format("{0}", m_devPIO.DoCS2.MyName);
            this.lbOutput4.Text = string.Format("{0}", m_devPIO.DoAVBL.MyName);
            this.lbOutput5.Text = string.Format("{0}", m_devPIO.DoTRREQ.MyName);
            this.lbOutput6.Text = string.Format("{0}", m_devPIO.DoBUSY.MyName);
            this.lbOutput7.Text = string.Format("{0}", m_devPIO.DoCOMPT.MyName);
            this.lbOutput8.Text = string.Format("{0}", m_devPIO.DoCONT.MyName);

            // IO Tag
            ucLed1.LedIOTag = m_devPIO.DiLDREQ.IsValid ? m_devPIO.DiLDREQ.Di : null;
            ucLed2.LedIOTag = m_devPIO.DiULREQ.IsValid ? m_devPIO.DiULREQ.Di : null;
            ucLed3.LedIOTag = m_devPIO.DiVA.IsValid ? m_devPIO.DiVA.Di : null;
            ucLed4.LedIOTag = m_devPIO.DiREADY.IsValid ? m_devPIO.DiREADY.Di : null;
            ucLed5.LedIOTag = m_devPIO.DiVS0.IsValid ? m_devPIO.DiVS0.Di : null;
            ucLed6.LedIOTag = m_devPIO.DiVS1.IsValid ? m_devPIO.DiVS1.Di : null;
            ucLed7.LedIOTag = m_devPIO.DiHOAVBL.IsValid ? m_devPIO.DiHOAVBL.Di : null;
            ucLed8.LedIOTag = m_devPIO.DiES.IsValid ? m_devPIO.DiES.Di : null;
            ucLed9.LedIOTag = m_devPIO.DoVALID.IsValid ? m_devPIO.DoVALID.Do : null;
            ucLed10.LedIOTag = m_devPIO.DoCS1.IsValid ? m_devPIO.DoCS1.Do : null;
            ucLed11.LedIOTag = m_devPIO.DoCS2.IsValid ? m_devPIO.DoCS2.Do : null;
            ucLed12.LedIOTag = m_devPIO.DoAVBL.IsValid ? m_devPIO.DoAVBL.Do : null;
            ucLed13.LedIOTag = m_devPIO.DoTRREQ.IsValid ? m_devPIO.DoTRREQ.Do : null;
            ucLed14.LedIOTag = m_devPIO.DoBUSY.IsValid ? m_devPIO.DoBUSY.Do : null;
            ucLed15.LedIOTag = m_devPIO.DoCOMPT.IsValid ? m_devPIO.DoCOMPT.Do : null;
            ucLed16.LedIOTag = m_devPIO.DoCONT.IsValid ? m_devPIO.DoCONT.Do : null;

            ucLedGo.LedIOTag = m_devPIO.PioComm.DiGo.IsValid ? m_devPIO.PioComm.DiGo.Di : null;
            ucLedSelect.LedIOTag = m_devPIO.PioComm.DoSelect.IsValid ? m_devPIO.PioComm.DoSelect.Do : null;
            return rv;
        }

        public void UpdateState()
        {
            try
            {
                if (m_OldMode != EqpStateManager.Instance.OpMode)
                {
                    m_OldMode = EqpStateManager.Instance.OpMode;

                    bool enable = EqpStateManager.Instance.OpMode == OperateMode.Manual;
                    ucLed9.Enabled = enable;
                    ucLed10.Enabled = enable;
                    ucLed11.Enabled = enable;
                    ucLed12.Enabled = enable;
                    ucLed13.Enabled = enable;
                    ucLed14.Enabled = enable;
                    ucLed15.Enabled = enable;
                    ucLed16.Enabled = enable;
                }
                string temp = string.Format("{0}", m_devPIO.PioComm.PIO_ID);
                if (lbPioID.Text != temp) lbPioID.Text = temp;
                temp = string.Format("{0}", m_devPIO.PioComm.PIO_CH);
                if (lbPioCH.Text != temp) lbPioCH.Text = temp;
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
