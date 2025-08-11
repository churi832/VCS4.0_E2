using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sineva.VHL.Library
{
    public delegate void DrawObjectSelect(object con);

    public class DrawTransparentPanel : Panel
    {
        #region Fields
        private MovingFlag m_FlagMaxPos = null;
        private MovingFlag m_FlagMinPos = null;
        private double m_MaxPosX = 0;
        private double m_MaxPosY = 0;
        private double m_MinPosX = 0;
        private double m_MinPosY = 0;
        private XyPosition m_CurPos = new XyPosition();
        private int m_MyInitX = 0;
        private int m_MyInitY = 0;

        public bool drag = false;
        public bool enab = false;
        private int m_opacity = 100;

        private int alpha;

        public event DrawObjectSelect OnSelectedObject = null;
        #endregion

        #region Property
        [Category("!Setting : Flag"), Description("최대위치에 해당하는 MovingFlag 선택")]
        public MovingFlag FlagMaxPos
        {
            get { return m_FlagMaxPos; }
            set { m_FlagMaxPos = value; }
        }
        [Category("!Setting : Flag"), Description("최소위치에 해당하는 MovingFlag 선택")]
        public MovingFlag FlagMinPos
        {
            get { return m_FlagMinPos; }
            set { m_FlagMinPos = value; }
        }

        [Category("!Setting : Animation"), DisplayName("최대이동좌표(X)")]
        public double MaxPosX { get => m_MaxPosX; set => m_MaxPosX = value; }
        [Category("!Setting : Animation"), DisplayName("최대이동좌표(Y)")]
        public double MaxPosY { get => m_MaxPosY; set => m_MaxPosY = value; }
        [Category("!Setting : Animation"), DisplayName("최소이동좌표(X)")]
        public double MinPosX { get => m_MinPosX; set => m_MinPosX = value; }
        [Category("!Setting : Animation"), DisplayName("최소이동좌표(Y)")]
        public double MinPosY { get => m_MinPosY; set => m_MinPosY = value; }

        public int Opacity
        {
            get
            {
                if (m_opacity > 100)
                {
                    m_opacity = 100;
                }
                else if (m_opacity < 1)
                {
                    m_opacity = 1;
                }
                return this.m_opacity;
            }
            set
            {
                this.m_opacity = value;
                if (this.Parent != null)
                {
                    Parent.Invalidate(this.Bounds, true);
                }
            }
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | 0x20;
                return cp;
            }
        }
        #endregion

        #region Constructor
        public DrawTransparentPanel()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.Opaque | ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
        }

        public bool Initialize()
        {
            if (m_FlagMaxPos != null && m_FlagMinPos != null)
            {
                double diffViewX = m_FlagMaxPos.Location.X - m_FlagMinPos.Location.X;
                double diffViewY = m_FlagMaxPos.Location.Y - m_FlagMinPos.Location.Y;

                if (diffViewX == 0.0 || m_FlagMinPos == null || m_FlagMaxPos == null)
                    m_MyInitX = this.Location.X;
                else
                    m_MyInitX = this.FlagMinPos.Location.X;

                if (diffViewY == 0.0 || m_FlagMinPos == null || m_FlagMaxPos == null)
                    m_MyInitY = this.Location.Y;
                else
                    m_MyInitY = this.FlagMinPos.Location.Y;
            }

            return true;
        }
        #endregion

        #region Methods
        public void SetCurPos(XyPosition pos)
        {
            if (m_FlagMaxPos == null || m_FlagMinPos == null) return;

            try
            {
                m_CurPos = pos;

                int x = 0;
                int y = 0;

                double diffViewX = m_FlagMaxPos.Location.X - m_FlagMinPos.Location.X;
                double diffViewY = m_FlagMaxPos.Location.Y - m_FlagMinPos.Location.Y; //화면과 Real이 역방향

                double diffRealX = (m_MaxPosX - m_MinPosX);
                double diffRealY = (m_MaxPosY - m_MinPosY);

                if (diffRealX != 0.0 && diffViewX != 0.0)
                    x = Convert.ToInt32((diffViewX / diffRealX) * pos.X);
                if (diffRealY != 0.0 && diffViewY != 0.0)
                    y = Convert.ToInt32((diffViewY / diffRealY) * pos.Y);

                x += m_MyInitX;
                y += m_MyInitY;

                Point newP = new Point(x, y);
                this.Location = newP;
            }
            catch
            {

            }
        }
        public XyPosition GetCurPos()
        {
            return m_CurPos;
        }

        public void UpdatePanel(Graphics g)
        {

            // Background를 투명하게 만들자
            Rectangle bounds = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            Color frmColor = this.Parent.BackColor;
            Brush bckColor = default(Brush);

            alpha = (m_opacity * 255) / 100;

            if (drag)
            {
                Color dragBckColor = default(Color);

                if (BackColor != Color.Transparent)
                {
                    int Rb = BackColor.R * alpha / 255 + frmColor.R * (255 - alpha) / 255;
                    int Gb = BackColor.G * alpha / 255 + frmColor.G * (255 - alpha) / 255;
                    int Bb = BackColor.B * alpha / 255 + frmColor.B * (255 - alpha) / 255;
                    dragBckColor = Color.FromArgb(Rb, Gb, Bb);
                }
                else
                {
                    dragBckColor = frmColor;
                }

                alpha = 255;
                bckColor = new SolidBrush(Color.FromArgb(alpha, dragBckColor));
            }
            else
            {
                bckColor = new SolidBrush(Color.FromArgb(alpha, this.BackColor));
            }

            if (this.BackColor != Color.Transparent | drag)
            {
                g.FillRectangle(bckColor, bounds);
            }

            bckColor.Dispose();
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            if (this.Parent != null)
            {
                Parent.Invalidate(this.Bounds, true);
            }
            base.OnBackColorChanged(e);
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnParentBackColorChanged(e);
        }

        protected override void OnClick(EventArgs e)
        {
            if (OnSelectedObject != null) OnSelectedObject.Invoke(this);
        }
        #endregion
    }
}
