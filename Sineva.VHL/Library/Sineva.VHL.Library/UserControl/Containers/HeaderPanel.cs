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
	public partial class HeaderPanel : Panel
	{
		private Control m_FriendControl;
		Point m_MyOrgLocation;
		Point m_FriendLocation;
		bool m_Once = true;
                
		[Category("!Title")]
		public string Title
		{
			get { return this.label1.Text; }
			set { this.label1.Text = value; }
		}

		[Category("!Title")]
		public Color TitleColor
		{
			get { return this.label1.BackColor; }
			set { this.label1.BackColor = value; }
		}

		[Category("!Title")]
		public Color TitleTextColor
		{
			get { return this.label1.ForeColor; }
			set { this.label1.ForeColor = value; }
		}
        [Category("!Title")]
        public ContentAlignment TitleTextAlign
        {
            get { return this.label1.TextAlign; }
            set { this.label1.TextAlign = value; }
        }
        [Category("!Title")]
        public BorderStyle TitleBorderStyle
        {
            get { return this.label1.BorderStyle; }
            set { this.label1.BorderStyle = value; }
        }

        public Control FriendControl
		{
			get { return m_FriendControl; }
			set 
			{ 
				m_FriendControl = value;
				if (m_FriendControl != null)
				{
					m_FriendLocation = m_FriendControl.Location;
					m_FriendControl.SizeChanged += new EventHandler(m_FriendControl_SizeChanged);
					m_FriendControl.LocationChanged += new EventHandler(m_FriendControl_LocationChanged);
				}
			}
		}


		public HeaderPanel()
		{
			InitializeComponent();
		}

        private void HeaderPanel_ControlAdded(object sender, System.Windows.Forms.ControlEventArgs e)
        {
            Control obj = sender as Control;
        }

        private void m_FriendControl_SizeChanged(object sender, EventArgs e)
		{
			Control obj = sender as Control;
			if (m_Once)
			{
				m_MyOrgLocation = this.Location;
				m_Once = false;
			}
			int dx = m_FriendLocation.X - obj.Location.X;
			int dy = m_FriendLocation.Y - obj.Location.Y;
			this.Location = new Point(m_MyOrgLocation.X + dx, m_MyOrgLocation.Y + dy);
		}

		private void m_FriendControl_LocationChanged(object sender, EventArgs e)
		{
			Control obj = sender as Control;
			if (m_Once)
			{
				m_MyOrgLocation = this.Location;
				m_Once = false;
			}
			int dx = obj.Location.X - m_FriendLocation.X;
			int dy = obj.Location.Y - m_FriendLocation.Y;
			this.Location = new Point(m_MyOrgLocation.X + dx, m_MyOrgLocation.Y + dy);
		}

		private void IcsPanel_Resize(object sender, EventArgs e)
		{
			this.label1.Size = new Size(this.Size.Width + 2, 20);
		}
	}
}
