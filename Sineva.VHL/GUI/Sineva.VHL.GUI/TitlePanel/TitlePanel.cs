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
using Sineva.VHL.IF.OCS;
using Sineva.VHL.IF.JCS;
using Sineva.VHL.Data;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data.LogIn;
using Sineva.VHL.Data.EventLog;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Device;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Sineva.VHL.GUI
{
    public partial class TitlePanel : UserControl
    {
        #region Field
        private readonly string _ModuleCheckItem_Emo = "EMO SWITCH";
        private readonly string _ModuleCheckItem_Safety = "SAFETY RELAY";
        private readonly string _ModuleCheckItem_Power = "SYSTEM POWER";
        private readonly string _ModuleCheckItem_Vision = "VISION I/F";
        private readonly string _ModuleCheckItem_Motion = "MOTION";
        private readonly string _ModuleCheckItem_IoLink = "I/O LINK";

        private EqpStateManager m_EqpManager = null;
        private OCSCommManager m_OcsManager = null;
        private JCSCommManager m_JcsManager = null;
        private ProcessDataHandler m_ProcessDataHandler = null;
        private ServoManager m_ServoManager = null;

        private FormModuleCheck m_ModuleCheckForm = null;
        private SortedList<string, LabelLamp.LampKind> m_ModuleCheckItems = new SortedList<string, LabelLamp.LampKind>();

        private bool m_Intialized = false;
        #endregion

        #region Constructor
        public TitlePanel()
        {
            InitializeComponent();

			InitInstance();
		}
        #endregion

        #region Method
        private void InitInstance()
        {
			if (XFunc.IsDesignTime()) return;

			m_EqpManager = EqpStateManager.Instance;
	        m_OcsManager = OCSCommManager.Instance;
            m_JcsManager = JCSCommManager.Instance;
	        m_ServoManager = ServoManager.Instance;
            m_ProcessDataHandler = ProcessDataHandler.Instance;

            EventLogHandler.Instance.AddMainMessage += new OccurMessageEventHandler(Instance_AddMainMessage);
            //InterlockManager.InterlockMessage += InterlockManager_InterlockMessage;
            lbVehicleNumber.SetText(AppConfig.Instance.ProjectType.ToString() + "_No." + AppConfig.Instance.VehicleNumber.ToString());
            tmrUpdate.Interval = 1000;
            tmrUpdate.Start();
        }

        private void CreateModuleCheckItems()
        {
            // 여기서 Check Item 목록을 생성하면... 프로젝트 마다 이 부분 수정을 해줘야 하므로... Low Level Instance 생성 완료되는 시점에 목록을 만들어주도록 해야겠지... 그놈이 또 알아서 Set/Reset 하도록 해야 할 수도...
            m_ModuleCheckItems.Clear();
            m_ModuleCheckItems.Add(_ModuleCheckItem_Emo, LabelLamp.LampKind.DualNormal);
            m_ModuleCheckItems.Add(_ModuleCheckItem_Safety, LabelLamp.LampKind.DualNormal);
            m_ModuleCheckItems.Add(_ModuleCheckItem_Power, LabelLamp.LampKind.DualNormal);
            m_ModuleCheckItems.Add(_ModuleCheckItem_Vision, LabelLamp.LampKind.DualNormal);
            m_ModuleCheckItems.Add(_ModuleCheckItem_Motion, LabelLamp.LampKind.DualNormal);
            m_ModuleCheckItems.Add(_ModuleCheckItem_IoLink, LabelLamp.LampKind.DualNormal);

            m_ModuleCheckForm = new FormModuleCheck();
            m_ModuleCheckForm.CreateControl(this.panelModuleStatus);        // CreateControl을 먼저 하든... AddModule을 먼저하든.... 어찌됐든... CreateControl을 무조건 하기만 하면 된다...
            foreach(KeyValuePair<string, LabelLamp.LampKind> pair in m_ModuleCheckItems)
            {
                m_ModuleCheckForm.AddModule(pair.Key, pair.Value);
            }
        }
        private void UpdateObjects()
        {
            try
            {
                UpdateDateTime();
                UpdateLabel();
                if (m_Intialized)
                {
                    UpdateLamp();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void UpdateDateTime()
        {
            try
            {
                DateTime dt = DateTime.Now;
                string value = string.Format("{0:yyyy/M/d HH:mm:ss}", dt);
                this.lbCurSystemTime.Text = value;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void UpdateLabel()
        {
            if(m_EqpManager == null || m_OcsManager == null || m_JcsManager == null) return;

            try
            {
                string command = string.Format("NONE");
                if (m_ProcessDataHandler.CurTransferCommand.IsValid)
                    command = string.Format("{0}", m_ProcessDataHandler.CurTransferCommand.CommandID);

                if (command != lbCommand.LabelText)
                {
                    lbCommand.LabelText = command;
                    if (m_ProcessDataHandler.CurTransferCommand.IsValid) lbCommand.TextColor = Color.Blue;
                    else lbCommand.TextColor = Color.Gray;
                }

                VehicleState vehicleState = m_ProcessDataHandler.CurVehicleStatus.GetVehicleStatus();
                if (vehicleState.ToString() != lbViewName.LabelText)
                {
                    lbViewName.LabelText = vehicleState.ToString();
                    if (vehicleState == VehicleState.EnRouteToSource) lbViewName.TextColor = Color.BlueViolet;
                    else if (vehicleState == VehicleState.EnRouteToDest) lbViewName.TextColor = Color.BlueViolet;
                    else if (vehicleState == VehicleState.Acquiring) lbViewName.TextColor = Color.DarkGreen;
                    else if (vehicleState == VehicleState.Depositing) lbViewName.TextColor = Color.DarkGreen;
                    else if (vehicleState == VehicleState.AcquireCompleted) lbViewName.TextColor = Color.Green;
                    else if (vehicleState == VehicleState.DepositeCompleted) lbViewName.TextColor = Color.Green;
                    else if (vehicleState == VehicleState.SourceEmpty) lbViewName.TextColor = Color.Red;
                    else if (vehicleState == VehicleState.AcquireFailed) lbViewName.TextColor = Color.Red;
                    else if (vehicleState == VehicleState.DestinationDouble) lbViewName.TextColor = Color.Red;
                    else if (vehicleState == VehicleState.DepositFailed) lbViewName.TextColor = Color.Red;
                    else lbViewName.TextColor = Color.Gray;
                }

                string state = string.Empty;
                if (GV.AutoTeachingModeOn) state = "AutoTeach";
                else state = Enum.GetName(typeof(OperateMode), m_EqpManager.OpMode);
                if (lbEqpMode.LabelText != state)
                    lbEqpMode.LabelText = state;

                state = Enum.GetName(typeof(EqpState), m_EqpManager.State);
                if (lbEqpState.LabelText != state)
                {
                    lbEqpState.LabelText = state;
                    if (m_EqpManager.State == EqpState.Down || m_EqpManager.State == EqpState.Pause || m_EqpManager.State == EqpState.Stop)
                    {
                        lbEqpState.LableColor = Color.White;
                        lbEqpState.SelectBackGround(enRGBSelect.Red);
                    }
                    else if (m_EqpManager.State == EqpState.Run)
                    {
                        lbEqpState.LableColor = Color.White;
                        lbEqpState.SelectBackGround(enRGBSelect.Green);
                    }
                    else if (m_EqpManager.State == EqpState.Idle)
                    {
                        lbEqpState.LableColor = Color.Black;
                        lbEqpState.SelectBackGround(enRGBSelect.Yellow);
                    }
                }

                state = m_OcsManager.OcsStatus.ConnectError ? "Error" :
                        m_OcsManager.OcsStatus.Connected ? "Ready" : "Not Ready";
                if (lbOcsState.LabelText != state)
                {
                    lbOcsState.LabelText = state;
                    if (state == "Ready") lbOcsState.SelectBackGround(enRGBSelect.Green);
                    else lbOcsState.SelectBackGround(enRGBSelect.Red);
                }
                state = m_JcsManager.JcsStatus.ConnectError ? "Error" :
                        m_JcsManager.JcsStatus.Connected ? "Ready" : "Not Ready";
                if (lbJcsState.LabelText != state)
                {
                    lbJcsState.LabelText = state;
                    if (state == "Ready") lbJcsState.SelectBackGround(enRGBSelect.Green);
                    else lbJcsState.SelectBackGround(enRGBSelect.Red);
                }

                string value = m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.LeftBcr.ToString("F1");
                state = "L : " + value;
                if (lbLeftBcr.LabelText != state)
                {
                    lbLeftBcr.LabelText = state;
                    if (value == "0.0") lbLeftBcr.BackColor = Color.Red;
                    else lbLeftBcr.BackColor = Color.Transparent;
                }

                value = m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.RightBcr.ToString("F1");
                state = "R : " + value;
                if (lbRightBcr.LabelText != state)
                {
                    lbRightBcr.LabelText = state;
                    if (value == "0.0") lbRightBcr.BackColor = Color.Red;
                    else lbRightBcr.BackColor = Color.Transparent;
                }

                string info = string.Format("{0}, {1}, N[{2} _ {3:F1}]", 
                    DevicesManager.Instance.DevSteer.GetFrontSteerDirection(), 
                    DevicesManager.Instance.DevSteer.GetRearSteerDirection(),
                    m_ProcessDataHandler.CurVehicleStatus.CurrentNode.NodeID,
                    m_ProcessDataHandler.CurVehicleStatus.CurrentPath.CurrentPositionOfLink);
                lbPathInfo.LabelText = info;                
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void UpdateLamp()
        {
            if(m_EqpManager == null)// || m_CimManager == null || m_Melsec == null)
                return;

            bool status = false;

            try
            {
                m_ModuleCheckForm.SetStatus(_ModuleCheckItem_Vision, status);   //LampVisionInterface.SetStatus(status);
            }
            catch { }

            try
            {
                status = true;
                foreach(_AxisBlock block in m_ServoManager.AxisBlocks)
                {
                    switch(block.ControlFamily)
                    {
                    case ServoControlFamily.MXP:
                        {
                            status &= (block as AxisBlockMXP).Connected;
                        }
                        break;
                    }
                }
                m_ModuleCheckForm.SetStatus(_ModuleCheckItem_Motion, status);
            }
            catch { }

            try
            {
                status = !GV.EmoAlarm;
                m_ModuleCheckForm.SetStatus(_ModuleCheckItem_Emo, status);  //LampEmo.SetStatus(status);

                status = !GV.PowerOn;
                m_ModuleCheckForm.SetStatus(_ModuleCheckItem_Power, status);    //LampPower.SetStatus(status);

                status = !GV.SaftyAlarm;
                m_ModuleCheckForm.SetStatus(_ModuleCheckItem_Safety, status);
            }
            catch { }
        }
        public void SetViewName(string name)
        {
            lbViewName.SetText(name);
        }
		public void SetLogin(bool login, DataItem_UserInfo account)
        {
            if(login)
			{
				logInView2.Message = "LOGOUT";
			}
            else
			{
				logInView2.Message = "LOGIN";
			}

			if (account != null)
			{
				lbLoginInfo.Text = string.Format("Welcome to\n {0} !", account.UserName);
				lbLoginInfo.ForeColor = Color.Blue;
			}
			else
			{
				lbLoginInfo.Text = string.Format(" Log Off ");
				lbLoginInfo.ForeColor = Color.Gray;
			}
        }
        private void AddNewMessage(string message)
        {
            try
            {
                if (cbMessage.Items.Count > 20)
                    cbMessage.Items.RemoveAt(0);

                cbMessage.SelectedIndex = cbMessage.Items.Add(DateTime.Now.ToString() + " " + message);
            }
            catch(Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        #endregion

        #region Event Handler
        private void Instance_AddMainMessage(string msg)
        {
            try
            {
                if (this.cbMessage.InvokeRequired)
                {
                    OccurMessageEventHandler d = new OccurMessageEventHandler(Instance_AddMainMessage);
                    this.Invoke(d, new object[] { msg });
                }
                else
                {
                    AddNewMessage(msg);
                }
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        void InterlockManager_InterlockMessage(string val)
        {
            if(this.cbMessage.InvokeRequired)
            {
                OccurMessageEventHandler d = new OccurMessageEventHandler(Instance_AddMainMessage);
                this.Invoke(d, new object[] { val });
            }
            else
            {
                AddNewMessage(val);
            }
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            UpdateObjects();
        }

        public void Initialize()
        {
            CreateModuleCheckItems();
            m_Intialized = true;
        }
        private void lbLeftBcr_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (AppConfig.Instance.Simulation.MY_DEBUG == false) return;
            using (DlgValueInput dlg = new DlgValueInput(lbLeftBcr.LabelText))
            {
                dlg.Text = "Key In BCR Value";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    double bcr = 0.0f;
                    if (double.TryParse(dlg.Value, out bcr))
                    {
                        DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().SetAxisLeftBarcode(bcr);
                    }
                }
                dlg.Dispose();
            }
        }

        private void lbRightBcr_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (AppConfig.Instance.Simulation.MY_DEBUG == false) return;
            using (DlgValueInput dlg = new DlgValueInput(lbRightBcr.LabelText))
            {
                dlg.Text = "Key In BCR Value";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    double bcr = 0.0f;
                    if (double.TryParse(dlg.Value, out bcr))
                    {
                        DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().SetAxisRightBarcode(bcr);
                    }
                }
                dlg.Dispose();
            }

        }
        #endregion
    }
}
