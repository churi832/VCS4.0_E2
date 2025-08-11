using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sineva.VHL.Library;
using Sineva.VHL.Data;

namespace Sineva.VHL.Data.LogIn
{
	public partial class LogInView : UserControl
	{
		public string Message
		{
			get { return this.button1.Text; }
			set 
			{ 
				this.button1.Text = value;
			}
		}

		public LogInView()
		{
			InitializeComponent();
		}

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                ButtonLog.WriteLog(string.Format("{0} : Mouse Click !", this.Name.ToString()));
                using (LogInForm form = new LogInForm())
                {
                    form.TopMost = true;
                    form.ShowDialog();
                }
            }
            catch(Exception ex)
            {
                return;
            }
        }
	}

	public class LogInChangeArgs : EventArgs
	{
		public string message;
	}

}
