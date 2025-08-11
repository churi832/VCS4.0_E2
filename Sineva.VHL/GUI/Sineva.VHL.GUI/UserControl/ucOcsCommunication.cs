using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Sineva.VHL.Data;
using Sineva.VHL.Data.Process;
using Sineva.VHL.IF.OCS;
using Sineva.VHL.Library;
using Sineva.VHL.Device.ServoControl;

namespace Sineva.VHL.GUI
{
    public partial class ucOcsCommunication : UserControl, IUpdateUCon
    {
        #region Fields
        private Command m_Command = new Command();
        private bool m_UpdateMotionProc = false;
        private int m_UpdateIndex = -1;
        private bool m_UpdatePathList = false;
        private List<Sineva.VHL.Data.Process.Path> m_PathLists = new List<Sineva.VHL.Data.Process.Path>();
        private bool m_UpdateCommand = false;
        private List<TransferCommand> m_UpdateCommandLists = new List<TransferCommand>();

        private string m_BeforeAlarmMessage = string.Empty;
        private string m_BeforeAlarmCode = string.Empty;
        List<_DevAxis> m_DevAxes = new List<_DevAxis>();
        #endregion

        #region Constructor
        public ucOcsCommunication()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods - Interface
        public bool Initialize()
        {
            btnCancelTest.Visible = AppConfig.Instance.Simulation.MY_DEBUG;

            InitListView(ProcessDataHandler.Instance.TransferCommands);
            InitPathGrid();
            this.filteredPropertyGrid1.SelectedObject = m_Command;
            if (ProcessDataHandler.Instance.CurTransferCommand.IsValid)
            {
                m_PathLists = ObjectCopier.Clone(ProcessDataHandler.Instance.CurTransferCommand.PathMaps);
                m_UpdatePathList = true;
            }

            m_DevAxes = ServoControlManager.Instance.GetDevAxes();

            EventHandlerManager.Instance.UpdatePathMapChanged += Instance_UpdatePathMapChanged;
            EventHandlerManager.Instance.UpdateMotionProcess += Instance_UpdateMotionProcess;
            EventHandlerManager.Instance.UpdateCommandChanged += Instance_UpdateCommandChanged;
            EventHandlerManager.Instance.AlarmHappened += Instance_AlarmHappened;

            Panel_debug.Visible = AppConfig.Instance.Simulation.MY_DEBUG;
            return true;
        }

