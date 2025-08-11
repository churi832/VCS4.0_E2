using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Sineva.OHT.Common
{
    public class ConfigFileSelector : UITypeEditor
    {
        #region Constructor
        public ConfigFileSelector()
        {
        }
        #endregion

        #region Override
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider sp, object value)
        {
            //IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)sp.GetService(typeof(IWindowsFormsEditorService));
            //if (edSvc == null) return value;

            //FormSimulationConfigHelper form = new FormSimulationConfigHelper();
            //FolderBrowserDialog dlg = new FolderBrowserDialog();
            FileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = Application.StartupPath;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FileSelect file = new FileSelect();
                file.SelectedFile = dlg.FileName;
                //path.SelectedPath = dlg.SelectedPath;

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
