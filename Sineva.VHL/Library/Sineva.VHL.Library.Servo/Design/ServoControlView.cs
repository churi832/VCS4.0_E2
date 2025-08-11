using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;

namespace Sineva.VHL.Library.Servo
{
	public partial class ServoControlView : UserControl
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

		private List<MotionSensor> m_MotionSensorList = new List<MotionSensor>();

        private bool m_Initialized = false;
        private IFormUpdate m_FormUpdate = null;
        #endregion

        #region Constructor
        public ServoControlView()
		{
			InitializeComponent();

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
		}
		#endregion

		#region Methods
		public bool Initialize(ServoManager instance)
		{
            if (!m_Initialized)
			{
                m_Manager = instance;

                if (m_Manager.Initialized == false)
                    m_Manager.Initialize();

                UpdateTreeView(m_Manager);

                foreach (ServoUnit unit in m_Manager.ServoUnits)
                {
                    unit.ReadTeachinDataFromFile();
                    unit.ReadVelDataFromFile();
                    unit.ReadSequenceDataToFile();
                }

                treeView.SelectedNode = treeView.Nodes[0];
                UpdateCurNode(treeView.SelectedNode.FirstNode);
                m_Initialized = true;
            }

            return m_Initialized;
		}

        public bool Initialize(ServoManager instance, IFormUpdate formUpdate)
        {
            if (!m_Initialized)
            {
                m_Manager = instance;

                m_FormUpdate = formUpdate;

                if (m_Manager.Initialized == false)
                    m_Manager.Initialize();

                UpdateTreeView(m_Manager);

                foreach (ServoUnit unit in m_Manager.ServoUnits)
                {
                    unit.ReadTeachinDataFromFile();
                    unit.ReadVelDataFromFile();
                    unit.ReadSequenceDataToFile();
                }

                treeView.SelectedNode = treeView.Nodes[0];
                UpdateCurNode(treeView.SelectedNode.FirstNode);
                m_Initialized = true;
            }

            return m_Initialized;
        }

        public void UpdateState()
		{
			EnabledButton();

            if (m_CurServoUnit == null) return;
            if (m_FormUpdate != null && m_FormUpdate.UpdateNeed == false) return;

            enAxisResult rv = enAxisResult.None;
            if (m_CurAxis == null) rv = m_CurServoUnit.MotionDone();
            else rv = m_CurServoUnit.MotionDoneAxis(m_CurAxis);
            if (rv != enAxisResult.None)
            {
                if (m_CurAxis == null) SetMsg(string.Format("{0}-Servo {1} !", m_CurServoUnit.GetName(), rv.ToString()));
                else SetMsg(string.Format("{0}-Servo {1}-Axis {2} !", m_CurServoUnit.GetName(), m_CurAxis.AxisName, rv.ToString()));
            }

			UpdateAxesStatus(m_Manager.AxisSource);

            if (m_CurAxis != null && m_CurAxis.SequenceState.IsSequenceBusy)
            {
                int cur = m_CurAxis.SequenceState.SequenceCurrentStep;
                int total = m_CurAxis.SequenceState.SequenceCurrentStep + m_CurAxis.SequenceState.SequenceRemainCount;
                lbSequenceInfo.Text = string.Format("{0} / {1}", cur, total);
                this.progressBar1.Value = cur;
            }

            if (m_CurAxis != null)
                label1.Text = string.Format("override test = {0}", (m_CurAxis as MpAxis).GetSpeedOverrideRate());
        }
        #endregion

