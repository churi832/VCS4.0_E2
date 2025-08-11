using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Data.EventLog
{
    public delegate void OccurMessageEventHandler(string msg);
    public class EventLogHandler
    {
        public static readonly EventLogHandler Instance = new EventLogHandler();

        #region Fields
        private static Queue<EventLogData> SetQ = new Queue<EventLogData>();
        private readonly static object m_LockKey = new object();

        private EventLogProvider m_EventLogProvider = null;
        #endregion

        public event OccurMessageEventHandler AddMainMessage;

        #region Constructor
        public EventLogHandler()
        {

        }
        #endregion

        #region Methods
        public bool Initialize()
        {
            m_EventLogProvider = EventLogProvider.Instance;
            m_EventLogProvider.LoadFromDB();

            TaskHandler.Instance.RegTask(new EventHanderThread(), 10, System.Threading.ThreadPriority.Normal);
            return true;
        }
        public void Add(string module1, string module2, string message, bool mainMsg)
        {
            lock (m_LockKey)
            {
                if (mainMsg && AddMainMessage != null) AddMainMessage(string.Format("{0}, {1}, {2}", module1, module2, message));

                EventLogData data = new EventLogData();
                data.Module1 = module1;
                data.Module2 = module2;
                data.Message = message;

                SetQ.Enqueue(data);
            }
        }

        public void AddEventLog(EventLogData msg)
        {
            m_EventLogProvider.Add(msg.Module1, msg.Module2, msg.Message);
        }
        #endregion

        #region Sequence
        public class EventHanderThread : XSequence
        {
            public EventHanderThread()
            {
                RegSeq(new SeqEventLogAddRemove());
            }
        }
        public class SeqEventLogAddRemove : XSeqFunc
        {
            public SeqEventLogAddRemove()
            {
                this.SeqName = "SeqEventLogAddRemove";
            }
            public override void SeqAbort()
            {
            }
            public override int Do()
            {
                int seqNo = SeqNo;
                switch (seqNo)
                {
                    case 0:
                        if (EventLogHandler.SetQ.Count > 0)
                        {
                            EventLogData msg = EventLogHandler.SetQ.Dequeue();
                            EventLogHandler.Instance.AddEventLog(msg);
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 10;
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 100)
                            {
                                seqNo = 0;
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;

            }
        }
        #endregion

    }
}
