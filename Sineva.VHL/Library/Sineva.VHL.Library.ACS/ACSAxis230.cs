using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices; //for COMException class
using ACS.SPiiPlusNET;
using System.Diagnostics;
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.Library.ACS
{
    public enum enACSAction230 : ushort
    {
        CommandNone = 0x0,
        HomeStart = 1 << 1,
        AlarmClear = 1 << 2,
        MotionStart = 1 << 3,
        MotionStop = 1 << 4,
        JogPlus = 1 << 5,
        JogMinus = 1 << 6,
        ServoOn = 1 << 7,
        ServoOff = 1 << 8,
        Reset = 1 << 9,
    }

    class ACSAxis230
    {
        #region Fields
        private Api _Channel = null;

        private AxisBlockACS m_AxisBlock = null;
        private _Axis m_Axis = null;
        private bool m_CommandHome = false;
        private bool m_CommandRun = false;
        private bool m_HomeEnd = false;

        private double m_CurPosition = 0.0; // FPOS
        private double m_CurSpeed = 0.0; //RVEL
        private double m_CurTorque = 0; //RMS
        private MotorStates m_MotorState = 0; //MST
        private AxisStates m_AxisState = 0; //AST
        private SafetyControlMasks m_AxisSafty = 0; //FAULT
        private int m_AlarmCode = 0; //MERR
        private ProgramStates m_ProgramState = 0;

        private int m_SeqHomeNo = 0;
        private int m_SeqMoveNo = 0;
        private int m_SeqJogNo = 0;
        private int m_SeqSegmentMoveNo = 0;

        private uint m_TickCounts = 0;

        private readonly static object m_LockKey = new object();
        #endregion

        #region Properties
        public double CurPosition
        {
            get { return m_CurPosition; }
            set { m_CurPosition = value; }
        }
        public double CurSpeed
        {
            get { return m_CurSpeed; }
            set { m_CurSpeed = value; }
        }
        public double CurTorque
        {
            get { return m_CurTorque; }
            set { m_CurTorque = value; }
        }
        public bool CommandHome
        {
            get { return m_CommandHome; }
            set { m_CommandHome = value; }
        }
        public bool CommandRun
        {
            get { return m_CommandRun; }
            set { m_CommandRun = value; }
        }
        public MotorStates MotorState
        {
            get { return m_MotorState; }
            set { m_MotorState = value; }
        }
        public AxisStates AxisState
        {
            get { return m_AxisState; }
            set { m_AxisState = value; }
        }
        public SafetyControlMasks AxisSafty
        {
            get { return m_AxisSafty; }
            set { m_AxisSafty = value; }
        }
        public bool HomeEnd
        {
            get { return m_HomeEnd; }
            set { m_HomeEnd = value; }
        }
        public int AlarmCode
        {
            get { return m_AlarmCode; }
            set { m_AlarmCode = value; }
        }
        public string AxisName
        {
            get { return m_Axis.AxisName; }
        }
        public ProgramStates ProgramState
        {
            get { return m_ProgramState; }
            set { m_ProgramState = value; }
        }

        #endregion

        #region Constructor
        public ACSAxis230(Api Ch, AxisBlockACS block, _Axis axis)
        {
            _Channel = Ch;
            m_AxisBlock = block;
            m_Axis = axis;
            // 여기서 Thread 돌리지 말자....ACSAxisCtrl에서 하자.....
            TaskHandler.Instance.RegTask(new TaskACSAxis(this), 5, System.Threading.ThreadPriority.Highest);
        }

        public void Init()
        {
            CommandRun = false;
            CommandHome = false;
            m_SeqHomeNo = 0;
            m_SeqMoveNo = 0;
            m_SeqJogNo = 0;
            m_SeqSegmentMoveNo = 0;
        }
        private void ExLog(COMException Ex)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            string msg = string.Format("Error from {0}, Message : {1}, HRESULT : {2:X}", Ex.Source, Ex.Message, Ex.ErrorCode);
            ExceptionLog.WriteLog(method, msg);
        }
        #endregion

        #region Methods - Control
        public bool Enable()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            //lock (m_LockKey)
            {
                bool rv = false;
                object pWait = 0;
                try
                {
                    if (AppConfig.Instance.Simulation.ACS && m_AxisBlock.BlockId > 0) return true; // Simulator가 한개만 만들어 지네...

                    //_Channel.EnableAsync((Axis)m_Axis.NodeId);
                    //if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId) _Channel.EnableAsync((Axis)m_Axis.SlaveNodeId);

                    _Channel.Enable((Axis)m_Axis.NodeId);
                    if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId) _Channel.Enable((Axis)m_Axis.SlaveNodeId);

                    rv = true;
                }
                catch (ACSException Ex)
                {
                    ExceptionLog.WriteLog(method, string.Format("ACS Enable{0} : {1}", m_Axis.NodeId, Ex.Message));
                }
                catch (COMException Ex)
                {
                    ExLog(Ex);
                }
                return rv;
            }
        }

        public bool Disable()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            //lock (m_LockKey)
            {
                bool rv = false;
                object pWait = 0;
                try
                {
                    if (AppConfig.Instance.Simulation.ACS && m_AxisBlock.BlockId > 0) return true; // Simulator가 한개만 만들어 지네...

                    //_Channel.DisableAsync((Axis)m_Axis.NodeId);
                    //if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId) _Channel.DisableAsync((Axis)m_Axis.SlaveNodeId);
                    _Channel.Disable((Axis)m_Axis.NodeId);
                    _Channel.WaitMotorEnabled((Axis)m_Axis.NodeId, 0, 5000);
                    Init();
                    rv = true;
                }
                catch (ACSException Ex)
                {
                    ExceptionLog.WriteLog(method, string.Format("ACS Disable{0} : {1}", m_Axis.NodeId, Ex.Message));
                }
                catch (COMException Ex)
                {
                    ExLog(Ex);
                }
                return rv;
            }
        }

        public bool AlarmClear()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            //lock (m_LockKey)
            {
                bool rv = false;
                object pWait = 0;
                try
                {
                    if (AppConfig.Instance.Simulation.ACS && m_AxisBlock.BlockId > 0) return true; // Simulator가 한개만 만들어 지네...

                    if (m_AxisSafty != SafetyControlMasks.ACSC_NONE || m_AlarmCode > 0)
                    {
                        _Channel.FaultClear((Axis)m_Axis.NodeId);
                        if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId) _Channel.FaultClear((Axis)m_Axis.SlaveNodeId);
                    }
                    rv = true;
                }
                catch (ACSException Ex)
                {
                    ExceptionLog.WriteLog(method, string.Format("ACS AlarmClear{0} : {1}", m_Axis.NodeId, Ex.Message));
                }
                catch (COMException Ex)
                {
                    ExLog(Ex);
                }
                return rv;
            }
        }

        public bool Stop()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            //lock (m_LockKey)
            {
                bool rv = false;
                object pWait = 0;
                try
                {
                    if (AppConfig.Instance.Simulation.ACS && m_AxisBlock.BlockId > 0) return true; // Simulator가 한개만 만들어 지네...

                    _Channel.Kill((Axis)m_Axis.NodeId);
                    rv = true;
                }
                catch (ACSException Ex)
                {
                    ExceptionLog.WriteLog(method, string.Format("ACS Stop{0} : {1}", m_Axis.NodeId, Ex.Message));
                }
                catch (COMException Ex)
                {
                    ExLog(Ex);
                }
                return rv;
            }
        }

        public bool EStop()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            //lock (m_LockKey)
            {
                bool rv = false;
                object pWait = 0;
                try
                {
                    if (AppConfig.Instance.Simulation.ACS && m_AxisBlock.BlockId > 0) return true; // Simulator가 한개만 만들어 지네...

                    _Channel.Halt((Axis)m_Axis.NodeId);
                    if (m_Axis.GantryType && m_Axis.NodeId != m_Axis.SlaveNodeId)
                        _Channel.Halt((Axis)m_Axis.SlaveNodeId);

                    if (m_ProgramState.HasFlag(ProgramStates.ACSC_PST_RUN))
                    {
                        if (m_Axis.NodeId == m_Axis.MasterNodeId)
                            _Channel.StopBuffer((ProgramBuffer)m_Axis.MasterNodeId);
                    }

                    Init();
                    rv = true;
                }
                catch (ACSException Ex)
                {
                    ExceptionLog.WriteLog(method, string.Format("ACS EStop{0} : {1}", m_Axis.NodeId, Ex.Message));
                }
                catch (COMException Ex)
                {
                    ExLog(Ex);
                }
                return rv;
            }
        }

        public bool JogPlus()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            //lock (m_LockKey)
            {
                bool rv = false;
                object pWait = 0;
                try
                {
                    if (AppConfig.Instance.Simulation.ACS && m_AxisBlock.BlockId > 0) return true; // Simulator가 한개만 만들어 지네...

                    double speed = m_AxisBlock.GetJogSpeed(m_Axis.AxisId);
                    _Channel.Jog(MotionFlags.ACSC_AMF_VELOCITY, (Axis)m_Axis.NodeId, speed * (int)GlobalDirection.ACSC_POSITIVE_DIRECTION);
                    rv = true;
                }
                catch (ACSException Ex)
                {
                    ExceptionLog.WriteLog(method, string.Format("ACS JogPlus{0} : {1}", m_Axis.NodeId, Ex.Message));
                }
                catch (COMException Ex)
                {
                    ExLog(Ex);
                }
                return rv;
            }
        }

        public bool JogMinus()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            //lock (m_LockKey)
            {
                bool rv = false;
                object pWait = 0;
                try
                {
                    if (AppConfig.Instance.Simulation.ACS && m_AxisBlock.BlockId > 0) return true; // Simulator가 한개만 만들어 지네...

                    double speed = m_AxisBlock.GetJogSpeed(m_Axis.AxisId);
                    _Channel.Jog(MotionFlags.ACSC_AMF_VELOCITY, (Axis)m_Axis.NodeId, speed * (int)GlobalDirection.ACSC_NEGATIVE_DIRECTION);
                    rv = true;
                }
                catch (ACSException Ex)
                {
                    ExceptionLog.WriteLog(method, string.Format("ACS JogMinus{0} : {1}", m_Axis.NodeId, Ex.Message));
                }
                catch (COMException Ex)
                {
                    ExLog(Ex);
                }
                return rv;
            }
        }

        public bool JogStop()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            //lock (m_LockKey)
            {
                bool rv = false;
                object pWait = 0;
                try
                {
                    if (AppConfig.Instance.Simulation.ACS && m_AxisBlock.BlockId > 0) return true; // Simulator가 한개만 만들어 지네...

                    _Channel.Halt((Axis)m_Axis.NodeId);
                    rv = true;
                }
                catch (ACSException Ex)
                {
                    ExceptionLog.WriteLog(method, string.Format("ACS JogStop{0} : {1}", m_Axis.NodeId, Ex.Message));
                }
                catch (COMException Ex)
                {
                    ExLog(Ex);
                }
                return rv;
            }
        }
        public bool Home()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            //lock (m_LockKey)
            {
                if (AppConfig.Instance.Simulation.ACS) return false;

                bool rv = false;
                object pWait = 0;
                try
                {
                    if (AppConfig.Instance.Simulation.ACS && m_AxisBlock.BlockId > 0) return true; // Simulator가 한개만 만들어 지네...

                    if (m_Axis.NodeId == m_Axis.MasterNodeId)
                    {
                        ProgramBuffer BufferNo = (ProgramBuffer)m_Axis.MasterNodeId; // buffer number
                        _Channel.RunBuffer(BufferNo, null);
                    }
                    rv = true;
                }
                catch (ACSException Ex)
                {
                    ExceptionLog.WriteLog(method, string.Format("ACS Home{0} : {1}", m_Axis.NodeId, Ex.Message));
                }
                catch (COMException Ex)
                {
                    ExLog(Ex);
                }
                return rv;
            }
        }

        public bool IsHomeOk()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            bool rv = false;
            object pWait = 0;
            try
            {
                if (AppConfig.Instance.Simulation.ACS && m_AxisBlock.BlockId > 0) return true; // Simulator가 한개만 만들어 지네...
                rv = m_ProgramState.HasFlag(ProgramStates.ACSC_PST_RUN) ? false : true;
            }
            catch (ACSException Ex)
            {
                ExceptionLog.WriteLog(method, string.Format("ACS IsHomeOk{0} : {1}", m_Axis.NodeId, Ex.Message));
            }
            catch (COMException Ex)
            {
                ExLog(Ex);
            }
            return rv;
        }

        public bool SetPosition(double pos)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            bool rv = false;
            object pWait = 0;
            try
            {
                if (AppConfig.Instance.Simulation.ACS && m_AxisBlock.BlockId > 0) return true; // Simulator가 한개만 만들어 지네...

                _Channel.SetFPosition((Axis)m_Axis.NodeId, pos);
                //ACSC_WAITBLOCK wb = _Channel.SetFPositionAsync((Axis)m_Axis.NodeId, pos);
                //double set_pos = (double)_Channel.GetResult(wb, 2000);
                rv = true;
            }
            catch (ACSException Ex)
            {
                ExceptionLog.WriteLog(method, string.Format("ACS SetPosition{0} : {1}", m_Axis.NodeId, Ex.Message));
            }
            catch (COMException Ex)
            {
                ExLog(Ex);
            }
            return rv;
        }

        public bool Move()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            //lock (m_LockKey)
            {
                bool rv = false;
                object pWait = 0;
                try
                {
                    if (AppConfig.Instance.Simulation.ACS && m_AxisBlock.BlockId > 0) return true; // Simulator가 한개만 만들어 지네...

                    double targetvel = m_AxisBlock.GetTargetSpeed(m_Axis.AxisId);
                    double targetpos = m_AxisBlock.GetTargetPosition(m_Axis.AxisId);

                    double targetAcc = m_AxisBlock.GetTargetAcc(m_Axis.AxisId);
                    if (targetAcc < 50.0) targetAcc = 50.0f;
                    double targetDec = targetAcc;

                    if (AppConfig.Instance.Simulation.ACS)
                    {
                        targetAcc = 1000;
                        targetDec = 1000;
                    }

                    double move_distance = Math.Abs(targetpos - m_CurPosition);
                    if (Math.Abs(move_distance) > 0.000001)
                    {
                        _Channel.SetVelocity((Axis)m_Axis.NodeId, targetvel);
                        _Channel.SetAcceleration((Axis)m_Axis.NodeId, targetAcc);
                        _Channel.SetDeceleration((Axis)m_Axis.NodeId, targetDec);
                        _Channel.SetJerk((Axis)m_Axis.NodeId, 10 * targetAcc);
                        _Channel.ToPoint(MotionFlags.ACSC_NONE, (Axis)m_Axis.NodeId, targetpos);
                        //_Channel.ToPointAsync(MotionFlags.ACSC_NONE, (Axis)m_Axis.NodeId, targetpos); // 이걸로 확인해 보자....!
                        rv = true;
                    }
                    else
                    {
                        rv = false;
                    }
                }
                catch (ACSException Ex)
                {
                    ExceptionLog.WriteLog(method, string.Format("ACS Move{0} : {1}", m_Axis.NodeId, Ex.Message));
                }
                catch (COMException Ex)
                {
                    ExLog(Ex);
                }
                return rv;
            }
        }

        public bool RelativeMove()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            //lock (m_LockKey)
            {
                bool rv = false;
                object pWait = 0;
                try
                {
                    if (AppConfig.Instance.Simulation.ACS && m_AxisBlock.BlockId > 0) return true; // Simulator가 한개만 만들어 지네...

                    double targetvel = m_AxisBlock.GetTargetSpeed(m_Axis.AxisId);
                    //double targetacc = m_AxisBlock.GetTargetAcc(m_Axis.AxisId);
                    double targetpos = m_AxisBlock.GetTargetPosition(m_Axis.AxisId) - m_CurPosition;
                    //targetacc = 1000;

                    double targetAcc = targetvel * 50;
                    double targetDec = targetvel * 50;

                    _Channel.SetVelocity((Axis)m_Axis.NodeId, targetvel);
                    _Channel.SetAcceleration((Axis)m_Axis.NodeId, 500);
                    _Channel.SetDeceleration((Axis)m_Axis.NodeId, 500);
                    _Channel.ToPoint(MotionFlags.ACSC_AMF_RELATIVE, (Axis)m_Axis.NodeId, targetpos);

                    rv = true;
                }
                catch (ACSException Ex)
                {
                    ExceptionLog.WriteLog(method, string.Format("ACS RelativeMove{0} : {1}", m_Axis.NodeId, Ex.Message));
                }
                catch (COMException Ex)
                {
                    ExLog(Ex);
                }
                return rv;
            }
        }
        public bool SegmentMove()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            {
                bool rv = false;
                object pWait = 0;
                try
                {
                    if (m_Axis.SegmentCommand.SubAxes.Count != 2) return false;

                    enSegmentType cmdType = m_Axis.SegmentCommand.SegmentType;
                    if (cmdType == enSegmentType.NONE) return false;

                    _Axis x_axis = m_Axis.SegmentCommand.SubAxes[0];
                    _Axis y_axis = m_Axis.SegmentCommand.SubAxes[1];
                    if (x_axis == null || y_axis == null) return false;

                    double x_vel = m_Axis.SegmentCommand.MoveVel.X;
                    double y_vel = m_Axis.SegmentCommand.MoveVel.Y;
                    double x_start_pos = m_Axis.SegmentCommand.StartPos.X;
                    double y_start_pos = m_Axis.SegmentCommand.StartPos.Y;
                    double x_end_pos = m_Axis.SegmentCommand.EndPos.X;
                    double y_end_pos = m_Axis.SegmentCommand.EndPos.Y;
                    double x_center_pos = m_Axis.SegmentCommand.CircleCenterPos.X;
                    double y_center_pos = m_Axis.SegmentCommand.CircleCenterPos.Y;
                    double radian = m_Axis.SegmentCommand.radian;
                    RotationDirection dir = m_Axis.SegmentCommand.MoveDirection == enAxisDir.Positive ? RotationDirection.ACSC_CLOCKWISE : RotationDirection.ACSC_COUNTERCLOCKWISE;

                    // Segment 동작은 Start 위치에 이동된 상태에서 시작한다.
                    // Current Position과 Start Position은 동일해야 한다.
                    Axis[] Axes = new Axis[2];
                    Axes[0] = (Axis)x_axis.NodeId;
                    Axes[1] = (Axis)y_axis.NodeId;
                    double[] start_point = new double[2];
                    start_point[0] = x_start_pos;
                    start_point[1] = y_start_pos;
                    double[] end_point = new double[2];
                    end_point[0] = x_end_pos;
                    end_point[1] = y_end_pos;
                    double[] center_point = new double[2];
                    center_point[0] = x_center_pos;
                    center_point[1] = y_center_pos;

                    //ExtendedSegmentedMotionExt(MotionFlags flags, Axis[] axes, double[] point, double velocity, double endVelocity, double junctionVelocity, double angle, double curveVelocity, double deviation, double radius, double maxLength, double starvationMargin, string segments);
                    _Channel.ExtendedSegmentedMotionExt(MotionFlags.ACSC_AMF_VELOCITY, // Velocity and corner radius flags are set, parameters now required.
                    Axes, //Axes 0,1 active
                    start_point, // Starting point
                    x_vel, // Velocity is set to 1000
                    0.0f, //EndVelocity is the default value
                    0.0f, //JunctionVelocity is the default value
                    0.0f, // Angle is the default value
                    0.0f, // CurveVElocity is the default value
                    0.0f, // Deviation
                    0.0f, // Radius is set
                    0.0f, // Maximum corner length is default
                    0.0f, // Starvation margin is the default value
                    null); //Segments are set to NULL

                    if (cmdType == enSegmentType.ARC)
                    {
                        //ExtendedSegmentArc1(MotionFlags flags, Axis[] axes, double[] center, double[] finalPoint, RotationDirection rotation, double velocity, double endVelocity, double time, string values, string variables, int index, string masks);
                        _Channel.ExtendedSegmentArc1(MotionFlags.ACSC_AMF_VARTIME | MotionFlags.ACSC_AMF_ENDVELOCITY,
                            Axes,
                            center_point,
                            end_point,
                            dir,
                            x_vel,
                            0.0f,
                            0.0f,
                            null,
                            null,
                            0,
                            null);
                    }
                    else if (cmdType == enSegmentType.CIRCLE)
                    {
                        // 360도인 경우 start = end 일거임.
                        //ExtendedSegmentArc2(MotionFlags flags, Axis[] axes, double[] center, double angle, double[] finalPoint, double velocity, double endVelocity, double time, string values, string variables, int index, string masks);
                        _Channel.ExtendedSegmentArc2(MotionFlags.ACSC_AMF_VARTIME | MotionFlags.ACSC_AMF_ENDVELOCITY,
                            Axes,
                            center_point,
                            radian,
                            end_point,
                            x_vel,
                            0.0f,
                            0.0f,
                            null,
                            null,
                            0,
                            null);
                    }
                    else if (cmdType == enSegmentType.LINE)
                    {
                        //SegmentLineExt(MotionFlags flags, Axis[] axes, double[] point, double velocity, double endVelocity, double time, string values, string variables, int index, string masks);
                        _Channel.SegmentLineExt(MotionFlags.ACSC_AMF_VELOCITY,
                            Axes,
                            end_point,
                            x_vel,
                            0.0f,
                            0.0f,
                            null,
                            null,
                            0,
                            null);
                    }
                    _Channel.EndSequenceM(Axes);
                }
                catch (ACSException Ex)
                {
                    ExceptionLog.WriteLog(method, string.Format("ACS RelativeMove{0} : {1}", m_Axis.NodeId, Ex.Message));
                }
                catch (COMException Ex)
                {
                    ExLog(Ex);
                }
                return rv;
            }
        }
        public bool IsMoveDone()
        {
            bool rv = true;
            rv &= m_MotorState.HasFlag(MotorStates.ACSC_MST_INPOS) ? true : false;
            rv &= m_MotorState.HasFlag(MotorStates.ACSC_MST_MOVE) ? false : true;

            return rv;
        }

        public bool IsMoving()
        {
            bool rv = true;
            rv &= m_MotorState.HasFlag(MotorStates.ACSC_MST_MOVE) ? true : false;
            rv &= m_MotorState.HasFlag(MotorStates.ACSC_MST_INPOS) ? false : true;

            return rv;
        }
        #endregion

        #region Thread Methods
        public void UpdateAxis()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            //if (m_Axis.NodeId != 1) return;
            try
            {
                //            if (AppConfig.Instance.Simulation.ACS)
                //            {
                //	m_CurPosition = m_AxisBlock.GetAbsTargetPosition(m_Axis.AxisId);
                //	m_CurSpeed = m_AxisBlock.GetAbsTargetSpeed(m_Axis.AxisId);
                //	if (!m_Axis.GantryType || m_Axis.NodeId != m_Axis.SlaveNodeId)
                //	{
                //		m_MotorState = _Channel.GetMotorState((Axis)m_Axis.NodeId);
                //		m_ProgramState = _Channel.GetProgramState((ProgramBuffer)m_Axis.NodeId);
                //	}
                //	m_AxisSafty = _Channel.GetFault((Axis)m_Axis.NodeId);
                //	if (m_AxisSafty.HasFlag(SafetyControlMasks.ACSC_SAFETY_LL)) m_AxisSafty ^= SafetyControlMasks.ACSC_SAFETY_LL;
                //	if (m_AxisSafty.HasFlag(SafetyControlMasks.ACSC_SAFETY_RL)) m_AxisSafty ^= SafetyControlMasks.ACSC_SAFETY_RL;
                //}

                m_AxisBlock.SetCurPosition(m_Axis.AxisId, Math.Round(m_CurPosition, 5));
                m_AxisBlock.SetCurSpeed(m_Axis.AxisId, Math.Round(m_CurSpeed, 5));
                m_AxisBlock.SetCurTorque(m_Axis.AxisId, m_CurTorque);

                // Get motor state of the Axis
                enAxisInFlag state = enAxisInFlag.None;
                // Servo On
                bool enable = m_MotorState.HasFlag(MotorStates.ACSC_MST_ENABLE);
                if (enable) state |= enAxisInFlag.SvOn;
                // InPos
                bool moving = m_MotorState.HasFlag(MotorStates.ACSC_MST_MOVE);
                bool inpos = moving ? false : true; // m_MotorState.HasFlag(MotorStates.ACSC_MST_INPOS);
                if (moving) state |= enAxisInFlag.Busy; // busy 추가 2020.06.01 hjyou
                if (enable && inpos && !CommandRun && !CommandHome) state |= enAxisInFlag.InPos;
                //if (m_Axis.AxisId == 1)
                //    Debug.WriteLine(string.Format("{0} = AxisState[{1} | {2}]", m_Axis.AxisId, inpos ? "INPOS" : "", moving ? "MOVING" : ""));

                // Limit Sensor
                bool negLimit = m_AxisSafty.HasFlag(SafetyControlMasks.ACSC_SAFETY_LL) && m_Axis.NegLimitUse ? true : false;
                bool posLimit = m_AxisSafty.HasFlag(SafetyControlMasks.ACSC_SAFETY_RL) && m_Axis.PosLimitUse ? true : false;
                if (AppConfig.Instance.Simulation.ACS) { negLimit = false; posLimit = false; }
                if (negLimit) state |= enAxisInFlag.Limit_N;
                if (posLimit) state |= enAxisInFlag.Limit_P;

                // Alarm
                if (m_AxisSafty.HasFlag(SafetyControlMasks.ACSC_SAFETY_PE)) m_AxisSafty ^= SafetyControlMasks.ACSC_SAFETY_PE;
                if (m_AxisSafty > SafetyControlMasks.ACSC_NONE) //|| m_AlarmCode > 0)
                {
                    if (!CommandHome && !m_Axis.CommandSkip) state |= enAxisInFlag.Alarm;
                    if (m_AlarmCode > 0) state |= enAxisInFlag.Alarm;

                    if (AppConfig.Instance.Simulation.ACS)
                        Debug.WriteLine(string.Format("{0} = AxisSafety{1} : AlarmCode{2}", m_Axis.AxisId, m_AxisSafty, m_AlarmCode));
                }

                // Home Sensor ?
                if (HomeEnd) state |= enAxisInFlag.HEnd;
                // Set State
                m_AxisBlock.SetMotionState(m_Axis.AxisId, state);
            }
            catch (ACSException Ex)
            {
                ExceptionLog.WriteLog(method, string.Format("ACS Update Axis{0} : {1}", m_Axis.NodeId, Ex.Message));
            }
            catch (COMException Ex)
            {
                ExLog(Ex);
            }
        }

        public void DoAxis()
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                enAxisOutFlag command = (enAxisOutFlag)m_AxisBlock.GetMotionCommand(m_Axis.AxisId);
                if (command != enAxisOutFlag.CommandNone)
                {
                    if (Convert.ToBoolean(command & enAxisOutFlag.ServoOn)) Enable();
                    else if (Convert.ToBoolean(command & enAxisOutFlag.ServoOff)) Disable();
                    else if (Convert.ToBoolean(command & enAxisOutFlag.AlarmClear)) AlarmClear();
                    else if (Convert.ToBoolean(command & enAxisOutFlag.MotionStop)) EStop();
                }

                SeqHome(command);
                SeqMove(command);
                SeqJog(command);
                SeqSegmentMove(command);
            }
            catch (ACSException Ex)
            {
                ExceptionLog.WriteLog(method, string.Format("ACS Do Axis{0} : {1}", m_Axis.NodeId, Ex.Message));
            }
            catch (COMException Ex)
            {
                ExLog(Ex);
            }
        }

        private void SeqHome(enAxisOutFlag command)
        {
            bool rv = false;

            int seqNo = m_SeqHomeNo;
            switch (seqNo)
            {
                case 0:
                    {
                        if (Convert.ToBoolean(command & enAxisOutFlag.HomeStart))
                        {
                            rv = Home();
                            if (rv)
                            {
                                object pWait = 0;
                                CommandHome = true;
                                seqNo = 10;
                            }
                            else
                            {
                                CommandHome = true;
                                m_TickCounts = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                        }
                    }
                    break;

                case 10:
                    {
                        if (m_ProgramState.HasFlag(ProgramStates.ACSC_PST_RUN))
                        {
                            m_TickCounts = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                    }
                    break;

                case 20:
                    {
                        if (!m_ProgramState.HasFlag(ProgramStates.ACSC_PST_RUN))
                        {
                            CommandHome = false;
                            //SetPosition(0);
                            seqNo = 0;
                        }
                    }
                    break;

                case 30:
                    {
                        if (XFunc.GetTickCount() - m_TickCounts > 500)
                        {
                            CommandHome = false;
                            seqNo = 0;
                        }
                    }
                    break;
            }
            m_SeqHomeNo = seqNo;
        }

        private void SeqMove(enAxisOutFlag command)
        {
            int seqNo = m_SeqMoveNo;
            switch (seqNo)
            {
                case 0:
                    {
                        bool rv = false;
                        if (Convert.ToBoolean(command & enAxisOutFlag.MotionStart))
                        {
                            rv = Move();
                            if (rv)
                            {
                                seqNo = 10;
                            }
                            else
                            {
                                CommandRun = true;
                                m_TickCounts = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                        }
                    }
                    break;

                case 10:
                    {
                        double target = Math.Round(m_AxisBlock.GetTargetPosition(m_Axis.AxisId), 6);
                        bool movingComp = Math.Abs(target - m_CurPosition) < 0.0001 ? true : false;
                        if (IsMoving() || movingComp)
                        {
                            CommandRun = true;
                            m_TickCounts = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                    }
                    break;

                case 20:
                    {
                        double ok_range = 0.000001f;
                        if (m_Axis.AxisId < 16) ok_range = 0.0001f; //Gantry 일 경우는 엄청 잘 들어가야 함. Jitter에 따라 범위를 정하자.......
                        else ok_range = 0.01f; // 10um 이내면 이동 완료 했다고 보자....jitter가 10um이내로 setting 되어야 함.....
                        double target = Math.Round(m_AxisBlock.GetTargetPosition(m_Axis.AxisId), 6);
                        bool movingComp = Math.Abs(target - m_CurPosition) < ok_range ? true : false;

                        if (IsMoveDone() && movingComp)
                        {
                            if (XFunc.GetTickCount() - m_TickCounts > 100)
                            {
                                CommandRun = false;
                                seqNo = 0;
                            }
                        }
                        else
                        {
                            m_TickCounts = XFunc.GetTickCount();
                        }
                    }
                    break;

                case 30:
                    {
                        if (XFunc.GetTickCount() - m_TickCounts > 500)
                        {
                            CommandRun = false;
                            seqNo = 0;
                        }
                    }
                    break;
            }
            m_SeqMoveNo = seqNo;
        }

        private void SeqSegmentMove(enAxisOutFlag command)
        {
            int seqNo = m_SeqSegmentMoveNo;
            switch (seqNo)
            {
                case 0:
                    {
                        bool rv = false;
                        if (Convert.ToBoolean(command & enAxisOutFlag.SequenceMotionStart))
                        {
                            rv = SegmentMove();
                            if (rv)
                            {
                                seqNo = 10;
                            }
                            else
                            {
                                CommandRun = true;
                                m_TickCounts = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                        }
                    }
                    break;

                case 10:
                    {
                        if (IsMoving())
                        {
                            CommandRun = true;
                            m_TickCounts = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                    }
                    break;

                case 20:
                    {
                        if (IsMoveDone())
                        {
                            if (XFunc.GetTickCount() - m_TickCounts > 100)
                            {
                                CommandRun = false;
                                seqNo = 0;
                            }
                        }
                        else
                        {
                            m_TickCounts = XFunc.GetTickCount();
                        }
                    }
                    break;

                case 30:
                    {
                        if (XFunc.GetTickCount() - m_TickCounts > 500)
                        {
                            CommandRun = false;
                            seqNo = 0;
                        }
                    }
                    break;
            }
            m_SeqSegmentMoveNo = seqNo;
        }

        private void SeqJog(enAxisOutFlag command)
        {
            int seqNo = m_SeqJogNo;

            switch (seqNo)
            {
                case 0:
                    {
                        bool rv = false;
                        if (Convert.ToBoolean(command & enAxisOutFlag.JogPlus))
                        {
                            rv = JogPlus();
                        }
                        else if (Convert.ToBoolean(command & enAxisOutFlag.JogMinus))
                        {
                            rv = JogMinus();
                        }

                        if (rv) seqNo = 10;
                        else seqNo = 0;
                    }
                    break;

                case 10:
                    {
                        bool rv = false;
                        rv |= Convert.ToBoolean(command & enAxisOutFlag.JogPlus);
                        rv |= Convert.ToBoolean(command & enAxisOutFlag.JogMinus);

                        if (!rv)
                        {
                            JogStop();
                            seqNo = 0;
                        }
                    }
                    break;
            }
            m_SeqJogNo = seqNo;
        }

        public void SetHomeBit(bool home)
        {
            m_HomeEnd = home;
        }

        public int GetAxisId()
        {
            return m_Axis.NodeId;
        }

        public _Axis GetAxis()
        {
            return m_Axis;
        }
        #endregion

        #region Thread
        public class TaskACSAxis : XSequence
        {
            public TaskACSAxis(ACSAxis230 cmd)
            {
                RegSeq(new SeqACSAxisControl(cmd));
            }
        }

        class SeqACSAxisControl : XSeqFunc
        {
            ACSAxis230 m_Command = null;
            public SeqACSAxisControl(ACSAxis230 cmd)
            {
                this.SeqName = $"SeqACSAxisControl";
                m_Command = cmd;
            }

            public override int Do()
            {
                if (m_Command.m_AxisBlock.Connected)
                {
                    m_Command.UpdateAxis();
                    m_Command.DoAxis();
                }
                return -1;
            }
        }
        #endregion
    }
}
