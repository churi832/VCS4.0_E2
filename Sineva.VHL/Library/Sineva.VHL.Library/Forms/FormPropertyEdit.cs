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
	public partial class FormPropertyEdit : Form
	{
        #region Fields
        public object m_Object = null;
        public string m_Context = string.Empty;
        #endregion

        #region Constructor
        public FormPropertyEdit()
		{
			InitializeComponent();
		}

		public FormPropertyEdit(object obj, string title = "")
		{
			InitializeComponent();

			m_Object = obj;
            this.Text = "Property Editor : " + title;
            this.propertyGrid1.SelectedObject = m_Object;
        }
        public FormPropertyEdit(string context, object obj)
		{
			InitializeComponent();

            m_Object = obj;
            m_Context = context;
			this.propertyGrid1.SelectedObject = m_Object;
  		}
        #endregion

        #region Button Event
        private void btnOk_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}
        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            ButtonLog.WriteLog("PropertyGrid", string.Format("Change Value = Old:{0}, New:{1}", e.OldValue, e.ChangedItem));
        }
        #endregion
    }
}
