/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.16 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public class UIEditorFileSelect : UITypeEditor
    {
        #region Constructor
        public UIEditorFileSelect()
        {
        }
        #endregion

        #region Override
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider sp, object value)
        {
            FileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = Application.StartupPath;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FileSelect file = new FileSelect();
                file.SelectedFile = dlg.FileName;
                value = file;
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        #endregion

    }
}
