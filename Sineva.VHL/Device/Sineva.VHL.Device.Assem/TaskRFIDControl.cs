using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Device.Assem
{
    public class TaskRFIDControl : XSequence
    {
        public static readonly TaskRFIDControl Instance = new TaskRFIDControl();
        private static object m_LockKey = new object();

        private TaskRFIDControl()
        {
            ThreadInfo.Name = "RFIDControl";
            TaskHandler.Instance.RegTask(this, 10);
        }

        public new void RegSeq(XSeqFunc seq)
        {
            lock (m_LockKey)
            {
                if (!IsStarted())
                {
                    this.SeqFuncs.Add(seq);
                }
            }
        }

        public new void Start()
        {
            lock (m_LockKey)
            {
                if (false == IsStarted())
                {
                    this.ThreadInfo.Start();
                }
                else
                {
                    Resume();
                }
            }
        }
    }
}
