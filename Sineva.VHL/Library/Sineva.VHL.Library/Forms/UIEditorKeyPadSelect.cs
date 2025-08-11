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
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;

namespace Sineva.VHL.Library
{
	public class UIEditorKeyPadSelect : UITypeEditor
	{
		#region Constructor
		public UIEditorKeyPadSelect()
		{
		}
		#endregion

		#region Override
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider sp, object value)
		{
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)sp.GetService(typeof(IWindowsFormsEditorService));
			if (edSvc == null) return value;

			KeyPadInfo keyPadInfo = (value as KeyPadInfo);
			if (keyPadInfo == null) keyPadInfo = new KeyPadInfo();

			FormKeyPadSelect ui = new FormKeyPadSelect();
			ui.Initialize(keyPadInfo.KeyPadType);    //Set current KeyPad
			edSvc.ShowDialog(ui);

			if (ui.DialogResult == DialogResult.OK)
			{
				keyPadInfo = new KeyPadInfo();
				keyPadInfo.KeyPadType = ui.KeyPadType;
			}

			return keyPadInfo;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		#endregion
	}
}
