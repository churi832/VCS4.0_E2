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
	public enum enRGBSelect : int
	{
		Red = 0,
		Green = 1,
		Blue = 2,
        Yellow = 3,
	}

	public partial class RGBLabel : UserControl
	{
		#region Fields
		Image[] m_Images = new Image[4];
		enRGBSelect m_CurBg = enRGBSelect.Blue;
		#endregion

		#region Properties
		public Font TextFont
		{
			get { return this.label1.Font; }
			set { this.label1.Font = value; }
		}

		public string LabelText
		{
			get { return this.label1.Text; }
			set { this.label1.Text = value; }
		}

		public Color LableColor
		{
			get { return this.label1.ForeColor; }
			set { this.label1.ForeColor = value; }
		}

		public enRGBSelect BgSelect
		{
			get
			{
				return m_CurBg;
			}
			set
			{
				m_CurBg = value;
				SelectBackGround(m_CurBg);
			}
		}

		#endregion

		#region Constructor
		public RGBLabel()
		{
			InitializeComponent();
			m_Images[0] = Properties.Resources.Frame_Red;
			m_Images[1] = Properties.Resources.Frame_Green;
			m_Images[2] = Properties.Resources.Frame_Blue;
            m_Images[3] = Properties.Resources.Frame_Yellow;
		}
		#endregion

		#region Methods
		public void SelectBackGround(enRGBSelect rgb)
		{
            if(this.BackgroundImage != m_Images[(int)rgb]) this.BackgroundImage = m_Images[(int)rgb];
		}
		#endregion
	}
}
