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
    public partial class FrmCommandSendMessage : Form
    {
        private VehicleIF_CommandSend m_VehicleIF_CommandSend;
        private DateTime m_DateTime;
        public FrmCommandSendMessage(VehicleIF_CommandSend _CommandSend,DateTime dateTime)
        {
            InitializeComponent();
            m_VehicleIF_CommandSend = _CommandSend;
            m_DateTime = dateTime;
        }

        private void FrmCommandSendMessage_Load(object sender, EventArgs e)
        {
            if (m_VehicleIF_CommandSend != null)
            {
                tbCommandID.Text = m_VehicleIF_CommandSend.TransferCommandID;
                tbCarrierID.Text = m_VehicleIF_CommandSend.CassetteID;
                tbPrimaryVHL.Text = m_VehicleIF_CommandSend.VehicleNumber.ToString();
                tbSecondaryVHL.Text = m_VehicleIF_CommandSend.VehicleNumber.ToString();
                tbNodes.Text = string.Join(",", m_VehicleIF_CommandSend.PathNodes);

            }
        }
    }
}
