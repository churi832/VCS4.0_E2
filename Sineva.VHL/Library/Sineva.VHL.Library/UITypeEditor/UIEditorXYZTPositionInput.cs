/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.17 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;

namespace Sineva.VHL.Library
{
	public class UIEditorXYZTPositionInput : UITypeEditor
	{
        public UIEditorXYZTPositionInput()
		{
		}

		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			FormXyztPositionInput form = new FormXyztPositionInput((XyztPosition)value);
			if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				value = form.m_Result;
			}
			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			//return base.GetEditStyle(context);
			return UITypeEditorEditStyle.Modal;
		}
	}
}
