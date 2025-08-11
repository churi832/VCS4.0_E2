using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sineva.VHL.Data;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Device;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Library;

namespace Sineva.VHL.GUI
{
    public partial class DlgPioTest : Form
    {
        #region Fields
        string[] m_InputName = new string[] { "Load Request", "Unload Request", "NC", "Ready", "NC", "NC", "Hand Off Available", "Emergency Stop" };
        string[] m_OutputName = new string[] { "Valid", "CS1", "CS2", "NC", "Transfer Request", "Busy", "Complete", "Continue" };
        string[] m_ADInputName = new string[] { "Valid", "AutoDoor Open", "AutoDoor Close", "NC", "NC", "NC", "NC", "Pass Possible" };
        string[] m_ADOutputName = new string[] { "Start", "NC", "NC", "NC", "NC", "NC", "NC", "NC" };
        string[] m_MTLInputName = new string[] { "Insert Request", "Export Request", "Busy", "Insert Ready", "Export Ready", "PIO Start", "NC", "Emergency Stop" };
        string[] m_MTLOutputName = new string[] { "Valid", "NC", "NC", "NC", "Move Request", "Move Start", "Move Complete", "NC" };
        #endregion

        #region Fields
        private string m_UsedPioName = "EQP";
        private List<CheckBox> m_CheckBoxes = new List<CheckBox>();
        private List<Label> m_InputLabels = new List<Label>();
        private List<Label> m_OutputLabels = new List<Label>();

        private List<_DevInput> m_Inputs = new List<_DevInput>();
        private List<_DevOutput> m_Outputs = new List<_DevOutput>();

        private DevEqPIO m_DevEqPio = new DevEqPIO();

        private bool m_ConnectRequest = false;
        private bool m_DisconnectRequest = false;

        private int m_PioID = 0;
        private int m_PioChannel = 0;

        private int m_SeletedPortID = 0;
        #endregion

        public DlgPioTest()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            m_CheckBoxes.Clear();
            m_CheckBoxes.Add(cbEqp);
            m_CheckBoxes.Add(cbMtl1);
            m_CheckBoxes.Add(cbAutoDoor1);
            m_CheckBoxes.Add(cbAutoDoor2);
            m_InputLabels.Clear();
            m_InputLabels.Add(lbInput1);
            m_InputLabels.Add(lbInput2);
            m_InputLabels.Add(lbInput3);
            m_InputLabels.Add(lbInput4);
            m_InputLabels.Add(lbInput5);
            m_InputLabels.Add(lbInput6);
            m_InputLabels.Add(lbInput7);
            m_InputLabels.Add(lbInput8);
            m_OutputLabels.Clear();
            m_OutputLabels.Add(lbOutput1);
            m_OutputLabels.Add(lbOutput2);
            m_OutputLabels.Add(lbOutput3);
            m_OutputLabels.Add(lbOutput4);
            m_OutputLabels.Add(lbOutput5);
            m_OutputLabels.Add(lbOutput6);
            m_OutputLabels.Add(lbOutput7);
            m_OutputLabels.Add(lbOutput8);

            m_DevEqPio = DevicesManager.Instance.DevEqpPIO;
            m_Inputs.Clear();
            m_Inputs.Add(m_DevEqPio.DiLDREQ);
            m_Inputs.Add(m_DevEqPio.DiULREQ);
            m_Inputs.Add(m_DevEqPio.DiVA);
            m_Inputs.Add(m_DevEqPio.DiREADY);
            m_Inputs.Add(m_DevEqPio.DiVS0);
            m_Inputs.Add(m_DevEqPio.DiVS1);
            m_Inputs.Add(m_DevEqPio.DiHOAVBL);
            m_Inputs.Add(m_DevEqPio.DiES);
            m_Outputs.Clear();
            m_Outputs.Add(m_DevEqPio.DoVALID);
            m_Outputs.Add(m_DevEqPio.DoCS1);
            m_Outputs.Add(m_DevEqPio.DoCS2);
            m_Outputs.Add(m_DevEqPio.DoAVBL);
            m_Outputs.Add(m_DevEqPio.DoTRREQ);
            m_Outputs.Add(m_DevEqPio.DoBUSY);
            m_Outputs.Add(m_DevEqPio.DoCOMPT);
            m_Outputs.Add(m_DevEqPio.DoCONT);
        }

        private void btnConntect_Click(object sender, EventArgs e)
        {
            if (m_PioChannel == 0 || m_PioID == 0)
            {
                MessageBox.Show("PIO ID, Channel Setting NG");
                return;
            }
            m_ConnectRequest = true;
            ButtonLog.WriteLog(this.Text.ToString(), string.Format("btnConntect_Click"));
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            m_DisconnectRequest = true;
            ButtonLog.WriteLog(this.Text.ToString(), string.Format("btnDisconnect_Click"));
        }

