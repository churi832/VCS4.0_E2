using Sineva.VHL.Data;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.IF.Vision;
using Sineva.VHL.Library;
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
    public partial class ucDevAutoTeaching : UCon, IUpdateUCon
    {
        #region Fields
        private DevAutoTeaching m_devAutoTeaching = null;
        private DevFoupGripper m_devFoupGripper = null;
        private bool m_VisionFind = false;
        private enVisionDevice m_VisionFindDevice = enVisionDevice.EQPCamera;

        private double m_VisionResultX = 0.0f;
        private double m_VisionResultY = 0.0f;
        private double m_VisionResultT = 0.0f;
        #endregion

        #region Constructor
        public ucDevAutoTeaching() : base(OperateMode.Manual)
        {
            InitializeComponent();
        }
        #endregion

        #region Methods - Interface
        public bool Initialize()
        {
            bool rv = true;
            m_devAutoTeaching = DevicesManager.Instance.DevAutoTeaching;
            m_devFoupGripper = DevicesManager.Instance.DevFoupGripper;
            this.ucServoStatusValueMaster.AxisTag = DevicesManager.Instance.DevTransfer.IsValid && DevicesManager.Instance.DevTransfer.AxisMaster.IsValid ? DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().AxisTag : null;
            this.ucServoStatusValueHoist.AxisTag = DevicesManager.Instance.DevFoupGripper.IsValid && DevicesManager.Instance.DevFoupGripper.AxisHoist.IsValid ? DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis().AxisTag : null;
            this.ucServoStatusValueSlide.AxisTag = DevicesManager.Instance.DevFoupGripper.IsValid && DevicesManager.Instance.DevFoupGripper.AxisSlide .IsValid? DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis().AxisTag : null;
            this.ucServoStatusValueRotate.AxisTag = DevicesManager.Instance.DevFoupGripper.IsValid && DevicesManager.Instance.DevFoupGripper.AxisTurn.IsValid ? DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis().AxisTag : null;

            this.lbIpAddress.Text = string.Format("{0}//{1}", m_devAutoTeaching.VisionIpAddress, m_devAutoTeaching.VisionPortNumber);
            this.cbOnlySensorOffsetFind.Checked = m_devAutoTeaching.OnlySensorOffsetFind;

            EventHandlerManager.Instance.AutoTeachingVisionResult += Instance_AutoTeachingVisionResult;
            return rv;
        }

        private void Instance_AutoTeachingVisionResult(double dx, double dy, double dt)
        {
            if (this.lbResultX.InvokeRequired || this.lbResultY.InvokeRequired || this.lbResultT.InvokeRequired ||
                this.lbOffsetMaster.InvokeRequired || this.lbOffsetHoist.InvokeRequired || this.lbOffsetSlide.InvokeRequired || this.lbOffsetRotate.InvokeRequired)
            {
                DelVoid_AutoTeachingVisionResult d = new DelVoid_AutoTeachingVisionResult(Instance_AutoTeachingVisionResult);
                this.Invoke(d, dx, dy, dt);
            }
            else
            {
                try
                {
                    m_VisionResultX = dx;
                    m_VisionResultY = dy;
                    m_VisionResultT = dt;
                    this.lbResultX.Text = string.Format("{0:F1}", dx);
                    this.lbResultY.Text = string.Format("{0:F1}", dy);
                    this.lbResultT.Text = string.Format("{0:F1}", dt);
                }
                catch (Exception ex)
                {
                    string log = string.Format("UserControl : [{0}]-[{1}]\n{2}", this.GetType(), this.Name, ex.Message);
                    ExceptionLog.WriteLog(log);
                }
            }
        }

        public void UpdateState()
        {
            try
            {
                if (!this.Visible) return;
                if (m_devAutoTeaching.IsValid)
                {
                    bool connected = m_devAutoTeaching.IsSocketConnected();
                    if (connected && lbEthernetConnection.Text != "Connected")
                    {
                        lbEthernetConnection.ForeColor = Color.Green;
                        lbEthernetConnection.Text = "Connected";
                    }
                    else if (!connected && lbEthernetConnection.Text != "Disconnected")
                    {
                        lbEthernetConnection.ForeColor = Color.Red;
                        lbEthernetConnection.Text = "Disconnected";
                    }
                    connected = m_devAutoTeaching.IsPioConnected();
                    if (connected && lbPIOConnection.Text != "Connected")
                    {
                        lbPIOConnection.ForeColor = Color.Green;
                        lbPIOConnection.Text = "Connected";
                    }
                    else if (!connected && lbPIOConnection.Text != "Disconnected")
                    {
                        lbPIOConnection.ForeColor = Color.Red;
                        lbPIOConnection.Text = "Disconnected";
                    }

                    connected = m_devAutoTeaching.IsEEIPSocketConnected();
                    if (connected && lbEEIPConnection.Text != "Connected")
                    {
                        lbEEIPConnection.ForeColor = Color.Green;
                        lbEEIPConnection.Text = "Connected";
                    }
                    else if (!connected && lbEEIPConnection.Text != "Disconnected")
                    {
                        lbEEIPConnection.ForeColor = Color.Red;
                        lbEEIPConnection.Text = "Disconnected";
                    }

                    bool sensor_on = m_devAutoTeaching.IsPioGo();
                    if (sensor_on && lbPIOGo.Text != "ON")
                    {
                        lbPIOGo.ForeColor = Color.Green;
                        lbPIOGo.Text = "ON";
                    }
                    else if (!sensor_on && lbPIOGo.Text != "OFF")
                    {
                        lbPIOGo.ForeColor = Color.Red;
                        lbPIOGo.Text = "OFF";
                    }
                    sensor_on = m_devAutoTeaching.IsReflectiveSensorOn();
                    if (sensor_on && lbReflectiveSensor.Text != "ON")
                    {
                        lbReflectiveSensor.ForeColor = Color.Green;
                        lbReflectiveSensor.Text = "ON";
                    }
                    else if (!sensor_on && lbReflectiveSensor.Text != "OFF")
                    {
                        lbReflectiveSensor.ForeColor = Color.Red;
                        lbReflectiveSensor.Text = "OFF";
                    }
                    sensor_on = m_devFoupGripper.IsLeftDoubleStorage();
                    if (sensor_on && lbLeftDoubleStorage.Text != "ON")
                    {
                        lbLeftDoubleStorage.ForeColor = Color.Green;
                        lbLeftDoubleStorage.Text = "ON";
                    }
                    else if (!sensor_on && lbLeftDoubleStorage.Text != "OFF")
                    {
                        lbLeftDoubleStorage.ForeColor = Color.Red;
                        lbLeftDoubleStorage.Text = "OFF";
                    }
                    sensor_on = m_devFoupGripper.IsRightDoubleStorage();
                    if (sensor_on && lbRightDoubleStorage.Text != "ON")
                    {
                        lbRightDoubleStorage.ForeColor = Color.Green;
                        lbRightDoubleStorage.Text = "ON";
                    }
                    else if (!sensor_on && lbRightDoubleStorage.Text != "OFF")
                    {
                        lbRightDoubleStorage.ForeColor = Color.Red;
                        lbRightDoubleStorage.Text = "OFF";
                    }

                    string height = m_devAutoTeaching.GetHeight().ToString();
                    if (lbHeightSensor.Text != height)
                    {
                        lbHeightSensor.ForeColor = Color.Green;
                        lbHeightSensor.Text = height;
                    }

                    string offset_value = string.Format("{0:F1}", SetupManager.Instance.SetupVision.SensorOffsetDistancePORT);
                    if (lbPortOffsetDistance.Text != offset_value) lbPortOffsetDistance.Text = offset_value;
                    offset_value = string.Format("{0:F1}", SetupManager.Instance.SetupVision.SensorOffsetDistanceLOHB);
                    if (lbLOhbOffsetDistance.Text != offset_value) lbLOhbOffsetDistance.Text = offset_value;
                    offset_value = string.Format("{0:F1}", SetupManager.Instance.SetupVision.SensorOffsetDistanceROHB);
                    if (lbROhbOffsetDistance.Text != offset_value) lbROhbOffsetDistance.Text = offset_value;

                    ucServoStatusValueMaster.UpdateState();
                    ucServoStatusValueHoist.UpdateState();
                    ucServoStatusValueSlide.UpdateState();
                    ucServoStatusValueRotate.UpdateState();

                    if (m_VisionFind)
                    {
                        int rv = m_devAutoTeaching.VisionFind(m_VisionFindDevice);
                        if (rv >= 0)
                        {
                            m_VisionFind = false;
                            if (rv > 0)
                            {
                                m_devAutoTeaching.SeqAbort();
                            }
                        }
                    }

                    string resultX = string.Format("{0:F1}", m_VisionResultX);
                    string resultY = string.Format("{0:F1}", m_VisionResultY);
                    string resultT = string.Format("{0:F1}", m_VisionResultT);

                    if (resultX != this.lbResultX.Text ||
                        resultY != this.lbResultY.Text ||
                        resultT != this.lbResultT.Text)
                    {
                        // Error Calculate
                        double wheel_error = m_VisionResultY;
                        double slide_error = m_VisionResultX * SetupManager.Instance.SetupVision.AutoTeachingSlideMoveRatio;
                        double rotate_error = m_VisionResultT * SetupManager.Instance.SetupVision.AutoTeachingRotateMoveRatio;

                        double hoist_error = 0;
                        PortType curPortType = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType;
                        if (curPortType == PortType.LeftEQPort || curPortType == PortType.RightEQPort || curPortType == PortType.TeachingStation)
                            hoist_error = m_devAutoTeaching.GetHeight() + SetupManager.Instance.SetupVision.AutoTeachingHoistMoveOffsetPORT;
                        else if (curPortType == PortType.LeftBuffer || curPortType == PortType.LeftTeachingStation)
                            hoist_error = m_devAutoTeaching.GetHeight() + SetupManager.Instance.SetupVision.AutoTeachingHoistMoveOffsetLOHB;
                        else hoist_error = m_devAutoTeaching.GetHeight() + SetupManager.Instance.SetupVision.AutoTeachingHoistMoveOffsetROHB;

                        this.lbOffsetMaster.Text = string.Format("{0:F1}", wheel_error);
                        this.lbOffsetHoist.Text = string.Format("{0:F1}", hoist_error);
                        this.lbOffsetSlide.Text = string.Format("{0:F1}", slide_error);
                        this.lbOffsetRotate.Text = string.Format("{0:F1}", rotate_error);
                    }
                }
            }
            catch (Exception ex)
            {
                string log = string.Format("UserControl : [{0}]-[{1}]\n{2}", this.GetType(), this.Name, ex.Message);
                ExceptionLog.WriteLog(log);
            }
        }
        #endregion

        #region Methods
        private void btnVisionFind_Click(object sender, EventArgs e)
        {
            if (EqpStateManager.Instance.OpMode == OperateMode.Auto) return;
            if (m_devAutoTeaching.IsSocketConnected() == false)
            {
                MessageBox.Show("Vision Unit Not Connected");
                return;
            }
            m_VisionFind = true;
            ButtonLog.WriteLog(this.Text.ToString(), string.Format("btnVisionFind_Click"));
        }
        private void cbVisionDevice_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).CheckState == CheckState.Unchecked) return;

            if (sender == this.cbVisionDeviceEqp)
            {
                m_VisionFindDevice = enVisionDevice.EQPCamera;
                this.cbVisionDeviceLeftOHB.Checked = false;
                this.cbVisionDeviceRightOHB.Checked = false;
            }
            else if (sender == this.cbVisionDeviceLeftOHB)
            {
                m_VisionFindDevice = enVisionDevice.OHBCamera_Left;
                this.cbVisionDeviceEqp.Checked = false;
                this.cbVisionDeviceRightOHB.Checked = false;
            }
            else if (sender == this.cbVisionDeviceRightOHB)
            {
                m_VisionFindDevice = enVisionDevice.OHBCamera_Right;
                this.cbVisionDeviceEqp.Checked = false;
                this.cbVisionDeviceLeftOHB.Checked = false;
            }
        }
        private void btnVisionStart_Click(object sender, EventArgs e)
        {
            GV.AutoTeachingModeOn = true;
            m_devAutoTeaching.AutoTeachingStart();
            ButtonLog.WriteLog(this.Text.ToString(), string.Format("btnVisionStart_Click"));
        }

        private void btnVisionStop_Click(object sender, EventArgs e)
        {
            GV.AutoTeachingModeOn = false;
            m_devAutoTeaching.AutoTeachingFinished();
            ButtonLog.WriteLog(this.Text.ToString(), string.Format("btnVisionStop_Click"));
        }
        private void cbOnlySensorOffsetFind_CheckedChanged(object sender, EventArgs e)
        {
            m_devAutoTeaching.OnlySensorOffsetFind = cbOnlySensorOffsetFind.Checked;
            ButtonLog.WriteLog(this.Text.ToString(), string.Format("cbOnlySensorOffsetFind_CheckedChanged : OnlySensorOffsetFind={0}", m_devAutoTeaching.OnlySensorOffsetFind));
        }
        #endregion
    }
}
