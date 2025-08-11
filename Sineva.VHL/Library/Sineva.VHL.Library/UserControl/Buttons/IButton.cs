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
    public partial class IButton : Button
	{
        #region enum
        [Flags]
        public enum enButtonType
        {
            Normal = 0x0,
            Pushed = 1 << 1,
            Toggle = 1 << 2,
        }
        #endregion

        #region Fields
        Image m_overImage;
        Image m_downImage;
        Image m_upImage;
        Image m_defaultImage;
		Image m_ImageBufDefault;
		Image m_ImageBufUp;
		Image m_BgDefault;
		Image m_BgPushed;
		Image m_BgOver;
		Image m_BgDisable;
        enButtonType m_ButtonType;
        bool m_BtnDown;
		bool m_Check = false;

        private ToolTip m_ToolTip = new ToolTip();
        #endregion

        #region Constructor
        public IButton()
        {
            InitializeComponent();
			this.MouseDown += new MouseEventHandler(IButton_MouseDown);
			this.MouseLeave +=new EventHandler(IButton_MouseLeave);
			this.MouseEnter += new EventHandler(IButton_MouseEnter);
			this.MouseUp += new MouseEventHandler(IButton_MouseUp);
			this.MouseHover += new EventHandler(IButton_MouseOver);
			this.BackgroundImageLayout = ImageLayout.Stretch;

		}
		#endregion

		#region Properties

		public bool UseOneImage { get; set; }

		public Image BgDefault
		{
			get { return m_BgDefault; }
			set 
			{ 
				this.BackgroundImage = value;
				m_BgDefault = value;
			}
		}

		public Image BgPushed
		{
			get { return m_BgPushed; }
			set { m_BgPushed = value; }
		}

		public Image BgOver
		{
			get { return m_BgOver; }
			set { m_BgOver = value; }
		}

		public Image BgDisable
		{
			get { return m_BgDisable; }
			set { m_BgDisable = value; }
		}

		public Image DefaultImage
        {
            set 
            { 
                m_defaultImage = value;
				m_ImageBufDefault = value;
                Image = value;
            }
            get { return m_defaultImage; }
        }

        public Image OverImage
        {
            set { m_overImage = value; }
            get { return m_overImage; }
        }

        public Image DownImage
        {
            set { m_downImage = value; }
            get { return m_downImage; }
        }

        public Image UpImage
        {
            set 
			{
				m_upImage = value;
				m_ImageBufUp = value;
			}
            get { return m_upImage; }
        }

        public enButtonType ButtonType
        {
            set { m_ButtonType = value; }
            get { return m_ButtonType; }
        }

		public Color ConnectedLableOnColor { get; set; }
		
		public Color ConnectedLableOffColor { get; set; }

		public Label ConnectedLabel { get; set; }

		public string Description { get; set; }

        public bool IsChecked { get { return m_Check; } }
		#endregion

		#region Methods
		//public void Init()
		//{
		//	this.Image = UpImage;
		//}
		public void SetCheck(bool check)
		{
			m_Check = check;
			if (check)
			{
				//m_ImageBuf = this.DefaultImage;
				this.m_defaultImage = m_overImage;
				this.m_upImage = m_overImage;
				this.Image = m_overImage;
			}
			else
			{
				this.m_defaultImage = m_ImageBufDefault;
				this.m_upImage = m_ImageBufUp;
				this.Image = m_defaultImage;
			}
		}

		public void SetConnectedLableOn()
		{
			if (this.ConnectedLabel == null) return;
			this.ConnectedLabel.ForeColor = ConnectedLableOnColor;
		}

		public void SetConnectedLableOff()
		{
			if (this.ConnectedLabel == null) return;
			this.ConnectedLabel.ForeColor = ConnectedLableOffColor;
		}

		private void IButton_MouseEnter(object sender, EventArgs e)
        {
			if (this.Enabled == false) return;

			if (ButtonType == enButtonType.Pushed)
            {
				if (UseOneImage == false) this.Image = OverImage;
				this.BackgroundImage = m_BgOver;
			}
            else
            {
                if (UseOneImage == false) this.Image = OverImage;
				this.BackgroundImage = m_BgOver;
			}
        }

		private void IButton_MouseDown(object sender, MouseEventArgs e)
        {
			if (this.Enabled == false) return;

			if (ButtonType == enButtonType.Pushed)
            {
				if (UseOneImage == false) this.Image = DownImage;
                if (!m_BtnDown) m_BtnDown = true;
                else m_BtnDown = false;
            }
            else
            {
				if (UseOneImage == false) this.Image = DownImage;
				this.BackgroundImage = m_BgPushed;
            }
            ButtonLog.WriteLog(this.Name.ToString(), "IButton_MouseDown");
        }

		private void IButton_MouseUp(object sender, MouseEventArgs e)
        {
			if (this.Enabled == false) return;
			
			if (ButtonType == enButtonType.Pushed)
            {
                if( !m_BtnDown ) this.Image = UpImage;
            }
            else
            {
				if (UseOneImage == false) this.Image = UpImage;
				this.BackgroundImage = m_BgOver;
			}
            ButtonLog.WriteLog(this.Name.ToString(), "IButton_MouseUp");
        }

		private void IButton_MouseLeave(object sender, EventArgs e)
        {
			if (this.Enabled == false) return;

            if (ButtonType == enButtonType.Pushed)
            {
				if (UseOneImage == false) this.Image = UpImage;
				this.BackgroundImage = m_BgDefault;
			}
            else
            {
				if (UseOneImage == false) this.Image = UpImage;
				this.BackgroundImage = m_BgDefault;
            }
        }
        private void IButton_EnabledChanged(object sender, EventArgs e)
        {
            if (this.Enabled == false)
            {
                this.BackgroundImage = BgDisable;
            }
            else
            {
                this.BackgroundImage = BgDefault;
                this.Update();
            }
        }
        private void IButton_MouseOver(object sender, EventArgs e)
        {
			m_ToolTip.RemoveAll();
			string msg = string.Format("{0}", this.Description);
			m_ToolTip.Show(msg, this, 5000);
        }
        #endregion

    }
}
