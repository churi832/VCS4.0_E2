using Sineva.VHL.Data;
using Sineva.VHL.Device;
using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Task
{
    public class TaskPool
    {
        #region Fields
        public static readonly TaskPool Instance = new TaskPool();
        TaskHandler m_TaskHandler = TaskHandler.Instance;
        #endregion

        #region Constructor
        public TaskPool()
        {
        }
        #endregion

        #region Methods
        public void InitTaskPool()
        {
            #region sequence

            try
            {
                m_TaskHandler.RegTask(TaskInterlockHighSpeed.Instance, 10, System.Threading.ThreadPriority.Highest, true);
                m_TaskHandler.RegTask(TaskCommon.Instance, 10, System.Threading.ThreadPriority.Highest);
                m_TaskHandler.RegTask(TaskInit.Instance, 10, System.Threading.ThreadPriority.Highest);
                m_TaskHandler.RegTask(TaskMain.Instance, 10, System.Threading.ThreadPriority.Highest, true);
                m_TaskHandler.RegTask(TaskMonitor.Instance, 20, System.Threading.ThreadPriority.Highest, true);
                m_TaskHandler.RegTask(TaskInterface.Instance, 10, System.Threading.ThreadPriority.Highest);
                m_TaskHandler.RegTask(TaskInterlock.Instance, 10, System.Threading.ThreadPriority.Highest);
                m_TaskHandler.RegTask(TaskJCS.Instance, 10, System.Threading.ThreadPriority.Highest);
                m_TaskHandler.RegTask(TaskOCS.Instance, 10, System.Threading.ThreadPriority.Highest);
                m_TaskHandler.RegTask(TaskUpdateMotionData.Instance, 10, System.Threading.ThreadPriority.Highest, true);
                m_TaskHandler.RegTask(TaskSearchLink.Instance, 10, System.Threading.ThreadPriority.Highest, true);
                m_TaskHandler.RegTask(TaskPadRemote.Instance, 30, System.Threading.ThreadPriority.Normal);
                m_TaskHandler.RegTask(TaskFDC.Instance, 100, System.Threading.ThreadPriority.Normal);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }                
            #endregion
        }

        public void TaskStart()
        {
            //m_TaskHandler.InitTaskHandler();
            TaskSequenceRunDelay DelayThread = new TaskSequenceRunDelay();
            DelayThread.Start();
        }

        public void TaskPause()
        {
            m_TaskHandler.PauseTask();
        }
        #endregion
    }

    // UI 작업이 모두 완료된 후 Task를 Running 하자
    public class TaskSequenceRunDelay : XSequence
    {
        public TaskSequenceRunDelay()
        {
            ThreadInfo.Name = string.Format("TaskSequenceRunDelay");
            this.ScanInterval = 20;

            RegSeq(new SequenceRunDelay(this));
        }

        public class SequenceRunDelay : XSeqFunc
        {
            #region Fields
            TaskSequenceRunDelay m_Parent = null;
            private List<int> m_ThreadToggleCount = new List<int>();
            private int m_MaxScanInterval = 100;
            private List<uint> m_ScanStartTicks = new List<uint>();
            private bool m_ResetTime = false;
            #endregion

            #region Constructor
            public SequenceRunDelay(TaskSequenceRunDelay parent)
            {
                m_Parent = parent;
                this.SeqName = "SequenceRunDelay";
            }
            #endregion

            #region Methods override
            public override int Do()
            {
                int rv = -1;
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 10;
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 5000) break;

                            TaskHandler.Instance.InitTaskHandler();

                            // Thread 가 동작할 시간을 좀 주자 ~~~
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        break;

                    case 20:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 5000) break;

                            int max = int.MinValue;
                            uint nowTicks = XFunc.GetTickCount();
                            m_ThreadToggleCount.Clear();
                            m_ScanStartTicks.Clear();
                            for (int i = 0; i < XSequence.StatusList.Count; i++)
                            {
                                m_ThreadToggleCount.Add(XSequence.StatusList[i].ToggleCount);
                                m_ScanStartTicks.Add(nowTicks);
                                if (XSequence.StatusList[i].ScanInterval > max) max = 5 * XSequence.StatusList[i].ScanInterval;
                            }
                            if (max != int.MinValue) m_MaxScanInterval = max;
                            else m_MaxScanInterval = 100;

                            m_ResetTime = true;
                            StartTicks = nowTicks;
                            seqNo = 100;
                        }
                        break;

                    case 100:
                        {
                            uint nowTicks = XFunc.GetTickCount();
                            if (m_ResetTime)
                            {
                                m_ResetTime = false;
                                for (int i = 0; i < XSequence.StatusList.Count; i++) m_ScanStartTicks[i] = nowTicks;
                            }

                            // Thread State를 관찰하고 Toggle이 변경되지 않으면 이상하게 생각하여 Abort 시키자
                            if (m_ThreadToggleCount.Count == XSequence.StatusList.Count)
                            {
                                string msg = string.Empty;
                                bool ng = false;
                                for (int i = 0; i < m_ThreadToggleCount.Count; i++)
                                {
                                    bool thread_ng = false;
                                    uint changeTicks = nowTicks - m_ScanStartTicks[i];
                                    if (changeTicks > (5.0f * XSequence.StatusList[i].ScanInterval))
                                    {
                                        thread_ng = m_ThreadToggleCount[i] == XSequence.StatusList[i].ToggleCount;
                                        if (thread_ng)
                                        {
                                            msg += string.Format("|{0}_{1}({2},{3})|", XSequence.StatusList[i].ThreadName, XSequence.StatusList[i].ThreadDescription, XSequence.StatusList[i].LastScanMilliSec, XSequence.StatusList[i].ToggleCount);
                                        }
                                        else
                                        {
                                            m_ScanStartTicks[i] = nowTicks;
                                        }
                                    }
                                    ng |= thread_ng;
                                    m_ThreadToggleCount[i] = XSequence.StatusList[i].ToggleCount;
                                }

                                if (ng)
                                {
                                    string message = string.Format("Thread Abnormal Stop Check ! NG_THREAD={0}", msg);
                                    ThreadCheckLog.WriteLog(message);
                                    //if (!isAlive) seqNo = 110; //무조건 멈추도록 해야 겠다..
                                    StartTicks = XFunc.GetTickCount();
                                    //m_ResetTime = true;
                                    //seqNo = 110; // Thread 감시를 해야 될것 같다....충돌을 해 버리네~~24년말
                                    seqNo = 1000; // 일단 Alarm을 띄우지 말고 Log 상태를 보자~~~
                                }
                                else if (nowTicks - StartTicks > 2 * m_MaxScanInterval)
                                {
                                    m_ResetTime = true;
                                    GV.ThreadStop = false;
                                    StartTicks = XFunc.GetTickCount();
                                }
                            }
                        }
                        break;

                    case 110:
                        {
                            uint nowTicks = XFunc.GetTickCount();
                            if (nowTicks - StartTicks < 300) break;

                            // Thread State를 관찰하고 Toggle이 변경되지 않으면 이상하게 생각하여 Abort 시키자
                            if (m_ThreadToggleCount.Count == XSequence.StatusList.Count)
                            {
                                string msg = string.Empty;
                                bool ng = false;
                                for (int i = 0; i < m_ThreadToggleCount.Count; i++)
                                {
                                    bool thread_ng = false;
                                    thread_ng = m_ThreadToggleCount[i] == XSequence.StatusList[i].ToggleCount;
                                    if (thread_ng)
                                    {
                                        msg += string.Format("|{0}_{1}({2},{3})|", XSequence.StatusList[i].ThreadName, XSequence.StatusList[i].ThreadDescription, XSequence.StatusList[i].LastScanMilliSec, XSequence.StatusList[i].ToggleCount);
                                    }
                                    ng |= thread_ng;
                                    m_ThreadToggleCount[i] = XSequence.StatusList[i].ToggleCount;
                                }

                                if (ng)
                                {
                                    GV.ThreadStop = true;
                                    EqpStateManager.Instance.SetOpMode(OperateMode.Manual);
                                    EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort);

                                    // Thread가 않돈다고 생각하면 여기서 Stop/Abort 처리를 해줘야 될것 같음.
                                    List<XSeqFunc> funcs = TaskHandler.Instance.GetFuncLists();
                                    foreach (XSeqFunc func in funcs)
                                    {
                                        func.SeqAbort();
                                        func.InitSeq();
                                    }
                                    DevicesManager.Instance.SeqAbort();

                                    string message = string.Format("Thread Abnormal Stop, All Abort, Program Restart ! NG_THREAD={0}", msg);
                                    EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.InterlockMessage, message);
                                    ThreadCheckLog.WriteLog(message);

                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 1000;
                                }
                                else if (nowTicks - StartTicks > m_MaxScanInterval)
                                {
                                    m_ResetTime = true;
                                    GV.ThreadStop = false;
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 100;
                                }
                            }
                        }
                        break;

                    case 1000:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;

                            bool ng = false;
                            if (m_ThreadToggleCount.Count == XSequence.StatusList.Count)
                            {
                                for (int i = 0; i < m_ThreadToggleCount.Count; i++)
                                {
                                    bool thread_ng = m_ThreadToggleCount[i] == XSequence.StatusList[i].ToggleCount;
                                    ng |= thread_ng;
                                }
                            }
                            if (!ng)
                            {
                                ThreadCheckLog.WriteLog("Thread Abnormal Stop Release [Thread OK]");

                                m_ResetTime = true;
                                GV.ThreadStop = false;
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 100;
                            }
                            else if (GV.OperatorCallConfirm)
                            {
                                ThreadCheckLog.WriteLog("Thread Abnormal Stop Release [Operator]");

                                m_ResetTime = true;
                                GV.OperatorCallConfirm = false;
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 100;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }

            private void SetThreadLog()
            {
                try
                {
                    int overCount = 0;
                    string msg = string.Format("\r\n");
                    foreach (XSequence seq in TaskHandler.Instance.Tasks)
                    {
                        int index = seq.ID;
                        if (index < XSequence.StatusList.Count)
                        {
                            float interval = seq.ScanInterval;
                            if (seq.ScanInterval < 5) interval = 3.0f * seq.ScanInterval;
                            else if (seq.ScanInterval < 10) interval = 2.0f * seq.ScanInterval;
                            else interval = 1.5f * (float)seq.ScanInterval;

                            if (XSequence.StatusList[index].LastScanMilliSec > interval)
                            {
                                //Sequence의 Scan Interval 설정에 따라 기준을 달리해 Log를 남긴다...
                                // <5ms =>  LastScanMilliSec > (ScanInterval * 3)
                                // >5ms && < 10ms =>  LastScanMilliSec > (ScanInterval * 2)
                                // >=10ms : LastScanMilliSec > (ScanInterval * 1.5)
                                msg += string.Format("ID={0}, ThreadName={1}, Description={2}, SeqFunc={3}, ToggleCount={4}, LastScanMilliSec={5}/{6}\r\n",
                                    index, seq.ToString(), XSequence.StatusList[index].ThreadDescription, seq.SeqFuncs.Count, XSequence.StatusList[index].ToggleCount, XSequence.StatusList[index].LastScanMilliSec,
                                    seq.ScanInterval);
                                overCount += 1;
                            }
                        }

                        foreach(XSeqFunc func in seq.SeqFuncs)
                        {
                            if (func.ScanTime > seq.ScanInterval)
                            {
                                msg += string.Format("SeqName={0}, ScanTime={1}\r\n", func.SeqName, func.ScanTime);
                            }
                        }
                    }
                    if (overCount > 0) ThreadCheckLog.WriteLog(msg);
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }
            #endregion
        }
    }
}
