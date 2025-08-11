using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Library.IO
{
    public partial class ucConfigIo : UserControl
    {
        #region Fields
        private IoManager m_IoFactory = null;
        private TreeView m_TreeView = null;
        private TreeNode m_CurNode = null;
        private TreeNode m_OldNode = null;
        #endregion

        public ucConfigIo()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
        }

        #region Methods
        public bool Initialize(IoManager factory)
        {
            m_IoFactory = factory;
			if(m_IoFactory.Initialized == false)
				m_IoFactory.ReadXml();

            UpdateTreeView(m_IoFactory);
            this.viewIoEdit1.Initialize(m_IoFactory);
            return true;
        }

        public void Save()
        {
            m_IoFactory.InitializeIoCollection();
            m_IoFactory.WriteXml();

            UpdateTreeView(m_IoFactory);
            this.viewIoEdit1.Initialize(m_IoFactory);
        }

		public void UpdateTreeView(IoManager factory)
		{
			this.treeView.BeginUpdate();
			this.treeView.Nodes.Clear();
			
			TreeNode node0 = new TreeNode();
			node0.Tag = factory;
			node0.Text = factory.ToString();
			LoadNodeImage(node0, factory.GetType(), IoType.DI);
			this.treeView.Nodes.Add(node0);

			foreach (_IoNode node in factory.Nodes)
			{
				TreeNode nodeIoNode = new TreeNode(node.ToString());
				nodeIoNode.Tag = node;
				LoadNodeImage(nodeIoNode, node.GetType(), IoType.DI);
				node0.Nodes.Add(nodeIoNode);

				foreach (_IoTerminal terminal in node.GetTerminals())
				{
					TreeNode nodeTerminal = new TreeNode(terminal.ToString());
					nodeTerminal.Tag = terminal;
					LoadNodeImage(nodeTerminal, terminal.GetType(), terminal.IoTypes[0]);
					nodeIoNode.Nodes.Add(nodeTerminal);
					foreach (IoChannel ch in terminal.GetChannels())
					{
						TreeNode nodeCh = new TreeNode(ch.ToString());
						nodeCh.Tag = ch;
						LoadNodeImage(nodeCh, ch.GetType(), ch.IoType);
						nodeTerminal.Nodes.Add(nodeCh);
					}
				}
				nodeIoNode.Expand();
			}
			node0.Expand();
			this.treeView.EndUpdate();
		}

		private void LoadNodeImage(TreeNode node, Type type, IoType ioType)
		{
			int imageId = 0;
			if (type == typeof(IoManager)) imageId = 0;
			else if (type == typeof(IoNodeMp2100)) imageId = 1;
            else if (type == typeof(IoNodeBnrX20CP0482)) imageId = 1;
            else if (type == typeof(IoNodeMxpEC)) imageId = 1;
            else if (type == typeof(IoTermMpIO2310_30)) imageId = 2;
            else if (type == typeof(IoTermBnrX20DI9372)) imageId = 3;
            else if (type == typeof(IoTermBnrX20DO9321)) imageId = 4;
            else if (type == typeof(IoTermBnrX20AI2632)) imageId = 3;
            else if (type == typeof(IoTermBnrX20AI4632)) imageId = 3;
            else if (type == typeof(IoTermBnrX20AIA744)) imageId = 3;
            else if (type == typeof(IoTermBnrX20AIB744)) imageId = 3;
            else if (type == typeof(IoTermBnrX20AO2632)) imageId = 4;
            else if (type == typeof(IoTermBnrX20AO4632)) imageId = 4;
            else if (type == typeof(IoTermMpAN2900)) imageId = 5;
			else if (type == typeof(IoTermMpAN2910)) imageId = 6;
            else if (type == typeof(IoTermMxpD232)) imageId = 3;
            else if (type == typeof(IoTermMxpDT32K)) imageId = 2;
			else if (type == typeof(IoTermMxpECK64)) imageId = 2;
			else if (type == typeof(IoTermMxpTR32K)) imageId = 4;
            else if (type == typeof(IoChannel))
			{
				if (ioType == IoType.DI) imageId = 7;
				else if (ioType == IoType.DO) imageId = 8;
				else if (ioType == IoType.AI) imageId = 9;
				else if (ioType == IoType.AO) imageId = 10;
			}
            else if (type == typeof(IoTermBnrX20MM2436)) imageId = 11;
            else if (type == typeof(BnrAxisChannel)) imageId = 12;
            node.ImageIndex = imageId;
			node.SelectedImageIndex = imageId;
			node.StateImageIndex = imageId;
		}
		#endregion

		#region [TreeView Event]
		private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			m_TreeView = sender as TreeView;
			m_CurNode = e.Node;
			this.propertyGrid.SelectedObject = m_CurNode.Tag;

			MouseButtons button = e.Button;
			if (button == System.Windows.Forms.MouseButtons.Right)
			{
				if (m_CurNode.ContextMenuStrip == null)
				{
					m_CurNode.ContextMenuStrip = this.contextMenuStrip;
				}

				if (m_CurNode.Level == 0) // Root (IoFactory)
				{
					m_CurNode.ContextMenuStrip.Items["Add"].Enabled = true;
					//m_CurNode.ContextMenuStrip.Items["Adds"].Enabled = false;
					m_CurNode.ContextMenuStrip.Items["Remove"].Enabled = false;
				}
				else if (m_CurNode.Level == 1) // Node
				{
					m_CurNode.ContextMenuStrip.Items["Add"].Enabled = true;
					//m_CurNode.ContextMenuStrip.Items["Adds"].Enabled = false;
					m_CurNode.ContextMenuStrip.Items["Remove"].Enabled = true;
				}
				else if (m_CurNode.Level == 2) // Terminal
				{
					m_CurNode.ContextMenuStrip.Items["Add"].Enabled = false;
					//m_CurNode.ContextMenuStrip.Items["Adds"].Enabled = true;
					m_CurNode.ContextMenuStrip.Items["Remove"].Enabled = true;
				}
			}
			if (m_OldNode != null) this.m_OldNode.ForeColor = Color.Black;
			this.m_CurNode.ForeColor = Color.Red;
			this.m_OldNode = m_CurNode;
		}

		private void Add_Click(object sender, EventArgs e)
		{
			if (m_CurNode.Level == 0)
			{
				FormAddIoNode form = new FormAddIoNode();
				if (form.ShowDialog() == DialogResult.OK)
				{
					foreach (_IoNode ioNode in form.NewNodes)
					{
						m_IoFactory.Nodes.Add(ObjectCopier.Clone(ioNode));
					}
				}
			}
			else if (m_CurNode.Level == 1)
			{
				_IoNode ioNode = m_CurNode.Tag as _IoNode;
                //FormAddIoTerminal form = new FormAddIoTerminal();
                FormAddIoTerminal form = new FormAddIoTerminal(ioNode);
                if(form.ShowDialog() == DialogResult.OK)
				{
					foreach (_IoTerminal terminal in form.NewTerminals)
					{
						ioNode.Terminals.Add(ObjectCopier.Clone(terminal));
					}
				}
			}
			UpdateTreeView(m_IoFactory);
		}

		private void Remove_Click(object sender, EventArgs e)
		{
			if (m_CurNode.Level == 1)
			{
				m_IoFactory.Nodes.Remove(m_CurNode.Tag as _IoNode);
				UpdateTreeView(m_IoFactory);
			}
			else if (m_CurNode.Level == 2)
			{
				_IoNode node = m_CurNode.Parent.Tag as _IoNode;
				node.Terminals.Remove(m_CurNode.Tag as _IoTerminal);
				UpdateTreeView(m_IoFactory);
			}
		}
		#endregion

    }
}
