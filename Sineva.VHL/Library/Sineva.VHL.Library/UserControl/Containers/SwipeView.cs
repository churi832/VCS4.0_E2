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
    public delegate void SwipeViewChanged(int a);
    public partial class SwipeView : UserControl
    {
        #region Field
        private string m_PageName = "SWIPE PAGE VIEW";
        private int m_PageNo = 0;
        private int m_SwipeMotionDelay = 1000;      // 1000ms (Swipe Animation을 1초간 진행해라)
        private int m_SwipeMotionFrame = 0;
        private int m_CurMotionFrame = 0;
        private bool m_TouchHover = false;
        private bool m_TouchSwipe = false;
        private Point m_PrevLocation = new Point();
        private Point m_TouchLocationHover = new Point();
        private Point m_TouchLocationSwipe = new Point();

        private Timer m_TmrPageMove = null;
        #endregion

        #region Events
        //public event DelVoid_Int ChangedPage = null;
        public event SwipeViewChanged ChangedPage = null;
        #endregion

        #region Property
        public int PageNo { get {return m_PageNo;} set {m_PageNo = value;} }
        public string PageName { get { return m_PageName; } set { m_PageName = value; } }
        #endregion

        public SwipeView()
        {
            InitializeComponent();
            
            if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                m_TmrPageMove = new Timer();
                m_TmrPageMove.Tick += tmrPageMove_Tick;
                m_TmrPageMove.Interval = 10;
                m_TmrPageMove.Start();
            }
        }

        private void tmrPageMove_Tick(object sender, EventArgs e)
        {
            if(m_TouchSwipe)
            {
                int tickMoveOffsetX = (m_TouchLocationSwipe.X - m_PrevLocation.X) / m_SwipeMotionFrame;
                int tickMoveOffsetY = m_TouchLocationSwipe.Y - m_PrevLocation.Y / m_SwipeMotionFrame;

                if(++m_CurMotionFrame >= m_SwipeMotionFrame)
                {
                    this.Location = m_TouchLocationSwipe;
                    if (ChangedPage != null) ChangedPage.Invoke(m_PageNo);
                    m_TouchSwipe = false;
                }
                else
                {
                    this.Location = new Point(m_PrevLocation.X + (tickMoveOffsetX * m_CurMotionFrame), m_PrevLocation.Y + (tickMoveOffsetY * m_CurMotionFrame));
                }
            }
            else if(m_TouchHover)
            {
                this.Location = m_TouchLocationHover;
                m_PrevLocation = this.Location;
            }
        }

        public void SetPageSwipe(Point locationSwipe, int delay)
        {
            m_TouchHover = false;

            m_SwipeMotionDelay = delay;
            m_SwipeMotionFrame = delay / m_TmrPageMove.Interval;
            m_CurMotionFrame = 0;
            m_TouchLocationSwipe = locationSwipe;

            m_PrevLocation = this.Location;
            m_TouchSwipe = true;
        }

        public void SetPageHover(Point locationHover)
        {
            m_TouchLocationHover = locationHover;
            m_TouchHover = true;
        }
    }
}
