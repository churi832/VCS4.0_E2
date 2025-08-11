using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using Sineva.VHL.Data;
using Sineva.VHL.Data.LogIn;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Library.MXP;

namespace Sineva.VHL.Device
{
	public partial class viewServoControl : UCon
	{
		struct JogInfo
		{
			public enAxisCoord Coord;
			public enAxisDir Dir;
		}

		#region Fields
		private ServoManager m_Manager = null;
		private ServoUnit m_CurServoUnit = null;
		private _Axis m_CurAxis = null;
		private TreeNode m_CurNode = null;
		private TreeNode m_OldNode = null;
		List<_Axis> m_TargetAxes = new List<_Axis>();

        private AuthorizationLevel m_CurUserLevel = AuthorizationLevel.Operator;

		List<CheckBox> cbMPOKList = new List<CheckBox>();

        private List<MotionSensor> m_MotionSensorList = new List<MotionSensor>();
        private bool m_JogButtonDown = false;
        #endregion

        #region Constructor
        public viewServoControl() : base(OperateMode.Manual)
        {
			InitializeComponent();
			cbMPOKList.Add(cbMP1OK);

			JogInfo jogInfo = new JogInfo();

			jogInfo.Coord = enAxisCoord.X;
			jogInfo.Dir = enAxisDir.Positive;
			this.btnJogX_plus.Tag = jogInfo;

			jogInfo.Coord = enAxisCoord.X;
			jogInfo.Dir = enAxisDir.Negative;
			this.btnJogX_minus.Tag = jogInfo;

			jogInfo.Coord = enAxisCoord.Y;
			jogInfo.Dir = enAxisDir.Positive;
			this.btnJogY_plus.Tag = jogInfo;

			jogInfo.Coord = enAxisCoord.Y;
			jogInfo.Dir = enAxisDir.Negative;
			this.btnJogY_minus.Tag = jogInfo;

			jogInfo.Coord = enAxisCoord.Z;
			jogInfo.Dir = enAxisDir.Positive;
			this.btnJogZ_plus.Tag = jogInfo;

			jogInfo.Coord = enAxisCoord.Z;
			jogInfo.Dir = enAxisDir.Negative;
			this.btnJogZ_minus.Tag = jogInfo;

			jogInfo.Coord = enAxisCoord.T;
			jogInfo.Dir = enAxisDir.Positive;
			this.btnJogT_plus.Tag = jogInfo;

			jogInfo.Coord = enAxisCoord.T;
			jogInfo.Dir = enAxisDir.Negative;
			this.btnJogT_minus.Tag = jogInfo;

			cbInterlockCheck.Checked = true;

			InterlockManager.InterlockMessage += InterlockManager_InterlockMessage;
		}
        void InterlockManager_InterlockMessage(string val)
		{
			if (this.lblMessage.InvokeRequired)
			{
				DelVoid_String d = new DelVoid_String(InterlockManager_InterlockMessage);
				this.Invoke(d, val);
			}
			else
			{
				this.lblMessage.Text = val;
			}
		}
		#endregion

		#region Methods
		public bool Initialize()
		{
			bool rv = true;

			m_Manager = ServoManager.Instance;

			if (m_Manager.Initialized == false)
				m_Manager.Initialize();

			UpdateTreeView(m_Manager);
            treeView.SelectedNode = treeView.Nodes[0];
            UpdateCurNode(treeView.SelectedNode.FirstNode);

            return rv;
		}
        public override void CancelOperation()
        {
            if (m_CurServoUnit != null)
                m_CurServoUnit.Repeat = false;
        }
        public void UpdateState()
		{
            if(this.Visible == false) return;
            if(m_Manager == null) return;
            
			foreach(_AxisBlock block in m_Manager.AxisBlocks)
			{
				if (block.BlockId < cbMPOKList.Count)
				{
					if (block.ControlFamily == ServoControlFamily.MXP)
					{
						cbMPOKList[block.BlockId].Checked = (block as AxisBlockMXP).Connected;
					}
				}
			}
            bool system_run = MxpManager.Instance.ControlState == MXP.KERNEL_STATUS.SYSTEM_RUN;
            system_run &= MxpManager.Instance.EtherCatReady == 1;
            btnMpStart.Enabled = !system_run;
            bool system_alarm = MxpManager.Instance.ControlState != MXP.KERNEL_STATUS.SYSTEM_RUN;
            btnMpStop.Enabled = system_alarm;

			if (m_CurServoUnit == null) return;

			enAxisResult rv = enAxisResult.None;
            //int AlmId = 0;
			if (m_CurAxis == null) rv = m_CurServoUnit.MotionDone();
			else rv = m_CurServoUnit.MotionDoneAxis(m_CurAxis);
			if (rv != enAxisResult.None)
			{
				if (m_CurAxis == null) lbMsg.Text = string.Format("{0}-Servo {1} !", m_CurServoUnit.GetName(), rv.ToString());
				else lbMsg.Text = string.Format("{0}-Servo {1}-Axis {2} !", m_CurServoUnit.GetName(), m_CurAxis.AxisName, rv.ToString());
			}

			UpdateAxesStatus(m_CurServoUnit.Axes);

            if (m_CurAxis != null && m_CurAxis.SequenceState.IsSequenceBusy)
            {
                int cur = m_CurAxis.SequenceState.SequenceCurrentStep;
                int total = m_CurAxis.SequenceState.SequenceCurrentStep + m_CurAxis.SequenceState.SequenceRemainCount;
                lbSequenceInfo.Text = string.Format("{0} / {1}", cur, total);
                this.progressBar1.Value = cur;
            }
        }
        #endregion