        #region Methods Treeview
        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			try
			{
                System.Windows.Forms.TreeView view = sender as System.Windows.Forms.TreeView;
                UpdateCurNode(e.Node);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
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

                // Teaching table Show
                LoadTeachingData(m_CurServoUnit);
                LoadMovingPropData(m_CurServoUnit);

                // Axes Status Show
                this.dgvAxesStatus.DataSource = m_Manager.AxisSource;
                LoadAxesStatus(m_Manager.AxisSource);
                UpdateAxesStatus(m_Manager.AxisSource);

                SetMsg("unit Select  :  " + m_CurServoUnit.ServoName);
                // Target Pos display
                LoadTargetPos();

                if (m_CurAxis != null)
                {
                    m_MotionSensorList.Clear();
                    m_MotionSensorList.Add(m_CurAxis.MotionSensorPara);
                    this.dgvMotionSensorTable.DataSource = m_MotionSensorList;
                    LoadMotionSensor();
                    LoadMotionProfile();
                }

                if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Vel)
                {
                    this.rbJogTypeVMove.Checked = true;
                }
                else if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative)
                {
                    this.rbJogTypeRelative.Checked = true;
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
                    nodeUnit.Expand();
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
                this.dgvTeachingTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                //this.dgvTeachingTable.RowHeadersVisible = false;
                //this.dgvTeachingTable.AutoGenerateColumns = false;
                this.dgvTeachingTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                this.dgvTeachingTable.Columns.Clear();

                int dataStartIndex = 0;

                DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
                colId.DataPropertyName = "PosId";
                colId.HeaderText = "ID";
                colId.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                this.dgvTeachingTable.Columns.Add(colId);
                dataStartIndex++;

                DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
                colName.DataPropertyName = "PosName";
                colName.HeaderText = "Name";
                colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                this.dgvTeachingTable.Columns.Add(colName);
                dataStartIndex++;

                // Create & Add Column Axes
                for (int i = 0; i < unit.Axes.Count; i++)
                {
                    DataGridViewTextBoxColumn colVal = new DataGridViewTextBoxColumn();
                    colVal.HeaderText = unit.Axes[i].AxisName;
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
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
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
                this.dgvVelTable.Columns.Add(colAxis);
                dataStartIndex++;

                DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
                colId.DataPropertyName = "PropId";
                colId.HeaderText = "ID";
                colId.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                this.dgvVelTable.Columns.Add(colId);
                dataStartIndex++;

                DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
                colName.DataPropertyName = "PropName";
                colName.HeaderText = "Name";
                colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
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
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
		}

        private void LoadAxesStatus(List<_Axis> axes)
		{
            try
            {
                this.dgvAxesStatus.AutoGenerateColumns = false;
                this.dgvAxesStatus.RowHeadersVisible = false;
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
                //colPos.DataPropertyName = "CurPos";
                colTorque.HeaderText = "Torque";
                colTorque.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvAxesStatus.Columns.Add(colTorque);

                DataGridViewTextBoxColumn colSpeed = new DataGridViewTextBoxColumn();
                //colPos.DataPropertyName = "CurPos";
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

                DataGridViewImageColumn colPLimit = new DataGridViewImageColumn();
                colPLimit.HeaderText = "+ Limit";
                colPLimit.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvAxesStatus.Columns.Add(colPLimit);

                DataGridViewImageColumn colNLimit = new DataGridViewImageColumn();
                colNLimit.HeaderText = "- Limit";
                colNLimit.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvAxesStatus.Columns.Add(colNLimit);


                DataGridViewTextBoxColumn colAlmCode = new DataGridViewTextBoxColumn();
                colAlmCode.HeaderText = "AlarmCode";
                colAlmCode.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvAxesStatus.Columns.Add(colAlmCode);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }

        private void LoadTargetPos()
		{
            try
            {
                this.dgvTargetPos.DataSource = m_CurServoUnit.Axes;
                this.dgvTargetPos.RowHeadersVisible = false;
                this.dgvTargetPos.SelectionMode = DataGridViewSelectionMode.CellSelect;
                this.dgvTargetPos.Columns.Clear();

                DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
                colId.DataPropertyName = "AxisId";
                colId.HeaderText = "ID";
                colId.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvTargetPos.Columns.Add(colId);

                DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
                colName.DataPropertyName = "AxisName";
                colName.HeaderText = "Name";
                colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvTargetPos.Columns.Add(colName);

                DataGridViewTextBoxColumn colPos = new DataGridViewTextBoxColumn();
                colPos.DataPropertyName = "TargetPos";
                colPos.HeaderText = "Pos";
                colPos.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                this.dgvTargetPos.Columns.Add(colPos);

                DataGridViewTextBoxColumn colVel = new DataGridViewTextBoxColumn();
                colVel.DataPropertyName = "TargetSpeed";
                colVel.HeaderText = "Vel";
                colVel.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
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
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
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
			try
			{
                for (int i = 0; i < axes.Count; i++)
                {
                    this.dgvAxesStatus[2, i].Value = (axes[i] as IAxisCommand).GetAxisCurPos().ToString("F4");
                    this.dgvAxesStatus[3, i].Value = (axes[i] as IAxisCommand).GetAxisCurTorque();
                    this.dgvAxesStatus[4, i].Value = (axes[i] as IAxisCommand).GetAxisCurSpeed();

                    int j = 5;
                    if (((axes[i] as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm)
                    {
                        this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On_Red;
                    }
                    else
                    {
                        this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                    }

                    if (((axes[i] as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InPos) == enAxisInFlag.InPos)
                    {
                        this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On;
                    }
                    else
                    {
                        this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                    }

                    if (((axes[i] as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.HEnd) == enAxisInFlag.HEnd)
                    {
                        this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On;
                    }
                    else
                    {
                        this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                    }

                    if (((axes[i] as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn)
                    {
                        this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On;
                    }
                    else
                    {
                        this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                    }

                    if (((axes[i] as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Org) == enAxisInFlag.Org)
                    {
                        this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On;
                    }
                    else
                    {
                        this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                    }

                    if (((axes[i] as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_N) == enAxisInFlag.Limit_N)
                    {
                        this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On_Red;
                    }
                    else
                    {
                        this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                    }

                    if (((axes[i] as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_P) == enAxisInFlag.Limit_P)
                    {
                        this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_On_Red;
                    }
                    else
                    {
                        this.dgvAxesStatus[j++, i].Value = Properties.Resources.BasicLamp_Off;
                    }
                    List<int> con_list = (axes[i] as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                    List<int> dri_list = (axes[i] as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                    if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);
                    string alarmlist = string.Format("{0},{1}", string.Join("-", con_list), string.Join("-", dri_list));

                    this.dgvAxesStatus[j++, i].Value = string.Format("{0},{1}", alarmlist, axes[i].SequenceState.SequenceAlarmId);
                }
            }
			catch (Exception ex)
			{
				System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
				ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
			}
        }

		private void EnabledButton()
		{
			bool allDisable = false;
			if (m_CurServoUnit == null) allDisable = true;
            if (m_FormUpdate != null && m_FormUpdate.UpdateNeed == false) allDisable = true;
			if (allDisable) 
			{
                btnMove.Enabled = false;
                btnRepeat.Enabled = false;
                btnSequenceMove.Enabled = false;
                return;
            }

            bool HomeEnd = true;
            bool InPos = true;
            bool Alarm = false;
			if (m_CurAxis == null)
			{
                for (int i = 0; i < m_CurServoUnit.Axes.Count; i++)
                {
                    _Axis axis = m_CurServoUnit.Axes[i];
                    if (!axis.CommandSkip)
                    {
                        if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm) Alarm = true;
                        if (!axis.InPosCheckSkip && ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InPos) != enAxisInFlag.InPos) InPos = false;
                        if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.HEnd) != enAxisInFlag.HEnd) HomeEnd = false;
                    }
                }
            }
			else
			{
                if (((m_CurAxis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm) Alarm = true;
                if (!m_CurAxis.InPosCheckSkip && ((m_CurAxis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InPos) != enAxisInFlag.InPos) InPos = false;
                if (((m_CurAxis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.HEnd) != enAxisInFlag.HEnd) HomeEnd = false;
            }
            if (InPos && HomeEnd && !Alarm)
            {
                // Manual 일때만 사용 가능하도록 하자....
                bool manual = EqpStateManager.Instance.OpMode != OperateMode.Auto ? true : false;
                btnMove.Enabled = manual;
                btnRepeat.Enabled = manual;
                btnSequenceMove.Enabled = manual;
            }
            else
            {
                btnMove.Enabled = false;
                btnRepeat.Enabled = false;
                btnSequenceMove.Enabled = false;
            }
        }
        private void dgvTeachingTable_CellClick(object sender, DataGridViewCellEventArgs e)
		{
            if (dgvTeachingTable.CurrentRow == null) return;
            string msg = string.Empty;
            for (int i = 0; i < dgvTeachingTable.CurrentRow.Cells.Count; i++)
                msg += string.Format("{0},", dgvTeachingTable.CurrentRow.Cells[i].Value);
            titledPanel4.Title = string.Format("Velocity  :  {0}", msg);
		}

        private void dgvVelTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvVelTable.CurrentRow == null) return;
            string msg = string.Empty;
            for (int i = 0; i < dgvVelTable.CurrentRow.Cells.Count; i++) 
                msg += string.Format("{0},", dgvVelTable.CurrentRow.Cells[i].Value);
            titledPanel5.Title = string.Format("Velocity  :  {0}", msg);
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
			try
			{
                if (m_CurServoUnit == null) return;
                // Update Cell values to MovingPropTable.AxesPosValueArray
                for (int i = 0; i < m_CurServoUnit.MovingPropTable.Count; i++)
                {
                    for (int j = 0; j < m_CurServoUnit.Axes.Count; j++)
                    {
                        int index = 3;
                        VelSet set = new VelSet
                        {
                            AxisCoord = m_CurServoUnit.Axes[i].AxisCoord,
                            Vel = double.Parse(this.dgvVelTable[index++, j * m_CurServoUnit.MovingPropTable.Count + i].Value.ToString()),
                            Acc = double.Parse(this.dgvVelTable[index++, j * m_CurServoUnit.MovingPropTable.Count + i].Value.ToString()),
                            Dec = double.Parse(this.dgvVelTable[index++, j * m_CurServoUnit.MovingPropTable.Count + i].Value.ToString()),
                            Jerk = double.Parse(this.dgvVelTable[index++, j * m_CurServoUnit.MovingPropTable.Count + i].Value.ToString()),
                        };
                        m_CurServoUnit.MovingPropTable[i].VelSetList[j] = set;
                    }
                }
                m_CurServoUnit.SaveVelDataToFile();
                // Update Cell values to TeachingTable.AxesPosValueArray
                for (int i = 0; i < m_CurServoUnit.TeachingTable.Count; i++)
                {
                    for (int j = 0; j < m_CurServoUnit.Axes.Count; j++)
                    {
                        m_CurServoUnit.TeachingTable[i].AxesPosValueArray[j] = double.Parse(this.dgvTeachingTable[2 + j, i].Value.ToString());
                    }
                }
                // Save it
                m_CurServoUnit.SaveTeachingDataToFile();
                m_CurServoUnit.SaveSequenceDataToFile();
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
		}
		#endregion

		#region Methods Event Apply To
		private void btnApplyToOp_Click(object sender, EventArgs e)
		{
			try
			{
                for (int i = 0; i < m_CurServoUnit.Axes.Count; i++)
                {
                    m_CurServoUnit.Axes[i].TargetPos = double.Parse(this.dgvTeachingTable.CurrentRow.Cells[2 + i].Value.ToString());
                }

                int teaching_id = 0;
                if (int.TryParse(this.dgvVelTable.CurrentRow.Cells[0].Value.ToString(), out teaching_id))
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
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
		}

		private void btnApplyToTeaching_Click(object sender, EventArgs e)
		{
			try
			{
                for (int i = 0; i < m_CurServoUnit.Axes.Count; i++)
                {
                    for (int j = 0; j < m_Manager.AxisSource.Count; j++)
                    {
                        if (m_CurServoUnit.Axes[i] == m_Manager.AxisSource[j])
                        {
                            dgvTeachingTable.CurrentRow.Cells[2 + i].Value =
                            this.dgvAxesStatus[2, j].Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
		}
		#endregion

		#region Methods Event Motor Operation
		private void btnReset_Click(object sender, EventArgs e)
		{
			if (m_CurServoUnit == null)
			{
				SetMsg(string.Format("Servo Unit Not Selected"));
				return;
			}

            enAxisResult rv = enAxisResult.None;
            if (m_CurAxis == null) // Servo Selected Case
			{
                rv = m_CurServoUnit.AlarmClear();
                SetMsg(string.Format("{0}-Reset {1} !", m_CurServoUnit.GetName(), rv.ToString()));
            }
            else // Axis Selected Case
			{
                rv = m_CurServoUnit.AlarmClear(m_CurAxis);
                SetMsg(string.Format("{0}:{1}-Reset !", m_CurServoUnit.GetName(), m_CurAxis.AxisName, rv.ToString()));
            }
        }

		private void btnServoOn_Click(object sender, EventArgs e)
		{
            if (m_CurServoUnit == null)
            {
                SetMsg(string.Format("Servo Unit Not Selected"));
                return;
            }
            enAxisResult rv = enAxisResult.None;
            if (m_CurAxis == null) // Servo Selected Case
			{
                rv = m_CurServoUnit.ServoOn();
                SetMsg(string.Format("{0}-Servo On {1} !", m_CurServoUnit.GetName(), rv.ToString()));
            }
            else // Axis Selected Case
			{
                rv = m_CurServoUnit.ServoOn(m_CurAxis);
                SetMsg(string.Format("{0}:{1}-Servo On {2} !", m_CurServoUnit.GetName(), m_CurAxis.AxisName, rv.ToString()));
            }
        }

		private void btnServoOff_Click(object sender, EventArgs e)
		{
            if (m_CurServoUnit == null)
            {
                SetMsg(string.Format("Servo Unit Not Selected"));
                return;
            }
            enAxisResult rv = enAxisResult.None;
            if (m_CurAxis == null) // Servo Selected Case
			{
                rv = m_CurServoUnit.ServoOff();
                SetMsg(string.Format("{0}-Servo Off {1} !", m_CurServoUnit.GetName(), rv.ToString()));
            }
            else // Axis Selected Case
			{
                rv = m_CurServoUnit.ServoOff(m_CurAxis);
                SetMsg(string.Format("{0}:{1}-Servo Off {2} !", m_CurServoUnit.GetName(), m_CurAxis.AxisName, rv.ToString()));
            }
        }

		private void btnOrigin_Click(object sender, EventArgs e)
		{
            if (m_CurServoUnit == null)
            {
                SetMsg(string.Format("Servo Unit Not Selected"));
                return;
            }
            enAxisResult rv = enAxisResult.None;
            if (m_CurAxis == null) // Servo Selected Case
			{
				rv = m_CurServoUnit.Home();
                SetMsg(string.Format("{0}-Origin Start {1} !", m_CurServoUnit.GetName(), rv.ToString()));
            }
            else // Axis Selected Case
			{
                SetMsg(string.Format("{0}:{1}-Origin Start {2} !", m_CurServoUnit.GetName(), m_CurAxis.AxisName, rv.ToString()));
                rv = m_CurServoUnit.Home(m_CurAxis);
			}
		}
		#endregion

		#region Methods Event Move Operation
		private void btnMove_Click(object sender, EventArgs e)
		{
            if (m_CurServoUnit == null)
            {
                SetMsg(string.Format("Servo Unit Not Selected"));
                return;
            }
            enAxisResult rv = enAxisResult.None;
            if (m_CurAxis == null) // Servo Selected Case
			{
                foreach (_Axis axis in m_CurServoUnit.Axes)
                {
					if (axis.CommandSkip) continue;
                    VelSet set = new VelSet
                    {
                        AxisCoord = axis.AxisCoord,
                        Vel = axis.TargetSpeed,
                        Acc = axis.TargetAcc,
                        Dec = axis.TargetDec,
                        Jerk = axis.TargetJerk,
                    };
					rv |= m_CurServoUnit.MoveAxisStart(axis, axis.TargetPos, set);
                }
                SetMsg(string.Format("{0}-Move Start {1} !", m_CurServoUnit.GetName(), rv.ToString()));
            }
            else // Axis Selected Case
			{
                if (m_CurAxis.CommandSkip) return;
                VelSet set = new VelSet
                {
                    AxisCoord = m_CurAxis.AxisCoord,
                    Vel = m_CurAxis.TargetSpeed,
                    Acc = m_CurAxis.TargetAcc,
                    Dec = m_CurAxis.TargetDec,
                    Jerk = m_CurAxis.TargetJerk,
                };
                rv = m_CurServoUnit.MoveAxisStart(m_CurAxis, m_CurAxis.TargetPos, set);
                SetMsg(string.Format("{0}:{1}-Move Start {2} !", m_CurServoUnit.GetName(), m_CurAxis.AxisName, rv.ToString()));
            }
        }

		private void btnStop_Click(object sender, EventArgs e)
		{
            if (m_CurServoUnit == null)
            {
                SetMsg(string.Format("Servo Unit Not Selected"));
                return;
            }
            enAxisResult rv = enAxisResult.None;
            if (m_CurAxis == null) // Servo Selected Case
            {
				rv = m_CurServoUnit.Stop();
                SetMsg(string.Format("{0}-Move Stop {1} !", m_CurServoUnit.GetName(), rv.ToString()));
            }
            else // Axis Selected Case
            {
                if (m_CurAxis.CommandSkip) return;
                rv = m_CurServoUnit.Stop(m_CurAxis);
                SetMsg(string.Format("{0}:{1}-Move Stop {2} !", m_CurServoUnit.GetName(), m_CurAxis.AxisName, rv.ToString()));
            }
            m_CurServoUnit.Repeat = false;
            foreach (_Axis axis in m_CurServoUnit.Axes) axis.Repeat = false;
        }

        private void btnRepeat_Click(object sender, EventArgs e)
		{
            if (m_CurServoUnit == null)
            {
                SetMsg(string.Format("Servo Unit Not Selected"));
                return;
            }

            string msg = string.Format("确定要进行Repeat操作吗？");
            if (MessageBox.Show(msg, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            if (m_CurServoUnit.Repeat)
            {
                foreach (_Axis axis in m_CurServoUnit.Axes) axis.Repeat = false;
                m_CurServoUnit.Repeat = false;
            }
            else
            {
                m_CurServoUnit.Repeat = true;
                if (m_CurAxis == null) // Servo Selected Case
                {
					foreach (_Axis axis in m_CurServoUnit.Axes)
					{
						if (axis.CommandSkip) continue;
						axis.Repeat = true;
					}
                }
                else // Axis Selected Case
                {
                    m_CurAxis.Repeat = true;
                }
            }
		}
        private void btnSequenceMove_Click(object sender, EventArgs e)
        {
            if (m_CurServoUnit == null)
            {
                SetMsg(string.Format("Servo Unit Not Selected"));
                return;
            }
            if (m_CurAxis == null) // Sequence Move는 무조건 한축만
            {
                SetMsg(string.Format("Axis Not Selected"));
                return;
            }
            if (m_CurAxis.CommandSkip) return;
            // Sensor Using 상태
            if (m_CurAxis.MotionSensorPara.SensorUse == 1)
            {
                if (m_CurAxis.MotionSensorPara.SlaveNo != 9 && m_CurAxis.MotionSensorPara.SlaveNo != 10)    // Slave No - 9 or 10
                {
                    SetMsg(string.Format("BCR Reading Slave Number NG"));
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
            SetMsg(string.Format("{0}:{1}-Sequence Move Start {2} !", m_CurServoUnit.GetName(), m_CurAxis.AxisName, rv.ToString()));

            LoadMotionProfile();
        }

        private void btnSequenceStop_Click(object sender, EventArgs e)
        {
            if (m_CurServoUnit == null)
            {
                SetMsg(string.Format("Servo Unit Not Selected"));
                return;
            }
            if (m_CurAxis == null) return;

            if (m_CurAxis.CommandSkip) return;
            enAxisResult rv = enAxisResult.None;
            rv = m_CurServoUnit.Stop(m_CurAxis);
            SetMsg(string.Format("{0}:{1}-Sequence Move Stop {2} !", m_CurServoUnit.GetName(), m_CurAxis.AxisName, rv.ToString()));
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
				this.trackBarJog.Enabled = true;
				this.tbJogSpeed.Text = (m_CurNode.Tag as ServoUnit).JogSpeed.ToString();
				this.tbJogDistance.Text = (m_CurNode.Tag as ServoUnit).JogDistance.ToString();
				foreach (_Axis axis in (m_CurNode.Tag as ServoUnit).Axes)
				{
					switch (axis.AxisCoord)
					{
						case enAxisCoord.X:
							this.btnJogX_plus.Enabled = true;
							this.btnJogX_minus.Enabled = true;
							this.btnJogX_plus.BackColor = Color.Yellow;
							this.btnJogX_minus.BackColor = Color.Yellow;
							this.trackBarJogX.Enabled = true;
							this.tbJogSpeedX.Text = axis.JogSpeed.ToString();
							this.tbJogDistanceX.Text = axis.JogDistance.ToString();
							break;
						case enAxisCoord.Y:
							this.btnJogY_plus.Enabled = true;
							this.btnJogY_minus.Enabled = true;
							this.btnJogY_plus.BackColor = Color.Yellow;
							this.btnJogY_minus.BackColor = Color.Yellow;
							this.trackBarJogY.Enabled = true;
							this.tbJogSpeedY.Text = axis.JogSpeed.ToString();
							this.tbJogDistanceY.Text = axis.JogDistance.ToString();
							break;
						case enAxisCoord.Z:
							this.btnJogZ_plus.Enabled = true;
							this.btnJogZ_minus.Enabled = true;
							this.btnJogZ_plus.BackColor = Color.Yellow;
							this.btnJogZ_minus.BackColor = Color.Yellow;
							this.trackBarJogZ.Enabled = true;
							this.tbJogSpeedZ.Text = axis.JogSpeed.ToString();
							this.tbJogDistanceZ.Text = axis.JogDistance.ToString();
							break;
						case enAxisCoord.T:
							this.btnJogT_plus.Enabled = true;
							this.btnJogT_minus.Enabled = true;
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
				switch ((m_CurNode.Tag as _Axis).AxisCoord)
				{
					case enAxisCoord.X:
						this.btnJogX_plus.Enabled = true;
						this.btnJogX_minus.Enabled = true;
						this.btnJogX_plus.BackColor = Color.Yellow;
						this.btnJogX_minus.BackColor = Color.Yellow;
						this.trackBarJogX.Enabled = true;
						this.tbJogSpeedX.Text = (m_CurNode.Tag as _Axis).JogSpeed.ToString();
						this.tbJogDistanceX.Text = (m_CurNode.Tag as _Axis).JogDistance.ToString();
						break;
					case enAxisCoord.Y:
						this.btnJogY_plus.Enabled = true;
						this.btnJogY_minus.Enabled = true;
						this.btnJogY_plus.BackColor = Color.Yellow;
						this.btnJogY_minus.BackColor = Color.Yellow;
						this.trackBarJogY.Enabled = true;
						this.tbJogSpeedY.Text = (m_CurNode.Tag as _Axis).JogSpeed.ToString();
						this.tbJogDistanceY.Text = (m_CurNode.Tag as _Axis).JogDistance.ToString();
						break;
					case enAxisCoord.Z:
						this.btnJogZ_plus.Enabled = true;
						this.btnJogZ_minus.Enabled = true;
						this.btnJogZ_plus.BackColor = Color.Yellow;
						this.btnJogZ_minus.BackColor = Color.Yellow;
						this.trackBarJogZ.Enabled = true;
						this.tbJogSpeedZ.Text = (m_CurNode.Tag as _Axis).JogSpeed.ToString();
						this.tbJogDistanceZ.Text = (m_CurNode.Tag as _Axis).JogDistance.ToString();
						break;
					case enAxisCoord.T:
						this.btnJogT_plus.Enabled = true;
						this.btnJogT_minus.Enabled = true;
						this.btnJogT_plus.BackColor = Color.Yellow;
						this.btnJogT_minus.BackColor = Color.Yellow;
						this.trackBarJogT.Enabled = true;
						this.tbJogSpeedT.Text = (m_CurNode.Tag as _Axis).JogSpeed.ToString();
						this.tbJogDistanceT.Text = (m_CurNode.Tag as _Axis).JogDistance.ToString();
						break;
				}
			}
		}
		#endregion

		#region Methods Event Jog Operation

		#region Event NumericUpDown & TrackBar Sync
		private void trackBarJog_ValueChanged(object sender, EventArgs e)
		{
			this.numericUpDown1.Value = (sender as System.Windows.Forms.TrackBar).Value;
			this.numericUpDown2.Value = (sender as System.Windows.Forms.TrackBar).Value;
			this.numericUpDown3.Value = (sender as System.Windows.Forms.TrackBar).Value;
			this.numericUpDown4.Value = (sender as System.Windows.Forms.TrackBar).Value;
			this.numericUpDown5.Value = (sender as System.Windows.Forms.TrackBar).Value;
		}

		private void trackBarJogX_ValueChanged(object sender, EventArgs e)
		{
			this.numericUpDown2.Value = (sender as System.Windows.Forms.TrackBar).Value;
		}

		private void trackBarJogY_ValueChanged(object sender, EventArgs e)
		{
			this.numericUpDown3.Value = (sender as System.Windows.Forms.TrackBar).Value;
		}

		private void trackBarJogZ_ValueChanged(object sender, EventArgs e)
		{
			this.numericUpDown4.Value = (sender as System.Windows.Forms.TrackBar).Value;
		}

		private void trackBarJogT_ValueChanged(object sender, EventArgs e)
		{
			this.numericUpDown5.Value = (sender as System.Windows.Forms.TrackBar).Value;
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
			if (this.rbJogTypeRelative.Checked)
			{
				if (this.m_CurServoUnit == null) return;
				m_CurServoUnit.JogMoveType = enServoJogMoveType.Relative;
			}
			else if (this.rbJogTypeVMove.Checked)
			{
				if (this.m_CurServoUnit == null) return;
				m_CurServoUnit.JogMoveType = enServoJogMoveType.Vel;
			}
		}

		private void btnJog_MouseDown(object sender, MouseEventArgs e)
		{
			JogInfo jogInfo = (JogInfo)(sender as System.Windows.Forms.Button).Tag;

			if (m_CurServoUnit == null) return;

			if( tbJogSpeed.Text != "" ) m_CurServoUnit.JogSpeed = float.Parse(tbJogSpeed.Text);
			if( tbJogDistance.Text != "") m_CurServoUnit.JogDistance = float.Parse(tbJogDistance.Text);
			foreach (_Axis axis in m_CurServoUnit.GetAxes())
			{
				if (axis.AxisCoord == enAxisCoord.X)
				{
					if( tbJogSpeedX.Text != "" ) axis.JogSpeed = float.Parse(tbJogSpeedX.Text);
					if( tbJogDistanceX.Text != "" ) axis.JogDistance = float.Parse(tbJogDistanceX.Text);
				}
				else if (axis.AxisCoord == enAxisCoord.Y)
				{
					if( tbJogSpeedY.Text != "" ) axis.JogSpeed = float.Parse(tbJogSpeedY.Text);
					if( tbJogDistanceY.Text != "" ) axis.JogDistance = float.Parse(tbJogDistanceY.Text);
				}
				else if (axis.AxisCoord == enAxisCoord.Z)
				{
					if( tbJogSpeedZ.Text != "" ) axis.JogSpeed = float.Parse(tbJogSpeedZ.Text);
					if( tbJogDistanceZ.Text != "" ) axis.JogDistance = float.Parse(tbJogDistanceZ.Text);
				}
				else if (axis.AxisCoord == enAxisCoord.T)
				{
					if( tbJogSpeedT.Text != "" ) axis.JogSpeed = float.Parse(tbJogSpeedT.Text);
					if( tbJogDistanceT.Text != "" ) axis.JogDistance = float.Parse(tbJogDistanceT.Text);
				}
			}

			if (m_CurAxis == null) // Servo Unit Case
			{
				foreach (_Axis axis in m_CurServoUnit.Axes)
				{
					if (axis.AxisCoord == jogInfo.Coord)
					{
                        VelSet set = new VelSet
                        {
                            AxisCoord = axis.AxisCoord,
                            Vel = axis.JogSpeed,
                            Acc = axis.JogAcc > 0.0f ? axis.JogAcc : axis.JogSpeed,
                            Dec = axis.JogDec > 0.0f ? axis.JogDec : axis.JogSpeed,
                            Jerk = axis.JogJerk > 0.0f ? axis.JogJerk : 2 * axis.JogSpeed,
                        };

                        if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Vel)
						{
							if (jogInfo.Dir == enAxisDir.Positive)
							{
								(axis as IAxisCommand).SetJogSpeedAsync(set);
								(axis as IAxisCommand).SetCommandAsync(enAxisOutFlag.JogPlus);
							}
							else
							{
								(axis as IAxisCommand).SetJogSpeedAsync(set);
								(axis as IAxisCommand).SetCommandAsync(enAxisOutFlag.JogMinus);
							}
						}
						else if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative)
						{
							if (jogInfo.Dir == enAxisDir.Positive)
							{
								m_CurServoUnit.MoveRelativeStart(axis, axis.JogDistance, set);
							}
							else
							{
								m_CurServoUnit.MoveRelativeStart(axis, (-1) * axis.JogDistance, set);
							}
						}
					}
				}
			}
			else // Axis Case
			{
                VelSet set = new VelSet
                {
                    AxisCoord = m_CurAxis.AxisCoord,
                    Vel = m_CurAxis.JogSpeed,
                    Acc = m_CurAxis.JogAcc > 0.0f ? m_CurAxis.JogAcc : m_CurAxis.JogSpeed,
                    Dec = m_CurAxis.JogDec > 0.0f ? m_CurAxis.JogDec : m_CurAxis.JogSpeed,
                    Jerk = m_CurAxis.JogJerk > 0.0f ? m_CurAxis.JogJerk : 2 * m_CurAxis.JogSpeed,
                };

                if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Vel)
				{
					if (jogInfo.Dir == enAxisDir.Positive)
					{
						(m_CurAxis as IAxisCommand).SetJogSpeedAsync(set);
						(m_CurAxis as IAxisCommand).SetCommandAsync(enAxisOutFlag.JogPlus);
					}
					else
					{
						(m_CurAxis as IAxisCommand).SetJogSpeedAsync(set);
						(m_CurAxis as IAxisCommand).SetCommandAsync(enAxisOutFlag.JogMinus);
					}
				}
				else if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Relative)
				{
					if (jogInfo.Dir == enAxisDir.Positive)
					{
						m_CurServoUnit.MoveRelativeStart(m_CurAxis, m_CurAxis.JogDistance, set);
					}
					else
					{
						m_CurServoUnit.MoveRelativeStart(m_CurAxis, (-1) * m_CurAxis.JogDistance, set);
					}
				}
			}
		}

		private void btnJog_MouseUp(object sender, MouseEventArgs e)
		{
			JogInfo jogInfo = (JogInfo)(sender as System.Windows.Forms.Button).Tag;

			if (m_CurServoUnit == null) return;
			if (m_CurAxis == null) // Servo Unit Case
			{
				if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Vel)
				{
                    (m_CurServoUnit as IServoUnit).ResetJogSpeed();
                    (m_CurServoUnit as IServoUnit).ResetCommand();
				}
			}
			else // Axis Case
			{
				if (m_CurServoUnit.JogMoveType == enServoJogMoveType.Vel)
				{
                    m_CurServoUnit.ResetJogSpeed(m_CurAxis);
                    m_CurServoUnit.ResetCommand(m_CurAxis);
				}
			}
		}
        #endregion

        private void tbJogSpeed_TextChanged(object sender, EventArgs e)
		{
			if (m_CurServoUnit == null) return;

            System.Windows.Forms.TextBox obj = (System.Windows.Forms.TextBox)sender;
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

            System.Windows.Forms.TextBox obj = (System.Windows.Forms.TextBox)sender;
			if (obj == tbJogDistance)
			{
				if (tbJogDistance.Text == "") return;
				tbJogDistanceX.Text = obj.Text;
				tbJogDistanceY.Text = obj.Text;
				tbJogDistanceZ.Text = obj.Text;
				tbJogDistanceT.Text = obj.Text;
			}
		}

        public void SetAuthority(AuthorizationLevel level)
        {
			bool manual = EqpStateManager.Instance.OpMode == OperateMode.Manual ? true : false;

            btnJogT_minus.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnJogT_plus.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnJogX_minus.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnJogX_plus.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnJogY_minus.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnJogY_plus.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnJogZ_minus.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnJogZ_plus.Enabled = level <= AuthorizationLevel.Maintenance && manual;

            btnServoOn.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnServoOff.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnOrigin.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnReset.Enabled = level <= AuthorizationLevel.Maintenance && manual;

            btnMove.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnRepeat.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnStop.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnSequenceMove.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnSequenceStop.Enabled = level <= AuthorizationLevel.Maintenance && manual;

            btnLoad.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnSave.Enabled = level <= AuthorizationLevel.Maintenance && manual;
            btnApplyToTeaching.Enabled = level <= AuthorizationLevel.Maintenance && manual;

            btnApplyToOp.Enabled = level <= AuthorizationLevel.Maintenance && manual;

            dgvTeachingTable.ReadOnly = level > AuthorizationLevel.Maintenance && manual;
            dgvVelTable.ReadOnly = level > AuthorizationLevel.Maintenance && manual;
            dgvMotionProfileTable.ReadOnly = level > AuthorizationLevel.Maintenance && manual;
            dgvMotionSensorTable.ReadOnly = level > AuthorizationLevel.Maintenance && manual;
            dgvTargetPos.ReadOnly = level > AuthorizationLevel.Maintenance && manual;
        }

		private void SetMsg(string msg)
		{
			lbMsg.Text = msg;
		}

        private void button1_Click(object sender, EventArgs e)
        {
            if (m_CurAxis == null) return;

            (m_CurAxis as MpAxis).SetSpeedOverrideRate(0.5f);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (m_CurAxis == null) return;

            (m_CurAxis as MpAxis).SetSpeedOverrideRate(0.1f);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (m_CurAxis == null) return;

            (m_CurAxis as MpAxis).SetSpeedOverrideRate(0.0f);
        }

        private void tbOverride_TextChanged(object sender, EventArgs e)
        {
            if (m_CurAxis == null) return;

            double speed_override = 0.0f;
            if (double.TryParse(tbOverride.Text, out speed_override))
            {
                (m_CurAxis as MpAxis).SetSpeedOverrideRate(speed_override);
            }
        }
    }
}
