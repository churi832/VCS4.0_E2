using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Library.Servo;
using Sineva.VHL.Library;

namespace Sineva.VHL.Library.MXP
{
    public class MxpIoCtrl
    {
        #region Fields
        private bool m_FirstOutputUpdate = false;
        private bool m_IsConnected = false;
        private IoNodeMxpEC m_Node = null;

        private string m_InputLogMessage = string.Empty;
        private string m_OutputLogMessage = string.Empty;        
        #endregion

        #region Properties
        public bool IsConnected
        {
            get { return m_IsConnected; }
            set { m_IsConnected = value; }
        }
        #endregion

        #region Constructor
        public MxpIoCtrl()
        {
        }
        public MxpIoCtrl(IoNodeMxpEC node)
        {
            m_Node = node;
        }
        #endregion

        #region Methods
        public bool InitializeIo(bool thread_create = true)
        {
            if (thread_create)
                TaskHandler.Instance.RegTask(new TaskMxpIo(this), 10, System.Threading.ThreadPriority.Normal);

            return true;
        }
        #endregion

        #region Thread
        public class TaskMxpIo : XSequence
        {
            MxpIoCtrl m_Ctrl = null;
            public TaskMxpIo(MxpIoCtrl ctrl)
            {
                m_Ctrl = ctrl;
                RegSeq(new SeqUpdateIo(m_Ctrl));
                RegSeq(new OnlyOneUpdateIo(m_Ctrl));
            }
            protected override void ExitRoutine()
            {
            }
        }

