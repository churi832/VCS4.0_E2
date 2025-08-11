using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Device.ServoControl
{
    public class UIEditorDevAxisTag : UITypeEditor
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public UIEditorDevAxisTag()
        {
        }
        #endregion

        #region Methods
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            using (FormAxisSelect form = new FormAxisSelect())
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (form.AxisTag != null)
                    {
                        DevAxisTag devAxisTag = new DevAxisTag(form.AxisTag.AxisName, form.AxisTag.AxisId);
                        value = devAxisTag;
                    }
                    else
                    {
                        DevAxisTag devAxisTag = new DevAxisTag();
                        devAxisTag.IsValid = false;
                        value = devAxisTag;
                    }
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
