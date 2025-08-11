using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Device.ServoControl
{
    public class UIEditorDevServoUnitTag : UITypeEditor
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public UIEditorDevServoUnitTag()
        {
        }
        #endregion

        #region Methods
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            using (FormServoUnitSelect form = new FormServoUnitSelect())
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (form.ServoTag != null)
                    {
                        DevServoUnitTag devServoUnitTag = new DevServoUnitTag(form.ServoTag.ServoName);
                        value = devServoUnitTag;
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
