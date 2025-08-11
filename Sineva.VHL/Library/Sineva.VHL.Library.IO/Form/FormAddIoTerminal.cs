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
	public partial class FormAddIoTerminal : Form
	{
		#region Fields
		private List<_IoTerminal> m_Types = new List<_IoTerminal>();
		private List<_IoTerminal> m_NewTerminals = new List<_IoTerminal>();
		private int m_AddCount = 1;
		#endregion

		#region Properties
		public List<_IoTerminal> NewTerminals
		{
			get { return m_NewTerminals; }
		}
		#endregion

		#region Constructor
		public FormAddIoTerminal()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			m_Types = _IoNode.GetTerminalTypes();
			this.listBoxTerminalTypes.DataSource = m_Types;
			this.cbCountSel.SelectedIndex = 0;
		}
		public FormAddIoTerminal(_IoNode node)
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			m_Types = _IoNode.GetTerminalTypes(node.IoNodeBusType);
			this.listBoxTerminalTypes.DataSource = m_Types;
			this.cbCountSel.SelectedIndex = 0;
		}
		#endregion

		#region Events
		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.propertyGrid1.SelectedObject = this.listBoxTerminalTypes.SelectedItem;
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
            ITerminalFactory factory = this.listBoxTerminalTypes.SelectedItem as ITerminalFactory;
            _IoTerminal terminal = factory.CreateObject();
            terminal = this.propertyGrid1.SelectedObject as _IoTerminal;
            terminal.CreateChannels();

            for (int i = 0; i < m_AddCount; i++)
			{
                m_NewTerminals.Add(terminal);
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
		#endregion
	}
}
