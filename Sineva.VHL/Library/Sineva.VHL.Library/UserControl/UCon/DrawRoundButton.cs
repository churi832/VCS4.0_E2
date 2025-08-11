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
    public class DrawRoundButton : Button
    {
        #region Fields
        private Color m_MouseDownColor = Color.Red;
        private Color m_MouseUpColor = Color.White;
        private Color m_SurroundColor = Color.Pink;
        private Color m_BorderColor = Color.Black;
        #endregion

        #region Property
        public Color MouseDownColor { get { return m_MouseDownColor; } set { m_MouseDownColor = value; this.Invalidate(false); } }
        public Color MouseUpColor { get { return m_MouseUpColor; } set { m_MouseUpColor = value; this.Invalidate(false); } }
        public Color SurroundColor { get { return m_SurroundColor; } set { m_SurroundColor = value; this.Invalidate(false); } }
        public Color BorderColor { get { return m_BorderColor; } set { m_BorderColor = value; this.Invalidate(false); } }
        #endregion

        #region Constructor
        public DrawRoundButton()
        {
        }
        #endregion

        #region Methods
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(this.ClientRectangle);

            this.Region = new Region(path);
        }

        protected override void OnPaint(PaintEventArgs pea)
        {
            // 마우스 클릭 확인 루틴
            bool bPress = this.Capture & ((MouseButtons & MouseButtons.Left) != 0)
                          & this.ClientRectangle.Contains(PointToClient(MousePosition));
            int x = bPress ? 2 : 1;

            Graphics grfx = pea.Graphics;
            Rectangle rect = this.ClientRectangle;
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(rect);
            PathGradientBrush pgbr = new PathGradientBrush(path);
            pgbr.CenterPoint = new PointF(x * (rect.Left + rect.Right) / 3,
                                          x * (rect.Top + rect.Bottom) / 3);
            pgbr.CenterColor = bPress ? m_MouseDownColor : m_MouseUpColor;
            pgbr.SurroundColors = new Color[] { m_SurroundColor };
            grfx.FillRectangle(pgbr, rect);

            Brush br = new LinearGradientBrush(rect, BorderColor, Color.Brown,
                           LinearGradientMode.ForwardDiagonal);
            Pen pen = new Pen(br, 2);
            grfx.DrawEllipse(pen, rect);

            StringFormat strfmt = new StringFormat();
            strfmt.Alignment = strfmt.LineAlignment = StringAlignment.Center;

            br = Enabled ? SystemBrushes.WindowText : SystemBrushes.GrayText;
            grfx.DrawString(this.Text, this.Font, br, rect, strfmt);

            if (this.Focused)
            {
                pen = new Pen(BorderColor);
                pen.DashStyle = DashStyle.Dash;
                grfx.DrawEllipse(pen, rect.X + 5, rect.Y + 5, rect.Width - 10, rect.Height - 10);
            }
        }
        #endregion
    }
}
