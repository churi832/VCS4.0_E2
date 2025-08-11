using Sineva.VHL.Library.Remoting;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sineva.VHL.GUI.TouchPad
{
    public partial class ManualCtrForm : Form
    {
        public ManualCtrForm()
        {
            InitializeComponent();

            btnInitilize();
        }

        private void btnInitilize()
        {
            this.btnFrontSteerStatus.Enabled = false;
            this.btnRearSteerStatus.Enabled = false;
            this.btnGripperStatus.Enabled = false;
            this.btnAntiDropStatus.Enabled = false;
        }

        private void jog_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.BackColor = Color.Chartreuse;
        }

        private void jog_MouseUp(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.BackColor = Color.Yellow;
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
    }
}
