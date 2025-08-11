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
    public partial class MovingObject : Panel
    {
        #region Fields
        private bool m_Initialized = false;
        private MovingFlag m_FlagMaxPos = null;
        private MovingFlag m_FlagMinPos = null;
        private double m_MaxPosX = 0;
        private double m_MaxPosY = 0;
        private double m_MinPosX = 0;
        private double m_MinPosY = 0;
        private double m_CurPosX = 0;
        private double m_CurPosY = 0;

        private Timer m_Timer;
        private Point m_MaxPosObjectPoint = new Point();
        private Point m_MinPosObjectPoint = new Point();

        private int m_MyInitX = 0;
        private int m_MyInitY = 0;

        private double m_SimulVal = 0.0;
        private bool m_Simul = false;

        private int m_UpdateInterval = 100;
        #endregion

        #region Properties
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
        [Category("!Setting : Action"), Description("UI Update Interval")]
        public int UpdateInterval
        {
            get { return m_UpdateInterval; }
            set { m_UpdateInterval = value; }
        }

        [Category("!Setting : Animation"), DisplayName("최대이동좌표(X)")]
        public double MaxPosX { get => m_MaxPosX; set => m_MaxPosX = value; }
        [Category("!Setting : Animation"), DisplayName("최대이동좌표(Y)")]
        public double MaxPosY { get => m_MaxPosY; set => m_MaxPosY = value; }
        [Category("!Setting : Animation"), DisplayName("최소이동좌표(X)")]
        public double MinPosX { get => m_MinPosX; set => m_MinPosX = value; }
        [Category("!Setting : Animation"), DisplayName("최소이동좌표(Y)")]
        public double MinPosY { get => m_MinPosY; set => m_MinPosY = value; }
        [Browsable(false)]
        public double CurPosX { get => m_CurPosX; set => m_CurPosX = value; }
        [Browsable(false)]
        public double CurPosY { get => m_CurPosY; set => m_CurPosY = value; }

        //[Category("!Setting : Animation"), DisplayName("최대이동거리"), Description("최대위치의 Raw 값")]
        //public PointF MaxPos { get => m_MaxPos; set => m_MaxPos = value; }
        //[Category("!Setting : Animation"), DisplayName("최대이동거리 (X)"), Description("최대위치의 Raw 값 X")]
        //public float MaxPosX { get => m_MaxPos.X; set => m_MaxPos.X = value; }
        //[Category("!Setting : Animation"), DisplayName("최대이동거리 (Y)"), Description("최대위치의 Raw 값 Y")]
        //public float MaxPosY { get => m_MaxPos.Y; set => m_MaxPos.Y = value; }
        //[Category("!Setting : Animation"), DisplayName("최소이동거리"), Description("최소위치의 Raw 값")]
        //public PointF MinPos { get => m_MinPos; set => m_MinPos = value; }
        //[Category("!Setting : Animation"), DisplayName("최소이동거리 (X)"), Description("최소위치의 Raw 값 X")]
        //public float MinPosX { get => m_MinPos.X; set => m_MinPos.X = value; }
        //[Category("!Setting : Animation"), DisplayName("최소이동거리 (Y)"), Description("최소위치의 Raw 값 Y")]
        //public float MinPosY { get => m_MinPos.Y; set => m_MinPos.Y = value; }
        //[Browsable(false)]
        //public float CurPosX { get => m_CurPos.X; set => m_CurPos = new PointF(value, CurPos.Y); }
        //[Browsable(false)]
        //public float CurPosY { get => m_CurPos.Y; set => m_CurPos = new PointF(CurPos.X, value); }
        //[Browsable(false)]
        //public PointF CurPos { get => m_CurPos; set => m_CurPos = value; }

        #endregion

        public MovingObject()
		{
            InitializeComponent();
            this.DoubleBuffered = true;
		}

		public bool Initialize()
		{
            try
            {
                if(m_Initialized == false)
                {
                    //if(CurPos == null) CurPos = new PointF();
                    //var rv = false;

                    m_MaxPosObjectPoint = m_FlagMaxPos.Location;
                    m_MinPosObjectPoint = m_FlagMinPos.Location;

                    double diffViewX = m_MaxPosObjectPoint.X - m_MinPosObjectPoint.X;
                    double diffViewY = m_MaxPosObjectPoint.Y - m_MinPosObjectPoint.Y;

                    if(diffViewX == 0.0 || m_FlagMinPos == null || m_FlagMaxPos == null)
                        m_MyInitX = this.Location.X;
                    else
                        m_MyInitX = this.m_FlagMinPos.Location.X;

                    if(diffViewY == 0.0 || m_FlagMinPos == null || m_FlagMaxPos == null)
                        m_MyInitY = this.Location.Y;
                    else
                        m_MyInitY = this.m_FlagMinPos.Location.Y;

                    m_Timer = new Timer();
                    m_Timer.Interval = m_UpdateInterval;
                    m_Timer.Tick += new EventHandler(tmrUpdateState_Tick);
                    m_Timer.Start();

                    m_Initialized = true;
                }
            }
            catch
            {
                return false;
            }
            return m_Initialized;
        }

        public void DoSimul()
		{
			m_Simul = true;
		}


		private void tmrUpdateState_Tick(object sender, EventArgs e)
		{
            DoAnimation();
		}
        protected virtual void DoAnimation()
        {
            if(m_Initialized == false) return;

            if(m_Simul)
            {
                SimulAging();
                m_Simul = false;
            }

            try
            {
                if(m_FlagMaxPos == null || m_FlagMinPos == null) return;

                int x = 0;
                int y = 0;

                double diffViewX = m_MaxPosObjectPoint.X - m_MinPosObjectPoint.X;
                double diffViewY = m_MaxPosObjectPoint.Y - m_MinPosObjectPoint.Y;

                double diffRealX = m_MaxPosX - m_MinPosX;
                double diffRealY = m_MaxPosY - m_MinPosY;

                //double curPosX = Math.Round(CurPos.X, 1);
                //double curPosY = Math.Round(CurPos.Y, 1);

                //if(m_MinPosX < 0) m_CurPosX += m_MaxPosX;
                //if(m_MinPosY < 0) m_CurPosY += m_MaxPosY;

                if(diffRealX != 0.0 && diffViewX != 0.0)
                    x = Convert.ToInt32((diffViewX / diffRealX) * (m_CurPosX - m_MinPosX));
                if(diffRealY != 0.0 && diffViewY != 0.0)
                    y = Convert.ToInt32((diffViewY / diffRealY) * (m_CurPosY - m_MinPosY));

                Point newP = new Point(m_MyInitX + x, m_MyInitY + y);
                this.Location = newP;
            }
            catch
            {
            }
        }

        private void SimulAging()
		{
            //if (MaxPos == null || MinPos == null) return;
			double diffRealX = m_MaxPosX - m_MinPosX;
			double diffRealY = m_MaxPosY - m_MinPosY;

            m_CurPosX = diffRealX / 2 * Math.Sin(m_SimulVal) + diffRealX / 2;
            m_CurPosY = diffRealY / 2 * Math.Sin(m_SimulVal) + diffRealY / 2;
            
			m_SimulVal += 0.1;
		}
	}
}
