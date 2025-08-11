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

namespace Sineva.VHL.IF.OCS
{
    public partial class FrmAlarmEventSendMessage : Form
    {
        private VehicleIF_AlarmEventSend m_VehicleIF_AlarmEventSend;
        private string m_DateTime;
        public FrmAlarmEventSendMessage(VehicleIF_AlarmEventSend vehicleIF_AlarmEventSend,string dateTime)
        {
            InitializeComponent();
            m_VehicleIF_AlarmEventSend = vehicleIF_AlarmEventSend;
            m_DateTime = dateTime;
        }
        private bool Initialize()
        {
            bool rv = true;
            cbAlarmStatus.DataSource = Enum.GetNames(typeof(AlarmSetCode));
            cbAlarmType.DataSource = Enum.GetNames(typeof(AlarmType));
            return rv;
        }

        private void FrmAlarmEventSendMessage_Load(object sender, EventArgs e)
        {
            Initialize();
            if (m_VehicleIF_AlarmEventSend != null)
            {
                tbPrimaryVHL.Text = m_VehicleIF_AlarmEventSend.VehicleNumber.ToString();
                tbAlarmID.Text = m_VehicleIF_AlarmEventSend.AlarmID.ToString();
                cbAlarmStatus.Text = m_VehicleIF_AlarmEventSend.AlarmStatus.ToString();
                cbAlarmType.Text = m_VehicleIF_AlarmEventSend.AlarmType.ToString();
                tbAlarmCode.Text = m_VehicleIF_AlarmEventSend.AlarmCode.ToString();
                tbSentTime.Text = m_DateTime;
            }
        }
    }
}
