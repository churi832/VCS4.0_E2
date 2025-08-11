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
    public partial class FrmEventSendMessage : Form
    {
        private VehicleIF_EventSend m_VehicleIF_EventSend;
        private DateTime m_DateTime;
        public FrmEventSendMessage(VehicleIF_EventSend vehicleIF_EventSend, DateTime dateTime)
        {
            InitializeComponent();
            m_VehicleIF_EventSend = vehicleIF_EventSend;
            m_DateTime = dateTime;
        }
        private bool Initialize()
        {
            bool rv = true;
            cbPrimaryVehicleEvent.DataSource = Enum.GetNames(typeof(VehicleEvent));
            return rv;
        }

        private void FrmEventSendMessage_Load(object sender, EventArgs e)
        {
            Initialize();
            if (m_VehicleIF_EventSend != null)
            {
                tbPrimaryVHL.Text = m_VehicleIF_EventSend.VehicleNumber.ToString();
                tbSecondaryVHL.Text = m_VehicleIF_EventSend.VehicleNumber.ToString();
                cbPrimaryVehicleEvent.Text = m_VehicleIF_EventSend.Event.ToString();
                tbSecondaryVehicleEvent.Text = m_VehicleIF_EventSend.Event.ToString();
            }
        }
    }
}
