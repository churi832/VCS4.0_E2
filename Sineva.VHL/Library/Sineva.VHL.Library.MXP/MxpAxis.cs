using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using static Sineva.VHL.Library.MXP.MXP;
using Sineva.VHL.Library.RegistryKey;

namespace Sineva.VHL.Library.MXP
{
    public class MxpAxis
    {
        #region Fields
        private AxisBlockMXP m_AxisBlock = null;
        private _Axis m_Axis = null;
        private MxpAxisCtrl m_AxisCtrl = null;

        private MXP.MXP_AXIS_STATEBIT m_AxisState = new MXP.MXP_AXIS_STATEBIT();
        private MXP.MXP_FUNCTION_STATUS_RESULT m_CommandResult = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
        private MXP.AXIS_ERROR m_AxisError = new MXP.AXIS_ERROR();
        private MXP.SEQUENCEMOVE_PROCESS_CHECK m_SequenceState = new MXP.SEQUENCEMOVE_PROCESS_CHECK();
        private MXP.MXP_MOVESTATE m_MoveState = new MXP.MXP_MOVESTATE();
        private TaskAxis m_TaskAxis = null;
        private readonly static object m_LockKey = new object();

        private readonly uint m_TorqueLimitAddress = 540;
        private double m_OldCurSpeed = 0.0f;
        private double m_OverrideRate = 0.0f; //기억하고 있는 최종 overrride
        private double m_OldOverrideRate = 0.0f; //override set 할때 current override를 기억하자!

        protected SequenceCommand m_SequenceCommand = new SequenceCommand();
        private bool m_SequenceMoving = false;
        private bool m_ProfileMoveMonitoringInit = false;
        private bool m_CommandRun = false;
        private bool m_CommandHome = false;
        private bool m_HomeEnd = false;
        private string m_HomeEndKey = "HomeEnd";
        private double m_ActualPosition = 0.0f;
        private string m_ActualPositionKey = "ActualPos";
        private bool m_OneTimePositionUpdate = true;
        private bool m_OverrideStop = false;
        private bool m_InRangeError = false;
        private bool m_InRangeChecking = false;
        private bool m_ExternalEncoderAbort = false;

        private UInt16 m_axisAlarmCount = 0;
        private UInt16 m_driveAlarmCount = 0;
        private UInt16[] m_arrAxisAlarmHistory = new UInt16[10];
        private UInt16[] m_arrADriveAlarmHistory = new UInt16[10];

        public static Dictionary<int, bool[]> ErrorTest = new Dictionary<int, bool[]>();
        #endregion

        #region Properties
        public MXP.MXP_AXIS_STATEBIT AxisState { get => m_AxisState; set => m_AxisState = value; }
        public MXP.MXP_FUNCTION_STATUS_RESULT CommandResult { get => m_CommandResult; set => m_CommandResult = value; }
        public MXP.AXIS_ERROR AxisError { get => m_AxisError; set => m_AxisError = value; }
        public MXP.SEQUENCEMOVE_PROCESS_CHECK SequenceState { get => m_SequenceState; set => m_SequenceState = value; }
        public MXP.MXP_MOVESTATE MoveState { get => m_MoveState; set => m_MoveState = value; }

        #endregion

        public MxpAxis(MxpAxisCtrl ctrl, AxisBlockMXP block, _Axis axis, bool thread_create = true) 
        {
            m_AxisCtrl = ctrl;
            m_AxisBlock = block;
            m_Axis = axis;

            if (thread_create)
            {
                m_TaskAxis = new TaskAxis(this);

                m_HomeEndKey = string.Format("{0}_HomeEnd", axis.AxisName);
                m_ActualPositionKey = string.Format("{0}_ActualPos", axis.AxisName);
                m_HomeEnd = GetHomeEnd();
                m_ActualPosition = GetActualPosition();

                TaskHandler.Instance.RegTask(m_TaskAxis, 10, System.Threading.ThreadPriority.Highest);

                // Override와 SequenceMove를 여기서 구현하기에는 너무 복잡하게 된다. 장비 운영과 너무 역인다.
                TaskHandler.Instance.RegTask(new TaskOverride(this), 5, System.Threading.ThreadPriority.Highest);
                TaskHandler.Instance.RegTask(new TaskSequenceMoveMonitor(this), 5, System.Threading.ThreadPriority.Highest);
            }
        }

        public void Init()
        {
            foreach (XSeqFunc seq in m_TaskAxis.SeqFuncs) seq.InitSeq();
            CommandResult = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            //(m_Axis as MpAxis).ControllerAlarmId = 0;
            //(m_Axis as MpAxis).DriverAlarmId = 0;
            (m_Axis as MpAxis).TorqueLimitBit = false;
            (m_Axis as MpAxis).SequenceMoveCommandSet = false;
            m_SequenceMoving = false;
        }
        public bool GetHomeEnd()
        {
            bool home_end = false;
            home_end = RegistryConfiguration.Instance.ReadEntry(m_HomeEndKey, home_end);
            return home_end;
        }
        public void SetHomeEnd(bool home_end)
        {
            m_HomeEnd = home_end;
            RegistryConfiguration.Instance.WriteEntry(m_HomeEndKey, m_HomeEnd);
        }
        public double GetActualPosition()
        {
            double pos = 0.0f;
            pos = RegistryConfiguration.Instance.ReadEntry(m_ActualPositionKey, pos);
            return pos;
        }
        public void SetActualPosition(double pos)
        {
            if (Math.Abs(m_ActualPosition - pos) > 1.0f)
            {
                m_ActualPosition = pos;
                RegistryConfiguration.Instance.WriteEntry(m_ActualPositionKey, pos);
            }
        }

        #region Thread Methods
        public MXP.MXP_FUNCTION_STATUS_RESULT UpdateState()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;

            try
            {
                //if (m_Axis.NodeId != 5) return rv;
                if (m_Axis != null && m_AxisBlock.Connected)
                {
                    bool slave_axis = m_Axis.GantryType && m_Axis.NodeId != m_Axis.MasterNodeId;

                    ///PDO READING//////////////////////////////////////////////////////////////////////////////////
                    //if (m_Axis.MotionSensorPara.SensorUse == 1 && m_Axis.MotionSensorPara.ControlMp) // 위치값은 항상 읽어야 할 것 같음.
                    {
                        int bcr_value = 0;
                        ushort size = 4;
                        ushort offset = 0;
                        byte[] output = new byte[size];
                        MXP.MXP_PDO_DIRECTION pdoDirection = MXP.MXP_PDO_DIRECTION.SERVO_WRITE;
                        if (m_Axis.LeftBcrNodeId != -1)
                        {
                            rv = MXP.ECAT_ReadPdoData((uint)m_Axis.LeftBcrNodeId, pdoDirection, (ushort)offset, size, ref output[0]);
                            if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                                MxpCommLog.WriteLog(string.Format("{0} ReadPDO Left Barcode NG {1}", m_Axis.AxisName, rv.ToString()));
                            else
                            {
                                string data = BitConverter.ToInt32(output, 0).ToString();
                                bcr_value = int.TryParse(data, out bcr_value) ? bcr_value : 0;
                                if (bcr_value != 0) m_Axis.LeftBcrPos = bcr_value / 10.0f - m_Axis.LeftBCROffset;
                            }
                        }
                        if (m_Axis.RightBcrNodeId != -1)
                        {
                            rv = MXP.ECAT_ReadPdoData((uint)m_Axis.RightBcrNodeId, pdoDirection, (ushort)offset, size, ref output[0]);
                            if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                                MxpCommLog.WriteLog(string.Format("{0} ReadPDO Right Barcode NG {1}", m_Axis.AxisName, rv.ToString()));
                            else
                            {
                                string data = BitConverter.ToInt32(output, 0).ToString();
                                bcr_value = int.TryParse(data, out bcr_value) ? bcr_value : 0;
                                if (bcr_value != 0) m_Axis.RightBcrPos = bcr_value / 10.0f - m_Axis.RightBCROffset;
                            }
                        }

                        /////////////////////////////////////////////////////////////////////////////////////
                        if (m_Axis.MotorType == enMotorType.Mxp)
                        {
                            offset = 7;
                            pdoDirection = MXP.MXP_PDO_DIRECTION.MXP_WRITE;
                            rv = MXP.ECAT_ReadPdoData((uint)m_Axis.MXConfigratorSlaveNo, pdoDirection, (ushort)offset, size, ref output[0]);
                            if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                                MxpCommLog.WriteLog(string.Format("{0} ReadPDO Command Velocity NG {1}", m_Axis.AxisName, rv.ToString()));
                            else m_AxisBlock.SetCommandSpeedPdo(m_Axis.AxisId, Math.Round((double)BitConverter.ToInt32(output, 0), 5));
                        }
                    }
                    /////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////////////////
                    ushort curStep = 0, curState = 0;
                    double tarVel = 0.0f, tarTime = 0.0f, curTime = 0.0f;
                    rv = MXP.AX_ReadTrajectoryStateEx((uint)m_Axis.AxisId, ref tarTime, ref curTime, ref tarVel, ref curStep, ref curState);
                    (m_Axis as MpAxis).TrajectoryTargetTime = tarTime;
                    (m_Axis as MpAxis).TrajectoryCurrentTime = curTime;
                    (m_Axis as MpAxis).TrajectoryTargetVelocity = tarVel;
                    (m_Axis as MpAxis).TrajectoryCurrentStep = curStep;
                    (m_Axis as MpAxis).TrajectoryState = curState;
                    /////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////////////////
                    MXP.MXP_AXIS_STATEBIT state = new MXP.MXP_AXIS_STATEBIT();
                    rv = MXP.AX_ReadStatus((uint)m_Axis.NodeId, ref state);
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} Update State NG {1}", m_Axis.AxisName, rv.ToString()));
                    else m_AxisState = state;
                    /////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////////////////
                    MXP.MXP_MOVESTATE motion_state = new MXP_MOVESTATE();
                    Single targetpos = (Single)m_AxisBlock.GetTargetPosition(m_Axis.AxisId);
                    Single in_range = (Single)m_Axis.InRangeValue;
                    rv = MXP.AX_MoveStateCheck((uint)m_Axis.NodeId, targetpos, in_range, ref motion_state);
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} Update Motion State NG {1}", m_Axis.AxisName, rv.ToString()));
                    else m_MoveState = motion_state;
                    /////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////////////////
                    UInt16 remain_count = 0;
                    UInt16 current_step = 0;
                    StringBuilder strCurStepName = new StringBuilder(100);
                    rv = MXP.AX_SequenceMoveCheck((uint)m_Axis.NodeId, ref current_step, strCurStepName, ref remain_count, ref m_SequenceState);
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} Sequence Move Check NG {1}", m_Axis.AxisName, rv.ToString()));
                    else
                    {
                        if (m_Axis.SequenceState.SequenceCurrentStep != current_step)
                        {
                            MxpCommLog.WriteLog(string.Format("{0} Sequence Move Step Change {1}/{2}", m_Axis.AxisName, current_step, remain_count));
                        }
                        m_Axis.SequenceState.SequenceRemainCount = remain_count;
                        m_Axis.SequenceState.SequenceCurrentStep = current_step;
                        m_Axis.SequenceState.SequenceStepName = strCurStepName.ToString();
                        m_Axis.SequenceState.IsSequenceBusy = m_SequenceState.busy == 1 ? true : false;
                        m_Axis.SequenceState.IsSequenceComplete = m_SequenceState.done == 1 ? true : false;
                        m_Axis.SequenceState.IsSequenceAlarm = m_SequenceState.errorOn == 1 ? true : false;
                        m_Axis.SequenceState.SequenceAlarmId = m_SequenceState.errorID;
                    }
                    /////////////////////////////////////////////////////////////////////////////////////

