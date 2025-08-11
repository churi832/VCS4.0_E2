/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.16 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Drawing.Design;

namespace Sineva.VHL.Library
{
    public class UIEditorPropertyEdit : UITypeEditor
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public UIEditorPropertyEdit()
		{
		}
        #endregion

        #region Methods
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider sp, object value)
		{
			System.Windows.Forms.Design.IWindowsFormsEditorService edSvc = (System.Windows.Forms.Design.IWindowsFormsEditorService)sp.GetService(typeof(System.Windows.Forms.Design.IWindowsFormsEditorService));

			if (edSvc == null)
			{   // uh oh.
				return value;
			}

			using (FormPropertyEdit form = new FormPropertyEdit(value))
			{
				if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					value = form.m_Object;
				}
			}
			return value;
		}

        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }
        #endregion
    }
}
