using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library.IO;
using static Sineva.VHL.Library.MXP.MXP;

namespace Sineva.VHL.Library.MXP
{
    public class MxpManager
    {
        public static readonly MxpManager Instance = new MxpManager();

        #region Fields
        private List<MxpAxisCtrl> m_AxisCtrls = new List<MxpAxisCtrl>();
        private List<MxpIoCtrl> m_IoCtrls = new List<MxpIoCtrl>();

        private MXP.KERNEL_STATUS m_ControlState = new MXP.KERNEL_STATUS();
        private MXP.MXP_ONLINE_STATE m_EtherCatOnlineMode = MXP.MXP_ONLINE_STATE.ONLINE_STATE_NONE;
        private Byte m_EtherCatReady = 0;
        private Byte[] m_SlaveReady = null;

        private bool m_Initialized = false;
        #endregion

        #region Properties
        public MXP.KERNEL_STATUS ControlState { get => m_ControlState; set => m_ControlState = value; }
        public MXP.MXP_ONLINE_STATE EtherCatOnlineMode { get => m_EtherCatOnlineMode; set => m_EtherCatOnlineMode = value; }
        public byte EtherCatReady { get => m_EtherCatReady; set => m_EtherCatReady = value; }

        public bool IsConnected
        {
            set
            {
                foreach (MxpAxisCtrl ctrl in m_AxisCtrls) ctrl.IsConnected = value;
                foreach (MxpIoCtrl ctrl in m_IoCtrls) ctrl.IsConnected = value;
            }
        }
        #endregion

        #region Constructor
        public MxpManager() 
        { 
        }
        public bool Initialize(bool thread_create = true)
        {
            if (m_Initialized) return true;

            bool rv = true;
            rv &= InitializeAxis(thread_create);
            rv &= InitializeIo(thread_create);
            int slave_count = m_AxisCtrls.Count + m_IoCtrls.Count;
            m_SlaveReady = new byte[slave_count];

            if (rv)
            {
                TaskHandler.Instance.RegTask(new TaskMxpInterface(this), 10, System.Threading.ThreadPriority.Normal);

                m_Initialized = true;
            }
            return rv;
        }
        public bool InitializeAxis(bool thread_create = true)
        {
            bool rv = true;
            foreach (_AxisBlock block in ServoManager.Instance.AxisBlocks)
            {
                if (block.ControlFamily == ServoControlFamily.MXP)
                {
                    (block as AxisBlockMXP).Initialize(block.ControlFamily);
                    m_AxisCtrls.Add(new MxpAxisCtrl(block as AxisBlockMXP));
                }
            }

            foreach (MxpAxisCtrl ctrl in m_AxisCtrls)
            {
                rv &= ctrl.Initialize(thread_create);
            }
            return true;
        }
        public bool InitializeIo(bool thread_create = true)
        {
            bool rv = true;
            foreach (_IoNode ioNode in IoManager.Instance.Nodes)
            {
                if (ioNode.GetType().ToString() == typeof(IoNodeMxpEC).ToString())
                {
                    m_IoCtrls.Add(new MxpIoCtrl((IoNodeMxpEC)ioNode));
                }
            }

            foreach (MxpIoCtrl ioCtrl in m_IoCtrls)
            {
                rv &= ioCtrl.InitializeIo();
            }

            return rv;
        }
        public bool AllServoStop()
        {
            bool rv = true;

            foreach (MxpAxisCtrl ctrl in m_AxisCtrls)
            {
                foreach (MxpAxis axis in ctrl.AxisList)
                {
                    //if (axis.AxisState.discreteMotion == 1) // 이동 동작 중 override로 멈춘경우...stop 명령을 먹지 않는다.
                    //    axis.Disable();
                    //else axis.Stop();
                    axis.Stop(true);
                    //axis.VelocityOverride(1.0f, 100.0f, true); //초기화
                }
            }
            return rv;
        }
        public void UpdateStateMsg()
        {
            try
            {
                string msg = string.Format("{0}|{1}|{2}", ControlState, m_EtherCatOnlineMode, EtherCatReady);
                foreach (_AxisBlock block in ServoManager.Instance.AxisBlocks) block.BlockStateMsg = msg;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }
        #endregion

        #region [MXP Interface Method]
        private MXP.MXP_FUNCTION_STATUS_RESULT GetMxpSysStatus()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT nResult = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                nResult = MXP.SYS_GetStatus(ref m_ControlState);
                if (nResult != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    MxpCommLog.WriteLog(string.Format("GetMxpSysStatus nResult = {0}", nResult));
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }

            return nResult;
        }
        private MXP.MXP_FUNCTION_STATUS_RESULT GetMasterOnlineStatus()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT nResult = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                nResult = MXP.ECAT_GetMasterOnlineMode(ref m_EtherCatOnlineMode);
                if (nResult != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    MxpCommLog.WriteLog(string.Format("GetMasterOnlineStatus nResult = {0}", nResult));
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }

