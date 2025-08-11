using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using static Sineva.VHL.Library.MXP.MxpManager;

namespace Sineva.VHL.Library.MXP
{
    public class MxpAxisCtrl
    {
        #region Fields
        private AxisBlockMXP m_AxisBlock = null;
        private List<MxpAxis> m_AxisList = new List<MxpAxis>();
        private static object m_Lock = new object();
        #endregion

        #region Properties
        public bool IsConnected 
        {
            get 
            {
                bool connected = false;
                if (m_AxisBlock != null) { connected = m_AxisBlock.Connected; }
                return connected;
            }
            set
            {
                if (m_AxisBlock != null) 
                    m_AxisBlock.Connected = value;
            }
        }
        public bool IsHeartBitOk
        {
            get 
            {
                bool heart_bit_ok = true;
                if (m_AxisBlock != null) { heart_bit_ok = m_AxisBlock.HeartBitOk; }
                return heart_bit_ok;
            }
            set
            {
                if (m_AxisBlock != null)
                    m_AxisBlock.HeartBitOk = value;
            }
        }
        public bool IsMxpHeartBitError
        {
            get
            {
                bool heart_bit_error = true;
                if (m_AxisBlock != null) { heart_bit_error = m_AxisBlock.IsHeartBitError; }
                return heart_bit_error;
            }
            set
            {
                if (m_AxisBlock != null)
                    m_AxisBlock.IsHeartBitError = value;
            }
        }
        public List<MxpAxis> AxisList { get => m_AxisList; set => m_AxisList = value; }
        #endregion

        #region Constructor
        public MxpAxisCtrl(AxisBlockMXP block) 
        {
            m_AxisBlock = block;
        }
        public bool Initialize(bool thread_create = true)
        {
            foreach (_Axis axis in m_AxisBlock.Axes)
            {
                if (axis.IsValid)
                {
                    AxisList.Add(new MxpAxis(this, m_AxisBlock, axis, thread_create));
                }
            }            

            if (thread_create)
            {
                TaskHandler.Instance.RegTask(new TaskHeartBit(this), 10, System.Threading.ThreadPriority.Normal);
                TaskHandler.Instance.RegTask(new TaskMxpPlcControl(this), 10, System.Threading.ThreadPriority.Normal);
            }
            return true;
        }
        public bool InitializePlc()
        {
            #region "Code"
            try
            {
                MXP.MXP_FUNCTION_STATUS_RESULT rv1;
                bool result = true;

                #region AXIS 
                rv1 = MXP.PLC_WriteBuffer((UInt32)enDriveParameter.MST_AXIS_NO, 4, BitConverter.GetBytes((UInt32)m_AxisBlock.MasterNodeId));
                result &= rv1 == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
                if (!result) MxpCommLog.WriteLog(string.Format($"Drive PLC Parameter Setting Failed({rv1}) : {enDriveParameter.MST_AXIS_NO} , MST_AXIS_NO:{m_AxisBlock.MasterNodeId}"));

                rv1 = MXP.PLC_WriteBuffer((UInt32)enDriveParameter.SLV_AXIS_NO, 4, BitConverter.GetBytes((UInt32)m_AxisBlock.SlaveNodeId));
                result &= rv1 == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
                if (!result) MxpCommLog.WriteLog(string.Format($"Drive PLC Parameter Setting Failed({rv1}) : {enDriveParameter.SLV_AXIS_NO} , SLV_AXIS_NO:{m_AxisBlock.SlaveNodeId}"));

                rv1 = MXP.PLC_WriteBuffer((UInt32)enDriveParameter.MST_SLAVE_NO, 4, BitConverter.GetBytes((UInt32)m_AxisBlock.MasterMXConfigratorSlaveNo));
                result &= rv1 == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
                if (!result) MxpCommLog.WriteLog(string.Format($"Drive PLC Parameter Setting Failed({rv1}) : {enDriveParameter.MST_SLAVE_NO} , MST_SLAVE_NO:{m_AxisBlock.MasterMXConfigratorSlaveNo}"));

                rv1 = MXP.PLC_WriteBuffer((UInt32)enDriveParameter.SLV_SLAVE_NO, 4, BitConverter.GetBytes((UInt32)m_AxisBlock.SlaveMXConfigratorSlaveNo));
                result &= rv1 == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
                if (!result) MxpCommLog.WriteLog(string.Format($"Drive PLC Parameter Setting Failed({rv1}) : {enDriveParameter.MST_SLAVE_NO} , SLV_SLAVE_NO:{m_AxisBlock.SlaveMXConfigratorSlaveNo}"));

                rv1 = MXP.PLC_WriteBuffer((UInt32)enDriveParameter.BCR_LEFT_SLAVE_NO, 4, BitConverter.GetBytes((UInt32)m_AxisBlock.LeftBcrSlaveNo));
                result &= rv1 == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
                if (!result) MxpCommLog.WriteLog(string.Format($"Drive PLC Parameter Setting Failed({rv1}) : {enDriveParameter.BCR_LEFT_SLAVE_NO} , BCR_LEFT_SLAVE_NO:{m_AxisBlock.LeftBcrSlaveNo}"));

                rv1 = MXP.PLC_WriteBuffer((UInt32)enDriveParameter.BCR_RIGHT_SLAVE_NO, 4, BitConverter.GetBytes((UInt32)m_AxisBlock.RightBcrSlaveNo));
                result &= rv1 == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
                if (!result) MxpCommLog.WriteLog(string.Format($"Drive PLC Parameter Setting Failed({rv1}) : {enDriveParameter.BCR_RIGHT_SLAVE_NO} , BCR_RIGHT_SLAVE_NO:{m_AxisBlock.RightBcrSlaveNo}"));
                #endregion

                return result;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return false;
            }
            #endregion
        }
        #endregion

