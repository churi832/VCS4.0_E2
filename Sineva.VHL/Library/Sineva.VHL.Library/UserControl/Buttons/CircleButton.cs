using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public partial class CircleButton : Button
    {
        #region Enum
        public enum ColorPreset : ushort
        {
            Blue,
            Green,
            Purple,
        }
        #endregion

        #region Field
        private System.ComponentModel.Container components = null;
        private ColorPreset m_ButtonColorPreset = ColorPreset.Blue;

        private Image m_DrawImage;
        private Image m_DrawBgImage;

        private Image m_ImageDefault;
        private Image m_ImageOver;
        private Image m_ImageDown;
        private Image m_ImageChecked;

        //protected Image m_BgImageOver;
        //protected Image m_BgImageDown;
        
        //private Image m_BgImageOverDisabled;
        //private Image m_BgImageDownDisabled;
        private Image m_BgImageChecked;

        private bool m_Checked = false;
        private bool m_MouseDown = false;
        #endregion

        #region Property
        public ColorPreset ButtonColorPreset { get => m_ButtonColorPreset; set => m_ButtonColorPreset = value; }
        public Image ImageDefault { get => m_ImageDefault; set => m_ImageDefault = value; }
        public Image ImageOver { get => m_ImageOver; set => m_ImageOver = value; }
        public Image ImageDown { get => m_ImageDown; set => m_ImageDown = value; }
        public Image ImageChecked { get => m_ImageChecked; set => m_ImageChecked = value; }
        #endregion

        #region Constructor
        public CircleButton()
        {
            InitializeComponent();

            if(m_ImageDefault != null) m_DrawImage = m_ImageDefault;
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();

            components = new System.ComponentModel.Container();
            this.Size = new Size(50, 50);
            this.MouseEnter += RoundButton_MouseEnter;
            this.MouseLeave += RoundButton_MouseLeave;
            this.MouseDown += RoundButton_MouseDown;
            this.MouseUp += RoundButton_MouseUp;
            this.SizeChanged += RoundButton_SizeChanged;
            this.EnabledChanged += RoundButton_EnabledChanged;

            //m_BgImageOverDisabled = Properties.Resources.Circle_BG_Release0;
            //m_BgImageDownDisabled = Properties.Resources.Circle_BG_Push0;
            m_BgImageChecked = Properties.Resources.Circle_BG_Checked;

            //m_BgImageDown = Properties.Resources.Circle_BG_Push1;
            //m_BgImageOver = Properties.Resources.Circle_BG_Release1;

            //m_DrawBgImage = m_BgImageOver;
            m_DrawBgImage = Properties.Resources.Circle_BG_Release1;
            
            this.Cursor = Cursors.Hand;
            this.ResumeLayout();
        }

        private void RoundButton_EnabledChanged(object sender, EventArgs e)
        {
            if(this.Enabled == false)
            {
                //if(m_MouseDown) m_DrawBgImage = m_BgImageDownDisabled;
                //else m_DrawBgImage = m_BgImageOverDisabled;
                if(m_MouseDown) m_DrawBgImage = Properties.Resources.Circle_BG_Push0;
                else m_DrawBgImage = Properties.Resources.Circle_BG_Release0;
            }
            else
            {
                //if(m_MouseDown) m_DrawBgImage = m_BgImageDown;
                //else m_DrawBgImage = m_BgImageOver;
                if(m_MouseDown)
                {
                    if(m_ButtonColorPreset == ColorPreset.Blue) m_DrawBgImage = Properties.Resources.Circle_BG_Push1;
                    else if(m_ButtonColorPreset == ColorPreset.Green) m_DrawBgImage = Properties.Resources.Circle_BG_Push2;
                    else if(m_ButtonColorPreset == ColorPreset.Purple) m_DrawBgImage = Properties.Resources.Circle_BG_Push3;
                }
                else
                {
                    if(m_ButtonColorPreset == ColorPreset.Blue) m_DrawBgImage = Properties.Resources.Circle_BG_Release1;
                    else if(m_ButtonColorPreset == ColorPreset.Green) m_DrawBgImage = Properties.Resources.Circle_BG_Release2;
                    else if(m_ButtonColorPreset == ColorPreset.Purple) m_DrawBgImage = Properties.Resources.Circle_BG_Release3;
                }
            }

            this.Invalidate();
        }

        private void RoundButton_SizeChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void RoundButton_MouseLeave(object sender, EventArgs e)
        {
            if(m_ImageDefault != null) m_DrawImage = m_ImageDefault;

            //if(this.Enabled) m_DrawBgImage = m_BgImageOver;
            //else m_DrawBgImage = m_BgImageOverDisabled;
            if(this.Enabled)
            {
                if(m_ButtonColorPreset == ColorPreset.Blue) m_DrawBgImage = Properties.Resources.Circle_BG_Release1;
                else if(m_ButtonColorPreset == ColorPreset.Green) m_DrawBgImage = Properties.Resources.Circle_BG_Release2;
                else if(m_ButtonColorPreset == ColorPreset.Purple) m_DrawBgImage = Properties.Resources.Circle_BG_Release3;
            }
            else m_DrawBgImage = Properties.Resources.Circle_BG_Release0;

            this.Invalidate();
        }

        private void RoundButton_MouseEnter(object sender, EventArgs e)
        {
            if(m_ImageOver != null) m_DrawImage = m_ImageOver;

            //if(this.Enabled) m_DrawBgImage = m_BgImageOver;
            //else m_DrawBgImage = m_BgImageOverDisabled;
            if(this.Enabled)
            {
                if(m_ButtonColorPreset == ColorPreset.Blue) m_DrawBgImage = Properties.Resources.Circle_BG_Release1;
                else if(m_ButtonColorPreset == ColorPreset.Green) m_DrawBgImage = Properties.Resources.Circle_BG_Release2;
                else if(m_ButtonColorPreset == ColorPreset.Purple) m_DrawBgImage = Properties.Resources.Circle_BG_Release3;
            }
            else m_DrawBgImage = Properties.Resources.Circle_BG_Release0;

            this.Invalidate();
        }

        private void RoundButton_MouseDown(object sender, MouseEventArgs e)
        {
            m_MouseDown = true;

            if(m_ImageDown != null) m_DrawImage = m_ImageDown;

            //if(this.Enabled) m_DrawBgImage = m_BgImageDown;
            //else m_DrawBgImage = m_BgImageDownDisabled;
            if(this.Enabled)
            {
                if(m_ButtonColorPreset == ColorPreset.Blue) m_DrawBgImage = Properties.Resources.Circle_BG_Push1;
                else if(m_ButtonColorPreset == ColorPreset.Green) m_DrawBgImage = Properties.Resources.Circle_BG_Push2;
                else if(m_ButtonColorPreset == ColorPreset.Purple) m_DrawBgImage = Properties.Resources.Circle_BG_Push3;
            }
            else m_DrawBgImage = Properties.Resources.Circle_BG_Push0;

            this.Invalidate();
        }

        private void RoundButton_MouseUp(object sender, MouseEventArgs e)
        {
            m_MouseDown = false;

            if(m_ImageOver != null) m_DrawImage = m_ImageOver;

            //if(this.Enabled) m_DrawBgImage = m_BgImageOver;
            //else m_DrawBgImage = m_BgImageOverDisabled;
            if(this.Enabled)
            {
                if(m_ButtonColorPreset == ColorPreset.Blue) m_DrawBgImage = Properties.Resources.Circle_BG_Release1;
                else if(m_ButtonColorPreset == ColorPreset.Green) m_DrawBgImage = Properties.Resources.Circle_BG_Release2;
                else if(m_ButtonColorPreset == ColorPreset.Purple) m_DrawBgImage = Properties.Resources.Circle_BG_Release3;
            }
            else m_DrawBgImage = Properties.Resources.Circle_BG_Release0;

            this.Invalidate();
        }
        #endregion

        #region Override Method
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            int size = Math.Min(ClientSize.Width, ClientSize.Height);
            Point circleStart = new Point(0, 0);
            if(ClientSize.Width > ClientSize.Height) circleStart.X = (ClientSize.Width - size) / 2;
            else circleStart.Y = (ClientSize.Height - size) / 2;


            // Graphics 초기화
            Graphics g = pevent.Graphics;
            g.Clear(this.Parent.BackColor);//g.Clear(this.BackColor);


            // Button Push 영역 설정
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(circleStart.X, circleStart.Y, size, size);
            this.Region = new Region(path);


            // Button Background Image
            if(m_Checked) g.DrawImage(m_BgImageChecked, circleStart.X, circleStart.Y, size, size);
            //if(this.Enabled == false) g.DrawImage(m_BgImageOverDisabled, circleStart.X, circleStart.Y, size, size);
            //else g.DrawImage(m_DrawBgImage, circleStart.X, circleStart.Y, size, size);
            g.DrawImage(m_DrawBgImage, circleStart.X, circleStart.Y, size, size);


            // Draw Button Foreground Image
            if(m_DrawImage == null && m_ImageDefault != null) m_DrawImage = m_ImageDefault;
            if(m_DrawImage != null)
            {
                int offsetX = (size - m_DrawImage.Width) / 2;
                int offsetY = (size - m_DrawImage.Height) / 2;
                g.DrawImage(m_DrawImage, circleStart.X + offsetX, circleStart.Y + offsetY, m_DrawImage.Width, m_DrawImage.Height);
            }
        }
        #endregion

        #region Method
        public void SetButtonChecked(bool check)
        {
            m_Checked = check;
            if(m_ImageChecked != null) m_DrawImage = m_DrawImage = m_ImageChecked;
            this.Invalidate();
        }
        #endregion
    }
}
