using Sineva.VHL.Data;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Device;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using Sineva.VHL.Library.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library.MXP;
using System.Net;

namespace Sineva.VHL.Task
{
    public class TaskInterlockHighSpeed : XSequence
    {
        public static readonly TaskInterlockHighSpeed Instance = new TaskInterlockHighSpeed();

        public TaskInterlockHighSpeed()
        {
            ThreadInfo.Name = string.Format("TaskInterlockHighSpeed");

            this.RegSeq(new SeqControlOverride());
            this.RegSeq(new SeqFrontDetectInterlock());
            this.RegSeq(new SeqVelocityLimitCheck());
            this.RegSeq(new SeqAutoServoOffCheck());
            this.RegSeq(new SeqSteerStateCheck());
            this.RegSeq(new SeqSaftyCheck());
            this.RegSeq(new SeqTorqueLimitApply());            
        }
    }
    public class SeqControlOverride : XSeqFunc
    {
        private const string FuncName = "[SeqControlOverride]";

        #region Fields
        private ProcessDataHandler m_ProcessDataHandler = null;
        private DevSOS m_DevSos = null;
        private _DevAxis m_MasterAxis = null;

        private double m_StopLimit = 300.0f;
        private bool m_IncreaseOverride = false;
        private bool m_DecreaseOverride = false;

        private bool m_FirstTimeUpdate = true;
        private bool m_OverrideResetting = false;
        private bool m_LogWrite = false;

        private uint m_ScanTimeCheckTicks = 0;
        private System.Diagnostics.Stopwatch m_StopWatch = new System.Diagnostics.Stopwatch();
        #endregion

        #region Constructor
        public SeqControlOverride()
        {
            this.SeqName = $"SeqControlOverride";

            m_ProcessDataHandler = ProcessDataHandler.Instance;
            m_DevSos = DevicesManager.Instance.DevSOSUpper;
            m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();

            EventHandlerManager.Instance.ServoAxisOriginStart += Instance_ServoAxisOriginStart;
            EventHandlerManager.Instance.OverrideResetting += Instance_OverrideResetting;
        }

