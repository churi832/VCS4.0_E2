using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Device.Assem
{
    public class TaskCpsSerialControl : XSequence
    {
        public static readonly TaskCpsSerialControl Instance = new TaskCpsSerialControl();
        private static object m_LockKey = new object();

        private TaskCpsSerialControl()
        {
            ThreadInfo.Name = "CpsSerialControl";
            TaskHandler.Instance.RegTask(this, 50);
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
