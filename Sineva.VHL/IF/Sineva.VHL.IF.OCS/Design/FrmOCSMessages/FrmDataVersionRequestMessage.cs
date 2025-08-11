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
    public partial class FrmDataVersionRequestMessage : Form
    {
        private VehicleIF_DataVersionRequest m_VehicleIF_DataVersionRequest;
        private DateTime m_DateTime;
        public FrmDataVersionRequestMessage(VehicleIF_DataVersionRequest vehicleIF_DataVersionRequest, DateTime dateTime)
        {
            InitializeComponent();
            m_VehicleIF_DataVersionRequest = vehicleIF_DataVersionRequest;
            m_DateTime = dateTime;
        }

        private void FrmDataVersionRequestMessage_Load(object sender, EventArgs e)
        {
            if (m_VehicleIF_DataVersionRequest != null)
            {
                tbPrimaryVHL.Text = m_VehicleIF_DataVersionRequest.VehicleNumber.ToString();
                tbSecondaryVHL.Text = m_VehicleIF_DataVersionRequest.VehicleNumber.ToString();
            }
        }
    }
}
