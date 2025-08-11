/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V1.0
 * Programmer	: Software Group
 * Issue Date	: 23.02.20
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
	public partial class FormXyztPositionInput : Form
	{
		public XyztPosition m_Result = null;
		public XyztPosition m_Origin = null;

        public FormXyztPositionInput()
		{
			InitializeComponent();
			m_Result = new XyztPosition();
		}

        public FormXyztPositionInput(XyztPosition pos)
		{
			m_Origin = pos;
			m_Result = new XyztPosition();

			InitializeComponent();
            try
            {
                this.tbX.Text = pos.X.ToString();
                this.tbY.Text = pos.Y.ToString();
				this.tbZ.Text = pos.Z.ToString();
                this.tbT.Text = pos.T.ToString();
            }
            catch
            {

            }
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			try
			{
                double result = 0;
                if (double.TryParse(tbX.Text, out result)) m_Result.X = result;
                if (double.TryParse(tbY.Text, out result)) m_Result.Y = result;
                if (double.TryParse(tbZ.Text, out result)) m_Result.Z = result;
                if (double.TryParse(tbT.Text, out result)) m_Result.T = result;
            }
            catch (Exception err)
			{
				ExceptionLog.WriteLog(err.ToString());
			}
            ButtonLog.WriteLog(this.Name.ToString(), string.Format("OK Click = [X={0}][Y={1}][T={2}]", m_Result.X, m_Result.Y, m_Result.T));
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Dispose();
        }

		private void btnCancel_Click(object sender, EventArgs e)
		{
            ButtonLog.WriteLog(this.Name.ToString(), string.Format("Cancel Click = [X={0}][Y={1}][T={2}]", m_Result.X, m_Result.Y, m_Result.T));
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Dispose();
		}
	}
}