        #region Thread
        public class TaskHeartBit : XSequence
        {
            MxpAxisCtrl m_Control = null;
            public TaskHeartBit(MxpAxisCtrl control)
            {
                m_Control = control;
                RegSeq(new SeqMxpHeartBitCheck(control));
            }
        }
        class SeqMxpHeartBitCheck : XSeqFunc
        {
            private MxpAxisCtrl m_Control = null;
            private readonly uint mxpHeartBitCountAddress = 5200;
            private readonly uint pcHeartBitCountAddress = 5196;
            private readonly uint pcHeartBitUsingAddress = 5208;
            private readonly uint pcHeartBitStopDistanceAddress = 5204;
            private readonly uint pdHeartBitTimeout = 5244;
            private readonly uint mxpHeartBitErrorOccur = 5216;
            private int HeartBitCount = 0;
            private bool HeartBitUsing = true;
            private uint LoopTicks = 0;
            private int HeartBitError = 0;
            private bool FirstOneTime = true;

            public SeqMxpHeartBitCheck(MxpAxisCtrl ctrl)
            {
                this.SeqName = $"SeqMxpHeartBitCheck";
                m_Control = ctrl;
            }
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            HeartBitUsing = m_Control.m_AxisBlock.HeartBitUsing;
                            if (m_Control.IsConnected && HeartBitUsing)
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else
                            {
                                m_Control.IsHeartBitOk = true;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (FirstOneTime)
                            {
                                if (XFunc.GetTickCount() - StartTicks < 5 * 1000) break;
                            }

                            HeartBitCount++;
                            MXP.MXP_FUNCTION_STATUS_RESULT nResult;
                            nResult = MXP.PLC_WriteInt32(pcHeartBitUsingAddress, HeartBitUsing ? 1 : 0, 0); /// Heart Bit Using
                            nResult = MXP.PLC_WriteFloat(pcHeartBitStopDistanceAddress, 300.0f, 0); /// emergency stop case = 0
                            nResult = MXP.PLC_WriteFloat(pdHeartBitTimeout, (float)m_Control.m_AxisBlock.HeartBitTimeout, 0); /// HeartBit Interlock Time
                            nResult = MXP.PLC_WriteInt32(pcHeartBitCountAddress, HeartBitCount, 0); /// PC->MXP
                            LoopTicks = XFunc.GetTickCount();
                            seqNo = 20;

                            //string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                            //System.Diagnostics.Debug.WriteLine($"{time} HEARTBIT COUNT Write : {HeartBitCount}");
                        }
                        break;

