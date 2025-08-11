/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V1.0
 * Programmer	: Software Group
 * Issue Date	: 23.02.20
 * Description	: 
 * 
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using MotionAPI;
using Sineva.VHL.Library.IO;

namespace Sineva.VHL.Library.Mp2100
{
    public class Mp2100IoCtrl
    {
        #region Fields
        private UInt32 m_CtrlHandle = 0;
        private CMotionAPI.COM_DEVICE m_ComDevice;
        private bool m_OpenComp = false;
        private bool m_Use = false;
        private bool m_OpenFail = false;

        private bool m_FristOutputUpdate = false;
        private IoNodeMp2100 m_Node = null;
        private List<_IoTerminal> m_Terminals = new List<_IoTerminal>();
        #endregion

        #region Properties
        public List<_IoTerminal> Terminals
        {
            get { return m_Terminals; }
            set { m_Terminals = value; }
        }

        public bool Use
        {
            get { return m_Use; }
            set { m_Use = value; }
        }
        #endregion

        #region Constructor / Destructor
        public Mp2100IoCtrl()
        {
        }

        public Mp2100IoCtrl(IoNodeMp2100 node)
        {
#if WIN64PCIe
			m_ComDevice.ComDeviceType = (UInt16)CMotionAPI.ApiDefs.COMDEVICETYPE_PCIe_MODE;
#else
            m_ComDevice.ComDeviceType = (UInt16)CMotionAPI.ApiDefs.COMDEVICETYPE_PCI_MODE;
#endif
            m_ComDevice.PortNumber = node.PortNo;
            m_ComDevice.CpuNumber = (ushort)node.CpuNo;	//cpuno;
            m_ComDevice.NetworkNumber = node.NetworkNo;
            m_ComDevice.StationNumber = node.StationNo;
            m_ComDevice.UnitNumber = node.UnitNo;
            m_ComDevice.IPAddress = node.IpAddress;
            m_ComDevice.Timeout = node.TimeOut;

            m_Node = node;
        }

        ~Mp2100IoCtrl()
        {
            //CloseController();
        }
        #endregion

        #region Methods
        public bool InitializeIo()
        {
            TaskHandler.Instance.RegTask(new TaskMp2100Io(this), 10, System.Threading.ThreadPriority.Normal);
            if (AppConfig.Instance.Simulation.IO) IoManager.Instance.UpdateRun = true;

            return true;
        }