                    float fReadData = 0.0f;
                    /////////////////////////////////////////////////////////////////////////////////////
                    rv = MXP.AX_ReadActualPosition((uint)m_Axis.NodeId, ref fReadData);
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} Read Actual Position NG {1}", m_Axis.AxisName, rv.ToString()));
                    else
                    {
                        if (m_OneTimePositionUpdate)
                        {
                            m_OneTimePositionUpdate = false;
                            if (Math.Abs(m_ActualPosition - fReadData) > 1.0f && m_Axis.HomeMethod == enHomeMethod.ZERO_MOVE_HOME) // Slide Home 방식때문
                            {
                                if (Math.Abs(fReadData) < 1.0f) SetPosition(m_ActualPosition);
                            }
                        }
                        else
                        {
                            SetActualPosition(fReadData);
                            m_AxisBlock.SetCurPosition(m_Axis.AxisId, Math.Round(fReadData, 5));
                        }
                    }
                    /////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////////////////
                    rv = MXP.AX_ReadActualVelocity((uint)m_Axis.NodeId, ref fReadData);
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} Read Actual Velocity NG {1}", m_Axis.AxisName, rv.ToString()));
                    else
                    {
                        if (m_OldCurSpeed < 0.1f) m_OldCurSpeed = fReadData; //초기화
                        double cur_speed = (1.0f - (m_Axis as MpAxis).FilterGain) * fReadData + (m_Axis as MpAxis).FilterGain * m_OldCurSpeed; //속도 Hunting이 심한 경우를 막기 위해
                        m_AxisBlock.SetCurSpeed(m_Axis.AxisId, Math.Round(cur_speed, 5));
                        m_OldCurSpeed = cur_speed;
                    }
                    /////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////////////////
                    float overrideValue = 0;
                    MXP.MXP_FUNCTION_STATUS_RESULT result = MXP.AX_ReadVelocityOverrideFactor((uint)m_Axis.NodeId, ref overrideValue);
                    (m_Axis as MpAxis).CurrentOverrideRate = (double)overrideValue;
                    /////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////////////////
                    if (m_Axis.TorqueCheckUse)
                    {
                        rv = MXP.AX_ReadActualTorque((uint)m_Axis.NodeId, ref fReadData);
                        if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                            MxpCommLog.WriteLog(string.Format("{0} Read Actual Torque NG {1}", m_Axis.AxisName, rv.ToString()));
                        else m_AxisBlock.SetCurTorque(m_Axis.AxisId, Math.Round(100.0f * fReadData, 5));
                    }
                    /////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////////////////
                    rv = MXP.AX_ReadCommandPosition((uint)m_Axis.NodeId, ref fReadData);
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} Read Command Position NG {1}", m_Axis.AxisName, rv.ToString()));
                    else m_AxisBlock.SetCommandPosition(m_Axis.AxisId, Math.Round(fReadData, 5));
                    /////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////////////////
                    rv = MXP.AX_ReadCommandVelocity((uint)m_Axis.NodeId, ref fReadData);
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} Read Command Velocity NG {1}", m_Axis.AxisName, rv.ToString()));
                    else m_AxisBlock.SetCommandSpeed(m_Axis.AxisId, Math.Round(fReadData, 5));
                    /////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////////////////
                    rv = MXP.AX_ReadFollowingError((uint)m_Axis.NodeId, ref fReadData);
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} Read Read FollowingError NG {1}", m_Axis.AxisName, rv.ToString()));
                    else m_AxisBlock.SetFollowingError(m_Axis.AxisId, Math.Round(fReadData, 5));
                    /////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////////////////
                    rv = MXP.AX_ReadError((uint)m_Axis.NodeId, ref m_AxisError); //추후 삭제 예정
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} Read Error NG {1}", m_Axis.AxisName, rv.ToString()));
                    else
                    {
                        (m_Axis as MpAxis).CommandAlarmId = (int)m_CommandResult;
                        //(m_Axis as MpAxis).ControllerAlarmId = m_AxisError.mxpError;
                        //(m_Axis as MpAxis).DriverAlarmId = Convert.ToInt32(m_AxisError.driveError.ToString("X"));

                        //if (m_AxisError.mxpError > 0 || Convert.ToInt32(m_AxisError.driveError.ToString("X")) > 0)
                        //    MxpCommLog.WriteLog(string.Format("mxpError:({0}),driveError:({1})", m_AxisError.mxpError, Convert.ToInt32(m_AxisError.driveError.ToString("X"))));
                    }

                    rv = MXP.AX_ReadErrorHistory((uint)m_Axis.NodeId, ref m_axisAlarmCount, ref m_driveAlarmCount, ref m_arrAxisAlarmHistory[0], ref m_arrADriveAlarmHistory[0]);
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} History Read Error NG {1}", m_Axis.AxisName, rv.ToString()));
                    else
                    {
                        if(AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            for (int i = 0; i < m_Axis.DriveErrorTest.Count; i++)
                            {
                                m_arrADriveAlarmHistory[i] = m_Axis.DriveErrorTest[i];
                            }
                            for (int i = 0; i < m_Axis.MXPErrorTest.Count; i++)
                            {
                                m_arrAxisAlarmHistory[i] = m_Axis.MXPErrorTest[i];
                            }
                            m_axisAlarmCount = (ushort)m_Axis.MXPErrorTest.Count;
                            m_driveAlarmCount = (ushort)m_Axis.DriveErrorTest.Count;
                            
                        }

                        (m_Axis as MpAxis).CommandAlarmId = (int)m_CommandResult;

                        int[] arraxisalarmlist = Array.ConvertAll(m_arrAxisAlarmHistory, x => Convert.ToInt32(x));
                        List<int> listAxisAlarm = arraxisalarmlist.OfType<int>().ToList();
                        (m_Axis as MpAxis).ControllerAlarmIdList = listAxisAlarm;
                        (m_Axis as MpAxis).ControllerAlarmCount = m_axisAlarmCount;

                        string[] arrdrivealarmlist = Array.ConvertAll(m_arrADriveAlarmHistory, x => Convert.ToString(x, 16));
                        List<string> charDriveAlarmlist = arrdrivealarmlist.OfType<string>().ToList();
                        List<int> listDriveAlarm = charDriveAlarmlist.ConvertAll(x => Convert.ToInt32(x));
                        (m_Axis as MpAxis).DriverAlarmIdList = listDriveAlarm;
                        (m_Axis as MpAxis).DriverAlarmCount = m_driveAlarmCount;

                        //if (m_axisAlarmCount > 0)
                        //{
                        //    string axis_alarm = string.Format("{0} Axis Alarm Count=({1}) | ", m_Axis.AxisName, m_axisAlarmCount);
                        //    axis_alarm += string.Format("Axis AlarmList : {0}", string.Join("-", listAxisAlarm));
                        //    MxpCommLog.WriteLog(axis_alarm);
                        //}
                        //if (m_driveAlarmCount > 0)
                        //{
                        //    string drive_alarm = string.Format("{0} Driver Alarm Count=({1}) | ", m_Axis.AxisName, m_driveAlarmCount);
                        //    drive_alarm += string.Format("Driver AlarmList : {0}", string.Join("-", listDriveAlarm));
                        //    MxpCommLog.WriteLog(drive_alarm);
                        //}
                    }

                    /////////////////////////////////////////////////////////////////////////////////////
                    Byte command_state = 0; // Command Run State,
                    MXP.AX_ReadInMotion((uint)m_Axis.NodeId, ref command_state);

                    /// state of the Axis ///////////////////////////////////////////////////////////////
                    enAxisInFlag inFlag = enAxisInFlag.None;
                    if (m_AxisState.powerOn == 1 && m_AxisState.disabled == 0) inFlag |= enAxisInFlag.SvOn;
                    if (m_AxisState.stopping == 1) inFlag |= enAxisInFlag.Busy;
                    if (m_AxisState.discreteMotion == 1) inFlag |= enAxisInFlag.Busy;
                    if (m_AxisState.continuousMotion == 1 && !slave_axis) inFlag |= enAxisInFlag.Busy;
                    if (m_AxisState.homing == 1 || m_CommandHome) inFlag |= enAxisInFlag.Busy;
                    //if (m_MoveState == MXP_MOVESTATE.MOVESTATE_MOVING) inFlag |= enAxisInFlag.Busy;
                    if (m_SequenceState.busy == 1) inFlag |= enAxisInFlag.Busy;
                    if (command_state == 1) inFlag |= enAxisInFlag.Busy;

                    //if ((m_AxisState.isHomed == 1 || m_HomeEnd) && m_CommandHome == false) inFlag |= enAxisInFlag.HEnd;

                    // SeqHome에서 m_AxisState.isHomed Bit보고 isHomed가 살면 HomeOffset 이동 후 m_HomeEnd 살릴거다..
                    // m_AxisState.isHomed 이거랑 OR 조건으로 보니까 HomeOffset 이동 하기 전에 Abort하면 그대로 Home 동작 정상 처리
                    // SeqHome에서는 Abort 되거나 MXP에서 isHomed Bit 안주면 그대로 Fail 처리함..
                    // 프로그램 재실행하거나 했을 때는 레지스터리에 저장 되어 있는 Home 정상 진행 여부 Read 해서 그대로 유지하니 문제 없을 듯..
                    if (m_AxisState.powerOn == 0 && m_HomeEnd) SetHomeEnd(false);
                    if (m_HomeEnd && m_CommandHome == false) inFlag |= enAxisInFlag.HEnd;

                    //if (m_MoveState != MXP_MOVESTATE.MOVESTATE_MOVING && 
                    //    m_AxisState.standstill == 1 &&
                    //    m_SequenceState.busy == 0 && command_state == 0) inFlag |= enAxisInFlag.InPos; // StandStill 상태가 이상하다. 멈추었는데 StandStill Off 상태임.
                    if (m_AxisState.disabled == 1)
                    {
                        if (m_AxisState.inPos == 1 && m_SequenceState.busy == 0) inFlag |= enAxisInFlag.InPos;
                    }
                    else
                    {
                        //if (m_Axis.GantryType && m_Axis.NodeId == m_Axis.SlaveNodeId)
                        //{
                        //    if (m_SequenceState.busy == 0) inFlag |= enAxisInFlag.InPos;
                        //}
                        //else
                        if (slave_axis && m_AxisState.continuousMotion == 1 && m_AxisState.inPos == 1 && m_SequenceState.busy == 0 && !m_CommandHome) inFlag |= enAxisInFlag.InPos;
                        else if (m_AxisState.standstill == 1 && m_AxisState.inPos == 1 && m_SequenceState.busy == 0 && !m_CommandHome) inFlag |= enAxisInFlag.InPos;
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //Console.WriteLine("NAME = {0},  STATE = {1}, MoveState = {2}, AxisState.standstill = {3}, Position = {4}, Velocity = {5}", 
                    //    m_Axis.AxisName, inFlag, m_MoveState, m_AxisState.standstill, (m_Axis as MpAxis).CurPos, (m_Axis as MpAxis).CurSpeed);
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (m_AxisState.homeAbsSwitch == 1) inFlag |= enAxisInFlag.Org;
                    if (m_AxisState.hardwareLimitNegativeEvent == 1) inFlag |= enAxisInFlag.Limit_N;
                    if (m_AxisState.hardwareLimitPositiveEvent == 1) inFlag |= enAxisInFlag.Limit_P;

                    if (IsAlarm()) inFlag |= enAxisInFlag.Alarm;
                    if (m_OverrideStop) inFlag |= enAxisInFlag.OverrideAbnormalStop;
                    if (m_InRangeError) inFlag |= enAxisInFlag.InRange_Error;
                    if (m_InRangeChecking) inFlag |= enAxisInFlag.InRange_Checking;

                    if (slave_axis) //Slave Motor
                    {
                        // Slave 축은 InPos, Busy, Home 상태 Update가 않된다.
                        MXP.MXP_AXIS_STATEBIT master_state = m_AxisCtrl.AxisList[m_Axis.MasterNodeId].AxisState;
                        MXP.SEQUENCEMOVE_PROCESS_CHECK sequence_state = m_AxisCtrl.AxisList[m_Axis.MasterNodeId].SequenceState;
                        enAxisInFlag master_inFlag = m_AxisCtrl.AxisList[m_Axis.MasterNodeId].m_Axis.AxisStatus;
                        if (master_state.stopping == 1) inFlag |= enAxisInFlag.Busy;
                        if (master_state.discreteMotion == 1) inFlag |= enAxisInFlag.Busy;
                        if (master_state.continuousMotion == 1) inFlag |= enAxisInFlag.Busy;
                        if (master_state.homing == 1) inFlag |= enAxisInFlag.Busy;
                        if (sequence_state.busy == 1) inFlag |= enAxisInFlag.Busy;
                        if (master_inFlag.HasFlag(enAxisInFlag.HEnd)) inFlag |= enAxisInFlag.HEnd;
                        if (master_state.standstill == 1 /*&& m_AxisState.synchronizedMotion == 1*/ &&
                            master_state.inPos == 1 &&
                            sequence_state.busy == 0 &&
                            m_CommandRun == false &&
                            m_SequenceMoving == false) inFlag |= enAxisInFlag.InPos;
                    }

                    if (AppConfig.Instance.Simulation.MY_DEBUG)
                    {
                        if ((Math.Abs(m_Axis.CurPos) < 1.0f) && inFlag != enAxisInFlag.InPos) inFlag |= enAxisInFlag.Org;
                    }

                    m_AxisBlock.SetMotionState(m_Axis.AxisId, inFlag);
                    /////////////////////////////////////////////////////////////////////////////////////
                    //AxisStateMsg///////////////////////////////////////////////////////////////////////////////////
                    string msg = string.Empty;
                    string alarm_msg = string.Empty;
                    // Axis State Monitor
                    MXP.MXP_AXIS_STATEBIT axisState = m_AxisState;
                    msg += "|";
                    msg += axisState.errorStop == 1 ? string.Format("errorStop|") : "";
                    msg += axisState.disabled == 1 ? string.Format("disabled|") : "";
                    msg += axisState.stopping == 1 ? string.Format("stopping|") : "";
                    msg += axisState.standstill == 1 ? string.Format("standstill|") : "";
                    msg += axisState.discreteMotion == 1 ? string.Format("discreteMotion|") : "";
                    msg += axisState.continuousMotion == 1 ? string.Format("continuousMotion|") : "";
                    msg += axisState.synchronizedMotion == 1 ? string.Format("synchronizedMotion|") : "";
                    msg += axisState.homing == 1 ? string.Format("homing|") : "";
                    msg += axisState.constantVelocity == 1 ? string.Format("constantVelocity|") : "";
                    msg += axisState.accelerating == 1 ? string.Format("accelerating|") : "";
                    msg += axisState.decelerating == 1 ? string.Format("decelerating|") : "";
                    msg += axisState.directionPositive == 1 ? string.Format("directionPositive|") : "";
                    msg += axisState.directionNegative == 1 ? string.Format("directionNegative|") : "";
                    msg += axisState.homeAbsSwitch == 1 ? string.Format("homeAbsSwitch|") : "";
                    msg += axisState.hardwareLimitPositiveEvent == 1 ? string.Format("hardwareLimitPositiveEvent|") : "";
                    msg += axisState.hardwareLimitNegativeEvent == 1 ? string.Format("hardwareLimitNegativeEvent|") : "";
                    msg += axisState.readyForPowerOn == 1 ? string.Format("readyForPowerOn|") : "";
                    msg += axisState.powerOn == 1 ? string.Format("powerOn|") : "";
                    msg += axisState.isHomed == 1 ? string.Format("isHomed|") : "";
                    msg += axisState.inPos == 1 ? string.Format("inPos|") : "";
                    msg += axisState.axisWarning == 1 ? string.Format("axisWarning|") : "";
                    msg += axisState.servoFault == 1 ? string.Format("servoFault|") : "";
                    msg += axisState.servoWarning == 1 ? string.Format("servoWarning|") : "";
                    msg += axisState.servoTargetReached == 1 ? string.Format("servoTargetReached|") : "";
                    msg += m_SequenceState.busy == 1 ? string.Format("m_SequenceStatebusy|") : "";
                    msg += command_state == 1 ? string.Format("command_state|") : "";
                    if (IsAlarm())
                    {
                        msg += string.Format("CommandResult=({0})|", m_CommandResult);
                        msg += string.Format("mxpError=({0})|", m_AxisError.mxpError);
                        msg += string.Format("driveError=({0})|", Convert.ToInt32(m_AxisError.driveError.ToString("X")));
                        msg += string.Format("sequenceError=({0})|", m_Axis.SequenceState.SequenceAlarmId);
                        msg += m_SequenceState.errorOn == 1 ? string.Format("SequenceState.errorOn|") : "";
                        //alarm_msg = string.Format("Mxp=({0})|Driver=({1})|Seq=({2})|", m_AxisError.mxpError, m_AxisError.driveError, m_Axis.SequenceState.SequenceAlarmId); //추후 삭제 예정
                        if (m_axisAlarmCount > 0)
                        {
                            msg += string.Format("MxpErrorCount=({0})|", m_axisAlarmCount);
                            msg += string.Format("MxpErrorList:({0})|", string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList));
                            alarm_msg += string.Format("MxpErrorList:({0})|", string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList));
                        }
                        if (m_driveAlarmCount > 0)
                        {
                            msg += string.Format("DriverErrorCount=({0})|",m_driveAlarmCount);
                            msg += string.Format("DriverErrorList:({0})|", string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList));
                            alarm_msg += string.Format("DriverErrorList:({0})|", string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList));
                        }
                    }
                    m_Axis.AxisStateMsg = msg;
                    m_Axis.AixsAlarmMsg = alarm_msg;
                    /////////////////////////////////////////////////////////////////////////////////////
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP UpdateState Axis{0} : {1}", m_Axis.NodeId, ex.Message));
            }
            return rv;
        }

        public bool IsAlarm()
        {
            bool alarm = false;
            if (m_SequenceMoving)
                alarm |= m_SequenceState.errorOn == 1 ? true : false;
            alarm |= m_AxisState.errorStop == 1 ? true : false;
            alarm |= m_AxisState.servoFault == 1 ? true : false;
            //alarm |= m_AxisError.mxpError != 0 ? true : false;
            //alarm |= m_AxisError.driveError != 0 ? true : false;
            alarm |= m_CommandResult != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR ? true : false;
            alarm |= m_axisAlarmCount > 0 ? true : false;
            alarm |= m_driveAlarmCount > 0 ? true : false;
            //alarm |= m_OverrideStop;

            return alarm;
        }

        public bool IsReady()
        {
            Byte ready_check = 0;
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.AX_ReadyCheck((uint)m_Axis.NodeId, ref ready_check);
            bool ready = ready_check == 1 ? true : false;
            if (rv == MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
            {
                ready &= m_AxisState.disabled == 0 ? true : false;
                ready &= m_AxisState.standstill == 1 ? true : false;
                ready &= m_MoveState == MXP_MOVESTATE.MOVESTATE_MOVING ? false : true;
                ready &= m_SequenceState.busy == 0 ? true : false;
            }
            return ready;
        }

        public MXP.MXP_FUNCTION_STATUS_RESULT CheckMoveState()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (m_Axis != null && m_AxisBlock.Connected)
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    Single targetpos = (Single)m_AxisBlock.GetTargetPosition(m_Axis.AxisId);
                    Single in_range = (Single)m_Axis.InRangeValue;

                    rv = MXP.AX_MoveStateCheck((uint)m_Axis.NodeId, targetpos, in_range, ref m_MoveState);
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} Move State NG {1}", m_Axis.AxisName, rv.ToString()));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP Move State {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT EmoStop()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (m_Axis != null && !m_Axis.CommandSkip && m_AxisBlock.Connected)
                {
                    MxpCommLog.WriteLog(string.Format("{0} AX_AllEMGStop - {1}", m_Axis.AxisName, m_SequenceMoving ? "Sequence Moving" : "Sequence Stop"));
                    Init();
                    /////////////////////////////////////////////////////////////////////////////////////
                    rv = MXP.AX_AllEMGStop();
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} AX_AllEMGStop NG {1}", m_Axis.AxisName, rv.ToString()));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP AX_AllEMGStop {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT Stop(bool forced = false, bool override_reset = true, bool inrange_reset = true)
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (m_Axis != null && !m_Axis.CommandSkip && (m_AxisBlock.Connected || forced))
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    Single targetdec = (Single)m_Axis.DecLimit;
                    Single targetjerk = (Single)m_Axis.JerkLimit;
                    if (m_Axis.GantryType && !forced)
                    {
                        double curVelocity = m_Axis.CurSpeed;
                        if (targetdec > 2 * curVelocity) targetdec = (Single)Math.Abs(2 * curVelocity);
                        if (targetdec < 100.0f) targetdec = 100.0f;
                        if (targetjerk > 4 * curVelocity) targetjerk = (Single)Math.Abs(4 * curVelocity);
                        if (targetjerk < 200.0f) targetjerk = 200.0f;
                    }
                    if (m_Axis.AxisName == "Hoist")
                    {
                        //올라갈때 내려올때 가감속을 다르게 가져가자 ~~~
                        double curPos = m_Axis.CurPos;
                        double targetpos = m_AxisBlock.GetTargetPosition(m_Axis.AxisId);
                        if (curPos < targetpos) //올라 갈때 가감속을 1/2로 하자
                        {
                            targetdec *= 0.5f; targetjerk *= 0.5f;
                        }
                    }
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (m_SequenceMoving)
                    {
                        m_SequenceMoving = false; // Sequence Move 관련 상태를 보면 않된다.
                        MxpCommLog.WriteLog(string.Format("{0} AX_Halt[{1},{2}] during Sequence Moving", m_Axis.AxisName, targetdec, targetjerk));
                    }
                    else MxpCommLog.WriteLog(string.Format("{0} AX_Halt[{1},{2}]", m_Axis.AxisName, targetdec, targetjerk));

                    if (m_Axis.GantryType && m_Axis.NodeId == m_Axis.MasterNodeId && m_CommandHome && m_Axis.HomeMethod == enHomeMethod.PLC)
                    {
                        MxpCommLog.WriteLog(string.Format("{0} AX_Halt Skip ! Homming State", m_Axis.AxisName));
                    }
                    else
                    {
                        rv = MXP.AX_Halt((uint)m_Axis.NodeId, targetdec, targetjerk);
                        if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                            MxpCommLog.WriteLog(string.Format("{0} AX_Halt NG {1}", m_Axis.AxisName, rv.ToString()));
                    }
                    /////////////////////////////////////////////////////////////////////////////////////
                    /// Master축인 경우 Slave 축에도 명령을 내려 주자 ... -49 Alarm이 뜬다...
                    //if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                    //{
                    //    rv = MXP.AX_Halt((uint)m_Axis.SlaveNodeId, targetdec, targetjerk);
                    //    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                    //        MxpCommLog.WriteLog(string.Format("Slave Axis Stop NG {0}", rv.ToString()));
                    //}
                    /////////////////////////////////////////////////////////////////////////////////////
                    m_CommandRun = false;
                    m_CommandHome = false;
                    (m_Axis as MpAxis).SequenceMoveCommandSet = false;
                    ResetExternalExcorder();
                    if (override_reset) m_OverrideStop = false;
                    if (inrange_reset) m_InRangeError = false;
                    m_InRangeChecking = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP AX_Halt {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT Pause(bool pause)
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (m_Axis != null && !m_Axis.CommandSkip && m_AxisBlock.Connected)
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    MxpCommLog.WriteLog(string.Format("{0} AX_Stop", m_Axis.AxisName));

                    /////////////////////////////////////////////////////////////////////////////////////
                    double curVelocity = m_Axis.CurSpeed;
                    Single targetdec = (Single)m_Axis.DecLimit;
                    if (targetdec > 2 * curVelocity) targetdec = (Single)Math.Abs(2 * curVelocity);
                    Single targetjerk = (Single)m_Axis.JerkLimit;
                    if (targetjerk > 4 * curVelocity) targetjerk = (Single)Math.Abs(4 * curVelocity);

                    byte set = pause ? (byte)1 : (byte)0;
                    rv = MXP.AX_Stop((uint)m_Axis.NodeId, set, targetdec, targetjerk);
                    /////////////////////////////////////////////////////////////////////////////////////
                    /// Master축인 경우 Slave 축에도 명령을 내려 주자
                    if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                    {
                        rv = MXP.AX_Stop((uint)m_Axis.SlaveNodeId, set, targetdec, targetjerk);
                    }

                    /////////////////////////////////////////////////////////////////////////////////////
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} {1} NG {2}", m_Axis.AxisName, pause ? "Pause" : "Release", rv.ToString()));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP {0} {1} : {2}", m_Axis.AxisName, pause ? "Pause" : "Release", ex.Message));
            }
            return rv;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT AlarmClear()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (m_Axis != null && !m_Axis.CommandSkip && m_AxisBlock.Connected)
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    MxpCommLog.WriteLog(string.Format("{0} AX_Reset", m_Axis.AxisName));

                    /////////////////////////////////////////////////////////////////////////////////////
                    m_SequenceMoving = false; // Sequence Move 관련 상태를 보면 않된다.
                    m_CommandHome = false;
                    m_CommandRun = false;
                    m_OverrideStop = false;
                    m_InRangeError = false;
                    m_InRangeChecking = false;
                    (m_Axis as MpAxis).SequenceMoveCommandSet = false;

                    rv = MXP.AX_Reset((uint)m_Axis.NodeId);                    
                    /////////////////////////////////////////////////////////////////////////////////////
                    /// Master축인 경우 Slave 축에도 명령을 내려 주자
                    if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                    {
                        /////////////////////////////////////////////////////////////////////////////////////
                        MxpCommLog.WriteLog(string.Format("{0} AX_Reset - Slave", m_Axis.AxisName));
                        rv = MXP.AX_Reset((uint)m_Axis.SlaveNodeId);
                    }
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} AX_Reset NG {1}", m_Axis.AxisName, rv.ToString()));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP AX_Reset {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT Disable(bool forced = false)
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (m_Axis != null && !m_Axis.CommandSkip && (m_AxisBlock.Connected || forced))
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    MxpCommLog.WriteLog(string.Format("{0} AX_Power OFF", m_Axis.AxisName));

                    /////////////////////////////////////////////////////////////////////////////////////
                    rv = MXP.AX_Power((uint)m_Axis.NodeId, 0);
                    /////////////////////////////////////////////////////////////////////////////////////
                    /// Master축인 경우 Slave 축에도 명령을 내려 주자
                    if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                    {
                        /////////////////////////////////////////////////////////////////////////////////////
                        MxpCommLog.WriteLog(string.Format("{0} AX_Power OFF - Slave", m_Axis.AxisName));
                        rv = MXP.AX_Power((uint)m_Axis.SlaveNodeId, 0);
                    }
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} AX_Power OFF NG {1}", m_Axis.AxisName, rv.ToString()));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP AX_Power OFF {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT Enable()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (m_Axis != null && !m_Axis.CommandSkip && m_AxisBlock.Connected)
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    MxpCommLog.WriteLog(string.Format("{0} AX_Power ON", m_Axis.AxisName));

                    /////////////////////////////////////////////////////////////////////////////////////
                    rv = MXP.AX_Power((uint)m_Axis.NodeId, 1);
                    /////////////////////////////////////////////////////////////////////////////////////
                    /// Master축인 경우 Slave 축에도 명령을 내려 주자
                    if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                    {
                        /////////////////////////////////////////////////////////////////////////////////////
                        MxpCommLog.WriteLog(string.Format("{0} AX_Power ON - Slave", m_Axis.AxisName));
                        rv = MXP.AX_Power((uint)m_Axis.SlaveNodeId, 1);
                    }
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} AX_Power ON NG {1}", m_Axis.AxisName, rv.ToString()));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP AX_Power ON {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT SetPosition(double position)
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (m_Axis != null && m_AxisBlock.Connected && !m_Axis.CommandSkip)
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    MxpCommLog.WriteLog(string.Format("{0} AX_SetPosition", m_Axis.AxisName));

                    /////////////////////////////////////////////////////////////////////////////////////
                    Single setPos = (Single)position;
                    rv = MXP.AX_SetPosition((uint)m_Axis.NodeId, setPos);
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} AX_SetPosition NG {1}", m_Axis.AxisName, rv.ToString()));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP AX_SetPosition {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }

        public MXP.MXP_FUNCTION_STATUS_RESULT SetExternalExcorder(double targetpos)
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.MasterNodeId) // Gantry Mater 축이 아닌경우
            {
                return rv;
            }
            try
            {
                if (m_Axis != null && m_AxisBlock.Connected && !m_Axis.CommandSkip)
                {
                    string msg = string.Empty;
                    if (m_Axis.BcrControl == enMxpBcrControl.PLC)
                    {
                        //2024.12.15 PLC에 정위치 정지하라는 신호를 켜준다.
                        rv = MXP.PLC_WriteBit((UInt32)enExactPosition.EXACT_POSITION_STATE, Convert.ToByte((int)enExactPositionStatus.Start), 1); // set = 0 : 미사용, 1 : 사용
                    }
                    else if (m_Axis.BcrControl == enMxpBcrControl.MXP)
                    {
                        float cmd_offset = 0.0f;
                        if (m_Axis.SequenceCommand.PositionSensorInfo.SlaveNo == m_Axis.LeftBcrNodeId) cmd_offset = (float)(m_Axis.LeftBCRCmdOffset + m_Axis.LeftBCROffset);
                        else if (m_Axis.SequenceCommand.PositionSensorInfo.SlaveNo == m_Axis.RightBcrNodeId) cmd_offset = (float)(m_Axis.RightBCRCmdOffset + m_Axis.RightBCROffset);

                        msg += string.Format("[{0}, {1}, {2}, {3}, {4}, {5}, {6}]",
                            m_Axis.SequenceCommand.PositionSensorInfo.SlaveNo,
                            m_Axis.SequenceCommand.PositionSensorInfo.Offset,
                            m_Axis.SequenceCommand.PositionSensorInfo.Size,
                            m_Axis.SequenceCommand.PositionSensorInfo.SensorPulseToUnit,
                            m_Axis.SequenceCommand.PositionSensorInfo.SensorTargetValue,
                            targetpos,
                            m_Axis.SequenceCommand.PositionSensorInfo.SensorScanDistance);
                        // BCR Set
                        rv = MXP.AX_SetExternalEncoder2((uint)m_Axis.NodeId,
                            1,
                            m_Axis.SequenceCommand.PositionSensorInfo.SlaveNo,
                            m_Axis.SequenceCommand.PositionSensorInfo.Offset,
                            m_Axis.SequenceCommand.PositionSensorInfo.Size,
                            m_Axis.SequenceCommand.PositionSensorInfo.SensorPulseToUnit,
                            m_Axis.SequenceCommand.PositionSensorInfo.SensorTargetValue + cmd_offset,
                            (float)targetpos,
                            m_Axis.SequenceCommand.PositionSensorInfo.SensorScanDistance);
                    }
                    if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                    {
                        m_Axis.SequenceState.IsExternalEncoderRun = true;
                        MxpCommLog.WriteLog(string.Format("{0} BCR Set ExternalEncoder OK : {1}", m_Axis.AxisName, msg));
                    }
                    else MxpCommLog.WriteLog(string.Format("{0} BCR Set ExternalEncoder NG : {1}", m_Axis.AxisName, msg));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP BCR Set ExternalEncoder {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT ResetExternalExcorder()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.MasterNodeId) // Gantry Mater 축이 아닌경우
            {
                return rv;
            }
            try
            {
                if (m_Axis != null && m_AxisBlock.Connected && !m_Axis.CommandSkip)
                {
                    m_Axis.SequenceState.IsExternalEncoderRun = false;
                    if (m_Axis.BcrControl == enMxpBcrControl.PLC)
                    {
                        //2024.12.15 PLC 동작 완료 신호를 꺼준다. 
                        rv = MXP.PLC_WriteBit((UInt32)enExactPosition.EXACT_POSITION_STATE, Convert.ToByte((int)enExactPositionStatus.Start), 0);
                        rv &= MXP.PLC_WriteBit((UInt32)enExactPosition.EXACT_POSITION_STATE, Convert.ToByte((int)enExactPositionStatus.End), 0);

                        if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        {
                            MxpCommLog.WriteLog($"{m_Axis.AxisName} BCR Set ExactPosition OK :");
                        }
                        else MxpCommLog.WriteLog($"{m_Axis.AxisName} BCR Set ExactPosition NG :");
                    }
                    else if (m_Axis.BcrControl == enMxpBcrControl.MXP)
                    {
                        Int32 encState = 0;
                        rv = MXP.AX_GetExternalEncoderState((uint)m_Axis.NodeId, ref encState);
                        if (rv == MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR && (MXP.ExternalEncoder_Mode)encState != ExternalEncoder_Mode.ExternalEncoder_Mode_NONE)
                        {
                            string msg = string.Empty;
                            msg += string.Format("[{0}, {1}, {2}, {3}, {4}, {5}]",
                                m_Axis.SequenceCommand.PositionSensorInfo.SlaveNo,
                                m_Axis.SequenceCommand.PositionSensorInfo.Offset,
                                m_Axis.SequenceCommand.PositionSensorInfo.Size,
                                m_Axis.SequenceCommand.PositionSensorInfo.SensorPulseToUnit,
                                m_Axis.SequenceCommand.PositionSensorInfo.SensorTargetValue,
                                m_Axis.SequenceCommand.PositionSensorInfo.SensorScanDistance);
                            // BCR Set
                            rv = MXP.AX_SetExternalEncoder2((uint)m_Axis.NodeId,
                                0,
                                m_Axis.SequenceCommand.PositionSensorInfo.SlaveNo,
                                m_Axis.SequenceCommand.PositionSensorInfo.Offset,
                                m_Axis.SequenceCommand.PositionSensorInfo.Size,
                                m_Axis.SequenceCommand.PositionSensorInfo.SensorPulseToUnit,
                                m_Axis.SequenceCommand.PositionSensorInfo.SensorTargetValue,
                                0.0f,
                                m_Axis.SequenceCommand.PositionSensorInfo.SensorScanDistance);
                            if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} BCR Reset ExternalEncoder OK : {1}", m_Axis.AxisName, msg));
                            }
                            else MxpCommLog.WriteLog(string.Format("{0} BCR Reset ExternalEncoder NG : {1}", m_Axis.AxisName, msg));
                        }
                        else
                        {
                            MxpCommLog.WriteLog(string.Format("{0} BCR Reset ExternalEncoder : [{1}, {2}]", m_Axis.AxisName, rv, (MXP.ExternalEncoder_Mode)encState));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP BCR Reset ExternalEncoder {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT Home()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (m_Axis != null && m_AxisBlock.Connected && !m_Axis.CommandSkip)
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    MxpCommLog.WriteLog(string.Format("{0} AX_Home", m_Axis.AxisName));
                    Init();
                    /////////////////////////////////////////////////////////////////////////////////////
                    ResetExternalExcorder(); // 혹시나 Set 되어 있으면 Reset 처리하자~~~
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (m_Axis.HomeMethod == enHomeMethod.ABS_ZERO)
                    {
                        rv = SetPosition(0.0f);
                    }
                    else if (m_Axis.HomeMethod == enHomeMethod.PLC)
                    {
                        /////////////////////////////////////////////////////////////////////////////////////
                        MxpCommLog.WriteLog(string.Format("{0} PLC_WriteBit(5350, 2, 1)", m_Axis.AxisName));

                        // 주행 축은 PLC On 신호만 줘도 PLC에서 자동으로 Off 처리함.
                        if (m_Axis.GantryType && m_Axis.NodeId == m_Axis.MasterNodeId)
                            rv = MXP.PLC_WriteBit((UInt32)5350, Convert.ToByte(2), (byte)1); // set = 0 : 미사용, 1 : 사용
                    }
                    else
                    {
                        rv = MXP.AX_Home((uint)m_Axis.NodeId);
                    }
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} AX_Home NG {1}", m_Axis.AxisName, rv.ToString()));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP AX_Home {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT Jog(MXP.MXP_DIRECTION_ENUM dir)
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (m_Axis != null && m_AxisBlock.Connected && !m_Axis.CommandSkip)
                {
                    //(m_Axis as MpAxis).TorqueLimitBit = true;
                    /////////////////////////////////////////////////////////////////////////////////////
                    Single targetvel = (Single)m_AxisBlock.GetJogSpeed(m_Axis.AxisId);
                    Single targetacc = (Single)m_AxisBlock.GetJogAcc(m_Axis.AxisId);
                    Single targetdec = (Single)m_AxisBlock.GetJogDec(m_Axis.AxisId);
                    Single targetjerk = (Single)m_AxisBlock.GetJogJerk(m_Axis.AxisId);
                    if (targetacc < 0.1f) targetacc = (float)m_Axis.AccDefault;
                    if (targetacc > 2 * targetvel) targetacc = Math.Abs(2 * targetvel);
                    if (targetdec < 0.1f) targetdec = (float)m_Axis.DecDefault;
                    if (targetdec > 2 * targetvel) targetdec = Math.Abs(2 * targetvel);
                    if (targetjerk < 0.1f) targetjerk = (float)m_Axis.JerkDefault;
                    if (targetjerk > 4 * targetvel) targetdec = Math.Abs(4 * targetvel);

                    /////////////////////////////////////////////////////////////////////////////////////
                    string msg = string.Empty;
                    msg += string.Format("[{0}, {1}, {2}, {3}]", targetvel, targetacc, targetdec, targetjerk);
                    MxpCommLog.WriteLog(string.Format("{0} {1}-AX_MoveVelocity Command : \r\n[Velocity, Acc, Dec, Jerk]\r\n{2}", m_Axis.AxisName, dir == MXP_DIRECTION_ENUM.MXP_POSITIVE_DIRECTION ? "Plus" : "Minus", msg));

                    rv = MXP.AX_MoveVelocity((uint)m_Axis.NodeId, targetvel, targetacc, targetdec, targetjerk, dir, MXP_BUFFERMODE_ENUM.MXP_ABORTING);
                    
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} AX_MoveVelocity NG {1}", m_Axis.AxisName, rv.ToString()));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP AX_MoveVelocity {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT Move()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (m_Axis != null && m_AxisBlock.Connected && !m_Axis.CommandSkip)
                {
                    Init();

                    double velocity_limit = (m_Axis as MpAxis).OverrideCollisionDistance / 2.0f;
                    /////////////////////////////////////////////////////////////////////////////////////
                    Single targetpos = (Single)m_AxisBlock.GetTargetPosition(m_Axis.AxisId);
                    Single targetvel = (Single)m_AxisBlock.GetTargetSpeed(m_Axis.AxisId);
                    Single targetacc = (Single)m_AxisBlock.GetTargetAcc(m_Axis.AxisId);
                    Single targetdec = (Single)m_AxisBlock.GetTargetDec(m_Axis.AxisId);
                    Single targetjerk = (Single)m_AxisBlock.GetTargetJerk(m_Axis.AxisId);
                    if (targetvel > velocity_limit) targetvel = (Single)velocity_limit;
                    if (targetacc < 0.1f) targetacc = (float)m_Axis.AccDefault;
                    if (targetdec < 0.1f) targetdec = (float)m_Axis.DecDefault;
                    if (targetjerk < 0.1f) targetjerk = (float)m_Axis.JerkDefault;
                    /////////////////////////////////////////////////////////////////////////////////////
                    string msg = string.Empty;
                    msg += string.Format("[{0}, {1}, {2}, {3}, {4}]", targetpos, targetvel, targetacc, targetdec, targetjerk);
                    MxpCommLog.WriteLog(string.Format("{0} AX_MoveAbsolute Move Command : \r\n[Position, Velocity, Acc, Dec, Jerk]\r\n{1}", m_Axis.AxisName, msg));

                    rv = MXP.AX_MoveAbsolute((uint)m_Axis.NodeId, targetpos, targetvel, targetacc, targetdec, targetjerk, MXP.MXP_DIRECTION_ENUM.MXP_NONE_DIRECTION, MXP.MXP_BUFFERMODE_ENUM.MXP_ABORTING);
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} AX_MoveAbsolute Move NG {1}", m_Axis.AxisName, rv.ToString()));

                    m_CommandRun = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP AX_MoveAbsolute Move {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT RelativeMove()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (m_Axis != null && m_AxisBlock.Connected && !m_Axis.CommandSkip)
                {
                    Init();

                    double velocity_limit = (m_Axis as MpAxis).OverrideCollisionDistance / 2.0f;
                    /////////////////////////////////////////////////////////////////////////////////////
                    Single targetpos = (Single)m_AxisBlock.GetTargetDistance(m_Axis.AxisId);
                    Single targetvel = (Single)m_AxisBlock.GetTargetSpeed(m_Axis.AxisId);
                    Single targetacc = (Single)m_AxisBlock.GetTargetAcc(m_Axis.AxisId);
                    Single targetdec = (Single)m_AxisBlock.GetTargetDec(m_Axis.AxisId);
                    Single targetjerk = (Single)m_AxisBlock.GetTargetJerk(m_Axis.AxisId);
                    if (targetvel > velocity_limit) targetvel = (Single)velocity_limit;
                    if (targetacc < 0.1f) targetacc = (float)m_Axis.AccDefault;
                    if (targetdec < 0.1f) targetdec = (float)m_Axis.DecDefault;
                    if (targetjerk < 0.1f) targetjerk = (float)m_Axis.JerkDefault;
                    /////////////////////////////////////////////////////////////////////////////////////
                    string msg = string.Empty;
                    msg += string.Format("[{0}, {1}, {2}, {3}, {4}]", targetpos, targetvel, targetacc, targetdec, targetjerk);
                    MxpCommLog.WriteLog(string.Format("{0} AX_MoveRelative Move Command : \r\n[Distance, Velocity, Acc, Dec, Jerk]\r\n{1}", m_Axis.AxisName, msg));

                    rv = MXP.AX_MoveRelative((uint)m_Axis.NodeId, targetpos, targetvel, targetacc, targetdec, targetjerk, MXP.MXP_BUFFERMODE_ENUM.MXP_ABORTING);
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} AX_MoveRelative Move NG {1}", m_Axis.AxisName, rv.ToString()));

                    m_CommandRun = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP AX_MoveRelative Move {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT SequenceMove()
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (m_Axis != null && m_AxisBlock.Connected && !m_Axis.CommandSkip)
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    SequenceCommand command = m_AxisBlock.GetSequenceCommand(m_Axis.AxisId);
                    int stepCount = command.MotionProfileCount;
                    MXP.SEQUENCE_MOVE_MODE moveMode = (MXP.SEQUENCE_MOVE_MODE)command.MoveMethod;
                    MXP.MXP_BUFFERMODE_ENUM bufferMode = (MXP.MXP_BUFFERMODE_ENUM)command.BufferMode;
                    //byte positionSensorUsing = (byte)command.PositionSensorInfo.SensorUse;
                    /////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////////////////
                    //if (command.PositionSensorInfo.ControlMp && command.PositionSensorInfo.SensorUse == 1)
                    //{
                    //    MotionSensor sensor_para = command.PositionSensorInfo;
                    //    byte use_sensor = (byte)sensor_para.SensorUse;
                    //    uint slave_no = (uint)sensor_para.SlaveNo; // 9, 10 번
                    //    ushort offset = (ushort)sensor_para.Offset;// 0x2013;
                    //    ushort size = (ushort)sensor_para.Size; //4
                    //    float sensor_target = sensor_para.SensorTargetValue + (float)m_Axis.BCROffset;
                    //    float in_range = sensor_para.SensorPositionSetRange; //0.5
                    //    float pulse_unit = sensor_para.SensorPulseToUnit; //0.1
                    //    float scan_start_distance = sensor_para.SensorScanDistance;
                    //    rv = MXP.AX_SetUsingPositionSensor((uint)m_Axis.NodeId, 
                    //        use_sensor, slave_no, offset, size, sensor_target, in_range, pulse_unit, scan_start_distance);
                    //}
                    m_SequenceCommand = ObjectCopier.Clone(command);
                    /////////////////////////////////////////////////////////////////////////////////////

                    if (command.MotionProfiles.Count > 0)
                    {
                        double velocity_limit = (m_Axis as MpAxis).OverrideCollisionDistance / 2.0f;
                        if (velocity_limit > 0)
                        {
                            // 만일 이동중일 경우 최대 속도는 Trajetory Velocity
                            if (command.MotionProfiles.First().Velocity > velocity_limit) 
                                command.MotionProfiles.First().Velocity = velocity_limit;
                        }
                        if ((m_Axis as MpAxis).TrajectoryTargetVelocity > 0)
                        {
                            if (velocity_limit > (m_Axis as MpAxis).TrajectoryTargetVelocity) 
                                velocity_limit = (m_Axis as MpAxis).TrajectoryTargetVelocity;
                        }
                    }

                    /////////////////////////////////////////////////////////////////////////////////////
                    MXP.SEQUENCEMOVE_STEP[] pData = new SEQUENCEMOVE_STEP[stepCount];
                    for (int i = 0; i < stepCount; i++)
                    {
                        pData[i].pos = (float)command.MotionProfiles[i].Distance;
                        pData[i].vel = (float)command.MotionProfiles[i].Velocity;
                        pData[i].acc = (float)command.MotionProfiles[i].Acceleration > 0.1f ? (float)command.MotionProfiles[i].Acceleration : m_Axis.AccDefault;
                        pData[i].dec = (float)command.MotionProfiles[i].Deceleration > 0.1f ? (float)command.MotionProfiles[i].Deceleration : m_Axis.DecDefault;
                        pData[i].jerk = (float)command.MotionProfiles[i].Jerk > 0.1f ? (float)command.MotionProfiles[i].Jerk : m_Axis.JerkDefault;
                        pData[i].velocityLimitUsing = command.MotionProfiles[i].VelocityLimitFlag;
                    }
                    /////////////////////////////////////////////////////////////////////////////////////////
                    //가감속과 거리에 따른 속도 재계산///////////////////////////////////////////////////////
                    Single[] arrStepAccDecDist = new Single[stepCount];
                    for (int i = 0; i < 2; i++)
                    {
                        MXP.MXP_FUNCTION_STATUS_RESULT speed_check = MXP.AX_ReadSequenceMoveStepAccDecDist((uint)m_Axis.NodeId, moveMode, bufferMode, pData, (UInt16)stepCount, ref arrStepAccDecDist[0]);
                        if (speed_check == MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        {
                            for (int n = 0; n < stepCount; n++)
                            {
                                float a = pData[n].dec;
                                float v1 = pData[n].vel;
                                float v2 = n + 1 < stepCount ? pData[n + 1].vel : 0.0f;
                                float s = Math.Abs(pData[n].pos);
                                if (s < arrStepAccDecDist[n])
                                {
                                    v1 = (float)Math.Sqrt(s * a + v2 * v2);
                                }
                                if (v1 < pData[n].vel) pData[n].vel = v1;
                                if (pData[n].vel < 100.0f)
                                {
                                    pData[n].vel = 100.0f;
                                    if (pData[n].vel > pData[n].acc) pData[n].acc = pData[n].vel;
                                    if (pData[n].vel > pData[n].dec) pData[n].dec = pData[n].vel;
                                    if (pData[n].vel > pData[n].jerk) pData[n].jerk = pData[n].vel;
                                }
                            }
                        }
                        else
                        {
                            // 속도 검증. Blending_Low이므로 마지막 구간부터 계산하여 보정하자.
                            // a : 가속도, v1 : start 속도, v2 : end 속도, t : 가속시간, min_s : 가속거리
                            // Motion Data => s : 이동거리
                            // t = 2 * (v1-v2)/a, min_s = v * t = {(v1+v2)/2} * {2 * (v1-v2)/a} = (v1+v2)(v1-v2)/a, s_curve 사용시 시간은 2배로
                            // if (s < min_s) start 속도 보정 else 완료
                            //  v1(start 속도) = SQRT (s * 2 * a + v2 ^ 2)
                            for (int n = stepCount - 1; n >= 0 ; n--)
                            {
                                float a = pData[n].dec;
                                float v1 = pData[n].vel;
                                float v2 = n + 1 < stepCount  ? pData[n + 1].vel : 0.0f;
                                float s = Math.Abs(pData[n].pos);
                                float sCurveRate = 100;

                                ACCTIME_TO_ACCDEC_REPLY accDecData = new ACCTIME_TO_ACCDEC_REPLY();
                                MXP.MXP_FUNCTION_STATUS_RESULT dec_check = MXP.AX_DistanceToAccDec((uint)m_Axis.NodeId, Math.Abs(v1 - v2), s, sCurveRate, ref accDecData);
                                if (dec_check == MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                                {
                                    if (accDecData.accDec > m_Axis.DecLimit)
                                    {
                                        v1 = (float)Math.Sqrt(s * a + v2 * v2);
                                    }
                                }
                                if (v1 < 100.0f) v1 = 100.0f;
                                if (pData[n].vel < 100.0f)
                                {
                                    pData[n].vel = 100.0f;
                                    if (pData[n].vel > pData[n].acc) pData[n].acc = pData[n].vel;
                                    if (pData[n].vel > pData[n].dec) pData[n].dec = pData[n].vel;
                                    if (pData[n].vel > pData[n].jerk) pData[n].jerk = pData[n].vel;
                                }
                            }
                        }
                    }
                    // 재 계산된 값을 돌려놓자...
                    string msg = "";
                    for (int i = 0; i < stepCount; i++)
                    {
                        command.MotionProfiles[i].Distance = (double)pData[i].pos;
                        command.MotionProfiles[i].Velocity = (double)pData[i].vel;
                        m_Axis.TargetAcc = command.MotionProfiles[i].Acceleration = (double)pData[i].acc;
                        command.MotionProfiles[i].Deceleration = (double)pData[i].dec;
                        command.MotionProfiles[i].Jerk = (double)pData[i].jerk;
                        msg += string.Format("\r\n[{0}, {1}, {2}, {3}, {4}]", pData[i].pos, pData[i].vel, pData[i].acc, pData[i].dec, pData[i].jerk);
                    }
                    List<double> distances = m_Axis.SequenceCommand.MotionProfiles.Select(x => x.Distance).ToList();
                    m_Axis.TargetPos = distances.Count > 0 ? m_Axis.CurPos + distances.Sum() : m_Axis.CurPos;

                    MxpCommLog.WriteLog(string.Format("{0} AX_SequenceMoveRequest_Ex Command : MotorTargetPosition={1}\r\n[Distance, Velocity, Acc, Dec, Jerk] {2}", m_Axis.AxisName, m_Axis.TargetPos, msg));
                    /////////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////////////////////
                    float sensorRunDistance = 5000.0f;
                    double len = pData[stepCount - 1].pos;
                    double vel = pData[stepCount - 1].vel;
                    double dec = pData[stepCount - 1].dec;
                    double dt = 2 * (vel / dec);
                    double ds = 0.5f * vel * dt;
                    if (vel * 1.0f <= 500.0f) ds = (float)(vel * 1.2f);
                    else if (ds > len) ds = 0.9f * len;
                    sensorRunDistance = (float)(1.0f * ds); //30% margin
                    /////////////////////////////////////////////////////////////////////////////////////////

                    byte plc_scan = command.PositionSensorInfo.ControlMp ? (byte)1 : (byte)0;
                    plc_scan = 0;
                    rv = MXP.AX_SequenceMoveRequest_Ex((uint)m_Axis.NodeId, moveMode, bufferMode, pData, (ushort)stepCount, plc_scan, sensorRunDistance);
                    //rv = MXP.AX_SequenceMoveRequest_Ex2((uint)m_Axis.NodeId, moveMode, bufferMode, pData, (ushort)stepCount);
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} AX_SequenceMoveRequest_Ex NG {1}", m_Axis.AxisName, rv.ToString()));

                    (m_Axis as MpAxis).SequenceMoveCommandSet = false;
                    m_SequenceMoving = true;
                    m_ProfileMoveMonitoringInit = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP AX_SequenceMoveRequest_Ex {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }
        public MXP.MXP_FUNCTION_STATUS_RESULT VelocityOverride(double rate, double time, bool forced = false)
        {
            MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
            try
            {
                if (m_Axis != null && !m_Axis.CommandSkip && (m_AxisBlock.Connected || forced))
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    rv = MXP.AX_SetOverride_Ex((uint)m_Axis.NodeId, (float)rate, (float)time);
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (rv != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                        MxpCommLog.WriteLog(string.Format("{0} VelocityOverride NG {1}", m_Axis.AxisName, rv.ToString()));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("MXP VelocityOverride {0} : {1}", m_Axis.AxisName, ex.Message));
            }
            return rv;
        }
        #endregion
        #region Thread - Task Override & Torque Limit
        private class TaskOverride : XSequence
        {
            public TaskOverride(MxpAxis mxp_axis)
            {
                SetSubInfo(mxp_axis.m_Axis.AxisName);
                bool gantry_slave = mxp_axis.m_Axis.GantryType && mxp_axis.m_Axis.NodeId == mxp_axis.m_Axis.SlaveNodeId;
                if (!gantry_slave)
                {
                    RegSeq(new SeqOverride(mxp_axis));
                    RegSeq(new SeqTorqueLimitCheck(mxp_axis));
                    RegSeq(new SeqFollowingErrorCheck(mxp_axis));
                }
            }
        }
        private class SeqOverride : XSeqFunc
        {
            private MxpAxis m_MxpAxis = null;
            private AxisBlockMXP m_AxisBlock = null;
            private _Axis m_Axis = null;

            private double m_Maximum_Distance = 10 * 1000.0f;
            private double m_Maximum_Velocity = 3500.0f;
            private double m_OverrideTime = 0.0f;
            private double m_CollisionDistance = 0.0f;

            private uint m_NotChangeCheckTime = 0;
            public SeqOverride(MxpAxis mxp_axis)
            {
                this.SeqName = $"SeqOverride_{mxp_axis.m_Axis.AxisName}";

                m_MxpAxis = mxp_axis;
                m_AxisBlock = mxp_axis.m_AxisBlock;
                m_Axis = mxp_axis.m_Axis;
                m_MxpAxis.m_OverrideRate = 1.1f; // 시작할때 1.0
            }
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                if (m_AxisBlock.Connected == false) return -1;
                if (m_MxpAxis.AxisState.powerOn == 0 || m_MxpAxis.AxisState.disabled == 1) return -1;

                enFrontDetectState sensor_state = (m_Axis as MpAxis).OverrideSensorState;
                double max_distance = (m_Axis as MpAxis).OverrideMaxDistance;
                double min_distance = (m_Axis as MpAxis).OverrideMinDistance;
                double max_link_velocity = (m_Axis as MpAxis).OverrideMaxVelocity;
                double temp_distance = (m_Axis as MpAxis).OverrideCollisionDistance;
                if (temp_distance <= 10000.0f) m_CollisionDistance = temp_distance;
                else if (temp_distance == 10000.0f && sensor_state == enFrontDetectState.enNone) m_CollisionDistance = temp_distance;

                //double collision_distance = (m_Axis as MpAxis).OverrideCollisionDistance;
                double current_override = (m_Axis as MpAxis).CurrentOverrideRate;
                double target_override = (m_Axis as MpAxis).TargetOverrideRate;

                // override 값이 변했는지 확인
                // override 0.0이면 멈추라는 뜻
                //  멈출때는 속도 거리 판정하여 가까이 근접하여 급정지하도록 하자 !
                // override > 0.0이면 아마 증속하라는 뜻
                //  collision distance를 확인하여 증속 가능한 최대속도로 time set
                double target_velocity = (m_Axis as MpAxis).TrajectoryTargetVelocity;
                double current_velocity = (m_Axis as MpAxis).CurSpeed;
                double limit_distance = (m_Axis as MpAxis).OverrideLimitDistance;
                double stop_distance = (m_Axis as MpAxis).OverrideStopDistance;
                double acceleration = (m_Axis as MpAxis).OverrideAcceleration;
                double deceleration = (m_Axis as MpAxis).OverrideDeceleration;
                if (acceleration == 0.0f) acceleration = 2000.0f;
                if (deceleration == 0.0f) deceleration = 3000.0f;

                if (current_velocity < 1000.0f) stop_distance -= 250.0f;
                else if (current_velocity < 2000.0f) stop_distance -= 200.0f;
                else if (current_velocity < 3000.0f) stop_distance -= 150.0f;
                if (stop_distance < 500.0f) stop_distance = 500.0f; //최소 계산거리

                double dt0 = current_velocity < 0.1f ? 0.1f : current_velocity / deceleration;
                double s0 = 1.2f * (0.5f * deceleration * dt0 * dt0); //감속거리, SCurve 적용할 경우 2배 늘어날 것 같음.
                if (m_MxpAxis.AxisState.accelerating == 1 && current_velocity > 100.0f) s0 += 200.0f; // 가속 시점에 밀리는 거리
                else if (m_MxpAxis.AxisState.accelerating == 1 && current_velocity > 500.0f) s0 += 300.0f;
                else if (m_MxpAxis.AxisState.accelerating == 1 && current_velocity > 1000.0f) s0 += 500.0f;
                double max_dec_time = 2 * (m_Maximum_Velocity / deceleration); // 2 * (3500 / 3000) = 2.2sec
                double max_acc_time = 4 * (m_Maximum_Velocity / acceleration); // 4 * (3500 / 3000) = 4.4sec
                double min_time = 0.3f;
                if (current_velocity > 1000.0f) min_time = 0.5f;
                else if (current_velocity > 100.0f) min_time = 0.4f;

                // 잔여 거리 계산
                double bcr_scan_offset = 0.0f;
                uint slave_no = (uint)m_Axis.SequenceCommand.PositionSensorInfo.SlaveNo; // 9, 10 번
                double curBCR = slave_no == m_Axis.LeftBcrNodeId ? m_Axis.LeftBcrPos : m_Axis.RightBcrPos;
                double remain_bcr_distance = m_Axis.SequenceCommand.PositionSensorInfo.SensorTargetValue - curBCR;
                if (m_MxpAxis.m_SequenceMoving &&
                    m_Axis.SequenceCommand.PositionSensorInfo.SensorUse == 1 &&
                    remain_bcr_distance > 0.0f &&
                    remain_bcr_distance < m_CollisionDistance - limit_distance) bcr_scan_offset = 0.0f;
                else bcr_scan_offset = limit_distance;

                bool stop_condition = false;
                stop_condition |= (s0 + 0.05 * max_distance) > max_distance; //Stop거리에서 500mm margin을 가지고 Interlock을 걸자~~~커브 구분필요함....
                if (m_CollisionDistance < 0.7f * max_distance && sensor_state > enFrontDetectState.enNone)
                {
                    // target = 0.0일때 감속 거리내에 들어온 경우...   
                    // 속도 300일때 Stop 위치 => stop_distance=1300-250=1050, s0 < 20 ... 근접거리 1000
                    if (sensor_state > enFrontDetectState.enDeccelation2)
                        stop_condition |= m_CollisionDistance < current_velocity;
                    stop_condition |= (target_override == 0.0f) && (m_CollisionDistance < s0 + stop_distance + bcr_scan_offset);             
                    stop_condition |= m_CollisionDistance < s0 + stop_distance;
                    //stop_condition &= current_override > 0.0f;
                    stop_condition &= current_velocity > 1.0f;
                }
                stop_condition |= (m_Axis as MpAxis).HoldingBit;
                stop_condition |= (m_Axis as MpAxis).PauseBit;

                //if (AppConfig.Instance.Simulation.MY_DEBUG)
                //{
                //    if (current_velocity > 10)
                //    {
                //        System.Diagnostics.Debug.WriteLine($"=> {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")} stop_condition({stop_condition}) : dt0({dt0}), current_velocity({current_velocity}), CollisionDistance({m_CollisionDistance}), s0({s0}), stop_distance({stop_distance}), bcr_scan_offset({bcr_scan_offset}), target_override({target_override})");
                //    }
                //}

                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_Axis.GantryType)
                            {
                                seqNo = 10;
                            }
                            else
                            {
                                // Gantry가 아닌 경우는 override값을 바로 쓰자 !
                                bool override_change = target_override != m_MxpAxis.m_OverrideRate;
                                override_change |= (m_Axis as MpAxis).HoldingBit;
                                override_change |= (m_Axis as MpAxis).PauseBit;
                                if (override_change)
                                {
                                    if ((m_Axis as MpAxis).HoldingBit) target_override = 0.0f;
                                    MXP.MXP_FUNCTION_STATUS_RESULT rv = m_MxpAxis.VelocityOverride(target_override, 0.1f * 1000.0f);
                                    if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                                    {
                                        MxpCommLog.WriteLog(string.Format("{0} Velocity Override Change Rate : Before_Override:{1}, Target_Override:{2}, Run_Speed:{3}", m_Axis.AxisName, m_MxpAxis.m_OverrideRate, target_override, m_Axis.CurSpeed));
                                        m_MxpAxis.m_OverrideRate = target_override;
                                    }
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            // velocity is zero and override is zero
                            bool recovery = target_velocity != 0.0f;
                            recovery &= current_velocity == 0.0f;
                            recovery &= current_override == 0.0f;
                            recovery &= m_CollisionDistance > stop_distance + bcr_scan_offset + 500.0f; // + 1000.0f;// Sensor 판단 오차 최소 1000mm(Level range)
                            if (recovery) target_override = 0.1f;

                            bool override_change = true;
                            override_change &= target_override != m_MxpAxis.m_OverrideRate;
                            override_change &= target_override != 0.0f;
                            override_change |= recovery;

                            if (target_velocity > max_link_velocity && max_link_velocity < 1000.0f) // Curve일때만 적용하자~~~
                            {
                                override_change |= target_velocity > 1.1f * max_link_velocity;// 비정상 속도 적용되는 경우
                            }

                            if (stop_condition)
                            {
                                m_NotChangeCheckTime = 0;
                                if (current_override > 0.0f)
                                {
                                    target_override = 0.0f;
                                    dt0 *= 1.0f;
                                    if (dt0 > max_dec_time) dt0 = max_dec_time;
                                    if (dt0 < min_time) dt0 = min_time;
                                    MXP.MXP_FUNCTION_STATUS_RESULT rv = m_MxpAxis.VelocityOverride(target_override, dt0 * 1000.0f);
                                    if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                                    {
                                        string msg = string.Format("{0} Velocity Override Stop SET : \r\n", m_Axis.AxisName);
                                        msg += string.Format("Target Override={0}\r\n", Math.Round(target_override, 4));
                                        msg += string.Format("Current Override={0}\r\n", Math.Round(current_override, 4));
                                        msg += string.Format("Decceleration={0}\r\n", deceleration);
                                        msg += string.Format("Override Set Time={0}\r\n", dt0 * 1000.0f);
                                        msg += string.Format("Collision Distance={0}\r\n", m_CollisionDistance);
                                        msg += string.Format("Decceleration Distance={0}\r\n", s0);
                                        msg += string.Format("stop_distance={0}\r\n", stop_distance);
                                        msg += string.Format("current_velocity={0}\r\n", current_velocity);
                                        MxpCommLog.WriteLog(msg);

                                        m_OverrideTime = dt0 * 1000.0f;
                                        m_MxpAxis.m_OverrideRate = target_override;
                                        m_MxpAxis.m_OldOverrideRate = current_override;
                                        (m_Axis as MpAxis).SetOverrideZeroTouchGUI = false;
                                        StartTicks = XFunc.GetTickCount();
                                        seqNo = 20;
                                    }
                                }
                            }
                            else if (override_change)
                            {
                                m_NotChangeCheckTime = 0;
                                double constant_velocity_ratio = (m_Axis as MpAxis).OverrideConstantVelocityRatio; //0.3f 이동거리 중 정속 비율
                                double override_increase_time_ratio = (m_Axis as MpAxis).OverrideIncreaseTimeRatio;
                                double distance_ratio = m_CollisionDistance == 0.0f ? 0.0f : m_CollisionDistance / max_distance; // m_Maximum_Distance;
                                double acceleration_distance = (1 - constant_velocity_ratio) * m_CollisionDistance * (deceleration / (acceleration + deceleration)); //가속 거리
                                double deceleration_distance = (1 - constant_velocity_ratio) * m_CollisionDistance * (acceleration / (acceleration + deceleration)); //감속 거리
                                double dt1 = deceleration_distance == 0.0f ? 0.1f : Math.Sqrt(2 * deceleration_distance / deceleration);
                                double v1 = distance_ratio * deceleration * dt1;
                                bool nn = acceleration_distance == 0.0 || (current_velocity + v1) == 0.0f;
                                double dt2 = nn ? 0.1f : 2 * acceleration_distance / (current_velocity + v1); //가속시간
                                double enable_override = v1 == 0.0f || target_velocity == 0.0f ? 1.0f : v1 / target_velocity;
                                if (target_velocity > max_link_velocity)
                                {
                                    // 이상 현상이다 가능한 속도로 줄여서 적용하자!
                                    //enable_override = v1 == 0.0f || max_link_velocity == 0.0f ? 1.0f : v1 / max_link_velocity;
                                    //double enable_velocity = enable_override * target_velocity;
                                    //if (enable_velocity > max_link_velocity)
                                    //{
                                    //    enable_override = max_link_velocity / enable_velocity;
                                    //}
                                    enable_override = max_link_velocity / target_velocity;
                                    dt2 = max_link_velocity / deceleration;
                                }
                                //else
                                //{
                                //    enable_override = v1 == 0.0f || target_velocity == 0.0f ? 1.0f : v1 / target_velocity;
                                //}

                                if (enable_override < 0.1f) enable_override = 0.1f;
                                if (target_override > enable_override) target_override = enable_override;
                                if (target_override > m_MxpAxis.m_OverrideRate) dt2 *= override_increase_time_ratio;
                                if (target_override > 1.0f) target_override = 1.0f;
                                if (dt2 > max_acc_time) max_acc_time = dt2;
                                if (dt2 < min_time) dt2 = min_time;

                                bool set_override = Math.Abs(target_override - m_MxpAxis.m_OverrideRate) > 0.01f;
                                if (target_override > m_MxpAxis.m_OverrideRate) // 증속일때 가능 시점 판단.
                                {
                                    // 곡선에서 Recovery가 되지 않을수 있다. 
                                    double recover_min_distance = Math.Min(max_distance, stop_distance + bcr_scan_offset + 1000.0f);
                                    set_override &= m_CollisionDistance > recover_min_distance - 1.0f; // 곡선 최소거리 or stop 가능 거리 판단, 증속하자~~
                                    set_override &= (m_Axis as MpAxis).HoldingBit == false;
                                    set_override &= (m_Axis as MpAxis).PauseBit == false;
                                }
                                if (set_override)
                                {
                                    MXP.MXP_FUNCTION_STATUS_RESULT rv = m_MxpAxis.VelocityOverride(target_override, dt2 * 1000.0f);
                                    if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                                    {
                                        string msg = string.Format("{0} Velocity Override Change SET : \r\n", m_Axis.AxisName);
                                        msg += string.Format("Target Override={0}\r\n", Math.Round(target_override, 4));
                                        msg += string.Format("Current Override={0}\r\n", Math.Round(current_override, 4));
                                        msg += string.Format("Decceleration={0}\r\n", deceleration);
                                        msg += string.Format("Override Set Time={0}\r\n", dt2 * 1000.0f);
                                        msg += string.Format("Collision Distance={0}\r\n", m_CollisionDistance);
                                        msg += string.Format("Decceleration Distance={0}\r\n", s0);
                                        msg += string.Format("stop_distance={0}\r\n", stop_distance);
                                        msg += string.Format("current_velocity={0}\r\n", current_velocity);
                                        MxpCommLog.WriteLog(msg);

                                        m_OverrideTime = dt2 * 1000.0f;
                                        m_MxpAxis.m_OverrideRate = target_override;
                                        m_MxpAxis.m_OldOverrideRate = current_override;
                                        (m_Axis as MpAxis).SetOverrideZeroTouchGUI = false;
                                        StartTicks = XFunc.GetTickCount();
                                        seqNo = 20;
                                    }
                                }
                            }
                            else
                            {
                                // target / current 값의 변화가 없는 경우
                                if (target_override == m_MxpAxis.m_OverrideRate && target_override == 1.0f && current_override <= 0.1f) m_NotChangeCheckTime++;
                                else m_NotChangeCheckTime = 0;

                                if (m_NotChangeCheckTime > 500)
                                {
                                    m_NotChangeCheckTime = 0;
                                    m_MxpAxis.m_OverrideRate = 0.1f;
                                }
                            }
                        }
                        break;

                    case 20:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 100.0f) break;
                            bool override_time_over = XFunc.GetTickCount() - StartTicks > 1.5f * m_OverrideTime;
                            bool increase_time_over = XFunc.GetTickCount() - StartTicks > 5.0f * m_OverrideTime;

                            bool override_change = true;
                            override_change &= Math.Abs(target_override - m_MxpAxis.m_OverrideRate) > 0.001f;
                            override_change &= target_override != 0.0f;
                            override_change &= XFunc.GetTickCount() - StartTicks > 0.5f * m_OverrideTime;

                            double collision_ng_distance = Math.Min(min_distance, limit_distance);
                            double sensor_remain_distance = (m_Axis as MpAxis).SensorRemainDistance; //BCR Remain
                            double changing_overrideRate = m_MxpAxis.m_OverrideRate - m_MxpAxis.m_OldOverrideRate; //변해야 할 값
                            double changed_overrideRate = m_MxpAxis.m_OldOverrideRate - current_override; // 얼마만큼 변하고 있나 ?
                            bool override_apply_complete = Math.Abs(m_MxpAxis.m_OverrideRate - current_override) <= 0.1f * Math.Abs(changing_overrideRate); // 목표값에 얼마나 근접했나 90% ?
                            bool override_changing_ng = Math.Abs(changed_overrideRate) < 0.1f * Math.Abs(changing_overrideRate);
                            override_changing_ng &= XFunc.GetTickCount() - StartTicks > 0.5f * m_OverrideTime; //변화가 없네 이상하다 급정지 하자 !
                            override_changing_ng &= current_override > 0.1f; // 고마 확인해도 되겠지...
                            override_changing_ng &= changing_overrideRate < 0; // 감속일때만 확인하자 !
                            override_changing_ng &= current_velocity > 10.0f;
                            override_changing_ng &= Math.Abs(changing_overrideRate) > 0.1f;
                            override_changing_ng &= sensor_remain_distance > m_CollisionDistance - collision_ng_distance; //BCR Remain Distance가 margin이 있는 경우에는 좀더 기다려 보자~~

                            bool collision_ng = m_CollisionDistance < collision_ng_distance; // 이해가 않된다.... m_CollisionDistance가 1000인데 왜 300보다 적다고 계산되지 ?
                            bool override_fail_timeover = XFunc.GetTickCount() - StartTicks > (m_OverrideTime + 3000.0f);  // 3초가 지났는데도 완료되지 않은 경우
                            bool override_fail = m_MxpAxis.m_OverrideRate < m_MxpAxis.m_OldOverrideRate; // 시간이 지난거 확인은 감속 조건만 처리
                            override_fail &= Math.Abs(changing_overrideRate) > 0.1f; //변화율이 10% 이상일때만 확인하자~~
                            override_fail &= override_fail_timeover;
                            override_fail &= changing_overrideRate < 0; // 감속일때만 확인하자 !
                            override_fail &= current_velocity > 10.0f;
                            override_fail |= collision_ng;

                            if (override_apply_complete)
                            {
                                string msg = string.Format("{0} Velocity Override Change OK : \r\n", m_Axis.AxisName);
                                msg += string.Format("Target Override={0}\r\n", Math.Round(m_MxpAxis.m_OverrideRate, 4));
                                msg += string.Format("Current Override={0}\r\n", Math.Round(current_override, 4));
                                msg += string.Format("Start Override={0}\r\n", Math.Round(m_MxpAxis.m_OldOverrideRate, 4));
                                msg += string.Format("Collision Distance={0}\r\n", m_CollisionDistance);
                                msg += string.Format("Override Set Time={0}\r\n", m_OverrideTime);
                                msg += string.Format("Apply Time={0}\r\n", XFunc.GetTickCount() - StartTicks);
                                msg += string.Format("changing_overrideRate={0}\r\n", changing_overrideRate);
                                msg += string.Format("changed_overrideRate={0}\r\n", changed_overrideRate);
                                msg += string.Format("current_velocity={0}\r\n", current_velocity);
                                MxpCommLog.WriteLog(msg);
                                seqNo = 10;
                            }
                            else if (override_change)
                            {
                                string msg = string.Format("{0} Target Override Changed SET : \r\n", m_Axis.AxisName);
                                msg += string.Format("Old Target Override={0}\r\n", Math.Round(m_MxpAxis.m_OverrideRate, 4));
                                msg += string.Format("New Target Override={0}\r\n", Math.Round(target_override, 4));
                                msg += string.Format("Current Override={0}\r\n", Math.Round(current_override, 4));
                                msg += string.Format("Collision Distance={0}\r\n", m_CollisionDistance);
                                msg += string.Format("changing_overrideRate={0}\r\n", changing_overrideRate);
                                msg += string.Format("changed_overrideRate={0}\r\n", changed_overrideRate);
                                msg += string.Format("current_velocity={0}\r\n", current_velocity);
                                MxpCommLog.WriteLog(msg);
                                seqNo = 10;
                            }
                            else if (stop_condition && (m_MxpAxis.m_OverrideRate > 0.0f || override_time_over)) // 시간이 지나면 stop 조건을 확인해야 한다.
                            {
                                string msg = string.Format("{0} Velocity Override Stop Condition : \r\n", m_Axis.AxisName);
                                msg += string.Format("Target Override={0}\r\n", Math.Round(m_MxpAxis.m_OverrideRate, 4));
                                msg += string.Format("Current Override={0}\r\n", Math.Round(current_override, 4));
                                msg += string.Format("Start Override={0}\r\n", Math.Round(m_MxpAxis.m_OldOverrideRate, 4));
                                msg += string.Format("Collision Distance={0}\r\n", m_CollisionDistance);
                                msg += string.Format("Override Set Time={0}\r\n", m_OverrideTime);
                                msg += string.Format("Apply Time={0}\r\n", XFunc.GetTickCount() - StartTicks);
                                msg += string.Format("changing_overrideRate={0}\r\n", changing_overrideRate);
                                msg += string.Format("changed_overrideRate={0}\r\n", changed_overrideRate);
                                msg += string.Format("current_velocity={0}\r\n", current_velocity);
                                MxpCommLog.WriteLog(msg);
                                seqNo = 10;
                            }
                            else if (override_changing_ng)
                            {
                                string msg = string.Format("{0} Velocity Override Change NG Condition : \r\n", m_Axis.AxisName);
                                msg += string.Format("Target Override={0}\r\n", Math.Round(m_MxpAxis.m_OverrideRate, 4));
                                msg += string.Format("Current Override={0}\r\n", Math.Round(current_override, 4));
                                msg += string.Format("Start Override={0}\r\n", Math.Round(m_MxpAxis.m_OldOverrideRate, 4));
                                msg += string.Format("Collision Distance={0}\r\n", m_CollisionDistance);
                                msg += string.Format("Override Set Time={0}\r\n", m_OverrideTime);
                                msg += string.Format("Apply Time={0}\r\n", XFunc.GetTickCount() - StartTicks);
                                msg += string.Format("changing_overrideRate={0}\r\n", changing_overrideRate);
                                msg += string.Format("changed_overrideRate={0}\r\n", changed_overrideRate);
                                msg += string.Format("current_velocity={0}\r\n", current_velocity);
                                MxpCommLog.WriteLog(msg);

                                m_MxpAxis.m_OverrideStop = true;
                                MXP.MXP_FUNCTION_STATUS_RESULT rv = m_MxpAxis.Stop(false, false, true);
                                MxpCommLog.WriteLog(string.Format("{0} Override Stop", m_Axis.AxisName));
                                seqNo = 10;
                            }
                            else if (override_fail)
                            {
                                string msg = string.Format("{0} Velocity Override Collision Limit Fail : \r\n", m_Axis.AxisName);
                                msg += string.Format("Target Override={0}\r\n", Math.Round(m_MxpAxis.m_OverrideRate, 4));
                                msg += string.Format("Current Override={0}\r\n", Math.Round(current_override, 4));
                                msg += string.Format("Start Override={0}\r\n", Math.Round(m_MxpAxis.m_OldOverrideRate, 4));
                                msg += string.Format("Collision Distance={0}\r\n", m_CollisionDistance);
                                msg += string.Format("Override Set Time={0}\r\n", m_OverrideTime);
                                msg += string.Format("Apply Time={0}\r\n", XFunc.GetTickCount() - StartTicks);
                                msg += string.Format("changing_overrideRate={0}\r\n", changing_overrideRate);
                                msg += string.Format("changed_overrideRate={0}\r\n", changed_overrideRate);
                                msg += string.Format("current_velocity={0}\r\n", current_velocity);
                                MxpCommLog.WriteLog(msg);

                                bool stop = collision_ng;
                                stop &= !override_fail_timeover;
                                stop &= current_override > 0.1f; // Touch GUI에서 0.1 설정하니까 0에 도달 못하고 시간이 지난 경우 발생.
                                if (stop)
                                {
                                    m_MxpAxis.m_OverrideStop = true;
                                    MXP.MXP_FUNCTION_STATUS_RESULT rv = m_MxpAxis.Stop(false, false, true);
                                    MxpCommLog.WriteLog(string.Format("{0} Override Stop, min_distance={1},limit_distance={2}", m_Axis.AxisName, min_distance, limit_distance));
                                }
                                else
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} Override Reschedule", m_Axis.AxisName));
                                }
                                seqNo = 10;
                            }
                            else if (increase_time_over)
                            {
                                bool set_override_zero = (m_Axis as MpAxis).SetOverrideZeroTouchGUI && current_override == 0.0f;
                                bool resetting = !stop_condition && target_override > 0.0f && current_override == 0.0f;

                                // Current Override가 0이고 Target Override가 1일 경우
                                if (set_override_zero || resetting)
                                {
                                    (m_Axis as MpAxis).SetOverrideZeroTouchGUI = false;
                                    MxpCommLog.WriteLog(string.Format("{0} Override Reschedule", m_Axis.AxisName));
                                    seqNo = 10;
                                }
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }
        }
        private class SeqTorqueLimitCheck: XSeqFunc
        {
            private MxpAxis m_MxpAxis = null;
            private AxisBlockMXP m_AxisBlock = null;
            private _Axis m_Axis = null;
            private int m_TorqueFollowing = 0;
            private bool m_UseSet = false;
            private float m_CurTorqueLimit = 3500.0f;
            private bool m_UpTicksSet = false;
            private uint m_UpTicks = 0;
            private bool m_DownTicksSet = false;
            private uint m_DownTicks = 0;
            public SeqTorqueLimitCheck(MxpAxis mxp_axis)
            {
                this.SeqName = $"SeqTorqueLimitCheck{mxp_axis.m_Axis.AxisName}";

                m_MxpAxis = mxp_axis;
                m_AxisBlock = mxp_axis.m_AxisBlock;
                m_Axis = mxp_axis.m_Axis;
                m_UseSet = (m_Axis as MpAxis).TorqueLimitUse ? false : true; //한번은 설정해야 한다. 여기서 초기화 필요
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
                            if (m_AxisBlock.Connected == false) break;
                            if ((m_Axis as MpAxis).TorqueLimitUse)
                            {
                                if (!m_UseSet) m_UseSet = true;
                                // 사용 설정
                                ushort address = (ushort)(m_Axis as MpAxis).TorqueLimitUseSetAddress;
                                MXP.MXP_FUNCTION_STATUS_RESULT rv;
                                rv = MXP.AX_WriteParameter((UInt32)m_Axis.NodeId, address, 1);
                                if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                                {
                                    rv = MXP.AX_WriteParameter((UInt32)m_Axis.SlaveNodeId, address, 1);
                                    MxpCommLog.WriteLog("Set Slave Torque Limit");
                                }
                                seqNo = 10;
                            }
                            else
                            {
                                if (m_UseSet)
                                {
                                    m_UseSet = false;
                                    ushort address = (ushort)(m_Axis as MpAxis).TorqueLimitUseSetAddress;
                                    MXP.MXP_FUNCTION_STATUS_RESULT rv;
                                    rv = MXP.AX_WriteParameter((UInt32)m_Axis.NodeId, address, 0);

                                    m_CurTorqueLimit = (float)(m_Axis as MpAxis).TorqueRunRate;
                                    ushort address0 = (ushort)(m_Axis as MpAxis).TorqueLimitOffstAddress;

                                    rv = MXP.AX_WriteParameter((UInt32)m_Axis.NodeId, address0, m_CurTorqueLimit);
                                    /////////////////////////////////////////////////////////////////////////////////////
                                    /// Master축인 경우 Slave 축에도 명령을 내려 주자
                                    if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                                    {
                                        rv = MXP.AX_WriteParameter((UInt32)m_Axis.SlaveNodeId, address0, m_CurTorqueLimit);
                                    }
                                    /////////////////////////////////////////////////////////////////////////////////////
                                    MxpCommLog.WriteLog(string.Format("{0} Torque Limit Setting : {1}, Result = {2}", m_Axis.AxisName, m_CurTorqueLimit, rv.ToString()));
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            ///Torque Limit 설정
                            if ((m_Axis as MpAxis).TorqueLimitBit)
                            {
                                m_CurTorqueLimit = (float)(m_Axis as MpAxis).TorqueLimitRate;
                                ushort address0 = (ushort)(m_Axis as MpAxis).TorqueLimitOffstAddress;
                                m_TorqueFollowing = 0;
                                ushort address1 = (ushort)(m_Axis as MpAxis).TorqueFollowingGainK3Address;

                                MXP.MXP_FUNCTION_STATUS_RESULT rv;
                                rv = MXP.AX_WriteParameter((UInt32)m_Axis.NodeId, address0, m_CurTorqueLimit);
                                /////////////////////////////////////////////////////////////////////////////////////
                                /// Master축인 경우 Slave 축에도 명령을 내려 주자
                                if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                                {
                                    rv = MXP.AX_WriteParameter((UInt32)m_Axis.SlaveNodeId, address0, m_CurTorqueLimit);
                                    rv = MXP.AX_WriteParameter((UInt32)m_Axis.SlaveNodeId, address1, m_TorqueFollowing);
                                }
                                /////////////////////////////////////////////////////////////////////////////////////

                                MxpCommLog.WriteLog(string.Format("{0} Torque Limit Setting : {1}, Result = {2}", m_Axis.AxisName, m_CurTorqueLimit, rv.ToString()));
                                seqNo = 20;
                            }
                            else if (m_CurTorqueLimit != (float)(m_Axis as MpAxis).TorqueRunRate)
                            {
                                m_CurTorqueLimit = (float)(m_Axis as MpAxis).TorqueRunRate;
                                ushort address0 = (ushort)(m_Axis as MpAxis).TorqueLimitOffstAddress;

                                MXP.MXP_FUNCTION_STATUS_RESULT rv;
                                rv = MXP.AX_WriteParameter((UInt32)m_Axis.NodeId, address0, m_CurTorqueLimit);
                                /////////////////////////////////////////////////////////////////////////////////////
                                /// Master축인 경우 Slave 축에도 명령을 내려 주자
                                if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                                {
                                    rv = MXP.AX_WriteParameter((UInt32)m_Axis.SlaveNodeId, address0, m_CurTorqueLimit);
                                }
                                /////////////////////////////////////////////////////////////////////////////////////
                                MxpCommLog.WriteLog(string.Format("{0} Torque Limit Setting : {1}, Result = {2}", m_Axis.AxisName, m_CurTorqueLimit, rv.ToString()));
                            }
                        }
                        break;

                    case 20:
                        {
                            if (!(m_Axis as MpAxis).TorqueLimitBit)
                            {
                                m_CurTorqueLimit = (float)(m_Axis as MpAxis).TorqueRunRate;
                                ushort address0 = (ushort)(m_Axis as MpAxis).TorqueLimitOffstAddress;

                                MXP.MXP_FUNCTION_STATUS_RESULT rv;
                                rv = MXP.AX_WriteParameter((UInt32)m_Axis.NodeId, address0, m_CurTorqueLimit);
                                /////////////////////////////////////////////////////////////////////////////////////
                                /// Master축인 경우 Slave 축에도 명령을 내려 주자
                                if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                                {
                                    rv = MXP.AX_WriteParameter((UInt32)m_Axis.SlaveNodeId, address0, m_CurTorqueLimit);
                                }
                                /////////////////////////////////////////////////////////////////////////////////////
                                MxpCommLog.WriteLog(string.Format("{0} Torque Limit Setting : {1}, Result = {2}", m_Axis.AxisName, m_CurTorqueLimit, rv.ToString()));
                                seqNo = 30;
                            }
                            //곡선을 나가기 전 Override가 걸리면 TorqueLimit를 풀자..
                            else if (m_MxpAxis.m_OverrideRate == 0.0f)
                            {
                                m_CurTorqueLimit = (float)(m_Axis as MpAxis).TorqueRunRate;
                                ushort address0 = (ushort)(m_Axis as MpAxis).TorqueLimitOffstAddress;

                                MXP.MXP_FUNCTION_STATUS_RESULT rv;
                                rv = MXP.AX_WriteParameter((UInt32)m_Axis.NodeId, address0, m_CurTorqueLimit);
                                /////////////////////////////////////////////////////////////////////////////////////
                                /// Master축인 경우 Slave 축에도 명령을 내려 주자
                                if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                                {
                                    rv = MXP.AX_WriteParameter((UInt32)m_Axis.SlaveNodeId, address0, m_CurTorqueLimit);
                                }
                                MxpCommLog.WriteLog(string.Format("{0} Override Stop. Torque Limit Release : {1}, Result = {2}", m_Axis.AxisName, m_CurTorqueLimit, rv.ToString()));
                                seqNo = 40;
                            }
                            else
                            {
                                TorquelimitOverMonitor();
                            }
                        }
                        break;

                    case 30:
                        {
                            if ((m_Axis as MpAxis).TorqueLimitBit)
                            {
                                seqNo = 0;
                            }
                            else if (m_TorqueFollowing < 30)
                            {
                                m_TorqueFollowing++;
                                ushort address1 = (ushort)(m_Axis as MpAxis).TorqueFollowingGainK3Address;
                                MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP.AX_WriteParameter((UInt32)m_Axis.SlaveNodeId, address1, m_TorqueFollowing);
                            }
                            else
                                seqNo = 0;
                        }
                        break;

                    case 40:
                        {
                            double target_velocity = Math.Abs((m_Axis as MpAxis).TrajectoryTargetVelocity);
                            double actual_velocity = Math.Abs(m_Axis.CurSpeed);
                            bool velocity_ok = actual_velocity > 0.8f * target_velocity; //700*0.8=560..0.9로하면 고속으로 휙 돌까봐 줄였습니다..
                            if (m_MxpAxis.m_OverrideRate > 0.0f && velocity_ok)
                            {
                                m_CurTorqueLimit = (float)(m_Axis as MpAxis).TorqueLimitRate;
                                ushort address0 = (ushort)(m_Axis as MpAxis).TorqueLimitOffstAddress;
                                m_TorqueFollowing = 0;
                                ushort address1 = (ushort)(m_Axis as MpAxis).TorqueFollowingGainK3Address;

                                MXP.MXP_FUNCTION_STATUS_RESULT rv;
                                rv = MXP.AX_WriteParameter((UInt32)m_Axis.NodeId, address0, m_CurTorqueLimit);
                                /////////////////////////////////////////////////////////////////////////////////////
                                /// Master축인 경우 Slave 축에도 명령을 내려 주자
                                if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                                {
                                    MxpCommLog.WriteLog("Slave Torque Limit");

                                    rv = MXP.AX_WriteParameter((UInt32)m_Axis.SlaveNodeId, address0, m_CurTorqueLimit);
                                    rv = MXP.AX_WriteParameter((UInt32)m_Axis.SlaveNodeId, address1, m_TorqueFollowing);
                                }
                                /////////////////////////////////////////////////////////////////////////////////////
                                MxpCommLog.WriteLog(string.Format("{0} Release Override. Torque Limit Setting : {1}, Result = {2}", m_Axis.AxisName, m_CurTorqueLimit, rv.ToString()));
                                seqNo = 20;
                            }
                            else if (!(m_Axis as MpAxis).TorqueLimitBit)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Torque Limit Re-Setting", m_Axis.AxisName));
                                seqNo = 20;
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }

            private void TorquelimitOverMonitor()
            {
                double target_velocity = Math.Abs((m_Axis as MpAxis).TrajectoryTargetVelocity);
                double command_velocity = Math.Abs((m_Axis as MpAxis).CommandSpeed);
                double actual_velocity = Math.Abs(m_Axis.CurSpeed);
                double curTorque = Math.Abs(m_Axis.CurTorque);

                // up condition
                // curTorque가 LimitTorque에 도달 했을때
                // actualVelocity가 commandVelocity보다 작은 경우
                // actualVelocity가 targetVelocity의 10% 이하이고
                // override가 0이 아닐때
                // 0.5sec간 유지되면 
                bool up_change_condition = true;
                up_change_condition &= (m_Axis as MpAxis).CurrentOverrideRate > 0.0f;
                up_change_condition &= curTorque >= 0.1f * m_CurTorqueLimit; //TorqueLimit 단위가 10배 임
                up_change_condition &= actual_velocity < command_velocity;
                up_change_condition &= actual_velocity < 0.1f * target_velocity;
                if (up_change_condition && !m_UpTicksSet) { m_UpTicks = XFunc.GetTickCount(); m_UpTicksSet = true; }
                else if (up_change_condition == false) m_UpTicksSet = false;

                bool up_condition = up_change_condition;
                up_condition &= m_CurTorqueLimit < 1500; // 최대로 올라갈수 있는 Limit은 1500으로 설정해 보자~~장비 Torque 상태에 다른수 있을 것 같다...
                up_condition &= m_UpTicksSet;
                up_condition &= XFunc.GetTickCount() - m_UpTicks > 500;

                // down condition
                // targetVelocity에 actual_velocity가 도달한 경우
                // m_CurTorqueLimit > (float)(m_Axis as MpAxis).TorqueLimitRate;
                bool down_change_condition = true;
                down_change_condition &= m_CurTorqueLimit > (float)(m_Axis as MpAxis).TorqueLimitRate;
                down_change_condition &= actual_velocity > 0.8f * target_velocity;
                if (down_change_condition && !m_DownTicksSet) { m_DownTicks = XFunc.GetTickCount(); m_DownTicksSet = true; }
                else if (down_change_condition == false) m_DownTicksSet = false;

                bool down_condition = down_change_condition;
                down_condition &= m_DownTicksSet;
                down_condition &= XFunc.GetTickCount() - m_DownTicks > 100;

                if (up_condition)
                {
                    m_CurTorqueLimit += 100;
                    ushort address0 = (ushort)(m_Axis as MpAxis).TorqueLimitOffstAddress;
                    MXP.MXP_FUNCTION_STATUS_RESULT rv;
                    rv = MXP.AX_WriteParameter((UInt32)m_Axis.NodeId, address0, m_CurTorqueLimit);
                    /////////////////////////////////////////////////////////////////////////////////////
                    /// Master축인 경우 Slave 축에도 명령을 내려 주자
                    if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                    {
                        rv = MXP.AX_WriteParameter((UInt32)m_Axis.SlaveNodeId, address0, m_CurTorqueLimit);
                    }
                    MxpCommLog.WriteLog(string.Format("{0} Torque Limit Setting : Torque-{1}, CmdSpeed-{2}, CurSpeed-{3}, TraVelocity-{4}, Result = {5}", m_Axis.AxisName, m_CurTorqueLimit, command_velocity, actual_velocity, target_velocity, rv.ToString()));
                }
                else if (down_condition)
                {
                    m_CurTorqueLimit -= 100;
                    ushort address0 = (ushort)(m_Axis as MpAxis).TorqueLimitOffstAddress;
                    MXP.MXP_FUNCTION_STATUS_RESULT rv;
                    rv = MXP.AX_WriteParameter((UInt32)m_Axis.NodeId, address0, m_CurTorqueLimit);
                    /////////////////////////////////////////////////////////////////////////////////////
                    /// Master축인 경우 Slave 축에도 명령을 내려 주자
                    if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                    {
                        rv = MXP.AX_WriteParameter((UInt32)m_Axis.SlaveNodeId, address0, m_CurTorqueLimit);
                    }
                    MxpCommLog.WriteLog(string.Format("{0} Torque Limit Setting : Torque-{1}, CmdSpeed-{2}, CurSpeed-{3}, TraVelocity-{4}, Result = {5}", m_Axis.AxisName, m_CurTorqueLimit, command_velocity, actual_velocity, target_velocity, rv.ToString()));
                }
            }
        }
        private class SeqFollowingErrorCheck : XSeqFunc
        {
            private MxpAxis m_MxpAxis = null;
            private AxisBlockMXP m_AxisBlock = null;
            private _Axis m_Axis = null;

            public SeqFollowingErrorCheck(MxpAxis mxp_axis)
            {
                this.SeqName = $"SeqFollowingErrorCheck{mxp_axis.m_Axis.AxisName}";
                m_MxpAxis = mxp_axis;
                m_AxisBlock = mxp_axis.m_AxisBlock;
                m_Axis = mxp_axis.m_Axis;
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
                            if (m_AxisBlock.Connected == false) break;
                            bool following_velocity_limit_use = (m_Axis as MpAxis).FollowingVelocityLimitUse;
                            double velocity_error_range = (m_Axis as MpAxis).FollowingVelocityLimitValue;
                            Byte usingLimit = 0;
                            Single offset = 0;
                            MXP.AX_GetVelocityLimitUsingState((UInt32)m_Axis.NodeId, ref usingLimit, ref offset);

                            if (following_velocity_limit_use && usingLimit == 0)
                            {
                                MXP.AX_SetVelocityLimit((UInt32)m_Axis.NodeId, 1, (float)velocity_error_range);
                            }
                            else if (!following_velocity_limit_use && usingLimit == 1)
                            {
                                MXP.AX_SetVelocityLimit((UInt32)m_Axis.NodeId, 0, (float)velocity_error_range);
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }
        }
        #endregion
        #region Thread - Task Axis
        private class TaskAxis : XSequence
        {
            public TaskAxis(MxpAxis mxp_axis)
            {
                SetSubInfo(mxp_axis.m_Axis.AxisName);

                RegSeq(new SeqUpdateState(mxp_axis));
                RegSeq(new SeqHome(mxp_axis));
                RegSeq(new SeqCommonCommand(mxp_axis));
                RegSeq(new SeqMove(mxp_axis));
                RegSeq(new SeqRelativeMove(mxp_axis));
                RegSeq(new SeqProfileMove(mxp_axis));
                RegSeq(new SeqJog(mxp_axis));
            }
        }
        private class SeqUpdateState : XSeqFunc
        {
            private MxpAxis m_MxpAxis = null;
            private AxisBlockMXP m_AxisBlock = null;

            public SeqUpdateState(MxpAxis mxp_axis)
            {
                this.SeqName = $"SeqUpdateState{mxp_axis.m_Axis.AxisName}";
                m_MxpAxis = mxp_axis;
                m_AxisBlock = mxp_axis.m_AxisBlock;
            }

            public override int Do()
            {
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_AxisBlock.Connected == false) break;

                            m_MxpAxis.UpdateState();
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 10;
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 10) 
                            {
                                seqNo = 0;
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }
        }
        private class SeqCommonCommand : XSeqFunc
        {
            private MxpAxis m_MxpAxis = null;
            private AxisBlockMXP m_AxisBlock = null;
            private _Axis m_Axis = null;
            public SeqCommonCommand(MxpAxis mxp_axis)
            {
                this.SeqName = $"SeqCommonCommand{mxp_axis.m_Axis.AxisName}";
                m_MxpAxis = mxp_axis;
                m_AxisBlock = mxp_axis.m_AxisBlock;
                m_Axis = mxp_axis.m_Axis;
            }

            public override int Do()
            {
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_AxisBlock.Connected == false) break;
                            if (m_Axis.CommandSkip) break;

                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            if (command != enAxisOutFlag.CommandNone)
                            {
                                if (Convert.ToBoolean(command & enAxisOutFlag.ServoOff))
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} Servo Off", m_Axis.AxisName));
                                    m_MxpAxis.Disable();
                                    m_MxpAxis.Init();
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 10;
                                }
                                else if (Convert.ToBoolean(command & enAxisOutFlag.MotionStop) && !m_Axis.CommandSkip)
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} Servo Stop", m_Axis.AxisName));
                                    m_MxpAxis.Stop();
                                    m_MxpAxis.Init(); // Stop 내부에서 초기화 하자
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 20;
                                }
                                else if (Convert.ToBoolean(command & enAxisOutFlag.AlarmClear) && !m_Axis.CommandSkip)
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} Alarm Clear", m_Axis.AxisName));
                                    m_MxpAxis.AlarmClear();
                                    m_MxpAxis.Init();
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 30;
                                }
                                else if (Convert.ToBoolean(command & enAxisOutFlag.ServoOn))
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} Servo On", m_Axis.AxisName));
                                    m_MxpAxis.Enable();
                                    m_MxpAxis.Init();
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 40;
                                }
                                else if (Convert.ToBoolean(command & enAxisOutFlag.MotionPause) && !m_Axis.CommandSkip) //멈추고 다른 명령을 받지 않음..... Release가 되어야 Next Command 실행 가능함.
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} Motion Pause", m_Axis.AxisName));
                                    m_MxpAxis.Pause(true);
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 50;
                                }
                                else if (Convert.ToBoolean(command & enAxisOutFlag.MotionRelease) && !m_Axis.CommandSkip)
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} Motion Release", m_Axis.AxisName));
                                    m_MxpAxis.Pause(false);
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 60;
                                }
                                else if (Convert.ToBoolean(command & enAxisOutFlag.ZeroSet) && !m_Axis.CommandSkip)
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} Motion Release", m_Axis.AxisName));
                                    m_MxpAxis.SetPosition(0.0f);
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 70;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            if (m_MxpAxis.AxisState.disabled == 1)
                            {
                                m_MxpAxis.SetHomeEnd(false);
                                MxpCommLog.WriteLog(string.Format("{0} Servo Off OK", m_Axis.AxisName));
                                seqNo = 100;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Servo Off TimeOver", m_Axis.AxisName));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 20:
                        {
                            bool stop = m_MxpAxis.AxisState.standstill == 1 ? true : false;
                            stop &= m_MxpAxis.SequenceState.busy == 0 ? true : false;
                            stop &= m_MxpAxis.MoveState != MXP_MOVESTATE.MOVESTATE_MOVING ? true : false;
                            if (stop)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Servo Stop OK", m_Axis.AxisName));
                                seqNo = 100;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Servo Stop TimeOver", m_Axis.AxisName));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 30:
                        {
                            if (m_MxpAxis.IsAlarm() == false)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Alarm Clear OK", m_Axis.AxisName));
                                seqNo = 100;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Alarm Clear TimeOver", m_Axis.AxisName));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 40:
                        {
                            if (m_MxpAxis.AxisState.disabled == 0)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Servo On OK", m_Axis.AxisName));
                                seqNo = 100;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Servo On TimeOver", m_Axis.AxisName));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 50:
                        {
                            bool stop = m_MxpAxis.AxisState.standstill == 1 ? true : false;
                            stop &= m_MxpAxis.SequenceState.busy == 0 ? true : false;
                            stop &= m_MxpAxis.MoveState != MXP_MOVESTATE.MOVESTATE_MOVING ? true : false;
                            if (stop)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Motion Pause OK", m_Axis.AxisName));
                                seqNo = 100;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Motion Pause TimeOver", m_Axis.AxisName));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 60:
                        {
                            bool stop = m_MxpAxis.AxisState.standstill == 1 ? true : false;
                            stop &= m_MxpAxis.SequenceState.busy == 0 ? true : false;
                            stop &= m_MxpAxis.MoveState != MXP_MOVESTATE.MOVESTATE_MOVING ? true : false;
                            if (!stop)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Motion Release OK", m_Axis.AxisName));
                                seqNo = 100;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Motion Release TimeOver", m_Axis.AxisName));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 70:
                        {
                            if (Math.Abs(m_Axis.CurPos) < 0.1f)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} ZeroSet OK", m_Axis.AxisName));
                                seqNo = 100;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} ZeroSet TimeOver", m_Axis.AxisName));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 100:
                        {
                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            if (command == enAxisOutFlag.CommandNone)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Motion Command None Check", m_Axis.AxisName));
                                seqNo = 0;
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }
        }
        private class SeqHome : XSeqFunc
        {
            private MxpAxis m_MxpAxis = null;
            private AxisBlockMXP m_AxisBlock = null;
            private _Axis m_Axis = null;
            private int m_RetryCount = 0;

            public SeqHome(MxpAxis mxp_axis)
            {
                this.SeqName = $"SeqHome{mxp_axis.m_Axis.AxisName}";
                m_MxpAxis = mxp_axis;
                m_AxisBlock = mxp_axis.m_AxisBlock;
                m_Axis = mxp_axis.m_Axis;
            }

            public override int Do()
            {
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_AxisBlock.Connected == false) break;

                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            if (Convert.ToBoolean(command & enAxisOutFlag.HomeStart))
                            {
                                m_RetryCount = 0;
                                if (m_MxpAxis.IsReady())
                                {
                                    m_MxpAxis.SetHomeEnd(false);
                                    m_MxpAxis.m_CommandHome = true;

                                    if (m_Axis.HomeMethod == enHomeMethod.ZERO_MOVE_HOME)
                                    {
                                        seqNo = 300;
                                    }
                                    else
                                    {
                                        m_MxpAxis.CommandResult = m_MxpAxis.Home();
                                        MxpCommLog.WriteLog(string.Format("{0} Home Start - {1}", m_Axis.AxisName, m_MxpAxis.CommandResult));
                                        StartTicks = XFunc.GetTickCount();
                                        if (m_MxpAxis.CommandResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                                        {
                                            StartTicks = XFunc.GetTickCount();
                                            if (m_Axis.HomeMethod == enHomeMethod.ABS_ZERO) seqNo = 100;
                                            else if (m_Axis.HomeMethod == enHomeMethod.PLC) seqNo = 100;
                                            else seqNo = 10;
                                        }
                                        else
                                        {
                                            StartTicks = XFunc.GetTickCount();
                                            if (m_Axis.HomeMethod == enHomeMethod.ABS_ZERO) seqNo = 200;
                                            else if (m_Axis.HomeMethod == enHomeMethod.PLC) seqNo = 200;
                                            else seqNo = 30;
                                        }
                                    }
                                }
                                else
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} Home NG - Not Ready", m_Axis.AxisName));
                                    seqNo = 1000;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            if (m_MxpAxis.AxisState.homing == 1)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Home Ing", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else if (m_MxpAxis.IsAlarm())
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Home NG - Mxp Alarm [mxp={1} driver={2}]", m_Axis.AxisName, string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList)));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Home NG - Not Response 1(sec)", m_Axis.AxisName)); //뭔가 이상하네... 처음으로 돌아가자... 상위에서 알람을 띄울거임.
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (m_MxpAxis.AxisState.isHomed == 1)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Home Completed", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                if (m_MxpAxis.SequenceState.busy == 1) seqNo = 40;
                                else seqNo = 50;
                            }
                            else if (m_MxpAxis.IsAlarm())
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Home NG - Mxp Alarm [mxp={1} driver={2}]", m_Axis.AxisName, string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList)));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 5 * 60 * 1000)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Home NG - Timeout", m_Axis.AxisName)); //Home 동작이 너무 오래 걸린다. 이상하네..5분 이상 할만한게 없는데....
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                        }
                        break;

                    case 30:
                        {
                            bool stop = m_MxpAxis.AxisState.standstill == 1 ? true : false;
                            stop &= m_MxpAxis.SequenceState.busy == 0 ? true : false;
                            stop &= m_MxpAxis.MoveState != MXP_MOVESTATE.MOVESTATE_MOVING ? true : false;
                            if (stop)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Home OK - {1}", m_Axis.AxisName, m_Axis.AxisStateMsg));
                                m_MxpAxis.m_CommandHome = false;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 500)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Home Timeout - {1}", m_Axis.AxisName, m_Axis.AxisStateMsg));
                                m_MxpAxis.m_CommandHome = false;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 40:
                        {
                            MXP.MXP_FUNCTION_STATUS_RESULT rv = m_MxpAxis.Stop();
                            MxpCommLog.WriteLog(string.Format("{0} Sequnce Move Busy ===> Stop", m_Axis.AxisName));
                            seqNo = 50;
                        }
                        break;

                    case 50:
                        {
                            // Home Offset 만큼 이동하자
                            /////////////////////////////////////////////////////////////////////////////////////
                            if (m_Axis.HomeOffset == 0.0f)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Home Offset = 0.0f", m_Axis.AxisName));
                                seqNo = 100;
                            }
                            else
                            {
                                double velocity = m_Axis.HomeSearchSpeed;
                                if (velocity == 0.0f) velocity = m_Axis.JogSpeed;
                                (m_Axis as IAxisCommand).SetTargetDistance(m_Axis.HomeOffset);
                                VelSet set = new VelSet
                                {
                                    Vel = velocity,
                                    Acc = velocity,
                                    Dec = velocity,
                                    Jerk = 2 * velocity,
                                };
                                (m_Axis as IAxisCommand).SetSpeedAsync(set);
                                /////////////////////////////////////////////////////////////////////////////////////

                                m_MxpAxis.CommandResult = m_MxpAxis.RelativeMove();
                                if (m_MxpAxis.CommandResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} Home Offset Relative Move Start", m_Axis.AxisName));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 60;
                                }
                                else
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} Home Offset Relative Move NG", m_Axis.AxisName));
                                    m_MxpAxis.m_CommandHome = false;
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 60:
                        {
                            m_MxpAxis.CheckMoveState();
                            bool cancel = false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_ABORT ? true : false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_NULL ? true : false;

                            if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_MOVING)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} RelativeMove Ing", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 70;
                            }
                            else if (m_MxpAxis.IsAlarm() || m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                            {
                                if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                                    MxpCommLog.WriteLog(string.Format("{0} RelativeMove State Fail", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} RelativeMove NG - Mxp Alarm [mxp={1} driver={2}]", m_Axis.AxisName, string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList)));
                                m_MxpAxis.m_CommandHome = false;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 70:
                        {
                            m_MxpAxis.CheckMoveState();
                            bool cancel = false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_ABORT ? true : false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_NULL ? true : false;

                            bool complete = m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_COMPLETE ? true : false;
                            complete |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_WAIT_INPOS ? true : false;
                            complete &= m_MxpAxis.AxisState.standstill == 1 ? true : false;

                            if (complete)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} RelativeMove Completed", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 80;
                            }
                            else if (m_MxpAxis.IsAlarm() || m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                            {
                                if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                                    MxpCommLog.WriteLog(string.Format("{0} RelativeMove State Fail", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} RelativeMove NG - Mxp Alarm [mxp={1} driver={2}]", m_Axis.AxisName, string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList)));
                                m_MxpAxis.m_CommandHome = false;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 5 * 60 * 1000 || cancel)
                            {
                                if (cancel) MxpCommLog.WriteLog(string.Format("{0} RelativeMove State Abort", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} RelativeMove NG - Timeout", m_Axis.AxisName));
                                m_MxpAxis.m_CommandHome = false;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 80:
                        {
                            // 현 위치를 Zero Set 하자
                            m_MxpAxis.CommandResult = m_MxpAxis.SetPosition(0.0f);
                            MxpCommLog.WriteLog(string.Format("{0} SetPosition(0.0f) - {1}", m_Axis.AxisName, m_MxpAxis.CommandResult));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 100;
                        }
                        break;

                    case 100:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 100) break;
                            
                            if (Math.Abs(m_Axis.CurPos) < 0.1f)
                            {
                                m_MxpAxis.SetHomeEnd(true);
                                m_MxpAxis.m_CommandHome = false;
                                MxpCommLog.WriteLog(string.Format("{0} SetPosition(0.0f) OK", m_Axis.AxisName));
                                if (m_Axis.HomeMethod == enHomeMethod.ABS_ZERO) seqNo = 110;
                                else if (m_Axis.HomeMethod == enHomeMethod.PLC) seqNo = 110;
                                else seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                if (m_Axis.HomeMethod == enHomeMethod.ABS_ZERO || m_Axis.HomeMethod == enHomeMethod.PLC)
                                {
                                    // 될때까지 retry 하자~~~
                                    m_MxpAxis.CommandResult = m_MxpAxis.Home();
                                    MxpCommLog.WriteLog(string.Format("{0} Home Start - {1}", m_Axis.AxisName, m_MxpAxis.CommandResult));
                                    StartTicks = XFunc.GetTickCount();
                                    if (m_MxpAxis.CommandResult != MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                                    {
                                        m_MxpAxis.m_CommandHome = false;
                                        MxpCommLog.WriteLog(string.Format("{0} SetPosition(0.0f) Timeout", m_Axis.AxisName));
                                        seqNo = 0;
                                    }
                                }
                                else
                                {
                                    m_MxpAxis.m_CommandHome = false;
                                    MxpCommLog.WriteLog(string.Format("{0} SetPosition(0.0f) Timeout", m_Axis.AxisName));
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 110:
                        {
                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            if (Convert.ToBoolean(command & enAxisOutFlag.HomeStart) == false)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} HomeStart Comand Off Check", m_Axis.AxisName));
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 500)
                            {
                                m_MxpAxis.m_CommandHome = false;
                                MxpCommLog.WriteLog(string.Format("{0} HomeStart Comand Off Timeout", m_Axis.AxisName));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 200:
                        {
                            if (m_RetryCount < 3)
                            {
                                if (m_Axis.HomeMethod == enHomeMethod.PLC)
                                {
                                    m_MxpAxis.CommandResult = MXP.PLC_WriteBit((UInt32)5350, Convert.ToByte(2), (byte)1); // set = 0 : 미사용, 1 : 사용
                                }
                                else
                                {
                                    m_MxpAxis.CommandResult = m_MxpAxis.SetPosition(0.0f);
                                }
                                if (m_MxpAxis.CommandResult == MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} SetPosition(0.0f) OK, HomeMethod(ABS_ZERO) - {1}", m_Axis.AxisName, m_MxpAxis.CommandResult));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 100;
                                }
                                else
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} SetPosition(0.0f) NG, HomeMethod(ABS_ZERO) - {1}", m_Axis.AxisName, m_MxpAxis.CommandResult));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 210;
                                }
                            }
                            else
                            {
                                m_MxpAxis.m_CommandHome = false;
                                MxpCommLog.WriteLog(string.Format("{0} SetPosition(0.0f) Retry Timeout", m_Axis.AxisName));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 210:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;
                            if (m_MxpAxis.IsReady())
                            {
                                m_RetryCount++;
                                seqNo = 200;
                            }
                            else
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Home NG - Not Ready", m_Axis.AxisName));
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 300:
                        {
                            double pos = (-1) * m_Axis.CurPos;
                            double velocity = 0.5f * Math.Abs(pos);
                            if (velocity == 0.0f) velocity = m_Axis.JogSpeed;
                            if (velocity < 5.0f) velocity = 5.0f;
                            (m_Axis as IAxisCommand).SetTargetDistance(pos + 5.0f); //이렇게 하면 무조건 + 방향에 있을거야...
                            VelSet set = new VelSet
                            {
                                Vel = velocity,
                                Acc = velocity,
                                Dec = velocity,
                                Jerk = 2 * velocity,
                            };
                            (m_Axis as IAxisCommand).SetSpeedAsync(set);
                            /////////////////////////////////////////////////////////////////////////////////////

                            m_MxpAxis.CommandResult = m_MxpAxis.RelativeMove();
                            if (m_MxpAxis.CommandResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Home Offset Relative Move Start", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 310;
                            }
                            else
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Home Offset Relative Move NG", m_Axis.AxisName));
                                m_MxpAxis.m_CommandHome = false;
                                seqNo = 0;
                            }

                        }
                        break;

                    case 310:
                        {
                            m_MxpAxis.CheckMoveState();
                            bool cancel = false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_ABORT ? true : false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_NULL ? true : false;

                            if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_MOVING)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} RelativeMove Ing", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 320;
                            }
                            else if (m_MxpAxis.IsAlarm() || m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                            {
                                if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                                    MxpCommLog.WriteLog(string.Format("{0} RelativeMove State Fail", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} RelativeMove NG - Mxp Alarm [mxp={1} driver={2}]", m_Axis.AxisName, string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList)));
                                m_MxpAxis.m_CommandHome = false;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 320:
                        {
                            m_MxpAxis.CheckMoveState();
                            bool cancel = false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_ABORT ? true : false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_NULL ? true : false;

                            bool complete = m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_COMPLETE ? true : false;
                            complete |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_WAIT_INPOS ? true : false;
                            complete &= m_MxpAxis.AxisState.standstill == 1 ? true : false;

                            if (complete)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} RelativeMove Completed", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 330;
                            }
                            else if (m_MxpAxis.IsAlarm() || m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                            {
                                if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                                    MxpCommLog.WriteLog(string.Format("{0} RelativeMove State Fail", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} RelativeMove NG - Mxp Alarm [mxp={1} driver={2}]", m_Axis.AxisName, string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList)));
                                m_MxpAxis.m_CommandHome = false;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 5 * 60 * 1000 || cancel)
                            {
                                if (cancel) MxpCommLog.WriteLog(string.Format("{0} RelativeMove State Abort", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} RelativeMove NG - Timeout", m_Axis.AxisName));
                                m_MxpAxis.m_CommandHome = false;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 330:
                        {
                            m_MxpAxis.CommandResult = m_MxpAxis.Home();
                            MxpCommLog.WriteLog(string.Format("{0} Home Start - {1}", m_Axis.AxisName, m_MxpAxis.CommandResult));
                            StartTicks = XFunc.GetTickCount();
                            if (m_MxpAxis.CommandResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                        }
                        break;

                    case 1000:
                        {
                            if (m_MxpAxis.IsReady())
                            {
                                seqNo = 0;
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }
        }
        private class SeqMove : XSeqFunc
        {
            private MxpAxis m_MxpAxis = null;
            private AxisBlockMXP m_AxisBlock = null;
            private _Axis m_Axis = null;
            private bool m_HomeOnCheck = false;
            private bool m_HomeOffCheck = false;

            public SeqMove(MxpAxis mxp_axis)
            {
                this.SeqName = $"SeqMove{mxp_axis.m_Axis.AxisName}";
                m_MxpAxis = mxp_axis;
                m_AxisBlock = mxp_axis.m_AxisBlock;
                m_Axis = mxp_axis.m_Axis;
            }

            public override int Do()
            {
                HomePositionCheck();

                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_AxisBlock.Connected == false) break;

                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            if (Convert.ToBoolean(command & enAxisOutFlag.MotionStart))
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Move Start - {1}", m_Axis.AxisName, m_MxpAxis.IsReady() ? "Ready State" : "NotReady State"));

                                bool command_cancel = Convert.ToBoolean(command & enAxisOutFlag.MotionCancel) ? true : false;
                                if (command_cancel)
                                {
                                    command = enAxisOutFlag.CommandNone;
                                    (m_Axis as IAxisCommand).SetCommandAsync(command);
                                    MxpCommLog.WriteLog(string.Format("{0} Command Cancel - {1}", m_Axis.AxisName, m_MxpAxis.IsReady() ? "Ready State" : "NotReady State"));
                                }

                                if (m_MxpAxis.AxisState.homeAbsSwitch == 1) m_HomeOffCheck = true;
                                else m_HomeOnCheck = true;

                                m_MxpAxis.CommandResult = m_MxpAxis.Move();
                                StartTicks = XFunc.GetTickCount();
                                if (m_MxpAxis.CommandResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR) seqNo = 10;
                                else seqNo = 30;
                            }
                        }
                        break;

                    case 10:
                        {
                            m_MxpAxis.CheckMoveState();
                            bool cancel = false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_ABORT ? true : false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_NULL ? true : false;

                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            bool command_ack = command == enAxisOutFlag.CommandNone ? true : false;
                            bool command_cancel = Convert.ToBoolean(command & enAxisOutFlag.MotionCancel) ? true : false;

                            if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_MOVING && command_ack)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Move Ing", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else if (m_MxpAxis.IsAlarm() || m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                            {
                                if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                                    MxpCommLog.WriteLog(string.Format("{0} Move State Fail", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} Move NG - Mxp Alarm [mxp={1} driver={2}]", m_Axis.AxisName, string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList)));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                            else if (command_cancel)
                            {
                                m_MxpAxis.m_CommandRun = false;
                                MxpCommLog.WriteLog(string.Format("{0} Command Cancel", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000 || cancel)
                            {
                                if (cancel) MxpCommLog.WriteLog(string.Format("{0} Move State Abort", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} Move NG - Not Response 1(sec)", m_Axis.AxisName)); //뭔가 이상하네... 처음으로 돌아가자... 상위에서 알람을 띄울거임.
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                        }
                        break;

                    case 20:
                        {
                            m_MxpAxis.CheckMoveState();
                            bool cancel = false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_ABORT ? true : false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_NULL ? true : false;

                            bool complete = m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_COMPLETE ? true : false;
                            //complete |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_WAIT_INPOS ? true : false;
                            complete &= m_MxpAxis.AxisState.standstill == 1 ? true : false;
                            complete |= m_Axis.SequenceState.IsExternalEncoderRun ? true : false;

                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            bool command_cancel = Convert.ToBoolean(command & enAxisOutFlag.MotionCancel) ? true : false;

                            if (complete)
                            {
                                m_MxpAxis.m_CommandRun = false;
                                MxpCommLog.WriteLog(string.Format("{0} Move Completed", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                if (m_Axis.SequenceState.IsExternalEncoderRun)
                                {
                                    seqNo = 0;
                                }
                                else
                                {
                                    if (m_MxpAxis.SequenceState.busy == 1) seqNo = 40;
                                    else seqNo = 0;
                                }
                            }
                            else if (m_MxpAxis.IsAlarm() || m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                            {
                                if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                                    MxpCommLog.WriteLog(string.Format("{0} Move State Fail", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} Move NG - Mxp Alarm [mxp={1} driver={2}]", m_Axis.AxisName, string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList)));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                            else if (command_cancel)
                            {
                                m_MxpAxis.m_CommandRun = false;
                                MxpCommLog.WriteLog(string.Format("{0} Command Cancel", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 5 * 60 * 1000 || cancel)
                            {
                                if (cancel) MxpCommLog.WriteLog(string.Format("{0} Move State Abort", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} Move NG - Timeout", m_Axis.AxisName)); 
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                        }
                        break;

                    case 30:
                        {
                            bool stop = m_MxpAxis.AxisState.standstill == 1 ? true : false;
                            stop &= m_MxpAxis.SequenceState.busy == 0 ? true : false;
                            stop &= m_MxpAxis.MoveState != MXP_MOVESTATE.MOVESTATE_MOVING ? true : false;
                            if (stop)
                            {
                                m_MxpAxis.m_CommandRun = false;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 500)
                            {
                                m_MxpAxis.m_CommandRun = false;
                                seqNo = 40;
                            }
                        }
                        break;

                    case 40:
                        {
                            MXP.MXP_FUNCTION_STATUS_RESULT rv = m_MxpAxis.Stop();
                            MxpCommLog.WriteLog(string.Format("{0} Sequnce Move Busy ===> Stop", m_Axis.AxisName));
                            seqNo = 0;
                        }
                        break;

                    case 100:
                        {
                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            bool ready = m_MxpAxis.IsReady() ? true : false;
                            ready &= Convert.ToBoolean(command & enAxisOutFlag.MotionStart) ? false : true; // Command 삭제 확인 후 돌아가자 !
                            if (ready)
                            {
                                m_MxpAxis.m_CommandRun = false;
                                seqNo = 0;
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }

            private void HomePositionCheck()
            {
                if (m_MxpAxis.m_CommandRun)
                {
                    if (m_HomeOffCheck && m_MxpAxis.AxisState.homeAbsSwitch == 0)
                    {
                        m_HomeOffCheck = false;
                        MxpCommLog.WriteLog(string.Format("{0} Home OFF Position = {1}, HomeOffset = {2}", m_Axis.AxisName, m_Axis.CurPos, m_Axis.HomeOffset));
                    }
                    else if (m_HomeOnCheck && m_MxpAxis.AxisState.homeAbsSwitch == 1)
                    {
                        m_HomeOnCheck = false;
                        double error = m_Axis.HomeOffset + m_Axis.CurPos;
                        (m_Axis as MpAxis).OriginOnDetectError = error;
                        MxpCommLog.WriteLog(string.Format("{0} Home ON Position = {1}, HomeOffset = {2}, error = {3}", m_Axis.AxisName, m_Axis.CurPos, m_Axis.HomeOffset, error));
                    }
                }
                else
                {
                    m_HomeOffCheck = false;
                    m_HomeOnCheck = false;
                }
            }
        }
        private class SeqRelativeMove : XSeqFunc
        {
            private MxpAxis m_MxpAxis = null;
            private AxisBlockMXP m_AxisBlock = null;
            private _Axis m_Axis = null;
            private bool m_HomeOnCheck = false;
            private bool m_HomeOffCheck = false;

            public SeqRelativeMove(MxpAxis mxp_axis)
            {
                this.SeqName = $"SeqRelativeMove{mxp_axis.m_Axis.AxisName}";
                m_MxpAxis = mxp_axis;
                m_AxisBlock = mxp_axis.m_AxisBlock;
                m_Axis = mxp_axis.m_Axis;
            }

            public override int Do()
            {
                HomePositionCheck();

                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_AxisBlock.Connected == false) break;

                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            if (Convert.ToBoolean(command & enAxisOutFlag.RelativeMoveStart))
                            {
                                MxpCommLog.WriteLog(string.Format("{0} RelativeMove Start - {1}", m_Axis.AxisName, m_MxpAxis.IsReady() ? "Ready State" : "NotReady State"));

                                bool command_cancel = Convert.ToBoolean(command & enAxisOutFlag.MotionCancel) ? true : false;
                                if (command_cancel)
                                {
                                    command = enAxisOutFlag.CommandNone;
                                    (m_Axis as IAxisCommand).SetCommandAsync(command);
                                    MxpCommLog.WriteLog(string.Format("{0} Command Cancel - {1}", m_Axis.AxisName, m_MxpAxis.IsReady() ? "Ready State" : "NotReady State"));
                                }

                                if (m_MxpAxis.AxisState.homeAbsSwitch == 1) m_HomeOffCheck = true;
                                else m_HomeOnCheck = true;
                                m_MxpAxis.CommandResult = m_MxpAxis.RelativeMove();
                                StartTicks = XFunc.GetTickCount();
                                if (m_MxpAxis.CommandResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR) seqNo = 10;
                                else seqNo = 30;
                            }
                        }
                        break;

                    case 10:
                        {
                            m_MxpAxis.CheckMoveState();
                            bool cancel = false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_ABORT ? true : false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_NULL ? true : false;

                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            bool command_ack = command == enAxisOutFlag.CommandNone ? true : false;
                            bool command_cancel = Convert.ToBoolean(command & enAxisOutFlag.MotionCancel) ? true : false;

                            if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_MOVING && command_ack)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} RelativeMove Ing", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else if (m_MxpAxis.IsAlarm() || m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                            {
                                if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                                    MxpCommLog.WriteLog(string.Format("{0} RelativeMove State Fail", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} RelativeMove NG - Mxp Alarm [mxp={1} driver={2}]", m_Axis.AxisName, string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList)));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                            else if (command_cancel)
                            {
                                m_MxpAxis.m_CommandRun = false;
                                MxpCommLog.WriteLog(string.Format("{0} Command Cancel", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000 || cancel)
                            {
                                if (cancel) MxpCommLog.WriteLog(string.Format("{0} Relative Move State Abort", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} RelativeMove NG - Not Response 1(sec)", m_Axis.AxisName)); //뭔가 이상하네... 처음으로 돌아가자... 상위에서 알람을 띄울거임.
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                        }
                        break;

                    case 20:
                        {
                            m_MxpAxis.CheckMoveState();
                            bool cancel = false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_ABORT ? true : false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_NULL ? true : false;

                            bool complete = m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_COMPLETE ? true : false;
                            complete |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_WAIT_INPOS ? true : false;
                            complete &= m_MxpAxis.AxisState.standstill == 1 ? true : false;

                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            bool command_cancel = Convert.ToBoolean(command & enAxisOutFlag.MotionCancel) ? true : false;

                            if (complete)
                            {
                                m_MxpAxis.m_CommandRun = false;
                                MxpCommLog.WriteLog(string.Format("{0} RelativeMove Completed", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                if (m_MxpAxis.SequenceState.busy == 1) seqNo = 40;
                                else seqNo = 0;
                            }
                            else if (m_MxpAxis.IsAlarm() || m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                            {
                                if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                                    MxpCommLog.WriteLog(string.Format("{0} RelativeMove State Fail", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} RelativeMove NG - Mxp Alarm [mxp={1} driver={2}]", m_Axis.AxisName, string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList)));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                            else if (command_cancel)
                            {
                                m_MxpAxis.m_CommandRun = false;
                                MxpCommLog.WriteLog(string.Format("{0} Command Cancel", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 5 * 60 * 1000 || cancel)
                            {
                                if (cancel) MxpCommLog.WriteLog(string.Format("{0} RelativeMove State Abort", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} RelativeMove NG - Timeout", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                        }
                        break;

                    case 30:
                        {
                            bool stop = m_MxpAxis.AxisState.standstill == 1 ? true : false;
                            stop &= m_MxpAxis.SequenceState.busy == 0 ? true : false;
                            stop &= m_MxpAxis.MoveState != MXP_MOVESTATE.MOVESTATE_MOVING ? true : false;
                            if (stop)
                            {
                                m_MxpAxis.m_CommandRun = false;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 500)
                            {
                                m_MxpAxis.m_CommandRun = false;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 40:
                        {
                            MXP.MXP_FUNCTION_STATUS_RESULT rv = m_MxpAxis.Stop();
                            MxpCommLog.WriteLog(string.Format("{0} RelativeMove Busy ===> Stop", m_Axis.AxisName));
                            seqNo = 0;
                        }
                        break;

                    case 100:
                        {
                            if (m_MxpAxis.IsReady())
                            {
                                m_MxpAxis.m_CommandRun = false;
                                seqNo = 0;
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }
            private void HomePositionCheck()
            {
                if (m_MxpAxis.m_CommandRun)
                {
                    if (m_HomeOffCheck && m_MxpAxis.AxisState.homeAbsSwitch == 0)
                    {
                        m_HomeOffCheck = false;
                        MxpCommLog.WriteLog(string.Format("{0} Home Sensor OFF Position = {1}", m_Axis.AxisName, m_Axis.CurPos));
                    }
                    else if (m_HomeOnCheck && m_MxpAxis.AxisState.homeAbsSwitch == 1)
                    {
                        m_HomeOnCheck = false;
                        double error = m_Axis.HomeOffset + m_Axis.CurPos;
                        (m_Axis as MpAxis).OriginOnDetectError = error;
                        MxpCommLog.WriteLog(string.Format("{0} Home Sensor ON Position = {1}", m_Axis.AxisName, m_Axis.CurPos));
                    }
                }
                else
                {
                    m_HomeOffCheck = false;
                    m_HomeOnCheck = false;
                }
            }
        }
        private class SeqProfileMove : XSeqFunc
        {
            private MxpAxis m_MxpAxis = null;
            private AxisBlockMXP m_AxisBlock = null;
            private _Axis m_Axis = null;
            public SeqProfileMove(MxpAxis mxp_axis)
            {
                this.SeqName = $"SeqProfileMove{mxp_axis.m_Axis.AxisName}";
                m_MxpAxis = mxp_axis;
                m_AxisBlock = mxp_axis.m_AxisBlock;
                m_Axis = mxp_axis.m_Axis;
            }

            public override int Do()
            {
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_AxisBlock.Connected == false) break;

                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            if (Convert.ToBoolean(command & enAxisOutFlag.SequenceMotionStart) && m_Axis.SequenceState.IsExternalEncoderRun == false)
                            {
                                //if (m_Axis.HomeMethod == enHomeMethod.ABS_ZERO && Math.Abs(m_Axis.CurPos) > 0.1f && m_MxpAxis.IsReady())
                                //{
                                //    // SetPosition(0.0f)을 하면 HomeEnd Flag가 죽는다...이상하다...
                                //    m_MxpAxis.CommandResult = m_MxpAxis.Home();
                                //    StartTicks = XFunc.GetTickCount();
                                //    if (m_MxpAxis.CommandResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR) seqNo = 10;
                                //    else seqNo = 50;
                                //}
                                //else
                                {
                                    m_MxpAxis.m_OverrideStop = false;
                                    m_MxpAxis.m_InRangeError = false;
                                    m_MxpAxis.m_InRangeChecking = false;
                                    MxpCommLog.WriteLog(string.Format("{0} SequenceMove {1}", m_Axis.AxisName, m_MxpAxis.IsReady() ? "Ready State" : "NotReady State"));
                                    seqNo = 20;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            // SetPosition(0.0f)을 하면 HomeEnd Flag가 죽는다...이상하다...
                            if (XFunc.GetTickCount() - StartTicks < 100) break;
                            if (m_Axis.HomeMethod == enHomeMethod.ABS_ZERO && m_MxpAxis.AxisState.isHomed == 0) break;
                            MxpCommLog.WriteLog(string.Format("{0} SequenceMove HOME Confirm", m_Axis.AxisName));
                            seqNo = 20;
                        }
                        break;

                    case 20:
                        {
                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            bool command_cancel = Convert.ToBoolean(command & enAxisOutFlag.MotionCancel) ? true : false;
                            if (command_cancel)
                            {
                                command = enAxisOutFlag.CommandNone;
                                (m_Axis as IAxisCommand).SetCommandAsync(command);
                                MxpCommLog.WriteLog(string.Format("{0} SequenceMove Command Cancel - {1}", m_Axis.AxisName, m_MxpAxis.IsReady() ? "Ready State" : "NotReady State"));
                            }

                            MxpCommLog.WriteLog(string.Format("{0} SequenceMove Start", m_Axis.AxisName));
                            m_MxpAxis.CommandResult = m_MxpAxis.SequenceMove();

                            m_MxpAxis.m_ExternalEncoderAbort = false;
                            StartTicks = XFunc.GetTickCount();
                            if (m_MxpAxis.CommandResult == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR) seqNo = 30;
                            else
                            {
                                MxpCommLog.WriteLog(string.Format("{0} SequenceMove NG[{1}] - Mxp Alarm [mxp={2} driver={3}]", m_Axis.AxisName, m_MxpAxis.CommandResult.ToString(), string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList)));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 50;
                            }
                        }
                        break;

                    case 30:
                        {
                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            bool command_ack = command == enAxisOutFlag.CommandNone ? true : false;
                            bool command_cancel = Convert.ToBoolean(command & enAxisOutFlag.MotionCancel) ? true : false;

                            if (m_MxpAxis.SequenceState.busy == 1 && command_ack)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} SequenceMove Ing", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 40;
                            }
                            else if (m_MxpAxis.IsAlarm())
                            {
                                MxpCommLog.WriteLog(string.Format("{0} SequenceMove NG - Mxp Alarm [mxp={1} driver={2} sequence={3}]", m_Axis.AxisName, string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList), m_MxpAxis.SequenceState.errorID));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 50;
                            }
                            else if (command_cancel)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} SequenceMove Command Cancel", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                m_MxpAxis.m_ExternalEncoderAbort = true;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} SequenceMove NG - Not Response 1(sec)", m_Axis.AxisName)); //뭔가 이상하네... 처음으로 돌아가자... 상위에서 알람을 띄울거임.
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 50;
                            }
                        }
                        break;

                    case 40:
                        {
                            bool doneOn = m_MxpAxis.SequenceState.done == 1 ? true : false;
                            bool standstillOn = m_MxpAxis.AxisState.standstill == 1 ? true : false;
                            bool busyOff = m_MxpAxis.SequenceState.busy == 0 ? true : false;

                            bool complete = doneOn; //m_MxpAxis.SequenceState.done == 1 ? true : false;
                            complete |= standstillOn; //m_MxpAxis.AxisState.standstill == 1 ? true : false;
                            complete &= busyOff; //m_MxpAxis.SequenceState.busy == 0 ? true : false;
                            complete |= m_Axis.SequenceState.IsExternalEncoderRun ? true : false;

                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            bool command_cancel = Convert.ToBoolean(command & enAxisOutFlag.MotionCancel) ? true : false;
                            if (complete)
                            {
                                m_MxpAxis.m_SequenceMoving = false;
                                MxpCommLog.WriteLog(string.Format("{0} SequenceMove Completed, SequenceDoneOn={1},StandStillOn={2},BusyOff={3},IsExternalEncoderRun={4},", m_Axis.AxisName, doneOn, standstillOn, busyOff, m_Axis.SequenceState.IsExternalEncoderRun));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 0;
                            }
                            else if (m_MxpAxis.IsAlarm())
                            {
                                MxpCommLog.WriteLog(string.Format("{0} SequenceMove NG - Mxp Alarm [mxp={1} driver={2} sequence={3}]", m_Axis.AxisName, string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList), m_MxpAxis.SequenceState.errorID));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 50;
                            }
                            else if (command_cancel)
                            {
                                m_MxpAxis.m_ExternalEncoderAbort = true;
                                MxpCommLog.WriteLog(string.Format("{0} SequenceMove Command Cancel", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 0;
                            }
                        }
                        break;

                    case 50:
                        {
                            bool stop = m_MxpAxis.AxisState.standstill == 1 ? true : false;
                            stop &= m_MxpAxis.SequenceState.busy == 0 ? true : false;
                            stop &= m_MxpAxis.MoveState != MXP_MOVESTATE.MOVESTATE_MOVING ? true : false;
                            if (stop)
                            {
                                m_MxpAxis.m_SequenceMoving = false;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 500)
                            {
                                seqNo = 0;
                            }
                        }
                        break;

                    case 100:
                        {
                            if (m_MxpAxis.IsReady())
                            {
                                seqNo = 0;
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }
        }
        private class SeqJog : XSeqFunc
        {
            private MxpAxis m_MxpAxis = null;
            private AxisBlockMXP m_AxisBlock = null;
            private _Axis m_Axis = null;
            public SeqJog(MxpAxis mxp_axis)
            {
                this.SeqName = $"SeqJog{mxp_axis.m_Axis.AxisName}";
                m_MxpAxis = mxp_axis;
                m_AxisBlock = mxp_axis.m_AxisBlock;
                m_Axis = mxp_axis.m_Axis;
            }

            public override int Do()
            {
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_AxisBlock.Connected == false) break;

                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            if (Convert.ToBoolean(command & enAxisOutFlag.JogPlus) || Convert.ToBoolean(command & enAxisOutFlag.JogMinus))
                            {
                                if (m_MxpAxis.IsReady())
                                {
                                    MXP_DIRECTION_ENUM dir = Convert.ToBoolean(command & enAxisOutFlag.JogPlus) ? MXP_DIRECTION_ENUM.MXP_POSITIVE_DIRECTION :
                                        Convert.ToBoolean(command & enAxisOutFlag.JogMinus) ? MXP_DIRECTION_ENUM.MXP_NEGATIVE_DIRECTION : MXP_DIRECTION_ENUM.MXP_NONE_DIRECTION;
                                    MXP.MXP_FUNCTION_STATUS_RESULT rv = m_MxpAxis.Jog(dir);
                                    MxpCommLog.WriteLog(string.Format("{0} Jog {1} Start", m_Axis.AxisName, dir == MXP_DIRECTION_ENUM.MXP_POSITIVE_DIRECTION ? "Plus" : "Minus"));
                                    StartTicks = XFunc.GetTickCount();
                                    if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR) seqNo = 10;
                                    else seqNo = 100;
                                }
                                else
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} Jog NG - Not Ready", m_Axis.AxisName));
                                    seqNo = 100;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                            if (!Convert.ToBoolean(command & enAxisOutFlag.JogPlus) && !Convert.ToBoolean(command & enAxisOutFlag.JogMinus))
                            {
                                MXP.MXP_FUNCTION_STATUS_RESULT rv = m_MxpAxis.Stop();
                                MxpCommLog.WriteLog(string.Format("{0} Jog Stop", m_Axis.AxisName));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 100:
                        {
                            if (m_MxpAxis.IsReady())
                            {
                                seqNo = 0;
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }
        }
        #endregion
        #region Thread - Task Sequence Move Stop
        private class TaskSequenceMoveMonitor : XSequence
        {
            public TaskSequenceMoveMonitor(MxpAxis mxp_axis)
            {
                SetSubInfo(mxp_axis.m_Axis.AxisName);

                if (mxp_axis.m_Axis.GantryType && mxp_axis.m_Axis.NodeId != mxp_axis.m_Axis.SlaveNodeId)
                    RegSeq(new SeqProfileMonitor(mxp_axis));
            }
        }
        private class SeqProfileMonitor : XSeqFunc
        {
            private MxpAxis m_MxpAxis = null;
            private AxisBlockMXP m_AxisBlock = null;
            private _Axis m_Axis = null;
            private int nRetry = 0;
            private bool m_StopMonitorRun = false;
            private bool m_InRangeCheckOK = false;
            private double m_AccumulateVelocity = 0.0f;
            private bool m_CompleteTimeCheck = false;
            private uint m_CompleteTimeTick = 0;
            private bool m_OneMoreMoveStart = false;

            public SeqProfileMonitor(MxpAxis mxp_axis)
            {
                this.SeqName = $"SeqProfileMonitor{mxp_axis.m_Axis.AxisName}";
                m_MxpAxis = mxp_axis;
                m_AxisBlock = mxp_axis.m_AxisBlock;
                m_Axis = mxp_axis.m_Axis;
            }

            public override int Do()
            {
                if (m_AxisBlock.Connected == false) return -1;
                if (m_Axis.GantryType && m_Axis.NodeId == m_Axis.SlaveNodeId) return -1; //Gantry Slave not need override

                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            m_MxpAxis.CheckMoveState();
                            // Motor Encorder 위치 오차가 크다. BCR RemainDistance를 활용하자~~~
                            double remain_motor_distance = m_Axis.TargetPos - m_Axis.CurPos;
                            double sensor_remain_distance = (m_Axis as MpAxis).SensorRemainDistance; //BCR Remain
                            double following_error = sensor_remain_distance - remain_motor_distance;
                            if (following_error < 0.0f) following_error = 0.0f;
                            else if (following_error > 1000.0f) following_error = 0.0f; // 너무 큰 경우
                            bool near_check = sensor_remain_distance < 1.5f * m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorScanDistance ? true : false; // 근처에 왔구나 ~~~
                            near_check |= remain_motor_distance < 1.5f * m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorScanDistance ? true : false; // 근처에 왔구나 ~~~
                            near_check |= remain_motor_distance < m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorScanDistance + following_error;
                            bool run_condition = true;
                            run_condition &= m_MxpAxis.m_SequenceMoving ? true : false;
                            run_condition &= m_MxpAxis.m_ExternalEncoderAbort ? false : true;
                            run_condition &= m_MxpAxis.m_SequenceCommand.PositionSensorInfo.ControlMp ? true : false;
                            run_condition &= m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorTargetValue > 0.0f ? true : false;
                            run_condition &= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_ABORT ? false : true;
                            run_condition &= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_NULL ? false : true;
                            run_condition &= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_MOVING ? true : false;
                            run_condition &= m_MxpAxis.AxisState.standstill == 0 ? true : false;
                            run_condition &= near_check; // 근처에 왔구나 ~~~
                            run_condition &= remain_motor_distance > 0.0f; // 이건 이상하다
                            //run_condition &= (m_Axis as MpAxis).TargetOverrideRate > 0.0f ? true : false;
                            run_condition &= Convert.ToBoolean(m_Axis.AxisStatus & enAxisInFlag.Busy) ? true : false;
                            run_condition &= (m_Axis as MpAxis).SequenceMoveCommandSet == false;
                            if (run_condition)
                            {
                                m_StopMonitorRun = (m_Axis as MpAxis).CommandSpeed < 5.0f ? true : false;
                                nRetry = 0;
                                MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor BCR Scan Start", m_Axis.AxisName));
                                seqNo = 10;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (m_MxpAxis.m_ExternalEncoderAbort || m_MxpAxis.m_SequenceMoving == false || (m_Axis as MpAxis).SequenceMoveCommandSet)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor BCR Scan Cancel", m_Axis.AxisName));
                                if (m_MxpAxis.m_ExternalEncoderAbort)
                                    MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor BCR Scan Abort", m_Axis.AxisName));
                                if (m_MxpAxis.m_SequenceMoving == false)
                                    MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor Sequence Moving false", m_Axis.AxisName));
                                if ((m_Axis as MpAxis).SequenceMoveCommandSet)
                                    MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor New SequenceMove Command Exist", m_Axis.AxisName));
                                seqNo = 0;
                            }
                            else
                            {
                                double remain_motor_distance = m_Axis.TargetPos - m_Axis.CurPos;
                                double sensor_remain_distance = (m_Axis as MpAxis).SensorRemainDistance; //BCR Remain
                                double following_error = sensor_remain_distance - remain_motor_distance; ;
                                if (following_error < 0.0f) following_error = 0.0f;
                                else if (following_error > 1000.0f) following_error = 0.0f; // 너무 큰 경우

                                // 잔여 거리 계산
                                uint slave_no = (uint)m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SlaveNo; // 9, 10 번
                                double curBCR = slave_no == m_Axis.LeftBcrNodeId ? m_Axis.LeftBcrPos : m_Axis.RightBcrPos;
                                double remain_bcr_distance = m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorTargetValue - curBCR;
                                double collision_distance = (m_Axis as MpAxis).OverrideCollisionDistance - (m_Axis as MpAxis).OverrideStopDistance; // 1300 기준으로 안쪽은 멈출수 없다. OverrideLimitDistance = 300 까지는 멈출수 있다.

                                bool normal_run = true;
                                normal_run &= m_MxpAxis.m_SequenceMoving;
                                normal_run &= remain_bcr_distance > 0.0f;
                                normal_run &= remain_bcr_distance < (m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorScanDistance + following_error);
                                normal_run &= remain_bcr_distance < collision_distance;
                                if (AppConfig.Instance.Simulation.MY_DEBUG)
                                {
                                    normal_run &= following_error < 1.0f;
                                }

                                bool abnormal_run = true; // 조건이 되었는데 왜 않 움직이지 ?
                                abnormal_run &= m_MxpAxis.m_SequenceMoving;
                                abnormal_run &= remain_bcr_distance > 0.0f;
                                abnormal_run &= remain_bcr_distance < 100.0f;
                                abnormal_run &= (m_Axis as MpAxis).OverrideCollisionDistance > 300.0f;

                                if (normal_run || abnormal_run) //충돌 전에 멈출수 있다. 그렇지 않으면 Override로 멈추겠지...
                                {
                                    bool command_set_ok = true;

                                    double commandPos = (m_Axis as MpAxis).CommandPos;
                                    double commandVel = (m_Axis as MpAxis).CommandSpeed;
                                    double actualVel = m_Axis.CurSpeed;
                                    //double targetvel = commandVel < m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorScanVelocity ?
                                    //    m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorScanVelocity : commandVel;
                                    double targetvel = Math.Min(commandVel, m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorScanVelocity);
                                    double targetacc = m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorScanAcceleration;
                                    double targetdec = m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorScanDeceleration;
                                    double targetjerk = m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorScanJerk;

                                    if (actualVel > targetvel)
                                    {
                                        if (Math.Abs(targetvel - actualVel) > 0.3f * targetvel) targetvel = Math.Min(targetvel, actualVel); // 가장 이상적인 경우는 현 속도를 유지하면서 BCR 위치에 도달하는 것.
                                      //else targetvel = actualVel; // 최저 속도 제한 = 300.0f mm/sec
                                      // 거리에 대한 최대 속도 제한 = 거리의 반
                                    }

                                    //감속거리를 계산하여 Target 위치에 멈출수 있는지 확인
                                    double dec_time = targetvel / m_Axis.DecLimit;
                                    double dec_distance = 0.5f * targetdec * dec_time * dec_time;
                                    if (dec_distance < remain_bcr_distance) // 정지 불가.. 가능한 가속도 계산해 보자~~
                                    {
                                        double new_dec = (2 * remain_bcr_distance) / (dec_time * dec_time);
                                    }

                                    if (targetvel > 2.0f * remain_bcr_distance) targetvel = 0.5f * remain_bcr_distance;
                                    if (targetvel < 100.0f)
                                    {
                                        if (targetvel > remain_bcr_distance) targetvel = remain_bcr_distance;
                                        else if (targetvel < 0.3f * remain_bcr_distance) targetvel = 0.3f * remain_bcr_distance; // targetvel이 0에 가까운 경우 발생 방지
                                        //else targetvel = 100.0f;
                                    }
                                    if (targetvel < 100.0f) targetvel = 100.0f;
                                    // acc, dec, jerk 제한
                                    if (targetvel < 720.0f)
                                    {
                                        if (m_Axis.BcrControl == enMxpBcrControl.PLC)
                                        {
                                            targetacc = targetvel;
                                            targetdec = targetvel;
                                            targetjerk = targetvel;
                                        }
                                        else
                                        {
                                            targetacc = 2 * targetvel;
                                            targetdec = 2 * targetvel;
                                            targetjerk = 0.8f * targetdec;
                                        }
                                    }

                                    // Target Position
                                    // Current BCR Position (Using Sensor에 따른 좌우 판단)
                                    double bcr_offset = 0; // slave_no == m_Axis.LeftBcrNodeId ? m_Axis.LeftBCROffset : m_Axis.RightBCROffset;
                                    double targetpos = commandPos + remain_bcr_distance + bcr_offset;
                                    if(AppConfig.Instance.Simulation.MY_DEBUG)
                                        targetpos = m_Axis.CurPos + remain_bcr_distance + bcr_offset;

                                    // 감속도 계산, 남은 거리로 갈수 있는 최대 속도 계산
                                    // 가속거리 = 0.5f * a * t1^2, 감속거리 = 0.5f * d * t3^2, 정속거리 = v * t2
                                    // 이동거리 = 가속거리 + 정속거리 + 감속거리
                                    double t1 = actualVel / targetacc;
                                    double t3 = targetvel / targetdec;
                                    double s1 = 0.5f * targetacc * t1 * t1;
                                    double s3 = 0.5f * targetdec * t3 * t3;
                                    double constant_velocity_distance = remain_bcr_distance - s1 - s3;
                                    if (constant_velocity_distance <= 0) // 정속 거리가 나오지 않으면 Max 속도를 새로 계산하자 
                                    {
                                        double acc_time_ratio = (t1) / (t1 + t3);
                                        double acc_len = acc_time_ratio * remain_bcr_distance;
                                        double acc_time = Math.Sqrt((2 * acc_len) / (targetacc));
                                        targetvel = 0.5f * targetacc * acc_time;
                                        if (targetvel < 10.0f) targetvel = 10.0f; // Velocity 0으로 계산되는 경우가 있네.
                                    }

                                    // 만일 ABS 명령을 날려도 멈출수 없는 거리/속도 라면 알람을 띄우자~~~
                                    // BCR Scan 가감속으로 멈출수 있는거리보다 짧으면 Alarm을 띄워야 할것 같다.
                                    double stop_time = targetvel / targetdec;
                                    double stop_distance = 0.5f * targetdec * stop_time * stop_time;
                                    command_set_ok &= stop_distance < 2 * (remain_bcr_distance + bcr_offset); // 2배보다 적은 경우 가능하지 않을까 ?
                                    if (command_set_ok)
                                    {
                                        m_StopMonitorRun = false;
                                        // ABS Command Set
                                        string msg = string.Empty;
                                        MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
                                        if (m_Axis.BcrControl == enMxpBcrControl.MXP)
                                        {
                                            msg += string.Format("[{0}, {1}, {2}, {3}, {4}, {5}， {6}, {7}, {8}]", targetpos, targetvel, targetacc, targetdec, targetjerk, remain_bcr_distance, commandVel, commandPos, curBCR);
                                            MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor AX_MoveAbsolute Move Command : \r\n[Position, Velocity, Acc, Dec, Jerk, remain_bcr_distance， commandVel, commandPos, curBCR]\r\n{1}", m_Axis.AxisName, msg));

                                            rv = MXP.AX_MoveAbsolute((uint)m_Axis.NodeId, (float)targetpos, (float)targetvel, (float)targetacc, (float)targetdec, (float)targetjerk, MXP.MXP_DIRECTION_ENUM.MXP_NONE_DIRECTION, MXP.MXP_BUFFERMODE_ENUM.MXP_ABORT_OVERRIDERESET);
                                        }
                                        else if (m_Axis.BcrControl == enMxpBcrControl.PLC)
                                        {
                                            float cmd_offset = 0.0f;
                                            if (m_Axis.SequenceCommand.PositionSensorInfo.SlaveNo == m_Axis.LeftBcrNodeId) cmd_offset = (float)(m_Axis.LeftBCRCmdOffset + m_Axis.LeftBCROffset);
                                            else if (m_Axis.SequenceCommand.PositionSensorInfo.SlaveNo == m_Axis.RightBcrNodeId) cmd_offset = (float)(m_Axis.RightBCRCmdOffset + m_Axis.RightBCROffset);

                                            MxpCommLog.WriteLog(string.Format("{0} Set Target BCR To BCR = {1}", m_Axis.AxisName, m_Axis.SequenceCommand.PositionSensorInfo.SensorTargetValue + cmd_offset));

                                            #region 2024.12.16 PLC
                                            //정위치 정지 변경.
                                            //1. PLC에 정위치정지시 필요한 목표 BCR정보, 속도, 가감속, 저크를 써준다. (Write PLC Data)
                                            //2. PLC에 정위지 정지 시작 신호를 킨다. (Write Start On)
                                            //3. PLC에서 정위치 정지를 완료하고 시작 신호를 끄고 완료 신호를 켜준다. (Read Start Off, End On)
                                            //4. 정위치 정지 완료 신호를 확인하고 정위치 정지 확인 후 완료 신호를 끈다.(Read End On, Write End Off)
                                            //2024.12.15 PLC에 정위치 정지에 필요한 Data를 써준다.
                                            msg += string.Format("[{0}, {1}, {2}, {3}, {4}, {5}， {6}, {7}, {8}]", targetpos, targetvel, targetacc, targetdec, targetjerk, remain_bcr_distance, commandVel, commandPos, curBCR);
                                            MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor Set ExactPostion Data : \r\n[Position, Velocity, Acc, Dec, Jerk, remain_bcr_distance， commandVel, commandPos, curBCR]\r\n{1}", m_Axis.AxisName, msg));

                                            rv = MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
                                            rv &= MXP.PLC_WriteBuffer((UInt32)enExactPosition.EXACT_POSITION_TARGETBCR, 4, BitConverter.GetBytes((UInt32)(m_Axis.SequenceCommand.PositionSensorInfo.SensorTargetValue + cmd_offset) * 10));
                                            rv &= MXP.PLC_WriteBuffer((UInt32)enExactPosition.EXACT_POSITION_VELOCITY, 4, BitConverter.GetBytes((float)targetvel));
                                            rv &= MXP.PLC_WriteBuffer((UInt32)enExactPosition.EXACT_POSITION_ACC, 4, BitConverter.GetBytes((float)targetacc));
                                            rv &= MXP.PLC_WriteBuffer((UInt32)enExactPosition.EXACT_POSITION_DEC, 4, BitConverter.GetBytes((float)targetdec));
                                            rv &= MXP.PLC_WriteBuffer((UInt32)enExactPosition.EXACT_POSITION_JERK, 4, BitConverter.GetBytes((float)targetjerk));
                                            rv &= MXP.PLC_WriteBuffer((UInt32)enDriveParameter.BCR_READ_DIRECTION, 2, BitConverter.GetBytes((UInt16)m_Axis.SequenceCommand.PositionSensorInfo.SlaveNo == m_Axis.LeftBcrNodeId ? 0 : 1));


                                            //PLC Test 용.. 강제로 2rd 속도를 조정한다.
                                            //double srd_Vel = 20.0f;
                                            //double srd_Acc = 20.0f;
                                            //double srd_Jetk = 40.0f;
                                            //int cycle_count = 4;
                                            //double position_ratio = 50.0f;
                                            //double velocity_ratio = 20.0f;
                                            //rv &= MXP.PLC_WriteBuffer((UInt32)enExactPosition.EXACT_CYCLE_COUNT, 4, BitConverter.GetBytes((UInt32)cycle_count));
                                            //rv &= MXP.PLC_WriteBuffer((UInt32)enExactPosition.COMPLETE_POSITION_RATIO, 4, BitConverter.GetBytes((float)position_ratio));
                                            //rv &= MXP.PLC_WriteBuffer((UInt32)enExactPosition.COMPLETE_VELOCITY_RATIO, 4, BitConverter.GetBytes((float)velocity_ratio));
                                            //rv &= MXP.PLC_WriteBuffer((UInt32)enExactPosition.SECOND_VELOCITY, 4, BitConverter.GetBytes((float)srd_Vel));
                                            //rv &= MXP.PLC_WriteBuffer((UInt32)enExactPosition.SECOND_ACC, 4, BitConverter.GetBytes((float)srd_Acc));
                                            //rv &= MXP.PLC_WriteBuffer((UInt32)enExactPosition.SECOND_JERK, 4, BitConverter.GetBytes((float)srd_Jetk));
                                            #endregion

                                        }
                                        /////////////////////////////////////////////////////////////////////////////////////
                                        if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                                        {
                                            MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor AX_MoveAbsolute Move OK {1}", m_Axis.AxisName, rv.ToString()));

                                            m_Axis.TargetPos = targetpos; //이걸 Update 해줘야 ... 운영쪽에서도 이동 위치를 알거야...
                                                                          // abs move를 하면 override를 1로 변경함. 이전 값으로 재 설정 필요
                                            m_Axis.TargetSpeed = targetvel;
                                            m_Axis.TargetAcc = targetacc;
                                            m_Axis.TargetDec = targetdec;
                                            m_Axis.TargetJerk = targetjerk;
                                            m_MxpAxis.m_OverrideRate = 1.0f; // MXP에서 1.0f으로 변경했음.

                                            if (m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorUse == 1)
                                            {
                                                rv = m_MxpAxis.SetExternalExcorder(targetpos);
                                            }
                                            if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                                            {
                                                if (m_Axis.SequenceState.IsExternalEncoderRun == false && targetvel > 300.0f) m_OneMoreMoveStart = true;
                                                m_MxpAxis.m_ProfileMoveMonitoringInit = false;                                                
                                                StartTicks = XFunc.GetTickCount();
                                                seqNo = 20;
                                            }
                                        }
                                        else
                                        {
                                            MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor AX_MoveAbsolute Move NG {1}", m_Axis.AxisName, rv.ToString()));
                                            StartTicks = XFunc.GetTickCount();
                                            seqNo = 100;
                                        }
                                    }
                                    else if (!m_StopMonitorRun)
                                    {
                                        // 속도가 올라갈때까지 기다려야 하는 경우가 있다...!
                                        MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor AX_MoveAbsolute Move NG. RemainDistance={1}, Current Velocity={2}", m_Axis.AxisName, remain_bcr_distance, actualVel));
                                        StartTicks = XFunc.GetTickCount();
                                        seqNo = 100;
                                    }
                                }
                            }
                        }
                        break;

                    case 20:
                        {
                            m_MxpAxis.CheckMoveState();
                            bool cancel = false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_ABORT ? true : false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_NULL ? true : false;
                            cancel &= Convert.ToBoolean(m_Axis.AxisStatus & enAxisInFlag.Busy) ? false : true;

                            if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_MOVING)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Move Ing", m_Axis.AxisName));
                                m_AccumulateVelocity = 0.0f;
                                m_CompleteTimeCheck = false;
                                m_CompleteTimeTick = XFunc.GetTickCount();
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                            else if (m_MxpAxis.IsAlarm() || m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                            {
                                if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                                    MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor State Fail", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor NG - Mxp Alarm [mxp={1} driver={2}]", m_Axis.AxisName, string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList)));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 90;
                            }
                            else if (m_MxpAxis.m_ExternalEncoderAbort)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor Command Cancel", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 100;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 1000 || cancel)
                            {
                                if (cancel) MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor State Abort", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor NG - Not Response 1(sec)", m_Axis.AxisName)); //뭔가 이상하네... 처음으로 돌아가자... 상위에서 알람을 띄울거임.
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 90;
                            }
                        }
                        break;

                    case 30:
                        {
                            m_MxpAxis.CheckMoveState();
                            bool cancel = false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_ABORT ? true : false;
                            cancel |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_NULL ? true : false;
                            cancel &= Convert.ToBoolean(m_Axis.AxisStatus & enAxisInFlag.Busy) ? false : true;

                            bool plc_control_complete = false;
                            if (m_Axis.BcrControl == enMxpBcrControl.PLC)
                            {
                                //PLC에서 Move 상태를 Abort로 변경하여 완료처리가 안되네...
                                //임시로 PLC의 완료상태를 받아와서 완료처리를 해보자..
                                Byte nExactEnd = 0;
                                MXP.MXP_FUNCTION_STATUS_RESULT nResult = MXP.PLC_ReadBit((UInt32)enExactPosition.EXACT_POSITION_STATE, Convert.ToByte((int)enExactPositionStatus.End), ref nExactEnd);
                                plc_control_complete = nExactEnd == 1;
                            }
                            bool complete = m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_COMPLETE ? true : false;
                            complete |= m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_WAIT_INPOS ? true : false;
                            complete |= plc_control_complete;
                            complete &= m_MxpAxis.AxisState.standstill == 1 ? true : false;

                            // BCR Scan이 아닌경우 오차가 심할수 있다. 100mm이내에 남고 100mm/sec 이하로 속도가 떨어지면 한번더 계산해서 이동 시키자...!
                            double motor_remain = m_Axis.TargetPos - m_Axis.CurPos;
                            bool abs_move_retry = m_OneMoreMoveStart; 
                            abs_move_retry &= m_Axis.SequenceState.IsExternalEncoderRun == false ? true : false;
                            abs_move_retry &= motor_remain > 10;
                            abs_move_retry &= motor_remain < 30.0f || m_Axis.CurSpeed < 100.0f;
                            abs_move_retry &= (m_Axis as MpAxis).SequenceMoveCommandSet == false;

                            if (m_MxpAxis.m_ProfileMoveMonitoringInit)
                            {
                                m_MxpAxis.m_ProfileMoveMonitoringInit = false;
                                // 잔여 거리 계산
                                uint slave_no = (uint)m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SlaveNo; // 9, 10 번
                                double curBCR = slave_no == m_Axis.LeftBcrNodeId ? m_Axis.LeftBcrPos : m_Axis.RightBcrPos;
                                double remain_bcr_distance = m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorTargetValue - curBCR;

                                MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor New Command [RemainDistance = {1}]", m_Axis.AxisName, remain_bcr_distance));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 0;
                            }
                            else if (m_MxpAxis.m_ExternalEncoderAbort)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor Command Cancel", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 100;
                            }
                            else if (m_MxpAxis.IsAlarm() || m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                            {
                                if (m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_FAIL)
                                    MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor State Fail", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor NG - Mxp Alarm [mxp={1} driver={2}]", m_Axis.AxisName, string.Join("-", (m_Axis as MpAxis).ControllerAlarmIdList), string.Join("-", (m_Axis as MpAxis).DriverAlarmIdList)));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 90;
                            }
                            else if (/*XFunc.GetTickCount() - StartTicks > 5 * 60 * 1000 || */cancel)
                            {
                                if (cancel) MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor State Abort", m_Axis.AxisName));
                                else MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor NG - Timeout", m_Axis.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 90;
                            }
                            else if (abs_move_retry)
                            {
                                m_OneMoreMoveStart = false;
                                MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor One More Move Start", m_Axis.AxisName));
                                seqNo = 200;
                            }
                            else if (complete)
                            {
                                uint slave_no = (uint)m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SlaveNo; // 9, 10 번
                                double curBCR = slave_no == m_Axis.LeftBcrNodeId ? m_Axis.LeftBcrPos : m_Axis.RightBcrPos;

                                MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor Completed {1},{2}", m_Axis.AxisName, (m_Axis as MpAxis).CommandPos, curBCR));
                                StartTicks = XFunc.GetTickCount();
                                if (m_Axis.SequenceState.IsExternalEncoderRun)
                                {
                                    m_MxpAxis.m_InRangeChecking = true;
                                    seqNo = 40;
                                }
                                else seqNo = 100;
                            }
                            else
                            {
                                // TraVelocity가 0이 아닌 경우가 있다. cmd_inrange를 만들자 ...
                                // CurVelocity의 누적이 1000 이상인 경우는 Oscillation 이라고 생각하여 InRange Alarm을 띄우자 ~~~
                                // 5msec 한번 누적, 1sec 200번, 떨림 50mm/sec이면, 1sec 10000
                                if (m_CompleteTimeCheck == false && XFunc.GetTickCount() - m_CompleteTimeTick > 1000)
                                {
                                    // 지령 위치에 도달했네....지령 속도가 0인데 .... 속도가 왔다갔다하는 경우....
                                    bool cmd_inrange = Math.Abs(m_Axis.TargetPos - (m_Axis as MpAxis).CommandPos) < 1.0f;
                                    cmd_inrange &= Math.Abs((m_Axis as MpAxis).CommandSpeed) < 0.1f;
                                    if (cmd_inrange)
                                    {
                                        m_CompleteTimeTick = XFunc.GetTickCount();
                                        m_CompleteTimeCheck = true;
                                    }
                                }
                                double actual_velocity = Math.Abs(m_Axis.CurSpeed);
                                if (m_CompleteTimeCheck && actual_velocity > 0.1f)
                                    m_AccumulateVelocity += actual_velocity;
                                if (m_AccumulateVelocity > 30000.0f && XFunc.GetTickCount() - m_CompleteTimeTick > 5000) // BCR Scan 시간을 5sec 주자... 계속 떨고 있다고 생각하자 ~~
                                {
                                    m_MxpAxis.m_InRangeChecking = false;
                                    m_MxpAxis.m_InRangeError = true;
                                    MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor InRange NG = {1}, Velocity Oscillation", m_Axis.AxisName, m_AccumulateVelocity));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 100;
                                }
                            }
                        }
                        break;

                    case 40:
                        {
                            bool plc_control_complete = false;
                            if (m_Axis.BcrControl == enMxpBcrControl.PLC)
                            {
                                //PLC에서 Move 상태를 Abort로 변경하여 완료처리가 안되네...
                                //임시로 PLC의 완료상태를 받아와서 완료처리를 해보자..
                                Byte nExactEnd = 0;
                                MXP.MXP_FUNCTION_STATUS_RESULT nResult = MXP.PLC_ReadBit((UInt32)enExactPosition.EXACT_POSITION_STATE, Convert.ToByte((int)enExactPositionStatus.End), ref nExactEnd);
                                plc_control_complete = nExactEnd == 1;
                            }

                            // 잔여 거리 계산
                            bool inPos = m_MxpAxis.MoveState == MXP_MOVESTATE.MOVESTATE_WAIT_INPOS ? true : false;
                            inPos |= plc_control_complete;

                            uint slave_no = (uint)m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SlaveNo; // 9, 10 번
                            double curBCR = slave_no == m_Axis.LeftBcrNodeId ? m_Axis.LeftBcrPos : m_Axis.RightBcrPos;
                            double remain_bcr_distance = m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorTargetValue - curBCR;
                            if (m_MxpAxis.m_ProfileMoveMonitoringInit)
                            {
                                m_MxpAxis.m_InRangeChecking = false;
                                m_MxpAxis.m_ProfileMoveMonitoringInit = false;
                                MxpCommLog.WriteLog(string.Format("{0} New Command Sequence Monitor = {1}", m_Axis.AxisName, remain_bcr_distance));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 0;
                            }
                            else if (Math.Abs(remain_bcr_distance) < m_Axis.InRangeValue && inPos)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor InRange#1 OK = {1}", m_Axis.AxisName, remain_bcr_distance));
                                StartTicks = XFunc.GetTickCount();
                                if (nRetry > 1)
                                {
                                    seqNo = 100;
                                }
                                else seqNo = 50;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000.0f)
                            {
                                m_MxpAxis.m_InRangeChecking = false;
                                m_MxpAxis.m_InRangeError = true;
                                MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor InRange NG = {1}", m_Axis.AxisName, remain_bcr_distance));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 100;
                            }
                        }
                        break;

                    case 50:
                        {
                            uint slave_no = (uint)m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SlaveNo; // 9, 10 번
                            double curBCR = slave_no == m_Axis.LeftBcrNodeId ? m_Axis.LeftBcrPos : m_Axis.RightBcrPos;
                            double remain_bcr_distance = m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorTargetValue - curBCR;
                            if (m_MxpAxis.m_ProfileMoveMonitoringInit)
                            {
                                m_MxpAxis.m_InRangeChecking = false;
                                m_MxpAxis.m_ProfileMoveMonitoringInit = false;
                                MxpCommLog.WriteLog(string.Format("{0} New Command Sequence Monitor = {1}", m_Axis.AxisName, remain_bcr_distance));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 0;
                            }
                            else if (Math.Abs(remain_bcr_distance) > m_Axis.InRangeValue)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor InRange#2 Over = {1}, Retry = {2}", m_Axis.AxisName, remain_bcr_distance, nRetry));
                                StartTicks = XFunc.GetTickCount();
                                //nRetry++; // NG Case는 증가하면 않된다. 
                                seqNo = 40;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 100.0f)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor InRange#2 OK = {1}, Retry = {2}", m_Axis.AxisName, remain_bcr_distance, nRetry));
                                StartTicks = XFunc.GetTickCount();
                                nRetry++;
                                seqNo = 40;
                            }
                        }
                        break;

                    case 90:
                        {
                            bool plc_control_complete = true;
                            if (m_Axis.BcrControl == enMxpBcrControl.PLC)
                            {
                                //PLC에서 Move 상태를 Abort로 변경하여 완료처리가 안되네...
                                //임시로 PLC의 완료상태를 받아와서 완료처리를 해보자..
                                Byte nExactEnd = 0;
                                MXP.MXP_FUNCTION_STATUS_RESULT nResult = MXP.PLC_ReadBit((UInt32)enExactPosition.EXACT_POSITION_STATE, Convert.ToByte((int)enExactPositionStatus.End), ref nExactEnd);
                                plc_control_complete &= nExactEnd == 1;
                            }

                            bool stop = m_MxpAxis.AxisState.standstill == 1 ? true : false;
                            stop &= m_MxpAxis.SequenceState.busy == 0 ? true : false;
                            stop &= m_MxpAxis.MoveState != MXP_MOVESTATE.MOVESTATE_MOVING ? true : false;
                            stop &= plc_control_complete;
                            if (stop)
                            {
                                seqNo = 100;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 500)
                            {
                                seqNo = 100;
                                MxpCommLog.WriteLog($"[{m_Axis.AxisName}] Stop Check NG : StandStill [{m_MxpAxis.AxisState.standstill == 1}], Busy[{m_MxpAxis.SequenceState.busy == 0}]," +
                                    $"Move[{m_MxpAxis.MoveState != MXP_MOVESTATE.MOVESTATE_MOVING}], nExactEnd[{plc_control_complete}]");
                            }
                        }
                        break;

                    case 100:
                        {
                            if (m_MxpAxis.m_ProfileMoveMonitoringInit)
                            {
                                double sensor_remain_distance = (m_Axis as MpAxis).SensorRemainDistance; //BCR Remain
                                m_MxpAxis.m_ProfileMoveMonitoringInit = false;
                                MxpCommLog.WriteLog(string.Format("{0} New Command Sequence Monitor = {1}", m_Axis.AxisName, sensor_remain_distance));
                                m_MxpAxis.m_InRangeChecking = false;
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 0;
                            }
                            else
                            {
                                MXP.MXP_FUNCTION_STATUS_RESULT rv = m_MxpAxis.Stop(false, true, false);
                                MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor Busy ===> Stop", m_Axis.AxisName));
                                m_MxpAxis.m_InRangeChecking = false;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 200:
                        {
                            if (m_MxpAxis.m_ExternalEncoderAbort || (m_Axis as MpAxis).SequenceMoveCommandSet)
                            {
                                MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor One More Move Cancel", m_Axis.AxisName));
                                if ((m_Axis as MpAxis).SequenceMoveCommandSet)
                                    MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor New SequenceMove Command Exist", m_Axis.AxisName));
                                seqNo = 0;
                            }
                            else
                            {
                                // Current Motor Condition
                                double commandPos = (m_Axis as MpAxis).CommandPos;
                                double commandVel = (m_Axis as MpAxis).CommandSpeed;
                                double targetvel = commandVel;
                                // 잔여 거리 계산
                                uint slave_no = (uint)m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SlaveNo; // 9, 10 번
                                double curBCR = slave_no == m_Axis.LeftBcrNodeId ? m_Axis.LeftBcrPos : m_Axis.RightBcrPos;
                                double remain_bcr_distance = m_MxpAxis.m_SequenceCommand.PositionSensorInfo.SensorTargetValue - curBCR;
                                if (remain_bcr_distance > 2) // 2mm 이상 남은 경우 한번더 이동하자~~
                                {
                                    if (targetvel > 2.0f * remain_bcr_distance) targetvel = 0.5f * remain_bcr_distance;
                                    if (targetvel < 100.0f)
                                    {
                                        if (targetvel > remain_bcr_distance) targetvel = remain_bcr_distance;
                                        else if (targetvel < 0.3f * remain_bcr_distance) targetvel = 0.3f * remain_bcr_distance; // targetvel이 0에 가까운 경우 발생 방지
                                                                                                                                 //else targetvel = 100.0f;
                                    }
                                    if (targetvel < 50.0f) targetvel = 50.0f;

                                    double targetpos = commandPos + remain_bcr_distance;
                                    double targetacc = (0.5f * targetvel * targetvel) / remain_bcr_distance;
                                    double targetdec = targetacc;
                                    double targetjerk = targetacc;

                                    // ABS Command Set
                                    string msg = string.Empty;
                                    MXP.MXP_FUNCTION_STATUS_RESULT rv = MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR;
                                    msg += string.Format("[{0}, {1}, {2}, {3}, {4}, {5}， {6}, {7}, {8}]", targetpos, targetvel, targetacc, targetdec, targetjerk, remain_bcr_distance, commandVel, commandPos, curBCR);
                                    MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor One More AX_MoveAbsolute Move Command : \r\n[Position, Velocity, Acc, Dec, Jerk, remain_bcr_distance， commandVel, commandPos, curBCR]\r\n{1}", m_Axis.AxisName, msg));
                                    rv = MXP.AX_MoveAbsolute((uint)m_Axis.NodeId, (float)targetpos, (float)targetvel, (float)targetacc, (float)targetdec, (float)targetjerk, MXP.MXP_DIRECTION_ENUM.MXP_NONE_DIRECTION, MXP.MXP_BUFFERMODE_ENUM.MXP_ABORT_OVERRIDERESET);

                                    if (rv == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                                    {
                                        MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor One More AX_MoveAbsolute Move OK {1}", m_Axis.AxisName, rv.ToString()));

                                        m_Axis.TargetPos = targetpos; //이걸 Update 해줘야 ... 운영쪽에서도 이동 위치를 알거야...
                                                                      // abs move를 하면 override를 1로 변경함. 이전 값으로 재 설정 필요
                                        m_Axis.TargetSpeed = targetvel;
                                        m_Axis.TargetAcc = targetacc;
                                        m_Axis.TargetDec = targetdec;
                                        m_Axis.TargetJerk = targetjerk;
                                        seqNo = 30;
                                    }
                                    else
                                    {
                                        MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor One More AX_MoveAbsolute Move NG {1}", m_Axis.AxisName, rv.ToString()));
                                        StartTicks = XFunc.GetTickCount();
                                        seqNo = 100;
                                    }
                                }
                                else
                                {
                                    MxpCommLog.WriteLog(string.Format("{0} Sequence Monitor One More AX_MoveAbsolute Move Not Need [Remain Distance ={1}]", m_Axis.AxisName, remain_bcr_distance));
                                    seqNo = 30;
                                }
                            }
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
