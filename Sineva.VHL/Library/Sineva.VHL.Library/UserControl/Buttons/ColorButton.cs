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
	public partial class ColorButton : Button
	{
		private Color m_ColorMouseUp;
		private Color m_ColorMouseDown;

		public Color ColorMouseUp
		{
			get { return m_ColorMouseUp; }
			set { m_ColorMouseUp = value; }
		}

		public Color ColorMouseDown
		{
			get { return m_ColorMouseDown; }
			set { m_ColorMouseDown = value; }
		}


		public ColorButton()
		{
			InitializeComponent();
			this.BackColor = m_ColorMouseUp;
		}

		private void ColorButton_MouseDown(object sender, MouseEventArgs e)
		{
			this.BackColor = m_ColorMouseDown;
            ButtonLog.WriteLog(this.Name.ToString(), "IButton_MouseDown");
		}

		private void ColorButton_MouseUp(object sender, MouseEventArgs e)
		{
			this.BackColor = m_ColorMouseUp;
            ButtonLog.WriteLog(this.Name.ToString(), "ColorButton_MouseUp");
        }
	}
}
