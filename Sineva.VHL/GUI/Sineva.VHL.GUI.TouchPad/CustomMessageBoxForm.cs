using AutoWindowsSize;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sineva.VHL.GUI.TouchPad
{
    public partial class CustomMessageBox : Form
    {
        private bool m_Res = false;
        private AutoSizeFormClass asc = new AutoSizeFormClass();
        public CustomMessageBox(string message)
        {
            InitializeComponent();

            asc.controllInitializeSize(this);

            this.SizeChanged += CustomMessageBox_SizeChanged;

            this.label1.Text = message;
            // 获取屏幕的尺寸
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            // 获取窗体的尺寸
            int formWidth = this.Width;
            int formHeight = this.Height;

            // 计算窗体在屏幕中央的位置
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point((screenWidth - formWidth) / 2, (screenHeight - formHeight) / 2);
        }

        private void CustomMessageBox_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }

        public bool Res { get => m_Res; set => m_Res = value; }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            m_Res = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
