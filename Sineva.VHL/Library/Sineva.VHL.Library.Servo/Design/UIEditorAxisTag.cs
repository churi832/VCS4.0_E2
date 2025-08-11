using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Sineva.VHL.Library.Servo
{
    [Serializable]
    public class UIEditorAxisTag : UITypeEditor
    {
		#region Fields
		#endregion

		#region Properties
		#endregion

		#region Constructor
        public UIEditorAxisTag()
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
					//if (form.AxisTag != null) value = form.AxisTag as AxisTag;
					value = form.AxisTag as AxisTag; // null 처리할 경우가 있다...!
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
