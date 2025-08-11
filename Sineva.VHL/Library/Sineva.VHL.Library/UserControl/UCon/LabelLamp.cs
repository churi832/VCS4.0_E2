using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public partial class LabelLamp : UserControl
    {
        #region enum
        public enum LampKind
        {
            SingleAlert,
            SingleNormal,
            DualAlert,
            DualNormal,
        }
        #endregion

        #region Field
        private LampKind m_LampKindSelect = LampKind.SingleNormal;
        private Bitmap m_OnImage;
        private Bitmap m_OffImage;
        private Bitmap m_NoopImage;
        private bool m_CurStatus;
        private int m_nCurStatus;
        private bool m_Initiated = false;

        private Timer m_TimerMouseHover = null;
        private Timer m_TimerTicks = null;
        private uint m_MouseHoverTicks = 0;
        private uint m_MouseLeaveTicks = 0;
        private bool m_MouseHover = false;
        private bool m_MouseHoverChecked = false;
        private bool m_MouseEnterLabelText = false;
        private bool m_MouseEnterPictureBox = false;

        private uint m_UserControlTicks = 0;
        #endregion

        #region Property
        [Category("!Image Setting")]
        public LampKind LampKindSelect
        {
            get { return m_LampKindSelect; }
            set { m_LampKindSelect = value; }
        }
        [Category("!Image Setting")]
        public Color ControlColor
        {
            get { return this.BackColor; }
            set { this.BackColor = value; }
        }
        [Category("!Text Setting")]
        public Font TextFont
        {
            get { return this.lblText.Font; }
            set { this.lblText.Font = value; }
        }
        [Category("!Text Setting")]
        public Color TextColor
        {
            get { return this.lblText.ForeColor; }
            set { this.lblText.ForeColor = value; }
        }
        [Category("!Text Setting")]
        public ContentAlignment TextAlignment
        {
            get { return this.lblText.TextAlign; }
            set { this.lblText.TextAlign = value; }
        }
        [Category("!Text Setting")]
        public string DisplayText
        {
            get { return this.lblText.Text; }
            set { this.lblText.Text = value; }
        }
        #endregion

        #region Constructor
        public LabelLamp()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;

            pbLamp.BackgroundImage = Properties.Resources.diode;
            pbLamp.BackgroundImageLayout = ImageLayout.Stretch;
            m_CurStatus = false;

            if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                m_TimerMouseHover = new Timer();
                m_TimerMouseHover.Interval = 100;
                m_TimerMouseHover.Tick += TimerMouseHover_Tick;
                m_TimerMouseHover.Start();

                m_TimerTicks = new Timer();
                m_TimerTicks.Interval = 10;
                m_TimerTicks.Tick += TimerTicks_Tick;
                m_TimerTicks.Start();

                pbLamp.MouseEnter += pbLamp_MouseEnter;
                pbLamp.MouseLeave += pbLamp_MouseLeave;
                lblText.MouseEnter += lblText_MouseEnter;
                lblText.MouseLeave += lblText_MouseLeave;
            }
        }
        #endregion

        #region Method
        private void Initialize()
        {
            if (m_Initiated)
                return;

            if (m_LampKindSelect == LampKind.SingleAlert || m_LampKindSelect == LampKind.SingleNormal)
            {
                m_OffImage = Properties.Resources.diode;
                if (m_LampKindSelect == LampKind.SingleAlert) m_OnImage = Properties.Resources.diode_red;
                else m_OnImage = Properties.Resources.diode_green;
            }
            else
            {
                if(m_LampKindSelect == LampKind.DualAlert)
                {
                    m_OffImage = Properties.Resources.diode_green;
                    m_OnImage = Properties.Resources.diode_red;
                }
                else
                {
                    m_OffImage = Properties.Resources.diode_red;
                    m_OnImage = Properties.Resources.diode_green;
                }
                m_NoopImage = Properties.Resources.diode;
                m_nCurStatus = 0;
            }

            pbLamp.BackgroundImage = m_OffImage;
            pbLamp.BackgroundImageLayout = ImageLayout.Stretch;
            m_CurStatus = false;

            m_Initiated = true;
        }

        public void SetStatus(bool on)
        {
            if (!m_Initiated)
                Initialize();

            if(on != m_CurStatus)
            {
                if(on && pbLamp.BackgroundImage != m_OnImage) pbLamp.BackgroundImage = m_OnImage;
                else if(!on && pbLamp.BackgroundImage != m_OffImage) pbLamp.BackgroundImage = m_OffImage;

                m_CurStatus = on;

                ButtonLog.WriteLog(this.Name.ToString(), string.Format("SetStatus={0}", on ? "ON" : "OFF"));
            }
        }
        /// <summary>
        /// OFF(-1) / NONE(0) / ON(1)
        /// </summary>
        /// <param name="status"></param>
        public void SetStatus(int status)
        {
            if(!m_Initiated) Initialize();

            if(m_LampKindSelect == LampKind.DualAlert || m_LampKindSelect == LampKind.DualNormal)
            {
                if(m_nCurStatus != status)
                {
                    m_nCurStatus = status;
                    if(m_nCurStatus == 0) pbLamp.BackgroundImage = m_NoopImage;
                    else if(m_nCurStatus < 0) pbLamp.BackgroundImage = m_OffImage;
                    else if(m_nCurStatus > 0) pbLamp.BackgroundImage = m_OnImage;
                }
            }
        }
        public bool GetStatus()
        {
            return m_CurStatus;
        }
        public int GetStatusThree()
        {
            return m_nCurStatus;
        }
        public LabelLamp Clone()
        {
            LabelLamp clone = new LabelLamp();
            clone.LampKindSelect = this.LampKindSelect;
            clone.ControlColor = this.ControlColor;
            clone.TextFont = this.TextFont;
            clone.TextColor = this.TextColor;
            clone.TextAlignment = this.TextAlignment;
            clone.DisplayText = this.DisplayText;
            clone.Size = this.Size;
            return clone;
        }
        #endregion

        #region Event Handler
        private void lblText_MouseLeave(object sender, EventArgs e)
        {
            m_MouseEnterLabelText = false;
        }
        private void lblText_MouseEnter(object sender, EventArgs e)
        {
            m_MouseEnterLabelText = true;
        }
        private void pbLamp_MouseLeave(object sender, EventArgs e)
        {
            m_MouseEnterPictureBox = false;
        }
        private void pbLamp_MouseEnter(object sender, EventArgs e)
        {
            m_MouseEnterPictureBox = true;
        }
        private void TimerMouseHover_Tick(object sender, EventArgs e)
        {
            //if(m_MouseEnterLabelText || m_MouseEnterPictureBox)
            //{
            //    if(!m_MouseHover)
            //    {
            //        m_MouseHoverTicks = XFunc.GetTickCount();
            //        m_MouseHover = true;
            //    }
            //    if(m_MouseHover && !m_MouseHoverChecked && XFunc.GetTickCount() - m_MouseHoverTicks > 500)
            //    {
            //        m_MouseHoverChecked = true;
            //        OnMouseHover(new EventArgs());
            //    }
            //}
            //else if(m_MouseHoverChecked)
            //{
            //    if(m_MouseHover)
            //    {
            //        m_MouseLeaveTicks = XFunc.GetTickCount();
            //        m_MouseHover = false;
            //    }
            //    if(!m_MouseHover && XFunc.GetTickCount() - m_MouseLeaveTicks > 300)
            //    {
            //        m_MouseHoverChecked = false;
            //        OnMouseLeave(new EventArgs());
            //    }
            //}
            if(m_MouseEnterLabelText || m_MouseEnterPictureBox)
            {
                if(!m_MouseHover)
                {
                    m_MouseHoverTicks = m_UserControlTicks;
                    m_MouseHover = true;
                }
                if(m_MouseHover && !m_MouseHoverChecked && m_UserControlTicks - m_MouseHoverTicks > 500)
                {
                    m_MouseHoverChecked = true;
                    OnMouseHover(new EventArgs());
                }
            }
            else if(m_MouseHoverChecked)
            {
                if(m_MouseHover)
                {
                    m_MouseLeaveTicks = m_UserControlTicks;
                    m_MouseHover = false;
                }
                if(!m_MouseHover && m_UserControlTicks - m_MouseLeaveTicks > 300)
                {
                    m_MouseHoverChecked = false;
                    OnMouseLeave(new EventArgs());
                }
            }
        }
        private void TimerTicks_Tick(object sender, EventArgs e)
        {
            m_UserControlTicks += (uint)m_TimerTicks.Interval;
        }
        private void LabelLamp_SizeChanged(object sender, EventArgs e)
        {
            pbLamp.Size = new Size(this.Height, this.Height);
        }

        private void LabelLamp_ParentChanged(object sender, EventArgs e)
        {
            if(this != null)
                pbLamp.Size = new Size(this.Height, this.Height);
        }
        #endregion
    }
}