        #region Methods Treeview
        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
            ButtonLog.WriteLog(this.Name.ToString(), "treeView_NodeMouseClick");
            TreeView view = sender as TreeView;
            UpdateCurNode(e.Node);
		}
        private void UpdateCurNode(TreeNode curNode)
        {
            try
            {
                m_CurNode = curNode;

                if (m_CurNode.Level == 2)
                {
                    if ((m_CurNode.Tag as _Axis).CommandSkip)
                    {
                        m_CurServoUnit = null;
                        m_CurAxis = null;
                        if (m_OldNode != null) m_OldNode.BackColor = Color.Transparent;
                        m_OldNode = m_CurNode;
                        return;
                    }
                }
                //MessageBox.Show(this.Size.ToString());

                UpdateJogOpCtrls();

                this.lblUnitName.Text = "";
                this.lblAxisName.Text = "";

                if (m_CurNode.Level == 0)
                {
                    m_CurServoUnit = null;
                    m_CurAxis = null;
                    if (m_OldNode != null) m_OldNode.BackColor = Color.Transparent;
                    m_OldNode = m_CurNode;
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

                bool jog_reverse = IsJogDirectionReverse(m_CurAxis);
                // Gantry X2 direction
                if (jog_reverse)
                {
                    this.btnJogX_minus.Text = "X+";
                    this.btnJogX_plus.Text = "X-";
                    this.btnJogY_minus.Text = "Y+";
                    this.btnJogY_plus.Text = "Y-";
                }
                else
                {
                    this.btnJogX_minus.Text = "X-";
                    this.btnJogX_plus.Text = "X+";
                    this.btnJogY_minus.Text = "Y-";
                    this.btnJogY_plus.Text = "Y+";
                }

                if (m_CurServoUnit != null)
                {
                    // Teaching table Show
                    LoadTeachingData(m_CurServoUnit);
                    LoadMovingPropData(m_CurServoUnit);

                    // Axes Status Show
                    this.dgvAxesStatus.DataSource = m_CurServoUnit.Axes;
                    LoadAxesStatus(m_CurServoUnit.Axes);
                    UpdateAxesStatus(m_CurServoUnit.Axes);

                    lbUnitSelect.Text = "Unit Select  :  " + m_CurServoUnit.ServoName;
                    // Target Pos display
                    LoadTargetPos();

                    if (m_CurAxis != null)
                    {
                        m_MotionSensorList.Clear();
                        int left_slave_no = m_CurAxis.LeftBcrNodeId;
                        int right_slave_no = m_CurAxis.RightBcrNodeId;
                        if (m_CurAxis.MotionSensorPara.SlaveNo != left_slave_no && m_CurAxis.MotionSensorPara.SlaveNo != left_slave_no)
                        {
                            m_CurAxis.MotionSensorPara.SlaveNo = (uint)left_slave_no;
                        }
                        m_MotionSensorList.Add(m_CurAxis.MotionSensorPara);
                        this.dgvMotionSensorTable.DataSource = m_MotionSensorList;
                        LoadMotionSensor();
                        LoadMotionProfile();
                    }

                    if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Vel)
                    {
                        this.rbJogTypeVMove.Checked = true;
                        this.rbJogTypeRelative.Checked = false;

                    }
                    else if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative)
                    {
                        this.rbJogTypeRelative.Checked = true;
                        this.rbJogTypeVMove.Checked = false;
                    }

                    if (IsOriginInterlock(m_CurServoUnit)) btnOrigin.Enabled = true;
                    else btnOrigin.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }

        private void UpdateTreeView(ServoManager manager)
		{
            try
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
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }
		#endregion

		#region Methods DataGridViews
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
                this.dgvTeachingTable.RowCount = unit.TeachingPointCount;
                for (int i = 0; i < unit.TeachingPointCount; i++)
                {
                    this.dgvTeachingTable.Rows[i].Cells[0].Value = unit.TeachingTable[i].PosId;
                    this.dgvTeachingTable.Rows[i].Cells[1].Value = unit.TeachingTable[i].PosName;
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
            catch(Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }

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
            if(!System.IO.File.Exists(fullPath)) return null;
            try
            {
                sr = new System.IO.StreamReader(fullPath);
                System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(columnsWidth.GetType());
                columnsWidth = xmlSerializer.Deserialize(sr) as int[];
                sr.Close();
                return columnsWidth;
            }
            catch(Exception ex)
            {
				ExceptionLog.WriteLog(ex.ToString());
                sr.Close();
                return null;
            }
        }
        private void WriteTeachingTableColumnsWidth(ServoUnit unit)
        {
            System.IO.StreamWriter sw = null;
            string path = AppConfig.Instance.XmlServoParameterPath + System.IO.Path.DirectorySeparatorChar + "UI";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string fullPath = path + System.IO.Path.DirectorySeparatorChar + unit.ServoName + ".xml";
            List<int> columnsWidth = new List<int>();
            for (int i = 0; i < dgvTeachingTable.Columns.Count; i++)
            {
                columnsWidth.Add(dgvTeachingTable.Columns[i].Width);
            }

            try
            {
                System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(columnsWidth.GetType());
                sw = new System.IO.StreamWriter(fullPath);
                xmlSerializer.Serialize(sw, columnsWidth);
                sw.Close();
            }
            catch(Exception ex)
            {
				ExceptionLog.WriteLog(ex.ToString());
                sw.Close();
            }
        }

		private void LoadMovingPropData(ServoUnit unit)
		{
            try
            {
                this.dgvVelTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                //this.dgvVelTable.RowHeadersVisible = false;
                this.dgvVelTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                this.dgvVelTable.Columns.Clear();

                int dataStartIndex = 0;

                DataGridViewTextBoxColumn colAxis = new DataGridViewTextBoxColumn();
                colAxis.DataPropertyName = "AxisName";
                colAxis.HeaderText = "Axis";
                colAxis.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                colAxis.ReadOnly = true;
                colAxis.SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvVelTable.Columns.Add(colAxis);
                dataStartIndex++;

                DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
                colId.DataPropertyName = "PropId";
                colId.HeaderText = "ID";
                colId.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                colId.ReadOnly = true;
                colId.SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvVelTable.Columns.Add(colId);
                dataStartIndex++;

                DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
                colName.DataPropertyName = "PropName";
                colName.HeaderText = "Name";
                colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                colName.ReadOnly = true;
                colName.SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvVelTable.Columns.Add(colName);
                dataStartIndex++;

                // Vel, Acc, Dec, Jerk 순으로
                string[] titles = { "Vel", "Acc", "Dec", "Jerk" };
                for (int i = 0; i < titles.Length; i++)
                {
                    DataGridViewTextBoxColumn colVal = new DataGridViewTextBoxColumn();
                    colVal.HeaderText = titles[i];
                    this.dgvVelTable.Columns.Add(colVal);
                }
                if (unit.MovingPropTable.Count == 0) return;

                int maxTable = unit.MovingPropTable.Count;
                int maxList = maxTable > 0 ? unit.MovingPropTable[0].VelSetList.Count : 0;
                this.dgvVelTable.RowCount = maxTable * maxList;
                for (int i = 0; i < maxTable; i++)
                {
                    for (int j = 0; j < maxList; j++)
                    {
                        int index = 0;
                        this.dgvVelTable.Rows[j * maxTable + i].Cells[index++].Value = unit.Axes[j].AxisName;
                        this.dgvVelTable.Rows[j * maxTable + i].Cells[index++].Value = unit.MovingPropTable[i].PropId;
                        this.dgvVelTable.Rows[j * maxTable + i].Cells[index++].Value = unit.MovingPropTable[i].PropName;
                        this.dgvVelTable.Rows[j * maxTable + i].Cells[index++].Value = unit.MovingPropTable[i].VelSetList[j].Vel;
                        this.dgvVelTable.Rows[j * maxTable + i].Cells[index++].Value = unit.MovingPropTable[i].VelSetList[j].Acc;
                        this.dgvVelTable.Rows[j * maxTable + i].Cells[index++].Value = unit.MovingPropTable[i].VelSetList[j].Dec;
                        this.dgvVelTable.Rows[j * maxTable + i].Cells[index++].Value = unit.MovingPropTable[i].VelSetList[j].Jerk;
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
            try
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
                colPos.HeaderText = "Pos";
                colPos.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvAxesStatus.Columns.Add(colPos);

                DataGridViewTextBoxColumn colSpeed = new DataGridViewTextBoxColumn();
                colSpeed.HeaderText = "Speed";
                colSpeed.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvAxesStatus.Columns.Add(colSpeed);

                DataGridViewTextBoxColumn colTorque = new DataGridViewTextBoxColumn();
                colTorque.HeaderText = "Torque";
                colTorque.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvAxesStatus.Columns.Add(colTorque);

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

                DataGridViewTextBoxColumn colAlmCode = new DataGridViewTextBoxColumn();
                colAlmCode.HeaderText = "AlarmCode";
                colAlmCode.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvAxesStatus.Columns.Add(colAlmCode);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

		private void LoadTargetPos()
		{
            if (m_CurServoUnit == null) return;

			this.dgvTargetPos.DataSource = m_CurServoUnit.Axes;
			this.dgvTargetPos.RowHeadersVisible = false;
			this.dgvTargetPos.SelectionMode = DataGridViewSelectionMode.CellSelect;
			this.dgvTargetPos.Columns.Clear();

			DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
			colId.DataPropertyName = "AxisId";
			colId.HeaderText = "ID";
			colId.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            colId.ReadOnly = true;
            colId.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.dgvTargetPos.Columns.Add(colId);

			DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
			colName.DataPropertyName = "AxisName";
			colName.HeaderText = "Name";
			colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            colName.ReadOnly = true;
            colName.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.dgvTargetPos.Columns.Add(colName);

			DataGridViewTextBoxColumn colPos = new DataGridViewTextBoxColumn();
			colPos.DataPropertyName = "TargetPos";
			colPos.HeaderText = "Target Pos";
            colPos.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            colPos.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.dgvTargetPos.Columns.Add(colPos);

			DataGridViewTextBoxColumn colVel = new DataGridViewTextBoxColumn();
			colVel.DataPropertyName = "TargetSpeed";
			colVel.HeaderText = "Target Vel";
            colVel.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            colVel.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.dgvTargetPos.Columns.Add(colVel);

            DataGridViewTextBoxColumn colAcc = new DataGridViewTextBoxColumn();
            colAcc.DataPropertyName = "TargetAcc";
            colAcc.HeaderText = "Acc";
            colAcc.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvTargetPos.Columns.Add(colAcc);

            DataGridViewTextBoxColumn colDec = new DataGridViewTextBoxColumn();
            colDec.DataPropertyName = "TargetDec";
            colDec.HeaderText = "Dec";
            colDec.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvTargetPos.Columns.Add(colDec);

            DataGridViewTextBoxColumn colJerk = new DataGridViewTextBoxColumn();
            colJerk.DataPropertyName = "TargetJerk";
            colJerk.HeaderText = "Jerk";
            colJerk.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvTargetPos.Columns.Add(colJerk);
        }
        private void LoadMotionSensor()
        {
            try
            {
                if (m_CurAxis == null) return;
                this.dgvMotionSensorTable.RowHeadersVisible = false;
                this.dgvMotionSensorTable.SelectionMode = DataGridViewSelectionMode.CellSelect;
                this.dgvMotionSensorTable.Columns.Clear();

                DataGridViewTextBoxColumn colControl = new DataGridViewTextBoxColumn();
                colControl.DataPropertyName = "ControlMp";
                colControl.HeaderText = "ControlMp";
                colControl.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionSensorTable.Columns.Add(colControl);

                DataGridViewTextBoxColumn colUse = new DataGridViewTextBoxColumn();
                colUse.DataPropertyName = "SensorUse";
                colUse.HeaderText = "Use";
                colUse.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionSensorTable.Columns.Add(colUse);

                DataGridViewTextBoxColumn colSlaveNo = new DataGridViewTextBoxColumn();
                colSlaveNo.DataPropertyName = "SlaveNo";
                colSlaveNo.HeaderText = "SlaveNo";
                colSlaveNo.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionSensorTable.Columns.Add(colSlaveNo);

                DataGridViewTextBoxColumn colOffset = new DataGridViewTextBoxColumn();
                colOffset.DataPropertyName = "Offset";
                colOffset.HeaderText = "Offset";
                colOffset.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionSensorTable.Columns.Add(colOffset);

                DataGridViewTextBoxColumn colSize = new DataGridViewTextBoxColumn();
                colSize.DataPropertyName = "Size";
                colSize.HeaderText = "Size";
                colSize.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionSensorTable.Columns.Add(colSize);

                DataGridViewTextBoxColumn colTarget = new DataGridViewTextBoxColumn();
                colTarget.DataPropertyName = "SensorTargetValue";
                colTarget.HeaderText = "TargetBCR";
                colTarget.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionSensorTable.Columns.Add(colTarget);

                DataGridViewTextBoxColumn colRange = new DataGridViewTextBoxColumn();
                colRange.DataPropertyName = "SensorPositionSetRange";
                colRange.HeaderText = "Range";
                colRange.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionSensorTable.Columns.Add(colRange);

                DataGridViewTextBoxColumn colP2U = new DataGridViewTextBoxColumn();
                colP2U.DataPropertyName = "SensorPulseToUnit";
                colP2U.HeaderText = "PulseToUnit";
                colP2U.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionSensorTable.Columns.Add(colP2U);

                DataGridViewTextBoxColumn colCalDist = new DataGridViewTextBoxColumn();
                colCalDist.DataPropertyName = "SensorScanDistance";
                colCalDist.HeaderText = "ScanDistance";
                colCalDist.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionSensorTable.Columns.Add(colCalDist);

                this.dgvMotionSensorTable.Update();
                this.dgvMotionSensorTable.Refresh();
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }

        private void LoadMotionProfile()
        {
            try
            {
                ///////////////////////////////////////////////////////////////////////////
                ///Motion Profiles
                this.dgvMotionProfileTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                this.dgvMotionProfileTable.SelectionMode = DataGridViewSelectionMode.CellSelect;
                this.dgvMotionProfileTable.Columns.Clear();
                this.dgvMotionProfileTable.Rows.Clear();

                DataGridViewTextBoxColumn colDistance = new DataGridViewTextBoxColumn();
                colDistance.DataPropertyName = "Distance";
                colDistance.HeaderText = "Distance";
                colDistance.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionProfileTable.Columns.Add(colDistance);

                DataGridViewTextBoxColumn colVel = new DataGridViewTextBoxColumn();
                colVel.DataPropertyName = "Velocity";
                colVel.HeaderText = "Vel";
                colVel.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionProfileTable.Columns.Add(colVel);

                DataGridViewTextBoxColumn colAcc = new DataGridViewTextBoxColumn();
                colAcc.DataPropertyName = "Acceleration";
                colAcc.HeaderText = "Acc";
                colAcc.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionProfileTable.Columns.Add(colAcc);

                DataGridViewTextBoxColumn colDec = new DataGridViewTextBoxColumn();
                colDec.DataPropertyName = "Deceleration";
                colDec.HeaderText = "Dec";
                colDec.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionProfileTable.Columns.Add(colDec);

                DataGridViewTextBoxColumn colJerk = new DataGridViewTextBoxColumn();
                colJerk.DataPropertyName = "Jerk";
                colJerk.HeaderText = "Jerk";
                colJerk.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionProfileTable.Columns.Add(colJerk);

                DataGridViewTextBoxColumn colLimitFlag = new DataGridViewTextBoxColumn();
                colLimitFlag.DataPropertyName = "VelocityLimitFlag";
                colLimitFlag.HeaderText = "VelLimitFlag";
                colLimitFlag.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvMotionProfileTable.Columns.Add(colLimitFlag);

                if (m_CurAxis == null) return;

                int maxList = m_CurAxis.SequenceCommand.MotionProfileCount;
                for (int i = 0; i < maxList; i++)
                {
                    double a = m_CurAxis.SequenceCommand.MotionProfiles[i].Distance;
                    double b = m_CurAxis.SequenceCommand.MotionProfiles[i].Velocity;
                    double c = m_CurAxis.SequenceCommand.MotionProfiles[i].Acceleration;
                    double d = m_CurAxis.SequenceCommand.MotionProfiles[i].Deceleration;
                    double e = m_CurAxis.SequenceCommand.MotionProfiles[i].Jerk;
                    byte f = m_CurAxis.SequenceCommand.MotionProfiles[i].VelocityLimitFlag;
                    this.dgvMotionProfileTable.Rows.Add(a, b, c, d, e, f);
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }
        private void dgvMotionProfileTable_MouseClick(object sender, MouseEventArgs e)
        {
            MouseButtons button = e.Button;
            if (button == System.Windows.Forms.MouseButtons.Right)
            {
                this.contextMenuStripMotionProfile.Show((DataGridView)sender, new Point(e.X, e.Y));
            }
        }
        private void dgvMotionProfileTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (m_CurAxis == null) return;

            int row = this.dgvMotionProfileTable.SelectedCells[0].RowIndex;
            int col = this.dgvMotionProfileTable.SelectedCells[0].ColumnIndex;
            double dVal = col != 5 ? double.Parse(this.dgvMotionProfileTable.SelectedCells[0].Value.ToString()) : 0.0f;
            byte bVal = col == 5 ? byte.Parse(this.dgvMotionProfileTable.SelectedCells[0].Value.ToString()) : (byte)0;
            if (col == 0) m_CurAxis.SequenceCommand.MotionProfiles[row].Distance = dVal;
            else if (col == 1) m_CurAxis.SequenceCommand.MotionProfiles[row].Velocity = dVal;
            else if (col == 2) m_CurAxis.SequenceCommand.MotionProfiles[row].Acceleration = dVal;
            else if (col == 3) m_CurAxis.SequenceCommand.MotionProfiles[row].Deceleration = dVal;
            else if (col == 4) m_CurAxis.SequenceCommand.MotionProfiles[row].Jerk = dVal;
            else if (col == 5) m_CurAxis.SequenceCommand.MotionProfiles[row].VelocityLimitFlag = bVal;
        }

        private void aDDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_CurAxis == null) return;

                MotionProfile new_profile = new MotionProfile();
                m_CurAxis.SequenceCommand.MotionProfiles.Add(new_profile);
                LoadMotionProfile();
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }

        private void dELToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_CurAxis == null) return;

                int row = this.dgvMotionProfileTable.SelectedCells[0].RowIndex;
                int col = this.dgvMotionProfileTable.SelectedCells[0].ColumnIndex;
                m_CurAxis.SequenceCommand.MotionProfiles.RemoveAt(row);
                LoadMotionProfile();
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }
        private void UpdateAxesStatus(List<_Axis> axes)
		{
            bool HomeEnd = true;
            bool InPos = true;
            bool Alarm = false;
            for (int i = 0; i < axes.Count; i++)
            {
                MpAxis mpAxis = (axes[i] as MpAxis);
                ushort decimalPoint = mpAxis.DecimalPoint;
                double _curpos = axes[i].CurPos; //(axes[i] as IAxisCommand).GetAxisCurPos();
                string posText = decimalPoint <= 4 ? string.Format("{0:F4}", _curpos) : decimalPoint <= 5 ? string.Format("{0:F5}", _curpos) : string.Format("{0:F6}", _curpos);
                string speedText = axes[i].CurSpeed.ToString("F4"); // (axes[i] as IAxisCommand).GetAxisCurSpeed().ToString("F4");
                string torqueText = axes[i].CurTorque.ToString("F2");// (axes[i] as IAxisCommand).GetAxisCurTorque().ToString("F2");
                this.dgvAxesStatus[2, i].Value = posText;
                this.dgvAxesStatus[3, i].Value = speedText;
                this.dgvAxesStatus[4, i].Value = torqueText;

                int j = 5;
                enAxisInFlag axisStatus = axes[i].AxisStatus;// (axes[i] as IAxisCommand).GetAxisCurStatus();
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
                List<int> con_list = mpAxis.ControllerAlarmIdList.FindAll(n => n != 0);
                List<int> dri_list = mpAxis.DriverAlarmIdList.FindAll(n => n != 0);
                if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);
                string alarmlist = string.Format("{0},{1}", string.Join("-", con_list), string.Join("-", dri_list));
                this.dgvAxesStatus[j++, i].Value = string.Format("{0},{1}", alarmlist, axes[i].SequenceState.SequenceAlarmId);
            }

            if (m_CurAxis != null)
            {
                enAxisInFlag axisStatus = m_CurAxis.AxisStatus;// (m_CurAxis as IAxisCommand).GetAxisCurStatus();
                if ((axisStatus & enAxisInFlag.Alarm) == enAxisInFlag.Alarm) if (m_CurAxis.CommandSkip) Alarm = true;
                else Alarm = false;
                HomeEnd = (axisStatus & enAxisInFlag.HEnd) == enAxisInFlag.HEnd;
                label1.Text = string.Format("Axes Current ___ {0} : {1}", m_CurAxis.AxisName, m_CurAxis.AxisStateMsg);
            }
            else
            {
                string axis_state = string.Empty;
                for (int i = 0; i < axes.Count; i++)
                {
                    if (i > 0) axis_state += "                        ";
                    axis_state += string.Format("{0} : {1}", axes[i].AxisName, axes[i].AxisStateMsg);
                    if (i < axes.Count - 1) axis_state += "\r\n";
                }
                label1.Text = string.Format("Axes Current ___ {0}", axis_state);
            }

            if (IsTensionType(m_CurServoUnit)) HomeEnd = true;

            if (InPos && HomeEnd && !Alarm)
            {
                // Manual 일때만 사용 가능하도록 하자....
                bool manual = EqpStateManager.Instance.OpMode == OperateMode.Manual ? true : false;
                btnMove.Enabled = m_CurUserLevel <= AuthorizationLevel.Maintenance && manual;
                btnRepeat.Enabled = m_CurUserLevel <= AuthorizationLevel.Maintenance && manual;
            }
            else
            {
                btnMove.Enabled = false;
                btnRepeat.Enabled = false;
            }
		}

		private void dgvTeachingTable_CellClick(object sender, DataGridViewCellEventArgs e)
		{
            ButtonLog.WriteLog(this.Name.ToString(), "dgvTeachingTable_CellClick");
            if (dgvTeachingTable.CurrentRow == null) return;
            string msg = string.Empty;
            for (int i = 0; i < dgvTeachingTable.CurrentRow.Cells.Count; i++)
                msg += string.Format("{0},", dgvTeachingTable.CurrentRow.Cells[i].Value);
            lbPosition.Text = string.Format("Position  :  {0}", msg);
		}

        private void dgvVelTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ButtonLog.WriteLog(this.Name.ToString(), "dgvVelTable_CellClick");
            if (dgvVelTable.CurrentRow == null) return;
            if (m_CurServoUnit == null) return;

            string msg = string.Empty;
            int teaching_id = 0;
            if (int.TryParse(this.dgvVelTable.CurrentRow.Cells[1].Value.ToString(), out teaching_id))
            {
                VelocityData teachingData = m_CurServoUnit.MovingPropTable[teaching_id];
                msg += string.Format("{0}_", teachingData.PropName);
                for (int i = 0; i < teachingData.VelSetList.Count; i++)
                {
                    VelSet set = teachingData.VelSetList[i];
                    msg += string.Format("[{0},{1},{2},{3}]_", set.Vel, set.Acc, set.Dec, set.Jerk);
                }
            }
            lbVelocity.Text = string.Format("Velocity  :  {0}", msg);
        }
		#endregion

		#region Mehtods Event Load Save
		private void btnLoad_Click(object sender, EventArgs e)
		{
			if (m_CurServoUnit == null) return;

			m_CurServoUnit.ReadTeachinDataFromFile();
			LoadTeachingData(m_CurServoUnit);

			m_CurServoUnit.ReadVelDataFromFile();
			LoadMovingPropData(m_CurServoUnit);

            m_CurServoUnit.ReadSequenceDataToFile();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			Save();
		}

		public void Save()
		{
			if (m_CurServoUnit == null) return;

            try
            {
                double temp = 0.0;
                // Update Cell values to MovingPropTable.AxesPosValueArray
                for (int i = 0; i < m_CurServoUnit.MovingPropTable.Count; i++)
                {
                    int masterId = -1;
                    for (int j = 0; j < m_CurServoUnit.Axes.Count; j++)
                    {
                        int index = 3;
                        VelSet set = new VelSet
                        {
                            AxisCoord = m_CurServoUnit.Axes[j].AxisCoord,
                            Vel = double.Parse(this.dgvVelTable[index++, j * m_CurServoUnit.MovingPropTable.Count + i].Value.ToString()),
                            Acc = double.Parse(this.dgvVelTable[index++, j * m_CurServoUnit.MovingPropTable.Count + i].Value.ToString()),
                            Dec = double.Parse(this.dgvVelTable[index++, j * m_CurServoUnit.MovingPropTable.Count + i].Value.ToString()),
                            Jerk = double.Parse(this.dgvVelTable[index++, j * m_CurServoUnit.MovingPropTable.Count + i].Value.ToString()),
                        };
                        // Slave Axis일 경우
                        if ((m_CurServoUnit.Axes[j].GantryType && m_CurServoUnit.Axes[j].NodeId != m_CurServoUnit.Axes[j].SlaveNodeId)) masterId = j;
                        if (m_CurServoUnit.Axes[j].GantryType && m_CurServoUnit.Axes[j].NodeId == m_CurServoUnit.Axes[j].SlaveNodeId)
                        {
                            if (masterId >= 0) set = m_CurServoUnit.MovingPropTable[i].VelSetList[masterId];
                        }
                        m_CurServoUnit.MovingPropTable[i].VelSetList[j] = set;
                    }
                }
                m_CurServoUnit.SaveVelDataToFile();
                // Update Cell values to TeachingTable.AxesPosValueArray
                for (int i = 0; i < m_CurServoUnit.TeachingTable.Count; i++)
                {
                    for (int j = 0; j < m_CurServoUnit.Axes.Count; j++)
                    {
                        m_CurServoUnit.TeachingTable[i].AxesPosValueArray[j] = double.TryParse(this.dgvTeachingTable[2 + j, i].Value.ToString(), out temp) ? temp : 0.0;
                    }
                }
                // Save it
                m_CurServoUnit.SaveTeachingDataToFile();
                m_CurServoUnit.SaveSequenceDataToFile();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion

        #region Methods Event Apply To
        private void btnApplyToOp_Click(object sender, EventArgs e)
		{
            if (m_CurServoUnit == null) return;
            try
            {
                for (int i = 0; i < m_CurServoUnit.Axes.Count; i++)
                {
                    m_CurServoUnit.Axes[i].TargetPos = double.Parse(this.dgvTeachingTable.CurrentRow.Cells[2 + i].Value.ToString());
                }
                int teaching_id = 0;
                if (int.TryParse(this.dgvVelTable.CurrentRow.Cells[1].Value.ToString(), out teaching_id))
                {
                    VelocityData teachingData = m_CurServoUnit.MovingPropTable[teaching_id];
                    for (int i = 0; i < teachingData.VelSetList.Count; i++)
                    {
                        m_CurServoUnit.Axes[i].TargetSpeed = teachingData.VelSetList[i].Vel;
                        m_CurServoUnit.Axes[i].TargetAcc = teachingData.VelSetList[i].Acc;
                        m_CurServoUnit.Axes[i].TargetDec = teachingData.VelSetList[i].Dec;
                        m_CurServoUnit.Axes[i].TargetJerk = teachingData.VelSetList[i].Jerk;
                    }
                }

                this.dgvTargetPos.Update();
                this.dgvTargetPos.Refresh();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

		private void btnApplyToTeaching_Click(object sender, EventArgs e)
		{
            if (m_CurServoUnit == null) return;
            try
            {
                for (int i = 0; i < m_CurServoUnit.Axes.Count; i++)
                {
                    if (m_CurServoUnit.Axes[i].CommandSkip) continue;
                    dgvTeachingTable.CurrentRow.Cells[2 + i].Value =
                        this.dgvAxesStatus[2, i].Value;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
		#endregion

		#region Methods Event Motor Operation
		private void btnReset_Click(object sender, EventArgs e)
		{
			if (m_CurServoUnit == null)
			{
				lbMsg.Text = string.Format("Servo Unit Not Selected !");
				return;
			}
			if (m_CurAxis == null)
				lbMsg.Text = string.Format("{0}-Reset !", m_CurServoUnit.GetName());
			else lbMsg.Text = string.Format("{0}-{1}-Axis Reset !", m_CurServoUnit.GetName(), m_CurAxis.AxisName);

			enAxisResult rv = enAxisResult.None;
			if (m_CurAxis == null) // Servo Selected Case
			{
				rv = m_CurServoUnit.AlarmClear();
			}
			else // Axis Selected Case
			{
                foreach (_Axis axis in m_CurServoUnit.Axes)
                {
                    if (axis.AxisCoord == m_CurAxis.AxisCoord)
                    {
                        rv |= m_CurServoUnit.AlarmClear(axis);
                    }
                }
			}
			lbMsg.Text = string.Format("{0}-Reset = {1}", m_CurServoUnit.GetName(), rv.ToString());
		}

		private void btnServoOn_Click(object sender, EventArgs e)
		{
			if (m_CurServoUnit == null)
			{
				lbMsg.Text = string.Format("Servo Unit Not Selected !");
				return;
			}
			if (m_CurAxis == null)
				lbMsg.Text = string.Format("{0} Servo On !", m_CurServoUnit.GetName());
			else lbMsg.Text = string.Format("{0}-{1} Servo On !", m_CurServoUnit.GetName(), m_CurAxis.AxisName);

			enAxisResult rv = enAxisResult.None;
			if (m_CurAxis == null) // Servo Selected Case
			{
				rv = m_CurServoUnit.ServoOn();
			}
			else // Axis Selected Case
			{
				rv = m_CurServoUnit.ServoOn(m_CurAxis);
			}
			lbMsg.Text = string.Format("{0} Servo On = {1}", m_CurServoUnit.GetName(), rv.ToString());
		}

		private void btnServoOff_Click(object sender, EventArgs e)
		{
			if (m_CurServoUnit == null)
			{
				lbMsg.Text = string.Format("Servo Unit Not Selected !");
				return;
			}
            string msg = string.Format("确定要进行Servo Off操作吗？");
            if (MessageBox.Show(msg, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            if (m_CurAxis == null)
				lbMsg.Text = string.Format("{0} Servo Off !", m_CurServoUnit.GetName());
			else lbMsg.Text = string.Format("{0}-{1} Servo Off !", m_CurServoUnit.GetName(), m_CurAxis.AxisName);

			enAxisResult rv = enAxisResult.None;
			if (m_CurAxis == null) // Servo Selected Case
			{
				rv = m_CurServoUnit.ServoOff();
			}
			else // Axis Selected Case
			{
                rv = m_CurServoUnit.ServoOff(m_CurAxis);
			}
			lbMsg.Text = string.Format("{0}- Servo Off = {1}", m_CurServoUnit.GetName(), rv.ToString());
		}

		private void btnOrigin_Click(object sender, EventArgs e)
		{
			if (m_CurServoUnit == null)
			{
				lbMsg.Text = string.Format("Servo Unit Not Selected !");
				return;
			}
            string msg = string.Format("确定要进行Origin操作吗？");
            if (MessageBox.Show(msg, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            if (!IsOriginInterlock(m_CurServoUnit))
            {
                lbMsg.Text = string.Format("Servo Unit Cann't Origin !");
                return;
            }

			if (m_CurAxis == null)
				lbMsg.Text = string.Format("{0} Origin Start!", m_CurServoUnit.GetName());
			else lbMsg.Text = string.Format("{0}-{1} Origin Start!", m_CurServoUnit.GetName(), m_CurAxis.AxisName);

			if (m_CurAxis == null) // Servo Selected Case
			{
				bool isSafe = true;
				foreach (_Axis axis in m_CurServoUnit.Axes)
				{
					if (cbInterlockCheck.Checked)
					{
                        if (axis.CommandSkip) continue;
                        isSafe &= InterlockManager.IsSafe(axis, 0.0f, true);
					}
				}
				if (isSafe == false)
				{
					MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					return;
				}
			}
            else
            {
                bool isSafe = InterlockManager.IsSafe(m_CurAxis, 0.0f, true);
                if (isSafe == false)
                {
                    MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }

			enAxisResult rv = enAxisResult.None;
			if (m_CurAxis == null) // Servo Selected Case
			{
				rv = m_CurServoUnit.Home();
			}
			else // Axis Selected Case
			{
				foreach (_Axis axis in m_CurServoUnit.Axes)
				{
					if ((axis.AxisCoord == m_CurAxis.AxisCoord) && (axis.HomeOrder == m_CurAxis.HomeOrder))
					{
                        if (axis.CommandSkip) continue;
						rv |= m_CurServoUnit.Home(axis);
					}
				}
			}
			lbMsg.Text = string.Format("{0} Origin = {1}", m_CurServoUnit.GetName(), rv.ToString());
			cbInterlockCheck.Checked = true;
		}
		#endregion

		#region Methods Event Move Operation
		private void btnMove_Click(object sender, EventArgs e)
		{
            if (m_CurServoUnit == null)
            {
                lbMsg.Text = string.Format("Servo Unit Not Selected !");
                return;
            }
            if (m_CurAxis == null)
                lbMsg.Text = string.Format("{0} Move Start!", m_CurServoUnit.GetName());
            else lbMsg.Text = string.Format("{0}-{1} Move Start!", m_CurServoUnit.GetName(), m_CurAxis.AxisName);

            bool isSafe = true;
            if (m_CurAxis == null) // Servo Selected Case
            {
                foreach (_Axis axis in m_CurServoUnit.Axes)
                {
                    if (cbInterlockCheck.Checked)
                    {
                        if (axis.CommandSkip) continue;
                        isSafe &= InterlockManager.IsSafe(axis, axis.TargetPos);
                    }
                }

                if (isSafe == false)
                {
                    if (Sineva.VHL.Data.LogIn.AccountManager.Instance.CurAccount == null)
                    {
                        MessageBox.Show("Interlock Condition and Unknown Account\nOperation Denied!!!");
                        return;
                    }
                    if (Sineva.VHL.Data.LogIn.AccountManager.Instance.CurAccount.Level < AuthorizationLevel.Maintenance)
                    {
                        MessageBox.Show("Interlock Condition, Operation Denied!!!");
                        return;
                    }

                    if (MessageBox.Show("Interlock Unsafe! Will You Move it? (OK: Move, Cancel: Cancel)", "INTERLOCK", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                        return;
                    ButtonLog.WriteLog("INTERLOCK", "************************** SERVO MOVE INTERLOCK IGNORED 1st Time **************************");
                    if (MessageBox.Show("Do you really want to move it? (YES: Move, No: Cancel)\n\n", "INTERLOCK!!!!!!!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                        return;
                    ButtonLog.WriteLog("INTERLOCK", "************************** SERVO MOVE INTERLOCK IGNORED Again, It Works!!! **************************");
                }
                enAxisResult rv = enAxisResult.None;
                for (int i = 0; i < m_CurServoUnit.Axes.Count; i++)
                {
                    if (m_CurServoUnit.Axes[i].CommandSkip) continue;
                    VelSet set = new VelSet
                    {
                        AxisCoord = m_CurServoUnit.Axes[i].AxisCoord,
                        Vel = m_CurServoUnit.Axes[i].TargetSpeed,
                        Acc = m_CurServoUnit.Axes[i].TargetAcc,
                        Dec = m_CurServoUnit.Axes[i].TargetDec,
                        Jerk = m_CurServoUnit.Axes[i].TargetJerk,
                    };
                    rv |= m_CurServoUnit.MoveAxisStart(m_CurServoUnit.Axes[i], m_CurServoUnit.Axes[i].TargetPos, set);
                }
                lbMsg.Text = string.Format("{0} Move = {1} !", m_CurServoUnit.GetName(), rv.ToString());
            }
            else // Axis Selected Case
            {
                if (cbInterlockCheck.Checked)
                {
                    isSafe &= InterlockManager.IsSafe(m_CurAxis, m_CurAxis.TargetPos);

                    if (isSafe == false)
                    {
                        MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
                if (m_CurAxis.CommandSkip) return;

                enAxisResult rv = enAxisResult.None;
                VelSet set = new VelSet
                {
                    AxisCoord = m_CurAxis.AxisCoord,
                    Vel = m_CurAxis.TargetSpeed,
                    Acc = m_CurAxis.TargetAcc,
                    Dec = m_CurAxis.TargetDec,
                    Jerk = m_CurAxis.TargetJerk,
                };
                rv = m_CurServoUnit.MoveAxisStart(m_CurAxis, m_CurAxis.TargetPos, set);
                lbMsg.Text = string.Format("{0} Move = {1} !", m_CurServoUnit.GetName(), rv.ToString());
            }

            // Slave Target Pos display
            {
                _Axis master_axis = null;
                foreach (_Axis axis in m_CurServoUnit.Axes)
                {
                    if (axis.GantryType && axis.NodeId == axis.MasterNodeId) master_axis = axis;
                    if (master_axis != null && axis.CommandSkip) //Pos, Prop 는 설정하자~~
                    {
                        VelSet set = new VelSet
                        {
                            AxisCoord = master_axis.AxisCoord,
                            Vel = master_axis.TargetSpeed,
                            Acc = master_axis.TargetAcc,
                            Dec = master_axis.TargetDec,
                            Jerk = master_axis.TargetJerk,
                        };
                        m_CurServoUnit.SetTargetPosition(axis, master_axis.TargetPos, set);
                    }
                }
                LoadTargetPos();
            }

            cbInterlockCheck.Checked = true;
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			if (m_CurServoUnit == null)
			{
				lbMsg.Text = string.Format("Servo Unit Not Selected !");
				return;
			}
			if (m_CurAxis == null)
				lbMsg.Text = string.Format("{0} Stop !", m_CurServoUnit.GetName());
			else lbMsg.Text = string.Format("{0}-{1} Stop !", m_CurServoUnit.GetName(), m_CurAxis.AxisName);

			enAxisResult rv = enAxisResult.None;
            if (m_CurAxis == null) // Servo Selected Case
            {
				rv = m_CurServoUnit.Stop();
			}
            else // Axis Selected Case
            {
				m_TargetAxes.Clear();
				foreach (_Axis axis in m_CurServoUnit.Axes)
				{
					if (axis.AxisCoord == m_CurAxis.AxisCoord)
					{
						m_TargetAxes.Add(axis);
					}
				}
				for (int i = 0; i < m_TargetAxes.Count; i++)
				{
					rv |= m_CurServoUnit.Stop(m_TargetAxes[i]);
				}
            }

            btnRepeat.Text = "Repeat";
            m_CurServoUnit.Repeat = false;
			foreach (_Axis axis in m_CurServoUnit.Axes) axis.Repeat = false;
			lbMsg.Text = string.Format("{0} Stop = {1} !", m_CurServoUnit.GetName(), rv.ToString());
		}

		private void btnRepeat_Click(object sender, EventArgs e)
		{
			if (m_CurServoUnit == null)
			{
				lbMsg.Text = string.Format("Servo Unit Not Selected !");
				return;
			}
            string msg = string.Format("确定要进行Repeat操作吗？");
            if (MessageBox.Show(msg, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

			if (m_CurAxis == null)
				lbMsg.Text = string.Format("{0}-Servo Repeat {1} !", m_CurServoUnit.GetName(), m_CurServoUnit.Repeat ? "Stop" : "Start");
			else lbMsg.Text = string.Format("{0}-Servo {1}-Axis Repeat {2} !", m_CurServoUnit.GetName(), m_CurAxis.AxisName, m_CurServoUnit.Repeat ? "Stop" : "Start");

            if (m_CurServoUnit.Repeat)
            {
				btnRepeat.Text = "Repeat";
                foreach (_Axis axis in m_CurServoUnit.Axes) axis.Repeat = false;
                m_CurServoUnit.Repeat = false;
            }
            else
            {
				btnRepeat.Text = "Stop";
                if (m_CurAxis == null) // Servo Selected Case
                {
					bool isSafe = true;
					foreach (_Axis axis in m_CurServoUnit.Axes)
					{
						if (cbInterlockCheck.Checked)
						{
                            if (axis.CommandSkip) continue;
							isSafe &= InterlockManager.IsSafe(axis, axis.TargetPos);
						}
					}

					if (isSafe == false)
					{
						MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
						return;
					}

                    foreach (_Axis axis in m_CurServoUnit.Axes)
                    {
                        if (axis.CommandSkip) continue;
                        axis.Repeat = true;
                    }
				}
				else
				{
					bool isSafe = true;
					if (cbInterlockCheck.Checked)
					{
						isSafe &= InterlockManager.IsSafe(m_CurAxis, m_CurAxis.TargetPos);
					}
					if (isSafe == false)
					{
						MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
						return;
					}
                    if (m_CurAxis.CommandSkip) return;

					m_CurAxis.Repeat = true;
				}

				m_CurServoUnit.Repeat = true;
            }

			cbInterlockCheck.Checked = true;
		}
        private void btnSequenceMove_Click(object sender, EventArgs e)
        {
            if (m_CurServoUnit == null)
            {
                lbMsg.Text = string.Format("Servo Unit Not Selected !");
                return;
            }
            if (m_CurAxis == null) // Sequence Move는 무조건 한축만
            {
                lbMsg.Text = string.Format("Axis Not Selected !");
                return;
            }
            if (m_CurAxis.CommandSkip) return;
            // Sensor Using 상태
            if (m_CurAxis.MotionSensorPara.SensorUse == 1)
            {
                int left_slave_no = m_CurAxis.LeftBcrNodeId;
                int right_slave_no = m_CurAxis.RightBcrNodeId;
                if (m_CurAxis.MotionSensorPara.SlaveNo != left_slave_no && m_CurAxis.MotionSensorPara.SlaveNo != right_slave_no)    // Slave No - 9 or 10
                {
                    lbMsg.Text = string.Format("BCR Reading Slave Number NG !");
                    return;
                }

                m_CurAxis.SequenceCommand.PositionSensorInfo = ObjectCopier.Clone(m_CurAxis.MotionSensorPara);
            }

            this.progressBar1.Minimum = 0;
            this.progressBar1.Maximum = m_CurAxis.SequenceCommand.MotionProfileCount;
            this.progressBar1.Value = 0;

            // Move Condition을 Check 하자....
            enAxisResult rv = enAxisResult.None;
            rv = m_CurServoUnit.SequenceMoveAxisStart(m_CurAxis, m_CurAxis.SequenceCommand);
            lbMsg.Text = string.Format("{0}:{1}-Sequence Move Start {2} !", m_CurServoUnit.GetName(), m_CurAxis.AxisName, rv.ToString());

            LoadMotionProfile();
        }

        private void btnSequenceStop_Click(object sender, EventArgs e)
        {
            if (m_CurServoUnit == null)
            {
                lbMsg.Text = string.Format("Servo Unit Not Selected !");
                return;
            }
            if (m_CurAxis == null) return;

            if (m_CurAxis.CommandSkip) return;
            enAxisResult rv = enAxisResult.None;
            rv = m_CurServoUnit.Stop(m_CurAxis);
            lbMsg.Text = string.Format("{0}:{1}-Sequence Move Stop {2} !", m_CurServoUnit.GetName(), m_CurAxis.AxisName, rv.ToString());
        }


        #endregion

        #region Methods Jog
        private void UpdateJogOpCtrls()
		{
			this.btnJogX_plus.Enabled = false;
			this.btnJogX_minus.Enabled = false;
			this.btnJogY_plus.Enabled = false;
			this.btnJogY_minus.Enabled = false;
			this.btnJogZ_plus.Enabled = false;
			this.btnJogZ_minus.Enabled = false;
			this.btnJogT_plus.Enabled = false;
			this.btnJogT_minus.Enabled = false;

			this.btnJogX_plus.BackColor = Color.LightGray;
			this.btnJogX_minus.BackColor = Color.LightGray;
			this.btnJogY_plus.BackColor = Color.LightGray;
			this.btnJogY_minus.BackColor = Color.LightGray;
			this.btnJogZ_plus.BackColor = Color.LightGray;
			this.btnJogZ_minus.BackColor = Color.LightGray;
			this.btnJogT_plus.BackColor = Color.LightGray;
			this.btnJogT_minus.BackColor = Color.LightGray;

			this.trackBarJog.Enabled = false;
			this.trackBarJogX.Enabled = false;
			this.trackBarJogY.Enabled = false;
			this.trackBarJogZ.Enabled = false;
			this.trackBarJogT.Enabled = false;

			this.tbJogSpeed.Text = "";
			this.tbJogSpeedX.Text = "";
			this.tbJogSpeedY.Text = "";
			this.tbJogSpeedZ.Text = "";
			this.tbJogSpeedT.Text = "";

			this.tbJogDistance.Text = "";
			this.tbJogDistanceX.Text = "";
			this.tbJogDistanceY.Text = "";
			this.tbJogDistanceZ.Text = "";
			this.tbJogDistanceT.Text = "";

			if (m_CurNode.Level == 1)
			{
                ServoUnit servoUnit = (m_CurNode.Tag as ServoUnit);
                this.trackBarJog.Enabled = true;
				this.tbJogSpeed.Text = servoUnit.JogSpeed.ToString();
				this.tbJogDistance.Text = servoUnit.JogDistance.ToString();
				foreach (_Axis axis in servoUnit.Axes)
				{
					switch (axis.AxisCoord)
					{
						case enAxisCoord.X:
                            //this.btnJogX_plus.Enabled = true;
                            //this.btnJogX_minus.Enabled = true;
                            this.btnJogX_plus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
                            this.btnJogX_minus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
							this.btnJogX_plus.BackColor = Color.Yellow;
							this.btnJogX_minus.BackColor = Color.Yellow;
                            this.trackBarJogX.Enabled = true;
							this.tbJogSpeedX.Text = axis.JogSpeed.ToString();
							this.tbJogDistanceX.Text = axis.JogDistance.ToString();
							break;
						case enAxisCoord.Y:
                            //this.btnJogY_plus.Enabled = true;
                            //this.btnJogY_minus.Enabled = true;
                            this.btnJogY_plus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
                            this.btnJogY_minus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
                            this.btnJogY_plus.BackColor = Color.Yellow;
							this.btnJogY_minus.BackColor = Color.Yellow;
							this.trackBarJogY.Enabled = true;
							this.tbJogSpeedY.Text = axis.JogSpeed.ToString();
							this.tbJogDistanceY.Text = axis.JogDistance.ToString();
							break;
						case enAxisCoord.Z:
                            //this.btnJogZ_plus.Enabled = true;
                            //this.btnJogZ_minus.Enabled = true;
                            this.btnJogZ_plus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
                            this.btnJogZ_minus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
                            this.btnJogZ_plus.BackColor = Color.Yellow;
							this.btnJogZ_minus.BackColor = Color.Yellow;
							this.trackBarJogZ.Enabled = true;
							this.tbJogSpeedZ.Text = axis.JogSpeed.ToString();
							this.tbJogDistanceZ.Text = axis.JogDistance.ToString();
							break;
						case enAxisCoord.T:
                            //this.btnJogT_plus.Enabled = true;
                            //this.btnJogT_minus.Enabled = true;
                            this.btnJogT_plus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
                            this.btnJogT_minus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
                            this.btnJogT_plus.BackColor = Color.Yellow;
							this.btnJogT_minus.BackColor = Color.Yellow;
							this.trackBarJogT.Enabled = true;
							this.tbJogSpeedT.Text = axis.JogSpeed.ToString();
							this.tbJogDistanceT.Text = axis.JogDistance.ToString();
							break;
					}
				}
			}
			else if (m_CurNode.Level == 2)
			{
                _Axis axis = (m_CurNode.Tag as _Axis);

                switch (axis.AxisCoord)
				{
					case enAxisCoord.X:
                        //this.btnJogX_plus.Enabled = true;
                        //this.btnJogX_minus.Enabled = true;
                        this.btnJogX_plus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
                        this.btnJogX_minus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
						this.btnJogX_plus.BackColor = Color.Yellow;
						this.btnJogX_minus.BackColor = Color.Yellow;
						this.trackBarJogX.Enabled = true;
						this.tbJogSpeedX.Text = axis.JogSpeed.ToString();
						this.tbJogDistanceX.Text = axis.JogDistance.ToString();
						break;
					case enAxisCoord.Y:
                        //this.btnJogY_plus.Enabled = true;
                        //this.btnJogY_minus.Enabled = true;
                        this.btnJogY_plus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
                        this.btnJogY_minus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
						this.btnJogY_plus.BackColor = Color.Yellow;
						this.btnJogY_minus.BackColor = Color.Yellow;
						this.trackBarJogY.Enabled = true;
						this.tbJogSpeedY.Text = axis.JogSpeed.ToString();
						this.tbJogDistanceY.Text = axis.JogDistance.ToString();
						break;
					case enAxisCoord.Z:
                        //this.btnJogZ_plus.Enabled = true;
                        //this.btnJogZ_minus.Enabled = true;
                        this.btnJogZ_plus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
                        this.btnJogZ_minus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
						this.btnJogZ_plus.BackColor = Color.Yellow;
						this.btnJogZ_minus.BackColor = Color.Yellow;
						this.trackBarJogZ.Enabled = true;
						this.tbJogSpeedZ.Text = axis.JogSpeed.ToString();
						this.tbJogDistanceZ.Text = axis.JogDistance.ToString();
						break;
					case enAxisCoord.T:
                        //this.btnJogT_plus.Enabled = true;
                        //this.btnJogT_minus.Enabled = true;
                        this.btnJogT_plus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
                        this.btnJogT_minus.Enabled = m_CurUserLevel < AuthorizationLevel.Maintenance;
						this.btnJogT_plus.BackColor = Color.Yellow;
						this.btnJogT_minus.BackColor = Color.Yellow;
						this.trackBarJogT.Enabled = true;
						this.tbJogSpeedT.Text = axis.JogSpeed.ToString();
						this.tbJogDistanceT.Text = axis.JogDistance.ToString();
						break;
				}
			}
		}
		#endregion

		#region Methods Event Jog Operation

		#region Event NumericUpDown & TrackBar Sync
		private void trackBarJog_ValueChanged(object sender, EventArgs e)
		{
			this.numericUpDown1.Value = (sender as TrackBar).Value;
			this.numericUpDown2.Value = (sender as TrackBar).Value;
			this.numericUpDown3.Value = (sender as TrackBar).Value;
			this.numericUpDown4.Value = (sender as TrackBar).Value;
			this.numericUpDown5.Value = (sender as TrackBar).Value;
		}

		private void trackBarJogX_ValueChanged(object sender, EventArgs e)
		{
			this.numericUpDown2.Value = (sender as TrackBar).Value;
		}

		private void trackBarJogY_ValueChanged(object sender, EventArgs e)
		{
			this.numericUpDown3.Value = (sender as TrackBar).Value;
		}

		private void trackBarJogZ_ValueChanged(object sender, EventArgs e)
		{
			this.numericUpDown4.Value = (sender as TrackBar).Value;
		}

		private void trackBarJogT_ValueChanged(object sender, EventArgs e)
		{
			this.numericUpDown5.Value = (sender as TrackBar).Value;
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			this.trackBarJog.Value = (int)(sender as NumericUpDown).Value;
			this.trackBarJogX.Value = (int)(sender as NumericUpDown).Value;
			this.trackBarJogY.Value = (int)(sender as NumericUpDown).Value;
			this.trackBarJogZ.Value = (int)(sender as NumericUpDown).Value;
			this.trackBarJogT.Value = (int)(sender as NumericUpDown).Value;
		}

		private void numericUpDown2_ValueChanged(object sender, EventArgs e)
		{
			this.trackBarJogX.Value = (int)(sender as NumericUpDown).Value;
		}

		private void numericUpDown3_ValueChanged(object sender, EventArgs e)
		{
			this.trackBarJogY.Value = (int)(sender as NumericUpDown).Value;
		}

		private void numericUpDown4_ValueChanged(object sender, EventArgs e)
		{
			this.trackBarJogZ.Value = (int)(sender as NumericUpDown).Value;
		}

		private void numericUpDown5_ValueChanged(object sender, EventArgs e)
		{
			this.trackBarJogT.Value = (int)(sender as NumericUpDown).Value;
		}
		#endregion

        private void rbJogType_Click(object sender, EventArgs e)
        {
            if (m_CurServoUnit == null) return;

            RadioButton btn = sender as RadioButton;
            if (btn == rbJogTypeVMove)
            {
                if (this.m_CurServoUnit == null) return;
                m_CurServoUnit.JogMoveType = enServoJogMoveType.Vel;
                this.rbJogTypeRelative.Checked = false;
                this.rbJogTypeVMove.Checked = true;
            }
            else if (btn == rbJogTypeRelative)
            {
                if (this.m_CurServoUnit == null) return;
                m_CurServoUnit.JogMoveType = enServoJogMoveType.Relative;
                this.rbJogTypeVMove.Checked = false;
                this.rbJogTypeRelative.Checked = true;
            }
        }

		private void btnJog_MouseDown(object sender, MouseEventArgs e)
		{
            m_JogButtonDown = true;
            JogInfo jogInfo = (JogInfo)(sender as Button).Tag;
            double vel_temp = 0.0f;
            double dist_temp = 0.0f;
            bool ValueOk = true;

            if (m_CurServoUnit == null)
            {
                lbMsg.Text = string.Format("Servo Unit Not Selected !");
                return;
            }

            if (m_CurAxis == null)
            {
                lbMsg.Text = string.Format("{0} Jog Move Start!", m_CurServoUnit.GetName());
                if (ValueOk &= TextToDouble(tbJogSpeed, ref vel_temp)) m_CurServoUnit.JogSpeed = (float)vel_temp;
                if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative)
                    if (ValueOk &= TextToDouble(tbJogDistance, ref dist_temp)) m_CurServoUnit.JogDistance = (float)dist_temp;
            }
            else lbMsg.Text = string.Format("{0}-{1} Jog Move Start!", m_CurServoUnit.GetName(), m_CurAxis.AxisName);

            foreach (_Axis axis in m_CurServoUnit.GetAxes())
            {
                if (axis.AxisCoord == enAxisCoord.X)
                {
                    ValueOk &= TextToDouble(tbJogSpeedX, ref vel_temp);
                    if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative)
                        if (ValueOk &= TextToDouble(tbJogDistanceX, ref dist_temp)) axis.JogDistance = (float)dist_temp;
                }
                else if (axis.AxisCoord == enAxisCoord.Y)
                {
                    ValueOk &= TextToDouble(tbJogSpeedY, ref vel_temp);
                    if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative)
                        if (ValueOk &= TextToDouble(tbJogDistanceY, ref dist_temp)) axis.JogDistance = (float)dist_temp;
                }
                else if (axis.AxisCoord == enAxisCoord.Z)
                {
                    ValueOk &= TextToDouble(tbJogSpeedZ, ref vel_temp);
                    if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative)
                        if (ValueOk &= TextToDouble(tbJogDistanceZ, ref dist_temp)) axis.JogDistance = (float)dist_temp;
                }
                else if (axis.AxisCoord == enAxisCoord.T)
                {
                    ValueOk &= TextToDouble(tbJogSpeedT, ref vel_temp);
                    if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative)
                        if (ValueOk &= TextToDouble(tbJogDistanceT, ref dist_temp)) axis.JogDistance = (float)dist_temp;
                }
                if (ValueOk)
                {
                    axis.JogSpeed = (float)vel_temp; axis.JogAcc = (float)vel_temp; axis.JogDec = (float)vel_temp; axis.JogJerk = 2 * (float)vel_temp;
                    if (axis.JogAcc > axis.AccLimit) axis.JogAcc = axis.AccLimit;
                    if (axis.JogDec > axis.DecLimit) axis.JogDec = axis.DecLimit;
                    if (axis.JogJerk > axis.JerkLimit) axis.JogJerk = axis.JerkLimit;
                }
                else
                {
                    break;
                }
            }
            if (!ValueOk) return;

            enAxisResult rv = enAxisResult.None;
            if (m_CurAxis == null) // Servo Unit Case
            {
                bool isSafe = true;
                // Check Interlock 
                if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative)
                {
                    // Set Target
                    foreach (_Axis axis in m_CurServoUnit.Axes)
                    {
                        if (axis.AxisCoord != jogInfo.Coord) continue;
                        if (axis.CommandSkip || IsJogDirectionReverse(axis)) continue;
                        if (cbInterlockCheck.Checked)
                        {
                            if (jogInfo.Dir == enAxisDir.Positive) axis.TargetPos = axis.JogDistance + (axis as IAxisCommand).GetAxisCurPos();
                            else axis.TargetPos = axis.JogDistance * -1.0 + (axis as IAxisCommand).GetAxisCurPos();
                        }
                    }
                    // Safe Check
                    foreach (_Axis axis in m_CurServoUnit.Axes)
                    {
                        if (axis.AxisCoord != jogInfo.Coord) continue;
                        if (axis.CommandSkip || IsJogDirectionReverse(axis)) continue;

                        if (jogInfo.Dir == enAxisDir.Positive)
                        {
                            if (cbInterlockCheck.Checked)
                            {
                                isSafe &= InterlockManager.IsSafe(axis, axis.JogDistance + (axis as IAxisCommand).GetAxisCurPos());
                            }
                        }
                        else
                        {
                            if (cbInterlockCheck.Checked)
                            {
                                isSafe &= InterlockManager.IsSafe(axis, axis.JogDistance * -1.0 + (axis as IAxisCommand).GetAxisCurPos());
                            }
                        }
                    }
                }

                if (isSafe == false)
                {
                    MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                // Send Command
                m_TargetAxes.Clear();
                foreach (_Axis axis in m_CurServoUnit.Axes)
                {
                    if (axis.AxisCoord == jogInfo.Coord)
                    {
                        if (axis.CommandSkip || IsJogDirectionReverse(axis)) continue;
                        m_TargetAxes.Add(axis);
                    }
                }
                if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Vel)
                {
                    for (int i = 0; i < m_TargetAxes.Count; i++)
                    {
                        VelSet set = new VelSet
                        {
                            AxisCoord = m_TargetAxes[i].AxisCoord,
                            Vel = m_TargetAxes[i].JogSpeed,
                            Acc = m_TargetAxes[i].JogAcc > 0.0f ? m_TargetAxes[i].JogAcc : m_TargetAxes[i].JogSpeed,
                            Dec = m_TargetAxes[i].JogDec > 0.0f ? m_TargetAxes[i].JogDec : m_TargetAxes[i].JogSpeed,
                            Jerk = m_TargetAxes[i].JogJerk > 0.0f ? m_TargetAxes[i].JogJerk : 2 * m_TargetAxes[i].JogSpeed,
                        };
                        if (jogInfo.Dir == enAxisDir.Positive)
                            rv |= m_CurServoUnit.JogMove(m_TargetAxes[i], enAxisOutFlag.JogPlus, set);
                        else
                            rv |= m_CurServoUnit.JogMove(m_TargetAxes[i], enAxisOutFlag.JogMinus, set);
                    }
                }
                else if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative)
                {
                    for (int i = 0; i < m_TargetAxes.Count; i++)
                    {
                        VelSet set = new VelSet
                        {
                            AxisCoord = m_TargetAxes[i].AxisCoord,
                            Vel = m_TargetAxes[i].JogSpeed,
                            Acc = m_TargetAxes[i].JogAcc > 0.0f ? m_TargetAxes[i].JogAcc : m_TargetAxes[i].JogSpeed,
                            Dec = m_TargetAxes[i].JogDec > 0.0f ? m_TargetAxes[i].JogDec : m_TargetAxes[i].JogSpeed,
                            Jerk = m_TargetAxes[i].JogJerk > 0.0f ? m_TargetAxes[i].JogJerk : 2 * m_TargetAxes[i].JogSpeed,
                        };
                        if (jogInfo.Dir == enAxisDir.Positive)
                            rv |= m_CurServoUnit.MoveRelativeStart(m_TargetAxes[i], m_TargetAxes[i].JogDistance, set);
                        else
                            rv |= m_CurServoUnit.MoveRelativeStart(m_TargetAxes[i], (-1) * m_TargetAxes[i].JogDistance, set);
                    }
                }
            }
            else // Axis Case
            {
                bool jog_reverse = IsJogDirectionReverse(m_CurAxis);
                int reverse = jog_reverse ? -1 : 1;

                bool isSafe = true;
                // Check Interlock 
                if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative)
                {
                    if (jogInfo.Dir == enAxisDir.Positive)
                    {
                        if (cbInterlockCheck.Checked)
                        {
                            isSafe &= InterlockManager.IsSafe(m_CurAxis, reverse * m_CurAxis.JogDistance + (m_CurAxis as IAxisCommand).GetAxisCurPos());
                        }
                    }
                    else
                    {
                        if (cbInterlockCheck.Checked)
                        {
                            isSafe &= InterlockManager.IsSafe(m_CurAxis, reverse * m_CurAxis.JogDistance * -1.0 + (m_CurAxis as IAxisCommand).GetAxisCurPos());
                        }
                    }
                }

                if (isSafe == false)
                {
                    MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                int nDir = 1;
                if (jog_reverse) nDir = -1;

                // Check Interlock 
                if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative && cbInterlockCheck.Checked)
                {
                    double target_pos = 0.0f;
                    if (jogInfo.Dir == enAxisDir.Positive)
                        target_pos = (m_CurAxis as IAxisCommand).GetAxisCurPos() + nDir * m_CurAxis.JogDistance;
                    else
                        target_pos = (m_CurAxis as IAxisCommand).GetAxisCurPos() + (-1) * nDir * m_CurAxis.JogDistance;

                    isSafe &= InterlockManager.IsSafe(m_CurAxis, target_pos);
                }

                if (isSafe == false)
                {
                    MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                VelSet set = new VelSet
                {
                    AxisCoord = m_CurAxis.AxisCoord,
                    Vel = m_CurAxis.JogSpeed,
                    Acc = m_CurAxis.JogAcc > 0.0f ? m_CurAxis.JogAcc : m_CurAxis.JogSpeed,
                    Dec = m_CurAxis.JogDec > 0.0f ? m_CurAxis.JogDec : m_CurAxis.JogSpeed,
                    Jerk = m_CurAxis.JogJerk > 0.0f ? m_CurAxis.JogJerk : 2 * m_CurAxis.JogSpeed,
                };
                // Send Command
                if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Vel)
                {
                    if (jog_reverse)
                    {
                        if (jogInfo.Dir == enAxisDir.Positive)
                            rv |= m_CurServoUnit.JogMove(m_CurAxis, enAxisOutFlag.JogMinus, set);
                        else
                            rv |= m_CurServoUnit.JogMove(m_CurAxis, enAxisOutFlag.JogPlus, set);
                    }
                    else
                    {
                        if (jogInfo.Dir == enAxisDir.Positive)
                            rv |= m_CurServoUnit.JogMove(m_CurAxis, enAxisOutFlag.JogPlus, set);
                        else
                           rv |= m_CurServoUnit.JogMove(m_CurAxis, enAxisOutFlag.JogMinus, set);
                    }
                }
                else if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative)
                {
                    if (IsTensionType(m_CurServoUnit))
                    {
                        MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                    if (IsSameCoordSyncMove(m_CurServoUnit))
                    {
                        if (IsClampYMoveInterlock(m_CurServoUnit, jogInfo))
                        {
                            MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }

                        m_TargetAxes.Clear();
                        foreach (_Axis axis in m_CurServoUnit.Axes)
                        {
                            if (axis.AxisCoord == m_CurAxis.AxisCoord && axis.MoveOrder == m_CurAxis.MoveOrder && !axis.CommandSkip)
                                m_TargetAxes.Add(axis);
                        }
                        for (int i = 0; i < m_TargetAxes.Count; i++)
                        {
                            if (jogInfo.Dir == enAxisDir.Positive)
                                rv |= m_CurServoUnit.MoveRelativeStart(m_TargetAxes[i], nDir * m_TargetAxes[i].JogDistance, set);
                            else
                                rv |= m_CurServoUnit.MoveRelativeStart(m_TargetAxes[i], (-1) * nDir * m_TargetAxes[i].JogDistance, set);
                        }
                    }
                    else
                    {
                        if (jogInfo.Dir == enAxisDir.Positive)
                            rv |= m_CurServoUnit.MoveRelativeStart(m_CurAxis, nDir * m_CurAxis.JogDistance, set);
                        else
                            rv |= m_CurServoUnit.MoveRelativeStart(m_CurAxis, (-1) * nDir * m_CurAxis.JogDistance, set);
                    }
                }
            }

            // Slave Target Pos display
            if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative)
            {
                _Axis master_axis = null;
                foreach (_Axis axis in m_CurServoUnit.Axes)
                {
                    if (axis.GantryType && axis.NodeId == axis.MasterNodeId) master_axis = axis;
                    if (master_axis != null && axis.CommandSkip) //Pos, Prop 는 설정하자~~
                    {
                        VelSet set = new VelSet
                        {
                            AxisCoord = master_axis.AxisCoord,
                            Vel = master_axis.TargetSpeed,
                            Acc = master_axis.TargetAcc,
                            Dec = master_axis.TargetDec,
                            Jerk = master_axis.TargetJerk,
                        };
                        m_CurServoUnit.SetTargetPosition(axis, master_axis.TargetPos, set);
                    }
                }
                LoadTargetPos();
            }

            lbMsg.Text = string.Format("{0} Jog Move = {1}", m_CurServoUnit.GetName(), rv.ToString());
            cbInterlockCheck.Checked = true;
		}

		private void btnJog_MouseUp(object sender, MouseEventArgs e)
		{
            m_JogButtonDown = false;
            JogInfo jogInfo = (JogInfo)(sender as Button).Tag;

			if (m_CurServoUnit == null) return;
			if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Vel)
			{
				m_CurServoUnit.ResetJogSpeed();
				m_CurServoUnit.ResetCommand();
			}
		}

        private void btnMpStart_Click(object sender, EventArgs e)
        {
            MxpManager.Instance.SystemRun();
        }

        private void btnMpStop_Click(object sender, EventArgs e)
        {
            MxpManager.Instance.SystemStop();
        }
        #endregion

        private void tbJogSpeed_TextChanged(object sender, EventArgs e)
		{
			if (m_CurServoUnit == null) return;

			TextBox obj = (TextBox)sender;
			if (obj == tbJogSpeed)
			{
				if (tbJogSpeed.Text == "") return;
				tbJogSpeedX.Text = obj.Text;
				tbJogSpeedY.Text = obj.Text;
				tbJogSpeedZ.Text = obj.Text;
				tbJogSpeedT.Text = obj.Text;
			}
		}

		private void tbJogDistance_TextChanged(object sender, EventArgs e)
		{
			if (m_CurServoUnit == null) return;

			TextBox obj = (TextBox)sender;
			if (obj == tbJogDistance)
			{
				if (tbJogDistance.Text == "") return;
				tbJogDistanceX.Text = obj.Text;
				tbJogDistanceY.Text = obj.Text;
				tbJogDistanceZ.Text = obj.Text;
				tbJogDistanceT.Text = obj.Text;
			}
		}

        private void cbInterlockCheck_CheckedChanged(object sender, EventArgs e)
        {
            if(cbInterlockCheck.Checked == false)
            {
                DataItem_UserInfo curAccount = Sineva.VHL.Data.LogIn.AccountManager.Instance.CurAccount;
                if(curAccount == null)
                {
                    cbInterlockCheck.Checked = true;
                    return;
                }
                if(curAccount.Level != AuthorizationLevel.Administrator)
                {
                    cbInterlockCheck.Checked = true;
                    return;
                }
            }
            ButtonLog.WriteLog(this.Name.ToString(), "cbInterlockCheck_CheckedChanged");
            ButtonLog.WriteLog(this.Name.ToString(), string.Format("Servo Interlock Check {0} !", cbInterlockCheck.Checked ? "SET" : "RELEASE"));
        }

		private bool TextToDouble(TextBox tb, ref double dVal)
		{
			bool rv = double.TryParse(tb.Text, out dVal);
			if (rv == false) { tb.Text = "0.0"; dVal = 0.0f; }
			return rv;
		}

        public void SetAuthority(AuthorizationLevel level)
        {

            bool manual_key = EqpStateManager.Instance.OpMode == OperateMode.Auto ? false : true;
            m_CurUserLevel = level;
            cbInterlockCheck.Enabled = level < AuthorizationLevel.Developer && manual_key;

            btnJogT_minus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnJogT_plus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnJogX_minus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnJogX_plus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnJogY_minus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnJogY_plus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnJogZ_minus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnJogZ_plus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;

            btnServoOn.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnServoOff.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnOrigin.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnReset.Enabled = level < AuthorizationLevel.Maintenance && manual_key;

            btnMove.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnRepeat.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnStop.Enabled = level < AuthorizationLevel.Maintenance && manual_key;

            btnLoad.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnSave.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnApplyToTeaching.Enabled = level < AuthorizationLevel.Maintenance && manual_key;

            btnApplyToOp.Enabled = level < AuthorizationLevel.Maintenance && manual_key;

            dgvTeachingTable.ReadOnly = level >= AuthorizationLevel.Maintenance && manual_key;
            dgvVelTable.ReadOnly = level >= AuthorizationLevel.Maintenance && manual_key;
            dgvTargetPos.ReadOnly = level >= AuthorizationLevel.Maintenance && manual_key;

            dgvMotionProfileTable.ReadOnly = level >= AuthorizationLevel.Maintenance && manual_key;
            dgvMotionSensorTable.ReadOnly = level >= AuthorizationLevel.Maintenance && manual_key;
        }

        private bool IsClampYMoveInterlock(ServoUnit servo, JogInfo jogInfo)
        {
            if (servo == null) return false;
            bool interlock = false;
            return interlock;
        }
        private bool IsGantryX1X2SyncInterlock(ServoUnit servo)
        {
            if(servo == null) return false;

            bool interlock = false;
            return interlock;
        }
        private bool IsGantryServo(ServoUnit servo)
        {
            if (servo == null) return false;
            bool gantry = false;
            return gantry;
        }
        private bool IsTransferServo(ServoUnit servo)
        {
            if (servo == null) return false;
            bool transfer = false;
            return transfer;
        }
        private bool IsFrameReceiver(ServoUnit servo)
        {
            if (servo == null) return false;
            bool receiver = false;
            return receiver;
        }
        private bool IsUvwAlignServo(ServoUnit servo)
        {
            if(servo == null) return false;
            bool uvw_align = false;
            return uvw_align;
        }
        private bool IsIndexAlignServo(ServoUnit servo)
        {
            if(servo == null) return false;
            bool index_align = false;
            return index_align;
        }
        private bool IsJogDirectionReverse(_Axis axis)
        {
            if (axis == null) return false;
            bool reverse = false;
            return reverse;
        }
        private bool IsTensionType(ServoUnit servo)
        {
            if (servo == null) return false;
            bool tension_type = false;
            return tension_type;
        }
        private bool IsSameCoordSyncMove(ServoUnit servo)
        {
            if (servo == null) return false;
            bool sync = false;
            return sync;
        }
        private bool IsOriginInterlock(ServoUnit servo)
        {
            if (servo == null) return false;
            bool origin_enable = true;
            return origin_enable;
        }
    }
}
