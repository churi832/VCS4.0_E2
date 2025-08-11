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
	public partial class DlgSelectValue : Form
	{
		#region Enum
		public enum SelectMode
		{
			ItemList,
			ItemCopy
		}
		#endregion

		#region Fields
		private SelectMode m_SelectMode = SelectMode.ItemCopy;
		private string m_SelectedValue;
		private string m_CurValue = null;
		private string[] m_Items = null;
		private string m_Caption = "";
		#endregion

		#region Properties
		public string SelectedValue
		{
			get { return m_SelectedValue; }
		}
		public string Caption
		{
			get { return m_Caption; }
			set { m_Caption = value; }
		}
		public string FirstLabelText
		{
			get { return lblFrom.Text; }
			set { lblFrom.Text = value; }
		}
		public string SecondLabelText
		{
			get { return lblTo.Text; }
			set { lblTo.Text = value; }
		}
		public SelectMode ItemSelectMode
		{
			get { return m_SelectMode; }
			set { m_SelectMode = value; }
		}
		#endregion

		#region Constructor
		public DlgSelectValue(string curValue, string[] items)
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			m_CurValue = curValue;
			m_Items = items;

			if (m_SelectMode == DlgSelectValue.SelectMode.ItemList)
			{
				this.FirstLabelText = "Current :";
				this.SecondLabelText = "New :";
			}
		}
		#endregion

		#region Methods
		private void btnOk_Click(object sender, EventArgs e)
		{
			if (m_SelectedValue != null) this.DialogResult = DialogResult.OK;
			else this.DialogResult = DialogResult.Cancel;
            this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;

            this.Close();
		}

		private void cboNewValue_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (((ComboBox)sender).SelectedItem == null) return;
			m_SelectedValue = (string)(((ComboBox)sender).SelectedItem);
		}

		private void DlgSelectValue_Load(object sender, EventArgs e)
		{
			this.Text = m_Caption;

			if (m_Items != null)
			{
				foreach (string item in m_Items)
				{
					cboNewValue.Items.Add(item);
				}
			}

			if (m_CurValue != null)
			{
				txtCurValue.Text = m_CurValue;
				if (cboNewValue.Items != null)
				{
					cboNewValue.SelectedItem = m_CurValue;
				}
			}
		}
		#endregion

	}
}
