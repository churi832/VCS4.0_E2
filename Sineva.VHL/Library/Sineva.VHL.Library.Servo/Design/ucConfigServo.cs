using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Library.Servo
{
    public partial class ucConfigServo : UserControl
    {
        private TreeView m_TreeView = null;
        private ServoManager m_ServoFactory = null;
        private TreeNode m_CurNodeServo = null;
        private TreeNode m_OldNodeServo = null;

        private TreeNode m_CurNodeMotion = null;
        private TreeNode m_OldNodeMotion = null;

        public ucConfigServo()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
        }

        public bool Initialize(ServoManager factory)
        {
            m_ServoFactory = factory;
            if (m_ServoFactory.Initialized == false)
                m_ServoFactory.Initialize();

            UpdateView();
            return true;
        }

        public void UpdateView()
        {
            UpdateTreeViewServo(m_ServoFactory);
            UpdateTreeViewMotion(m_ServoFactory);
            List<MpAxis> mp_axisSource = new List<MpAxis>();
            foreach (_Axis axis in m_ServoFactory.AxisSource) mp_axisSource.Add(axis as MpAxis);
            this.dgvAxes.DataSource = mp_axisSource;
            //this.dgvAxes.DataSource = m_ServoFactory.AxisSource;
        }

        public void Save()
        {
            bool check_ok = true;
            int block_axes_count = 0;
            foreach (_AxisBlock block in m_ServoFactory.AxisBlocks)
            {
                foreach (_Axis axis in block.Axes)
                {
                    check_ok &= m_ServoFactory.CheckAxisNameDuplicated(axis);
                    if (!check_ok) break;
                }
                block_axes_count += block.Axes.Count;
            }

            if (m_ServoFactory.AxisSource.Count != block_axes_count)
            {
                // 만일 Axis 개수가 바뀌었다면 AxisSource를 변경해야 함.
                m_ServoFactory.AxisSource.Clear();
                foreach (_AxisBlock block in m_ServoFactory.AxisBlocks)
                {
                    foreach (_Axis axis in block.Axes)
                    {
                        if (m_ServoFactory.CheckAxisNameDuplicated(axis))
                        {
                            if (axis.CheckedItem != null) axis.CheckedItem.Init(axis);
                            m_ServoFactory.AxisSource.Add(axis);
                        }
                    }
                }
            }

            if (check_ok)
            {
                m_ServoFactory.WriteXml();
                UpdateView();
            }
        }

        public void UpdateTreeViewServo(ServoManager factory)
        {
            this.treeViewServo.BeginUpdate();
            this.treeViewServo.Nodes.Clear();

            TreeNode node0 = new TreeNode(factory.ToString());
            node0.Tag = factory;
            this.treeViewServo.Nodes.Add(node0);

            if (factory.ServoUnits == null) return;
            foreach (ServoUnit unit in factory.ServoUnits)
            {
                if (unit == null) return;
                TreeNode nodeUnit = new TreeNode(unit.ToString());
                nodeUnit.Tag = unit;
                node0.Nodes.Add(nodeUnit);
                foreach (_Axis axis in unit.Axes)
                {
                    if (axis != null)
                    {
                        TreeNode nodeAxis = new TreeNode(axis.ToString());
                        nodeAxis.Tag = axis;
                        nodeUnit.Nodes.Add(nodeAxis);
                    }
                }
                nodeUnit.Expand();
            }
            node0.Expand();
            this.treeViewServo.EndUpdate();
        }

        public void UpdateTreeViewMotion(ServoManager factory)
        {
            this.treeViewMotion.BeginUpdate();
            this.treeViewMotion.Nodes.Clear();

            TreeNode node0 = new TreeNode();
            node0.Tag = factory;
            node0.Text = factory.ToString();
            this.treeViewMotion.Nodes.Add(node0);

            foreach (_AxisBlock block in factory.AxisBlocks)
            {
                TreeNode nodeBlockNode = new TreeNode(block.ToString());
                nodeBlockNode.Tag = block;
                node0.Nodes.Add(nodeBlockNode);

                foreach (_Axis axis in block.Axes)
                {
                    TreeNode nodeAxis = new TreeNode(axis.ToString());
                    nodeAxis.Tag = axis;
                    nodeBlockNode.Nodes.Add(nodeAxis);
                }
                nodeBlockNode.Expand();
            }
            node0.Expand();
            this.treeViewMotion.EndUpdate();
        }

        private void treeViewServo_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            m_TreeView = sender as TreeView;
            m_CurNodeServo = e.Node;
            this.propertyGridServo.SelectedObject = m_CurNodeServo.Tag;

            MouseButtons button = e.Button;
            if (button == System.Windows.Forms.MouseButtons.Right)
            {
                if (m_CurNodeServo.ContextMenuStrip == null)
                {
                    m_CurNodeServo.ContextMenuStrip = this.contextMenuStrip;
                }

                if (m_CurNodeServo.Level == 0) // Root (IoFactory)
                {
                    m_CurNodeServo.ContextMenuStrip.Items["Add"].Enabled = true;
                    //m_CurNode.ContextMenuStrip.Items["Adds"].Enabled = false;
                    m_CurNodeServo.ContextMenuStrip.Items["Remove"].Enabled = false;
                }
                else if (m_CurNodeServo.Level == 1) // Node
                {
                    m_CurNodeServo.ContextMenuStrip.Items["Add"].Enabled = true;
                    //m_CurNode.ContextMenuStrip.Items["Adds"].Enabled = false;
                    m_CurNodeServo.ContextMenuStrip.Items["Remove"].Enabled = true;
                }
                else if (m_CurNodeServo.Level == 2) // Terminal
                {
                    m_CurNodeServo.ContextMenuStrip.Items["Add"].Enabled = false;
                    //m_CurNode.ContextMenuStrip.Items["Adds"].Enabled = true;
                    m_CurNodeServo.ContextMenuStrip.Items["Remove"].Enabled = true;
                }
            }
            if (m_OldNodeServo != null) this.m_OldNodeServo.ForeColor = Color.Black;
            this.m_CurNodeServo.ForeColor = Color.Red;
            this.m_OldNodeServo = m_CurNodeServo;
        }

        private void treeViewMotion_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            m_TreeView = sender as TreeView;
            m_CurNodeMotion = e.Node;
            this.propertyGridMotion.SelectedObject = m_CurNodeMotion.Tag;


            MouseButtons button = e.Button;
            if (button == System.Windows.Forms.MouseButtons.Right)
            {
                if (m_CurNodeMotion.ContextMenuStrip == null)
                {
                    m_CurNodeMotion.ContextMenuStrip = this.contextMenuStrip;
                }

                if (m_CurNodeMotion.Level == 0) // Root (IoFactory)
                {
                    m_CurNodeMotion.ContextMenuStrip.Items["Add"].Enabled = true;
                    //m_CurNode.ContextMenuStrip.Items["Adds"].Enabled = false;
                    m_CurNodeMotion.ContextMenuStrip.Items["Remove"].Enabled = false;
                }
                else if (m_CurNodeMotion.Level == 1) // Node
                {
                    m_CurNodeMotion.ContextMenuStrip.Items["Add"].Enabled = true;
                    //m_CurNode.ContextMenuStrip.Items["Adds"].Enabled = false;
                    m_CurNodeMotion.ContextMenuStrip.Items["Remove"].Enabled = true;
                }
                else if (m_CurNodeMotion.Level == 2) // Terminal
                {
                    m_CurNodeMotion.ContextMenuStrip.Items["Add"].Enabled = false;
                    //m_CurNode.ContextMenuStrip.Items["Adds"].Enabled = true;
                    m_CurNodeMotion.ContextMenuStrip.Items["Remove"].Enabled = true;
                }
            }
            if (m_OldNodeMotion != null) this.m_OldNodeMotion.ForeColor = Color.Black;
            this.m_CurNodeMotion.ForeColor = Color.Red;
            this.m_OldNodeMotion = m_CurNodeMotion;
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (m_TreeView == null) return;
            if (m_TreeView == this.treeViewServo)
            {
                if (m_CurNodeServo.Level == 0)
                {
                    m_ServoFactory.ServoUnits.Add(new ServoUnit());
                    m_ServoFactory.RedefineServoId();
                }
                else if (m_CurNodeServo.Level == 1)
                {
                    ServoUnit unit = m_CurNodeServo.Tag as ServoUnit;
                    FormAddAxis_ServoUnit form = new FormAddAxis_ServoUnit();
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        foreach (_Axis ax in form.NewAxes)
                        {
                            unit.Axes.Add(ax);
                        }
                    }
                }
                UpdateTreeViewServo(m_ServoFactory);
            }
            else
            {
                if (m_CurNodeMotion.Level == 0)
                {
                    FormAddAxisBlock form = new FormAddAxisBlock();
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        foreach (_AxisBlock block in form.NewBlocks)
                        {
                            m_ServoFactory.AxisBlocks.Add(block);
                        }
                    }
                }
                else if (m_CurNodeMotion.Level == 1)
                {
                    _AxisBlock block = m_CurNodeMotion.Tag as _AxisBlock;
                    FormAddAxis form = new FormAddAxis();
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        foreach (_Axis axis in form.NewAxes)
                        {
                            axis.AxisName = "MpSpare";
                            block.Axes.Add(axis);
                        }
                        m_ServoFactory.RedefineAxisId();
                    }
                }
                UpdateTreeViewMotion(m_ServoFactory);
            }
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            if (m_TreeView == null) return;
            if (m_TreeView == this.treeViewServo)
            {
                if (m_CurNodeServo.Level == 1)
                {
                    m_ServoFactory.ServoUnits.Remove(m_CurNodeServo.Tag as ServoUnit);
                }
                else if (m_CurNodeServo.Level == 2)
                {
                    ServoUnit unit = m_CurNodeServo.Parent.Tag as ServoUnit;
                    unit.Axes.Remove(m_CurNodeServo.Tag as _Axis);
                }
                UpdateTreeViewServo(m_ServoFactory);
            }
            else
            {
                if (m_CurNodeMotion.Level == 1)
                {
                    m_ServoFactory.AxisBlocks.Remove(m_CurNodeMotion.Tag as _AxisBlock);
                }
                else if (m_CurNodeMotion.Level == 2)
                {
                    _AxisBlock block = m_CurNodeMotion.Parent.Tag as _AxisBlock;
                    block.Axes.Remove(m_CurNodeMotion.Tag as _Axis);
                }
                UpdateTreeViewMotion(m_ServoFactory);
            }
        }

        private void dgvAxes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                try
                {
                    string s = Clipboard.GetText();
                    string[] lines = s.Split('\n');
                    int iFail = 0, iRow = dgvAxes.CurrentCell.RowIndex;
                    int iCol = dgvAxes.CurrentCell.ColumnIndex;
                    DataGridViewCell oCell;
                    //if (dgvAxes.Rows.Count < lines.Length)
                    //    dgvAxes.Rows.Add(lines.Length - 1);
                    foreach (string line in lines)
                    {
                        if (iRow < dgvAxes.RowCount && line.Length > 0)
                        {
                            string[] sCells = line.Split('\t');
                            if (sCells.Length != this.dgvAxes.ColumnCount) continue;

                            for (int i = 0; i < sCells.GetLength(0); ++i)
                            {
                                if (iCol + i < this.dgvAxes.ColumnCount)
                                {
                                    oCell = dgvAxes[iCol + i, iRow];
                                    if (!oCell.ReadOnly)
                                    {
                                        if (oCell.ValueType == typeof(enHomeMethod))
                                        {
                                            oCell.Value = EnumUtil<enHomeMethod>.Parse(sCells[i]);
                                        }
                                        else if (oCell.ValueType == typeof(AxisCheckItem))
                                        {
                                            oCell.Value = EnumUtil<AxisCheckItem>.Parse(sCells[i]);
                                        }
                                        else if (oCell.ValueType == typeof(enAxisInFlag))
                                        {
                                            oCell.Value = EnumUtil<enAxisInFlag>.Parse(sCells[i]);
                                        }
                                        else if (oCell.ValueType == typeof(enMotorType))
                                        {
                                            oCell.Value = EnumUtil<enMotorType>.Parse(sCells[i]);
                                        }
                                        else if (oCell.ValueType == typeof(enAxisResult))
                                        {
                                            oCell.Value = EnumUtil<enAxisResult>.Parse(sCells[i]);
                                        }
                                        else if (oCell.ValueType == typeof(enAxisCoord))
                                        {
                                            oCell.Value = EnumUtil<enAxisCoord>.Parse(sCells[i]);
                                        }
                                        else if (oCell.ValueType == typeof(Scale) ||
                                            oCell.ValueType == typeof(ServoCmdProxy))
                                        {

                                        }
                                        else
                                        {
                                            oCell.Value = Convert.ChangeType(sCells[i],
                                                                  oCell.ValueType);
                                            //  oCell.Style.BackColor = Color.Tomato;
                                        }
                                        //if (oCell.Value != null || oCell.Value.ToString() != sCells[i])
                                        //{
                                        //}
                                        //else
                                        //    iFail++;
                                        //only traps a fail if the data has changed 
                                        //and you are pasting into a read only cell
                                    }
                                }
                                else
                                { break; }
                            }
                            iRow++;
                        }
                        else
                        { break; }
                        if (iFail > 0)
                            MessageBox.Show(string.Format("{0} updates failed due" +
                                            " to read only column setting", iFail));
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("The data you pasted is in the wrong format for the cell");
                    return;
                }
            }
        }
    }
}
