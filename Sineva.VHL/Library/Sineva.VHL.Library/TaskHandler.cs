/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.17 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System.Collections.Generic;

namespace Sineva.VHL.Library
{
    public class TaskHandler
    {
        public static readonly TaskHandler Instance = new TaskHandler();

        #region Fields
        public List<XSequence> m_Tasks = new List<XSequence>();
        #endregion

        #region Properties
        public List<XSequence> Tasks
        {
            get { return m_Tasks; }
            set { m_Tasks = value; }
        }
        #endregion

        #region Methods
        public void InitTaskHandler()
        {
            foreach (XSequence task in m_Tasks)
            {
                task.Start();
            }
        }

        public void PauseTask()
        {
            foreach (XSequence task in m_Tasks)
            {
                task.Pause();
            }
        }

        public void AbortTask()
        {
            foreach (var task in m_Tasks)
            {
                task.Abort();
            }
        }

        public void RegTask(XSequence task, int scanInterval)
        {
            task.ScanInterval = scanInterval;
            m_Tasks.Add(task);
        }

        public void RegTask(XSequence task, int scanInterval, System.Threading.ThreadPriority priority, bool timerType = false)
        {
            task.TimerType = timerType;
            task.ScanInterval = scanInterval;
            task.ThreadInfo.Priority = priority;
            m_Tasks.Add(task);
        }

        public List<XSeqFunc> GetFuncLists()
        {
            List<XSeqFunc> lists = new List<XSeqFunc>();
            foreach (var task in m_Tasks) lists.AddRange(task.SeqFuncs);
            return lists;
        }
        #endregion
    }
}
