using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Library.Servo
{
	public partial class FormAddAxis : Form
	{
		#region Fields
		private List<_Axis> m_Types = new List<_Axis>();
		private List<_Axis> m_NewAxes = new List<_Axis>();
		private int m_AddCount = 1;
		#endregion

		#region Properties
		public List<_Axis> NewAxes
		{
			get { return m_NewAxes; }
		}
		#endregion

		public FormAddAxis()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			List<_Axis> lists = _AxisBlock.GetAxisTypes();
            foreach (_Axis ax in lists)
            {
                if (ax.AxisName.Contains("HomeOffset")) continue;
                if (ax.AxisName.Contains(string.Format("Spare{0}", ax.AxisId))) continue;
                m_Types.Add(ax);
            }

            this.listBoxAxisTypes.DataSource = m_Types;
			this.cbCountSel.SelectedIndex = 0;
            this.cbCountSel.Items.Clear();
            for (int i = 0; i < 70; i++) this.cbCountSel.Items.Add(string.Format("{0}", i + 1));
		}

		private void listBoxAxisTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.propertyGrid1.SelectedObject = this.listBoxAxisTypes.SelectedItem;
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			IAxisFactory factory = this.listBoxAxisTypes.SelectedItem as IAxisFactory;
			for (int i = 0; i < m_AddCount; i++)
			{
				_Axis axis = factory.CreateObject();
				//axis = this.propertyGrid1.SelectedObject as _Axis;
				//axis.CreateChannels();
				NewAxes.Add(axis);
			}
			this.Dispose();
		}

		private void btnCanel_Click(object sender, EventArgs e)
		{
			this.Dispose();
		}

		private void cbCountSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_AddCount = int.Parse((string)(this.cbCountSel.SelectedItem));
		}
	}
}
