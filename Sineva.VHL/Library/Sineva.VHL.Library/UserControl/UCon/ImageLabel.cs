using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public partial class ImageLabel : UserControl
    {
        #region Field
        private bool m_AutoResizeTextSize = false;
        #endregion

        #region Property
        [Category("!UserControl")]
        public string LabelText
        {
            get { return this.label1.Text; }
            set { this.label1.Text = value; }
        }
        [Category("!UserControl")]
        public Color TextColor
        {
            get { return this.label1.ForeColor; }
            set { this.label1.ForeColor = value; }
        }
        [Category("!UserControl")]
        public Font TextFont
        {
            get { return this.label1.Font; }
            set { this.label1.Font = value; }
        }
        [Category("!UserControl")]
        public Image ImageOfBackground
        {
            get { return this.panel1.BackgroundImage; }
            set { this.panel1.BackgroundImage = value; }
        }
        [Category("!UserControl")]
        public ImageLayout ImageLayout
        {
            get { return this.panel1.BackgroundImageLayout; }
            set { this.panel1.BackgroundImageLayout = value; }
        }
        [Category("!UserControl")]
        public RightToLeft TextFromRight
        {
            get { return this.label1.RightToLeft; }
            set { this.label1.RightToLeft = value; }
        }
        [Category("!UserControl")]
        public bool AutoResizeTextSize
        {
            get { return m_AutoResizeTextSize; }
            set { m_AutoResizeTextSize = value; }
        }
        #endregion

        public ImageLabel()
        {
            InitializeComponent();
            SetDoubleBuffer();   
        }

        protected void SetDoubleBuffer()
        {
            //this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            //this.UpdateStyles();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            if(m.Msg != 0x14)
                base.OnNotifyMessage(m);
        }

        public void SetText(string text)
        {
            if (this.label1.Text != text)
            {
                this.label1.Text = text;
                ButtonLog.WriteLog(this.Name.ToString(), string.Format("SetText={0}", text));
            }

            if (m_AutoResizeTextSize)
            {
                double w = ((float)label1.Width / (float)text.Length);
                double h = 0.7f * label1.Height;
                double autoSize = (w < h) ? w : h;
                Font newFont = new Font(label1.Font.FontFamily, (float)autoSize, label1.Font.Style);
                this.label1.Font = newFont;
            }
        }

        private void label1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.OnMouseDoubleClick(e);
        }
    }
}
