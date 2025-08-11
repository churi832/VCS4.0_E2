using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library;

namespace Sineva.VHL.Device.Assem
{
    public class TaskDeviceControl : XSequence
    {
        public static readonly TaskDeviceControl Instance = new TaskDeviceControl();
        private static object m_LockKey = new object();

        private TaskDeviceControl()
        {
            ThreadInfo.Name = "DeviceControl";
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
