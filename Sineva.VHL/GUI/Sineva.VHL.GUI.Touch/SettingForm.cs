using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.GUI.Touch
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();

            this.tbIpAddress.Text = TouchManager.Instance.IpAddress.ToString();
            this.tbPortNumber.Text = TouchManager.Instance.IpPort.ToString();
            this.tbWrInsTimeout.Text = TouchManager.Instance.WrInsTimeOut.ToString();
            this.tbSendInterval.Text = TouchManager.Instance.SendInterval.ToString();
            this.tbDevAddress.Text = TouchManager.Instance.DevAddr.ToString();
            this.tbConnectRetryInterval.Text = TouchManager.Instance.ConnectRetryInterval.ToString();
            this.tbHeartBitCheckTime.Text = TouchManager.Instance.HeartBitCheckTime.ToString();
            this.tbSlaveNo.Text = TouchManager.Instance.SlaveNo.ToString();
            this.tbOffset.Text = TouchManager.Instance.Offset.ToString();
            this.tbStartAddress.Text = TouchManager.Instance.StartAddress.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            TouchManager.Instance.IpAddress = this.tbIpAddress.Text;
            int value = 0;
            TouchManager.Instance.IpPort = int.TryParse(this.tbPortNumber.Text, out value) ? value : 0;
            TouchManager.Instance.WrInsTimeOut = int.TryParse(this.tbWrInsTimeout.Text, out value) ? value : 0;
            TouchManager.Instance.SendInterval = int.TryParse(this.tbSendInterval.Text, out value) ? value : 0;
            TouchManager.Instance.ConnectRetryInterval = int.TryParse(this.tbConnectRetryInterval.Text, out value) ? value : 0;
            int dev = int.TryParse(this.tbDevAddress.Text, out value) ? value : 0;
            TouchManager.Instance.DevAddr = (byte)dev;
            int checkTime = int.TryParse(this.tbHeartBitCheckTime.Text, out value) ? value : 0;
            TouchManager.Instance.HeartBitCheckTime = checkTime;
            UInt32 slaveNo = 0;
            TouchManager.Instance.SlaveNo = UInt32.TryParse(this.tbSlaveNo.Text, out slaveNo) ? slaveNo : 0;
            ushort value1 = 0;
            TouchManager.Instance.Offset = ushort.TryParse(this.tbOffset.Text, out value1) ? value1 : (ushort)0;
            TouchManager.Instance.StartAddress = ushort.TryParse(this.tbStartAddress.Text, out value1) ? value1 : (ushort)0;
            base.DialogResult = DialogResult.OK;
            this.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            this.Dispose();
        }
    }
}
