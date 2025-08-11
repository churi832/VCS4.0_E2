using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Design;

namespace Sineva.VHL.Library.Servo
{
    public class UIEditorTeachingPosition : UITypeEditor
    {
		#region Fields
		#endregion

		#region Properties
		#endregion

		#region Constructor
        public UIEditorTeachingPosition()
		{
		}
		#endregion

		#region Methods
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
            using (FormTeachingPositionSelect form = new FormTeachingPositionSelect())
			{
				if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if(form.SelectedTeachingPoint != null) value = form.SelectedTeachingPoint;
                    else value = new TeachingData();
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
