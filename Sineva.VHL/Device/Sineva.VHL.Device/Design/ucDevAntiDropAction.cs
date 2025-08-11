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
    public partial class ucDevAntiDropAction : UCon, IUpdateUCon
    {
        #region Fields
        private string m_ExceptionMessage = string.Empty;
        private DevAntiDrop m_devFront = null;
        private DevAntiDrop m_devRear = null;

        private bool m_FrontLock = false;
        private bool m_FrontUnlock = false;
        private bool m_RearLock = false;
        private bool m_RearUnlock = false;
        private bool m_Wait = false;
        private UInt32 m_StartTick = 0;
        #endregion
        #region Constructor
        public ucDevAntiDropAction() : base(OperateMode.Manual)
        {
            InitializeComponent();
        }
        #endregion

        #region Methods - Interface
        public bool Initialize()
        {
            bool rv = true;

            m_devFront = DevicesManager.Instance.DevFrontAntiDrop;
            m_devRear = DevicesManager.Instance.DevRearAntiDrop;
            if (m_devFront.IsValid)
            {
                dsdFrontAntiDrop.FirstStateIoTag = m_devFront.DiForward.Di;
                dsdFrontAntiDrop.SecondStateIoTag = m_devFront.DiBackward.Di;
            }
            if (m_devRear.IsValid)
            {
                dsdRearAntiDrop.FirstStateIoTag = m_devRear.DiForward.Di;
                dsdRearAntiDrop.SecondStateIoTag = m_devRear.DiBackward.Di;
            }
            return rv;
        }
        public override void CancelOperation()
        {
            m_RearLock = false;
            m_RearUnlock = false;
            m_FrontLock = false;
            m_FrontUnlock = false;
            cbRepeat.CheckState = CheckState.Unchecked;
        }
        public void UpdateState()
        {
            try
            {
                bool enable = EqpStateManager.Instance.OpMode == OperateMode.Manual;
                enable &= !m_FrontLock;
                enable &= !m_FrontUnlock;
                enable &= !m_RearLock;
                enable &= !m_RearUnlock;
                cbAllSelect.Enabled = enable;
                if (m_devFront.IsValid)
                {
                    btnFrontAntiDropLock.Enabled = enable && !m_devFront.GetLock();
                    btnFrontAntiDropUnlock.Enabled = enable && !m_devFront.GetUnlock();
                }
                if (m_devRear.IsValid)
                {
                    btnRearAntiDropLock.Enabled = enable && !m_devRear.GetLock();
                    btnRearAntiDropUnlock.Enabled = enable && !m_devRear.GetUnlock();
                }
                if (m_Wait)
                {
                    if (XFunc.GetTickCount() - m_StartTick < 2000) return;
                    m_Wait = false;
                }
                // Action
                if (m_FrontLock || m_FrontUnlock)
                {
                    if (m_devFront.IsValid)
                    {
                        if (m_FrontLock && !m_Wait)
                        {
                            int rv = m_devFront.Lock();
                            if (rv >= 0)
                            {
                                m_FrontLock = false;
                                m_FrontUnlock = cbRepeat.Checked;
                                m_Wait = cbRepeat.Checked;
                                m_StartTick = XFunc.GetTickCount();
                                if (rv > 0) MessageBox.Show("Lock Alarm : {0}", EqpAlarm.GetAlarmMsg(rv));
                            }
                        }
                        if (m_FrontUnlock && !m_Wait)
                        {
                            int rv = m_devFront.Unlock();
                            if (rv >= 0)
                            {
                                m_FrontUnlock = false;
                                m_FrontLock = cbRepeat.Checked;
                                m_Wait = cbRepeat.Checked;
                                m_StartTick = XFunc.GetTickCount();
                                if (rv > 0) MessageBox.Show("Unlock Alarm : {0}", EqpAlarm.GetAlarmMsg(rv));
                            }
                        }
                    }
                    else
                    {
                        m_FrontLock = false;
                        m_FrontUnlock = false;
                    }
                }

                if (m_RearLock || m_RearUnlock)
                {
                    if (m_devRear.IsValid)
                    {
                        if (m_RearLock && !m_Wait)
                        {
                            int rv = m_devRear.Lock();
                            if (rv >= 0)
                            {
                                m_RearLock = false;
                                m_RearUnlock = cbRepeat.Checked;
                                m_Wait = cbRepeat.Checked;
                                m_StartTick = XFunc.GetTickCount();
                                if (rv > 0) MessageBox.Show("Lock Alarm : {0}", EqpAlarm.GetAlarmMsg(rv));
                            }
                        }
                        if (m_RearUnlock && !m_Wait)
                        {
                            int rv = m_devRear.Unlock();
                            if (rv >= 0)
                            {
                                m_RearUnlock = false;
                                m_RearLock = cbRepeat.Checked;
                                m_Wait = cbRepeat.Checked;
                                m_StartTick = XFunc.GetTickCount();
                                if (rv > 0) MessageBox.Show("Unlock Alarm : {0}", EqpAlarm.GetAlarmMsg(rv));
                            }
                        }
                    }
                    else
                    {
                        m_RearLock = false;
                        m_RearUnlock = false;
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
        private void btnFrontAntiDropLock_Click(object sender, EventArgs e)
        {
            if (!m_devFront.IsValid) return;
            if (EqpStateManager.Instance.OpMode != OperateMode.Manual) return;
            m_FrontLock = true;
            m_RearLock = cbAllSelect.Checked;

            ButtonLog.WriteLog("btnFrontAntiDropLock_Click", string.Format("cbAllSelect:{0}", cbAllSelect.Checked));
        }

        private void btnFrontAntiDropUnlock_Click(object sender, EventArgs e)
        {
            if (!m_devFront.IsValid) return;
            if (EqpStateManager.Instance.OpMode != OperateMode.Manual) return;
            m_FrontUnlock = true;
            m_RearUnlock = cbAllSelect.Checked;

            ButtonLog.WriteLog("btnFrontAntiDropUnlock_Click", string.Format("cbAllSelect:{0}", cbAllSelect.Checked));
        }

        private void btnRearAntiDropLock_Click(object sender, EventArgs e)
        {
            if (!m_devRear.IsValid) return;
            if (EqpStateManager.Instance.OpMode != OperateMode.Manual) return;
            m_RearLock = true;
            m_FrontLock = cbAllSelect.Checked;

            ButtonLog.WriteLog("btnRearAntiDropLock_Click", string.Format("cbAllSelect:{0}", cbAllSelect.Checked));
        }

        private void btnRearAntiDropUnlock_Click(object sender, EventArgs e)
        {
            if (!m_devRear.IsValid) return;
            if (EqpStateManager.Instance.OpMode != OperateMode.Manual) return;
            m_RearUnlock = true;
            m_FrontLock = cbAllSelect.Checked;

            ButtonLog.WriteLog("btnRearAntiDropUnlock_Click", string.Format("cbAllSelect:{0}", cbAllSelect.Checked));
        }
        #endregion
    }
}
