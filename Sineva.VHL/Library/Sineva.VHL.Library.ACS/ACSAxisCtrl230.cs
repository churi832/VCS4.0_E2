using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices; //for COMException class
using ACS.SPiiPlusNET;
using System.Diagnostics;
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.Library.ACS
{
    public class ACSAxisCtrl230
    {
        #region Fields
        private Api AcsChannel = null;

        private AxisBlockACS m_AxisBlock = null;
        private List<ACSAxis230> m_AxisCommands = new List<ACSAxis230>();
        private uint m_Ping = 0;
        private bool OneTimeDefine = true;
        private bool m_OneTimeUpdatePos = false;
        #endregion

        #region Properties
        #endregion

        #region Contructor
        public ACSAxisCtrl230(AxisBlockACS block)
        {
            m_AxisBlock = block;
            // Create new object of class channel
            AcsChannel = new Api();
            if (AcsChannel == null) return;
            AcsChannel.PROGRAMEND += AcsChannel_PROGRAMEND;
            ///////////////////////////////////////////////////////////////////////
            // 이미 연결된 Connection 정보가 있으면 빠져 나와야 한다.
            string ExeName = "GUI";
            string ExeWithVsHost = "GUI.vshost.exe";
            ACSC_CONNECTION_DESC[] ConnectionList = AcsChannel.GetConnectionsList();
            for (int i = 0; i < ConnectionList.Length; i++)
            {
                if (ConnectionList[i].Application.Contains(ExeName)) AcsChannel.TerminateConnection(ConnectionList[i]);
            }
            ////////////////////////////////////////////////////////////////////////
        }

        void AcsChannel_PROGRAMEND(BufferMasks axis)
        {
            string msg = string.Format("ACSAxisCtrl = {0} PROGRAM END", axis);
            ServoLog.WriteLog(msg);
        }

        public bool Initialize()
        {
            bool task_run = false;
            foreach (_Axis axis in m_AxisBlock.Axes)
            {
                if (axis.IsValid)
                {
                    m_AxisCommands.Add(new ACSAxis230(AcsChannel, m_AxisBlock, axis));
                    task_run = true;
                }
            }

            if (AppConfig.Instance.Simulation.ACS && m_AxisBlock.BlockId > 0) task_run = false;

            if (task_run)
                TaskHandler.Instance.RegTask(new TaskACS(this, m_AxisBlock), 10, System.Threading.ThreadPriority.Normal);
            return true;
        }
        #endregion

        #region [ACS Interface Method]
        private uint OpenController()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            uint rv = 0;
            if (!m_AxisBlock.Connected)
            {
                if (AppConfig.Instance.Simulation.ACS)
                    m_AxisBlock.ChannelType = enACSChannelType.Simulator;

                try
                {
                    if (m_AxisBlock.ChannelType == enACSChannelType.Serial)
                    {
                        int index = 0;
                        foreach (PortNo port in Enum.GetValues(typeof(PortNo)))
                        {
                            index++;
                            if (m_AxisBlock.PortNo == port) break;
                        }
                        AcsChannel.OpenCommSerial(index, m_AxisBlock.BaudRate);
                    }
                    else if (m_AxisBlock.ChannelType == enACSChannelType.Network)
                    {
                        AcsChannel.OpenCommEthernetTCP(m_AxisBlock.RemoteIpString, 701);
                    }
                    else if (m_AxisBlock.ChannelType == enACSChannelType.PCI)
                    {
                        AcsChannel.OpenCommPCI(m_AxisBlock.PciSlotNo);
                    }
                    else
                    {
                        AcsChannel.OpenCommSimulator();
                    }
                    m_AxisBlock.Connected = AcsChannel.IsConnected;
                }
                catch (ACSException Ex)
                {
                    if (Ex.ErrorCode == -2147211370) AcsChannel.CloseComm();
                    ExceptionLog.WriteLog(method, string.Format("ACS OpenController : {0}", Ex.Message));
                }
                catch (COMException Ex)
                {
                    if (Ex.ErrorCode == -2147211369) AcsChannel.CloseComm();
                    ExLog(Ex);
                }
            }
            return rv;
        }

        private uint CloseController()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            uint rv = 0;
            if (m_AxisBlock.Connected)
            {
                try
                {
                    AcsChannel.CloseHistoryBuffer();
                    AcsChannel.CloseComm();
                    if (AppConfig.Instance.Simulation.ACS) AcsChannel.CloseSimulator();

                    m_AxisBlock.Connected = false;
                }
                catch (ACSException Ex)
                {
                    ExceptionLog.WriteLog(method, string.Format("ACS CloseController : {0}", Ex.Message));
                }
                catch (COMException Ex)
                {
                    ExLog(Ex);
                }
            }
            return rv;
        }

        private void UpdateController()
        {
            //ACS SPiiPlus User Mode Driver 사용 디버깅 시 주석처리
            //if (AppConfig.Instance.Simulation.ACS) return;

            UpdateCommunicationState(); // Controller Run Check
            UpdateAxisState();
            OpenController(); //Controller Open Check
        }

        /*
         *ACS Ping Program
         *1. GLOBAL INT HEART_BIT
         *2. WHILE 1
         *3.   HEART_BIT = 1
         *4.   WAIT 497;
         *5.   HEART_BIT = 0
         *6.   WAIT 498;
         *7. END
         *8. STOP
         */
        private void UpdateCommunicationState()
        {
            object pWait = 0;
            if (m_AxisBlock.Connected == false) return;

            try
            {
                string home_variable = string.Format("HomeFlag");
                string heart_bit = string.Format("HEART_BIT");

                if (OneTimeDefine)
                {
                    OneTimeDefine = false;
                    AcsChannel.ClearVariables();
                    AcsChannel.DeclareVariable(AcsplVariableType.ACSC_INT_TYPE, heart_bit);

                    //if (AppConfig.Instance.Simulation.ACS)
                    //{
                    //    AcsChannel.DeclareVariable(AcsplVariableType.ACSC_INT_TYPE, home_variable);
                    //    Int32[] home_flag = new Int32[64];
                    //    for (int i = 0; i < 64; i++) home_flag[i] = 1;
                    //    AcsChannel.WriteVariable(home_flag, home_variable, ProgramBuffer.ACSC_NONE, -1, -1, -1, -1); 
                    //}
                }

                int timeout = 30000;
                {
                    m_Ping++;
                    if (m_Ping >= uint.MaxValue - 1) m_Ping = 0;
                    object result = null;
                    try
                    {
                        AcsChannel.WriteVariable(m_Ping, heart_bit, ProgramBuffer.ACSC_NONE, -1, -1, -1, -1);
                        result = AcsChannel.ReadVariable(heart_bit, ProgramBuffer.ACSC_NONE, -1, -1, -1, -1);
                    }
                    catch (ACSException Ex)
                    {
                        if (Ex.ErrorCode == 1064)
                        {
                            AcsChannel.DeclareVariable(AcsplVariableType.ACSC_INT_TYPE, heart_bit);
                            Thread.Sleep(10);
                        }
                    }

                    if (result != null)
                    {
                        string _ping = string.Format("{0}", result);
                        if (_ping != m_Ping.ToString())
                        {
                            CloseController();
                            m_AxisBlock.Connected = false;
                        }
                    }
                    Thread.Sleep(1);
                }

                if (true)
                {
                    object obj_homeflag = null;
                    try
                    {
                        obj_homeflag = AcsChannel.ReadVariable(home_variable, ProgramBuffer.ACSC_NONE, -1, -1, -1, -1);
                    }
                    catch (ACSException Ex)
                    {
                        if (Ex.ErrorCode == 1064) // Not define "HomeFlag"
                        {
                            if (AppConfig.Instance.Simulation.ACS)
                            {
                                string variable = string.Format("HomeFlag{0}(64)", m_AxisBlock.BlockId);
                                AcsChannel.DeclareVariable(AcsplVariableType.ACSC_INT_TYPE, variable);

                            }
                            Thread.Sleep(10);
                            return;
                        }
                    }

                    Int32[] home_state = null;
                    if (AppConfig.Instance.Simulation.ACS && obj_homeflag != null)
                    {
                        if (obj_homeflag.GetType() == typeof(Int32[]))
                            home_state = (Int32[])obj_homeflag;
                        else home_state = new Int32[64];
                        for (int i = 0; i < m_AxisCommands.Count; i++)
                        {
                            _Axis axis = m_AxisCommands[i].GetAxis();
                            int id = axis.NodeId;
                            if (m_AxisCommands[i].CommandHome) home_state[id] = 1;

                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(axis.AxisId);
                            if (axis.GantryType && axis.NodeId != axis.SlaveNodeId) //Gantry Master 일 경우
                            {
                                int slave_id = axis.SlaveNodeId;
                                if (Convert.ToBoolean(command & enAxisOutFlag.ServoOn)) m_AxisCommands[slave_id].Enable();
                                if (Convert.ToBoolean(command & enAxisOutFlag.ServoOff)) m_AxisCommands[slave_id].Disable();
                                m_AxisCommands[slave_id].MotorState = m_AxisCommands[i].MotorState;
                                home_state[slave_id] = home_state[id];
                            }
                        }
                        AcsChannel.WriteVariable(home_state, home_variable, ProgramBuffer.ACSC_NONE, -1, -1, -1, -1);
                    }
                    else
                    {
                        home_state = (Int32[])obj_homeflag;
                    }

                    int index = 0;
                    foreach (ACSAxis230 axis in m_AxisCommands)
                    {
                        // Home Bit Monitor
                        index = axis.GetAxisId();
                        if (home_state != null && index < home_state.Length)
                        {
                            bool home = string.Format("{0}", home_state[index]) == "1" ? true : false;
                            axis.SetHomeBit(home);
                        }
                    }
                    Thread.Sleep(1);
                }
            }
            catch (ACSException Ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                if (Ex.ErrorCode == -2147211370) // Communication Error 발생시....
                {
                    CloseController();
                    m_AxisBlock.Connected = false;
                }
                ExceptionLog.WriteLog(method, string.Format("ACS{0} UpdateCommunicationState : {1}", m_AxisBlock.BlockId, Ex.Message));
            }
            catch (COMException Ex)
            {
                if (Ex.ErrorCode == -2147211370) // Communication Error 발생시....
                {
                    CloseController();
                    m_AxisBlock.Connected = false;
                }
                ExLog(Ex);
            }
        }

        private void UpdateAxisState()
        {
            object pWait = 0;
            if (m_AxisBlock.Connected == false) return;

            try
            {
                ////////////////////////////////////////////////////////////////////////////////////// ==> 한번에 read variable 하는 방법으로 바꾸자...
                string torque_variable = string.Format("RMS");
                object obj_torque = null;
                try
                {
                    obj_torque = AcsChannel.ReadVariable(torque_variable, ProgramBuffer.ACSC_NONE, -1, -1, -1, -1);
                }
                catch (ACSException Ex)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    ExceptionLog.WriteLog(method,string.Format("ACS UpdateAxisState[RMS] : {0}", Ex.Message));
                    Thread.Sleep(10);
                }
                ////////////////////////////////////////////////////////////////////////////////////// ==> 한번에 read variable 하는 방법으로 바꾸자...
                string pos_variable = string.Format("FPOS");
                object obj_position = null;
                try
                {
                    obj_position = AcsChannel.ReadVariable(pos_variable, ProgramBuffer.ACSC_NONE, -1, -1, -1, -1);
                }
                catch (ACSException Ex)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    ExceptionLog.WriteLog(method, string.Format("ACS UpdateAxisState[FPOS] : {0}", Ex.Message));
                    Thread.Sleep(10);
                }
                ////////////////////////////////////////////////////////////////////////////////////// ==> 한번에 read variable 하는 방법으로 바꾸자...
                string vel_variable = string.Format("RVEL");
                object obj_speed = null;
                try
                {
                    obj_speed = AcsChannel.ReadVariable(vel_variable, ProgramBuffer.ACSC_NONE, -1, -1, -1, -1);
                }
                catch (ACSException Ex)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    ExceptionLog.WriteLog(method, string.Format("ACS UpdateAxisState[RVEL] : {0}", Ex.Message));
                    Thread.Sleep(10);
                }
                ////////////////////////////////////////////////////////////////////////////////////// ==> 한번에 read variable 하는 방법으로 바꾸자...
                string mst_variable = string.Format("MST");
                object obj_mst = null;
                try
                {
                    obj_mst = AcsChannel.ReadVariable(mst_variable, ProgramBuffer.ACSC_NONE, -1, -1, -1, -1);
                }
                catch (ACSException Ex)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    ExceptionLog.WriteLog(method, string.Format("ACS UpdateAxisState[MST] : {0}", Ex.Message));
                    Thread.Sleep(10);
                }
                ////////////////////////////////////////////////////////////////////////////////////// ==> 한번에 read variable 하는 방법으로 바꾸자...
                //string ast_variable = string.Format("AST");
                //object obj_ast = null;
                //try
                //{
                //    obj_ast = AcsChannel.ReadVariable(ast_variable, ProgramBuffer.ACSC_NONE, -1, -1, -1, -1);
                //}
                //catch (ACSException Ex)
                //{
                //    ExceptionLog.WriteLog(string.Format("ACS UpdateAxisState[AST] : {0}", Ex.Message));
                //    Thread.Sleep(10);
                //}
                ////////////////////////////////////////////////////////////////////////////////////// ==> 한번에 read variable 하는 방법으로 바꾸자...Motion Move : >0, Motion Stop : == 0
                //string gttime_variable = string.Format("GTTIME");
                //object obj_gttime = null;
                //try
                //{
                //    obj_gttime = AcsChannel.ReadVariable(gttime_variable, ProgramBuffer.ACSC_NONE, -1, -1, -1, -1);
                //}
                //catch (ACSException Ex)
                //{
                //    ExceptionLog.WriteLog(string.Format("ACS UpdateAxisState[GTTIME] : {0}", Ex.Message));
                //    Thread.Sleep(10);
                //}
                ////////////////////////////////////////////////////////////////////////////////////// ==> 한번에 read variable 하는 방법으로 바꾸자...
                string fault_variable = string.Format("FAULT");
                object obj_fault = null;
                try
                {
                    obj_fault = AcsChannel.ReadVariable(fault_variable, ProgramBuffer.ACSC_NONE, -1, -1, -1, -1);
                }
                catch (ACSException Ex)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    ExceptionLog.WriteLog(method, string.Format("ACS UpdateAxisState[FAULT] : {0}", Ex.Message));
                    Thread.Sleep(10);
                }
                ////////////////////////////////////////////////////////////////////////////////////// ==> 한번에 read variable 하는 방법으로 바꾸자...
                string merr_variable = string.Format("MERR");
                object obj_merr = null;
                try
                {
                    obj_merr = AcsChannel.ReadVariable(merr_variable, ProgramBuffer.ACSC_NONE, -1, -1, -1, -1);
                }
                catch (ACSException Ex)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    ExceptionLog.WriteLog(method, string.Format("ACS UpdateAxisState[MERR] : {0}", Ex.Message));
                    Thread.Sleep(10);
                }
                ////////////////////////////////////////////////////////////////////////////////////// ==> 한번에 read variable 하는 방법으로 바꾸자...
                string pst_variable = string.Format("PST");
                object obj_pst = null;
                try
                {
                    obj_pst = AcsChannel.ReadVariable(pst_variable, ProgramBuffer.ACSC_NONE, -1, -1, -1, -1);
                }
                catch (ACSException Ex)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    ExceptionLog.WriteLog(method, string.Format("ACS UpdateAxisState[PST] : {0}", Ex.Message));
                    Thread.Sleep(10);
                }

                ////////////////////////////////////////////////////////////////////////////////////// ==> 한번에 read variable 하는 방법으로 바꾸자...
                if (obj_position == null || obj_speed == null || obj_torque == null || obj_mst == null || /*obj_ast == null ||*/ obj_fault == null || obj_merr == null || obj_pst == null) return;
                double[] position_state = (double[])obj_position;
                double[] speed_state = (double[])obj_speed;
                double[] torque_state = (double[])obj_torque;
                int[] mst_state = (int[])obj_mst;
                //int[] ast_state = (int[])obj_ast;
                int[] fault_state = (int[])obj_fault;
                int[] merr_state = (int[])obj_merr;
                int[] pst_state = (int[])obj_pst;

                int nodeId = 0;
                int origin_nodeId = 0;
                int slave_nodeId = 0;
                foreach (ACSAxis230 axis in m_AxisCommands)
                {
                    // Home Bit Monitor
                    nodeId = axis.GetAxisId();
                    origin_nodeId = axis.GetAxis().MasterNodeId;
                    slave_nodeId = axis.GetAxis().SlaveNodeId;
                    if (position_state != null && nodeId < position_state.Length)
                    {
                        axis.CurPosition = position_state[nodeId];
                        if (!m_OneTimeUpdatePos)
                        {
                            _Axis ax = axis.GetAxis();
                            ax.TargetPos = axis.CurPosition;
                        }
                        if (AppConfig.Instance.Simulation.ACS) axis.CurPosition = m_AxisBlock.GetTargetPosition(axis.GetAxis().AxisId);
                    }
                    if (speed_state != null && nodeId < speed_state.Length)
                    {
                        axis.CurSpeed = speed_state[nodeId];
                        if (AppConfig.Instance.Simulation.ACS) axis.CurSpeed = m_AxisBlock.GetTargetSpeed(axis.GetAxis().AxisId);
                    }
                    if (torque_state != null && nodeId < torque_state.Length)
                    {
                        axis.CurTorque = torque_state[nodeId] / 10000.0f;
                    }
                    if (mst_state != null && nodeId < mst_state.Length)
                    {
                        if (axis.GetAxis().GantryType)
                        {
                            // Slave Node에도 상태를 동일하게 가져가자...
                            axis.MotorState = (MotorStates)mst_state[nodeId];
                            if (nodeId != slave_nodeId && slave_nodeId < mst_state.Length) mst_state[slave_nodeId] = mst_state[nodeId];
                        }
                        else
                        {
                            axis.MotorState = (MotorStates)mst_state[nodeId];
                        }
                    }
                    //if (ast_state != null && nodeId < ast_state.Length)
                    //{
                    //    axis.AxisState = (AxisStates)ast_state[nodeId];
                    //}
                    if (fault_state != null && nodeId < fault_state.Length)
                    {
                        axis.AxisSafty = (SafetyControlMasks)fault_state[nodeId];
                        if (AppConfig.Instance.Simulation.ACS)
                        {
                            if (axis.AxisSafty.HasFlag(SafetyControlMasks.ACSC_SAFETY_LL)) axis.AxisSafty ^= SafetyControlMasks.ACSC_SAFETY_LL;
                            if (axis.AxisSafty.HasFlag(SafetyControlMasks.ACSC_SAFETY_RL)) axis.AxisSafty ^= SafetyControlMasks.ACSC_SAFETY_RL;
                        }
                    }
                    if (merr_state != null && nodeId < merr_state.Length)
                    {
                        axis.AlarmCode = merr_state[nodeId];
                    }
                    if (pst_state != null && origin_nodeId < pst_state.Length)
                    {
                        axis.ProgramState = (ProgramStates)pst_state[origin_nodeId];
                    }
                }
                m_OneTimeUpdatePos = true;

                Thread.Sleep(10);
            }
            catch (ACSException Ex)
            {
                if (Ex.ErrorCode == -2147211370) // Communication Error 발생시....
                {
                    CloseController();
                    m_AxisBlock.Connected = false;
                }
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, string.Format("ACS{0} UpdateAxisState : {1}", m_AxisBlock.BlockId, Ex.Message));
            }
            catch (COMException Ex)
            {
                if (Ex.ErrorCode == -2147211370) // Communication Error 발생시....
                {
                    CloseController();
                    m_AxisBlock.Connected = false;
                }
                ExLog(Ex);
            }
        }

        private void ExLog(COMException Ex)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            string msg = string.Format("Error from {0}, Message : {1}, HRESULT : {2:X}", Ex.Source, Ex.Message, Ex.ErrorCode);
            ExceptionLog.WriteLog(method, msg);
        }
        #endregion

        #region Thread
        public class TaskACS : XSequence
        {
            ACSAxisCtrl230 m_Control = null;
            AxisBlockACS m_Block = null;

            public TaskACS(ACSAxisCtrl230 control, AxisBlockACS block)
            {
                m_Control = control;
                m_Block = block;
                RegSeq(new SeqACSConrol(control));
            }

            protected override void ExitRoutine()
            {
                m_Control.CloseController();
            }
        }

        class SeqACSConrol : XSeqFunc
        {
            ACSAxisCtrl230 m_Control = null;
            public SeqACSConrol(ACSAxisCtrl230 ctrl)
            {
                this.SeqName = $"SeqACSConrol";
                m_Control = ctrl;
            }

            public override int Do()
            {
                m_Control.UpdateController();
                return -1;
            }
        }
        #endregion
    }
}
