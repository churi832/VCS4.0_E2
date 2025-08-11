using Sineva.VHL.Data.Process;
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

namespace Sineva.VHL.GUI
{
    public partial class ucCommandDataView : UCon, IUpdateUCon
    {
        #region Fields
        private string m_ExceptionMessage = string.Empty;
        #endregion

        #region Constructor
        public ucCommandDataView() : base(OperateMode.Manual)
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
            try
            {
                bool isValid = ProcessDataHandler.Instance.CurTransferCommand.IsValid;
                bool certain = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Certain;

                lbCurrentNode.Text = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.FromNodeID.ToString() + "," + ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortID.ToString();
                lbTargetNode.Text = ProcessDataHandler.Instance.CurTransferCommand.TargetNode.ToString();
                lbCommandID.Text = ProcessDataHandler.Instance.CurTransferCommand.CommandID.ToString();
                lbCarrierID.Text = ProcessDataHandler.Instance.CurTransferCommand.CassetteID.ToString();
                lbTargetLeftBCR.Text = ProcessDataHandler.Instance.CurTransferCommand.TargetLeftBcrPosition.ToString("0.00");
                lbTargetRightBCR.Text = ProcessDataHandler.Instance.CurTransferCommand.TargetRightBcrPosition.ToString("0.00");

                if (certain && lbCurrentNode.BackColor != Color.LightGreen) lbCurrentNode.BackColor = Color.LightGreen;
                else if (!certain && lbCurrentNode.BackColor != Color.White) lbCurrentNode.BackColor = Color.White;

                if (isValid && lbTargetNode.BackColor != Color.LightGreen)
                {
                    lbTargetNode.BackColor = Color.LightGreen;
                    lbCommandID.BackColor = Color.LightGreen;
                    lbCarrierID.BackColor = Color.LightGreen;
                    lbTargetLeftBCR.BackColor = Color.LightGreen;
                    lbTargetRightBCR.BackColor = Color.LightGreen;
                }
                else if (!isValid && lbTargetNode.BackColor != Color.White)
                {
                    lbTargetNode.BackColor = Color.White;
                    lbCommandID.BackColor = Color.White;
                    lbCarrierID.BackColor = Color.White;
                    lbTargetLeftBCR.BackColor = Color.White;
                    lbTargetRightBCR.BackColor = Color.White;
                }
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
