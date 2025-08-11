using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sineva.VHL.Data;
using Sineva.VHL.Device;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.IF.OCS;
using Sineva.VHL.IF.JCS;
using Sineva.VHL.Library;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.GUI
{
    public partial class JobsForm : GeneralForm, IFormUpdate
    {
        #region Fields
        private bool m_UpdateNeed = false;
        private List<IUpdateUCon> m_Ucons = new List<IUpdateUCon>();
        private System.Timers.Timer m_Timer = new System.Timers.Timer(100);
        private DevicesManager m_DevicesManager = DevicesManager.Instance;
        private bool m_Initiated = false;
        DlgPioTest m_PioTestForm = new DlgPioTest();
        private uint m_TabPageChangeToOcsTicks = 0;
        private bool m_TicksStart = false;

        private FormLifeTimeDisplay m_DlgLifeTimeDisplay = null;
        private DevTransfer m_devTransfer = null;
        #endregion

        public JobsForm()
        {
            InitializeComponent();

            ucDevPio1.Visible = DevicesManager.Instance.DevEqpPIO.IsValid;
            ucDevGripperPio1.Visible = DevicesManager.Instance.DevGripperPIO.IsValid;
            antiDropPanel.Visible = DevicesManager.Instance.DevFrontAntiDrop.IsValid && DevicesManager.Instance.DevRearAntiDrop.IsValid;
            gripperPanel.Visible = DevicesManager.Instance.DevGripperPIO.IsValid;
            rfidPanel.Visible = DevicesManager.Instance.DevRFID.IsValid;
            autoteachingPanel.Visible = DevicesManager.Instance.DevAutoTeaching.IsValid;
            piotestPanel.Visible = DevicesManager.Instance.DevEqpPIO.IsValid;
            ucDevCleaner1.Visible = AppConfig.Instance.VehicleType == VehicleType.Clean && DevicesManager.Instance.DevCleaner.IsValid;
            m_devTransfer = DevicesManager.Instance.DevTransfer;

            uamMasterVelocity.AxisTag = m_devTransfer.AxisMaster.GetDevAxis().AxisTag;
            uamMasterTorque.AxisTag = m_devTransfer.AxisMaster.GetDevAxis().AxisTag;
            ssvMasterVelocity.AxisTag = m_devTransfer.AxisMaster.GetDevAxis().AxisTag;
            ssvMasterTorque.AxisTag = m_devTransfer.AxisMaster.GetDevAxis().AxisTag;

            this.logList1.Initialize();
            this.alarmCurrentView1.InitGridView(Data.Alarm.AlarmCurrentProvider.Instance);
            EventHandlerManager.Instance.AutoTeachingVisionResult += Instance_AutoTeachingVisionResult;
            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                ucOBSStatusUp.ShowObsSimulationTestDlg += instanceShowObsSimulationTestDlg;
                ucOBSStatusLower.ShowObsSimulationTestDlg += instanceShowObsSimulationTestDlg;

                checkBox1.Visible = true;
                checkBox2.Visible = true;
                checkBox3.Visible = true;
                checkBox4.Visible = true;
                checkBox5.Visible = true;
                checkBox6.Visible = true;
            }
        }

        private void Instance_AutoTeachingVisionResult(double dx, double dy, double dt)
        {
            try
            {
                if (this.lbVisionResult.InvokeRequired)
                {
                    DelVoid_AutoTeachingVisionResult d = new DelVoid_AutoTeachingVisionResult(Instance_AutoTeachingVisionResult);
                    this.Invoke(d, dx, dy, dt);
                }
                else
                {
                    this.lbVisionResult.Text = string.Format("Result : dx={0:F1}, dy={1:F1}, dt={2:F2}", dx, dy, dt);
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        #region IFormUpdate 멤버

        public bool UpdateNeed
        {
            get
            {
                return m_UpdateNeed;
            }
            set
            {
                m_UpdateNeed = value;
            }
        }
        public void KillTimer()
        {
            this.m_Timer.Enabled = false;
        }
        #endregion

        private void JobsForm_Load(object sender, EventArgs e)
        {
            if (XFunc.IsRunTime())
            {
                int index = 0;
                IUpdateUCon con = null;

                try
                {
                    if (m_DevicesManager.DevSOSUpper.IsValid) ucOBSStatusUp.DevSoS = m_DevicesManager.DevSOSUpper;
                    else ucOBSStatusUp.Visible = false;
                    if (m_DevicesManager.DevOBSLower.IsValid) ucOBSStatusLower.DevOBS = m_DevicesManager.DevOBSLower;
                    else ucOBSStatusLower.Visible = false;
                    if (m_DevicesManager.DevOBSLookDown.IsValid) ucOBSStatusPBS.DevOBS = m_DevicesManager.DevOBSLookDown;
                    else ucOBSStatusPBS.Visible = false;

                    dsdFrontSteerStatus.FirstStateIoTag = m_DevicesManager.DevSteer.FrontSteer.DiSensorFw.Count > 0 ? m_DevicesManager.DevSteer.FrontSteer.DiSensorFw.First() : null;
                    dsdFrontSteerStatus.SecondStateIoTag = m_DevicesManager.DevSteer.FrontSteer.DiSensorBw.Count > 0 ? m_DevicesManager.DevSteer.FrontSteer.DiSensorBw.First() : null;
                    dsdRearSteerStatus.FirstStateIoTag = m_DevicesManager.DevSteer.FrontSteer.DiSensorFw.Count > 0 ? m_DevicesManager.DevSteer.RearSteer.DiSensorFw.First() : null;
                    dsdRearSteerStatus.SecondStateIoTag = m_DevicesManager.DevSteer.FrontSteer.DiSensorBw.Count > 0 ? m_DevicesManager.DevSteer.RearSteer.DiSensorBw.First() : null;

                    UpdateUConFunc.GetAllUpdateUCon(this, ref m_Ucons);
                    foreach (var ucon in m_Ucons)
                    {
                        con = ucon;
                        ucon.Initialize();
                        index++;
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    if (con != null) message += string.Format("\n{0} [{1}]", con.ToString(), index);
                    MessageBox.Show(message);
                    ExceptionLog.WriteLog(message);
                }

                try
                {
                    // Toolbar Setting
                    panelToolbar.Controls.Add(new toolbarJobView());
                    foreach (UserControl ctrl in panelToolbar.Controls)
                    {
                        ctrl.Dock = DockStyle.Fill;
                        if (tabControlJob.Tag == ctrl.Tag)
                            ctrl.Visible = true;
                        else
                            ctrl.Visible = false;
                    }

                    EqpStateManager.Instance.SetOpMode(OperateMode.Manual);
                    EqpStateManager.Instance.SetRunMode(EqpRunMode.Stop);

                    m_Timer.Enabled = true;
                    m_Timer.Elapsed += m_Timer_Elapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    ExceptionLog.WriteLog(ex.Message);
                }
            }

            m_Initiated = true;
        }

        private void m_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (!update_start) BeginInvoke(new DelVoid_Void(UpdateState));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.Message);
            }
        }

        static bool update_start = false;
        static bool update_doing = true;
        private void UpdateState()
        {
            update_start = true;
            int index = 0;
            IUpdateUCon con = null;
            try
            {
                if (m_UpdateNeed == false || !update_doing)
                {
                    update_start = false;
                    return;
                }
                //if (AppConfig.Instance.Simulation.MY_DEBUG)
                //    System.Diagnostics.Debug.WriteLine($"=> {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}");

                foreach (IUpdateUCon ucon in m_Ucons)
                {
                    try
                    {
                        con = ucon;
                        ucon.UpdateState();
                    }
                    catch
                    {
                    }
                    index++;
                }
                lbMxpOverride.Text = string.Format("{0:F2}", ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.MxpOverrideRatio);
                UpdateAntiDropState();
                UpdateFoupExist();
                UpdateInformation();
                ChangedOCSTabPage();
                update_doing = true;

                //if (AppConfig.Instance.Simulation.MY_DEBUG)
                //    System.Diagnostics.Debug.WriteLine($"<= {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}\r\n");
            }
            catch (Exception ex)
            {
                update_doing = false;
                string message = ex.Message;
                if (con != null) message += string.Format("\n{0} [{1}]", con.ToString(), index);
                MessageBox.Show(message);
                ExceptionLog.WriteLog(message);
            }
            update_start = false;
        }
        public void SetUserControlAuthority(AuthorizationLevel level)
        {
        }
        private void pLifeTimeDisplay_Click(object sender, EventArgs e)
        {
            if (m_DlgLifeTimeDisplay == null || m_DlgLifeTimeDisplay.IsDisposed)
                m_DlgLifeTimeDisplay = new FormLifeTimeDisplay();
            m_DlgLifeTimeDisplay.Show();
        }
        private void pAlarmReset_Click(object sender, EventArgs e)
        {
            IsPushedSwitch.m_AlarmRstPushed = true;
            foreach (ServoUnit servo in ServoManager.Instance.ServoUnits)
            {
                servo.AlarmClear();
            }
        }
        private void pBuzzerOff_Click(object sender, EventArgs e)
        {
            IsPushedSwitch.m_BuzzerOffPushed = true;
        }
        #region Jobs-Manual
        

        private void tabControlJob_VisibleChanged(object sender, EventArgs e)
        {
            if (tabControlJob.Visible)
            {
                if (XFunc.IsRunTime())
                {
                    //this.manualTeachingView1.Initialize();
                }
            }
        }

        private void ChangedOCSTabPage()
        {
            if (EqpStateManager.Instance.OpMode != OperateMode.Auto) return;
            // MainForm을 띄우고 있으면 CPU 부하율이 높다.
            if (tabControlJob.SelectedTab.Tag.ToString() != "OCS" && m_TicksStart == false)
            {
                m_TabPageChangeToOcsTicks = XFunc.GetTickCount();
                m_TicksStart = true;
            }

            if (m_TicksStart && XFunc.GetTickCount() - m_TabPageChangeToOcsTicks > 30 * 1000)
            {
                m_TicksStart = false;
                for (int i = 0; i < tabControlJob.TabPages.Count; i++)
                {
                    if (tabControlJob.TabPages[i].Tag.ToString() == "OCS")
                        tabControlJob.SelectedTab = tabControlJob.TabPages[i];
                }
            }
        }
        #endregion
        #region Jobs-Main
        private void UpdateAntiDropState()
        {
            try
            {
                if (m_DevicesManager.DevFrontAntiDrop.IsValid)
                {
                    bool front_lock = m_DevicesManager.DevFrontAntiDrop.GetLock();
                    pbFrontAntiDrop.Visible = front_lock;
                }
                else pbFrontAntiDrop.Visible = false;
                if (m_DevicesManager.DevRearAntiDrop.IsValid)
                {
                    bool rear_lock = m_DevicesManager.DevRearAntiDrop.GetLock();
                    pbRearAntiDrop.Visible = rear_lock;
                }
                else pbRearAntiDrop.Visible = false;

                if (m_DevicesManager.DevCleaner.IsValid)
                {
                    lbCleaner.Visible = true;
                    if (m_DevicesManager.DevCleaner.IsRun()) lbCleaner.BackColor = Color.LightGreen;
                    else if (m_DevicesManager.DevCleaner.IsAlarm()) lbCleaner.BackColor = Color.Red;
                    else lbCleaner.BackColor = Color.LightGray;

                    if (m_DevicesManager.DevCleaner.IsAlarm()) lbCleaner.Text = "High Temperature";
                    else if (m_DevicesManager.DevCleaner.IsDoorClose()) lbCleaner.Text = "Cleaning";
                    else lbCleaner.Text = "Door Open";
                }
                else lbCleaner.Visible = false;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void UpdateFoupExist()
        {
            try
            {
                if (m_DevicesManager.DevGripperPIO.IsValid)
                {
                    bool exist = m_DevicesManager.DevGripperPIO.IsProductExist();
                    pbFOUP.Visible = exist;
                }
                else pbFOUP.Visible = false;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void UpdateInformation()
        {
            try
            {
                lbOCSSystemByte.Text = string.Format("OCS : {0:X}", OCSCommManager.Instance.OcsComm.SystemByte);
                lbJCSSystemByte.Text = string.Format("JCS : {0:X}", JCSCommManager.Instance.JcsComm.SystemByte);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion

        private void btnRFIDRead_Click(object sender, EventArgs e)
        {
            try
            {
                DevicesManager.Instance.DevRFID.ReadStart();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnAutoTeachingMonitor_Click(object sender, EventArgs e)
        {
            EventHandlerManager.Instance.InvokeAutoTeachingMonitorShow(true);
        }

        private void btnPioTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_PioTestForm == null)
                {
                    m_PioTestForm = new DlgPioTest();
                }
                m_PioTestForm.TopMost = true;
                m_PioTestForm.Show();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void instanceShowObsSimulationTestDlg(int type)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    DelVoid_Int d = new DelVoid_Int(instanceShowObsSimulationTestDlg);
                    this.Invoke(d, type);
                }
                else
                {
                    DlgFrontDetectTest dlg = new DlgFrontDetectTest();
                    dlg.TopMost = true;
                    dlg.Show();
                }   
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                CheckBox box = sender as CheckBox;
                int index = -1;
                if (box == checkBox1) index = 0;
                else if (box == checkBox2) index = 1;
                else if (box == checkBox3) index = 2;
                else if (box == checkBox4) index = 3;
                else if (box == checkBox5) index = 4;
                else if (box == checkBox6) index = 5;
                if (index >= 0) GV.TEST_INTERLOCK[index] = box.Checked;
            }
        }
    }
}