        private void Instance_OverrideResetting()
        {
            try
            {
                // Holding Release가 되면 새로 Override값을 설정하자 !
                m_OverrideResetting = true;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void Instance_ServoAxisOriginStart(int a)
        {
            try
            {
                if (a == m_MasterAxis.GetAxis().AxisId)
                {
                    double current_position = m_MasterAxis.GetCurPosition();
                    m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionPosition = /*current_position + */m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionDistance;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion

        #region Methods
        public override void SeqAbort()
        {
            m_IncreaseOverride = false;
            m_DecreaseOverride = false;
            if (AlarmId > 0)
            {
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            this.InitSeq();
        }
        public override int Do()
        {
            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                m_StopWatch.Reset();
                m_StopWatch.Start();
            }

            double current_position = m_MasterAxis.GetCurPosition();
            enFrontDetectState currentState = m_DevSos.GetFrontDetectState();
            enFrontDetectState beforeState = m_ProcessDataHandler.CurVehicleStatus.ObsStatus.ObsUpperSensorState;
            AlwaysCheck(currentState, beforeState, current_position);

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        m_StopLimit = SetupManager.Instance.SetupWheel.OverrideLimitDistance;

                        m_DecreaseOverride = false;
                        m_IncreaseOverride= false;

                        // Current Link의 조건 셋팅 및 update
                        int area_changed = m_DevSos.SetOBS((uint)m_ProcessDataHandler.CurVehicleStatus.ObsStatus.ObsUpperAreaValue);
                        if (m_ProcessDataHandler.CurVehicleStatus.ObsStatus.OverrideDontControl)
                        {
                            m_ProcessDataHandler.CurVehicleStatus.ObsStatus.OverrideRatio = 1.0f;
                            SequenceLog.WriteLog(FuncName, $"Override Don't Control Area, Path={m_ProcessDataHandler.CurVehicleStatus.CurrentPath.ToString()}, (before:{beforeState}, current:{currentState})");
                            seqNo = 20;
                        }
                        else if (currentState != beforeState || area_changed == 1 || m_FirstTimeUpdate || m_OverrideResetting)
                        {
                            m_FirstTimeUpdate = false; // Program을 시작하면 Override가 0인 상태를 유지한다.
                            // 앞차와의 기준 거리를 Update 하자...
                            // 만일 Area가 변경된 경우 CollisionDistance가 Area 범위 내에 있으면 유지 시키자~~
                            bool update_position = true;
                            if (area_changed == 1)
                            {
                                double min = m_DevSos.GetSensorLevelMinDistance();
                                double max = m_DevSos.GetSensorLevelMaxDistance();
                                double check_distance = m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionDistance;
                                if (check_distance > min && check_distance < max) update_position = false;
                            }
                            if (update_position)
                            {
                                double collision_distance = m_DevSos.GetTableDistanceOfDetectState(beforeState);
                                m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionPosition = current_position + collision_distance;
                            }
                            if (currentState == enFrontDetectState.enNone || m_OverrideResetting) m_IncreaseOverride = true;
                            else if (currentState < m_ProcessDataHandler.CurVehicleStatus.ObsStatus.ObsUpperSensorState) m_IncreaseOverride = true;
                            else if (currentState > m_ProcessDataHandler.CurVehicleStatus.ObsStatus.ObsUpperSensorState) m_DecreaseOverride = true;

                            SequenceLog.WriteLog(FuncName, $"Forward Sensor Changed (before:{beforeState}, current:{currentState})");
                            m_OverrideResetting = false;
                            m_ProcessDataHandler.CurVehicleStatus.ObsStatus.ObsUpperSensorState = currentState;

                            double noneDistance = m_DevSos.GetTableAreaNoneDistance();
                            m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionMaxDistance = noneDistance;

                            double minDistance = m_DevSos.GetTableAreaMinDistance();
                            m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionMinDistance = minDistance;
                            seqNo = 10;
                        }
                        else if (currentState == enFrontDetectState.enNone)
                        {
                            double noneDistance = m_DevSos.GetTableAreaNoneDistance();
                            if (noneDistance < 99999.0f)
                            {
                                m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionMaxDistance = noneDistance;

                                if (Math.Abs(m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionDistance - noneDistance) > 1.0f/* || Math.Abs(m_ProcessDataHandler.CurVehicleStatus.ObsStatus.OverrideRatio - 1.0f) > 0.001f*/)
                                {
                                    m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionDistance = noneDistance;
                                    SequenceLog.WriteLog(FuncName, $"Forward Sensor NONE Changed (before:{beforeState}, current:{currentState}, CollisionDistance:{noneDistance})");
                                    m_IncreaseOverride = true;
                                    seqNo = 10;
                                }
                            }
                        }
                    }
                    break;

                case 10:
                    {
                        // Sensor 상태에 따라 Override 값 계산
                        if (m_DecreaseOverride)
                        {
                            m_DecreaseOverride = false;

                            // 도착 위치와 충돌 위치간 비교
                            // 도착 위치 < 충돌 위치 - StopLimit => Override 적용할 필요 없다.
                            double collision_distance = m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionDistance;
                            double remain_distance = m_ProcessDataHandler.CurTransferCommand.RemainMotorDistance;
                            bool apply = true;
                            if (remain_distance > 0)
                                apply &= remain_distance > collision_distance - SetupManager.Instance.SetupWheel.OverrideStopDistance; //OverrideStopDistance=1300 m_StopLimit=300;
                            apply |= remain_distance < m_StopLimit; // 이때는 무조건 적용해야 한다.
							apply |= currentState >= enFrontDetectState.enDeccelation5;

                            if (apply)
                            {
                                m_ProcessDataHandler.CurVehicleStatus.ObsStatus.OverrideRatio = 0.0f;
                                string msg = $"Set Decrease Override = 0.0f, current OBS:{m_ProcessDataHandler.CurVehicleStatus.ObsStatus.ObsUpperSensorState}, " +
                                    $"collision_distance={collision_distance}, remain_distance={remain_distance}, m_StopLimit={m_StopLimit}";
                                SequenceLog.WriteLog(FuncName, msg);
                                seqNo = 0;
                            }
                            else
                            {
                                // 이때는 BCR 정지할거라 생각...
                                string msg = $"Set Decrease Override Skip, current OBS:{m_ProcessDataHandler.CurVehicleStatus.ObsStatus.ObsUpperSensorState}, " +
                                    $"collision_distance={collision_distance}, remain_distance={remain_distance}, m_StopLimit={m_StopLimit}";
                                SequenceLog.WriteLog(FuncName, msg);
                                seqNo = 0;
                            }
                        }
                        else if (m_IncreaseOverride)
                        {
                            m_IncreaseOverride = false;
                            double collision_distance = m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionDistance;
                            if (collision_distance >= m_DevSos.GetEnableAccelerationMinDistance())
                            {
                                // Collision Distance로 증속 가능한 최대 속도를 계산하자.
                                double max_distance = m_DevSos.GetTableAreaMaxDistance();

                                double target_velocity = Math.Min(m_ProcessDataHandler.CurVehicleStatus.CurrentPath.Velocity, m_MasterAxis.GetTrajectoryVelocity()); //m_MasterAxis.GetTrajectoryVelocity();
                                double command_velocity = m_MasterAxis.GetCommandVelocity();
                                double acceleration = (m_MasterAxis.GetAxis() as MpAxis).OverrideAcceleration;
                                double deceleration = (m_MasterAxis.GetAxis() as MpAxis).OverrideDeceleration;
                                if (acceleration == 0.0f) acceleration = 2000.0f;
                                if (deceleration == 0.0f) deceleration = 3000.0f;
                                // 증속 가능한 속도는 정속이 거리의 1/2이 되도록 계산
                                // 가감속거리는 가감속도 비율로 계산가능 (가속거리 = 이동거리 * (감속도 / (가속도+감속도)))
                                // 감속거리 = 1/2 * dec * t1 ^ 2 (감속은 무조건 멈춘다는 조건임, 그래서 감속시간을 구하면 증속가능속도를 구할수 있음.)
                                // t1 = SQRT(2*감속거리/dec), v1 = dec * t1
                                // 가속거리 = 1/2 * (v0+v1) * t0
                                // t0 = 2 * 가속거리 / (v0+v1)
                                // v1 = acc * t0
                                // 여기서 필요한 값은 v1임, 정속 비율 r0 = 0.3
                                double constant_velocity_ratio = 0.3f;
                                double distance_ratio = collision_distance / max_distance;
                                double dec_distance = ((1 - constant_velocity_ratio) * collision_distance) * (acceleration / (acceleration + deceleration));
                                double acc_distance = ((1 - constant_velocity_ratio) * collision_distance) * (deceleration / (acceleration + deceleration));
                                double t1 = Math.Sqrt(2 * dec_distance / deceleration);
                                double v1 = distance_ratio * deceleration * t1;
                                double v0 = command_velocity;
                                double t0 = 2 * acc_distance / (v0 + v1);
                                double set_override = target_velocity == 0.0f ? 1.0f : v1 / target_velocity;

                                if (v1 > v0)
                                {
                                    if (set_override > 1.0f) set_override = 1.0f;
                                    m_ProcessDataHandler.CurVehicleStatus.ObsStatus.OverrideRatio = set_override;
                                    SequenceLog.WriteLog(FuncName, $"Set Increase Override = {set_override}, (set_velocity:{v1} > command_velocity:{v0}), target_velocity={target_velocity}");
                                }
                            }
                            SequenceLog.WriteLog(FuncName, $"Set Increase Override : collision_distance={collision_distance}, GetEnableAccelerationMinDistance:{m_DevSos.GetEnableAccelerationMinDistance()}");
                            seqNo = 0;
                        }
                        else
                        {
                            seqNo = 0;
                        }
                    }
                    break;

                case 20:
                    {
                        // Control 하지 않더라도 이전 상태를 계속 유지해야 될것 같음.
                        m_ProcessDataHandler.CurVehicleStatus.ObsStatus.ObsUpperSensorState = currentState;
                        if (m_ProcessDataHandler.CurVehicleStatus.ObsStatus.OverrideDontControl == false)
                        {
                            SequenceLog.WriteLog(FuncName, $"Override Don't Control Area Release, Path={m_ProcessDataHandler.CurVehicleStatus.CurrentPath.ToString()})");
                            seqNo = 0;
                        }
                    }
                    break;
            }
            //if (AppConfig.Instance.Simulation.MY_DEBUG)
            //{
            //    double watch = (double)m_StopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;
            //    string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
            //    System.Diagnostics.Debug.WriteLine($"{time} : [SeqControlOverride] [{watch}]");
            //}

            m_ScanTimeCheckTicks = XFunc.GetTickCount();
            this.SeqNo = seqNo;
            return -1;
        }
        public void AlwaysCheck(enFrontDetectState currentState, enFrontDetectState beforeState, double current_position)
        {
            if (currentState > enFrontDetectState.enNone && currentState <= enFrontDetectState.enStop)
            {
                // Sensor가 감지될때만 Update 하자
                // 감지 되지 않은 상태에서 계산하면 collision distance가 minus가 되고.... collision position도 계속 update 해야 되네 ...~~
                // 동일 Level로 같이 이동하는 경우...Level 최소 distance를 사용하자!
                double distance = m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionPosition - current_position;
                double minDistance = m_DevSos.GetSensorLevelMinDistance();
                double maxDistance = m_DevSos.GetSensorLevelMaxDistance();

                bool scan_time_ok = m_ScanTimeCheckTicks == 0;
                scan_time_ok |= XFunc.GetTickCount() - m_ScanTimeCheckTicks < 100;
                bool minDistanceApply = true;
                if (scan_time_ok)
                {
                    // Scan이 정상적이라면 최소거리를 계속 유지해야만 Override 발생시 멈추지 않고 이동할수있다.
                    minDistanceApply &= minDistance < 99999.0f;
                }
                else
                {
                    // Logic Error... 8단계 일때만 적용하자 ~~. DevSOS 전방 센서 Update Delay 될때 문제가 됨.... OverrideAbnormalStop Error가 다발
                    minDistanceApply &= minDistance < 99999.0f;
                    minDistanceApply &= currentState >= enFrontDetectState.enDeccelation8;
                }
                if (minDistanceApply) // Logic Error .... 앞차 간격 유지 이동시킴.
                {
                    if (distance < minDistance)
                    {
                        m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionPosition = current_position + minDistance;
                        distance = minDistance;
                    }
                }
                if (Math.Abs(distance - 10000.0f) < 1.0f && Math.Abs(m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionDistance - 10000.0f) > 1.0f)
                {
                    SequenceLog.WriteLog(FuncName, $"Forward Sensor Abnormal Changed (before:{beforeState}, current:{currentState}, Distance:{m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionDistance}, Set Distance ={distance})");
                }
                if (distance > maxDistance) distance = maxDistance; // Sensor 범위를 벗어나는 경우는 막자~~
                m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionDistance = distance;
            }

            // 8단계 이상이면 무조건 Pause
            bool pause = currentState >= enFrontDetectState.enDeccelation8;
            if (currentState > enFrontDetectState.enDeccelation2)
                pause |= m_ProcessDataHandler.CurVehicleStatus.ObsStatus.CollisionDistance < m_MasterAxis.GetCommandVelocity();
            m_MasterAxis.SetPause(pause);

            // FrontDetect or VelocityLimit => Holding
            bool hold = GV.AbnormalFrontDetectInterlock || GV.AbnormalVelocityLimitInterlock;
            m_MasterAxis.SetHolding(hold);
        }

        #endregion
    }
    /// <summary>
    /// 하부 Sensor 사용
    /// 직선 구간 진행 중
    /// 1(8m),2(4m),3(1m)차 센서 사용범위를 BCR로 등록 한다.
    /// 사용범위 내에서 센서가 감지 되었는데 Collision Distance가 None일 경우 Abnormal로 판단.
    /// SetHolding 하여 긴급 정지 시키자 ~~~
    /// </summary>
    public class SeqFrontDetectInterlock : XSeqFunc
    {
        public static readonly string FuncName = "[SeqFrontDetectInterlock]";
        #region Fields
        private DatabaseHandler m_DatabaseHandler = null;
        private List<DataItem_FrontDetectFilter> m_ControlFronts = new List<DataItem_FrontDetectFilter>();
        private DevOBS m_DevObs = null;
        private DevSOS m_DevSos = null;
        private _DevAxis m_MasterAxis = null;

        private AlarmData m_ALM_AbnormalFrontDetectInterlock = null;
        private int m_CheckCount = 0;

        private Data.Process.Path m_MyPath = new Data.Process.Path();
        #endregion

        #region Constructor
        public SeqFrontDetectInterlock()
        {
            this.SeqName = $"SeqFrontDetectInterlock";

            m_DatabaseHandler = DatabaseHandler.Instance;
            m_DevObs = DevicesManager.Instance.DevOBSLower;
            m_DevSos = DevicesManager.Instance.DevSOSUpper;
            m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();

            m_ALM_AbnormalFrontDetectInterlock = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, "INTERLOCK", "LOWER OBS", "Abnormal Detect Interlock Alarm");

            //EventHandlerManager.Instance.LinkChanged += Instance_LinkChanged;
        }
        private void Instance_LinkChanged(object obj)
        {
            // 처음부터 다시
            this.SeqNo = 0;
        }
        public override void SeqAbort()
        {
            GV.AbnormalFrontDetectInterlock = false;
            if (AlarmId > 0)
            {
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            this.InitSeq();
        }
        #endregion

        #region Override
        public override int Do()
        {
            if (SetupManager.Instance.SetupSafty.OBSLowerSensorUse == Use.NoUse)
            {
                this.SeqNo = 0;
                if (AlarmId > 0)
                {
                    EqpAlarm.Reset(AlarmId);
                    AlarmId = 0;
                }
                return -1;
            }
            if (m_MyPath.LinkID != ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LinkID ||
                    m_MyPath.ToLinkID != ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.ToLinkID)
            {
                m_MyPath = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath;
                this.SeqNo = 0; //처음부터 다시..... Link가 바뀔때마다 처음부터 다시 시작은 해야 겠는데...
            }

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        // 직선 구간에서 Lower Sensor를 모니터링 하자
                        // 만일 하부가 감지 되었는데 거리 값이 None이면 이상하다.. Halt 시키자 ! 벽이면 어떻게 하지 !
                        // 하부 센서 사용 구간을 정해야 하나 ?
                        if (m_DevSos.GetOBS() != (uint)ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.ObsUpperAreaValue)
                            m_DevSos.SetOBS((uint)ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.ObsUpperAreaValue);
                        bool area_changed_lower = m_DevObs.SetOBS((uint)ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.UBSLevel1);
                        if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type == LinkType.Straight ||
                            ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type == LinkType.LeftBranchStraight ||
                            ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type == LinkType.RightBranchStraight ||
                            ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type == LinkType.LeftJunctionStraight ||
                            ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type == LinkType.RightJunctionStraight)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("{0} Lower Front Detect Check Start", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));

                            m_ControlFronts.Clear();
                            int linkID = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LinkID;
                            m_ControlFronts = m_DatabaseHandler.DictionaryFrontDetectFilter.Values.Where(x => x.LinkId == linkID && x.UseFlag == true).ToList();

                            m_CheckCount = 0;
                            seqNo = 100;
                        }
                        else if (GV.AbnormalFrontDetectInterlock)
                        {
                            GV.AbnormalFrontDetectInterlock = false; // Release 해야 Hold Bit를 해제될수 있다.
                        }
                    }
                    break;

