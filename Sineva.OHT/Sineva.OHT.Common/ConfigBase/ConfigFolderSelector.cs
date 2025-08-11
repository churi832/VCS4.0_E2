using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Sineva.OHT.Common
{
    public class ConfigFolderSelector : UITypeEditor
    {
        #region Constructor
        public ConfigFolderSelector()
        {
        }
        #endregion

        #region Override
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.SelectedPath = Application.StartupPath;

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
