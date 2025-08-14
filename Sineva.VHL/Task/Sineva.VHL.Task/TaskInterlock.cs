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
using System.Reflection;
using Sineva.VHL.Data.EventLog;
using Sineva.VHL.IF.OCS;

namespace Sineva.VHL.Task
{
    public class TaskInterlock : XSequence
    {
        public static readonly TaskInterlock Instance = new TaskInterlock();

        public TaskInterlock()
        {
            ThreadInfo.Name = string.Format("TaskInterlock");

            this.RegSeq(new SeqAutoRecovery());
            this.RegSeq(new SeqInterlockAutoRecovery());
            this.RegSeq(new SeqCpsVoltageLowCheck());
            this.RegSeq(new SeqRouteChangedCheck());
            this.RegSeq(new SeqBcrNotChangedCheck());
            this.RegSeq(new SeqMxpHeartBitCheck());
            this.RegSeq(new SeqMxpEtherCatConnectionCheck());
            this.RegSeq(new SeqOverrideAbnormalStopCheck());
            //this.RegSeq(new SeqFoupCoverNotDetectCheck());//2024.12.31 Foup Cover Detect //미사용
            this.RegSeq(new SeqTouchHeartBitCheck());
        }
    }

    public class SeqAutoRecovery : XSeqFunc
    {
        private const string FuncName = "[SeqAutoRecovery]";

        #region Fields
        private ProcessDataHandler m_ProcessDataHandler = null;
        private DevicesManager m_DeviceManager = null;
        private AlarmData m_ALM_AutoRecoveryStartFail = null;

        private bool m_RecoveryCheck = false;
        #endregion

        #region Constructor
        public SeqAutoRecovery()
        {
            this.SeqName = "SeqAutoRecovery";
            m_ProcessDataHandler = ProcessDataHandler.Instance;
            m_DeviceManager = DevicesManager.Instance;

            m_ALM_AutoRecoveryStartFail = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, "SeqAutoRecovery", "Recovery Condition", "Command State Not Excute Alarm");

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
            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        // Auto - Start 운전 상태
                        // CurTransfer Command가 유효한 상태
                        // GetVehicleStatus() == VehicleState.EnRouteToDest | EnRouteToSource | Go 일때
                        // 
                        // Abort Mode로 전환.... 모든 Sequence가 초기화 되길 기다린다.
                        // Stop 상태로 전환
                        // Initialize 실시
                        // Ready가 완료 된 경우 .... 
                        // Auto - Start 상태로 전환

