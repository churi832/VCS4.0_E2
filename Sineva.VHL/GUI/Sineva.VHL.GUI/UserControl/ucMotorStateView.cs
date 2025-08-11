using Sineva.VHL.Data.Process;
using Sineva.VHL.Device;
using Sineva.VHL.IF.JCS;
using Sineva.VHL.Library;
using Sineva.VHL.Task;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.GUI
{
    public partial class ucMotorStateView : UCon, IUpdateUCon
    {
        #region Fields
        private string m_ExceptionMessage = string.Empty;
        #endregion

        #region Constructor
        public ucMotorStateView() : base(OperateMode.Manual)
        {
            InitializeComponent();
        }
        #endregion

        #region Method
        public bool Initialize()
        {
            bool rv = true;
            ssvSlavePosition.AxisTag = DevicesManager.Instance.DevTransfer.AxisSlave.IsValid ? DevicesManager.Instance.DevTransfer.AxisSlave.GetDevAxis().AxisTag : null;
            ssdSlaveStatus.AxisTag = DevicesManager.Instance.DevTransfer.AxisSlave.IsValid ? DevicesManager.Instance.DevTransfer.AxisSlave.GetDevAxis().AxisTag : null;
            ssdSlaveHomeDone.AxisTag = DevicesManager.Instance.DevTransfer.AxisSlave.IsValid ? DevicesManager.Instance.DevTransfer.AxisSlave.GetDevAxis().AxisTag : null;
            ssvMasterPosition.AxisTag = DevicesManager.Instance.DevTransfer.AxisMaster.IsValid ? DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().AxisTag : null;
            ssdMasterStatus.AxisTag = DevicesManager.Instance.DevTransfer.AxisMaster.IsValid ? DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().AxisTag : null;
            ssdMasterHomeDone.AxisTag = DevicesManager.Instance.DevTransfer.AxisMaster.IsValid ? DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().AxisTag : null;

            ssvHoistPosition.AxisTag = DevicesManager.Instance.DevFoupGripper.AxisHoist.IsValid ? DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis().AxisTag : null;
            ssdHoistStatus.AxisTag = DevicesManager.Instance.DevFoupGripper.AxisHoist.IsValid ? DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis().AxisTag : null;
            ssdHoistHomeDone.AxisTag = DevicesManager.Instance.DevFoupGripper.AxisHoist.IsValid ? DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis().AxisTag : null;
            ssvSlidePosition.AxisTag = DevicesManager.Instance.DevFoupGripper.AxisSlide.IsValid ? DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis().AxisTag : null;
            ssdSlideStatus.AxisTag = DevicesManager.Instance.DevFoupGripper.AxisSlide.IsValid ? DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis().AxisTag : null;
            ssdSlideHomeDone.AxisTag = DevicesManager.Instance.DevFoupGripper.AxisSlide.IsValid ? DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis().AxisTag : null;
            ssvRotatePosition.AxisTag = DevicesManager.Instance.DevFoupGripper.AxisTurn.IsValid ? DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis().AxisTag : null;
            ssdRotateStatus.AxisTag = DevicesManager.Instance.DevFoupGripper.AxisTurn.IsValid ? DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis().AxisTag : null;
            ssdRotateHomeDone.AxisTag = DevicesManager.Instance.DevFoupGripper.AxisTurn.IsValid ? DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis().AxisTag : null;
            ssvFrontAntiDropPosition.AxisTag = DevicesManager.Instance.DevFrontAntiDrop.Axis.IsValid ? DevicesManager.Instance.DevFrontAntiDrop.Axis.GetDevAxis().AxisTag : null;
            ssdFrontAntiDropStatus.AxisTag = DevicesManager.Instance.DevFrontAntiDrop.Axis.IsValid ? DevicesManager.Instance.DevFrontAntiDrop.Axis.GetDevAxis().AxisTag : null;
            ssdFrontAntiDropHomeDone.AxisTag = DevicesManager.Instance.DevFrontAntiDrop.Axis.IsValid ? DevicesManager.Instance.DevFrontAntiDrop.Axis.GetDevAxis().AxisTag : null;
            ssvRearAntiDropPosition.AxisTag = DevicesManager.Instance.DevRearAntiDrop.Axis.IsValid ? DevicesManager.Instance.DevRearAntiDrop.Axis.GetDevAxis().AxisTag : null;
            ssdRearAntiDropStatus.AxisTag = DevicesManager.Instance.DevRearAntiDrop.Axis.IsValid ? DevicesManager.Instance.DevRearAntiDrop.Axis.GetDevAxis().AxisTag : null;
            ssdRearAntiDropHomeDone.AxisTag = DevicesManager.Instance.DevRearAntiDrop.Axis.IsValid ? DevicesManager.Instance.DevRearAntiDrop.Axis.GetDevAxis().AxisTag : null;

            bool visible = ssvHoistPosition.AxisTag != null;
            lbHoist.Visible = ssvHoistPosition.Visible = ssdHoistStatus.Visible = ssdHoistHomeDone.Visible = visible;
            visible = ssvSlidePosition.AxisTag != null;
            lbSlide.Visible = ssvSlidePosition.Visible = ssdSlideStatus.Visible = ssdSlideHomeDone.Visible = visible;
            visible = ssvRotatePosition.AxisTag != null;
            lbRotate.Visible = ssvRotatePosition.Visible = ssdRotateStatus.Visible = ssdRotateHomeDone.Visible = visible;
            visible = ssvFrontAntiDropPosition.AxisTag != null;
            lbFrontAntiDrop.Visible = ssvFrontAntiDropPosition.Visible = ssdFrontAntiDropStatus.Visible = ssdFrontAntiDropHomeDone.Visible = visible;
            visible = ssvRearAntiDropPosition.AxisTag != null;
            lbRearAntiDrop.Visible = ssvRearAntiDropPosition.Visible = ssdRearAntiDropStatus.Visible = ssdRearAntiDropHomeDone.Visible = visible;

            return rv;
        }

        public void UpdateState()
        {
            try
            {
            }
            catch (Exception ex)
            {
                if (m_ExceptionMessage != ex.Message)
                {
                    m_ExceptionMessage = ex.Message;
                    string log = string.Format("UserControl : [{0}]-[{1}]\n{2}", this.GetType(), this.Name, ex.Message);
                    ExceptionLog.WriteLog(log);
                }

            }
        }
        #endregion
    }
}
