using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Design;
using Sineva.VHL.Data.DbAdapter;

namespace Sineva.VHL.Data.Process
{
    public class UIEditorPortSelect : UITypeEditor
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public UIEditorPortSelect()
        {
        }
        #endregion

        #region Methods
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value.GetType() != typeof(int)) return value;

            using (UIEditorPortSelectForm form = new UIEditorPortSelectForm((int)-1))
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return form.SelectPortID;
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
