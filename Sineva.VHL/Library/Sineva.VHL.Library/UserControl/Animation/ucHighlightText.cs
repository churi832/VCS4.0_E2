using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public partial class ucHighlightText : UserControl
    {
        #region Fields
        private string m_No = string.Empty;
        private string m_Text = string.Empty;
        private enMeasureProc m_Progress = enMeasureProc.Begin;
        private bool m_Select = false;
        #endregion

        #region Properties
        public string No
        {
            get { return m_No; }
            set { m_No = value; lbNo.Text = m_No.ToString(); }
        }
        public string Text1
        {
            get { return m_Text; }
            set { m_Text = value; lbText.Text = m_Text.ToString(); }
        }
        public enMeasureProc Progress
        {
            get { return m_Progress; }
            set
            {
                m_Progress = value;
                if (m_Progress == enMeasureProc.Begin) cbDone.Image = Properties.Resources.icon_4;
                else if (m_Progress == enMeasureProc.Ing) cbDone.Image = Properties.Resources.icon_20;
                else if (m_Progress == enMeasureProc.End) cbDone.Image = Properties.Resources.icon_2;
                else cbDone.Hide();
            }
        }
        public bool Select
        {
            get { return m_Select; }
            set { m_Select = value; }
        }
        #endregion

        public ucHighlightText()
        {
            InitializeComponent();

            if (XFunc.IsRunTime())
                this.DoubleBuffered = true;
        }

        public void SetButton()
        {
            m_Select = true;
            lbText.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            lbText.BackColor = Color.Yellow;
            lbText.ForeColor = Color.Black;
        }

        public void ResetButton()
        {
            m_Select = false;
            lbText.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            lbText.BackColor = Color.Transparent;
            lbText.ForeColor = Color.White;
        }

        private void LbText_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_Select)
            {
                // back ground color를 변경하자
                lbText.BackColor = Color.LightCyan;
                lbText.ForeColor = Color.Black;
            }
        }

        private void LbText_MouseLeave(object sender, EventArgs e)
        {
            if (!m_Select)
            {
                lbText.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                lbText.BackColor = Color.Transparent;
                lbText.ForeColor = Color.White;
            }
        }

        private void LbText_MouseHover(object sender, EventArgs e)
        {
            // Text Font를 Bold로 바꾸자
            lbText.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        }

        private void LbText_MouseClick(object sender, MouseEventArgs e)
        {
            this.OnMouseClick(e);
            lbText.BackColor = Color.Yellow;
            lbText.ForeColor = Color.Black;
            m_Select = true;
        }
    }
}
