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
    public class UIEditorFolderSelect : UITypeEditor
    {
        #region Constructor
        public UIEditorFolderSelect()
        {
        }
        #endregion

        #region Override
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.SelectedPath = AppConfig.GetSolutionPath();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FolderSelect folder = new FolderSelect();
                folder.SelectedFolder = dlg.SelectedPath;
                value = folder;
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
