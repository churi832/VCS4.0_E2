using ICS.GUI;
using Sineva.VHL.Data;
using Sineva.VHL.Data.EventLog;
using Sineva.VHL.Data.LogIn;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using Sineva.VHL.Library.Remoting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sineva.VHL.Device;
using Sineva.VHL.Data.Alarm;
using System.Threading;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.Library.RegistryKey;
using Sineva.VHL.Library.IO;
using Sineva.VHL.IF.OCS;

namespace Sineva.VHL.GUI
{
    public partial class MainForm : Form
    {
        #region Fields
        private EqpStateManager m_EqpManager = null;
        private JobsForm m_JobsForm = null;
        private SetupForm m_SetupForm = null;
        private MotorForm m_MotorForm = null;
        private SystemForm m_SystemForm = null;
        private AlarmForm m_AlarmForm = null;
        private DatabaseForm m_DatabaseForm = null;
        private List<IButton> m_NaviButtons = new List<IButton>();

        private FormNotifyConfigError m_DlgNotifyError = null;
        private DlgOpCallSingleMessage m_DlgOpCallSingleMessage = null;
        private DlgOpCallSingleMessage m_DlgOpCallCommand = null;
        private FormEqpInitialize m_DlgEqpInit;
        private FormDataDisplay m_DlgTpdMonitor = null;
        private FormAutoTeaching m_DlgAutoTeaching = null;

        private System.Windows.Forms.Timer m_TimerExit = null;
        private BackgroundWorker m_TouchGuiWorker = null;
        private BackgroundWorker m_WebGuiActionWorker = null;
        public delegate void ChangeModeHandler(OperateMode mode);
        public static event ChangeModeHandler ChangeMode;
        public delegate void ChangeRunModeHandler(EqpRunMode mode);
        public static event ChangeRunModeHandler ChangeRunMode;
        private System.Timers.Timer m_restoreTimer;
        private ServoUnit currentServoUnit = null;
        private VelSet set = new VelSet();
        private _Axis currentAxis = null;
        #endregion

        #region Constructor
        public MainForm()
        {
            InitializeComponent();
            string version = string.Empty;
            version = "  [" + AppConfig.Instance.Version + "]" + " _ [" + AppConfig.Instance.ProjectType.ToString() + "]"; // + DateTime.Now.ToString("(yyyy-MM-dd HH:mm:dd)");
            this.Text += version;

            ///////////////////////////////////////////////////////
            /// Instance Set
            m_EqpManager = EqpStateManager.Instance;
            ///////////////////////////////////////////////////////

            ///////////////////////////////////////////////////////
            /// Event Set
            EventHandlerManager.Instance.NotifyConfigErrorMessage += Instance_NotifyErrorMessage;
            EventHandlerManager.Instance.OperateModeChanged += Instance_OperateModeChanged;
            EventHandlerManager.Instance.OperatorCalled += Instance_OperatorCalled;
            EventHandlerManager.Instance.OperatorCallCommand += Instance_OperatorCallCommand;
            EventHandlerManager.Instance.AutoTeachingMonitorShow += Instance_AutoTeachingMonitorShow;

            AccountManager.Instance.AccountLogInOut += Instance_AccountLogInOut;

            PadRemoteGUI.ChangeMode += RemoteObject_ChangeMode;
            PadRemoteGUI.ChangeRunMode += PadRemoteGUI_ChangeRunMode;
            ///////////////////////////////////////////////////////
            bool ok = false;
            if (XFunc.IsRunTime())
            {
                ok = InitializeMainFormComponents();

                int m_SlaveNo = 0;
                int m_Offset = 0;
                int m_StartAddress = 0;
                ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.IoModuleSlaveNo = RegistryConfiguration.Instance.ReadEntry("SLAVENO", m_SlaveNo);
                ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.IoChannelOffset = RegistryConfiguration.Instance.ReadEntry("OFFSET", m_Offset);
                ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.IoChannelStartAddress = RegistryConfiguration.Instance.ReadEntry("STARTADDRESS", m_StartAddress);
            }
            if (!ok)
            {
                MessageBox.Show("Vehicle Program Initialize Failed!!!");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }

            timerUpdate.Enabled = true;

            #region Initialize Tasks
            {
                Sineva.VHL.Task.TaskPool.Instance.InitTaskPool();
                Sineva.VHL.Task.TaskPool.Instance.TaskStart();
            }
            #endregion

            // Touch GUI Start
            m_TouchGuiWorker = new BackgroundWorker();
            m_TouchGuiWorker.WorkerSupportsCancellation = true;
            m_TouchGuiWorker.DoWork += new DoWorkEventHandler(DoWork);
            m_TouchGuiWorker.RunWorkerAsync(new string[] { this.Text });

            m_WebGuiActionWorker = new BackgroundWorker();
            m_WebGuiActionWorker.WorkerSupportsCancellation = true;
            m_WebGuiActionWorker.DoWork += new DoWorkEventHandler(DoActionWork);
            m_WebGuiActionWorker.RunWorkerAsync(new string[] { this.Text });

            AppConfig.AppMainInitiated = true;
            EventLogHandler.Instance.Add("GUI", "MAIN", "Program Start", true);
            SequenceLog.WriteLog(string.Format("Program Start {0}", version));

            this.Resize += MainForm_Resize;
        }
        #endregion

        #region Methods
        private bool InitializeMainFormComponents()
        {
            bool ok = true;
            ///////////////////////////////////////////////////////
            /// Initialize Ctrl, Libs
            ok &= Sineva.VHL.Data.DbAdapter.DatabaseHandler.Instance.Initialize();
            ok &= Sineva.VHL.Data.EventLog.EventLogHandler.Instance.Initialize();
            ok &= Sineva.VHL.Data.LogIn.AccountManager.Instance.Initialize();
            ok &= Sineva.VHL.Data.Alarm.AlarmHandler.Instance.Initialize();
            ok &= Sineva.VHL.Data.Setup.SetupManager.Instance.Initialize();
            ok &= Sineva.VHL.Data.Process.ProcessDataHandler.Instance.Initialize();

            if (SetupManager.Instance.SetupOperation.NetworkControlUse == Use.Use)
            {
                string networkName = AppConfig.Instance.WifiNetworkConnectionID;
                string wifiName = AppConfig.Instance.WifiName;
                bool wifi_enable = Sineva.VHL.Library.SimpleWifi.NetworkManager.Instance.StartMonitor(networkName, wifiName);
                if (wifi_enable == false) MessageBox.Show(string.Format("Wifi Network Setting Failed!!! WifiNetworkConnectionID={0}", networkName));
                ok &= wifi_enable;
            }

            ok &= Sineva.VHL.IF.OCS.OCSCommManager.Instance.Initialize();
            ok &= Sineva.VHL.IF.JCS.JCSCommManager.Instance.Initialize();
            ok &= Sineva.VHL.IF.GL310.SosLabGlManager.Instance.Initialize();

            ok &= Sineva.VHL.Library.IO.IoManager.Instance.Initialize();
            ok &= Sineva.VHL.Library.Servo.ServoManager.Instance.Initialize();
            ok &= Sineva.VHL.Library.MXP.MxpManager.Instance.Initialize();

            ok &= Sineva.VHL.Device.ServoControl.ServoControlManager.Instance.Initialize();
            ok &= Sineva.VHL.Device.DevicesManager.Instance.Initialize();
            ProfileInterlockPositionSetting();
            ///////////////////////////////////////////////////////
            // IPC Client를 만들자 ...!
            RemoteManager.PadInstance.Initialize(ConnectionMode.Server, CHANNEL_TYPE.TCP, "127.0.0.1");
            RemoteManager.TouchInstance.Initialize(ConnectionMode.Client, CHANNEL_TYPE.IPC, "127.0.0.1");
            //RemoteManager.PadInstance.Initialize(ConnectionMode.Client, CHANNEL_TYPE.HTTP, "127.0.0.1");

            ///////////////////////////////////////////////////////
            /// Form & Dialog Create
            m_DlgOpCallSingleMessage = new DlgOpCallSingleMessage();
            m_DlgOpCallCommand = new DlgOpCallSingleMessage();

            m_DatabaseForm = new DatabaseForm();
            m_DatabaseForm.MdiParent = this;
            m_DatabaseForm.Dock = DockStyle.Fill;
            this.btnNaviDatabase.Tag = m_DatabaseForm;
            m_NaviButtons.Add(btnNaviDatabase);
            m_DatabaseForm.Show();

            m_AlarmForm = new AlarmForm();
            m_AlarmForm.MdiParent = this;
            m_AlarmForm.Dock = DockStyle.Fill;
            this.btnNaviAlarm.Tag = m_AlarmForm;
            m_NaviButtons.Add(btnNaviAlarm);
            m_AlarmForm.Show();

            m_SystemForm = new SystemForm();
            m_SystemForm.MdiParent = this;
            m_SystemForm.Dock = DockStyle.Fill;
            this.btnNaviSystem.Tag = m_SystemForm;
            m_NaviButtons.Add(btnNaviSystem);
            m_SystemForm.Show();

            m_MotorForm = new MotorForm();
            m_MotorForm.MdiParent = this;
            m_MotorForm.Dock = DockStyle.Fill;
            this.btnNaviMotor.Tag = m_MotorForm;
            m_NaviButtons.Add(btnNaviMotor);
            m_MotorForm.Show();

            m_SetupForm = new SetupForm();
            m_SetupForm.MdiParent = this;
            m_SetupForm.Dock = DockStyle.Fill;
            this.btnNaviSetup.Tag = m_SetupForm;
            m_NaviButtons.Add(btnNaviSetup);
            m_SetupForm.Show();

            m_JobsForm = new JobsForm();
            m_JobsForm.MdiParent = this;
            m_JobsForm.Dock = DockStyle.Fill;
            this.btnNaviJobs.Tag = m_JobsForm;
            m_NaviButtons.Add(btnNaviJobs);
            m_JobsForm.Show();
            m_JobsForm.UpdateNeed = true;

            titlePanel1.Initialize();
            ///////////////////////////////////////////////////////

            ///////////////////////////////////////////////////////
            m_DlgEqpInit = new FormEqpInitialize();
            //m_DlgTpdMonitor = new FormDataDisplay();
            //m_DlgTpdMonitor.Show();

            m_DlgAutoTeaching = new FormAutoTeaching();

            return ok;
        }