                    case 20:
                        {
                            if (XFunc.GetTickCount() - LoopTicks < 100) break;
                            MXP.MXP_FUNCTION_STATUS_RESULT nResult;

                            Int32 heart_bit_count = 0;
                            nResult = MXP.PLC_ReadInt32(mxpHeartBitCountAddress, ref heart_bit_count); /// MXP->PC
                            if (HeartBitCount <= heart_bit_count)
                            {
                                HeartBitCount = heart_bit_count;
                                m_Control.IsHeartBitOk = true;
                                seqNo = 0;

                                //string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                                //System.Diagnostics.Debug.WriteLine($"{time} heart_bit_count Read : {heart_bit_count}");
                            }
                            else if (XFunc.GetTickCount() - StartTicks > m_Control.m_AxisBlock.HeartBitTimeout && AppConfig.Instance.Simulation.MY_DEBUG == false)
                            {
                                // MXP 점검 필요
                                MxpCommLog.WriteLog(string.Format("HeartBit NG[{0}!={1}]", HeartBitCount, heart_bit_count));
                                m_Control.IsHeartBitOk = false;
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;

                                //string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                                //System.Diagnostics.Debug.WriteLine($"{time} heart_bit_count Read : {HeartBitCount}, {heart_bit_count}");
                            }

                            Int32 heart_bit_error = 0;
                            nResult = MXP.PLC_ReadInt32(mxpHeartBitErrorOccur, ref heart_bit_error); /// MXP->PC
                            if (FirstOneTime) { HeartBitError = heart_bit_error; FirstOneTime = false; }

                            bool heartBitError = HeartBitError >= 0;
                            heartBitError &= HeartBitError != heart_bit_error;
                            if (heartBitError && !m_Control.IsMxpHeartBitError)
                            {
                                MxpCommLog.WriteLog(string.Format("HeartBitError Occur[{0}!={1}]", HeartBitError, heart_bit_error));
                                HeartBitError = heart_bit_error;
                                m_Control.IsMxpHeartBitError = true;

                                //string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                                //System.Diagnostics.Debug.WriteLine($"{time} HeartBitError Read : {HeartBitError}");
                            }
                            else if (m_Control.IsHeartBitOk && m_Control.IsMxpHeartBitError)
                            {
                                MxpCommLog.WriteLog(string.Format("HeartBitError Reset[{0}!={1}]", HeartBitError, heart_bit_error));
                                m_Control.IsMxpHeartBitError = false;
                            }
                        }
                        break;

                    case 1000:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;
                            seqNo = 0;
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }
        }
        public class TaskMxpPlcControl : XSequence
        {
            MxpAxisCtrl m_Control = null;
            public TaskMxpPlcControl(MxpAxisCtrl control)
            {
                m_Control = control;
                RegSeq(new SeqMxpPlcCommunication(control));
            }
        }
        class SeqMxpPlcCommunication : XSeqFunc
        {
            private MxpAxisCtrl m_Control = null;
            private bool CornerControlUsing = false;
            private uint pcCornerControlStartAddress = 5400;
            private uint mxpCornerStateStartAddress = 5401;
            private uint pcCornerFunctionStartAddress = 5404;

            private int pcCornerControlSize = 1;
            private int mxpCornerStateSize = 3;
            private int pcCornerFunctionSize = 176;
            private bool FirstOneTime = true;

