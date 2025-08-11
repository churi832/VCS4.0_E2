using Sineva.VHL.GUI.TouchPad.Properties;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Remoting;
using System.Windows.Forms;

namespace Sineva.VHL.GUI.TouchPad
{
    public partial class ucTitle : UserControl
    {
        public delegate void ModeChaneHandler(OperateMode targetMode);

        public static event ModeChaneHandler ModeChange;
        public ucTitle()
        {
            InitializeComponent();
        }

        public void UpdateStatus(PadRemoteGUI remote)
        {
            this.labelEQPState.Text = remote.EqState.ToString();
            this.labelVehicleNum.Text = Program.SelectedNum;
            this.txtEQPMode.Text = remote.OpMode.ToString();
            this.labelOCSState.Text = remote.OcsState;
            this.labelJCSState.Text = remote.JcsState;
            this.labelVelocity.Text = remote.Velocity.ToString();
            if (remote.OpMode == OperateMode.Manual)
            {
                ModeChange(OperateMode.Manual);
            }
            else if (remote.OpMode == OperateMode.Auto)
            {
                ModeChange(OperateMode.Auto);
            }
        }

        public void UpdateTime(string time)
        {
            this.labelTime.Text = time;
        }

        public void UpdateVehicleNum(string vehicleNum)
        {
            this.labelVehicleNum.Text = vehicleNum;
        }

        private void txtEQPMode_Click(object sender, System.EventArgs e)
        {
            if (txtEQPMode.Text != OperateMode.Manual.ToString())
            {
                CustomMessageBox customMessageBox = new CustomMessageBox("确定要切换为Manual模式吗？");
                customMessageBox.ShowDialog();
                if (customMessageBox.Res == false) { return; }
                RemoteManager.PadInstance.Remoting.RemoteGUI.ChangeOpMode(OperateMode.Manual);
                if (RemoteManager.PadInstance.Remoting.RemoteGUI.OpMode == OperateMode.Manual)
                {
                    ModeChange(OperateMode.Manual);
                }
            }
            else
            {
                CustomMessageBox customMessageBox = new CustomMessageBox("确定要切换为Auto模式吗？");
                customMessageBox.ShowDialog();
                if (customMessageBox.Res == false) { return; }
                RemoteManager.PadInstance.Remoting.RemoteGUI.ChangeOpMode(OperateMode.Auto);
                if (RemoteManager.PadInstance.Remoting.RemoteGUI.OpMode == OperateMode.Auto)
                {
                    ModeChange(OperateMode.Auto);
                }
            }
        }

        private void txtEQPMode_TextChanged(object sender, System.EventArgs e)
        {
            Button button = sender as Button;
            switch (button.Text)
            {
                case "Manual":
                    button.BackgroundImage = Properties.Resources.Frame_Blue;
                    break;
                case "Auto":
                    button.BackgroundImage = Properties.Resources.Frame_Green;
                    break;
                default:
                    button.BackgroundImage = Properties.Resources.Frame_Red;
                    break;
            }
        }

        private void labelVehicleNum_Click(object sender, System.EventArgs e)
        {
            NumSelectForm numSelectForm = new NumSelectForm();
            numSelectForm.Show();
        }

        private void labelJCSAndOCSState_TextChanged(object sender, System.EventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Text)
            {
                case "Ready":
                    btn.BackgroundImage = Properties.Resources.Frame_Green;
                    break;
                case "Not Ready":
                    btn.BackgroundImage = Properties.Resources.Frame_Red;
                    break;
            }
        }

        private void labelEQPState_TextChanged(object sender, System.EventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Text)
            {
                case "Run":
                    btn.BackgroundImage = Properties.Resources.Frame_Green;
                    break;
                case "Stop":
                    btn.BackgroundImage = Properties.Resources.Frame_Red;
                    break;
                case "Idle":
                    btn.BackgroundImage = Resources.Frame_Yellow;
                    break;
                case "Down":
                    btn.BackgroundImage = Properties.Resources.Frame_Red;
                    break;
            }
        }
    }
}
