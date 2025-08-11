using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.IF.GL310
{
    public class TaskGlControl : XSequence
    {
        public static readonly TaskGlControl Instance = new TaskGlControl();
        private static object m_LockKey = new object();

        private TaskGlControl()
        {
            ThreadInfo.Name = "SosControl";
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
