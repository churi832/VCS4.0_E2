using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data;
using Sineva.VHL.Library;
using Sineva.VHL.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Library.Servo;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.Device.Assem;
using System.Diagnostics;
using System.IO;
using static Sineva.VHL.Task.TaskOCS;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.IF.JCS;
using Sineva.VHL.IF.OCS;
using Sineva.VHL.Library.IO;

namespace Sineva.VHL.Task
{
    public class TaskCommon : XSequence
    {
        public static readonly TaskCommon Instance = new TaskCommon();

        public TaskCommon()
        {
            this.RegSeq(new SeqEqpState());
            this.RegSeq(new SeqAbortProcess());
            this.RegSeq(new SeqSaveCurState());

            this.RegSeq(new SeqModeMonitor());
            this.RegSeq(new SeqAbortMonitor());

            this.RegSeq(new SeqDataCollection());
            this.RegSeq(new SeqUserLoginMonitor());
        }
        public class SeqModeMonitor : XSeqFunc
        {
            private const string FuncName = "[SeqModeMonitor]";
            #region Fields
            private EqpStateManager m_EqpManager = null;
            #endregion

            #region Constructor
            public SeqModeMonitor()
            {
                this.SeqName = "SeqModeMonitor";
                m_EqpManager = EqpStateManager.Instance;
            }
            public override void SeqAbort()
            {
                this.InitSeq();
            }
            #endregion

            #region Methods override
            public override int Do()
            {
                int rv = -1;
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        if (EqpStateManager.Instance.OpMode == OperateMode.Auto)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("MODE Change (=> Auto)"));
                            seqNo = 10;
                        }
                        break;

