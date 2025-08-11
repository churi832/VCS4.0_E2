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
	public partial class FormKeyPadSelect : Form
	{
		private Type m_Type = null;
		private static List<Type> m_Types = null;

		public Type KeyPadType
		{
			get { return m_Type; }
		}

		public FormKeyPadSelect()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}

		public void Initialize(Type type)
		{
			m_Type = type;
			this.labelSelectedTitle.Text = (m_Type != null) ? m_Type.Name : "Not Selected";
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
            this.Close();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.listBox1.SelectedItem == null) return;
			Type type = this.listBox1.SelectedItem as Type;
			this.labelSelectedTitle.Text = type.Name;
		}

		private void buttonSelect_Click(object sender, EventArgs e)
		{
			if (this.listBox1.SelectedItem == null)
			{
				MessageBox.Show("Not Selected!");
				return;
			}

			m_Type = this.listBox1.SelectedItem as Type;
            this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void FormKeyPadSelect_Load(object sender, EventArgs e)
		{
			if (m_Types == null)
			{
				m_Types = new List<Type>();

				//jemoon : Design Time에서는 아래의 방법이 안통한다.
				//그래서 일단은 수동으로 Type 추가

				m_Types.Add(typeof(KeyPadTextBox));


				//Assembly asm = Assembly.Load(this.GetType().Namespace);
				//Type[] types = asm.GetTypes();
				//foreach (Type type in types)
				//{
				//    if (XFunc.CheckTypeCompatibility(typeof(KeyPad), type, Compatibility.Compatible))
				//    {
				//        if (type == typeof(KeyPad))
				//        {
				//            m_Types.Add(Activator.CreateInstance(type) as KeyPad);
				//        }
				//    }
				//}
			}

			this.listBox1.DataSource = m_Types;
		}

		private void buttonClear_Click(object sender, EventArgs e)
		{
			m_Type = null;
            this.DialogResult = DialogResult.OK;
		}
	}
}