        private void lbOutput_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                Label lb = (Label)sender;
                int index = -1;
                int.TryParse(lb.Tag.ToString(), out index);
                if (index >= 0 && index < 8)
                {
                    if (m_DevEqPio.PioComm.IsConnected && m_DevEqPio.PioComm.IsGo())
                    {
                        bool flg = m_Outputs[index].IsDetected;
                        m_Outputs[index].SetDo(!flg);
                    }
                    else
                    {
                        MessageBox.Show("Pio Comm Not Conneted");
                    }
                }
                ButtonLog.WriteLog(this.Text.ToString(), string.Format("lbOutput_MouseDoubleClick : {0}_{1}", lb.Text, index));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void cbUsedPIO_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).CheckState == CheckState.Checked) 
            {
                foreach (CheckBox cb in m_CheckBoxes)
                {
                    if (cb != (sender as CheckBox)) cb.Checked = false;                   
                }
                m_UsedPioName = (string)(sender as CheckBox).Tag;
                UpdateLableName();
            }
        }

        private void UpdateLableName()
        {
            try
            {
                bool ng = false;
                int index = 0;
                if (m_UsedPioName == (string)cbEqp.Tag)
                {
                    foreach (Label lb in m_InputLabels) lb.Text = m_InputName[index++]; index = 0;
                    foreach (Label lb in m_OutputLabels) lb.Text = m_OutputName[index++];

                    if (m_SeletedPortID != 0)
                    {
                        if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(m_SeletedPortID))
                        {
                            m_PioID = DatabaseHandler.Instance.DictionaryPortDataList[m_SeletedPortID].PIOID;
                            m_PioChannel = DatabaseHandler.Instance.DictionaryPortDataList[m_SeletedPortID].PIOCH;
                        }
                        else ng = true;
                    }
                    else ng = true;
                }
                else if (m_UsedPioName == (string)cbMtl1.Tag)
                {
                    foreach (Label lb in m_InputLabels) lb.Text = m_MTLInputName[index++]; index = 0;
                    foreach (Label lb in m_OutputLabels) lb.Text = m_MTLOutputName[index++];

                    DataItem_PIODevice pio = DatabaseHandler.Instance.DictionaryPIODevice.Values.Where(x => x.DeviceType == PIODeviceType.MTL).FirstOrDefault();
                    if (pio != null)
                    {
                        m_PioID = pio.PIOID;
                        m_PioChannel = pio.PIOCH;
                    }
                    else ng = true;
                }
                else if (m_UsedPioName == (string)cbAutoDoor1.Tag || m_UsedPioName == (string)cbAutoDoor2.Tag)
                {
                    foreach (Label lb in m_InputLabels) lb.Text = m_ADInputName[index++]; index = 0;
                    foreach (Label lb in m_OutputLabels) lb.Text = m_ADOutputName[index++];

                    DataItem_PIODevice pio = null;
                    if (m_UsedPioName == (string)cbAutoDoor1.Tag)
                        pio = DatabaseHandler.Instance.DictionaryPIODevice.Values.Where(x => x.DeviceType == PIODeviceType.AutoDoor1).FirstOrDefault();
                    else if (m_UsedPioName == (string)cbAutoDoor2.Tag)
                        pio = DatabaseHandler.Instance.DictionaryPIODevice.Values.Where(x => x.DeviceType == PIODeviceType.AutoDoor2).FirstOrDefault();
                    if (pio != null)
                    {
                        m_PioID = pio.PIOID;
                        m_PioChannel = pio.PIOCH;
                    }
                    else ng = true;
                }
                if (ng)
                {
                    m_PioID = 0;
                    m_PioChannel = 0;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void tbPortID_Click(object sender, EventArgs e)
        {
            if (m_UsedPioName != (string)cbEqp.Tag) return;
            int id = 0;
            using (UIEditorPortSelectForm form = new UIEditorPortSelectForm((int)id))
            {
                form.StartPosition = FormStartPosition.CenterParent;
                form.TopMost = true;
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    m_SeletedPortID = form.SelectPortID;
                    tbPortID.Text = string.Format("{0}", m_SeletedPortID);
                }
            }
            ButtonLog.WriteLog(this.Text.ToString(), string.Format("tbPortID_Click : m_SeletedPortID={0}", m_SeletedPortID));

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                bool connect = m_DevEqPio.PioComm.IsConnected;
                btnConntect.Enabled = !connect && !m_ConnectRequest;
                btnDisconnect.Enabled = connect && !m_DisconnectRequest;
                UpdateState();

                if (m_ConnectRequest || m_DisconnectRequest)
                {
                    int id = 0;
                    int ch = 0;
                    if (m_ConnectRequest) { id = m_PioID; ch = m_PioChannel; }
                    int rv1 = m_DevEqPio.SetChannelId(id, ch);
                    if (rv1 >= 0)
                    {
                        m_ConnectRequest = false;
                        m_DisconnectRequest = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void UpdateState()
        {
            try
            {
                int index = 0;
                foreach (_DevInput input in m_Inputs)
                {
                    if (input.IsDetected && m_InputLabels[index].BackColor != Color.Green) m_InputLabels[index].BackColor = Color.Green;
                    else if (!input.IsDetected && m_InputLabels[index].BackColor != Color.White) m_InputLabels[index].BackColor = Color.White;
                    index++;
                }
                index = 0;
                foreach (_DevOutput output in m_Outputs)
                {
                    if (output.IsDetected && m_OutputLabels[index].BackColor != Color.Red) m_OutputLabels[index].BackColor = Color.Green;
                    else if (!output.IsDetected && m_OutputLabels[index].BackColor != Color.White) m_OutputLabels[index].BackColor = Color.White;
                    index++;
                }

                bool isGo = m_DevEqPio.PioComm.IsGo();
                if (isGo && lbGo.BackColor != Color.Green) lbGo.BackColor = Color.Green;
                else if (!isGo && lbGo.BackColor != Color.White) lbGo.BackColor = Color.White;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