            public SeqMxpPlcCommunication(MxpAxisCtrl ctrl)
            {
                this.SeqName = $"SeqMxpPlcCommunication";

                m_Control = ctrl;
                pcCornerControlStartAddress = m_Control.m_AxisBlock.CornerControlStartAddress;
                pcCornerFunctionStartAddress = m_Control.m_AxisBlock.CornerFunctionStartAddress;
                mxpCornerStateStartAddress = m_Control.m_AxisBlock.CornerStateStartAddress;

                int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(MxpPlcCornerControl));
                if (size != 0) pcCornerControlSize = size;
                size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(MxpPlcCornerFunction));
                if (size != 0) pcCornerFunctionSize = size;
                size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(MxpPlcCornerState));
                if (size != 0) mxpCornerStateSize = size;
            }
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            CornerControlUsing = m_Control.m_AxisBlock.CornerControlUsing;
                            if (m_Control.IsConnected && CornerControlUsing)
                            {
                                if (FirstOneTime)
                                {
                                    FirstOneTime = false;
                                    FirstAllRead();
                                }
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 10)
                            {
                                WriteBufferToPLC();
                                ReadBufferFromPLC();
                                seqNo = 0;
                            }
                        }
                        break;

                    case 1000:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;
                            seqNo = 0;
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }

            public void FirstAllRead()
            {
                try
                {
                    // Update MxpPlcCornerControl, MxpPlcCornerFunction, MxpPlcCornerState
                    MXP.MXP_FUNCTION_STATUS_RESULT nResult;
                    UInt32 nAddress = 0;
                    Byte[] nBytes0 = new Byte[pcCornerControlSize];
                    uint size = (uint)pcCornerControlSize;
                    nAddress = pcCornerControlStartAddress;
                    nResult = MXP.PLC_ReadBuffer(nAddress, size, ref nBytes0[0]);
                    if (nResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                    {
                        MxpPlcCornerControl con = (MxpPlcCornerControl)XFunc.ByteToStructure(nBytes0, typeof(MxpPlcCornerControl));
                        m_Control.m_AxisBlock.SetMxpPlcControl(con);
                    }

                    Byte[] nBytes1 = new Byte[mxpCornerStateSize];
                    size = (uint)mxpCornerStateSize;
                    nAddress = mxpCornerStateStartAddress;
                    nResult = MXP.PLC_ReadBuffer(nAddress, size, ref nBytes1[0]);
                    if (nResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                    {
                        MxpPlcCornerState state = (MxpPlcCornerState)XFunc.ByteToStructure(nBytes1, typeof(MxpPlcCornerState));
                        m_Control.m_AxisBlock.SetMxpPlcState(state);
                    }

                    Byte[] nBytes2 = new Byte[pcCornerFunctionSize];
                    size = (uint)pcCornerFunctionSize;
                    nAddress = pcCornerFunctionStartAddress;
                    nResult = MXP.PLC_ReadBuffer(nAddress, size, ref nBytes2[0]);
                    if (nResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                    {
                        MxpPlcCornerFunction func = (MxpPlcCornerFunction)XFunc.ByteToStructure(nBytes2, typeof(MxpPlcCornerFunction));
                        m_Control.m_AxisBlock.SetMxpPlcFunction(func);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }

            public void WriteBufferToPLC()
            {
                try
                {
                    MXP.MXP_FUNCTION_STATUS_RESULT nResult;
                    UInt32 nAddress = 0;
                    Byte[] nBytes0 = m_Control.m_AxisBlock.GetMxpPlcControl();
                    uint size = (uint)pcCornerControlSize;
                    nAddress = pcCornerControlStartAddress;
                    nResult = MXP.PLC_WriteBuffer(nAddress, size, nBytes0);
                    if (nResult != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR && AppConfig.Instance.Simulation.MY_DEBUG)
                    {
                        string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                        System.Diagnostics.Debug.WriteLine($"{time} MxpPlcCornerControl Write : Error ({nResult})");
                    }

                    Byte[] nBytes1 = m_Control.m_AxisBlock.GetMxpPlcFunction();
                    size = (uint)pcCornerFunctionSize;
                    nAddress = pcCornerFunctionStartAddress;
                    nResult = MXP.PLC_WriteBuffer(nAddress, size, nBytes1);
                    if (nResult != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR && AppConfig.Instance.Simulation.MY_DEBUG)
                    {
                        string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                        System.Diagnostics.Debug.WriteLine($"{time} MxpPlcCornerControl Write : Error ({nResult})");
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }

            public void ReadBufferFromPLC()
            {
                try
                {
                    MXP.MXP_FUNCTION_STATUS_RESULT nResult;
                    UInt32 nAddress = 0;
                    Byte[] nBytes = new Byte[mxpCornerStateSize];
                    uint size = (uint)mxpCornerStateSize;
                    nAddress = mxpCornerStateStartAddress;
                    nResult = MXP.PLC_ReadBuffer(nAddress, size, ref nBytes[0]);
                    if (nResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                    {
                        MxpPlcCornerState state = (MxpPlcCornerState)XFunc.ByteToStructure(nBytes, typeof(MxpPlcCornerState));
                        m_Control.m_AxisBlock.SetMxpPlcState(state);
                    }
                    else if (AppConfig.Instance.Simulation.MY_DEBUG)
                    {
                        string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                        System.Diagnostics.Debug.WriteLine($"{time} MxpPlcCornerControl Write : Error ({nResult})");
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }

        }
        #endregion
    }
}
