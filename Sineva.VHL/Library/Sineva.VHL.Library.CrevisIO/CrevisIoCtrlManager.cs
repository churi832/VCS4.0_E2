using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sineva.VHL.Library.IO;

namespace Sineva.VHL.Library.CrevisIO
{
    public class CrevisIoCtrlManager
    {
        #region Field
        private static object m_LockKey = new object();
        private bool m_Initialized = false;
        private int m_ControllerCount = 0;
        private int m_ControllerInitFailCount = 0;
        private uint m_ControllerConnection = 0x0;
        #endregion

        #region Property
        public int ControllerCount { get { return m_ControllerCount; } }
        public int ControllerFailCount { get { return m_ControllerInitFailCount; } }
        public uint ControllerConnection
        {
            get { return m_ControllerConnection; }
            set { m_ControllerConnection = value; }
        }
        public bool IsAllConnected
        {
            get
            {
                if (m_ControllerInitFailCount > 0) return false;
                if (m_ControllerCount == 0) return false;

                bool connected = true;
                for (int i = 0; i < m_ControllerCount; i++)
                {
                    connected &= (m_ControllerConnection >> i & 0x1) == 0x1;
                }
                return connected;
            }
        }
        #endregion

        #region Constructor
        public static readonly CrevisIoCtrlManager Instance = new CrevisIoCtrlManager();
        private CrevisIoCtrlManager()
        {
        }
        #endregion

        #region Method
        public bool Initialize()
        {
            lock (m_LockKey)
            {
                if (!m_Initialized)
                {
                    if (IoManager.Instance.Initialized == false) IoManager.Instance.Initialize();
                    List<CrevisModbusCtrl> modbusCtrls = new List<CrevisModbusCtrl>();
                    foreach (_IoNode ioNode in IoManager.Instance.Nodes)
                    {
                        try
                        {
                            IoNodeCrevisModbus node = (ioNode as IoNodeCrevisModbus);
                            if (node != null)
                            {
                                modbusCtrls.Add(new CrevisModbusCtrl(ioNode));
                            }
                        }
                        catch { }
                    }

                    foreach (CrevisModbusCtrl ctrl in modbusCtrls)
                    {
                        m_ControllerCount++;
                        if (ctrl.Initialize() == false)
                            m_ControllerInitFailCount++;
                    }

                    if (m_ControllerInitFailCount > 0)
                    {
                        MessageBox.Show(string.Format("Crevis I/O application cannot be run.\n{0} Controllers are not initialized", m_ControllerInitFailCount));
                    }
                    else
                    {
                        TaskHandler.Instance.RegTask(new TaskCrevisModbusControl(modbusCtrls.ToArray()), 10, System.Threading.ThreadPriority.Normal);
                    }
                    m_Initialized = true;
                }
            }
            return m_Initialized;
        }
        public void Connect(CrevisModbusCtrl ctrl)
        {
            ctrl.Open();
        }
        #endregion
    }

    internal class TaskCrevisModbusControl : XSequence
    {
        #region Field
        private CrevisIoCtrlManager m_Manager = null;
        private bool m_AllConnected = false;
        private CrevisModbusCtrl[] m_ModbusControls = null;
        #endregion

        #region Property
        public bool AllConnected
        {
            get { return m_AllConnected; }
            set { m_AllConnected = value; }
        }
        public CrevisModbusCtrl[] ModbusControls
        {
            get { return m_ModbusControls; }
            set { m_ModbusControls = value; }
        }
        #endregion

        public TaskCrevisModbusControl(CrevisModbusCtrl[] controls)
        {
            m_Manager = CrevisIoCtrlManager.Instance;
            ModbusControls = controls;
            RegSeq(new SeqModbusControl(this));
        }
    }

    internal class SeqModbusControl : XSeqFunc
    {
        private CrevisIoCtrlManager m_Manager = null;
        private TaskCrevisModbusControl m_Control = null;
        private Queue<int> m_QueueTryConnect = new Queue<int>();
        private int m_ModbusId = 0;

        public SeqModbusControl(TaskCrevisModbusControl control)
        {
            SeqName = "[CREVIS IO]";
            m_Control = control;
            m_Manager = CrevisIoCtrlManager.Instance;
        }

        public override int Do()
        {
            UpdateConnection();

            int seqNo = SeqNo;
            switch (seqNo)
            {
                case 0:
                    if (m_Manager.IsAllConnected == false)
                    {
                        for (int i = 0; i < m_Control.ModbusControls.Length; i++)
                        {
                            if (m_Control.ModbusControls[i].IsConnected() == false)
                            {
                                if (m_Control.ModbusControls[i].Open())
                                {
                                    m_ModbusId = i;
                                    CrevisCommLog.WriteLog(string.Format("Adapter({0}) Connect Request", m_ModbusId));
                                    seqNo = 10;
                                }
                                else
                                {
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 100;
                                }
                                break;
                            }
                        }
                    }
                    else if (GetElapsedTicks() > 10)
                    {
                        for (int i = 0; i < m_Control.ModbusControls.Length; i++)
                        {
                            m_Control.ModbusControls[i].UpdateIo();
                        }
                        StartTicks = XFunc.GetTickCount();
                    }
                    break;
                case 10:
                    if (m_Control.ModbusControls[m_ModbusId].BusyConnect == false)
                    {
                        if (m_Control.ModbusControls[m_ModbusId].IsConnected())
                        {
                            CrevisCommLog.WriteLog(string.Format("Adapter({0}) is connected", m_ModbusId));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        else
                        {
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 100;
                        }
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 20;
                    }
                    break;
                case 20:
                    if (GetElapsedTicks() > 1000)
                    {
                        seqNo = 0;
                    }
                    break;
                case 100:
                    if (GetElapsedTicks() > 3 * 1000)
                    {
                        seqNo = 0;
                    }
                    break;
            }

            SeqNo = seqNo;
            return -1;
        }
        private void UpdateConnection()
        {
            uint status = 0;
            for (int i = 0; i < m_Control.ModbusControls.Length; i++)
            {
                status |= (uint)(m_Control.ModbusControls[i].IsConnected() ? 0x1 << i : 0);
            }

            m_Manager.ControllerConnection = status;
        }
    }
}
