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
    public partial class SwipePanel : SplitContainer
    {
        #region Field
        private readonly int MAX_PAGE_NO = 2;
        private bool m_SwipeEnabled = false;
        private bool m_SwipeActive = false;
        private bool m_SwipeEnablePrevent = false;

        private int m_CurPage = 1;
        private Timer m_TimerSwipeMotion = null;
        private Point m_SwipeStartPoint = new Point();

        private bool m_SwipeToPrev = false;
        private bool m_SwipeToNext = false;
        private int m_SwipeMotionTick = 100;
        #endregion

        #region Property
        public new Orientation Orientation
        {
            get { return base.Orientation; }
            set
            {
                base.Orientation = value;
                if(base.Orientation == System.Windows.Forms.Orientation.Vertical)
                    base.SplitterDistance = base.Width - base.SplitterWidth;
                else
                    base.SplitterDistance = base.Height - base.SplitterWidth;
                m_CurPage = 1;
            }
        }
        #endregion

        #region Constructor
        public SwipePanel()
        {
            InitializeComponent();
            InitControl();

            this.DoubleBuffered = true;
        }
        #endregion

        #region Method
        private void InitControl()
        {
            this.Dock = DockStyle.None;
            this.SplitterWidth = 14;
            this.Panel1MinSize = this.Panel2MinSize = 0;
            this.BackColor = Color.DimGray;
            this.Panel1.BackColor = this.Panel2.BackColor = Color.White;
            this.IsSplitterFixed = true;
            if(this.Orientation == System.Windows.Forms.Orientation.Vertical)
                this.SplitterDistance = this.Width - this.SplitterWidth;
            else
                this.SplitterDistance = this.Height - this.SplitterWidth;

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SwipePanel_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SwipePanel_MouseUp);
            this.MouseHover += new System.EventHandler(this.SwipePanel_MouseHover);
            this.MouseLeave += new System.EventHandler(this.SwipePanel_MouseLeave);
            this.MouseDoubleClick += new MouseEventHandler(this.SwipePanel_MouseDoubleClick);
            this.SizeChanged += new System.EventHandler(this.SwipePanel_SizeChanged);

            m_TimerSwipeMotion = new Timer();
            m_TimerSwipeMotion.Tick += new System.EventHandler(this.TimerSwipeMotion_Tick);
            m_TimerSwipeMotion.Interval = 10;

            m_CurPage = 1;
        }

        public void PageChangeToNext()
        {
            if(this.Orientation == System.Windows.Forms.Orientation.Vertical)
                this.SplitterDistance = this.Width - this.SplitterWidth;
            else
                this.SplitterDistance = this.Height - this.SplitterWidth;

            if(++m_CurPage > MAX_PAGE_NO) m_CurPage = MAX_PAGE_NO;
        }
        public void PageChangeToPrev()
        {
            this.SplitterDistance = 0;

            if(--m_CurPage < 1) m_CurPage = 1;
        }
        #endregion

        #region Event Handler
        private void TimerSwipeMotion_Tick(object sender, EventArgs e)
        {
            if(m_SwipeToPrev || m_SwipeToNext)
            {
                int sign = m_SwipeToPrev ? 1 : -1;

                //int splitterDistance = 0;
                //if(this.Orientation == System.Windows.Forms.Orientation.Vertical)
                //    splitterDistance = this.SplitterDistance + this.Width % 100 * sign;//this.Width / 5 * sign + this.Width % 5 * sign;
                //else
                //    splitterDistance = this.SplitterDistance + this.Height % 100 * sign;//this.Height / 10 * sign + this.Height % 5 * sign;
                //if(splitterDistance <= 0) splitterDistance = 0;

                int splitterDistance = 0;
                if(this.Orientation == System.Windows.Forms.Orientation.Vertical)
                    splitterDistance = this.SplitterDistance + m_SwipeMotionTick * sign;//this.Width / 5 * sign + this.Width % 5 * sign;
                else
                    splitterDistance = this.SplitterDistance + m_SwipeMotionTick * sign;//this.Height / 10 * sign + this.Height % 5 * sign;
                if(splitterDistance <= 0) splitterDistance = 0;

                this.SplitterDistance = splitterDistance;

                int remains = 0;
                if(this.Orientation == System.Windows.Forms.Orientation.Vertical)
                    remains = m_SwipeToNext ? this.SplitterDistance : this.Width - this.SplitterDistance - this.SplitterWidth;
                else
                    remains = m_SwipeToNext ? this.SplitterDistance : this.Height - this.SplitterDistance - this.SplitterWidth;
                if(remains == 0)
                {
                    m_SwipeToNext = m_SwipeToPrev = false;
                    m_TimerSwipeMotion.Stop();
                }
            }
        }
        private void SwipePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if(m_SwipeEnabled == false) return;
            this.BackColor = Color.WhiteSmoke;

            m_SwipeStartPoint.X = e.X;
            m_SwipeStartPoint.Y = e.Y;
            m_SwipeActive = true;
        }
        private void SwipePanel_MouseUp(object sender, MouseEventArgs e)
        {
            if(m_SwipeActive)
            {
                m_SwipeActive = false;

                bool swipeCancel = false;
                if(this.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    swipeCancel = e.Y > this.Location.Y + this.Height || e.Y < 0;
                }
                else
                {
                    swipeCancel = e.X > this.Location.X + this.Width || e.X < this.Location.X;
                }
                if(swipeCancel) return;


                bool motionToNext = false, motionToPrev = false;
                if(this.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    motionToNext = this.SplitterDistance > this.Width / 2 && (e.X - m_SwipeStartPoint.X < this.Width / 10 * -1);
                    motionToPrev = this.SplitterDistance <= this.Width / 2 && (e.X - m_SwipeStartPoint.X > this.Width / 10);

                    int swipeDist = Math.Abs(e.X - m_SwipeStartPoint.X);
                    m_SwipeMotionTick = (int)((double)swipeDist * ((double)swipeDist / (double)this.Width)) / 2;
                    if(m_SwipeMotionTick < this.Width / 20) m_SwipeMotionTick = this.Width / 20;
                }
                else
                {
                    motionToNext = this.SplitterDistance > this.Height / 2 && (e.Y - m_SwipeStartPoint.Y < this.Height / 10 * -1);
                    motionToPrev = this.SplitterDistance <= this.Height / 2 && (e.Y - m_SwipeStartPoint.Y > this.Height / 10);

                    int swipeDist = Math.Abs(e.Y - m_SwipeStartPoint.Y);
                    m_SwipeMotionTick = (int)((double)swipeDist * ((double)swipeDist / (double)this.Height)) / 2;
                    if(m_SwipeMotionTick < this.Height / 20) m_SwipeMotionTick = this.Height / 20;
                }

                if(motionToNext)
                {
                    //this.SplitterDistance = 0;
                    m_SwipeToPrev = false;
                    m_SwipeToNext = true;
                    m_CurPage++;
                    m_TimerSwipeMotion.Start();
                }
                else if(motionToPrev)
                {
                    //this.SplitterDistance = this.Width - this.SplitterWidth;
                    m_SwipeToNext = false;
                    m_SwipeToPrev = true;
                    m_CurPage--;
                    m_TimerSwipeMotion.Start();
                }


                bool mouseHoverIng = true;
                if(this.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    //mouseHoverIng &= e.X >= this.Location.X + this.SplitterDistance && e.X <= this.Location.X + this.SplitterDistance + this.SplitterWidth;
                    //mouseHoverIng &= e.Y >= this.Location.Y && e.Y <= this.Location.Y + this.Height;
                    mouseHoverIng &= e.X >= this.SplitterDistance && e.X <= this.SplitterDistance + this.SplitterWidth;
                    mouseHoverIng &= e.Y >= 0 && e.Y <= this.Height;
                }
                else
                {
                    mouseHoverIng &= e.Y >= this.SplitterDistance && e.Y <= this.SplitterDistance + this.SplitterWidth;
                    mouseHoverIng &= e.X >= 0 && e.X <= this.Width;
                }

                if(mouseHoverIng)
                    this.BackColor = Color.LightGray;
                else
                    this.BackColor = Color.DimGray;
            }
        }
        private void SwipePanel_MouseHover(object sender, EventArgs e)
        {
            this.BackColor = Color.LightGray;
            m_SwipeEnabled = true;
        }
        private void SwipePanel_MouseLeave(object sender, EventArgs e)
        {
            m_SwipeEnabled = false;
            if(m_SwipeActive == false) this.BackColor = Color.DimGray;
        }
        private void SwipePanel_SizeChanged(object sender, EventArgs e)
        {
            if(this.Orientation == System.Windows.Forms.Orientation.Vertical)
                this.SplitterDistance = this.Width - this.SplitterWidth;
            else
                this.SplitterDistance = this.Height - this.SplitterWidth;
            m_CurPage = 1;
        }
        private void SwipePanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(m_CurPage == MAX_PAGE_NO) PageChangeToPrev();
            else PageChangeToNext();
        }
        #endregion
    }
}
