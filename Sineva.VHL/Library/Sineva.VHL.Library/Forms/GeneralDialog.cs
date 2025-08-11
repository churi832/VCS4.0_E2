using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public partial class GeneralDialog : Form
    {
        #region Field
        private Enable m_WindowButtonCloseEnable = Enable.Disable;
        private Enable m_WindowButtonMinimizeEnable = Enable.Disable;
        private Enable m_WindowButtonLockEnable = Enable.Disable;
        private bool m_WindowConfineScreen = false;

        private Color m_ColorTitleBgDeactive = Color.DimGray;
        private Color m_ColorTitleBgActive = Color.GreenYellow;
        private Color m_ColorTitleBgMoving = Color.Violet;
        private Color m_ColorTitleDeactive = Color.White;
        private Color m_ColorTitleActive = Color.Blue;
        private Color m_ColorTitleMoving = Color.Yellow;
        private Point m_OffsetMouseLocation = new Point(0, 0);
        private Size m_MinimizeSize = new Size(300, 25);

        private readonly Bitmap ImageWindowMinimize = Properties.Resources.chars002_underbar;
        private readonly Bitmap ImageWindowNormalize = Properties.Resources.chars003_topbar;
        private readonly Bitmap ImageWindowLock = Properties.Resources.i001_Lock;
        private readonly Bitmap ImageWindowUnlock = Properties.Resources.i002_Unlock;

        private Point m_MouseLocation = new Point();
        private Size m_InitSize = new Size();
        private Point m_MyLocation = new Point();
        private bool m_Minimized = false;
        private bool m_WindowLocked = false;
        private Rectangle m_ScreenRect = new Rectangle();
        #endregion

        #region Property
        [Category("!Window Setting : Layout"), DisplayName("Activate : Close Button")]
        public Enable WindowButtonCloseEnable
        {
            get { return m_WindowButtonCloseEnable; }
            set { m_WindowButtonCloseEnable = value; this.pbButtonClose.Visible = value == Enable.Enable; }
        }
        [Category("!Window Setting : Layout"), DisplayName("Activate : Minimize Button")]
        public Enable WindowButtonMinimizeEnable
        {
            get { return m_WindowButtonMinimizeEnable; }
            set { m_WindowButtonMinimizeEnable = value; this.pbButtonMinimize.Visible = value == Enable.Enable; }
        }
        [Category("!Window Setting : Layout"), DisplayName("Activate : Lock Button")]
        public Enable WindowButtonLockEnable
        {
            get { return m_WindowButtonLockEnable; }
            set { m_WindowButtonLockEnable = value; this.pbButtonLock.Visible = value == Enable.Enable; }
        }
        [Category("!Window Setting : Layout"), DisplayName("Window Confine Screen")]
        public bool WindowConfineScreen
        {
            get { return m_WindowConfineScreen; }
            set { m_WindowConfineScreen = value; }
        }
        [Category("!Window Setting : Layout"), DisplayName("Window Minimize Size")]
        public Size MinimizeSize
        {
            get { return m_MinimizeSize; }
            set
            {
                Size temp = value;
                if(temp.Width < 100) temp.Width = 100;
                if(temp.Height < 25) temp.Height = 25;
                m_MinimizeSize = temp;
            }
        }
        [Category("!Window Setting : Title"), DisplayName("Background Color : Screen Moving")]
        public Color ColorTitleBgMoving
        {
            get { return m_ColorTitleBgMoving; }
            set { m_ColorTitleBgMoving = value; }
        }
        [Category("!Window Setting : Title"), DisplayName("Background Color : Screen Activate")]
        public Color ColorTitleBgActive
        {
            get { return m_ColorTitleBgActive; }
            set { m_ColorTitleBgActive = value; }
        }
        [Category("!Window Setting : Title"), DisplayName("Background Color : Screen Deactivate")]
        public Color ColorTitleBgDeactive
        {
            get { return m_ColorTitleBgDeactive; }
            set { m_ColorTitleBgDeactive = value; }
        }
        [Category("!Window Setting : Title"), DisplayName("Foreground : Screen Moving")]
        public Color ColorTitleMoving
        {
            get { return m_ColorTitleMoving; }
            set { m_ColorTitleMoving = value; }
        }
        [Category("!Window Setting : Title"), DisplayName("Foreground Color : Screen Activate")]
        public Color ColorTitleActive
        {
            get { return m_ColorTitleActive; }
            set { m_ColorTitleActive = value; }
        }
        [Category("!Window Setting : Title"), DisplayName("Foreground Color : Screen Deactivate")]
        public Color ColorTitleDeactive
        {
            get { return m_ColorTitleDeactive; }
            set { m_ColorTitleDeactive = value; }
        }
        [Category("!Window Setting : Title"), DisplayName("Mouse Location Offset")]
        public Point OffsetMouseLocation
        {
            get { return m_OffsetMouseLocation; }
            set { m_OffsetMouseLocation = value; }
        }
        #endregion

        public GeneralDialog()
        {
            InitializeComponent();

            m_InitSize = this.Size;

            pbButtonClose.MouseClick += ButtonClose_MouseClick;
            pbButtonMinimize.MouseClick += ButtonMinimize_MouseClick;
            pbButtonLock.MouseClick += ButtonLock_MouseClick;
        }

        #region Method
        private void ButtonMinimize_MouseClick(object sender, MouseEventArgs e)
        {
            if(m_Minimized)
                this.Size = m_InitSize;
            else
                this.Size = m_MinimizeSize;
            m_Minimized = !m_Minimized;

            if(m_Minimized) pbButtonMinimize.BackgroundImage = ImageWindowNormalize;
            else pbButtonMinimize.BackgroundImage = ImageWindowMinimize;
        }
        private void ButtonLock_MouseClick(object sender, MouseEventArgs e)
        {
            m_WindowLocked = !m_WindowLocked;
            LockWindow(m_WindowLocked);

            if(m_WindowLocked)
            {
                this.pbButtonLock.BackgroundImage = ImageWindowUnlock;
                this.pbButtonLock.BackColor = Color.Red;
            }
            else
            {
                this.pbButtonLock.BackgroundImage = ImageWindowLock;
                this.pbButtonLock.BackColor = Color.GreenYellow;
            }
        }
        private void ButtonClose_MouseClick(object sender, MouseEventArgs e)
        {
            m_MyLocation = this.Location;
            if(this.Modal)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
            else this.Hide();
        }
        protected void LockWindow(bool set)
        {
            foreach(Control control in this.Controls)
            {
                control.Enabled = !set;
            }

            pbButtonClose.Enabled = true;
            pbButtonLock.Enabled = true;
            pbButtonMinimize.Enabled = true;
            lblTitle.Enabled = true;
        }
        #endregion

        #region Event Handler
        private void Title_MouseDown(object sender, MouseEventArgs e)
        {
            m_MouseLocation = new Point(-e.X - m_OffsetMouseLocation.X, -e.Y - m_OffsetMouseLocation.Y);
            lblTitle.BackColor = m_ColorTitleBgMoving;
        }
        private void Title_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button != MouseButtons.Left) return;

            Point mousePoint = Control.MousePosition;
            mousePoint.Offset(m_MouseLocation.X, m_MouseLocation.Y);
            this.Location = mousePoint;
        }
        private void Title_MouseUp(object sender, MouseEventArgs e)
        {
            if(m_WindowConfineScreen)
            {
                if(this.Location.X < m_ScreenRect.X) this.Location = new Point(m_ScreenRect.X, this.Location.Y);
                else if(this.Location.X + this.Width > m_ScreenRect.Width + m_ScreenRect.X) this.Location = new Point(m_ScreenRect.X + m_ScreenRect.Width - this.Width, this.Location.Y);

                if(this.Location.Y < m_ScreenRect.Y) this.Location = new Point(this.Location.X, m_ScreenRect.Y);
                else if(this.Location.Y + this.Height > m_ScreenRect.Height + m_ScreenRect.Y) this.Location = new Point(this.Location.X, m_ScreenRect.Y + m_ScreenRect.Height - this.Height);
            }

            if(this.Focused) lblTitle.BackColor = m_ColorTitleBgActive;
            else lblTitle.BackColor = m_ColorTitleBgDeactive;
        }
        private void Dialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }
        private void Dialog_SizeChanged(object sender, EventArgs e)
        {
            pbButtonClose.Location = new Point(panelTitle.Width - pbButtonClose.Width - 2, (panelTitle.Height - pbButtonClose.Height) / 2);
            pbButtonMinimize.Location = new Point(pbButtonClose.Location.X - pbButtonMinimize.Width - 4, pbButtonClose.Location.Y);
            pbButtonLock.Location = new Point(this.Width - pbButtonLock.Width - 2, this.Height - pbButtonLock.Height - 2);
        }
        private void Dialog_Deactivate(object sender, EventArgs e)
        {
            lblTitle.BackColor = m_ColorTitleBgDeactive;
        }

        private void Dialog_Activated(object sender, EventArgs e)
        {
            lblTitle.BackColor = m_ColorTitleBgActive;
        }
        private void IcsDialog_Shown(object sender, EventArgs e)
        {
            if(m_WindowButtonLockEnable == Enable.Enable) pbButtonLock.BringToFront();
        }
        #endregion
    }
}
