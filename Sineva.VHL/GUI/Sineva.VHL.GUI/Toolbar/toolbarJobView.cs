using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sineva.VHL.Library;
using Sineva.VHL.Data;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Library.Servo;
using Sineva.VHL.Device;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.Alarm;

namespace Sineva.VHL.GUI
{
    public partial class toolbarJobView : UserControl
    {
        #region Field
        private EqpStateManager m_EqpManager = null;
        private DevicesManager m_DeviceManager = null;
        private bool m_ReadyButtonToggleOn = false;
        #endregion

        #region Constructor
        public toolbarJobView()
        {
            InitializeComponent();

            InitInstance();
            InitControl();
            MainForm.ChangeMode += MainForm_ChangeMode;
            MainForm.ChangeRunMode += MainForm_ChangeRunMode;
        }

        private void MainForm_ChangeRunMode(EqpRunMode mode)
        {
            switch (mode)
            {
                case EqpRunMode.None:
                    break;
                case EqpRunMode.Stop:
                    ButtonActionStart();
                    break;
                case EqpRunMode.Start:
                    ButtonActionStart();
                    break;
                case EqpRunMode.Pause:
                    ButtonActionPause();
                    break;
                case EqpRunMode.Abort:
                    ButtonActionAbort();
                    break;
                default:
                    break;
            }
        }

