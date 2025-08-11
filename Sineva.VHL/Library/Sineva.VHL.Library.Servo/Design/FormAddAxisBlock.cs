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
	public partial class FormAddAxisBlock : Form
	{
		private List<_AxisBlock> m_Types = new List<_AxisBlock>();
		private List<_AxisBlock> m_NewBlocks = new List<_AxisBlock>();

		public List<_AxisBlock> NewBlocks
		{
			get { return m_NewBlocks; }
		}

		public FormAddAxisBlock()
		{
			InitializeComponent();

			m_Types = ServoManager.GetBlockTypes();
			this.listBoxBlockSelect.DataSource = m_Types;
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			IAxisBlockFactory factory = this.listBoxBlockSelect.SelectedItem as IAxisBlockFactory;
			_AxisBlock block = factory.CreateObject();
			//ioNode.CreateChannels();
			this.m_NewBlocks.Add(block);
			this.Dispose();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Dispose();
		}
	}
}