        private void Instance_AlarmHappened(int a)
        {
            try
            {
                if (this.lbAlarmText.InvokeRequired)
                {
                    DelVoid_Int d = new DelVoid_Int(Instance_AlarmHappened);
                    this.Invoke(d, a);
                }
                else
                {
                    string timeFormat = "yyyy-MM-dd HH:mm:ss.fff";
                    string message = string.Format("{0}  {1}", DateTime.Now.ToString(timeFormat), EqpAlarm.GetAlarmMsg(a));
                    lbAlarmText.Text = string.Format("Last Alarm    : {0}\r\nBefore Alarm : {1}", message, m_BeforeAlarmMessage);
                    m_BeforeAlarmMessage = message;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void Instance_UpdateCommandChanged(object obj)
        {
            try
            {
                if (this.listBox1.InvokeRequired)
                {
                    DelVoid_Object d = new DelVoid_Object(Instance_UpdateCommandChanged);
                    this.Invoke(d, obj);
                }
                else
                {
                    List<TransferCommand> cmds = obj as List<TransferCommand>;
                    m_UpdateCommandLists = cmds;
                    m_UpdateCommand = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void Instance_UpdatePathMapChanged(object obj)
        {
            try
            {
                if (this.dgvPathList.InvokeRequired)
                {
                    DelVoid_Object d = new DelVoid_Object(Instance_UpdatePathMapChanged);
                    this.Invoke(d, obj);
                }
                else
                {
                    m_PathLists = ObjectCopier.Clone((obj as TransferCommand).PathMaps);
                    m_UpdatePathList = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void Instance_UpdateMotionProcess(object a)
        {
            try
            {
                if (this.dgvPathList.InvokeRequired)
                {
                    DelVoid_Object d = new DelVoid_Object(Instance_UpdateMotionProcess);
                    this.Invoke(d, a);
                }
                else
                {
                    Sineva.VHL.Data.Process.Path path = a as Sineva.VHL.Data.Process.Path;
                    if (ProcessDataHandler.Instance.CurTransferCommand.IsValid)
                    {
                        if (ProcessDataHandler.Instance.CurTransferCommand.PathMaps == null) return;
                        if (path == null) m_UpdateIndex = -1;
                        else m_UpdateIndex = path.Index;
                        m_UpdateMotionProc = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public void UpdateState()
        {
            UpdatePathList();
            UpdateMotionProcess();
            UpdateCycleTime();
            UpdateCommandList();

            string alarm_code = string.Empty;
            foreach (_DevAxis axis in m_DevAxes)
            {
                if (axis.IsAlarm())
                {
                    alarm_code += axis.GetAxis().AxisName;
                    alarm_code += "_";
                    alarm_code += axis.GetAxis().AixsAlarmMsg;
                    alarm_code += "\r\n";
                }
            }
            if (alarm_code != m_BeforeAlarmCode)
            {
                m_BeforeAlarmCode = alarm_code;
                lbServoAlarmCode.Text = alarm_code;
            }
        }
        #endregion

        #region Methods
        private void InitListView(List<TransferCommand> cmds)
        {
            this.listBox1.DataSource = null;
            this.listBox1.Refresh();
            this.listBox1.DataSource = cmds;
        }
        private void btnCommandAdd_Click(object sender, EventArgs e)
        {
            m_Command = (Command)this.filteredPropertyGrid1.SelectedObject;
            if (ProcessDataHandler.Instance.TransferCommands.Select(x => x.CommandID).Contains(m_Command.CommandID))
            {
                MessageBox.Show("Same Command-ID Exist !");
            }
            else
            {
                ProcessDataHandler.Instance.CreateTransferCommand(m_Command);
            }
            ButtonLog.WriteLog(this.Text.ToString(), string.Format("btnCommandAdd_Click"));
        }
        private void btnCommandDel_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem == null) return;
            if (ProcessDataHandler.Instance.TransferCommands.Select(x => x.CommandID).Contains(m_Command.CommandID) == false)
            {
                MessageBox.Show("Same Command-ID Not Exist !");
            }
            else
            {
                Command cmd = this.listBox1.SelectedItem as Command;
                ProcessDataHandler.Instance.DeleteTransferCommand(cmd);
            }
            ButtonLog.WriteLog(this.Text.ToString(), string.Format("btnCommandDel_Click"));
        }
        private void btnCommandSave_Click(object sender, EventArgs e)
        {
            m_Command = (Command)this.filteredPropertyGrid1.SelectedObject;
            if (ProcessDataHandler.Instance.TransferCommands.Select(x => x.CommandID).Contains(m_Command.CommandID) == false)
            {
                MessageBox.Show("Same Command-ID Not Exist !");
            }
            else
            {
                ProcessDataHandler.Instance.SaveTransferCommand(m_Command);
                InitListView(ProcessDataHandler.Instance.TransferCommands);
            }
            ButtonLog.WriteLog(this.Text.ToString(), string.Format("btnCommandSave_Click"));
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem == null) return;
            m_Command = ObjectCopier.Clone(this.listBox1.SelectedItem as Command);
            ShowCyclePanel(m_Command);
        }
        private void InitPathGrid()
        {
            // Sticks Grid
            this.dgvPathList.AutoGenerateColumns = false;
            this.dgvPathList.RowHeadersVisible = false;
            this.dgvPathList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            // Columns
            this.dgvPathList.Columns.Clear();
            this.dgvPathList.ColumnCount = 12;

            int columnIndex = 0;
            this.dgvPathList.Columns[columnIndex].HeaderText = "Index";
            this.dgvPathList.Columns[columnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            columnIndex++;
            this.dgvPathList.Columns[columnIndex].HeaderText = "Proc";
            this.dgvPathList.Columns[columnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            columnIndex++;
            this.dgvPathList.Columns[columnIndex].HeaderText = "LinkID";
            this.dgvPathList.Columns[columnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            columnIndex++;
            this.dgvPathList.Columns[columnIndex].HeaderText = "FromNode";
            this.dgvPathList.Columns[columnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            columnIndex++;
            this.dgvPathList.Columns[columnIndex].HeaderText = "ToNode";
            this.dgvPathList.Columns[columnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            columnIndex++;
            this.dgvPathList.Columns[columnIndex].HeaderText = "Distance";
            this.dgvPathList.Columns[columnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            columnIndex++;
            this.dgvPathList.Columns[columnIndex].HeaderText = "Speed";
            this.dgvPathList.Columns[columnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            columnIndex++;
            this.dgvPathList.Columns[columnIndex].HeaderText = "ACC";
            this.dgvPathList.Columns[columnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            columnIndex++;
            this.dgvPathList.Columns[columnIndex].HeaderText = "DEC";
            this.dgvPathList.Columns[columnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            columnIndex++;
            this.dgvPathList.Columns[columnIndex].HeaderText = "JERK";
            this.dgvPathList.Columns[columnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            columnIndex++;
            this.dgvPathList.Columns[columnIndex].HeaderText = "Steer";
            this.dgvPathList.Columns[columnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            columnIndex++;
            this.dgvPathList.Columns[columnIndex].HeaderText = "BCR";
            this.dgvPathList.Columns[columnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            /////////////////////////////////////////////////////////
        }
        private void ShowCyclePanel(Command command)
        {
            int totalCount = 0;
            int waitTime = 0;
            bool aging = false;
            if (command.ProcessCommand == OCSCommand.CycleHoistAging)
            {
                totalCount = GV.HoistCycleTotalCount;
                waitTime = GV.HoistCycleWaitTime;
                aging = true;
            }
            else if (command.ProcessCommand == OCSCommand.CycleSteerAging)
            {
                totalCount = GV.SteerCycleTotalCount;
                waitTime = GV.SteerCycleWaitTime;
                aging = true;
            }
            else if (command.ProcessCommand == OCSCommand.CycleAntiDropAging)
            {
                totalCount = GV.AntiDropCycleTotalCount;
                waitTime = GV.AntiDropCycleWaitTime;
                aging = true;
            }
            else if (command.ProcessCommand == OCSCommand.CycleWheelMoveAging)
            {
                totalCount = GV.WheelMoveCycleTotalCount;
                waitTime = GV.WheelMoveCycleWaitTime;
                aging = true;
            }
            if (aging)
            {
                tbCycleTotalCount.Text = totalCount.ToString();
                tbCycleWaitTime.Text = waitTime.ToString();
                this.panelCycleInfo.Visible = true;
            }
            else
            {
                tbCycleTotalCount.Text = "0";
                tbCycleWaitTime.Text = "0";
                this.panelCycleInfo.Visible = false;
            }
        }
        private void UpdateCommandList()
        {
            if (m_UpdateCommand)
            {
                m_UpdateCommand = false;
                this.listBox1.DataSource = null;
                this.listBox1.Refresh();
                this.listBox1.DataSource = m_UpdateCommandLists;
            }
        }
        private void UpdatePathList()
        {
            try
            {
                if (m_UpdatePathList)
                {
                    m_UpdatePathList = false;
                    // Rows
                    this.dgvPathList.Rows.Clear();
                    int index = 0;
                    foreach (Sineva.VHL.Data.Process.Path path in m_PathLists)
                    {
                        object[] obj = new object[]
                        { index++, path.MotionProc, path.LinkID, path.FromNodeID,
                        path.ToNodeID, path.RunDistance, path.Velocity, path.Acceleration,
                        path.Deceleration, path.Jerk, path.SteerDirection, path.BcrDirection, };
                        this.dgvPathList.Rows.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void UpdateMotionProcess()
        {
            try
            {
                if (m_UpdateMotionProc)
                {
                    m_UpdateMotionProc = false;
                    if (ProcessDataHandler.Instance.CurTransferCommand.IsValid)
                    {
                        if (ProcessDataHandler.Instance.CurTransferCommand.PathMaps == null) return;
                        if (m_UpdateIndex == -1)
                        {
                            int index = 0;
                            foreach (Sineva.VHL.Data.Process.Path findPath in ProcessDataHandler.Instance.CurTransferCommand.PathMaps)
                            {
                                int col = 1;
                                dgvPathList[col, index].Value = findPath.MotionProc;
                                index++;
                            }
                        }
                        else if (m_UpdateIndex < dgvPathList.RowCount)
                        {
                            int index = m_UpdateIndex;
                            Sineva.VHL.Data.Process.Path path = ProcessDataHandler.Instance.CurTransferCommand.PathMaps.Find(x => x.Index == index);
                            if (path != null)
                            {
                                int col = 1;
                                dgvPathList[col, index].Value = path.MotionProc;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void btnMakePath_Click(object sender, EventArgs e)
        {
            TransferCommand selected_command = this.listBox1.SelectedItem as TransferCommand;
            if (selected_command == null) return;

            bool command_check = false;
            command_check |= selected_command.ProcessCommand == OCSCommand.Transfer;
            command_check |= selected_command.ProcessCommand == OCSCommand.Go;
            command_check |= selected_command.ProcessCommand == OCSCommand.Teaching;
            if (command_check == false)
            {
                MessageBox.Show("Process Command NG ! cann't make path");
                return;
            }
            bool clear_paths = true;
            bool ok = selected_command.MakeNodes(ProcessDataHandler.Instance.CurVehicleStatus, clear_paths); // PathNodes는 Full Node List 임. 
            if (ok)
            {
                ok = selected_command.MakeRouteFullPath(ProcessDataHandler.Instance.CurVehicleStatus);
            }
            if (ok == false)
            {
                MessageBox.Show("Process Command make path NG");
                return;
            }
            ButtonLog.WriteLog(this.Text.ToString(), string.Format("btnMakePath_Click"));
        }
        private void btnSetCurCommand_Click(object sender, EventArgs e)
        {
            if (ProcessDataHandler.Instance.CommandQueue.Count > 0)
            {
                TransferCommand cmd = ProcessDataHandler.Instance.CommandQueue.Peek();
                if (cmd.IsValid)
                {
                    ProcessDataHandler.Instance.CurTransferCommand = cmd;
                    SequenceLog.WriteLog("[GUI]", string.Format("Manual Command SET : {0}", cmd.CommandID));
                }
                else
                {
                    ProcessDataHandler.Instance.DeleteTransferCommand(cmd);
                }
                ButtonLog.WriteLog(this.Text.ToString(), string.Format("btnSetCurCommand_Click"));
            }
        }
        private void tbCycleInfo_TextChanged(object sender, EventArgs e)
        {
            int totalCount = 0;
            int waitTime = 0;
            if (int.TryParse(tbCycleTotalCount.Text, out totalCount))
            {
                if (m_Command.ProcessCommand == OCSCommand.CycleHoistAging) GV.HoistCycleTotalCount = totalCount;
                else if (m_Command.ProcessCommand == OCSCommand.CycleSteerAging) GV.SteerCycleTotalCount = totalCount;
                else if (m_Command.ProcessCommand == OCSCommand.CycleAntiDropAging) GV.AntiDropCycleTotalCount = totalCount;
                else if (m_Command.ProcessCommand == OCSCommand.CycleWheelMoveAging) GV.WheelMoveCycleTotalCount = totalCount;
            }
            if (int.TryParse(tbCycleWaitTime.Text, out waitTime))
            {
                if (m_Command.ProcessCommand == OCSCommand.CycleHoistAging) GV.HoistCycleWaitTime = waitTime;
                else if (m_Command.ProcessCommand == OCSCommand.CycleSteerAging) GV.SteerCycleWaitTime = waitTime;
                else if (m_Command.ProcessCommand == OCSCommand.CycleAntiDropAging) GV.AntiDropCycleWaitTime = waitTime;
                else if (m_Command.ProcessCommand == OCSCommand.CycleWheelMoveAging) GV.WheelMoveCycleWaitTime = waitTime;
            }
        }
        private void UpdateCycleTime()
        {
            try
            {
                int acquireTime = GV.AcquireCycleTime;
                int depositTime = GV.AcquireCycleTime;
                int steerChangeTime = GV.SteerCycleTime;
                int antiDropTime = GV.AntiDropCycleTime;
                if (this.tbAquireTime.Text != acquireTime.ToString()) this.tbAquireTime.Text = acquireTime.ToString();
                if (this.tbDepositTime.Text != depositTime.ToString()) this.tbDepositTime.Text = depositTime.ToString();
                if (this.tbSteerChangeTime.Text != steerChangeTime.ToString()) this.tbSteerChangeTime.Text = steerChangeTime.ToString();
                if (this.tbAntiDropTime.Text != antiDropTime.ToString()) this.tbAntiDropTime.Text = antiDropTime.ToString();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion

        private void btnCancelTest_Click(object sender, EventArgs e)
        {
            OCSCommManager.Instance.OcsStatus.SetFlag(InterfaceFlag.WaitCancelRequest, FlagValue.ON);
        }

        private void cbb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            enSimulationFlag flag = enSimulationFlag.None;

            if (checkBox.Name == "cbbUpRangeOver_Acquire") flag = enSimulationFlag.UpRangeOver_Acquire;
            else if (checkBox.Name == "cbbFoupSingleDetect_Acquire") flag = enSimulationFlag.FoupSingleDetect_Acquire;
            else if (checkBox.Name == "cbbES_Signal_Off_before_Acquire") flag = enSimulationFlag.ES_Signal_Off_Before_Acquire;
            else if (checkBox.Name == "cbbES_Signal_Off_Acquiring") flag = enSimulationFlag.ES_Signal_Off_Acquiring;
            else if (checkBox.Name == "cbbHO_Signal_Off_before_Acquire") flag = enSimulationFlag.HO_Signal_Off_Before_Acquire;
            else if (checkBox.Name == "cbbHO_Signal_Off_Acquiring") flag = enSimulationFlag.HO_Signal_Off_Acquiring;
            else if (checkBox.Name == "cbbHoistSwing_Acquire") flag = enSimulationFlag.Hoist_Swing_Interlock_Acquire;
            else if (checkBox.Name == "cbbUpRangeOver_Deposit") flag = enSimulationFlag.UpRangeOver_Deposit;
            else if (checkBox.Name == "cbbFoupSingleDetect_Deposit") flag = enSimulationFlag.FoupSingleDetect_Deposit;
            else if (checkBox.Name == "cbbES_Signal_Off_before_Deposit") flag = enSimulationFlag.ES_Signal_Off_Before_Deposit;
            else if (checkBox.Name == "cbbES_Signal_Off_Depositing") flag = enSimulationFlag.ES_Signal_Off_Depositing;
            else if (checkBox.Name == "cbbHO_Signal_Off_before_Deposit") flag = enSimulationFlag.HO_Signal_Off_Before_Deposit;
            else if (checkBox.Name == "cbbHO_Signal_Off_Depositing") flag = enSimulationFlag.HO_Signal_Off_Depositing;
            else if (checkBox.Name == "cbbHoistSwing_Deposit") flag = enSimulationFlag.Hoist_Swing_Interlock_Deposit;
            else if (checkBox.Name == "cbbTA1_Acquire") flag = enSimulationFlag.PIO_TA1_Acquire;
            else if (checkBox.Name == "cbbTA2_Acquire") flag = enSimulationFlag.PIO_TA2_Acquire;
            else if (checkBox.Name == "cbbTA3_Acquire") flag = enSimulationFlag.PIO_TA3_Acquire;
            else if (checkBox.Name == "cbbTA1_Deposit") flag = enSimulationFlag.PIO_TA1_Deposit;
            else if (checkBox.Name == "cbbTA2_Deposit") flag = enSimulationFlag.PIO_TA2_Deposit;
            else if (checkBox.Name == "cbbTP3_Deposit") flag = enSimulationFlag.PIO_TP3_Deposit;
            else if (checkBox.Name == "cbbTA3_Deposit") flag = enSimulationFlag.PIO_TA3_Deposit;
            else if (checkBox.Name == "cbbLimitDetect_Acquire") flag = enSimulationFlag.HoistLimitDetect_Acquire;
            else if (checkBox.Name == "cbbLimitDetect_Deposit") flag = enSimulationFlag.HoistLimitDetect_Deposit;
            else if (checkBox.Name == "cbbRangeOver_Acquire") flag = enSimulationFlag.InRangeOver_Acquire;
            else if (checkBox.Name == "cbbRangeOver_Deposit") flag = enSimulationFlag.InRangeOver_Deposit;


            if (checkBox.Checked) GV.SimulationFlag |= flag;
            else GV.SimulationFlag &= ~flag;
        }
    }
}
