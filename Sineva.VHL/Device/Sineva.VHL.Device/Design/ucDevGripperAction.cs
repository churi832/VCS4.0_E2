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
    public partial class ucDevGripperAction : UCon, IUpdateUCon
    {
        #region Fields
        private string m_ExceptionMessage = string.Empty;
        private DevGripperPIO m_devGripperPio = null;
        private bool m_Open = false;
        private bool m_Close = false;
        private bool m_Wait = false;
        private UInt32 m_StartTick = 0;
        #endregion

        #region Constructor
        public ucDevGripperAction() : base(OperateMode.Manual)
        {
            InitializeComponent();
        }
        #endregion

        #region Methods - Interface
        public bool Initialize()
        {
            bool rv = true;
            m_devGripperPio = DevicesManager.Instance.DevGripperPIO;
            if (m_devGripperPio.IsValid)
            {
                dsdGripperStatus.FirstStateIoTag = m_devGripperPio.DiGripperOpen.Di;
                dsdGripperStatus.SecondStateIoTag = m_devGripperPio.DiGripperClose.Di;
            }
            return rv;
        }
        public override void CancelOperation()
        {
            m_Open = false;
            m_Close = false;
            cbRepeat.CheckState = CheckState.Unchecked;
        }
        public void UpdateState()
        {
            try
            {
                bool enable = EqpStateManager.Instance.OpMode == OperateMode.Manual;
                enable &= !m_Open;
                enable &= !m_Close;
                // Button Enable
                if (m_devGripperPio.IsValid)
                {
                    bool openEnable = true;
                    openEnable &= !m_devGripperPio.DiHoistHome.IsDetected;
                    openEnable &= !m_devGripperPio.DiHoistUp.IsDetected;
                    openEnable &= !m_devGripperPio.DiHoistLimit.IsDetected;
                    openEnable &= !m_devGripperPio.DiLeftProductExist.IsDetected;
                    openEnable &= !m_devGripperPio.DiRightProductExist.IsDetected;
                    bool down = DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis().GetCurPosition() < -10.0f;
                    if (down)
                    {
                        bool down_openEnable = true;
                        down_openEnable &= !m_devGripperPio.DiHoistLimit.IsDetected;
                        down_openEnable &= m_devGripperPio.DiHoistUp.IsDetected;
                        down_openEnable &= m_devGripperPio.DiHoistHome.IsDetected;
                        openEnable |= down_openEnable;
                    }
                    btnGripperOpen.Enabled = openEnable && enable && !m_devGripperPio.IsGripperOpen();
                    btnGripperClose.Enabled = enable && !m_devGripperPio.IsGripperClose();
                }
                if (m_Wait)
                {
                    if (XFunc.GetTickCount() - m_StartTick < 2000) return;
                    m_Wait = false;
                }
                // Action
                if (m_Open || m_Close)
                {
                    if (m_devGripperPio.IsValid)
                    {
                        if (m_Open && !m_Wait)
                        {
                            int rv = m_devGripperPio.GripperOpen();
                            if (rv >= 0)
                            {
                                m_Open = false;
                                m_Close = cbRepeat.Checked;
                                m_Wait = cbRepeat.Checked;
                                m_StartTick = XFunc.GetTickCount();
                                if (rv > 0) MessageBox.Show("Open Alarm : {0}", m_devGripperPio.ErrorMessage);
                            }
                        }
                        if (m_Close && !m_Wait)
                        {
                            int rv = m_devGripperPio.GripperClose();
                            if (rv >= 0)
                            {
                                m_Close = false;
                                m_Open = cbRepeat.Checked;
                                m_Wait = cbRepeat.Checked;
                                m_StartTick = XFunc.GetTickCount();
                                if (rv > 0) MessageBox.Show("Close Alarm : {0}", m_devGripperPio.ErrorMessage);
                            }
                        }
                    }
                    else
                    {
                        m_Open = false;
                        m_Close = false;
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
        private void btnGripperOpen_Click(object sender, EventArgs e)
        {
            if (!m_devGripperPio.IsValid) return;
            if (EqpStateManager.Instance.OpMode != OperateMode.Manual) return;
            m_Open = true;
            ButtonLog.WriteLog("btnGripperOpen_Click", string.Format("OPEN"));
        }

        private void btnGripperClose_Click(object sender, EventArgs e)
        {
            if (!m_devGripperPio.IsValid) return;
            if (EqpStateManager.Instance.OpMode != OperateMode.Manual) return;
            m_Close = true;
            ButtonLog.WriteLog("btnGripperOpen_Click", string.Format("CLOSE"));
        }
        #endregion
    }
}