        private bool UpdateIo()
        {
            if (!m_IsConnected) return false;
            if (!m_FirstOutputUpdate) return false;

            bool changed_io = false;
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                m_InputLogMessage = string.Empty;
                m_OutputLogMessage = string.Empty;

                bool normal_update = true;
                foreach (_IoTerminal terminal in m_Node.Terminals)
                {
                    if (Type.Equals(terminal.GetType(), typeof(IoTermMxpD232))) //Input 32접점
                    {
                        IoTermMxpD232 term = terminal as IoTermMxpD232;
                        UInt32 nReadValue = 0;
                        if (AppConfig.Instance.Simulation.IO == false)
                            rv = MXP.IO_ReadDword((UInt32)term.SlaveNo, MXP.MXP_IO_TYPE.IO_IN, (UInt16)0, ref nReadValue);
                        if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        {
                            for (int i = 0; i < term.InChannelCnt; i++)
                            {
                                if (term.Channels[i].IsBContact == false)
                                    term.Channels[i].State = ((nReadValue >> i) & 0x01) > 0 ? "ON" : "OFF";
                                else
                                    term.Channels[i].State = ((nReadValue >> i) & 0x01) > 0 ? "OFF" : "ON";
                                //m_InputLogMessage += "\t" + term.Channels[i].Id + ":";
                                m_InputLogMessage += term.Channels[i].State == "ON" ? "1" : "0";
                            }
                            if (term.InValue != nReadValue) changed_io = true;
                            term.InValue = nReadValue;
                        }
                        else
                        {
                            normal_update = false;
                            MxpCommLog.WriteLog(string.Format("D232 Read Error {0}-{1}", (UInt32)term.SlaveNo, rv.ToString()));
                        }
                    }
                    else if (Type.Equals(terminal.GetType(), typeof(IoTermMxpDT32K))) //Input 16접점, Output 16접점
                    {
                        IoTermMxpDT32K term = terminal as IoTermMxpDT32K;
                        UInt16 nReadValue = 0;
                        if (AppConfig.Instance.Simulation.IO == false)
                            rv = MXP.IO_ReadWord((UInt32)term.SlaveNo, MXP.MXP_IO_TYPE.IO_IN, (UInt16)0, ref nReadValue);
                        if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        {
                            for (int i = 0; i < term.InChannelCnt; i++)
                            {
                                if (term.Channels[i].IsBContact == false)
                                    term.Channels[i].State = ((nReadValue >> i) & 0x01) > 0 ? "ON" : "OFF";
                                else
                                    term.Channels[i].State = ((nReadValue >> i) & 0x01) > 0 ? "OFF" : "ON";
                                //m_InputLogMessage += "\t" + term.Channels[i].Id + ":";
                                m_InputLogMessage += term.Channels[i].State == "ON" ? "1" : "0";
                            }
                            if (term.InValue != nReadValue) changed_io = true;
                            term.InValue = nReadValue;
                        }
                        else
                        {
                            normal_update = false;
                            MxpCommLog.WriteLog(string.Format("DT32K Read Error {0}-{1}", (UInt32)term.SlaveNo, rv.ToString()));
                        }

                        UInt16 nWriteValue = 0;
                        ushort val;
                        for (int i = 0; i < term.OutChannelCnt; i++)
                        {
                            val = string.Equals(term.Channels[16 + i].State, "ON") ? (ushort)1 : (ushort)0;
                            nWriteValue |= (UInt16)(val << i);
                            //m_OutputLogMessage += "\t" + term.Channels[i].Id + ":";
                            m_OutputLogMessage += val == 1 ? "1" : "0";
                        }

                        if (AppConfig.Instance.Simulation.IO == false)
                            rv = MXP.IO_WriteWord((UInt32)term.SlaveNo, (UInt16)0, nWriteValue);
                        if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        {
                            normal_update = false;
                            MxpCommLog.WriteLog(string.Format("DT32K Write Error {0}-{1}", (UInt32)term.SlaveNo, rv.ToString()));
                        }
                        else
                        {
                            if (term.OutValue != nWriteValue) changed_io = true;
                            term.OutValue = nWriteValue;
                        }
                    }
                    else if (Type.Equals(terminal.GetType(), typeof(IoTermMxpTR32K))) //Output 32접점
                    {
                        IoTermMxpTR32K term = terminal as IoTermMxpTR32K;
                        UInt32 nWriteValue = 0;
                        ushort val;
                        for (int i = 0; i < term.OutChannelCnt; i++)
                        {
                            val = string.Equals(term.Channels[i].State, "ON") ? (ushort)1 : (ushort)0;
                            nWriteValue |= (UInt32)(val << i);
                            //m_OutputLogMessage += "\t" + term.Channels[i].Id + ":";
                            m_OutputLogMessage += val == 1 ? "1" : "0";
                        }

                        if (AppConfig.Instance.Simulation.IO == false)
                            rv = MXP.IO_WriteDword((UInt32)term.SlaveNo, (UInt16)0, nWriteValue);
                        if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        {
                            normal_update = false;
                            MxpCommLog.WriteLog(string.Format("TR32K Write Error {0}-{1}", (UInt32)term.SlaveNo, rv.ToString()));
                        }
                        else
                        {
                            if (term.OutValue != nWriteValue) changed_io = true;
                            term.OutValue = nWriteValue;
                        }
                    }
                    else if (Type.Equals(terminal.GetType(), typeof(IoTermMxpECK64))) //Input 32접점, Output 32접점
                    {
                        IoTermMxpECK64 term = terminal as IoTermMxpECK64;
                        UInt32 nReadValue = 0;
                        if (AppConfig.Instance.Simulation.IO == false)
                            rv = MXP.IO_ReadDword((UInt32)term.SlaveNo, MXP.MXP_IO_TYPE.IO_IN, (UInt16)0, ref nReadValue);
                        if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        {
                            for (int i = 0; i < term.InChannelCnt; i++)
                            {
                                if (term.Channels[i].IsBContact == false)
                                    term.Channels[i].State = ((nReadValue >> i) & 0x01) > 0 ? "ON" : "OFF";
                                else
                                    term.Channels[i].State = ((nReadValue >> i) & 0x01) > 0 ? "OFF" : "ON";
                                //m_InputLogMessage += "\t" + term.Channels[i].Id + ":";
                                m_InputLogMessage += term.Channels[i].State == "ON" ? "1" : "0";
                            }
                            if (term.InValue != nReadValue) changed_io = true;
                            term.InValue = nReadValue;
                        }
                        else
                        {
                            normal_update = false;
                            MxpCommLog.WriteLog(string.Format("ECK64 Read Error {0}-{1}", (UInt32)term.SlaveNo, rv.ToString()));
                        }

                        UInt32 nWriteValue = 0;
                        ushort val;
                        for (int i = 0; i < term.OutChannelCnt; i++)
                        {
                            val = string.Equals(term.Channels[32 + i].State, "ON") ? (ushort)1 : (ushort)0;
                            nWriteValue |= (UInt32)(val << i);
                            //m_OutputLogMessage += "\t" + term.Channels[i].Id + ":";
                            m_OutputLogMessage += val == 1 ? "1" : "0";
                        }

                        if (AppConfig.Instance.Simulation.IO == false)
                            rv = MXP.IO_WriteDword((UInt32)term.SlaveNo, (UInt16)0, nWriteValue);
                        if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        {
                            normal_update = false;
                            MxpCommLog.WriteLog(string.Format("ECK64 Write Error {0}-{1}", (UInt32)term.SlaveNo, rv.ToString()));
                        }
                        else
                        {
                            if (term.OutValue != nWriteValue) changed_io = true;
                            term.OutValue = nWriteValue;
                        }
                    }
                }
                if (normal_update)
                {
                    IoManager.Instance.UpdateRun = true;
                    if (changed_io && AppConfig.Instance.UseIOLogRecord)
                    {
                        int split_length = 1;
                        string msg = string.Empty;
                        msg += "\tINPUT\t";
                        string msg0 = m_InputLogMessage;
                        int count = msg0.Length / split_length;
                        for (int i = 0; i < count; i++) msg0 = msg0.Insert((i + 1) * split_length + i, ",");
                        msg += msg0;
                        msg += "\tOUTPUT\t";
                        msg0 = m_OutputLogMessage;
                        count = msg0.Length / split_length;
                        for (int i = 0; i < count; i++) msg0 = msg0.Insert((i + 1) * split_length + i, ",");
                        msg += msg0;
                        IOMonitorLog.WriteLog(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP UpdateIo : {0}", ex.Message));
                return false;
            }
            return changed_io;
        }

        private void FirstOutputUpdate()
        {
            if (m_FirstOutputUpdate) return;
            if (!m_IsConnected) return;

            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                bool normal_update = true;
                foreach (_IoTerminal terminal in m_Node.Terminals)
                {
                    if (Type.Equals(terminal.GetType(), typeof(IoTermMxpDT32K))) //Input 16접점, Output 16접점
                    {
                        IoTermMxpDT32K term = terminal as IoTermMxpDT32K;
                        UInt16 nReadValue = 0;
                        rv = MXP.IO_ReadWord((UInt32)term.SlaveNo, MXP.MXP_IO_TYPE.IO_OUT, (UInt16)0, ref nReadValue);
                        if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        {
                            for (int i = 0; i < term.OutChannelCnt; i++)
                            {
                                term.Channels[16 + i].State = ((nReadValue >> i) & 0x01) > 0 ? "ON" : "OFF";
                            }
                            term.OutValue = nReadValue;
                        }
                        else
                        {
                            normal_update = false;
                            MxpCommLog.WriteLog(string.Format("DT32K Read State Error {0}-{1}", (UInt32)term.SlaveNo, rv.ToString()));
                        }
                    }
                    else if (Type.Equals(terminal.GetType(), typeof(IoTermMxpTR32K))) //Output 32접점
                    {
                        IoTermMxpTR32K term = terminal as IoTermMxpTR32K;
                        UInt32 nReadValue = 0;
                        rv = MXP.IO_ReadDword((UInt32)term.SlaveNo, MXP.MXP_IO_TYPE.IO_OUT, (UInt16)0, ref nReadValue);
                        if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        {
                            for (int i = 0; i < term.OutChannelCnt; i++)
                            {
                                term.Channels[i].State = ((nReadValue >> i) & 0x01) > 0 ? "ON" : "OFF";
                            }
                            term.OutValue = nReadValue;
                        }
                        else
                        {
                            normal_update = false;
                            MxpCommLog.WriteLog(string.Format("TR32K Read State Error {0}-{1}", (UInt32)term.SlaveNo, rv.ToString()));
                        }
                    }
                }
                if (normal_update) m_FirstOutputUpdate = true;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP UpdateIo : {0}", ex.Message));
            }
        }

        class SeqUpdateIo : XSeqFunc
        {
            MxpIoCtrl m_Ctrl = null;

            public SeqUpdateIo(MxpIoCtrl ctrl)
            {
                this.SeqName = $"SeqUpdateIo";

                m_Ctrl = ctrl;
            }

            public override int Do()
            {
                if (AppConfig.Instance.Simulation.MXP)
                {
                    IoManager.Instance.UpdateRun = true;
                    return -1;
                }

                m_Ctrl.UpdateIo();
                return -1;
            }
        }

        class OnlyOneUpdateIo : XSeqFunc
        {
            MxpIoCtrl m_Ctrl = null;

            public OnlyOneUpdateIo(MxpIoCtrl ctrl)
            {
                this.SeqName = $"OnlyOneUpdateIo";
                m_Ctrl = ctrl;
            }

            public override int Do()
            {
                m_Ctrl.FirstOutputUpdate();
                return -1;
            }
        }
        #endregion
    }
}
