using AutoWindowsSize;
using Sineva.VHL.Data;
using Sineva.VHL.Device;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Remoting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sineva.VHL.GUI.TouchPad
{
    public partial class MainForm : Form
    {
        #region Fields
        private AutoCtrForm m_autoControl = null;

        private readonly string m_EQPModeManual = "手动控制";
        private readonly string m_EQPModeAuto = "自动控制";

        private bool m_FrontLeft = false;
        private bool m_FrontRight = false;
        private bool m_RearLeft = false;
        private bool m_RearRight = false;

        private List<IUpdateUCon> m_Ucons = new List<IUpdateUCon>();
        private bool m_UpdateNeed = false;
        private System.Timers.Timer timer = new System.Timers.Timer(1000);
        private bool m_connected = false;

        private List<string> m_CurAlarm = new List<String> { "" };

        AutoSizeFormClass asc = new AutoSizeFormClass();

        #endregion

        public MainForm()
        {

            InitializeComponent();
            this.Load += MainForm_Load;
            this.Shown += MainForm_Shown;
            this.SizeChanged += MainForm_SizeChanged;
            //this.FormBorderStyle = FormBorderStyle.None;     //设置窗体为无边框样式
            //this.WindowState = FormWindowState.Maximized;    //最大化窗体 

            NumSelectForm.VehicleNumChange += NumSelectForm_VehicleNumChange;
            ucTitle.ModeChange += ModeControl;
            ServoStates.ChangeServoState += ServoStates_ChangeServoState;

            Initilize();

            timerStatesUpdate.Interval = 500; // 设置时间间隔为200ms
            timerStatesUpdate.Tick += TimerStatesUpdate_Tick;
            timerStatesUpdate.Start(); // 启动计时器
        }

        private void ServoStates_ChangeServoState(string servoName, ServoStates.ServoAction servoAction)
        {
            RemoteItem.DeviceType deviceType = RemoteItem.DeviceType.None;
            switch (servoName)
            {
                case "Slave":
                    deviceType = RemoteItem.DeviceType.SlaveTransfer;
                    break;
                case "Master":
                    deviceType = RemoteItem.DeviceType.MasterTransfer;
                    break;
                case "Hoist":
                    deviceType = RemoteItem.DeviceType.Hoist;
                    break;
                case "Slide":
                    deviceType = RemoteItem.DeviceType.Slide;
                    break;
                case "Rotate":
                    deviceType = RemoteItem.DeviceType.Rotate;
                    break;
                case "FrontAntiDrop":
                    deviceType = RemoteItem.DeviceType.FrontAntiDrop;
                    break;
                case "RearAntiDrop":
                    deviceType = RemoteItem.DeviceType.RearAntiDrop;
                    break;
                default:
                    break;
            }

        }

        private void ModeControl(OperateMode targetMode)
        {
            switch (targetMode)
            {
                case OperateMode.Manual:
                    foreach (Control u in this.Controls)
                    {
                        if (u.Tag?.ToString() == "control")
                        {
                            u.Enabled = true;
                        }
                    }
                    break;
                case OperateMode.Auto:
                    foreach (Control u in this.Controls)
                    {
                        if (u.Tag?.ToString() == "control")
                        {
                            u.Enabled = false;
                        }
                    }
                    break;
            }
        }

        private void TimerStatesUpdate_Tick(object sender, EventArgs e)
        {
            try
            {
                PadRemoteGUI remote = RemoteManager.PadInstance.Remoting.RemoteGUI;
                VehicleStateUpdate(remote);
            }
            catch (System.Net.Sockets.SocketException)
            {
                timerStatesUpdate.Enabled = false;
                CustomMessageBox custom = new CustomMessageBox("连接已断开，请重新连接！");
                custom.ShowDialog();
                this.Close();
            }
        }


        private void VehicleStateUpdate(PadRemoteGUI remote)
        {
            if (remote == null)
            {
                return;
            }
            if (remote.IsAlarm)
            {
                var temp = remote.AlarmName;
                if (temp.Count > 0)
                {
                    if (m_CurAlarm[0] != temp[0])
                    {
                        m_CurAlarm = remote.AlarmName;
                        CustomMessageBox customMessageBox = new CustomMessageBox(string.Format("ALARM：{0}", remote.AlarmName[0]));
                        customMessageBox.Show();
                        return;
                    }
                }
            }
            servoStates1.UpdateStatus(remote);
            ucTitle1.UpdateStatus(remote);
            UpdateUI();

        }
        private void UpdateUI()
        {
            this.btnFrontSteerStatus.Text = RemoteManager.PadInstance.Remoting.RemoteGUI.FrontSteerStatus.ToString();
            this.btnRearSteerStatus.Text = RemoteManager.PadInstance.Remoting.RemoteGUI.RearSteerStatus.ToString();
            this.btnGripperStatus.Text = RemoteManager.PadInstance.Remoting.RemoteGUI.GripperStatus.ToString();
            this.btnFrontAntiDropStatus.Text = RemoteManager.PadInstance.Remoting.RemoteGUI.FrontAntiDropStatus.ToString();
            this.btnRearAntiDropStatus.Text = RemoteManager.PadInstance.Remoting.RemoteGUI.RearAntiDropStatus.ToString();
            this.lbConnectionStatus.Text = string.Format("当前连接状态：{0}", RemoteManager.PadInstance.Remoting.RemoteGUI.HeartBitRun ? "已连接" : "未连接");

            if (RemoteManager.PadInstance.Remoting.RemoteGUI.OpMode == OperateMode.Manual)
            {
                btnMode.Text = "手动模式";
                btnMode.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.right_manual_default;
            }
            else if (RemoteManager.PadInstance.Remoting.RemoteGUI.OpMode == OperateMode.Auto)
            {
                btnMode.Text = "自动模式";
                btnMode.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.right_auto_default;
            }
            if (RemoteManager.PadInstance.Remoting.RemoteGUI.EqRunMode == EqpRunMode.Start)
            {
                btnEqpState.Text = "循环运行";
                btnEqpState.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.right_start_default;
            }
            else if (RemoteManager.PadInstance.Remoting.RemoteGUI.EqRunMode == EqpRunMode.Stop)
            {
                btnEqpState.Text = "循环停止";
                btnEqpState.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.right_stop_default;
            }
            btnReady.Enabled = RemoteManager.PadInstance.Remoting.RemoteGUI.IsReady;
        }

        private void NumSelectForm_VehicleNumChange()
        {
            ucTitle1.UpdateVehicleNum(Program.SelectedNum);
            timerStatesUpdate.Stop();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            //setEQPMode(m_EQPModeManual);
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            int index = 0;
            IUpdateUCon con = null;
            m_UpdateNeed = true;

            asc.controllInitializeSize(this);
            //this.labelTime.Text = DateTime.Now.ToString();
            //this.labelVehicleNum.Text = Program.SelectedNum;

            UpdateUConFunc.GetAllUpdateUCon(this, ref m_Ucons);
            foreach (var ucon in m_Ucons)
            {
                con = ucon;
                ucon.Initialize();
                index++;
            }
        }

        private void Initilize()
        {
            if (RemoteManager.PadInstance.Remoting == null)
            {
                Environment.Exit(0);
            }
            this.btnVolecitySlow.BackgroundImage = Properties.Resources.buttonFace1_2;

            try
            {
                ModeControl(RemoteManager.PadInstance.Remoting.RemoteGUI.OpMode);
                //timer.Enabled = true;

                //timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timeTick);

                RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(RemoteItem.DeviceType.None, RemoteItem.ActionType.None, RemoteItem.VelocitySelect.Slow);
                //timer.Stop();
            }
            catch (System.Net.Sockets.SocketException exception)
            {
                ExceptionLog.WriteLog($"{exception}");
                CustomMessageBox customMessageBox = new CustomMessageBox("连接失败，请检查网络！");
                customMessageBox.ShowDialog();
                this.Close();
                timerStatesUpdate.Stop();
                Program.numSelectForm.Show();
            }

        }

        //private void timeTick(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        timer.Stop();
        //        Exception exception = new Exception("连接超时");
        //        throw exception;
        //    }
        //    catch
        //    {
        //        RemoteManager.PadInstance.Remoting.RemoteGUI = null;
        //        CustomMessageBox customMessageBox = new CustomMessageBox("连接失败，请检查网络后重新连接！");
        //        customMessageBox.ShowDialog();
        //        Application.Restart();
        //    }
        //}

        private void jog_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                RemoteItem.DeviceType deviceType = RemoteItem.DeviceType.None;
                RemoteItem.ActionType actionType = RemoteItem.ActionType.None;

                btn.BackColor = Color.Chartreuse;
                switch (btn.Name)
                {
                    case "btnJogTransferPlus":
                        deviceType = RemoteItem.DeviceType.MasterTransfer;
                        actionType = RemoteItem.ActionType.JogPlus;
                        break;
                    case "btnJogTransferMiuns":
                        deviceType = RemoteItem.DeviceType.MasterTransfer;
                        actionType = RemoteItem.ActionType.JogMinus;
                        break;
                    case "btnJogSlidePlus":
                        deviceType = RemoteItem.DeviceType.Slide;
                        actionType = RemoteItem.ActionType.JogPlus;
                        break;
                    case "btnJogSlideMinus":
                        deviceType = RemoteItem.DeviceType.Slide;
                        actionType = RemoteItem.ActionType.JogMinus;
                        break;
                    case "btnJogRotatePlus":
                        deviceType = RemoteItem.DeviceType.Rotate;
                        actionType = RemoteItem.ActionType.JogPlus;
                        break;
                    case "btnJogRotateMinus":
                        deviceType = RemoteItem.DeviceType.Rotate;
                        actionType = RemoteItem.ActionType.JogMinus;
                        break;
                    case "btnJogHoistPlus":
                        deviceType = RemoteItem.DeviceType.Hoist;
                        actionType = RemoteItem.ActionType.JogPlus;
                        break;
                    case "btnJogHoistMinus":
                        deviceType = RemoteItem.DeviceType.Hoist;
                        actionType = RemoteItem.ActionType.JogMinus;
                        break;
                    case "btnJogStop":
                        deviceType = RemoteItem.DeviceType.None;
                        actionType = RemoteItem.ActionType.JogStop;
                        break;

                }
                RemoteItem.VelocitySelect velocitySelect = VehicleManager.Instance.Volecity;
                RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(deviceType, actionType, velocitySelect);

            }
            catch (System.Net.Sockets.SocketException)
            {
                CustomMessageBox message = new CustomMessageBox("连接失败，请检查网络后重试！");
                message.ShowDialog();

            }
        }

        private void jog_MouseUp(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.BackColor = Color.Yellow;
            RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(RemoteItem.DeviceType.None, RemoteItem.ActionType.JogStop, RemoteItem.VelocitySelect.Slow);
        }

        private void btnVolecity_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            this.btnVolecitySlow.BackgroundImage = Properties.Resources.buttonFace1;
            this.btnVolecityMid.BackgroundImage = Properties.Resources.buttonFace1;
            this.btnVolecityHigh.BackgroundImage = Properties.Resources.buttonFace1;

            button.BackgroundImage = Properties.Resources.buttonFace1_2;
            switch (button.Name)
            {
                case "btnVolecitySlow":
                    VehicleManager.Instance.Volecity = RemoteItem.VelocitySelect.Slow; break;
                case "btnVolecityMid":
                    VehicleManager.Instance.Volecity = RemoteItem.VelocitySelect.Mid; break;
                case "btnVolecityHigh":
                    VehicleManager.Instance.Volecity = RemoteItem.VelocitySelect.High; break;
            }
        }

        private void btnServo_MouseDown(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.BackgroundImage = Properties.Resources.buttonFace1_2;
        }

        private void btnServo_MouseUp(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.BackgroundImage = Properties.Resources.buttonFace1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                DateTime dt = DateTime.Now;
                string value = string.Format("{0:yyyy/M/d HH:mm:ss}", dt);
                ucTitle1.UpdateTime(value);


            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteLog(ex.ToString());
            }

        }

        private void btnModeCtr_Click(object sender, EventArgs e)
        {
            //if ((sender as Button).Tag == null) return;
            //setEQPMode((sender as Button).Text);

            //((Form)(((Button)sender).Tag)).BringToFront();
            //((Form)(((Button)sender).Tag)).Focus();
            //((Form)(((Button)sender).Tag)).TopLevel = false;
            //this.ContentPanel.Controls.Clear();
            //this.ContentPanel.Controls.Add(((Form)(((Button)sender).Tag)));
            //((Form)(((Button)sender).Tag)).Show();

        }

        private void btnSteerStatus_TextChanged(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Text)
            {
                case "None":
                    btn.BackgroundImage = Properties.Resources.buttonFace3_2;
                    break;
                case "Left":
                    btn.BackgroundImage = Properties.Resources.buttonFace1_2;
                    break;
                case "Right":
                    btn.BackgroundImage = Properties.Resources.buttonFace2;
                    break;
                default:
                    break;
            }
        }

        private void btnGripperStatus_TextChanged(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Text)
            {
                case "None":
                    btn.BackgroundImage = Properties.Resources.buttonFace1_3;
                    break;
                case "Open":
                    btn.BackgroundImage = Properties.Resources.buttonFace2;
                    break;
                case "Close":
                    btn.BackgroundImage = Properties.Resources.buttonFace3_2;
                    break;
                default:
                    break;
            }
        }
        private void btnAntiDropStatus_TextChanged(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Text)
            {
                case "None":
                    btn.BackgroundImage = Properties.Resources.buttonFace1_3;
                    break;
                case "Lock":
                    btn.BackgroundImage = Properties.Resources.buttonFace2;
                    break;
                case "UnLock":
                    btn.BackgroundImage = Properties.Resources.buttonFace3_2;
                    break;
                default:
                    break;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Restart();
        }

        private void btnManual_Click(object sender, EventArgs e)
        {
        }

        private void btnReady_Click(object sender, EventArgs e)
        {
            RemoteManager.PadInstance.Remoting.RemoteGUI.ChangeOpMode(OperateMode.Recovery);
        }

        private void btnMode_Click(object sender, EventArgs e)
        {

            if (btnMode.Text == "自动模式")
            {
                RemoteManager.PadInstance.Remoting.RemoteGUI.ChangeOpMode(OperateMode.Manual);
            }
            else
            {
                RemoteManager.PadInstance.Remoting.RemoteGUI.ChangeOpMode(OperateMode.Auto);

            }
        }

        private void btnEqpState_Click(object sender, EventArgs e)
        {
            if (btnEqpState.Text == "循环停止")
            {
                RemoteManager.PadInstance.Remoting.RemoteGUI.ChangeEqpRunMode(EqpRunMode.Stop);
            }
            else
            {
                RemoteManager.PadInstance.Remoting.RemoteGUI.ChangeEqpRunMode(EqpRunMode.Start);
            }

        }

        private void btnAbort_MouseClick(object sender, MouseEventArgs e)
        {
            RemoteManager.PadInstance.Remoting.RemoteGUI.ChangeEqpRunMode(EqpRunMode.Abort);
        }


        private void btnFrontSteerStatus_Click(object sender, EventArgs e)
        {
            int rv1 = -1;
            switch (btnRearSteerStatus.Text)
            {
                case "LEFT":
                    try
                    {
                        Button btn = sender as Button;
                        RemoteItem.DeviceType deviceType = RemoteItem.DeviceType.FrontSteer;
                        RemoteItem.ActionType actionType = RemoteItem.ActionType.SteerRight;
                        RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(deviceType, actionType);
                    }
                    catch (System.Net.Sockets.SocketException)
                    {
                        CustomMessageBox message = new CustomMessageBox("连接失败，请检查网络后重试！");
                        message.ShowDialog();
                    }
                    break;
                case "RIGHT":
                    try
                    {
                        Button btn = sender as Button;
                        RemoteItem.DeviceType deviceType = RemoteItem.DeviceType.FrontSteer;
                        RemoteItem.ActionType actionType = RemoteItem.ActionType.SteerLeft;
                        RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(deviceType, actionType);
                    }
                    catch (System.Net.Sockets.SocketException)
                    {
                        CustomMessageBox message = new CustomMessageBox("连接失败，请检查网络后重试！");
                        message.ShowDialog();
                    }
                    break;
            }
        }

        private void btnRearSteerStatus_Click(object sender, EventArgs e)
        {
            int rv1 = -1;
            switch (btnRearSteerStatus.Text)
            {
                case "LEFT":
                    try
                    {
                        Button btn = sender as Button;
                        RemoteItem.DeviceType deviceType = RemoteItem.DeviceType.RearSteer;
                        RemoteItem.ActionType actionType = RemoteItem.ActionType.SteerRight;
                        RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(deviceType, actionType);
                    }
                    catch (System.Net.Sockets.SocketException)
                    {
                        CustomMessageBox message = new CustomMessageBox("连接失败，请检查网络后重试！");
                        message.ShowDialog();
                    }
                    break;
                case "RIGHT":
                    try
                    {
                        Button btn = sender as Button;
                        RemoteItem.DeviceType deviceType = RemoteItem.DeviceType.RearSteer;
                        RemoteItem.ActionType actionType = RemoteItem.ActionType.SteerLeft;
                        RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(deviceType, actionType);
                    }
                    catch (System.Net.Sockets.SocketException)
                    {
                        CustomMessageBox message = new CustomMessageBox("连接失败，请检查网络后重试！");
                        message.ShowDialog();
                    }
                    break;
            }
        }
    }
}
