using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Device
{

    public partial class ucServoStatusDisplay : UCon, IUpdateUCon
    {
        #region Enum
        public enum StatusType
        {
            HomeDone,
            ServoOnOff,
            Alarm,
        }
        #endregion

        #region Fields
        private string m_ExceptionMessage = string.Empty;
        private AxisTag m_AxisTag = new AxisTag();
        private StatusType m_StatusType = StatusType.HomeDone;
        private Color m_BackColorOfSignalOff;
        private Color m_BackColorOfSignalOn;
        private Color m_ForeColorOfOfSignalOff;
        private Color m_ForeColorOfOfSignalOn;
        #endregion

        #region Constructor
        public ucServoStatusDisplay() : base(OperateMode.Manual)
        {
            InitializeComponent();
        }
        #endregion

        #region Property
        [Category("! Setting - Device Select")]
        public AxisTag AxisTag
        {
            get { return m_AxisTag; }
            set { m_AxisTag = value; }
        }
        [Category("! setting - device select")]
        public StatusType StatusType1
        {
            get { return m_StatusType; }
            set { m_StatusType = value; }
        }

        [Category("! Setting - UI")]
        public Color BackColorOfSignalOff
        {
            get { return m_BackColorOfSignalOff; }
            set { m_BackColorOfSignalOff = value; }
        }
        [Category("! Setting - UI")]
        public Color BackColorOfSignalOn
        {
            get { return m_BackColorOfSignalOn; }
            set { m_BackColorOfSignalOn = value; }
        }
        [Category("! Setting - UI")]
        public Color ForeColorOfOfSignalOff
        {
            get { return m_ForeColorOfOfSignalOff; }
            set { m_ForeColorOfOfSignalOff = value; }
        }
        [Category("! Setting - UI")]
        public Color ForeColorOfOfSignalOn
        {
            get { return m_ForeColorOfOfSignalOn; }
            set { m_ForeColorOfOfSignalOn = value; }
        }
        [Category("! Setting - UI")]
        public string TextValue
        {
            get { return this.label1.Text; }
            set { this.label1.Text = value; }
        }
        [Category("! Setting - UI")]
        public Font TextFont
        {
            get { return this.label1.Font; }
            set { this.label1.Font = value; }
        }
        #endregion

        #region Method
        public bool Initialize()
        {
            return true;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        public void UpdateState()
        {
            if (!this.Visible) return;

            try
            {
                if (m_AxisTag != null && m_AxisTag.GetAxis() != null)
                {
                    enAxisInFlag axisStatus = (m_AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus();
                    string lbText = string.Empty;
                    Color lbBackColor = Color.Empty;
                    Color lbForceColor = Color.Empty;
                    switch (StatusType1)
                    {
                        case StatusType.HomeDone:
                            {
                                lbText = (axisStatus & enAxisInFlag.HEnd) == enAxisInFlag.HEnd ? "OK" : "NG";
                                lbBackColor = (axisStatus & enAxisInFlag.HEnd) == enAxisInFlag.HEnd ? BackColorOfSignalOn : BackColorOfSignalOff;
                                lbForceColor = (axisStatus & enAxisInFlag.HEnd) == enAxisInFlag.HEnd ? ForeColorOfOfSignalOn : ForeColorOfOfSignalOff;
                            }
                            break;
                        case StatusType.ServoOnOff:
                            {
                                lbText = (axisStatus & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? "Servo ON" : "Servo Off";
                                lbBackColor = (axisStatus & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? BackColorOfSignalOn : BackColorOfSignalOff;
                                lbForceColor = (axisStatus & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? ForeColorOfOfSignalOn : ForeColorOfOfSignalOff;
                            }
                            break;
                        case StatusType.Alarm:
                            {
                                lbText = (axisStatus & enAxisInFlag.Alarm) == enAxisInFlag.Alarm ? "Alarm" : "None";
                                lbBackColor = (axisStatus & enAxisInFlag.Alarm) == enAxisInFlag.Alarm ? BackColorOfSignalOn : BackColorOfSignalOff;
                                lbForceColor = (axisStatus & enAxisInFlag.Alarm) == enAxisInFlag.Alarm ? ForeColorOfOfSignalOn : ForeColorOfOfSignalOff;
                            }
                            break;
                    }
                    if (label1.Text != lbText && lbText != string.Empty) label1.Text = lbText;
                    if (label1.BackColor != lbBackColor && lbBackColor != Color.Empty) label1.BackColor = lbBackColor;
                    if (label1.ForeColor != lbForceColor && lbForceColor != Color.Empty) label1.ForeColor = lbForceColor;
                }
            }
            catch (Exception e)
            {
                if (m_ExceptionMessage != e.Message)
                {
                    m_ExceptionMessage = e.Message;
                    string log = string.Format("UserControl : [{0}]-[{1}]\n{2}", this.GetType(), this.Name, e.Message);
                    ExceptionLog.WriteLog(log);
                }
            }
        }
        #endregion
    }
}