            return nResult;
        }
        private MXP.MXP_FUNCTION_STATUS_RESULT GetEcatReadyCheck()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT nResult = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                nResult = MXP.ECAT_ReadyCheck(ref m_EtherCatReady);
                if (nResult != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                {
                    m_EtherCatReady = 0;
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    MxpCommLog.WriteLog(string.Format("GetMasterOnlineStatus nResult = {0}", nResult));
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }

            return nResult;
        }

        private MXP.MXP_FUNCTION_STATUS_RESULT GetSlaveReadyCheck(UInt32 slaveNo)
        {
            MXP.MXP_FUNCTION_STATUS_RESULT nResult = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (slaveNo >= 0 && slaveNo < m_SlaveReady.Length)
                {
                    byte ready = m_SlaveReady[slaveNo];
                    nResult = MXP.ECAT_SlaveReadyCheck(slaveNo, ref ready);
                    if (nResult != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                    {
                        System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                        MxpCommLog.WriteLog(string.Format("GetMasterOnlineStatus nResult = {0}", nResult));
                    }
                    m_SlaveReady[slaveNo] = ready;
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }

            return nResult;
        }

        public MXP.MXP_FUNCTION_STATUS_RESULT SystemInit()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT nResult = new MXP.MXP_FUNCTION_STATUS_RESULT();
            try
            {
                nResult = MXP.SYS_Init();
                if (nResult != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    MxpCommLog.WriteLog(string.Format("SystemInit nResult = {0}", nResult));
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                throw ex;
            }
            return nResult;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT SystemRun()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT nResult = new MXP.MXP_FUNCTION_STATUS_RESULT();
            try
            {
                nResult = MXP.SYS_Run();
                if (nResult != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    MxpCommLog.WriteLog(string.Format("SystemRun nResult = {0}", nResult));
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                throw ex;
            }
            return nResult;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT SystemStop()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT nResult = new MXP.MXP_FUNCTION_STATUS_RESULT();
            try
            {
                nResult = MXP.SYS_Stop();
                if (nResult != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    MxpCommLog.WriteLog(string.Format("SystemStop nResult = {0}", nResult));
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                throw ex;
            }
            return nResult;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT SystemDestory()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT nResult = new MXP.MXP_FUNCTION_STATUS_RESULT();
            try
            {
                nResult = MXP.SYS_Destroy();
                if (nResult != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    MxpCommLog.WriteLog(string.Format("SYS_Destroy nResult = {0}", nResult));
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                throw ex;
            }
            return nResult;
        }

        public MXP.MXP_FUNCTION_STATUS_RESULT ReadVelocityProfile(uint axisNo, Single fPosition, Single fstartV, Single fVel, Single fendV, Single fAcc, Single fdec, Single fJerk, ref Byte shortFlag)
        {
            MXP.MXP_FUNCTION_STATUS_RESULT nResult = new MXP.MXP_FUNCTION_STATUS_RESULT();
            try
            {
                Byte bShortFlag = 0;
                Single[] stepT = new Single[7];
                Single[] stepV = new Single[7];
                Single[] stepD = new Single[7];

                nResult = MXP.AX_ReadVelocityProfile(axisNo, fPosition, fstartV, fVel, fendV, fAcc, fdec, fJerk, ref bShortFlag, ref stepV[0], ref stepD[0], ref stepT[0]);
                if (nResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                {
                    shortFlag = bShortFlag;
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return nResult;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT GetDistanceToAccDec(uint axisNo, Single targetVel, Single distance, Single sCurveRate, ref MXP.ACCTIME_TO_ACCDEC_REPLY data)
        {
            MXP.MXP_FUNCTION_STATUS_RESULT nResult = new MXP.MXP_FUNCTION_STATUS_RESULT();
            try
            {
                MXP.MXP_FUNCTION_STATUS_RESULT result = MXP.AX_DistanceToAccDec(axisNo, targetVel, distance, sCurveRate, ref data);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return nResult;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT GetSequenceMoveDistanceToAccDec(uint axisNo, MXP.SEQUENCE_MOVE_MODE nMoveMode, MXP.MXP_BUFFERMODE_ENUM nBufferedMode, MXP.SEQUENCEMOVE_STEP[] pData, UInt16 stepCount, ref Single stepAccDecDist)
        {
            MXP.MXP_FUNCTION_STATUS_RESULT nResult = new MXP.MXP_FUNCTION_STATUS_RESULT();
            try
            {
                MXP.MXP_FUNCTION_STATUS_RESULT result = MXP.AX_ReadSequenceMoveStepAccDecDist(axisNo, nMoveMode, nBufferedMode, pData, stepCount, ref stepAccDecDist);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return nResult;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT SetExternalExcorder(uint axisNo, MotionSensor motionSensor, float targetpos)
        {
            MXP.MXP_FUNCTION_STATUS_RESULT nResult = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                string msg = string.Empty;
                msg += string.Format("[{0}, {1}, {2}, {3}, {4}, {5}, {6}]",
                    motionSensor.SlaveNo,
                    motionSensor.Offset,
                    motionSensor.Size,
                    motionSensor.SensorPulseToUnit,
                    motionSensor.SensorTargetValue,
                    targetpos,
                    motionSensor.SensorScanDistance);

                MXP.MXP_FUNCTION_STATUS_RESULT result = MXP.AX_SetExternalEncoder2(axisNo, 
                                                                                    1, 
                                                                                    motionSensor.SlaveNo, 
                                                                                    motionSensor.Offset, 
                                                                                    motionSensor.Size, 
                                                                                    motionSensor.SensorPulseToUnit, 
                                                                                    motionSensor.SensorTargetValue, 
                                                                                    targetpos, 
                                                                                    motionSensor.SensorScanDistance);
                if (result == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                {
                    MxpCommLog.WriteLog(string.Format("Axis-{0} BCR Set ExternalEncoder OK : [{1}]", axisNo, msg));
                }
                else MxpCommLog.WriteLog(string.Format("Axis-{0} BCR Set ExternalEncoder NG : [{1}]", axisNo, msg));
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return nResult;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT ResetExternalExcorder(uint axisNo, MotionSensor motionSensor)
        {
            MXP.MXP_FUNCTION_STATUS_RESULT nResult = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                string msg = string.Empty;
                msg += string.Format("[{0}, {1}, {2}, {3}, {4}, {5}]",
                    motionSensor.SlaveNo,
                    motionSensor.Offset,
                    motionSensor.Size,
                    motionSensor.SensorPulseToUnit,
                    motionSensor.SensorTargetValue,
                    motionSensor.SensorScanDistance);

                MXP.MXP_FUNCTION_STATUS_RESULT result = MXP.AX_SetExternalEncoder2(axisNo,
                                                                                    0,
                                                                                    motionSensor.SlaveNo,
                                                                                    motionSensor.Offset,
                                                                                    motionSensor.Size,
                                                                                    motionSensor.SensorPulseToUnit,
                                                                                    motionSensor.SensorTargetValue,
                                                                                    0.0f,
                                                                                    motionSensor.SensorScanDistance);
                if (result == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                {
                    MxpCommLog.WriteLog(string.Format("Axis-{0} BCR Reset ExternalEncoder OK : [{1}]", axisNo, msg));
                }
                else MxpCommLog.WriteLog(string.Format("Axis-{0} BCR Reset ExternalEncoder NG : [{1}]", axisNo, msg));
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return nResult;
        }
        #endregion
        #region [PLC Interface Method] - Activation
        public bool InitilaizePlc()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            bool rv = true;
            try
            {
                foreach (MxpAxisCtrl ctrl in m_AxisCtrls)
                {
                    rv &= ctrl.InitializePlc();
                }
                MxpCommLog.WriteLog(string.Format($"PLC Parameter Init Result : {rv}"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));

                return false;
            }
            return rv;
        }
        public bool InitializeCornerActivation()
        {
            bool rv = true;
            try
            {
                //아래 있는 Bit는 켜지면 안된다.
                rv &= SetCornerDriveActivation((int)enActivationMode.Activation, (int)enActivationOnOff.OFF) == 0;
                rv &= SetCornerDriveActivation((int)enActivationMode.Abort, (int)enActivationOnOff.OFF) == 0;
                rv &= SetCornerDriveActivation((int)enActivationMode.Pause, (int)enActivationOnOff.OFF) == 0;
                MxpCommLog.WriteLog(string.Format($"Activation Init Result : {rv}"));
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return false;
            }
            return rv;
        }
        /// Activation     : MXP.MXP_FUNCTION_STATUS_RESULT rv1 = MXP.PLC_WriteBit((UInt32) 5400, Convert.ToByte(0), Byte set); // set = 0 : 미사용, 1 : 사용
        /// Abort          : MXP.MXP_FUNCTION_STATUS_RESULT rv1 = MXP.PLC_WriteBit((UInt32) 5400, Convert.ToByte(1), Byte set); // set = 0 : 미사용, 1 : 사용
        /// Pause          : MXP.MXP_FUNCTION_STATUS_RESULT rv1 = MXP.PLC_WriteBit((UInt32) 5400, Convert.ToByte(2), Byte set); // set = 0 : 미사용, 1 : 사용
        public int SetCornerDriveActivation(int cmd, Byte set)
        {
            try
            {
                MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.PLC_WriteBit((UInt32)enDriveParameter.ACTIVATION, Convert.ToByte(cmd), set); // set = 0 : 미사용, 1 : 사용

                return (int)rv;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return (int)MXP.MXP_FUNCTION_STATUS_RESULT.RT_ERROR_EXCEPTIONERROR;
            }
        }
        #endregion

        #region Thread
        public class TaskMxpInterface : XSequence
        {
            MxpManager m_Control = null;
            public TaskMxpInterface(MxpManager control)
            {
                m_Control = control;
                RegSeq(new SeqMxpStateCheck(control));
            }

            protected override void ExitRoutine()
            {
                m_Control.SystemDestory();
            }
        }
        class SeqMxpStateCheck : XSeqFunc
        {
            MxpManager m_Control = null;

            public SeqMxpStateCheck(MxpManager ctrl)
            {
                this.SeqName = $"SeqMxpStateCheck";
                m_Control = ctrl;
            }
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                m_Control.UpdateStateMsg();
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            // nResult값은 Run 상태 전에 FUNCTION ERROR 발생함. Init -> Run이 되어야 정상동작하므로 여기서 판단할 필요 없음.
                            MXP.MXP_FUNCTION_STATUS_RESULT nResult = m_Control.GetMxpSysStatus();
                            MxpCommLog.WriteLog(string.Format("MXP Controller Status {0} ", nResult));
                            if (m_Control.ControlState < MXP.KERNEL_STATUS.SYSTEM_IDLE)
                            {
                                //m_Control.SystemStop();
                                seqNo = 10;
                            }
                            else if (m_Control.ControlState < MXP.KERNEL_STATUS.SYSTEM_READY)
                            {
                                seqNo = 10;
                            }
                            else if (m_Control.ControlState == MXP.KERNEL_STATUS.SYSTEM_READY)
                            {
                                seqNo = 20;
                            }
                            else if (m_Control.ControlState == MXP.KERNEL_STATUS.SYSTEM_RUN)
                            {
                                seqNo = 30;
                            }
                        }
                        break;

                    case 10:
                        {
                            MXP.MXP_FUNCTION_STATUS_RESULT nResult = m_Control.SystemInit(); // No Error 상태로 변한다.
                            MxpCommLog.WriteLog(string.Format("MXP Controller System Init {0} ", nResult));
                            if (nResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                            {
                                // plc parameter setting after SystemInit 
                                m_Control.InitilaizePlc();
                                m_Control.InitializeCornerActivation();
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 20:
                        {
                            MXP.MXP_FUNCTION_STATUS_RESULT nResult = m_Control.SystemRun(); // No Error 상태로 변한다.
                            MxpCommLog.WriteLog(string.Format("MXP Controller System Run {0} ", nResult));
                            if (nResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 30:
                        {
                            MXP.MXP_FUNCTION_STATUS_RESULT nResult = m_Control.GetMasterOnlineStatus();
                            MxpCommLog.WriteLog(string.Format("MXP Controller EtherCat Master Online Status {0} ", nResult));
                            if (nResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                            {
                                if (m_Control.EtherCatOnlineMode <= MXP.MXP_ONLINE_STATE.ONLINE_STATE_ERROR_NONE)
                                {
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 40;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > 1000)
                                {
                                    m_Control.SystemStop();
                                    MxpCommLog.WriteLog(string.Format("MXP Controller System Stop"));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 1000;
                                }
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_Control.SystemStop();
                                MxpCommLog.WriteLog(string.Format("MXP Controller System Stop"));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 40:
                        {
                            MXP.MXP_FUNCTION_STATUS_RESULT nResult = m_Control.GetEcatReadyCheck();
                            //MxpCommLog.WriteLog(string.Format("MXP Controller EtherCat Master Ready Check {0} ", m_Control.EtherCatReady));
                            if (nResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                            {
                                if (m_Control.EtherCatReady == 1)
                                {
                                    MxpCommLog.WriteLog(string.Format("MXP Controller EtherCat Ready"));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 50;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > 5000)
                                {
                                    MxpCommLog.WriteLog(string.Format("MXP Controller EtherCat Not Ready"));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 1000;
                                }
                            }
                            else if (nResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_ERROR_NOTCONNECT_FIRSTSLAVE)
                            {
                                m_Control.SystemStop();
                                MxpCommLog.WriteLog(string.Format("MXP Controller System Stop"));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 1000;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_Control.SystemStop();
                                MxpCommLog.WriteLog(string.Format("MXP Controller System Stop"));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 50:
                        {
                            MXP.MXP_FUNCTION_STATUS_RESULT nResult = m_Control.GetMxpSysStatus();
                            if (nResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                            {
                                if (m_Control.ControlState == MXP.KERNEL_STATUS.SYSTEM_RUN)
                                {
                                    MxpCommLog.WriteLog(string.Format("MXP Controller System Run"));
                                    m_Control.AllServoStop();
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 100;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > 1000)
                                {
                                    m_Control.SystemStop();
                                    MxpCommLog.WriteLog(string.Format("MXP Controller System Stop"));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 1000;
                                }
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_Control.SystemStop();
                                MxpCommLog.WriteLog(string.Format("MXP Controller System Stop"));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 100:
                        {
                            MXP.MXP_FUNCTION_STATUS_RESULT nResult = m_Control.GetMxpSysStatus();
                            MXP.MXP_FUNCTION_STATUS_RESULT nResult1 = m_Control.GetEcatReadyCheck();

                            if (nResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR &&
                                nResult1 == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                            {
                                if (m_Control.ControlState == MXP.KERNEL_STATUS.SYSTEM_RUN &&
                                    m_Control.EtherCatReady == 1)
                                {
                                    ServoManager.Instance.UpdateRun = true;
                                    m_Control.IsConnected = true;
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 110;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > 1000)
                                {
                                    m_Control.SystemStop();
                                    MxpCommLog.WriteLog(string.Format("MXP Controller System Stop"));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 1000;
                                }
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                m_Control.SystemStop();
                                MxpCommLog.WriteLog(string.Format("MXP Controller System Stop"));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 110:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1 * 1000) break;
                            seqNo = 100;
                        }
                        break;

                    case 1000:
                        {
                            m_Control.IsConnected = false;

                            if (XFunc.GetTickCount() - StartTicks < 1000) break;
                            seqNo = 0;
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }            
        }
        #endregion  

    }
}
