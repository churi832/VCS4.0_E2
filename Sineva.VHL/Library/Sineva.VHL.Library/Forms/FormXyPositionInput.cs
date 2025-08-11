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
	public partial class FormXyPositionInput : Form
	{
		public XyPosition m_Result = null;
		public XyPosition m_Origin = null;

		public FormXyPositionInput()
		{
			InitializeComponent();
			m_Result = new XyPosition();
		}

		public FormXyPositionInput(XyPosition pos)
		{
			m_Origin = pos;
			m_Result = new XyPosition();

			InitializeComponent();
            try
            {
                this.tbX.Text = pos.X.ToString();
                this.tbY.Text = pos.Y.ToString();
            }
            catch
            {

            }
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                double result = 0;
                if (double.TryParse(tbX.Text, out result)) m_Result.X = result;
                if (double.TryParse(tbY.Text, out result)) m_Result.Y = result;
            }
			catch (Exception err)
			{
                ExceptionLog.WriteLog(method, String.Format(err.ToString()));
            }
            //ButtonLog.WriteLog(this.Name.ToString(), string.Format("OK Click = [X={0}][Y={1}]", m_Result.X, m_Result.Y));
            //ButtonLog.WriteLog(this.Name.ToString(), "btnOk_Click", true);
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Dispose();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
            //ButtonLog.WriteLog(this.Name.ToString(), string.Format("Cancel Click = [X={0}][Y={1}]", m_Result.X, m_Result.Y));
            //ButtonLog.WriteLog(this.Name.ToString(), "btnCancel_Click", true);

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Dispose();
		}
	}
}
