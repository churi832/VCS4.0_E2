using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Design;

namespace Sineva.VHL.Library.Servo
{
    public class UIEditorServoUnitTag : UITypeEditor
    {
		#region Fields
		#endregion

		#region Properties
		#endregion

		#region Constructor
        public UIEditorServoUnitTag()
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
                    if (form.ServoTag != null) value = form.ServoTag;
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
