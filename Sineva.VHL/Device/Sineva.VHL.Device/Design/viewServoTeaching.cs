using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Device
{
    public partial class viewServoTeaching : UCon
    {
        private ServoManager m_Manager = null;
        private TreeNode m_CurNode = null;
        private TreeNode m_OldNode = null;
        private ServoUnit m_CurServoUnit = null;
        private _Axis m_CurAxis = null;
        private AuthorizationLevel m_CurUserLevel = AuthorizationLevel.Operator;


        public viewServoTeaching() : base(OperateMode.Manual)
        {
            InitializeComponent();
        }
        public bool Initialize()
        {
            bool rv = true;

            m_Manager = ServoManager.Instance;

            if (m_Manager.Initialized == false)
                m_Manager.Initialize();

            UpdateTreeView(m_Manager);

            return rv;
        }
        private void UpdateTreeView(ServoManager manager)
        {
            this.treeView.BeginUpdate();
            this.treeView.Nodes.Clear();

            TreeNode nodeRoot = new TreeNode(manager.ToString());
            nodeRoot.Tag = manager;
            this.treeView.Nodes.Add(nodeRoot);

            if (manager.ServoUnits == null) return;
            foreach (ServoUnit unit in manager.ServoUnits)
            {
                if (unit == null) return;
                TreeNode nodeUnit = new TreeNode(unit.ToString());
                nodeUnit.Tag = unit;
                nodeRoot.Nodes.Add(nodeUnit);
                foreach (_Axis axis in unit.Axes)
                {
                    TreeNode nodeAxis = new TreeNode(axis.ToString());
                    nodeAxis.Tag = axis;
                    nodeUnit.Nodes.Add(nodeAxis);
                }
                //nodeUnit.Expand();
            }
            nodeRoot.Expand();
            this.treeView.EndUpdate();
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ButtonLog.WriteLog(this.Name.ToString(), "treeView_NodeMouseClick");
            TreeView view = sender as TreeView;
            m_CurNode = e.Node;

            //MessageBox.Show(this.Size.ToString());

            UpdateStepMoveOpCtrls();

            this.lblUnitName.Text = "";
            this.lblAxisName.Text = "";


            if (m_CurNode.Level == 0)
            {
                m_CurServoUnit = null;
                m_CurAxis = null;
                return;
            }
            else if (m_CurNode.Level == 1)
            {
                m_CurServoUnit = m_CurNode.Tag as ServoUnit;
                m_CurAxis = null;
                if (m_OldNode != null) m_OldNode.BackColor = Color.Transparent;
                this.lblUnitName.Text = m_CurServoUnit.ServoName;
                this.lblAxisName.Text = "All axes";
            }
            else if (m_CurNode.Level == 2)
            {
                m_CurServoUnit = m_CurNode.Parent.Tag as ServoUnit;
                m_CurAxis = m_CurNode.Tag as _Axis;

                this.lblUnitName.Text = m_CurServoUnit.ServoName;
                this.lblAxisName.Text = m_CurAxis.AxisName;
            }


            // Tree node color setting
            m_CurNode.BackColor = SystemColors.Highlight;
            if (m_OldNode != null) m_OldNode.BackColor = Color.Transparent;
            m_OldNode = m_CurNode;

            // Teaching table Show
            this.dgvTeachingTable.DataSource = m_CurServoUnit.TeachingTable;
            LoadTeachingData(m_CurServoUnit);

            // Axes Status Show
            this.dgvAxesStatus.DataSource = m_CurServoUnit.Axes;
            LoadAxesStatus(m_CurServoUnit.Axes);
            UpdateAxesStatus(m_CurServoUnit.Axes);

            lbUnitSelect.Text = "Unit Select  :  " + m_CurServoUnit.ServoName;

        }
        private void UpdateStepMoveOpCtrls()
        {
        }
        private void LoadTeachingData(ServoUnit unit)
        {
            try
            {
                //this.dgvTeachingTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                //this.dgvTeachingTable.RowHeadersVisible = false;
                //this.dgvTeachingTable.AutoGenerateColumns = false;
                this.dgvTeachingTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                this.dgvTeachingTable.Columns.Clear();

                int dataStartIndex = 0;

                DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
                colId.DataPropertyName = "PosId";
                colId.HeaderText = "ID";
                //colId.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                colId.ReadOnly = true;
                colId.SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvTeachingTable.Columns.Add(colId);
                dataStartIndex++;

                DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
                colName.DataPropertyName = "PosName";
                colName.HeaderText = "Name";
                colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                colName.ReadOnly = true;
                colName.SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvTeachingTable.Columns.Add(colName);
                dataStartIndex++;

                // Create & Add Column Axes
                // for (int i = 0; i < unit.TeachingTable[0].AxesPosValueArray.Count; i++)
                for (int i = 0; i < unit.Axes.Count; i++)
                {
                    DataGridViewTextBoxColumn colVal = new DataGridViewTextBoxColumn();
                    colVal.HeaderText = unit.Axes[i].AxisName;
                    colVal.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    colVal.SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvTeachingTable.Columns.Add(colVal);
                }

                // Teaching point data set
                for (int i = 0; i < unit.TeachingPointCount; i++)
                {
                    for (int j = 0; j < unit.TeachingTable[0].AxesPosValueArray.Count; j++)
                    {
                        this.dgvTeachingTable.Rows[i].Cells[dataStartIndex + j].Value = unit.TeachingTable[i].AxesPosValueArray[j];
                    }
                }

                int[] widthArray = ReadTeachingTableColumnsWidth(unit);
                if (widthArray != null && widthArray.Length == this.dgvTeachingTable.Columns.Count)
                {
                    for (int i = 0; i < widthArray.Length; i++)
                    {
                        int curColWidth = 30;
                        if (i != 0)
                        {
                            if (widthArray[i] < 40)
                                curColWidth = 40;
                            else if (widthArray[i] > this.dgvTeachingTable.Width - 100)
                                curColWidth = (int)(this.dgvTeachingTable.Width / widthArray.Length);
                            else
                                curColWidth = widthArray[i];
                        }
                        this.dgvTeachingTable.Columns[i].Width = curColWidth;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }

        }

        private void LoadAxesStatus(List<_Axis> axes)
        {
            this.dgvAxesStatus.AutoGenerateColumns = false;
            this.dgvAxesStatus.RowHeadersVisible = false;
            //this.dgvAxesStatus.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
            this.dgvAxesStatus.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            this.dgvAxesStatus.Columns.Clear();

            DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
            colId.DataPropertyName = "AxisId";
            colId.HeaderText = "ID";
            colId.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvAxesStatus.Columns.Add(colId);

            DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
            colName.DataPropertyName = "AxisName";
            colName.HeaderText = "Name";
            colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvAxesStatus.Columns.Add(colName);

            DataGridViewTextBoxColumn colPos = new DataGridViewTextBoxColumn();
            //colPos.DataPropertyName = "CurPos";
            colPos.HeaderText = "Pos";
            colPos.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvAxesStatus.Columns.Add(colPos);

            DataGridViewTextBoxColumn colTorque = new DataGridViewTextBoxColumn();
            //colTorque.DataPropertyName = "CurTorque";
            colTorque.HeaderText = "Torque";
            colTorque.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvAxesStatus.Columns.Add(colTorque);

            DataGridViewTextBoxColumn colSpeed = new DataGridViewTextBoxColumn();
            //colSpeed.DataPropertyName = "CurSpeed";
            colSpeed.HeaderText = "Speed";
            colSpeed.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvAxesStatus.Columns.Add(colSpeed);

            DataGridViewImageColumn colAlarm = new DataGridViewImageColumn();
            colAlarm.HeaderText = "Alarm";
            colAlarm.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvAxesStatus.Columns.Add(colAlarm);

            DataGridViewImageColumn colInPos = new DataGridViewImageColumn();
            colInPos.HeaderText = "In\r\nPos";
            colInPos.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvAxesStatus.Columns.Add(colInPos);

            DataGridViewImageColumn colHomeEnd = new DataGridViewImageColumn();
            colHomeEnd.HeaderText = "Home\r\nEnd";
            colHomeEnd.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvAxesStatus.Columns.Add(colHomeEnd);

            DataGridViewImageColumn colSvOn = new DataGridViewImageColumn();
            colSvOn.HeaderText = "Servo\r\nOn";
            colSvOn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvAxesStatus.Columns.Add(colSvOn);

            DataGridViewImageColumn colOrg = new DataGridViewImageColumn();
            colOrg.HeaderText = "Origin";
            colOrg.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvAxesStatus.Columns.Add(colOrg);

            DataGridViewImageColumn colNLimit = new DataGridViewImageColumn();
            colNLimit.HeaderText = "- Limit";
            colNLimit.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvAxesStatus.Columns.Add(colNLimit);

            DataGridViewImageColumn colPLimit = new DataGridViewImageColumn();
            colPLimit.HeaderText = "+ Limit";
            colPLimit.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvAxesStatus.Columns.Add(colPLimit);

            DataGridViewImageColumn colCmdConfirm = new DataGridViewImageColumn();
            colCmdConfirm.HeaderText = "Cmd\r\nConfirm";
            colNLimit.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvAxesStatus.Columns.Add(colCmdConfirm);
        }
        private int[] ReadTeachingTableColumnsWidth(ServoUnit unit)
        {
            System.IO.StreamReader sr = null;
            string path = AppConfig.Instance.XmlServoParameterPath + System.IO.Path.DirectorySeparatorChar + "UI";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
                return null;
            }

            int[] columnsWidth = new int[0];

            string fullPath = path + System.IO.Path.DirectorySeparatorChar + unit.ServoName + ".xml";
            if (!System.IO.File.Exists(fullPath)) return null;
            try
            {
                sr = new System.IO.StreamReader(fullPath);
                System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(columnsWidth.GetType());
                columnsWidth = xmlSerializer.Deserialize(sr) as int[];
                sr.Close();
                return columnsWidth;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
                sr.Close();
                return null;
            }
        }
        private void UpdateAxesStatus(List<_Axis> axes)
        {
            bool HomeEnd = true;
            bool InPos = true;
            bool Alarm = false;
            for (int i = 0; i < axes.Count; i++)
            {
                ushort decimalPoint = axes[i].DecimalPoint;// (axes[i] as MpAxis).DecimalPoint;
                double _curpos = axes[i].CurPos; // (axes[i] as IAxisCommand).GetAxisCurPos();
                string posText = decimalPoint <= 4 ? string.Format("{0:F4}", _curpos) : decimalPoint <= 5 ? string.Format("{0:F5}", _curpos) : string.Format("{0:F6}", _curpos);
                string torqueText = axes[i].CurTorque.ToString("F2");// (axes[i] as IAxisCommand).GetAxisCurTorque().ToString("F2");
                string speedText = axes[i].CurSpeed.ToString("F4"); // (axes[i] as IAxisCommand).GetAxisCurSpeed().ToString("F4");
                this.dgvAxesStatus[2, i].Value = posText;
                this.dgvAxesStatus[3, i].Value = torqueText;
                this.dgvAxesStatus[4, i].Value = speedText;

                int j = 5;
                enAxisInFlag axisStatus = (axes[i] as IAxisCommand).GetAxisCurStatus();
                if ((axisStatus & enAxisInFlag.Alarm) == enAxisInFlag.Alarm)
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On_Red;
                    if (!axes[i].CommandSkip) Alarm = true;
                }
                else
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                }

                if ((axisStatus & enAxisInFlag.InPos) == enAxisInFlag.InPos)
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On;
                }
                else
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                    if (!axes[i].CommandSkip) InPos = false;
                }

                if ((axisStatus & enAxisInFlag.HEnd) == enAxisInFlag.HEnd)
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On;
                }
                else
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                    if (!axes[i].CommandSkip) HomeEnd = false;
                }

                if ((axisStatus & enAxisInFlag.SvOn) == enAxisInFlag.SvOn)
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On;
                }
                else
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                }

                if ((axisStatus & enAxisInFlag.Org) == enAxisInFlag.Org)
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On;
                }
                else
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                }

                if ((axisStatus & enAxisInFlag.Limit_N) == enAxisInFlag.Limit_N)
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On_Red;
                }
                else
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                }

                if ((axisStatus & enAxisInFlag.Limit_P) == enAxisInFlag.Limit_P)
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On_Red;
                }
                else
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                }

                if ((axisStatus & enAxisInFlag.Cmd_Confirm) == enAxisInFlag.Cmd_Confirm)
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On;
                }
                else
                {
                    this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                }
            }

            if (m_CurAxis != null)
            {
                if (((m_CurAxis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm) if (m_CurAxis.CommandSkip) Alarm = true;
                else Alarm = false;
            }

            if (InPos && HomeEnd && !Alarm)
            {
                // Manual 일때만 사용 가능하도록 하자....
                bool manual = EqpStateManager.Instance.OpMode == OperateMode.Manual ? true : false;
                btnStepMove.Enabled = m_CurUserLevel <= AuthorizationLevel.Maintenance && manual;
            }
            else
            {
                btnStepMove.Enabled = false;
            }
        }
        public void SetAuthority(AuthorizationLevel level)
        {

            bool manual_key = EqpStateManager.Instance.OpMode == OperateMode.Auto ? false : true;
            m_CurUserLevel = level;

            btnStepMove.Enabled = level < AuthorizationLevel.Maintenance && manual_key;

            dgvTeachingTable.ReadOnly = level >= AuthorizationLevel.Developer && manual_key;
        }
    }
}
