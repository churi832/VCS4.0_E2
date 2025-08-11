using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data.LogIn;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using Sineva.VHL.Library.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static Sineva.VHL.Device.ucManualTeachingView;

namespace Sineva.VHL.Device
{
    public partial class ucManualTeachingView : UCon, IUpdateUCon
    {
        public class CurrentPositionInfo
        {
            public double CurrentBarcodeLeft { get; set; }
            public double CurrentBarcodeRight { get; set; }
            public double HoistCurrentPos { get; set; }
            public double SlideCurrentPos { get; set; }
            public double RotateCurrentPos { get; set; }
        }
        public class PositionOffsetInfo
        {
            public double BarcodeLeftOffset { get; set; }
            public double BarcodeRightOffset { get; set; }
            public double HoistPosOffset { get; set; }
            public double SlidePosOffset { get; set; }
            public double RotatePosOffset { get; set; }
        }
        public enum UpdateTeachingPosition
        {
            None,
            Load,
            Unload,
            All,
        }

        #region Fields
        private Dictionary<int, DataItem_Port> m_PortDictory = new Dictionary<int, DataItem_Port>();
        private BindingList<DataItem_Port> m_Ports = new BindingList<DataItem_Port>();
        private CurrentPositionInfo m_CurrentPositionInfo = new CurrentPositionInfo();
        private PositionOffsetInfo m_PositionOffsetInfo = new PositionOffsetInfo();
        private ServoManager m_ServoManager;
        private AuthorizationLevel m_CurUserLevel = AuthorizationLevel.Operator;
        private PortType m_PortType;
        private DevGripperPIO m_devGripperPio = null;
        private enAxisCoord m_currentAxisCoord = enAxisCoord.X;
        private int m_SelectPortID = 0;
        private int m_prePortID = 0;
        private DevTransfer m_devTransfer = null;
        #endregion

        #region Constructor
        public ucManualTeachingView() : base(OperateMode.Manual)
        {
            InitializeComponent();
        }
        #endregion

        #region Methods - Interface
        public bool Initialize()
        {
            bool rv = true;
            try
            {
                AccountManager.Instance.AccountLogInOut += Instance_AccountLogInOut;

                m_ServoManager = ServoManager.Instance;
                PortListInitialize();
                TeachingDataGridInitialize();
                AxesStatusGridInitialize();
                CurrentPositionGridInitialize();
                PositionOffsetGridInitialize();
                SetAuthority(AuthorizationLevel.Operator);
                m_devGripperPio = DevicesManager.Instance.DevGripperPIO;
                m_devTransfer = DevicesManager.Instance.DevTransfer;
                if (m_devGripperPio.IsValid)
                {
                    this.ucLedO1.LedIOTag = m_devGripperPio.DiHoistUp.Di;
                    this.ucLedO2.LedIOTag = m_devGripperPio.DiHoistLimit.Di;
                    this.ucLedO3.LedIOTag = m_devGripperPio.DiHoistHome.Di;
                    this.ucLedO4.LedIOTag = m_devGripperPio.DiLeftProductExist.Di;
                    this.ucLedO5.LedIOTag = m_devGripperPio.DiRightProductExist.Di;
                }
                else
                {
                    this.ucLedO1.Visible = false; label2.Visible = false;
                    this.ucLedO2.Visible = false; label3.Visible = false;
                    this.ucLedO3.Visible = false; label4.Visible = false;
                    this.ucLedO4.Visible = false; label5.Visible = false;
                    this.ucLedO5.Visible = false; label7.Visible = false;
                }

                btnBcrSet.Visible = AppConfig.Instance.Simulation.MY_DEBUG;
                if (!DevicesManager.Instance.DevFoupGripper.AxisHoist.IsValid)
                {
                    tbpHoist.Parent = null;
                }
                if (!DevicesManager.Instance.DevFoupGripper.AxisSlide.IsValid)
                {
                    tbpSlide.Parent = null;
                }
                if (!DevicesManager.Instance.DevFoupGripper.AxisTurn.IsValid)
                {
                    tbpRotate.Parent = null;
                }
            }
            catch (Exception ex)
            {
                string log = string.Format("UserControl : [{0}]-[{1}]\n{2}", this.GetType(), this.Name, ex.Message);
                ExceptionLog.WriteLog(log);

                rv = false;
            }
            return rv;
        }

        public void UpdateState()
        {
            if (this.Visible == false) return;
            if (m_ServoManager == null) return;
            try
            {
                UpdateAxesStatus(m_ServoManager.AxisSource);
                if (string.IsNullOrEmpty(tbPortID.Text))
                {
                    var portID = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortID;
                    if (m_prePortID != portID)
                    {
                        int selectIndex = m_PortDictory.Select(x => x.Key).ToList().IndexOf(portID);
                        ltBoxPortList.SelectedIndex = selectIndex;
                        m_prePortID = portID;
                    }
                }
            }
            catch (Exception ex)
            {
                string log = string.Format("UserControl : [{0}]-[{1}]\n{2}", this.GetType(), this.Name, ex.Message);
                ExceptionLog.WriteLog(log);
            }
        }

