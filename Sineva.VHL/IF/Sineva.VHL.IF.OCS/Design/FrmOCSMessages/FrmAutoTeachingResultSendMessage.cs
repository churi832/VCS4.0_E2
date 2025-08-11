using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.IF.OCS
{
    public partial class FrmAutoTeachingResultSendMessage : Form
    {
        private VehicleIF_AutoTeachingResultSend m_VehicleIF_AutoTeachingResultSend;
        private DateTime m_DateTime;
        public FrmAutoTeachingResultSendMessage(VehicleIF_AutoTeachingResultSend vehicleIF_AutoTeachingResultSend, DateTime dateTime)
        {
            InitializeComponent();
            m_VehicleIF_AutoTeachingResultSend = vehicleIF_AutoTeachingResultSend;
            m_DateTime = dateTime;
        }
        private bool Initialize()
        {
            bool rv = true;
            cbPrimaryResult.DataSource = Enum.GetNames(typeof(VehicleOperationResult));
            cbSecondaryResult.DataSource = Enum.GetNames(typeof(VehicleOperationResult));
            return rv;
        }
        private void FrmAutoTeachingResultSendMessage_Load(object sender, EventArgs e)
        {
            Initialize();
            if (m_VehicleIF_AutoTeachingResultSend != null)
            {
                tbPrimaryVHL.Text = m_VehicleIF_AutoTeachingResultSend.VehicleNumber.ToString();
                tbSecondaryVHL.Text = m_VehicleIF_AutoTeachingResultSend.VehicleNumber.ToString();
                tbPrimaryPortID.Text = m_VehicleIF_AutoTeachingResultSend.PortNumber.ToString();
                cbPrimaryResult.Text = m_VehicleIF_AutoTeachingResultSend.Result.ToString();
                tbSentTime.Text = m_DateTime.ToString();
            }
        }
    }
}
