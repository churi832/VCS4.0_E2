using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sineva.VHL.Library
{
    public class DrawCircle : DrawTransparentPanel
    {
        #region Fields
        private bool m_GradientType = true;
        private int m_PenWidth = 5;
        private DashStyle m_PenDashStyle = DashStyle.Dash;

        private Color m_BorderColor = Color.Black;
        private bool m_BorderVisible = true;
        #endregion

        #region Property
        public DashStyle PenDashStyle { get { return m_PenDashStyle; } set { m_PenDashStyle = value; this.Invalidate(false); } }
        public int PenWidth { get { return m_PenWidth; } set { m_PenWidth = value; this.Invalidate(false); } }
        public bool GradientType { get { return m_GradientType; } set { m_GradientType = value; this.Invalidate(false); } }
        public Color BorderColor { get { return m_BorderColor; } set { m_BorderColor = value; this.Invalidate(false); } }
        public bool BorderVisible { get { return m_BorderVisible; } set { m_BorderVisible = value; this.Invalidate(false); } }
        #endregion

        #region Constructor
        public DrawCircle()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }
        #endregion

        #region Methods
        protected override void OnResize(EventArgs e)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(this.ClientRectangle);

            this.Region = new Region(path);
        }
        protected override void OnPaint(PaintEventArgs pea)
        {
            Graphics g = pea.Graphics;
            UpdatePanel(g);

            // Circle을 그리자
            Rectangle rect = this.ClientRectangle;
            GraphicsPath path = new GraphicsPath();
            rect.Inflate((-1) * (PenWidth / 2), (-1) * (PenWidth / 2));
            path.AddEllipse(rect);

            Brush br = null;
            if (GradientType) br = new LinearGradientBrush(rect, BorderColor, Color.Brown, LinearGradientMode.ForwardDiagonal);
            else br = new SolidBrush(BorderColor);
            if (br != null && BorderVisible)
            {
                Pen pen = new Pen(br, PenWidth);
                pen.DashStyle = m_PenDashStyle;
                g.DrawEllipse(pen, rect);
                br.Dispose();
            }

            g.Dispose();

            base.OnPaint(pea);
        }
        #endregion
    }
}
