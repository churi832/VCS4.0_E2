using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Data.Alarm
{
    public struct AlarmItem
    {
        public AlarmCondition Condition;
        public int AlarmId;
    }

    public class AlarmHandler
    {
        #region Fields
        private static Queue<int> SetQ = new Queue<int>();
        private static Queue<int> RstQ = new Queue<int>();

        private AlarmListProvider m_AlarmListProvider = null;
        private AlarmHistoryProvider m_AlarmHistoryProvider = null;
        private AlarmCurrentProvider m_CurrentAlarmProvider = null;
        private readonly static object m_LockKey = new object();

        public static readonly AlarmHandler Instance = new AlarmHandler();

        public static bool AutoRecoveryNeed = false;
        #endregion

        private AlarmHandler()
        {

        }

        public bool Initialize()
        {
            m_AlarmListProvider = AlarmListProvider.Instance;
            m_AlarmHistoryProvider = AlarmHistoryProvider.Instance;
            m_CurrentAlarmProvider = AlarmCurrentProvider.Instance;

            ResetAllAlarm();
            AlarmListProvider.Instance.LoadFromDb();
            AlarmHistoryProvider.Instance.LoadFromDB();

            TaskHandler.Instance.RegTask(new AlarmHistoryThread(), 10, System.Threading.ThreadPriority.Normal);
            return true;
        }

        public bool IsContainAlarm(int id)
        {
            return SetQ.Contains(id);
        }
        public void SetQAlarm(int id)
        {
            SetQ.Enqueue(id);
        }
        public void RstQAlarm(int id)
        {
            RstQ.Enqueue(id);
        }
        public bool SetAlarm(int id)
        {
            bool ok = false;
            lock (m_LockKey)
            {
                try
                {
                    AlarmData alarm = m_AlarmListProvider.GetAlarm(id);
                    if (alarm != null)
                    {
                        bool occured = m_CurrentAlarmProvider.IsAlarm(alarm);
                        if (!occured)
                        {
                            m_CurrentAlarmProvider.Add(alarm);

                            AlarmHistoryData history = new AlarmHistoryData(alarm, DateTime.Now);
                            m_AlarmHistoryProvider.Add(history);

                            ok = true;
                        }
                        if (alarm.Level == AlarmLevel.S && alarm.Code != AlarmCode.IrrecoverableError) 
                            AutoRecoveryNeed = true;
                    }
                }
                catch (Exception err)
                {
                    ExceptionLog.WriteLog(err.ToString());
                }
            }
            return ok;
        }
        public bool ResetAlarm(int id)
        {
            bool ok = false;
            lock (m_LockKey)
            {
                try
                {
                    AlarmData alarm = m_AlarmListProvider.GetAlarm(id);
                    if (alarm != null)
                    {
                        bool occured = m_CurrentAlarmProvider.IsAlarm(alarm);
                        m_CurrentAlarmProvider.fireResetAlarm(alarm);

                        if (occured)
                        {
                            m_CurrentAlarmProvider.Remove(alarm);
                            ok = true;
                        }
                    }
                }
                catch (Exception err)
                {
                    ExceptionLog.WriteLog(err.ToString());
                }
            }
            return ok;
        }
        public string GetAlarmMessage(int id)
        {
            if (m_AlarmListProvider == null) return "";
            string msg = "";
            AlarmData alarm = m_AlarmListProvider.GetAlarm(id);
            if (alarm != null) msg = alarm.Name;
            return msg;
        }
        public void ResetAllAlarm()
        {
            m_CurrentAlarmProvider.ResetAllAlarm();
        }
        public class AlarmHistoryThread : XSequence
        {
            public AlarmHistoryThread()
            {
                RegSeq(new SeqAlarmAddRemove());
            }
        }
        public class SeqAlarmAddRemove : XSeqFunc
        {
            public SeqAlarmAddRemove()
            {
                this.SeqName = "SeqAlarmAddRemove";
            }
            public override void SeqAbort()
            {
            }
            public override int Do()
            {
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        if (AppConfig.AppMainInitiated)
                        {
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 10;
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 5000)
                            {
                                seqNo = 100;
                            }
                        }
                        break;

                    case 100:
                        if (AlarmHandler.SetQ.Count > 0)
                        {
                            AlarmHandler.Instance.SetAlarm(AlarmHandler.SetQ.Dequeue());
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        else if (AlarmHandler.RstQ.Count > 0)
                        {
                            AlarmHandler.Instance.ResetAlarm(AlarmHandler.RstQ.Dequeue());
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        break;

                    case 110:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 200)
                            {
                                seqNo = 100;
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }
        }
    }
}