                    case 10:
                        {
                            if (EqpStateManager.Instance.OpMode == OperateMode.Manual ||
                                EqpStateManager.Instance.OpMode == OperateMode.SemiAuto)
                            {
                                if (EqpStateManager.Instance.OpMode == OperateMode.Manual)
                                    SequenceLog.WriteLog(FuncName, string.Format("MODE Change (Auto => Manual) : Reset SeqNo"));
                                else if (EqpStateManager.Instance.OpMode == OperateMode.SemiAuto)
                                    SequenceLog.WriteLog(FuncName, string.Format("MODE Change (Auto => SemiAuto) : Reset SeqNo"));

                                List<XSeqFunc> funcs = TaskHandler.Instance.GetFuncLists();
                                foreach (XSeqFunc func in funcs)
                                {
                                    if (func.SeqName == "SeqModeMonitor" || func.SeqName == "SeqAbortMonitor") continue;
                                    func.SeqAbort();
                                }
                                SequenceLog.WriteLog(FuncName, string.Format("Reset SeqNo"));

                                GV.scmAbortSeqRun.Start = true;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 1000:
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;

                            SequenceLog.WriteLog(FuncName, string.Format("Alarm Reset"));
                            EqpAlarm.Reset(AlarmId);
                            AlarmId = 0;
                            seqNo = ReturnSeqNo;
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            #endregion
        }
        public class SeqAbortMonitor : XSeqFunc
        {
            private const string FuncName = "[SeqAbortMonitor]";
            #region Fields
            private EqpStateManager m_EqpManager = null;
            #endregion

            #region Constructor
            public SeqAbortMonitor()
            {
                this.SeqName = "SeqAbortMonitor";
                m_EqpManager = EqpStateManager.Instance;
            }
            public override void SeqAbort()
            {
                this.InitSeq();
            }
            #endregion

            #region Methods override
            public override int Do()
            {
                int rv = -1;
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        if (m_EqpManager.RunMode == EqpRunMode.Abort)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Abort Mode Start"));
                            List<XSeqFunc> funcs = TaskHandler.Instance.GetFuncLists();
                            foreach (XSeqFunc func in funcs)
                            {
                                if (func.SeqName == "SeqModeMonitor" || func.SeqName == "SeqAbortMonitor") continue;
                                if (func.SeqName == "SeqInterlockAutoRecovery" && GV.InterlockRecoveryStart) continue;
                                if (func.SeqName == "SeqCpsVoltageLowCheck" ||
                                    func.SeqName == "SeqRouteChangedCheck" ||
                                    func.SeqName == "SeqBcrNotChangedCheck" ||
                                    func.SeqName == "SeqMxpHeartBitCheck" ||
                                    func.SeqName == "SeqMxpEtherCatConnectionCheck" ||
                                    func.SeqName == "SeqOverrideAbnormalStopCheck" ||
                                    func.SeqName == "SeqTouchHeartBitCheck") continue;
                                func.SeqAbort();
                            }
                            GV.InterlockRecoveryStart = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Reset SeqNo"));

                            if (!GV.InterlockServoStop)
                                GV.scmAbortSeqRun.Start = true;

                            GV.InterlockServoStop = false;
                            seqNo = 10;
                        }
                        break;

                    case 10:
                        {
                            if (EqpStateManager.Instance.RunMode != EqpRunMode.Abort)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Abort Mode Release"));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 1000:
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;

                            SequenceLog.WriteLog(FuncName, string.Format("Alarm Reset"));
                            EqpAlarm.Reset(AlarmId);
                            AlarmId = 0;
                            seqNo = ReturnSeqNo;
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return rv;
            }
            #endregion
        }
        public class SeqEqpState : XSeqFunc
        {
            private const string FuncName = "[SeqEqpState]";

            #region Fields
            private bool IdleWait = false;
            XTimer m_Timer = new XTimer("SeqEqpStateTimer");
            #endregion

            #region Constructor
            public SeqEqpState()
            {
                this.SeqName = $"SeqEqpState";
            }
            #endregion

            #region Methods
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
                            // Command가 있으면 Run
                            bool command_run = ProcessDataHandler.Instance.CurTransferCommand.IsValid;
                            command_run &= ProcessDataHandler.Instance.CurTransferCommand.CommandStatus == ProcessStatus.Assigned ||
                                ProcessDataHandler.Instance.CurTransferCommand.CommandStatus == ProcessStatus.Processing;
                            bool heavy_alarm = AlarmCurrentProvider.Instance.IsHeavyAlarm();
                            if (ProcessDataHandler.Instance.CurTransferCommand.IsValid)
                            {
                                heavy_alarm &= ProcessDataHandler.Instance.CurTransferCommand.CommandStatus != ProcessStatus.Canceling;
                                heavy_alarm &= ProcessDataHandler.Instance.CurTransferCommand.CommandStatus != ProcessStatus.Aborting;
                            }

                            if (heavy_alarm)
                            {
                                EqpStateManager.Instance.SetState(EqpState.Down);
                                if (EqpStateManager.Instance.RunMode != EqpRunMode.Abort &&
                                    EqpStateManager.Instance.RunMode != EqpRunMode.Stop)
                                {
                                    if (EqpStateManager.Instance.OpMode == OperateMode.Auto && EqpStateManager.Instance.RunMode == EqpRunMode.Start)
                                        EqpStateManager.Instance.EqpAutoStartChangeToStop = true; // Auto/Stop일때 Start 상태로 바꿀지 말지 판단.

                                    SequenceLog.WriteLog(FuncName, string.Format("Heavy Alarm Occur => Stop"));
                                    EqpStateManager.Instance.SetRunMode(EqpRunMode.Stop);
                                }
                            }
                            else
                            {
                                if (EqpStateManager.Instance.OpMode == OperateMode.Recovery)
                                {
                                    EqpStateManager.Instance.SetState(EqpState.PM);
                                    EqpStateManager.Instance.SetRunMode(EqpRunMode.Stop);
                                }
                                else
                                {
                                    if (EqpStateManager.Instance.OpMode == OperateMode.Auto)
                                    {
                                        if (EqpStateManager.Instance.RunMode == EqpRunMode.Abort)
                                        {
                                            EqpStateManager.Instance.SetState(EqpState.Stop);
                                        }
                                        else if (EqpStateManager.Instance.RunMode == EqpRunMode.Stop)
                                        {
                                            EqpStateManager.Instance.SetState(EqpState.Stop);
                                        }
                                        else if (EqpStateManager.Instance.RunMode == EqpRunMode.Pause)
                                        {
                                            EqpStateManager.Instance.SetState(EqpState.Pause);
                                        }
                                        else if (!EqpStateManager.Instance.EqpInitComp)
                                        {
                                            EqpStateManager.Instance.SetState(EqpState.Stop);
                                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Stop);
                                            SequenceLog.WriteLog(FuncName, string.Format("Auto/InitComp(false) => Stop"));
                                        }
                                        else
                                        {
                                            if (command_run)
                                            {
                                                EqpStateManager.Instance.SetState(EqpState.Run);
                                            }
                                            else
                                            {
                                                if (EqpStateManager.Instance.State != EqpState.Idle)
                                                {
                                                    if (!IdleWait)
                                                    {
                                                        IdleWait = true;
                                                        StartTicks = XFunc.GetTickCount();
                                                    }
                                                    seqNo = 100;
                                                    break;
                                                }
                                            }
                                            if (EqpStateManager.Instance.EqpAutoStartChangeToStop)
                                                EqpStateManager.Instance.EqpAutoStartChangeToStop = false;
                                        }
                                    }
                                    else
                                    {
                                        // Manual 상태는 STOP
                                        EqpStateManager.Instance.SetState(EqpState.Stop);
                                        //EqpStateManager.Instance.SetRunMode(EqpRunMode.Stop);
                                    }
                                }
                            }
                            IdleWait = false;
                            m_Timer.Start(200);
                            seqNo = 10;
                        }
                        break;

