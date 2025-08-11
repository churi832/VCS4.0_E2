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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Sineva.VHL.Device
{
    public partial class ucDevOutRiggerAction : UCon, IUpdateUCon
    {
        #region Fields
        private string m_ExceptionMessage = string.Empty;
        private DevOutRigger m_devFront = null;
        private DevOutRigger m_devRear = null;

        private bool m_FrontLock = false;
        private bool m_FrontUnlock = false;
        private bool m_RearLock = false;
        private bool m_RearUnlock = false;
        private bool m_Wait = false;
        private UInt32 m_StartTick = 0;
        #endregion

        #region Constructor
        public ucDevOutRiggerAction() : base(OperateMode.Manual)
        {
            InitializeComponent();
        }
        #endregion

        #region Methods - Interface
        public bool Initialize()
        {
            bool rv = true;
            m_devFront = DevicesManager.Instance.DevFrontOutRigger;
            m_devRear = DevicesManager.Instance.DevRearOutRigger;
            dsdFrontOutRiggerStatus.FirstStateIoTag = m_devFront.DiLock.Di;
            dsdFrontOutRiggerStatus.SecondStateIoTag = m_devFront.DiUnlock.Di;
            dsdRearOutRiggerStatus.FirstStateIoTag = m_devRear.DiLock.Di;
            dsdRearOutRiggerStatus.SecondStateIoTag = m_devRear.DiUnlock.Di;
            return rv;
        }
        public override void CancelOperation()
        {
            m_FrontLock = false;
            m_FrontUnlock = false;
            m_RearLock = false;
            m_RearUnlock = false;
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

                // Enable Button
                if (m_devFront.IsValid)
                {
                    btnFrontOutRiggerLock.Enabled = enable && !(m_devFront.GetLock() || m_devFront.GetLockState());
                    btnFrontOutRiggerUnlock.Enabled = enable && !(m_devFront.GetUnlock() || m_devFront.GetUnlockState());
                }
                if (m_devRear.IsValid)
                { 
                    btnRearOutRiggerLock.Enabled = enable && !(m_devRear.GetLock() || m_devRear.GetLockState());
                    btnRearOutRiggerUnlock.Enabled = enable && !(m_devRear.GetUnlock() || m_devRear.GetUnlockState());
                }
                if (m_Wait)
                {
                    if (XFunc.GetTickCount() - m_StartTick < 1000) return;
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
        private void btnFrontOutRiggerLock_Click(object sender, EventArgs e)
        {
            if (!m_devFront.IsValid) return;
            if (EqpStateManager.Instance.OpMode != OperateMode.Manual) return;
            m_FrontLock = true;
            m_RearLock = cbAllSelect.Checked;
            ButtonLog.WriteLog("btnFrontOutRiggerLock_Click", string.Format("cbAllSelect={0}", cbAllSelect.Checked));
        }

        private void btnFrontOutRiggerUnlock_Click(object sender, EventArgs e)
        {
            if (!m_devFront.IsValid) return;
            if (EqpStateManager.Instance.OpMode != OperateMode.Manual) return;
            m_FrontUnlock = true;
            m_RearUnlock = cbAllSelect.Checked;
            ButtonLog.WriteLog("btnFrontOutRiggerUnlock_Click", string.Format("cbAllSelect={0}", cbAllSelect.Checked));
        }

        private void btnRearOutRiggerLock_Click(object sender, EventArgs e)
        {
            if (!m_devRear.IsValid) return;
            if (EqpStateManager.Instance.OpMode != OperateMode.Manual) return;
            m_RearLock = true;
            m_FrontLock = cbAllSelect.Checked;
            ButtonLog.WriteLog("btnRearOutRiggerLock_Click", string.Format("cbAllSelect={0}", cbAllSelect.Checked));
        }

        private void btnRearOutRiggerUnlock_Click(object sender, EventArgs e)
        {
            if (m_devRear.IsValid) return;
            if (EqpStateManager.Instance.OpMode != OperateMode.Manual) return;
            m_RearUnlock = true;
            m_RearUnlock = cbAllSelect.Checked;
            ButtonLog.WriteLog("btnRearOutRiggerUnlock_Click", string.Format("cbAllSelect={0}", cbAllSelect.Checked));
        }
        #endregion
    }
}
