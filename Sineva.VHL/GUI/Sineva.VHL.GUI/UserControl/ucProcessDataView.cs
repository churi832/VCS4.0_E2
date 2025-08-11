using Sineva.VHL.Data.Process;
using Sineva.VHL.Device;
using Sineva.VHL.IF.JCS;
using Sineva.VHL.IF.OCS;
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
    public partial class ucProcessDataView : UserControl, IUpdateUCon
    {
        #region Fields
        private string m_ExceptionMessage = string.Empty;
        #endregion

        #region Constructor
        public ucProcessDataView()
        {
            InitializeComponent();
        }
        #endregion

        #region Method
        public bool Initialize()
        {
            bool rv = true;
            return rv;
        }

        public void UpdateState()
        {
            if (!this.Visible) return;

            try
            {
                string temp = ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState.ToString();
                if (temp != lbVehicleState.Text) lbVehicleState.Text = temp;
                temp = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus.ToString();
                if (temp != lbCarrierInstall.Text) lbCarrierInstall.Text = temp;
                temp = ProcessDataHandler.Instance.CurTransferCommand.RemainBcrDistance.ToString("F1");
                if (temp != lbRemainBcr.Text) lbRemainBcr.Text = temp;
                temp = ProcessDataHandler.Instance.CurTransferCommand.RemainMotorDistance.ToString("F1");
                if (temp != lbRemainMotor.Text) lbRemainMotor.Text = temp;
                temp = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.VirtualBcr.ToString("F1");
                if (temp != lbVirtualBcr.Text) lbVirtualBcr.Text = temp;
                temp = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.VirtualRunBcr.ToString("F1");
                if (temp != lbVirtualRunBcr.Text) lbVirtualRunBcr.Text = temp;
                temp = ProcessDataHandler.Instance.CurVehicleStatus.IsInPosition ? "OK" : "NG";
                if (temp != lbInPosition.Text) lbInPosition.Text = temp;
                temp = ProcessDataHandler.Instance.CurVehicleStatus.IsNearPosition ? "OK" : "NG";
                if (temp != lbNearPosition.Text) lbNearPosition.Text = temp;
                temp = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.OverrideRatio.ToString("F1");
                if (temp != lbOverrideRatio.Text) lbOverrideRatio.Text = temp;
                temp = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.CollisionDistance.ToString("F1");
                if (temp != lbCollisitionDistance.Text) lbCollisitionDistance.Text = temp;
                bool jcs_permit = TaskJCS.Instance.JcsControl.ReceivedPermit;
                lbJCSPermit.Text = jcs_permit ?  "Permit" : "Wait";
                if (jcs_permit && lbJCSPermit.ForeColor != Color.Green) lbJCSPermit.ForeColor = Color.Green;
                else if (!jcs_permit && lbJCSPermit.ForeColor != Color.White) lbJCSPermit.ForeColor = Color.White;
                bool autodoor_permit = TaskInterface.Instance.AutoDoorControl.ReceivedPermit;
                lbAutoDoorPermit.Text = autodoor_permit ? "Permit" : "Wait";
                if (autodoor_permit && lbAutoDoorPermit.ForeColor != Color.Green) lbAutoDoorPermit.ForeColor = Color.Green;
                else if (!autodoor_permit && lbAutoDoorPermit.ForeColor != Color.White) lbAutoDoorPermit.ForeColor = Color.White;
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

        private void ucProcessData_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (sender == lbCarrierInstall)
                {
                    bool foup_exist = DevicesManager.Instance.DevGripperPIO.IsProductExist();
                    bool carrier_install = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == CarrierState.Installed;
                    bool ok = (foup_exist && carrier_install) || (!foup_exist && !carrier_install);
                    if (!ok)
                    {
                        if (MessageBox.Show("do you want to change carrier state !", "SAVE", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            if (foup_exist)
                            {
                                ProcessDataHandler.Instance.CurVehicleStatus.SetCarrierStatus(CarrierState.Installed);
                                //OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.CarrierInstalled);
                            }
                            else
                            {
                                ProcessDataHandler.Instance.CurVehicleStatus.SetCarrierStatus(CarrierState.None);
                                //OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.CarrierRemoved);
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
    }
}
