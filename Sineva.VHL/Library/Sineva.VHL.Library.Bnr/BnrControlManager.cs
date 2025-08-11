using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Reflection;
using System.ComponentModel;
using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Library.BnrModbus.Data;
using Sineva.VHL.Library.BnrModbus.Device;
using Sineva.VHL.Library.BnrModbus.Utility;

namespace Sineva.VHL.Library.Bnr
{
    public class BnrControlManager
    {
        public static readonly BnrControlManager Instance = new BnrControlManager();

        #region Fields
        private List<BnrModbusCtrl> m_ModbusCtrls = new List<BnrModbusCtrl>();
        private bool m_HasBnrMotor = false;
        #endregion

        #region Properties
        [XmlIgnore(), Browsable(false)]
        public bool HasBnrMotor
        {
            get { return m_HasBnrMotor; }
            set { m_HasBnrMotor = value; }
        }
        #endregion

        #region Constructor
        private BnrControlManager()
        {

        }

        public bool Initialize()
        {
            bool rv = false;

            foreach (_IoNode ioNode in IoManager.Instance.Nodes)
            {
                if (ioNode.GetType().ToString() == typeof(IoNodeBnrX20CP0482).ToString())
                {
                    m_ModbusCtrls.Add(new BnrModbusCtrl((IoNodeBnrX20CP0482)ioNode));
                }
            }

            foreach (BnrModbusCtrl ctrl in m_ModbusCtrls)
            {
                rv &= ctrl.Initialize();
                m_HasBnrMotor |= ctrl.IsBnrAxisExist();
                TaskHandler.Instance.RegTask(new TaskBnrControl(ctrl), 10, System.Threading.ThreadPriority.Normal);
            }

            return rv;
        }
        #endregion

        #region Methods
        #endregion

        #region Sequence
        public class TaskBnrControl : XSequence
        {
            BnrModbusCtrl m_Ctrl = null;
            public TaskBnrControl(BnrModbusCtrl ctrl)
            {
                m_Ctrl = ctrl;
                RegSeq(new SeqConnection(ctrl));
                RegSeq(new SeqUpdateBnrModus(ctrl));
            }

            protected override void ExitRoutine()
            {
            }
        }

        class SeqConnection : XSeqFunc
        {
            BnrModbusCtrl m_Ctrl = null;

            public SeqConnection(BnrModbusCtrl ctrl)
            {
                m_Ctrl = ctrl;
            }

            public override int Do()
            {
                int seqNo = this.SeqNo;
                switch(SeqNo)
                {
                    case 0:
                        {
                            if (!m_Ctrl.IsConnected()) m_Ctrl.ClientConnection();
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 10;
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;
                            if (m_Ctrl.IsConnected())
                            {
                                IoManager.Instance.UpdateRun = true;
                            }
                            else
                            {
                                IoManager.Instance.UpdateRun = false;
                                seqNo = 0;
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }
        }

        class SeqUpdateBnrModus : XSeqFunc
        {
            BnrModbusCtrl m_Ctrl = null;

            public SeqUpdateBnrModus(BnrModbusCtrl ctrl)
            {
                m_Ctrl = ctrl;
            }

            public override int Do()
            {
                if (m_Ctrl.IsConnected()) m_Ctrl.Update();
                return -1;
            }
        }
        #endregion
    }
}
