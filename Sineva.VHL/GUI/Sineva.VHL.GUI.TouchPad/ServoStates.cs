using Sineva.VHL.Device;
using Sineva.VHL.Library.Remoting;
using System.Drawing;
using System.Windows.Forms;


namespace Sineva.VHL.GUI.TouchPad
{
    public partial class ServoStates : UserControl
    {
        public enum ServoAction
        {
            None,
            ServoOn,
            ServoOff,
            Home,
            ErrorReset,
        }
        public delegate void ChangeServoStateHandler(string servoName, ServoAction servoAction);
        public static event ChangeServoStateHandler ChangeServoState;
        public ServoStates()
        {
            InitializeComponent();
        }
        public void UpdateStatus(PadRemoteGUI remote)
        {
            this.ssvSlavePosition.TextValue = remote.SlavePosition.ToString();
            this.ssdSlaveStatus.Text = remote.SlaveServoState;
            this.ssdSlaveHomeDone.Text = remote.SlaveHomeDone ? "OK" : "NG";
            //this.ssdSlaveAlarm.Text = remote.SlaveAlarm;
            this.ssvMasterPosition.TextValue = remote.MasterPosition.ToString();
            this.ssdMasterStatus.Text = remote.MasterServoState;
            this.ssdMasterHomeDone.Text = remote.MasterHomeDone ? "OK" : "NG";
            //this.ssdMasterAlarm.Text = remote.MasterAlarm;
            this.ssvHoistPosition.TextValue = remote.HoistPosition.ToString();
            this.ssdHoistStatus.Text = remote.HoistServoState;
            this.ssdHoistHomeDone.Text = remote.HoistHomeDone ? "OK" : "NG";
            //this.ssdHoistAlarm.Text = remote.HoistAlarm;
            this.ssvRotatePosition.TextValue = remote.RotatePosition.ToString();
            this.ssdRotateStatus.Text = remote.RotateServoState;
            this.ssdRotateHomeDone.Text = remote.RotateHomeDone ? "OK" : "NG";
            //this.ssdRotateAlarm.Text = remote.RotateAlarm;
            this.ssvSlidePosition.TextValue = remote.SlidePosition.ToString();
            this.ssdSlideStatus.Text = remote.SlideServoState;
            this.ssdSlideHomeDone.Text = remote.SlideHomeDone ? "OK" : "NG";
            //this.ssdSlideAlarm.Text = remote.SlideAlarm;
            this.ssvFrontAntiDropPosition.TextValue = remote.FrontAntiDropPosition.ToString();
            this.ssdFrontAntiDropStatus.Text = remote.FrontAntiDropServoState;
            this.ssdFrontAntiDropHomeDone.Text = remote.FrontAntiDropHomeDone ? "OK" : "NG";
            //this.ssdFrontAntiDropAlarm.Text = remote.FrontAntiDropAlarm;
            this.ssvRearAntiDropPosition.TextValue = remote.RearAntiDropPosition.ToString();
            this.ssdRearAntiDropStatus.Text = remote.RearAntiDropServoState;
            this.ssdRearAntiDropHomeDone.Text = remote.RearAntiDropHomeDone ? "OK" : "NG";
            //this.ssdRearAntiDropAlarm.Text = remote.RearAntiDropAlarm.ToString();
        }

        private void ssdSlaveStatus_TextChanged(object sender, System.EventArgs e)
        {
            Label label = sender as Label;

            if (label.Text == "Servo Off" || label.Text == "NG")
            {
                label.BackColor = Color.Yellow;
            }
            else if (label.Text == "Servo On" || label.Text == "OK")
            {
                label.BackColor = Color.Lime;
            }
        }

        private void ssdSlaveStatus_DoubleClick(object sender, System.EventArgs e)
        {
            Label label = sender as Label;
            ServoAction action = ServoAction.None;
            if (label.Text == "Servo ON")
            {
                action = ServoAction.ServoOff;
            }
            else if (label.Text == "Servo Off")
            {
                action = ServoAction.ServoOn;
            }
            else if (label.Text == "Alarm")
            {
                action = ServoAction.ErrorReset;
            }
            if (!string.IsNullOrEmpty(label.Tag?.ToString()))
            {
                ChangeServoState?.Invoke(label.Tag.ToString(), action);
            }

        }
    }
}

