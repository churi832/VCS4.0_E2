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
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Sineva.VHL.Library.IO
{
	public class UIEditorIoTag : UITypeEditor
	{
		#region Fields
		#endregion

		#region Properties
		#endregion

		#region Constructor
		public UIEditorIoTag()
		{
		}
		#endregion

		#region Methods
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			using (FormIoSelect form = new FormIoSelect())
			{
				if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
                    value = form.m_Tag;
                }
            }
			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		#endregion
	}
}
