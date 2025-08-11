using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.Servo
{
    public class TaskServoControl : XSequence
    {
        public static readonly TaskServoControl Instance = new TaskServoControl();
        private static object m_LockKey = new object();

        private TaskServoControl()
        {
            ThreadInfo.Name = "ServoControl";
            TaskHandler.Instance.RegTask(this, 10, System.Threading.ThreadPriority.Highest);
        }
        public bool Initialize()
        {
            // Sequence 등록
            foreach (ServoUnit unit in ServoManager.Instance.ServoUnits)
            {
                RegSeq(new SeqRepeat(unit));
                foreach (_Axis axis in unit.Axes)
                {
                    RegSeq(new SeqServoAxis(unit, axis));
                }
            }
            return true;
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
                // Thread Start
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

        #region sequence
        public class SeqServoAxis : XSeqFunc
        {
            XTimer m_Timer1 = new XTimer("Timer1");
            ServoUnit m_Robot = null;
            _Axis m_Axis = null;

            enAxisInFlag m_TargetStatus = enAxisInFlag.None;
            enAxisInFlag m_ExclusionStatus = enAxisInFlag.None;
            private DateTime _LastTime = DateTime.Now;

            uint m_CommandWaitTime = 600 * 1000;
            private bool logPos = false;

            public SeqServoAxis(ServoUnit servo, _Axis axis)
            {
                this.SeqName = $"SeqServoAxis_{servo.ServoName}_{axis.AxisName}";

                m_Robot = servo;
                m_Axis = axis;
            }
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                //lock (ServoManager.LockKey)
                {
                    enAxisOutFlag cmdType = m_Axis.CmdPxy.Cmdtype;
                    // Motion Stop & Servo Off Check
                    if (m_Robot.IsTriggerExist(m_Axis) == true)
                    {
                        if (Convert.ToBoolean(cmdType & enAxisOutFlag.MotionStop) || Convert.ToBoolean(cmdType & enAxisOutFlag.ServoOff))
                        {
                            this.SeqNo = 0;
                        }
                        else if (((m_Axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InPos) != enAxisInFlag.InPos) // 구동중
                        {
                            if (Convert.ToBoolean(cmdType & enAxisOutFlag.MotionStart) ||
                                Convert.ToBoolean(cmdType & enAxisOutFlag.SequenceMotionStart) ||
                                Convert.ToBoolean(cmdType & enAxisOutFlag.RelativeMoveStart))
                            {
                                m_Axis.CmdPxy.Cmdtype |= enAxisOutFlag.MotionCancel; // Cancel 추가
                                this.SeqNo = 0; 
                            } // 이동중 명령 가능 
                        }
                    }

                    int seqNo = this.SeqNo;
                    switch (seqNo)
                    {
                        case 0:
                            // Trigger Check
                            if (m_Robot.IsTriggerExist(m_Axis) == true)
                            {
                                ServoLog.WriteLog(string.Format("{0}.{1} {2} IsTriggerExist", m_Robot.ServoName, m_Axis.AxisName, cmdType));

                                Thread.Sleep(1);
                                m_Robot.ResetTrigger(m_Axis);
                                m_TargetStatus = enAxisInFlag.None;
                                m_ExclusionStatus = enAxisInFlag.None;
                                m_Axis.CmdResult = enAxisResult.None;
                                logPos = false;

                                m_CommandWaitTime = 600 * 1000;
                                if (Convert.ToBoolean(cmdType & enAxisOutFlag.MotionStart))
                                {
                                    double in_spec = m_Axis.InRangeValue;// 0.01f; // 1 / Math.Pow(10, (m_Axis as MpAxis).DecimalPoint); // 최소 제어 단위
                                    // 목표위치와 현 위치가 동일하면 OK
                                    if (Math.Abs(m_Axis.TargetPos - (m_Axis as IAxisCommand).GetAxisCurPos()) < in_spec) // 0.0001f
                                    {
                                        m_Robot.ResetCommand(m_Axis);
                                        m_Axis.CmdResult = enAxisResult.Success;
                                    }
                                    else
                                    {
                                        if (!m_Axis.CommandSkip)
                                        {
                                            // Command Write ///////////////////////////////////////////////////////////
                                            (m_Axis as IAxisCommand).SetPosAsync(Convert.ToSingle(m_Axis.TargetPos));
                                            VelSet vel_set = new VelSet
                                            {
                                                Vel = Convert.ToSingle(m_Axis.TargetSpeed),
                                                Acc = Convert.ToSingle(m_Axis.TargetAcc),
                                                Dec = Convert.ToSingle(m_Axis.TargetDec),
                                                Jerk = Convert.ToSingle(m_Axis.TargetJerk),
                                            };
                                            (m_Axis as IAxisCommand).SetSpeedAsync(vel_set);

                                            m_TargetStatus |= enAxisInFlag.InPos;
                                            m_TargetStatus |= enAxisInFlag.SvOn;
                                        }

                                        logPos = true;
                                    }
                                }
                                else if (Convert.ToBoolean(cmdType & enAxisOutFlag.RelativeMoveStart))
                                {
                                    if (!m_Axis.CommandSkip)
                                    {
                                        // Command Write ///////////////////////////////////////////////////////////
                                        m_Axis.TargetPos = m_Axis.CurPos + m_Axis.TargetDistance;
                                        //(m_Axis as IAxisCommand).SetTargetDistance(m_Axis.TargetDistance);
                                        (m_Axis as IAxisCommand).SetPosAsync(Convert.ToSingle(m_Axis.TargetPos));
                                        (m_Axis as IAxisCommand).SetTargetDistance(Convert.ToSingle(m_Axis.TargetDistance));
                                        VelSet vel_set = new VelSet
                                        {
                                            Vel = Convert.ToSingle(m_Axis.TargetSpeed),
                                            Acc = Convert.ToSingle(m_Axis.TargetAcc),
                                            Dec = Convert.ToSingle(m_Axis.TargetDec),
                                            Jerk = Convert.ToSingle(m_Axis.TargetJerk),
                                        };
                                        (m_Axis as IAxisCommand).SetSpeedAsync(vel_set);

                                        m_TargetStatus |= enAxisInFlag.InPos;
                                        m_TargetStatus |= enAxisInFlag.SvOn;
                                    }

                                    logPos = true;
                                }
                                else if (Convert.ToBoolean(cmdType & enAxisOutFlag.SequenceMotionStart))
                                {
                                    if (!m_Axis.CommandSkip)
                                    {
                                        // Command Write ///////////////////////////////////////////////////////////
                                        List<double> distances = m_Axis.SequenceCommand.MotionProfiles.Select(x => x.Distance).ToList();
                                        m_Axis.TargetPos = distances.Count > 0 ? m_Axis.CurPos + distances.Sum() : m_Axis.CurPos;
                                        m_TargetStatus |= enAxisInFlag.SvOn;
                                        m_TargetStatus |= enAxisInFlag.InPos;
                                    }
                                    logPos = true;
                                }
                                else if (Convert.ToBoolean(cmdType & enAxisOutFlag.HomeStart))
                                {
                                    // Command Write ///////////////////////////////////////////////////////////
                                    if (!m_Axis.CommandSkip)
                                    {
                                        m_TargetStatus |= enAxisInFlag.HEnd;
                                        m_TargetStatus |= enAxisInFlag.SvOn;
                                        m_TargetStatus |= enAxisInFlag.InPos;
                                        EventHandlerManager.Instance.InvokeServoAxisOriginStart(m_Axis.AxisId);
                                    }
                                    if (m_Axis.HomeMethod == enHomeMethod.ABS_ZERO || m_Axis.HomeMethod == enHomeMethod.PLC) m_CommandWaitTime = 10 * 1000; //ABS_Zero Home 진행 중일때는 10초만 기다리자~~
                                    else m_CommandWaitTime = 180 * 1000;
                                        logPos = true;
                                }
                                else if (Convert.ToBoolean(cmdType & enAxisOutFlag.MotionStop))
                                {
                                    // Command Write ///////////////////////////////////////////////////////////
                                    //(m_Axis as IAxisCommand).SetCommandAsync(m_Axis.CmdPxy.Cmdtype);
                                    (m_Axis as IAxisCommand).SetHolding(false);
                                    m_TargetStatus |= enAxisInFlag.InPos;
                                    m_CommandWaitTime = 10 * 1000;
                                }
                                else if (Convert.ToBoolean(cmdType & enAxisOutFlag.ServoOn))
                                {
                                    // Command Write ///////////////////////////////////////////////////////////
                                    //(m_Axis as IAxisCommand).SetCommandAsync(m_Axis.CmdPxy.Cmdtype);
                                    (m_Axis as IAxisCommand).SetHolding(false);
                                    m_TargetStatus |= enAxisInFlag.SvOn;
                                    m_CommandWaitTime = 10 * 1000;
                                }
                                else if (Convert.ToBoolean(cmdType & enAxisOutFlag.ServoOff))
                                {
                                    // Command Write ///////////////////////////////////////////////////////////
                                    //(m_Axis as IAxisCommand).SetCommandAsync(m_Axis.CmdPxy.Cmdtype);
                                    (m_Axis as IAxisCommand).SetHolding(false);
                                    m_ExclusionStatus |= enAxisInFlag.SvOn;
                                }
                                else if (Convert.ToBoolean(cmdType & enAxisOutFlag.AlarmClear))
                                {
                                    // Command Write ///////////////////////////////////////////////////////////
                                    //(m_Axis as IAxisCommand).SetCommandAsync(m_Axis.CmdPxy.Cmdtype);
                                    m_ExclusionStatus |= enAxisInFlag.Alarm;
                                }
                                else if (Convert.ToBoolean(cmdType & enAxisOutFlag.ZeroSet))
                                {
                                    // Command Write ///////////////////////////////////////////////////////////
                                    //(m_Axis as IAxisCommand).SetCommandAsync(m_Axis.CmdPxy.Cmdtype);
                                    //double curPos = (m_Axis as IAxisCommand).GetAxisCurPos();
                                    //(m_Axis as IAxisCommand).SetHomeOffsetAsync(Convert.ToSingle(curPos));
                                    m_TargetStatus |= enAxisInFlag.InPos;
                                    m_CommandWaitTime = 10 * 1000;
                                }
                                else if (Convert.ToBoolean(cmdType & enAxisOutFlag.Reset))
                                {
                                    m_Axis.CmdResult = enAxisResult.CmdError;
                                    m_Robot.ResetCommand(m_Axis);
                                    ServoLog.WriteLog(string.Format("[SeqServoAxis] CmdError Occur! cmdType = {0}", cmdType));
                                }
                                else
                                {
                                    m_Axis.CmdResult = enAxisResult.CmdError;
                                    m_Robot.ResetCommand(m_Axis);
                                    ServoLog.WriteLog(string.Format("[SeqServoAxis] CmdError Occur! cmdType = {0}", cmdType));
                                }

                                if (m_Axis.CmdResult == enAxisResult.None)
                                {
                                    // Command 수행 Time Over Check 하자...Controller에 쓸수 있는 시간을 기다리자...
                                    this.StartTicks = XFunc.GetTickCount();
                                    _LastTime = DateTime.Now;
                                    seqNo = 10;
                                }
                            }
                            break;

                        case 10:
                            if ((DateTime.Now - _LastTime).TotalMilliseconds > 10)
                            {
                                bool cmd_skip = false;
                                cmd_skip |= Convert.ToBoolean(cmdType & enAxisOutFlag.RelativeMoveStart);
                                cmd_skip |= Convert.ToBoolean(cmdType & enAxisOutFlag.SequenceMotionStart);
                                cmd_skip |= Convert.ToBoolean(cmdType & enAxisOutFlag.MotionStart);
                                cmd_skip |= Convert.ToBoolean(cmdType & enAxisOutFlag.HomeStart);
                                cmd_skip &= m_Axis.CommandSkip ? true : false;

                                if (cmd_skip == false)
                                {
                                    (m_Axis as IAxisCommand).SetCommandAsync(m_Axis.CmdPxy.Cmdtype);
                                    ServoLog.WriteLog(string.Format("{0}.{1} {2} SetCommandAsync", m_Robot.ServoName, m_Axis.AxisName, cmdType));
                                }
                                _LastTime = DateTime.Now;
                                bool busy_no_check = Convert.ToBoolean(cmdType & enAxisOutFlag.MotionCancel);
                                busy_no_check |= Convert.ToBoolean(cmdType & enAxisOutFlag.HomeStart) && m_Axis.HomeMethod == enHomeMethod.ABS_ZERO;
                                if (busy_no_check) seqNo = 30;
                                else seqNo = 20;
                            }
                            break;

                        case 20:
                            if ((DateTime.Now - _LastTime).TotalMilliseconds > 10)
                            {
                                bool cmd_move = false;
                                cmd_move |= Convert.ToBoolean(cmdType & enAxisOutFlag.RelativeMoveStart);
                                cmd_move |= Convert.ToBoolean(cmdType & enAxisOutFlag.SequenceMotionStart);
                                cmd_move |= Convert.ToBoolean(cmdType & enAxisOutFlag.MotionStart);
                                cmd_move |= Convert.ToBoolean(cmdType & enAxisOutFlag.HomeStart);

                                // Axis Move Check (InPos Off Check. case of Move and home)
                                if (cmd_move)
                                {
                                    if (m_Axis.CommandSkip)
                                    {
                                        //Gantry Y2 같은 경우 Monitoring만 한다....
                                        m_Axis.CmdResult = enAxisResult.Success;
                                        m_Robot.ResetCommand(m_Axis);
                                        ServoLog.WriteLog(string.Format("{0}.{1} {2} ResetCommand [Skip]", m_Robot.ServoName, m_Axis.AxisName, cmdType));
                                        seqNo = 0;
                                    }
                                    else
                                    {
                                        // MP Busy Signal 관련 프로그램 Update 후 SeqServoAxis 내용 적용되어야 함.
                                        bool motion_start = false;
                                        motion_start |= ((m_Axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InPos) != enAxisInFlag.InPos && (DateTime.Now - _LastTime).TotalMilliseconds > 300 ? true : false;
                                        motion_start |= ((m_Axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Busy) == enAxisInFlag.Busy ? true : false;

                                        if (motion_start)
                                        {
                                            m_Robot.ResetCommand(m_Axis);
                                            ServoLog.WriteLog(string.Format("{0}.{1} {2} ResetCommand [Busy]", m_Robot.ServoName, m_Axis.AxisName, cmdType));
                                            _LastTime = DateTime.Now;
                                            seqNo = 30;
                                        }
                                        else if (this.GetElapsedTicks() > 50000)
                                        {
                                            m_Axis.LastErrorMsg = "Axis response timeout";
                                            m_Axis.CmdResult = enAxisResult.Timeover;
                                            ServoLog.WriteLog(string.Format("{0}.{1} {2} ResetCommand [Timeout]", m_Robot.ServoName, m_Axis.AxisName, cmdType));
                                            m_Robot.ResetCommand(m_Axis);
                                            seqNo = 0;
                                        }
                                    }
                                }
                                else if (Convert.ToBoolean(cmdType & enAxisOutFlag.MotionStop))
                                {
                                    if (((m_Axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) != enAxisInFlag.SvOn)
                                    {
                                        m_Axis.LastErrorMsg = "Axis Already Servo Off";
                                        m_Axis.CmdResult = enAxisResult.Success;

                                        m_Robot.ResetCommand(m_Axis);
                                        ServoLog.WriteLog(string.Format("{0}.{1} {2} ResetCommand [ServoOff]", m_Robot.ServoName, m_Axis.AxisName, cmdType));
                                        seqNo = 0;
                                    }
                                    else
                                    {
                                        _LastTime = DateTime.Now;
                                        seqNo = 30;
                                    }
                                }
                                else if ((DateTime.Now - _LastTime).TotalMilliseconds > 200)
                                {
                                    _LastTime = DateTime.Now;
                                    seqNo = 30;
                                }
                            }
                            break;

                        case 30:
                            if ((DateTime.Now - _LastTime).TotalMilliseconds > 50) //Cancel 될수 있는 시간을 기다려 주자~~
                            {
                                if ((m_Axis as MpAxis).CurrentOverrideRate == 0.0f) StartTicks = XFunc.GetTickCount();

                                // Command Complete Check
                                if (m_TargetStatus != enAxisInFlag.None)
                                {
                                    //System.Diagnostics.Trace.WriteLine(DateTime.Now + string.Format("{0}", (m_Axis as IAxisCommand).GetAxisCurStatus()));
                                    enAxisInFlag state = m_Axis.AxisStatus; // (m_Axis as IAxisCommand).GetAxisCurStatus();
                                    if ((state & enAxisInFlag.Alarm) == enAxisInFlag.Alarm)
                                    {
                                        m_Axis.CmdResult = enAxisResult.Alarm;
                                        m_Robot.ResetCommand(m_Axis);
                                        ServoLog.WriteLog(string.Format("{0}.{1} {2} ResetCommand [Alarm]", m_Robot.ServoName, m_Axis.AxisName, cmdType));
                                        seqNo = 0;
                                    }
                                    //else if ((state & enAxisInFlag.OverrideAbnormalStop) == enAxisInFlag.OverrideAbnormalStop)
                                    //{
                                    //    m_Axis.CmdResult = enAxisResult.OverrideAbnormalStop;
                                    //    m_Robot.ResetCommand(m_Axis);
                                    //    seqNo = 0;
                                    //}
                                    else if ((state & enAxisInFlag.InRange_Error) == enAxisInFlag.InRange_Error)
                                    {
                                        m_Axis.CmdResult = enAxisResult.InrangeError;
                                        m_Robot.ResetCommand(m_Axis);
                                        ServoLog.WriteLog(string.Format("{0}.{1} {2} ResetCommand [InrangeError]", m_Robot.ServoName, m_Axis.AxisName, cmdType));
                                        seqNo = 0;
                                    }
                                    else if ((state & m_TargetStatus) == m_TargetStatus)
                                    {
                                        m_Robot.ResetCommand(m_Axis);
                                        ServoLog.WriteLog(string.Format("{0}.{1} {2} ResetCommand [Done]", m_Robot.ServoName, m_Axis.AxisName, cmdType));
                                        seqNo = 40;
                                    }
                                    else if (this.GetElapsedTicks() > m_CommandWaitTime)
                                    {
                                        m_Axis.CmdResult = enAxisResult.Timeover;
                                        m_Robot.ResetCommand(m_Axis);
                                        ServoLog.WriteLog(string.Format("{0}.{1} {2} ResetCommand [Timeover]", m_Robot.ServoName, m_Axis.AxisName, cmdType));
                                        seqNo = 0;
                                    }
                                }
                                else if (m_ExclusionStatus != enAxisInFlag.None)
                                {
                                    enAxisInFlag state = m_Axis.AxisStatus; // (m_Axis as IAxisCommand).GetAxisCurStatus();
                                    if ((state & m_ExclusionStatus) != m_ExclusionStatus)
                                    {
                                        m_Robot.ResetCommand(m_Axis);
                                        ServoLog.WriteLog(string.Format("{0}.{1} {2} ResetCommand [Done]", m_Robot.ServoName, m_Axis.AxisName, cmdType));
                                        seqNo = 40;
                                    }
                                    else if (this.GetElapsedTicks() > 5000) // Servo Off & Alarm clear는 5초이내에 확인 되어야 함.
                                    {
                                        m_Axis.CmdResult = enAxisResult.Timeover;
                                        ServoLog.WriteLog(string.Format("{0}.{1} {2} ResetCommand [Timeover]", m_Robot.ServoName, m_Axis.AxisName, cmdType));
                                        m_Robot.ResetCommand(m_Axis);
                                        seqNo = 0;
                                    }
                                }
                                else
                                {
                                    if (m_Axis.CommandSkip)
                                    {
                                        //Gantry Y2 같은 경우 Monitoring만 한다....
                                        m_Axis.CmdResult = enAxisResult.Success;
                                        m_Robot.ResetCommand(m_Axis);
                                        ServoLog.WriteLog(string.Format("{0}.{1} {2} ResetCommand [Skip]", m_Robot.ServoName, m_Axis.AxisName, cmdType));
                                        seqNo = 0;
                                    }
                                }
                            }
                            break;

                        case 40:
                            {
                                enAxisInFlag state = m_Axis.AxisStatus; // (m_Axis as IAxisCommand).GetAxisCurStatus();
                                if ((state & enAxisInFlag.Cmd_Confirm) != enAxisInFlag.Cmd_Confirm)
                                {
                                    m_Axis.CmdResult = enAxisResult.Success;
                                    ServoLog.WriteLog(string.Format("{0}.{1} {2} Confirm", m_Robot.ServoName, m_Axis.AxisName, cmdType));
                                    if (logPos)
                                    {
                                        logPos = false;
                                        _LastTime = DateTime.Now;
                                        seqNo = 0;
                                    }
                                    else
                                    {
                                        seqNo = 0;
                                    }
                                }
                            }
                            break;
                    }
                    this.SeqNo = seqNo;
                    return -1;
                }
            }
        }

        public class SeqRepeat : XSeqFunc
        {
            ServoUnit m_ServoUnit = null;
            List<_Axis> m_TargetAxes = new List<_Axis>();
            XTimer m_Timer1 = new XTimer("Timer1");
            bool TargetPos = true;
            int m_RepeatCount = 0;

            public SeqRepeat(ServoUnit unit)
            {
                this.SeqName = $"SeqRepeat{unit.ServoName}";
                m_ServoUnit = unit;
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
                        if (m_ServoUnit.Repeat)
                        {
                            if (m_ServoUnit.GetReady())
                            {
                                m_TargetAxes.Clear();
                                // TargetAxis collect
                                foreach (_Axis axis in m_ServoUnit.Axes)
                                {
                                    if (axis.Repeat) m_TargetAxes.Add(axis);
                                }

                                // 현 위치값 buffer에 저장
                                foreach (_Axis axis in m_TargetAxes)
                                {
                                    axis.BufferPos = (axis as IAxisCommand).GetAxisCurPos();
                                }
                                m_RepeatCount = 0;
                                m_Timer1.Start(5000);
                                seqNo = 10;
                            }
                            else
                            {
                                m_ServoUnit.Repeat = false;
                                foreach (_Axis axis in m_ServoUnit.Axes)
                                {
                                    axis.Repeat = false;
                                }
                            }
                        }
                        break;

                    case 10:
                        if (m_ServoUnit.GetReady())
                        {
                            if (TargetPos)
                            {
                                foreach (_Axis axis in m_TargetAxes)
                                {
                                    (axis as IAxisCommand).SetPosAsync(Convert.ToSingle(axis.TargetPos));
                                    VelSet set = new VelSet
                                    {
                                        Vel = axis.TargetSpeed,
                                        Acc = axis.TargetAcc,
                                        Dec = axis.TargetDec,
                                        Jerk = axis.TargetJerk,
                                    };
                                    (axis as IAxisCommand).SetSpeedAsync(set);
                                    (axis as IAxisCommand).SetCommandAsync(enAxisOutFlag.MotionStart);
                                }
                                TargetPos = false;
                            }
                            else
                            {
                                foreach (_Axis axis in m_TargetAxes)
                                {
                                    (axis as IAxisCommand).SetPosAsync(Convert.ToSingle(axis.BufferPos));
                                    VelSet set = new VelSet
                                    {
                                        Vel = axis.TargetSpeed,
                                        Acc = axis.TargetAcc,
                                        Dec = axis.TargetDec,
                                        Jerk = axis.TargetJerk,
                                    };
                                    (axis as IAxisCommand).SetSpeedAsync(set);
                                    (axis as IAxisCommand).SetCommandAsync(enAxisOutFlag.MotionStart);
                                }
                                TargetPos = true;
                            }
                            m_Timer1.Start(5000);
                            string logMsg = "";
                            foreach (_Axis axis in m_TargetAxes)
                            {
                                logMsg += string.Format("[{0}]Cur={1} ", axis.AxisName, (axis as IAxisCommand).GetAxisCurPos());
                            }
                            ServoLog.WriteLog(string.Format("Servo Repeat Start : {0} - {1}, Repeat Count:[{2}]", m_ServoUnit.ServoName, logMsg, m_RepeatCount));

                            seqNo = 20;
                        }
                        else
                        {
                            m_ServoUnit.Repeat = false;
                            foreach (_Axis axis in m_ServoUnit.Axes)
                            {
                                (axis as IAxisCommand).SetCommandAsync(enAxisOutFlag.CommandNone);
                                axis.Repeat = false;
                            }
                            seqNo = 0;
                        }
                        break;

                    case 20:
                        {
                            string logMsg = "";
                            bool allStarted = true;
                            foreach (_Axis axis in m_TargetAxes)
                            {
                                bool start = false;
                                start |= (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InPos) != enAxisInFlag.InPos) ? true : false;
                                start |= (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Busy) == enAxisInFlag.Busy) ? true : false;
                                allStarted &= start;
                            }

                            if (allStarted)
                            {
                                foreach (_Axis axis in m_TargetAxes)
                                    (axis as IAxisCommand).SetCommandAsync(enAxisOutFlag.CommandNone);
                                seqNo = 30;
                            }
                            else if (m_Timer1.Over)
                            {
                                m_ServoUnit.Repeat = false;
                                foreach (_Axis axis in m_ServoUnit.Axes)
                                {
                                    logMsg += string.Format("[{0}]cur={1},Touqe={2} ", axis.AxisName, (axis as IAxisCommand).GetAxisCurPos(), (axis as IAxisCommand).GetAxisCurTorque());
                                    (axis as IAxisCommand).SetCommandAsync(enAxisOutFlag.CommandNone);
                                    axis.Repeat = false;
                                    ServoLog.WriteLog(string.Format("Servo Repeat Time Out : {0} - {1}", axis.AxisName, axis.LastErrorMsg));
                                }
                                ServoLog.WriteLog(string.Format("Servo Repeat Time Out : {0} - {1}", m_ServoUnit.ServoName, logMsg));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 30:
                        {
                            bool complete = true;
                            bool alarm = false;
                            string logMsg = "";
                            foreach (_Axis axis in m_TargetAxes)
                            {
                                complete &= (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InPos) == enAxisInFlag.InPos) ? true : false;
								if(!AppConfig.Instance.Simulation.MY_DEBUG)
                                    complete &= (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Busy) != enAxisInFlag.Busy) ? true : false;
                                alarm |= (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm) ? true : false;
                                //alarm |= (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.OverrideAbnormalStop) == enAxisInFlag.OverrideAbnormalStop) ? true : false;
                                alarm |= (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InRange_Error) == enAxisInFlag.InRange_Error) ? true : false;
                            }

                            if (complete)
                            {
                                m_Timer1.Start(5000); //time delay                             
                                foreach (_Axis axis in m_TargetAxes)
                                {
                                    logMsg += string.Format("[{0}]Target={1},vel={2} ", axis.AxisName, axis.TargetPos, axis.TargetSpeed);
                                }
                                ServoLog.WriteLog(string.Format("Servo Repeat Complete : {0} - {1}", m_ServoUnit.ServoName, logMsg));
                                seqNo = 40;
                            }
                            else if (alarm)
                            {
                                m_ServoUnit.Repeat = false;
                                foreach (_Axis axis in m_ServoUnit.Axes)
                                {
                                    logMsg += string.Format("[{0}]cur={1},Touqe={2} ", axis.AxisName, (axis as IAxisCommand).GetAxisCurPos(), (axis as IAxisCommand).GetAxisCurTorque());
                                    axis.Repeat = false;
                                    ServoLog.WriteLog(string.Format("Servo Repeat NG : {0} - {1}", axis.AxisName, axis.LastErrorMsg));
                                }
                                ServoLog.WriteLog(string.Format("Servo Repeat NG : {0} - {1}", m_ServoUnit.ServoName, logMsg));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 40:
                        if (!m_ServoUnit.Repeat)
                        {
                            seqNo = 0;
                        }
                        else if (m_Timer1.Over)
                        {
                            m_RepeatCount++;
                            seqNo = 10;
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