        private uint OpenController()
        {
            //lock (m_LockKey)
            {
                UInt32 rv;
                if (AppConfig.Instance.Simulation.IO == false)
                {
                    rv = CMotionAPI.ymcOpenController(ref m_ComDevice, ref m_CtrlHandle);
                    if (rv == CMotionAPI.MP_SUCCESS)
                    {
                        rv = CMotionAPI.ymcSetAPITimeoutValue(3000);
                        if (rv == CMotionAPI.MP_SUCCESS)
                        {
                            m_OpenComp = true;
                            return CMotionAPI.MP_SUCCESS;
                        }
                        else
                        {
                            MessageBox.Show(String.Format("Mp2100 SetApiTimeout Error(Code = 0x{0:X})", rv));
                            return rv;
                        }
                    }
                    else
                    {
                        MessageBox.Show(String.Format("Mp2100 Open Error(Code = 0x{0:X})", rv));
                        return rv;
                    }
                }
                else
                {
                    MessageBox.Show("Mp2100 run Simulation Mode", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    m_OpenComp = true;
                    return CMotionAPI.MP_SUCCESS;
                }
            }
        }

        public uint CloseController()
        {
            UInt32 rv = CMotionAPI.MP_SUCCESS;
            if (AppConfig.Instance.Simulation.IO == false)
            {
                try
                {
                    rv = CMotionAPI.ymcCloseController(m_CtrlHandle);
                    m_CtrlHandle = 0;
                }
                catch (Exception err)
                {
                    ExceptionLog.WriteLog(err.Message);
                    rv = CMotionAPI.MP_FAIL;
                }
            }
            return rv;
        }

        private uint GetRegisterDataHandle(string registerName, ref uint hRegisterData)
        {
            uint rv = CMotionAPI.MP_SUCCESS;
            if (AppConfig.Instance.Simulation.IO == false)
            {
                try
                {
                    rv = CMotionAPI.ymcGetRegisterDataHandle(registerName, ref hRegisterData);
                }
                catch (Exception err)
                {
                    ExceptionLog.WriteLog(err.ToString());
                    rv = CMotionAPI.MP_FAIL;
                }

            }
            return rv;
        }

        private uint GetRegisterData(UInt32 hRegisterData, UInt32 RegisterDataNumber, UInt32[] pRegisterData, ref UInt32 pReadDataNumber)
        {
            uint rv = CMotionAPI.MP_SUCCESS;
            if (AppConfig.Instance.Simulation.IO == false)
            {
                try
                {
                    rv = CMotionAPI.ymcGetRegisterData(hRegisterData, RegisterDataNumber, pRegisterData, ref pReadDataNumber);
                }
                catch (Exception err)
                {
                    ExceptionLog.WriteLog(err.ToString());
                    rv = CMotionAPI.MP_FAIL;
                }
            }
            return rv;
        }

        private uint SetRegisterData(UInt32 hRegisterData, UInt32 RegisterDataNumber, UInt32[] pRegisterData)
        {
            //lock (m_LockKey)
            {
                uint rv = CMotionAPI.MP_SUCCESS;
                if (AppConfig.Instance.Simulation.IO == false)
                {
                    try
                    {
                        rv = CMotionAPI.ymcSetRegisterData(hRegisterData, RegisterDataNumber, pRegisterData);
                    }
                    catch (Exception err)
                    {
                        ExceptionLog.WriteLog(err.ToString());
                        rv = CMotionAPI.MP_FAIL;
                    }

                }
                return rv;
            }
        }

        private void UpdateIo()
        {
            if (!m_OpenComp && m_OpenFail == false)
            {
                if (CMotionAPI.MP_SUCCESS != OpenController())
                {
                    m_OpenFail = true;
                    return;
                }
            }
            if (!m_FristOutputUpdate) return;

            string registerName = "";
            uint hRegisterData = 0x00000000;
            uint registerDataNumber = 0;
            uint longDataNumber = 0;
            uint readDataNumber = 0;

            foreach (_IoTerminal terminal in m_Node.Terminals)
            {
                if (Type.Equals(terminal.GetType(), typeof(IoTermMpIO2310_30)))
                {
                    IoTermMpIO2310_30 term = terminal as IoTermMpIO2310_30;

                    // Read Registry ta and update io //////////////////////////////////////////////////////////////
                    // Read Registry ta and update io //////////////////////////////////////////////////////////////
                    registerName = term.InRegPrefix + string.Format("{0:X4}", term.InStartAddress);
                    uint rv = GetRegisterDataHandle(registerName, ref hRegisterData);
                    if (rv == CMotionAPI.MP_SUCCESS)
                    {
                        registerDataNumber = term.InRegisterCount; // 4 * 16 bit (channel)
                        longDataNumber = registerDataNumber / 2;
                        uint[] regLongData = new uint[longDataNumber];
                        rv = GetRegisterData(hRegisterData, longDataNumber, regLongData, ref readDataNumber);
                        if (rv == CMotionAPI.MP_SUCCESS)// && AppConfig.Instance.Simulation.IO == false)
                        {
                            if (AppConfig.Instance.Simulation.IO)
                            {
                                for (int i = 0; i < longDataNumber; i++)
                                {
                                    regLongData[i] = 0;
                                    uint val;
                                    for (int j = 0; j < 32; j++)
                                    {
                                        if (term.Channels[(i * 32) + j].IsBContact == false)
                                            val = string.Equals(term.Channels[(i * 32) + j].State, "ON") ? (uint)1 : (uint)0;
                                        else val = string.Equals(term.Channels[(i * 32) + j].State, "ON") ? (uint)0 : (uint)1;
                                        regLongData[i] |= (uint)(val << j);
                                    }
                                }
                            }

                            // Parsing Read Data
                            for (int i = 0; i < registerDataNumber / 2; i++)
                            {
                                for (int j = 0; j < 32; j++)
                                {
                                    if (term.Channels[(i * 32) + j].IsBContact == false)
                                        term.Channels[(i * 32) + j].State = ((regLongData[i] >> j) & 0x01) > 0 ? "ON" : "OFF";
                                    else
                                        term.Channels[(i * 32) + j].State = ((regLongData[i] >> j) & 0x01) > 0 ? "OFF" : "ON";
                                }
                            }

                            IoManager.Instance.UpdateRun = true;
                        }
                        else if (AppConfig.Instance.Simulation.IO == false)
                        {
                            m_OpenComp = false;
                            m_OpenFail = true;
                            ExceptionLog.WriteLog(String.Format("Mp2100 {0} GetRegisterData Error(Code = 0x{1:X})", registerName, rv));
                        }
                        regLongData = null;
                    }
                    else
                    {
                        m_OpenComp = false;
                        m_OpenFail = true;
                        ExceptionLog.WriteLog(String.Format("Mp2100 {0} GetRegisterDataHandel Error(Code = 0x{1:X})", registerName, rv));
                    }

                    // Write io to Registry data //////////////////////////////////////////////////////////////
                    // Write io to Registry data //////////////////////////////////////////////////////////////
                    registerName = term.OutRegPrefix + string.Format("{0:X4}", term.OutStartAddress);
                    rv = GetRegisterDataHandle(registerName, ref hRegisterData);

                    if (rv == CMotionAPI.MP_SUCCESS && (hRegisterData != (uint)0) && AppConfig.Instance.Simulation.IO == false)
                    {
                        registerDataNumber = term.OutRegisterCount; // 4 * 16 bit (channel)
                        longDataNumber = registerDataNumber / 2;
                        uint[] regLongData = new uint[longDataNumber];
                        for (int i = 0; i < longDataNumber; i++)
                        {
                            regLongData[i] = 0;
                            uint val;
                            for (int j = 0; j < 32; j++)
                            {
                                val = string.Equals(term.Channels[64 + (i * 32) + j].State, "ON") ? (uint)1 : (uint)0;
                                regLongData[i] |= (uint)(val << j);
                            }
                        }
                        rv = SetRegisterData(hRegisterData, longDataNumber, regLongData);
                        if (rv != CMotionAPI.MP_SUCCESS)
                        {
                            m_OpenComp = false;
                            m_OpenFail = true;
                            ExceptionLog.WriteLog(String.Format("Mp2100 {0} SetRegisterData Error(Code = 0x{1:X})", registerName, rv));
                        }
                        regLongData = null;
                    }
                    else if (AppConfig.Instance.Simulation.IO == false)
                    {
                        m_OpenComp = false;
                        m_OpenFail = true;
                        ExceptionLog.WriteLog(String.Format("Mp2100 {0} GetRegisterDataHandel Error(Code = 0x{1:X})", registerName, rv));
                    }
                }
                else if (Type.Equals(terminal.GetType(), typeof(IoTermMpAN2900)))
                {
                }
                else if (Type.Equals(terminal.GetType(), typeof(IoTermMpAN2910)))
                {
                }
            }
        }

        private void FirstOutputUpdate()
        {
            if (m_FristOutputUpdate) return;
            if (!m_OpenComp && m_OpenFail == false)
            {
                if (CMotionAPI.MP_SUCCESS != OpenController())
                {
                    m_OpenFail = true;
                    return;
                }
            }

            string registerName = "";
            uint hRegisterData = 0x00000000;
            uint registerDataNumber = 0;
            uint longDataNumber = 0;
            uint readDataNumber = 0;

            foreach (_IoTerminal terminal in m_Node.Terminals)
            {
                if (Type.Equals(terminal.GetType(), typeof(IoTermMpIO2310_30)))
                {
                    IoTermMpIO2310_30 term = terminal as IoTermMpIO2310_30;

                    // Write io to Registry data //////////////////////////////////////////////////////////////
                    // Write io to Registry data //////////////////////////////////////////////////////////////
                    registerName = term.OutRegPrefix + string.Format("{0:X4}", term.OutStartAddress);
                    uint rv = GetRegisterDataHandle(registerName, ref hRegisterData);
                    if (rv == CMotionAPI.MP_SUCCESS && AppConfig.Instance.Simulation.IO == false)
                    {
                        registerDataNumber = term.OutRegisterCount; // 4 * 16 bit (channel)
                        longDataNumber = registerDataNumber / 2;
                        uint[] regLongData = new uint[longDataNumber];
                        rv = GetRegisterData(hRegisterData, longDataNumber, regLongData, ref readDataNumber);
                        if (rv == CMotionAPI.MP_SUCCESS && AppConfig.Instance.Simulation.IO == false)
                        {
                            // Parsing Read Data
                            for (int i = 0; i < registerDataNumber / 2; i++)
                            {
                                for (int j = 0; j < 32; j++)
                                {
                                    term.Channels[64 + (i * 32) + j].State = ((regLongData[i] >> j) & 0x01) > 0 ? "ON" : "OFF";
                                    //if(term.Channels[64 + (i * 32) + j].IsBContact == false)
                                    //    term.Channels[64 + (i * 32) + j].State = ((regLongData[i] >> j) & 0x01) > 0 ? "ON" : "OFF";
                                    //else
                                    //    term.Channels[64 + (i * 32) + j].State = ((regLongData[i] >> j) & 0x01) > 0 ? "OFF" : "ON";
                                }
                            }
                        }
                        else if (AppConfig.Instance.Simulation.IO == false)
                        {
                            m_OpenComp = false;
                            m_OpenFail = true;
                            ExceptionLog.WriteLog(String.Format("Mp2100 {0} GetRegisterData Error(Code = 0x{1:X})", registerName, rv));
                        }
                        regLongData = null;

                    }
                    else if (AppConfig.Instance.Simulation.IO == false)
                    {
                        m_OpenComp = false;
                        m_OpenFail = true;
                        ExceptionLog.WriteLog(String.Format("Mp2100 {0} GetRegisterDataHandel Error(Code = 0x{1:X})", registerName, rv));
                    }
                }
            }
            m_FristOutputUpdate = true;
        }
        #endregion

        public class TaskMp2100Io : XSequence
        {
            SeqUpdateIo m_SeqUpdate = null;
            Mp2100IoCtrl m_Ctrl = null;
            public TaskMp2100Io(Mp2100IoCtrl ctrl)
            {
                m_Ctrl = ctrl;
                m_SeqUpdate = new SeqUpdateIo(ctrl);
                RegSeq(m_SeqUpdate);
                RegSeq(new OnlyOneUpdateIo(ctrl));
            }

            protected override void ExitRoutine()
            {
                m_Ctrl.CloseController();
            }
        }

        class SeqUpdateIo : XSeqFunc
        {
            Mp2100IoCtrl m_Ctrl = null;

            public SeqUpdateIo(Mp2100IoCtrl ctrl)
            {
                m_Ctrl = ctrl;
            }

            public override int Do()
            {
                if (AppConfig.Instance.Simulation.MP) return -1;

                m_Ctrl.UpdateIo();
                return -1;
            }
        }

        class OnlyOneUpdateIo : XSeqFunc
        {
            Mp2100IoCtrl m_Ctrl = null;

            public OnlyOneUpdateIo(Mp2100IoCtrl ctrl)
            {
                m_Ctrl = ctrl;
            }

            public override int Do()
            {
                m_Ctrl.FirstOutputUpdate();
                return -1;
            }
        }
    }
}
