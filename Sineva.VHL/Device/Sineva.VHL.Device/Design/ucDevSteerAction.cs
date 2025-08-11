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
    public partial class ucDevSteerAction : UCon, IUpdateUCon
    {
        #region Fields
        private string m_ExceptionMessage = string.Empty;
        private DevSteer m_devSteer = null;

        private bool m_FrontLeft = false;
        private bool m_FrontRight = false;
        private bool m_RearLeft = false;
        private bool m_RearRight = false;
        private bool m_Wait1 = false;
        private bool m_Wait2 = false;
        private UInt32 m_StartTick = 0;

        #endregion

        #region Constructor
        public ucDevSteerAction() : base(OperateMode.Manual)
        {
            InitializeComponent();
        }
        #endregion

        #region Methods - Interface
        public bool Initialize()
        {
            bool rv = true;
            m_devSteer = DevicesManager.Instance.DevSteer;
            dsdFrontSteerStatus_Manual.FirstStateIoTag = m_devSteer.FrontSteer.DiSensorFw.Count > 0 ? m_devSteer.FrontSteer.DiSensorFw.First() : null;
            dsdFrontSteerStatus_Manual.SecondStateIoTag = m_devSteer.FrontSteer.DiSensorBw.Count > 0 ? m_devSteer.FrontSteer.DiSensorBw.First() : null;
            dsdRearSteerStatus_Manual.FirstStateIoTag = m_devSteer.FrontSteer.DiSensorFw.Count > 0 ? m_devSteer.RearSteer.DiSensorFw.First() : null;
            dsdRearSteerStatus_Manual.SecondStateIoTag = m_devSteer.FrontSteer.DiSensorBw.Count > 0 ? m_devSteer.RearSteer.DiSensorBw.First() : null;
            return rv;
        }
        public override void CancelOperation()
        {
            m_FrontLeft = false;
            m_FrontRight = false;
            m_RearLeft = false;
            m_RearRight = false;
            cbRepeat.CheckState = CheckState.Unchecked;
        }
        public void UpdateState()
        {
            try
            {
                bool enable = EqpStateManager.Instance.OpMode == OperateMode.Manual;
                enable &= !m_FrontLeft;
                enable &= !m_FrontRight;
                enable &= !m_RearLeft;
                enable &= !m_RearRight;

                // Enable Button
                if (m_devSteer.IsValid)
                {
                    btnFrontSteerLeft.Enabled = enable && m_devSteer.GetFrontSteerDirection() != enSteerDirection.Left;
                    btnFrontSteerRight.Enabled = enable && m_devSteer.GetFrontSteerDirection() != enSteerDirection.Right;
                    btnRearSteerLeft.Enabled = enable && m_devSteer.GetRearSteerDirection() != enSteerDirection.Left;
                    btnRearSteerRight.Enabled = enable && m_devSteer.GetRearSteerDirection() != enSteerDirection.Right;
                    cbAllSelect.Enabled = enable;
                }
                if (m_Wait1 || m_Wait2)
                {
                    if (cbAllSelect.Checked)
                    {
                        if ( m_Wait1 && m_Wait2)
                        {
                            if (XFunc.GetTickCount() - m_StartTick < 2000) return;
                            m_Wait1 = false;
                            m_Wait2 = false;
                        }
                    }
                    else
                    {
                        if (XFunc.GetTickCount() - m_StartTick < 2000) return;
                        m_Wait1 = false;
                        m_Wait2 = false;
                    }
                }
                int rv1 = -1;
                int rv2 = -1;
                // Action
                if (m_FrontLeft || m_FrontRight)
                {
                    if (m_devSteer.IsValid)
                    {
                        if (m_FrontLeft && !m_Wait1)
                        {
                            rv1 = m_devSteer.FrontLeft();
                            if (rv1 >= 0)
                            {
                                m_FrontLeft = false;
                                m_Wait1 = cbRepeat.Checked;
                                m_FrontRight = cbRepeat.Checked;
                                m_devSteer.FrontSteer.ResetOutput();
                                m_StartTick = XFunc.GetTickCount();
                                if (rv1 > 0) MessageBox.Show("Lock Alarm : {0}", EqpAlarm.GetAlarmMsg(rv1));
                            }
                        }
                        if (m_FrontRight && !m_Wait1)
                        {
                            rv1 = m_devSteer.FrontRight();
                            if (rv1 >= 0)
                            {
                                m_FrontRight = false;
                                m_Wait1 = cbRepeat.Checked;
                                m_FrontLeft = cbRepeat.Checked;
                                m_devSteer.FrontSteer.ResetOutput();
                                m_StartTick = XFunc.GetTickCount();
                                if (rv1 > 0) MessageBox.Show("Unlock Alarm : {0}", EqpAlarm.GetAlarmMsg(rv1));
                            }
                        }
                    }
                    else
                    {
                        m_FrontLeft = false;
                        m_FrontRight = false;
                    }
                }
                if (m_RearLeft || m_RearRight)
                {
                    if (m_devSteer.IsValid)
                    {
                        if (m_RearLeft && !m_Wait2)
                        {
                            rv2 = m_devSteer.RearLeft();
                            if (rv2 >= 0)
                            {
                                m_RearLeft = false;
                                m_RearRight = cbRepeat.Checked;
                                m_Wait2 = cbRepeat.Checked;
                                m_devSteer.RearSteer.ResetOutput();
                                m_StartTick = XFunc.GetTickCount();
                                if (rv2 > 0) MessageBox.Show("Lock Alarm : {0}", EqpAlarm.GetAlarmMsg(rv2));
                            }
                        }
                        if (m_RearRight && !m_Wait2)
                        {
                            rv2 = m_devSteer.RearRight();
                            if (rv2 >= 0)
                            {
                                m_RearRight = false;
                                m_RearLeft = cbRepeat.Checked;
                                m_Wait2 = cbRepeat.Checked;
                                m_devSteer.RearSteer.ResetOutput();
                                m_StartTick = XFunc.GetTickCount();
                                if (rv2 > 0) MessageBox.Show("Unlock Alarm : {0}", EqpAlarm.GetAlarmMsg(rv2));
                            }
                        }
                    }
                    else
                    {
                        m_RearLeft = false;
                        m_RearRight = false;
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

        #region Methods
        private void btnFrontSteerLeft_Click(object sender, EventArgs e)
        {
            if (!m_devSteer.IsValid) return;
            if (EqpStateManager.Instance.OpMode != OperateMode.Manual) return;
            m_FrontLeft = true;
            m_RearLeft = cbAllSelect.Checked;
            ButtonLog.WriteLog("btnFrontSteerLeft_Click", string.Format("cbAllSelect={0}", cbAllSelect.Checked));
        }
        private void btnFrontSteerRight_Click(object sender, EventArgs e)
        {
            if (!m_devSteer.IsValid) return;
            if (EqpStateManager.Instance.OpMode != OperateMode.Manual) return;
            m_FrontRight = true;
            m_RearRight = cbAllSelect.Checked;
            ButtonLog.WriteLog("btnFrontSteerRight_Click", string.Format("cbAllSelect={0}", cbAllSelect.Checked));
        }
        private void btnRearSteerLeft_Click(object sender, EventArgs e)
        {
            if (!m_devSteer.IsValid) return;
            if (EqpStateManager.Instance.OpMode != OperateMode.Manual) return;
            m_RearLeft = true;
            m_FrontLeft = cbAllSelect.Checked;
            ButtonLog.WriteLog("btnRearSteerLeft_Click", string.Format("cbAllSelect={0}", cbAllSelect.Checked));
        }
        private void btnRearSteerRight_Click(object sender, EventArgs e)
        {
            if (!m_devSteer.IsValid) return;
            if (EqpStateManager.Instance.OpMode != OperateMode.Manual) return;
            m_RearRight = true;
            m_FrontRight = cbAllSelect.Checked;
            ButtonLog.WriteLog("btnRearSteerRight_Click", string.Format("cbAllSelect={0}", cbAllSelect.Checked));
        }
        #endregion
    }
}
