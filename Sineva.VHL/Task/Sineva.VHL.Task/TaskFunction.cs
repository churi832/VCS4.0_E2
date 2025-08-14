using Sineva.VHL.Data;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Device;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.IF.JCS;
using Sineva.VHL.IF.OCS;
using Sineva.VHL.IF.Vision;
using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Library.Servo;
using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Sineva.VHL.Task
{
    /// <summary>
    /// AcquireSequence
    /// 1. Condition Check
    ///    i) CurTransferCommand Valid 유무
    ///    ii) SourceID == currentPortID 확인
    ///    iii) Foup not Exist => Gripper Release 확인
    ///    iv) Buffer Type일 경우 DoubleStorage Sensor ON
    ///    v) AntiDrop Unlock
    /// 2. pio used ? IfFlagRecv.StartReq = true
    /// 3. pio used ? IfFlagRecv.Busy 확인
    /// 4. PBS or IfFlagRecv.ES Interlock Run Set
    /// 5. Rotate Port Teaching Position Move
    /// 6. Slide Port Teaching Position Move
    /// 7. Hoist Port UP Teaching Position Move 
    /// 8. Hoist Port Down Teaching Position Move (sensor미리감지, 도착 sensor미감지 interlcok, sensing method select)
    ///    오류일때 Up/Down 3회 Retry
    ///    비정상 : SEQ11
    /// 9. Gripper Close -> Sensor 감지 위치로 Up -> Foup Exist 확인
    ///    : Foup Exist 확인 되지 않으면 -> SEQ20
    /// 10. Hoist Port UP Teaching Position Move 
    ///    pio used ? IfFlagRecv.OnIng 확인 되지 않으면 -> SEQ20
    /// 11. Hoist Wait Position Move
    /// 12. PBS or IfFlagRecv.ES Interlock Run Reset
    /// 13. Fout Exist 확인 
    ///     pio used ?
    ///      유 : IfFlagRecv.Complete = true
    ///      무 : IfFlagRecv.OperateCancel = true
    /// 14. Slide Wait Position Move
    /// 15. Turn Wait Position Move
    /// 16. Fout Exist 확인
    ///      유 : Acquire Complete
    ///      무 : Acquire Fail
    /// 
    /// 20. Down 위치 이동
    /// 21. Gripper Open -> SEQ11
    /// 
    /// Interlock [PBS or ES]
    /// 1. Motion Stop
    /// 2. Release Check
    ///    Time Check
    /// 3. Release => dev.Abort하고 이전 Sequence 번호 이동(Last Target Position으로 이동)
    /// 4. Time Over =>  Wait<->UP<->DOWN 위치에 따라 구분
    ///     Wait<->UP : SEQ11으로 이동하여 진행
    ///     UP<->DOWN : SEQ20으로 이동하여 진행
    /// </summary>
    public class SeqAcquire : XSeqFunc
    {
        public static readonly string FuncName = "[SeqAcquire]";
        XTimer m_Timer = new XTimer("SeqAcquire");

        #region Fields
        private DevEqPIO m_devEqPio = null;
        private DevFoupGripper m_devFoupGripper = null;
        private DevGripperPIO m_devGripperPio = null;
        private DevAntiDrop m_devAntiDropFront = null;
        private DevAntiDrop m_devAntiDropRear = null;
        private DevOBS m_devLookDown = null;

        private XyztPosition m_WaitPosition = new XyztPosition(); // wait
        private XyztPosition m_Target1Position = new XyztPosition(); // before down
        private XyztPosition m_Target2Position = new XyztPosition(); // down
        private XyztPosition m_Target3Position = new XyztPosition(); // up search
        private List<VelSet> m_Target1VelSets = new List<VelSet>(); // foup not exist move
        private List<VelSet> m_Target2VelSets = new List<VelSet>(); // foup exist move
        private List<VelSet> m_TargetSlowVelSets = new List<VelSet>(); // slow move
        private double m_UpSensorDetectPosition = 0.0f;
        private double m_BeforeUpSensorDetectPosition = 0.0f;
        private int m_StopRetry = 0;
        private bool m_LimitSensorDetectNg = false;
        private int m_MotionRetry = 0;
        private double m_TrackingDiff = 0;
        private double bcrRangeError = 0;

        private bool m_MoveComp1 = false;
        private bool m_MoveComp2 = false;

        private bool m_OldInterlockState = false;
        private bool m_InterlockSet = false;
        private bool m_ESInterlockSet = false;
        private bool m_HOInterlockSet = false;
        private UInt32 m_InterlockCheckStartTime = 0;
        private bool m_AutoRecoveryEnable = false;
        private bool m_UpDetectInterlockSet = false;

        private int m_LimitSensorCheckRetry = 0;
        private AlarmData m_ALM_HoistProductLimitCheckRetryError = null;
        private AlarmData m_ALM_AcquireFailError = null;
        private AlarmData m_ALM_LookDownSensorWait = null;
        private AlarmData m_ALM_AcquiringTorqueLimitAlarm = null;
        private AlarmData m_ALM_FoupCoverNotDetectError = null;
        private AlarmData m_ALM_FoupCoverOpenError = null;
        private AlarmData m_ALM_AcquireFailLookDownSensor = null;
        private AlarmData m_ALM_LookDownSensorStatistics = null; //CRRC Look Down Sensor 감지 되지마자 Warning 발생용.. 단순 통계용이기 때문에 Warning만 발생 시킴.
        private AlarmData m_ALM_AcquireFailESSignal = null;
        private AlarmData m_ALM_AcquireFailHOSignal = null;
        private AlarmData m_ALM_ESSignalWait = null;
        private AlarmData m_ALM_HOSignalWait = null;
        private AlarmData m_ALM_HoistUpSensorAbnormalDetectPositionError = null;
        private AlarmData m_ALM_HoistUpSensorAbnormalDetectPositionWarning = null;
        private AlarmData m_ALM_HoistLimitSensorAbnormalDetectPositionError = null;
        private AlarmData m_ALM_HoistLimitSensorAbnormalDetectPositionWarning = null;
        private AlarmData m_ALM_BcrCheckOverInrangeTargetPositionError = null;

        private bool m_PioAbort = false;
        private bool m_LookDownAlarmFlag = false;
        private bool m_ESSignalAlarmFlag = false;
        private bool m_HOSignalAlarmFlag = false;
        
        #endregion

        #region Constructor
        public SeqAcquire()
        {
            this.SeqName = $"SeqAcquire";

            m_devEqPio = DevicesManager.Instance.DevEqpPIO;
            m_devFoupGripper = DevicesManager.Instance.DevFoupGripper;
            m_devGripperPio = DevicesManager.Instance.DevGripperPIO;
            m_devAntiDropFront = DevicesManager.Instance.DevFrontAntiDrop;
            m_devAntiDropRear = DevicesManager.Instance.DevRearAntiDrop;
            m_devLookDown = DevicesManager.Instance.DevOBSLookDown;


            AddCaseMemo();
            this.TaskLayer = (int)enTaskLayer.layerAcquire;

            m_ALM_HoistProductLimitCheckRetryError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqAcquire", "Acquire Sequence", "Product Limit Check Retry Alarm");
            m_ALM_AcquireFailError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqAcquire", "Acquire Sequence", "Acquire Fail Alarm");
            m_ALM_LookDownSensorWait = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentStatusWarning, false, "SeqAcquire", "Acquire Sequence", "Look Down Interlock Warning");
            m_ALM_AcquiringTorqueLimitAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqAcquire", "Acquire Sequence", "Hoist Torque Limit Alarm");
            m_ALM_FoupCoverNotDetectError = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, "SeqAcquire", "Acquire Sequence", "Foup Cover Not Detect Alarm");
            m_ALM_FoupCoverOpenError = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, "SeqAcquire", "Acquire Sequence", "Foup Cover Open Alarm");
            m_ALM_AcquireFailLookDownSensor = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqAcquire", "Acquire Sequence", "Look Down Sensor Detected Alarm");
            m_ALM_LookDownSensorStatistics = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentStatusWarning, false, "SeqAcquire", "Acquire Sequence", "Look Down Sensor Statistics Warning");
            m_ALM_AcquireFailESSignal = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "DevEqPio", "Acquire Sequence", "PIO ES Signal Off Alarm");
            m_ALM_AcquireFailHOSignal = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "DevEqPio", "Acquire Sequence", "PIO HO Signal Off Alarm");
            m_ALM_ESSignalWait = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentStatusWarning, false, "DevEqPio", "Acquire Sequence", "PIO ES Signal Interlock Warning");
            m_ALM_HOSignalWait = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentStatusWarning, false, "DevEqPio", "Acquire Sequence", "PIO HO Signal Interlock Warning");

            m_ALM_HoistUpSensorAbnormalDetectPositionError = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, "SeqAcquire", "Acquire Sequence", "Port Up Sensor Detected at Abnormal Position Alarm");
            m_ALM_HoistUpSensorAbnormalDetectPositionWarning = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, false, "SeqAcquire", "Acquire Sequence", "OHB Up Sensor Detected at Abnormal Position Warning");
            m_ALM_HoistLimitSensorAbnormalDetectPositionError = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, "SeqAcquire", "Acquire Sequence", "Port Limit Sensor Detected at Abnormal Position Alarm");
            m_ALM_HoistLimitSensorAbnormalDetectPositionWarning = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, false, "SeqAcquire", "Acquire Sequence", "OHB Limit Sensor Detected at Abnormal Position Warning");
            m_ALM_BcrCheckOverInrangeTargetPositionError = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, "SeqAcquire", "Acquire Sequence", "Current Bcr Position Check Over Inrange Target Position Alarm");
        }
        public override void SeqAbort()
        {
            if (AlarmId > 0)
            {
                if (m_AutoRecoveryEnable)
                    ProcessDataHandler.Instance.CurVehicleStatus.SetVehicleStatus(VehicleState.AcquireFailed);

                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            if (this.scm.Ing)
            {
                //Device 초기화
                if (m_devFoupGripper.IsValid) m_devFoupGripper.SeqAbort();
                if (m_devGripperPio.IsValid) m_devGripperPio.SeqAbort();
                if (m_devAntiDropFront.IsValid) m_devAntiDropFront.SeqAbort();
                if (m_devAntiDropRear.IsValid) m_devAntiDropRear.SeqAbort();
                if (m_devLookDown.IsValid) m_devLookDown.SeqAbort();
                this.scm.Abort = true;
            }
            if (m_devLookDown.IsValid) m_devLookDown.SetNoUse();
            this.InitSeq();
        }
        private void AddCaseMemo()
        {
            try
            {
                this.SeqCaseMemoLists.Add(0, string.Format("Acquire Sequence Start !"));
                this.SeqCaseMemoLists.Add(1, string.Format("Acquire Motor Condition Check !"));
                this.SeqCaseMemoLists.Add(10, string.Format("Acquire Interlock Condition Check !"));
                this.SeqCaseMemoLists.Add(20, string.Format("Anti Drop Unlock Check"));
                this.SeqCaseMemoLists.Add(30, string.Format("Gripper Open Check"));
                this.SeqCaseMemoLists.Add(40, string.Format("Look Down Sensor OFF Check"));
                this.SeqCaseMemoLists.Add(50, string.Format("PIO Interface Start Wait"));
                this.SeqCaseMemoLists.Add(100, string.Format("Acquire Position & Velocity Setting"));
                this.SeqCaseMemoLists.Add(110, string.Format("Slide & Rotate Before Down Position Move"));
                this.SeqCaseMemoLists.Add(120, string.Format("Hoist Before Down Position Move"));
                this.SeqCaseMemoLists.Add(130, string.Format("Hoist Down Position Move"));
                this.SeqCaseMemoLists.Add(140, string.Format("Hoist Down + DownRangeLimit Position Move"));
                this.SeqCaseMemoLists.Add(150, string.Format("Hoist Move Stop"));
                this.SeqCaseMemoLists.Add(153, string.Format("Check Up Sensor Detected"));
                this.SeqCaseMemoLists.Add(155, string.Format("The sensor is detected in the wrong Hoist position and serves to check the sensor"));
                this.SeqCaseMemoLists.Add(160, string.Format("Hoist Current + (-3mm) Position Setting"));
                this.SeqCaseMemoLists.Add(170, string.Format("Hoist Current + (-3mm) Position Move"));
                this.SeqCaseMemoLists.Add(200, string.Format("Gripper Close Enable Condition Confirm"));
                this.SeqCaseMemoLists.Add(210, string.Format("Hoist Before Down Position Retry Move"));
                this.SeqCaseMemoLists.Add(300, string.Format("Gripper Close"));
                this.SeqCaseMemoLists.Add(310, string.Format("Hoist Before Down Position Move"));
                this.SeqCaseMemoLists.Add(320, string.Format("UL REQ OFF & Foup Exist Check"));
                this.SeqCaseMemoLists.Add(350, string.Format("Recovery Hoist Down Position Move"));
                this.SeqCaseMemoLists.Add(360, string.Format("Recovery Gripper Open"));
                this.SeqCaseMemoLists.Add(370, string.Format("Recovery Foup Not Exist Check"));
                this.SeqCaseMemoLists.Add(400, string.Format("Recovery Hoist Move Stop"));
                this.SeqCaseMemoLists.Add(410, string.Format("Recovery Hoist & Slide & Rotate Home Position Move"));
                this.SeqCaseMemoLists.Add(450, string.Format("Hoist & Slide & Rotate Wait Position Move"));
                this.SeqCaseMemoLists.Add(500, string.Format("Foup Exist Check"));
                this.SeqCaseMemoLists.Add(510, string.Format("PIO Disconnect Complete Confirm"));
                this.SeqCaseMemoLists.Add(520, string.Format("Recovery Foup Exist. PIO Complete Confirm"));
                this.SeqCaseMemoLists.Add(600, string.Format("Recovery PIO Alarm Acquire Cancel"));
                this.SeqCaseMemoLists.Add(610, string.Format("Recovery PIO Disconnect Complete Confirm"));
                this.SeqCaseMemoLists.Add(900, string.Format("Anti Drop Lock"));
                this.SeqCaseMemoLists.Add(1000, string.Format("Alarm Reset Wait"));
                this.SeqCaseMemoLists.Add(2000, string.Format("PIO Interlock Signal Release Wait"));
                this.SeqCaseMemoLists.Add(2010, string.Format("PIO Interlock Timeout Wait"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("{0} EXCEPTION : {1}", FuncName, ex.ToString()));
            }
        }
        #endregion

        #region Override
        public override int Do()
        {
            if (this.scm.Pause) return -1;
            if (AppConfig.Instance.VehicleType == VehicleType.Clean) return -1;

            int rv = -1;
            int rv1 = -1;
            int rv2 = -1;
            int rv3 = -1;

            SaftyCheck();
            bool carrier_installed = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
            int target_port = ProcessDataHandler.Instance.CurTransferCommand.SourceID;
            int current_port = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortID;
            bool ohb = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftBuffer;
            ohb |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightBuffer;

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        m_AutoRecoveryEnable = false;
                        this.scm.Start = true;
                        StartTicks = XFunc.GetTickCount();
                        m_PioAbort = false;
                        m_LookDownAlarmFlag = false;
                        m_ESSignalAlarmFlag = false;
                        m_HOSignalAlarmFlag = false;
                        m_MotionRetry = 0;

                        if (SetupManager.Instance.SetupPIO.PioAllAutoUsing) //Teaching 진행하고 확인할때.. 설비 PIO 준비가 안되어있는 경우가 많아서.. 해제 방법은 필요..Demo 같은 경우는 아예 PIO가 없기도 하고..
                        {
                            bool always_use = false;
                            always_use |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftSTKPort;
                            always_use |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightSTKPort;
                            always_use |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftEQPort;
                            always_use |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightEQPort;
                            ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed = always_use;
                        }
                        m_devLookDown.SetNoUse();

                        seqNo = 1;
                    }
                    break;

                case 1:
                    // Loader Z축 간섭 위치 확인
                    {
                        rv1 = CheckDeviceMotor();
                        if (rv1 == 0)
                        {
                            m_LimitSensorCheckRetry = 0;
                            SequenceLog.WriteLog(FuncName, "Acquire Start");
                            seqNo = 10;
                        }
                        else if (rv1 > 0 && (XFunc.GetTickCount() - StartTicks > 5000))
                        {
                            m_AutoRecoveryEnable = true;
                            EqpAlarm.Set(rv1);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 10:
                    {
                        bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                        // SeqAcquire에서 Check 하고 StartReq 하자
                        if (IsAntiDropUnlock() == false)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("AntiDrop Unlock NG"));
                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20; // 20으로 이동하여 Unlock 실행하자!
                        }
                        else if (m_devGripperPio.IsGripperClose())
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Gripper Close State. Open Try"));
                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            seqNo = 30;
                        }
                        else if (m_InterlockSet)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Look Down Sensor ON State. Wait OFF"));
                            StartTicks = XFunc.GetTickCount();

                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                            {
                                AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            SequenceLog.WriteLog(FuncName, string.Format("The first detection of the Look Down Sensor has begun. - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            seqNo = 40;
                        }
                        else if (ProcessDataHandler.Instance.CurTransferCommand.IsValid == false)
                        {
                            m_AutoRecoveryEnable = true;
                            AlarmId = ProcessDataHandler.Instance.ALM_CommandNotExistError.ID;
                            SequenceLog.WriteLog(FuncName, string.Format("Port Unloading Interface, Transfer Command Not Valid Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                        else if (current_port != target_port)
                        {
                            m_AutoRecoveryEnable = true;
                            AlarmId = ProcessDataHandler.Instance.ALM_SourcePortMismatchError.ID;
                            SequenceLog.WriteLog(FuncName, string.Format("Source ID isn't same Current Port ID - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                        else if (!IsFoupNotExist() || carrier_installed)
                        {
                            m_AutoRecoveryEnable = true;
                            if (m_devGripperPio.DiLeftProductExist.IsDetected)
                                AlarmId = m_devGripperPio.ALM_ProductLeftExistAlarm.ID;
                            else AlarmId = m_devGripperPio.ALM_ProductRightExistAlarm.ID;
                            SequenceLog.WriteLog(FuncName, string.Format("Port Unloading Interface, Foup Exist Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                        else if (IsDoubleStorage() && !AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_AutoRecoveryEnable = true;
                            ProcessDataHandler.Instance.CurVehicleStatus.SetVehicleStatus(VehicleState.SourceEmpty);
                            OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.SourceEmpty);

                            if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftBuffer)
                                AlarmId = m_devFoupGripper.ALM_LeftDoubleStorageError.ID;
                            else AlarmId = m_devFoupGripper.ALM_RightDoubleStorageError.ID;
                            SequenceLog.WriteLog(FuncName, string.Format("Port Unloading Interface, Double Storage Check, Foup Not Exist at Source Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            //EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 3000;
                        }
                        else
                        {
                            if (pio_used)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("PIO Use, Interface Start!"));

                                m_devEqPio.IfFlagSend.Reset();//반송 Case에 맞지않는 Flag는 끄자
                                m_devEqPio.IfFlagRecv.Start();
                                seqNo = 50;
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("PIO NoUse, FoupGripper Action Start"));
                                seqNo = 100;
                            }
                        }
                    }
                    break;

                case 20:
                    {
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devAntiDropFront.Unlock();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (!m_MoveComp2)
                        {
                            rv2 = m_devAntiDropRear.Unlock();
                            if (rv2 == 0) m_MoveComp2 = true;
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            //Target까지 움직였는데 Sensor가 감지가 안될경우 무언정지된다..
                            if (IsAntiDropUnlock() == true)
                            {
                                m_MoveComp1 = false;
                                m_MoveComp2 = false;
                                SequenceLog.WriteLog(FuncName, string.Format("AntiDrop Unlock OK"));
                                seqNo = 0;
                            }
                            else if ((XFunc.GetTickCount() - StartTicks > 3000))
                            {
                                if (m_devAntiDropFront.IsValid ? m_devAntiDropFront.GetUnlock() == false : false)
                                {
                                    AlarmId = m_devAntiDropFront.ALM_AntiDropNotDetectBWAlarm.ID;
                                    EqpAlarm.Set(AlarmId);
                                }
                                else if (m_devAntiDropRear.IsValid ? m_devAntiDropRear.GetUnlock() == false : false)
                                {
                                    AlarmId = m_devAntiDropRear.ALM_AntiDropNotDetectBWAlarm.ID;
                                    EqpAlarm.Set(AlarmId);
                                }

                                m_AutoRecoveryEnable = false;
                                SequenceLog.WriteLog(FuncName, $"Anti-Drop has completed moving, but the sensor is not detected. FW:{m_devAntiDropFront.GetUnlock()}, BW:{m_devAntiDropRear.GetUnlock()}");
                                ReturnSeqNo = 0;
                                seqNo = 1000;
                            }
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            m_AutoRecoveryEnable = true;
                            if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceLog.WriteLog(FuncName, string.Format("FrontAntiDrop Unlock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else if (rv2 > 0)
                            {
                                AlarmId = rv2;
                                SequenceLog.WriteLog(FuncName, string.Format("RearAntiDrop Unlock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 30:
                    {
                        rv1 = m_devGripperPio.GripperOpen();
                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Gripper Open OK"));
                            seqNo = 0;
                        }
                        else if (rv1 > 0)
                        {
                            m_AutoRecoveryEnable = true;
                            AlarmId = rv1;
                            SequenceLog.WriteLog(FuncName, string.Format("Gripper Open Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 40:
                    {
                        if (m_InterlockSet == false)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Look Down Sensor OFF OK"));
                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                            {
                                AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                EqpAlarm.Reset(AlarmId);
                            }
                            seqNo = 0;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 10000)
                        {
                            m_AutoRecoveryEnable = true;

                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                            {
                                AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                EqpAlarm.Reset(AlarmId);
                            }

                            if (m_ESInterlockSet)
                            {
                                AlarmId = m_ALM_ESSignalWait.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("PIO ES Signal ON Timeover Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else if (m_HOInterlockSet)
                            {
                                AlarmId = m_ALM_HOSignalWait.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("PIO HO Signal ON Timeover Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else
                            {
                                AlarmId = m_ALM_LookDownSensorWait.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("Look Down Sensor ON Timeover Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 50:
                    {
                        if (m_devEqPio.IfFlagRecv.Busy)
                        {
                            m_devEqPio.IfFlagRecv.Busy = false;
                            SequenceLog.WriteLog(FuncName, string.Format("PIO Busy, FoupGripper Action Start"));
                            seqNo = 100;
                        }
                        else if (m_devEqPio.IfFlagRecv.Cancel)
                        {
                            m_devEqPio.IfFlagRecv.Cancel = false;
                            SequenceLog.WriteLog(FuncName, string.Format("PIO Cancel, PIO Fail"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 600;
                        }
                    }
                    break;

                case 100:
                    {
                        m_LimitSensorDetectNg = false;

                        bool pbs_use = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PBSUsed;
                        if (pbs_use)
                        {
                            uint pbs_no = (uint)ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PBSSelectNo;
                            m_devLookDown.SetOBS(pbs_no);
                        }
                        // Calculate Position
                        m_WaitPosition = m_devFoupGripper.GetTeachingPosition(m_devFoupGripper.TeachingPointWait.PosId);
                        m_Target1Position = m_devFoupGripper.GetBeforeDownPos(target_port, carrier_installed); // Before Down Position
                        m_Target2Position = m_devFoupGripper.GetDownPos(target_port, carrier_installed, true); // Down Position, Sensor Search No-Use
                        ushort down_prop = IsPortTypeBuffer() ? m_devFoupGripper.TeachingVelocityBufDown.PropId : m_devFoupGripper.TeachingVelocityDown.PropId;
                        ushort up_prop = IsPortTypeBuffer() ? m_devFoupGripper.TeachingVelocityBufUp.PropId : m_devFoupGripper.TeachingVelocityUp.PropId;
                        m_Target1VelSets = m_devFoupGripper.GetTeachingVelSets(down_prop);
                        m_Target2VelSets = m_devFoupGripper.GetTeachingVelSets(up_prop);
                        m_TargetSlowVelSets = m_devFoupGripper.GetTeachingVelSets(m_devFoupGripper.TeachingVelocityLow.PropId); // 저속으로 이동하면서 Sensor Detect 시점을 찾자 !
                        if (m_WaitPosition != null && m_Target1Position != null && m_Target2Position != null && m_Target1VelSets != null && m_TargetSlowVelSets != null && m_Target2VelSets != null)
                        {
                            double offset = SetupManager.Instance.SetupHoist.HoistSensorDetectMoveDistance;
                            m_UpSensorDetectPosition = m_Target2Position.Z - offset; // GetDownPos에서 offset을 + 했던 값.
                            m_BeforeUpSensorDetectPosition = m_Target2Position.Z - offset;
                            ///////////////////////////////////////////////////////////////
                            string msg = "Velocity:[";
                            foreach (VelSet set in m_Target1VelSets) msg += string.Format("[{0},{1},{2},{3}],", set.Vel, set.Acc, set.Dec, set.Jerk);
                            foreach (VelSet set in m_Target2VelSets) msg += string.Format("[{0},{1},{2},{3}],", set.Vel, set.Acc, set.Dec, set.Jerk);
                            foreach (VelSet set in m_TargetSlowVelSets) msg += string.Format("[{0},{1},{2},{3}],", set.Vel, set.Acc, set.Dec, set.Jerk);
                            msg += "]";
                            ///////////////////////////////////////////////////////////////
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit Move Start - DOWN1:[{0}], DOWN2:[{1}], Velocity:[{2}]", m_Target1Position.ToString(), m_Target2Position.ToString(), msg));
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            seqNo = 110;
                        }
                        else
                        {
                            m_AutoRecoveryEnable = true;
                            if (m_Target1VelSets == null || m_TargetSlowVelSets == null || m_Target2VelSets == null)
                            {
                                AlarmId = m_devFoupGripper.ALM_SettingError.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("DevFoupGripper TeachingVelocity isn't TeachingVelocityDown Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else
                            {
                                AlarmId = ProcessDataHandler.Instance.ALM_SourcePortMismatchError.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("Teaching Port Database isn't Port ID = [{0}] Alarm - {1}", target_port, EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 110:
                    {
                        // Slide & Rotate Port Teaching Position Move
                        if (!m_MoveComp1)
                        {
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                                rv1 = m_devFoupGripper.ContinuousMove(enAxisMask.aYT, m_Target1Position, m_Target1VelSets);
                            else
                                rv1 = m_devFoupGripper.Move(enAxisMask.aY | enAxisMask.aT, m_Target1Position, m_Target1VelSets);

                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                        {
                            if (Math.Abs(m_devFoupGripper.AxisSlide.GetDevAxis().GetCurPosition() - m_Target1Position.Y) < Math.Abs(m_Target1Position.Y * 0.1f) || m_MoveComp1)
                            {
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.AcquireStart);

                                m_MoveComp1 = false;
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Continuous_Motion_Use", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                                seqNo = 120;
                            }
                        }
                        if (m_MoveComp1)
                        {
                            OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.AcquireStart);

                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Before Down Move OK", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            if (SetupManager.Instance.SetupHoist.HoistTwoStepDown)
                            {
                                seqNo = 120;
                            }
                            else seqNo = 130;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Before Down Move NG", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            seqNo = 400;
                        }
                    }
                    break;

                case 120:
                    {
                        // Hoist Port Before Down Teaching Position Move
                        if (!m_MoveComp1)
                        {
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                            {
                                rv1 = m_devFoupGripper.ContinuousMove(enAxisMask.aZ, m_Target1Position, m_Target1VelSets);
                            }
                            else
                            {
                                rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target1Position, m_Target1VelSets);
                            }

                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_devGripperPio.IsHoistLimit() || m_devGripperPio.IsHoistUp())
                        {
                            if (m_devGripperPio.IsHoistLimit()) m_LimitSensorDetectNg = true;
                            else if (m_devGripperPio.IsHoistUp()) m_UpDetectInterlockSet = true;

                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                m_devGripperPio.DiHoistHome.SetDi(false);
                                m_devGripperPio.DiHoistUp.SetDi(false);
                                m_devGripperPio.DiHoistLimit.SetDi(false);
                            }
                            string detect_sensor = m_devGripperPio.IsHoistUp() ? "UP Detect" : "Product Limit Detect";
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move NG ({1})", m_devFoupGripper.AxisHoist.AxisName, detect_sensor));
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;

                            if (ohb)
                            {
                                seqNo = 400;
                            }
                            else
                            {
                                AlarmId = m_ALM_HoistLimitSensorAbnormalDetectPositionError.ID;
                                EqpAlarm.Set(AlarmId);
                                m_AutoRecoveryEnable = false;
                                ReturnSeqNo = 0;
                            }
                        }
                        else if (m_InterlockSet)
                        {
                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move Interlock", m_devFoupGripper.AxisHoist.AxisName));
                            m_devFoupGripper.SeqAbort();
                            m_InterlockCheckStartTime = XFunc.GetTickCount();

                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                            {
                                AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                EqpAlarm.Set(AlarmId);
                            }

                            SequenceLog.WriteLog(FuncName, string.Format("The first detection of the Look Down Sensor has begun. - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            ReturnSeqNo = seqNo;
                            seqNo = 2000;
                        }
                        else if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                        {
                            VelSet set = m_TargetSlowVelSets.Find(x => x.AxisCoord == enAxisCoord.Z);

                            bool continuous_Hoist_Enable = false;
                            if (ohb)
                            {
                                continuous_Hoist_Enable |= Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition() - m_Target1Position.Z) < 15.0f;
                                continuous_Hoist_Enable &= Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCommandVelocity()) < set.Vel * 3.0f;
                            }
                            else
                            {
                                continuous_Hoist_Enable |= Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition() - m_Target1Position.Z) < 35.0f;
                                continuous_Hoist_Enable &= Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCommandVelocity()) < set.Vel * 10.0f;
                            }

                            if (continuous_Hoist_Enable || m_MoveComp1)
                            {
                                m_MoveComp1 = false;
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Continuous Hoist Slow Move Start!", m_devFoupGripper.AxisHoist.AxisName));
                                SequenceLog.WriteLog(FuncName, $"Pos : {m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition()}, Target : {m_Target1Position.Z}, Diff : {Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition() - m_Target1Position.Z)}" +
                                    $"Vel : {m_devFoupGripper.AxisHoist.GetDevAxis().GetCommandVelocity()}, SetVel : {set.Vel}");
                                double limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitOHB : SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitPort;
                                m_Target2Position.Z += limit;
                                m_MoveComp2 = true;
                                seqNo = 130;
                            }
                        }
                        else if (m_MoveComp1)
                        {
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                            {
                                double limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitOHB : SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitPort;
                                m_Target2Position.Z += limit;
                            }
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move OK", m_devFoupGripper.AxisHoist.AxisName));

                            seqNo = 130;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            seqNo = 400;
                        }
                    }
                    break;

                case 130:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if ((GV.SimulationFlag & enSimulationFlag.HoistLimitDetect_Acquire) == enSimulationFlag.HoistLimitDetect_Acquire)
                            {
                                m_devGripperPio.DiHoistLimit.SetDi(true);
                            }
                        }
                        // Hoist Port Down Teaching Position Move
                        double up_limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitOHB :
                                                SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitPort;
                        double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                        bool up_sensor_interlock = cur_position > (m_BeforeUpSensorDetectPosition + up_limit) && m_devGripperPio.IsHoistUp(); //Before Down 보다 높은 위치에서 UP 감지
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            int ntest = new Random().Next(3, 8);
                            if (Math.Abs(cur_position - m_Target2Position.Z) < ntest)
                            {
                                m_devGripperPio.DiHoistHome.SetDi(true);
                                m_devGripperPio.DiHoistUp.SetDi(true);
                            }
                        }
                        // Hoist Port Teaching Position Move
                        if (!m_MoveComp1)
                        {
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                            {
                                if (SetupManager.Instance.SetupHoist.HoistTwoStepDown)
                                    rv1 = m_devFoupGripper.ContinuousMove(enAxisMask.aZ, m_Target2Position, m_TargetSlowVelSets, m_MoveComp2);
                                else rv1 = m_devFoupGripper.ContinuousMove(enAxisMask.aZ, m_Target2Position, m_Target1VelSets, m_MoveComp2);
                                if (m_MoveComp2) m_MoveComp2 = false;
                            }
                            else
                            {
                                if (SetupManager.Instance.SetupHoist.HoistTwoStepDown)
                                    rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target2Position, m_TargetSlowVelSets);
                                else rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target2Position, m_Target1VelSets);
                            }
                            if (rv1 == 0) m_MoveComp1 = true;
                        }

                        if (m_devGripperPio.IsHoistLimit() && !m_LimitSensorDetectNg) //처음 내려갈때 확인
                        {
                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                m_devGripperPio.DiHoistHome.SetDi(false);
                                m_devGripperPio.DiHoistUp.SetDi(false);
                                m_devGripperPio.DiHoistLimit.SetDi(false);
                            }

                            m_LimitSensorDetectNg = true;

                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down Move NG (Product Limit Detect, {1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            m_devFoupGripper.SeqAbort();

                            m_MoveComp1 = false;

                            if (ohb)
                            {
                                seqNo = 400;
                            }
                            else
                            {
                                AlarmId = m_ALM_HoistLimitSensorAbnormalDetectPositionError.ID;
                                EqpAlarm.Set(AlarmId);
                                m_AutoRecoveryEnable = false;
                                ReturnSeqNo = 0;
                            }
                        }
                        else if (up_sensor_interlock)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#1 and Range Over ({1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            m_UpSensorDetectPosition = cur_position;
                            m_devFoupGripper.SeqAbort();
                            seqNo = 150;
                        }
                        else if (m_devGripperPio.IsHoistUp())
                        {
                            m_MoveComp1 = false;
                            m_UpSensorDetectPosition = cur_position;
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#1 ({1}), Continuous Motion. Check Exact Position", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                                StartTicks = XFunc.GetTickCount();
                                m_MoveComp2 = true;
                                seqNo = 215;
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#1 ({1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                                m_devFoupGripper.SeqAbort();
                                m_StopRetry = 0;
                                seqNo = 150;
                            }
                        }
                        else if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down Move OK ({1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));

                            m_Target3Position = ObjectCopier.Clone(m_Target2Position);
                            double limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitOHB : SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitPort;
                            double up_offset = m_LimitSensorDetectNg ? 2 * m_LimitSensorCheckRetry : 0.0f; //Limit가 감지된다 좀 올리자~~~
                            m_Target3Position.Z = m_BeforeUpSensorDetectPosition + limit + up_offset;

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 140;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down Move NG (rv={1})", m_devFoupGripper.AxisHoist.AxisName, rv1));
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            seqNo = 400;
                        }
                    }
                    break;

                case 140:
                    {
                        // Hoist Port Down + DownRangeLimit Position Move
                        double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target3Position, m_TargetSlowVelSets);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }

                        if (m_devGripperPio.IsHoistLimit() && !m_LimitSensorDetectNg)
                        {
                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                m_devGripperPio.DiHoistHome.SetDi(false);
                                m_devGripperPio.DiHoistUp.SetDi(false);
                            }

                            m_LimitSensorDetectNg = true;

                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down + DownRangeLimit Move NG (Product Limit Detect, {1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;

                            if (ohb)
                            {
                                seqNo = 400;
                            }
                            else
                            {
                                AlarmId = m_ALM_HoistLimitSensorAbnormalDetectPositionError.ID;
                                EqpAlarm.Set(AlarmId);
                                m_AutoRecoveryEnable = false;
                                ReturnSeqNo = 0;
                            }
                        }
                        else if (m_devGripperPio.IsHoistUp())
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#2 ({1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            m_UpSensorDetectPosition = cur_position;
                            m_devFoupGripper.SeqAbort();
                            StartTicks = XFunc.GetTickCount();
                            m_StopRetry = 0;
                            seqNo = 150;
                        }
                        else if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down + DownRangeLimit Move OK ({1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 200;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down + DownRangeLimit Move NG (rv={1})", m_devFoupGripper.AxisHoist.AxisName, rv1));
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            seqNo = 400;
                        }
                    }
                    break;

                case 150:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 200) break;
                        // Hoist Stop
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.AxisHoist.GetDevAxis().Stop();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }

                        if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            double error = Math.Abs(m_UpSensorDetectPosition - m_BeforeUpSensorDetectPosition);
                            double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Stop OK {{0}}", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            StartTicks = XFunc.GetTickCount();

                            if (m_devGripperPio.IsHoistUp())
                            {
                                if (error < 3.0f)
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#3 ({1}, {2})", m_devFoupGripper.AxisHoist.AxisName, cur_position, error));
                                    seqNo = 160;
                                }
                                else
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#3 and Range Over ({1}, {2})", m_devFoupGripper.AxisHoist.AxisName, cur_position, error));
                                    seqNo = 153;
                                }
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up NotDetect#3 ({1}, {2})", m_devFoupGripper.AxisHoist.AxisName, cur_position, error));
                                seqNo = 130;
                            }
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Stop NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            seqNo = 400;
                        }
                    }
                    break;

                case 153:
                    {
                        bool hoistup = m_devGripperPio.IsHoistUp();
                        // Hoist Port Down Teaching Position Move
                        double up_limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitOHB :
                                                SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitPort;
                        double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                        bool up_sensor_interlock = m_UpSensorDetectPosition > (m_BeforeUpSensorDetectPosition + up_limit) && hoistup; //Before Down 보다 높은 위치에서 UP 감지, 센서 감지된 위치끼리 비교해야 좀더 명확해짐.
                        if (up_sensor_interlock)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#4 and Range Over ({1})({2})", m_devFoupGripper.AxisHoist.AxisName, cur_position, m_UpSensorDetectPosition));
                            seqNo = 155;
                        }
                        else if (!hoistup)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up NotDetect#4 ({1})({2})", m_devFoupGripper.AxisHoist.AxisName, cur_position, m_UpSensorDetectPosition));
                            seqNo = 130;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 400)// Up Sensor가 정상 감지했을 경우.
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#4 ({1})({2})", m_devFoupGripper.AxisHoist.AxisName, cur_position, m_UpSensorDetectPosition));
                            seqNo = 160;
                        }
                    }
                    break;

                case 155:
                    {
                        bool hoistup = m_devGripperPio.IsHoistUp();
                        if (!hoistup)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Up Sensor Monitoring Result : Not Detected After Delay.", m_devFoupGripper.AxisHoist.AxisName));
                            seqNo = 130;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 1000)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Up Sensor Monitoring Result : Detected in Abnormal Hoist Position.", m_devFoupGripper.AxisHoist.AxisName));

                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                m_devGripperPio.DiHoistHome.SetDi(false);
                                m_devGripperPio.DiHoistUp.SetDi(false);
                            }

                            if (ohb)
                            {
                                m_UpDetectInterlockSet = true;
                                m_MoveComp1 = false;
                                m_devFoupGripper.SeqAbort();
                                seqNo = 400;
                            }
                            else
                            {
                                if (IsBcrOverInange(ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus, current_port))
                                {
                                    AlarmId = m_ALM_BcrCheckOverInrangeTargetPositionError.ID;
                                    SequenceLog.WriteLog(FuncName, string.Format("BCR Check Over Inrange Target Position Alarm - BcrInrageError:{0}", bcrRangeError));
                                }
                                else
                                {
                                    AlarmId = m_ALM_HoistUpSensorAbnormalDetectPositionError.ID;
                                }
                                EqpAlarm.Set(AlarmId);
                                m_AutoRecoveryEnable = false;
                                ReturnSeqNo = 0;
                                seqNo = 1000;
                            }
                        }
                    }
                    break;

                case 160:
                    {
                        if (m_devFoupGripper.AxisHoist.GetDevAxis().IsAxisReady())
                        {
                            m_devFoupGripper.SeqAbort();
                            double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();

                            // CurrentPosition + HoistSensorDetectMoveDistance(-3mm) Move
                            double offset = SetupManager.Instance.SetupHoist.HoistSensorDetectMoveDistance;
                            m_Target3Position.Z = m_UpSensorDetectPosition + offset;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) UpDetect + HoistSensorDetectMoveDistance Move Start ({1}, {2})", m_devFoupGripper.AxisHoist.AxisName, m_Target3Position.Z, cur_position));
                            seqNo = 170;
                        }
                        else if (m_StopRetry > 3)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Stop TimeOver NG", m_devFoupGripper.AxisHoist.AxisName, m_StopRetry));
                            m_StopRetry = 0;
                            seqNo = 200;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 2000)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Stop TimeOver Retry {1}", m_devFoupGripper.AxisHoist.AxisName, m_StopRetry));
                            m_StopRetry++;
                            seqNo = 150;
                        }
                    }
                    break;

                case 170:
                    {
                        double minus_limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitOHB : SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitPort;
                        bool limit_skip = true;
                        limit_skip &= m_Target3Position.Z > m_Target2Position.Z + minus_limit;
                        limit_skip &= m_Target3Position.Z < m_Target2Position.Z + 5.0;

                        // HoistSensorDetectMoveDistance(-3mm) Move
                        double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target3Position, m_TargetSlowVelSets);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_devGripperPio.IsHoistLimit() && !limit_skip && !m_LimitSensorDetectNg)
                        {
                                if (AppConfig.Instance.Simulation.MY_DEBUG)
                                {
                                    m_devGripperPio.DiHoistHome.SetDi(false);
                                    m_devGripperPio.DiHoistUp.SetDi(false);
                                    m_devGripperPio.DiHoistLimit.SetDi(false);
                                }

                                m_LimitSensorDetectNg = true;

                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) UpDetect + HoistSensorDetectMoveDistance Move NG (Product Limit Detect, {1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            seqNo = 400;
                        }
                        else if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) UpDetect + HoistSensorDetectMoveDistance Move OK ({1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 200;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) UpDetect + HoistSensorDetectMoveDistance Move NG (rv={1})", m_devFoupGripper.AxisHoist.AxisName, rv1));
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            seqNo = 400;
                        }
                    }
                    break;

                case 200:
                    {
                        /////////////////////////////////////////////////////////////////////////////////////////////////////
                        /// Sensor 유효Range : down position +- Range Limit
                        /// min = down position + DownRangeLimit
                        /// max = down position + UpRangeLimit
                        /// min < cur position < max
                        /////////////////////////////////////////////////////////////////////////////////////////////////////
                        double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                        double up_limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitOHB :
                            SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitPort;
                        double down_limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitOHB :
                            SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitPort;

                        double min = m_BeforeUpSensorDetectPosition + down_limit;
                        double max = m_BeforeUpSensorDetectPosition + up_limit;

                        bool close_enable = IsGripCloseEnableCondition();
                        close_enable &= cur_position < max;
                        close_enable &= cur_position > min;

                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if ((GV.SimulationFlag & enSimulationFlag.UpRangeOver_Acquire) == enSimulationFlag.UpRangeOver_Acquire || (GV.SimulationFlag & enSimulationFlag.InRangeOver_Acquire) == enSimulationFlag.InRangeOver_Acquire)
                            {
                                close_enable = false;
                            }
                        }

                        if (close_enable)
                        {
                            m_MoveComp1 = false;
                            // Tracking Offset Update ////////////////////////////////////////////////////////
                            //double error = m_UpSensorDetectPosition - m_BeforeUpSensorDetectPosition; // current up sensor detect - before detect 
                            //TeachingOffsetAdapter.Instance.SetAutoTrackingValue(true, target_port, error);
                            //System.Diagnostics.Debug.WriteLine($"[SeqAcquire] [{error}], [{min}<{cur_position}<{max}]");
                            //여기에서 Tracking 값을 갱신하는 경우 Error가 발생했을때도 동일하게 갱신이 된다.. 위치 이동..
                            m_TrackingDiff = m_UpSensorDetectPosition - m_BeforeUpSensorDetectPosition;
                            //////////////////////////////////////////////////////////////////////////////////
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit UP Sensor Detected, min={0} < curPosition={1} < max={2}, error={3}", min, cur_position, max, m_TrackingDiff));
                            seqNo = 300;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 1000)
                        {
                            if (!ohb)
                            {
                                if (AppConfig.Instance.Simulation.MY_DEBUG)
                                {
                                    m_devGripperPio.DiHoistHome.SetDi(false);
                                    m_devGripperPio.DiHoistUp.SetDi(false);
                                }

                                if (IsBcrOverInange(ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus, current_port))
                                {
                                    AlarmId = m_ALM_BcrCheckOverInrangeTargetPositionError.ID;
                                    SequenceLog.WriteLog(FuncName, string.Format("BCR Check Over Inrange Target Position Alarm - BcrInrageError:{0}", bcrRangeError));
                                }
                                else
                                {
                                    AlarmId = m_ALM_HoistUpSensorAbnormalDetectPositionError.ID;
                                }
                                EqpAlarm.Set(AlarmId);
                                m_AutoRecoveryEnable = false;
                                ReturnSeqNo = 0;
                                seqNo = 1000;
                            }
                            else
                            {
                                if (!IsGripCloseEnableCondition())
                                    SequenceLog.WriteLog(FuncName, string.Format("IsGripCloseEnableCondition = false, Hoist Limit:{0},Hoist Up:{1},Hoist Home:{2}, Search Retry:{3}",
                                        m_devGripperPio.IsHoistLimit(), m_devGripperPio.IsHoistUp(), m_devGripperPio.IsHoistHome(), m_LimitSensorCheckRetry));
                                else
                                    SequenceLog.WriteLog(FuncName, string.Format("IsGripCloseEnableCondition = false, BeforeUpSensorDetectPosition={0}, down_limit={1}, up_limit={2}, Search Retry:{3}",
                                        m_BeforeUpSensorDetectPosition, down_limit, up_limit, m_LimitSensorCheckRetry));

                                m_LimitSensorDetectNg = m_devGripperPio.IsHoistLimit();

                                if (m_LimitSensorCheckRetry < 3)
                                {
                                    m_LimitSensorCheckRetry++;
                                    seqNo = 210;
                                }
                                else
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit UP Sensor Search NG, min={0} < curPosition={1} < max={2}", min, cur_position, max));
                                    m_LimitSensorCheckRetry = 0;
                                    m_devFoupGripper.SeqAbort();
                                    m_MoveComp1 = false;

                                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                                        {
                                            m_devGripperPio.DiHoistHome.SetDi(false);
                                            m_devGripperPio.DiHoistUp.SetDi(false);
                                        }
                                        m_UpDetectInterlockSet = true;
                                    seqNo = 400;
                                }
                            }
                        }
                    }
                    break;

                case 210:
                    {
                        // Hoist Port Before Down Teaching Position Move
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target1Position, m_TargetSlowVelSets);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                if ((GV.SimulationFlag & enSimulationFlag.UpRangeOver_Acquire) == enSimulationFlag.UpRangeOver_Acquire)
                                {
                                    m_devGripperPio.DiHoistHome.SetDi(false);
                                    m_devGripperPio.DiHoistUp.SetDi(false);
                                }
                            }

                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move OK", m_devFoupGripper.AxisHoist.AxisName));
                            seqNo = 130;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move NG - rv={1}", m_devFoupGripper.AxisHoist.AxisName, rv1));
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            seqNo = 400;
                        }
                    }
                    break;
                case 215:
                    {
                        double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                        if (cur_position < m_UpSensorDetectPosition + SetupManager.Instance.SetupHoist.HoistSensorDetectMoveDistance)
                        {
                            SequenceLog.WriteLog(FuncName, $"Foup Gripper Unit ({m_devFoupGripper.AxisHoist.AxisName}) sensor detect Move after Up Detect. curPos:{cur_position}, Check Pos : {m_UpSensorDetectPosition + SetupManager.Instance.SetupHoist.HoistSensorDetectMoveDistance} Continuous Motion.");
                            seqNo = 220;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 1000)
                        {
                            SequenceLog.WriteLog(FuncName, $"Foup Gripper Unit ({m_devFoupGripper.AxisHoist.AxisName}) Detect move after sensor detect. detected late. curPos:{cur_position}, Check Pos : {m_UpSensorDetectPosition + SetupManager.Instance.SetupHoist.HoistSensorDetectMoveDistance} Continuous Motion.");
                            seqNo = 230;
                        }
                    }
                    break;
                case 220:
                    {
                        // Hoist Stop
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.AxisHoist.GetDevAxis().Stop();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }

                        bool hoistup = m_devGripperPio.IsHoistUp();
                        // Hoist Port Down Teaching Position Move
                        double up_limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitOHB :
                                                SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitPort;
                        double down_limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitOHB :
                            SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitPort;

                        double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                        bool up_sensor_interlock = m_UpSensorDetectPosition > (m_BeforeUpSensorDetectPosition + up_limit) && hoistup; //Before Down 보다 높은 위치에서 UP 감지, 센서 감지된 위치끼리 비교해야 좀더 명확해짐.

                        double min = m_BeforeUpSensorDetectPosition + down_limit;
                        double max = m_BeforeUpSensorDetectPosition + up_limit;

                        bool close_enable = IsGripCloseEnableCondition();
                        close_enable &= cur_position < max;
                        close_enable &= cur_position > min;

                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if ((GV.SimulationFlag & enSimulationFlag.UpRangeOver_Acquire) == enSimulationFlag.UpRangeOver_Acquire)
                            {
                                close_enable = false;
                            }
                        }

                        if (up_sensor_interlock || !hoistup)
                        {
                            //Up 위치 이상 감지 또는 Up Sensor가 꺼졌어.. 그럼 이상하니까 다시해야되지않아..?
                            if (up_sensor_interlock)
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#4 and Range Over ({1})({2}) Continuous Motion.", m_devFoupGripper.AxisHoist.AxisName, cur_position, m_UpSensorDetectPosition));
                            else
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up NotDetect#4 ({1})({2}) Continuous Motion", m_devFoupGripper.AxisHoist.AxisName, cur_position, m_UpSensorDetectPosition));
                            seqNo = 230;
                        }
                        else if (close_enable || m_MoveComp1)// Up Sensor가 정상 감지했을 경우.
                        {
                            m_MoveComp1 = false;
                            double error = m_UpSensorDetectPosition - m_BeforeUpSensorDetectPosition; // current up sensor detect - before detect 
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit Continuous Motion UP Sensor Detected, min={0} < curPosition={1} < max={2}, error={3}", min, cur_position, max, error));
                            seqNo = 300;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Stop NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            seqNo = 400;
                        }
                    }
                    break;

                case 230:
                    {
                        if (m_MotionRetry < 3)
                        {
                            double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();

                            #region simulation
                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                if ((GV.SimulationFlag & enSimulationFlag.HoistLimitDetect_Acquire) == enSimulationFlag.HoistLimitDetect_Acquire)
                                {
                                    m_devGripperPio.DiHoistLimit.SetDi(true);
                                }

                                if (Math.Abs(cur_position - m_Target2Position.Z) < 1.0f)
                                {
                                    m_devGripperPio.DiHoistHome.SetDi(true);
                                    m_devGripperPio.DiHoistUp.SetDi(true);
                                }
                                else
                                {
                                    m_devGripperPio.DiHoistHome.SetDi(false);
                                    m_devGripperPio.DiHoistUp.SetDi(false);
                                }
                            }
                            #endregion

                            // Hoist Port Teaching Position Move
                            if (!m_MoveComp1)
                            {
                                rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target1Position, m_TargetSlowVelSets);
                                if (rv1 == 0) m_MoveComp1 = true;
                            }

                            if (m_devGripperPio.IsHoistLimit() && !m_LimitSensorDetectNg)
                            {
                                if (AppConfig.Instance.Simulation.MY_DEBUG)
                                {
                                    m_devGripperPio.DiHoistHome.SetDi(false);
                                    m_devGripperPio.DiHoistUp.SetDi(false);
                                    m_devGripperPio.DiHoistLimit.SetDi(false);
                                }

                                m_LimitSensorDetectNg = true;

                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down Move NG (Product Limit Detect, Continuous Motion{1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                                m_devFoupGripper.SeqAbort();

                                m_MoveComp1 = false;
                                m_MoveComp1 = false;

                                if (ohb)
                                {
                                    seqNo = 400;
                                }
                                else
                                {
                                    AlarmId = m_ALM_HoistLimitSensorAbnormalDetectPositionError.ID;
                                    EqpAlarm.Set(AlarmId);
                                    m_AutoRecoveryEnable = false;
                                    ReturnSeqNo = 0;
                                }
                            }
                            else if (!m_devGripperPio.IsHoistUp() || m_MoveComp1)
                            {
                                m_MoveComp1 = false;
                                StartTicks = XFunc.GetTickCount();
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect Retry Continuous Motion. Position:{1}", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                                seqNo = 130;

                            }
                            else if (rv1 > 0)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down Move NG (rv={1})", m_devFoupGripper.AxisHoist.AxisName, rv1));
                                m_devFoupGripper.SeqAbort();
                                m_MoveComp1 = false;
                                seqNo = 400;
                            }
                        }
                        else
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect Continuous Motion. Retry Count Over.  (rv={1})", m_devFoupGripper.AxisHoist.AxisName, rv1));
                            m_MoveComp1 = false;
                            if (ohb)
                            {
                                m_UpDetectInterlockSet = true;
                                m_devFoupGripper.SeqAbort();
                                seqNo = 400;
                            }
                            else
                            {
                                AlarmId = m_ALM_HoistUpSensorAbnormalDetectPositionError.ID;
                                EqpAlarm.Set(AlarmId);
                                m_AutoRecoveryEnable = false;
                                ReturnSeqNo = 0;
                                seqNo = 1000;
                            }
                        }
                    }
                    break;

                case 300:
                    {
                        // Gripper Close 동작
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devGripperPio.GripperClose();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Close OK"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 305;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Close NG"));
                            m_devGripperPio.SeqAbort();

                            // Gripper Open 부터 Recovery
                            //AlarmId = rv1;
                            //EqpAlarm.Set(AlarmId);
                            //ReturnSeqNo = seqNo; 
                            //seqNo = 1000;
                            //seqNo = 350;
                            seqNo = 360; // 바로 Open 동작을 하자 ~~ Hoist Move Interlock이 걸린다.
                        }
                    }
                    break;

                case 305:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devGripperPio.DiLeftProductExist.SetDi(true);
                            if ((GV.SimulationFlag & enSimulationFlag.FoupSingleDetect_Acquire) == enSimulationFlag.FoupSingleDetect_Acquire)
                                m_devGripperPio.DiRightProductExist.SetDi(false);
                            else
                                m_devGripperPio.DiRightProductExist.SetDi(true);
                        }

                        //Foup Detect를 확인해서 불필요한 Interlock Alarm을 줄이자.
                        bool foup_exist = m_devGripperPio.IsProductExist();

                        if (foup_exist)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Exist Check OK!"));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();
                            seqNo = 310;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 1000)
                        {
                            if (!m_devGripperPio.DiRightProductExist.IsDetected)
                            {
                                AlarmId = m_devGripperPio.ALM_ProductRightNotExistAlarm.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            else if (!m_devGripperPio.DiLeftProductExist.IsDetected)
                            {
                                AlarmId = m_devGripperPio.ALM_ProductLeftNotExistAlarm.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            else if (IsBcrOverInange(ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus, current_port))
                            {
                                AlarmId = m_ALM_BcrCheckOverInrangeTargetPositionError.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("BCR Check Over Inrange Target Position Alarm - BcrInrageError:{0}", bcrRangeError));
                                EqpAlarm.Set(AlarmId);
                            }
                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, $"Acquire Foup Abnormal Exist After Grip Close. Reson : Left:{m_devGripperPio.DiLeftProductExist.IsDetected}, Right:{m_devGripperPio.DiRightProductExist.IsDetected}");
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 310:
                    {
                        double limit = SetupManager.Instance.SetupHoist.HoistTorqueLimit;
                        double cur_torque = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurTorque();
                        bool torque_limit_alarm = cur_torque > limit;

                        // Hoist Port Teaching Before Down Position Move
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target1Position, m_TargetSlowVelSets);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                        {
                            VelSet set = m_TargetSlowVelSets.Find(x => x.AxisCoord == enAxisCoord.Z);

                            bool continuous_Hoist_Enable = false;
                            continuous_Hoist_Enable |= Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition() - m_Target1Position.Z) < 20.0f;
                            continuous_Hoist_Enable &= Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCommandVelocity()) >= set.Vel * 0.5f;
                            continuous_Hoist_Enable &= !torque_limit_alarm;
                            continuous_Hoist_Enable &= SetupManager.Instance.SetupSafty.CheckFoupAfterGripOpen == Use.NoUse ? m_devGripperPio.IsProductExist() : true;

                            if (continuous_Hoist_Enable || m_MoveComp1)
                            {
                                m_MoveComp1 = false;
                                StartTicks = XFunc.GetTickCount();
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Continuous Hoist Wait Move Start!", m_devFoupGripper.AxisHoist.AxisName));
                                SequenceLog.WriteLog(FuncName, $"Pos : {m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition()}, Target : {m_Target1Position.Z}, Diff : {Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition() - m_Target1Position.Z)}" +
                                    $"Vel : {m_devFoupGripper.AxisHoist.GetDevAxis().GetCommandVelocity()}, SetVel : {set.Vel}");

                                seqNo = 320;
                            }
                        }
                        else if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move OK", m_devFoupGripper.AxisHoist.AxisName));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 320;
                        }
                        else if (torque_limit_alarm)
                        {
                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move NG (Torque Limit ({1}>{2}))", m_devFoupGripper.AxisHoist.AxisName, cur_torque, limit));
                            m_devFoupGripper.SeqAbort();

                            AlarmId = m_ALM_AcquiringTorqueLimitAlarm.ID;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; // Open 확인 후 Home 동작을 하자
                            seqNo = 1000;
                        }
                        else if (rv1 > 0)
                        {
                            if (!m_devGripperPio.DiRightProductExist.IsDetected)
                            {
                                AlarmId = m_devGripperPio.ALM_ProductRightNotExistAlarm.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            else if (!m_devGripperPio.DiLeftProductExist.IsDetected)
                            {
                                AlarmId = m_devGripperPio.ALM_ProductLeftNotExistAlarm.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            else
                            {
                                AlarmId = rv1;
                                EqpAlarm.Set(AlarmId);
                            }

                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before  Down Move NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_devFoupGripper.SeqAbort();

                            ReturnSeqNo = seqNo; // Open 확인 후 Home 동작을 하자
                            seqNo = 1000;
                        }
                    }
                    break;

                case 320:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devGripperPio.DiLeftProductExist.SetDi(true);
                            m_devGripperPio.DiRightProductExist.SetDi(true);
                        }
                        // Foup 유무 확인 &
                        // Acquire때는 보면 안된다..ESWIN 고객 요청.. Poort중에 UL_Req가 300mm정도까지 감지되는게 있음.. EQ에서 기구 수정이 불가하다함.
                        bool timeover = XFunc.GetTickCount() - StartTicks > m_devEqPio.TimerTa3 * 1000;
                        bool foup_exist = m_devGripperPio.IsProductExist();
                        bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;

                        if (AppConfig.Instance.ProjectType != enProjectType.ESWIN)
                        {
                            timeover |= m_devEqPio.IfFlagRecv.Abort;
                            foup_exist &= pio_used ? m_devEqPio.IfFlagRecv.OnIng : true;
                        }

                        if (foup_exist)
                        {
                            OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.CarrierInstalled);

                            SequenceLog.WriteLog(FuncName, string.Format("beforeDown Position Foup Exist OK! "));
                            m_MoveComp2 = true;
                            seqNo = 420;
                        }
                        else if (timeover)
                        {
                            m_devEqPio.IfFlagRecv.Abort = false; // PIO Alarm
                            
                            m_PioAbort = pio_used;
                           
                            if (m_devGripperPio.IsProductExist() == false)
                                SequenceLog.WriteLog(FuncName, string.Format("IsProductExist Fail!"));
                            else
                                SequenceLog.WriteLog(FuncName, string.Format("m_devEqPio.IfFlagRecv.OnIng false"));
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Not Exist NG!"));
							m_MoveComp2 = true;
                            seqNo = 350; // Down 위치로 이동 후 Gripper Open
                        }
                    }
                    break;

                case 350:
                    {
                        // Hoist Port Teaching Down Position Move
                        if (!m_MoveComp1)
                        {
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                            {
                                rv1 = m_devFoupGripper.ContinuousMove(enAxisMask.aZ, m_Target2Position, m_TargetSlowVelSets, m_MoveComp2);
                                if (rv1 == 0) m_MoveComp1 = true;
                                if (m_MoveComp2) m_MoveComp2 = false;
                            }
                            else
                            {
                                rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target2Position, m_TargetSlowVelSets);
                                if (rv1 == 0) m_MoveComp1 = true;
                            }
                        }
                        if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down Move OK", m_devFoupGripper.AxisHoist.AxisName));
                            seqNo = 360;
                        }
                        else if (rv1 > 0)
                        {
                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down Move NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_devFoupGripper.SeqAbort();

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; // Open 확인 후 Home 동작을 하자
                            seqNo = 1000;
                        }
                    }
                    break;

                case 360:
                    {
                        // Gripper Open 동작
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devGripperPio.GripperOpen();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Open OK"));
                            seqNo = 370;
                        }
                        else if (rv1 > 0)
                        {
                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Open NG"));
                            m_devGripperPio.SeqAbort();
                            if (IsBcrOverInange(ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus, current_port))
                            {
                                AlarmId = m_ALM_BcrCheckOverInrangeTargetPositionError.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("BCR Check Over Inrange Target Position Alarm - BcrInrageError:{0}", bcrRangeError));
                            }
                            else
                            {
                                AlarmId = rv1;
                            }
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; // Open 확인 후 Gripper Open 부터 Recovery
                            seqNo = 1000;
                        }
                    }
                    break;

                case 370:
                    {
                        // Foup 이 감지될 경우 Up 동작을 하면 않된다...!
                        if (m_devGripperPio.IsProductExist() == false)
                        {
                            m_devFoupGripper.SeqAbort();
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Not Exist Check OK"));
                            seqNo = 420;
                        }
                        else
                        {
                            m_devFoupGripper.SeqAbort();
                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Not Exist Check NG (Moving Interlock)"));

                            if (m_devGripperPio.DiLeftProductExist.IsDetected)
                                AlarmId = m_devGripperPio.ALM_ProductLeftExistMoveAlarm.ID;
                            else AlarmId = m_devGripperPio.ALM_ProductRightExistMoveAlarm.ID;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; // Foup Exist 확인부터 Recovery
                            seqNo = 1000;
                        }
                    }
                    break;

                case 400:
                    {
                        // Hoist Stop
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.AxisHoist.GetDevAxis().Stop();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                            m_PioAbort = pio_used;
                            m_devEqPio.IfFlagRecv.OperateCancel = true;
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Stop OK", m_devFoupGripper.AxisHoist.AxisName));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 410;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Stop NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_devFoupGripper.SeqAbort();
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 410;
                        }
                    }
                    break;

                case 410:
                    {
                        // Gripper Home
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Home(enAxisMask.aZ | enAxisMask.aY | enAxisMask.aT);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("[{0} | {1} | {2}] : Initialize OK", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            m_MoveComp1 = false;
                            seqNo = 500;
                        }
                        else if (rv1 > 0)
                        {
                            m_AutoRecoveryEnable = true;
                            m_devFoupGripper.SeqAbort();
                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);

                            SequenceLog.WriteLog(FuncName, string.Format("[{0} | {1} | {2}] : Initialize Alarm - {3}", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName, EqpAlarm.GetAlarmMsg(AlarmId)));

                            ReturnSeqNo = seqNo; // Home 동작을 하자.
                            seqNo = 1000;
                        }
                    }
                    break;
                case 420:
                    {
                        // Hoist Wait Teaching Position Move
                        if (!m_MoveComp1)
                        {
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                            {
                                rv1 = m_devFoupGripper.ContinuousMove(enAxisMask.aZ, m_WaitPosition, m_Target2VelSets, m_MoveComp2);
                                if (rv1 == 0) m_MoveComp1 = true;
                                if (m_MoveComp2) m_MoveComp2 = false;
                            }
                            else
                            {
                                rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_WaitPosition, m_Target2VelSets);
                                if (rv1 == 0) m_MoveComp1 = true;
                            }
                        }
                        if (SetupManager.Instance.SetupOperation.Early_Motion_Use == Use.Use && !IsPortTypeBuffer())
                        {
                            bool continuous_Slide_Rotate_Enable = false;
                            continuous_Slide_Rotate_Enable |= Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition() - m_WaitPosition.Z) < 20.0f;
                            continuous_Slide_Rotate_Enable &= m_devGripperPio.IsProductExist();

                            if (continuous_Slide_Rotate_Enable || m_MoveComp1)
                            {
                                m_MoveComp1 = false;
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Early Motion Rotate,Slide Home Start after Acquire Complete!", m_devFoupGripper.AxisHoist.AxisName));

                                seqNo = 500;
                            }
                            else if (m_InterlockSet)
                            {
                                m_AutoRecoveryEnable = true;
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1} | {2}) Wait Move Interlock", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                                m_devFoupGripper.SeqAbort();
                                m_InterlockCheckStartTime = XFunc.GetTickCount();
                                ReturnSeqNo = seqNo;
                                seqNo = 2000;
                            }
                        }
                        else if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                        {
                            bool continuous_Slide_Rotate_Enable = false;
                            continuous_Slide_Rotate_Enable |= Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition() - m_WaitPosition.Z) < 20.0f;
                            continuous_Slide_Rotate_Enable &= m_devGripperPio.IsProductExist();

                            if (continuous_Slide_Rotate_Enable || m_MoveComp1)
                            {
                                m_MoveComp1 = false;
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Continuous Hoist Home Check & Slide & Rotate Move Start!", m_devFoupGripper.AxisHoist.AxisName));

                                seqNo = 450;
                            }
                            else if (m_InterlockSet)
                            {
                                m_AutoRecoveryEnable = true;
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1} | {2}) Wait Move Interlock", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                                m_devFoupGripper.SeqAbort();
                                m_InterlockCheckStartTime = XFunc.GetTickCount();
                                ReturnSeqNo = seqNo;
                                seqNo = 2000;
                            }
                        }
                        else if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;

                            bool home_start = false;
                            enAxisInFlag state = (m_devFoupGripper.AxisHoist.GetDevAxis().GetAxis() as IAxisCommand).GetAxisCurStatus();
                            if (state.HasFlag(enAxisInFlag.Org))
                            {
                                double origin_detect_error = (m_devFoupGripper.AxisHoist.GetDevAxis().GetAxis() as MpAxis).OriginOnDetectError;
                                double range = SetupManager.Instance.SetupHoist.OriginOnDetectErrorRange;
                                home_start = Math.Abs(origin_detect_error) > range;
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move OK, Home Sensor Check Abnormal [{2},{3}]", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName, origin_detect_error, range));
                            }
                            else
                            {
                                home_start = true;
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move OK, Home Sensor OFF State", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            }
                            if (home_start)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move OK, Home Start", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                                seqNo = 435;
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move OK", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                                seqNo = 440;
                            }
                        }
                        else if (m_InterlockSet)
                        {
                            m_AutoRecoveryEnable = true;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1} | {2}) Wait Move Interlock", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            m_devFoupGripper.SeqAbort();
                            m_InterlockCheckStartTime = XFunc.GetTickCount();
                            ReturnSeqNo = seqNo;
                            seqNo = 2000;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1} | {2}) Wait Move NG", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            m_devFoupGripper.SeqAbort();

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 400; // Close 확인 후 Home 동작을 하자.
                            seqNo = 1000;
                        }
                    }
                    break;

                case 435:
                    {
                        // Gripper Home
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Home(enAxisMask.aZ);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("[{0} | {1} | {2}] : Home Position OK", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            m_MoveComp1 = false;
                            seqNo = 440;
                        }
                        else if (rv1 > 0)
                        {
                            m_AutoRecoveryEnable = true;
                            m_devFoupGripper.SeqAbort();
                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);

                            SequenceLog.WriteLog(FuncName, string.Format("[{0} | {1} | {2}] : Initialize Alarm - {4}", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName, EqpAlarm.GetAlarmMsg(AlarmId)));

                            ReturnSeqNo = seqNo; // Home 동작을 하자.
                            seqNo = 1000;
                        }
                    }
                    break;

                case 440:
                    //2024.12.31 Check Foup interlock ..
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devFoupGripper.DiFoupCapCheck.SetDi(true);
                            m_devFoupGripper.DiFoupOpenCheck.SetDi(false);
                        }
                        bool foup_exist = IsFoupExist();
                        bool foup_cover_normal = foup_exist;
                        //if (SetupManager.Instance.SetupSafty.FoupCoverOpenCheckUse == Use.Use) foup_cover_normal &= m_devFoupGripper.IsFoupCoverOpen() == false;
                        //if (SetupManager.Instance.SetupSafty.FoupCoverExistCheckUse == Use.Use) foup_cover_normal &= m_devFoupGripper.IsFoupCoverDetect();
                        foup_cover_normal &= m_devFoupGripper.DiFoupOpenCheck.IsValid ? m_devFoupGripper.IsFoupCoverOpen() == false : true;
                        foup_cover_normal &= m_devFoupGripper.DiFoupCapCheck.IsValid ? m_devFoupGripper.IsFoupCoverDetect() : true;

                        if (foup_cover_normal || !foup_exist)
                        {
                            m_MotionRetry = 0;
                            SequenceLog.WriteLog(FuncName, $"Acquire Foup Cover Normal State!");
                            seqNo = 450;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 2000)
                        {
                            string msg = "";
                            //if (SetupManager.Instance.SetupSafty.FoupCoverOpenCheckUse == Use.Use && m_devFoupGripper.IsFoupCoverOpen())
                            if (m_devFoupGripper.DiFoupOpenCheck.IsValid && m_devFoupGripper.IsFoupCoverOpen())
                            {
                                msg += " Foup Cover Open!!";
                                AlarmId = m_ALM_FoupCoverOpenError.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            //if (SetupManager.Instance.SetupSafty.FoupCoverExistCheckUse == Use.Use && m_devFoupGripper.IsFoupCoverDetect() == false)
                            else if (m_devFoupGripper.DiFoupCapCheck.IsValid && m_devFoupGripper.IsFoupCoverDetect() == false)
                            {
                                msg += " Foup Cover Not Detect!!";
                                AlarmId = m_ALM_FoupCoverNotDetectError.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, $"Acquire Foup Cover Abnormal State! Reson : {msg}");
                            ReturnSeqNo = seqNo;
                            seqNo = 1000;
                        }
                    }
                    break;
                case 450:
                    {
                        // Slide & Rotate Wait Teaching Position Move
                        if (!m_MoveComp1)
                        {
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                            {
                                rv1 = m_devFoupGripper.ContinuousMove(enAxisMask.aYT, m_WaitPosition, m_Target2VelSets);
                            }
                            else
                            {
                                rv1 = m_devFoupGripper.Move(enAxisMask.aY | enAxisMask.aT, m_WaitPosition, m_Target2VelSets);
                            }

                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            bool checkSlidePosition = Math.Abs(m_devFoupGripper.AxisSlide.GetDevAxis().GetCurPosition()) < 3.0f;
                            bool checkRotatePosition = Math.Abs(m_devFoupGripper.AxisTurn.GetDevAxis().GetCurPosition()) < 1.0f;
                            if (checkSlidePosition && checkRotatePosition)
                            {
                                bool home_start = false;
                                enAxisInFlag state = (m_devFoupGripper.AxisHoist.GetDevAxis().GetAxis() as IAxisCommand).GetAxisCurStatus();
                                if (state.HasFlag(enAxisInFlag.Org))
                                {
                                    double origin_detect_error = (m_devFoupGripper.AxisHoist.GetDevAxis().GetAxis() as MpAxis).OriginOnDetectError;
                                    double range = SetupManager.Instance.SetupHoist.OriginOnDetectErrorRange;
                                    home_start = Math.Abs(origin_detect_error) > range;
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move OK, Home Sensor Check Abnormal#2 [{2},{3}]", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName, origin_detect_error, range));
                                }
                                else
                                {
                                    home_start = true;
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move OK, Home Sensor OFF State", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                                }
                                if (home_start)
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move OK, Home Start", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                                    seqNo = 460;
                                }
                                else
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move OK", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                                    seqNo = 480;
                                }
                            }
                            else
                            {
                                seqNo = 470;
                            }
                        }
                        else if (m_InterlockSet)
                        {
                            m_AutoRecoveryEnable = true;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1} ) Wait Move Interlock", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            m_devFoupGripper.SeqAbort();
                            m_InterlockCheckStartTime = XFunc.GetTickCount();

                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                            {
                                AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            SequenceLog.WriteLog(FuncName, string.Format("The first detection of the Look Down Sensor has begun. - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            ReturnSeqNo = seqNo;
                            seqNo = 2000;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1} | {2}) Wait Move NG", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            m_devFoupGripper.SeqAbort();

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 400; // Close 확인 후 Home 동작을 하자.
                            seqNo = 1000;
                        }
                    }
                    break;

                case 460:
                    {
                        // Gripper Home
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Home(enAxisMask.aZ);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("[{0} | {1} | {2}] : Initialize OK", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            m_MoveComp1 = false;
                            seqNo = 480;
                        }
                        else if (rv1 > 0)
                        {
                            m_AutoRecoveryEnable = true;
                            m_devFoupGripper.SeqAbort();
                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);

                            SequenceLog.WriteLog(FuncName, string.Format("[{0} | {1} | {2}] : Initialize Alarm - {4}", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName, EqpAlarm.GetAlarmMsg(AlarmId)));

                            ReturnSeqNo = seqNo; // Home 동작을 하자.
                            seqNo = 1000;
                        }
                    }
                    break;

                case 470:
                    {
                        // retry Slide & Rotate Wait Teaching Position Move 
                        if (m_MotionRetry < 3)
                        {
                            bool checkSlidePosition = Math.Abs(m_devFoupGripper.AxisSlide.GetDevAxis().GetCurPosition()) < 3.0f;
                            bool checkRotatePosition = Math.Abs(m_devFoupGripper.AxisTurn.GetDevAxis().GetCurPosition()) < 1.0f;

                            if (checkSlidePosition && checkRotatePosition)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move OK2", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 500;
                            }
                            else
                            {
                                m_MotionRetry++;
                                StartTicks = XFunc.GetTickCount();
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move NG. Retry Move Cnt:{2}", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName, m_MotionRetry));
                                seqNo = 450;
                            }
                        }
                        else
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1} | {2}) Wait Move NG", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 400; // Close 확인 후 Home 동작을 하자.
                            seqNo = 1000;
                        }
                    }
                    break;

                case 480:
                    //2024.12.31 Check Foup interlock ..
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devFoupGripper.DiFoupCapCheck.SetDi(true);
                            m_devFoupGripper.DiFoupOpenCheck.SetDi(false);
                        }
                        bool foup_exist = IsFoupExist();
                        bool foup_cover_normal = foup_exist;
                        //if (SetupManager.Instance.SetupSafty.FoupCoverOpenCheckUse == Use.Use) foup_cover_normal &= m_devFoupGripper.IsFoupCoverOpen() == false;
                        //if (SetupManager.Instance.SetupSafty.FoupCoverExistCheckUse == Use.Use) foup_cover_normal &= m_devFoupGripper.IsFoupCoverDetect();
                        foup_cover_normal &= m_devFoupGripper.DiFoupOpenCheck.IsValid ? m_devFoupGripper.IsFoupCoverOpen() == false : true;
                        foup_cover_normal &= m_devFoupGripper.DiFoupCapCheck.IsValid ? m_devFoupGripper.IsFoupCoverDetect() : true;

                        if (foup_cover_normal || !foup_exist)
                        {
                            m_MotionRetry = 0;
                            SequenceLog.WriteLog(FuncName, $"Acquire Foup Cover Normal State!");
                            seqNo = 500;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 2000)
                        {
                            string msg = "";
                            //if (SetupManager.Instance.SetupSafty.FoupCoverOpenCheckUse == Use.Use && m_devFoupGripper.IsFoupCoverOpen())
                            if (m_devFoupGripper.DiFoupOpenCheck.IsValid && m_devFoupGripper.IsFoupCoverOpen())
                            {
                                msg += " Foup Cover Open!!";
                                AlarmId = m_ALM_FoupCoverOpenError.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            //if (SetupManager.Instance.SetupSafty.FoupCoverExistCheckUse == Use.Use && m_devFoupGripper.IsFoupCoverDetect() == false)
                            else if (m_devFoupGripper.DiFoupCapCheck.IsValid && m_devFoupGripper.IsFoupCoverDetect() == false)
                            {
                                msg += " Foup Cover Not Detect!!";
                                AlarmId = m_ALM_FoupCoverNotDetectError.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, $"Acquire Foup Cover Abnormal State! Reson : {msg}");
                            ReturnSeqNo = seqNo;
                            seqNo = 1000;
                        }
                    }
                    break;
                case 500:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devGripperPio.DiHoistUp.SetDi(false);
                        }
                        m_devLookDown.SetNoUse();

                        bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                        bool abort = m_devEqPio.IfFlagRecv.Abort || m_PioAbort;
                        if (IsFoupExist() && !abort)
                        {
                            // acquire success
                            ProcessDataHandler.Instance.CurVehicleStatus.SetCarrierStatus(IF.OCS.CarrierState.Installed);

                            //Acquire/Deposit Complete 될 때 Tracking 값을 변경하자..
                            TeachingOffsetAdapter.Instance.SetAutoTrackingValue(true, target_port, m_TrackingDiff);

                            SequenceLog.WriteLog(FuncName, "Acquire Complete");
                            if (pio_used) m_devEqPio.IfFlagRecv.Complete = true;

                            seqNo = 510;
                        }
                        else
                        {
                            // acquire Fail
                            if (!IsFoupExist())
                                SequenceLog.WriteLog(FuncName, "Acquire Foup Empty (NG)");
                            if (abort) SequenceLog.WriteLog(FuncName, "Acquire PIO Abort (NG)");
                            if (pio_used) m_devEqPio.IfFlagRecv.OperateCancel = true; // Alarm Reset 하고 Disconnect 해라~~

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 520;
                        }
                    }
                    break;

                case 510:
                    {
                        bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                        bool pio_complete = true;
                        if (pio_used) pio_complete &= m_devEqPio.IfFlagRecv.PioOff;
                        bool abort = m_devEqPio.IfFlagRecv.Abort || m_PioAbort;

                        if (pio_complete)
                        {
                            // Acquire Complete Report
                            OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.AcquireCompleted);

                            m_devEqPio.IfFlagRecv.PioOff = false;
                            
                                seqNo = 900;
                            
                        }
                        else if (abort)
                        {
                            // acquire Fail
                            SequenceLog.WriteLog(FuncName, "Acquire PIO Abort (PIO Ready OFF NG)");
                            if (pio_used) m_devEqPio.IfFlagRecv.OperateCancel = true; // Alarm Reset 하고 Disconnect 해라~~

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 520;
                        }
                    }
                    break;

                case 520:
                    {
                        // 만일 Foup이 있는 경우는 Complete 처리하고 PIO OFF를 기다리자
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        // Abnormal Case
                        bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                        bool pio_complete = true;
                        bool pio_off_ng = false;
                        if (pio_used)
                        {
                            pio_complete &= m_devEqPio.IfFlagRecv.PioOff;
                            pio_off_ng = m_devEqPio.IfFlagRecv.PioOffNg;
                        }

                        if (pio_complete || pio_off_ng) // Disconnect 처리 완료 (OK or NG)
                        {
                            m_devEqPio.IfFlagRecv.PioOff = false;
                            m_devEqPio.IfFlagRecv.PioOffNg = false;
                            if (IsFoupExist()) ProcessDataHandler.Instance.CurVehicleStatus.SetCarrierStatus(IF.OCS.CarrierState.Installed);
                            else ProcessDataHandler.Instance.CurVehicleStatus.SetCarrierStatus(IF.OCS.CarrierState.None);

                            if (IsFoupExist() && pio_complete)
                            {
                                SequenceLog.WriteLog(FuncName, "Foup Exist & PIO Disconnected Check OK");
                                // Acquire Complete Report                                
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.AcquireCompleted);

                                    seqNo = 900;
                                
                            }
                            else
                            {
                                // acquire fail
                                ProcessDataHandler.Instance.CurVehicleStatus.SetVehicleStatus(VehicleState.AcquireFailed);
                                bool abort = m_devEqPio.IfFlagRecv.Abort || m_PioAbort;

                                if (abort || pio_off_ng)
                                {
                                    m_devEqPio.IfFlagRecv.Abort = false;
                                    SequenceLog.WriteLog(FuncName, "Acquire PIO Alarm (Abort)");
                                    OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.AcquireInterfaceFail);
                                    if (m_LookDownAlarmFlag)
                                    {
                                        if (m_ESSignalAlarmFlag)
                                            AlarmId = m_ALM_AcquireFailESSignal.ID;
                                        else if (m_HOSignalAlarmFlag)
                                            AlarmId = m_ALM_AcquireFailHOSignal.ID;
                                        else
                                            AlarmId = m_ALM_AcquireFailLookDownSensor.ID;

                                        EqpAlarm.Set(AlarmId);
                                    }
                                    else if (m_UpDetectInterlockSet)
                                    {
                                        if (IsBcrOverInange(ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus, current_port))
                                        {
                                            AlarmId = m_ALM_BcrCheckOverInrangeTargetPositionError.ID;
                                            EqpAlarm.Set(AlarmId);
                                            SequenceLog.WriteLog(FuncName, string.Format("BCR Check Over Inrange Target Position Alarm - BcrInrageError:{0}", bcrRangeError));
                                        }
                                        else
                                        { 
                                           AlarmId = m_ALM_HoistUpSensorAbnormalDetectPositionError.ID;
                                        }
                                        m_UpDetectInterlockSet = false;
                                    }
                                    else if (m_LimitSensorDetectNg)
                                    {
                                        m_LimitSensorDetectNg = false;

                                        AlarmId = m_ALM_HoistLimitSensorAbnormalDetectPositionError.ID;
                                    }
                                    else
                                    {
                                        AlarmId = m_ALM_AcquireFailError.ID;
                                        EqpAlarm.Set(AlarmId);
                                    }
                                    seqNo = 1000;
                                    ReturnSeqNo = 0;
                                }
                                else
                                {
                                    SequenceLog.WriteLog(FuncName, "Acquire Fail");
                                    OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.AcquireFailed);
                                    if (m_LookDownAlarmFlag)
                                    {
                                        if (m_ESSignalAlarmFlag)
                                            AlarmId = m_ALM_AcquireFailESSignal.ID;
                                        else if (m_HOSignalAlarmFlag)
                                            AlarmId = m_ALM_AcquireFailHOSignal.ID;
                                        else
                                            AlarmId = m_ALM_AcquireFailLookDownSensor.ID;
                                    }
                                    else if (m_UpDetectInterlockSet)
                                    {
                                        if (IsBcrOverInange(ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus, current_port))
                                        {
                                            AlarmId = m_ALM_BcrCheckOverInrangeTargetPositionError.ID;
                                            SequenceLog.WriteLog(FuncName, string.Format("BCR Check Over Inrange Target Position Alarm - BcrInrageError:{0}", bcrRangeError));
                                        }
                                        else
                                        {
                                            AlarmId = m_ALM_HoistUpSensorAbnormalDetectPositionWarning.ID;
                                        }
                                        m_UpDetectInterlockSet = false;
                                    }
                                    else if (m_LimitSensorDetectNg)
                                    {
                                        m_LimitSensorDetectNg = false;

                                        AlarmId = m_ALM_HoistLimitSensorAbnormalDetectPositionWarning.ID;
                                    }
                                    else
                                        AlarmId = m_ALM_AcquireFailError.ID;

                                    EqpAlarm.Set(AlarmId);
                                    seqNo = 1000;
                                    ReturnSeqNo = 0;
                                }
                            }
                        }
                    }
                    break;

                case 600:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        // PIO Cancel 상태. 이미 Alarm 이 발생한 상태. OCS에서 알아서 Abort 시킬 것임.
                        SequenceLog.WriteLog(FuncName, "PIO Alarm Acquire Cancel");
                        m_devEqPio.IfFlagRecv.Cancel = false;
                        m_devEqPio.IfFlagRecv.OperateCancel = true; // Alarm Reset 하고 Disconnect 해라~~
                        seqNo = 610;
                    }
                    break;

                case 610:
                    {
                        bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                        bool pio_complete = true;
                        bool pio_off_ng = false;
                        if (pio_used)
                        {
                            pio_complete &= m_devEqPio.IfFlagRecv.PioOff;
                            pio_off_ng = m_devEqPio.IfFlagRecv.PioOffNg;
                        }

                        if (pio_complete || pio_off_ng) // Disconnect 처리 완료 (OK or NG)
                        {
                            m_devEqPio.IfFlagRecv.PioOff = false;
                            m_devEqPio.IfFlagRecv.PioOffNg = false;

                            // PIO Cancel 상태. 이미 Alarm 이 발생한 상태. OCS에서 알아서 Abort 시킬 것임.
                            ProcessDataHandler.Instance.CurVehicleStatus.SetCarrierStatus(IF.OCS.CarrierState.None);
                            ProcessDataHandler.Instance.CurVehicleStatus.SetVehicleStatus(VehicleState.AcquireFailed);
                            OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.AcquireInterfaceFail);
                            //AlarmId = m_ALM_AcquireFailError.ID;
                            //EqpAlarm.Set(AlarmId);
                            seqNo = 1000;
                            ReturnSeqNo = 0;
                        }
                    }
                    break;
                case 900:
                    {
                        if (SetupManager.Instance.SetupOperation.Early_Motion_Use == Use.Use)
                        {
                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            SequenceLog.WriteLog(FuncName, "Early Motion On. Not Check Axis Home Sensor");
                            seqNo = 920;
                        }
                        else
                        {
                            bool homeslide = DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis().IsHomeSensor();
                            bool homehoist = DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis().IsHomeSensor();
                            bool homerotate = DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis().IsHomeSensor();

                            if ((homeslide && homehoist && homerotate) || AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                m_MoveComp1 = false;
                                m_MoveComp2 = false;
                                seqNo = 910;
                            }
                            else
                            {
                                if (!homeslide) AlarmId = DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis().ALM_HomeNotDetectError.ID;
                                if (!homehoist) AlarmId = DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis().ALM_HomeNotDetectError.ID;
                                if (!homerotate) AlarmId = DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis().ALM_HomeNotDetectError.ID;

                                EqpAlarm.Set(AlarmId);
                                seqNo = 1000;
                                ReturnSeqNo = 0;
                            }
                        }
                    }
                    break;
                case 910:
                    {
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devAntiDropFront.Lock();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (!m_MoveComp2)
                        {
                            rv2 = m_devAntiDropRear.Lock();
                            if (rv2 == 0) m_MoveComp2 = true;
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("AntiDrop Lock OK"));

                            this.scm.Start = false;
                            this.scm.End = true;
                            rv = 0;
                            seqNo = 0;
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            m_AutoRecoveryEnable = true;
                            if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceLog.WriteLog(FuncName, string.Format("FrontAntiDrop Lock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else if (rv2 > 0)
                            {
                                AlarmId = rv2;
                                SequenceLog.WriteLog(FuncName, string.Format("RearAntiDrop Lock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 920:
                    {
                        string msg = string.Empty;
                        m_devFoupGripper.SetCommandWaitMove();
                        msg = "Set Gripper Early Motion CMD.";
                        if (!m_devAntiDropFront.GetLock() && !m_devAntiDropFront.IsCommandBusy())
                        {
                            msg += " And Front Anti Drop Lock !";
                            if (m_devAntiDropFront.IsValid) m_devAntiDropFront.SetCommandLock();
                        }
                        if (!m_devAntiDropRear.GetLock() && !m_devAntiDropRear.IsCommandBusy())
                        {
                            msg += " And  Rear Anti Drop Lock !";
                            if (m_devAntiDropRear.IsValid) m_devAntiDropRear.SetCommandLock();
                        }

                        SequenceLog.WriteLog(FuncName, msg);

                        this.scm.Start = false;
                        this.scm.End = true;
                        rv = 0;
                        seqNo = 0;
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;
                            m_MoveComp1 = false;
                            if (ReturnSeqNo == 400) m_devFoupGripper.SeqAbort();
                            //EqpAlarm.Reset(AlarmId);
                            //SequenceLog.WriteLog(FuncName, string.Format("Reset Alarm : Code[{0}]", AlarmId));
                            EqpAlarm.ResetAll(); //중알람이면 다시 띄우겠지.. 일단 다 지우자
                            SequenceLog.WriteLog(FuncName, string.Format("Reset Alarm Reset all"));
                            AlarmId = 0;
                            seqNo = ReturnSeqNo;
                        }
                    }
                    break;

                case 2000:
                    {
                        if (m_InterlockSet == false)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Move Interlock Sensor Release", m_devFoupGripper.AxisHoist.AxisName));
                            m_InterlockCheckStartTime = XFunc.GetTickCount();
                            seqNo = 2010;
                        }
                        else if (XFunc.GetTickCount() - m_InterlockCheckStartTime > SetupManager.Instance.SetupPIO.PioInterlockWaitTime * 1000)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Move Interlock Sensor Recovery", m_devFoupGripper.AxisHoist.AxisName));
                            m_LookDownAlarmFlag = true;
                            m_ESSignalAlarmFlag = m_ESInterlockSet;
                            m_HOSignalAlarmFlag = m_HOInterlockSet;

                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                            {
                                AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                EqpAlarm.Reset(AlarmId);
                            }
                            seqNo = 400;
                        }
                    }
                    break;

                case 2010:
                    {
                        //release 대기시간 필요
                        if (m_InterlockSet)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Move Interlock Sensor ON", m_devFoupGripper.AxisHoist.AxisName));
                            m_InterlockCheckStartTime = XFunc.GetTickCount();
                            seqNo = 2000;
                        }
                        else if (XFunc.GetTickCount() - m_InterlockCheckStartTime > SetupManager.Instance.SetupPIO.PioInterlockWaitTime * 1000)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Move Interlock Sensor Release Confirm, Move Start", m_devFoupGripper.AxisHoist.AxisName));
                            m_LookDownAlarmFlag = false;
                            m_ESSignalAlarmFlag = m_ESInterlockSet;
                            m_HOSignalAlarmFlag = m_HOInterlockSet;

                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                            {
                                AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                EqpAlarm.Reset(AlarmId);
                            }

                            seqNo = ReturnSeqNo;
                        }
                    }
                    break;

                case 3000:
                    {
                        // Source Empty, Double Storage Event Report 후 대기
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Reset Double Storage or Source Empty Event. Retry !", AlarmId));
                            AlarmId = 0;
                            seqNo = ReturnSeqNo;
                        }
                    }
                    break;
            }
            this.SeqNo = seqNo;
            return rv;
        }
        #endregion
        #region Device Motor ReadyCheck
        private int CheckDeviceMotor()
        {
            AlarmId = 0;
            if (m_devFoupGripper.IsValid)
            {
                if (m_devFoupGripper.AxisHoist.IsValid) if (AlarmId == 0) AlarmId = m_devFoupGripper.AxisHoist.GetDevAxis().IsAxisReady() == false ? m_devFoupGripper.AxisHoist.GetDevAxis().ALM_NotReadyError.ID : 0;
                if (m_devFoupGripper.AxisSlide.IsValid) if (AlarmId == 0) AlarmId = m_devFoupGripper.AxisSlide.GetDevAxis().IsAxisReady() == false ? m_devFoupGripper.AxisSlide.GetDevAxis().ALM_NotReadyError.ID : 0;
                if (m_devFoupGripper.AxisTurn.IsValid) if (AlarmId == 0) AlarmId = m_devFoupGripper.AxisTurn.GetDevAxis().IsAxisReady() == false ? m_devFoupGripper.AxisTurn.GetDevAxis().ALM_NotReadyError.ID : 0;
            }
            if (m_devGripperPio.IsValid)
            {
                if (AlarmId == 0) AlarmId = m_devGripperPio.PioComm.IsGo() == false ? m_devGripperPio.PioComm.ALM_PioGoResponseError.ID : 0;
            }
            if (m_devAntiDropFront.IsValid)
                if (AlarmId == 0) AlarmId = m_devAntiDropFront.IsAlarm() ? m_devAntiDropFront.ALM_AntiDropAlarm.ID : 0;
            if (m_devAntiDropRear.IsValid)
                if (AlarmId == 0) AlarmId = m_devAntiDropRear.IsAlarm() ? m_devAntiDropRear.ALM_AntiDropAlarm.ID : 0;
            return AlarmId;
        }
        #endregion
        #region Methods - sensor, position function
        private bool IsFoupExist()
        {
            bool foup_exist = true;
            try
            {
                if (m_devGripperPio.IsValid)
                {
                    foup_exist &= m_devGripperPio.IsProductExist(); // sensor ng
                    foup_exist &= m_devGripperPio.DiGripperClose.IsDetected;
                    foup_exist &= !m_devGripperPio.IsHoistLimit();
                    foup_exist &= m_devGripperPio.IsHoistHome();
                }
                if (AppConfig.Instance.Simulation.MY_DEBUG)
                {
                    foup_exist |= ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return foup_exist;
        }
        private bool IsFoupNotExist()
        {
            bool foup_exist = true;
            try
            {
                if (m_devGripperPio.IsValid)
                {
                    foup_exist &= m_devGripperPio.IsProductNotExist();
                    foup_exist &= m_devGripperPio.DiGripperOpen.IsDetected;
                    foup_exist &= !m_devGripperPio.IsHoistLimit();
                    foup_exist &= !m_devGripperPio.IsHoistUp();
                    foup_exist &= !m_devGripperPio.IsHoistHome();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return foup_exist;
        }
        private bool IsDoubleStorage()
        {
            bool foup_exist = true; //default true해서 buffer일때만 확인하겠지...
            try
            {
                if (ProcessDataHandler.Instance.CurTransferCommand.IsValid)
                {
                    if (m_devFoupGripper.IsValid)
                    {
                        if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftBuffer)
                            foup_exist &= m_devFoupGripper.IsLeftDoubleStorage();
                        else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightBuffer)
                            foup_exist &= m_devFoupGripper.IsRightDoubleStorage();
                        else foup_exist = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return foup_exist;
        }
        private bool IsAntiDropUnlock()
        {
            bool anti_drop_unlock = true;
            try
            {
                anti_drop_unlock &= m_devAntiDropFront.IsValid ? m_devAntiDropFront.GetUnlock() : true;
                anti_drop_unlock &= m_devAntiDropRear.IsValid ? m_devAntiDropRear.GetUnlock() : true;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return anti_drop_unlock;
        }
        private bool IsGripCloseEnableCondition()
        {
            bool enable = true;
            try
            {
                if (m_devGripperPio.IsValid)
                {
                    enable &= !m_devGripperPio.IsHoistLimit();
                    enable &= m_devGripperPio.IsHoistUp();
                    enable &= m_devGripperPio.IsHoistHome();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return enable;
        }
        private void SaftyCheck()
        {
            bool interlock = false;
            try
            {
                bool look_down_use = true;
                look_down_use &= SetupManager.Instance.SetupSafty.PBSSensorUse == Use.Use;
                look_down_use &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PBSUsed;
                look_down_use &= m_devLookDown.IsUsingObs();
                if (look_down_use)
                    interlock |= m_devLookDown.GetFrontDetectState() > enFrontDetectState.enNone;
                
                m_ESInterlockSet = m_devEqPio.IfFlagRecv.ES;
                interlock |= m_ESInterlockSet;

                if (AppConfig.Instance.ProjectType == enProjectType.ESWIN)
                {
                    m_HOInterlockSet = m_devEqPio.IfFlagRecv.HO;
                    interlock |= m_HOInterlockSet;
                }

                double target = m_devFoupGripper.AxisHoist.GetDevAxis().GetTargetPos();
                double current = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                bool hoist_up = target - current > 10.0f; // 0 - (-500) = 500, 0 - (-1) = 1
                if (current > -450.0f && hoist_up) interlock = false;

                m_InterlockSet = interlock;
                if (m_OldInterlockState != interlock)
                {
                    m_OldInterlockState = interlock;
                    SequenceLog.WriteLog(FuncName, string.Format("Move Interlock Sensor Condition : LookDown={0}, IfFlagRecv.ES={1},IfFlagRecv.HO={2}, CurPosition={3}", m_devLookDown.GetFrontDetectState(), m_devEqPio.IfFlagRecv.ES, m_devEqPio.IfFlagRecv.HO, current));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private bool IsPortTypeBuffer()
        {
            bool rv = false;
            rv |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftBuffer;
            rv |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightBuffer;
            return rv;
        }
        private bool IsBcrOverInange(BcrStatus bcr, int portId)
        {
            bool inPosition = false; // 한바퀴 돌거니 ?

            DataItem_Port source_port = DatabaseHandler.Instance.DictionaryPortDataList[portId];
            double source_left_bcr = source_port.BarcodeLeft + source_port.DriveLeftOffset;
            double source_right_bcr = source_port.BarcodeRight + source_port.DriveRightOffset;
            double left_diff = Math.Abs(source_left_bcr - bcr.LeftBcr);
            double right_diff = Math.Abs(source_right_bcr - bcr.RightBcr);
            bcrRangeError = Math.Min(left_diff, right_diff);
            inPosition = bcrRangeError > 1.0f;

            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                if ((GV.SimulationFlag & enSimulationFlag.InRangeOver_Acquire) == enSimulationFlag.InRangeOver_Acquire)
                {
                    inPosition = true;
                }
            }

            return inPosition;
        }
        #endregion
    }

    public class SeqDeposit : XSeqFunc
    {
        public static readonly string FuncName = "[SeqDeposit]";
        XTimer m_Timer = new XTimer("SeqDeposit");

        #region Fields
        private DevEqPIO m_devEqPio = null;
        private DevFoupGripper m_devFoupGripper = null;
        private DevGripperPIO m_devGripperPio = null;
        private DevAntiDrop m_devAntiDropFront = null;
        private DevAntiDrop m_devAntiDropRear = null;
        private DevOBS m_devLookDown = null;

        private XyztPosition m_WaitPosition = new XyztPosition(); // wait
        private XyztPosition m_Target1Position = new XyztPosition(); // before down
        private XyztPosition m_Target2Position = new XyztPosition(); // down
        private XyztPosition m_Target3Position = new XyztPosition(); // up search
        private List<VelSet> m_Target1VelSets = new List<VelSet>(); // foup not exist move
        private List<VelSet> m_Target2VelSets = new List<VelSet>(); // foup exist move
        private List<VelSet> m_TargetSlowVelSets = new List<VelSet>(); // slow move
        private double m_UpSensorDetectPosition = 0.0f;
        private double m_BeforeUpSensorDetectPosition = 0.0f;
        private int m_StopRetry = 0;
        private bool m_LimitSensorDetectNg = false;
        private int m_AntiDropRetry = 0;
        private int m_MotionRetry = 0;

        private bool m_MoveComp1 = false;
        private bool m_MoveComp2 = false;

        private bool m_OldInterlockState = false;
        private bool m_InterlockSet = false;
        private bool m_ESInterlockSet = false;
        private bool m_HOInterlockSet = false;
        private UInt32 m_InterlockCheckStartTime = 0;
        private bool m_AutoRecoveryEnable = false;
        private bool m_UpDetectInterlockSet = false;
        private double m_TrackingDiff = 0;
        private double bcrRangeError = 0;

        private int m_LimitSensorCheckRetry = 0;
        private AlarmData m_ALM_HoistProductLimitCheckRetryError = null;
        private AlarmData m_ALM_DepositFailError = null;
        private AlarmData m_ALM_LookDownSensorWait = null;
        private AlarmData m_ALM_FoupCoverNotDetectError = null;
        private AlarmData m_ALM_FoupCoverOpenError = null;
		private AlarmData m_ALM_DepositFailLookDownSensor = null;
        private AlarmData m_ALM_LookDownSensorStatistics = null; //CRRC Look Down Sensor 감지 되지마자 Warning 발생용.. 단순 통계용이기 때문에 Warning만 발생 시킴.
        private AlarmData m_ALM_DepositFailESSignal = null;
        private AlarmData m_ALM_DepositFailHOSignal = null;
        private AlarmData m_ALM_ESSignalWait = null;
        private AlarmData m_ALM_HOSignalWait = null;
        private AlarmData m_ALM_HoistUpSensorAbnormalDetectPositionError = null;
        private AlarmData m_ALM_HoistUpSensorAbnormalDetectPositionWarning = null;
        private AlarmData m_ALM_HoistLimitSensorAbnormalDetectPositionError = null;
        private AlarmData m_ALM_HoistLimitSensorAbnormalDetectPositionWarning = null;
        private AlarmData m_ALM_BcrCheckOverInrangeTargetPositionError = null;

        private bool m_PioAbort = false;
        private bool m_LookDownAlarmFlag = false;
        private bool m_ESSignalAlarmFlag = false;
        private bool m_HOSignalAlarmFlag = false;
        #endregion

        #region Constructor
        public SeqDeposit()
        {
            this.SeqName = $"SeqDeposit";

            m_devEqPio = DevicesManager.Instance.DevEqpPIO;
            m_devFoupGripper = DevicesManager.Instance.DevFoupGripper;
            m_devGripperPio = DevicesManager.Instance.DevGripperPIO;
            m_devAntiDropFront = DevicesManager.Instance.DevFrontAntiDrop;
            m_devAntiDropRear = DevicesManager.Instance.DevRearAntiDrop;
            m_devLookDown = DevicesManager.Instance.DevOBSLookDown;
            AddCaseMemo();
            this.TaskLayer = (int)enTaskLayer.layerDeposit;

            m_ALM_HoistProductLimitCheckRetryError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqDeposit", "Deposit Sequence", "Product Limit Check Retry Alarm");
            m_ALM_DepositFailError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqDeposit", "Deposit Sequence", "Deposit Fail Alarm");
            m_ALM_LookDownSensorWait = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentStatusWarning, false, "SeqDeposit", "Deposit Sequence", "Look Down Interlock Warning");
            m_ALM_FoupCoverNotDetectError = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, "SeqDeposit", "Deposit Sequence", "Foup Cover Not Detect Alarm");
            m_ALM_FoupCoverOpenError = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, true, "SeqDeposit", "Deposit Sequence", "Foup Cover Open Alarm");
			m_ALM_DepositFailLookDownSensor = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqDeposit", "Deposit Sequence", "Look Down Sensor Detected Alarm");
            m_ALM_LookDownSensorStatistics = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentStatusWarning, false, "SeqDeposit", "Deposit Sequence", "Look Down Sensor Statistics Warning");
            m_ALM_DepositFailESSignal = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "DevEqPio", "Deposit Sequence", "PIO ES Signal Off Alarm");
            m_ALM_DepositFailHOSignal = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "DevEqPio", "Deposit Sequence", "PIO HO Signal Off Alarm");
            m_ALM_ESSignalWait = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentStatusWarning, false, "DevEqPio", "Deposit Sequence", "PIO ES Signal Interlock Warning");
            m_ALM_HOSignalWait = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentStatusWarning, false, "DevEqPio", "Deposit Sequence", "PIO HO Signal Interlock Warning");

            m_ALM_HoistUpSensorAbnormalDetectPositionError = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, "SeqDeposit", "Deposit Sequence", "Port Up Sensor Detected at Abnormal Position Alarm");
            m_ALM_HoistUpSensorAbnormalDetectPositionWarning = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, false, "SeqDeposit", "Deposit Sequence", "OHB Up Sensor Detected at Abnormal Position Warning");

            m_ALM_HoistLimitSensorAbnormalDetectPositionError = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, "SeqDeposit", "Deposit Sequence", "Port Limit Sensor Detected at Abnormal Position Alarm");
            m_ALM_HoistLimitSensorAbnormalDetectPositionWarning = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, false, "SeqDeposit", "Deposit Sequence", "OHB Limit Sensor Detected at Abnormal Position Warning");
            m_ALM_BcrCheckOverInrangeTargetPositionError = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, "SeqDeposit", "Deposit Sequence", "Current Bcr Position Check Over Inrange Target Position Alarm");
        }
        public override void SeqAbort()
        {
            if (AlarmId > 0)
            {
                if (m_AutoRecoveryEnable)
                    ProcessDataHandler.Instance.CurVehicleStatus.SetVehicleStatus(VehicleState.DepositFailed);

                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            if (this.scm.Ing)
            {
                //Device 초기화
                if (m_devFoupGripper.IsValid) m_devFoupGripper.SeqAbort();
                if (m_devGripperPio.IsValid) m_devGripperPio.SeqAbort();
                if (m_devAntiDropFront.IsValid) m_devAntiDropFront.SeqAbort();
                if (m_devAntiDropRear.IsValid) m_devAntiDropRear.SeqAbort();
                if (m_devLookDown.IsValid) m_devLookDown.SeqAbort();
                this.scm.Abort = true;
            }
            if (m_devLookDown.IsValid) m_devLookDown.SetNoUse();
            this.InitSeq();
        }
        private void AddCaseMemo()
        {
            try
            {
                this.SeqCaseMemoLists.Add(0, string.Format("Deposit Sequence Start !"));
                this.SeqCaseMemoLists.Add(1, string.Format("Deposit Motor Condition Check !"));
                this.SeqCaseMemoLists.Add(10, string.Format("Deposit Interlock Condition Check !"));
                this.SeqCaseMemoLists.Add(20, string.Format("Anti Drop Unlock Check"));
                this.SeqCaseMemoLists.Add(30, string.Format("Gripper Open Check"));
                this.SeqCaseMemoLists.Add(40, string.Format("Look Down Sensor OFF Check"));
                this.SeqCaseMemoLists.Add(50, string.Format("PIO Interface Start Wait"));
                this.SeqCaseMemoLists.Add(100, string.Format("Deposit Position & Velocity Setting"));
                this.SeqCaseMemoLists.Add(110, string.Format("Slide & Rotate Before Down Position Move"));
                this.SeqCaseMemoLists.Add(120, string.Format("Hoist Before Down Position Move"));
                this.SeqCaseMemoLists.Add(130, string.Format("Hoist Down Position Move"));
                this.SeqCaseMemoLists.Add(140, string.Format("Hoist Down + DownRangeLimit Position Move"));
                this.SeqCaseMemoLists.Add(150, string.Format("Hoist Move Stop"));
                this.SeqCaseMemoLists.Add(153, string.Format("Check Up Sensor Detected"));
                this.SeqCaseMemoLists.Add(155, string.Format("The sensor is detected in the wrong Hoist position and serves to check the sensor"));
                this.SeqCaseMemoLists.Add(160, string.Format("Hoist Current + (-3mm) Position Setting"));
                this.SeqCaseMemoLists.Add(170, string.Format("Hoist Current + (-3mm) Position Move"));
                this.SeqCaseMemoLists.Add(200, string.Format("Gripper Open Enable Condition Confirm"));
                this.SeqCaseMemoLists.Add(210, string.Format("Hoist Before Down Position Retry Move"));
                this.SeqCaseMemoLists.Add(300, string.Format("LD REQ OFF Check"));
                this.SeqCaseMemoLists.Add(310, string.Format("Gripper Open"));
                this.SeqCaseMemoLists.Add(320, string.Format("UL REQ OFF & Foup Not Exist Check"));
                this.SeqCaseMemoLists.Add(350, string.Format("Recovery Hoist Down Position Move"));
                this.SeqCaseMemoLists.Add(360, string.Format("Recovery Gripper Close"));
                this.SeqCaseMemoLists.Add(370, string.Format("Recovery Foup Exist Check"));
                this.SeqCaseMemoLists.Add(390, string.Format("Hoist Before Down Position Move"));
                this.SeqCaseMemoLists.Add(400, string.Format("Recovery Hoist Move Stop"));
                this.SeqCaseMemoLists.Add(410, string.Format("Recovery Hoist & Slide & Rotate Home Position Move"));
                this.SeqCaseMemoLists.Add(450, string.Format("Hoist & Slide & Rotate Wait Position Move"));
                this.SeqCaseMemoLists.Add(500, string.Format("Foup Not Exist Check"));
                this.SeqCaseMemoLists.Add(510, string.Format("PIO Disconnect Complete Confirm"));
                this.SeqCaseMemoLists.Add(520, string.Format("Recovery Foup Not Exist. PIO Complete Confirm"));
                this.SeqCaseMemoLists.Add(600, string.Format("Recovery PIO Alarm Deposite Cancel"));
                this.SeqCaseMemoLists.Add(610, string.Format("Recovery PIO Disconnect Complete Confirm"));
                this.SeqCaseMemoLists.Add(1000, string.Format("Alarm Reset Wait"));
                this.SeqCaseMemoLists.Add(2000, string.Format("PIO Interlock Signal Release Wait"));
                this.SeqCaseMemoLists.Add(2010, string.Format("PIO Interlock Timeout Wait"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("{0} EXCEPTION : {1}", FuncName, ex.ToString()));
            }
        }
        #endregion

        #region Override
        public override int Do()
        {
            if (this.scm.Pause) return -1;
            if (AppConfig.Instance.VehicleType == VehicleType.Clean) return -1;

            int rv = -1;
            int rv1 = -1;
            int rv2 = -1;
            int rv3 = -1;

            SaftyCheck();
            bool carrier_installed = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
            int target_port = ProcessDataHandler.Instance.CurTransferCommand.DestinationID;
            int current_port = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortID;
            bool ohb = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftBuffer;
            ohb |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightBuffer;

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        m_AutoRecoveryEnable = false;
                        this.scm.Start = true;
                        StartTicks = XFunc.GetTickCount();

                        m_PioAbort = false;
                        m_LookDownAlarmFlag = false;
                        m_ESSignalAlarmFlag = false;
                        m_HOSignalAlarmFlag = false;

                        if (SetupManager.Instance.SetupPIO.PioAllAutoUsing) //Teaching 진행하고 확인할때.. 설비 PIO 준비가 안되어있는 경우가 많아서.. 해제 방법은 필요..Demo 같은 경우는 아예 PIO가 없기도 하고..
                        {
                            bool always_use = false;
                            always_use |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftSTKPort;
                            always_use |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightSTKPort;
                            always_use |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftEQPort;
                            always_use |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightEQPort;
                            ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed = always_use;
                        }

                        m_devLookDown.SetNoUse();

                        seqNo = 1;
                    }
                    break;

                case 1:
                    // Loader Z축 간섭 위치 확인
                    {
                        rv1 = CheckDeviceMotor();
                        if (rv1 == 0)
                        {
                            m_LimitSensorCheckRetry = 0;
                            SequenceLog.WriteLog(FuncName, "Deposit Start");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 2;
                        }
                        else if (rv1 > 0 && (XFunc.GetTickCount() - StartTicks > 5000))
                        {
                            m_AutoRecoveryEnable = true;
                            EqpAlarm.Set(rv1);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break; 
                case 2:
                    {
                        if(AppConfig.Instance.Simulation.MY_DEBUG)
                            m_devFoupGripper.DiFoupCapCheck.SetDi(true);

                        // FoupCover State Check
                        bool foupCover_NormalState = CheckFoupCover();
                        if (foupCover_NormalState)
                        {
                            SequenceLog.WriteLog(FuncName, "Check Foup Cover Normal State.");
                            seqNo = 10;
                        }
                        else if ((XFunc.GetTickCount() - StartTicks > 1000))
                        {
                            string msg = "";
                            //if (SetupManager.Instance.SetupSafty.FoupCoverOpenCheckUse == Use.Use && m_devFoupGripper.IsFoupCoverOpen())
                            if (m_devFoupGripper.DiFoupOpenCheck.IsValid && m_devFoupGripper.IsFoupCoverOpen())
                            {
                                msg += " Foup Cover Open!!";
                                AlarmId = m_ALM_FoupCoverOpenError.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            //if (SetupManager.Instance.SetupSafty.FoupCoverExistCheckUse == Use.Use && m_devFoupGripper.IsFoupCoverDetect() == false)
                            else if (m_devFoupGripper.DiFoupCapCheck.IsValid && m_devFoupGripper.IsFoupCoverDetect() == false)
                            {
                                msg += " Foup Cover Not Detect!!";
                                AlarmId = m_ALM_FoupCoverNotDetectError.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, $"Foup Cover Abnrmal State. Reson : {msg}");
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;
                case 10:
                    {
                        bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                        // SeqDeposit에서 Check 하고 StartReq 하자
                        if (IsAntiDropUnlock() == false || IsAntiDropBusy() == true)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("AntiDrop Unlock NG"));
                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            m_AntiDropRetry = 0;

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20; // 20으로 이동하여 Unlock 실행하자!
                        }
                        else if (m_devGripperPio.IsGripperOpen())
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Gripper Open State. Close Try"));
                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            seqNo = 30;
                        }
                        else if (m_InterlockSet)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Look Down Sensor ON State. Wait OFF"));
                            StartTicks = XFunc.GetTickCount();

                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                            {
                                AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            SequenceLog.WriteLog(FuncName, string.Format("The first detection of the Look Down Sensor has begun. - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            seqNo = 40;
                        }
                        else if (ProcessDataHandler.Instance.CurTransferCommand.IsValid == false)
                        {
                            m_AutoRecoveryEnable = true;
                            AlarmId = ProcessDataHandler.Instance.ALM_CommandNotExistError.ID;
                            SequenceLog.WriteLog(FuncName, string.Format("Port Loading Interface, Transfer Command Not Valid Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                        else if (current_port != target_port)
                        {
                            m_AutoRecoveryEnable = true;
                            AlarmId = ProcessDataHandler.Instance.ALM_DestinationPortMismatchError.ID;
                            SequenceLog.WriteLog(FuncName, string.Format("Destination ID isn't same Current Port ID - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                        else if (!IsFoupExist() || carrier_installed == false)
                        {
                            m_AutoRecoveryEnable = true;
                            if (m_devGripperPio.DiLeftProductExist.IsDetected == false)
                                AlarmId = m_devGripperPio.ALM_ProductLeftNotExistAlarm.ID;
                            else AlarmId = m_devGripperPio.ALM_ProductRightNotExistAlarm.ID;
                            SequenceLog.WriteLog(FuncName, string.Format("Port Loading Interface, Foup Not Exist Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                        else if (IsDoubleStorage() == false && !AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_AutoRecoveryEnable = true;
                            ProcessDataHandler.Instance.CurVehicleStatus.SetVehicleStatus(VehicleState.DestinationDouble);
                            OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DestinationDoubleStorage);

                            if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftBuffer)
                                AlarmId = m_devFoupGripper.ALM_LeftDoubleStorageError.ID;
                            else AlarmId = m_devFoupGripper.ALM_RightDoubleStorageError.ID;
                            SequenceLog.WriteLog(FuncName, string.Format("Port Unloading Interface, Double Storage Check, Foup Exist at Destination Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            //EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 3000;
                        }
                        else
                        {
                            if (pio_used)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("PIO Use, Interface Start!"));

                                m_devEqPio.IfFlagRecv.Reset();//반송 Case에 맞지않는 Flag는 끄자
                                m_devEqPio.IfFlagSend.Start();
                                seqNo = 50;
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("PIO NoUse, FoupGripper Action Start"));
                                seqNo = 100;
                            }
                        }
                    }
                    break;

                case 20:
                    {
                        if (IsAntiDropBusy() == false)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("AntiDrop CMD Empty!AntiDrop Unlock Start"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 25;
                        }
                        else if ((XFunc.GetTickCount() - StartTicks > 5000))
                        {
                            //이전 동작이 끝나지가 않는데.. Abort를 진행하고 넘기자..
                            AntiDropCMDStop();
                            StartTicks = XFunc.GetTickCount();
                            SequenceLog.WriteLog(FuncName, string.Format("AntiDrop CMD Cbort!"));
                            seqNo = 25;
                        }
                    }
                    break;
                case 25:
                    {
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devAntiDropFront.Unlock();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (!m_MoveComp2)
                        {
                            rv2 = m_devAntiDropRear.Unlock();
                            if (rv2 == 0) m_MoveComp2 = true;
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            if (IsAntiDropUnlock() == true)
                            {
                                m_MoveComp1 = false;
                                m_MoveComp2 = false;
                                SequenceLog.WriteLog(FuncName, string.Format("AntiDrop Unlock OK"));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 0;
                            }
                            else
                            { 
                                SequenceLog.WriteLog(FuncName, $"AntiDrop Unlock NG. Retry Action");
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 27;
                            }
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            m_AutoRecoveryEnable = true;
                            if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceLog.WriteLog(FuncName, string.Format("FrontAntiDrop Unlock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else if (rv2 > 0)
                            {
                                AlarmId = rv2;
                                SequenceLog.WriteLog(FuncName, string.Format("RearAntiDrop Unlock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;
                case 26:
                    {
                        if (m_AntiDropRetry < 3)
                        {
                            if (IsAntiDropUnlock() == true)
                            {
                                m_MoveComp1 = false;
                                m_MoveComp2 = false;
                                SequenceLog.WriteLog(FuncName, string.Format("AntiDrop Unlock OK"));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 0;
                            }
                            else if ((XFunc.GetTickCount() - StartTicks > 1000))
                            {
                                m_AntiDropRetry++;
                                SequenceLog.WriteLog(FuncName, $"Don't Move Anti-Drop. Retry Unlock. Count : {m_AntiDropRetry} ");
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 25;
                            }
                        }
                        else
                        {
                            if (m_devAntiDropFront.IsValid ? m_devAntiDropFront.GetUnlock() == false : false)
                            {
                                AlarmId = m_devAntiDropFront.ALM_AntiDropNotDetectBWAlarm.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            else if (m_devAntiDropRear.IsValid ? m_devAntiDropRear.GetUnlock() == false : false)
                            {
                                AlarmId = m_devAntiDropRear.ALM_AntiDropNotDetectBWAlarm.ID;
                                EqpAlarm.Set(AlarmId);
                            }

                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, $"Anti-Drop has completed moving, but the sensor is not detected. FW:{m_devAntiDropFront.GetUnlock()}, BW:{m_devAntiDropRear.GetUnlock()}");
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;
                case 27:
                    {
                        //이동이 완료되었는데 Sensor가 감지되지 않는 상황이면 Home을 진행하자...
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devAntiDropFront.Home();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (!m_MoveComp2)
                        {
                            rv2 = m_devAntiDropRear.Home();
                            if (rv2 == 0) m_MoveComp2 = true;
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
						 	m_AntiDropRetry++;
                            SequenceLog.WriteLog(FuncName, $"AntiDrop Home Complete. Retry Action Start!!");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 25;
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            m_AutoRecoveryEnable = true;
                            if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceLog.WriteLog(FuncName, string.Format("FrontAntiDrop Home Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else if (rv2 > 0)
                            {
                                AlarmId = rv2;
                                SequenceLog.WriteLog(FuncName, string.Format("RearAntiDrop Home Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;
                case 30:
                    {
                        rv1 = m_devGripperPio.GripperClose();
                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Gripper Close OK"));
                            seqNo = 0;
                        }
                        else if (rv1 > 0)
                        {
                            m_AutoRecoveryEnable = true;
                            AlarmId = rv1;
                            SequenceLog.WriteLog(FuncName, string.Format("Gripper Close Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 40:
                    {
                        if (m_InterlockSet == false)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Look Down Sensor OFF OK"));

                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                            {
                                AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                EqpAlarm.Reset(AlarmId);
                            }

                            seqNo = 0;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 10000)
                        {
                            m_AutoRecoveryEnable = true;
                            
                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                            {
                                AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                EqpAlarm.Reset(AlarmId);
                            }

                            if (m_ESInterlockSet)
                            {
                                AlarmId = m_ALM_ESSignalWait.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("PIO ES Signal ON Timeover Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else if (m_HOInterlockSet)
                            {
                                AlarmId = m_ALM_HOSignalWait.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("PIO HO Signal ON Timeover Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else
                            {
                                AlarmId = m_ALM_LookDownSensorWait.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("Look Down Sensor ON Timeover Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 50:
                    {
                        if (m_devEqPio.IfFlagSend.Busy)
                        {
                            m_devEqPio.IfFlagSend.Busy = false;
                            SequenceLog.WriteLog(FuncName, string.Format("PIO Busy, FoupGripper Action Start"));
                            seqNo = 100;
                        }
                        else if (m_devEqPio.IfFlagSend.Cancel)
                        {
                            m_devEqPio.IfFlagSend.Cancel = false;
                            SequenceLog.WriteLog(FuncName, string.Format("PIO Cancel, PIO Fail"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 600;
                        }
                    }
                    break;

                case 100:
                    {
                        m_LimitSensorDetectNg = false;

                        bool pbs_use = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PBSUsed;
                        if (pbs_use)
                        {
                            uint pbs_no = (uint)ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PBSSelectNo;
                            m_devLookDown.SetOBS(pbs_no);
                        }

                        // Calculate Position
                        m_WaitPosition = m_devFoupGripper.GetTeachingPosition(m_devFoupGripper.TeachingPointWait.PosId);
                        m_Target1Position = m_devFoupGripper.GetBeforeDownPos(target_port, carrier_installed); // Before Down Position
                        m_Target2Position = m_devFoupGripper.GetDownPos(target_port, carrier_installed, false); // Down Position, Sensor Search No-Use
                        ushort down_prop = IsPortTypeBuffer() ? m_devFoupGripper.TeachingVelocityBufDown.PropId : m_devFoupGripper.TeachingVelocityDown.PropId;
                        ushort up_prop = IsPortTypeBuffer() ? m_devFoupGripper.TeachingVelocityBufUp.PropId : m_devFoupGripper.TeachingVelocityUp.PropId;
                        m_Target1VelSets = m_devFoupGripper.GetTeachingVelSets(down_prop);
                        m_Target2VelSets = m_devFoupGripper.GetTeachingVelSets(up_prop);
                        m_TargetSlowVelSets = m_devFoupGripper.GetTeachingVelSets(m_devFoupGripper.TeachingVelocityLow.PropId); // 저속으로 이동하면서 Sensor Detect 시점을 찾자 !
                        if (m_WaitPosition != null && m_Target1Position != null && m_Target2Position != null && m_Target1VelSets != null && m_TargetSlowVelSets != null && m_Target2VelSets != null)
                        {
                            double offset = SetupManager.Instance.SetupHoist.HoistSensorDetectMoveDistance;
                            m_UpSensorDetectPosition = m_Target2Position.Z - offset;
                            m_BeforeUpSensorDetectPosition = m_Target2Position.Z - offset;
                            ///////////////////////////////////////////////////////////////
                            string msg = "Velocity:[";
                            foreach (VelSet set in m_Target1VelSets) msg += string.Format("[{0},{1},{2},{3}],", set.Vel, set.Acc, set.Dec, set.Jerk);
                            foreach (VelSet set in m_Target2VelSets) msg += string.Format("[{0},{1},{2},{3}],", set.Vel, set.Acc, set.Dec, set.Jerk);
                            foreach (VelSet set in m_TargetSlowVelSets) msg += string.Format("[{0},{1},{2},{3}],", set.Vel, set.Acc, set.Dec, set.Jerk);
                            msg += "]";
                            ///////////////////////////////////////////////////////////////
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit Move Start - DOWN1:[{0}], DOWN2:[{1}], Velocity:[{2}]", m_Target1Position.ToString(), m_Target2Position.ToString(), msg));
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            seqNo = 110;
                        }
                        else
                        {
                            m_AutoRecoveryEnable = true;
                            if (m_Target1VelSets == null || m_TargetSlowVelSets == null || m_Target2VelSets == null)
                            {
                                AlarmId = m_devFoupGripper.ALM_SettingError.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("DevFoupGripper TeachingVelocity isn't TeachingVelocityDown Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else
                            {
                                AlarmId = ProcessDataHandler.Instance.ALM_SourcePortMismatchError.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("Teaching Port Database isn't Port ID = [{0}] Alarm - {1}", target_port, EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 110:
                    {
                        // Slide & Rotate Port Teaching Position Move
                        if (!m_MoveComp1)
                        {
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                                rv1 = m_devFoupGripper.ContinuousMove(enAxisMask.aYT, m_Target1Position, m_Target1VelSets);
                            else
                                rv1 = m_devFoupGripper.Move(enAxisMask.aY | enAxisMask.aT, m_Target1Position, m_Target1VelSets);

                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                        {
                            if (Math.Abs(m_devFoupGripper.AxisSlide.GetDevAxis().GetCurPosition() - m_Target1Position.Y) < Math.Abs(m_Target1Position.Y * 0.05f) || m_MoveComp1)
                            {
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepositStart);

                                m_MoveComp1 = false;

                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Continuous_Motion_Use", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                                seqNo = 120;
                            }
                        }
                        if (m_MoveComp1)
                        {
                            OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepositStart);

                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Before Down Move OK", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            if (SetupManager.Instance.SetupHoist.HoistTwoStepDown)
                            {
                                seqNo = 120;
                            }
                            else seqNo = 130;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Before Down Move NG", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();

                            //AlarmId = rv1;
                            //EqpAlarm.Set(AlarmId);
                            //ReturnSeqNo = seqNo; // Home 동작을 하자 !
                            //seqNo = 1000;
                            bool axisReady = true;
                            axisReady &= m_devFoupGripper.AxisSlide.GetDevAxis().IsAxisReady();
                            axisReady &= m_devFoupGripper.AxisTurn.GetDevAxis().IsAxisReady();
                            seqNo = 400; // 300으로 가면 사고 남...hoist를 내리지도 않았는데 grip open하면 않됨.
                        }
                    }
                    break;

                case 120:
                    {
                        // Hoist Port Before Down Teaching Position Move
                        if (!m_MoveComp1)
                        {
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                            {
                                rv1 = m_devFoupGripper.ContinuousMove(enAxisMask.aZ, m_Target1Position, m_Target1VelSets);
                            }
                            else
                            {
                                rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target1Position, m_Target1VelSets);
                            }
                            if (rv1 == 0) m_MoveComp1 = true;
                        }

                        if (m_devGripperPio.IsHoistLimit() || m_devGripperPio.IsHoistUp())
                        {
                            if (m_devGripperPio.IsHoistLimit()) m_LimitSensorDetectNg = true;
                            else if (m_devGripperPio.IsHoistUp()) m_UpDetectInterlockSet = true;

                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                m_devGripperPio.DiHoistHome.SetDi(false);
                                m_devGripperPio.DiHoistUp.SetDi(false);
                                m_devGripperPio.DiHoistLimit.SetDi(false);
                            }

                            m_MoveComp1 = false;
                            string detect_sensor = m_devGripperPio.IsHoistUp() ? "UP Detect" : "Product Limit Detect";
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move NG ({1})", m_devFoupGripper.AxisHoist.AxisName, detect_sensor));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();

                            if (ohb)
                            {
                                seqNo = 400;
                            }
                            else
                            {
                                AlarmId = m_ALM_HoistLimitSensorAbnormalDetectPositionError.ID;
                                EqpAlarm.Set(AlarmId);
                                m_AutoRecoveryEnable = false;
                                ReturnSeqNo = 0;
                            }
                        }
                        else if (m_InterlockSet)
                        {
                            m_AutoRecoveryEnable = false;

                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                            {
                                AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            SequenceLog.WriteLog(FuncName, string.Format("The first detection of the Look Down Sensor has begun. - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move Interlock", m_devFoupGripper.AxisHoist.AxisName));
                            m_devFoupGripper.SeqAbort();
                            m_InterlockCheckStartTime = XFunc.GetTickCount();
                            ReturnSeqNo = seqNo;
                            seqNo = 2000;
                        }
                        else if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                        {
                            VelSet set = m_TargetSlowVelSets.Find(x => x.AxisCoord == enAxisCoord.Z);

                            bool continuous_Hoist_Enable = false;
                            if (ohb)
                            {
                                continuous_Hoist_Enable |= Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition() - m_Target1Position.Z) < 15.0f;
                                continuous_Hoist_Enable &= Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCommandVelocity()) < set.Vel * 3.0f;
                            }
                            else
                            {
                                continuous_Hoist_Enable |= Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition() - m_Target1Position.Z) < 35.0f;
                                continuous_Hoist_Enable &= Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCommandVelocity()) < set.Vel * 10.0f;
                            }

                            if (continuous_Hoist_Enable || m_MoveComp1)
                            {
                                m_MoveComp1 = false;
                                m_MoveComp2 = true;
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Continuous Hoist Slow Move Start!", m_devFoupGripper.AxisHoist.AxisName));
                                SequenceLog.WriteLog(FuncName, $"Pos : {m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition()}, Target : {m_Target1Position.Z}, Diff : {Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition() - m_Target1Position.Z)}" +
                                    $"Vel : {m_devFoupGripper.AxisHoist.GetDevAxis().GetCommandVelocity()}, SetVel : {set.Vel}");
                                double limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitOHB : SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitPort;
                                m_Target2Position.Z += limit;
                                seqNo = 130;
                            }
                        }
                        else if (m_MoveComp1)
                        {
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                            {
                                double limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitOHB : SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitPort;
                                m_Target2Position.Z += limit;
                            }
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move OK", m_devFoupGripper.AxisHoist.AxisName));
                            seqNo = 130;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();
                            seqNo = 400;
                        }
                    }
                    break;

                case 130:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if ((GV.SimulationFlag & enSimulationFlag.HoistLimitDetect_Deposit) == enSimulationFlag.HoistLimitDetect_Deposit)
                            {
                                m_devGripperPio.DiHoistLimit.SetDi(true);
                            }
                        }
                        // Hoist Port Down Teaching Position Move
                        double up_limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitOHB :
                                                SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitPort;
                        double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                        bool up_sensor_interlock = cur_position > (m_BeforeUpSensorDetectPosition + up_limit) && m_devGripperPio.IsHoistUp(); //Before Down 보다 높은 위치에서 UP 감지
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            int ntest = new Random().Next(3, 8);
                            if (Math.Abs(cur_position - m_Target2Position.Z) < ntest)
                            {
                                m_devGripperPio.DiHoistHome.SetDi(true);
                                m_devGripperPio.DiHoistUp.SetDi(true);
                            }
                        }
                        // Hoist Port Teaching Position Move
                        if (!m_MoveComp1)
                        {
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                            {
                                if (SetupManager.Instance.SetupHoist.HoistTwoStepDown)
                                    rv1 = m_devFoupGripper.ContinuousMove(enAxisMask.aZ, m_Target2Position, m_TargetSlowVelSets, m_MoveComp2);
                                else rv1 = m_devFoupGripper.ContinuousMove(enAxisMask.aZ, m_Target2Position, m_Target1VelSets, m_MoveComp2);
                                if (m_MoveComp2) m_MoveComp2 = false;
                            }
                            else
                            {
                                if (SetupManager.Instance.SetupHoist.HoistTwoStepDown)
                                    rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target2Position, m_TargetSlowVelSets);
                                else rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target2Position, m_Target1VelSets);
                            }
                            
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_devGripperPio.IsHoistLimit() && !m_LimitSensorDetectNg)
                        {
                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                m_devGripperPio.DiHoistHome.SetDi(false);
                                m_devGripperPio.DiHoistUp.SetDi(false);
                                m_devGripperPio.DiHoistLimit.SetDi(false);
                            }

                            m_LimitSensorDetectNg = true;

                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down Move NG (Product Limit Detect, {1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();

                            if (ohb)
                            {
                                seqNo = 400;
                            }
                            else
                            {
                                AlarmId = m_ALM_HoistLimitSensorAbnormalDetectPositionError.ID;
                                EqpAlarm.Set(AlarmId);
                                m_AutoRecoveryEnable = false;
                                ReturnSeqNo = 0;
                            }

                        }
                        else if (up_sensor_interlock)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) UP Detect#1 and Range Over ({1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            m_UpSensorDetectPosition = cur_position;
                            m_devFoupGripper.SeqAbort();
                            m_StopRetry = 0;
                            seqNo = 150;
                        }
                        else if (m_devGripperPio.IsHoistUp())
                        {
                            m_MoveComp1 = false;
                            m_UpSensorDetectPosition = cur_position;
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#1 ({1}), Continuous Motion. Check Exact Position", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                                StartTicks = XFunc.GetTickCount();
                                m_MoveComp2 = true;
                                seqNo = 215;
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#1 ({1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                                m_devFoupGripper.SeqAbort();
                                m_StopRetry = 0;
                                seqNo = 150;
                            }
                            
                        }
                        else if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down Move OK ({1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));

                            m_Target3Position = ObjectCopier.Clone(m_Target2Position);
                            double limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitOHB : SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitPort;
                            double up_offset = m_LimitSensorDetectNg ? 2 * m_LimitSensorCheckRetry : 0.0f; //Limit가 감지된다 좀 올리자~~~
                            m_Target3Position.Z = m_BeforeUpSensorDetectPosition + limit + up_offset;

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 140;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down Move NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();
                            seqNo = 400;
                        }
                    }
                    break;

                case 140:
                    {
                        // Hoist Port Down + DownRangeLimit Position Move
                        double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target3Position, m_TargetSlowVelSets);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_devGripperPio.IsHoistLimit() && !m_LimitSensorDetectNg)
                        {
                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                m_devGripperPio.DiHoistHome.SetDi(false);
                                m_devGripperPio.DiHoistUp.SetDi(false);
                            }

                            m_LimitSensorDetectNg = true;

                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down + DownRangeLimit Move NG (Product Limit Detect, {1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();

                            if (ohb)
                            {
                                seqNo = 400;
                            }
                            else
                            {
                                AlarmId = m_ALM_HoistLimitSensorAbnormalDetectPositionError.ID;
                                EqpAlarm.Set(AlarmId);
                                m_AutoRecoveryEnable = false;
                                ReturnSeqNo = 0;
                            }
                        }
                        else if (m_devGripperPio.IsHoistUp())
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#2 ({1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            m_UpSensorDetectPosition = cur_position;
                            m_devFoupGripper.SeqAbort();
                            StartTicks = XFunc.GetTickCount();
                            m_StopRetry = 0;
                            seqNo = 150;
                        }
                        else if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down + DownRangeLimit Move OK ({1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 200;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down + DownRangeLimit Move NG (rv={1})", m_devFoupGripper.AxisHoist.AxisName, rv1));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();
                            seqNo = 400;
                        }
                    }
                    break;

                case 150:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 200) break;
                        // Hoist Stop
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.AxisHoist.GetDevAxis().Stop();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            double error = Math.Abs(m_UpSensorDetectPosition - m_BeforeUpSensorDetectPosition);
                            double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Stop OK ({1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            StartTicks = XFunc.GetTickCount();

                            if (m_devGripperPio.IsHoistUp())
                            {
                                if (error < 3.0f)
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#3 ({1}, {2})", m_devFoupGripper.AxisHoist.AxisName, cur_position, error));
                                    seqNo = 160;
                                }
                                else
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#3 and Range Over ({1}, {2})", m_devFoupGripper.AxisHoist.AxisName, cur_position, error));
                                    seqNo = 153;
                                }
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up NotDetect#3 ({1}, {2})", m_devFoupGripper.AxisHoist.AxisName, cur_position, error));
                                seqNo = 130;
                            }
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Stop NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();
                            seqNo = 400;
                        }
                    }
                    break;

                case 153:
                    {
                        bool hoistup = m_devGripperPio.IsHoistUp();
                        // Hoist Port Down Teaching Position Move
                        double up_limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitOHB :
                                                SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitPort;
                        double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                        bool up_sensor_interlock = m_UpSensorDetectPosition > (m_BeforeUpSensorDetectPosition + up_limit) && hoistup; //Before Down 보다 높은 위치에서 UP 감지, 감지 기준으로 서로 비교를 해야 함. cur_position은 좀더 내려간 위치일 것임.
                        if (up_sensor_interlock)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#4 and Range Over ({1})({2})", m_devFoupGripper.AxisHoist.AxisName, cur_position, m_UpSensorDetectPosition));
                            seqNo = 155;
                        }
                        else if (!hoistup)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up NotDetect#4 ({1})({2})", m_devFoupGripper.AxisHoist.AxisName, cur_position, m_UpSensorDetectPosition));
                            seqNo = 130;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 400)// Up Sensor가 정상 감지했을 경우.
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#4 ({1})({2})", m_devFoupGripper.AxisHoist.AxisName, cur_position, m_UpSensorDetectPosition));
                            seqNo = 160;
                        }
                    }
                    break;

                case 155:
                    {
                        bool hoistup = m_devGripperPio.IsHoistUp();
                        if (!hoistup)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Up Sensor Monitoring Result : Not Detected After Delay.", m_devFoupGripper.AxisHoist.AxisName));
                            seqNo = 130;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 1000)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Up Sensor Monitoring Result : Detected in Abnormal Hoist Position.", m_devFoupGripper.AxisHoist.AxisName));

                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                m_devGripperPio.DiHoistHome.SetDi(false);
                                m_devGripperPio.DiHoistUp.SetDi(false);
                            }

                            if (ohb)
                            {
                                m_UpDetectInterlockSet = true;
                                m_MoveComp1 = false;
                                m_devFoupGripper.SeqAbort();
                                seqNo = 400;
                            }
                            else
                            {
                                if (IsBcrOverInange(ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus, current_port))
                                {
                                    AlarmId = m_ALM_BcrCheckOverInrangeTargetPositionError.ID;
                                    SequenceLog.WriteLog(FuncName, string.Format("BCR Check Over Inrange Target Position Alarm - BcrInrageError:{0}", bcrRangeError));
                                }
                                else
                                {
                                    AlarmId = m_ALM_HoistUpSensorAbnormalDetectPositionError.ID;
                                }
                                EqpAlarm.Set(AlarmId);
                                m_AutoRecoveryEnable = false;
                                ReturnSeqNo = 0;
                                seqNo = 1000;
                            }
                        }
                    }
                    break;

                case 160:
                    {
                        if (m_devFoupGripper.AxisHoist.GetDevAxis().IsAxisReady())
                        {
                            m_devFoupGripper.SeqAbort();
                            double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();

                            // CurrentPosition + HoistSensorDetectMoveDistance(-3mm) Move
                            double offset = SetupManager.Instance.SetupHoist.HoistSensorDetectMoveDistance;
                            m_Target3Position.Z = m_UpSensorDetectPosition + offset;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) UpDetect + HoistSensorDetectMoveDistance Move Start ({1},{2})", m_devFoupGripper.AxisHoist.AxisName, m_Target3Position.Z, cur_position));
                            seqNo = 170;
                        }
                        else if (m_StopRetry > 3)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Stop TimeOver NG", m_devFoupGripper.AxisHoist.AxisName, m_StopRetry));
                            m_StopRetry = 0;
                            seqNo = 200;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 2000)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Stop TimeOver Retry {1}", m_devFoupGripper.AxisHoist.AxisName, m_StopRetry));
                            m_StopRetry++;
                            seqNo = 150;
                        }
                    }
                    break;

                case 170:
                    {
                        // HoistSensorDetectMoveDistance(-3mm) Move
                        double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();

                        double minus_limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitOHB : SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitPort;
                        bool limit_skip = true;
                        limit_skip &= m_Target3Position.Z > m_Target2Position.Z + minus_limit;
                        limit_skip &= m_Target3Position.Z < m_Target2Position.Z + 5.0;

                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target3Position, m_TargetSlowVelSets);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_devGripperPio.IsHoistLimit() && !limit_skip && !m_LimitSensorDetectNg)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) UpDetect + HoistSensorDetectMoveDistance Move NG (Product Limit Detect, {1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();
                            seqNo = 400;
                        }
                        else if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) UpDetect + HoistSensorDetectMoveDistance Move OK ({1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 200;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) UpDetect + HoistSensorDetectMoveDistance Move NG (rv={1})", m_devFoupGripper.AxisHoist.AxisName, rv1));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();
                            seqNo = 400;
                        }
                    }
                    break;

                case 200:
                    {
                        /////////////////////////////////////////////////////////////////////////////////////////////////////
                        /// Sensor 유효Range : down position +- Range Limit
                        /// min = down position + DownRangeLimit
                        /// max = down position + UpRangeLimit
                        /// min < cur position < max
                        /////////////////////////////////////////////////////////////////////////////////////////////////////
                        double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                        double up_limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitOHB :
                            SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitPort;
                        double down_limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitOHB :
                            SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitPort;

                        double min = m_BeforeUpSensorDetectPosition + down_limit;
                        double max = m_BeforeUpSensorDetectPosition + up_limit;

                        bool open_enable = IsGripOpenEnableCondition();
                        open_enable &= cur_position < max;
                        open_enable &= cur_position > min;

                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if ((GV.SimulationFlag & enSimulationFlag.UpRangeOver_Deposit) == enSimulationFlag.UpRangeOver_Deposit || (GV.SimulationFlag & enSimulationFlag.InRangeOver_Deposit) == enSimulationFlag.InRangeOver_Deposit)
                            {
                                open_enable = false;
                            }
                        }

                        if (open_enable)
                        {
                            m_MoveComp1 = false;
                            // Tracking Offset Update ////////////////////////////////////////////////////////
                            //double error = m_UpSensorDetectPosition - m_BeforeUpSensorDetectPosition; // current up sensor detect - before detect 
                            //TeachingOffsetAdapter.Instance.SetAutoTrackingValue(false, target_port, error);
                            //System.Diagnostics.Debug.WriteLine($"[SeqDeposite] [{error}], [{min}<{cur_position}<{max}]");
                            //여기에서 Tracking 값을 갱신하는 경우 Error가 발생했을때도 동일하게 갱신이 된다.. 위치 이동..
                            m_TrackingDiff = m_UpSensorDetectPosition - m_BeforeUpSensorDetectPosition;
                            //////////////////////////////////////////////////////////////////////////////////
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit UP Sensor Detected, min={0} < curPosition={1} < max={2}, error={3}", min, cur_position, max, m_TrackingDiff));
                            seqNo = 300;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 1000)
                        {
                            if (!ohb)
                            {
                                if (AppConfig.Instance.Simulation.MY_DEBUG)
                                {
                                    m_devGripperPio.DiHoistHome.SetDi(false);
                                    m_devGripperPio.DiHoistUp.SetDi(false);
                                }
                                
                                if (IsBcrOverInange(ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus, current_port))
                                {
                                    AlarmId = m_ALM_BcrCheckOverInrangeTargetPositionError.ID;
                                    SequenceLog.WriteLog(FuncName, string.Format("BCR Check Over Inrange Target Position Alarm - BcrInrageError:{0}", bcrRangeError));
                                }
                                else
                                { 
                                    AlarmId = m_ALM_HoistUpSensorAbnormalDetectPositionError.ID;
                                }
                                EqpAlarm.Set(AlarmId);
                                m_AutoRecoveryEnable = false;
                                ReturnSeqNo = 0;
                                seqNo = 1000;
                            }
                            else
                            {
                                if (!IsGripOpenEnableCondition())
                                    SequenceLog.WriteLog(FuncName, string.Format("IsGripOpenEnableCondition = false, Hoist Limit:{0},Hoist Up:{1},Hoist Home:{2}, Search Retry:{3}",
                                        m_devGripperPio.IsHoistLimit(), m_devGripperPio.IsHoistUp(), m_devGripperPio.IsHoistHome(), m_LimitSensorCheckRetry));
                                else
                                    SequenceLog.WriteLog(FuncName, string.Format("IsGripOpenEnableCondition = false, BeforeUpSensorDetectPosition={0}, down_limit={1}, up_limit={2}, Search Retry:{3}",
                                        m_BeforeUpSensorDetectPosition, down_limit, up_limit, m_LimitSensorCheckRetry));

                                m_LimitSensorDetectNg = m_devGripperPio.IsHoistLimit();

                                if (m_LimitSensorCheckRetry < 3)
                                {
                                    m_LimitSensorCheckRetry++;
                                    seqNo = 210;
                                }
                                else
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit UP Sensor Search NG, min={0} < curPosition={1} < max={2}", min, cur_position, max));
                                    m_LimitSensorCheckRetry = 0;
                                    m_MoveComp1 = false;
                                    m_devFoupGripper.SeqAbort();

                                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                                        {
                                            m_devGripperPio.DiHoistHome.SetDi(false);
                                            m_devGripperPio.DiHoistUp.SetDi(false);
                                        }
                                        m_UpDetectInterlockSet = true;
                                    seqNo = 400;
                                }
                            }
                        }
                    }
                    break;

                case 210:
                    {
                        // Hoist Port Before Down Teaching Position Move
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target1Position, m_TargetSlowVelSets);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                m_devGripperPio.DiHoistHome.SetDi(false);
                                m_devGripperPio.DiHoistUp.SetDi(false);
                            }

                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move OK", m_devFoupGripper.AxisHoist.AxisName));
                            seqNo = 130;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();
                            seqNo = 400;
                        }
                    }
                    break;
                case 215:
                    {
                        double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                        if (cur_position < m_UpSensorDetectPosition + SetupManager.Instance.SetupHoist.HoistSensorDetectMoveDistance)
                        {
                            SequenceLog.WriteLog(FuncName, $"Foup Gripper Unit ({m_devFoupGripper.AxisHoist.AxisName}) sensor detect Move after Up Detect. curPos:{cur_position}, Check Pos : {m_UpSensorDetectPosition + SetupManager.Instance.SetupHoist.HoistSensorDetectMoveDistance} Continuous Motion.");
                            seqNo = 220;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 1000)
                        {
                            SequenceLog.WriteLog(FuncName, $"Foup Gripper Unit ({m_devFoupGripper.AxisHoist.AxisName}) Detect move after sensor detect. detected late. curPos:{cur_position}, Check Pos : {m_UpSensorDetectPosition + SetupManager.Instance.SetupHoist.HoistSensorDetectMoveDistance} Continuous Motion.");
                            seqNo = 230;
                        }
                    }
                    break;
                case 220:
                    {
                        // Hoist Stop
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.AxisHoist.GetDevAxis().Stop();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }

                        bool hoistup = m_devGripperPio.IsHoistUp();
                        // Hoist Port Down Teaching Position Move
                        double up_limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitOHB :
                                                SetupManager.Instance.SetupHoist.HoistSensorDetectUpRangeLimitPort;
                        double down_limit = ohb ? SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitOHB :
                            SetupManager.Instance.SetupHoist.HoistSensorDetectDownRangeLimitPort;

                        double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                        bool up_sensor_interlock = m_UpSensorDetectPosition > (m_BeforeUpSensorDetectPosition + up_limit) && hoistup; //Before Down 보다 높은 위치에서 UP 감지, 센서 감지된 위치끼리 비교해야 좀더 명확해짐.

                        double min = m_BeforeUpSensorDetectPosition + down_limit;
                        double max = m_BeforeUpSensorDetectPosition + up_limit;

                        bool close_enable = IsGripOpenEnableCondition();
                        close_enable &= cur_position < max;
                        close_enable &= cur_position > min;

                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if ((GV.SimulationFlag & enSimulationFlag.UpRangeOver_Deposit) == enSimulationFlag.UpRangeOver_Deposit)
                            {
                                close_enable = false;
                            }
                        }

                        if (up_sensor_interlock || !hoistup)
                        {
                            //Up 위치 이상 감지 또는 Up Sensor가 꺼졌어.. 그럼 이상하니까 다시해야되지않아..?
                            if (up_sensor_interlock)
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect#4 and Range Over ({1})({2}) Continuous Motion.", m_devFoupGripper.AxisHoist.AxisName, cur_position, m_UpSensorDetectPosition));
                            else
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up NotDetect#4 ({1})({2}) Continuous Motion", m_devFoupGripper.AxisHoist.AxisName, cur_position, m_UpSensorDetectPosition));
                            seqNo = 230;
                        }
                        else if (close_enable || m_MoveComp1)// Up Sensor가 정상 감지했을 경우.
                        {
                            m_MoveComp1 = false;
                            double error = m_UpSensorDetectPosition - m_BeforeUpSensorDetectPosition; // current up sensor detect - before detect 
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit Continuous Motion UP Sensor Detected, min={0} < curPosition={1} < max={2}, error={3}", min, cur_position, max, error));
                            seqNo = 300;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Stop NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            seqNo = 400;
                        }
                    }
                    break;

                case 230:
                    {
                        if (m_MotionRetry < 3)
                        {
                            double cur_position = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();

                            #region simulation
                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                if ((GV.SimulationFlag & enSimulationFlag.HoistLimitDetect_Deposit) == enSimulationFlag.HoistLimitDetect_Deposit)
                                {
                                    m_devGripperPio.DiHoistLimit.SetDi(true);
                                }

                                if (Math.Abs(cur_position - m_Target2Position.Z) < 1.0f)
                                {
                                    m_devGripperPio.DiHoistHome.SetDi(true);
                                    m_devGripperPio.DiHoistUp.SetDi(true);
                                }
                                else
                                {
                                    m_devGripperPio.DiHoistHome.SetDi(false);
                                    m_devGripperPio.DiHoistUp.SetDi(false);
                                }
                            }
                            #endregion

                            // Hoist Port Teaching Position Move
                            if (!m_MoveComp1)
                            {
                                rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target1Position, m_TargetSlowVelSets);
                                if (rv1 == 0) m_MoveComp1 = true;
                            }

                            if (m_devGripperPio.IsHoistLimit() && !m_LimitSensorDetectNg)
                            {
                                if (AppConfig.Instance.Simulation.MY_DEBUG)
                                {
                                    m_devGripperPio.DiHoistHome.SetDi(false);
                                    m_devGripperPio.DiHoistUp.SetDi(false);
                                    m_devGripperPio.DiHoistLimit.SetDi(false);
                                }

                                m_LimitSensorDetectNg = true;

                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down Move NG (Product Limit Detect, Continuous Motion{1})", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                                m_devFoupGripper.SeqAbort();

                                m_MoveComp1 = false;
                                m_MoveComp1 = false;

                                if (ohb)
                                {
                                    seqNo = 400;
                                }
                                else
                                {
                                    AlarmId = m_ALM_HoistLimitSensorAbnormalDetectPositionError.ID;
                                    EqpAlarm.Set(AlarmId);
                                    m_AutoRecoveryEnable = false;
                                    ReturnSeqNo = 0;
                                }
                            }
                            else if (!m_devGripperPio.IsHoistUp() || m_MoveComp1)
                            {
                                m_MoveComp1 = false;
                                StartTicks = XFunc.GetTickCount();
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect Retry Continuous Motion. Position:{1}", m_devFoupGripper.AxisHoist.AxisName, cur_position));
                                seqNo = 130;

                            }
                            else if (rv1 > 0)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down Move NG (rv={1})", m_devFoupGripper.AxisHoist.AxisName, rv1));
                                m_devFoupGripper.SeqAbort();
                                m_MoveComp1 = false;
                                seqNo = 400;
                            }
                        }
                        else
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Up Detect Continuous Motion. Retry Count Over.  (rv={1})", m_devFoupGripper.AxisHoist.AxisName, rv1));
                            m_MoveComp1 = false;
                            if (ohb)
                            {
                                m_UpDetectInterlockSet = true;
                                m_devFoupGripper.SeqAbort();
                                seqNo = 400;
                            }
                            else
                            {
                                AlarmId = m_ALM_HoistUpSensorAbnormalDetectPositionError.ID;
                                EqpAlarm.Set(AlarmId);
                                m_AutoRecoveryEnable = false;
                                ReturnSeqNo = 0;
                                seqNo = 1000;
                            }
                        }
                    }
                    break;

                case 300:
                    {
                        // LD REQ OFF 될때까지 기다리자 ~~
                        // Foup 유무 확인 & 
                        bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                        bool eqp_foup_exist = pio_used ? m_devEqPio.IfFlagSend.OnIng : true;

                        if (eqp_foup_exist)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("EQP Foup Exist OK!"));
                            seqNo = 310;
                        }
                        else if (m_devEqPio.IfFlagSend.Abort || XFunc.GetTickCount() - StartTicks > m_devEqPio.TimerTp3 * 1000)
                        {
                            m_devEqPio.IfFlagSend.Abort = false;
                            m_PioAbort = true;

                            if (m_devEqPio.IfFlagSend.OnIng == false)
                                SequenceLog.WriteLog(FuncName, string.Format("m_devEqPio.IfFlagSend.OnIng == false"));
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Exist NG!"));
                            seqNo = 370; // Open 전 상태이니 바로 올려도 될것 같다.
                        }
                    }
                    break;

                case 310:
                    {
                        // Gripper Open 동작
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devGripperPio.GripperOpen();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Open OK"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 320;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Open NG"));
                            m_devGripperPio.SeqAbort();

                            //AlarmId = rv1;
                            //EqpAlarm.Set(AlarmId);
                            //ReturnSeqNo = seqNo; // Gripper Open 부터 Recovery
                            //seqNo = 1000;
                            //seqNo = 350;
                            seqNo = 360; // 바로 Close 동작을 하자 ~~ Hoist Move Interlock이 걸린다.
                        }
                    }
                    break;

                case 320:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devGripperPio.DiLeftProductExist.SetDi(false);
                            if ((GV.SimulationFlag & enSimulationFlag.FoupSingleDetect_Deposit) == enSimulationFlag.FoupSingleDetect_Deposit)
                                m_devGripperPio.DiRightProductExist.SetDi(true);
                            else
                                m_devGripperPio.DiRightProductExist.SetDi(false);
                        }

                        // Foup 유무 확인 & LD REQ OFF
                        bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                        bool foup_not_exist = m_devGripperPio.IsProductNotExist();
                        foup_not_exist &= pio_used ? m_devEqPio.IfFlagSend.OnIng : true;

                        if (foup_not_exist)
                        {
                            OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.CarrierRemoved);

                            SequenceLog.WriteLog(FuncName, string.Format("Foup Not Exist OK!"));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();
                            seqNo = 450; //390;
                        }
                        else if (m_devEqPio.IfFlagSend.Abort || XFunc.GetTickCount() - StartTicks > m_devEqPio.TimerTp3 * 1000)
                        {
                            m_devEqPio.IfFlagSend.Abort = false;
                            m_PioAbort = pio_used;

                            if (m_devGripperPio.IsProductNotExist() == false)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Exist NG!"));

                                if (m_devGripperPio.DiRightProductExist.IsDetected)
                                {
                                    AlarmId = m_devGripperPio.ALM_ProductRightExistAlarm.ID;
                                }
                                else if (m_devGripperPio.DiLeftProductExist.IsDetected)
                                {
                                    AlarmId = m_devGripperPio.ALM_ProductLeftExistAlarm.ID;
                                }
                                else if (IsBcrOverInange(ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus, current_port))
                                {
                                    AlarmId = m_ALM_BcrCheckOverInrangeTargetPositionError.ID;
                                    SequenceLog.WriteLog(FuncName, string.Format("BCR Check Over Inrange Target Position Alarm - BcrInrageError:{0}", bcrRangeError));
                                }
                                EqpAlarm.Set(AlarmId);
                                m_AutoRecoveryEnable = false;
                                SequenceLog.WriteLog(FuncName, $"Deposit Foup Abnormal Exist After Grip Open. Reson : Left:{m_devGripperPio.DiLeftProductExist.IsDetected}, Right:{m_devGripperPio.DiRightProductExist.IsDetected}");
                                ReturnSeqNo = 0;
                                seqNo = 1000;
                            }
                            else if (m_devEqPio.IfFlagSend.OnIng == false)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("m_devEqPio.IfFlagSend.OnIng == false"));
                                seqNo = 350; // Down 위치로 이동 후 Gripper Open
                            }
                        }
                    }
                    break;

                case 350:
                    {
                        // Hoist Port Teaching Down Position Move
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target2Position, m_TargetSlowVelSets);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down Move OK", m_devFoupGripper.AxisHoist.AxisName));
                            seqNo = 360;
                        }
                        else if (rv1 > 0)
                        {
                            if (m_devGripperPio.DiRightProductExist.IsDetected)
                            {
                                AlarmId = m_devGripperPio.ALM_ProductRightExistAlarm.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            else if (m_devGripperPio.DiLeftProductExist.IsDetected)
                            {
                                AlarmId = m_devGripperPio.ALM_ProductLeftExistAlarm.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            else
                            {
                                AlarmId = rv1;
                                EqpAlarm.Set(AlarmId);
                            }

                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Down Move NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_devFoupGripper.SeqAbort();

                            ReturnSeqNo = seqNo; // Open 확인 후 Home 동작을 하자
                            seqNo = 1000;
                        }
                    }
                    break;

                case 360:
                    {
                        // Gripper Close 동작
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devGripperPio.GripperClose();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Close OK"));
                            seqNo = 370;
                        }
                        else if (rv1 > 0)
                        {
                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Close NG"));
                            m_devGripperPio.SeqAbort();
                            if (IsBcrOverInange(ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus, current_port))
                            {
                                AlarmId = m_ALM_BcrCheckOverInrangeTargetPositionError.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("BCR Check Over Inrange Target Position Alarm - BcrInrageError:{0}", bcrRangeError));
                            }
                            else
                            {
                                AlarmId = rv1;
                            }
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; // Close 확인 후  Recovery
                            seqNo = 1000;
                        }
                    }
                    break;

                case 370:
                    {
                        // Foup 이 감지돼야 Up 동작을 하자...!
                        if (m_devGripperPio.IsProductExist())
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Exist Check OK"));
                            seqNo = 390;
                        }
                        else
                        {
                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Not Exist Check NG (Moving Interlock)"));
                            if (m_devGripperPio.DiLeftProductExist.IsDetected == false)
                                AlarmId = m_devGripperPio.ALM_ProductLeftNotExistMoveAlarm.ID;
                            else AlarmId = m_devGripperPio.ALM_ProductRightNotExistMoveAlarm.ID;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; // Open 확인 후 Gripper Open 부터 Recovery
                            seqNo = 1000;
                        }
                    }
                    break;

                case 390:
                    {
                        // Hoist Port Teaching Before Down Position Move
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_Target1Position, m_TargetSlowVelSets);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move OK", m_devFoupGripper.AxisHoist.AxisName));
                            
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 450;
                        }
                        else if (rv1 > 0)
                        {
                            m_AutoRecoveryEnable = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Before Down Move NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_devFoupGripper.SeqAbort();

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; // Open 확인 후 Home 동작을 하자
                            seqNo = 1000;
                        }
                    }
                    break;

                case 400:
                    {
                        // Hoist Stop
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.AxisHoist.GetDevAxis().Stop();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                            m_PioAbort = pio_used;
                            m_devEqPio.IfFlagSend.OperateCancel = true;
                            m_devFoupGripper.SeqAbort();
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Stop OK", m_devFoupGripper.AxisHoist.AxisName));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 410;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Stop NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_devFoupGripper.SeqAbort();
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 410;
                        }
                    }
                    break;

                case 410:
                    {
                        // Gripper Home
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Home(enAxisMask.aZ | enAxisMask.aY | enAxisMask.aT);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("[{0} | {1} | {2}] : Initialize OK", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            seqNo = 500;
                        }
                        else if (rv1 > 0)
                        {
                            m_AutoRecoveryEnable = false;
                            m_devFoupGripper.SeqAbort();
                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);

                            SequenceLog.WriteLog(FuncName, string.Format("[{0} | {1} | {2}] : Initialize Alarm - {3}", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName, EqpAlarm.GetAlarmMsg(AlarmId)));

                            ReturnSeqNo = seqNo; // Home 동작을 하자.
                            seqNo = 1000;
                        }
                    }
                    break;

                case 450:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 200) break;

                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Move(enAxisMask.aZ, m_WaitPosition, m_Target2VelSets);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }

                        if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                        {
                            bool continuous_Slide_Rotate_Enable = false;
                            continuous_Slide_Rotate_Enable |= Math.Abs(m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition() - m_WaitPosition.Z) < 20.0f;
                            continuous_Slide_Rotate_Enable &= m_devGripperPio.IsProductExist();

                            if (continuous_Slide_Rotate_Enable || m_MoveComp1)
                            {
                                m_MoveComp1 = false;
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Continuous Hoist Home Check & Slide & Rotate Move Start!", m_devFoupGripper.AxisHoist.AxisName));

                                seqNo = 465;
                            }
                            else if (m_InterlockSet)
                            {
                                m_AutoRecoveryEnable = false;

                                if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                                {
                                    AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                    EqpAlarm.Set(AlarmId);
                                }
                                SequenceLog.WriteLog(FuncName, string.Format("The first detection of the Look Down Sensor has begun. - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Wait Move Interlock", m_devFoupGripper.AxisHoist.AxisName));
                                m_devFoupGripper.SeqAbort();
                                m_InterlockCheckStartTime = XFunc.GetTickCount();
                                ReturnSeqNo = seqNo;
                                seqNo = 2000;
                            }
                        }
                        else if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            bool home_start = false;
                            enAxisInFlag state = (m_devFoupGripper.AxisHoist.GetDevAxis().GetAxis() as IAxisCommand).GetAxisCurStatus();
                            m_MotionRetry = 0;

                            if (state.HasFlag(enAxisInFlag.Org))
                            {
                                double origin_detect_error = (m_devFoupGripper.AxisHoist.GetDevAxis().GetAxis() as MpAxis).OriginOnDetectError;
                                double range = SetupManager.Instance.SetupHoist.OriginOnDetectErrorRange;
                                home_start = Math.Abs(origin_detect_error) > range;
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Wait Move OK, Home Sensor Check Abnormal [{1},{2}]", m_devFoupGripper.AxisHoist.AxisName, origin_detect_error, range));
                            }
                            else
                            {
                                home_start = true;
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Wait Move OK, Home Sensor OFF State", m_devFoupGripper.AxisHoist.AxisName));
                            }
                            if (home_start)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Wait Move OK, Home Start", m_devFoupGripper.AxisHoist.AxisName));
								StartTicks = XFunc.GetTickCount(); 
                                seqNo = 460;
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Wait Move OK", m_devFoupGripper.AxisHoist.AxisName));
                                seqNo = 465;
                            }
                        }
                        else if (m_InterlockSet)
                        {
                            m_AutoRecoveryEnable = false;

                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                            {
                                AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                EqpAlarm.Set(AlarmId);
                            }
                            SequenceLog.WriteLog(FuncName, string.Format("The first detection of the Look Down Sensor has begun. - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Wait Move Interlock", m_devFoupGripper.AxisHoist.AxisName));
                            m_devFoupGripper.SeqAbort();
                            m_InterlockCheckStartTime = XFunc.GetTickCount();
                            ReturnSeqNo = seqNo;
                            seqNo = 2000;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Wait Move NG", m_devFoupGripper.AxisHoist.AxisName));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 400; // Close 확인 후 Home 동작을 하자.
                            seqNo = 1000;
                        }
                    }
                    break;
                
                case 460:
                    {
                        // Gripper Home
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Home(enAxisMask.aZ);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("[{0} | {1} | {2}] : Initialize OK", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            seqNo = 465;
                        }
                        else if (rv1 > 0)
                        {
                            m_AutoRecoveryEnable = false;
                            m_devFoupGripper.SeqAbort();
                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);

                            SequenceLog.WriteLog(FuncName, string.Format("[{0} | {1} | {2}] : Initialize Alarm - {4}", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName, EqpAlarm.GetAlarmMsg(AlarmId)));

                            ReturnSeqNo = seqNo; // Home 동작을 하자.
                            seqNo = 1000;
                        }
                    }
                    break;

                case 465:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 200) break;

                        // Slide & Rotate Wait Teaching Position Move
                        if (!m_MoveComp1)
                        {
                            if (SetupManager.Instance.SetupOperation.Continuous_Motion_Use == Use.Use)
                            {
                                rv1 = m_devFoupGripper.ContinuousMove(enAxisMask.aYT, m_WaitPosition, m_Target2VelSets);
                            }
                            else
                            {
                                rv1 = m_devFoupGripper.Move(enAxisMask.aY | enAxisMask.aT, m_WaitPosition, m_Target2VelSets);
                            }

                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            bool checkSlidePosition = Math.Abs(m_devFoupGripper.AxisSlide.GetDevAxis().GetCurPosition()) < 3.0f;
                            bool checkRotatePosition = Math.Abs(m_devFoupGripper.AxisTurn.GetDevAxis().GetCurPosition()) < 1.0f;
                            if (checkSlidePosition && checkRotatePosition)
                            {
                                bool home_start = false;
                                enAxisInFlag state = (m_devFoupGripper.AxisHoist.GetDevAxis().GetAxis() as IAxisCommand).GetAxisCurStatus();
                                if (state.HasFlag(enAxisInFlag.Org))
                                {
                                    double origin_detect_error = (m_devFoupGripper.AxisHoist.GetDevAxis().GetAxis() as MpAxis).OriginOnDetectError;
                                    double range = SetupManager.Instance.SetupHoist.OriginOnDetectErrorRange;
                                    home_start = Math.Abs(origin_detect_error) > range;
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move OK, Home Sensor Check Abnormal#2 [{2},{3}]", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName, origin_detect_error, range));
                                }
                                else
                                {
                                    home_start = true;
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move OK, Home Sensor OFF State", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                                }
                                if (home_start)
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move OK, Home Start", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                                    seqNo = 480;
                                }
                                else
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move OK", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                                    seqNo = 500;
                                }
                            }
                            else
                            {
                                seqNo = 470;
                            }
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1} | {2}) Wait Move NG", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 400; // Close 확인 후 Home 동작을 하자.
                            seqNo = 1000;
                        }
                    }
                    break;
                case 470:
                    {
						if (XFunc.GetTickCount() - StartTicks < 200) break;
						
                        // retry Slide & Rotate Wait Teaching Position Move 
                        if (m_MotionRetry < 3)
                        {
                            bool checkSlidePosition = Math.Abs(m_devFoupGripper.AxisSlide.GetDevAxis().GetCurPosition()) < 3.0f;
                            bool checkRotatePosition = Math.Abs(m_devFoupGripper.AxisTurn.GetDevAxis().GetCurPosition()) < 1.0f;

                            if (checkSlidePosition && checkRotatePosition)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move OK2", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 500;
                            }
                            else
                            {
                                m_MotionRetry++;
                                StartTicks = XFunc.GetTickCount();
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1}) Wait Move NG. Retry Move Cnt:{2}", m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName, m_MotionRetry));
                                seqNo = 465;
                            }
                        }
                        else
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1} | {2}) Wait Move NG", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 400; // Close 확인 후 Home 동작을 하자.
                            seqNo = 1000;
                        }
                    }
                    break;

                case 480:
                    {
                        // Gripper Home
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Home(enAxisMask.aZ);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("[{0} | {1} | {2}] : Initialize OK#2", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            seqNo = 500;
                        }
                        else if (rv1 > 0)
                        {
                            m_AutoRecoveryEnable = false;
                            m_devFoupGripper.SeqAbort();
                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);

                            SequenceLog.WriteLog(FuncName, string.Format("[{0} | {1} | {2}] : Initialize Alarm - {4}", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName, EqpAlarm.GetAlarmMsg(AlarmId)));

                            ReturnSeqNo = seqNo; // Home 동작을 하자.
                            seqNo = 1000;
                        }
                    }
                    break;

                case 500:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devGripperPio.DiHoistHome.SetDi(false);
                            m_devGripperPio.DiHoistUp.SetDi(false);
                        }
                        m_devLookDown.SetNoUse();

                        bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                        bool abort = m_PioAbort || m_devEqPio.IfFlagSend.Abort;
                        if (IsFoupNotExist() && !abort)
                        {
                            // deposit success
                            SequenceLog.WriteLog(FuncName, "Deposit Complete");
                            ProcessDataHandler.Instance.CurVehicleStatus.SetCarrierStatus(IF.OCS.CarrierState.None);

                            //Acquire/Deposit Complete 될 때 Tracking 값을 변경하자..
                            TeachingOffsetAdapter.Instance.SetAutoTrackingValue(false, target_port, m_TrackingDiff);

                            if (pio_used) m_devEqPio.IfFlagSend.Complete = true;
                            seqNo = 510;
                        }
                        else
                        {
                            // deposit Fail
                            if (!IsFoupNotExist())
                                SequenceLog.WriteLog(FuncName, "Deposit Foup Exist (NG)");
                            if (abort) SequenceLog.WriteLog(FuncName, "Deposit PIO Abort (NG)");
                            if (pio_used) m_devEqPio.IfFlagSend.OperateCancel = true; // Alarm Reset 하고 Disconnect 해라~~

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 520;
                        }
                    }
                    break;

                case 510:
                    {
                        bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                        bool pio_complete = true;
                        if (pio_used) pio_complete &= m_devEqPio.IfFlagSend.PioOff;
                        bool abort = m_PioAbort || m_devEqPio.IfFlagSend.Abort;

                        if (pio_complete)
                        {
                            m_devEqPio.IfFlagSend.PioOff = false;
                            SequenceLog.WriteLog(FuncName, "PIO Disconnected Check OK");
                            // Deposite Complete Report
                            OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepositCompleted);
                            this.scm.Start = false;
                            this.scm.End = true;
                            rv = 0;
                            seqNo = 0;
                        }
                        else if (abort)
                        {
                            // deposit Fail
                            SequenceLog.WriteLog(FuncName, "Deposit PIO Abort (Reay OFF NG)");
                            if (pio_used) m_devEqPio.IfFlagSend.OperateCancel = true; // Alarm Reset 하고 Disconnect 해라~~

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 520;
                        }
                    }
                    break;

                case 520:
                    {
                        // 만일 Foup이 없는 경우는 Complete 처리하고 PIO OFF를 기다리자
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        // Abnormal Case
                        bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                        bool pio_complete = true;
                        bool pio_off_ng = false;
                        if (pio_used)
                        {
                            pio_complete &= m_devEqPio.IfFlagSend.PioOff;
                            pio_off_ng = m_devEqPio.IfFlagSend.PioOffNg;
                        }

                        if (pio_complete || pio_off_ng)
                        {
                            m_devEqPio.IfFlagSend.PioOff = false;
                            m_devEqPio.IfFlagSend.PioOffNg = false;
                            if (IsFoupNotExist()) ProcessDataHandler.Instance.CurVehicleStatus.SetCarrierStatus(IF.OCS.CarrierState.None);
                            else ProcessDataHandler.Instance.CurVehicleStatus.SetCarrierStatus(IF.OCS.CarrierState.Installed);

                            if (IsFoupNotExist() && pio_complete)
                            {
                                // deposit success
                                SequenceLog.WriteLog(FuncName, "Foup Not Exist & PIO Disconnected Check OK");
                                // Deposite Complete Report                                
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepositCompleted);
                                this.scm.Start = false;
                                this.scm.End = true;
                                rv = 0;
                                seqNo = 0;
                            }
                            else
                            {
                                
                                ProcessDataHandler.Instance.CurVehicleStatus.SetVehicleStatus(VehicleState.DepositFailed);
                                bool abort = m_PioAbort || m_devEqPio.IfFlagSend.Abort;

                                if (abort || pio_off_ng)
                                {
                                    m_devEqPio.IfFlagSend.Abort = false;
                                    SequenceLog.WriteLog(FuncName, "PIO PIO Alarm (Abort)");
                                    OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepositInterfaceFail);

                                    if (m_LookDownAlarmFlag)
                                    {
                                        if (m_ESSignalAlarmFlag)
                                            AlarmId = m_ALM_DepositFailESSignal.ID;
                                        else if (m_HOSignalAlarmFlag)
                                            AlarmId = m_ALM_DepositFailHOSignal.ID;
                                        else
                                            AlarmId = m_ALM_DepositFailLookDownSensor.ID;
											
                                    	EqpAlarm.Set(AlarmId);
                                    }
                                    else if (m_UpDetectInterlockSet)
                                    {
                                        if (IsBcrOverInange(ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus, current_port))
                                        {
                                            AlarmId = m_ALM_BcrCheckOverInrangeTargetPositionError.ID;
                                            EqpAlarm.Set(AlarmId);
                                            SequenceLog.WriteLog(FuncName, string.Format("BCR Check Over Inrange Target Position Alarm - BcrInrageError:{0}", bcrRangeError));
                                        }
                                        else
                                        {
                                            AlarmId = m_ALM_HoistUpSensorAbnormalDetectPositionError.ID;
                                        }
                                        m_UpDetectInterlockSet = false;
                                    }
                                    else if (m_LimitSensorDetectNg)
                                    {
                                        m_LimitSensorDetectNg = false;

                                        AlarmId = m_ALM_HoistLimitSensorAbnormalDetectPositionError.ID;
                                    }
                                    else
                                    {
                                        AlarmId = m_ALM_DepositFailError.ID;
                                        EqpAlarm.Set(AlarmId);
                                    }
                                    seqNo = 1000;
                                    ReturnSeqNo = 0;
                                }
                                else
                                {
                                    m_AutoRecoveryEnable = true;
                                    // deposit fail
                                    SequenceLog.WriteLog(FuncName, "Deposit Fail");
                                    OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepositFailed);
                                    if (m_LookDownAlarmFlag)
                                    {
                                        if (m_ESSignalAlarmFlag)
                                            AlarmId = m_ALM_DepositFailESSignal.ID;
                                        else if (m_HOSignalAlarmFlag)
                                            AlarmId = m_ALM_DepositFailHOSignal.ID;
                                        else
                                            AlarmId = m_ALM_DepositFailLookDownSensor.ID;
                                    }
                                    else if (m_UpDetectInterlockSet)
                                    {
                                        if (IsBcrOverInange(ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus, current_port))
                                        {
                                            AlarmId = m_ALM_BcrCheckOverInrangeTargetPositionError.ID;
                                            SequenceLog.WriteLog(FuncName, string.Format("BCR Check Over Inrange Target Position Alarm - BcrInrageError:{0}", bcrRangeError));
                                        }
                                        else
                                        {
                                            AlarmId = m_ALM_HoistUpSensorAbnormalDetectPositionWarning.ID;
                                        }
                                            m_UpDetectInterlockSet = false;
                                    }
                                    else if (m_LimitSensorDetectNg)
                                    {
                                        m_LimitSensorDetectNg = false;

                                        AlarmId = m_ALM_HoistLimitSensorAbnormalDetectPositionWarning.ID;
                                    }
                                    else
                                        AlarmId = m_ALM_DepositFailError.ID;
                                    EqpAlarm.Set(AlarmId);
                                    seqNo = 1000;
                                    ReturnSeqNo = 0;
                                }
                            }
                        }
                    }
                    break;

                case 600:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        // PIO Cancel 상태. 이미 Alarm 이 발생한 상태. OCS에서 알아서 Abort 시킬 것임.
                        SequenceLog.WriteLog(FuncName, "PIO Alarm Deposit Cancel");
                        m_devEqPio.IfFlagSend.Cancel = false;
                        m_devEqPio.IfFlagSend.OperateCancel = true; // Alarm reset 하고 Disconnect 해라~~
                        seqNo = 610;
                    }
                    break;

                case 610:
                    {
                        bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                        bool pio_complete = true;
                        bool pio_off_ng = false;
                        if (pio_used)
                        {
                            pio_complete &= m_devEqPio.IfFlagSend.PioOff;
                            pio_off_ng = m_devEqPio.IfFlagSend.PioOffNg;
                        }

                        if (pio_complete || pio_off_ng) // Disconnect 처리 완료 (OK or NG)
                        {
                            m_devEqPio.IfFlagSend.PioOff = false;
                            m_devEqPio.IfFlagSend.PioOffNg = false;

                            // PIO Cancel 상태. 이미 Alarm 이 발생한 상태. OCS에서 알아서 Abort 시킬 것임.
                            SequenceLog.WriteLog(FuncName, "PIO Alarm Deposit Cancel");
                            ProcessDataHandler.Instance.CurVehicleStatus.SetCarrierStatus(IF.OCS.CarrierState.Installed);
                            ProcessDataHandler.Instance.CurVehicleStatus.SetVehicleStatus(VehicleState.DepositFailed);
                            OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepositInterfaceFail);
                            //AlarmId = m_ALM_DepositFailError.ID;
                            //EqpAlarm.Set(AlarmId);
                            seqNo = 1000;
                            ReturnSeqNo = 0;
                        }
                    }
                    break;

                case 1000:
                    if (IsPushedSwitch.IsAlarmReset)
                    {
                        IsPushedSwitch.m_AlarmRstPushed = false;

                        //EqpAlarm.Reset(AlarmId);
                        //SequenceLog.WriteLog(FuncName, string.Format("Reset Alarm : Code[{0}]", AlarmId));
                        EqpAlarm.ResetAll(); //중알람이면 다시 띄우겠지.. 일단 다 지우자
                        SequenceLog.WriteLog(FuncName, string.Format("Reset Alarm Reset all"));
                        AlarmId = 0;
                        seqNo = ReturnSeqNo;
                    }
                    break;

                case 2000:
                    {
                        if (m_InterlockSet == false)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Move Interlock Sensor Release", m_devFoupGripper.AxisHoist.AxisName));
                            m_InterlockCheckStartTime = XFunc.GetTickCount();
                            seqNo = 2010;
                        }
                        else if (XFunc.GetTickCount() - m_InterlockCheckStartTime > SetupManager.Instance.SetupPIO.PioInterlockWaitTime * 1000)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Move Interlock Sensor Recovery", m_devFoupGripper.AxisHoist.AxisName));
                            m_LookDownAlarmFlag = true;
                            m_ESSignalAlarmFlag = m_ESInterlockSet;
                            m_HOSignalAlarmFlag = m_HOInterlockSet;
                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                            {
                                AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                EqpAlarm.Reset(AlarmId);
                            }

                            m_MoveComp1 = false;
                            m_devFoupGripper.SeqAbort();
                            seqNo = 400;
                        }
                    }
                    break;

                case 2010:
                    {
                        //release 대기시간 필요
                        if (m_InterlockSet)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Move Interlock Sensor ON", m_devFoupGripper.AxisHoist.AxisName));
                            m_InterlockCheckStartTime = XFunc.GetTickCount();
                            seqNo = 2000;
                        }
                        else if (XFunc.GetTickCount() - m_InterlockCheckStartTime > SetupManager.Instance.SetupPIO.PioInterlockWaitTime * 1000)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0}) Move Interlock Sensor Release Confirm, Move Start", m_devFoupGripper.AxisHoist.AxisName));
                            m_LookDownAlarmFlag = false;
                            m_ESSignalAlarmFlag = m_ESInterlockSet;
                            m_HOSignalAlarmFlag = m_HOInterlockSet;

                            if (AppConfig.Instance.ProjectType == enProjectType.CRRC)
                            {
                                AlarmId = m_ALM_LookDownSensorStatistics.ID;
                                EqpAlarm.Reset(AlarmId);
                            }

                            seqNo = ReturnSeqNo;
                        }
                    }
                    break;

                case 3000:
                    {
                        // Source Empty, Double Storage Event Report 후 대기
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Reset Double Storage or Source Empty Event. Retry !", AlarmId));
                            AlarmId = 0;
                            seqNo = ReturnSeqNo;
                        }
                    }
                    break;
            }
            this.SeqNo = seqNo;
            return rv;
        }
        #endregion
        #region Device Motor ReadyCheck
        private int CheckDeviceMotor()
        {
            AlarmId = 0;
            if (m_devFoupGripper.IsValid)
            {
                if (m_devFoupGripper.AxisHoist.IsValid) if (AlarmId == 0) AlarmId = m_devFoupGripper.AxisHoist.GetDevAxis().IsAxisReady() == false ? m_devFoupGripper.AxisHoist.GetDevAxis().ALM_NotReadyError.ID : 0;
                if (m_devFoupGripper.AxisSlide.IsValid) if (AlarmId == 0) AlarmId = m_devFoupGripper.AxisSlide.GetDevAxis().IsAxisReady() == false ? m_devFoupGripper.AxisSlide.GetDevAxis().ALM_NotReadyError.ID : 0;
                if (m_devFoupGripper.AxisTurn.IsValid) if (AlarmId == 0) AlarmId = m_devFoupGripper.AxisTurn.GetDevAxis().IsAxisReady() == false ? m_devFoupGripper.AxisTurn.GetDevAxis().ALM_NotReadyError.ID : 0;
            }
            if (m_devGripperPio.IsValid)
            {
                if (AlarmId == 0) AlarmId = m_devGripperPio.PioComm.IsGo() == false ? m_devGripperPio.PioComm.ALM_PioGoResponseError.ID : 0;
            }
            if (m_devAntiDropFront.IsValid) 
                if (AlarmId == 0) AlarmId = m_devAntiDropFront.IsAlarm() ? m_devAntiDropFront.ALM_AntiDropAlarm.ID : 0;
            if (m_devAntiDropRear.IsValid) 
                if (AlarmId == 0) AlarmId = m_devAntiDropRear.IsAlarm() ? m_devAntiDropRear.ALM_AntiDropAlarm.ID : 0;
            return AlarmId;
        }
        #endregion
        #region Methods - sensor, position function
        private bool IsFoupExist()
        {
            bool foup_exist = true;
            try
            {
                if (m_devGripperPio.IsValid)
                {
                    foup_exist &= m_devGripperPio.IsProductExist(); // sensor ng
                    foup_exist &= m_devGripperPio.DiGripperClose.IsDetected;
                    foup_exist &= !m_devGripperPio.IsHoistLimit();
                    foup_exist &= m_devGripperPio.IsHoistHome();
                }
                if (AppConfig.Instance.Simulation.MY_DEBUG)
                {
                    foup_exist |= ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return foup_exist;
        }

        private bool IsFoupNotExist()
        {
            bool foup_exist = true;
            try
            {
                if (m_devGripperPio.IsValid)
                {
                    foup_exist &= m_devGripperPio.IsProductNotExist();
                    foup_exist &= m_devGripperPio.DiGripperOpen.IsDetected;
                    foup_exist &= !m_devGripperPio.IsHoistLimit();
                    foup_exist &= !m_devGripperPio.IsHoistUp();
                    foup_exist &= !m_devGripperPio.IsHoistHome();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return foup_exist;
        }
        private bool IsDoubleStorage()
        {
            bool foup_exist = false; //default true해서 buffer일때만 확인하겠지...
            try
            {
                if (ProcessDataHandler.Instance.CurTransferCommand.IsValid)
                {
                    if (m_devFoupGripper.IsValid)
                    {
                        if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftBuffer)
                            foup_exist |= m_devFoupGripper.IsLeftDoubleStorage();
                        else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightBuffer)
                            foup_exist |= m_devFoupGripper.IsRightDoubleStorage();
                        else foup_exist = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return foup_exist;
        }
        private bool IsAntiDropUnlock()
        {
            bool anti_drop_unlock = true;
            try
            {
                anti_drop_unlock &= m_devAntiDropFront.IsValid ? m_devAntiDropFront.GetUnlock() : true;
                anti_drop_unlock &= m_devAntiDropRear.IsValid ? m_devAntiDropRear.GetUnlock() : true;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return anti_drop_unlock;
        }
        private bool IsAntiDropBusy()
        {
            bool anti_drop_busy = false;
            try
            {
                anti_drop_busy |= m_devAntiDropFront.IsValid ? m_devAntiDropFront.IsCommandBusy() : false;
                anti_drop_busy |= m_devAntiDropRear.IsValid ? m_devAntiDropRear.IsCommandBusy() : false;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return anti_drop_busy;
        }
        private void AntiDropCMDStop()
        {
            try
            {
                if (m_devAntiDropFront.IsValid ? m_devAntiDropFront.IsCommandBusy() : false)
                {
                    m_devAntiDropFront.SeqAbort();
                }
                if (m_devAntiDropRear.IsValid ? m_devAntiDropRear.IsCommandBusy() : false)
                {
                    m_devAntiDropRear.SeqAbort();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return ;
        }
        private bool IsGripOpenEnableCondition()
        {
            bool enable = true;
            try
            {
                if (m_devGripperPio.IsValid)
                {
                    enable &= !m_devGripperPio.IsHoistLimit();
                    enable &= m_devGripperPio.IsHoistUp();
                    enable &= m_devGripperPio.IsHoistHome();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return enable;
        }
        private void SaftyCheck()
        {
            bool interlock = false;
            try
            {
                bool look_down_use = true;
                look_down_use &= SetupManager.Instance.SetupSafty.PBSSensorUse == Use.Use;
                look_down_use &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PBSUsed;
                look_down_use &= m_devLookDown.IsUsingObs();
                if (look_down_use)
                    interlock |= m_devLookDown.GetFrontDetectState() > enFrontDetectState.enNone;

                m_ESInterlockSet = m_devEqPio.IfFlagSend.ES;
                interlock |= m_ESInterlockSet;

                if (AppConfig.Instance.ProjectType == enProjectType.ESWIN)
                {
                    m_HOInterlockSet = m_devEqPio.IfFlagSend.HO;
                    interlock |= m_HOInterlockSet;
                }

                double target = m_devFoupGripper.AxisHoist.GetDevAxis().GetTargetPos();
                double current = m_devFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                bool hoist_up = target - current > 10.0f; // 0 - (-500) = 500, 0 - (-1) = 1
                if (current > -450.0f && hoist_up) interlock = false;

                m_InterlockSet = interlock;
                if (m_OldInterlockState != interlock)
                {
                    m_OldInterlockState = interlock;
                    SequenceLog.WriteLog(FuncName, string.Format("Move Interlock Sensor Condition : LookDown={0}, IfFlagSend.ES={1},  IfFlagSend.HO={2}, CurPosition={3}", m_devLookDown.GetFrontDetectState(), m_devEqPio.IfFlagSend.ES, m_devEqPio.IfFlagSend.HO, current));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private bool IsPortTypeBuffer()
        {
            bool rv = false;
            rv |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftBuffer;
            rv |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightBuffer;
            return rv;
        }
        private bool CheckFoupCover()
        {
            //2024.12.31 Foup Cover Interlock Check..
            bool foup_cover_normal_state_interlock = true;
            //if (SetupManager.Instance.SetupSafty.FoupCoverOpenCheckUse == Use.Use) foup_cover_normal_state_interlock &= m_devFoupGripper.IsFoupCoverOpen() == false;
            //if (SetupManager.Instance.SetupSafty.FoupCoverExistCheckUse == Use.Use) foup_cover_normal_state_interlock &= m_devFoupGripper.IsFoupCoverDetect();
            foup_cover_normal_state_interlock &= m_devFoupGripper.DiFoupOpenCheck.IsValid ? m_devFoupGripper.IsFoupCoverOpen() == false : true;
            foup_cover_normal_state_interlock &= m_devFoupGripper.DiFoupCapCheck.IsValid ? m_devFoupGripper.IsFoupCoverDetect() : true;

            return foup_cover_normal_state_interlock;
        }
        private bool IsBcrOverInange(BcrStatus bcr, int portId)
        {
            bool inPosition = false; // 한바퀴 돌거니 ?

            DataItem_Port source_port = DatabaseHandler.Instance.DictionaryPortDataList[portId];
            double source_left_bcr = source_port.BarcodeLeft + source_port.DriveLeftOffset;
            double source_right_bcr = source_port.BarcodeRight + source_port.DriveRightOffset;
            double left_diff = Math.Abs(source_left_bcr - bcr.LeftBcr);
            double right_diff = Math.Abs(source_right_bcr - bcr.RightBcr);
            bcrRangeError = Math.Min(left_diff, right_diff);
            inPosition = bcrRangeError > 1.0f;

            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                if ((GV.SimulationFlag & enSimulationFlag.InRangeOver_Deposit) == enSimulationFlag.InRangeOver_Deposit)
                {
                    inPosition = true;
                }
            }

            return inPosition;
        }
        #endregion
    }

    public class SeqVehicleMove : XSeqFunc
    {
        public static readonly string FuncName = "[SeqVehicleMove]";
        XTimer m_Timer = new XTimer("SeqVehicleMove");

        #region Fields
        private DevTransfer m_devTransfer = null;
        private DevFoupGripper m_devFoupGripper = null;
        private DevGripperPIO m_devGripperPio = null;
        private DevAntiDrop m_devAntiDropFront = null;
        private DevAntiDrop m_devAntiDropRear = null;
        private DevSteer m_devSteer = null;
        private DevEqPIO m_devEqPio = null;

        private AlarmData m_ALM_MakeNodeListError = null;
        private AlarmData m_ALM_MakePathListError = null;
        private AlarmData m_ALM_OCSReceivedPathNullError = null;
        private AlarmData m_ALM_NextPathNullError = null;
        private AlarmData m_ALM_TeachingValueZeroError = null;

        private bool m_VehicleMoving = false;
        #endregion

        #region Constructor
        public SeqVehicleMove()
        {
            this.SeqName = $"SeqVehicleMove";

            m_devTransfer = DevicesManager.Instance.DevTransfer;
            m_devFoupGripper = DevicesManager.Instance.DevFoupGripper;
            m_devGripperPio = DevicesManager.Instance.DevGripperPIO;
            m_devAntiDropFront = DevicesManager.Instance.DevFrontAntiDrop;
            m_devAntiDropRear = DevicesManager.Instance.DevRearAntiDrop;
            m_devSteer = DevicesManager.Instance.DevSteer;
            m_devEqPio = DevicesManager.Instance.DevEqpPIO;
            AddCaseMemo();
            this.TaskLayer = (int)enTaskLayer.layerVehicleMove;

            m_ALM_MakeNodeListError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqVehicleMove", "Vehicle Move Sequence", "Make Node List Null Alarm");
            m_ALM_MakePathListError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqVehicleMove", "Vehicle Move Sequence", "Make Path List Null Alarm");
            m_ALM_OCSReceivedPathNullError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqVehicleMove", "Vehicle Move Sequence", "OCS Received Path Null Alarm");
            m_ALM_NextPathNullError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqVehicleMove", "Vehicle Move Sequence", "Make Next Path Null Alarm");
            m_ALM_TeachingValueZeroError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqVehicleMove", "Vehicle Move Sequence", "Teaching Port BCR Zero Alarm");
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
                if (m_devTransfer.IsValid) m_devTransfer.SeqAbort();
                if (m_devFoupGripper.IsValid) m_devFoupGripper.SeqAbort();
                if (m_devGripperPio.IsValid) m_devGripperPio.SeqAbort();
                if (m_devAntiDropFront.IsValid) m_devAntiDropFront.SeqAbort();
                if (m_devAntiDropRear.IsValid) m_devAntiDropRear.SeqAbort();
                if (m_devSteer.IsValid) m_devSteer.SeqAbort();
                this.scm.Abort = true;
            }
            this.InitSeq();
            m_VehicleMoving = false;
        }
        private void AddCaseMemo()
        {
            try
            {
                this.SeqCaseMemoLists.Add(0, string.Format("VehicleMove Sequence Start !"));
                this.SeqCaseMemoLists.Add(1, string.Format("VehicleMove Motor Condition Check !"));
                this.SeqCaseMemoLists.Add(10, string.Format("VehicleMove Wheel Motor ZeroSet !"));
                this.SeqCaseMemoLists.Add(20, string.Format("OCS & JCS Connection Check"));
                this.SeqCaseMemoLists.Add(30, string.Format("OCS & JCS Connection Wait"));
                this.SeqCaseMemoLists.Add(40, string.Format("Path Request"));
                this.SeqCaseMemoLists.Add(50, string.Format("Path Received Wait"));
                this.SeqCaseMemoLists.Add(60, string.Format("Make Full Path Information"));
                this.SeqCaseMemoLists.Add(70, string.Format("Current Position Check"));
                this.SeqCaseMemoLists.Add(100, string.Format("Override Status Check"));
                this.SeqCaseMemoLists.Add(110, string.Format("Collision Distance Check"));
                this.SeqCaseMemoLists.Add(120, string.Format("JCS Start"));
                this.SeqCaseMemoLists.Add(130, string.Format("JCS Running Check"));
                this.SeqCaseMemoLists.Add(200, string.Format("Set Moving Command"));
                this.SeqCaseMemoLists.Add(210, string.Format("SPL & MTL Pio Check Confirm"));
                this.SeqCaseMemoLists.Add(220, string.Format("SPL Insert On Signal Wait"));
                this.SeqCaseMemoLists.Add(230, string.Format("MTL Insert On Signal Wait"));
                this.SeqCaseMemoLists.Add(300, string.Format("Sequence Move"));
                this.SeqCaseMemoLists.Add(310, string.Format("Next Step Move Condition Check"));
                this.SeqCaseMemoLists.Add(320, string.Format("JCS & AutoDoor Permit Check"));
                this.SeqCaseMemoLists.Add(1000, string.Format("Alarm Reset Wait"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("{0} EXCEPTION : {1}", FuncName, ex.ToString()));
            }
        }
        #endregion

        #region Override
        public override int Do()
        {
            if (this.scm.Pause) return -1;

            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                stopWatch.Reset();
                stopWatch.Start();
            }

            int rv = -1;
            int rv1 = -1;
            int rv2 = -1;
            int rv3 = -1;

            TransferCommand curCommand = ProcessDataHandler.Instance.CurTransferCommand;
            VehicleStatus curVehicleStatus = ProcessDataHandler.Instance.CurVehicleStatus;
            OCSStatus ocsStatus = OCSCommManager.Instance.OcsStatus;
            if (m_VehicleMoving == false)
            {
                if (curCommand.CommandStatus == ProcessStatus.Aborting || curCommand.CommandStatus == ProcessStatus.Canceling)
                {
                    SequenceLog.WriteLog(FuncName, string.Format("SequenceMove Cancel(Vehicle Stop) {0}, TargetNode[{1}], ToNodeID[{2}]", curCommand.CommandStatus, curCommand.TargetNode, curVehicleStatus.CurrentPath.ToNodeID));
                    this.SeqNo = 900;
                }
            }

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        if (curCommand.ProcessCommand == OCSCommand.Go || curCommand.ProcessCommand == OCSCommand.Teaching)
                        {
                            curVehicleStatus.SetVehicleStatus(VehicleState.Go);
                        }
                        m_VehicleMoving = false;
                        this.scm.Start = true;
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 1;
                    }
                    break;

                case 1:
                    {
                        rv1 = CheckDeviceMotor();
                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName, "Vehicle Move Start");
                            
                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                                ProcessDataHandler.Instance.CurTransferCommand.IsNeedMakeRouteFullPath = true;

                            seqNo = 10;
                        }
                        else if (rv1 > 0 && (XFunc.GetTickCount() - StartTicks > 5000))
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Device & Motor State Check NG"));
                            EqpAlarm.Set(rv1);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 10:
                    {
                        rv1 = m_devTransfer.Home();
                        if (rv1 == 0)
                        {
                            // 출발전에 Motion 위치값
                            SequenceLog.WriteLog(FuncName, string.Format("Transfer Wheel : Zero Set OK"));
                            curVehicleStatus.CurrentBcrStatus.SetVirtualStartBcr();
                            seqNo = 20;
                        }
                        else if (rv1 > 0)
                        {
                            m_devTransfer.SeqAbort();
                            AlarmId = rv1;
                            SequenceLog.WriteLog(FuncName, string.Format("Transfer Wheel : Zero Set Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 20:
                    {
                        // OCS & JCS Connection 확인
                        bool connected = true;
                        connected &= ocsStatus.Connected;
                        connected &= ocsStatus.Connected;
                        if (connected)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("OCS & JCS Connected Check OK"));
                            curCommand.SetMakeNodes(curVehicleStatus);
                            seqNo = 40;
                        }
                        else
                        {
                            if (ocsStatus.Connected == false)
                                SequenceLog.WriteLog(FuncName, string.Format("OCS Connected NG, Waiting"));
                            else if (JCSCommManager.Instance.JcsStatus.Connected == false)
                                SequenceLog.WriteLog(FuncName, string.Format("JCS Connected NG, Waiting"));
                            seqNo = 30;
                        }
                    }
                    break;

                case 30:
                    {
                        bool connected = true;
                        connected &= OCSCommManager.Instance.OcsStatus.Connected;
                        connected &= JCSCommManager.Instance.JcsStatus.Connected;
                        if (connected)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("OCS & JCS Connected Check OK"));
                            curCommand.SetMakeNodes(curVehicleStatus);
                            seqNo = 40;
                        }
                    }
                    break;

                case 40:
                    {
                        // Path Request 조건을 만들자 !
                        // m_DestinationChangeCommand.IsValid를 확인하여 변경 요청이 있으면 Destination을 바꾸어서 Node 검색
                        // Node가 2개 이하인 경우 Vehicle에서 경로를 찾고 이동 시키자!
                        // 그렇지 않으면 PathRequest를 하여 경로를 OCS에서 받자
                        //bool clear_paths = true;
                        //bool ok = curCommand.MakeNodes(curVehicleStatus, clear_paths); // PathNodes는 Full Node List 임. 
                        rv1 = curCommand.ResultMakeNodes();
                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Make Node List : {0}", string.Join("-", curCommand.PathNodes)));
                            if (curCommand.PathNodes.Count > 1) // 0인 경우 자기Path내 이동, 1인 경우 이웃 Path까지 이동. 1일 경우에는 Start/End Node가 같다....
                            {
                                PathRequestType type = PathRequestType.Go;
                                if (curCommand.ProcessCommand == OCSCommand.Transfer)
                                {
                                    if (curVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToDest) type = PathRequestType.MoveToDestination;
                                    else type = PathRequestType.MoveToSource;
                                }
                                ocsStatus.PathRequestType = type;
                                ocsStatus.PathRequestStartNode = PassRequestStartNodeCheck(curVehicleStatus.CurrentPath);
                                ocsStatus.PathRequestEndNode = curCommand.EndNode;
                                ocsStatus.SetFlag(InterfaceFlag.PathRequest, FlagValue.ON);
                                SequenceLog.WriteLog(FuncName, string.Format("Path Request : Type:{0}, Start:{1}, End:{2}", 
                                    ocsStatus.PathRequestType,
                                    ocsStatus.PathRequestStartNode,
                                    ocsStatus.PathRequestEndNode));

                                StartTicks = XFunc.GetTickCount();
                                seqNo = 50;
                            }
                            else
                            {
                                // 동일 Link 이동인 경우는 PassRequest 할 필요가 없다..
                                SequenceLog.WriteLog(FuncName, string.Format("Single Path Move : {0}", curCommand.EndNode));
                                curCommand.SetMakeRouteFullPath(curVehicleStatus);
                                //seqNo = 60;
                                seqNo = 55;
                            }
                        }
                        else if (rv1 > 0)
                        {
                            AlarmId = m_ALM_MakeNodeListError.ID;
                            EqpAlarm.Set(AlarmId);
                            SequenceLog.WriteLog(FuncName, string.Format("Make Node List Alarm - Command:{0}", curCommand));
                            ReturnSeqNo = seqNo; //Data 확인 후 다시 여기로...
                            seqNo = 1000;
                        }
                    }
                    break;

                case 50:
                    {
                        if (ocsStatus.GetFlag(InterfaceFlag.PathReceived) == FlagValue.ON)
                        {
                            ocsStatus.ResetFlag(InterfaceFlag.PathReceived);
                            SequenceLog.WriteLog(FuncName, string.Format("Received Path List : {0}", string.Join("-", ocsStatus.PathRequestRecvNodes)));
                            ocsStatus.OCSRestartReschedule = false;

                            int fromNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.FromNodeID;
                            int toNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.ToNodeID;
                            int startNode = ocsStatus.PathRequestStartNode;
                            int endNode = ocsStatus.PathRequestEndNode;
                            // next path의 start node를 first로 선택한 경우 request와 current node는 다르다.
                            bool path_list_check = false;
                            if (ocsStatus.PathRequestRecvNodes.Count > 0)
                            {
                                path_list_check = ocsStatus.PathRequestRecvNodes.First() == fromNode;
                                path_list_check |= ocsStatus.PathRequestRecvNodes.First() == toNode;
                                path_list_check |= ocsStatus.PathRequestRecvNodes.First() == startNode;
                                path_list_check &= ocsStatus.PathRequestRecvNodes.Last() == endNode;
                            }
                            if (path_list_check)
                            {
                                if (ocsStatus.PathRequestRecvNodes.Count > 0)
                                {
                                    curCommand.SetPathNodes(ocsStatus.PathRequestRecvNodes);
                                    curCommand.SetMakeRouteFullPath(curVehicleStatus);
                                    //seqNo = 60;
                                    seqNo = 55;
                                }
                                else
                                {
                                    AlarmId = m_ALM_OCSReceivedPathNullError.ID;
                                    EqpAlarm.Set(AlarmId);
                                    SequenceLog.WriteLog(FuncName, string.Format("Received Path Null Alarm - Command:{0}", curCommand));
                                    ReturnSeqNo = seqNo; // Cancel/Abort 처리해야 할것 같음.
                                    seqNo = 1000;
                                }
                            }
                            else
                            {
                                AlarmId = m_ALM_OCSReceivedPathNullError.ID;
                                EqpAlarm.Set(AlarmId);
                                SequenceLog.WriteLog(FuncName, string.Format("Received Path Abnormal (Not Contain Start/End Node[{0},{1}]) Alarm - Command:{2}", startNode, endNode, curCommand));
                                ReturnSeqNo = seqNo; // Cancel/Abort 처리해야 할것 같음.
                                seqNo = 1000;
                            }
                        }
                        else if (ocsStatus.GetFlag(InterfaceFlag.PathReceivedNG) == FlagValue.ON)
                        {
                            ocsStatus.ResetFlag(InterfaceFlag.PathReceivedNG);
                            AlarmId = m_ALM_OCSReceivedPathNullError.ID;
                            EqpAlarm.Set(AlarmId);
                            SequenceLog.WriteLog(FuncName, string.Format("Received Path Abnormal (Path Node List is Null) Alarm - Command:{0}", curCommand));
                            ReturnSeqNo = seqNo; // Cancel/Abort 처리해야 할것 같음.
                            seqNo = 1000;
                        }
                        else if (ocsStatus.OCSRestartReschedule && XFunc.GetTickCount() - StartTicks > 1000)
                        {
                            ocsStatus.OCSRestartReschedule = false;
                            // 마지막 Setting 되어 있는 명령이 있을 거다...!
                            ocsStatus.SetFlag(InterfaceFlag.PathRequest, FlagValue.ON);
                            SequenceLog.WriteLog(FuncName, string.Format("OCS Restart, Retry Path Request !"));
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 5000)
                        {
                            ocsStatus.OCSRestartReschedule = false;
                            // 마지막 Setting 되어 있는 명령이 있을 거다...!
                            ocsStatus.SetFlag(InterfaceFlag.PathRequest, FlagValue.ON);
                            SequenceLog.WriteLog(FuncName, string.Format("Path Received TimeOver, Retry Path Request ! need Check OCS path"));
                            StartTicks = XFunc.GetTickCount();
                        }
                    }
                    break;

                case 55:
                    {
                        if(TaskSearchLink.Instance.SearchLink.UpdateCurrentPathComplete)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Current Path Update Complete in SearchLink."));
                            seqNo = 60;
                        }
                    }
                    break;

                case 60:
                    {
                        //bool ok = curCommand.MakeRouteFullPath(curVehicleStatus);
                        rv1 = curCommand.ResultMakeRouteFullPath();
                        if (rv1 == 0)
                        {
                            string msg = string.Format("{0}\r\n", curCommand.ToString());
                            foreach (Sineva.VHL.Data.Process.Path path in curCommand.PathMaps) { msg += string.Format("{0}\r\n", path.ToString()); }
                            SequenceLog.WriteLog(FuncName, string.Format("MakeRouteFullPath - Command:{0}", msg));

                            if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LinkID == curCommand.PathMaps.First().LinkID)
                            {
                                ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Index = curCommand.PathMaps.First().Index;
                            }

                            seqNo = 70;
                        }
                        else if (rv1 > 0)
                        {
                            // Location인데 BCR 값이 0인 경우 NotTeachingAlarm
                            bool teaching_alarm = false;
                            bool carrier_installed = curVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
                            bool go_move = true;
                            go_move &= curCommand.ProcessCommand == OCSCommand.Go;
                            go_move &= curCommand.TypeOfDestination == enGoCommandType.ByDistance;
                            if (!go_move)
                            {
                                int portID = 0;
                                if (curCommand.ProcessCommand == OCSCommand.Go || curCommand.ProcessCommand == OCSCommand.Teaching) portID = curCommand.DestinationID;
                                else if (carrier_installed && curCommand.DestinationID != 0) portID = curCommand.DestinationID;
                                else if (curCommand.SourceID != 0) portID = curCommand.SourceID;
                                if (portID != 0 && DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(portID))
                                {
                                    bool teaching_zero = true;
                                    teaching_zero &= DatabaseHandler.Instance.DictionaryPortDataList[portID].BarcodeLeft == 0.0f;
                                    teaching_zero &= DatabaseHandler.Instance.DictionaryPortDataList[portID].BarcodeRight == 0.0f;
                                    teaching_alarm = teaching_zero;
                                }
                            }
                            if (teaching_alarm)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("MakeRouteFullPath Teaching Port BCR Zero Alarm - Command:{0}", curCommand));
                                AlarmId = m_ALM_TeachingValueZeroError.ID;
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = seqNo; //Data 확인 후 다시 여기로...
                                seqNo = 1000;
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("MakeRouteFullPath Path Calculate Alarm - Command:{0}", curCommand));
                                AlarmId = m_ALM_MakePathListError.ID;
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = seqNo; //Data 확인 후 다시 여기로...
                                seqNo = 1000;
                            }
                        }
                    }
                    break;

                case 70:
                    {
                        // 정위치 확인 후 target position이면 완료
                        double curBCR = 0.0f, targetBCR = 0.0f;
                        if (IsCommandTargetPosition(curCommand, curVehicleStatus.CurrentBcrStatus, ref curBCR, ref targetBCR))
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("VehicleMove Skip ! Target Position-{0} equal Cur Position-{1}", curBCR, targetBCR));
                            seqNo = 900;
                        }
                        else if (curVehicleStatus.CurrentPath.Certain) // 정상적으로 current path를 찾았군...
                        {
                            if (Math.Abs(curVehicleStatus.CurrentBcrStatus.VirtualRunBcr) < 1.0f)
                            {
                                AntiDropLock();

                                // Reset이 정상적으로 되었군.
                                SequenceLog.WriteLog(FuncName, string.Format("Virtual BCR Reset ! VirtualBcr-{0}, VirtualRunBcr-{1}", 
                                    curVehicleStatus.CurrentBcrStatus.VirtualBcr, curVehicleStatus.CurrentBcrStatus.VirtualRunBcr));
                                seqNo = 100;
                            }
                            else
                            {
                                // Virtual BCR값을 초기화 하자~~
                                curVehicleStatus.CurrentBcrStatus.SetVirtualStartBcr();
                            }
                        }
                    }
                    break;

                case 100:
                    {
                        //EventHandlerManager.Instance.InvokeUpdatePathData(null, false);

                        // Override 상태 확인, Override가 걸려 있는 경우 굳이 출발 명령을 내릴 이유가 없다. waiting
                        if (curVehicleStatus.ObsStatus.OverrideRatio < 1.0f)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Wait Command Run, Override Ratio = {0}", curVehicleStatus.ObsStatus.OverrideRatio));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        else
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Override Check OK, Override Ratio = {0}", curVehicleStatus.ObsStatus.OverrideRatio));
                            seqNo = 110;
                        }
                    }
                    break;

                case 110:
                    {
                        // 근처에 AutoDoor가 있는가 ? 속도 확인 불필요함.
                        DataItem_Node fromNode = curVehicleStatus.CurrentPath.IsFromNode();
                        DataItem_Node toNode = curVehicleStatus.CurrentPath.IsToNode();
                        bool dont_check = curVehicleStatus.CurrentPath.Type == LinkType.AutoDoorCurve;
                        dont_check |= curVehicleStatus.CurrentPath.Type == LinkType.AutoDoorStraight;
                        dont_check |= fromNode != null ? fromNode.Type == NodeType.AutoDoorIn1 : false;
                        dont_check |= fromNode != null ? fromNode.Type == NodeType.AutoDoorIn2 : false;
                        dont_check |= toNode != null ? toNode.Type == NodeType.AutoDoorIn1 : false;
                        dont_check |= toNode != null ? toNode.Type == NodeType.AutoDoorIn2 : false;
                        // 충돌 거리가 확보되면 명령을 내리자 

                        bool CollisionOK = curVehicleStatus.CurrentPath.IsCorner() ? curVehicleStatus.ObsStatus.ObsUpperSensorState <= enFrontDetectState.enDeccelation1 : curVehicleStatus.ObsStatus.ObsUpperSensorState <= enFrontDetectState.enDeccelation6;
                        CollisionOK |= curVehicleStatus.ObsStatus.CollisionDistance > SetupManager.Instance.SetupWheel.VehicleMoveStartEnableCollisionDistance;

                        if (CollisionOK || dont_check)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Release Command Run, Collision Distance ={0} Check OK", curVehicleStatus.ObsStatus.CollisionDistance));
                            seqNo = 120;
                        }
                    }
                    break;

                case 120:
                    {
                        TaskInterface.Instance.AutoDoorControl.AutoDoorStart(curCommand.FullPathNodes);
                        TaskJCS.Instance.JcsControl.JCSStart(curCommand);
                        SequenceLog.WriteLog(FuncName, "JCS & AutoDoor Start");
                        seqNo = 130;
                    }
                    break;

                case 130:
                    {
                        // AutoDoor는 굳이 확인할 필요 없다...!
                        if (TaskJCS.Instance.JcsControl.JcsRunning)
                        {
                            TaskJCS.Instance.JcsControl.JcsRunning = false;
                            SequenceLog.WriteLog(FuncName, "JCS Running Confirm");
                            seqNo = 200;
                        }
                    }
                    break;

                case 200:
                    {
                        bool moving = GV.WheelBusy;
                        m_VehicleMoving = moving;

                        // Destination Change가 내려 왔을 경우.
                        bool destination_change_valid = ProcessDataHandler.Instance.DestinationChangeCommand.IsValid;
                        destination_change_valid &= curVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToDest; // Destination 이동 중
                        destination_change_valid &= !moving;

                        bool RouteChangeStart = !moving;
                        RouteChangeStart &= GV.OpRouteChange || curCommand.CommandStatus == ProcessStatus.RouteChanging;
                        
                        if (destination_change_valid)
                        {
                            ProcessDataHandler.Instance.DestinationChangeCommand.IsValid = false;
                            curCommand.DestinationID = ProcessDataHandler.Instance.DestinationChangeCommand.DestinationID;
                            seqNo = 30; // 다시 Schedule 해라 ~~~
                        }
                        else if(RouteChangeStart)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Route Change Start!! Need Path Request!!"));
                            curCommand.SetProcessStatus(ProcessStatus.Processing);
                            GV.OpRouteChange = false;
                            GV.RouteChangeOk = true;

                            seqNo = 30;
                        }
                        else
                        {
                            // EndNode가 마지막 도착 위치 임. CurrentPath.FromNodeID == EndNode 일 경우에는 BCR 정위치 확인 필요.
                            // m_ProcessDataHandler.CurVehicleStatus.IsInPosition 도착 확인용.
                            // m_ProcessDataHandler.CurVehicleStatus.IsNearPosition 마지막 Link에 진입했다...
                            // PathMaps => RunPathMaps을 계산 하자 !
                            bool ok = curCommand.MakeNextTargetPath(curVehicleStatus.CurrentPath, curVehicleStatus);
                            if (ok)
                            {
                                // Virtual BCR값을 초기화 하자~~
                                //curVehicleStatus.CurrentBcrStatus.SetVirtualStartBcr();
                                string msg = string.Format("{0}\r\n", curCommand.TargetNode.ToString());
                                foreach (Sineva.VHL.Data.Process.Path path in curCommand.RunPathMaps) { msg += string.Format("{0}\r\n", path.ToString()); }
                                SequenceLog.WriteLog(FuncName, string.Format("\r\nMakeNextTargetPath - NextTarget:{0}", msg));
                                
                                StartTicks = XFunc.GetTickCount();

                                seqNo = 210;
                            }
                            else
                            {
                                // 더이상 진행할 Path가 없다고 하네...
                                // 그럼 목표 위치에 도달했나 ? 그렇지 않으면 뭔가 이상함(Alarm 띄우자)
                                if (curVehicleStatus.IsInPosition)
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("Next Target Path null & InPosition ! Retry Schedule !"));
                                    seqNo = 900;
                                }
                                else
                                {
                                    // Pass를 새로 받아서 다시 이동해 보자 ~~ 더이상 진행할 path가 없다고 하는데 목표 위치가 아니다...
                                    SequenceLog.WriteLog(FuncName, string.Format("Next Target Path null & Not InPosition ! Retry Schedule !"));
                                    //seqNo = 900;

                                    AlarmId = m_ALM_NextPathNullError.ID;
                                    EqpAlarm.Set(AlarmId);
                                    ReturnSeqNo = 900; // Cancel/Abort 처리해야 할것 같음.
                                    seqNo = 1000;
                                }
                            }
                        }
                    }
                    break;

                case 210:
                    {
                        if ((XFunc.GetTickCount() - StartTicks < 100)) break;
                        // MTL/SPL은 출발전에 확인. PIO Run
                        // AutoDoor/JCS는 도착 시점에 확인
                        double bcrLeft = curVehicleStatus.CurrentBcrStatus.LeftBcr;
                        double bcrRight = curVehicleStatus.CurrentBcrStatus.RightBcr;
                        DataItem_Node curNode = curVehicleStatus.CurrentPath.IsNearNode(bcrLeft, bcrRight); // 무조건 ToNode를 선택할 수 없다.
                        if (curNode.Type == NodeType.Lifter)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("PIO Use, SPL Interface Start!"));
                            m_devEqPio.IfFlagSpl.CS = 2; // 1부터 시작, 진입1, 진출2
                            seqNo = 230;
                        }
                        else if (curNode.Type == NodeType.LifterIn)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("PIO Use, SPL Interface Start!"));
                            m_devEqPio.IfFlagSpl.CS = 1; // 1부터 시작, 진입1, 진출2
                            m_devEqPio.IfFlagSpl.StartReq = true;
                            seqNo = 230;
                        }
                        else if (curNode.Type == NodeType.MTLIn || curNode.Type == NodeType.MTL)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("PIO Use, MTL Interface Start!"));
                            DataItem_Node targetNode = DatabaseHandler.Instance.GetNodeDataOrNull(curCommand.TargetNode);
                            if (targetNode.Type == NodeType.MTL)
                                m_devEqPio.IfFlagMtl.CS = 2; // (Rail Out)진입2
                            else
                            {
                                if(curNode.Type == NodeType.MTL)
                                    m_devEqPio.IfFlagMtl.CS = 1; // (Rail In)진출1
                                else
                                    m_devEqPio.IfFlagMtl.CS = 3; // (Pass)진입&진출3
                            } 
                            m_devEqPio.IfFlagMtl.StartReq = true;
                            seqNo = 240;
                        }
                        else
                        {
                            curVehicleStatus.ObsStatus.OverrideDontControl = false;

                            // Steer Condition Check
                            bool steer_permit = true;
                            enSteerDirection set_dir = curVehicleStatus.CurrentPath.SteerDirection;
                            if (set_dir != enSteerDirection.DontCare)
                            {
                                double curPos = curVehicleStatus.CurrentPath.CurrentPositionOfLink;
                                double setFrontPos = curVehicleStatus.CurrentPath.SteerGuideLengthFromNode;
                                double setRearPos = curVehicleStatus.CurrentPath.SteerGuideLengthFromNode + DevicesManager.Instance.DevSteer.FrontRearOffset;
                                steer_permit &= curPos > setFrontPos ? m_devSteer.GetFrontSteerDirection() == set_dir : true;
                                steer_permit &= curPos > setRearPos ? m_devSteer.GetRearSteerDirection() == set_dir : true;
                            }

                            // 첫번째인지 ? 마지막 ? Path 인지 구분
                            bool firstPath = curVehicleStatus.CurrentPath.FromLinkID == 0;
                            bool lastPath = curVehicleStatus.CurrentPath.ToLinkID == 0;
                            bool toNodeisJCS = curNode.JCSCheck > 0;
                            toNodeisJCS |= TaskJCS.Instance.JcsControl.IsJcsArea;
                            bool toNodeisAutoDoor = curNode.Type == NodeType.AutoDoorIn1 || curNode.Type == NodeType.AutoDoorIn2;
                            toNodeisAutoDoor &= TaskInterface.Instance.AutoDoorControl.IsAutoDoorOutExist(curNode.NodeID, 5);

                            // toNode가 JCS면 Permit을 받고 가자 !
                             bool jcs_permit = true;
                            if (!lastPath && toNodeisJCS)
                            {
                                jcs_permit &= TaskJCS.Instance.JcsControl.ReceivedPermit;
                            }
                            bool autoDoor_permit = true;
                            if (!lastPath && toNodeisAutoDoor)
                            {
                                autoDoor_permit &= TaskInterface.Instance.AutoDoorControl.ReceivedPermit;
                            }
                            if (jcs_permit && autoDoor_permit && steer_permit)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Moving Start!"));
                                //임시 로그
                                SequenceLog.WriteLog(FuncName, $"CurNode: {curNode.NodeID}, PassRequest : {TaskJCS.Instance.JcsControl.PassRequest}, Pass Permit : {TaskJCS.Instance.JcsControl.ReceivedPermit}, autoDoor permit : {TaskInterface.Instance.AutoDoorControl.ReceivedPermit}");

                                seqNo = 220;
                            }
                            else if((XFunc.GetTickCount() - StartTicks > 5 * 1000)) //5초에 한번씩 Log 찍자.. Log보면 전부다 True여야하는데 출발을 안해서리... 뭐때문인지 확실하지가 않네..
                            {
                                if(!jcs_permit)
                                    SequenceLog.WriteLog(FuncName, string.Format("Wait JCS Pass Permit, PassRequest : {0}, Pass Permit : {1}", TaskJCS.Instance.JcsControl.PassRequest, TaskJCS.Instance.JcsControl.ReceivedPermit));
                                if (!autoDoor_permit)
                                    SequenceLog.WriteLog(FuncName, string.Format("Wait Auto Door Pass Permit"));
                                if (!steer_permit)
                                    SequenceLog.WriteLog(FuncName, string.Format("Wait Steer Permit"));

                                StartTicks = XFunc.GetTickCount();
                            }

                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                                System.Diagnostics.Debug.WriteLine($"{time}, SeqVehicle Move-{seqNo},      {TaskJCS.Instance.JcsControl.ReceivedPermit},{TaskInterface.Instance.AutoDoorControl.ReceivedPermit},{toNodeisJCS}, {toNodeisAutoDoor}, NodeID, {curNode.NodeID}, NodeType, {curNode.Type}");
                            }
                        }
                    }
                    break;

                case 220:
                    {
                        // BCR Scan 중 신규명령이 못내려가도록 막자~~~
                        if (m_devTransfer.AxisMaster.GetDevAxis().GetAxis().SequenceState.IsExternalEncoderRun) break;
                        // 위치 계산하고 Motion 명령을 내리자 !
                        rv1 = m_devTransfer.SetCommand(curCommand.RunPathMaps);
                        if (rv1 == 0)
                        {
                            curCommand.SetPathMotionRun(curCommand.RunPathMaps); // MotionRun을 true 하자~~
                            curVehicleStatus.CurrentBcrStatus.SetVirtualStartBcr();
                            SequenceLog.WriteLog(FuncName, string.Format("Motion SetCommand OK"));
                            seqNo = 300;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Motion SetCommand Alarm"));
                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; //Data 확인 후 다시 여기로...
                            seqNo = 1000;
                        }
                    }
                    break;

                case 230:
                    {
                        if (m_devEqPio.IfFlagSpl.Busy)
                        {
                            m_devEqPio.IfFlagSpl.Busy = false;
                            curVehicleStatus.ObsStatus.OverrideDontControl = true;
                            // 이동 하라고 하네....
                            SequenceLog.WriteLog(FuncName, string.Format("PIO Use => SPL Busy, Moving Start !"));
                            seqNo = 220;
                        }
                    }
                    break;

                case 240:
                    {
                        if (m_devEqPio.IfFlagMtl.Busy)
                        {
                            m_devEqPio.IfFlagMtl.Busy = false;
                            curVehicleStatus.ObsStatus.OverrideDontControl = true;
                            // 이동 하라고 하네....
                            SequenceLog.WriteLog(FuncName, string.Format("PIO Use => MTL Busy, Moving Start !"));
                            seqNo = 220;
                        }
                    }
                    break;

                case 300:
                    {
                        m_VehicleMoving = true;
                        if (curCommand.TargetNodeOfCommandSet != curCommand.TargetNode) curCommand.TargetNodeOfCommandSet = curCommand.TargetNode;
                        // Anti Drop Release
                        AntiDropRelease();
                        //Early Motion
                        EarlyMotion();
                        // Last Step Process
                        bool last_step = curCommand.RunPathMaps.Count > 0 ?
                            (curCommand.RunPathMaps.Last().ToNodeID == curCommand.PathMaps.Last().ToNodeID && curCommand.RunPathMaps.Last().Index == curCommand.PathMaps.Last().Index) :
                            true;
                        double target_velocity = m_devTransfer.AxisMaster.GetDevAxis().GetTrajectoryVelocity();
                        // Link Type check... LastPath가 RightBranchStraight or LeftBranchStraight 경우. Port 가 있다고 생각함. 이때는 Permit check를 하지 말자
                        bool branch_junction_straight = curCommand.PathMaps.Last().Type == LinkType.LeftBranchStraight ||
                                                        curCommand.PathMaps.Last().Type == LinkType.RightBranchStraight ||
                                                        curCommand.PathMaps.Last().Type == LinkType.LeftJunctionStraight ||
                                                        curCommand.PathMaps.Last().Type == LinkType.RightJunctionStraight;
                        bool last_step_move = false;
                        if (curCommand.PathMaps.Count > 1)
                            last_step_move |= curCommand.TargetNode == curCommand.PathMaps[curCommand.PathMaps.Count - 1].FromNodeID;
                        if (curCommand.PathMaps.Count > 2)
                            last_step_move |= curCommand.TargetNode == curCommand.PathMaps[curCommand.PathMaps.Count - 2].FromNodeID;

                        if (branch_junction_straight && last_step_move)
                        {
                            target_velocity = Math.Min(curCommand.PathMaps.Last().Velocity, curCommand.PathMaps.Last().RunDistance);
                        }

                        // 이동 모니터
                        // BCR과 Encorder값 차이가 심하다... 다시 ... SequenceMove해라....
                        // curVehicleStatus.DistanceErrorBCREncorder
                        double curVelocity = m_devTransfer.AxisMaster.GetDevAxis().GetCurVelocity();
                        bool velocity_range = curVelocity < curCommand.StopCheckVelocity + 10.0f;
                        velocity_range &= curVelocity < target_velocity + 10.0f;
                        velocity_range &= curVelocity > 10.0f; // 도착 시점에 새로운 명령을 날리면 ABS Move->Stop 상황이 발생할수 있어 꼬인다....
                        bool pass_permit = TaskInterface.Instance.AutoDoorControl.PassPermitCheck || TaskJCS.Instance.JcsControl.PassPermitCheck;
                        // ToNode가 JCS일때만 Permit 처리, AutoDoor Area일때만 AutoDoor Permit 처리
                        if (TaskInterface.Instance.AutoDoorControl.PassPermitCheck) // AutoDoor는 PassRequest NG일 경우가 없다...pass request가 완료 된 후 receivedpermit
                            pass_permit &= TaskInterface.Instance.AutoDoorControl.ReceivedPermit;
                        if (TaskJCS.Instance.JcsControl.PassPermitCheck)
                            pass_permit &= TaskJCS.Instance.JcsControl.ReceivedPermit && SetupManager.Instance.SetupJCS.ContinueMode;
                        pass_permit &= !last_step ? curCommand.TargetNode == curVehicleStatus.CurrentPath.ToNodeID : false; // 중간에 멈추는 경우 targetNode => ToNode
                        pass_permit &= curCommand.RemainBcrDistance < curCommand.StopCheckDistance;
                        pass_permit &= curCommand.RemainBcrDistance > 50.0f; // 도착 시점에 새로운 명령을 날리면 ABS Move->Stop 상황이 발생할수 있어 꼬인다....
                        pass_permit &= curCommand.IsStopCheckVelocity ? velocity_range : true;
                        pass_permit &= GV.WheelBusy;
                        pass_permit &= curVehicleStatus.ObsStatus.MxpOverrideRatio < 1.0f ? false : true; // override 가 걸려 있으면 그냥 멈추자 !
                        pass_permit &= curCommand.CommandStatus != ProcessStatus.Canceling;
                        //pass_permit &= curCommand.CommandStatus != ProcessStatus.Aborting;

                        bool following_error = curVehicleStatus.DistanceErrorBCREncorder;
                        following_error &= curVehicleStatus.CurrentPath.Type == LinkType.Straight;
                        //following_error &= curVehicleStatus.CurrentPath.BcrScanUse == false; // 마지막 Path에서만 사용해도 될것 같다.
                        following_error &= curVehicleStatus.CurrentPath.CurrentPositionOfLink > 10.0f;
                        following_error &= curVehicleStatus.CurrentPath.CurrentPositionOfLink < 0.2f * curVehicleStatus.CurrentPath.RunDistance;
                        following_error &= curVehicleStatus.CurrentPath.RunDistance > 10000.0f;
                        following_error &= GV.WheelBusy;
                        following_error &= false;

                        rv1 = m_devTransfer.SequenceMove();
                        if (rv1 == 0)
                        {
                            if (curCommand.CommandStatus == ProcessStatus.Aborting || curCommand.CommandStatus == ProcessStatus.Canceling)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("SequenceMove Cancel {0}, last_step[{1}], TargetNode[{2}], ToNodeID[{3}]", curCommand.CommandStatus, last_step, curCommand.TargetNode, curVehicleStatus.CurrentPath.ToNodeID));
                                rv = 0;
                                seqNo = 0;
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("SequenceMove Complete, last_step[{0}], TargetNode[{1}], ToNodeID[{2}]", last_step, curCommand.TargetNode, curVehicleStatus.CurrentPath.ToNodeID));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 310;
                            }
                        }
                        else if (pass_permit)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Pass Permit ! Current Path = {0}, Current Velocity = {1}, Remain Distance = {2}", curVehicleStatus.CurrentPath.ToString(), curVelocity, curCommand.RemainBcrDistance));
                            seqNo = 200;
                        }
                        else if (following_error)
                        {
                            double bcr_cur_position = curVehicleStatus.CurrentBcrStatus.VirtualRunBcr;
                            double motor_cur_position = DevicesManager.Instance.DevTransfer.MotionRunPosition;
                            SequenceLog.WriteLog(FuncName, string.Format("Following Error ! BCR Position={0}, Motor Position=(1)", bcr_cur_position, motor_cur_position));
                            seqNo = 200;
                        }
                        else if (rv1 > 0)
                        {
                            bool recovery_enable_case = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == CarrierState.Installed;
                            recovery_enable_case &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type == LinkType.Straight;
                            bool recovery_alarm = rv1 == m_devTransfer.ALM_SequenceMoveInRangeError.ID;
                            recovery_alarm |= rv1 == m_devTransfer.AxisMaster.GetDevAxis().ALM_Timeover.ID;
                            recovery_enable_case &= recovery_alarm;
                            if (recovery_enable_case)
                            {
                                GV.VehicleMoveRecovery = true;
                                SequenceLog.WriteLog(FuncName, string.Format("Motion SequenceMove Alarm [Recovery Try] - {0}", rv1));
                                rv = rv1;
                                ReturnSeqNo = 0;
                                seqNo = 1000;
                            }
                            else
                            {
                                GV.VehicleMoveRecovery = false;
                                SequenceLog.WriteLog(FuncName, string.Format("Motion SequenceMove Alarm - {0}", rv1));
                                AlarmId = rv1;
                                EqpAlarm.Set(AlarmId);
                                m_devTransfer.AxisMaster.GetDevAxis().SeqAbort();
                                ReturnSeqNo = 200; //Data 확인 후 다시 여기로...
                                seqNo = 1000;
                            }
                        }

                        //if (AppConfig.Instance.Simulation.MY_DEBUG)
                        //    Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} : " +
                        //        $"JCS ({TaskJCS.Instance.JcsControl.PassRequest}, " +
                        //        $"{TaskJCS.Instance.JcsControl.ReceivedPermit}), " +
                        //        $"({m_devTransfer.AxisMaster.GetDevAxis().GetAxis().SequenceState.IsExternalEncoderRun}, " +
                        //        $"{Math.Round(m_devTransfer.AxisMaster.GetDevAxis().GetAxis().CurSpeed, 1)}), " +
                        //        $"({Math.Round(curCommand.RemainBcrDistance, 1)}, " +
                        //        $"{Math.Round(curCommand.RemainMotorDistance, 1)}), " +
                        //        $"{curVehicleStatus.CurrentPath}");
                    }
                    break;

                case 310:
                    {
                        double bcrLeft = curVehicleStatus.CurrentBcrStatus.LeftBcr;
                        double bcrRight = curVehicleStatus.CurrentBcrStatus.RightBcr;
                        DataItem_Node curNode = curVehicleStatus.CurrentPath.IsNearNode(bcrLeft, bcrRight); // 무조건 ToNode를 선택할 수 없다.
                        bool bcr_scan_use = curCommand.RunPathMaps.Count > 0 ? curCommand.RunPathMaps.Last().BcrScanUse : false;
                        if (bcr_scan_use)
                        {
                            // PathMaps의 마지막 도착 위치이다. (EndNode 도착할때만 BCR Scan Use)
                            double curBCR = 0.0f, targetBCR = 0.0f;
                            bool inPosition = IsCommandTargetPosition(curCommand, curVehicleStatus.CurrentBcrStatus, ref curBCR, ref targetBCR);
                            inPosition |= curVehicleStatus.IsInPosition;

                            if (inPosition)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("IsInPosition BCR = true OK"));
                                curCommand.UpdateMotionProcess(curVehicleStatus.CurrentPath, false);
                                // BCR 정위치가 않되었다고 하네... 200으로 가서 path 살펴보고 재지령 내리자 !
                                // 도착노드가 MTL/SPL 인가 ?
                                if (curNode.Type == NodeType.MTL)
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("MTL Completed"));
                                    m_devEqPio.IfFlagMtl.Complete = true;
                                    seqNo = 900; // Target Position Arrived
                                }
                                else if (curNode.Type == NodeType.Lifter)
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("SPL Completed"));
                                    m_devEqPio.IfFlagSpl.Complete = true;
                                    seqNo = 200; // Target Position Arrived
                                }
                                else 
                                    seqNo = 900; // Target Position Arrived
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10 * 1000)
                            {
                                // Pass를 새로 받아서 다시 이동해 보자 ~~ 더이상 진행할 path가 없다고 하는데 목표 위치가 아니다...
                                SequenceLog.WriteLog(FuncName, string.Format("Not BCR IsInPosition ! Retry Schedule !"));
                                seqNo = 900;
                            }
                        }
                        else
                        {
                            // Abort 상황이기 때문에 새로 Schedule 하지 말고 기다린다.
                            //bool permit_check_need = true;
                            //permit_check_need &= curCommand.CommandStatus != ProcessStatus.Canceling;
                            //permit_check_need &= curCommand.CommandStatus != ProcessStatus.Aborting;

                            if (curVehicleStatus.IsInPosition)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("IsInPosition Encoder = true OK"));
                                curCommand.UpdateMotionProcess(curVehicleStatus.CurrentPath, false);
                            }
                            bool lastPath = curVehicleStatus.CurrentPath.ToLinkID == 0;
                            if (lastPath)
                            {
                                seqNo = 200; // Target Position Arrived
                            }
                            else //if (permit_check_need)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("(JCS&AutoDoor) Permit Check Need"));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 320;
                            }
                        }
                    }
                    break;

                case 320:
                    {
                        if (curCommand.CommandStatus == ProcessStatus.Aborting || curCommand.CommandStatus == ProcessStatus.Canceling)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("SequenceMove Cancel {0}, TargetNode[{1}], ToNodeID[{2}]", curCommand.CommandStatus, curCommand.TargetNode, curVehicleStatus.CurrentPath.ToNodeID));
                            rv = 0;
                            seqNo = 0;
                        }
                        else
                        {
                            double bcrLeft = curVehicleStatus.CurrentBcrStatus.LeftBcr;
                            double bcrRight = curVehicleStatus.CurrentBcrStatus.RightBcr;
                            DataItem_Node curNode = curVehicleStatus.CurrentPath.IsNearNode(bcrLeft, bcrRight); // 무조건 ToNode를 선택할 수 없다.
                                                                                                                // 첫번째인지 ? 마지막 ? Path 인지 구분
                            bool firstPath = curVehicleStatus.CurrentPath.FromLinkID == 0;
                            bool lastPath = curVehicleStatus.CurrentPath.ToLinkID == 0;
                            bool toNodeisJCS = curNode.JCSCheck > 0;
                            //toNodeisJCS &= TaskJCS.Instance.JcsControl.PassPermitCheck;
                            bool toNodeisAutoDoor = curNode.Type == NodeType.AutoDoorIn1 || curNode.Type == NodeType.AutoDoorIn2;
                            toNodeisAutoDoor &= TaskInterface.Instance.AutoDoorControl.IsAutoDoorOutExist(curNode.NodeID, 5);
                            //toNodeisAutoDoor &= TaskInterface.Instance.AutoDoorControl.PassPermitCheck;

                            if (lastPath)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("No More Next Process Path"));
                                seqNo = 200;
                            }
                            else
                            {
                                bool jcs_permit = true;
                                if (toNodeisJCS)
                                {
                                    jcs_permit &= TaskJCS.Instance.JcsControl.ReceivedPermit;
                                }
                                bool autoDoor_permit = true;
                                if (toNodeisAutoDoor && TaskInterface.Instance.AutoDoorControl.AutoDoorRunning)
                                {
                                    autoDoor_permit &= TaskInterface.Instance.AutoDoorControl.ReceivedPermit;
                                }
                                if (jcs_permit && autoDoor_permit)
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("(JCS&AutoDoor) Permit Check OK"));
                                    seqNo = 200;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > 30 * 1000)
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("(JCS&AutoDoor) Permit Wait."));
                                    StartTicks = XFunc.GetTickCount();
                                }
                                if (AppConfig.Instance.Simulation.MY_DEBUG)
                                {
                                    string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                                    System.Diagnostics.Debug.WriteLine($"{time}, SeqVehicle Move-{seqNo},      {TaskJCS.Instance.JcsControl.ReceivedPermit},{TaskInterface.Instance.AutoDoorControl.ReceivedPermit},{toNodeisJCS}, {toNodeisAutoDoor}, NodeID, {curNode.NodeID}, NodeType, {curNode.Type}");
                                }
                            }
                        }
                    }
                    break;

                case 900:
                    {
                        SequenceLog.WriteLog(FuncName, "Vehicle Move Complete");
                        m_VehicleMoving = false;

                        if (curCommand.ProcessCommand == OCSCommand.Go)
                        {
                            if (curCommand.DestinationID == SetupManager.Instance.SetupOCS.VehicleRemoveMTLNodeId)
                            {
                                curVehicleStatus.SetVehicleStatus(VehicleState.Removed);
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.Removed);
                            }
                            else if (curCommand.CommandStatus != ProcessStatus.Aborting && curCommand.CommandStatus != ProcessStatus.Canceling)
                            {
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.GoCompleted);
                            }
                        }
                        else if (curCommand.ProcessCommand == OCSCommand.Transfer)
                        {
                            if (curVehicleStatus.CarrierStatus == CarrierState.Installed)
                            {
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.ArrivedAtToPosition);
                                curCommand.UpdateTransferTime(transferUpdateTime.MoveToDestination);
                            }
                            else
                            {
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.ArrivedAtFromPosition);
                                curCommand.UpdateTransferTime(transferUpdateTime.MoveToSource);
                            }
                        }
                        this.scm.Start = false;
                        this.scm.End = true;
                        rv = 0;
                        seqNo = 0;
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

            if (AppConfig.Instance.Simulation.MY_DEBUG && false)
            {
                double watch = (double)stopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;
                string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                System.Diagnostics.Debug.WriteLine($"{time} : [{this.SeqName}-{this.SeqNo}] [{watch}]");
            }

            this.SeqNo = seqNo;
            return rv;
        }
        #endregion
        #region Device Motor ReadyCheck
        private int CheckDeviceMotor()
        {
            AlarmId = 0;
            if (m_devTransfer.IsValid)
            {
                if (m_devTransfer.AxisMaster.IsValid) if (AlarmId == 0) AlarmId = m_devTransfer.AxisMaster.GetDevAxis().IsAxisReady() == false ? m_devTransfer.AxisMaster.GetDevAxis().ALM_NotReadyError.ID : 0;
                if (m_devTransfer.AxisSlave.IsValid) if (AlarmId == 0) AlarmId = m_devTransfer.AxisSlave.GetDevAxis().IsAxisReady() == false ? m_devTransfer.AxisSlave.GetDevAxis().ALM_NotReadyError.ID : 0;
            }
            if (m_devFoupGripper.IsValid)
            {
                if (m_devFoupGripper.AxisSlide.IsValid) if (AlarmId == 0) AlarmId = m_devFoupGripper.AxisSlide.GetDevAxis().IsAxisReady() == false ? m_devFoupGripper.AxisSlide.GetDevAxis().ALM_NotReadyError.ID : 0;
                if (m_devFoupGripper.AxisHoist.IsValid) if (AlarmId == 0) AlarmId = m_devFoupGripper.AxisHoist.GetDevAxis().IsAxisReady() == false ? m_devFoupGripper.AxisHoist.GetDevAxis().ALM_NotReadyError.ID : 0;
                if (m_devFoupGripper.AxisTurn.IsValid) if (AlarmId == 0) AlarmId = m_devFoupGripper.AxisTurn.GetDevAxis().IsAxisReady() == false ? m_devFoupGripper.AxisTurn.GetDevAxis().ALM_NotReadyError.ID : 0;
            }
            if (m_devGripperPio.IsValid)
            {
                if (AlarmId == 0) AlarmId = m_devGripperPio.PioComm.IsGo() == false ? m_devGripperPio.PioComm.ALM_PioGoResponseError.ID : 0;
            }
            return AlarmId;
        }
        #endregion
        #region Methods - sensor, position function
        private bool IsFoupExist()
        {
            bool foup_exist = true;
            try
            {
                if (m_devGripperPio.IsValid)
                {
                    foup_exist &= m_devGripperPio.IsProductExist(); // sensor ng
                    foup_exist &= m_devGripperPio.DiGripperClose.IsDetected;
                    foup_exist &= !m_devGripperPio.IsHoistLimit();
                    foup_exist &= m_devGripperPio.IsHoistHome();
                }
                if (AppConfig.Instance.Simulation.MY_DEBUG)
                {
                    foup_exist = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }

            return foup_exist;
        }
        private void AntiDropLock()
        {
            //if (SetupManager.Instance.SetupOperation.SyncActionOfWheelAndAntiDrop && IsFoupExist())
            if (IsFoupExist())
            {
                if (m_devAntiDropFront.IsValid && !m_devAntiDropFront.IsCommandBusy()) m_devAntiDropFront.SetCommandLock();
                if (m_devAntiDropRear.IsValid && !m_devAntiDropRear.IsCommandBusy()) m_devAntiDropRear.SetCommandLock();
            }
        }
        private void AntiDropRelease()
        {
            bool antidrop_Realease_Enable = SetupManager.Instance.SetupOperation.Early_Motion_Use == Use.Use;
            antidrop_Realease_Enable &= ProcessDataHandler.Instance.CurTransferCommand.ProcessCommand == OCSCommand.Transfer;

            if (antidrop_Realease_Enable)
            {
                bool antiDrop_Release = true;//ProcessDataHandler.Instance.CurVehicleStatus.IsNearPosition;
                antiDrop_Release &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.FromNodeID == ProcessDataHandler.Instance.CurTransferCommand.EndNode;
                antiDrop_Release &= m_devTransfer.AxisMaster.GetDevAxis().GetAxis().SequenceState.IsExternalEncoderRun; //이러면 정위치 정지시에만 진행하지않을까?
                antiDrop_Release &= ProcessDataHandler.Instance.CurTransferCommand.RemainBcrDistance < SetupManager.Instance.SetupOperation.Early_Motion_Range || ProcessDataHandler.Instance.CurTransferCommand.RemainMotorDistance < SetupManager.Instance.SetupOperation.Early_Motion_Range;

                if (antiDrop_Release && !m_devAntiDropFront.GetUnlock() && !m_devAntiDropFront.IsCommandBusy())
                {
                    SequenceLog.WriteLog(FuncName, string.Format("Front Anti Drop Release !"));
                    if (m_devAntiDropFront.IsValid) m_devAntiDropFront.SetCommandUnlock();
                }
                if (antiDrop_Release && !m_devAntiDropRear.GetUnlock() && !m_devAntiDropRear.IsCommandBusy())
                {
                    SequenceLog.WriteLog(FuncName, string.Format("Rear Anti Drop Release !"));
                    if (m_devAntiDropRear.IsValid) m_devAntiDropRear.SetCommandUnlock();
                }
            }
        }
        public bool m_motion_complete = false;
        public int m_motion_return_value = -1;
        private void EarlyMotion()
        {
            try
            {
                bool Motion_Enable = SetupManager.Instance.SetupOperation.Early_Motion_Use == Use.Use;
                Motion_Enable &= ProcessDataHandler.Instance.CurTransferCommand.ProcessCommand == OCSCommand.Transfer;
                Motion_Enable &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.FromNodeID == ProcessDataHandler.Instance.CurTransferCommand.EndNode;
                Motion_Enable &= m_devTransfer.AxisMaster.GetDevAxis().GetAxis().SequenceState.IsExternalEncoderRun; //이러면 정위치 정지시에만 진행하지않을까?
                Motion_Enable &= ProcessDataHandler.Instance.CurTransferCommand.RemainBcrDistance < SetupManager.Instance.SetupOperation.Early_Motion_Range || ProcessDataHandler.Instance.CurTransferCommand.RemainMotorDistance < SetupManager.Instance.SetupOperation.Early_Motion_Range;

                if (Motion_Enable)
                {
                    if (ProcessDataHandler.Instance.CurVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToSource || ProcessDataHandler.Instance.CurVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToDest)
                    {
                        IEnumerable<DataItem_Port> tempPort = ProcessDataHandler.Instance.CurVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToSource ? DatabaseHandler.Instance.DictionaryPortDataList.Values.Where(n => n.PortID == ProcessDataHandler.Instance.CurTransferCommand.SourceID).ToList() :
                            ProcessDataHandler.Instance.CurVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToDest ? DatabaseHandler.Instance.DictionaryPortDataList.Values.Where(n => n.PortID == ProcessDataHandler.Instance.CurTransferCommand.DestinationID).ToList() : null;

                        bool carrier_installed = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
                        DataItem_Port portInfo = tempPort.First();

                        if (tempPort.Count() > 0)
                        {
                            bool isOHB = portInfo.PortType == PortType.LeftBuffer || portInfo.PortType == PortType.RightBuffer;

                            XyztPosition m_TargetPosition = new XyztPosition();
                            m_TargetPosition = m_devFoupGripper.GetDownPos(portInfo.PortID, carrier_installed, true);
                            ushort down_prop = m_devFoupGripper.TeachingVelocityBufDown.PropId;
                            List<VelSet> m_Target1VelSets = new List<VelSet>(); // foup not exist move
                            m_Target1VelSets = m_devFoupGripper.GetTeachingVelSets(down_prop);

                            if (m_TargetPosition.T > 5.0f || m_TargetPosition.Y > 10.0f) return;

                            bool position_OK = Math.Abs(m_TargetPosition.T - m_devFoupGripper.AxisTurn.GetDevAxis().GetCurPosition()) < 1f;
                            if (!isOHB)
                                position_OK &= Math.Abs(m_TargetPosition.Y - m_devFoupGripper.AxisSlide.GetDevAxis().GetCurPosition()) < 1f;

                            if (!m_motion_complete && !position_OK)
                            {
                                if (isOHB)
                                    m_motion_return_value = m_devFoupGripper.Move(enAxisMask.aT, m_TargetPosition, m_Target1VelSets);
                                else
                                    m_motion_return_value = m_devFoupGripper.Move(enAxisMask.aYT, m_TargetPosition, m_Target1VelSets);

                                if (m_motion_return_value == 0) m_motion_complete = true;
                            }

                            if (m_motion_complete)
                            {
                                m_motion_complete = false;
                                if (isOHB)
                                    SequenceLog.WriteLog($"Ealry Motion Complete! Port ID = {portInfo.PortID}, isOHB = {isOHB}, Rotate Taget Pos = {m_TargetPosition.T}, Cur Pos = {m_devFoupGripper.AxisTurn.GetDevAxis().GetCurPosition()}");
                                else
                                    SequenceLog.WriteLog($"Ealry Motion Complete! Port ID = {portInfo.PortID}, isOHB = {isOHB}, Rotate Taget Pos = {m_TargetPosition.T}, Cur Pos = {m_devFoupGripper.AxisTurn.GetDevAxis().GetCurPosition()}, " +
                                        $"Slide Taget Pos = {m_TargetPosition.Y}, Cur Pos = {m_devFoupGripper.AxisSlide.GetDevAxis().GetCurPosition()}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        public int PassRequestStartNodeCheck(Sineva.VHL.Data.Process.Path path)
        {
            int startNode = path.ToNodeID;
            try
            {
                // StartNode는 CurrentLink의 ToNode를 default로 설정한다.
                // 만일 BCR이 Link의 SteerGuideLegthToNode 값이 내에 있을 경우는 Steer 방향에 따라 Start 위치가 달라질수 있다.
                bool steerGuideRange = path.RemainDistanceOfLink < path.SteerGuideLengthToNode;
                bool nextBranchType = DatabaseHandler.Instance.DictionaryLinkDataList.Values.Where(x => x.Type == LinkType.LeftBranch ||
                                                                                    x.Type == LinkType.RightBranch ||
                                                                                    x.Type == LinkType.LeftSBranch ||
                                                                                    x.Type == LinkType.RightSBranch ||
                                                                                    x.Type == LinkType.LeftCompositedSCurveBranch ||
                                                                                    x.Type == LinkType.RightCompositedSCurveBranch)
                                                                .Where(y => y.FromNodeID == startNode).ToList().Count > 0;

                if (steerGuideRange && nextBranchType)
                {
                    // Branch Node를 찾아야 함.
                    // Link Branch Type인 current Node에서 출발할때는 Steer 방향에 따라 EndNode를 startnode로 잡아야 한다.
                    // 커브구간이라면 무조건 Link는 2개가 나올거다...
                    List<DataItem_Link> similarLinks = DatabaseHandler.Instance.DictionaryLinkDataList.Values.Where(y => y.FromNodeID == startNode).ToList();
                    if (similarLinks.Count > 1)
                    {
                        enSteerDirection front_steer = m_devSteer.GetFrontSteerDirection();
                        enSteerDirection rear_steer = m_devSteer.GetRearSteerDirection();
                        bool left = front_steer == enSteerDirection.Left && rear_steer == enSteerDirection.Left;
                        bool right = front_steer == enSteerDirection.Right && rear_steer == enSteerDirection.Right;

                        List<DataItem_Link> left_similarLinks = new List<DataItem_Link>();
                        left_similarLinks.AddRange(similarLinks.Where(item => item.Type == LinkType.LeftBranch ||
                                                                        item.Type == LinkType.LeftSBranch ||
                                                                        item.Type == LinkType.LeftCompositedSCurveBranch).ToList());
                        bool left_type = left_similarLinks.Count > 0 ? true : false;
                        List<DataItem_Link> right_similarLinks = new List<DataItem_Link>();
                        right_similarLinks.AddRange(similarLinks.Where(item => item.Type == LinkType.RightBranch ||
                                                                        item.Type == LinkType.RightSBranch ||
                                                                        item.Type == LinkType.RightCompositedSCurveBranch).ToList());
                        bool right_type = right_similarLinks.Count > 0 ? true : false;
                        foreach (DataItem_Link link in similarLinks)
                        {
                            bool link_left_type = link.Type == LinkType.LeftBranch ? true : false;
                            link_left_type |= link.Type == LinkType.LeftSBranch ? true : false;
                            link_left_type |= link.Type == LinkType.LeftCompositedSCurveBranch ? true : false;
                            bool link_right_type = link.Type == LinkType.RightBranch ? true : false;
                            link_right_type |= link.Type == LinkType.RightSBranch ? true : false;
                            link_right_type |= link.Type == LinkType.RightCompositedSCurveBranch ? true : false;

                            bool set_codition = left && left_type && link_left_type;
                            set_codition |= left && right_type && !link_right_type;
                            set_codition |= right && right_type && link_right_type;
                            set_codition |= right && left_type && !link_left_type;

                            if (set_codition)
                            {
                                startNode = link.ToNodeID;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return startNode;
        }

        // Command의 목표 위치를 확인하자
        private bool IsCommandTargetPosition(TransferCommand command, BcrStatus bcr, ref double curBCR, ref double targetBCR)
        {
            bool inPosition = false; // 한바퀴 돌거니 ?
            try
            {
                double oppsiteCurBCR = 0.0f;
                double oppsiteTargetBCR = 0.0f;
                if (command.TargetBcrCheckDirection == enBcrCheckDirection.Both)
                {
                    double left_diff = command.TargetLeftBcrPosition - bcr.LeftBcr;
                    double right_diff = command.TargetRightBcrPosition - bcr.RightBcr;
                    if (Math.Abs(left_diff) < Math.Abs(right_diff))
                    {
                        curBCR = bcr.LeftBcr;
                        targetBCR = command.TargetLeftBcrPosition;
                        oppsiteCurBCR = bcr.RightBcr;
                        oppsiteTargetBCR = command.TargetRightBcrPosition;
                    }
                    else
                    {
                        curBCR = bcr.RightBcr;
                        targetBCR = command.TargetRightBcrPosition;
                        oppsiteCurBCR = bcr.LeftBcr;
                        oppsiteTargetBCR = command.TargetLeftBcrPosition;
                    }
                }
                else
                {
                    bool right_bcr_use = command.TargetBcrCheckDirection == enBcrCheckDirection.Right;
                    curBCR = right_bcr_use ? bcr.RightBcr : bcr.LeftBcr;
                    targetBCR = right_bcr_use ? command.TargetRightBcrPosition : command.TargetLeftBcrPosition;
                    oppsiteCurBCR = right_bcr_use ? bcr.LeftBcr : bcr.RightBcr;
                    oppsiteTargetBCR = right_bcr_use ? command.TargetLeftBcrPosition : command.TargetRightBcrPosition;
                }
                inPosition = Math.Abs(targetBCR - curBCR) < 2.0f;
                inPosition &= Math.Abs(oppsiteTargetBCR - oppsiteCurBCR) < 10.0f;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return inPosition;
        }

        #endregion
    }

    public class SeqTransfer : XSeqFunc
    {
        public static readonly string FuncName = "[SeqTransfer]";
        XTimer m_Timer = new XTimer("SeqTransfer");

        #region Fields
        private DevGripperPIO m_devGripperPio = null;

        private SeqVehicleMove m_SeqVehicleMove = new SeqVehicleMove();
        private SeqAcquire m_SeqAcquire = new SeqAcquire();
        private SeqDeposit m_SeqDeposit = new SeqDeposit();
        private SeqCarrierIdReading m_SeqCarrierIdMapping = new SeqCarrierIdReading();
        private List<XSeqFunc> m_SubSeqLists = new List<XSeqFunc>();

        private int m_SeqVehicleMoveRetry = 0;
        #endregion

        #region Properties
        public List<XSeqFunc> SubSeqLists { get { return m_SubSeqLists; } }
        #endregion

        #region Constructor
        public SeqTransfer()
        {
            this.SeqName = $"SeqTransfer";

            SubSeqLists.Add(m_SeqAcquire);
            SubSeqLists.Add(m_SeqDeposit);
            SubSeqLists.Add(m_SeqVehicleMove);
            SubSeqLists.Add(m_SeqCarrierIdMapping);

            m_devGripperPio = DevicesManager.Instance.DevGripperPIO;
            AddCaseMemo();
            this.TaskLayer = (int)enTaskLayer.layerVehicleMove;
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
                if (m_devGripperPio.IsValid) m_devGripperPio.SeqAbort();
                this.scm.Abort = true;
            }
            m_SeqVehicleMove.SeqAbort();
            m_SeqAcquire.SeqAbort();
            m_SeqDeposit.SeqAbort();
            m_SeqCarrierIdMapping.SeqAbort();
            this.InitSeq();
        }
        private void AddCaseMemo()
        {
            try
            {
                this.SeqCaseMemoLists.Add(0, string.Format("Transfer Sequence Start !"));
                this.SeqCaseMemoLists.Add(10, string.Format("Command Select !"));
                this.SeqCaseMemoLists.Add(100, string.Format("SeqVehicle Move Run"));
                this.SeqCaseMemoLists.Add(200, string.Format("SeqAcquire Run"));
                this.SeqCaseMemoLists.Add(300, string.Format("SeqDeposit Run"));
                this.SeqCaseMemoLists.Add(900, string.Format("Transfer Sequence Complete"));
                this.SeqCaseMemoLists.Add(1000, string.Format("Alarm Reset Wait"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("{0} EXCEPTION : {1}", FuncName, ex.ToString()));
            }
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

            TransferCommand curCommand = ProcessDataHandler.Instance.CurTransferCommand;
            VehicleStatus curVehicleStatus = ProcessDataHandler.Instance.CurVehicleStatus;
            OCSStatus ocsStatus = OCSCommManager.Instance.OcsStatus;

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        this.scm.Start = true;
                        StartTicks = XFunc.GetTickCount();

                        seqNo = 10;
                    }
                    break;

                case 10:
                    {
                        // 만일 Foup이 감지되어 있는 경우
                        if (curCommand.OnlyTransferMove == false)
                        {
                            curVehicleStatus.SetCarrierStatus(IsFoupExist() ? IF.OCS.CarrierState.Installed : IF.OCS.CarrierState.None);
                        }

                        bool abnormal_finished_status = curVehicleStatus.GetVehicleStatus() == VehicleState.SourceEmpty;
                        abnormal_finished_status |= curVehicleStatus.GetVehicleStatus() == VehicleState.AcquireFailed;
                        abnormal_finished_status |= curVehicleStatus.GetVehicleStatus() == VehicleState.DestinationDouble;
                        abnormal_finished_status |= curVehicleStatus.GetVehicleStatus() == VehicleState.DepositFailed;
                        // Transfer Command Run
                        // Source이동/Destination이동 판단 필요.
                        // 
                        bool carrier_installed = curVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
                        int source_port = curCommand.SourceID;
                        int target_port = curCommand.DestinationID;
                        int current_port = curVehicleStatus.CurrentPort.PortID;
                        // 정위치 확인 후 target position이면 완료

                        //double curBCR = 0.0f, targetBCR = 0.0f;
                        //bool inRange = IsCommandTargetPosition(curCommand, curVehicleStatus.CurrentBcrStatus, ref curBCR, ref targetBCR);

                        if ((source_port == 0 && target_port == 0) || (target_port==0 && carrier_installed))
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Abnormal Finished ! SourceID={0}, DestinationID={1}", source_port, target_port));
                            seqNo = 900;
                        }
                        else
                        {
                            bool source_inrange = IsPortPosition(curVehicleStatus.CurrentBcrStatus, source_port);
                            source_inrange &= curVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToSource;
                            bool destination_inrange = IsPortPosition(curVehicleStatus.CurrentBcrStatus, target_port);
                            destination_inrange &= curVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToDest;

                            if (curVehicleStatus.GetVehicleStatus() == VehicleState.DepositeCompleted)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Normal Finished ! SourceID={0}, DestinationID={1}", source_port, target_port));
                                seqNo = 900;
                            }
                            else if (abnormal_finished_status)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Abnormal Finished ! SourceID={0}, DestinationID={1}", source_port, target_port));
                                seqNo = 900;
                            }
                            else if (/*source_port == current_port && */!carrier_installed && source_inrange)
                            {
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepartToFromPosition);
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.ArrivedAtFromPosition);
                                curVehicleStatus.SetVehicleStatus(VehicleState.Acquiring);
                                // Acquire 동작을 하자 !
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Not Exist ! SourceID equal Current Port ! SourceID={0}, DestinationID={1}", source_port, target_port));
                                if (curCommand.OnlyTransferMove)
                                {
                                    OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.AcquireStart);
                                    curVehicleStatus.SetCarrierStatus(CarrierState.Installed);
                                    OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.AcquireCompleted);
                                }
                                else
                                {
                                    seqNo = 200;
                                }
                            }
                            else if (/*target_port == current_port && */carrier_installed && destination_inrange)
                            {
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepartToToPosition);
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.ArrivedAtToPosition);
                                curVehicleStatus.SetVehicleStatus(VehicleState.Depositing);
                                // Deport 동작을 하자 !
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Exist ! DestinationID equal Current Port ! SourceID={0}, DestinationID={1}", source_port, target_port));
                                if (curCommand.OnlyTransferMove)
                                {
                                    OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepositStart);
                                    curVehicleStatus.SetCarrierStatus(CarrierState.None);
                                    OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepositCompleted);
                                }
                                else
                                {
                                    seqNo = 300;
                                }
                            }
                            else
                            {
                                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                if (curVehicleStatus.CarrierStatus == CarrierState.Installed)
                                {
                                    curVehicleStatus.SetVehicleStatus(VehicleState.EnRouteToDest);
                                    OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepartToToPosition);
                                    curCommand.UpdateTransferTime(transferUpdateTime.MoveToDestination);
                                }
                                else
                                {
                                    curVehicleStatus.SetVehicleStatus(VehicleState.EnRouteToSource);
                                    OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepartToFromPosition);
                                    curCommand.UpdateTransferTime(transferUpdateTime.MoveToSource);
                                }
                                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                curCommand.SetProcessStatus(ProcessStatus.Processing);
                                SequenceLog.WriteLog(FuncName, string.Format("{0} Move Start", curVehicleStatus.CarrierStatus == CarrierState.Installed ? "Destination" : "Source"));
                                m_SeqVehicleMoveRetry = 0;
                                GV.VehicleMoveRecovery = false;
                                seqNo = 100;
                            }
                        }
                    }
                    break;

                case 100:
                    {
                        // SeqVehicleMove
                        rv1 = m_SeqVehicleMove.Do();
                        if (rv1 == 0)
                        {
                            if (curCommand.CommandStatus == ProcessStatus.Aborting || curCommand.CommandStatus == ProcessStatus.Canceling)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Move {0}", curCommand.CommandStatus));
                                rv = 0;
                                seqNo = 0;
                            }
                            else
                            {
                                int source_port = curCommand.SourceID;
                                int target_port = curCommand.DestinationID;
                                int current_port = curVehicleStatus.CurrentPort.PortID;
                                bool source_inrange = IsPortPosition(curVehicleStatus.CurrentBcrStatus, source_port);
                                source_inrange &= curVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToSource;
                                bool destination_inrange = IsPortPosition(curVehicleStatus.CurrentBcrStatus, target_port);
                                destination_inrange &= curVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToDest;

                                SequenceLog.WriteLog(FuncName, string.Format("Move OK [{0}, {1}]", curVehicleStatus.CarrierStatus == CarrierState.Installed ? curCommand.DestinationID : curCommand.SourceID, curVehicleStatus.CarrierStatus));

                                if (curVehicleStatus.CarrierStatus != CarrierState.Installed && source_inrange)
                                {
                                    OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepartToFromPosition);
                                    OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.ArrivedAtFromPosition);
                                    curVehicleStatus.SetVehicleStatus(VehicleState.Acquiring);
                                    // Acquire 동작을 하자 !
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Not Exist ! SourceID equal Current Port({0}) ! SourceID={1}, DestinationID={2}", current_port, source_port, target_port));
                                    seqNo = 200;
                                }
                                else if (curVehicleStatus.CarrierStatus == CarrierState.Installed && destination_inrange)
                                {
                                    OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepartToToPosition);
                                    OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.ArrivedAtToPosition);
                                    curVehicleStatus.SetVehicleStatus(VehicleState.Depositing);
                                    // Deport 동작을 하자 !
                                    SequenceLog.WriteLog(FuncName, string.Format("Foup Exist ! DestinationID equal Current Port({0}) ! SourceID={1}, DestinationID={2}", current_port, source_port, target_port));
                                    seqNo = 300;
                                }
                                else
                                {
                                    curVehicleStatus.SetVehicleStatus(VehicleState.Parked);
                                    // Move Complete !
                                    SequenceLog.WriteLog(FuncName, string.Format("Current Port({0}) ! SourceID={1}, DestinationID={2}", current_port, source_port, target_port));
                                    seqNo = 10;
                                }
                            }
                        }
                        else if (GV.VehicleMoveRecovery)
                        {
                            GV.VehicleMoveRecovery = false;
                            if (m_SeqVehicleMoveRetry < 2 && curVehicleStatus.CarrierStatus == CarrierState.Installed) // Destination 이동 상황일때만
                            {
                                m_SeqVehicleMoveRetry++;
                                SequenceLog.WriteLog(FuncName, string.Format("VehicleMove Recovery Start, VehicleMove RecoveryCount={0}, CarrierStatus={1}", m_SeqVehicleMoveRetry, curVehicleStatus.CarrierStatus));
                                m_SeqVehicleMove.SeqAbort();
                            }
                            else if (rv1 > 0)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Move Alarm [{0}, {1}]", curVehicleStatus.CarrierStatus == CarrierState.Installed ? curCommand.DestinationID : curCommand.SourceID, curVehicleStatus.CarrierStatus));
                                AlarmId = rv1;
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = seqNo; //Data 확인 후 다시 여기로...
                                seqNo = 1000;
                            }
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Move Alarm [{0}, {1}]", curVehicleStatus.CarrierStatus == CarrierState.Installed ? curCommand.DestinationID : curCommand.SourceID, curVehicleStatus.CarrierStatus));
                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; //Data 확인 후 다시 여기로...
                            seqNo = 1000;
                        }
                    }
                    break;

                case 200:
                    {
                        // Acquire
                        rv1 = m_SeqAcquire.Do();
                        if (rv1 == 0)
                        {
                            curVehicleStatus.SetVehicleStatus(VehicleState.AcquireCompleted);

                            // finished
                            SequenceLog.WriteLog(FuncName, string.Format("Acquire OK [{0},{1}]", curCommand.SourceID, curVehicleStatus.CarrierStatus));
                            if (SetupManager.Instance.SetupOperation.RFIdUse == Use.Use) seqNo = 400;
                            else seqNo = 10;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Acquire Alarm [{0},{1}]", curCommand.SourceID, curVehicleStatus.CarrierStatus));
                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; //
                            seqNo = 1000;
                        }
                    }
                    break;

                case 300:
                    {
                        // Deposit
                        rv1 = m_SeqDeposit.Do();
                        if (rv1 == 0)
                        {
                            curVehicleStatus.SetVehicleStatus(VehicleState.DepositeCompleted);

                            // finished
                            SequenceLog.WriteLog(FuncName, string.Format("Deposit OK [{0},{1}]", curCommand.DestinationID, curVehicleStatus.CarrierStatus));
                            seqNo = 10;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Deposit Alarm [{0},{1}]", curCommand.DestinationID, curVehicleStatus.CarrierStatus));
                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; //
                            seqNo = 1000;
                        }
                    }
                    break;

                case 400:
                    {
                        // RF ID Mapping
                        rv1 = m_SeqCarrierIdMapping.Do();
                        if (rv1 == 0)
                        {
                            // finished
                            SequenceLog.WriteLog(FuncName, string.Format("Carrier ID Mapping OK"));
                            seqNo = 10;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Carrier ID Mapping Alarm"));
                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; //
                            seqNo = 1000;
                        }
                    }
                    break;

                case 900:
                    {
                        curCommand.SetProcessStatus(ProcessStatus.Completed);
                        SequenceLog.WriteLog(FuncName, "SeqTransfer Complete");

                        this.scm.Start = false;
                        this.scm.End = true;
                        rv = 0;
                        seqNo = 0;
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
            return rv;
        }
        #endregion
        #region Methods - sensor, position function
        private bool IsFoupExist()
        {
            bool foup_exist = true;
            try
            {
                if (m_devGripperPio.IsValid)
                {
                    //foup_exist &= m_devGripperPio.IsProductExist(); //sensor ng
                    //foup_exist &= m_devGripperPio.DiGripperClose.IsDetected;
                    foup_exist &= !m_devGripperPio.IsHoistLimit();
                    foup_exist &= m_devGripperPio.IsHoistHome();
                }
                if (AppConfig.Instance.Simulation.MY_DEBUG)
                {
                    foup_exist = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return foup_exist;
        }
        // Command의 목표 위치를 확인하자
        // PathList가 없는 상태에서 BCR Direction은 판단하기 쉽지 않다.
        // Left/Right 중 한개라도 일치하면 OK하자
        private bool IsPortPosition(BcrStatus bcr, int portId)
        {
            bool inPosition = false; // 한바퀴 돌거니 ?
            if (portId != 0)
            {
                if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(portId))
                {
                    DataItem_Port source_port = DatabaseHandler.Instance.DictionaryPortDataList[portId];
                    double source_left_bcr = source_port.BarcodeLeft + source_port.DriveLeftOffset;
                    double source_right_bcr = source_port.BarcodeRight + source_port.DriveRightOffset;
                    double left_diff = Math.Abs(source_left_bcr - bcr.LeftBcr);
                    double right_diff = Math.Abs(source_right_bcr - bcr.RightBcr);
                    double range_error = Math.Min(left_diff, right_diff);
                    inPosition = range_error < 2.0f;
                }
            }
            return inPosition;
        }
        private bool IsCommandTargetPosition(TransferCommand command, BcrStatus bcr, ref double curBCR, ref double targetBCR)
        {
            bool inPosition = false; // 한바퀴 돌거니 ?
            try
            {
                if (command.TargetBcrCheckDirection == enBcrCheckDirection.Both)
                {
                    double left_diff = command.TargetLeftBcrPosition - bcr.LeftBcr;
                    double right_diff = command.TargetRightBcrPosition - bcr.RightBcr;
                    if (Math.Abs(left_diff) < Math.Abs(right_diff))
                    {
                        curBCR = bcr.LeftBcr;
                        targetBCR = command.TargetLeftBcrPosition;
                    }
                    else
                    {
                        curBCR = bcr.RightBcr;
                        targetBCR = command.TargetRightBcrPosition;
                    }
                }
                else
                {
                    bool right_bcr_use = command.TargetBcrCheckDirection == enBcrCheckDirection.Right;
                    curBCR = right_bcr_use ? bcr.RightBcr : bcr.LeftBcr;
                    targetBCR = right_bcr_use ? command.TargetRightBcrPosition : command.TargetLeftBcrPosition;
                }
                if (Math.Abs(targetBCR - curBCR) < 2.0f) inPosition = true;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return inPosition;
        }
        #endregion
    }

    public class SeqAutoTeaching : XSeqFunc
    {
        public static readonly string FuncName = "[SeqAutoTeaching]";
        XTimer m_Timer = new XTimer("SeqAutoTeaching");

        #region Fields
        private DevGripperPIO m_devGripperPio = null;

        private SeqVehicleMove m_SeqVehicleMove = new SeqVehicleMove();
        private SeqVisionCalibration m_SeqVisionCalibration = new SeqVisionCalibration();
        #endregion

        #region Constructor
        public SeqAutoTeaching()
        {
            this.SeqName = $"SeqAutoTeaching";

            m_devGripperPio = DevicesManager.Instance.DevGripperPIO;
            AddCaseMemo();
            this.TaskLayer = (int)enTaskLayer.layerVehicleMove;
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
                m_SeqVehicleMove.SeqAbort();
                m_SeqVisionCalibration.SeqAbort();
                //Device 초기화
                if (m_devGripperPio.IsValid) m_devGripperPio.SeqAbort();
                this.scm.Abort = true;
            }
            this.InitSeq();
        }
        private void AddCaseMemo()
        {
            try
            {
                this.SeqCaseMemoLists.Add(0, string.Format("AutoTeaching Sequence Start !"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("{0} EXCEPTION : {1}", FuncName, ex.ToString()));
            }
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

            TransferCommand curCommand = ProcessDataHandler.Instance.CurTransferCommand;
            VehicleStatus curVehicleStatus = ProcessDataHandler.Instance.CurVehicleStatus;
            OCSStatus ocsStatus = OCSCommManager.Instance.OcsStatus;

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        this.scm.Start = true;
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 10;
                    }
                    break;

                case 10:
                    {
                        // 만일 Foup이 감지되어 있는 경우
                        curVehicleStatus.SetCarrierStatus(IsFoupExist() ? IF.OCS.CarrierState.Installed : IF.OCS.CarrierState.None);

                        // Transfer Command Run
                        // Source이동/Destination이동 판단 필요.
                        // 
                        bool carrier_installed = curVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
                        int source_port = curCommand.SourceID;
                        int target_port = curCommand.DestinationID;
                        int current_port = curVehicleStatus.CurrentPort.PortID;

                        if ((source_port == 0 && target_port == 0) || !carrier_installed)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Abnormal Finished ! SourceID={0}, DestinationID={0}", source_port, target_port));
                            seqNo = 900;
                        }
                        else
                        {
                            if (target_port == current_port && carrier_installed)
                            {
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepartToToPosition);
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.ArrivedAtToPosition);
                                curVehicleStatus.SetVehicleStatus(VehicleState.AutoTeaching);
                                SequenceLog.WriteLog(FuncName, string.Format("Foup Exist ! DestinationID equal Current Port ! SourceID={0}, DestinationID={1}", source_port, target_port));
                                seqNo = 200;
                            }
                            else
                            {
                                curVehicleStatus.SetVehicleStatus(VehicleState.EnRouteToDest);
                                OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.DepartToToPosition);
                                curCommand.UpdateTransferTime(transferUpdateTime.MoveToDestination);

                                curCommand.SetProcessStatus(ProcessStatus.Processing);
                                SequenceLog.WriteLog(FuncName, string.Format("{0} Move Start", curVehicleStatus.CarrierStatus == CarrierState.Installed ? "Destination" : "Source"));
                                seqNo = 100;
                            }
                        }
                    }
                    break;

                case 100:
                    {
                        // SeqVehicleMove
                        rv1 = m_SeqVehicleMove.Do();
                        if (rv1 == 0)
                        {
                            if (curCommand.CommandStatus == ProcessStatus.Aborting || curCommand.CommandStatus == ProcessStatus.Canceling)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Move {0}", curCommand.CommandStatus));
                                rv = 0;
                                seqNo = 0;
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("{0} Move OK", curVehicleStatus.CarrierStatus == CarrierState.Installed ? "Destination" : "Source"));
                                seqNo = 10;
                            }
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("{0} Move Alarm", curVehicleStatus.CarrierStatus == CarrierState.Installed ? "Destination" : "Source"));
                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; //Data 확인 후 다시 여기로...
                            seqNo = 1000;
                        }
                    }
                    break;

                case 200:
                    {
                        // Vision Calibration
                        rv1 = m_SeqVisionCalibration.Do();
                        if (rv1 == 0)
                        {
                            // finished
                            curVehicleStatus.SetVehicleStatus(VehicleState.Parked);
                            SequenceLog.WriteLog(FuncName, string.Format("TeachPort-{0} AutoTeaching OK", curCommand.DestinationID));
                            seqNo = 900;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("TeachPort-{0} AutoTeaching Alarm", curCommand.DestinationID));
                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; //
                            seqNo = 1000;
                        }
                    }
                    break;

                case 900:
                    {
                        curCommand.SetProcessStatus(ProcessStatus.Completed);
                        SequenceLog.WriteLog(FuncName, "AutoTeaching Complete");

                        this.scm.Start = false;
                        this.scm.End = true;
                        rv = 0;
                        seqNo = 0;
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
            return rv;
        }
        #endregion
        #region Methods - sensor, position function
        private bool IsFoupExist()
        {
            bool foup_exist = true;
            try
            {
                if (m_devGripperPio.IsValid)
                {
                    foup_exist &= m_devGripperPio.IsProductExist();
                    foup_exist &= m_devGripperPio.DiGripperClose.IsDetected;
                    foup_exist &= !m_devGripperPio.IsHoistLimit();
                    foup_exist &= m_devGripperPio.IsHoistHome();
                }
                if (AppConfig.Instance.Simulation.MY_DEBUG)
                {
                    foup_exist = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return foup_exist;
        }
        #endregion
    }
    /// <summary>
    /// 1. Teaching Port로 Vehicle Move
    /// 2. - 위치로 500mm를 천천히 이동하면서 반사판 or double storage 센서 감지 찾기
    /// 3. + 위치로 Teaching offset distance 만큼 이동 시킴
    /// 4. 기존 offset값 reset
    /// 5. port teaching 위치로 이동 (slide, rotate, wheel)
    /// 6. Vision Find => slide, rotate, wheel 위치 계산 => save database
    /// 7. 
    /// </summary>
    public class SeqVisionCalibration : XSeqFunc
    {
        public static readonly string FuncName = "[SeqVisionCalibration]";
        XTimer m_Timer = new XTimer("SeqVisionCalibration");
        #region Fields
        private DevEqPIO m_devEqPio = null;
        private DevFoupGripper m_devFoupGripper = null;
        private DevGripperPIO m_devGripperPio = null;
        private DevAntiDrop m_devAntiDropFront = null;
        private DevAntiDrop m_devAntiDropRear = null;
        private DevOBS m_devLookDown = null;
        private DevAutoTeaching m_devAutoTeaching = null;
        private DevTransfer m_devTransfer = null;

        private double m_TransferMovingOffset = 0.0f;
        private double m_TargetPosition = 0.0f;
        private VelSet m_SearchingVelSet = new VelSet();
        private XyztPosition m_WaitPosition = new XyztPosition(); // wait
        private XyztPosition m_Target1Position = new XyztPosition(); // Teaching Position
        private List<VelSet> m_Target1VelSets = new List<VelSet>(); // Measure Position
        private List<VelSet> m_TargetSlowVelSets = new List<VelSet>(); // slow move

        private bool m_MoveComp1 = false;
        private bool m_MoveComp2 = false;
        private bool m_LastSensorFind = false;
        private double m_SetLeftBcr = 0.0f;
        private double m_SetRightBcr = 0.0f;

        private AlarmData m_ALM_AutoTeachingFailError = null;
        private enVisionDevice m_VisionFindDevice = enVisionDevice.EQPCamera;
        private bool m_AutoTeachingComplete = false;
        private bool m_SlideWaitPosition = true;
        #endregion

        #region Constructor
        public SeqVisionCalibration()
        {
            this.SeqName = $"SeqVisionCalibration";

            m_devEqPio = DevicesManager.Instance.DevEqpPIO;
            m_devFoupGripper = DevicesManager.Instance.DevFoupGripper;
            m_devGripperPio = DevicesManager.Instance.DevGripperPIO;
            m_devAntiDropFront = DevicesManager.Instance.DevFrontAntiDrop;
            m_devAntiDropRear = DevicesManager.Instance.DevRearAntiDrop;
            m_devLookDown = DevicesManager.Instance.DevOBSLookDown;
            m_devAutoTeaching = DevicesManager.Instance.DevAutoTeaching;
            m_devTransfer = DevicesManager.Instance.DevTransfer;

            AddCaseMemo();
            this.TaskLayer = (int)enTaskLayer.layerDeposit;

            m_ALM_AutoTeachingFailError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SeqVisionCalibration", "Vision Sequence", "Auto Teaching Fail Alarm");

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
                if (m_devFoupGripper.IsValid) m_devFoupGripper.SeqAbort();
                if (m_devGripperPio.IsValid) m_devGripperPio.SeqAbort();
                if (m_devAntiDropFront.IsValid) m_devAntiDropFront.SeqAbort();
                if (m_devAntiDropRear.IsValid) m_devAntiDropRear.SeqAbort();
                if (m_devLookDown.IsValid) m_devLookDown.SeqAbort();
                if (m_devAutoTeaching.IsValid) m_devAutoTeaching.SeqAbort();
                if (m_devTransfer.IsValid) m_devTransfer.SeqAbort();

                this.scm.Abort = true;
            }
            this.InitSeq();
        }
        private void AddCaseMemo()
        {
            try
            {
                this.SeqCaseMemoLists.Add(0, string.Format("Vision Calibration Sequence Start !"));

            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("{0} EXCEPTION : {1}", FuncName, ex.ToString()));
            }
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

            bool carrier_installed = ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed;
            int target_port = ProcessDataHandler.Instance.CurTransferCommand.DestinationID;
            int current_port = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortID;
            PortType curPortType = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType;
            bool isEQ_PORT = false;
            if (curPortType == PortType.LeftEQPort) isEQ_PORT = true;
            else if (curPortType == PortType.RightEQPort) isEQ_PORT = true;
            else if (curPortType == PortType.TeachingStation) isEQ_PORT = true;
            bool isLeftOHB = false;
            if (!isEQ_PORT)
            {
                if (curPortType == PortType.LeftBuffer) isLeftOHB = true;
                else if (curPortType == PortType.LeftTeachingStation) isLeftOHB = true;
            }
            bool sensor_detected = false;
            if (isEQ_PORT) sensor_detected = m_devAutoTeaching.IsReflectiveSensorOn();
            if (curPortType == PortType.LeftBuffer || curPortType == PortType.LeftTeachingStation) sensor_detected = m_devFoupGripper.IsLeftDoubleStorage();
            else if (curPortType == PortType.RightBuffer || curPortType == PortType.RightTeachingStation) sensor_detected = m_devFoupGripper.IsRightDoubleStorage();
            double searchDistance = SetupManager.Instance.SetupVision.SensorSearchDistance;

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        //EventHandlerManager.Instance.InvokeAutoTeachingMonitorShow(true);

                        this.scm.Start = true;
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 1;
                    }
                    break;

                case 1:
                    // Loader Z축 간섭 위치 확인
                    {
                        rv1 = CheckDeviceMotor();
                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName, "Auto Teaching Start");
                            seqNo = 10;
                        }
                        else if (rv1 > 0 && (XFunc.GetTickCount() - StartTicks > 5000))
                        {
                            EqpAlarm.Set(rv1);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 10:
                    {
                        bool teaching_port_null = DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(target_port) == false;

                        // SeqDeposit에서 Check 하고 StartReq 하자
                        if (ProcessDataHandler.Instance.CurTransferCommand.IsValid == false)
                        {
                            AlarmId = ProcessDataHandler.Instance.ALM_CommandNotExistError.ID;
                            SequenceLog.WriteLog(FuncName, string.Format("Port Loading Interface, Transfer Command Not Valid Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                        else if (current_port != target_port || teaching_port_null)
                        {
                            AlarmId = ProcessDataHandler.Instance.ALM_DestinationPortMismatchError.ID;
                            SequenceLog.WriteLog(FuncName, string.Format("Destination ID isn't same Current Port ID - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                        else if (IsFoupNotExist() || carrier_installed == false)
                        {
                            if (m_devGripperPio.DiLeftProductExist.IsDetected == false)
                                AlarmId = m_devGripperPio.ALM_ProductLeftNotExistAlarm.ID;
                            else AlarmId = m_devGripperPio.ALM_ProductRightNotExistAlarm.ID;
                            SequenceLog.WriteLog(FuncName, string.Format("Port Loading Interface, Foup Not Exist Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                        else if (IsDoubleStorage() == false)
                        {
                            if (curPortType == PortType.LeftBuffer) AlarmId = m_devFoupGripper.ALM_LeftDoubleStorageError.ID;
                            else AlarmId = m_devFoupGripper.ALM_RightDoubleStorageError.ID;
                            SequenceLog.WriteLog(FuncName, string.Format("Port Unloading Interface, Double Storage Check, Foup Not Exist at Destination Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                        else if (IsAntiDropUnlock() == false)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("AntiDrop Unlock NG"));
                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            seqNo = 20; // 20으로 이동하여 Unlock 실행하자!
                        }
                        else if (m_devGripperPio.IsGripperOpen())
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Gripper Open State. Close Try !"));
                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            seqNo = 30;
                        }
                        else if (m_devAutoTeaching.IsReady() == false)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Auto Teaching Unit Not Ready !"));
                            m_devAutoTeaching.AutoTeachingStart();
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 40;
                        }
                        else
                        {
                            // 기준 호기이므로 Offset 값을 Reset 하고 시작하자!
                            if (isEQ_PORT)
                            {
                                m_VisionFindDevice = enVisionDevice.EQPCamera;
                            }
                            else
                            {
                                if (curPortType == PortType.LeftBuffer) m_VisionFindDevice = enVisionDevice.OHBCamera_Left;
                                else if (curPortType == PortType.RightBuffer) m_VisionFindDevice = enVisionDevice.OHBCamera_Right;
                                else if (curPortType == PortType.LeftTeachingStation) m_VisionFindDevice = enVisionDevice.OHBCamera_Left;
                                else if (curPortType == PortType.RightTeachingStation) m_VisionFindDevice = enVisionDevice.OHBCamera_Right;
                            }
                            m_devFoupGripper.ResetTeachingOffset(current_port);
                            m_TransferMovingOffset = 0.0f;
                            m_AutoTeachingComplete = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Vision Calibration Start"));
                            seqNo = 100;
                        }
                    }
                    break;

                case 20:
                    {
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devAntiDropFront.Unlock();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (!m_MoveComp2)
                        {
                            rv2 = m_devAntiDropRear.Unlock();
                            if (rv2 == 0) m_MoveComp2 = true;
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("AntiDrop Unlock OK"));
                            seqNo = 10;
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceLog.WriteLog(FuncName, string.Format("FrontAntiDrop Unlock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else if (rv2 > 0)
                            {
                                AlarmId = rv2;
                                SequenceLog.WriteLog(FuncName, string.Format("RearAntiDrop Unlock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 30:
                    {
                        rv1 = m_devGripperPio.GripperClose();
                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Gripper Close OK"));
                            seqNo = 10;
                        }
                        else if (rv1 > 0)
                        {
                            AlarmId = rv1;
                            SequenceLog.WriteLog(FuncName, string.Format("Gripper Close Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 40:
                    {
                        if (m_devAutoTeaching.IsReady())
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Auto Teaching Unit Ready OK"));
                            seqNo = 10;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 30000)
                        {
                            AlarmId = m_devAutoTeaching.ALM_SettingError.ID;
                            SequenceLog.WriteLog(FuncName, string.Format("Auto Teaching Not Ready Alarm, Check Setting! - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 100:
                    {
                        // Sensor Find 
                        //  1. Sensor ON일때
                        //    i) (-) 방향으로 이동하면서 OFF Find  
                        //  2. Sensor OFF일때 
                        //    i) (+) 방향으로 이동하면서 ON Find ... 
                        //       1) OK :  1.Sensor ON일때 부터 시작
                        //       2) NG : (-) 방향으로 2배 이동하면서 ON Find ...
                        //          OK : 1.Sensor ON일때 부터 시작
                        //          NG : Alarm 
                        // 주행축을 -방향으로 이동하면서 기준 위치를 찾자 !
                        if (sensor_detected)
                        {
                            m_TargetPosition = m_devTransfer.AxisMaster.GetDevAxis().GetCurPosition() - searchDistance;
                            ushort propId = m_devTransfer.TeachingVelocitySearch.PropId;
                            m_SearchingVelSet = m_devTransfer.AxisMaster.GetDevAxis().GetTeachingVel(propId);
                            m_MoveComp1 = false;

                            seqNo = 110;
                        }
                        else
                        {
                            m_TargetPosition = m_devTransfer.AxisMaster.GetDevAxis().GetCurPosition() + searchDistance;
                            ushort propId = m_devTransfer.TeachingVelocitySearch.PropId;
                            m_SearchingVelSet = m_devTransfer.AxisMaster.GetDevAxis().GetTeachingVel(propId);
                            m_MoveComp1 = false;
                            m_LastSensorFind = false;

                            seqNo = 120;
                        }
                    }
                    break;

                case 110:
                    {
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devTransfer.Move(m_TargetPosition, m_SearchingVelSet, true);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (sensor_detected == false) // Sensor OFF Find
                        {
                            m_MoveComp1 = false;
                            m_SetLeftBcr = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                            m_SetRightBcr = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;

                            m_devTransfer.SeqAbort();
                            SequenceLog.WriteLog(FuncName, string.Format("Wheel Master Sensor Search Move Finished, Sensor OFF Find OK"));
                            seqNo = 150;
                        }
                        else if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Wheel Master Sensor Search Move Finished, Sensor OFF Find NG"));
                            seqNo = 500;//일단 500으로 가서 끝내자 ~~
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Wheel Master Sensor Search Move NG"));
                            m_devTransfer.SeqAbort();

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 100;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 120:
                    {
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devTransfer.Move(m_TargetPosition, m_SearchingVelSet, true);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (sensor_detected) // Sensor ON Find
                        {
                            m_MoveComp1 = false;
                            m_devTransfer.SeqAbort();
                            SequenceLog.WriteLog(FuncName, string.Format("Wheel Master Sensor Search Move Finished, Sensor ON Find OK"));
                            seqNo = 140;
                        }
                        else if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            if (m_LastSensorFind)
                            {
                                m_LastSensorFind = false;
                                SequenceLog.WriteLog(FuncName, string.Format("Wheel Master Sensor Search Move Finished, Sensor ON Find NG"));
                                seqNo = 500;//일단 500으로 가서 끝내자 ~~
                            }
                            else
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Wheel Master Sensor Search Move Finished, Sensor ON Find NG"));
                                seqNo = 130;
                            }
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Wheel Master Sensor Search Move NG"));
                            m_devTransfer.SeqAbort();

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 100;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 130:
                    {
                        m_TargetPosition = m_devTransfer.AxisMaster.GetDevAxis().GetCurPosition() - 2 * searchDistance;
                        ushort propId = m_devTransfer.TeachingVelocitySearch.PropId;
                        m_SearchingVelSet = m_devTransfer.AxisMaster.GetDevAxis().GetTeachingVel(propId);
                        m_MoveComp1 = false;
                        m_LastSensorFind = true;

                        seqNo = 120;
                    }
                    break;

                case 140:
                    {
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devTransfer.AxisMaster.GetDevAxis().Stop();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Wheel Master Sensor Search Stop OK"));
                            seqNo = 100;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Wheel Master Sensor Search Stop Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            m_devTransfer.SeqAbort();
                            AlarmId = rv1;
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;

                case 150:
                    {
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devTransfer.AxisMaster.GetDevAxis().Stop();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            double curLeftBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                            double curRightBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                            double curHoist = DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();
                            DataItem_Port port = DatabaseHandler.Instance.DictionaryPortDataList[target_port];
                            bool teaching_station = curPortType == PortType.TeachingStation;
                            teaching_station |= curPortType == PortType.LeftTeachingStation;
                            teaching_station |= curPortType == PortType.RightTeachingStation;
                            if (teaching_station || m_devAutoTeaching.OnlySensorOffsetFind)
                            {
                                double leftOffset = port.BarcodeLeft - m_SetLeftBcr;
                                double rightOffset = port.BarcodeRight - m_SetRightBcr;
                                double y_offset = (leftOffset + rightOffset) / 2.0f;
                                double z_offset = 0.0f;
                                if (isEQ_PORT) SetupManager.Instance.SetupVision.SensorOffsetDistancePORT = y_offset;
                                else if (isLeftOHB) SetupManager.Instance.SetupVision.SensorOffsetDistanceLOHB = y_offset;
                                else SetupManager.Instance.SetupVision.SensorOffsetDistanceROHB = y_offset;
                                if (isEQ_PORT)
                                {
                                    double dz = m_devAutoTeaching.GetHeight();
                                    z_offset = -1 * (port.HoistPosition + dz);
                                    SetupManager.Instance.SetupVision.AutoTeachingHoistMoveOffsetPORT = z_offset;
                                }
                                SetupManager.Instance.SaveSetupVision();
                                SequenceLog.WriteLog(FuncName, string.Format("Sensor Offset Fink OK :[Wheel={0},Hoist={1}]", y_offset, z_offset));
                            }

                            double moving_offset = ((port.BarcodeLeft - curLeftBCR) + (port.BarcodeRight - curRightBCR)) / 2.0f;
                            m_TransferMovingOffset = moving_offset;
                            m_SlideWaitPosition = true;
                            seqNo = 200;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Wheel Master Sensor Search Stop Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            m_devTransfer.SeqAbort();
                            AlarmId = rv1;
                            rv = AlarmId;
                            seqNo = 0;
                        }
                    }
                    break;

                case 200:
                    {
                        //BackLash 때문에 넣은거 같은데.. Back Lash 없는 설비는 문제가 됨..
                        //if (m_TransferMovingOffset > 3.0f && !m_devTransfer.IsLastMovePlusDirection()) m_TransferMovingOffset += 3.0f;
                        //else if (m_TransferMovingOffset < -3.0f && m_devTransfer.IsLastMovePlusDirection()) m_TransferMovingOffset -= 3.0f;

                        m_TargetPosition = m_devTransfer.AxisMaster.GetDevAxis().GetCurPosition() + m_TransferMovingOffset;
                        ushort propId = m_devTransfer.TeachingVelocitySearch.PropId;
                        m_SearchingVelSet = m_devTransfer.AxisMaster.GetDevAxis().GetTeachingVel(propId);
                        SequenceLog.WriteLog(FuncName, string.Format("Wheel Master Move Start - Vision Measure Position :[{0}], TransferMovingOffset : [{1}]", m_TargetPosition.ToString(), m_TransferMovingOffset.ToString()));

                        m_MoveComp1 = false;
                        seqNo = 210;
                    }
                    break;

                case 210:
                    {
                        bool safty = false;
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devTransfer.Move(m_TargetPosition, m_SearchingVelSet, safty);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Wheel Master Sensor Offset Distance Move OK"));
                            seqNo = 220;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Wheel Master Sensor Offset Distance Move NG"));
                            m_devTransfer.SeqAbort();

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 220:
                    {
                        // Calculate Position
                        m_WaitPosition = m_devFoupGripper.GetTeachingPosition(m_devFoupGripper.TeachingPointWait.PosId);
                        m_Target1Position = m_devFoupGripper.GetBeforeDownPos(target_port, carrier_installed); // Teaching Position, Hoist는 없으니까. Z 값은 의미 없음.
                        m_Target1VelSets = m_devFoupGripper.GetTeachingVelSets(m_devFoupGripper.TeachingVelocityMid.PropId);
                        m_TargetSlowVelSets = m_devFoupGripper.GetTeachingVelSets(m_devFoupGripper.TeachingVelocityLow.PropId); // 저속으로 이동하면서 Sensor Detect 시점을 찾자 !
                        if (m_WaitPosition != null && m_Target1Position != null && m_Target1VelSets != null && m_TargetSlowVelSets != null)
                        {
                            ///////////////////////////////////////////////////////////////
                            string msg = "Velocity:[";
                            foreach (VelSet set in m_Target1VelSets) msg += string.Format("[{0},{1},{2},{3}],", set.Vel, set.Acc, set.Dec, set.Jerk);
                            foreach (VelSet set in m_TargetSlowVelSets) msg += string.Format("[{0},{1},{2},{3}],", set.Vel, set.Acc, set.Dec, set.Jerk);
                            msg += "]";
                            ///////////////////////////////////////////////////////////////
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit Move Start - Vision Measure Position :[{0}], Velocity:[{1}]", m_Target1Position.ToString(), msg));
                            m_MoveComp1 = false;
                            seqNo = 230;
                        }
                        else
                        {
                            if (m_Target1VelSets == null || m_TargetSlowVelSets == null)
                            {
                                AlarmId = m_devFoupGripper.ALM_SettingError.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("DevFoupGripper TeachingVelocity isn't TeachingVelocityMid Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else
                            {
                                AlarmId = ProcessDataHandler.Instance.ALM_SourcePortMismatchError.ID;
                                SequenceLog.WriteLog(FuncName, string.Format("Teaching Port Database isn't Port ID = [{0}] Alarm - {1}", target_port, EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 230:
                    {
                        // Slide & Rotate Port Teaching Position Move
                        if (!m_MoveComp1)
                        {
                            if (m_SlideWaitPosition)
                                rv1 = m_devFoupGripper.Move(enAxisMask.aY | enAxisMask.aT, m_Target1Position, m_Target1VelSets);
                            else rv1 = m_devFoupGripper.Move(enAxisMask.aY | enAxisMask.aT, m_Target1Position, m_TargetSlowVelSets);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            m_SlideWaitPosition = false;
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit Move OK - Vision Measure Position :[{0}]", m_Target1Position.ToString()));

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 300;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit Move Alarm - Vision Measure Position :[{0}]", m_Target1Position.ToString()));
                            m_devFoupGripper.SeqAbort();

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo; 
                            seqNo = 1000;
                        }
                    }
                    break;

                case 300:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;
                        seqNo = 310;
                    }
                    break;

                case 310:
                    {
                        rv1 = m_devAutoTeaching.VisionFind(m_VisionFindDevice);
                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Vision Measure OK :[dx={0},dy={1},dt={2}]", m_devAutoTeaching.AlignDiffX, m_devAutoTeaching.AlignDiffY, m_devAutoTeaching.AlignDiffT));

                            if (!isEQ_PORT && m_devAutoTeaching.OnlySensorOffsetFind)
                            {
                                DataItem_Port port = DatabaseHandler.Instance.DictionaryPortDataList[target_port];

                                double dz = m_devAutoTeaching.GetHeight();
                                double z_offset = port.HoistPosition;// - 1 * (port.HoistPosition + dz);
                                if (isLeftOHB) SetupManager.Instance.SetupVision.AutoTeachingHoistMoveOffsetLOHB = z_offset;
                                else SetupManager.Instance.SetupVision.AutoTeachingHoistMoveOffsetROHB = z_offset;
                                SetupManager.Instance.SaveSetupVision();
                                SequenceLog.WriteLog(FuncName, string.Format("Sensor Offset Fink OK :[Hoist={0}]", z_offset));
                            }

                            seqNo = 320;
                        }
                        else if (rv1 > 0)
                        {
                            m_devAutoTeaching.SeqAbort();
                            SequenceLog.WriteLog(FuncName, string.Format("Vision Measure Alarm - {0}", rv1));

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 320:
                    {
                        DataItem_Port port = DatabaseHandler.Instance.DictionaryPortDataList[target_port];

                        double curLeftBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                        double curRightBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                        double curHoist = DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis().GetCurPosition();

                        double dx = m_devAutoTeaching.AlignDiffX;
                        double dy = m_devAutoTeaching.AlignDiffY;
                        double dt = m_devAutoTeaching.AlignDiffT;
                        double dz = m_devAutoTeaching.GetHeight();
                        // Error Calculate
                        double wheel_error = -0.5f * dy;
                        double slide_error = dx * SetupManager.Instance.SetupVision.AutoTeachingSlideMoveRatio;
                        double rotate_error = -1 * dt * SetupManager.Instance.SetupVision.AutoTeachingRotateMoveRatio;
                        double hoist_error = port.HoistPosition;
                        if (isEQ_PORT) hoist_error = -1 * (dz + SetupManager.Instance.SetupVision.AutoTeachingHoistMoveOffsetPORT);
                        else if (isLeftOHB) hoist_error = SetupManager.Instance.SetupVision.AutoTeachingHoistMoveOffsetLOHB; // - 1 * (dz + SetupManager.Instance.SetupVision.AutoTeachingHoistMoveOffsetLOHB);
                        else hoist_error = SetupManager.Instance.SetupVision.AutoTeachingHoistMoveOffsetROHB; // - 1 * (dz + SetupManager.Instance.SetupVision.AutoTeachingHoistMoveOffsetROHB);

                        SequenceLog.WriteLog(FuncName, string.Format("Vision Find Error, wheel_error={0}, slide_error={1}, rotate_error={2}, hoist_error={3}", wheel_error, slide_error, rotate_error, hoist_error));

                        port.BarcodeLeft = curLeftBCR + wheel_error;
                        port.BarcodeRight = curRightBCR + wheel_error;
                        port.SlidePosition += slide_error;
                        port.RotatePosition += rotate_error;
                        port.HoistPosition = curHoist + hoist_error;
                        port.UnloadSlidePosition += slide_error;
                        port.UnloadRotatePosition += rotate_error;
                        port.UnloadHoistPosition = curHoist + hoist_error;

                        bool teaching_station = curPortType == PortType.TeachingStation;
                        teaching_station |= curPortType == PortType.LeftTeachingStation;
                        teaching_station |= curPortType == PortType.RightTeachingStation;
                        if (teaching_station == false && m_devAutoTeaching.OnlySensorOffsetFind == false)
                        {
                            DatabaseHandler.Instance.UpdatePort(port);
                        }

                        // Spec In Check
                        bool specIn = true;
                        specIn &= Math.Abs(dx) < SetupManager.Instance.SetupVision.VisionSpecX;
                        specIn &= Math.Abs(dy) < SetupManager.Instance.SetupVision.VisionSpecY;
                        specIn &= Math.Abs(dt) < SetupManager.Instance.SetupVision.VisionSpecT;
                        if (specIn || teaching_station)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Vision Find Spec OK"));
                            m_AutoTeachingComplete = true;
                            seqNo = 330;
                        }
                        else
                        {
                            m_TransferMovingOffset = wheel_error;
                            SequenceLog.WriteLog(FuncName, string.Format("Vision Find Spec Out, Wheel Error:{0}", wheel_error));
                            seqNo = 200;
                        }
                    }
                    break;

                case 330:
                    {
                        // Slide & Rotate Wait Teaching Position Move
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devFoupGripper.Move(enAxisMask.aZ | enAxisMask.aY | enAxisMask.aT, m_WaitPosition, m_Target1VelSets);
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (m_MoveComp1)
                        {
                            m_MoveComp1 = false;
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1} | {2}) Wait Move OK", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            seqNo = 500;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Foup Gripper Unit ({0} | {1} | {2}) Wait Move NG", m_devFoupGripper.AxisHoist.AxisName, m_devFoupGripper.AxisSlide.AxisName, m_devFoupGripper.AxisTurn.AxisName));
                            m_devFoupGripper.SeqAbort();

                            AlarmId = rv1;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = seqNo;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 500:
                    {
                        if (m_AutoTeachingComplete)
                        {
                            OCSCommManager.Instance.OcsStatus.AutoTeachingResult = VehicleOperationResult.Success;
                            SequenceLog.WriteLog(FuncName, "Auto Teaching Complete");
                        }
                        else
                        {
                            OCSCommManager.Instance.OcsStatus.AutoTeachingResult = VehicleOperationResult.Fail;
                            SequenceLog.WriteLog(FuncName, "Auto Teaching Fail");
                        }
                        OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.AutoTeachingCompleted);

                        //m_devAutoTeaching.AutoTeachingFinished();
                        EventHandlerManager.Instance.InvokeAutoTeachingMonitorShow(false);
                        this.scm.Start = false;
                        this.scm.End = true;
                        if (m_AutoTeachingComplete) rv = 0;
                        else rv = m_ALM_AutoTeachingFailError.ID;
                        seqNo = 0;
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
            return rv;
        }
        #endregion
        #region Device Motor ReadyCheck
        private int CheckDeviceMotor()
        {
            AlarmId = 0;
            if (m_devFoupGripper.IsValid)
            {
                if (m_devFoupGripper.AxisHoist.IsValid) if (AlarmId == 0) AlarmId = m_devFoupGripper.AxisHoist.GetDevAxis().IsAxisReady() == false ? m_devFoupGripper.AxisHoist.GetDevAxis().ALM_NotReadyError.ID : 0;
                if (m_devFoupGripper.AxisSlide.IsValid) if (AlarmId == 0) AlarmId = m_devFoupGripper.AxisSlide.GetDevAxis().IsAxisReady() == false ? m_devFoupGripper.AxisSlide.GetDevAxis().ALM_NotReadyError.ID : 0;
                if (m_devFoupGripper.AxisTurn.IsValid) if (AlarmId == 0) AlarmId = m_devFoupGripper.AxisTurn.GetDevAxis().IsAxisReady() == false ? m_devFoupGripper.AxisTurn.GetDevAxis().ALM_NotReadyError.ID : 0;
            }
            if (m_devGripperPio.IsValid)
            {
                if (AlarmId == 0) AlarmId = m_devGripperPio.PioComm.IsGo() == false ? m_devGripperPio.PioComm.ALM_PioGoResponseError.ID : 0;
            }
            if (m_devAntiDropFront.IsValid)
                if (AlarmId == 0) AlarmId = m_devAntiDropFront.IsAlarm() ? m_devAntiDropFront.ALM_AntiDropAlarm.ID : 0;
            if (m_devAntiDropRear.IsValid)
                if (AlarmId == 0) AlarmId = m_devAntiDropRear.IsAlarm() ? m_devAntiDropRear.ALM_AntiDropAlarm.ID : 0;
            return AlarmId;
        }
        #endregion
        #region Methods - sensor, position function
        private bool IsFoupNotExist()
        {
            bool foup_exist = true;
            try
            {
                if (m_devGripperPio.IsValid)
                {
                    foup_exist &= m_devGripperPio.IsProductNotExist();
                    foup_exist &= m_devGripperPio.DiGripperOpen.IsDetected;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return foup_exist;
        }
        private bool IsDoubleStorage()
        {
            bool foup_exist = false; //default true해서 buffer일때만 확인하겠지...
            try
            {
                if (ProcessDataHandler.Instance.CurTransferCommand.IsValid)
                {
                    if (m_devFoupGripper.IsValid)
                    {
                        if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftBuffer)
                            foup_exist |= m_devFoupGripper.IsLeftDoubleStorage();
                        else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightBuffer)
                            foup_exist |= m_devFoupGripper.IsRightDoubleStorage();
                        else foup_exist = true;

                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return foup_exist;
        }
        private bool IsAntiDropUnlock()
        {
            bool anti_drop_unlock = true;
            try
            {
                anti_drop_unlock &= m_devAntiDropFront.IsValid ? m_devAntiDropFront.GetUnlock() : true;
                anti_drop_unlock &= m_devAntiDropRear.IsValid ? m_devAntiDropRear.GetUnlock() : true;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return anti_drop_unlock;
        }
        private bool IsGripOpenEnableCondition()
        {
            bool enable = true;
            try
            {
                if (m_devGripperPio.IsValid)
                {
                    enable &= !m_devGripperPio.IsHoistLimit();
                    enable &= m_devGripperPio.IsHoistUp();
                    enable &= m_devGripperPio.IsHoistHome();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return enable;
        }
        #endregion
    }

    public class SeqCarrierIdReading : XSeqFunc
    {
        public static readonly string FuncName = "[SeqCarrierIdReading]";
        XTimer m_Timer = new XTimer("SeqCarrierIdReading");

        #region Fields
        private DevGripperPIO m_devGripperPio = null;
        private DevRFID m_devRFID = null;
        private AlarmData m_ALM_CarrierIdMismatchAlarm = null;
        private AlarmData m_ALM_CarrierIdReadingAlarm = null;
        #endregion

        #region Constructor
        public SeqCarrierIdReading()
        {
            this.SeqName = $"SeqCarrierIdReading";

            m_devRFID = DevicesManager.Instance.DevRFID;

            AddCaseMemo();
            this.TaskLayer = (int)enTaskLayer.layerSemiAuto;

            m_ALM_CarrierIdMismatchAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, false, "SeqCarrierIdReading", "RFID Sequence", "Carrier ID Mismatch Alarm");
            m_ALM_CarrierIdReadingAlarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, false, "SeqCarrierIdReading", "RFID Sequence", "Carrier ID Reading Alarm");
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
                if (m_devRFID.IsValid) m_devRFID.SeqAbort();
                this.scm.Abort = true;
            }
            this.InitSeq();
        }
        private void AddCaseMemo()
        {
            try
            {
                this.SeqCaseMemoLists.Add(0, string.Format("Carrier Mapping Sequence Start !"));
                this.SeqCaseMemoLists.Add(1000, string.Format("Alarm Reset Wait"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("{0} EXCEPTION : {1}", FuncName, ex.ToString()));
            }
        }
        #endregion

        #region Override
        public override int Do()
        {
            if (this.scm.Pause) return -1;
            int rv = -1;

            TransferCommand curCommand = ProcessDataHandler.Instance.CurTransferCommand;
            OCSStatus ocsStatus = OCSCommManager.Instance.OcsStatus;

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        this.scm.Start = true;
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 10;
                    }
                    break;

                case 10:
                    {
                        bool reading = m_devRFID.ReadStart();
                        if (reading)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Carrier ID Reading Start !"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        else
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Carrier ID Reader Not Valid !"));
                            seqNo = 900;
                        }
                    }
                    break;

                case 20:
                    {
                        if (m_devRFID.RFIDReading)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("RF ID Reading - {0}/{1}", curCommand.CassetteID, m_devRFID.RFIDTag));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                        else if (m_devRFID.RFIDConnectedNg)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("RF ID Connected Alarm  - {0}/{1}", curCommand.CassetteID, m_devRFID.RFIDTag));
                            m_devRFID.RFIDTag = string.Empty;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 1000)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("RF ID Reading Not Response - {0}/{1}", curCommand.CassetteID, m_devRFID.RFIDTag));
                            m_devRFID.RFIDTag = string.Empty;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 30;
                        }
                    }
                    break;

                case 30:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devRFID.RFIDTag = curCommand.CassetteID;
                            m_devRFID.RFIDReadingNg = false;
                            m_devRFID.RFIDConnectedNg = false;
                            m_devRFID.RFIDReadCompleted = true;
                        }

                        bool rfid_reading_finished = XFunc.GetTickCount() - StartTicks > 5000;
                        rfid_reading_finished |= m_devRFID.RFIDReadCompleted;
                        rfid_reading_finished |= m_devRFID.RFIDReadingNg;
                        rfid_reading_finished |= m_devRFID.RFIDConnectedNg;
                        if (rfid_reading_finished)
                        {
                            ocsStatus.CarrierIDScanRFIDTag = m_devRFID.RFIDTag;
                            if (m_devRFID.RFIDTag != string.Empty)
                            {
                                ocsStatus.CarrierIDScanResult = IDReadResult.Success;
                                SequenceLog.WriteLog(FuncName, string.Format("Carrier ID Reading Complete - {0}", m_devRFID.RFIDTag));
                            }
                            else if (m_devRFID.RFIDConnectedNg || m_devRFID.RFIDReadingNg)
                            {
                                ocsStatus.CarrierIDScanResult = IDReadResult.ReadFail;
                                if (m_devRFID.RFIDConnectedNg)
                                    SequenceLog.WriteLog(FuncName, string.Format("Carrier ID Connection Fail - {0}", m_devRFID.RFIDTag));
                                else SequenceLog.WriteLog(FuncName, string.Format("Carrier ID Reading Fail - {0}", m_devRFID.RFIDTag));
                            }
                            else
                            {
                                ocsStatus.CarrierIDScanResult = IDReadResult.Empty;
                                SequenceLog.WriteLog(FuncName, string.Format("Carrier ID Reading Empty - {0}", m_devRFID.RFIDTag));
                            }
                            ocsStatus.SetFlag(InterfaceFlag.CarrierIdReadingComp, FlagValue.ON);
                            m_devRFID.ReadStop();

                            if (curCommand.IsCarrierIdScan)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("Carrier ID Scan OK {0}", ocsStatus.CarrierIDScanRFIDTag));
                                seqNo = 900;
                            }
                            else
                            {
                                bool mismatch = ocsStatus.CarrierIDScanResult == IDReadResult.Success;
                                mismatch &= curCommand.CassetteID != ocsStatus.CarrierIDScanRFIDTag;
                                if (mismatch)
                                {
                                    //OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.CarrierIDMismatch);
                                    SequenceLog.WriteLog(FuncName, string.Format("Carrier ID Mismatch {0},{1}", curCommand.CassetteID, ocsStatus.CarrierIDScanRFIDTag));
                                    seqNo = 900;
                                }
                                else if (curCommand.CassetteID == ocsStatus.CarrierIDScanRFIDTag)
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("Carrier ID Match OK {0},{1}", curCommand.CassetteID, ocsStatus.CarrierIDScanRFIDTag));
                                    seqNo = 900;
                                }
                                else
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("Carrier ID Match NG {0},{1}", curCommand.CassetteID, ocsStatus.CarrierIDScanRFIDTag));
                                    seqNo = 900;
                                }
                            }
                        }
                    }
                    break;

                case 900:
                    {
                        this.scm.Start = false;
                        this.scm.End = true;
                        rv = 0;
                        seqNo = 0;
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
            return rv;
        }
        #endregion
    }

    public class SeqCycleTest : XSeqFunc
    {
        public static readonly string FuncName = "[SeqCycleTest]";

        #region Fields
        private SeqAcquire m_Acquire = new SeqAcquire();
        private SeqDeposit m_Deposit = new SeqDeposit();
        private DevTransfer m_devTransfer = new DevTransfer();
        private DevSteer m_devSteer = new DevSteer();
        private DevAntiDrop m_devAntiDropFront = new DevAntiDrop();
        private DevAntiDrop m_devAntiDropRear = new DevAntiDrop();
        private int m_HoistRepeatCount = 0;
        private int m_SteerRepeatCount = 0;
        private int m_AntiDropRepeatCount = 0;
        private int m_WheelMoveRepeatCount = 0;

        private List<Data.Process.Path> m_RunPathMaps = new List<Data.Process.Path>();
        private double m_ReturnPosition = 0.0f;
        private VelSet m_ReturnVelSet = new VelSet();
        private double m_ReturnDistance = 0.0f;


        private bool m_MoveComp1 = false;
        private bool m_MoveComp2 = false;
        #endregion

        #region Constructor
        public SeqCycleTest()
        {
            this.SeqName = $"SeqCycleTest";

            m_devSteer = DevicesManager.Instance.DevSteer;
            m_devAntiDropFront = DevicesManager.Instance.DevFrontAntiDrop;
            m_devAntiDropRear = DevicesManager.Instance.DevRearAntiDrop;
            m_devTransfer = DevicesManager.Instance.DevTransfer;
        }
        #endregion

        #region Override
        public override void SeqAbort()
        {
            if (AlarmId > 0)
            {
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }

            m_Acquire.SeqAbort();
            m_Deposit.SeqAbort();
            m_devTransfer.SeqAbort();
            m_devSteer.SeqAbort();
            m_devAntiDropFront.SeqAbort();
            m_devAntiDropRear.SeqAbort();
            base.SeqAbort();
        }
        public override void InitSeq()
        {
            m_Acquire.InitSeq();
            m_Deposit.InitSeq();
            base.InitSeq();
        }
        #endregion

        #region Sequence
        public override int Do()
        {
            int rv = -1;
            int rv1 = -1;
            int rv2 = -1;
            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        TransferCommand cmd = ProcessDataHandler.Instance.CurTransferCommand;
                        if (cmd.ProcessCommand == IF.OCS.OCSCommand.CycleHoistAging && SetupManager.Instance.SetupHoist.FoupGripperUse == Use.Use) seqNo = 100;
                        else if (cmd.ProcessCommand == IF.OCS.OCSCommand.CycleSteerAging) seqNo = 200;
                        else if (cmd.ProcessCommand == IF.OCS.OCSCommand.CycleAntiDropAging && SetupManager.Instance.SetupOperation.AntiDropUse == Use.Use) seqNo = 300;
                        else if (cmd.ProcessCommand == IF.OCS.OCSCommand.CycleWheelMoveAging) seqNo = 400;
                        else
                        {
                            rv = 0;
                        }
                    }
                    break;

                case 100:
                    {
                        TransferCommand cmd = ProcessDataHandler.Instance.CurTransferCommand;
                        int sourceID = cmd.SourceID;
                        int destinationID = cmd.DestinationID;
                        bool normal = true;
                        normal &= (sourceID == destinationID);
                        normal &= DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(sourceID);
                        normal &= DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(destinationID);
                        if (normal)
                        {
                            m_Acquire.SeqAbort();
                            m_Deposit.SeqAbort();
                            m_HoistRepeatCount = 0;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        else
                        {
                            ProcessDataHandler.Instance.CurTransferCommand.IsValid = false;
                            string message = string.Format("Abnormal Invalid Command {0}!", cmd.CommandID);
                            EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.InterlockMessage, message);
                            rv = 0;
                            seqNo = 0;
                        }
                    }
                    break;

                case 110:
                    {
                        if (EqpStateManager.Instance.RunMode == EqpRunMode.Stop) break;
                        if (XFunc.GetTickCount() - StartTicks < GV.HoistCycleWaitTime * 1000) break;
                        if (m_HoistRepeatCount < GV.HoistCycleTotalCount)
                        {
                            StartTicks = XFunc.GetTickCount();
                            if (ProcessDataHandler.Instance.CurVehicleStatus.CarrierStatus == IF.OCS.CarrierState.Installed)
                                seqNo = 130;
                            else seqNo = 120;
                        }
                        else
                        {
                            rv = 0;
                            seqNo = 0;
                        }
                    }
                    break;

                case 120:
                    {
                        rv1 = m_Acquire.Do();
                        if (rv1 == 0)
                        {
                            UInt32 action_time = XFunc.GetTickCount() - StartTicks;
                            GV.AcquireCycleTime = (int)action_time;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        else if (rv1 > 0)
                        {
                            m_Acquire.SeqAbort();
                            rv = rv1;
                            seqNo = 0;
                        }
                    }
                    break;

                case 130:
                    {
                        rv1 = m_Deposit.Do();
                        if (rv1 == 0)
                        {
                            UInt32 action_time = XFunc.GetTickCount() - StartTicks;
                            GV.DepositCycleTime = (int)action_time;
                            m_HoistRepeatCount++;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        else if (rv1 > 0)
                        {
                            m_Deposit.SeqAbort();
                            rv = rv1;
                            seqNo = 0;
                        }
                    }
                    break;

                case 200:
                    {
                        m_SteerRepeatCount = 0;
                        m_devSteer.SeqAbort();
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 210;
                    }
                    break;

                case 210:
                    {
                        if (EqpStateManager.Instance.RunMode == EqpRunMode.Stop) break;
                        if (XFunc.GetTickCount() - StartTicks < GV.SteerCycleWaitTime * 1000) break;
                        if (m_SteerRepeatCount < GV.SteerCycleTotalCount)
                        {
                            m_MoveComp1 = false;
                            m_MoveComp2 = false;

                            StartTicks = XFunc.GetTickCount();
                            if (m_devSteer.GetFrontSteerDirection() == enSteerDirection.Left ||
                                m_devSteer.GetRearSteerDirection() == enSteerDirection.Left)
                                seqNo = 230;
                            else seqNo = 220;
                        }
                        else
                        {
                            rv = 0;
                            seqNo = 0;
                        }
                    }
                    break;

                case 220:
                    {
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devSteer.FrontLeft();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (!m_MoveComp2)
                        {
                            rv2 = m_devSteer.RearLeft();
                            if (rv2 == 0) m_MoveComp2 = true;
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            seqNo = 230;
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            if (rv1 > 0) rv = rv1;
                            else if (rv2 > 0) rv = rv2;
                            seqNo = 0;
                        }
                    }
                    break;

                case 230:
                    {
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devSteer.FrontRight();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (!m_MoveComp2)
                        {
                            rv2 = m_devSteer.RearRight();
                            if (rv2 == 0) m_MoveComp2 = true;
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            m_SteerRepeatCount++;
                            UInt32 change_time = XFunc.GetTickCount() - StartTicks;
                            GV.SteerCycleTime = (int)change_time;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 210;
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            if (rv1 > 0) rv = rv1;
                            else if (rv2 > 0) rv = rv2;
                            seqNo = 0;
                        }
                    }
                    break;

                case 300:
                    {
                        m_AntiDropRepeatCount = 0;
                        m_devAntiDropFront.SeqAbort();
                        m_devAntiDropRear.SeqAbort();
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 310;
                    }
                    break;

                case 310:
                    {
                        if (EqpStateManager.Instance.RunMode == EqpRunMode.Stop) break;
                        if (XFunc.GetTickCount() - StartTicks < GV.AntiDropCycleWaitTime * 1000) break;
                        if (m_AntiDropRepeatCount < GV.AntiDropCycleTotalCount)
                        {
                            m_MoveComp1 = false;
                            m_MoveComp2 = false;

                            StartTicks = XFunc.GetTickCount();
                            if (m_devAntiDropFront.GetLock() || m_devAntiDropRear.GetLock())
                                seqNo = 330;
                            else seqNo = 320;
                        }
                        else
                        {
                            rv = 0;
                            seqNo = 0;
                        }
                    }
                    break;

                case 320:
                    {
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devAntiDropFront.Lock();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (!m_MoveComp2)
                        {
                            rv2 = m_devAntiDropRear.Lock();
                            if (rv2 == 0) m_MoveComp2 = true;
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            m_MoveComp1 = false;
                            m_MoveComp2 = false;
                            seqNo = 330;
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            if (rv1 > 0) rv = rv1;
                            else if (rv2 > 0) rv = rv2;
                            seqNo = 0;
                        }
                    }
                    break;

                case 330:
                    {
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devAntiDropFront.Unlock();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (!m_MoveComp2)
                        {
                            rv2 = m_devAntiDropRear.Unlock();
                            if (rv2 == 0) m_MoveComp2 = true;
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            m_AntiDropRepeatCount++;
                            UInt32 change_time = XFunc.GetTickCount() - StartTicks;
                            GV.AntiDropCycleTime = (int)change_time;
                            StartTicks = XFunc.GetTickCount();

                            seqNo = 310;
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            if (rv1 > 0) rv = rv1;
                            else if (rv2 > 0) rv = rv2;
                            seqNo = 0;
                        }
                    }
                    break;

                case 400:
                    {
                        TransferCommand cmd = ProcessDataHandler.Instance.CurTransferCommand;
                        bool normal = true;
                        normal &= cmd.TypeOfDestination == enGoCommandType.ByDistance;
                        normal &= cmd.TargetNodeToDistance > 0;
                        normal &= cmd.TargetNodeToDistance < ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Distance;
                        normal &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type == LinkType.Straight;
                        if (normal)
                        {
                            m_RunPathMaps.Clear();

                            // Command Make Target
                            double curLeftBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                            double curRightBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                            Data.Process.Path path = ObjectCopier.Clone(ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath);
                            path.LeftBCRStart = curLeftBCR;
                            path.RightBCRStart = curRightBCR;
                            path.LeftBCRTarget = path.LeftBCRBegin + cmd.TargetNodeToDistance;
                            path.RightBCRTarget = path.RightBCRBegin + cmd.TargetNodeToDistance;
                            if (path.BcrDirection == enBcrCheckDirection.Both) path.BcrDirection = enBcrCheckDirection.Left;
                            path.RunDistance = path.BcrDirection == enBcrCheckDirection.Left ? path.LeftBCRTarget - curLeftBCR : path.RightBCRTarget - curRightBCR;
                            path.BcrScanUse = true;
                            if (path.RunDistance > 0 && path.RunDistance < path.Distance)
                            {
                                m_RunPathMaps.Add(path);

                                // Current Position Set
                                //m_ReturnPosition = m_devTransfer.AxisMaster.GetDevAxis().GetCurPosition(); // Motor Encoder 기준으로 돌아가니까 앞으로 갔을 때와 뒤로 갔을 때의 실제 움직인 거리가 달라서 점점 Shift됨..
                                //도착한 Motor Encoder에서 움직인 Distance만큼 뒤로가자..
                                m_ReturnDistance = path.BcrDirection == enBcrCheckDirection.Left ? path.LeftBCRTarget - path.LeftBCRStart : path.RightBCRTarget - path.RightBCRStart;
                                m_ReturnVelSet = new VelSet();
                                m_ReturnVelSet.Vel = path.Velocity;
                                m_ReturnVelSet.Acc = path.Acceleration;
                                m_ReturnVelSet.Dec = path.Deceleration;
                                m_ReturnVelSet.Jerk = path.Jerk;

                                m_devTransfer.SeqAbort();
                                m_WheelMoveRepeatCount = 0;
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 410;
                            }
                            else
                            {
                                ProcessDataHandler.Instance.CurTransferCommand.IsValid = false;
                                string message = string.Format("Abnormal Invalid Command {0}!", cmd.CommandID);
                                EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.InterlockMessage, message);
                                rv = 0;
                                seqNo = 0;
                            }
                        }
                        else
                        {
                            ProcessDataHandler.Instance.CurTransferCommand.IsValid = false;
                            string message = string.Format("Abnormal Invalid Command {0}!", cmd.CommandID);
                            EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.InterlockMessage, message);
                            rv = 0;
                            seqNo = 0;
                        }
                    }
                    break;

                case 410:
                    {
                        if (EqpStateManager.Instance.RunMode == EqpRunMode.Stop) break;
                        if (XFunc.GetTickCount() - StartTicks < GV.WheelMoveCycleWaitTime * 1000) break;
                        if (m_WheelMoveRepeatCount < GV.WheelMoveCycleTotalCount)
                        {
                            // 동일 Target BCR를 유지하기 위해 RunDistance를 갱신하자 !
                            Data.Process.Path path = ObjectCopier.Clone(ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath);
                            double curLeftBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                            double curRightBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                            path.RunDistance = path.BcrDirection == enBcrCheckDirection.Left ? path.LeftBCRTarget - curLeftBCR : path.RightBCRTarget - curRightBCR;

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 420;
                        }
                        else
                        {
                            rv = 0;
                            seqNo = 0;
                        }
                    }
                    break;

                case 420:
                    {
                        // 위치 계산하고 Motion 명령을 내리자 !
                        rv1 = m_devTransfer.SetCommand(m_RunPathMaps);
                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Motion SetCommand OK"));
                            seqNo = 430;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Motion SetCommand Alarm"));
                            AlarmId = rv1;
                            rv = rv1;
                            seqNo = 0;
                        }
                    }
                    break;

                case 430:
                    {
                        rv1 = m_devTransfer.SequenceMove();
                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Motion SequenceMove Complete"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 440;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Motion SequenceMove Alarm"));
                            AlarmId = rv1;
                            rv = rv1;
                            seqNo = 0;
                        }
                    }
                    break;

                case 440:
                    {
                        if (XFunc.GetTickCount() - StartTicks < GV.WheelMoveCycleWaitTime * 1000) break;

                        m_ReturnPosition = m_devTransfer.AxisMaster.GetDevAxis().GetCurPosition() - m_ReturnDistance;
                        seqNo = 450;
                    }
                    break;

                case 450:
                    {
                        rv1 = m_devTransfer.Move(m_ReturnPosition, m_ReturnVelSet, true);
                        if (rv1 == 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Return Position Move Complete"));
                            UInt32 action_time = XFunc.GetTickCount() - StartTicks;
                            GV.WheelMoveCycleTime = (int)action_time;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 410;
                        }
                        else if (rv1 > 0)
                        {
                            SequenceLog.WriteLog(FuncName, string.Format("Return Position Move Alarm"));
                            AlarmId = rv1;
                            rv = rv1;
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