        private void Instance_AccountLogInOut(bool logIn, Data.DbAdapter.DataItem_UserInfo account)
        {
            try
            {
                if (logIn)
                {
                    SetAuthority(AccountManager.Instance.CurAccount.Level);
                }
                else
                {
                    SetAuthority(AuthorizationLevel.Operator);
                }
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        #endregion

        #region Methods
        private void PortListInitialize()
        {
            m_PortDictory = DatabaseHandler.Instance.DictionaryPortDataList;
            //int selectIndex = m_PortDictory.Select(x => x.Key).ToList().IndexOf(m_SelectPortID);
            ltBoxPortList.DataSource = m_PortDictory.Select(x => x.Key).Where(x => x.ToString().Contains(tbPortID.Text)).ToList();
            //ltBoxPortList.SelectedIndex = selectIndex;
        }
        private void TeachingDataGridInitialize()
        {
            dgvTeachingData.AutoGenerateColumns = false;
            dgvTeachingData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTeachingData.AllowUserToAddRows = false;
            dgvTeachingData.AllowUserToDeleteRows = false;
            dgvTeachingData.AllowUserToResizeRows = false;
            dgvTeachingData.MultiSelect = false;
            dgvTeachingData.ReadOnly = true;
            dgvTeachingData.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "LeftBCR", DataPropertyName = "BarcodeLeft", Width = 65 });
            dgvTeachingData.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "RightBCR", DataPropertyName = "BarcodeRight", Width = 65 });
            dgvTeachingData.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "HoistPos", DataPropertyName = "HoistPosition", Width = 65 });
            dgvTeachingData.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "SlidePos", DataPropertyName = "SlidePosition", Width = 65 });
            dgvTeachingData.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "RotatePos", DataPropertyName = "RotatePosition", Width = 65 });
            dgvTeachingData.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "UnloadHoistPos", DataPropertyName = "UnloadHoistPosition" });
            dgvTeachingData.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "UnloadSlidePos", DataPropertyName = "UnloadSlidePosition" });
            dgvTeachingData.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "UnloadRotatePos", DataPropertyName = "UnloadRotatePosition" });
            dgvTeachingData.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "LeftOffset", DataPropertyName = "DriveLeftOffset", Width = 65 });
            dgvTeachingData.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "RightOffset", DataPropertyName = "DriveRightOffset", Width = 65 });
            dgvTeachingData.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "SlideOffset", DataPropertyName = "SlideOffset", Width = 65 });
            dgvTeachingData.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "HoistOffset", DataPropertyName = "HoistOffset", Width = 65 });
            dgvTeachingData.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "RotateOffset", DataPropertyName = "RotateOffset", Width = 65 });
            dgvTeachingData.DataSource = m_Ports;
        }
        private void AxesStatusGridInitialize()
        {
            dgvAxesStatus.AutoGenerateColumns = false;
            dgvAxesStatus.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvAxesStatus.AllowUserToAddRows = false;
            dgvAxesStatus.AllowUserToDeleteRows = false;
            dgvAxesStatus.AllowUserToResizeRows = false;
            dgvAxesStatus.MultiSelect = false;
            dgvAxesStatus.ReadOnly = true;
            dgvAxesStatus.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "ID", DataPropertyName = "AxisId" });
            dgvAxesStatus.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Name", DataPropertyName = "AxisName" });
            dgvAxesStatus.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Pos", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvAxesStatus.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Speed" });
            dgvAxesStatus.Columns.Add(new DataGridViewImageColumn() { HeaderText = "Alarm" });
            dgvAxesStatus.Columns.Add(new DataGridViewImageColumn() { HeaderText = "In Pos" });
            dgvAxesStatus.Columns.Add(new DataGridViewImageColumn() { HeaderText = "Home End" });
            dgvAxesStatus.Columns.Add(new DataGridViewImageColumn() { HeaderText = "Servo On" });
            dgvAxesStatus.DataSource = m_ServoManager.AxisSource;
        }
        private void CurrentPositionGridInitialize()
        {
            dgvCurrentPosition.AutoGenerateColumns = false;
            dgvCurrentPosition.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCurrentPosition.AllowUserToAddRows = false;
            dgvCurrentPosition.AllowUserToDeleteRows = false;
            dgvCurrentPosition.AllowUserToResizeRows = false;
            dgvCurrentPosition.MultiSelect = false;
            dgvCurrentPosition.ReadOnly = true;
            dgvCurrentPosition.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "CurrentBarcodeLeft", DataPropertyName = "CurrentBarcodeLeft", Width = 150 });
            dgvCurrentPosition.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "CurrentBarcodeRight", DataPropertyName = "CurrentBarcodeRight", Width = 150 });
            dgvCurrentPosition.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "HoistCurrentPosition", DataPropertyName = "HoistCurrentPos", Width = 150 });
            dgvCurrentPosition.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "SlideCurrentPosition", DataPropertyName = "SlideCurrentPos", Width = 150 });
            dgvCurrentPosition.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "RotateCurrentPosition", DataPropertyName = "RotateCurrentPos", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvCurrentPosition.RowCount = 1;
        }
        private void PositionOffsetGridInitialize()
        {
            dgvOffset.AutoGenerateColumns = false;
            dgvOffset.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOffset.AllowUserToAddRows = false;
            dgvOffset.AllowUserToDeleteRows = false;
            dgvOffset.AllowUserToResizeRows = false;
            dgvOffset.MultiSelect = false;
            dgvOffset.ReadOnly = true;
            dgvOffset.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "BarcodeLeftOffset", DataPropertyName = "BarcodeLeftOffset", Width = 150 });
            dgvOffset.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "BarcodeRightOffset", DataPropertyName = "BarcodeRightOffset", Width = 150 });
            dgvOffset.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "HoistPosOffset", DataPropertyName = "HoistPosOffset", Width = 150 });
            dgvOffset.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "SlidePosOffset", DataPropertyName = "SlidePosOffset", Width = 150 });
            dgvOffset.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "RotatePosOffset", DataPropertyName = "RotatePosOffset", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvOffset.RowCount = 1;
        }
        private void UpdateCurrentPosition()
        {
            try
            {
                int rowIndex = dgvCurrentPosition.RowCount;
                if (rowIndex > 0)
                {
                    dgvCurrentPosition[0, rowIndex - 1].Value = m_CurrentPositionInfo.CurrentBarcodeLeft;
                    dgvCurrentPosition[1, rowIndex - 1].Value = m_CurrentPositionInfo.CurrentBarcodeRight;
                    dgvCurrentPosition[2, rowIndex - 1].Value = m_CurrentPositionInfo.HoistCurrentPos;
                    dgvCurrentPosition[3, rowIndex - 1].Value = m_CurrentPositionInfo.SlideCurrentPos;
                    dgvCurrentPosition[4, rowIndex - 1].Value = m_CurrentPositionInfo.RotateCurrentPos;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void UpdateOffsetPosition()
        {
            try
            {
                int rowIndex = dgvOffset.RowCount;
                if (rowIndex > 0)
                {
                    dgvOffset[0, rowIndex - 1].Value = m_PositionOffsetInfo.BarcodeLeftOffset;
                    dgvOffset[1, rowIndex - 1].Value = m_PositionOffsetInfo.BarcodeRightOffset;
                    dgvOffset[2, rowIndex - 1].Value = m_PositionOffsetInfo.HoistPosOffset;
                    dgvOffset[3, rowIndex - 1].Value = m_PositionOffsetInfo.SlidePosOffset;
                    dgvOffset[4, rowIndex - 1].Value = m_PositionOffsetInfo.RotatePosOffset;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        #endregion

        #region Method
        private void LoadTeachingData(int portID)
        {
            try
            {
                if (m_PortDictory.ContainsKey(portID))
                {
                    m_PortType = m_PortDictory[portID].PortType;
                    m_Ports.Clear();
                    m_Ports.Add(m_PortDictory[portID]);
                    dgvTeachingData.Refresh();
                    UpdateBtnVisiable(m_PortType);
                }
            }
            catch (Exception ex)
            {
                string log = string.Format("UserControl : [{0}]-[{1}]\n{2}", this.GetType(), this.Name, ex.Message);
                ExceptionLog.WriteLog(log);
            }
        }
        private void UpdateAxesStatus(List<_Axis> axes)
        {
            try
            {
                for (int i = 0; i < axes.Count; i++)
                {
                    bool HomeEnd = true;
                    bool InPos = true;
                    bool Alarm = false;
                    ushort decimalPoint = axes[i].DecimalPoint;
                    double _curpos = (axes[i] as IAxisCommand).GetAxisCurPos();
                    string posText = decimalPoint <= 4 ? string.Format("{0:F4}", _curpos) : decimalPoint <= 5 ? string.Format("{0:F5}", _curpos) : string.Format("{0:F6}", _curpos);
                    string speedText = (axes[i] as IAxisCommand).GetAxisCurSpeed().ToString("F4");
                    this.dgvAxesStatus[2, i].Value = posText;
                    this.dgvAxesStatus[3, i].Value = speedText;

                    int j = 4;
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
                    decimalPoint = 1;
                    switch (axes[i].AxisCoord)
                    {
                        case enAxisCoord.X:
                            double leftBcr = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                            double rightBcr = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                            if (m_Ports.Count > 0)
                            {
                                m_PositionOffsetInfo.BarcodeLeftOffset = Math.Round(leftBcr - m_Ports[0].BarcodeLeft - m_Ports[0].DriveLeftOffset, decimalPoint);
                                m_PositionOffsetInfo.BarcodeRightOffset = Math.Round(rightBcr - m_Ports[0].BarcodeRight - m_Ports[0].DriveRightOffset, decimalPoint);
                            }
                            m_CurrentPositionInfo.CurrentBarcodeLeft = leftBcr;
                            m_CurrentPositionInfo.CurrentBarcodeRight = rightBcr;
                            break;
                        case enAxisCoord.Y:
                            m_CurrentPositionInfo.SlideCurrentPos = Math.Round(_curpos, decimalPoint);
                            if (m_Ports.Count > 0)
                            {
                                m_PositionOffsetInfo.SlidePosOffset = Math.Round(_curpos, decimalPoint) - m_Ports[0].SlidePosition - m_Ports[0].SlideOffset;
                            }
                            break;
                        case enAxisCoord.Z:
                            m_CurrentPositionInfo.HoistCurrentPos = Math.Round(_curpos, decimalPoint);
                            if (m_Ports.Count > 0)
                            {
                                m_PositionOffsetInfo.HoistPosOffset = Math.Round(_curpos, decimalPoint) - m_Ports[0].HoistPosition - m_Ports[0].HoistOffset;
                            }
                            break;
                        case enAxisCoord.T:
                            m_CurrentPositionInfo.RotateCurrentPos = Math.Round(_curpos, decimalPoint);
                            if (m_Ports.Count > 0)
                            {
                                m_PositionOffsetInfo.RotatePosOffset = Math.Round(_curpos, decimalPoint) - m_Ports[0].RotatePosition - m_Ports[0].RotateOffset;
                            }
                            break;
                    }
                }
                UpdateCurrentPosition();
                UpdateOffsetPosition();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void SearchPortListByPortID(string portID)
        {
            ltBoxPortList.DataSource = m_PortDictory.Select(x => x.Key).Where(x => x.ToString().Contains(portID)).ToList();
        }

        public void SetAuthority(AuthorizationLevel level)
        {

            bool manual_key = EqpStateManager.Instance.OpMode == OperateMode.Auto ? false : true;
            m_CurUserLevel = level;
            btnJogT_minus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnJogT_plus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnJogX_minus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnJogX_plus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnJogY_minus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnJogY_plus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnJogZ_minus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
            btnJogZ_plus.Enabled = level < AuthorizationLevel.Maintenance && manual_key;
        }
        private void ResettingOffset(int portID)
        {
            try
            {
                if (!m_PortDictory.ContainsKey(portID))
                {
                    MessageBox.Show($"PortID {portID} not exist!");
                    return;
                }
                DataItem_Port newTeachingData = m_PortDictory[portID];
                newTeachingData.DriveLeftOffset = 0.0f;
                newTeachingData.DriveRightOffset = 0.0f;
                newTeachingData.HoistOffset = 0.0f;
                newTeachingData.SlideOffset = 0.0f;
                newTeachingData.RotateOffset = 0.0f;
                if (DatabaseHandler.Instance.UpdatePort(newTeachingData))
                {
                    PortListInitialize();
                    MessageBox.Show($"PortID {portID} resetting offset success!");
                }

                ButtonLog.WriteLog(this.Name.ToString(), "ResettingOffset");
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void UpdateTeachingDataToDataBase(int portID, UpdateTeachingPosition updateTeachingPosition, bool isOffset)
        {
            try
            {
                if (!m_PortDictory.ContainsKey(portID))
                {
                    MessageBox.Show($"PortID {portID} not exist!");
                    return;
                }
                DataItem_Port newTeachingData = m_PortDictory[portID];
                switch (updateTeachingPosition)
                {
                    case UpdateTeachingPosition.None:
                        MessageBox.Show($"Please choose load or unload position to update!");
                        return;
                    case UpdateTeachingPosition.Load:
                        if (isOffset == true)
                        {
                            //newTeachingData.DriveLeftOffset = double.Parse(dgvOffset.Rows[0].Cells[0].Value.ToString()) + newTeachingData.DriveLeftOffset;
                            //newTeachingData.DriveRightOffset = double.Parse(dgvOffset.Rows[0].Cells[1].Value.ToString()) + newTeachingData.DriveRightOffset;
                            //newTeachingData.HoistOffset = double.Parse(dgvOffset.Rows[0].Cells[2].Value.ToString()) + newTeachingData.HoistOffset;
                            //newTeachingData.SlideOffset = double.Parse(dgvOffset.Rows[0].Cells[3].Value.ToString()) + newTeachingData.SlideOffset;
                            //newTeachingData.RotateOffset = double.Parse(dgvOffset.Rows[0].Cells[4].Value.ToString()) + newTeachingData.RotateOffset;
                            if (m_Ports.Count > 0)
                            {
                                newTeachingData.DriveLeftOffset = m_CurrentPositionInfo.CurrentBarcodeLeft - m_Ports[0].BarcodeLeft;
                                newTeachingData.DriveRightOffset = m_CurrentPositionInfo.CurrentBarcodeRight - m_Ports[0].BarcodeRight;
                                newTeachingData.SlideOffset = m_CurrentPositionInfo.SlideCurrentPos - m_Ports[0].SlidePosition;
                                newTeachingData.HoistOffset = m_CurrentPositionInfo.HoistCurrentPos - m_Ports[0].HoistPosition;
                                newTeachingData.RotateOffset = m_CurrentPositionInfo.RotateCurrentPos - m_Ports[0].RotatePosition;
                            }
                        }
                        else
                        {
                            newTeachingData.BarcodeLeft = double.Parse(dgvCurrentPosition.Rows[0].Cells[0].Value.ToString());
                            newTeachingData.BarcodeRight = double.Parse(dgvCurrentPosition.Rows[0].Cells[1].Value.ToString());
                            newTeachingData.HoistPosition = double.Parse(dgvCurrentPosition.Rows[0].Cells[2].Value.ToString());
                            newTeachingData.SlidePosition = double.Parse(dgvCurrentPosition.Rows[0].Cells[3].Value.ToString());
                            newTeachingData.RotatePosition = double.Parse(dgvCurrentPosition.Rows[0].Cells[4].Value.ToString());
                        }
                        break;
                    case UpdateTeachingPosition.Unload:
                        if (isOffset == true)
                        {
                            //newTeachingData.DriveLeftOffset = double.Parse(dgvOffset.Rows[0].Cells[0].Value.ToString()) + newTeachingData.DriveLeftOffset;
                            //newTeachingData.DriveRightOffset = double.Parse(dgvOffset.Rows[0].Cells[1].Value.ToString()) + newTeachingData.DriveRightOffset;
                            //newTeachingData.HoistOffset = double.Parse(dgvOffset.Rows[0].Cells[2].Value.ToString()) + newTeachingData.HoistOffset;
                            //newTeachingData.SlideOffset = double.Parse(dgvOffset.Rows[0].Cells[3].Value.ToString()) + newTeachingData.SlideOffset;
                            //newTeachingData.RotateOffset = double.Parse(dgvOffset.Rows[0].Cells[4].Value.ToString()) + newTeachingData.RotateOffset;
                            if (m_Ports.Count > 0)
                            {
                                newTeachingData.DriveLeftOffset = m_CurrentPositionInfo.CurrentBarcodeLeft - m_Ports[0].BarcodeLeft;
                                newTeachingData.DriveRightOffset = m_CurrentPositionInfo.CurrentBarcodeRight - m_Ports[0].BarcodeRight;
                                newTeachingData.SlideOffset = m_CurrentPositionInfo.SlideCurrentPos - m_Ports[0].SlidePosition;
                                newTeachingData.HoistOffset = m_CurrentPositionInfo.HoistCurrentPos - m_Ports[0].HoistPosition;
                                newTeachingData.RotateOffset = m_CurrentPositionInfo.RotateCurrentPos - m_Ports[0].RotatePosition;
                            }
                        }
                        else
                        {
                            newTeachingData.BarcodeLeft = double.Parse(dgvCurrentPosition.Rows[0].Cells[0].Value.ToString());
                            newTeachingData.BarcodeRight = double.Parse(dgvCurrentPosition.Rows[0].Cells[1].Value.ToString());
                            newTeachingData.UnloadHoistPosition = double.Parse(dgvCurrentPosition.Rows[0].Cells[2].Value.ToString());
                            newTeachingData.UnloadSlidePosition = double.Parse(dgvCurrentPosition.Rows[0].Cells[3].Value.ToString());
                            newTeachingData.UnloadRotatePosition = double.Parse(dgvCurrentPosition.Rows[0].Cells[4].Value.ToString());
                        }
                        break;
                    case UpdateTeachingPosition.All:
                        if (isOffset == true)
                        {
                            //newTeachingData.DriveLeftOffset = double.Parse(dgvOffset.Rows[0].Cells[0].Value.ToString()) + newTeachingData.DriveLeftOffset;
                            //newTeachingData.DriveRightOffset = double.Parse(dgvOffset.Rows[0].Cells[1].Value.ToString()) + newTeachingData.DriveRightOffset;
                            //newTeachingData.HoistOffset = double.Parse(dgvOffset.Rows[0].Cells[2].Value.ToString()) + newTeachingData.HoistOffset;
                            //newTeachingData.SlideOffset = double.Parse(dgvOffset.Rows[0].Cells[3].Value.ToString()) + newTeachingData.SlideOffset;
                            //newTeachingData.RotateOffset = double.Parse(dgvOffset.Rows[0].Cells[4].Value.ToString()) + newTeachingData.RotateOffset;
                            if (m_Ports.Count > 0)
                            {
                                newTeachingData.DriveLeftOffset = m_CurrentPositionInfo.CurrentBarcodeLeft - m_Ports[0].BarcodeLeft;
                                newTeachingData.DriveRightOffset = m_CurrentPositionInfo.CurrentBarcodeRight - m_Ports[0].BarcodeRight;
                                newTeachingData.SlideOffset = m_CurrentPositionInfo.SlideCurrentPos - m_Ports[0].SlidePosition;
                                newTeachingData.HoistOffset = m_CurrentPositionInfo.HoistCurrentPos - m_Ports[0].HoistPosition;
                                newTeachingData.RotateOffset = m_CurrentPositionInfo.RotateCurrentPos - m_Ports[0].RotatePosition;
                            }
                        }
                        else
                        {
                            newTeachingData.BarcodeLeft = double.Parse(dgvCurrentPosition.Rows[0].Cells[0].Value.ToString());
                            newTeachingData.BarcodeRight = double.Parse(dgvCurrentPosition.Rows[0].Cells[1].Value.ToString());
                            newTeachingData.HoistPosition = double.Parse(dgvCurrentPosition.Rows[0].Cells[2].Value.ToString());
                            newTeachingData.SlidePosition = double.Parse(dgvCurrentPosition.Rows[0].Cells[3].Value.ToString());
                            newTeachingData.RotatePosition = double.Parse(dgvCurrentPosition.Rows[0].Cells[4].Value.ToString());
                            newTeachingData.UnloadHoistPosition = double.Parse(dgvCurrentPosition.Rows[0].Cells[2].Value.ToString());
                            newTeachingData.UnloadSlidePosition = double.Parse(dgvCurrentPosition.Rows[0].Cells[3].Value.ToString());
                            newTeachingData.UnloadRotatePosition = double.Parse(dgvCurrentPosition.Rows[0].Cells[4].Value.ToString());
                        }
                        break;
                    default:
                        return;
                }
                if (DatabaseHandler.Instance.UpdatePort(newTeachingData))
                {
                    PortListInitialize();
                    MessageBox.Show("Save to DataBase success!");
                }
                else
                {
                    MessageBox.Show("Save to DataBase fail!");
                }

                ButtonLog.WriteLog(this.Name.ToString(), string.Format("UpdateTeachingDataToDataBase : portID={0}, updateTeachingPosition={1}, isOffset={2}",
                    portID, updateTeachingPosition, isOffset));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private double GetTargetPosition(enAxisCoord axisCoord, bool before_down_position)
        {
            double targetPosition = 0.0;
            try
            {
                if (ltBoxPortList.SelectedItem == null)
                {
                    MessageBox.Show("Please choose one Port!");
                    return 0.0;
                }
                int portID = int.Parse(ltBoxPortList.SelectedItem.ToString());
                if (!m_PortDictory.ContainsKey(portID))
                {
                    return 0.0;
                }
                DataItem_Port teachingData = m_PortDictory[portID];
                switch (axisCoord)
                {
                    case enAxisCoord.X:
                        break;
                    case enAxisCoord.Y:
                        if (rbLoad.Checked)
                        {
                            targetPosition = teachingData.SlidePosition + teachingData.SlideOffset;
                        }
                        else
                        {
                            targetPosition = teachingData.UnloadSlidePosition + teachingData.SlideOffset;
                        }
                        break;
                    case enAxisCoord.Z:
                        if (rbLoad.Checked)
                        {
                            if (before_down_position)
                                targetPosition = teachingData.BeforeHoistPosition + teachingData.HoistOffset;
                            else targetPosition = teachingData.HoistPosition + teachingData.HoistOffset;
                        }
                        else
                        {
                            if (before_down_position)
                                targetPosition = teachingData.BeforeUnloadHoistPosition + teachingData.HoistOffset;
                            else targetPosition = teachingData.UnloadHoistPosition + teachingData.HoistOffset;
                        }
                        break;
                    case enAxisCoord.T:
                        if (rbLoad.Checked)
                        {
                            targetPosition = teachingData.RotatePosition + teachingData.RotateOffset;
                        }
                        else
                        {
                            targetPosition = teachingData.UnloadRotatePosition + teachingData.RotateOffset;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return targetPosition;
        }
        private void StopAxis(enAxisCoord axisCoord)
        {
            try
            {
                ServoUnit currentServoUnit = null;
                _Axis currentAxis = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == axisCoord)
                    {
                        if (axis.CommandSkip) continue;
                        currentAxis = axis;
                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                enAxisResult rv = currentServoUnit.Stop(currentAxis);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void MoveToPosition(enAxisCoord axisCoord, double velocity, bool isHome, bool isBeforDown = false)
        {
            try
            {
                ServoUnit currentServoUnit = null;
                _Axis currentAxis = null;
                bool isSafe = true;

                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == axisCoord)
                    {
                        if (axis.CommandSkip) continue;
                        currentAxis = axis;
                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                var targetPosition = isHome ? 0.0 : GetTargetPosition(axisCoord, isBeforDown);
                isSafe &= InterlockManager.IsSafe(currentAxis, targetPosition);

                if (isSafe == false)
                {
                    MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                if (currentAxis.CommandSkip) return;

                enAxisResult rv = enAxisResult.None;

                VelSet set = currentServoUnit.GetVel(currentAxis, DevicesManager.Instance.DevFoupGripper.TeachingVelocityLow.PropId);
                set.Vel = velocity;
                if (MessageBox.Show($"Do you want to move {currentAxis.AxisName} to teaching position {targetPosition} ?", "Move To Teaching Position", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
                rv = currentServoUnit.MoveAxisStart(currentAxis, targetPosition, set);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void UpdateBtnVisiable(PortType portType)
        {
            try
            {
                if (portType == PortType.LeftTeachingStation)
                {
                    btnUpdateOffset.Text = "Update Left Buffer Offset";
                    btnUpdateOffset.Visible = true;
                }
                else if (portType == PortType.RightTeachingStation)
                {
                    btnUpdateOffset.Text = "Update Right Buffer Offset";
                    btnUpdateOffset.Visible = true;
                }
                else
                {
                    btnUpdateOffset.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion

        #region Windows Control Event Methods
        private void ltBoxPortList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ltBoxPortList.SelectedItem != null)
                {
                    lbUnitSelect.Text = $"Select Port  :{ltBoxPortList.SelectedItem.ToString()}";
                    int portId = int.Parse(ltBoxPortList.SelectedItem.ToString());
                    LoadTeachingData(portId);
                    m_SelectPortID = portId;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void btnUpdateOffset_Click(object sender, EventArgs e)
        {
            try
            {
                ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnUpdateOffset_Click"));
                if (cbReferenceVehicle.Checked)
                {
                    MessageBox.Show("Reference Vehicle no need update offset!");
                    return;
                }
                string msg = "Left Buffer";
                List<DataItem_Port> updatePorts = new List<DataItem_Port>();
                switch (m_PortType)
                {
                    case PortType.LeftTeachingStation:
                        updatePorts = m_PortDictory.Where(x => x.Value.PortType == PortType.LeftBuffer).Select(x => x.Value).ToList();
                        msg = "Left Buffer";
                        break;
                    case PortType.RightTeachingStation:
                        updatePorts = m_PortDictory.Where(x => x.Value.PortType == PortType.RightBuffer).Select(x => x.Value).ToList();
                        msg = "Right Buffer";
                        break;
                    default:
                        MessageBox.Show("Port Type is not teaching station!");
                        return;
                }

                if (MessageBox.Show($"Do you want to save current offset to all of {msg},Count : {updatePorts.Count}", "Save to Offset", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
                List<int> Result = new List<int>();
                foreach (var port in updatePorts)
                {
                    port.DriveLeftOffset = double.Parse(dgvOffset.Rows[0].Cells[0].Value.ToString()) + port.DriveLeftOffset;
                    port.DriveRightOffset = double.Parse(dgvOffset.Rows[0].Cells[1].Value.ToString()) + port.DriveRightOffset;
                    port.HoistOffset = double.Parse(dgvOffset.Rows[0].Cells[2].Value.ToString()) + port.HoistOffset;
                    port.SlideOffset = double.Parse(dgvOffset.Rows[0].Cells[3].Value.ToString()) + port.SlideOffset;
                    port.RotateOffset = double.Parse(dgvCurrentPosition.Rows[0].Cells[4].Value.ToString()) + port.RotateOffset;
                    if (DatabaseHandler.Instance.UpdatePort(port))
                    {
                        Result.Add(port.PortID);
                    }
                }
                MessageBox.Show($"Update {msg} Complete! Count:{Result.Count},All count:{updatePorts.Count}");
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void tbPortID_TextChanged(object sender, EventArgs e)
        {
            SearchPortListByPortID(tbPortID.Text);
        }

        #region Manual Jog
        private void btnJogX_minus_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                bool ValueOk = true;
                _Axis currentAxis = null;
                ServoUnit currentServoUnit = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.X && axis.GantryType && axis.AxisName == m_devTransfer.AxisMaster.AxisName)
                    {
                        if (axis.CommandSkip) continue;

                        if (ValueOk &= double.TryParse(tbMasterVelocity.Text, out double masterVelocity))
                        {
                            axis.JogSpeed = (float)masterVelocity;
                        }
                        if (rbMasterStepMove.Checked == true)
                        {
                            if (ValueOk &= double.TryParse(tbMasterDistance.Text, out double masterDistance)) axis.JogDistance = (float)masterDistance;
                        }
                        currentAxis = axis;

                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (!ValueOk)
                {
                    string msg = "Please enter velocity";
                    if (rbMasterStepMove.Checked == true)
                    {
                        msg = "Please enter velocity and Distance";
                    }
                    MessageBox.Show(msg);
                    return;
                }

                enAxisResult rv = enAxisResult.None;
                bool isSafe = true;
                // Check Interlock 
                if (rbMasterStepMove.Checked == true)
                {
                    if (currentAxis.CommandSkip) return;
                    isSafe &= InterlockManager.IsSafe(currentAxis, currentAxis.JogDistance * -1.0 + (currentAxis as IAxisCommand).GetAxisCurPos());
                }

                if (isSafe == false)
                {
                    MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                VelSet set = new VelSet
                {
                    AxisCoord = currentAxis.AxisCoord,
                    Vel = currentAxis.JogSpeed,
                    Acc = currentAxis.JogAcc > 0.0f ? currentAxis.JogAcc : currentAxis.JogSpeed,
                    Dec = currentAxis.JogDec > 0.0f ? currentAxis.JogDec : currentAxis.JogDec,
                    Jerk = currentAxis.JogJerk > 0.0f ? currentAxis.JogJerk : currentAxis.JogJerk,
                };
                if (rbMasterVelocityMove.Checked == true)
                {
                    currentServoUnit.JogMove(currentAxis, enAxisOutFlag.JogMinus, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogX_minus_MouseDown : velocity={0}", set.Vel));
                }
                else if (rbMasterStepMove.Checked == true)
                {
                    currentServoUnit.MoveRelativeStart(currentAxis, (-1) * currentAxis.JogDistance, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogX_minus_MouseDown : velocity={0}, distance={1}", set.Vel, currentAxis.JogDistance));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnJogX_minus_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                ServoUnit currentServoUnit = null;
                _Axis currentAxis = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.X && axis.GantryType && axis.AxisName == m_devTransfer.AxisMaster.AxisName)
                    {
                        if (axis.CommandSkip) continue;
                        currentAxis = axis;
                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (rbMasterVelocityMove.Checked == true)
                {
                    currentServoUnit.ResetJogSpeed();
                    currentServoUnit.ResetCommand();
                }
                ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogX_minus_MouseUp"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void btnJogX_plus_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                bool ValueOk = true;
                _Axis currentAxis = null;
                ServoUnit currentServoUnit = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.X && axis.GantryType && axis.AxisName == m_devTransfer.AxisMaster.AxisName)
                    {
                        if (axis.CommandSkip) continue;

                        if (ValueOk &= double.TryParse(tbMasterVelocity.Text, out double masterVelocity))
                        {
                            axis.JogSpeed = (float)masterVelocity;
                        }
                        if (rbMasterStepMove.Checked == true)
                        {
                            if (ValueOk &= double.TryParse(tbMasterDistance.Text, out double masterDistance)) axis.JogDistance = (float)masterDistance;
                        }
                        currentAxis = axis;

                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (!ValueOk)
                {
                    string msg = "Please enter velocity";
                    if (rbMasterStepMove.Checked == true)
                    {
                        msg = "Please enter velocity and Distance";
                    }
                    MessageBox.Show(msg);
                    return;
                }


                enAxisResult rv = enAxisResult.None;
                bool isSafe = true;
                // Check Interlock 
                if (rbMasterStepMove.Checked == true)
                {
                    if (currentAxis.CommandSkip) return;
                    isSafe &= InterlockManager.IsSafe(currentAxis, currentAxis.JogDistance + (currentAxis as IAxisCommand).GetAxisCurPos());
                }

                if (isSafe == false)
                {
                    MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                VelSet set = new VelSet
                {
                    AxisCoord = currentAxis.AxisCoord,
                    Vel = currentAxis.JogSpeed,
                    Acc = currentAxis.JogAcc > 0.0f ? currentAxis.JogAcc : currentAxis.JogSpeed,
                    Dec = currentAxis.JogDec > 0.0f ? currentAxis.JogDec : currentAxis.JogDec,
                    Jerk = currentAxis.JogJerk > 0.0f ? currentAxis.JogJerk : currentAxis.JogJerk,
                };
                if (rbMasterVelocityMove.Checked == true)
                {
                    currentServoUnit.JogMove(currentAxis, enAxisOutFlag.JogPlus, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogX_plus_MouseDown : velocity={0}", set.Vel));
                }
                else if (rbMasterStepMove.Checked == true)
                {
                    currentServoUnit.MoveRelativeStart(currentAxis, currentAxis.JogDistance, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogX_plus_MouseDown : velocity={0}, distance={1}", set.Vel, currentAxis.JogDistance));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnJogX_plus_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                ServoUnit currentServoUnit = null;
                _Axis currentAxis = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.X && axis.GantryType && axis.AxisName == m_devTransfer.AxisMaster.AxisName)
                    {
                        if (axis.CommandSkip) continue;
                        currentAxis = axis;
                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (rbMasterVelocityMove.Checked == true)
                {
                    currentServoUnit.ResetJogSpeed();
                    currentServoUnit.ResetCommand();
                }
                ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogX_plus_MouseUp"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnJogT_minus_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                bool ValueOk = true;
                _Axis currentAxis = null;
                ServoUnit currentServoUnit = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.T)
                    {
                        if (axis.CommandSkip) continue;

                        if (ValueOk &= double.TryParse(tbRotateVelocity.Text, out double rotateVelocity))
                        {
                            axis.JogSpeed = (float)rotateVelocity;
                        }
                        if (rbRotateStepMove.Checked == true)
                        {
                            if (ValueOk &= double.TryParse(tbRotateDistance.Text, out double rotateDistance)) axis.JogDistance = (float)rotateDistance;
                        }
                        currentAxis = axis;

                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (!ValueOk)
                {
                    string msg = "Please enter velocity";
                    if (rbRotateStepMove.Checked == true)
                    {
                        msg = "Please enter velocity and Distance";
                    }
                    MessageBox.Show(msg);
                    return;
                }


                enAxisResult rv = enAxisResult.None;
                bool isSafe = true;
                // Check Interlock 
                if (rbRotateStepMove.Checked == true)
                {
                    if (currentAxis.CommandSkip) return;
                    isSafe &= InterlockManager.IsSafe(currentAxis, currentAxis.JogDistance * -1.0 + (currentAxis as IAxisCommand).GetAxisCurPos());
                }

                if (isSafe == false)
                {
                    MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                VelSet set = new VelSet
                {
                    AxisCoord = currentAxis.AxisCoord,
                    Vel = currentAxis.JogSpeed,
                    Acc = currentAxis.JogAcc > 0.0f ? currentAxis.JogAcc : currentAxis.JogSpeed,
                    Dec = currentAxis.JogDec > 0.0f ? currentAxis.JogDec : currentAxis.JogDec,
                    Jerk = currentAxis.JogJerk > 0.0f ? currentAxis.JogJerk : currentAxis.JogJerk,
                };
                if (rbRotateVelocityMove.Checked == true)
                {
                    currentServoUnit.JogMove(currentAxis, enAxisOutFlag.JogMinus, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogT_minus_MouseDown : velocity={0}", set.Vel));
                }
                else if (rbRotateStepMove.Checked == true)
                {
                    currentServoUnit.MoveRelativeStart(currentAxis, (-1) * currentAxis.JogDistance, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogT_minus_MouseDown : velocity={0}, distance={1}", set.Vel, currentAxis.JogDistance));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void btnJogT_minus_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                ServoUnit currentServoUnit = null;
                _Axis currentAxis = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.T)
                    {
                        if (axis.CommandSkip) continue;
                        currentAxis = axis;
                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (rbRotateVelocityMove.Checked == true)
                {
                    currentServoUnit.ResetJogSpeed();
                    currentServoUnit.ResetCommand();
                }
                ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogT_minus_MouseUp"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void btnJogT_plus_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                bool ValueOk = true;
                _Axis currentAxis = null;
                ServoUnit currentServoUnit = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.T)
                    {
                        if (axis.CommandSkip) continue;

                        if (ValueOk &= double.TryParse(tbRotateVelocity.Text, out double rotateVelocity))
                        {
                            axis.JogSpeed = (float)rotateVelocity;
                        }
                        if (rbRotateStepMove.Checked == true)
                        {
                            if (ValueOk &= double.TryParse(tbRotateDistance.Text, out double rotateDistance)) axis.JogDistance = (float)rotateDistance;
                        }
                        currentAxis = axis;

                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (!ValueOk)
                {
                    string msg = "Please enter velocity";
                    if (rbRotateStepMove.Checked == true)
                    {
                        msg = "Please enter velocity and Distance";
                    }
                    MessageBox.Show(msg);
                    return;
                }


                enAxisResult rv = enAxisResult.None;
                bool isSafe = true;
                // Check Interlock 
                if (rbRotateStepMove.Checked == true)
                {
                    if (currentAxis.CommandSkip) return;
                    isSafe &= InterlockManager.IsSafe(currentAxis, currentAxis.JogDistance + (currentAxis as IAxisCommand).GetAxisCurPos());
                }

                if (isSafe == false)
                {
                    MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                VelSet set = new VelSet
                {
                    AxisCoord = currentAxis.AxisCoord,
                    Vel = currentAxis.JogSpeed,
                    Acc = currentAxis.JogAcc > 0.0f ? currentAxis.JogAcc : currentAxis.JogSpeed,
                    Dec = currentAxis.JogDec > 0.0f ? currentAxis.JogDec : currentAxis.JogDec,
                    Jerk = currentAxis.JogJerk > 0.0f ? currentAxis.JogJerk : currentAxis.JogJerk,
                };
                if (rbRotateVelocityMove.Checked == true)
                {
                    currentServoUnit.JogMove(currentAxis, enAxisOutFlag.JogPlus, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogT_plus_MouseDown : velocity={0}", set.Vel));
                }
                else if (rbRotateStepMove.Checked == true)
                {
                    currentServoUnit.MoveRelativeStart(currentAxis, currentAxis.JogDistance, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogT_plus_MouseDown : velocity={0}, distance={1}", set.Vel, currentAxis.JogDistance));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnJogT_plus_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                ServoUnit currentServoUnit = null;
                _Axis currentAxis = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.T)
                    {
                        if (axis.CommandSkip) continue;
                        currentAxis = axis;
                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (rbRotateVelocityMove.Checked == true)
                {
                    currentServoUnit.ResetJogSpeed();
                    currentServoUnit.ResetCommand();
                }
                ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogT_plus_MouseUp"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnJogY_minus_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                bool ValueOk = true;
                _Axis currentAxis = null;
                ServoUnit currentServoUnit = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.Y)
                    {
                        if (axis.CommandSkip) continue;

                        if (ValueOk &= double.TryParse(tbSlideVelocity.Text, out double slideVelocity))
                        {
                            axis.JogSpeed = (float)slideVelocity;
                        }
                        if (rbSlideStepMove.Checked == true)
                        {
                            if (ValueOk &= double.TryParse(tbSlideDistance.Text, out double slideDistance)) axis.JogDistance = (float)slideDistance;
                        }
                        currentAxis = axis;

                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (!ValueOk)
                {
                    string msg = "Please enter velocity";
                    if (rbSlideStepMove.Checked == true)
                    {
                        msg = "Please enter velocity and Distance";
                    }
                    MessageBox.Show(msg);
                    return;
                }


                enAxisResult rv = enAxisResult.None;
                bool isSafe = true;
                // Check Interlock 
                if (rbSlideStepMove.Checked == true)
                {
                    if (currentAxis.CommandSkip) return;
                    isSafe &= InterlockManager.IsSafe(currentAxis, currentAxis.JogDistance * -1.0 + (currentAxis as IAxisCommand).GetAxisCurPos());
                }

                if (isSafe == false)
                {
                    MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                VelSet set = new VelSet
                {
                    AxisCoord = currentAxis.AxisCoord,
                    Vel = currentAxis.JogSpeed,
                    Acc = currentAxis.JogAcc > 0.0f ? currentAxis.JogAcc : currentAxis.JogSpeed,
                    Dec = currentAxis.JogDec > 0.0f ? currentAxis.JogDec : currentAxis.JogDec,
                    Jerk = currentAxis.JogJerk > 0.0f ? currentAxis.JogJerk : currentAxis.JogJerk,
                };
                if (rbSlideVelocityMove.Checked == true)
                {
                    currentServoUnit.JogMove(currentAxis, enAxisOutFlag.JogMinus, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogY_minus_MouseDown : velocity={0}", set.Vel));
                }
                else if (rbSlideStepMove.Checked == true)
                {
                    currentServoUnit.MoveRelativeStart(currentAxis, (-1) * currentAxis.JogDistance, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogY_minus_MouseDown : velocity={0}, distance={1}", set.Vel, currentAxis.JogDistance));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void btnJogY_minus_MouseUp(object sender, MouseEventArgs e)
        {
            ServoUnit currentServoUnit = null;
            _Axis currentAxis = null;
            foreach (_Axis axis in m_ServoManager.AxisSource)
            {
                if (axis.AxisCoord == enAxisCoord.Y)
                {
                    if (axis.CommandSkip) continue;
                    currentAxis = axis;
                    break;
                }
            }
            if (currentAxis == null)
            {
                return;
            }
            foreach (ServoUnit unit in m_ServoManager.ServoUnits)
            {
                if (unit == null) return;
                if (unit.Axes.Contains(currentAxis))
                {
                    currentServoUnit = unit;
                    break;
                }
            }
            if (currentServoUnit == null)
            {
                return;
            }
            if (rbSlideVelocityMove.Checked == true)
            {
                currentServoUnit.ResetJogSpeed();
                currentServoUnit.ResetCommand();
            }
            ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogY_minus_MouseUp"));

        }
        private void btnJogY_plus_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                bool ValueOk = true;
                _Axis currentAxis = null;
                ServoUnit currentServoUnit = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.Y)
                    {
                        if (axis.CommandSkip) continue;

                        if (ValueOk &= double.TryParse(tbSlideVelocity.Text, out double slideVelocity))
                        {
                            axis.JogSpeed = (float)slideVelocity;
                        }
                        if (rbSlideStepMove.Checked == true)
                        {
                            if (ValueOk &= double.TryParse(tbSlideDistance.Text, out double slideDistance)) axis.JogDistance = (float)slideDistance;
                        }
                        currentAxis = axis;

                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (!ValueOk)
                {
                    string msg = "Please enter velocity";
                    if (rbSlideStepMove.Checked == true)
                    {
                        msg = "Please enter velocity and Distance";
                    }
                    MessageBox.Show(msg);
                    return;
                }


                enAxisResult rv = enAxisResult.None;
                bool isSafe = true;
                // Check Interlock 
                if (rbSlideStepMove.Checked == true)
                {
                    if (currentAxis.CommandSkip) return;
                    isSafe &= InterlockManager.IsSafe(currentAxis, currentAxis.JogDistance + (currentAxis as IAxisCommand).GetAxisCurPos());
                }

                if (isSafe == false)
                {
                    MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                VelSet set = new VelSet
                {
                    AxisCoord = currentAxis.AxisCoord,
                    Vel = currentAxis.JogSpeed,
                    Acc = currentAxis.JogAcc > 0.0f ? currentAxis.JogAcc : currentAxis.JogSpeed,
                    Dec = currentAxis.JogDec > 0.0f ? currentAxis.JogDec : currentAxis.JogDec,
                    Jerk = currentAxis.JogJerk > 0.0f ? currentAxis.JogJerk : currentAxis.JogJerk,
                };
                if (rbSlideVelocityMove.Checked == true)
                {
                    currentServoUnit.JogMove(currentAxis, enAxisOutFlag.JogPlus, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogY_plus_MouseDown : velocity={0}", set.Vel));
                }
                else if (rbSlideStepMove.Checked == true)
                {
                    currentServoUnit.MoveRelativeStart(currentAxis, currentAxis.JogDistance, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogY_plus_MouseDown : velocity={0}, distance={1}", set.Vel, currentAxis.JogDistance));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnJogY_plus_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                ServoUnit currentServoUnit = null;
                _Axis currentAxis = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.Y)
                    {
                        if (axis.CommandSkip) continue;
                        currentAxis = axis;
                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (rbSlideVelocityMove.Checked == true)
                {
                    currentServoUnit.ResetJogSpeed();
                    currentServoUnit.ResetCommand();
                }
                ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogY_plus_MouseUp"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnJogZ_minus_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                bool ValueOk = true;
                _Axis currentAxis = null;
                ServoUnit currentServoUnit = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.Z)
                    {
                        if (axis.CommandSkip) continue;

                        if (ValueOk &= double.TryParse(tbHoistVeloctiy.Text, out double hoistVelocity))
                        {
                            axis.JogSpeed = (float)hoistVelocity;
                        }
                        if (rbHoistStepMove.Checked == true)
                        {
                            if (ValueOk &= double.TryParse(tbHoistDistance.Text, out double hoistDistance)) axis.JogDistance = (float)hoistDistance;
                        }
                        currentAxis = axis;

                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (!ValueOk)
                {
                    string msg = "Please enter velocity";
                    if (rbHoistStepMove.Checked == true)
                    {
                        msg = "Please enter velocity and Distance";
                    }
                    MessageBox.Show(msg);
                    return;
                }


                enAxisResult rv = enAxisResult.None;
                bool isSafe = true;
                // Check Interlock 
                if (rbHoistStepMove.Checked == true)
                {
                    if (currentAxis.CommandSkip) return;
                    isSafe &= InterlockManager.IsSafe(currentAxis, currentAxis.JogDistance * -1.0 + (currentAxis as IAxisCommand).GetAxisCurPos());
                }

                if (isSafe == false)
                {
                    MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                VelSet set = new VelSet
                {
                    AxisCoord = currentAxis.AxisCoord,
                    Vel = currentAxis.JogSpeed,
                    Acc = currentAxis.JogAcc > 0.0f ? currentAxis.JogAcc : currentAxis.JogSpeed,
                    Dec = currentAxis.JogDec > 0.0f ? currentAxis.JogDec : currentAxis.JogDec,
                    Jerk = currentAxis.JogJerk > 0.0f ? currentAxis.JogJerk : currentAxis.JogJerk,
                };
                if (rbHoistVelocityMove.Checked == true)
                {
                    currentServoUnit.JogMove(currentAxis, enAxisOutFlag.JogMinus, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogZ_minus_MouseDown : velocity={0}", set.Vel));
                }
                else if (rbHoistStepMove.Checked == true)
                {
                    currentServoUnit.MoveRelativeStart(currentAxis, (-1) * currentAxis.JogDistance, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogZ_minus_MouseDown : velocity={0}, distance={1}", set.Vel, currentAxis.JogDistance));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnJogZ_minus_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                ServoUnit currentServoUnit = null;
                _Axis currentAxis = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.Z)
                    {
                        if (axis.CommandSkip) continue;
                        currentAxis = axis;
                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (rbHoistVelocityMove.Checked == true)
                {
                    currentServoUnit.ResetJogSpeed();
                    currentServoUnit.ResetCommand();
                }
                ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogZ_minus_MouseUp"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnJogZ_plus_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                bool ValueOk = true;
                _Axis currentAxis = null;
                ServoUnit currentServoUnit = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.Z)
                    {
                        if (axis.CommandSkip) continue;

                        if (ValueOk &= double.TryParse(tbHoistVeloctiy.Text, out double hoistVelocity))
                        {
                            axis.JogSpeed = (float)hoistVelocity;
                        }
                        if (rbHoistStepMove.Checked == true)
                        {
                            if (ValueOk &= double.TryParse(tbHoistDistance.Text, out double hoistDistance)) axis.JogDistance = (float)hoistDistance;
                        }
                        currentAxis = axis;

                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (!ValueOk)
                {
                    string msg = "Please enter velocity";
                    if (rbHoistStepMove.Checked == true)
                    {
                        msg = "Please enter velocity and Distance";
                    }
                    MessageBox.Show(msg);
                    return;
                }


                enAxisResult rv = enAxisResult.None;
                bool isSafe = true;
                // Check Interlock 
                if (rbHoistStepMove.Checked == true)
                {
                    if (currentAxis.CommandSkip) return;
                    isSafe &= InterlockManager.IsSafe(currentAxis, currentAxis.JogDistance + (currentAxis as IAxisCommand).GetAxisCurPos());
                }

                if (isSafe == false)
                {
                    MessageBox.Show("Interlock Unsafe!", "INTERLOCK", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                VelSet set = new VelSet
                {
                    AxisCoord = currentAxis.AxisCoord,
                    Vel = currentAxis.JogSpeed,
                    Acc = currentAxis.JogAcc > 0.0f ? currentAxis.JogAcc : currentAxis.JogSpeed,
                    Dec = currentAxis.JogDec > 0.0f ? currentAxis.JogDec : currentAxis.JogDec,
                    Jerk = currentAxis.JogJerk > 0.0f ? currentAxis.JogJerk : currentAxis.JogJerk,
                };
                if (rbHoistVelocityMove.Checked == true)
                {
                    currentServoUnit.JogMove(currentAxis, enAxisOutFlag.JogPlus, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogZ_plus_MouseDown : velocity={0}", set.Vel));
                }
                else if (rbHoistStepMove.Checked == true)
                {
                    currentServoUnit.MoveRelativeStart(currentAxis, currentAxis.JogDistance, set);
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogZ_plus_MouseDown : velocity={0}, distance={1}", set.Vel, currentAxis.JogDistance));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnJogZ_plus_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                ServoUnit currentServoUnit = null;
                _Axis currentAxis = null;
                foreach (_Axis axis in m_ServoManager.AxisSource)
                {
                    if (axis.AxisCoord == enAxisCoord.Z)
                    {
                        if (axis.CommandSkip) continue;
                        currentAxis = axis;
                        break;
                    }
                }
                if (currentAxis == null)
                {
                    return;
                }
                foreach (ServoUnit unit in m_ServoManager.ServoUnits)
                {
                    if (unit == null) return;
                    if (unit.Axes.Contains(currentAxis))
                    {
                        currentServoUnit = unit;
                        break;
                    }
                }
                if (currentServoUnit == null)
                {
                    return;
                }
                if (rbHoistVelocityMove.Checked == true)
                {
                    currentServoUnit.ResetJogSpeed();
                    currentServoUnit.ResetCommand();
                }

                ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnJogZ_plus_MouseUp"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        #endregion

        private void btn_MoveToTeaching(object sender, EventArgs e)
        {
            try
            {
                double velocity = 0.0f;
                Button button = sender as Button;
                if (button == btnHoistMoveToTeaching)
                {
                    if (double.TryParse(tbHoistVeloctiy.Text, out velocity))
                    {
                        MoveToPosition(enAxisCoord.Z, velocity, false);
                    }
                    else
                    {
                        MessageBox.Show("Please enter velocity value!");
                    }
                }
                else if (button == btnSlideMoveToTeaching)
                {
                    if (double.TryParse(tbSlideVelocity.Text, out velocity))
                    {
                        MoveToPosition(enAxisCoord.Y, velocity, false);
                    }
                    else
                    {
                        MessageBox.Show("Please enter velocity value!");
                    }
                }
                else if (button == btnRotateMoveToTeaching)
                {
                    if (double.TryParse(tbRotateVelocity.Text, out velocity))
                    {
                        MoveToPosition(enAxisCoord.T, velocity, false);
                    }
                    else
                    {
                        MessageBox.Show("Please enter velocity value!");
                    }
                }
                ButtonLog.WriteLog(this.Name.ToString(), string.Format("btn_MoveToTeaching : Button={0}, velocity={1}", button.Text, velocity));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnHoistMoveToBeforeDown_Click(object sender, EventArgs e)
        {
            try
            {
                if (double.TryParse(tbHoistVeloctiy.Text, out double velocity))
                {
                    MoveToPosition(enAxisCoord.Z, velocity, false, true);
                }
                else
                {
                    MessageBox.Show("Please enter velocity value!");
                }
                ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnHoistMoveToBeforeDown_Click : velocity={0}", velocity));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void btn_MoveToHome(object sender, EventArgs e)
        {
            try
            {
                Button button = sender as Button;
                if (button == btnHoistMoveToHome)
                {
                    if (double.TryParse(tbHoistVeloctiy.Text, out double velocity))
                    {
                        MoveToPosition(enAxisCoord.Z, velocity, true);
                    }
                    else
                    {
                        MessageBox.Show("Please enter velocity value!");
                    }
                }
                else if (button == btnSlideMoveToHome)
                {
                    if (double.TryParse(tbSlideVelocity.Text, out double velocity))
                    {
                        MoveToPosition(enAxisCoord.Y, velocity, true);
                    }
                    else
                    {
                        MessageBox.Show("Please enter velocity value!");
                    }
                }
                else if (button == btnRotateMoveToHome)
                {
                    if (double.TryParse(tbRotateVelocity.Text, out double velocity))
                    {
                        MoveToPosition(enAxisCoord.T, velocity, true);
                    }
                    else
                    {
                        MessageBox.Show("Please enter velocity value!");
                    }
                }
                ButtonLog.WriteLog(this.Name.ToString(), string.Format("btn_MoveToHome : Button={0}", button.Text));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                Button button = sender as Button;
                if (button == btnHoistJogStop)
                {
                    StopAxis(enAxisCoord.Z);
                }
                else if (button == btnSlideJogStop)
                {
                    StopAxis(enAxisCoord.Y);
                }
                else if (button == btnRotateJogStop)
                {
                    StopAxis(enAxisCoord.T);
                }
                else if (button == btnMasterJogStop)
                {
                    StopAxis(enAxisCoord.X);
                }
                ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnStop_Click : Button={0}", button.Text));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void btnSaveDataBase_Click(object sender, EventArgs e)
        {
            try
            {
                if (ltBoxPortList.SelectedItem == null)
                {
                    MessageBox.Show("Please choose one Port to save to database!");
                    return;
                }
                if (MessageBox.Show($"Do you want to save current position to Port {ltBoxPortList.SelectedItem.ToString()}", "Save to Teaching Data", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
                UpdateTeachingPosition teachingPosition = UpdateTeachingPosition.None;
                if (rbLoad.Checked)
                {
                    teachingPosition = UpdateTeachingPosition.Load;
                }
                else if (rbUnload.Checked)
                {
                    teachingPosition = UpdateTeachingPosition.Unload;
                }
                if (int.TryParse(ltBoxPortList.SelectedItem.ToString(), out int portID))
                {
                    int currentSelectedIndex = ltBoxPortList.SelectedIndex;
                    UpdateTeachingDataToDataBase(portID, teachingPosition, !cbReferenceVehicle.Checked);
                    ltBoxPortList.SelectedIndex = currentSelectedIndex;
                }

                ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnSaveDataBase_Click : rbLoad={0},rbUnload={1}, cbReferenceVehicle={2}, ltBoxPortList={3}",
                    rbLoad.Checked, rbUnload.Checked, cbReferenceVehicle.Checked, ltBoxPortList.SelectedItem));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void btnBcrSet_Click(object sender, EventArgs e)
        {
            try
            {
                if (ltBoxPortList.SelectedItem == null)
                {
                    MessageBox.Show("Please choose one Port to set BCR value!");
                    return;
                }
                if (int.TryParse(ltBoxPortList.SelectedItem.ToString(), out int portID))
                {
                    DataItem_Port teachingData = ObjectCopier.Clone(m_PortDictory[portID]);
                    //teachingData.BarcodeLeft = 1887116.8;
                    //teachingData.BarcodeRight = 2230039.6;
                    double leftOffset = 0;// 3;
                    double rightOffset = 0;// -3;
                    ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr = teachingData.BarcodeLeft + leftOffset;
                    ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr = teachingData.BarcodeRight + rightOffset;
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnBcrSet_Click : BarcodeLeft={0},BarcodeRight={1}", teachingData.BarcodeLeft, teachingData.BarcodeRight));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void btnResettingOffset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Do you want to resetting offset of current port ", "Resetting offset", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                if (ltBoxPortList.SelectedItem == null)
                {
                    MessageBox.Show("Please choose one Port to reset offset!");
                    return;
                }
                if (int.TryParse(ltBoxPortList.SelectedItem.ToString(), out int portID))
                {
                    int currentSelectedIndex = ltBoxPortList.SelectedIndex;
                    ResettingOffset(portID);
                    ltBoxPortList.SelectedIndex = currentSelectedIndex;
                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnResettingOffset_Click : ltBoxPortList={0}", ltBoxPortList.SelectedItem));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion


    }
}
