using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Device.Assem
{
    public class TaskSerialControl : XSequence
    {
        public static readonly TaskSerialControl Instance = new TaskSerialControl();
        private static object m_LockKey = new object();

        private TaskSerialControl()
        {
            ThreadInfo.Name = "SerialControl";
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