                        if (GV.AutoRecoveryStart)
                        {
                            GV.AutoRecoveryStart = false;

                            bool vehicle_moving_cmd = false; // 이동한다는 명령을 받고 이동 중인 상황
                            vehicle_moving_cmd |= m_ProcessDataHandler.CurVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToSource;
                            vehicle_moving_cmd |= m_ProcessDataHandler.CurVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToDest;
                            vehicle_moving_cmd |= m_ProcessDataHandler.CurVehicleStatus.GetVehicleStatus() == VehicleState.Go;
                            vehicle_moving_cmd |= m_ProcessDataHandler.CurVehicleStatus.GetVehicleStatus() == VehicleState.AcquireCompleted;
                            vehicle_moving_cmd |= m_ProcessDataHandler.CurVehicleStatus.GetVehicleStatus() == VehicleState.DepositeCompleted;
                            vehicle_moving_cmd |= m_ProcessDataHandler.CurVehicleStatus.GetVehicleStatus() == VehicleState.AcquireFailed;
                            vehicle_moving_cmd |= m_ProcessDataHandler.CurVehicleStatus.GetVehicleStatus() == VehicleState.DepositFailed;
                            vehicle_moving_cmd |= m_ProcessDataHandler.CurVehicleStatus.GetVehicleStatus() == VehicleState.NotAssigned; //명령 시작전에도 autoRecovery가 되어야지
                            if (m_ProcessDataHandler.CurVehicleStatus.GetVehicleStatus() == VehicleState.Acquiring || m_ProcessDataHandler.CurVehicleStatus.GetVehicleStatus() == VehicleState.Depositing)
                            {
                                vehicle_moving_cmd |= DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis().GetCurPosition() > -10.0f;
                            }

                            bool recovery = EqpStateManager.Instance.OpMode == OperateMode.Auto;
                            recovery &= m_ProcessDataHandler.CurTransferCommand.IsValid == false;
                            recovery &= vehicle_moving_cmd;

                            if (recovery)
                            {
                                SequenceLog.WriteLog(FuncName, "Eqp Recovery Start");
                                if (EqpStateManager.Instance.RunMode != EqpRunMode.Abort)
                                {
                                    EqpStateManager.Instance.SetRunMode(EqpRunMode.Stop);
                                    SequenceLog.WriteLog(FuncName, "Run Mode Stop");
                                }
                                else
                                {
                                    SequenceLog.WriteLog(FuncName, "Run Mode Abort");
                                }
                                EqpAlarm.ResetAll();
                                SequenceLog.WriteLog(FuncName, "Alarm All Reset");

                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName, "Eqp Recovery can't Start");
                                if (m_RecoveryCheck)
                                {
                                    if (XFunc.GetTickCount() - StartTicks > 30 * 1000)
                                    {
                                        AlarmId = m_ALM_AutoRecoveryStartFail.ID;
                                        EqpAlarm.Set(AlarmId);
                                        seqNo = 1000;
                                    }
                                }
                                else
                                {
                                    StartTicks = XFunc.GetTickCount();
                                    m_RecoveryCheck = true;

                                }
                            }
                        }
                    }
                    break;

                case 10:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;
                        EqpStateManager.Instance.ResetInitState(); //초기화 후 다시 진행해라....
                        EqpStateManager.Instance.EqpRecoveryInitReq = true;
                        SequenceLog.WriteLog(FuncName, "EQP Intialize Request");
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 15;
                    }
                    break;

                case 15:
                    {
                        if (EqpStateManager.Instance.EqpInitComp == false)
                        {
                            SequenceLog.WriteLog(FuncName, "EQP Intialize EqpInitComp==false");
                            seqNo = 20;
                        }
                    }
                    break;

                case 20:
                    {
                        if (EqpStateManager.Instance.EqpInitComp)
                        {
                            SequenceLog.WriteLog(FuncName, "EQP Intialize Complete");
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Start);
                            SequenceLog.WriteLog(FuncName, "Run Mode Start");
                            seqNo = 0;
                        }
                    }
                    break;

                case 1000:
                    if (IsPushedSwitch.IsAlarmReset)
                    {
                        IsPushedSwitch.m_AlarmRstPushed = false;
                        EqpAlarm.Reset(AlarmId);
                        SequenceLog.WriteLog(FuncName, string.Format("Reset Alarm : Code[{0}]", AlarmId));
                        AlarmId = 0;
                        seqNo = ReturnSeqNo;
                    }
                    break;

            }
            this.SeqNo = seqNo;

            return -1;
        }
    }
    public class SeqInterlockAutoRecovery : XSeqFunc
    {
        private const string FuncName = "[SeqInterlockAutoRecovery]";

        #region Fields
        private _DevAxis m_MasterAxis = null;
        private DevSOS m_DevSos = null;

        private AlarmData m_ALM_AbnormalCpsLowVoltageError = null;
        private AlarmData m_ALM_RouteChangeTimeoverError = null;
        private AlarmData m_ALM_BcrNotChangedError = null;
        private AlarmData m_ALM_MxpHeartBitError = null;
        private AlarmData m_ALM_EthcatDisconnectedError = null;
        private AlarmData m_ALM_TouchHeartBitError = null;

        private int m_CurInitializeCount = 0;
        private int m_MaxInitializeCount = 3;
        #endregion

        #region Constructor
        public SeqInterlockAutoRecovery()
        {
            this.SeqName = "SeqInterlockAutoRecovery";

            m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();
            m_DevSos = DevicesManager.Instance.DevSOSUpper;

            m_ALM_AbnormalCpsLowVoltageError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "INTERLOCK", "Auto Recovery", "CPS Abnormal Low Voltage Alarm");
            m_ALM_RouteChangeTimeoverError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "INTERLOCK", "Auto Recovery", "Route Change Timeover Alarm");
            m_ALM_BcrNotChangedError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "INTERLOCK", "Auto Recovery", "BCR Not Changed Alarm");
            m_ALM_MxpHeartBitError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "INTERLOCK", "Auto Recovery", "MXP Heart Bit Alarm");
            m_ALM_EthcatDisconnectedError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "INTERLOCK", "Auto Recovery", "EtherCat Disconnected Alarm");
            m_ALM_TouchHeartBitError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "INTERLOCK", "Auto Recovery", "TouchGUI Heart Bit Alarm");
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
            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        // 1. Auto 운전중 Interlock Alarm 발생(Interlock은 멈춘후 Alarm)
                        // MxpHeartBitInterlock
                        // RouteChangeTimeOverInterlock
                        // BCRNotChangedInterlock 
                        // OverrideAbnormalStopInterlock 
                        // CpsLowVoltageInterlock 
                        // 2. EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort);
                        // 3. GV.scmAbortSeqRun.Start
                        // 4. if (GV.scmAbortSeqRun.Start == false)
                        // 5. EqpStateManager.Instance.SetRunMode(EqpRunMode.Stop);
                        // 6. EqpAlarm.ResetAll();
                        // 7. EqpStateManager.Instance.ResetInitState();
                        // 8. EqpStateManager.Instance.EqpRecoveryInitReq = true;
                        // 9. if (EqpStateManager.Instance.EqpInitComp)
                        // 10. Interlock Alarm 확인
                        // 11. Alarm 유 : SetAlarm(InterlockAlarm.ID)
                        // 12. Alarm 무 : EqpStateManager.Instance.SetRunMode(EqpRunMode.Start);
                        // 13. Start 조건 : 전방감지 상태,

                        bool run = EqpStateManager.Instance.OpMode == OperateMode.Auto;

                        if (run)
                        {
                            bool interlock_alarm = false;
                            interlock_alarm |= GV.MxpHeartBitInterlock;
                            interlock_alarm |= GV.RouteChangeTimeOverInterlock;
                            interlock_alarm |= GV.BCRNotChangedInterlock;
                            interlock_alarm |= GV.OverrideAbnormalStopInterlock;
                            interlock_alarm |= GV.CpsLowVoltageInterlock;
                            interlock_alarm |= GV.EtherCatDisconnectError;
                            interlock_alarm |= GV.TouchHeartBitInterlock;

                            if (interlock_alarm)
                            {
                                GV.InterlockRecoveryStart = true;
                                string msg = "";
                                if (GV.MxpHeartBitInterlock) msg += " | MxpHeartBitInterlock";
                                if (GV.RouteChangeTimeOverInterlock) msg += " | RouteChangeTimeOverInterlock";
                                if (GV.BCRNotChangedInterlock) msg += " | BCRNotChangedInterlock";
                                if (GV.OverrideAbnormalStopInterlock)
                                {
                                    msg += " | OverrideAbnormalStopInterlock"; 
                                    GV.RecoveryProcess = true; //전방감지 정지면 Alarm 및 Manual로 빠지면 안됨.. 고객요청
                                }
                                if (GV.CpsLowVoltageInterlock) msg += " | CpsLowVoltageInterlock";
                                if (GV.EtherCatDisconnectError) msg += " | EtherCatDisconnectError";
                                if (GV.TouchHeartBitInterlock) msg += " | TouchHeartBitInterlock";
                                SequenceLog.WriteLog(FuncName, string.Format("Interlock Occur {0}", msg));

                                EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort);
                                SequenceLog.WriteLog(FuncName, "Abort Sequence Start");
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                        }
                    }
                    break;

                case 10:
                    {
                        if (GV.scmAbortSeqRun.Ing || GV.scmAbortSeqRun.End)
                        {
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 10000) //Abort 하는 시간이 10초는 필요하네...
                        {
                            SequenceLog.WriteLog(FuncName, "Abort Sequence Timeover");
                            // 왜 Abort 진행을 못하는거지 ?
                            // 다시 Retry 해라...!
                            List<XSeqFunc> funcs = TaskHandler.Instance.GetFuncLists();
                            foreach (XSeqFunc func in funcs)
                            {
                                if (func.SeqName == "SeqAbortMonitor") { func.SeqAbort(); break; }
                            }
                            StartTicks = XFunc.GetTickCount();
                            if (EqpStateManager.Instance.RunMode != EqpRunMode.Abort) 
                                seqNo = 15; //이상하네 왜 Abort가 아니지 ?
                        }
                    }
                    break;

                case 15:
                    {
                        EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort);
                        SequenceLog.WriteLog(FuncName, "Abort Sequence Start");
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 10;
                    }
                    break;

                case 20:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;
                        if (GV.scmAbortSeqRun.End)
                        {
                            SequenceLog.WriteLog(FuncName, "Abort Sequence Completed");
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Stop); // Stop으로 전환 후 초기화 하자~~
                            EqpAlarm.ResetAll();
                            SequenceLog.WriteLog(FuncName, "Alarm All Reset");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                    }
                    break;

                case 30:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;
                        EqpStateManager.Instance.ResetInitState(); //초기화 후 다시 진행해라....
                        EqpStateManager.Instance.EqpRecoveryInitReq = true;
                        SequenceLog.WriteLog(FuncName, "EQP Intialize Request");
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 35;
                    }
                    break;

                case 35:
                    {
                        if (GV.InitStart)
                        {
                            SequenceLog.WriteLog(FuncName, "EQP Intialize Start");
                            seqNo = 40;
                        }
                    }
                    break;

                case 40:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;
                        if (GV.InitComp)
                        {
                            SequenceLog.WriteLog(FuncName, "EQP Intialize Complete");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 50;
                        }
                        else if (GV.InitFail)
                        {
                            SequenceLog.WriteLog(FuncName, "EQP Intialize Fail");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 50;
                        }
                    }
                    break;

                case 50:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        bool interlock_alarm = false;
                        interlock_alarm |= GV.MxpHeartBitInterlock;
                        interlock_alarm |= GV.RouteChangeTimeOverInterlock;
                        interlock_alarm |= GV.BCRNotChangedInterlock;
                        interlock_alarm |= GV.OverrideAbnormalStopInterlock;
                        interlock_alarm |= GV.CpsLowVoltageInterlock;
                        interlock_alarm |= GV.EtherCatDisconnectError;
                        interlock_alarm |= GV.TouchHeartBitInterlock;

                        if (interlock_alarm) // Interlock Alarm이 해결이 않되고 있다...! Alarm 띄우고 기다리자 ~~~
                        {
                            string msg = "";
                            if (GV.MxpHeartBitInterlock) { msg += " | MxpHeartBitInterlock"; AlarmId = m_ALM_MxpHeartBitError.ID; }
                            if (GV.RouteChangeTimeOverInterlock) { msg += " | RouteChangeTimeOverInterlock"; AlarmId = m_ALM_RouteChangeTimeoverError.ID; }
                            if (GV.BCRNotChangedInterlock) { msg += " | BCRNotChangedInterlock"; AlarmId = m_ALM_BcrNotChangedError.ID; }
                            if (GV.OverrideAbnormalStopInterlock) { msg += " | OverrideAbnormalStopInterlock"; AlarmId = m_MasterAxis.ALM_OverrideAbnormalStop.ID; }
                            if (GV.CpsLowVoltageInterlock) { msg += " | CpsLowVoltageInterlock"; AlarmId = m_ALM_AbnormalCpsLowVoltageError.ID; }
                            if (GV.EtherCatDisconnectError) { msg += " | EtherCatDisconnectError"; AlarmId = m_ALM_EthcatDisconnectedError.ID; }
                            if (GV.TouchHeartBitInterlock) { msg += " | TouchHeartBitInterlock"; AlarmId = m_ALM_TouchHeartBitError.ID; }

                            SequenceLog.WriteLog(FuncName, string.Format("Cann't Recovery Interlock Alarm {0}", msg));                            
                            GV.RecoveryProcess = false; //이때는 Manual이 되어야지

                            EqpAlarm.Set(AlarmId);
                            seqNo = 1000;
                        }
                        else
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Interlock All Release !"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 60;
                        }
                    }
                    break;

                case 60:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        // 도착 위치와 충돌 위치간 비교
                        // 도착 위치 < 충돌 위치 - StopLimit => Override 적용할 필요 없다.
                        enFrontDetectState currentState = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.ObsUpperSensorState;
                        double collision_distance = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.CollisionDistance;
                        bool start = currentState < enFrontDetectState.enDeccelation5;
                        //start &= collision_distance > 4000.0f;
                        double FrontDetectMidCollision = m_DevSos.GetTableAreaMaxDistance() / 2.0f; //최대의 반정도 Distance (ex.10m->5m, 2m->1m)
                        start &= collision_distance > FrontDetectMidCollision;
                        start &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.IsCorner() == false;
                        if (start)
                        {
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Start);
                            GV.RecoveryProcess = false; //초기화가 끝나고 진행해야된다.
                            
                            SequenceLog.WriteLog(FuncName, "Run Mode Start");
                            seqNo = 0;
                        }
                    }
                    break;

                case 1000:
                    if (IsPushedSwitch.IsAlarmReset)
                    {
                        IsPushedSwitch.m_AlarmRstPushed = false;
                        EqpAlarm.Reset(AlarmId);
                        SequenceLog.WriteLog(FuncName, string.Format("Reset Alarm : Code[{0}]", AlarmId));
                        AlarmId = 0;
                        seqNo = ReturnSeqNo;
                    }
                    break;

            }
            this.SeqNo = seqNo;

            return -1;
        }
    }

    // CPS Votalge Monitor를 하여 LowVoltage가 되면 Interlock을 걸자 
    // OutputVoltage < SetupManage.Instance.SetupSafty.CpsVoltageLow
    // 유지 시간 : SetupManage.Instance.SetupSafty.CpsVoltageKeepTime
    public class SeqCpsVoltageLowCheck : XSeqFunc
    {
        public static readonly string FuncName = "[SeqCpsVoltageLowCheck]";
        #region Fields
        private List<_DevAxis> m_DevAxes = new List<_DevAxis>();
        private List<bool> m_StopComp = new List<bool>();
        private List<bool> m_ServoReady = new List<bool>();
        private DevCPS m_devCPS = new DevCPS();
        #endregion

        #region Constructor
        public SeqCpsVoltageLowCheck()
        {
            this.SeqName = $"SeqCpsVoltageLowCheck";

            m_devCPS = DevicesManager.Instance.DevCPS;
            for (int i = 0; i < ServoControlManager.Instance.DevServoUnits.Count; i++)
            {
                m_DevAxes.AddRange(ServoControlManager.Instance.DevServoUnits[i].DevAxes);
                m_StopComp.Add(false);
                m_ServoReady.Add(false);
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
            GV.CpsLowVoltageInterlock = false;
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
                        bool run = EqpStateManager.Instance.OpMode == OperateMode.Auto;
                        run &= EqpStateManager.Instance.RunMode == EqpRunMode.Start;
                        run &= GV.WheelBusy;
                        run &= SetupManager.Instance.SetupSafty.CpsVoltageCheckUse == Use.Use;
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if (GV.TEST_INTERLOCK[0]) { run &= GV.TEST_INTERLOCK[0]; GV.TEST_INTERLOCK[0] = false; }
                        }
                        else
                        {
                            run &= m_devCPS.IsValid && m_devCPS.IsMonitoring();
                        }
                        if (run)
                        {
                            int currentVoltage = m_devCPS.OutputVoltage;
                            int lowVoltage = SetupManager.Instance.SetupSafty.CpsVoltageLow;
                            if (currentVoltage < lowVoltage)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("{0} CPS Voltage Low Detect, OutputVoltage={1}, BoostVoltage={2}", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath, m_devCPS.OutputVoltage, m_devCPS.BoostVoltage));
                                for (int i = 0; i < m_ServoReady.Count; i++) m_ServoReady[i] = false;
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                        }
                    }
                    break;

                case 10:
                    {
                        bool ready = true;
                        for (int i = 0; i < m_ServoReady.Count; i++)
                        {
                            if (!m_ServoReady[i])
                            {
                                bool rv1 = m_DevAxes[i].IsAxisReady(false, true);
                                if (rv1) m_ServoReady[i] = true;
                            }
                            ready &= m_ServoReady[i];
                        }
                        bool voltage_normal = false;
                        int currentVoltage = m_devCPS.OutputVoltage;
                        int lowVoltage = SetupManager.Instance.SetupSafty.CpsVoltageLow;
                        voltage_normal = currentVoltage > lowVoltage;

                        if (voltage_normal)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("{0} CPS Voltage Recovery, OutputVoltage={1}, BoostVoltage={2}", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath, m_devCPS.OutputVoltage, m_devCPS.BoostVoltage));
                            seqNo = 0;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupSafty.CpsVoltageKeepTime && ready)
                        {
                            // Alarm 발생 ... Servo OFF
                            SequenceLog.WriteLog(FuncName, string.Format("{0} CPS Voltage Check Timeover, OutputVoltage={1}, BoostVoltage={2}", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath, m_devCPS.OutputVoltage, m_devCPS.BoostVoltage));
                            //AlarmId = m_ALM_AbnormalCpsLowVoltage.ID;
                            //EqpAlarm.Set(AlarmId);

                            GV.InterlockServoStop = true; // 미리 Stop을 한다는 것을 AbortSequence에게 알려 주자~~. 그래야 중복 Stop을 하지 않는다.
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort); // Servo Stop을 내리기전에 Abort하여 Sequence를 멈추는게 좋겠다.
                            SequenceLog.WriteLog(FuncName, "Abort Sequence Start");

                            for (int i = 0; i < m_StopComp.Count; i++) m_StopComp[i] = false;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                    }
                    break;

                case 20:
                    {
                        // Servo Motor All Stop
                        bool complete = true;
                        for (int i = 0; i < m_StopComp.Count; i++)
                        {
                            if (!m_StopComp[i])
                            {
                                int rv1 = m_DevAxes[i].Stop();
                                if (rv1 >= 0) m_StopComp[i] = true;
                            }
                            complete &= m_StopComp[i];
                        }

                        if (complete)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("{0} CPS Voltage Interlock, All Servo Stop, OutputVoltage={1}, BoostVoltage={2}", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath, m_devCPS.OutputVoltage, m_devCPS.BoostVoltage));

                            GV.CpsLowVoltageInterlock = true;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("{0} CPS Voltage Interlock, All Servo Stop [Timeover], OutputVoltage={1}, BoostVoltage={2}", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath, m_devCPS.OutputVoltage, m_devCPS.BoostVoltage));

                            GV.CpsLowVoltageInterlock = true;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                    }
                    break;

                case 30:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        int lowVoltage = SetupManager.Instance.SetupSafty.CpsVoltageLow;
                        int currentVoltage = m_devCPS.OutputVoltage;
                        bool voltage_normal = SetupManager.Instance.SetupSafty.CpsVoltageCheckUse == Use.NoUse;
                        voltage_normal |= currentVoltage > lowVoltage;
                        if (voltage_normal)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("{0} CPS Voltage Alarm Reset, OutputVoltage={1}, BoostVoltage={2}", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath, m_devCPS.OutputVoltage, m_devCPS.BoostVoltage));
                            GV.CpsLowVoltageInterlock = false;
                            seqNo = 0;
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

    public class SeqRouteChangedCheck : XSeqFunc
    {
        public static readonly string FuncName = "[SeqRouteChangedCheck]";
        #region Fields
        private List<_DevAxis> m_DevAxes = new List<_DevAxis>();
        private List<bool> m_StopComp = new List<bool>();
        private bool m_Stopping = false;
        private Data.Process.Path m_MyPath = new Data.Process.Path();
        #endregion

        #region Constructor
        public SeqRouteChangedCheck()
        {
            this.SeqName = $"SeqRouteChangedCheck";

            for (int i = 0; i < ServoControlManager.Instance.DevServoUnits.Count; i++)
            {
                m_DevAxes.AddRange(ServoControlManager.Instance.DevServoUnits[i].DevAxes);
                m_StopComp.Add(false);
            }

            //EventHandlerManager.Instance.LinkChanged += Instance_LinkChanged;
        }
        private void Instance_LinkChanged(object obj)
        {
            // 처음부터 다시
            if (m_Stopping == false)
                this.SeqNo = 0; // Link가 바뀔때마다 처음부터 다시 시작은 해야 겠는데...
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
            GV.RouteChangeTimeOverCheck = false;
            GV.RouteChangeTimeOverInterlock = false;
            m_Stopping = false;
        }
        public override int Do()
        {
            if (m_MyPath.LinkID != ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LinkID ||
                    m_MyPath.ToLinkID != ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.ToLinkID)
            {
                m_MyPath = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath;
                if (m_Stopping == false) this.SeqNo = 0; //처음부터 다시..... Link가 바뀔때마다 처음부터 다시 시작은 해야 겠는데...
            }

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        bool run = EqpStateManager.Instance.OpMode == OperateMode.Auto;
                        run &= EqpStateManager.Instance.RunMode == EqpRunMode.Start;
                        run &= GV.WheelBusy;
                        run &= GV.RouteChangeTimeOverCheck;
                        if (run)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("{0} Route Change Check Start", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));

                            m_Stopping = false;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 10;
                        }
                    }
                    break;

                case 10:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 200) break;

                        // 경로를 이탈했을 경우를 대비하여 Interlock을 걸어주자 ~~~
                        // Path의 Target Motor Position을 지났는데 Next Path를 찾지 못하는 경우 Alarm을 띄우자~~
                        double curPos = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().GetCurPosition();
                        double curPathTarget = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.MotorTargetPosition;
                        double range = 0.2f * ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.RunDistance;
                        if (range > 2000.0f) range = 2000.0f; // 2m 이상 지나가면 무조건 세우자 ~~
                        else if (range < 500.0f) range = 500.0f;// 0.5m 최소거리 설정.
                        double error = curPos - curPathTarget;

                        bool isCheckCondition = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.IsCorner() == false;
                        isCheckCondition &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Distance > 1000.0f;
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if (GV.TEST_INTERLOCK[1]) { range = 0; isCheckCondition = true; GV.TEST_INTERLOCK[1] = false; } // Alarm을 띄우자 ~~~
                        }

                        if (error > range && isCheckCondition)
                        {
                            double leftBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                            double rightBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                            string msg = $"Current Motor Position : {curPos}, Motor Target Position : {curPathTarget}";
                            SequenceLog.WriteLog(FuncName, $"Search Link Found Abnormal : CurrentPath={ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath} range={range},error={error}, BCR=({leftBCR},{rightBCR})\r\n{msg}");

                            SequenceLog.WriteLog(FuncName, string.Format("{0} Route Change Timeover Alarm Occur", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));

                            GV.InterlockServoStop = true; // 미리 Stop을 한다는 것을 AbortSequence에게 알려 주자~~. 그래야 중복 Stop을 하지 않는다.
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort); // Servo Stop을 내리기전에 Abort하여 Sequence를 멈추는게 좋겠다.
                            SequenceLog.WriteLog(FuncName, "Abort Sequence Start"); 

                            for (int i = 0; i < m_StopComp.Count; i++) m_StopComp[i] = false;
                            m_Stopping = true;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        else if (GV.RouteChangeTimeOverCheck == false)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("{0} Route Change Timeover Release", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));
                            seqNo = 0;
                        }
                    }
                    break;

                case 20:
                    {
                        // Servo Motor All Stop
                        bool complete = true;
                        for (int i = 0; i < m_StopComp.Count; i++)
                        {
                            if (!m_StopComp[i])
                            {
                                int rv1 = m_DevAxes[i].Stop();
                                if (rv1 >= 0) m_StopComp[i] = true;
                            }
                            complete &= m_StopComp[i];
                        }
                        if (complete)
                        {
                            GV.RouteChangeTimeOverInterlock = true;
                            SequenceLog.WriteLog(FuncName, string.Format("{0} Route Change Interlock, All Servo Stop", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                        {
                            GV.RouteChangeTimeOverInterlock = true;

                            SequenceLog.WriteLog(FuncName, string.Format("{0} Route Change Interlock, All Servo Stop [TimeOver]", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                    }
                    break;

                case 30:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        GV.RouteChangeTimeOverCheck = false;
                        GV.RouteChangeTimeOverInterlock = false;
                        SequenceLog.WriteLog(FuncName, string.Format("{0} Route Change Timeover Release", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));
                        m_Stopping = false;
                        seqNo = 0;
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;
                            GV.RouteChangeTimeOverInterlock = false;

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
    }

    public class SeqBcrNotChangedCheck : XSeqFunc
    {
        public static readonly string FuncName = "[SeqBcrNotChangedCheck]";
        #region Fields
        private List<_DevAxis> m_DevAxes = new List<_DevAxis>();
        private List<bool> m_StopComp = new List<bool>();
        private double m_OldLeftBcr = 0.0f;
        private double m_OldRightBcr = 0.0f;
        private double m_OldMotorPosition = 0.0f;
        #endregion

        #region Constructor
        public SeqBcrNotChangedCheck()
        {
            this.SeqName = $"SeqBcrNotChangedCheck";

            for (int i = 0; i < ServoControlManager.Instance.DevServoUnits.Count; i++)
            {
                m_DevAxes.AddRange(ServoControlManager.Instance.DevServoUnits[i].DevAxes);
                m_StopComp.Add(false);
            }
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
            GV.BCRNotChangedInterlock = false;
        }
        public override int Do()
        {
            bool run = EqpStateManager.Instance.OpMode == OperateMode.Auto;
            run &= EqpStateManager.Instance.RunMode == EqpRunMode.Start;
            run &= GV.WheelBusy;
            bool bcr_changed_check = false;
            if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Certain)
            {
                double curLinkPosition = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.CurrentPositionOfLink;
                double linkDistance = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Distance;
                bcr_changed_check = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type == LinkType.Straight;
                bcr_changed_check &= linkDistance > 500.0f;
                bcr_changed_check &= curLinkPosition > 0.1f * linkDistance;
                bcr_changed_check &= curLinkPosition < 0.9f * linkDistance;
            }

            int rv1 = -1;
            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ///BCR 이동 상태를 확인하여 변하지 않는 경우에는 멈추도록 하자. 단 직선구간에서만 적용하자
                        ///속도에 따른 이동거리 계산하여 interlock을 걸어야 한다.
                        if (run && bcr_changed_check)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("{0} BCR Not Changed Check Start", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));
                            seqNo = 10;
                        }
                    }
                    break;

                case 10:
                    {
                        m_OldLeftBcr = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                        m_OldRightBcr = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                        m_OldMotorPosition = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().GetCurPosition();
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 20;
                    }
                    break;

                case 20:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 200) break;

                        double curVelocity = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().GetCommandVelocity();
                        bcr_changed_check &= curVelocity > 300.0f;

                        if (run && bcr_changed_check)
                        {
                            double leftBcr = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                            double rightBcr = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                            double motorPosition = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().GetCurPosition();

                            double diffTime = (XFunc.GetTickCount() - StartTicks) / 1000.0f;
                            double changedLength = 0.2f * (motorPosition - m_OldMotorPosition);
                            double changedBcr = Math.Max(leftBcr - m_OldLeftBcr, rightBcr - m_OldRightBcr);
                            bool ng = true;
                            ng &= Math.Abs(changedBcr) < Math.Abs(changedLength);
                            ng &= changedBcr > 0;
                            ng &= changedLength > 0;
                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                if (GV.TEST_INTERLOCK[2]) { ng = GV.TEST_INTERLOCK[2]; GV.TEST_INTERLOCK[2] = false; } // Alarm을 띄우자 ~~~
                            }

                            if (ng)
                            {
                                string msg = $"leftBcr=({leftBcr},{m_OldLeftBcr}), rightBcr=({rightBcr},{m_OldRightBcr}), motorPosition=({motorPosition},{m_OldMotorPosition}), diffTime={diffTime}, curVelocity={curVelocity}";
                                SequenceLog.WriteLog(FuncName, string.Format("BCR Not Changed Alarm Occur : {0}", msg));

                                GV.InterlockServoStop = true; // 미리 Stop을 한다는 것을 AbortSequence에게 알려 주자~~. 그래야 중복 Stop을 하지 않는다.
                                EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort); // Servo Stop을 내리기전에 Abort하여 Sequence를 멈추는게 좋겠다.
                                SequenceLog.WriteLog(FuncName, "Abort Sequence Start");

                                for (int i = 0; i < m_StopComp.Count; i++) m_StopComp[i] = false;
                                seqNo = 30;
                            }
                            else
                            {
                                seqNo = 10;
                            }
                        }
                        else if (curVelocity <= 300.0f)
                        {
                            seqNo = 10;
                        }
                        else
                        {
                            GV.BCRNotChangedInterlock = false;
                            SequenceLog.WriteLog(FuncName, string.Format("{0} BCR Not Changed Check End", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));
                            seqNo = 0;
                        }
                    }
                    break;

                case 30:
                    {
                        // Servo Motor All Stop
                        bool complete = true;
                        for (int i = 0; i < m_StopComp.Count; i++)
                        {
                            if (!m_StopComp[i])
                            {
                                rv1 = m_DevAxes[i].Stop();
                                if (rv1 >= 0) m_StopComp[i] = true;
                            }
                            complete &= m_StopComp[i];
                        }
                        if (complete)
                        {
                            GV.BCRNotChangedInterlock = true;
                            SequenceLog.WriteLog(FuncName, string.Format("BCR Not Changed Alarm, All Servo Stop"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 40;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                        {
                            GV.BCRNotChangedInterlock = true;
                            SequenceLog.WriteLog(FuncName, string.Format("BCR Not Changed Alarm, All Servo Stop [Timeover]"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 40;
                        }
                    }
                    break;

                case 40:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        GV.BCRNotChangedInterlock = false;
                        SequenceLog.WriteLog(FuncName, string.Format("{0} BCR Not Changed Release", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath));
                        seqNo = 0;
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;
                            GV.BCRNotChangedInterlock = false;

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
    }

    public class SeqMxpHeartBitCheck : XSeqFunc
    {
        public static readonly string FuncName = "[SeqMxpHeartBitCheck]";
        #region Fields
        private List<_DevAxis> m_DevAxes = new List<_DevAxis>();
        private List<bool> m_StopComp = new List<bool>();
        #endregion

        #region Constructor
        public SeqMxpHeartBitCheck()
        {
            this.SeqName = $"SeqMxpHeartBitCheck";

            for (int i = 0; i < ServoControlManager.Instance.DevServoUnits.Count; i++)
            {
                m_DevAxes.AddRange(ServoControlManager.Instance.DevServoUnits[i].DevAxes);
                m_StopComp.Add(false);
            }
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
            GV.MxpHeartBitInterlock = false;
        }
        public override int Do()
        {
            bool run = EqpStateManager.Instance.OpMode == OperateMode.Auto;
            run &= EqpStateManager.Instance.RunMode == EqpRunMode.Start;
            run &= GV.WheelBusy;

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        if (run)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("MXP Heart Bit Check Start"));
                            seqNo = 10;
                        }
                    }
                    break;

                case 10:
                    {
                        bool heartbit_ok = true;
                        bool heartbit_error = false;
                        foreach (_AxisBlock block in ServoManager.Instance.AxisBlocks)
                        {
                            if (block.ControlFamily == ServoControlFamily.MXP)
                            {
                                heartbit_ok &= (block as AxisBlockMXP).HeartBitOk;
                                heartbit_error |= (block as AxisBlockMXP).IsHeartBitError;
                            }
                        }
                        GV.MxpHeartBitNg = !heartbit_ok;
                        GV.MxpHeartBitError = heartbit_error;
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if (GV.TEST_INTERLOCK[3]) { GV.MxpHeartBitError = GV.TEST_INTERLOCK[3]; GV.TEST_INTERLOCK[3] = false; } // Alarm을 띄우자 ~~~
                            else { GV.MxpHeartBitNg = false; GV.MxpHeartBitError = false; }
                        }

                        if (GV.MxpHeartBitNg || GV.MxpHeartBitError)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Heart Bit Alarm Occur"));

                            GV.InterlockServoStop = true; // 미리 Stop을 한다는 것을 AbortSequence에게 알려 주자~~. 그래야 중복 Stop을 하지 않는다.
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort); // Servo Stop을 내리기전에 Abort하여 Sequence를 멈추는게 좋겠다.
                            SequenceLog.WriteLog(FuncName, "Abort Sequence Start");

                            for (int i = 0; i < m_StopComp.Count; i++) m_StopComp[i] = false;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        else if (!run)
                        {
                            GV.MxpHeartBitInterlock = false;
                            SequenceLog.WriteLog(FuncName, string.Format("MXP Heart Bit Check End"));
                            seqNo = 0;
                        }
                    }
                    break;

                case 20:
                    {
                        // Servo Motor All Stop
                        bool complete = true;
                        for (int i = 0; i < m_StopComp.Count; i++)
                        {
                            if (!m_StopComp[i])
                            {
                                int rv1 = m_DevAxes[i].Stop();
                                if (rv1 >= 0) m_StopComp[i] = true;
                            }
                            complete &= m_StopComp[i];
                        }
                        if (complete)
                        {
                            GV.MxpHeartBitInterlock = true;
                            SequenceLog.WriteLog(FuncName, string.Format("Heart Bit Alarm Occur, All Servo Stop"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                        {
                            GV.MxpHeartBitInterlock = true;
                            SequenceLog.WriteLog(FuncName, string.Format("Heart Bit Alarm Occur, All Servo Stop [Timeover]"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                    }
                    break;

                case 30:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        bool heartbit_ok = true;
                        bool heartbit_error = false;
                        foreach (_AxisBlock block in ServoManager.Instance.AxisBlocks)
                        {
                            if (block.ControlFamily == ServoControlFamily.MXP)
                            {
                                heartbit_ok &= (block as AxisBlockMXP).HeartBitOk;
                                heartbit_error |= (block as AxisBlockMXP).IsHeartBitError;
                            }
                        }
                        GV.MxpHeartBitNg = !heartbit_ok;
                        GV.MxpHeartBitError = heartbit_error;
                        if (!GV.MxpHeartBitNg && !GV.MxpHeartBitError)
                        {
                            GV.MxpHeartBitInterlock = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Heart Bit Alarm Release"));
                            seqNo = 0;
                        }
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;
                            GV.MxpHeartBitInterlock = false;

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
    }
    public class SeqMxpEtherCatConnectionCheck : XSeqFunc
    {
        public static readonly string FuncName = "[SeqMxpEtherCatConnectionCheck]";
        #region Fields
        private List<_DevAxis> m_DevAxes = new List<_DevAxis>();
        private List<bool> m_StopComp = new List<bool>();
        private AlarmData m_ALM_EthcatDisconnectedError = null;
        #endregion

        #region Constructor
        public SeqMxpEtherCatConnectionCheck()
        {
            this.SeqName = $"SeqMxpEtherCatConnectionCheck";

            for (int i = 0; i < ServoControlManager.Instance.DevServoUnits.Count; i++)
            {
                m_DevAxes.AddRange(ServoControlManager.Instance.DevServoUnits[i].DevAxes);
                m_StopComp.Add(false);
            }

            m_ALM_EthcatDisconnectedError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "INTERLOCK", "Auto Recovery", "EtherCat Disconnected Alarm");
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
            GV.EtherCatDisconnectError = false;
        }
        public override int Do()
        {
            bool run = EqpStateManager.Instance.OpMode == OperateMode.Auto;
            run &= EqpStateManager.Instance.RunMode == EqpRunMode.Start;

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        if (run)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("EtherCat Link Error Check Start"));
                            seqNo = 10;
                        }
                    }
                    break;

                case 10:
                    {
                        bool ethercat_link_error = false;
                        foreach (_AxisBlock block in ServoManager.Instance.AxisBlocks)
                        {
                            if (block.ControlFamily == ServoControlFamily.MXP)
                            {
                                ethercat_link_error |= (block as AxisBlockMXP).Connected ? false : true;
                            }
                        }
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if (GV.TEST_INTERLOCK[4]) { ethercat_link_error = GV.TEST_INTERLOCK[4]; GV.TEST_INTERLOCK[4] = false; } // Alarm을 띄우자 ~~~
                            else { ethercat_link_error = false; }
                        }

                        if (ethercat_link_error)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("EtherCat Link Error Occur"));

                            GV.InterlockServoStop = true; // 미리 Stop을 한다는 것을 AbortSequence에게 알려 주자~~. 그래야 중복 Stop을 하지 않는다.
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort); // Servo Stop을 내리기전에 Abort하여 Sequence를 멈추는게 좋겠다.
                            SequenceLog.WriteLog(FuncName, "Abort Sequence Start");

                            for (int i = 0; i < m_StopComp.Count; i++) m_StopComp[i] = false;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        else if (!run)
                        {
                            GV.EtherCatDisconnectError = false;
                            SequenceLog.WriteLog(FuncName, string.Format("EtherCat Link Error Check End"));
                            seqNo = 0;
                        }
                    }
                    break;

                case 20:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;
                        // Servo Motor All Stop
                        bool complete = true;
                        // EtherCat Error 발생한 경우에는 이미 Servo 가 OFF 되어 있을 것인데....
                        //for (int i = 0; i < m_StopComp.Count; i++)
                        //{
                        //    if (!m_StopComp[i])
                        //    {
                        //        int rv1 = m_DevAxes[i].Stop();
                        //        if (rv1 >= 0) m_StopComp[i] = true;
                        //    }
                        //    complete &= m_StopComp[i];
                        //}
                        if (complete)
                        {
                            GV.EtherCatDisconnectError = true;
                            SequenceLog.WriteLog(FuncName, string.Format("EtherCat Link Error Occur, All Servo Stop"));

                            if (AppConfig.Instance.ProjectType == enProjectType.ESWIN)
                            {
                                AlarmId = m_ALM_EthcatDisconnectedError.ID;
                                EqpAlarm.Set(AlarmId);
                            }

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                        {
                            GV.EtherCatDisconnectError = true;
                            SequenceLog.WriteLog(FuncName, string.Format("EtherCat Link Error Occur, All Servo Stop [Timeover]"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                    }
                    break;

                case 30:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        bool ethercat_link_error = false;
                        foreach (_AxisBlock block in ServoManager.Instance.AxisBlocks)
                        {
                            if (block.ControlFamily == ServoControlFamily.MXP)
                            {
                                ethercat_link_error |= (block as AxisBlockMXP).Connected ? false : true;
                            }
                        }

                        if (!ethercat_link_error)
                        {
                            GV.EtherCatDisconnectError = false;
                            SequenceLog.WriteLog(FuncName, string.Format("EtherCat Link Error Release"));
                            if (AppConfig.Instance.ProjectType == enProjectType.ESWIN)
                            {
                                EqpAlarm.Reset(AlarmId);
                            }
                            seqNo = 0;
                        }
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;
                            GV.EtherCatDisconnectError = false;

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
    }

    public class SeqOverrideAbnormalStopCheck : XSeqFunc
    {
        public static readonly string FuncName = "[SeqOverrideAbnormalStopCheck]";
        #region Fields
        private _DevAxis m_MasterAxis = null;
        #endregion

        #region Constructor
        public SeqOverrideAbnormalStopCheck()
        {
            this.SeqName = $"SeqOverrideAbnormalStopCheck";

            m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();
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
            GV.OverrideAbnormalStopInterlock = false;
        }
        public override int Do()
        {
            bool run = EqpStateManager.Instance.OpMode == OperateMode.Auto;
            run &= EqpStateManager.Instance.RunMode == EqpRunMode.Start;
            run &= GV.WheelBusy;

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        // Auto-Start 진행 중
                        // TransferCommand가 있을때
                        if (run)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Override Abnormal Stop Check Start"));
                            seqNo = 10;
                        }
                    }
                    break;

                case 10:
                    {
                        enAxisInFlag state = m_MasterAxis.GetAxis().AxisStatus; // (m_MasterAxis.GetAxis() as IAxisCommand).GetAxisCurStatus();
                        bool override_stop = state.HasFlag(enAxisInFlag.OverrideAbnormalStop);
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if (GV.TEST_INTERLOCK[5]) { override_stop = GV.TEST_INTERLOCK[5]; GV.TEST_INTERLOCK[5] = false; } // Alarm을 띄우자 ~~~
                            else { override_stop = false; }
                        }

                        if (override_stop)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Override Abnormal Stop Alarm"));

                            GV.InterlockServoStop = true; // 미리 Stop을 한다는 것을 AbortSequence에게 알려 주자~~. 그래야 중복 Stop을 하지 않는다.
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort); // Servo Stop을 내리기전에 Abort하여 Sequence를 멈추는게 좋겠다.
                            SequenceLog.WriteLog(FuncName, "Abort Sequence Start");

                            seqNo = 20;
                        }
                        else if (!run)
                        {
                            GV.OverrideAbnormalStopInterlock = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Override Abnormal Stop Check End"));
                            seqNo = 0;
                        }
                    }
                    break;

                case 20:
                    {
                        enAxisInFlag state = m_MasterAxis.GetAxis().AxisStatus; //(m_MasterAxis.GetAxis() as IAxisCommand).GetAxisCurStatus();
                        bool master_stop = true;
                        master_stop &= (state & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
                        master_stop &= (state & enAxisInFlag.InPos)== enAxisInFlag.InPos ? true : false;
                        if (master_stop)
                        {
                            GV.OverrideAbnormalStopInterlock = true;
                            SequenceLog.WriteLog(FuncName, string.Format("Override Abnormal Master Axis Stop Check"));
                            //AlarmId = m_MasterAxis.ALM_OverrideAbnormalStop.ID;
                            //EqpAlarm.Set(AlarmId);

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                    }
                    break;

                case 30:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        enAxisInFlag state = m_MasterAxis.GetAxis().AxisStatus; //(m_MasterAxis.GetAxis() as IAxisCommand).GetAxisCurStatus();
                        bool override_stop = state.HasFlag(enAxisInFlag.OverrideAbnormalStop);

                        if (!override_stop)
                        {
                            GV.OverrideAbnormalStopInterlock = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Override Abnormal Release"));
                            seqNo = 0;
                        }
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;
                            GV.OverrideAbnormalStopInterlock = false;

                            EqpAlarm.Reset(AlarmId);
                            SequenceLog.WriteLog(FuncName, string.Format("Reset Alarm : Code[{0}]", AlarmId));
                            AlarmId = 0;
                            seqNo = 0;
                        }
                    }
                    break;
            }

            this.SeqNo = seqNo;
            return -1;
        }
    }
    public class SeqFoupCoverNotDetectCheck : XSeqFunc
    {
        public static readonly string FuncName = "[SeqFoupCoverNotDetectCheck]";
        #region Fields
        private _DevAxis m_MasterAxis = null;
        private DevFoupGripper m_devFoupGripper = null;
        private List<_DevAxis> m_DevAxes = new List<_DevAxis>();
        private List<bool> m_StopComp = new List<bool>();
        #endregion

        #region Constructor
        public SeqFoupCoverNotDetectCheck()
        {
            this.SeqName = $"SeqFoupCoverNotDetectCheck";

            m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();
            m_devFoupGripper = DevicesManager.Instance.DevFoupGripper;

            for (int i = 0; i < ServoControlManager.Instance.DevServoUnits.Count; i++)
            {
                m_DevAxes.AddRange(ServoControlManager.Instance.DevServoUnits[i].DevAxes);
                m_StopComp.Add(false);
            }
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
            int seqNo = this.SeqNo;

            if (!m_devFoupGripper.Initialized) return -1;
            if (!m_devFoupGripper.DiFoupOpenCheck.IsValid) return -1;

            bool run = EqpStateManager.Instance.OpMode == OperateMode.Auto;
            run &= EqpStateManager.Instance.RunMode == EqpRunMode.Start;
            run &= SetupManager.Instance.SetupSafty.FoupCoverOpenCheckUse == Use.Use;
            run &= ProcessDataHandler.Instance.CurTransferCommand.IsValid;
            run &= ProcessDataHandler.Instance.CurVehicleStatus.GetVehicleStatus() != VehicleState.Acquiring;
            run &= ProcessDataHandler.Instance.CurVehicleStatus.GetVehicleStatus() != VehicleState.Depositing;
            run &= DevicesManager.Instance.DevGripperPIO.IsProductExist();
            run &= GV.WheelBusy;

            switch (seqNo)
            {
                case 0:
                    {
                        // Auto-Start 진행 중
                        if (run)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Cover Open Interlock Check Start !"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 10;
                        }
                    }
                    break;

                case 10:
                    {
                        // TransferCommand가 있을때
                        // Foup이 있을 때
                        // 움직이고 있을 때
                        bool foup_open = m_devFoupGripper.IsFoupCoverOpen();
                        if (foup_open)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Cover Open !! Start Timer"));

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        else if (!run)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Cover Open Interlock Check Stop !"));
                            seqNo = 0;
                        }
                    }
                    break;

                case 20:
                    {
                        bool foup_open = m_devFoupGripper.IsFoupCoverOpen();

                        if (!run || !foup_open)
                        {
                            seqNo = 10;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupSafty.CoverNotDetectCheckTime)
                        {
                            GV.InterlockServoStop = true; // 미리 Stop을 한다는 것을 AbortSequence에게 알려 주자~~. 그래야 중복 Stop을 하지 않는다.
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort); // Servo Stop을 내리기전에 Abort하여 Sequence를 멈추는게 좋겠다.
                            //Foup Cover가 없다.. 멈춰야지..
                            for (int i = 0; i < m_StopComp.Count; i++) m_StopComp[i] = false;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                    }
                    break;

                case 30:
                    {
                        bool complete = true;
                        for (int i = 0; i < m_StopComp.Count; i++)
                        {
                            if (!m_StopComp[i])
                            {
                                int rv1 = m_DevAxes[i].Stop();
                                if (rv1 >= 0) m_StopComp[i] = true;
                            }
                            complete &= m_StopComp[i];
                        }

                        if (complete)
                        {
                            for (int i = 0; i < m_StopComp.Count; i++) m_StopComp[i] = false;

                            SequenceLog.WriteLog(FuncName, "Foup Cover Open Not Detect. Emergency Stop!");
                            AlarmId = m_devFoupGripper.ALM_FoupCapOpenError.ID;
                            EqpAlarm.Set(AlarmId);
                            seqNo = 1000;
                        }
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Cover Open Not Detect Alarm Reset"));
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
    }
    public class SeqTouchHeartBitCheck : XSeqFunc
    {
        public static readonly string FuncName = "[SeqTouchHeartBitCheck]";
        #region Fields
        private List<_DevAxis> m_DevAxes = new List<_DevAxis>();
        private List<bool> m_StopComp = new List<bool>();
        #endregion

        #region Constructor
        public SeqTouchHeartBitCheck()
        {
            this.SeqName = $"SeqTouchHeartBitCheck";

            for (int i = 0; i < ServoControlManager.Instance.DevServoUnits.Count; i++)
            {
                m_DevAxes.AddRange(ServoControlManager.Instance.DevServoUnits[i].DevAxes);
                m_StopComp.Add(false);
            }
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
            GV.TouchHeartBitInterlock = false;
        }
        public override int Do()
        {
            bool run = EqpStateManager.Instance.OpMode == OperateMode.Auto;
            run &= EqpStateManager.Instance.RunMode == EqpRunMode.Start;
            run &= GV.WheelBusy;

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        if (run)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("TouchGUI Heart Bit Check Start"));
                            seqNo = 10;
                        }
                        else
                        {
                            GV.TouchHeartBitError = false;
                        }
                    }
                    break;

                case 10:
                    {
                        bool heartbit_error = GV.TouchHeartBitError;
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if (GV.TEST_INTERLOCK[6]) { heartbit_error |= GV.TEST_INTERLOCK[6]; GV.TEST_INTERLOCK[6] = false; } // Alarm을 띄우자 ~~~
                        }

                        if (heartbit_error)
                        {
                            GV.TouchHeartBitError = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Heart Bit Alarm Occur"));

                            GV.InterlockServoStop = true; // 미리 Stop을 한다는 것을 AbortSequence에게 알려 주자~~. 그래야 중복 Stop을 하지 않는다.
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort); // Servo Stop을 내리기전에 Abort하여 Sequence를 멈추는게 좋겠다.
                            SequenceLog.WriteLog(FuncName, "Abort Sequence Start");

                            for (int i = 0; i < m_StopComp.Count; i++) m_StopComp[i] = false;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        else if (!run)
                        {
                            GV.TouchHeartBitError = false;
                            GV.TouchHeartBitInterlock = false;
                            SequenceLog.WriteLog(FuncName, string.Format("TouchGUI Heart Bit Check End"));
                            seqNo = 0;
                        }
                    }
                    break;

                case 20:
                    {
                        // Servo Motor All Stop
                        bool complete = true;
                        for (int i = 0; i < m_StopComp.Count; i++)
                        {
                            if (!m_StopComp[i])
                            {
                                int rv1 = m_DevAxes[i].Stop();
                                if (rv1 >= 0) m_StopComp[i] = true;
                            }
                            complete &= m_StopComp[i];
                        }
                        if (complete)
                        {
                            GV.TouchHeartBitInterlock = true;
                            SequenceLog.WriteLog(FuncName, string.Format("Heart Bit Alarm Occur, All Servo Stop"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                        {
                            GV.TouchHeartBitInterlock = true;
                            SequenceLog.WriteLog(FuncName, string.Format("Heart Bit Alarm Occur, All Servo Stop [Timeover]"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                    }
                    break;

                case 30:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        if (!GV.TouchHeartBitError)
                        {
                            GV.TouchHeartBitInterlock = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Heart Bit Alarm Release"));
                            seqNo = 0;
                        }
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;
                            GV.TouchHeartBitInterlock = false;

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
    }

}
