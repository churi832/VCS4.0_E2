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

namespace Sineva.VHL.Library.IO
{
	public class UIEditorIoTags : UITypeEditor
	{
		#region Fields
		#endregion

		#region Properties
		#endregion

		#region Constructor
		public UIEditorIoTags()
		{
		}
		#endregion

		#region Methods
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (value.GetType() != typeof(List<IoTag>)) return value;

			using (FormIoTagsSelect form = new FormIoTagsSelect(value as List<IoTag>))
			{
				if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
					if (form.m_Tags != null) value = form.m_Tags;
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
