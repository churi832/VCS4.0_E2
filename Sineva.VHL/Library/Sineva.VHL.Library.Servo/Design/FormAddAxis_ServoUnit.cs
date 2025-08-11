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
	public partial class FormAddAxis_ServoUnit : Form
	{
		private List<_Axis> m_Types = new List<_Axis>();
		private List<_Axis> m_NewAxes = new List<_Axis>();
		private List<_Axis> m_AxisPool = new List<_Axis>();
		private int m_AddCount = 1;

		#region Properties
		public List<_Axis> NewAxes
		{
			get { return m_NewAxes; }
		}
		#endregion

		#region Constructor
		public FormAddAxis_ServoUnit()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			m_Types = ServoUnit.GetAxisTypes();
			//this.listBoxAxisTypes.DataSource = m_Types;
			this.cbCountSel.SelectedIndex = 0;

			foreach (_AxisBlock block in ServoManager.Instance.AxisBlocks)
			{
				foreach (_Axis axis in block.Axes)
				{
                    if (axis.AxisName == string.Format("Mp{0}Spare{1}", block.BlockId, axis.AxisId)) continue;
                    if (axis.AxisName.Contains("HomeOffset")) continue;

                    m_AxisPool.Add(axis);
				}
			}
			this.listBoxAxisTypes.DataSource = m_AxisPool;
			this.cbCountSel.SelectedIndex = 0;
		}
		#endregion

		private void listBoxAxisTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.propertyGrid1.SelectedObject = this.listBoxAxisTypes.SelectedItem;
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.OK;

			for (int i = 0; i < this.listBoxAxisTypes.SelectedItems.Count; i++)
			{
				m_NewAxes.Add(this.listBoxAxisTypes.SelectedItems[i] as _Axis);
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
