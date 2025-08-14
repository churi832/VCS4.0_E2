using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Data;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data.EventLog;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Device;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.IF.OCS;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.Task
{
    public class TaskMain : XSequence
    {
        #region Field
        public static readonly TaskMain Instance = new TaskMain();
        #endregion

        #region Constructor

        public TaskMain()
        {
            ThreadInfo.Name = string.Format("TaskMain");

            this.RegSeq(new SeqAuto());
        }
        #endregion
    }
    #region Methods
    public class SeqAuto : XSeqFunc
    {
        private const string FuncName = "[SeqAuto]";
        #region Fields
        private _DevAxis m_MasterAxis = null;
        private _DevAxis m_SlaveAxis = null;
        private _DevAxis m_SlideAxis = null;
        private _DevAxis m_HoistAxis = null;
        private _DevAxis m_RotateAxis = null;

        private SeqCycleTest m_SeqCycleTest = new SeqCycleTest();
        private SeqTransfer m_SeqTransfer = new SeqTransfer();
        private SeqVehicleMove m_SeqVehicleMove = new SeqVehicleMove();
        private SeqAutoTeaching m_SeqAutoTeaching = new SeqAutoTeaching();

        private System.Diagnostics.Stopwatch m_StopWatch = new System.Diagnostics.Stopwatch();
        private List<XSeqFunc> m_SubSeqLists = new List<XSeqFunc>();
        #endregion

        #region Properties
        public List<XSeqFunc> SubSeqLists { get { return m_SubSeqLists; } }
        #endregion

        #region Constructor
        public SeqAuto()
        {
            this.SeqName = $"SeqAuto";

            m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();
            m_SlaveAxis = DevicesManager.Instance.DevTransfer.AxisSlave.GetDevAxis();
            m_SlideAxis = DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis();
            m_HoistAxis = DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis();
            m_RotateAxis = DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis();

            SubSeqLists.Add(m_SeqCycleTest);
            SubSeqLists.Add(m_SeqTransfer);
            SubSeqLists.Add(m_SeqVehicleMove);
            SubSeqLists.Add(m_SeqAutoTeaching);
            StartTicks = XFunc.GetTickCount();
        }
        #endregion

        #region Override
        public override void SeqAbort()
        {
            TransferCommand curCmd = ProcessDataHandler.Instance.CurTransferCommand;
            bool aging = curCmd.ProcessCommand == IF.OCS.OCSCommand.CycleHoistAging;
            aging |= curCmd.ProcessCommand == IF.OCS.OCSCommand.CycleSteerAging;
            aging |= curCmd.ProcessCommand == IF.OCS.OCSCommand.CycleAntiDropAging;
            aging |= curCmd.ProcessCommand == IF.OCS.OCSCommand.CycleWheelMoveAging;
            aging &= curCmd.IsValid;
            if (aging)
            {
                ProcessDataHandler.Instance.DeleteTransferCommand(ProcessDataHandler.Instance.CurTransferCommand);
            }

            m_SeqCycleTest.SeqAbort();
            m_SeqTransfer.SeqAbort();
            m_SeqVehicleMove.SeqAbort();
            m_SeqAutoTeaching.SeqAbort();
            InitSeq();
            SequenceLog.WriteLog(FuncName, string.Format("SeqAbort, SeqNo = 0"));
        }
        public override void InitSeq()
        {
            m_SeqCycleTest.InitSeq();
            m_SeqTransfer.InitSeq();
            m_SeqVehicleMove.InitSeq();
            m_SeqAutoTeaching.InitSeq();
            this.SeqNo = 0;
        }
        #endregion

        #region Sequence
        public override int Do()
        {
            if (EqpStateManager.Instance.OpMode != OperateMode.Auto ||
                EqpStateManager.Instance.RunMode == EqpRunMode.Abort)
            {
                InitSeq();
                return -1;
            }
            if (EqpStateManager.Instance.RunMode == EqpRunMode.None ||
                EqpStateManager.Instance.RunMode == EqpRunMode.Pause)
            {
                return -1;
            }

            int rv = -1;
            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        bool command_cancel = ProcessDataHandler.Instance.CurTransferCommand.IsValid;
                        command_cancel &= (ProcessDataHandler.Instance.CurTransferCommand.CommandStatus == ProcessStatus.Canceling ||
                            ProcessDataHandler.Instance.CurTransferCommand.CommandStatus == ProcessStatus.Aborting);

                        if (EqpStateManager.Instance.RunMode == EqpRunMode.Stop || command_cancel)
                        {
                            // 장비상태가 정상인데 Stop 때문에 진행을 못하는 경우
                            //bool stop_ng_recovery = IsEnableAutoRun() == false;
                            //stop_ng_recovery &= EqpStateManager.Instance.EqpInitComp == false;
                            //stop_ng_recovery &= EqpStateManager.Instance.EqpInitIng == false;
                            //stop_ng_recovery &= EqpStateManager.Instance.EqpInitReq == false;
                            //stop_ng_recovery &= EqpStateManager.Instance.EqpRecoveryInitReq == false;
                            //stop_ng_recovery &= EqpStateManager.Instance.EqpAutoRunInitReq == false;
                            //stop_ng_recovery &= ProcessDataHandler.Instance.CommandQueue.Count == 0;
                            //stop_ng_recovery &= EqpStateManager.Instance.EqpAutoStartChangeToStop; // Auto 운전 중 Alarm이 발생한 경우
                            //if (stop_ng_recovery) //명령이 없을때
                            //{
                            //    SequenceLog.WriteLog(FuncName, string.Format("Auto/Stop Condition NG"));
                            //    EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.OperationConfirm, "Auto/Stop Condition NG");
                            //    StartTicks = XFunc.GetTickCount();
                            //    seqNo = 600;
                            //}
                            break;
                        }

                        // 장비 상태 확인 후 -> Run 가능한 상태이면 진행해야 함.
                        // EQP Init => Foup 유무 확인 => Data 초기화 => Servo 확인 
                        // Alarm이면 Alarm Reset 후 진행
                        // Safty 관련 Check 유무 확인
                        if (ProcessDataHandler.Instance.CommandQueue.Count > 0)
                        {
                            TransferCommand cmd = ProcessDataHandler.Instance.CommandQueue.Peek();
                            if (cmd.IsValid)
                            {
                                if (IsEnableAutoRun())
                                {
                                    EventLogHandler.Instance.Add(FuncName, "Auto", string.Format("Command-{0} Start", cmd.CommandID), true);
                                    ProcessDataHandler.Instance.CurTransferCommand = cmd;
                                    ProcessDataHandler.Instance.CurTransferCommand.UpdateTransferTime(transferUpdateTime.CommandAssigned);
                                    ProcessDataHandler.Instance.CurTransferCommand.SetProcessStatus(ProcessStatus.Assigned);
                                    OCSCommManager.Instance.OcsStatus.ClearVehicleEvent();
                                    SequenceLog.WriteLog(FuncName, string.Format("Command Start : {0}", cmd.CommandID));
                                    if (AppConfig.Instance.VehicleType == VehicleType.Clean)
                                    {
                                        if (cmd.ProcessCommand == IF.OCS.OCSCommand.Go) { m_SeqVehicleMove.SeqAbort(); seqNo = 200; }
                                        else
                                        {
                                            ProcessDataHandler.Instance.DeleteTransferCommand(cmd);
                                        }
                                    }
                                    else
                                    {
                                        if (cmd.ProcessCommand == IF.OCS.OCSCommand.Transfer) { m_SeqTransfer.SeqAbort(); seqNo = 100; }
                                        else if (cmd.ProcessCommand == IF.OCS.OCSCommand.Go) { m_SeqVehicleMove.SeqAbort(); seqNo = 200; }
                                        else if (cmd.ProcessCommand == IF.OCS.OCSCommand.Teaching) { m_SeqAutoTeaching.SeqAbort(); seqNo = 300; }
                                        else if (cmd.ProcessCommand == IF.OCS.OCSCommand.CycleHoistAging ||
                                            cmd.ProcessCommand == IF.OCS.OCSCommand.CycleSteerAging ||
                                            cmd.ProcessCommand == IF.OCS.OCSCommand.CycleAntiDropAging ||
                                            cmd.ProcessCommand == IF.OCS.OCSCommand.CycleWheelMoveAging) { m_SeqCycleTest.SeqAbort(); seqNo = 2000; }
                                        else
                                        {
                                            ProcessDataHandler.Instance.DeleteTransferCommand(cmd);
                                        }
                                    }

                                    if (seqNo != 0)
                                    {
                                        ProcessDataHandler.Instance.CurTransferCommand._SaveCurState = true;
                                        // 명령 시작 전이기때문에 Command는 Not Assigned 상태로 시작하자~~
                                        ProcessDataHandler.Instance.CurVehicleStatus.SetVehicleStatus(VehicleState.NotAssigned);
                                    }
                                }
                                else
                                {
                                    //장비를 초기화하여 준비가 되도록 하자....
                                    SequenceLog.WriteLog(FuncName, "Auto Run Condition NG!");
                                    string message = string.Empty;
                                    if (m_MasterAxis != null) message += string.Format("{0}={1}|", m_MasterAxis.GetName(), m_MasterAxis.GetAxis().AxisStateMsg);
                                    if (m_SlaveAxis != null) message += string.Format("{0}={1}|", m_SlaveAxis.GetName(), m_SlaveAxis.GetAxis().AxisStateMsg);
                                    if (m_SlideAxis != null) message += string.Format("{0}={1}|", m_SlideAxis.GetName(), m_SlideAxis.GetAxis().AxisStateMsg);
                                    if (m_HoistAxis != null) message += string.Format("{0}={1}|", m_HoistAxis.GetName(), m_HoistAxis.GetAxis().AxisStateMsg);
                                    if (m_RotateAxis != null) message += string.Format("{0}={1}|", m_RotateAxis.GetName(), m_RotateAxis.GetAxis().AxisStateMsg);
                                    message += string.Format("TransferMoveEnable={0}|", GV.TransferMoveEnable);
                                    message += string.Format("TransferMoveEnableCode={0}|", GV.TransferMoveEnableCode);
                                    message += string.Format("EqpInitComp={0}|", EqpStateManager.Instance.EqpInitComp);
                                    message += string.Format("ThreadStop={0}|", GV.ThreadStop);
                                    message += string.Format("BeltCutInterlock={0}|", GV.BeltCutInterlock);
                                    message += string.Format("SwingSensorInterlock={0}|", GV.SwingSensorInterlock);
                                    message += string.Format("BumpCollisionInterlock={0}|", GV.BumpCollisionInterlock);
                                    message += string.Format("PowerOn={0}|", GV.PowerOn);
                                    message += string.Format("EmoAlarm={0}|", GV.EmoAlarm);
                                    message += string.Format("SaftyAlarm={0}|", GV.SaftyAlarm);
                                    message += string.Format("CleanerDoorOpenInterlock={0}|", GV.CleanerDoorOpenInterlock);
                                    if (m_SlideAxis != null)
                                    {
                                        bool SlideHome = (m_SlideAxis.GetAxis().AxisStatus & enAxisInFlag.Org) == enAxisInFlag.Org;
                                        message += string.Format("{0}=HomeSensor={1}|", m_SlideAxis.GetName(), SlideHome);
                                    }
                                    if (m_HoistAxis != null)
                                    {
                                        bool HoistHome = (m_HoistAxis.GetAxis().AxisStatus & enAxisInFlag.Org) == enAxisInFlag.Org;
                                        message += string.Format("{0}=HomeSensor={1}|", m_HoistAxis.GetName(), HoistHome);
                                    }
                                    if (m_RotateAxis != null)
                                    {
                                        bool RotateHome = (m_RotateAxis.GetAxis().AxisStatus & enAxisInFlag.Org) == enAxisInFlag.Org;
                                        message += string.Format("{0}=HomeSensor={1}|", m_RotateAxis.GetName(), RotateHome);
                                    }
                                    SequenceLog.WriteLog(FuncName, message);

                                    EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.OperationConfirm, "Auto Run Condition NG");
                                    seqNo = 500;
                                }
                            }
                            else
                            {
                                ProcessDataHandler.Instance.DeleteTransferCommand(cmd);
                            }
                        }
                    }
                    break;

                case 100:
                    {
                        int rv1 = m_SeqTransfer.Do();
                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName, "SeqTransfer Completed !");
                            seqNo = 900;
                        }
                        else if (rv1 > 0)
                        {
                            m_SeqTransfer.SeqAbort();

                            // 명령을 Abort 하도록 하자...!
                            SequenceLog.WriteLog(FuncName, "SeqTransfer Abnormal End !");
                            seqNo = 0;
                        }
                    }
                    break;

                case 200:
                    {
                        // Go Command Run
                        int rv1 = m_SeqVehicleMove.Do();
                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName, "SeqVehicleMove Completed !");
                            seqNo = 900;
                        }
                        else if (rv1 > 0)
                        {
                            m_SeqVehicleMove.SeqAbort();

                            // 명령을 Abort 하도록 하자...!
                            SequenceLog.WriteLog(FuncName, "SeqVehicleMove Abnormal End !");
                            seqNo = 0;
                        }
                    }
                    break;

                case 300:
                    {
                        // Auto Teaching Command Run
                        int rv1 = m_SeqAutoTeaching.Do();
                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName, "SeqAutoTeaching Completed !");
                            seqNo = 900;
                        }
                        else if (rv1 > 0)
                        {
                            m_SeqAutoTeaching.SeqAbort();

                            // 명령을 Abort 하도록 하자...!
                            SequenceLog.WriteLog(FuncName, "SeqAutoTeaching Abnormal End !");
                            seqNo = 0;
                        }
                    }
                    break;

                case 500:
                    {
                        DefualtLog();
                        if (IsEnableAutoRun())
                        {
                            SequenceLog.WriteLog(FuncName, "Auto Run Condition OK!");
                            EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.Hide, "");
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Start);
                            seqNo = 0;
                        }
                        else if (IsServoReady() == false || EqpStateManager.Instance.EqpInitComp == false)
                        {
                            //초기화를 시도해 보자
                            EqpStateManager.Instance.ResetInitState(); //초기화 후 다시 진행해라....
                            EqpStateManager.Instance.EqpAutoRunInitReq = true;
                            SequenceLog.WriteLog(FuncName, "EQP Intialize Request");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 505;
                        }
                        else
                        {
                            //이때는 Interlock Case 이므로 장비 운전이 불가한 상태임.
                            // Operator Call을 띄우고 대기하도록 하자~~~
                        }
                    }
                    break;

                case 505:
                    {
                        if (EqpStateManager.Instance.EqpInitComp == false)
                        {
                            SequenceLog.WriteLog(FuncName, "EQP Intialize EqpInitComp==false");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 510;
                        }
                    }
                    break;

                case 510:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;
                        if (EqpStateManager.Instance.EqpInitComp)
                        {
                            SequenceLog.WriteLog(FuncName, "EQP Intialize Complete");
                            seqNo = 500;
                        }
                    }
                    break;

                case 600:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 5000) break;
                        //초기화를 시도해 보자
                        EqpStateManager.Instance.ResetInitState(); //초기화 후 다시 진행해라....
                        EqpStateManager.Instance.EqpAutoRunInitReq = true;
                        SequenceLog.WriteLog(FuncName, "EQP Intialize Request");
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 605;
                    }
                    break;

                case 605:
                    {
                        if (EqpStateManager.Instance.EqpInitComp == false)
                        {
                            SequenceLog.WriteLog(FuncName, "EQP Intialize EqpInitComp==false");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 610;
                        }
                    }
                    break;

                case 610:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;
                        if (EqpStateManager.Instance.EqpInitComp)
                        {
                            SequenceLog.WriteLog(FuncName, "EQP Intialize Complete => Start");
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Start);
                            seqNo = 0;
                        }
                    }
                    break;

                case 900:
                    {
                        EventLogHandler.Instance.Add(FuncName, "Auto", string.Format("Command-{0} Finished", ProcessDataHandler.Instance.CurTransferCommand.CommandID), true);

                        //ProcessDataHandler.Instance.CurTransferCommand.UpdateTransferTime(transferUpdateTime.CommandCompleted); //DeleteTransferCommand에서 처리하고 있음.
                        ProcessDataHandler.Instance.DeleteTransferCommand(ProcessDataHandler.Instance.CurTransferCommand);

                        //Vehicle Move에서 Remove처리
                        //이후 SeqAuto에서 바로 NotAssigned 처리
                        //이것 때문에 MTL 내려가는 와중에 OCS로 명령을 받아 움직임..
                        //Cylinder에 부딪혀 Servo Alarm 발생..
                        //Auto 전환 할 때 NotAssigned 되도록 하자..
                        if (ProcessDataHandler.Instance.CurVehicleStatus.GetVehicleStatus() == VehicleState.Removed)
                            EqpStateManager.Instance.SetOpMode(OperateMode.Manual);
                        else
                            ProcessDataHandler.Instance.CurVehicleStatus.SetVehicleStatus(VehicleState.NotAssigned);

                        SequenceLog.WriteLog(FuncName, string.Format("Command Finished : {0}", ProcessDataHandler.Instance.CurTransferCommand.CommandID));
                        seqNo = 0;
                    }
                    break;

                case 2000:
                    {
                        // Cycle Test Aging
                        int rv1 = m_SeqCycleTest.Do();
                        if (rv1 == 0)
                        {
                            ProcessDataHandler.Instance.DeleteTransferCommand(ProcessDataHandler.Instance.CurTransferCommand);

                            SequenceLog.WriteLog(FuncName, "Cycle Test Completed !");
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Stop);
                            seqNo = 0;
                        }
                        else if (rv1 > 0)
                        {
                            ProcessDataHandler.Instance.DeleteTransferCommand(ProcessDataHandler.Instance.CurTransferCommand);

                            SequenceLog.WriteLog(FuncName, "Cycle Test Alarm !");
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Stop);
                            string msg = string.Format("{0}-Cycle Test Alarm : {1}", ProcessDataHandler.Instance.CurTransferCommand.ProcessCommand, EqpAlarm.GetAlarmMsg(rv1));
                            EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.OperationConfirm, msg);
                            seqNo = 0;
                        }
                    }
                    break;
            }

            this.SeqNo = seqNo;
            return rv;
        }
        // 초기화가 되어 있어야 한다.
        // All Servo Ready
        private bool IsServoReady()
        {
            bool isReady = true;
            isReady &= m_MasterAxis != null ? m_MasterAxis.IsAxisReady() : true;
            //isReady &= m_SlaveAxis != null ? m_SlaveAxis.IsAxisReady() : true;
            isReady &= m_SlideAxis != null ? m_SlideAxis.IsAxisReady() : true;
            isReady &= m_HoistAxis != null ? m_HoistAxis.IsAxisReady() : true;
            isReady &= m_RotateAxis != null ? m_RotateAxis.IsAxisReady() : true;
            isReady &= GV.TransferMoveEnable;
            return isReady;
        }
        private bool IsEnableAutoRun()
        {
            bool isEnable = true;
            isEnable &= EqpStateManager.Instance.State != EqpState.Down;
            isEnable &= EqpStateManager.Instance.EqpInitComp;
            isEnable &= !GV.ThreadStop;
            isEnable &= !GV.BeltCutInterlock;
            isEnable &= !GV.SwingSensorInterlock;
            isEnable &= !GV.BumpCollisionInterlock;
            isEnable &= GV.PowerOn;
            isEnable &= !GV.EmoAlarm;
            isEnable &= !GV.SaftyAlarm;
            isEnable &= !GV.CleanerDoorOpenInterlock;
            isEnable &= IsServoReady();
            return isEnable;
        }

        private bool IsAxisInposFlag()
        {
            bool isAxisInpos = true;
            isAxisInpos &= m_MasterAxis != null ? m_MasterAxis.IsInpos() : true;
            //isAxisInpos &= m_SlaveAxis != null ? m_SlaveAxis.IsInpos() : true;
            isAxisInpos &= m_SlideAxis != null ? m_SlideAxis.IsInpos() : true;
            isAxisInpos &= m_HoistAxis != null ? m_MasterAxis.IsInpos() : true;
            isAxisInpos &= m_RotateAxis != null ? m_RotateAxis.IsInpos() : true;

            return isAxisInpos;
        }

        private void DefualtLog()
        {
            if (m_StopWatch.IsRunning == false) m_StopWatch.Start();
            if (m_StopWatch.ElapsedTicks > 5 * 1000)
            {
                ///////////////////////////////////////////////////////////////
                string msg = string.Format("\r\n");
                msg += string.Format("[SeqNo, State, RunMode, OpMode]\r\n");
                msg += string.Format("[{0}, {1}, {2}, {3}]\r\n", this.SeqNo, EqpStateManager.Instance.State, EqpStateManager.Instance.RunMode, EqpStateManager.Instance.OpMode);
                msg += string.Format("[EnableAutoRun, CMDQueueCount, ProcessStatus]\r\n");
                msg += string.Format("[{0}, {1}, {2}]\r\n", IsEnableAutoRun(), ProcessDataHandler.Instance.CommandQueue.Count, ProcessDataHandler.Instance.CurTransferCommand.GetProcessStatus());
                if (IsEnableAutoRun() == false)
                {
                    msg += string.Format("[EnableAutoRun]\r\n");
                    msg += string.Format("[EqpInitComp, ThreadStop, BeltCutInterlock, SwingSensorInterlock, BumpCollisionInterlock, PowerOn, TransferMoveEnable]\r\n");
                    msg += string.Format("[{0}, {1}, {2}, {3}, {4}, {5}, {6}]\r\n", EqpStateManager.Instance.EqpInitComp, GV.ThreadStop, GV.BeltCutInterlock, GV.SwingSensorInterlock, GV.BumpCollisionInterlock, GV.PowerOn, GV.TransferMoveEnable);
                    msg += string.Format("[EmoAlarm, SaftyAlarm, CleanerDoorOpenInterlock, MasterAxisReady, SlaveAxisReady, SlideAxisReady, HoistAxisReady, RotateAxisReady]\r\n");
                    msg += string.Format("[{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}]\r\n",
                        GV.EmoAlarm, GV.SaftyAlarm, GV.CleanerDoorOpenInterlock, m_MasterAxis.IsAxisReady(), m_SlaveAxis.IsAxisReady(), m_SlideAxis.IsAxisReady(), m_HoistAxis.IsAxisReady(), m_RotateAxis.IsAxisReady());

                    if (IsAxisInposFlag() == false)
                    {
                        msg += string.Format("[MasterAxisInPos, SlaveAxisInPos, SlideAxisInPos, HoistAxisInPos, RotateAxisInPos]\r\n");
                        msg += string.Format("[{0}, {1}, {2}, {3}, {4}]\r\n", m_MasterAxis.IsInpos(), m_SlaveAxis.IsInpos(), m_SlideAxis.IsInpos(), m_HoistAxis.IsInpos(), m_RotateAxis.IsInpos());
                    }
                    if (GV.TransferMoveEnable == false)
                    {
                        bool HoistSafePos = Math.Abs(m_HoistAxis.GetCurPosition()) < 10.0f;
                        bool SlideSafePos = Math.Abs(m_SlideAxis.GetCurPosition()) < 10.0f;
                        bool WheelBreak = DevicesManager.Instance.DevTransfer.DiWheelBrake.IsDetected;
                        bool FrontDetectSafe = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.ObsUpperSensorState != enFrontDetectState.enStop;
                        bool GripClose = DevicesManager.Instance.DevGripperPIO.IsHoistLimit() ? DevicesManager.Instance.DevGripperPIO.IsGripperClose() : true;

                        msg += string.Format("[TransferMoveEnable]\r\n");
                        msg += string.Format("[SteerNotChangeInterlock, HoistSafePos, SlideSafePos, WheelBreak, FrontDetectSafe, GripClose]\r\n");
                        msg += string.Format("[{0}, {1}, {2}, {3}, {4}, {5}]\r\n", GV.SteerNotChangeInterlock, HoistSafePos, SlideSafePos, WheelBreak, FrontDetectSafe, GripClose);
                    }

                }
                ///////////////////////////////////////////////////////////////
                SeqAutoLog.WriteLog(FuncName, msg);

                m_StopWatch.Reset();
                m_StopWatch.Start();
            }

        }
        #endregion
    }
    #endregion
}
