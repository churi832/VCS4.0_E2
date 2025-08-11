using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Design;

namespace Sineva.VHL.Library.Servo
{
    public class UIEditorTeachingVelocity : UITypeEditor
    {
		#region Fields
		#endregion

		#region Properties
		#endregion

		#region Constructor
        public UIEditorTeachingVelocity()
		{
		}
		#endregion

		#region Methods
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
            using (FormTeachingVelocitySelect form = new FormTeachingVelocitySelect())
			{
				if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if(form.SelectedVelocityPoint != null) value = form.SelectedVelocityPoint;
                    else value = new VelocityData();
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