                    case 10:
                        if (m_Timer.Over)
                        {
                            seqNo = 0;
                        }
                        break;

                    case 100:
                        {
                            int idle_delay_time = 5;
                            if (XFunc.GetTickCount() - StartTicks > idle_delay_time * 1000)
                            {
                                EqpStateManager.Instance.SetState(EqpState.Idle);
                                IdleWait = false;
                            }
                            seqNo = 0;
                        }
                        break;
                }


                this.SeqNo = seqNo;
                return -1;
            }
            #endregion
        }
        public class SeqSaveCurState : XSeqFunc
        {
            #region Fields
            private bool m_UpdateMotionProcess = false;
            private Data.Process.Path m_MyPath = new Data.Process.Path();

            private System.Diagnostics.Stopwatch m_StopWatch = new System.Diagnostics.Stopwatch();
            #endregion

            #region Constructor
            public SeqSaveCurState()
            {
                this.SeqName = $"SeqSaveCurState";

                //EventHandlerManager.Instance.LinkChanged += Instance_LinkChanged;
            }
            private void Instance_LinkChanged(object obj)
            {
                ProcessDataHandler.Instance._SaveCurState = true;
                m_UpdateMotionProcess = true;
            }
            #endregion

            #region Methods - Override
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                if (m_MyPath.LinkID != ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LinkID ||
                    m_MyPath.ToLinkID != ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.ToLinkID)
                {
                    m_MyPath = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath;
                    ProcessDataHandler.Instance.CurTransferCommand.UpdateMotionProcess(m_MyPath, true);
                    ProcessDataHandler.Instance._SaveCurState = true;
                }

                if (AppConfig.Instance.Simulation.MY_DEBUG)
                {
                    m_StopWatch.Reset();
                    m_StopWatch.Start();
                }

                int seqNo = SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            //if (ProcessDataHandler.Instance.IsSaveCurState())
                            //{
                            //    bool save_able = true;
                            //    save_able &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type == LinkType.Straight;
                            //    save_able &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Distance > 15 * 1000;
                            //    save_able &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.RemainDistanceOfLink > 5000;
                            //    save_able |= !GV.WheelBusy;
                            //    if (save_able)
                            //    {
                            //        ProcessDataHandler.Instance.Save();
                            //        StartTicks = XFunc.GetTickCount();
                            //        seqNo = 10;
                            //    }
                            //}
                            if (!GV.WheelBusy || !ProcessDataHandler.Instance.CurTransferCommand.IsValid)
                            {
                                if (ProcessDataHandler.Instance.IsSaveCurState())
                                {
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 10;
                                }
                            }
                            if (EqpStateManager.Instance.State == EqpState.Idle || EqpStateManager.Instance.OpMode == OperateMode.Manual)
                            {
                                if (DevicesManager.Instance.IsSave() && m_MyPath.Type != LinkType.MTL)
                                {
                                    DevicesManager.Instance.Save();
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            if (GetElapsedTicks() > 2000)
                            {
                                ProcessDataHandler.Instance.Save();
                                seqNo = 0;
                            }
                            //else if (GV.WheelBusy || ProcessDataHandler.Instance.CurTransferCommand.IsValid)
                            //{
                            //    seqNo = 0;
                            //}
                        }
                        break;
                }
                SeqNo = seqNo;

                //if (AppConfig.Instance.Simulation.MY_DEBUG)
                //{
                //    double watch = (double)m_StopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;
                //    string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                //    System.Diagnostics.Debug.WriteLine($"{time} : [SeqSaveCurState] [{watch}]");
                //}
                return -1;
            }
            #endregion
        }
        public class SeqAbortProcess : XSeqFunc
        {
            private const string FuncName = "[SeqAbortProcess]";

            #region Fields
            private OperateMode m_OldOpMode = OperateMode.Manual;
            private List<_DevAxis> m_DevAxes = new List<_DevAxis>();
            private List<bool> m_StopComp = new List<bool>();
            private DevEqPIO m_devEqPio = null;
            #endregion

            #region Constructor
            public SeqAbortProcess()
            {
                EventHandlerManager.Instance.RunModeChanged += Instance_RunModeChanged;
                this.SeqName = $"SeqAbortProcess";

                for (int i = 0; i < ServoControlManager.Instance.DevServoUnits.Count; i++)
                {
                    m_DevAxes.AddRange(ServoControlManager.Instance.DevServoUnits[i].DevAxes);
                    m_StopComp.Add(false);
                }
                m_devEqPio = DevicesManager.Instance.DevEqpPIO;
            }

            private void Instance_RunModeChanged(EqpRunMode runMode)
            {
                try
                {
                    if (runMode == EqpRunMode.Abort)
                    {
                        GV.RecoveryProcess = true;
                        this.InitSeq();
                    }
                    if (runMode == EqpRunMode.Start) GV.RecoveryProcess = false;
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
                GV.scmAbortSeqRun.Reset();
                this.SeqNo = 0;
            }

            public override int Do()
            {
                int rv1 = -1;
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            // 순서 : Start(true) -> Ing(true) -> End(true) -> Reset
                            if (GV.scmAbortSeqRun.Start)
                            {
                                GV.scmAbortSeqRun.Start = false;
                                GV.scmAbortSeqRun.Ing = true;

                                if (EqpStateManager.Instance.RunMode == EqpRunMode.Abort)
                                {
                                    string message = string.Format("Abort Sequence Running ... waiting !");

                                    if (ProcessDataHandler.Instance.CurTransferCommand.IsValid)
                                    {
                                        ProcessDataHandler.Instance.CurTransferCommand.IsNeedMakeRouteFullPath = true;

                                        if (ProcessDataHandler.Instance.CurTransferCommand.CommandStatus == ProcessStatus.Canceling ||
                                            ProcessDataHandler.Instance.CurTransferCommand.CommandStatus == ProcessStatus.Aborting)
                                            EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.SilencePopup, message);
                                        else EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.InterlockMessage, message);
                                    }
                                    else EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.InterlockMessage, message);
                                }
                                else if (EqpStateManager.Instance.OpMode != OperateMode.Auto)
                                {
                                    if (ProcessDataHandler.Instance.CurTransferCommand.IsValid)
                                        ProcessDataHandler.Instance.CurTransferCommand.IsNeedMakeRouteFullPath = true;
                                    string message = string.Format("Mode Change Sequence Running ... waiting !");
                                    EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.InterlockMessage, message);
                                }

                                DevicesManager.Instance.SeqAbort();
                                SequenceLog.WriteLog(FuncName, string.Format("All Devices Abort Call"));
                                for (int i = 0; i < m_StopComp.Count; i++) m_StopComp[i] = false;
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                        }
                        break;

                    case 10:
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
                                SequenceLog.WriteLog(FuncName, string.Format("All Motor Stop Completed"));
                                for (int i = 0; i < m_StopComp.Count; i++) m_StopComp[i] = false;
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 100) break; // 끈기전 delay가 필요하네...
                                                                                // PIO Comm을 닫고 마무리 하자...!
                            if (m_devEqPio.PioComm.IsGo())
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("PIO GO Signal ON Status. All signal OFF"));

                                m_devEqPio.ResetIfRecv();
                                m_devEqPio.ResetIfSend();
                                m_devEqPio.ResetPIO();
                                seqNo = 30;
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("PIO GO Signal OFF Status!"));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 40;
                            }
                        }
                        break;

                    case 30:
                        {
                            int id = 0;
                            int ch = 0;
                            rv1 = m_devEqPio.SetChannelId(id, ch);
                            if (rv1 == 0)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("EQP_PIO [{0}, {1}] Set Channel OK", id, ch));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 40;
                            }
                            else if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceLog.WriteLog(FuncName, string.Format("EQP_PIO [{0}, {1}] Set Channel Alarm - {0}", id, ch, EqpAlarm.GetAlarmMsg(AlarmId)));

                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 0;
                                seqNo = 1000;
                            }
                        }
                        break;

                    case 40:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 2000) break;
                            if (AlarmHandler.AutoRecoveryNeed)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Auto Recovery Need ! Initialize Request"));
                                if (EqpStateManager.Instance.EqpInitIng || EqpStateManager.Instance.EqpInitComp)
                                    EqpStateManager.Instance.ResetInitState(); //초기화 후 다시 진행해라....
                            }
                            EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.InterlockMessage, string.Empty);

                            SequenceLog.WriteLog(FuncName, string.Format("Abort Process Finished"));
                            GV.scmAbortSeqRun.End = true;
                            seqNo = 0;
                        }
                        break;

                    case 1000:
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;
                            GV.scmAbortSeqRun.Start = false;

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
            #endregion
        }
        public class SeqDataCollection : XSeqFunc
        {
            private const string FuncName = "[SeqDataCollection]";

            #region Fields
            private OCSCommManager m_OCSManager = null;
            private JCSCommManager m_JCSManager = null;
            private ProcessDataHandler m_ProcessDataHandler = null;
            private EqpStateManager m_EqpStateManager = null;
            private DevicesManager m_DeviceManager = null;
            #endregion

            #region Contructor
            public SeqDataCollection()
            {
                this.SeqName = $"SeqDataCollection";

                m_OCSManager = OCSCommManager.Instance;
                m_JCSManager = JCSCommManager.Instance;
                m_ProcessDataHandler = ProcessDataHandler.Instance;
                m_EqpStateManager = EqpStateManager.Instance;
                m_DeviceManager = DevicesManager.Instance;
            }
            #endregion

            #region XSeqFunction overrides
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
                            int cur_node_id = m_ProcessDataHandler.CurVehicleStatus.CurrentNode.NodeID;
                            int next_node_id = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.ToNodeID;
                            int cur_link_id = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.LinkID;
                            bool auto_run = m_EqpStateManager.EqpInitComp;
                            auto_run &= m_EqpStateManager.OpMode == OperateMode.Auto;
                            bool checkStatus = m_ProcessDataHandler.CurTransferCommand.GetProcessStatus() != ProcessStatus.RouteChanging;
                            checkStatus &= GV.RecoveryProcess == false;
                            if (checkStatus)
                            {
                                bool start = m_EqpStateManager.RunMode == EqpRunMode.Start;
                                //start |= m_EqpStateManager.RunMode == EqpRunMode.Abort; // Auto/Abort 처리되는 동안 Auto Mode가 끄짐방지
                                auto_run &= start;
                            }

                            bool inRail = true;
                            inRail &= cur_link_id > 0;
                            inRail &= cur_node_id > 0;
                            inRail &= m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.LeftBcr != 0 || m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.RightBcr != 0;//BCR이 들어오지않으면 In Rail상태가 되면 안된다..
                            inRail &= m_ProcessDataHandler.CurVehicleStatus.GetVehicleStatus() != VehicleState.Removed;
                            m_ProcessDataHandler.CurVehicleStatus.InRail = inRail ? VehicleInRailStatus.InRail : VehicleInRailStatus.OutOfRail;
                            bool working = false;
                            working |= m_ProcessDataHandler.CurVehicleStatus.CurrentVehicleState == VehicleState.Acquiring;
                            working |= m_ProcessDataHandler.CurVehicleStatus.CurrentVehicleState == VehicleState.Depositing;
                            bool command_busy = m_ProcessDataHandler.CurTransferCommand.IsValid;
                            if (command_busy)
                            {
                                command_busy &= m_ProcessDataHandler.CurTransferCommand.CommandStatus >= ProcessStatus.Assigned;
                                command_busy &= m_ProcessDataHandler.CurTransferCommand.CommandStatus <= ProcessStatus.Completed;
                            }

                            double cur_vehicle_position = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.CurrentPositionOfLink;
                            bool obs_used = true;
                            obs_used &= SetupManager.Instance.SetupSafty.OBSUpperSensorUse == Use.Use;
                            obs_used &= SetupManager.Instance.SetupSafty.OBSLowerSensorUse == Use.Use;
                            bool jcs_used = true;
                            jcs_used &= JCSCommManager.Instance.JcsComm.JcsUse;
                            bool pause = false;
                            pause |= m_EqpStateManager.RunMode != EqpRunMode.Start;
                            pause |= ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.MxpOverrideRatio < 0.01f;
                            pause |= GV.WheelBusy == false;
                            pause &= m_ProcessDataHandler.CurTransferCommand.IsValid;

                            // OCS State Report Data 
                            m_OCSManager.OcsStatus.VehicleNumber = AppConfig.Instance.VehicleNumber;
                            m_OCSManager.OcsStatus.InRailStatus = inRail;
                            m_OCSManager.OcsStatus.PowerOnStatus = GV.PowerOn;
                            m_OCSManager.OcsStatus.AutoRunStatus = auto_run;
                            m_OCSManager.OcsStatus.PauseStatus = pause;
                            m_OCSManager.OcsStatus.ErrorStatus = AlarmCurrentProvider.Instance.IsHeavyAlarm();
                            m_OCSManager.OcsStatus.MovingStatus = GV.WheelBusy;
                            m_OCSManager.OcsStatus.BusyStatus = command_busy;
                            bool auto_teach = GV.AutoTeachingModeOn;
                            auto_teach |= m_ProcessDataHandler.CurVehicleStatus.CurrentVehicleState == VehicleState.AutoTeaching;
                            m_OCSManager.OcsStatus.AutoTeachingStatus = auto_teach;
                            m_OCSManager.OcsStatus.CarrierExistStatus = m_DeviceManager.DevGripperPIO.IsProductExist();
                            m_OCSManager.OcsStatus.CurrentSpeed = (int)m_DeviceManager.DevTransfer.AxisMaster.GetDevAxis().GetCurVelocity();
                            m_OCSManager.OcsStatus.BarcodeValue1 = (int)ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                            m_OCSManager.OcsStatus.BarcodeValue2 = (int)ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                            m_OCSManager.OcsStatus.CurrentNodeNo = (ushort)cur_node_id;
                            m_OCSManager.OcsStatus.CurrentLinkNo = cur_link_id;
                            m_OCSManager.OcsStatus.CurrentVehiclePosition = (int)cur_vehicle_position;
                            m_OCSManager.OcsStatus.NextNodeNo = (ushort)next_node_id;
                            m_OCSManager.OcsStatus.TargetNodeNo = (ushort)m_ProcessDataHandler.CurTransferCommand.EndNode;
                            m_OCSManager.OcsStatus.CurrentCommandID = m_ProcessDataHandler.CurTransferCommand.IsValid ? m_ProcessDataHandler.CurTransferCommand.CommandID : "";
                            m_OCSManager.OcsStatus.UseFrontDetectionSensor = obs_used;
                            m_OCSManager.OcsStatus.UseJCS = jcs_used;

                            List<bool> inputs = IoManager.Instance.GetStateInputs();
                            int length = Math.Min(inputs.Count, m_OCSManager.OcsStatus.InputStatus.Length);
                            Array.Copy(inputs.ToArray(), 0, m_OCSManager.OcsStatus.InputStatus, 0, length);
                            List<bool> outputs = IoManager.Instance.GetStateOutputs();
                            length = Math.Min(outputs.Count, m_OCSManager.OcsStatus.OutputStatus.Length);
                            Array.Copy(outputs.ToArray(), 0, m_OCSManager.OcsStatus.OutputStatus, 0, length);

                            // JCS State Report Data 
                            //m_JCSManager.JcsStatus.JunctionNumber = 0;
                            m_JCSManager.JcsStatus.CurrentNode = cur_node_id;
                            m_JCSManager.JcsStatus.CurrentLink = cur_link_id;
                            m_JCSManager.JcsStatus.CurrentPositionOfLink = cur_vehicle_position;
                            m_JCSManager.JcsStatus.InRailState = inRail;
                            m_JCSManager.JcsStatus.WorkingState = working;
                            m_JCSManager.JcsStatus.PauseState = m_EqpStateManager.State == EqpState.Pause;
                            m_JCSManager.JcsStatus.DownState = m_EqpStateManager.State == EqpState.Down;

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
            #endregion
        }

        public class SeqUserLoginMonitor : XSeqFunc
        {
            private const string FuncName = "[SeqUserLoginMonitor]";

            #region Fields
            private Query_UserList m_QueryUser = null;
            private List<DataItem_UserInfo> users = null;
            #endregion

            #region Contructor
            public SeqUserLoginMonitor()
            {
                this.SeqName = $"SeqUserLoginMonitor";
            }
            #endregion

            #region XSeqFunction overrides
            public override void SeqAbort()
            {
            }
            public override int Do()
            {
                int seqNo = this.SeqNo;
                try
                {
                    switch (seqNo)
                    {
                        case 0:
                            {
                                m_QueryUser = new Query_UserList(DatabaseAdapter.Instance.GetJobSession());
                                users = m_QueryUser.SelectAllOrNull();
                                if (users.Any(x => !string.IsNullOrEmpty(x.ClientIp)))
                                {
                                    seqNo = 20;
                                }
                                else
                                {
                                    seqNo = 10;
                                }
                                StartTicks = XFunc.GetTickCount();
                            }
                            break;

                        case 10:
                            {
                                if (XFunc.GetTickCount() - StartTicks > 2000)
                                {
                                    seqNo = 0;
                                }
                            }
                            break;
                        case 20:
                            {
                                var loginUsers = users.Where(x => !string.IsNullOrEmpty(x.ClientIp));
                                foreach (var user in loginUsers)
                                {
                                    if (DateTime.Now - user.LoginTime >= TimeSpan.FromSeconds(5))
                                    {
                                        m_QueryUser.UpdateClientIp(user.UserName, "");
                                    }
                                }
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.Message);
                }

                this.SeqNo = seqNo;
                return -1;
            }
            #endregion
        }
    }
}
