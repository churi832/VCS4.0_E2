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
using System.Drawing.Design;

namespace Sineva.VHL.Library
{
	public partial class KeyPad : Form
	{
		#region Fields
		protected KeyInValidation m_Validation = null;
		protected string m_OldValue = "";
		private BackgroundWorker backgroundWorker1;
		protected string m_NewValue = "";
		#endregion

		#region Properties
		[Category("HIT : Setting")]
		public KeyInValidation Validation
		{
			get { return m_Validation; }
			set { m_Validation = value; }
		}
		[Category("HIT : Setting")]
		public string OldValue
		{
			get { return m_OldValue; }
			set { m_OldValue = value; }
		}
		[Category("HIT : Setting")]
		public string NewValue
		{
			get { return m_NewValue; }
			set { m_NewValue = value; }
		}
		[Category("HIT : Setting")]
		public string Caption
		{
			get { return this.Text; }
			set { this.Text = value; }
		}
		#endregion

		#region Constructor
		public KeyPad()
		{
		}
		#endregion

		#region Override
		public override string ToString()
		{
			return this.GetType().Name;
		}
		#endregion

		private void InitializeComponent()
		{
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.SuspendLayout();
			// 
			// KeyPad
			// 
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Name = "KeyPad";
			this.ResumeLayout(false);
		}
	}

	[Serializable()]
	[Editor(typeof(UIEditorKeyPadSelect), typeof(UITypeEditor))]
	public class KeyPadInfo
	{
		private Type m_Type = null;
		public Type KeyPadType
		{
			get { return m_Type; }
			set { m_Type = value; }
		}

		public KeyPadInfo()
		{
		}

		public KeyPad CreateInstance()
		{
			return Activator.CreateInstance(m_Type) as KeyPad;
		}

		public override string ToString()
		{
			return (m_Type != null) ? m_Type.Name : "Not Seleted";
		}
	}
}
