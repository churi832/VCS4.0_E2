using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Sineva.VHL.Device.ucDevOBSStatus;
using Sineva.VHL.Device;
using Sineva.VHL.Data.Process;

namespace Sineva.VHL.GUI
{
    public partial class DlgFrontDetectTest : Form
    {
        FrontSensorType m_SensorType = FrontSensorType.SOS;
        public DlgFrontDetectTest()
        {
            InitializeComponent();
        }
        public DlgFrontDetectTest(FrontSensorType sensor)
        {
            InitializeComponent();
            m_SensorType = sensor;
            if (m_SensorType == FrontSensorType.OBS)
            {
                button2.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button8.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_SensorType == FrontSensorType.SOS)
            {
                textBox1.Text = string.Format("{0}", DevicesManager.Instance.DevSOSUpper.GetOBS());
                textBox3.Text = string.Format("{0}", DevicesManager.Instance.DevSOSUpper.GetFrontDetectState());
            }
            else if (m_SensorType == FrontSensorType.OBS)
            {
                textBox1.Text = string.Format("{0}", DevicesManager.Instance.DevOBSLower.GetOBS());
                textBox3.Text = string.Format("{0}", DevicesManager.Instance.DevOBSLower.GetFrontDetectState());
            }
            textBox4.Text = string.Format("{0}", ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.CollisionDistance);
            textBox5.Text = string.Format("{0}", ProcessDataHandler.Instance.CurTransferCommand.RemainBcrDistance);
        }

        private void button_MouseClick(object sender, MouseEventArgs e)
        {
            uint set_value = 0;
            if (uint.TryParse((sender as Button).Text, out set_value))
            {
                bool bit1 = ((set_value >> 0) & 0x01) == 0x01;
                bool bit2 = ((set_value >> 1) & 0x01) == 0x01;
                bool bit3 = ((set_value >> 2) & 0x01) == 0x01;
                bool bit4 = ((set_value >> 3) & 0x01) == 0x01;
                if (m_SensorType == FrontSensorType.SOS)
                {
                    DevicesManager.Instance.DevSOSUpper.OBS.OBS1.SetDi(bit1);
                    DevicesManager.Instance.DevSOSUpper.OBS.OBS2.SetDi(bit2);
                    DevicesManager.Instance.DevSOSUpper.OBS.OBS3.SetDi(bit3);
                    DevicesManager.Instance.DevSOSUpper.OBS.OBS4.SetDi(bit4);
                }
                else if (m_SensorType == FrontSensorType.OBS)
                {
                    DevicesManager.Instance.DevOBSLower.OBS.OBS1.SetDi(bit1);
                    DevicesManager.Instance.DevOBSLower.OBS.OBS2.SetDi(bit2);
                    DevicesManager.Instance.DevOBSLower.OBS.OBS3.SetDi(bit3);
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            uint set_value = 0;
            if (uint.TryParse(textBox2.Text, out set_value))
            {
                if (m_SensorType == FrontSensorType.SOS)
                {
                    ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.UBSLevel0 = (int)set_value;
                    DevicesManager.Instance.DevSOSUpper.SetOBS(set_value);
                }
                else
                {
                    ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.UBSLevel1 = (int)set_value;
                    //DevicesManager.Instance.DevOBSLower.SetOBS(set_value);
                }
            }
        }
    }
}
