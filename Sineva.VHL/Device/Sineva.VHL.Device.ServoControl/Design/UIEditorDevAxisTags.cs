using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Design;
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.Device.ServoControl
{
    public class UIEditorDevAxisTags : UITypeEditor
    {
		#region Fields
		#endregion

		#region Properties
		#endregion

		#region Constructor
        public UIEditorDevAxisTags()
		{
		}
		#endregion

		#region Methods
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (value.GetType() != typeof(List<DevAxisTag>)) return value;

            using (FormSelectDevAxisTags form = new FormSelectDevAxisTags(value as List<DevAxisTag>))
			{
				if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (form.AxesTag != null)
                    {
						value = form.AxesTag;
                    }
					else
					{
						List<DevAxisTag> devAxisTag = new List<DevAxisTag>();
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
