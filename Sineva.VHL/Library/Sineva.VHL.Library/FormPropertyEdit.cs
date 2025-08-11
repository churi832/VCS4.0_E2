/****************************************************************
 * Copyright	: www.kpscorp.co.kr
 * Version		: V1.0
 * Programmer	: KPS-개발4팀
 * Issue Date	: 14.01.21
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

namespace ICS.Lib
{
	public partial class FormPropertyEdit : Form
	{
		public object m_Object = null;
        public string m_Context = string.Empty;
		public FormPropertyEdit()
		{
			InitializeComponent();
		}

		public FormPropertyEdit(object obj, string title = "")
		{
			InitializeComponent();

			m_Object = obj;
            this.Text = "Property Editor : " + title;
		}
		public FormPropertyEdit(string context, object obj)
		{
			InitializeComponent();

            m_Object = obj;
            m_Context = context;
			this.propertyGrid1.SelectedObject = m_Object;
  		}
		
		private void btnOk_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}
	}
}
