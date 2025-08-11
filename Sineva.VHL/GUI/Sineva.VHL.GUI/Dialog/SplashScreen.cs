using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ICS.GUI
{
	public partial class SplashScreen : Form
	{
		public SplashScreen(Image backGroundImage)
		{
			InitializeComponent();

			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.BackColor = Color.Transparent;

			if (backGroundImage != null)
			{
				this.BackgroundImage = backGroundImage;
				this.ClientSize = this.BackgroundImage.Size;
			}
			CopyrightShow(this.lbCopyright);

			timer1.Start();
		}

		public SplashScreen(Image backGroundImage, int duration)
		{
			InitializeComponent();

			if (backGroundImage != null)
			{
				this.BackgroundImage = backGroundImage;
				this.Size = this.BackgroundImage.Size;
			}
			CopyrightShow(this.lbCopyright);

			timer1.Interval = duration;
			timer1.Start();
			timer1.Enabled = true;
		}

		public void CopyrightShow(Label lbl)
		{
			string msg = "";
			msg += Assembly.GetEntryAssembly().GetName().Name;
			msg += "\nVersion: ";
			msg += Assembly.GetEntryAssembly().GetName().Version.ToString();
			msg += "\n";
			msg += Assembly.GetEntryAssembly().GetName().VersionCompatibility;
			msg += "\n";
			msg += "\n";
			msg += "ⓒ 2024 Sineva Technology Co., Ltd. All rights reserved";
			msg += "\n";
			msg += "Programmed by sineva.com.cn";
			lbl.Text = msg;
		}

		public void Exit()
		{
			this.Dispose();
			//this.Close();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			Exit();
		}
	}
}
