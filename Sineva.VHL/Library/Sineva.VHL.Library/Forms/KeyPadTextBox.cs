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
	public partial class KeyPadTextBox : KeyPad
	{
		#region Constructor
		public KeyPadTextBox()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}

		public KeyPadTextBox(string oldValue)
			: this()
		{
			m_OldValue = oldValue;
			m_NewValue = oldValue;
		}
		#endregion


		#region Methods
		private void btnOk_Click(object sender, EventArgs e)
		{
			string newValue = txtNewVal.Text;
			bool ok = true;
			if (string.IsNullOrEmpty(newValue))
			{
				ok = false;
			}
			else
			{
				newValue = m_Validation.CheckRange(newValue);

				if (m_Validation.Format == OptionFormat.IpAddress)
				{
					char[] token = { '.' };
					string[] temp = newValue.Split(token);

					if (temp == null || temp.Length < 4)
					{
						ok = false;
					}
				}
			}

			if (ok)
			{
				m_NewValue = newValue;
				this.DialogResult = DialogResult.OK;
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

		private void KeyPadTextBox_Load(object sender, EventArgs e)
		{
			if (m_Validation.Format == OptionFormat.FilePath)
			{
				btnSelectFolder.Visible = true;
				txtNewVal.ReadOnly = true;

				if (!string.IsNullOrEmpty(m_OldValue))
				{
					txtOldVal.Text = m_OldValue;
				}
			}
			else
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

				this.txtNewVal.SetValidationInfo(m_Validation); 
				this.txtNewVal.UsedInKeyPad = true;
			}
		}
		#endregion

		private void btnSelectFolder_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				string path = dlg.SelectedPath;
				txtNewVal.Text = path;
			}
		}
	}
}
