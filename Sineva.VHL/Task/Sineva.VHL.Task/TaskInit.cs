using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data;
using Sineva.VHL.Device;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Data.EventLog;
using Sineva.VHL.Data.Process;
using System.Runtime;
using Sineva.VHL.Library.MXP;

namespace Sineva.VHL.Task
{
    public class TaskInit : XSequence
    {
        public static readonly TaskInit Instance = new TaskInit();

        public TaskInit()
        {
            ThreadInfo.Name = string.Format("EquipmentInit");

            this.RegSeq(new SeqInit());
        }
    }
    public class SeqInit : XSeqFunc
    {
        private const string FuncName = "[INIT]";

        #region Fields
        SeqInitConditionCheck m_SeqInitConditionCheck = new SeqInitConditionCheck();
        SeqInitTransfer m_SeqInitTransfer = new SeqInitTransfer();

        public static Queue<XSeqFunc> QueueInitRunSeq = new Queue<XSeqFunc>();
        private EqpStateManager m_EqpManager = null;
        private XSeqFunc m_CurSeq = null;
        #endregion

        #region Constructor
        public SeqInit()
        {
            this.SeqName = "Equipment Init";
            m_EqpManager = EqpStateManager.Instance;
        }
        public override void SeqAbort()
        {
            if (AlarmId > 0)
            {
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            GV.InitComp = false;
            GV.InitStart = false;
            GV.InitFail = false;
            m_SeqInitConditionCheck.SeqAbort();
            m_SeqInitTransfer.SeqAbort();
            this.InitSeq();
        }
        #endregion

        #region Override
        public override int Do()
        {
            int rv = -1;
            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        if (m_EqpManager.EqpInitReq || m_EqpManager.EqpRecoveryInitReq || m_EqpManager.EqpAutoRunInitReq)
                        {
                            SequenceLog.WriteLog(FuncName, "Equipment Initilize Start");
                            EventLogHandler.Instance.Add(FuncName, "EQUIPMENT", "Initialize Start", true);

                            m_SeqInitConditionCheck.scm.Reset(); // always reset .... !
                            m_SeqInitTransfer.scm.Reset();


                            GV.RecoveryProcess = true; //Init시에는 Auto상태로 보고해야됨..
                            GV.SteerNotChangeInterlock = false; // DevSteer.SeqMonitor가 초기화 될수 있도록 하자~~

                            List<XSeqFunc> funcs = TaskHandler.Instance.GetFuncLists();
                            foreach (XSeqFunc func in funcs) 
                            {
                                if (func.SeqName == "SeqModeMonitor" || func.SeqName == "SeqAbortMonitor") continue;
                                if (m_EqpManager.EqpRecoveryInitReq && func.SeqName == "SeqAutoRecovery") continue;
                                if (m_EqpManager.EqpRecoveryInitReq && func.SeqName == "SeqInterlockAutoRecovery") continue;
                                if (m_EqpManager.EqpAutoRunInitReq && func.SeqName == "SeqAuto") continue;
                                if (func.SeqName == "SeqCpsVoltageLowCheck" ||
                                    func.SeqName == "SeqRouteChangedCheck" ||
                                    func.SeqName == "SeqBcrNotChangedCheck" ||
                                    func.SeqName == "SeqMxpHeartBitCheck" ||
                                    func.SeqName == "SeqMxpEtherCatConnectionCheck" ||
                                    func.SeqName == "SeqOverrideAbnormalStopCheck" ||
                                    func.SeqName == "SeqTouchHeartBitCheck") continue;

                                func.SeqAbort(); 
                            }

                            // Init Tag 초기화
                            m_SeqInitConditionCheck.InitSeq();
                            m_SeqInitTransfer.InitSeq();
                            QueueInitRunSeq.Clear();
                            QueueInitRunSeq.Enqueue(m_SeqInitConditionCheck);
                            QueueInitRunSeq.Enqueue(m_SeqInitTransfer);
                            foreach (XSeqFunc seq in QueueInitRunSeq)
                            {
                                if (seq != null && seq.scm.End == false)
                                {
                                    seq.InitTag.State = InitState.Init;
                                    m_EqpManager.SetInitState(seq.InitTag);
                                }
                            }

                            m_EqpManager.EqpInitReq = false;
                            m_EqpManager.EqpRecoveryInitReq = false;
                            m_EqpManager.EqpAutoRunInitReq = false;

                            GV.InitFail = false;
                            GV.InitComp = false;
                            GV.InitStart = true;
                            seqNo = 10;
                        }
                    }
                    break;

                case 10:
                    {
                        if (QueueInitRunSeq.Count > 0)
                        {
                            m_CurSeq = QueueInitRunSeq.Peek();
                            if (m_CurSeq == null)
                            {
                                string msg = string.Format("{0} not Exist ! \r\n SETUP/Setup Parameter/SetupWizards/Task Schedule", m_CurSeq.ToString());
                                SequenceLog.WriteLog(FuncName, msg);
                                EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.OperationEnd, msg);
                                EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort);
                            }
                            else
                            {
                                if (m_CurSeq.scm.End)
                                {
                                    // 초기화가 완료된건 빼자
                                    m_CurSeq.InitTag.CheckStatus = InitCheckState.OK;
                                    m_CurSeq.InitTag.State = InitState.Comp;
                                    m_EqpManager.SetInitState(m_CurSeq.InitTag);

                                    QueueInitRunSeq.Dequeue(); //한개를 빼자
                                    StartTicks = XFunc.GetTickCount();
                                }
                                else
                                {
                                    m_CurSeq.InitTag.CheckStatus = InitCheckState.Checking;
                                    m_EqpManager.SetInitState(m_CurSeq.InitTag);
                                    seqNo = 20;
                                }
                            }
                        }
                        else
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Equipment Initilaize Finished"));
                            EventLogHandler.Instance.Add(FuncName, "EQUIPMENT", "Initialize Finished", true);

