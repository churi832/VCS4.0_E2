using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Sineva.VHL.Library
{
	public partial class FormCollectionPropertyEdit : Form
	{
        public List<object> m_Object = new List<object>();
        private object m_Context = null;
        private Type Object_Type = null;
		public FormCollectionPropertyEdit()
		{
			InitializeComponent();
		}

		public FormCollectionPropertyEdit(object obj, string title = "")
		{
			InitializeComponent();

            this.Text = "Property Editor : " + title;
		}
        public FormCollectionPropertyEdit(object context, object obj)
		{
			InitializeComponent();

            Object_Type = obj.GetType();
            this.lbCollection.DataSource = obj;

            foreach (object obj1 in this.lbCollection.Items)
            {
                m_Object.Add(obj1);
            }
            //foreach (PropertyInfo info in XFunc.GetProperties(obj))
            //{
            //    object obj1 = info.GetValue(obj, null);
            //    m_Object.Add(obj1);
            //}
            m_Context = context;


            //PropertyInfo[] info = context.GetType().GetProperties();
            //System.Collections.IList lists = (System.Collections.IList)info[0].GetValue(obj, null);

            PropertyInfo obj2 = context.GetType().GetProperty("PropertyName");
            object context_name = obj2.GetValue(context, null);

            this.Text = "Property Editor : " + context_name;
        }
		
		private void btnOk_Click(object sender, EventArgs e)
        {
            //m_Object = lbCollection.Items;
			DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            lbCollection.Refresh();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            object newT = new object();
            newT = Activator.CreateInstance(Object_Type.BaseType);
            m_Object.Add(newT);
            this.lbCollection.DataSource = m_Object;
            this.lbCollection.Refresh();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            int index = lbCollection.SelectedIndex;
            this.lbCollection.DataSource = null;
            m_Object.RemoveAt(index);
            this.lbCollection.DataSource = m_Object;
        }

        private void lbCollection_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.propertyGrid1.DeviceRuntimeSettignProperty(lbCollection.SelectedItem);

        }
	}
}
