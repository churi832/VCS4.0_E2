using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ICS.Lib
{
    public partial class FormNotifyConfigError : Form
    {
        public FormNotifyConfigError()
        {
            InitializeComponent();
        }

        public void ShowError(string message, bool newItemTop = true)
        {
            this.Visible = true;
            if(newItemTop)
                this.listBox1.Items.Insert(0, message);
            else
                this.listBox1.Items.Add(message);
        }

        private void chkTopMost_CheckStateChanged(object sender, EventArgs e)
        {
            this.TopMost = chkTopMost.Checked;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.listBox1.Items.Count; i++) sb.Append(this.listBox1.Items[i] + "\r\n");
            Clipboard.SetText(sb.ToString());
        }
    }
}