                case 100:
                    {
                        bool front_abnormal = false;
                        double leftBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                        double rightBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                        enFrontDetectState lower_front = m_DevObs.GetFrontDetectState();
                        enFrontDetectState upper_front = m_DevSos.GetFrontDetectState();
                        bool abnormal = true;
                        abnormal &= upper_front < enFrontDetectState.enDeccelation3;
                        abnormal &= lower_front != enFrontDetectState.enNone;
                        abnormal &= GV.WheelBusy;
                        if (abnormal)
                        {
                            // 이상한 상황
                            foreach (DataItem_FrontDetectFilter item in m_ControlFronts)
                            {
                                bool checkRange = false;
                                if (item.LeftBcrStart != 0.0f && item.LeftBcrEnd != 0.0f)
                                    checkRange |= leftBCR > item.LeftBcrStart && leftBCR < item.LeftBcrEnd;
                                if (item.RightBcrStart != 0.0f && item.RightBcrEnd != 0.0f)
                                    checkRange |= rightBCR > item.RightBcrStart && rightBCR < item.RightBcrEnd;

                                bool levelCheck = false;
                                levelCheck |= lower_front == enFrontDetectState.enDeccelation1 && item.SensorLevel == 1;
                                levelCheck |= lower_front == enFrontDetectState.enDeccelation2 && item.SensorLevel == 2;
                                levelCheck |= lower_front == enFrontDetectState.enStop && item.SensorLevel == 3;
                                if (checkRange && levelCheck) front_abnormal = true;
                            }
                            //ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.ObsLowerSensorState = lower_front;
                            //m_MasterAxis.SetHolding(lower_front == enFrontDetectState.enStop);
                        }
                        ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.ObsLowerSensorState = lower_front;

                        if (front_abnormal)
                        {
                            if (lower_front == enFrontDetectState.enStop) GV.AbnormalFrontDetectInterlock = true;

                            m_CheckCount++;
                            double upper_collision_distance = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.CollisionDistance;
                            double lower_collision_distance = m_DevObs.GetTableDistanceOfDetectState(lower_front);
                            SequenceLog.WriteLog(FuncName, string.Format("{0} Lower Front Abnormal({1}) Lower=({2},{3}), Upper=({4},{5})", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath, m_CheckCount, lower_front, lower_collision_distance, upper_front, upper_collision_distance));

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        else
                        {
                            GV.AbnormalFrontDetectInterlock = false;
                            m_CheckCount = 0;
                        }
                    }
                    break;

                case 110:
                    {
                        if (m_CheckCount > 3)
                        {
                            GV.AbnormalFrontDetectInterlock = true;
                            //m_MasterAxis.SetHolding(true);
                            SequenceLog.WriteLog(FuncName, string.Format("{0} Front Abnormal Hold SET", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));
                            SequenceLog.WriteLog(FuncName, string.Format("Abnormal Lower Front Detect Interlock Alarm Set"));
                            // 지금 상황은 Alarm으로 처리하지 말자 ! 자동 복구 될거니까
                            //AlarmId = m_ALM_AbnormalFrontDetectInterlock.ID;
                            //EqpAlarm.Set(AlarmId);

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 120;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 30)
                        {
                            seqNo = 100;
                        }
                    }
                    break;

                case 120:
                    {
                        bool front_abnormal = false;
                        double leftBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                        double rightBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;

                        enFrontDetectState lower_front = m_DevObs.GetFrontDetectState();
                        enFrontDetectState upper_front = m_DevSos.GetFrontDetectState();
                        bool abnormal = true;
                        abnormal &= upper_front < enFrontDetectState.enDeccelation3;
                        abnormal &= lower_front != enFrontDetectState.enNone;
                        if (abnormal)
                        {
                            // 이상한 상황
                            foreach (DataItem_FrontDetectFilter item in m_ControlFronts)
                            {
                                bool checkRange = false;
                                if (item.LeftBcrStart != 0.0f && item.LeftBcrEnd != 0.0f)
                                    checkRange |= leftBCR > item.LeftBcrStart && leftBCR < item.LeftBcrEnd;
                                if (item.RightBcrStart != 0.0f && item.RightBcrEnd != 0.0f)
                                    checkRange |= rightBCR > item.RightBcrStart && rightBCR < item.RightBcrEnd;

                                bool levelCheck = false;
                                levelCheck |= lower_front == enFrontDetectState.enDeccelation1 && item.SensorLevel == 1;
                                levelCheck |= lower_front == enFrontDetectState.enDeccelation2 && item.SensorLevel == 2;
                                levelCheck |= lower_front == enFrontDetectState.enStop && item.SensorLevel == 3;
                                if (checkRange && levelCheck) front_abnormal = true;
                            }
                        }

                        if (!front_abnormal)
                        {
                            GV.AbnormalFrontDetectInterlock = false;
                            EventHandlerManager.Instance.InvokeOverrideResetting();
                            SequenceLog.WriteLog(FuncName, string.Format("{0} Lower Front Normal ({1}) Lower=({2}), Upper=({3})", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath, m_CheckCount, lower_front, upper_front));
                            SequenceLog.WriteLog(FuncName, string.Format("Abnormal Lower Front Detect Interlock Alarm Reset"));
                            // 지금 상황은 Alarm으로 처리하지 말자 ! 자동 복구 될거니까
                            if (AlarmId > 0)
                            {
                                AlarmId = m_ALM_AbnormalFrontDetectInterlock.ID;
                                EqpAlarm.Reset(AlarmId);
                                AlarmId = 0;
                            }
                            seqNo = 0;
                        }
                        else if (AlarmId == 0)
                        {
                            if (XFunc.GetTickCount() - StartTicks > 60 * 1000)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Set Abnormal Lower Front Detect Interlock Alarm"));
                                AlarmId = m_ALM_AbnormalFrontDetectInterlock.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                        }
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;

                            SequenceLog.WriteLog(FuncName, string.Format("Alarm Reset({0})", AlarmId));
                            EqpAlarm.Reset(AlarmId);
                            AlarmId = 0;
                            seqNo = 0;
                        }
                    }
                    break;
            }

