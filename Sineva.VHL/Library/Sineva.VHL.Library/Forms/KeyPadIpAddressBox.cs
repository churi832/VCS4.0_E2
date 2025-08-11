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
	public partial class KeyPadIpAddressBox : KeyPad
	{
		#region Constructor
		public KeyPadIpAddressBox(string oldValue)
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			m_OldValue = oldValue;
			m_NewValue = oldValue;
		}

		public KeyPadIpAddressBox()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}
		#endregion

		#region Methods
		private void btnOk_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(txtNewVal.Text))
			{
				string pattern = @"^((0|1[0-9]{0,2}|2[0-9]{0,1}|2[0-4][0-9]|25[0-5]|[3-9][0-9]{0,1})\.){3}(0|1[0-9]{0,2}|2[0-9]{0,1}|2[0-4][0-9]|25[0-5]|[3-9][0-9]{0,1})$";

				System.Text.RegularExpressions.Regex regIp = new System.Text.RegularExpressions.Regex(pattern);
				if (regIp.IsMatch(txtNewVal.Text) == false)
				{
					MessageBox.Show("Wrong Data");
					this.DialogResult = DialogResult.Cancel;
				}
				else
				{
					m_NewValue = txtNewVal.Text;
					this.DialogResult = DialogResult.OK;
				}
			}
			else
			{
				MessageBox.Show("Wrong Data");
				this.DialogResult = DialogResult.Cancel;
			}

            this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void KeyPadIpAddressBox_Load(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(m_OldValue))
			{
				txtOldVal.Text = m_OldValue;
			}
			if (!string.IsNullOrEmpty(m_NewValue))
			{
				txtNewVal.Text = m_NewValue;
			}

			this.lblFormat.Text = m_Validation.Format.ToString();

			if (m_Validation.LimitCheckUse)
			{
				this.lblRange.Text = m_Validation.Low + " ~ " + m_Validation.High;
				this.lblRange.Visible = true;
				this.lblRangeTitle.Visible = true;
			}

			//this.txtNewVal.SetValidationInfo(m_Validation);
		}
		#endregion
	}
}
