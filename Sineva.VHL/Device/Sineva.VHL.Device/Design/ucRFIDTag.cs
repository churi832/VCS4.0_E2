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

namespace Sineva.VHL.Device.Design
{
    public partial class ucRFIDTag : UCon, IUpdateUCon
    {
        private DevRFID m_DevRFID;
        public ucRFIDTag()
        {
            InitializeComponent();
        }

        public bool Initialize()
        {
            m_DevRFID = DevicesManager.Instance.DevRFID;
            EventHandlerManager.Instance.CarrierIDReadingConfirm += Instance_CarrierIDReadingConfirm;
            return true;
        }

        private void Instance_CarrierIDReadingConfirm(string carrier_id)
        {
            try
            {
                if (lbTag.InvokeRequired)
                {
                    DelVoid_String d = new DelVoid_String(Instance_CarrierIDReadingConfirm);
                    this.Invoke(d, carrier_id);
                }
                else
                {
                    lbTag.Text = carrier_id;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public void UpdateState()
        {
            try
            {
                if (m_DevRFID.RFIDReading)
                {
                    lbTag.BackColor = Color.LightYellow;
                }
                else
                {
                    lbTag.BackColor = Color.LightGreen;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
    }
}
