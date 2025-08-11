/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.17 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Sineva.VHL.Library
{
    public class XTimer
    {
        private struct TagTimer
        {
            public bool Over;
            public bool Start;
            public bool Pause;

            public UInt32 StartedTickCounts;
            public UInt32 ElapsedTickCounts;
            public UInt32 TargetTickCounts;
        }

        #region Fields
        private readonly static object m_LockKey = new object();
        private const int _TimerResolution = 20;
        private int m_Id;
        private string m_Name;
        private TagTimer m_Flag;
        private static int m_Count;
        private static System.Timers.Timer m_SystemTimer = null;
        private static List<XTimer> m_List = new List<XTimer>();
        #endregion

        #region Properties
        public bool Over
        {
            get
            {
                if (true != m_Flag.Over) return false;
                else
                {
                    m_Flag.Over = false;
                    return true;
                }
            }
        }

        public bool IsPaused
        {
            get { return m_Flag.Pause; }
        }

        public UInt32 CurElapsedTickCounts
        {
            get { return m_Flag.ElapsedTickCounts; }
        }

        public UInt32 TargetTickCounts
        {
            get { return m_Flag.TargetTickCounts; }
        }
        #endregion

        #region Constructor
        public XTimer(string name)
        {
            m_Name = name;
            m_Id = m_Count++;

            if (null == m_SystemTimer)
            {
                m_SystemTimer = new System.Timers.Timer();
                m_SystemTimer.Interval = _TimerResolution;
                m_SystemTimer.Elapsed += new ElapsedEventHandler(this.TimerTick);
            }

            m_List.Add(this);
        }

        public XTimer(string name, int milliseconds)
            : this(name)
        {
            m_Flag.TargetTickCounts = (UInt32)(milliseconds - (UInt32)(_TimerResolution * 0.5));
        }

        ~XTimer()
        {
            if (null != m_SystemTimer)
            {
                m_SystemTimer.Enabled = false;
            }
        }
        #endregion

        #region Methods
        private void TimerTick(object source, ElapsedEventArgs e)
        {
            lock (m_LockKey)
            {
                //DateTime time = DateTime.Now;
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                try
                {
                    UInt32 TickCount = XFunc.GetTickCount();

                    foreach (XTimer timer in m_List)
                    {
                        if (true == timer.m_Flag.Start)
                        {
                            if (false == timer.m_Flag.Pause)
                            {
                                //TimeSpan diff = time - timer.m_Flag.StartedTime;
                                //timer.m_Flag.ElapsedTicks = diff.TotalMilliseconds;
                                timer.m_Flag.ElapsedTickCounts = TickCount - timer.m_Flag.StartedTickCounts;
                            }

                            //if (timer.m_Flag.ElapsedTicks >= timer.m_Flag.TargetTicks)
                            //{
                            //    timer.m_Flag.Start = false;
                            //    timer.m_Flag.Over = true;
                            //}

                            if (timer.m_Flag.ElapsedTickCounts >= timer.m_Flag.TargetTickCounts)
                            {
                                timer.m_Flag.Start = false;
                                timer.m_Flag.Over = true;
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    ExceptionLog.WriteLog(method, String.Format(err.ToString()));
                }
            }
        }

        public void Start()
        {
            lock (m_LockKey)
            {
                m_Flag.Over = false;
                m_Flag.ElapsedTickCounts = 0;
                m_Flag.StartedTickCounts = XFunc.GetTickCount();

                m_Flag.Pause = false;
                m_Flag.Start = true;
            }

            if (false == m_SystemTimer.Enabled)
            {
                m_SystemTimer.Start();
            }
        }

        public void Start(int milliseconds)
        {
            lock (m_LockKey)
            {
                m_Flag.Over = false;

                m_Flag.ElapsedTickCounts = 0;
                int tagetTicks = (int)(milliseconds - (int)(_TimerResolution * 0.5));
                tagetTicks = tagetTicks < 0 ? 0 : tagetTicks;
                m_Flag.TargetTickCounts = (uint)tagetTicks;
                m_Flag.StartedTickCounts = XFunc.GetTickCount();

                m_Flag.Pause = false;
                m_Flag.Start = true;
            }

            if (false == m_SystemTimer.Enabled)
            {
                m_SystemTimer.Start();
            }
        }

        public void Start(bool startCond, bool pausedCond, int milliseconds)
        {
            if (startCond && !m_Flag.Start && !m_Flag.Over)
            {
                Start(milliseconds);
            }
            else if (!startCond)
            {
                m_Flag.Start = false;
                m_Flag.Over = false;
                if (!pausedCond)
                {
                    m_Flag.ElapsedTickCounts = 0;
                }
            }
        }

        public void Pause()
        {
            m_Flag.Pause = true;
        }

        public void Resume()
        {
            //m_Flag.StartedTime = DateTime.Now.AddMilliseconds(-m_Flag.ElapsedTicks);
            m_Flag.StartedTickCounts = XFunc.GetTickCount() - m_Flag.ElapsedTickCounts;
            m_Flag.Pause = false;
        }
        #endregion
    }
}
