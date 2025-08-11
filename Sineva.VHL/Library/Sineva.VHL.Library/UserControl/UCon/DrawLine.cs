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
    public class DrawLine : DrawTransparentPanel
    {
        #region Enum
        public enum DrawingType
        {
            Horizontal,
            Vertical,
            Cross,
            LeftDiagonal,
            RightDiagonal,
            CrossDiagonal,
        }
        #endregion

        #region Fields
        private DrawingType m_LineType = DrawingType.Horizontal;
        private LineCap m_LineStartCap = LineCap.Flat;
        private LineCap m_LineEndCap = LineCap.Flat;
        private bool m_GradientType = true;
        private DashStyle m_PenDashStyle = DashStyle.Dash;
        private int m_PenWidth = 5;
        private Color m_LineColor = Color.Black;
        #endregion

        #region Property
        public DrawingType LineType { get { return m_LineType; } set { m_LineType = value; this.Invalidate(false); } }
        public bool GradientType { get { return m_GradientType; } set { m_GradientType = value; this.Invalidate(false); } }
        public Color LineColor { get { return m_LineColor; } set { m_LineColor = value; this.Invalidate(false); } }
        public DashStyle PenDashStyle { get { return m_PenDashStyle; } set { m_PenDashStyle = value; this.Invalidate(false); } }
        public int PenWidth { get { return m_PenWidth; } set { m_PenWidth = value; this.Invalidate(false); } }
        public LineCap LineStartCap { get { return m_LineStartCap; } set { m_LineStartCap = value; this.Invalidate(false); } }
        public LineCap LineEndCap { get { return m_LineEndCap; } set { m_LineEndCap = value; this.Invalidate(false); } }
        #endregion

        #region Constructor
        public DrawLine()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }
        #endregion

        #region Methods
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(this.ClientRectangle);

            this.Region = new Region(path);
        }

        protected override void OnPaint(PaintEventArgs pea)
        {
            // 마우스 클릭 확인 루틴
            bool bPress = this.Capture & ((MouseButtons & MouseButtons.Left) != 0)
                          & this.ClientRectangle.Contains(PointToClient(MousePosition));
            int x = bPress ? 2 : 1;

            Graphics g = pea.Graphics;
            UpdatePanel(g);

            // Line을 그리자
            Rectangle rect = this.ClientRectangle;
            Brush br = null;
            if (GradientType) br = new LinearGradientBrush(rect, LineColor, Color.Brown, LinearGradientMode.ForwardDiagonal);
            else br = new SolidBrush(LineColor);
            if (br != null)
            {
                Pen pen = new Pen(br, PenWidth);
                pen.DashStyle = m_PenDashStyle;
                pen.StartCap = m_LineStartCap;
                pen.EndCap = m_LineEndCap;
                int start_margin = 0;
                int end_margin = 0;
                if (m_LineStartCap == LineCap.Flat || m_LineStartCap == LineCap.NoAnchor || m_LineStartCap == LineCap.AnchorMask || m_LineStartCap == LineCap.Custom || m_LineStartCap == LineCap.ArrowAnchor) start_margin = 0;
                else if (m_LineStartCap == LineCap.Square || m_LineStartCap == LineCap.Round || m_LineStartCap == LineCap.Triangle) start_margin = PenWidth / 2;
                else if (m_LineStartCap == LineCap.SquareAnchor) start_margin = PenWidth - 4;
                else if (m_LineStartCap == LineCap.RoundAnchor || m_LineStartCap == LineCap.DiamondAnchor) start_margin = PenWidth + 1;
                if (m_LineEndCap == LineCap.Flat || m_LineEndCap == LineCap.NoAnchor || m_LineEndCap == LineCap.AnchorMask || m_LineEndCap == LineCap.Custom || m_LineEndCap == LineCap.ArrowAnchor) end_margin = 0;
                else if (m_LineEndCap == LineCap.Square || m_LineEndCap == LineCap.Round || m_LineEndCap == LineCap.Triangle) end_margin = PenWidth / 2;
                else if (m_LineEndCap == LineCap.SquareAnchor) end_margin = PenWidth - 4;
                else if (m_LineEndCap == LineCap.RoundAnchor || m_LineEndCap == LineCap.DiamondAnchor) end_margin = PenWidth + 1;

                if (LineType == DrawingType.Horizontal)
                {
                    g.DrawLine(pen, rect.X + start_margin, rect.Y + rect.Height / 2, rect.X + Width - end_margin, rect.Y + rect.Height / 2);
                    rect.Height = PenWidth;
                }
                else if (LineType == DrawingType.Vertical)
                {
                    g.DrawLine(pen, rect.X + rect.Width / 2, rect.Y + start_margin, rect.X + rect.Width / 2, rect.Y + rect.Height - end_margin);
                    rect.Width = PenWidth;
                }
                else if (LineType == DrawingType.Cross)
                {
                    g.DrawLine(pen, rect.X + start_margin, rect.Y + rect.Height / 2, rect.X + Width - end_margin, rect.Y + rect.Height / 2);
                    g.DrawLine(pen, rect.X + rect.Width / 2, rect.Y + start_margin, rect.X + rect.Width / 2, rect.Y + rect.Height - end_margin);
                    rect.Width = PenWidth;
                }
                else if (LineType == DrawingType.LeftDiagonal)
                {
                    g.DrawLine(pen, rect.X + start_margin, rect.Y + start_margin, rect.X + rect.Width - end_margin, rect.Y + rect.Height - end_margin);
                    rect.Width = PenWidth;
                }
                else if (LineType == DrawingType.RightDiagonal)
                {
                    g.DrawLine(pen, rect.X + rect.Width - start_margin, rect.Y + start_margin, rect.X + end_margin, rect.Y + rect.Height - end_margin);
                    rect.Width = PenWidth;
                }
                else if (LineType == DrawingType.CrossDiagonal)
                {
                    g.DrawLine(pen, rect.X + start_margin, rect.Y + start_margin, rect.X + rect.Width - end_margin, rect.Y + rect.Height - end_margin);
                    g.DrawLine(pen, rect.X + rect.Width - start_margin, rect.Y + start_margin, rect.X + end_margin, rect.Y + rect.Height - end_margin);
                    rect.Width = PenWidth;
                }

                br.Dispose();
            }

            g.Dispose();

            base.OnPaint(pea);
        }

        #endregion
    }
}
