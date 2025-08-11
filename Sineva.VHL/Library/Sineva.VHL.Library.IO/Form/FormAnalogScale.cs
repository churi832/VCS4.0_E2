using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Library.IO
{
    public partial class FormAnalogScale : Form
    {
        #region Field
        private IoChannel m_IoChannel = null;
        private AnalogScale m_NewScale = null;

        private Timer _tmrUpdate = null;
        #endregion

        public FormAnalogScale(IoChannel io)
        {
            InitializeComponent();

            if(io != null)
            {
                m_IoChannel = io;
                m_NewScale = ObjectCopier.Clone(io.Scale);

                tbAdcMax.Text = m_NewScale.AdcMax.ToString();
                tbAdcMin.Text = m_NewScale.AdcMin.ToString();
                tbRealMax.Text = m_NewScale.RealMax.ToString();
                tbRealMin.Text = m_NewScale.RealMin.ToString();
                chkScaleUse.Checked = m_NewScale.UseScale;
                numDecimal.Value = m_NewScale.DecimalPoint;

                cbPreset.DataSource = Enum.GetValues(typeof(ScalePreset));
                cbPreset.SelectedItem = null;
                cbUnitType.DataSource = Enum.GetValues(typeof(UnitType));
                cbUnitType.SelectedItem = m_NewScale.Unit;


                _tmrUpdate = new Timer();
                _tmrUpdate.Tick += OnTimerUpdate;
                _tmrUpdate.Interval = 100;
                _tmrUpdate.Start();
            }
        }

        private void OnTimerUpdate(object sender, EventArgs e)
        {
            tbCurAdc.Text = m_IoChannel.State;
            int adc;
            if(int.TryParse(tbCurAdc.Text, out adc))
            {
                m_NewScale.CurAdc = adc;
                m_NewScale.UseScale = chkScaleUse.Checked;
                double scaleVal = m_NewScale.GetCalValue();
                if(tbValue.Text != scaleVal.ToString()) tbValue.Text = scaleVal.ToString();
            }
            else
            {
                if(tbValue.Text != "N/A") tbValue.Text = "N/A";
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            int adc;
            double real;
            if(int.TryParse(tbAdcMax.Text, out adc)) { m_NewScale.AdcMax = adc; tbAdcMax.BackColor = Color.LightYellow; } else tbAdcMax.BackColor = Color.Red;
            if(int.TryParse(tbAdcMin.Text, out adc)) { m_NewScale.AdcMin = adc; tbAdcMin.BackColor = Color.LightYellow; } else tbAdcMin.BackColor = Color.Red;
            if(double.TryParse(tbRealMax.Text, out real)) { m_NewScale.RealMax = real; tbRealMax.BackColor = Color.LightYellow; } else tbRealMax.BackColor = Color.Red;
            if(double.TryParse(tbRealMin.Text, out real)) { m_NewScale.RealMin = real; tbRealMin.BackColor = Color.LightYellow; } else tbRealMin.BackColor = Color.Red;
            m_NewScale.UseScale = chkScaleUse.Checked;
            m_NewScale.DecimalPoint = (int)numDecimal.Value;
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            m_NewScale = ObjectCopier.Clone(m_IoChannel.Scale);

            tbAdcMax.Text = m_NewScale.AdcMax.ToString();
            tbAdcMin.Text = m_NewScale.AdcMin.ToString();
            tbRealMax.Text = m_NewScale.RealMax.ToString();
            tbRealMin.Text = m_NewScale.RealMin.ToString();
            chkScaleUse.Checked = m_NewScale.UseScale;
            numDecimal.Value = m_NewScale.DecimalPoint;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            m_IoChannel.Scale = m_NewScale;
            m_IoChannel.Scale.UseScale = chkScaleUse.Checked;
            m_IoChannel.Scale.Unit = cbUnitType.SelectedItem != null ? (UnitType)cbUnitType.SelectedItem : UnitType.None;

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cbPreset_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if(cbPreset.SelectedItem != null)
            {
                AnalogScale preset = m_NewScale.GetPreset((ScalePreset)cbPreset.SelectedItem);

                tbAdcMax.Text = preset.AdcMax.ToString();
                tbAdcMin.Text = preset.AdcMin.ToString();
                tbRealMax.Text = preset.RealMax.ToString();
                tbRealMin.Text = preset.RealMin.ToString();
                cbUnitType.SelectedItem = preset.Unit;
            }
        }
    }
}
