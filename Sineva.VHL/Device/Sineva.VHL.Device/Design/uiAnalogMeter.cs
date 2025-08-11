using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Device
{
    public class uiAnalogMeter : Control, IUpdateUCon
    {
        #region Enum

        public enum AnalogMeterStyle
        {
            Circular = 0,
        };
        public enum ValueType
        {
            Position,
            Velocity,
            Torque,
        }
        #endregion Enumerator

        #region Fields

        private AnalogMeterStyle m_MeterStyle;
        private Color m_NeedleColor;
        private Color m_FillColor;
        private Color m_ScaleColor;
        private bool viewGlass;
        private double m_Value;
        private double minValue;
        private double maxValue;
        private int scaleDivisions;
        private int scaleSubDivisions;
        private LBAnalogMeterRenderer renderer;
        private int m_DecimalPoint = 2;
        private AxisTag m_AxisTag = new AxisTag();
        private ValueType m_TypeOfValue = ValueType.Velocity;

        #endregion Properties variables

        #region Class variables

        protected PointF needleCenter;
        protected RectangleF drawRect;
        protected RectangleF glossyRect;
        protected RectangleF needleCoverRect;
        protected float startAngle;
        protected float endAngle;
        protected float drawRatio;
        protected LBAnalogMeterRenderer defaultRenderer;

        #endregion Class variables



        #region Costructors

        public uiAnalogMeter()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer, true);
            base.DoubleBuffered = true;

            // Properties initialization
            m_NeedleColor = Color.Yellow;
            m_ScaleColor = Color.White;
            m_MeterStyle = AnalogMeterStyle.Circular;
            viewGlass = false;
            startAngle = 135;
            endAngle = 405;
            minValue = 0;
            maxValue = 100;
            m_Value = 0;
            scaleDivisions = 11;
            scaleSubDivisions = 4;
            renderer = null;

            // Set the styles for drawing
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);

            // Create the default renderer
            defaultRenderer = new LBDefaultAnalogMeterRenderer();
            defaultRenderer.AnalogMeter = this;

            Width = Height = 180;
        }

        #endregion Costructors

        #region Properties
        [Category("Custom"), Description("AxisTag")]
        public AxisTag AxisTag
        {
            get { return m_AxisTag; }
            set { m_AxisTag = value; }
        }
        [Category("Custom"), Description("TypeOfValue")]
        public ValueType TypeOfValue
        {
            get { return m_TypeOfValue; }
            set { m_TypeOfValue = value; }
        }
        [Category("Custom"), Description("DecimalPoint")]
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
        [Category("Appearance"), Description("Style of the control"), DefaultValue(AnalogMeterStyle.Circular)]
        public AnalogMeterStyle MeterStyle
        {
            get { return m_MeterStyle; }
            set
            {
                m_MeterStyle = value;
                Invalidate();
            }
        }

        [Category("Appearance"), Description("Color of the body of the control"), DefaultValue(typeof(Color), "80, 160, 255")]
        public Color BodyColor
        {
            get => m_FillColor;
            set => m_FillColor = value;
        }

        [Category("Appearance"), Description("Color of the needle"), DefaultValue(typeof(Color), "Yellow")]
        public Color NeedleColor
        {
            get { return m_NeedleColor; }
            set
            {
                m_NeedleColor = value;
                Invalidate();
            }
        }

        [Category("Appearance"), Description("Show or hide the glass effect"), DefaultValue(false)]
        public bool ViewGlass
        {
            get { return viewGlass; }
            set
            {
                viewGlass = value;
                Invalidate();
            }
        }

        [Category("Appearance"), Description("Color of the scale of the control"), DefaultValue(typeof(Color), "White")]
        public Color ScaleColor
        {
            get { return m_ScaleColor; }
            set
            {
                m_ScaleColor = value;
                Invalidate();
            }
        }

        [Category("Behavior"), Description("Value of the data"), DefaultValue(0)]
        public double Value
        {
            get { return m_Value; }
            set
            {
                double val = value;
                if (val > maxValue)
                    val = maxValue;

                if (val < minValue)
                    val = minValue;

                m_Value = val;
                Invalidate();
            }
        }

        [Category("Behavior"), Description("Minimum value of the data"), DefaultValue(0)]
        public double MinValue
        {
            get { return minValue; }
            set
            {
                minValue = value;
                Invalidate();
            }
        }

        [Category("Behavior"), Description("Maximum value of the data"), DefaultValue(100)]
        public double MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                Invalidate();
            }
        }

        [Category("Appearance"), Description("Number of the scale divisions"), DefaultValue(11)]
        public int ScaleDivisions
        {
            get { return scaleDivisions; }
            set
            {
                scaleDivisions = value;
                CalculateDimensions();
                Invalidate();
            }
        }

        [Category("Appearance"), Description("Number of the scale subdivisions"), DefaultValue(4)]
        public int ScaleSubDivisions
        {
            get { return scaleSubDivisions; }
            set
            {
                scaleSubDivisions = value;
                CalculateDimensions();
                Invalidate();
            }
        }

        [Browsable(false)]
        public LBAnalogMeterRenderer Renderer
        {
            get { return renderer; }
            set
            {
                renderer = value;
                if (renderer != null)
                    renderer.AnalogMeter = this;
                Invalidate();
            }
        }

        #endregion Properties

        #region Public methods

        public float GetDrawRatio()
        {
            return drawRatio;
        }

        public float GetStartAngle()
        {
            return startAngle;
        }

        public float GetEndAngle()
        {
            return endAngle;
        }

        public PointF GetNeedleCenter()
        {
            return needleCenter;
        }

        #endregion Public methods

        #region Events delegates

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            // Calculate dimensions
            CalculateDimensions();
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0) return;
            RectangleF _rc = new RectangleF(0, 0, Width, Height);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (renderer == null)
            {
                defaultRenderer.DrawBackground(e.Graphics, _rc);
                defaultRenderer.DrawBody(e.Graphics, drawRect);
                defaultRenderer.DrawThresholds(e.Graphics, drawRect);
                defaultRenderer.DrawDivisions(e.Graphics, drawRect);
                defaultRenderer.DrawUM(e.Graphics, drawRect);
                defaultRenderer.DrawValue(e.Graphics, drawRect);
                defaultRenderer.DrawNeedle(e.Graphics, drawRect);
                defaultRenderer.DrawNeedleCover(e.Graphics, needleCoverRect);
                defaultRenderer.DrawGlass(e.Graphics, glossyRect);
            }
            else
            {
                if (Renderer.DrawBackground(e.Graphics, _rc) == false)
                    defaultRenderer.DrawBackground(e.Graphics, _rc);
                if (Renderer.DrawBody(e.Graphics, drawRect) == false)
                    defaultRenderer.DrawBody(e.Graphics, drawRect);
                if (Renderer.DrawThresholds(e.Graphics, drawRect) == false)
                    defaultRenderer.DrawThresholds(e.Graphics, drawRect);
                if (Renderer.DrawDivisions(e.Graphics, drawRect) == false)
                    defaultRenderer.DrawDivisions(e.Graphics, drawRect);
                if (Renderer.DrawUM(e.Graphics, drawRect) == false)
                    defaultRenderer.DrawUM(e.Graphics, drawRect);
                if (Renderer.DrawValue(e.Graphics, drawRect) == false)
                    defaultRenderer.DrawValue(e.Graphics, drawRect);
                if (Renderer.DrawNeedle(e.Graphics, drawRect) == false)
                    defaultRenderer.DrawNeedle(e.Graphics, drawRect);
                if (Renderer.DrawNeedleCover(e.Graphics, needleCoverRect) == false)
                    defaultRenderer.DrawNeedleCover(e.Graphics, needleCoverRect);
                if (Renderer.DrawGlass(e.Graphics, glossyRect) == false)
                    defaultRenderer.DrawGlass(e.Graphics, glossyRect);
            }
        }

        #endregion Events delegates

        #region Virtual functions

        protected virtual void CalculateDimensions()
        {
            // Rectangle
            float x = 0;
            float y = 0;
            float w = Width;
            float h = Height;

            // Calculate ratio
            drawRatio = (Math.Min(w, h)) / 200;
            if (drawRatio < 0.00000001)
                drawRatio = 1;

            // Draw rectangle
            drawRect.X = x;
            drawRect.Y = y;
            drawRect.Width = w - 2;
            drawRect.Height = h - 2;

            if (w < h)
                drawRect.Height = w;
            else if (w > h)
                drawRect.Width = h;

            if (drawRect.Width < 10)
                drawRect.Width = 10;
            if (drawRect.Height < 10)
                drawRect.Height = 10;

            // Calculate needle center
            needleCenter.X = drawRect.X + (drawRect.Width / 2);
            needleCenter.Y = drawRect.Y + (drawRect.Height / 2);

            // Needle cover rect
            needleCoverRect.X = needleCenter.X - (20 * drawRatio);
            needleCoverRect.Y = needleCenter.Y - (20 * drawRatio);
            needleCoverRect.Width = 40 * drawRatio;
            needleCoverRect.Height = 40 * drawRatio;

            // Glass effect rect
            glossyRect.X = drawRect.X + (20 * drawRatio);
            glossyRect.Y = drawRect.Y + (10 * drawRatio);
            glossyRect.Width = drawRect.Width - (40 * drawRatio);
            glossyRect.Height = needleCenter.Y + (30 * drawRatio);
        }

        public void UpdateState()
        {
            if (!this.Visible) return;

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
                    value = Math.Round(value, m_DecimalPoint);
                    if (double.IsNaN(m_Value) || Value != value)
                    {
                        Value = value;
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        public bool Initialize()
        {
            bool rv = true;
            return rv;
        }
        #endregion
    }
}