            this.SeqNo = seqNo;
            return -1;
        }
        #endregion
    }
    // Curve 진입할때 속도를 확인하여 제한을 걸어야 한다.
    // Remain Distance < 2000 => Velocity < 2000
    //                 < 1000 => Velocity < 1000
    public class SeqVelocityLimitCheck : XSeqFunc
    {
        public static readonly string FuncName = "[SeqVelocityLimitCheck]";
        #region Fields
        private _DevAxis m_MasterAxis = null;

        List<DataItem_VelocityLimit> m_ControlLimits = new List<DataItem_VelocityLimit>();
        private AlarmData m_ALM_AbnormalVelocityOvershoot = null;

        private Data.Process.Path m_MyPath = new Data.Process.Path();
		private int CheckTime = 500;
        #endregion

        #region Constructor
        public SeqVelocityLimitCheck()
        {
            this.SeqName = $"SeqVelocityLimitCheck";
            m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();

            //EventHandlerManager.Instance.LinkChanged += Instance_LinkChanged;
            m_ALM_AbnormalVelocityOvershoot = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "INTERLOCK", "Velocity Limit", "Abnormal Overshoot Alarm");
        }
        private void Instance_LinkChanged(object obj)
        {
            // 처음부터 다시
            if (GV.AbnormalVelocityLimitInterlock == false)
            {
                this.SeqNo = 0;
            }
        }
        public override void SeqAbort()
        {
            GV.AbnormalVelocityLimitInterlock = false;
            if (AlarmId > 0)
            {
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            this.InitSeq();
        }
        #endregion

        #region Override
        public override int Do()
        {
            if (m_MyPath.LinkID != ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LinkID ||
                    m_MyPath.ToLinkID != ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.ToLinkID)
            {
                m_MyPath = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath;
                if (GV.AbnormalVelocityLimitInterlock == false) this.SeqNo = 0;//처음부터 다시..... Link가 바뀔때마다 처음부터 다시 시작은 해야 겠는데...
            }

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        int myLinkID = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LinkID;
                        int toLinkID = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.ToLinkID;
                        m_ControlLimits = DatabaseHandler.Instance.DictionaryVelocityLimit.Values.Where(x => x.LinkId == myLinkID && x.ToLinkId == toLinkID).ToList();
                        if (m_ControlLimits != null && m_ControlLimits.Count > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("{0} Velocity Limit Check Start", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));
                            seqNo = 100;
                        }
                        else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.IsCorner())
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("{0} Curve Velocity Limit Check Start", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));
                            seqNo = 200;
                        }
                    }
                    break;

                case 100:
                    {
                        bool velocity_abnormal = false;
                        double leftBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                        double rightBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                        double curVelocity = m_MasterAxis.GetCommandVelocity();//.GetCurVelocity();
                        foreach (DataItem_VelocityLimit item in m_ControlLimits)
                        {
                            bool checkRange = false;
                            if (item.LeftBcrStart != 0.0f && item.LeftBcrEnd != 0.0f)
                                checkRange |= leftBCR > item.LeftBcrStart && leftBCR < item.LeftBcrEnd;
                            if (item.RightBcrStart != 0.0f && item.RightBcrEnd != 0.0f)
                                checkRange |= rightBCR > item.RightBcrStart && rightBCR < item.RightBcrEnd;
                            if (checkRange)
                            {
                                if (curVelocity > item.MaxVelocity)
                                {
                                    velocity_abnormal = true;
                                    SequenceLog.WriteLog(FuncName, string.Format("{0} Velocity Abnormal :  Velocity={1:F1}>{2:F1},LeftBcr={3:F1}<{4:F1}<{5:F1},RightBcr={6:F1}<{7:F1}<{8:F1}", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath, curVelocity, item.MaxVelocity, item.LeftBcrStart, leftBCR, item.LeftBcrEnd, item.RightBcrStart, rightBCR, item.RightBcrEnd));
                                    break;
                                }
                            }
                        }

                        if (velocity_abnormal)
                        {
                            //m_MasterAxis.SetHolding(true);
                            GV.AbnormalVelocityLimitInterlock = true;
                            SequenceLog.WriteLog(FuncName, string.Format("{0} Velocity Abnormal Hold SET", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));

                            SequenceLog.WriteLog(FuncName, string.Format("Abnormal Velocity Overshoot Alarm Set"));
                            AlarmId = m_ALM_AbnormalVelocityOvershoot.ID;
                            EqpAlarm.Set(AlarmId);

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        else if (AlarmId > 0)
                        {
                            if ((m_MasterAxis.GetAxis() as MpAxis).HoldingBit == false)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Abnormal Velocity Overshoot Alarm Reset"));
                                AlarmId = m_ALM_AbnormalVelocityOvershoot.ID;
                                EqpAlarm.Reset(AlarmId);
                                AlarmId = 0;
                                seqNo = 0;
                            }
                        }
                    }
                    break;

                case 110:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        double curVelocity = m_MasterAxis.GetCommandVelocity();//GetCurVelocity();
                        if (curVelocity < 10.0f)
                        {
                            GV.AbnormalVelocityLimitInterlock = false;
                            SequenceLog.WriteLog(FuncName, string.Format("{0} Velocity Abnormal Stop Confirm {1}", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath, curVelocity));
                            seqNo = 100;
                        }
                    }
                    break;

                case 200:
                    {
                        bool velocity_abnormal = false;
                        double limitVelocity = 1000.0f; // 1.5f * ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Velocity;
                        double curVelocity = m_MasterAxis.GetCommandVelocity();//GetCurVelocity();
                        if (curVelocity > limitVelocity)
                        {
                            velocity_abnormal = true;
                            SequenceLog.WriteLog(FuncName, string.Format("{0} Curve Velocity Abnormal :  curVelocity={1:F1}>limitVelocity={2:F1}", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath, curVelocity, limitVelocity));
                        }

                        if (velocity_abnormal)
                        {
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 210;
                        }
                        else if (AlarmId > 0)
                        {
                            if ((m_MasterAxis.GetAxis() as MpAxis).HoldingBit == false)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Abnormal Curve Velocity Overshoot Alarm Reset"));
                                AlarmId = m_ALM_AbnormalVelocityOvershoot.ID;
                                EqpAlarm.Reset(AlarmId);
                                AlarmId = 0;
                                seqNo = 0;
                            }
                        }
                    }
                    break;

                case 210:
                    {
                        bool velocity_abnormal = false;
                        double limitVelocity = 1000.0f; // 1.5f * ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Velocity;
                        double curVelocity = m_MasterAxis.GetCommandVelocity();//GetCurVelocity();
                        if (curVelocity > limitVelocity)
                        {
                            velocity_abnormal = true;
                        }
                        if (velocity_abnormal)
                        {
                            if (XFunc.GetTickCount() - StartTicks > CheckTime)
                            {
                                //m_MasterAxis.SetHolding(true);
                                GV.AbnormalVelocityLimitInterlock = true;
                                SequenceLog.WriteLog(FuncName, string.Format("{0} Curve Velocity Abnormal Hold SET", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));

                                SequenceLog.WriteLog(FuncName, string.Format("Abnormal Curve Velocity Overshoot Alarm Set"));
                                AlarmId = m_ALM_AbnormalVelocityOvershoot.ID;
                                EqpAlarm.Set(AlarmId);

                                StartTicks = XFunc.GetTickCount();
                                seqNo = 220;
                            }
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 100)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("{0} Curve Velocity Normal :  curVelocity={1:F1}>limitVelocity={2:F1}", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath, curVelocity, limitVelocity));
                            seqNo = 200;
                        }

                        if (!velocity_abnormal && AlarmId > 0)
                        {
                            if ((m_MasterAxis.GetAxis() as MpAxis).HoldingBit == false)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Abnormal Velocity Overshoot Alarm Reset"));
                                AlarmId = m_ALM_AbnormalVelocityOvershoot.ID;
                                EqpAlarm.Reset(AlarmId);
                                AlarmId = 0;
                                seqNo = 0;
                            }
                        }
                    }
                    break;

                case 220:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        double curVelocity = m_MasterAxis.GetCommandVelocity();//GetCurVelocity();
                        if (curVelocity < 10.0f)
                        {
                            GV.AbnormalVelocityLimitInterlock = false;

                            SequenceLog.WriteLog(FuncName, string.Format("{0} Velocity Abnormal Stop Confirm {1}", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath, curVelocity));
                            seqNo = 200;
                        }
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;

                            SequenceLog.WriteLog(FuncName, string.Format("Alarm Reset({0})", AlarmId));
                            EqpAlarm.Reset(AlarmId);
                            AlarmId = 0;
                            seqNo = 0;
                        }
                    }
                    break;
            }

            this.SeqNo = seqNo;
            return -1;
        }
        #endregion
    }
    public class SeqAutoServoOffCheck : XSeqFunc
    {
        public static readonly string FuncName = "[SeqAutoServoOffCheck]";
        #region Fields
        private List<_DevAxis> m_DevAxes = new List<_DevAxis>();
        #endregion

        #region Constructor
        public SeqAutoServoOffCheck()
        {
            this.SeqName = $"SeqAutoServoOffCheck";

            for (int i = 0; i < ServoControlManager.Instance.DevServoUnits.Count; i++)
            {
                m_DevAxes.AddRange(ServoControlManager.Instance.DevServoUnits[i].DevAxes);
            }
        }
        public override void SeqAbort()
        {
            if (AlarmId > 0)
            {
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            this.InitSeq();
        }
        #endregion

        #region Override
        public override int Do()
        {
            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        if (EqpStateManager.Instance.OpMode == OperateMode.Auto && EqpStateManager.Instance.EqpInitComp)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("AutoMode change ! Servo Ready State Check Start !"));
                            seqNo = 10;
                        }
                        else if (GV.AutoModeServoNotReady)
                        {
                            bool ready = true;
                            foreach (_DevAxis axis in m_DevAxes)
                            {
                                if (axis.GetAxis().AxisName == DevicesManager.Instance.DevTransfer.AxisSlave.AxisName) continue;
                                enAxisInFlag status = axis.GetAxis().AxisStatus; // (axis.GetAxis() as IAxisCommand).GetAxisCurStatus();
                                ready &= (status & enAxisInFlag.SvOn) == enAxisInFlag.SvOn;
                            }
                            if (ready)
                            {
                                EqpAlarm.Reset(AlarmId);
                                SequenceLog.WriteLog(FuncName, string.Format("Reset Alarm : Code[{0}]", AlarmId));
                                AlarmId = 0;

                                GV.AutoModeServoNotReady = false;
                                SequenceLog.WriteLog(FuncName, string.Format("ManualMode change ! Servo Ready State Check Stop !"));
                                seqNo = 0;
                            }
                        }
                    }
                    break;

                case 10:
                    {
                        if (EqpStateManager.Instance.OpMode == OperateMode.Auto && EqpStateManager.Instance.EqpInitComp)
                        {
                            bool home = false;
                            int alarm_id = 0;
                            foreach (_DevAxis axis in m_DevAxes)
                            {
                                if (axis.GetAxis().AxisName == DevicesManager.Instance.DevTransfer.AxisSlave.AxisName) continue;
                                if (axis.AxisTag != null && !axis.AxisTag.GetAxis().IsValid) continue;
                                if (!axis.Homing)
                                {
                                    bool ready = true;
                                    enAxisInFlag status = axis.GetAxis().AxisStatus; //(axis.GetAxis() as IAxisCommand).GetAxisCurStatus();
                                    ready &= (status & enAxisInFlag.SvOn) == enAxisInFlag.SvOn;
                                    if (axis.GetAxis().AxisName != DevicesManager.Instance.DevTransfer.AxisMaster.AxisName)
                                        ready &= (status & enAxisInFlag.HEnd) == enAxisInFlag.HEnd;
                                    if (!ready)
                                    {
                                        if (GV.AutoModeServoNotReady == false) // Log를 여러번 찍는걸 막자
                                        {
                                            if ((status & enAxisInFlag.SvOn) != enAxisInFlag.SvOn)
                                                SequenceLog.WriteLog(FuncName, string.Format("{0} : Axis is Not Servo On", axis.GetAxis().AxisName));
                                            if ((status & enAxisInFlag.HEnd) != enAxisInFlag.HEnd)
                                                SequenceLog.WriteLog(FuncName, string.Format("{0} : Axis is Not Home", axis.GetAxis().AxisName));
                                        }
                                        alarm_id = axis.ALM_AutoModeServoNotReadyError.ID;
                                        break;
                                    }
                                }
                                home |= axis.Homing;
                            }

                            if (home)
                            {
                                string msg = string.Empty;
                                foreach (_DevAxis axis in m_DevAxes)
                                    if (axis.Homing) msg += string.Format("{0},", axis.GetName());

                                SequenceLog.WriteLog(FuncName, string.Format("Axis Home : {0}", msg));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                            else if (alarm_id == 0)
                            {
                                if (GV.AutoModeServoNotReady)
                                {
                                    GV.AutoModeServoNotReady = false;
                                    if (AlarmId != 0) EqpAlarm.Reset(AlarmId);
                                    AlarmId = 0;
                                }

                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else if (GV.AutoModeServoNotReady == false)
                            {
                                GV.AutoModeServoNotReady = true;
                                AlarmId = alarm_id;
                                SequenceLog.WriteLog(FuncName, string.Format("Axis Not Ready - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                                EqpAlarm.Set(AlarmId);
                            }
                        }
                        else
                        {
                            seqNo = 0;
                        }
                    }
                    break;

                case 20:
                    {
                        if (XFunc.GetTickCount() - StartTicks > 1000)
                        {
                            seqNo = 10;
                        }
                    }
                    break;

                case 30:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        bool home = false;
                        foreach (_DevAxis axis in m_DevAxes)
                        {
                            if (axis.GetAxis().AxisName == DevicesManager.Instance.DevTransfer.AxisSlave.AxisName) continue;
                            home |= axis.Homing;
                        }
                        if (!home)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Axis Home OK"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 40;
                        }
                    }
                    break;

                case 40:
                    {
                        if (XFunc.GetTickCount() - StartTicks > 2000)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Axis Home Delay Time"));
                            seqNo = 10;
                        }
                    }
                    break;
            }

            this.SeqNo = seqNo;
            return -1;
        }
        #endregion
    }
    // Steer 조건을 확인하고 있다가 Link의 남은 거리가 현재 속도로 멈출수 없는 거리면 Stop
    // Steer가 이동하면서 깜빡이는 문제 때문에 설정과 반대방향일때만 Interlock으로 사용하자 ~
    public class SeqSteerStateCheck : XSeqFunc
    {
        private const string FuncName = "[SeqSteerStateCheck]";

        #region Fields
        private ProcessDataHandler m_ProcessDataHandler = null;
        private DevicesManager m_DeviceManager = null;

        #endregion

        #region Constructor
        public SeqSteerStateCheck()
        {
            this.SeqName = $"SeqSteerStateCheck";

            m_ProcessDataHandler = ProcessDataHandler.Instance;
            m_DeviceManager = DevicesManager.Instance;
        }
        #endregion

        #region Methods
        public override void SeqAbort()
        {
            if (AlarmId > 0)
            {
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            this.InitSeq();
        }
        public override int Do()
        {
            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        // Auto 운전일때만 확인하자 ! Manual 일때는 불확실성이 많다.
                        if (EqpStateManager.Instance.OpMode != OperateMode.Auto && m_DeviceManager.DevSteer.IsValid)
                        {
                            bool sCurve = false;
                            sCurve |= m_ProcessDataHandler.CurVehicleStatus.CurrentPath.Type == LinkType.LeftSBranch;
                            sCurve |= m_ProcessDataHandler.CurVehicleStatus.CurrentPath.Type == LinkType.RightSBranch;
                            if (sCurve) break;
                            if (m_ProcessDataHandler.CurVehicleStatus.CurrentPath.Certain == false) break;
                            double linkRemainDistance = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.RemainDistanceOfLink;
                            if (linkRemainDistance > 0 && linkRemainDistance < m_ProcessDataHandler.CurVehicleStatus.CurrentPath.Distance)
                            {
                                double curWheelVelocity = m_DeviceManager.DevTransfer.AxisMaster.GetDevAxis().GetCurVelocity();
                                double decMax = SetupManager.Instance.SetupWheel.StraightMaxDec;
                                double dt = 2.0f * curWheelVelocity / decMax;
                                double stopDistance = 0.5f * curWheelVelocity * dt;
                                double offset = m_DeviceManager.DevSteer.FrontRearOffset;
                                if (linkRemainDistance < stopDistance)
                                {
                                    enSteerDirection link_steer = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.SteerDirection;
                                    enSteerDirection front_steer = m_DeviceManager.DevSteer.GetFrontSteerDirection();
                                    if (link_steer != enSteerDirection.DontCare)
                                    {
                                        enSteerDirection check_steer = link_steer == enSteerDirection.Left ? enSteerDirection.Right : enSteerDirection.Left;
                                        if (check_steer == front_steer)
                                        {
                                            m_DeviceManager.DevTransfer.SeqAbort();
                                            int linkID = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.LinkID;
                                            SequenceLog.WriteLog(FuncName, $"Steer Not Changed (LINK:{linkID}, linkSteer:{link_steer}, front_steer:{front_steer}) linkRemainDistance:{linkRemainDistance} < StopDistance:{stopDistance}");

                                            if (link_steer == enSteerDirection.Left)
                                                AlarmId = m_DeviceManager.DevSteer.ALM_FrontSteerLeftChangedInterlockAlarm.ID;
                                            else AlarmId = m_DeviceManager.DevSteer.ALM_FrontSteerRightChangedInterlockAlarm.ID;
                                            EqpAlarm.Set(AlarmId);
                                            seqNo = 1000;
                                        }
                                    }
                                }
                                else if (linkRemainDistance + offset < stopDistance)
                                {
                                    enSteerDirection link_steer = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.SteerDirection;
                                    enSteerDirection rear_steer = m_DeviceManager.DevSteer.GetRearSteerDirection();
                                    if (link_steer != enSteerDirection.DontCare)
                                    {
                                        enSteerDirection check_steer = link_steer == enSteerDirection.Left ? enSteerDirection.Right : enSteerDirection.Left;
                                        if (check_steer == rear_steer)
                                        {
                                            m_DeviceManager.DevTransfer.SeqAbort();
                                            int linkID = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.LinkID;
                                            SequenceLog.WriteLog(FuncName, $"Steer Not Changed (LINK:{linkID}, linkSteer:{link_steer}, rear_steer:{rear_steer}) linkRemainDistance:{linkRemainDistance} < StopDistance:{stopDistance}");

                                            if (link_steer == enSteerDirection.Left)
                                                AlarmId = m_DeviceManager.DevSteer.ALM_RearSteerLeftChangedInterlockAlarm.ID;
                                            else AlarmId = m_DeviceManager.DevSteer.ALM_RearSteerRightChangedInterlockAlarm.ID;
                                            EqpAlarm.Set(AlarmId);
                                            seqNo = 1000;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;

                            SequenceLog.WriteLog(FuncName, string.Format("Alarm Reset({0})", AlarmId));
                            EqpAlarm.Reset(AlarmId);
                            AlarmId = 0;
                            seqNo = 0;
                        }
                    }
                    break;
            }

            this.SeqNo = seqNo;
            return -1;
        }
        #endregion
    }

    public class SeqSaftyCheck : XSeqFunc
    {
        private const string FuncName = "[SeqSaftyCheck]";

        #region Fields
        private ProcessDataHandler m_ProcessDataHandler = null;
        private DevicesManager m_DeviceManager = null;
        private _DevAxis m_MasterAxis = null;
        private _DevAxis m_SlideAxis = null;
        private _DevAxis m_HoistAxis = null;
        private _DevAxis m_RotateAxis = null;
        #endregion

        #region Constructor
        public SeqSaftyCheck()
        {
            this.SeqName = $"SeqSaftyCheck";

            m_ProcessDataHandler = ProcessDataHandler.Instance;
            m_DeviceManager = DevicesManager.Instance;
            m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();
            m_SlideAxis = DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis();
            m_HoistAxis = DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis();
            m_RotateAxis = DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis();
        }
        #endregion
        public override void SeqAbort()
        {
            if (AlarmId > 0)
            {
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            this.InitSeq();
        }
        public override int Do()
        {
            int rv1 = -1;

            // Interlock Alarm 발생할 경우....
            // 모든 동작을 중단한다.
            bool interlock_alarm = !GV.PowerOn;
            interlock_alarm |= GV.EmoAlarm;
            interlock_alarm |= GV.SaftyAlarm;
            interlock_alarm |= GV.CpAlarm;
            interlock_alarm |= GV.SteerNotChangeInterlock && GV.WheelBusy;
            interlock_alarm |= GV.BumpCollisionInterlock && GV.WheelBusy;
            interlock_alarm |= GV.BeltCutInterlock;
            interlock_alarm |= GV.SwingSensorInterlock;
            interlock_alarm |= GV.AutoModeServoNotReady;
            interlock_alarm |= GV.ThreadStop;
            interlock_alarm |= GV.CleanerDoorOpenInterlock;

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        if (IoManager.Instance.UpdateRun == false) break;
                        if (ServoManager.Instance.UpdateRun == false) break;
                        if (XFunc.GetTickCount() - StartTicks < 5000) break;

                        // Transfer Wheel Move Enable Check
                        int move_enable_code = 0;
                        if (GV.BumpCollisionInterlock) move_enable_code = 1;
                        else if (GV.SteerNotChangeInterlock) move_enable_code = 2;
                        else if (m_HoistAxis != null) if (Math.Abs(m_HoistAxis.GetCurPosition()) > 10.0f) move_enable_code = 3;
                        else if (m_SlideAxis != null) if (Math.Abs(m_SlideAxis.GetCurPosition()) > 30.0f) move_enable_code = 4;
                        else if (m_DeviceManager.DevTransfer.IsValid)
                        {
                            if (m_DeviceManager.DevTransfer.DiWheelBrake.IsValid) if (m_DeviceManager.DevTransfer.DiWheelBrake.IsDetected == false) move_enable_code = 5;
                        }
                        if (m_ProcessDataHandler.CurVehicleStatus.ObsStatus.ObsUpperSensorState == enFrontDetectState.enStop) move_enable_code = 6;
                        if (m_DeviceManager.DevGripperPIO.IsValid)
                        {
                            bool existing_foup = false;
                            existing_foup |= m_DeviceManager.DevGripperPIO.IsHoistLimit();
                            existing_foup |= m_DeviceManager.DevGripperPIO.IsHoistUp();
                            existing_foup |= m_DeviceManager.DevGripperPIO.IsHoistHome();
                            if (existing_foup && m_DeviceManager.DevGripperPIO.IsGripperClose() == false) move_enable_code = 7;
                        }

                        if (m_HoistAxis != null) if ((m_HoistAxis.GetAxis().AxisStatus & enAxisInFlag.Org) != enAxisInFlag.Org) move_enable_code = 8;
                            if (SetupManager.Instance.SetupOperation.Early_Motion_Use == Use.NoUse)
                            {
                                if (m_SlideAxis != null) if ((m_SlideAxis.GetAxis().AxisStatus & enAxisInFlag.Org) != enAxisInFlag.Org) move_enable_code = 9;
                                else if (m_RotateAxis != null) if ((m_RotateAxis.GetAxis().AxisStatus & enAxisInFlag.Org) != enAxisInFlag.Org) move_enable_code = 10;
                            }
                        GV.TransferMoveEnableCode = move_enable_code;
                        GV.TransferMoveEnable = move_enable_code == 0;

                        // Hoist Move Enable Check
                        int hoist_move_enable_code = 0;

                        if (GV.BeltCutInterlock) hoist_move_enable_code = 1;
                        else if (GV.SwingSensorInterlock) hoist_move_enable_code = 2;
                        else if (m_MasterAxis != null && m_MasterAxis.IsMoving()) hoist_move_enable_code = 3;

                        if (m_DeviceManager.DevFoupGripper.IsValid && m_DeviceManager.DevFoupGripper.DiHoistBrake.IsValid)
                        {
                            bool servo_on = m_HoistAxis != null ? m_HoistAxis.GetAxis().AxisStatus.HasFlag(enAxisInFlag.SvOn) : true;
                            if (servo_on && !m_DeviceManager.DevFoupGripper.DiHoistBrake.IsDetected)  hoist_move_enable_code = 4; // release
                        }
                        if (m_DeviceManager.DevGripperPIO.IsValid)
                        {
                            bool foup_exist = m_DeviceManager.DevGripperPIO.IsGripperClose();
                            foup_exist &= m_DeviceManager.DevGripperPIO.IsProductExist() || m_DeviceManager.DevGripperPIO.IsHoistHome();
                            bool foup_not_exist = m_DeviceManager.DevGripperPIO.IsGripperOpen();
                                foup_not_exist &= m_DeviceManager.DevGripperPIO.IsProductNotExist();
                            if ((!foup_exist && !foup_not_exist) || (foup_exist && foup_not_exist)) hoist_move_enable_code = 5;
                        }
                        GV.HoistMoveEnableCode = hoist_move_enable_code;
                        GV.HoistMoveEnable = hoist_move_enable_code == 0;

                        // Slide Move Enable Check
                        int slide_move_enable_code = 0;

                        if (m_MasterAxis != null && m_MasterAxis.IsMoving()) hoist_move_enable_code = 1;
                        GV.SlideMoveEnableCode = slide_move_enable_code;
                        GV.SlideMoveEnable = slide_move_enable_code == 0;

                        // Gripper Open Enable Check
                        bool carrier_installed = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
                        bool open_enable1 = true;
                        if (m_HoistAxis != null) open_enable1 &= m_HoistAxis.GetCurPosition() < -10.0f;
                        if (m_DeviceManager.DevGripperPIO.IsValid)
                        {
                            open_enable1 &= m_DeviceManager.DevGripperPIO.DiHoistHome.IsDetected;
                            open_enable1 &= m_DeviceManager.DevGripperPIO.DiHoistUp.IsDetected;
                            open_enable1 &= !m_DeviceManager.DevGripperPIO.DiHoistLimit.IsDetected;
                        }

                        bool open_enable2 = true;
                        if (m_DeviceManager.DevGripperPIO.IsValid)
                        {
                            open_enable2 &= !m_DeviceManager.DevGripperPIO.DiHoistUp.IsDetected;
                            open_enable2 &= !m_DeviceManager.DevGripperPIO.DiHoistLimit.IsDetected;
                            if (m_DeviceManager.DevGripperPIO.DiLeftProductExist.IsValid)
                                open_enable2 &= !m_DeviceManager.DevGripperPIO.DiLeftProductExist.IsDetected;
                            if (m_DeviceManager.DevGripperPIO.DiRightProductExist.IsValid)
                                open_enable2 &= !m_DeviceManager.DevGripperPIO.DiRightProductExist.IsDetected;
                        }
                        GV.GripOpenEnable = open_enable1 || (!carrier_installed && open_enable2);

                        if (interlock_alarm)
                        {
                            SequenceLog.WriteLog(FuncName, "Safty Interlock Occur");

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 10;
                        }
                    }
                    break;

                case 10:
                    {
                        int interlcokcheckTime = 200;
                        if (GV.SteerNotChangeInterlock) interlcokcheckTime = 10;
                        if (XFunc.GetTickCount() - StartTicks > interlcokcheckTime)
                        {
                            string message = string.Format("Interlock Alarm Occur ! PowerOn={0}, EmoAlarm={1}, SaftyAlarm={2}, CpAlarm={3}, SteerNotChangeInterlock={4}, BumpCollisionInterlock={5}, BeltCutInterlock={6}, ServoNotReady={7}, SwingSensorInterlock={8}, ThreadStop={9}, CleanerDoorOpenInterlock={10}",
                                !GV.PowerOn, GV.EmoAlarm, GV.SaftyAlarm, GV.CpAlarm, GV.SteerNotChangeInterlock, GV.BumpCollisionInterlock, GV.BeltCutInterlock, GV.AutoModeServoNotReady, GV.SwingSensorInterlock, GV.ThreadStop, GV.CleanerDoorOpenInterlock);
                            EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.InterlockMessage, message);

                            //if (EqpStateManager.Instance.RunMode == EqpRunMode.Start) // Start 일때만 Stop로 전환하자 ~~
                            //    EqpStateManager.Instance.SetRunMode(EqpRunMode.Stop);

                            if (EqpStateManager.Instance.EqpInitIng || EqpStateManager.Instance.EqpInitComp)
                                EqpStateManager.Instance.ResetInitState();

                            DevicesManager.Instance.SeqAbort();
                            SequenceLog.WriteLog(FuncName, string.Format("All Devices Abort Call. {0}", message));

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        else if (!interlock_alarm)
                        {
                            seqNo = 0;
                        }
                    }
                    break;

                case 20:
                    {
                        if (interlock_alarm == false && XFunc.GetTickCount() - StartTicks > 2000)
                        {
                            EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.Hide, "");
                            SequenceLog.WriteLog(FuncName, string.Format("Interlock Alarm Release"));
                            seqNo = 0;
                        }
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;

                            SequenceLog.WriteLog(FuncName, string.Format("Reset Alarm : Code[{0}]", AlarmId));
                            EqpAlarm.Reset(AlarmId);
                            AlarmId = 0;
                            seqNo = ReturnSeqNo;
                        }
                    }
                    break;

            }
            this.SeqNo = seqNo;

            return -1;
        }
    }

    public class SeqTorqueLimitApply : XSeqFunc
    {
        private const string FuncName = "[SeqTorqueLimitApply]";

        #region Fields
        private ProcessDataHandler m_ProcessDataHandler = null;
        private _DevAxis m_MasterAxis = null;
        #endregion

        #region Constructor
        public SeqTorqueLimitApply()
        {
            this.SeqName = $"SeqTorqueLimitApply";

            m_ProcessDataHandler = ProcessDataHandler.Instance;
            m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();
        }
        #endregion

        #region Methods
        public override void SeqAbort()
        {
            if (AlarmId > 0)
            {
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            this.InitSeq();
        }
        public override int Do()
        {
            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        // Torque Limit Bit 설정을 하자
                        Data.Process.Path path = m_ProcessDataHandler.CurVehicleStatus.CurrentPath;
                        if (path != null && m_MasterAxis.IsValid)
                        {
                            _Axis master_axis = m_MasterAxis.GetAxis();
                            if ((master_axis as MpAxis).TorqueLimitUse)
                            {
                                Data.Process.Path beforePath = m_ProcessDataHandler.CurTransferCommand.GetFromPath(path);
                                Data.Process.Path nextPath = m_ProcessDataHandler.CurTransferCommand.GetToPath(path);
                                bool corner_torque0 = nextPath != null ? nextPath.IsCorner() : false;
                                corner_torque0 &= path.RemainDistanceOfLink < SetupManager.Instance.SetupOperation.TorqueLimitApplyMargin_BeforeCurve;
                                corner_torque0 &= path.Distance > 5.0f * SetupManager.Instance.SetupOperation.TorqueLimitApplyMargin_BeforeCurve;
                                
                                bool corner_torque1 = beforePath != null ? beforePath.IsCorner() : false;
                                corner_torque1 &= path.CurrentPositionOfLink < SetupManager.Instance.SetupOperation.TorqueLimitApplyMargin_AfterCurve;
                                corner_torque1 &= path.Distance > 5.0f * SetupManager.Instance.SetupOperation.TorqueLimitApplyMargin_AfterCurve;
                                
                                bool corner_torque = path.IsCorner();
                                corner_torque |= corner_torque0;
                                corner_torque |= corner_torque1;
                                
                                if (corner_torque)
                                {
                                    (master_axis as MpAxis).TorqueLimitBit = true;
                                    path.TorqueLimit = (int)(master_axis as MpAxis).TorqueLimitRate;
                                }
                                else if (ProcessDataHandler.Instance.CurTransferCommand.IsValid)
                                {
                                    // corner -> straight -> corner가 연속으로 되어 있는 경우는 제외하자 ~~
                                    Path fromPath = ProcessDataHandler.Instance.CurTransferCommand.GetFromPath(path);
                                    if (fromPath != null && fromPath.IsCorner() && path.Distance < 2000.0f)
                                    {
                                        (master_axis as MpAxis).TorqueLimitBit = true;
                                        path.TorqueLimit = (int)(master_axis as MpAxis).TorqueLimitRate;
                                    }
                                    else
                                    {
                                        (master_axis as MpAxis).TorqueLimitBit = false;
                                        path.TorqueLimit = (int)(master_axis as MpAxis).TorqueRunRate;
                                    }
                                }
                                else
                                {
                                    (master_axis as MpAxis).TorqueLimitBit = false;
                                    path.TorqueLimit = (int)(master_axis as MpAxis).TorqueRunRate;
                                }
                            }
                        }
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;

                            SequenceLog.WriteLog(string.Format("{0} Alarm Reset", FuncName));
                            EqpAlarm.Reset(AlarmId);
                            AlarmId = 0;
                            seqNo = 0;
                        }
                    }
                    break;
            }

            this.SeqNo = seqNo;
            return -1;
        }
        #endregion
    }

}
