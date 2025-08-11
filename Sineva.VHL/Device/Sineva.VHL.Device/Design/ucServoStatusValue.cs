using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.Device
{
    public partial class ucServoStatusValue : UCon, IUpdateUCon
    {
        #region Enum
        public enum ValueType
        {
            Position,
            Velocity,
            Torque,
        }
        #endregion
        
        #region Field
        private string m_ExceptionMessage = string.Empty;
        private AxisTag m_AxisTag = new AxisTag();
        private ValueType m_TypeOfValue = ValueType.Position;
        private int m_DecimalPoint = 4;
        private double m_Value = double.NaN;
        #endregion

        #region Property
        [Category("! Setting - Device Select")]
        public AxisTag AxisTag
        {
            get { return m_AxisTag; }
            set { m_AxisTag = value; }
        }
        [Category("! Setting - Device Select")]
        public ValueType TypeOfValue
        {
            get { return m_TypeOfValue; }
            set { m_TypeOfValue = value; }
        }

        [Category("! Setting - UI")]
        public ContentAlignment TextAlignment
        {
            get { return this.label1.TextAlign; }
            set { this.label1.TextAlign = value; }
        }
        [Category("! Setting - UI")]
        public Color ColorOfBox
        {
            get { return this.label1.BackColor; }
            set { this.label1.BackColor = value; }
        }
        [Category("! Setting - UI")]
        public Color ColorOfText
        {
            get { return this.label1.ForeColor; }
            set { this.label1.ForeColor = value; }
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
        [Category("! Setting - UI")]
        public int DecimalPoint
        {
            get { return m_DecimalPoint; }
            set
            {
                if (value < 0) value = 0;
                else if (value > 8) value = 8;
                m_DecimalPoint = value;
            }
        }
        #endregion

        #region Constructor
        public ucServoStatusValue() : base(OperateMode.Manual)
        {
            InitializeComponent();
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
            if (!this.Visible)
                return;

            try
            {
                if (m_AxisTag != null && m_AxisTag.GetAxis() != null)
                {
                    double value = 0;
                    switch (m_TypeOfValue)
                    {
                        case ValueType.Position:
                            value = m_AxisTag.GetAxis().CurPos; // (m_AxisTag.GetAxis() as IAxisCommand).GetAxisCurPos();
                            break;
                        case ValueType.Velocity:
                            value = m_AxisTag.GetAxis().CurSpeed; // (m_AxisTag.GetAxis() as IAxisCommand).GetAxisCurSpeed();
                            break;
                        case ValueType.Torque:
                            value = m_AxisTag.GetAxis().CurTorque; // (m_AxisTag.GetAxis() as IAxisCommand).GetAxisCurTorque();
                            break;
                    }

                    if (m_Value != value || string.IsNullOrEmpty(label1.Text))
                    {
                        if (double.IsNaN(m_Value) || m_Value != value)
                        {
                            m_Value = value;
                            label1.Text = m_DecimalPoint == 1 ? string.Format("{0:F1}", m_Value) :
                                            m_DecimalPoint == 2 ? string.Format("{0:F2}", m_Value) :
                                            m_DecimalPoint == 3 ? string.Format("{0:F3}", m_Value) :
                                            m_DecimalPoint == 4 ? string.Format("{0:F4}", m_Value) :
                                            m_DecimalPoint == 5 ? string.Format("{0:F5}", m_Value) :
                                            m_DecimalPoint == 6 ? string.Format("{0:F6}", m_Value) :
                                            m_DecimalPoint == 7 ? string.Format("{0:F7}", m_Value) :
                                            m_DecimalPoint == 8 ? string.Format("{0:F8}", m_Value) : string.Format("{0:F0}", m_Value);
                        }
                    }
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

        public string GetText()
        {
            return label1.Text;
        }
        #endregion
    }
}