        private void ProfileInterlockPositionSetting()
        {
            try
            {
                /// Profile Position Interlock Lists를 만들자!
                lock (DatabaseHandler.Instance.DictionaryPortDataList)
                {
                    Dictionary<int, List<XyztPosition>> leftProfiles = new Dictionary<int, List<XyztPosition>>();
                    Dictionary<int, List<XyztPosition>> rightProfiles = new Dictionary<int, List<XyztPosition>>();
                    List<DataItem_Port> ports = DatabaseHandler.Instance.DictionaryPortDataList.Values.Where(x => x.ProfileExistPosition != enProfileExistPosition.None).ToList();
                    foreach (DataItem_Port port in ports)
                    {
                        double profile_left = port.BarcodeLeft;
                        double profile_right = port.BarcodeRight;
                        XyztPosition intr_range = new XyztPosition();
                        if (port.ProfileExistPosition == enProfileExistPosition.FrontSide)
                        {
                            profile_left += SetupManager.Instance.SetupSafty.FrontProfileDistance;
                            profile_right += SetupManager.Instance.SetupSafty.FrontProfileDistance;
                            intr_range.X = profile_left + SetupManager.Instance.SetupSafty.FrontSideProfileUnSafeDistance;
                            intr_range.Y = profile_right + SetupManager.Instance.SetupSafty.FrontSideProfileUnSafeDistance;
                            intr_range.Z = profile_left + SetupManager.Instance.SetupSafty.RearSideProfileUnSafeDistance;
                            intr_range.T = profile_right + SetupManager.Instance.SetupSafty.RearSideProfileUnSafeDistance;
                            if (port.PortType == PortType.LeftBuffer)
                            {
                                if (leftProfiles.ContainsKey(port.LinkID))
                                {
                                    leftProfiles[port.LinkID].Add(intr_range);
                                }
                                else
                                {
                                    List<XyztPosition> temps = new List<XyztPosition>();
                                    temps.Add(intr_range);
                                    leftProfiles.Add(port.LinkID, temps);
                                }
                            }
                            else
                            {
                                if (rightProfiles.ContainsKey(port.LinkID))
                                {
                                    rightProfiles[port.LinkID].Add(intr_range);
                                }
                                else
                                {
                                    List<XyztPosition> temps = new List<XyztPosition>();
                                    temps.Add(intr_range);
                                    rightProfiles.Add(port.LinkID, temps);
                                }
                            }
                        }
                        else if (port.ProfileExistPosition == enProfileExistPosition.RearSide)
                        {
                            profile_left += SetupManager.Instance.SetupSafty.RearProfileDistance;
                            profile_right += SetupManager.Instance.SetupSafty.RearProfileDistance;
                            intr_range.X = profile_left + SetupManager.Instance.SetupSafty.RearSideProfileUnSafeDistance;
                            intr_range.Y = profile_right + SetupManager.Instance.SetupSafty.RearSideProfileUnSafeDistance;
                            intr_range.Z = profile_left + SetupManager.Instance.SetupSafty.FrontSideProfileUnSafeDistance;
                            intr_range.T = profile_right + SetupManager.Instance.SetupSafty.FrontSideProfileUnSafeDistance;
                            if (port.PortType == PortType.LeftBuffer)
                            {
                                if (leftProfiles.ContainsKey(port.LinkID))
                                {
                                    leftProfiles[port.LinkID].Add(intr_range);
                                }
                                else
                                {
                                    List<XyztPosition> temps = new List<XyztPosition>();
                                    temps.Add(intr_range);
                                    leftProfiles.Add(port.LinkID, temps);
                                }
                            }
                            else
                            {
                                if (rightProfiles.ContainsKey(port.LinkID))
                                {
                                    rightProfiles[port.LinkID].Add(intr_range);
                                }
                                else
                                {
                                    List<XyztPosition> temps = new List<XyztPosition>();
                                    temps.Add(intr_range);
                                    rightProfiles.Add(port.LinkID, temps);
                                }
                            }
                        }
                    }
                    InterlockManager.SetProfilePosition(leftProfiles, rightProfiles);
                }
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        #endregion

        #region Event
        private void Instance_NotifyErrorMessage(string val)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    DelVoid_String d = new DelVoid_String(Instance_NotifyErrorMessage);
                    this.Invoke(d, val);
                }
                else
                {
                    if (m_DlgNotifyError == null || m_DlgNotifyError.IsDisposed)
                        m_DlgNotifyError = new FormNotifyConfigError();

                    m_DlgNotifyError.ShowError(val);
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void Instance_OperateModeChanged(OperateMode opMode)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    DelVoid_OperateMode d = new DelVoid_OperateMode(Instance_OperateModeChanged);
                    this.Invoke(d, opMode);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void Instance_OperatorCalled(OperatorCallKind kind, string message)
        {
            if (!AppConfig.AppMainInitiated || AppConfig.AppMainDisposed) return;
            try
            {
                if (this.InvokeRequired)
                {
                    DelVoid_OperatorCall d = new DelVoid_OperatorCall(Instance_OperatorCalled);
                    this.Invoke(d, kind, message);
                }
                else
                {
                    switch (kind)
                    {
                        case OperatorCallKind.HostMessage:
                            {
                                if (m_DlgOpCallSingleMessage == null || m_DlgOpCallSingleMessage.IsDisposed)
                                    m_DlgOpCallSingleMessage = new DlgOpCallSingleMessage();

                                m_DlgOpCallSingleMessage.ShowMessage(message);
                            }
                            break;
                        case OperatorCallKind.OperationEnd:
                            {
                                if (m_DlgOpCallSingleMessage == null || m_DlgOpCallSingleMessage.IsDisposed)
                                    m_DlgOpCallSingleMessage = new DlgOpCallSingleMessage();

                                m_DlgOpCallSingleMessage.ShowMessage(message);
                            }
                            break;
                        case OperatorCallKind.OperationSelect:
                            {
                                if (m_DlgOpCallSingleMessage == null || m_DlgOpCallSingleMessage.IsDisposed)
                                    m_DlgOpCallSingleMessage = new DlgOpCallSingleMessage();

                                m_DlgOpCallSingleMessage.ShowMessage(message);
                            }
                            break;
                        case OperatorCallKind.OperationConfirm:
                            {
                                if (m_DlgOpCallSingleMessage == null || m_DlgOpCallSingleMessage.IsDisposed)
                                    m_DlgOpCallSingleMessage = new DlgOpCallSingleMessage();

                                GV.OperatorCallConfirm = false;
                                m_DlgOpCallSingleMessage.ShowMessage(message);
                            }
                            break;
                        case OperatorCallKind.InterlockMessage:
                            {
                                if (m_DlgOpCallSingleMessage == null || m_DlgOpCallSingleMessage.IsDisposed)
                                    m_DlgOpCallSingleMessage = new DlgOpCallSingleMessage();

                                GV.OperatorCallBuzzerOn = true;
                                m_DlgOpCallSingleMessage.ShowMessage(message);
                            }
                            break;
                        case OperatorCallKind.SaftyInterlock:
                            {
                            }
                            break;
                        case OperatorCallKind.SilencePopup:
                            {
                                if (m_DlgOpCallSingleMessage == null || m_DlgOpCallSingleMessage.IsDisposed)
                                    m_DlgOpCallSingleMessage = new DlgOpCallSingleMessage();

                                m_DlgOpCallSingleMessage.ShowMessageSilence(message);
                            }
                            break;
                        case OperatorCallKind.Hide:
                            {
                                GV.OperatorCallBuzzerOn = false;
                                m_DlgOpCallSingleMessage.Hide();
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void Instance_OperatorCallCommand(string message, string[] commands, bool show)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    DelVoid_OperatorCallCommand d = new DelVoid_OperatorCallCommand(Instance_OperatorCallCommand);
                    this.Invoke(d, message, commands, show);
                }
                else
                {
                    if (show)
                    {
                        if (m_DlgOpCallCommand == null || m_DlgOpCallCommand.IsDisposed)
                            m_DlgOpCallCommand = new DlgOpCallSingleMessage();

                        GV.OperatorCallConfirm = false;
                        m_DlgOpCallCommand.ShowMessage(message, commands);
                    }
                    else
                    {
                        m_DlgOpCallCommand.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void Instance_AutoTeachingMonitorShow(bool a)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    DelVoid_Bool d = new DelVoid_Bool(Instance_AutoTeachingMonitorShow);
                    this.Invoke(d, a);
                }
                else
                {
                    if (a)
                    {
                        if (m_DlgAutoTeaching == null || m_DlgAutoTeaching.IsDisposed)
                            m_DlgAutoTeaching = new FormAutoTeaching();
                        m_DlgAutoTeaching.Show();
                    }
                    else
                    {
                        m_DlgAutoTeaching.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void Instance_AccountLogInOut(bool logIn, Data.DbAdapter.DataItem_UserInfo account)
        {
            try
            {
                titlePanel1.SetLogin(logIn, account);
                if (logIn)
                {
                    EventLogHandler.Instance.Add("GUI", "MAIN", "Log In", true);

                    if (AccountManager.Instance.CurAccount != null)
                    {
                        this.m_MotorForm.SetUserControlAuthority(AccountManager.Instance.CurAccount.Level);
                        this.m_SetupForm.SetUserControlAuthority(AccountManager.Instance.CurAccount.Level);
                        this.m_JobsForm.SetUserControlAuthority(AccountManager.Instance.CurAccount.Level);
                        this.m_SystemForm.SetUserControlAuthority(AccountManager.Instance.CurAccount.Level);
                    }
                }
                else
                {
                    EventLogHandler.Instance.Add("GUI", "MAIN", "Log Out", true);

                    this.m_MotorForm.SetUserControlAuthority(AuthorizationLevel.Operator);
                    this.m_JobsForm.SetUserControlAuthority(AuthorizationLevel.Operator);
                    this.m_SetupForm.SetUserControlAuthority(AuthorizationLevel.Operator);
                    this.m_SystemForm.SetUserControlAuthority(AuthorizationLevel.Operator);
                }
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        #endregion

        #region UI - Event
        private void MainForm_Load(object sender, EventArgs e)
        {
            Instance_AccountLogInOut(false, null);
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_TimerExit.Enabled == false) ExitProcess();
        }

        private void btnNavi_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Tag == null) return;

            ((Form)(((Button)sender).Tag)).BringToFront();
            ((Form)(((Button)sender).Tag)).Focus();
            foreach (IFormUpdate form in this.MdiChildren)
            {
                form.UpdateNeed = false;
            }
            ((IFormUpdate)(((Button)sender).Tag)).UpdateNeed = true;

            //this.lblViewName.Text = ((IButton)sender).Description;
            //titlePanel1.SetViewName(((IButton)sender).Description);

            this.btnNaviJobs.SetCheck(false);
            this.btnNaviAlarm.SetCheck(false);
            this.btnNaviMotor.SetCheck(false);
            this.btnNaviSetup.SetCheck(false);
            this.btnNaviSystem.SetCheck(false);
            this.btnNaviDatabase.SetCheck(false);

            ((IButton)sender).SetCheck(true);

            // Button Color Setting
            this.btnNaviJobs.BackColor = Color.Transparent;
            this.btnNaviAlarm.BackColor = Color.Transparent;
            this.btnNaviMotor.BackColor = Color.Transparent;
            this.btnNaviSetup.BackColor = Color.Transparent;
            this.btnNaviSystem.BackColor = Color.Transparent;
            this.btnNaviDatabase.BackColor = Color.Transparent;

            foreach (ServoUnit servo in ServoManager.Instance.ServoUnits) servo.Repeat = false;

            foreach (IButton btn in m_NaviButtons)
            {
                btn.BackColor = Color.Transparent;
                btn.SetConnectedLableOff();
            }
            ((IButton)sender).SetConnectedLableOn();
        }
        private void btnEms_Click(object sender, EventArgs e)
        {
            //int almId = 0;
            foreach (ServoUnit servo in ServoManager.Instance.ServoUnits)
            {
                servo.Stop();
            }
            ProcessDataHandler.Instance._SaveCurState = true;
            m_EqpManager.SetRunMode(EqpRunMode.Stop);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            EventLogHandler.Instance.Add("GUI", "MAIN", "Exit pressed!", true);
            if (MessageBox.Show("Exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == System.Windows.Forms.DialogResult.Yes)
            {
                timerUpdate.Enabled = false;

                EventLogHandler.Instance.Add("GUI", "MAIN", "Exit!", false);
                ExitProcess();
            }
        }
        private void ExitProcess()
        {
            XSequence.StopGraceful();
            if (m_TouchGuiWorker != null) m_TouchGuiWorker.CancelAsync();
            if (SetupManager.Instance.SetupOperation.NetworkControlUse == Use.Use)
                Sineva.VHL.Library.SimpleWifi.NetworkManager.Instance.Destory();
            Process[] process_gui = Process.GetProcessesByName("Sineva.VHL.GUI.Touch");
            if (process_gui.Length > 0) foreach (Process p in process_gui) p.Kill();

            // Kill All Timers
            m_JobsForm.KillTimer();
            m_MotorForm.KillTimer();
            m_SystemForm.KillTimer();
            m_AlarmForm.KillTimer();
            m_DatabaseForm.KillTimer();

            m_TimerExit = new System.Windows.Forms.Timer();
            m_TimerExit.Interval = 2000;
            m_TimerExit.Enabled = true;
            m_TimerExit.Tick += m_TimerExit_Tick;
            SplashScreen form = new SplashScreen(Sineva.VHL.GUI.Properties.Resources.logo, 5000);
            form.ShowDialog();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                StartRestoreTimer(); 
            }
        }
        private void StartRestoreTimer()
        {
            if (m_restoreTimer == null)
            {
                m_restoreTimer = new System.Timers.Timer(60000); 
                m_restoreTimer.Elapsed += RestoreForm;
                m_restoreTimer.AutoReset = false;
            }
            m_restoreTimer.Start();
        }

        private void RestoreForm(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => this.WindowState = FormWindowState.Maximized));
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        #endregion

        #region Methods
        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            if (m_EqpManager.EqpInitIng && !m_DlgEqpInit.Visible) m_DlgEqpInit.Show();
            else if (!m_EqpManager.EqpInitIng && m_DlgEqpInit.Visible) m_DlgEqpInit.Hide();
        }

        void m_TimerExit_Tick(object sender, EventArgs e)
        {
            m_TimerExit.Enabled = false;
            TaskHandler.Instance.AbortTask();
            Exit();
        }
        private void Exit()
        {
            AppConfig.AppMainDisposed = true;
            this.Dispose();
            //Application.Exit();
            SequenceLog.WriteLog(string.Format("Program Exit !"));
        }
        #endregion

        #region Background Work
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            string[] info = (string[])e.Argument;

            uint startTicks = 0;
            int seqNo = 0;
            bool init_state = true;
            try
            {
                bool connect = true;
                while (connect)
                {
                    switch (seqNo)
                    {
                        case 0:
                            {
                                if (RemoteManager.TouchInstance.Conneted && !GV.ThreadStop)
                                {
                                    if (RemoteManager.TouchInstance.Remoting.TouchGUI.HeartBitAlarm)
                                    {
                                        RemoteManager.TouchInstance.Remoting.TouchGUI.HeartBitAlarm = false;
                                        GV.TouchHeartBitError = true;
                                    }
                                    startTicks = XFunc.GetTickCount();
                                    seqNo = 10;
                                }
                                else
                                {
                                    startTicks = XFunc.GetTickCount();
                                    seqNo = 2000;
                                }
                            }
                            break;

                        case 10:
                            {
                                if (XFunc.GetTickCount() - startTicks > 200)
                                {
                                    RemoteManager.TouchInstance.Remoting.TouchGUI.HeartBitRun = true;
                                    RemoteManager.TouchInstance.Remoting.TouchGUI.HeartBitCount++;
                                    UpdateRemoteTouch();
                                    seqNo = 0;
                                }
                            }
                            break;
                        case 2000:
                            {
                                if (XFunc.GetTickCount() - startTicks > 5000)
                                {
                                    seqNo = 0;
                                }
                            }
                            break;
                    }

                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }

            if (init_state == true)
                e.Result = new string[] { info[0], "Connected" };
            else e.Result = new string[] { info[0], "Disconnected" };
        }

        private void DoActionWork(object sender, DoWorkEventArgs e)
        {
            string[] info = (string[])e.Argument;

            uint startTicks = 0;
            int seqNo = 0;
            bool init_state = true;
            try
            {
                bool connect = true;
                while (connect)
                {
                    switch (seqNo)
                    {
                        case 0:
                            {
                                if (RemoteManager.TouchInstance.Conneted && !GV.ThreadStop)
                                {
                                    startTicks = XFunc.GetTickCount();
                                    seqNo = 10;
                                }
                                else
                                {
                                    startTicks = XFunc.GetTickCount();
                                    seqNo = 2000;
                                }
                            }
                            break;

                        case 10:
                            {
                                if (XFunc.GetTickCount() - startTicks > 200)
                                {
                                    seqNo = 100;
                                }
                            }
                            break;
                        case 100:
                            {
                                if (RemoteManager.TouchInstance.Conneted && !GV.ThreadStop)
                                {
                                    if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteAction != WebActionType.None)
                                    {
                                        seqNo = 200;
                                    }
                                    else
                                    {
                                        seqNo = 0;
                                    }
                                }
                                else
                                {
                                    seqNo = 0;
                                }
                            }
                            break;
                        case 200:
                            {
                                switch (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteAction)
                                {
                                    case WebActionType.None:
                                        seqNo = 1000;
                                        break;
                                    case WebActionType.JogPlus:
                                    case WebActionType.JogMinus:
                                    case WebActionType.JogStop:
                                    case WebActionType.StepMove:
                                    case WebActionType.ServoOn:
                                    case WebActionType.ServoOff:
                                    case WebActionType.ErrorReset:
                                    case WebActionType.Origin:
                                    case WebActionType.MoveAxis:
                                        seqNo = 600;
                                        break;
                                    case WebActionType.SteerLeft:
                                        seqNo = 700;
                                        break;
                                    case WebActionType.SteerRight:
                                        seqNo = 710;
                                        break;
                                    case WebActionType.AntiDropLock:
                                        seqNo = 720;
                                        break;
                                    case WebActionType.AntiDropUnLock:
                                        seqNo = 730;
                                        break;
                                    case WebActionType.GripperClose:
                                        seqNo = 740;
                                        break;
                                    case WebActionType.GripperOpen:
                                        seqNo = 750;
                                        break;
                                    case WebActionType.Manual:
                                        seqNo = 500;
                                        break;
                                    case WebActionType.Auto:
                                        seqNo = 510;
                                        break;
                                    case WebActionType.Ready:
                                        seqNo = 520;
                                        break;
                                    case WebActionType.Abort:
                                        seqNo = 560;
                                        break;
                                    case WebActionType.Stop:
                                        seqNo = 530;
                                        break;
                                    case WebActionType.Start:
                                        seqNo = 540;
                                        break;
                                    case WebActionType.Pause:
                                        seqNo = 550;
                                        break;
                                    case WebActionType.OffsetUpdate:
                                        seqNo = 400;
                                        break;
                                    case WebActionType.CommandAdd:
                                        seqNo = 410;
                                        break;
                                    case WebActionType.CommandDelete:
                                        seqNo = 420;
                                        break;
                                    case WebActionType.OpCallConfirm:
                                        seqNo = 430;
                                        break;
                                    case WebActionType.OpCallBuzzerOff:
                                        seqNo = 440;
                                        break;
                                    case WebActionType.CarrierInstall:
                                        seqNo = 450;
                                        break;
                                    case WebActionType.CarrierNone:
                                        seqNo = 460;
                                        break;
                                    case WebActionType.DataRefresh:
                                        seqNo = 470;
                                        break;
                                    default:
                                        seqNo = 1000;
                                        break;
                                }
                                startTicks = XFunc.GetTickCount();
                            }
                            break;
                        case 400:
                            {
                                int port = RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.OffsetUpdate.PortID;
                                if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(port))
                                {
                                    DataItem_Port newTeachingData = DatabaseHandler.Instance.DictionaryPortDataList[port];
                                    newTeachingData.DriveLeftOffset = RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.OffsetUpdate.DriveLeftOffset;
                                    newTeachingData.DriveRightOffset = RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.OffsetUpdate.DriveRightOffset;
                                    newTeachingData.HoistOffset = RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.OffsetUpdate.HoistOffset;
                                    newTeachingData.SlideOffset = RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.OffsetUpdate.SlideOffset;
                                    newTeachingData.RotateOffset = RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.OffsetUpdate.RotateOffset;
                                    DatabaseHandler.Instance.UpdatePort(newTeachingData);
                                    RemoteManager.TouchInstance.Remoting.TouchGUI.ResponseMsg = $"Update PortID:{port} success!";
                                }
                                else
                                {
                                    RemoteManager.TouchInstance.Remoting.TouchGUI.ResponseMsg = $"PortID:{port} not exist,Update fail!";
                                }
                                seqNo = 1000;
                            }
                            break;
                        case 410:
                            {
                                var command = RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.WebCommand;
                                var newCommand = new Command
                                {
                                    CommandID = command.CommandID,
                                    CassetteID = command.CassetteID,
                                    ProcessCommand = (OCSCommand)command.CommandType,
                                    SourceID = command.SourceID,
                                    DestinationID = command.DestinationID,
                                    TargetNodeToDistance = command.TargetNodeToDistance,
                                    TypeOfDestination = (enGoCommandType)command.TypeOfDestination,
                                    IsValid = command.IsValid,
                                };
                                bool exist = ProcessDataHandler.Instance.TransferCommands.Select(x => x.CommandID).Contains(newCommand.CommandID);
                                if (!exist)
                                {
                                    ProcessDataHandler.Instance.CreateTransferCommand(newCommand);
                                    RemoteManager.TouchInstance.Remoting.TouchGUI.ResponseMsg = $"Create TransferCommand:{command.CommandID} success!";
                                }
                                RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.WebCommand = new WebCommand();
                                seqNo = 1000;
                            }
                            break;
                        case 420:
                            {
                                var command = RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.WebCommand;
                                var newCommand = new Command
                                {
                                    CommandID = command.CommandID,
                                    CassetteID = command.CassetteID,
                                    ProcessCommand = (OCSCommand)command.CommandType,
                                    SourceID = command.SourceID,
                                    DestinationID = command.DestinationID,
                                    TargetNodeToDistance = command.TargetNodeToDistance,
                                    TypeOfDestination = (enGoCommandType)command.TypeOfDestination,
                                    IsValid = command.IsValid,
                                };
                                bool exist = ProcessDataHandler.Instance.TransferCommands.Select(x => x.CommandID).Contains(newCommand.CommandID);
                                if (exist)
                                {
                                    ProcessDataHandler.Instance.DeleteTransferCommand(newCommand);
                                    RemoteManager.TouchInstance.Remoting.TouchGUI.ResponseMsg = $"Delete TransferCommand:{command.CommandID} success!";
                                }
                                RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.WebCommand = new WebCommand();
                                seqNo = 1000;
                            }
                            break;
                        case 430:
                            {
                                GV.OperatorCallBuzzerOn = false;
                                GV.OperatorCallConfirm = true;
                                if (m_DlgOpCallSingleMessage != null)
                                {
                                    m_DlgOpCallSingleMessage.Visible = false;
                                }
                                seqNo = 1000;
                            }
                            break;
                        case 440:
                            {
                                GV.OperatorCallBuzzerOn = false;
                                seqNo = 1000;
                            }
                            break;
                        case 450:
                            {
                                ProcessDataHandler.Instance.CurVehicleStatus.SetCarrierStatus(CarrierState.Installed);
                                seqNo = 1000;
                            }
                            break;
                        case 460:
                            {
                                ProcessDataHandler.Instance.CurVehicleStatus.SetCarrierStatus(CarrierState.None);
                                seqNo = 1000;
                            }
                            break;
                        case 470:
                            {
                                int port = RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.OffsetUpdate.PortID;
                                if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(port))
                                {
                                    var m_port = DatabaseHandler.Instance.QueryPort.SelectSingleOrNullByPortId(port);
                                    DatabaseHandler.Instance.DictionaryPortDataList[port].BarcodeLeft = m_port.BarcodeLeft;
                                    DatabaseHandler.Instance.DictionaryPortDataList[port].BarcodeRight = m_port.BarcodeRight;
                                    DatabaseHandler.Instance.DictionaryPortDataList[port].HoistPosition = m_port.HoistPosition;
                                    DatabaseHandler.Instance.DictionaryPortDataList[port].SlidePosition = m_port.SlidePosition;
                                    DatabaseHandler.Instance.DictionaryPortDataList[port].RotatePosition = m_port.RotatePosition;
                                    DatabaseHandler.Instance.DictionaryPortDataList[port].UnloadHoistPosition = m_port.UnloadHoistPosition;
                                    DatabaseHandler.Instance.DictionaryPortDataList[port].UnloadSlidePosition = m_port.UnloadSlidePosition;
                                    DatabaseHandler.Instance.DictionaryPortDataList[port].UnloadRotatePosition = m_port.UnloadRotatePosition;
                                }
                                else
                                {
                                    RemoteManager.TouchInstance.Remoting.TouchGUI.ResponseMsg = $"PortID:{port} not exist,Update fail!";
                                }
                                seqNo = 1000;
                            }
                            break;
                        #region State Change
                        case 500:
                            {
                                ChangeMode?.Invoke(OperateMode.Manual);
                                seqNo = 1000;
                            }
                            break;
                        case 510:
                            {
                                ChangeMode?.Invoke(OperateMode.Auto);
                                seqNo = 1000;

                            }
                            break;
                        case 520:
                            {
                                if (!m_EqpManager.EqpInitIng && !m_EqpManager.EqpInitComp)
                                {
                                    m_EqpManager.EqpInitReq = true;
                                }
                                seqNo = 1000;

                            }
                            break;
                        case 530:
                            {
                                ChangeRunMode?.Invoke(EqpRunMode.Stop);
                                seqNo = 1000;

                            }
                            break;
                        case 540:
                            {
                                ChangeRunMode?.Invoke(EqpRunMode.Start);
                                seqNo = 1000;

                            }
                            break;
                        case 550:
                            {
                                ChangeRunMode?.Invoke(EqpRunMode.Pause);
                                seqNo = 1000;

                            }
                            break;
                        case 560:
                            {
                                ChangeRunMode?.Invoke(EqpRunMode.Abort);
                                seqNo = 1000;

                            }
                            break;
                        #endregion

                        #region Servo Control
                        case 600:
                            {
                                enAxisCoord enAxis = enAxisCoord.X;
                                switch (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice)
                                {
                                    case WebDeviceType.None:
                                        break;
                                    case WebDeviceType.MasterTransfer:
                                        enAxis = enAxisCoord.X;
                                        set.AxisCoord = enAxisCoord.X;
                                        break;
                                    case WebDeviceType.Slide:
                                        enAxis = enAxisCoord.Y;
                                        set.AxisCoord = enAxisCoord.Y;
                                        break;
                                    case WebDeviceType.Rotate:
                                        enAxis = enAxisCoord.T;
                                        set.AxisCoord = enAxisCoord.T;
                                        break;
                                    case WebDeviceType.Hoist:
                                        enAxis = enAxisCoord.Z;
                                        set.AxisCoord = enAxisCoord.Z;
                                        break;
                                    default:
                                        break;
                                }

                                foreach (_Axis axis in ServoManager.Instance.AxisSource)
                                {
                                    if (axis.AxisCoord == enAxis)
                                    {
                                        if (axis.CommandSkip) continue;
                                        currentAxis = axis;
                                        break;
                                    }
                                }
                                if (currentAxis == null)
                                {
                                    seqNo = 1000;
                                }
                                else
                                {
                                    foreach (ServoUnit unit in ServoManager.Instance.ServoUnits)
                                    {
                                        if (unit == null) continue;
                                        if (unit.Axes.Contains(currentAxis))
                                        {
                                            currentServoUnit = unit;
                                            break;
                                        }
                                    }
                                    if (currentServoUnit == null)
                                    {
                                        seqNo = 1000;
                                    }
                                    else
                                    {
                                        set.Vel = RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteVelocity.Velocity;
                                        set.Acc = currentAxis.JogAcc > 0.0f ? currentAxis.JogAcc : set.Vel;
                                        set.Dec = currentAxis.JogDec > 0.0f ? currentAxis.JogDec : set.Vel;
                                        set.Jerk = currentAxis.JogJerk > 0.0f ? currentAxis.JogJerk : set.Vel;
                                        seqNo = 605;
                                    }
                                }
                            }
                            break;
                        case 605:
                            {
                                switch (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteAction)
                                {
                                    case WebActionType.None:
                                        seqNo = 1000;
                                        break;
                                    case WebActionType.JogPlus:
                                        seqNo = 610;
                                        break;
                                    case WebActionType.JogMinus:
                                        seqNo = 615;
                                        break;
                                    case WebActionType.JogStop:
                                        seqNo = 620;
                                        break;
                                    case WebActionType.ServoOn:
                                        seqNo = 625;
                                        break;
                                    case WebActionType.ServoOff:
                                        seqNo = 630;
                                        break;
                                    case WebActionType.ErrorReset:
                                        seqNo = 635;
                                        break;
                                    case WebActionType.Origin:
                                        seqNo = 640;
                                        break;
                                    case WebActionType.StepMove:
                                        seqNo = 645;
                                        break;
                                    case WebActionType.MoveAxis:
                                        seqNo = 650;
                                        break;
                                    default:
                                        seqNo = 1000;
                                        break;
                                }
                            }
                            break;
                        case 610:
                            {
                                currentServoUnit.JogMove(currentAxis, enAxisOutFlag.JogPlus, set);
                                bool stop = false;
                                stop |= !RemoteManager.TouchInstance.Conneted;
                                stop |= RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteAction == WebActionType.None;
                                stop |= RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteAction == WebActionType.JogStop;

                                stop |= DateTime.Now - RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.UpdateTime > TimeSpan.FromSeconds(2);
                                if (stop)
                                {
                                    RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteAction = WebActionType.JogStop;
                                    seqNo = 620;
                                }
                            }
                            break;
                        case 615:
                            {
                                currentServoUnit.JogMove(currentAxis, enAxisOutFlag.JogMinus, set);
                                bool stop = false;
                                stop |= !RemoteManager.TouchInstance.Conneted;
                                stop |= RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteAction == WebActionType.None;
                                stop |= RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteAction == WebActionType.JogStop;
                                stop |= DateTime.Now - RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.UpdateTime > TimeSpan.FromSeconds(2);
                                if (stop)
                                {
                                    RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteAction = WebActionType.JogStop;
                                    seqNo = 620;
                                }
                                break;
                            }
                        case 620:
                            {
                                currentServoUnit.Stop(currentAxis);
                                currentServoUnit.ResetJogSpeed();
                                //currentServoUnit.ResetCommand();
                                seqNo = 1000;
                            }
                            break;
                        case 625:
                            {
                                currentServoUnit.ServoOn(currentAxis);
                                seqNo = 1000;
                            }
                            break;
                        case 630:
                            {
                                currentServoUnit.ServoOff(currentAxis);
                                seqNo = 1000;
                            }
                            break;
                        case 635:
                            {
                                currentServoUnit.AlarmClear(currentAxis);
                                seqNo = 1000;
                            }
                            break;
                        case 640:
                            {
                                currentServoUnit.Home(currentAxis);
                                seqNo = 1000;
                            }
                            break;
                        case 645:
                            {
                                currentServoUnit.MoveRelativeStart(currentAxis, RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteVelocity.Distance, set);
                                seqNo = 1000;
                            }
                            break;
                        case 650:
                            {
                                currentServoUnit.MoveAxisStart(currentAxis, RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteVelocity.Distance, set);
                                seqNo = 1000;
                            }
                            break;
                        #endregion
                        #region Device Action
                        case 700:
                            {
                                if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice == WebDeviceType.AllSteer)
                                {
                                    int rv1 = DevicesManager.Instance.DevSteer.FrontLeft();
                                    if (rv1 >= 0)
                                    {
                                        DevicesManager.Instance.DevSteer.FrontSteer.ResetOutput();
                                    }
                                    int rv2 = DevicesManager.Instance.DevSteer.RearLeft();
                                    if (rv2 >= 0)
                                    {
                                        DevicesManager.Instance.DevSteer.RearSteer.ResetOutput();
                                    }
                                    if (rv1 >= 0 && rv2 >= 0)
                                    {
                                        seqNo = 1000;
                                    }
                                }
                                else if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice == WebDeviceType.FrontSteer)
                                {
                                    int rv1 = DevicesManager.Instance.DevSteer.FrontLeft();
                                    if (rv1 >= 0)
                                    {
                                        DevicesManager.Instance.DevSteer.FrontSteer.ResetOutput();
                                        seqNo = 1000;

                                    }
                                }
                                else if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice == WebDeviceType.RearSteer)
                                {
                                    int rv1 = DevicesManager.Instance.DevSteer.RearLeft();
                                    if (rv1 >= 0)
                                    {
                                        DevicesManager.Instance.DevSteer.RearSteer.ResetOutput();
                                        seqNo = 1000;

                                    }
                                }
                                else if (XFunc.GetTickCount() - startTicks > 2000)
                                {
                                    seqNo = 1000;
                                }
                            }
                            break;
                        case 710:
                            {
                                if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice == WebDeviceType.AllSteer)
                                {
                                    int rv1 = DevicesManager.Instance.DevSteer.FrontRight();
                                    if (rv1 >= 0)
                                    {
                                        DevicesManager.Instance.DevSteer.FrontSteer.ResetOutput();
                                    }
                                    int rv2 = DevicesManager.Instance.DevSteer.RearRight();
                                    if (rv2 >= 0)
                                    {
                                        DevicesManager.Instance.DevSteer.RearSteer.ResetOutput();
                                    }
                                    if (rv1 >= 0 && rv2 >= 0)
                                    {
                                        seqNo = 1000;
                                    }
                                }
                                else if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice == WebDeviceType.FrontSteer)
                                {
                                    int rv1 = DevicesManager.Instance.DevSteer.FrontRight();
                                    if (rv1 >= 0)
                                    {
                                        DevicesManager.Instance.DevSteer.FrontSteer.ResetOutput();
                                        seqNo = 1000;

                                    }
                                }
                                else if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice == WebDeviceType.RearSteer)
                                {
                                    int rv1 = DevicesManager.Instance.DevSteer.RearRight();
                                    if (rv1 >= 0)
                                    {
                                        DevicesManager.Instance.DevSteer.RearSteer.ResetOutput();
                                        seqNo = 1000;

                                    }
                                }
                                else if (XFunc.GetTickCount() - startTicks > 2000)
                                {
                                    seqNo = 1000;
                                }
                            }
                            break;
                        case 720:
                            {
                                if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice == WebDeviceType.AllAntiDrop)
                                {
                                    int rv1 = DevicesManager.Instance.DevFrontAntiDrop.Lock();
                                    int rv2 = DevicesManager.Instance.DevRearAntiDrop.Lock();
                                    if (rv1 >= 0 && rv2 >= 0)
                                    {
                                        seqNo = 1000;
                                    }
                                }
                                else if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice == WebDeviceType.FrontAntiDrop)
                                {
                                    int rv1 = DevicesManager.Instance.DevFrontAntiDrop.Lock();
                                    if (rv1 >= 0)
                                    {
                                        seqNo = 1000;
                                    }
                                }
                                else if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice == WebDeviceType.RearAntiDrop)
                                {
                                    int rv1 = DevicesManager.Instance.DevRearAntiDrop.Lock();
                                    if (rv1 >= 0)
                                    {
                                        seqNo = 1000;
                                    }
                                }
                                else if (XFunc.GetTickCount() - startTicks > 2000)
                                {
                                    seqNo = 1000;
                                }
                            }
                            break;
                        case 730:
                            {
                                if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice == WebDeviceType.AllAntiDrop)
                                {
                                    int rv1 = DevicesManager.Instance.DevFrontAntiDrop.Unlock();
                                    int rv2 = DevicesManager.Instance.DevRearAntiDrop.Unlock();
                                    if (rv1 >= 0 && rv2 >= 0)
                                    {
                                        seqNo = 1000;
                                    }
                                    else if (XFunc.GetTickCount() - startTicks > 2000)
                                    {
                                        seqNo = 1000;
                                    }
                                }
                                else if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice == WebDeviceType.FrontAntiDrop)
                                {
                                    int rv1 = DevicesManager.Instance.DevFrontAntiDrop.Unlock();
                                    if (rv1 >= 0)
                                    {
                                        seqNo = 1000;
                                    }
                                    else if (XFunc.GetTickCount() - startTicks > 2000)
                                    {
                                        seqNo = 1000;
                                    }
                                }
                                else if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice == WebDeviceType.RearAntiDrop)
                                {
                                    int rv1 = DevicesManager.Instance.DevRearAntiDrop.Unlock();
                                    if (rv1 >= 0)
                                    {
                                        seqNo = 1000;
                                    }
                                    else if (XFunc.GetTickCount() - startTicks > 2000)
                                    {
                                        seqNo = 1000;
                                    }
                                }
                                else if (XFunc.GetTickCount() - startTicks > 2000)
                                {
                                    seqNo = 1000;
                                }
                            }
                            break;
                        case 740:
                            {
                                if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice == WebDeviceType.Gripper)
                                {
                                    int rv1 = DevicesManager.Instance.DevGripperPIO.GripperClose();
                                    if (rv1 >= 0)
                                    {
                                        seqNo = 1000;
                                    }
                                }
                                else if (XFunc.GetTickCount() - startTicks > 2000)
                                {
                                    seqNo = 1000;
                                }
                            }
                            break;
                        case 750:
                            {
                                if (RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.RemoteDevice == WebDeviceType.Gripper)
                                {
                                    int rv1 = DevicesManager.Instance.DevGripperPIO.GripperOpen();
                                    if (rv1 >= 0)
                                    {
                                        seqNo = 1000;
                                    }
                                }
                                else if (XFunc.GetTickCount() - startTicks > 2000)
                                {
                                    seqNo = 1000;
                                }
                            }
                            break;
                        #endregion
                        case 1000:
                            {
                                RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction = new WebAction(WebDeviceType.None, WebActionType.None);
                                seqNo = 0;
                            }
                            break;
                        case 2000:
                            {
                                if (XFunc.GetTickCount() - startTicks > 5000)
                                {
                                    seqNo = 0;
                                }
                            }
                            break;
                    }

                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }

            if (init_state == true)
                e.Result = new string[] { info[0], "Connected" };
            else e.Result = new string[] { info[0], "Disconnected" };
        }
        private void UpdateRemoteTouch()
        {
            try
            {
                if (RemoteManager.TouchInstance.Conneted /*&& false*/)
                {
                    RemoteManager.TouchInstance.Remoting.TouchGUI.OpMode = EqpStateManager.Instance.OpMode;
                    RemoteManager.TouchInstance.Remoting.TouchGUI.EqState = EqpStateManager.Instance.State;
                    RemoteManager.TouchInstance.Remoting.TouchGUI.Velocity = ProcessDataHandler.Instance.CurVehicleStatus.MasterWheelVelocity;
                    RemoteManager.TouchInstance.Remoting.TouchGUI.ObsUpArea = (int)DevicesManager.Instance.DevSOSUpper.GetOBS();
                    int obs = (int)DevicesManager.Instance.DevSOSUpper.GetFrontDetectState();
                    RemoteManager.TouchInstance.Remoting.TouchGUI.ObsUp1 = ((obs >> 0) & 0x01) == 0x01 ? true : false;
                    RemoteManager.TouchInstance.Remoting.TouchGUI.ObsUp2 = ((obs >> 1) & 0x01) == 0x01 ? true : false;
                    RemoteManager.TouchInstance.Remoting.TouchGUI.ObsUp3 = ((obs >> 2) & 0x01) == 0x01 ? true : false;
                    RemoteManager.TouchInstance.Remoting.TouchGUI.ObsUp4 = ((obs >> 3) & 0x01) == 0x01 ? true : false;
                    RemoteManager.TouchInstance.Remoting.TouchGUI.ObsDownArea = (int)DevicesManager.Instance.DevOBSLower.GetOBS();
                    RemoteManager.TouchInstance.Remoting.TouchGUI.ObsDown1 = DevicesManager.Instance.DevOBSLower.GetFrontDetectState() == enFrontDetectState.enDeccelation1;
                    RemoteManager.TouchInstance.Remoting.TouchGUI.ObsDown2 = DevicesManager.Instance.DevOBSLower.GetFrontDetectState() == enFrontDetectState.enDeccelation2;
                    RemoteManager.TouchInstance.Remoting.TouchGUI.ObsDown3 = DevicesManager.Instance.DevOBSLower.GetFrontDetectState() == enFrontDetectState.enDeccelation3;
                    RemoteManager.TouchInstance.Remoting.TouchGUI.ObsDown4 = DevicesManager.Instance.DevOBSLower.GetFrontDetectState() == enFrontDetectState.enDeccelation4;
                    RemoteManager.TouchInstance.Remoting.TouchGUI.Destination = ProcessDataHandler.Instance.CurTransferCommand.GetTransferInformation().Destination;
                    RemoteManager.TouchInstance.Remoting.TouchGUI.CarrierID = ProcessDataHandler.Instance.CurTransferCommand.GetTransferInformation().CarrierID;
                    WebGUI webGUI = new WebGUI();
                    List<ServoStatus> servoStatusList = new List<ServoStatus>();
                    for (int i = 0; i < ServoManager.Instance.AxisSource.Count; i++)
                    {
                        var servoStatus = new ServoStatus();
                        var axis = ServoManager.Instance.AxisSource[i] as MpAxis;
                        ushort decimalPoint = axis.DecimalPoint;
                        servoStatus.AxisId = axis.AxisId;
                        servoStatus.AxisName = axis.AxisName;
                        servoStatus.Position = axis.GetAxisCurPos();
                        servoStatus.Speed = axis.GetAxisCurSpeed();
                        enAxisInFlag axisStatus = axis.GetAxisCurStatus();
                        servoStatus.IsAlarm = (axisStatus & enAxisInFlag.Alarm) == enAxisInFlag.Alarm /*&& !item.CommandSkip*/;
                        servoStatus.InPosition = (axisStatus & enAxisInFlag.InPos) == enAxisInFlag.InPos/* && !item.CommandSkip*/;
                        servoStatus.HomeEnd = (axisStatus & enAxisInFlag.HEnd) == enAxisInFlag.HEnd /*&& !item.CommandSkip*/;
                        servoStatus.ServoOn = (axisStatus & enAxisInFlag.SvOn) == enAxisInFlag.SvOn;
                        servoStatus.IsOrg = (axisStatus & enAxisInFlag.Org) == enAxisInFlag.Org;
                        List<int> con_list = axis.ControllerAlarmIdList.FindAll(n => n != 0);
                        List<int> dri_list = axis.DriverAlarmIdList.FindAll(n => n != 0);
                        if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);
                        string alarmlist = string.Format("{0},{1}", string.Join("-", con_list), string.Join("-", dri_list));
                        servoStatus.AlarmCode = $"{alarmlist},{axis.SequenceState.SequenceAlarmId}";
                        servoStatusList.Add(servoStatus);
                    }
                    List<IoStatus> ioStatuses = new List<IoStatus>();
                    for (int i = 0; i < IoManager.DiChannels.Count; i++)
                    {
                        var iostatus = new IoStatus();
                        iostatus.ID = IoManager.DiChannels[i].Id;
                        iostatus.Name = IoManager.DiChannels[i].Name;
                        iostatus.IoType = IoManager.DiChannels[i].IoType;
                        iostatus.Description = IoManager.DiChannels[i].Description;
                        iostatus.WiringNo = IoManager.DiChannels[i].WiringNo;
                        iostatus.State = IoManager.DiChannels[i].State;
                        iostatus.IsBContact = IoManager.DiChannels[i].IsBContact;
                        ioStatuses.Add(iostatus);
                    }
                    for (int i = 0; i < IoManager.DoChannels.Count; i++)
                    {
                        var iostatus = new IoStatus();
                        iostatus.ID = IoManager.DoChannels[i].Id;
                        iostatus.Name = IoManager.DoChannels[i].Name;
                        iostatus.IoType = IoManager.DoChannels[i].IoType;
                        iostatus.Description = IoManager.DoChannels[i].Description;
                        iostatus.WiringNo = IoManager.DoChannels[i].WiringNo;
                        iostatus.State = IoManager.DoChannels[i].State;
                        iostatus.IsBContact = IoManager.DoChannels[i].IsBContact;
                        ioStatuses.Add(iostatus);
                    }
                    List<TeachingPortData> teachingPortDatas = new List<TeachingPortData>();
                    for (int i = 0; i < DatabaseHandler.Instance.DictionaryPortDataList.Values.Count; i++)
                    {
                        var data = new TeachingPortData();
                        var item = DatabaseHandler.Instance.DictionaryPortDataList.Values.ToList()[i];
                        data.BarcodeLeft = item.BarcodeLeft;
                        data.BarcodeRight = item.BarcodeRight;
                        data.BeforeHoistPosition = item.BeforeHoistPosition;
                        data.BeforeUnloadHoistPosition = item.BeforeUnloadHoistPosition;
                        data.DriveLeftOffset = item.DriveLeftOffset;
                        data.DriveRightOffset = item.DriveRightOffset;
                        data.HoistOffset = item.HoistOffset;
                        data.HoistPosition = item.HoistPosition;
                        data.LinkID = item.LinkID;
                        data.NodeID = item.NodeID;
                        data.OffsetUsed = item.OffsetUsed;
                        data.PBSSelectNo = item.PBSSelectNo;
                        data.PBSUsed = item.PBSUsed;
                        data.PIOCH = item.PIOCH;
                        data.PIOCS = item.PIOCS;
                        data.PIOID = item.PIOID;
                        data.PIOUsed = item.PIOUsed;
                        data.PortID = item.PortID;
                        data.PortProhibition = item.PortProhibition;
                        data.PortType = (int)item.PortType;
                        data.ProfileExistPosition = (int)item.ProfileExistPosition;
                        data.RotateOffset = item.RotateOffset;
                        data.RotatePosition = item.RotatePosition;
                        data.SlideOffset = item.SlideOffset;
                        data.SlidePosition = item.SlidePosition;
                        data.State = item.State;
                        data.UnloadHoistPosition = item.UnloadHoistPosition;
                        data.UnloadRotatePosition = item.UnloadRotatePosition;
                        data.UnloadSlidePosition = item.UnloadSlidePosition;
                        teachingPortDatas.Add(data);
                    }
                    List<CommandData> commandDatas = new List<CommandData>();
                    for (int i = 0; i < ProcessDataHandler.Instance.TransferCommands.Count; i++)
                    {
                        CommandData data = new CommandData();
                        var item = ProcessDataHandler.Instance.TransferCommands[i];
                        data.CassetteID = item.CassetteID;
                        data.CommandID = item.CommandID;
                        data.DestinationID = item.DestinationID;
                        data.IsValid = item.IsValid;
                        data.SourceID = item.SourceID;
                        data.ProcessCommand = (int)item.ProcessCommand;
                        data.TypeOfDestination = item.TypeOfDestination;
                        data.TargetNodeToDistance = item.TargetNodeToDistance;
                        commandDatas.Add(data);
                    }
                    List<IniTag> iniTags = new List<IniTag>();
                    var tags = m_EqpManager.GetEqpInitState(out bool update);
                    for (int i = 0; i < tags.Count(); i++)
                    {
                        IniTag tag = new IniTag();
                        tag.ItemName = tags[i].ItemName;
                        tag.CheckStatus = tags[i].CheckStatus;
                        tag.State = tags[i].State;
                        iniTags.Add(tag);
                    }
                    webGUI.ServoStatusList = servoStatusList;
                    webGUI.TransferCommands = commandDatas;
                    webGUI.IOList = ioStatuses;
                    webGUI.TeachingPortDataList = teachingPortDatas;
                    webGUI.IniTags = iniTags;
                    webGUI.EqpInitComp = EqpStateManager.Instance.EqpInitComp;
                    webGUI.EqpInitIng = EqpStateManager.Instance.EqpInitIng;
                    webGUI.IsAutoTeachingMode = GV.AutoTeachingModeOn;
                    webGUI.FrontSteerDirection = DevicesManager.Instance.DevSteer.GetFrontSteerDirection();
                    webGUI.RearSteerDirection = DevicesManager.Instance.DevSteer.GetRearSteerDirection();
                    webGUI.UpdateTime = DateTime.Now;
                    webGUI.EqpRunMode = EqpStateManager.Instance.RunMode;
                    webGUI.TransferCommand = ProcessDataHandler.Instance.CurTransferCommand;
                    webGUI.VehicleStatus = ProcessDataHandler.Instance.CurVehicleStatus;
                    webGUI.OcsState = IF.OCS.OCSCommManager.Instance.OcsStatus.ConnectError ? "Error" :
                        IF.OCS.OCSCommManager.Instance.OcsStatus.Connected ? "Ready" : "Not Ready";
                    webGUI.JcsState = IF.JCS.JCSCommManager.Instance.JcsStatus.ConnectError ? "Error" :
                        IF.JCS.JCSCommManager.Instance.JcsStatus.Connected ? "Ready" : "Not Ready";
                    webGUI.JcsPermit = VHL.Task.TaskJCS.Instance.JcsControl.ReceivedPermit;
                    webGUI.AutoDoorPermit = VHL.Task.TaskInterface.Instance.AutoDoorControl.ReceivedPermit;
                    webGUI.Pbs1 = DevicesManager.Instance.DevOBSLookDown.GetFrontDetectState() == enFrontDetectState.enDeccelation1;
                    webGUI.FrontAntiDropLock = DevicesManager.Instance.DevFrontAntiDrop.GetLock();
                    webGUI.FrontAntiDropUnLock = DevicesManager.Instance.DevFrontAntiDrop.GetUnlock();
                    webGUI.RearAntiDropLock = DevicesManager.Instance.DevRearAntiDrop.GetLock();
                    webGUI.RearAntiDropUnLock = DevicesManager.Instance.DevRearAntiDrop.GetUnlock();
                    webGUI.GripperOpen = DevicesManager.Instance.DevGripperPIO.IsGripperOpen();
                    webGUI.GripperClose = DevicesManager.Instance.DevGripperPIO.IsGripperClose();
                    webGUI.HoistHome = DevicesManager.Instance.DevGripperPIO.DiHoistHome.IsDetected;
                    webGUI.HoistUp = DevicesManager.Instance.DevGripperPIO.DiHoistUp.IsDetected;
                    webGUI.HoistLimit = DevicesManager.Instance.DevGripperPIO.DiHoistLimit.IsDetected;
                    webGUI.LeftProductExist = DevicesManager.Instance.DevGripperPIO.DiLeftProductExist.IsDetected;
                    webGUI.RightProductExist = DevicesManager.Instance.DevGripperPIO.DiRightProductExist.IsDetected;
                    webGUI.DlgEqpInitShow = m_DlgEqpInit != null ? m_DlgEqpInit.Visible : false;
                    webGUI.DlgNotifyErrorShow = m_DlgNotifyError != null ? m_DlgNotifyError.Visible : false;
                    webGUI.DlgOpCallCommandShow = m_DlgOpCallCommand != null ? m_DlgOpCallCommand.Visible : false;
                    webGUI.DlgOpCallSingleMessageShow = m_DlgOpCallSingleMessage != null ? m_DlgOpCallSingleMessage.Visible : false;
                    webGUI.OpCallMsg = m_DlgOpCallSingleMessage != null ? m_DlgOpCallSingleMessage.GetMessage() : "";
                    webGUI.FoupExist = DevicesManager.Instance.DevGripperPIO.IsProductExist();
                    RemoteManager.TouchInstance.Remoting.TouchGUI.Web = webGUI;
                    if (AlarmCurrentProvider.Instance.IsHeavyAlarm())
                    {
                        RemoteManager.TouchInstance.Remoting.TouchGUI.IsAlarm = true;
                        RemoteManager.TouchInstance.Remoting.TouchGUI.AlarmIds = string.Join(",", AlarmCurrentProvider.Instance.GetCurrentAlarmIds());
                    }
                    else
                    {
                        RemoteManager.TouchInstance.Remoting.TouchGUI.IsAlarm = false;
                        RemoteManager.TouchInstance.Remoting.TouchGUI.AlarmIds = string.Empty;
                    }
                    if (RemoteManager.TouchInstance.Remoting.TouchGUI.SetOverrideZero)
                    {
                        RemoteManager.TouchInstance.Remoting.TouchGUI.SetOverrideZero = false;
                        (DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().GetAxis() as MpAxis).SetOverrideZeroTouchGUI = true;
                    }
                    RemoteManager.TouchInstance.Remoting.TouchGUI.AddWebData();
                    Debug.WriteLine(RemoteManager.TouchInstance.Remoting.TouchGUI.WebDataCount());


                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
                if (AppConfig.Instance.Simulation.MY_DEBUG) Debug.WriteLine(string.Format("UpdateRemoteTouch Exception {0}", ex.ToString()));

            }
        }

        private void RemoteObject_ChangeMode(OperateMode mode)
        {
            
            ChangeMode?.Invoke(mode);
        }
        private void PadRemoteGUI_ChangeRunMode(EqpRunMode mode)
        {
            ChangeRunMode?.Invoke(mode);
        }
        #endregion
    }
}