        private void MainForm_ChangeMode(OperateMode mode)
        {
            switch (mode)
            {
                case OperateMode.None:
                    break;
                case OperateMode.Manual:
                    ButtonActionManual();
                    break;
                case OperateMode.SemiAuto:
                    break;
                case OperateMode.Auto:
                    ButtonActionAuto();
                    break;
                case OperateMode.Recovery:
                    ButtonActionReady();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Event Handler
        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            if (!this.Visible)
                return;

            UpdateObjects();
        }
        private void toolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripButton button = sender as ToolStripButton;

                if (button == toolStripButtonReady) ButtonActionReady();
                else if (button == toolStripButtonAuto) ButtonActionAuto();
                else if (button == toolStripButtonManual) ButtonActionManual();
                else if (button == toolStripButtonStart) ButtonActionStart();
                else if (button == toolStripButtonPause) ButtonActionPause();
                else if (button == toolStripButtonAbort) ButtonActionAbort();

                if (button != null && sender is ToolStripButton) ButtonLog.WriteLog(string.Format("{0}.{1}", this.Name.ToString(), (sender as ToolStripButton).Text));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void Instance_RunModeChanged(EqpRunMode runMode)
        {
            if (this.toolStrip1.InvokeRequired)
            {
                DelVoid_EqpRunMode d = new DelVoid_EqpRunMode(Instance_RunModeChanged);
                this.Invoke(d, runMode);
            }
            else
            {
                try
                {
                    this.toolStripButtonAbort.Image = global::Sineva.VHL.GUI.Properties.Resources.right_abort_default;
                    this.toolStripButtonPause.Image = global::Sineva.VHL.GUI.Properties.Resources.right_pause_default;
                    this.toolStripButtonStart.Image = global::Sineva.VHL.GUI.Properties.Resources.right_start_default;

                    if (runMode == EqpRunMode.Abort)
                    {
                        this.toolStripButtonAbort.Image = global::Sineva.VHL.GUI.Properties.Resources.right_abort_click;
                        Sineva.VHL.Data.EventLog.EventLogHandler.Instance.Add("GUI", "MAIN", "Cycle Abort", true);
                    }
                    else if (runMode == EqpRunMode.Pause)
                    {
                        this.toolStripButtonPause.Image = global::Sineva.VHL.GUI.Properties.Resources.right_pause_click;
                        Sineva.VHL.Data.EventLog.EventLogHandler.Instance.Add("GUI", "MAIN", "Cycle Pause", true);
                    }
                    else
                    {
                        if (runMode == EqpRunMode.Start)
                        {
                            this.toolStripButtonStart.Text = "CYCLE START";
                            this.toolStripButtonStart.Image = global::Sineva.VHL.GUI.Properties.Resources.right_start_click;
                            Sineva.VHL.Data.EventLog.EventLogHandler.Instance.Add("GUI", "MAIN", "Cycle Start", true);
                        }
                        else if (runMode == EqpRunMode.Stop)
                        {
                            this.toolStripButtonStart.Text = "CYCLE STOP";
                            this.toolStripButtonStart.Image = global::Sineva.VHL.GUI.Properties.Resources.right_stop_click;
                            Sineva.VHL.Data.EventLog.EventLogHandler.Instance.Add("GUI", "MAIN", "Cycle Stop", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog (ex.ToString());
                }
            }
        }
        private void Instance_OperateModeChanged(OperateMode opMode)
        {
            if (this.toolStrip1.InvokeRequired)
            {
                DelVoid_OperateMode d = new DelVoid_OperateMode(Instance_OperateModeChanged);
                this.Invoke(d, opMode);
            }
            else
            {
                try
                {
                    this.toolStripButtonAuto.Image = global::Sineva.VHL.GUI.Properties.Resources.right_manual_default;
                    this.toolStripButtonManual.Image = global::Sineva.VHL.GUI.Properties.Resources.right_manual_default;
                    if (opMode == OperateMode.Auto)
                    {
                        this.toolStripButtonAuto.Image = global::Sineva.VHL.GUI.Properties.Resources.right_auto_click;
                    }
                    else if (opMode == OperateMode.Manual)
                    {
                        this.toolStripButtonManual.Image = global::Sineva.VHL.GUI.Properties.Resources.right_manual_click;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }
        }
        #endregion

        #region Method
        private void InitInstance()
        {
            m_EqpManager = EqpStateManager.Instance;
            m_DeviceManager = DevicesManager.Instance;
            EventHandlerManager.Instance.OperateModeChanged += Instance_OperateModeChanged;
            EventHandlerManager.Instance.RunModeChanged += Instance_RunModeChanged;
        }
        private void InitControl()
        {
            tmrUpdate.Interval = 1000;
            tmrUpdate.Start();
        }

        private void UpdateObjects()
        {
            try
            {
                UpdateBgImage();
                UpdateButtonEnable();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void UpdateBgImage()
        {
            try
            {
                bool statusOn, statusIng;

                // Button - Ready
                statusOn = statusIng = true;
                statusOn &= m_EqpManager.EqpInitComp;
                statusIng &= m_EqpManager.EqpInitIng;
                if (statusOn)
                {
                    this.toolStripButtonReady.Image = global::Sineva.VHL.GUI.Properties.Resources.right_ready_click;
                }
                else if (statusIng)
                {
                    //if(XFunc.GetTickCount() - m_ReadyButtonToggleTick > 200)
                    {
                        //m_ReadyButtonToggleTick = XFunc.GetTickCount();
                        m_ReadyButtonToggleOn = !m_ReadyButtonToggleOn;
                        if (m_ReadyButtonToggleOn)
                            this.toolStripButtonReady.Image = global::Sineva.VHL.GUI.Properties.Resources.right_ready_over;
                        else
                            this.toolStripButtonReady.Image = global::Sineva.VHL.GUI.Properties.Resources.right_ready_default;
                    }
                }
                else
                {
                    this.toolStripButtonReady.Image = global::Sineva.VHL.GUI.Properties.Resources.right_ready_default;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog (ex.ToString());
            }
        }
        private void UpdateButtonEnable()
        {
            try
            {
                bool enable;

                // Button - Ready
                enable = true;
                enable &= !m_EqpManager.EqpInitComp & !m_EqpManager.EqpInitIng;
                this.toolStripButtonReady.Enabled = enable;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog (ex.ToString());
            }
        }

        private void ButtonActionReady()
        {
            try
            {
                if (m_EqpManager.EqpInitIng || m_EqpManager.EqpInitComp)
                {
                    return;
                }

                m_EqpManager.EqpInitReq = true;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog (ex.ToString());
            }
        }
        private void ButtonActionAuto()
        {
            try
            {
                //if (GV.AutoModeSwitched == false && AppConfig.Instance.Simulation.MY_DEBUG == false)
                //{
                //    MessageBox.Show("change Auto/Teach Key Switch ! [Teach => Auto]");
                //    return;
                //}
                if (m_DeviceManager.DevSOSUpper != null && (m_DeviceManager.DevSOSUpper.IsValid == false || m_DeviceManager.DevSOSUpper.OBS.IsValid == false))
                {
                    MessageBox.Show("Front Upper Sensor is not vaild,Need Check!");
                    return;
                }
                if (!m_EqpManager.EqpInitComp && !AppConfig.Instance.Simulation.MY_DEBUG)
                {
                    MessageBox.Show("Need Equipment Initialzie !");
                    return;
                }

                bool SearchLinkOk = true;
                SearchLinkOk &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LinkID > 0;
                SearchLinkOk &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentNode.NodeID > 0;
                if (!SearchLinkOk)
                {
                    MessageBox.Show("Search Link Fail.. Check the barcode and the link !");
                    return;
                }

                //Remove 이후 바로 NotAssigned 되는 것 때문에 MTL 내려가는 와중에 OCS로 명령을 받아 움직임..
                //Cylinder에 부딪혀 Servo Alarm 발생..
                //Auto 전환 할 때 NotAssigned 되도록 하자..
                if (ProcessDataHandler.Instance.CurVehicleStatus.GetVehicleStatus() == VehicleState.Removed)
                    ProcessDataHandler.Instance.CurVehicleStatus.SetVehicleStatus(VehicleState.NotAssigned);

                bool heavy_alarm = AlarmCurrentProvider.Instance.IsHeavyAlarm();
                if (heavy_alarm)
                {
                    MessageBox.Show("Heavy Alarm Exist. Check the Alarm !");
                    return;
                }

                string msg1 = string.Empty;
                foreach (_AxisBlock block in ServoManager.Instance.AxisBlocks)
                {
                    if (block.ControlFamily == ServoControlFamily.MXP)
                    {
                        if ((block as AxisBlockMXP).Connected == false) msg1 += string.Format("MXP Link Error");
                    }
                }
                foreach (ServoUnit servo in ServoManager.Instance.ServoUnits)
                {
                    bool sv_on = false, no_alarm = false, in_pos = false, h_end = false;
                    foreach (_Axis ax in servo.GetAxes())
                    {
                        string msg2 = string.Empty;
                        IAxisCommand axis = ax as IAxisCommand;
                        sv_on = (axis.GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
                        no_alarm = (axis.GetAxisCurStatus() & enAxisInFlag.Alarm) != enAxisInFlag.Alarm ? true : false;
                        in_pos = (axis.GetAxisCurStatus() & enAxisInFlag.InPos) == enAxisInFlag.InPos ? true : false;
                        h_end = (axis.GetAxisCurStatus() & enAxisInFlag.HEnd) == enAxisInFlag.HEnd ? true : false;
                        if (!sv_on) msg2 += string.Format("[Servo OFF]");
                        if (!no_alarm) msg2 += string.Format("[Servo Alarm]");
                        if (!in_pos) msg2 += string.Format("[Servo Not Ready]");
                        if (!h_end) msg2 += string.Format("[Servo Not HOME]");
                        bool ready = sv_on && no_alarm && in_pos && h_end;
                        if (!ready) msg1 += string.Format("{0}.{1} : {2}\r\n", servo.GetName(), ax.AxisName, msg2);
                    }
                    if (servo.Repeat) servo.Stop();
                    servo.Repeat = false;
                }

                if (msg1 != string.Empty)
                {
                    EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.OperationConfirm, msg1);
                    return;
                }

                m_EqpManager.SetOpMode(OperateMode.Auto);
                m_EqpManager.SetRunMode(EqpRunMode.Stop);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void ButtonActionSemiAuto()
        {
            m_EqpManager.SetRunMode(EqpRunMode.Stop);
            m_EqpManager.SetOpMode(OperateMode.SemiAuto);
        }
        private void ButtonActionManual()
        {
            m_EqpManager.SetRunMode(EqpRunMode.Stop);
            m_EqpManager.SetOpMode(OperateMode.Manual);
        }
        private void ButtonActionStart()
        {
            try
            {
                if (GV.scmAbortSeqRun.Ing)
                {
                    if (DialogResult.Yes == MessageBox.Show("Now Abort Check Sequence Run state !\r\n Please Check Alarm State !", this.GetType().Name, MessageBoxButtons.YesNo)) return;
                }

                if (m_EqpManager.RunMode == EqpRunMode.Start || m_EqpManager.RunMode == EqpRunMode.Abort)
                    m_EqpManager.SetRunMode(EqpRunMode.Stop);
                else
                {
                    bool heavy_alarm = AlarmCurrentProvider.Instance.IsHeavyAlarm();
                    if (heavy_alarm)
                    {
                        MessageBox.Show("Heavy Alarm Exist. Check the Alarm !");
                        return;
                    }

                    if (m_EqpManager.OpMode == OperateMode.Auto || m_EqpManager.OpMode == OperateMode.SemiAuto)
                    {
                        // Use Check // 필수 사용 항목이 설정되어 있는지 확인하자 !
                        bool no_check = false;
                        string message = "";
                        bool heart_bit_use = true;
                        bool ethercat_link_error = false;
                        foreach (_AxisBlock block in ServoManager.Instance.AxisBlocks)
                        {
                            if (block.ControlFamily == ServoControlFamily.MXP) heart_bit_use &= (block as AxisBlockMXP).HeartBitUsing;
                            ethercat_link_error |= (block as AxisBlockMXP).Connected ? false : true;
                        }

                        if (heart_bit_use == false) { no_check = true; message = "MXP Heart Bit No-Use !\r\nPlease change to use !"; }
                        else if (Data.Setup.SetupManager.Instance.SetupSafty.OBSUpperSensorUse == Use.NoUse)
                        { no_check = true; message = "Upper Forward Detection Sensor No-Use !\r\nPlease change to use !"; }
                        else if (Data.Setup.SetupManager.Instance.SetupSafty.OBSLowerSensorUse == Use.NoUse &&
                            Sineva.VHL.Data.LogIn.AccountManager.Instance.CurAccount.Level >= AuthorizationLevel.Administrator)
                        { no_check = true; message = "Lower Forward Detection Sensor No-Use !\r\nPlease change to use !"; }
                        if (ethercat_link_error) { no_check = true; message = "MXP EtherCat Link Error !\r\nPlease Check MXP Status"; }

                        if (no_check)
                        {
                            if (DialogResult.Yes == MessageBox.Show(message, this.GetType().Name, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly))
                            {
                            }
                        }
                        else
                        {
                            m_EqpManager.SetRunMode(EqpRunMode.Start);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void ButtonActionPause()
        {
            m_EqpManager.SetRunMode(EqpRunMode.Pause);
        }
        private void ButtonActionAbort()
        {
            m_EqpManager.SetRunMode(EqpRunMode.Abort);
        }
        #endregion
    }
}