                            GV.RecoveryProcess = false; //Init완료시에는 끄자..
                            GV.InitComp = true;
                            GV.InitStart = false;
                            GV.InitFail = false;
                            rv = 0;
                            seqNo = 0;
                        }
                    }
                    break;

                case 20:
                    {
                        m_CurSeq.TactTime = (XFunc.GetTickCount() - StartTicks) / 1000.0f;

                        int rv1 = m_CurSeq.Do();
                        if (rv1 == 0)
                        {
                            m_CurSeq.InitTag.CheckStatus = InitCheckState.OK;
                            m_CurSeq.InitTag.State = InitState.Comp;
                            m_EqpManager.SetInitState(m_CurSeq.InitTag);

                            QueueInitRunSeq.Dequeue(); //한개를 빼자
                            seqNo = 10;
                        }
                        else if (rv1 > 0)
                        {
                            m_CurSeq.InitTag.State = InitState.Fail;
                            m_CurSeq.InitTag.CheckStatus = InitCheckState.NG;
                            m_EqpManager.SetInitState(m_CurSeq.InitTag);

                            GV.RecoveryProcess = false; //Alarm에는 끄자..
                            GV.InitFail = true;
                            GV.InitComp = false;
                            GV.InitStart = false;

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            SequenceLog.WriteLog(FuncName, string.Format("Set Alarm : Code[{0}], Message[{1}]", AlarmId, EqpAlarm.GetAlarmMsg(AlarmId)));
                            seqNo = 1000;
                        }
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;
                            m_EqpManager.ResetInitState();

                            EqpAlarm.Reset(AlarmId);
                            SequenceLog.WriteLog(FuncName, string.Format("Reset Alarm : Code[{0}]", AlarmId));

                            AlarmId = 0;
                            seqNo = 0;
                        }
                        break;
                    }
            }

            this.SeqNo = seqNo;
            return rv;
        }
        #endregion
    }
    public class SeqInitConditionCheck : XSeqFunc
    {
        // Func Name
        public static readonly string FuncName = "[SeqInitConditionCheck]";
        public static readonly string FuncName0 = "[INIT]";

        #region Fields
        // New Init Tag
        private XInitTag m_InitTags = null;
        private EqpStateManager m_EqpManager = null;
        private DevGripperPIO m_devGripperPio = null;

        private AlarmData m_ALM_EquipmentInterlockConditionError = null;
        private AlarmData m_ALM_EquipmentFoupExistConditionError = null;
        private AlarmData m_ALM_EquipmentGripperSerialOpenError = null;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public SeqInitConditionCheck()
        {
            this.SeqName = $"SeqInitConditionCheck";

            m_EqpManager = EqpStateManager.Instance;
            m_devGripperPio = DevicesManager.Instance.DevGripperPIO;

            this.InitTag = new XInitTag("1. EQP Condition");
            m_InitTags = new XInitTag("1.1. EQP Condition Check");
            m_EqpManager.RegisterInitItem(this.InitTag);
            m_EqpManager.RegisterInitItem(m_InitTags);

            // Add Alarm
            m_ALM_EquipmentInterlockConditionError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqInit", "Init Condition", "Equipment Interlock Alarm");
            m_ALM_EquipmentFoupExistConditionError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqInit", "Init Condition", "Equipment Foup Exist Alarm");
            m_ALM_EquipmentGripperSerialOpenError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqInit", "Init Condition", "Gripper Serial Communication Open Timeout Alarm");
            AddCaseMemo();
            this.TaskLayer = (int)enTaskLayer.layerInit;
        }
        private void AddCaseMemo()
        {
            try
            {
                this.SeqCaseMemoLists.Add(0, string.Format("Interlock Check"));
                this.SeqCaseMemoLists.Add(10, string.Format("Gripper PIO Check"));
                this.SeqCaseMemoLists.Add(20, string.Format("Foup Data Check"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("{0} EXCEPTION : {1}", FuncName0, ex.ToString()));
            }
        }

        public override void SeqAbort()
        {
            if (AlarmId > 0)
            {
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            bool run = false;
            run |= this.InitTag.State == InitState.Init;
            run |= this.InitTag.State == InitState.Fail;
            run |= m_InitTags.State == InitState.Init;
            run |= m_InitTags.State == InitState.Fail;

            if (m_InitTags.State == InitState.Init || m_InitTags.State == InitState.Fail)
            {
                this.InitTag.State = InitState.Noop;
                this.InitTag.CheckStatus = string.Format("Aborted");
                m_EqpManager.SetInitState(this.InitTag);
                m_InitTags.State = InitState.Noop;
                m_InitTags.CheckStatus = string.Format("Aborted");
                m_EqpManager.SetInitState(m_InitTags);

            }
            this.InitSeq();
        }
        #endregion

        #region Override
        public override int Do()
        {
            if (this.scm.Pause) return -1;
            int rv = -1;
            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        this.scm.Start = true;
                        m_InitTags.State = InitState.Init;
                        m_InitTags.CheckStatus = string.Format("Interlock Condition : {0}", InitCheckState.Checking);
                        m_EqpManager.SetInitState(m_InitTags);

                        AlarmHandler.AutoRecoveryNeed = false;

                        DevicesManager.Instance.SeqAbort();
                        MxpManager.Instance.InitializeCornerActivation();
                        SequenceLog.WriteLog(FuncName0, string.Format("Devices All Initialize"), seqNo);
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 10;
                    }
                    break;

                case 10:
                    {
                        if (GV.MxpHeartBitInterlock && XFunc.GetTickCount() - StartTicks < 10 * 1000) break;
                        if (GV.EtherCatDisconnectError && XFunc.GetTickCount() - StartTicks < 10 * 1000) break;
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        bool alarm = false;
                        alarm |= GV.MxpHeartBitInterlock;
                        alarm |= GV.EtherCatDisconnectError;
                        alarm |= GV.BeltCutInterlock;
                        alarm |= GV.BumpCollisionInterlock;
                        alarm |= GV.EmoAlarm;

                        if (alarm)
                        {
                            m_InitTags.State = InitState.Fail;
                            m_InitTags.CheckStatus = string.Format("Interlock : {0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitTags);
                            string msg = string.Empty;
                            if (GV.MxpHeartBitInterlock) msg += string.Format("Mxp Heart Bit Interlock");
                            if (GV.EtherCatDisconnectError) msg += string.Format("Mxp Link Disconnected Interlock");
                            if (GV.BeltCutInterlock) msg += string.Format("Belt Cut Interlock");
                            if (GV.BumpCollisionInterlock) msg += string.Format("Bump Collision Interlock");
                            if (GV.EmoAlarm) msg += string.Format("Emo Interlock");

                            SequenceLog.WriteLog(FuncName0, string.Format("Equipment Condition Check Alarm : [{0}]", msg), seqNo);
                            AlarmId = m_ALM_EquipmentInterlockConditionError.ID;
                            rv = AlarmId;
                            seqNo = 0;
                        }
                        else
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Equipment Condition Check OK"), seqNo);
                            m_InitTags.CheckStatus = string.Format("Gripper PIO Comm Reset : {0}", InitCheckState.Checking);
                            m_EqpManager.SetInitState(m_InitTags);

                            if (m_devGripperPio.IsValid)
                            {
                                if (m_devGripperPio.PioComm.IsConnected == false) // go까지 가 있어야 한다.
                                {
                                    m_devGripperPio.SeqAbort();
                                }
                            }
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                    }
                    break;

                case 20:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;
                        }

                        // 자동 연결하고 있을거니까 조금만 기다려 보자 ~~~
                        bool connected = true;
                        if (m_devGripperPio.IsValid)
                        {
                            connected &= m_devGripperPio.PioComm.IsConnected;
                        }
                        if (connected)
                        {
                            m_InitTags.CheckStatus = string.Format("Foup Data Exist : {0}", InitCheckState.Checking);
                            m_EqpManager.SetInitState(m_InitTags);

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                        {
                            m_InitTags.State = InitState.Fail;
                            m_InitTags.CheckStatus = string.Format("Gripper Serial NG : {0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitTags);

                            SequenceLog.WriteLog(FuncName0, string.Format("Gripper Serial Open Timeout Alarm"), seqNo);
                            AlarmId = m_ALM_EquipmentGripperSerialOpenError.ID;
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;

                case 30:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;
                        }

                        bool skip_exist_check = false;
                        if (ProcessDataHandler.Instance.CurTransferCommand.IsValid)
                        {
                            skip_exist_check = ProcessDataHandler.Instance.CurTransferCommand.OnlyTransferMove;
                        }

                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            bool flg = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
                            m_devGripperPio.DiLeftProductExist.SetDi(flg);
                            m_devGripperPio.DiRightProductExist.SetDi(flg);
                            m_devGripperPio.DiHoistHome.SetDi(flg);
                        }
                        bool foup_exist = false; 
                        if (m_devGripperPio.IsValid)
                        {
                            // Only Foup Exist Check
                            foup_exist = false;
                            foup_exist |= m_devGripperPio.DiLeftProductExist.IsDetected;
                            foup_exist |= m_devGripperPio.DiRightProductExist.IsDetected;
                            foup_exist |= m_devGripperPio.DiHoistHome.IsDetected;
                            //foup_exist &= m_devGripperPio.DiGripperClose.IsDetected;
                        }
                        bool carrier_data_exist = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
                        bool foup_mismatch = foup_exist && !carrier_data_exist;
                        foup_mismatch |= !foup_exist && carrier_data_exist;

                        if (!foup_mismatch || skip_exist_check)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Data Condition Check OK"), seqNo);
                            m_InitTags.State = InitState.Comp;
                            m_InitTags.CheckStatus = string.Format("{0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitTags);

                            this.scm.Start = false;
                            this.scm.End = true;
                            rv = 0;
                            seqNo = 0;
                        }
                        else
                        {
                            m_InitTags.State = InitState.Fail;
                            m_InitTags.CheckStatus = string.Format("Foup Data : {0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitTags);

                            if (foup_exist)
                                SequenceLog.WriteLog(FuncName0, string.Format("Foup Data Mismatch : Foup Exist Sensor Check Alarm"), seqNo);
                            else SequenceLog.WriteLog(FuncName0, string.Format("Foup Data Mismatch : Data Valid Check Alarm"), seqNo);
                            AlarmId = m_ALM_EquipmentFoupExistConditionError.ID;
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;
            }
            this.SeqNo = seqNo;
            return rv;
        }
        #endregion
    }
    public class SeqInitTransfer : XSeqFunc
    {
        // Func Name
        public static readonly string FuncName = "[SeqInitTransfer]";
        public static readonly string FuncName0 = "[INIT]";
        // New Timer 
        XTimer m_Timer = new XTimer("SeqInitTransfer");

        #region Fields
        // New Init Tag
        private XInitTag m_InitGripper = new XInitTag("2.1. GripperOpenClose");
        private XInitTag m_InitAntiDrop = new XInitTag("2.2. AntiDropHome");
        private XInitTag m_InitSteer = new XInitTag("2.3. SteerState");
        private XInitTag m_InitTransfer = new XInitTag("2.4. TransferWheel");
        private XInitTag m_InitHoist = new XInitTag("2.5. Hoist");
        private XInitTag m_InitSlide = new XInitTag("2.6. Slide");
        private XInitTag m_InitTurn = new XInitTag("2.7. Turn");
        private XInitTag m_InitCurveRecovery = new XInitTag("2.8. Curve Recovery");

        private EqpStateManager m_EqpManager = null;
        private DevGripperPIO m_devGripperPio = null;
        private DevAntiDrop m_devAntiDropFront = null;
        private DevAntiDrop m_devAntiDropRear = null;
        private DevSteer m_devSteer = null;
        private DevTransfer m_devTransfer = null;
        private DevFoupGripper m_devFoupGripper = null;

        private bool m_MoveComp1 = false;
        private bool m_MoveComp2 = false;
        private bool m_MoveComp3 = false;

        private bool m_SteerLeftCheck = false;
        private bool m_SteerRightCheck = false;

        private VelSet m_SearchingVelSet = new VelSet();
        private AlarmData m_ALM_CurveRecoveryFailError = null;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public SeqInitTransfer()
        {
            this.SeqName = $"SeqInitTransfer";

            m_EqpManager = EqpStateManager.Instance;
            m_devGripperPio = DevicesManager.Instance.DevGripperPIO;
            m_devAntiDropFront = DevicesManager.Instance.DevFrontAntiDrop;
            m_devAntiDropRear = DevicesManager.Instance.DevRearAntiDrop;
            m_devSteer = DevicesManager.Instance.DevSteer;
            m_devTransfer = DevicesManager.Instance.DevTransfer;
            m_devFoupGripper = DevicesManager.Instance.DevFoupGripper;

            this.InitTag = new XInitTag("2. Transfer");
            m_EqpManager.RegisterInitItem(this.InitTag);
            m_EqpManager.RegisterInitItem(m_InitGripper);
            m_EqpManager.RegisterInitItem(m_InitAntiDrop);
            m_EqpManager.RegisterInitItem(m_InitSteer);
            m_EqpManager.RegisterInitItem(m_InitTransfer);
            m_EqpManager.RegisterInitItem(m_InitHoist);
            m_EqpManager.RegisterInitItem(m_InitSlide);
            m_EqpManager.RegisterInitItem(m_InitTurn);
            m_EqpManager.RegisterInitItem(m_InitCurveRecovery);

            // Add Alarm
            m_ALM_CurveRecoveryFailError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqInit", "Init Condition", "Curve Recovery Fail Alarm");

            AddCaseMemo();
            this.TaskLayer = (int)enTaskLayer.layerInit;
        }
        private void AddCaseMemo()
        {
            try
            {
                this.SeqCaseMemoLists.Add(0, string.Format("Gripper Action Check"));
                this.SeqCaseMemoLists.Add(10, string.Format("Gripper Open"));
                this.SeqCaseMemoLists.Add(20, string.Format("Anti Drop Home Start"));
                this.SeqCaseMemoLists.Add(30, string.Format("Anti Drop Homing"));
                this.SeqCaseMemoLists.Add(40, string.Format("Foup Exist Check"));
                this.SeqCaseMemoLists.Add(50, string.Format("Anti Drop Unlock"));
                this.SeqCaseMemoLists.Add(70, string.Format("Steer Action Start"));
                this.SeqCaseMemoLists.Add(80, string.Format("Steer Left Move"));
                this.SeqCaseMemoLists.Add(90, string.Format("Steer Right Move"));
                this.SeqCaseMemoLists.Add(100, string.Format("Transfer Wheel Home Start"));
                this.SeqCaseMemoLists.Add(110, string.Format("Transfer Wheel Homing"));
                this.SeqCaseMemoLists.Add(120, string.Format("Hoist Home Start"));
                this.SeqCaseMemoLists.Add(130, string.Format("Hoist Homing"));
                this.SeqCaseMemoLists.Add(140, string.Format("Slide Home Start"));
                this.SeqCaseMemoLists.Add(150, string.Format("Slide Homing"));
                this.SeqCaseMemoLists.Add(160, string.Format("Turn Home Start"));
                this.SeqCaseMemoLists.Add(170, string.Format("Turn Homing"));
                this.SeqCaseMemoLists.Add(180, string.Format("Foup Exist Check"));
                this.SeqCaseMemoLists.Add(190, string.Format("Anti Drop Lock"));
                this.SeqCaseMemoLists.Add(200, string.Format("Cuver Area Check Auto Recovery Start"));
                this.SeqCaseMemoLists.Add(210, string.Format("Straight Area Confirm"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("{0} EXCEPTION : {1}", FuncName0, ex.ToString()));
            }
        }

        public override void SeqAbort()
        {
            if (AlarmId > 0)
            {
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            if (this.scm.Ing)
            {
                //Device 초기화
                m_devGripperPio.SeqAbort();
                m_devAntiDropFront.SeqAbort();
                m_devAntiDropRear.SeqAbort();
                m_devSteer.SeqAbort();
                m_devTransfer.SeqAbort();
                m_devFoupGripper.SeqAbort();
                this.scm.Abort = true;
            }

            bool run = false;
            run |= this.InitTag.State == InitState.Init;
            run |= this.InitTag.State == InitState.Fail;
            run |= m_InitGripper.State == InitState.Init;
            run |= m_InitGripper.State == InitState.Fail;
            run |= m_InitAntiDrop.State == InitState.Init;
            run |= m_InitAntiDrop.State == InitState.Fail;
            run |= m_InitSteer.State == InitState.Init;
            run |= m_InitSteer.State == InitState.Fail;
            run |= m_InitTransfer.State == InitState.Init;
            run |= m_InitTransfer.State == InitState.Fail;
            run |= m_InitHoist.State == InitState.Init;
            run |= m_InitHoist.State == InitState.Fail;
            run |= m_InitSlide.State == InitState.Init;
            run |= m_InitSlide.State == InitState.Fail;
            run |= m_InitTurn.State == InitState.Init;
            run |= m_InitTurn.State == InitState.Fail;
            run |= m_InitCurveRecovery.State == InitState.Init;
            run |= m_InitCurveRecovery.State == InitState.Fail;
            if (run)
            {
                this.InitTag.State = InitState.Noop;
                this.InitTag.CheckStatus = string.Format("Aborted");
                m_EqpManager.SetInitState(this.InitTag);
                m_InitGripper.State = InitState.Noop;
                m_InitGripper.CheckStatus = string.Format("Aborted");
                m_EqpManager.SetInitState(m_InitGripper);
                m_InitAntiDrop.State = InitState.Noop;
                m_InitAntiDrop.CheckStatus = string.Format("Aborted");
                m_EqpManager.SetInitState(m_InitAntiDrop);
                m_InitSteer.State = InitState.Noop;
                m_InitSteer.CheckStatus = string.Format("Aborted");
                m_EqpManager.SetInitState(m_InitSteer);
                m_InitTransfer.State = InitState.Noop;
                m_InitTransfer.CheckStatus = string.Format("Aborted");
                m_EqpManager.SetInitState(m_InitTransfer);
                m_InitHoist.State = InitState.Noop;
                m_InitHoist.CheckStatus = string.Format("Aborted");
                m_EqpManager.SetInitState(m_InitHoist);
                m_InitSlide.State = InitState.Noop;
                m_InitSlide.CheckStatus = string.Format("Aborted");
                m_EqpManager.SetInitState(m_InitSlide);
                m_InitTurn.State = InitState.Noop;
                m_InitTurn.CheckStatus = string.Format("Aborted");
                m_InitCurveRecovery.State = InitState.Noop;
                m_InitCurveRecovery.CheckStatus = string.Format("Aborted");                
                m_EqpManager.SetInitState(m_InitCurveRecovery);
            }
            this.InitSeq();
        }
        #endregion

        #region Override
        public override int Do()
        {
            if (this.scm.Pause) return -1;
            int rv = -1;
            int rv1 = -1;
            int rv2 = -1;
            int rv3 = -1;
            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        this.scm.Start = true;
                        m_SteerLeftCheck = false;
                        m_SteerRightCheck = false;

                        m_InitGripper.State = InitState.Init;
                        m_InitGripper.CheckStatus = string.Format("{0}", InitCheckState.Checking);
                        m_EqpManager.SetInitState(m_InitGripper);

                        // OverrideStopAlarm 확인
                        // InRangeAlarm 확인
                        enAxisInFlag status = (m_devTransfer.AxisMaster.GetDevAxis().GetAxis() as IAxisCommand).GetAxisCurStatus();
                        if (status.HasFlag(enAxisInFlag.OverrideAbnormalStop) || status.HasFlag(enAxisInFlag.InRange_Error))
                        {
                            m_devTransfer.AxisMaster.GetDevAxis().GetServoUnit().AlarmClear();
                        }

                        bool foup_exist = false;
                        if (m_devGripperPio.IsValid)
                        {
                            foup_exist = m_devGripperPio.IsProductExist();
                            foup_exist |= m_devGripperPio.DiHoistHome.IsDetected;
                        }

                        if (foup_exist)
                        {
                            bool gripper_close = m_devGripperPio.IsValid ? m_devGripperPio.DiGripperClose.IsDetected : false;
                            if (gripper_close)
                            {
                                SequenceLog.WriteLog(FuncName0, string.Format("Foup Exist, Gripper Close OK"));
                                m_InitGripper.State = InitState.Comp;
                                m_InitGripper.CheckStatus = string.Format("{0}", InitCheckState.OK);
                                m_EqpManager.SetInitState(m_InitGripper);

                                // Foup이 있으므로 Gripper Close 상태로 두자 !
                                seqNo = 20;
                            }
                            else
                            {
                                // Foup이 있네. 무조건 Close 하자...!
                                SequenceLog.WriteLog(FuncName0, string.Format("Gripper Close"));
                                seqNo = 15;
                            }
                        }
                        else
                        {
                            // Foup이 없으므로 Gripper Open 하자 !
                            SequenceLog.WriteLog(FuncName0, string.Format("Gripper Open"));
                            seqNo = 10;
                        }
                    }
                    break;

                case 10:
                    {
                        if (m_devGripperPio.IsValid) 
                            rv1 = m_devGripperPio.GripperOpen();
                        else rv1 = 0;

                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Gripper Open OK"));
                            m_InitGripper.State = InitState.Comp;
                            m_InitGripper.CheckStatus = string.Format("{0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitGripper);

                            seqNo = 20;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Gripper Open Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            m_InitGripper.State = InitState.Fail;
                            m_InitGripper.CheckStatus = string.Format("{0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitGripper);

                            AlarmId = rv1;
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;

                case 15:
                    {
                        if (m_devGripperPio.IsValid)
                            rv1 = m_devGripperPio.GripperClose();
                        else rv1 = 0;

                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Gripper Close OK"));
                            m_InitGripper.State = InitState.Comp;
                            m_InitGripper.CheckStatus = string.Format("{0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitGripper);

                            seqNo = 20;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Gripper Close Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            m_InitGripper.State = InitState.Fail;
                            m_InitGripper.CheckStatus = string.Format("{0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitGripper);

                            AlarmId = rv1;
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;

                case 20:
                    {
                        bool anti_home = m_devAntiDropFront.IsValid ? m_devAntiDropFront.Type == enAntiDropType.Motor : false;
                        if (anti_home)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("AntiDrop Homing"));

                            m_InitAntiDrop.State = InitState.Init;
                            m_InitAntiDrop.CheckStatus = string.Format("{0}", InitCheckState.ServoHoming);
                            m_EqpManager.SetInitState(m_InitAntiDrop);

                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            seqNo = 30;
                        }
                        else
                        {
                            seqNo = 40;
                        }
                    }
                    break;

                case 30:
                    {
                        if (!m_MoveComp1)
                        {
                            if (m_devAntiDropFront.IsValid)
                                rv1 = m_devAntiDropFront.Home();
                            else rv1 = 0;
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (!m_MoveComp2)
                        {
                            if (m_devAntiDropRear.IsValid)
                                rv2 = m_devAntiDropRear.Home();
                            else rv2 = 0;
                            if (rv2 == 0) m_MoveComp2 = true;
                        }
                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("AntiDrop : Homing OK"));
                            m_InitAntiDrop.State = InitState.Comp;
                            m_InitAntiDrop.CheckStatus = string.Format("{0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitAntiDrop);
                            seqNo = 40;
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            if (m_devAntiDropFront.IsValid) m_devAntiDropFront.SeqAbort();
                            if (m_devAntiDropRear.IsValid) m_devAntiDropRear.SeqAbort();

                            m_InitAntiDrop.State = InitState.Fail;
                            m_InitAntiDrop.CheckStatus = string.Format("{0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitAntiDrop);

                            if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceLog.WriteLog(FuncName0, string.Format("FrontAntiDrop Homing Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else if (rv2 > 0)
                            {
                                AlarmId = rv2;
                                SequenceLog.WriteLog(FuncName0, string.Format("RearAntiDrop Homing Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;

                case 40:
                    {
                        m_MoveComp1 = false;
                        m_MoveComp2 = false;

                        m_InitAntiDrop.State = InitState.Init;
                        m_InitAntiDrop.CheckStatus = string.Format("{0}", InitCheckState.Checking);
                        m_EqpManager.SetInitState(m_InitAntiDrop);

                        bool foup_exist = false;
                        if (m_devGripperPio.IsValid)
                        {
                            foup_exist &= m_devGripperPio.IsProductExist();
                            foup_exist &= m_devGripperPio.DiGripperClose.IsDetected;
                        }
                        // Foup Exist 상관없이 Unlock 수행.. 그래야 hoist home에 문제가 없음.
                        SequenceLog.WriteLog(FuncName0, string.Format("Foup Exist !"));
                        // Anti Drop을 Unlock 하자...
                        SequenceLog.WriteLog(FuncName0, string.Format("AntiDrop Unlock"));
                        seqNo = 50;
                    }
                    break;

                case 50:
                    {
                        if (!m_MoveComp1)
                        {
                            if (m_devAntiDropFront.IsValid)
                                rv1 = m_devAntiDropFront.Unlock();
                            else rv1 = 0;
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (!m_MoveComp2)
                        {
                            if (m_devAntiDropRear.IsValid)
                                rv2 = m_devAntiDropRear.Unlock();
                            else rv2 = 0;
                            if (rv2 == 0) m_MoveComp2 = true;
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("AntiDrop Unlock OK"));
                            m_InitAntiDrop.State = InitState.Comp;
                            m_InitAntiDrop.CheckStatus = string.Format("Unlock {0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitAntiDrop);

                            seqNo = 70;
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            if (m_devAntiDropFront.IsValid) m_devAntiDropFront.SeqAbort();
                            if (m_devAntiDropRear.IsValid) m_devAntiDropRear.SeqAbort();

                            m_InitAntiDrop.State = InitState.Fail;
                            m_InitAntiDrop.CheckStatus = string.Format("Unlock {0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitAntiDrop);

                            if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceLog.WriteLog(FuncName0, string.Format("FrontAntiDrop Unlock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else if (rv2 > 0)
                            {
                                AlarmId = rv2;
                                SequenceLog.WriteLog(FuncName0, string.Format("RearAntiDrop Unlock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;

                case 70:
                    {
                        bool check_enable = true;
                        Sineva.VHL.Data.Process.Path myPath = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath;
                        int linkID = myPath.LinkID;
                        if (linkID != 0)
                        {
                            double curBcrPos = myPath.CurrentPositionOfLink;
                            check_enable &= curBcrPos > myPath.SteerGuideLengthFromNode;
                            check_enable &= curBcrPos < myPath.Distance - myPath.SteerGuideLengthToNode;
                        }
                        m_MoveComp1 = false;
                        m_MoveComp2 = false;
                        if (myPath.SteerDirection == enSteerDirection.Left) m_SteerRightCheck = true;
                        else if (myPath.SteerDirection == enSteerDirection.Right) m_SteerLeftCheck = true;
                        if (myPath.SteerChangeLeftBCR > 0.0f || myPath.SteerChangeRightBCR > 0.0f || !check_enable)
                        {
                            m_SteerLeftCheck = true;
                            m_SteerRightCheck = true;
                        }
                        // Steer 동작 TEST
                        if (m_SteerLeftCheck && m_SteerRightCheck)
                        {
                            if (m_devSteer.IsValid) m_devSteer.SeqAbort();

                            SequenceLog.WriteLog(FuncName0, string.Format("Steer Action Test Complete"));

                            m_InitSteer.State = InitState.Comp;
                            m_InitSteer.CheckStatus = string.Format("STEER {0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitSteer);
                            seqNo = 100;
                        }
                        else
                        {
                            if (m_devSteer.IsValid) m_devSteer.SeqAbort();

                            SequenceLog.WriteLog(FuncName0, string.Format("Steer Action Test"));
                            enSteerDirection actionDir = enSteerDirection.DontCare;
                            enSteerDirection curDir = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.SteerDirection;
                            if (curDir == enSteerDirection.Right)
                            {
                                if (!m_SteerLeftCheck) actionDir = enSteerDirection.Left;
                                else if (!m_SteerRightCheck) actionDir = enSteerDirection.Right;
                            }
                            else
                            {
                                if (!m_SteerRightCheck) actionDir = enSteerDirection.Right;
                                else if (!m_SteerLeftCheck) actionDir = enSteerDirection.Left;
                            }

                            m_InitSteer.State = InitState.Init;
                            m_InitSteer.CheckStatus = string.Format("{0} {1}", actionDir.ToString(), InitCheckState.Checking);
                            m_EqpManager.SetInitState(m_InitSteer);
                            StartTicks = XFunc.GetTickCount();
                            if (actionDir == enSteerDirection.Right) seqNo = 90;
                            else if (actionDir == enSteerDirection.Left) seqNo = 80;
                            else
                            {
                                m_SteerLeftCheck = true;
                                m_SteerRightCheck = true;
                            }
                        }
                    }
                    break;

                case 80:
                    {
                        // Left 이동.... 시간 Check 하자....!
                        if (!m_MoveComp1)
                        {
                            if (m_devSteer.IsValid)
                                rv1 = m_devSteer.FrontLeft();
                            else rv1 = 0;
                            if (rv1 == 0)
                            {
                                m_devSteer.FrontSteer.ResetOutput();   //25.01.06 重置Output   by XHX
                                m_MoveComp1 = true;
                            }
                        }
                        if (!m_MoveComp2)
                        {
                            if (m_devSteer.IsValid)
                                rv2 = m_devSteer.RearLeft();
                            else rv2 = 0;
                            if (rv2 == 0)
                            {
                                m_devSteer.RearSteer.ResetOutput();
                                m_MoveComp2 = true;
                            }
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            if (m_devSteer.IsValid) m_devSteer.SeqAbort();

                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            m_SteerLeftCheck = true;

                            UInt32 change_time = XFunc.GetTickCount() - StartTicks;
                            SequenceLog.WriteLog(FuncName0, string.Format("Steer Left Move OK ! Moving Time = {0}", change_time));

                            m_InitSteer.CheckStatus = string.Format("Left {0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitSteer);
                            seqNo = 70;
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            if (m_devSteer.IsValid) m_devSteer.SeqAbort();

                            m_InitSteer.State = InitState.Fail;
                            m_InitSteer.CheckStatus = string.Format("Left {0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitSteer);

                            if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceLog.WriteLog(FuncName0, string.Format("FrontSteer Left Move Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else if (rv2 > 0)
                            {
                                AlarmId = rv2;
                                SequenceLog.WriteLog(FuncName0, string.Format("RearSteer Left Move Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;

                case 90:
                    {
                        // Right 이동.... 시간 Check 하자....!
                        if (!m_MoveComp1)
                        {
                            if (m_devSteer.IsValid)
                                rv1 = m_devSteer.FrontRight();
                            else rv1 = 0;
                            if (rv1 == 0)
                            {
                                m_devSteer.FrontSteer.ResetOutput();
                                m_MoveComp1 = true;
                            }
                        }
                        if (!m_MoveComp2)
                        {
                            if (m_devSteer.IsValid)
                                rv2 = m_devSteer.RearRight();
                            else rv2 = 0;
                            if (rv2 == 0)
                            {
                                m_devSteer.RearSteer.ResetOutput();
                                m_MoveComp2 = true;
                            }
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            if (m_devSteer.IsValid) m_devSteer.SeqAbort();

                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            m_SteerRightCheck = true;

                            UInt32 change_time = XFunc.GetTickCount() - StartTicks;
                            SequenceLog.WriteLog(FuncName0, string.Format("Steer Right Move OK ! Moving Time = {0}", change_time));

                            m_InitSteer.CheckStatus = string.Format("Right {0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitSteer);
                            seqNo = 70;
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            if (m_devSteer.IsValid) m_devSteer.SeqAbort();

                            m_InitSteer.State = InitState.Fail;
                            m_InitSteer.CheckStatus = string.Format("Right {0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitSteer);

                            if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceLog.WriteLog(FuncName0, string.Format("FrontSteer Right Move Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else if (rv2 > 0)
                            {
                                AlarmId = rv2;
                                SequenceLog.WriteLog(FuncName0, string.Format("RearSteer Right Move Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;

                case 100:
                    {
                        SequenceLog.WriteLog(FuncName0, string.Format("Transfer Wheel Homing"));

                        m_InitTransfer.State = InitState.Init;
                        m_InitTransfer.CheckStatus = string.Format("{0}", InitCheckState.ServoHoming);
                        m_EqpManager.SetInitState(m_InitTransfer);

                        m_MoveComp1 = false;
                        seqNo = 110;
                    }
                    break;

                case 110:
                    {
                        if (!m_MoveComp1)
                        {
                            if (m_devTransfer.IsValid)
                                rv1 = m_devTransfer.Home();
                            else rv1 = 0;
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Transfer Wheel : Initialize OK"));
                            m_InitTransfer.State = InitState.Comp;
                            m_InitTransfer.CheckStatus = string.Format("{0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitTransfer);
                            seqNo = 120;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Transfer Wheel : Initialize Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            if (m_devTransfer.IsValid) m_devTransfer.SeqAbort();

                            m_InitTransfer.State = InitState.Fail;
                            m_InitTransfer.CheckStatus = string.Format("{0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitTransfer);

                            AlarmId = rv1;
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;

                case 120:
                    {
                        SequenceLog.WriteLog(FuncName0, string.Format("Hoist Homing"));

                        m_InitHoist.State = InitState.Init;
                        m_InitHoist.CheckStatus = string.Format("{0}", InitCheckState.ServoHoming);
                        m_EqpManager.SetInitState(m_InitHoist);

                        m_MoveComp1 = false;
                        seqNo = 130;
                    }
                    break;

                case 130:
                    {
                        if (!m_MoveComp1)
                        {
                            if (m_devFoupGripper.IsValid)
                                rv1 = m_devFoupGripper.Home(enAxisMask.aZ);
                            else rv1 = 0;
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Hoist : Initialize OK"));
                            m_InitHoist.State = InitState.Comp;
                            m_InitHoist.CheckStatus = string.Format("{0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitHoist);
                            seqNo = 140;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Hoist : Initialize Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            if (m_devFoupGripper.IsValid) m_devFoupGripper.SeqAbort();

                            m_InitHoist.State = InitState.Fail;
                            m_InitHoist.CheckStatus = string.Format("{0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitHoist);

                            AlarmId = rv1;
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;

                case 140:
                    {
                        SequenceLog.WriteLog(FuncName0, string.Format("Slide Homing"));

                        m_InitSlide.State = InitState.Init;
                        m_InitSlide.CheckStatus = string.Format("{0}", InitCheckState.ServoHoming);
                        m_EqpManager.SetInitState(m_InitSlide);

                        m_MoveComp1 = false;
                        seqNo = 150;
                    }
                    break;

                case 150:
                    {
                        if (!m_MoveComp1)
                        {
                            if (m_devFoupGripper.IsValid)
                                rv1 = m_devFoupGripper.Home(enAxisMask.aY);
                            else rv1 = 0;
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Slide : Initialize OK"));
                            m_InitSlide.State = InitState.Comp;
                            m_InitSlide.CheckStatus = string.Format("{0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitSlide);
                            seqNo = 160;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Slide : Initialize Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            if (m_devFoupGripper.IsValid) m_devFoupGripper.SeqAbort();

                            m_InitSlide.State = InitState.Fail;
                            m_InitSlide.CheckStatus = string.Format("{0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitSlide);

                            AlarmId = rv1;
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;

                case 160:
                    {
                        SequenceLog.WriteLog(FuncName0, string.Format("Turn Homing"));

                        m_InitTurn.State = InitState.Init;
                        m_InitTurn.CheckStatus = string.Format("{0}", InitCheckState.ServoHoming);
                        m_EqpManager.SetInitState(m_InitTurn);

                        m_MoveComp1 = false;
                        seqNo = 170;
                    }
                    break;

                case 170:
                    {
                        if (!m_MoveComp1)
                        {
                            if (m_devFoupGripper.IsValid)
                                rv1 = m_devFoupGripper.Home(enAxisMask.aT);
                            else rv1 = 0;
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Turn : Initialize OK"));
                            m_InitTurn.State = InitState.Comp;
                            m_InitTurn.CheckStatus = string.Format("{0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitTurn);

                            seqNo = 180;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Turn : Initialize Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            if (m_devFoupGripper.IsValid) m_devFoupGripper.SeqAbort();

                            m_InitTurn.State = InitState.Fail;
                            m_InitTurn.CheckStatus = string.Format("{0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitTurn);

                            AlarmId = rv1;
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;

                case 180:
                    {
                        m_MoveComp1 = false;
                        m_MoveComp2 = false;

                        bool foup_exist = false;
                        if (m_devGripperPio.IsValid)
                        {
                            foup_exist = m_devGripperPio.IsProductExist();
                            foup_exist &= m_devGripperPio.DiGripperClose.IsDetected;
                        }

                        if (foup_exist)
                        {
                            m_InitAntiDrop.State = InitState.Init;
                            m_InitAntiDrop.CheckStatus = string.Format("{0}", InitCheckState.Checking);
                            m_EqpManager.SetInitState(m_InitAntiDrop);

                            // Foup이 있는 상태이므로 Anti Drop을 Lock 하자...
                            SequenceLog.WriteLog(FuncName0, string.Format("Foup Exist, AntiDrop Lock"));
                            seqNo = 190;
                        }
                        else
                        {
                            // Foup이 없으므로 Anti Drop을 Unlock 하자...
                            SequenceLog.WriteLog(FuncName0, string.Format("Foup Not Exist"));
                            seqNo = 200;
                        }
                    }
                    break;

                case 190:
                    {
                        if (!m_MoveComp1)
                        {
                            if (m_devAntiDropFront.IsValid)
                                rv1 = m_devAntiDropFront.Lock();
                            else rv1 = 0;
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (!m_MoveComp2)
                        {
                            if (m_devAntiDropRear.IsValid)
                                rv2 = m_devAntiDropRear.Lock();
                            else rv2 = 0;
                            if (rv2 == 0) m_MoveComp2 = true;
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("AntiDrop Lock OK"));
                            m_InitAntiDrop.State = InitState.Comp;
                            m_InitAntiDrop.CheckStatus = string.Format("Lock {0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitAntiDrop);

                            seqNo = 200;
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            if (m_devAntiDropFront.IsValid) m_devAntiDropFront.SeqAbort();
                            if (m_devAntiDropRear.IsValid) m_devAntiDropRear.SeqAbort();

                            m_InitAntiDrop.State = InitState.Fail;
                            m_InitAntiDrop.CheckStatus = string.Format("Lock {0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitAntiDrop);

                            if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceLog.WriteLog(FuncName0, string.Format("FrontAntiDrop Lock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else if (rv2 > 0)
                            {
                                AlarmId = rv2;
                                SequenceLog.WriteLog(FuncName0, string.Format("RearAntiDrop Lock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;

                case 200:
                    {
                        m_InitCurveRecovery.State = InitState.Init;
                        m_InitCurveRecovery.CheckStatus = string.Format("{0}", InitCheckState.Checking);
                        m_EqpManager.SetInitState(m_InitCurveRecovery);

                        if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.IsCorner())
                        {
                            if (ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.OverrideRatio > 0.0f)
                            {
                                if (m_devTransfer.IsValid)
                                {
                                    // Curve Area Case => Jog Move
                                    ushort propId = m_devTransfer.TeachingVelocitySearch.PropId;
                                    m_SearchingVelSet = m_devTransfer.AxisMaster.GetDevAxis().GetTeachingVel(propId);
                                    m_devTransfer.AxisMaster.GetDevAxis().JogMove(enAxisOutFlag.JogPlus, m_SearchingVelSet);

                                    SequenceLog.WriteLog(FuncName0, string.Format("Curve Recovery Check : AxisMaster JOG Plus Start"));
                                    seqNo = 210;
                                }
                                else
                                {
                                    SequenceLog.WriteLog(FuncName0, string.Format("DevTransfer's IsValid is false ! need Manual Recovery !"));

                                    m_InitCurveRecovery.State = InitState.Fail;
                                    m_InitCurveRecovery.CheckStatus = string.Format("{0}", InitCheckState.NG);
                                    m_EqpManager.SetInitState(m_InitCurveRecovery);

                                    AlarmId = m_ALM_CurveRecoveryFailError.ID;
                                    rv = AlarmId;
                                    seqNo = 0;
                                }
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName0, string.Format("Override Stop Status ! need Manual Recovery !"));

                                m_InitCurveRecovery.State = InitState.Fail;
                                m_InitCurveRecovery.CheckStatus = string.Format("{0}", InitCheckState.NG);
                                m_EqpManager.SetInitState(m_InitCurveRecovery);

                                AlarmId = m_ALM_CurveRecoveryFailError.ID;
                                rv = AlarmId;
                                seqNo = 0;
                            }
                        }
                        else
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Curve Recovery Check OK"));

                            m_InitCurveRecovery.State = InitState.Comp;
                            m_InitCurveRecovery.CheckStatus = string.Format("{0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitCurveRecovery);

                            this.scm.Start = false;
                            this.scm.End = true;
                            rv = 0;
                            seqNo = 0;
                        }
                    }
                    break;

                case 210:
                    {
                        bool move_stop = true;
                        move_stop &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.IsCorner() == false;
                        //move_stop &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Distance > 2000.0f;
                        move_stop &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.CurrentPositionOfLink > 300.0f; //1000.0f;

                        bool move_alarm = false;
                        if (m_devTransfer.IsValid)
                            move_alarm |= m_devTransfer.AxisMaster.GetDevAxis().IsAlarm();

                        if (move_stop)
                        {
                            if (m_devTransfer.IsValid) m_devTransfer.AxisMaster.GetDevAxis().JogStop();
                            SequenceLog.WriteLog(FuncName0, string.Format("Curve Recovery Check : AxisMaster JOG Stop"));
                            SequenceLog.WriteLog(FuncName0, string.Format("Curve Recovery Check OK"));

                            m_InitCurveRecovery.State = InitState.Comp;
                            m_InitCurveRecovery.CheckStatus = string.Format("{0}", InitCheckState.OK);
                            m_EqpManager.SetInitState(m_InitCurveRecovery);

                            this.scm.Start = false;
                            this.scm.End = true;
                            rv = 0;
                            seqNo = 0;
                        }
                        else if (move_alarm)
                        {
                            SequenceLog.WriteLog(FuncName0, string.Format("Override Stop Status ! need Manual Recovery !"));

                            m_InitCurveRecovery.State = InitState.Fail;
                            m_InitCurveRecovery.CheckStatus = string.Format("{0}", InitCheckState.NG);
                            m_EqpManager.SetInitState(m_InitCurveRecovery);

                            AlarmId = m_ALM_CurveRecoveryFailError.ID;
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;
            }
            this.SeqNo = seqNo;
            return rv;
        }
        #endregion
    }

}
