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
    public partial class TouchLabel : Label
    {
        private Color m_UserBackColorDefault = Color.OrangeRed;
        private Color m_UserBackColorMouseEnter = Color.YellowGreen;
        private Color m_UserBackColorMouseDown = Color.LightBlue;
        private Color m_UserForeColorDefault = Color.White;
        private Color m_UserForeColorMouseEnter = Color.Blue;
        private Color m_UserForeColorMouseDown = Color.White;

        private bool m_MouseDown = false;
        //private bool m_MouseSwipe = false;

        private bool m_TouchHovering = false;
        private bool m_TouchSwipeLeft = false;
        private bool m_TouchSwipeRight = false;
        private int m_HoveringPixels = 0;

        private Point m_LocationTouch = new Point();
        private Point m_LocationMoving = new Point();

        private PointF m_MouseMovingDrawPixel = new PointF();
        private PointF m_SwipeLengthPerMilisec = new PointF();
        private Point m_TouchHoveringLastPixels = new Point();
        private PointF m_TouchHoveringSpeed = new PointF();
        private UInt32 m_TickCount = 0;

        private Timer m_TimerTicks = null;
        private uint m_UserControlTicks = 0;

        #region Property - UI Design
        [Category("!User Setting"), DisplayName("BackColor Normal")]
        public Color UserBackColorDefault { get { return m_UserBackColorDefault;} set {m_UserBackColorDefault = value; }}
        [Category("!User Setting"), DisplayName("BackColor MouseEnter")]
        public Color UserBackColorMouseEnter { get { return  m_UserBackColorMouseEnter;} set {m_UserBackColorMouseEnter = value;} }
        [Category("!User Setting"), DisplayName("BackColor MouseDown")]
        public Color UserBackColorMouseDown { get { return  m_UserBackColorMouseDown;} set {m_UserBackColorMouseDown = value; }}
        [Category("!User Setting"), DisplayName("ForeColor Normal")]
        public Color UserForeColorDefault { get { return  m_UserForeColorDefault;} set {m_UserForeColorDefault = value; }}
        [Category("!User Setting"), DisplayName("ForeColor MouseEnter")]
        public Color UserForeColorMouseEnter { get { return  m_UserForeColorMouseEnter;} set {m_UserForeColorMouseEnter = value; }}
        [Category("!User Setting"), DisplayName("ForeColor MouseDown")]
        public Color UserForeColorMouseDown { get { return m_UserForeColorMouseDown;} set {m_UserForeColorMouseDown = value; }}
        #endregion

        #region Property
        //public bool IsMouseSwipe { get { bool rv = m_MouseSwipe; m_MouseSwipe = false; return rv; } }
        public PointF MouseMovingDrawPixel { get { PointF rv = new PointF(m_MouseMovingDrawPixel.X, m_MouseMovingDrawPixel.Y); m_MouseMovingDrawPixel.X = 0; m_MouseMovingDrawPixel.Y = 0; return rv; } }
        public PointF SwipeLengthPerMilisec { get { PointF rv = new PointF(m_SwipeLengthPerMilisec.X, m_SwipeLengthPerMilisec.Y); m_SwipeLengthPerMilisec = new PointF(); return rv; } }
        public Point TouchHoveringLastPixels { get { return  m_TouchHoveringLastPixels; }}//{ get { Point rv = new Point(m_TouchHoveringLastPixels.X, m_TouchHoveringLastPixels.Y); m_TouchHoveringLastPixels = new Point(); return rv; } }

        public bool IsTouchHovering { get { return  m_TouchHovering; }}
        public bool IsTouchSwipeLeft { get { bool rv = m_TouchSwipeLeft; m_TouchSwipeLeft = false; return rv; } }
        public bool IsTouchSwipeRight { get { bool rv = m_TouchSwipeRight; m_TouchSwipeRight = false; return rv; } }
        public int HoveringPixels { get { return m_HoveringPixels; } }
        #endregion

        public TouchLabel()
        {
            InitializeComponent();
            this.AutoSize = false;
            this.TextAlign = ContentAlignment.MiddleCenter;

            this.MouseDown += TouchLabel_MouseDown;
            this.MouseUp += TouchLabel_MouseUp;
            this.MouseMove += TouchLabel_MouseMove;
            this.MouseDoubleClick += TouchLabel_MouseDoubleClick;
            this.MouseEnter += TouchLabel_MouseEnter;
            this.MouseLeave += TouchLabel_MouseLeave;

            m_TimerTicks = new Timer();
            m_TimerTicks.Interval = 10;
            m_TimerTicks.Tick += TimerTicks_Tick;
            m_TimerTicks.Start();

            if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
            }
        }

        private void TimerTicks_Tick(object sender, EventArgs e)
        {
            m_UserControlTicks += (uint)m_TimerTicks.Interval;
        }

        private void TouchLabel_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = UserBackColorDefault;
            this.ForeColor = UserForeColorDefault;
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
        }

        private void TouchLabel_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = m_UserBackColorMouseEnter;
            this.ForeColor = m_UserForeColorMouseEnter;
        }

        private void TouchLabel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            bool areaLeft = e.X < this.Width * 0.3;
            bool areaRight = e.X > this.Width * 0.7;
            bool areaTop = e.Y < this.Height * 0.3;
            bool areaBottom = e.Y > this.Height * 0.7;

            m_TouchSwipeLeft = areaLeft;
            m_TouchSwipeRight = areaRight;
        }

        private void TouchLabel_MouseMove(object sender, MouseEventArgs e)
        {
            if(m_MouseDown == false) return;

            if(m_TouchHovering == false)
            {
                //m_TickCount = XFunc.GetTickCount();
                m_TickCount = m_UserControlTicks;
                m_LocationMoving = new Point(e.X, e.Y);
                m_TouchHoveringLastPixels = new Point(m_LocationMoving.X - m_LocationTouch.X, m_LocationMoving.Y - m_LocationTouch.Y);
                m_TouchHovering = true;
                return;
            }

            //uint curTick = XFunc.GetTickCount();
            uint curTick = m_UserControlTicks;
            uint time = curTick - m_TickCount;
            Point newMovePixels = new Point(e.X - m_LocationMoving.X, e.Y - m_LocationMoving.Y);
            m_LocationMoving.X = e.X;
            m_LocationMoving.Y = e.Y;

            m_TouchHoveringSpeed.X = newMovePixels.X / (float)time;
            m_TouchHoveringSpeed.Y = newMovePixels.Y / (float)time;
            m_TouchHoveringLastPixels.X += newMovePixels.X;
            m_TouchHoveringLastPixels.Y += newMovePixels.Y;
            m_TickCount = curTick;
        }

        private void TouchLabel_MouseUp(object sender, MouseEventArgs e)
        {
            bool mouseLeaved = false;
            mouseLeaved |= e.X < 0 || e.X > this.Width;
            mouseLeaved |= e.Y < 0 || e.Y > this.Height;
            if(mouseLeaved)
            {
                this.BackColor = m_UserBackColorDefault;
                this.ForeColor = m_UserForeColorDefault;
            }
            else
            {
                this.BackColor = m_UserBackColorMouseEnter;
                this.ForeColor = m_UserForeColorMouseEnter;
            }
            
            if(m_MouseDown == false) return;

            PointF lengthRatio = new PointF();
            lengthRatio.X = (float)(m_LocationMoving.X - m_LocationTouch.X) / this.Width;
            //lengthRatio.Y = (m_LocationMoving.Y - m_LocationTouch.Y) / this.Height;

            bool swipe = false;
            swipe |= Math.Abs(lengthRatio.X) > 0.5;                 // 한쪽방향으로 Label 크기의 40%이상 움직였을 경우
            //swipe |= XFunc.GetTickCount() - m_TickCount < 200;      // MouseMove(TouchHovering) 이후, 200ms 이내에 Mouse를 놓았을 경우
            swipe |= m_UserControlTicks - m_TickCount < 200;
            if(swipe)
            {
                m_TouchSwipeLeft = m_LocationMoving.X < m_LocationTouch.X;
                m_TouchSwipeRight = m_LocationMoving.X > m_LocationTouch.X;
            }

            m_MouseDown = false;
            m_TouchHovering = false;
            //m_MouseSwipe = swipe;

            this.Cursor = System.Windows.Forms.Cursors.Hand;
        }

        private void TouchLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                m_MouseDown = true;
                m_LocationTouch = new Point(e.X, e.Y);

                this.BackColor = UserBackColorMouseDown;
                this.ForeColor = UserForeColorMouseDown;
            }
        }
    }
}
