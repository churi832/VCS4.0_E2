/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.17 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Sineva.VHL.Library
{
    public class ThreadStatus
    {
        private string m_ThreadName;
        private string m_ThreadDescription;
        private float m_LastScanMilliSec;
        private bool m_Toggle;
        private int m_ToggleCount;
        private int m_ScanInterval;
        public string ThreadName
        {
            get { return m_ThreadName; }
            set { m_ThreadName = value; }
        }
        public string ThreadDescription
        {
            get { return m_ThreadDescription; }
            set { m_ThreadDescription = value; }
        }

        public float LastScanMilliSec
        {
            get { return m_LastScanMilliSec; }
            set { m_LastScanMilliSec = value; }
        }

        public bool Toggle
        {
            get { return m_Toggle; }
            set { m_Toggle = value; }
        }
        public int ToggleCount
        {
            get { return m_ToggleCount; }
            set { m_ToggleCount = value; }
        }
        public int ScanInterval
        {
            get { return m_ScanInterval; }
            set { m_ScanInterval = value; }
        }
        public ThreadStatus()
        {
        }

        public ThreadStatus(string name)
        {
            m_ThreadName = name;
            m_ToggleCount = 0;
        }
        #region override
        public override string ToString()
        {
            return string.Format("{0}_{1}", ThreadName, ThreadDescription);
        }
        #endregion
    }

    public class XSequence
    {
        #region Fields
        private Thread m_Thread = null;
        private int m_ScanInterval = 100;
        private List<XSeqFunc> m_SeqFuncs = new List<XSeqFunc>();
        private static int m_Serial = 0;
        private int m_Id;
        private static List<System.Diagnostics.Stopwatch> m_Watchs = new List<System.Diagnostics.Stopwatch>();
        private static List<ThreadStatus> m_StatusList = new List<ThreadStatus>();
        private static List<System.Diagnostics.Stopwatch> m_SeqFuncWatchs = new List<System.Diagnostics.Stopwatch>();
        private static bool m_StopGraceful = false;
        #endregion

        #region Fields - MultiMediaTimer
        private XMultiMediaTimer m_Timer;
        private bool m_TimerType = false;
        public static float m_CurrentCPU = 0;
        #endregion

        #region Properties
        public List<XSeqFunc> SeqFuncs
        {
            get { return m_SeqFuncs; }
            set { m_SeqFuncs = value; }
        }

        public Thread ThreadInfo
        {
            get { return m_Thread; }
            set { m_Thread = value; }
        }
        public int ScanInterval
        {
            get { return m_ScanInterval; }
            set 
            { 
                m_ScanInterval = value; 
                if (m_Id < m_StatusList.Count) 
                    m_StatusList[m_Id].ScanInterval = value; 
            }
        }

        public static List<ThreadStatus> StatusList
        {
            get { return m_StatusList; }
            set { m_StatusList = value; }
        }

        public bool IsBackground
        {
            get { return m_Thread.IsBackground; }
            set { m_Thread.IsBackground = value; }
        }

        public int ID
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public bool TimerType
        {
            get { return m_TimerType; }
            set { m_TimerType = value; }
        }
        #endregion

        public XSequence(int scanInterval) : this()
        {
            m_ScanInterval = scanInterval;
        }

        public XSequence()
        {
            m_Id = m_Serial;
            m_Serial++;

            m_Watchs.Add(new System.Diagnostics.Stopwatch());
            m_SeqFuncWatchs.Add(new System.Diagnostics.Stopwatch());
            m_StatusList.Add(new ThreadStatus(this.ToString()));
            m_StatusList[m_Id].ScanInterval = m_ScanInterval;

            this.m_Thread = new Thread(new ThreadStart(this.ThreadProc));
            this.m_Thread.IsBackground = true;
        }

        public void ThreadProc()
        {
            while (!m_StopGraceful)
            {
                Sequence();
            }

            ExitRoutine();
        }

        public virtual void Sequence()
        {
            try
            {
                //if (m_Watchs[m_Id].IsRunning)
                {
                    //m_Watchs[m_Id].Stop();
                    //m_StatusList[m_Id].LastScanMilliSec = (float)((double)m_Watchs[m_Id].ElapsedTicks * 1000.0 /
                    //    (double)System.Diagnostics.Stopwatch.Frequency);
                    m_StatusList[m_Id].Toggle = !m_StatusList[m_Id].Toggle;
                    m_StatusList[m_Id].ToggleCount++;
                    //m_Watchs[m_Id].Reset();
                }
                //m_Watchs[m_Id].Start();

                if (m_Watchs[m_Id].IsRunning == false) m_Watchs[m_Id].Start();
                m_Watchs[m_Id].Reset();
                m_Watchs[m_Id].Start();

                try
                {
                    Thread.Sleep(m_ScanInterval);
                }
                catch (Exception e) { }

                foreach (XSeqFunc seq in SeqFuncs)
                {
                    seq.Do();
                }

                m_Watchs[m_Id].Stop();
                m_StatusList[m_Id].LastScanMilliSec = (float)((double)m_Watchs[m_Id].ElapsedTicks * 1000.0 /
                    (double)System.Diagnostics.Stopwatch.Frequency);

                //if (m_Id == 31 || m_Id == 35 || m_Id == 38 || m_Id == 39 || m_Id == 44)
                //{
                //    Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} : OnTimer_Tick ({m_Id},{m_StatusList[m_Id].ThreadName},{m_StatusList[m_Id].LastScanMilliSec},{m_CurrentCPU})");
                //}
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public void SetSubInfo(string subInfo)
        {
            m_StatusList[m_Id].ThreadDescription = subInfo;
        }
        public void RegSeq(XSeqFunc seq)
        {
            SeqFuncs.Add(seq);
        }

        protected virtual void ExitRoutine()
        {
            if (m_TimerType)
            {
                m_Timer.Stop();
            }
            else
            {
                m_Thread.Abort();
            }
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }

        public Boolean IsStarted()
        {
            if (m_TimerType)
            {
                return m_Timer!= null && m_Timer.IsRunning;
            }
            else
            {
                int i = (int)(this.m_Thread.ThreadState & ThreadState.Unstarted);

                return (i == 0) ? true : false;
            }
        }
        public Boolean IsAlive()
        {
            return m_Thread.IsAlive;
        }

        public void Start()
        {
            if (m_TimerType)
            {
                if (m_Timer == null)
                {
                    m_Timer = new XMultiMediaTimer();
                    m_Timer.Mode = TimerMode.Periodic;
                    m_Timer.Period = m_ScanInterval;
                    //m_Timer.SynchronizingObject = this;
                    m_Timer.Tick += new System.EventHandler(OnTimer_Tick);
                }
                if (!m_Timer.IsRunning)
                    m_Timer.Start();
            }
            else
            {
                if (false == IsStarted())
                {
                    this.m_Thread.Start();
                }
                else
                {
                    Resume();
                }
            }
        }

        public Boolean IsSuspended()
        {
            int i = (int)(this.m_Thread.ThreadState & ThreadState.Suspended);

            return (i != 0) ? true : false;
        }

        public void Pause()
        {
            if (m_TimerType)
            {
                if (m_Timer != null) m_Timer.Stop();
            }
            else
            {
                if (true == IsStarted() &&
                    false == IsSuspended() &&
                    m_Thread.IsAlive)
                {
                    this.m_Thread.Suspend();
                }
            }
        }

        public static void StopGraceful()
        {
            m_StopGraceful = true;
        }

        public void Resume()
        {
            if (m_TimerType)
            {
                if (m_Timer != null && !m_Timer.IsRunning) 
                    m_Timer.Start();
            }
            if (true == IsStarted() &&
                true == IsSuspended() &&
                m_Thread.IsAlive)
            {
                this.m_Thread.Resume();
            }
        }
        public bool IsResume()
        {
            bool resume = true;
            resume &= IsStarted();
            resume &= IsSuspended();
            resume &= m_Thread.IsAlive;
            return resume;
        }

        public void Abort()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                if (m_TimerType)
                {
                    if (m_Timer != null) m_Timer.Stop();
                }
                else this.m_Thread.Abort();
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(method, String.Format(err.ToString()));
            }
        }

        #region Methods - MultiMediaTimer
        private void OnTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                m_StatusList[m_Id].Toggle = !m_StatusList[m_Id].Toggle;
                m_StatusList[m_Id].ToggleCount++;

                if (m_Watchs[m_Id].IsRunning == false) m_Watchs[m_Id].Start();
                m_StatusList[m_Id].LastScanMilliSec = (float)((double)m_Watchs[m_Id].ElapsedTicks * 1000.0 /
                    (double)System.Diagnostics.Stopwatch.Frequency);

                //if (AppConfig.Instance.Simulation.MY_DEBUG)
                //    Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} : OnTimer_Tick ({m_Id},{m_StatusList[m_Id].ThreadName},{m_StatusList[m_Id].LastScanMilliSec},{m_CurrentCPU})");

                if (m_StatusList[m_Id].LastScanMilliSec > 0.5f * m_ScanInterval) // Timer 무한 생성시 연속 가능한 시간 제한...
                {
                    foreach (XSeqFunc seq in SeqFuncs)
                    {
                        seq.Do();
                    }
                    m_Watchs[m_Id].Reset();
                    m_Watchs[m_Id].Start();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion
    }
}
