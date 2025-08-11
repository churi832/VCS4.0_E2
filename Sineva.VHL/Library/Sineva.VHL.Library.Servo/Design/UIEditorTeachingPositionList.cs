using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Design;

namespace Sineva.VHL.Library.Servo
{
    public class UIEditorTeachingPositionList : UITypeEditor
    {
		#region Fields
		#endregion

		#region Properties
		#endregion

		#region Constructor
        public UIEditorTeachingPositionList()
		{
		}
		#endregion

		#region Methods
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (value.GetType() != typeof(List<TeachingData>)) return value;

			using (FormTeachingPositionListSelect form = new FormTeachingPositionListSelect(value as List<TeachingData>))
			{
				if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if(form.SelectedTeachingPoint != null) value = form.SelectedTeachingPoint;
                    else value = new List<TeachingData>();
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
