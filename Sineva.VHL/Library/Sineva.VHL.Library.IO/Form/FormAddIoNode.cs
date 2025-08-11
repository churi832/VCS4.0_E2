using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Library.IO
{
	public partial class FormAddIoNode : Form
	{
		private List<_IoNode> m_Types = new List<_IoNode>();
		private List<_IoNode> m_NewNodes = new List<_IoNode>();

		public List<_IoNode> NewNodes
		{
			get { return m_NewNodes; }
		}

		public FormAddIoNode()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			m_Types = IoManager.GetNodeTypes();
			this.listBoxNodeSelect.DataSource = m_Types;
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			INodeFactory factory = this.listBoxNodeSelect.SelectedItem as INodeFactory;
			_IoNode ioNode = factory.CreateObject();
			//ioNode.CreateChannels();
			this.m_NewNodes.Add(ioNode); 
			this.Dispose();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Dispose();
		}
	}
}
